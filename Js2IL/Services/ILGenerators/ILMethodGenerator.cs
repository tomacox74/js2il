using Acornima;
using Acornima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Generates Intermediate Language (IL) code from a JavaScript Abstract Syntax Tree (AST) for a method
    /// </summary>
    internal class ILMethodGenerator : IMethodExpressionEmitter
    {
        private readonly Variables _variables;
        private readonly BaseClassLibraryReferences _bclReferences;
        private readonly MetadataBuilder _metadataBuilder;
    private readonly InstructionEncoder _il;
    private readonly ControlFlowBuilder _cfb;
        private readonly BinaryOperators _binaryOperators;
        private readonly IMethodExpressionEmitter _expressionEmitter;
        private readonly Runtime _runtime;
        private readonly MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private readonly Dispatch.DispatchTableGenerator _dispatchTableGenerator;
        private readonly ClassRegistry _classRegistry;
        // When emitting inside a class instance method (.ctor or method), enable 'this' handling
        private readonly bool _inClassMethod;
        private readonly string? _currentClassName;
        // Tracks the name of the variable currently being initialized, to name arrow-function scopes consistently
        // with SymbolTableBuilder (e.g., ArrowFunction_<targetName>) when emitting an ArrowFunctionExpression on the RHS.
        private string? _currentAssignmentTarget;
        // Tracks simple associations from variable name to class name when initialized via `const x = new ClassName()`
        private readonly Dictionary<string, string> _variableToClass = new(StringComparer.Ordinal);
        // Loop control stack for handling continue/break targets
        private readonly Stack<LoopContext> _loopStack = new();

        private readonly struct LoopContext
        {
            public readonly LabelHandle ContinueLabel;
            public readonly LabelHandle BreakLabel;
            public LoopContext(LabelHandle @continue, LabelHandle @break)
            {
                ContinueLabel = @continue;
                BreakLabel = @break;
            }
        }
        /*
         * Temporary exposure of private members until refactoring gets cleaner
         * need to determine what the difference is between generating the main method and generating any generic method
         */
        public Variables Variables => _variables;
        public MetadataBuilder MetadataBuilder => _metadataBuilder;
        public InstructionEncoder IL => _il;

        public ILMethodGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, Dispatch.DispatchTableGenerator dispatchTableGenerator, ClassRegistry? classRegistry = null, bool inClassMethod = false, string? currentClassName = null)
        {
            _variables = variables;
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            var methodIl = new BlobBuilder();
            _cfb = new ControlFlowBuilder();
            _il = new InstructionEncoder(methodIl, _cfb);
            this._runtime = new Runtime(metadataBuilder, _il);
            _binaryOperators = new BinaryOperators(metadataBuilder, _il, variables, this, bclReferences, _runtime);

            // temporary as we set the table for further refactoring
            this._expressionEmitter = this;
            _methodBodyStreamEncoder = methodBodyStreamEncoder;
            _dispatchTableGenerator = dispatchTableGenerator ?? throw new ArgumentNullException(nameof(dispatchTableGenerator));
            _classRegistry = classRegistry ?? new ClassRegistry();
            _inClassMethod = inClassMethod;
            _currentClassName = currentClassName;
        }

        // Case-insensitive property getter helper for AST reflection
        private static System.Reflection.PropertyInfo? GetPropertyIgnoreCase(object target, string propertyName)
        {
            if (target == null || string.IsNullOrEmpty(propertyName)) return null;
            var t = target.GetType();
            return t.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
        }

        // Parse a Raw regex literal string like "/pattern/flags" into (pattern, flags)
        private static (string? pattern, string? flags) ParseRegexRaw(string raw)
        {
            if (string.IsNullOrEmpty(raw) || raw[0] != '/') return (null, null);
            int lastSlash = -1;
            bool escaped = false;
            for (int i = 1; i < raw.Length; i++)
            {
                char c = raw[i];
                if (!escaped)
                {
                    if (c == '\\') { escaped = true; continue; }
                    if (c == '/') { lastSlash = i; break; }
                }
                else
                {
                    escaped = false;
                }
            }
            if (lastSlash <= 0) return (null, null);
            var pattern = raw.Substring(1, lastSlash - 1);
            var flags = lastSlash + 1 < raw.Length ? raw.Substring(lastSlash + 1) : string.Empty;
            return (pattern, flags);
        }

        public void DeclareVariable(VariableDeclaration variableDeclaraion)
        {
            // TODO need to handle multiple
            var variableAST = variableDeclaraion.Declarations.FirstOrDefault()!;
            var variableName = (variableAST.Id as Acornima.Ast.Identifier)!.Name;

            // resolve the variable via Variables
            var variable = _variables.FindVariable(variableName) ?? throw new InvalidOperationException($"Variable '{variableName}' not found.");

            // If this is a lexical (block) scoped variable (let/const) inside a block shadowing an outer declaration,
            // the variable.ScopeName will be the block scope (Block_LxCy). We must load that scope instance local.
            bool isBlockScope = variable.ScopeName.StartsWith("Block_L", StringComparison.Ordinal);

            // now we need to generate the expession portion
            if (variableAST.Init != null)
            {
                // New approach: Store to scope field
                var scopeLocalIndex = _variables.GetScopeLocalSlot(variable.ScopeName);
                if (scopeLocalIndex.Address == -1)
                {
                    throw new InvalidOperationException($"Scope '{variable.ScopeName}' not found in local slots");
                }

                // Load scope instance first for stfld
                if (scopeLocalIndex.Location == ObjectReferenceLocation.Parameter)
                {
                    _il.LoadArgument(scopeLocalIndex.Address);
                }
                else if (scopeLocalIndex.Location == ObjectReferenceLocation.ScopeArray)
                {
                    _il.LoadArgument(0); // Load scope array parameter
                    _il.LoadConstantI4(scopeLocalIndex.Address); // Load array index
                    _il.OpCode(ILOpCode.Ldelem_ref); // Load scope from array
                }
                else
                {
                    _il.LoadLocal(scopeLocalIndex.Address);
                }

                // Evaluate initializer and store to scope field
                var prevAssignmentTarget = _currentAssignmentTarget;
                try
                {
                    // Let emitted RHS know which identifier is being initialized (for variable->class mapping)
                    _currentAssignmentTarget = variableName;
                    variable.Type = this._expressionEmitter.Emit(variableAST.Init, new TypeCoercion() { boxResult = true });

                    // If initializer is require('module'), tag the variable with its runtime intrinsic CLR type
                    if (variableAST.Init is CallExpression reqCall
                        && reqCall.Callee is Identifier rid
                        && string.Equals(rid.Name, "require", StringComparison.Ordinal)
                        && reqCall.Arguments.Count == 1)
                    {
                        string? mod = null;
                        if (reqCall.Arguments[0] is StringLiteral s) mod = s.Value;
                        else if (reqCall.Arguments[0] is Literal glit && glit.Value is string gs) mod = gs;
                        if (!string.IsNullOrEmpty(mod))
                        {
                            var rt = ResolveNodeModuleType(mod!);
                            if (rt != null)
                            {
                                var vrec = _variables.FindVariable(variableName);
                                if (vrec != null)
                                {
                                    vrec.RuntimeIntrinsicType = rt;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    _currentAssignmentTarget = prevAssignmentTarget;
                }

                // Now stack: [scope_instance] [value] - perfect for stfld
                _il.OpCode(ILOpCode.Stfld);
                _il.Token(variable.FieldHandle);
            }
        }

        private static Type? ResolveNodeModuleType(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            // Accept optional node: prefix
            if (name.StartsWith("node:", StringComparison.OrdinalIgnoreCase))
                name = name.Substring("node:".Length);
            var asm = typeof(JavaScriptRuntime.Object).Assembly;
            foreach (var t in asm.GetTypes())
            {
                if (!string.Equals(t.Namespace, "JavaScriptRuntime.Node", StringComparison.Ordinal)) continue;
                var attribs = t.GetCustomAttributes(typeof(JavaScriptRuntime.Node.NodeModuleAttribute), inherit: false);
                if (attribs.Length == 0) continue;
                var attr = (JavaScriptRuntime.Node.NodeModuleAttribute)attribs[0]!;
                if (string.Equals(attr.Name, name, StringComparison.OrdinalIgnoreCase)) return t;
            }
            return null;
        }

    public void GenerateStatementsForBody(string scopeName, bool createScopeInstance, NodeList<Statement> statements)
        {
            int? createdLocalIndex = null;
            if (createScopeInstance)
            {
                var registry = _variables.GetVariableRegistry();
                if (registry != null)
                {
                    var scopeTypeHandle = registry.GetScopeTypeHandle(scopeName);
                    if (!scopeTypeHandle.IsNil)
                    {
                        // Build .ctor member ref for the scope type
                        var ctorSigBuilder = new BlobBuilder();
                        new BlobEncoder(ctorSigBuilder)
                            .MethodSignature(isInstanceMethod: true)
                            .Parameters(0, rt => rt.Void(), p => { });
                        var ctorRef = _metadataBuilder.AddMemberReference(
                            scopeTypeHandle,
                            _metadataBuilder.GetOrAddString(".ctor"),
                            _metadataBuilder.GetOrAddBlob(ctorSigBuilder));

                        // Allocate a local slot to hold the new scope instance and construct it
                        createdLocalIndex = _variables.AllocateBlockScopeLocal(scopeName);
                        _il.OpCode(ILOpCode.Newobj);
                        _il.Token(ctorRef);
                        _il.StoreLocal(createdLocalIndex.Value);

                        // Track lexical scope so variable resolution prefers it
                        _variables.PushLexicalScope(scopeName);
                    }
                }
            }

            // Iterate through each statement in the block
            foreach (var statement in statements.Where(s => s is not FunctionDeclaration))
            {
                GenerateStatement(statement);
            }

            if (createdLocalIndex.HasValue)
            {
                _variables.PopLexicalScope(scopeName);
                // Clear the local to release the scope instance for GC
                _il.OpCode(ILOpCode.Ldnull);
                _il.StoreLocal(createdLocalIndex.Value);
            }
        }

        public void InitializeLocalFunctionVariables(IEnumerable<FunctionDeclaration> functionDeclarations)
        {
            // Iterate through each function declaration in the block
            foreach (var functionDeclaration in functionDeclarations)
            {
                InitializeLocalFunctionVariable(functionDeclaration);
            }
        }

        public void InitializeLocalFunctionVariable(FunctionDeclaration functionDeclaration)
        {
            var functionName = (functionDeclaration.Id as Acornima.Ast.Identifier)!.Name;
            var functionVariable = _variables.FindVariable(functionName) ?? throw new InvalidOperationException($"Variable '{functionName}' not found.");

            var dispatchDelegateField = _dispatchTableGenerator.GetFieldDefinitionHandle(functionName);

            // now we assign a local variable to the function delegate
            // in the general case the local feels wasteful but there is scenarons where it could be assigned a different value

            // Store using scope field
            var scopeLocalIndex = _variables.GetScopeLocalSlot(functionVariable.ScopeName);
            if (scopeLocalIndex.Address == -1)
            {
                throw new InvalidOperationException($"Scope '{functionVariable.ScopeName}' not found in local slots");
            }

            // Load scope instance, then load delegate value, then store to field
            if (scopeLocalIndex.Location == ObjectReferenceLocation.Parameter)
            {
                _il.LoadArgument(scopeLocalIndex.Address);
            }
            else if (scopeLocalIndex.Location == ObjectReferenceLocation.ScopeArray)
            {
                _il.LoadArgument(0); // Load scope array parameter
                _il.LoadConstantI4(scopeLocalIndex.Address); // Load array index
                _il.OpCode(ILOpCode.Ldelem_ref); // Load scope from array
            }
            else
            {
                _il.LoadLocal(scopeLocalIndex.Address);
            }
            _il.OpCode(ILOpCode.Ldsfld);
            _il.Token(dispatchDelegateField);
            _il.OpCode(ILOpCode.Stfld);
            _il.Token(functionVariable.FieldHandle);
        }

        public void GenerateStatement(Statement statement)
        {
            switch (statement)
            {
                case ThrowStatement throwStatement:
                    GenerateThrowStatement(throwStatement);
                    break;
                case TryStatement tryStatement:
                    GenerateTryStatement(tryStatement);
                    break;
                case ClassDeclaration:
                    // Class declarations are emitted as .NET types by ClassesGenerator;
                    // no IL is required in the method body for the declaration itself.
                    break;
                case VariableDeclaration variableDeclaration:
                    DeclareVariable(variableDeclaration);
                    break;
                case ExpressionStatement expressionStatement:
                    GenerateExpressionStatement(expressionStatement);
                    break;
                case ForStatement forStatement:
                    GenerateForStatement(forStatement);
                    break;
                case WhileStatement whileStatement:
                    GenerateWhileStatement(whileStatement);
                    break;
                case DoWhileStatement doWhileStatement:
                    GenerateDoWhileStatement(doWhileStatement);
                    break;
                case ContinueStatement:
                    if (_loopStack.Count == 0)
                        throw new InvalidOperationException("'continue' used outside of a loop");
                    {
                        var ctx = _loopStack.Peek();
                        _il.Branch(ILOpCode.Br, ctx.ContinueLabel);
                    }
                    break;
                case BreakStatement:
                    if (_loopStack.Count == 0)
                        throw new InvalidOperationException("'break' used outside of a loop");
                    {
                        var ctx = _loopStack.Peek();
                        _il.Branch(ILOpCode.Br, ctx.BreakLabel);
                    }
                    break;
                case IfStatement ifStatement:
                    GenerateIfStatement(ifStatement);
                    break;
                case BlockStatement blockStatement:
                    // Handle BlockStatement with its own lexical scope if it declared let/const
                    GenerateBlock(blockStatement);
                    break;
                case ReturnStatement returnStatement:
                    GenerateReturnStatement(returnStatement);
                    break;
                case EmptyStatement:
                    // Empty statements (like standalone semicolons) do nothing
                    break;
                default:
                    throw new NotSupportedException($"Unsupported statement type: {statement.Type}");
            }
        }

        

        /// <summary>
        /// Generates code for a BlockStatement, creating a new scope object if the block declares any let/const bindings.
        /// This enables correct shadowing behavior for block scoped variables.
        /// </summary>
        private void GenerateBlock(BlockStatement blockStatement)
        {
            // Heuristic: if the symbol table created a block scope we would have a distinct Scope in the registry.
            // Since we don't have direct Scope reference here, we re-scan the statements for VariableDeclarations
            // containing 'let' or 'const'. The parser represented them already; BindingKind stored in registry determines field attributes.
            bool hasLexical = blockStatement.Body.Any(s =>
                s is VariableDeclaration vd && vd.Kind is VariableDeclarationKind.Let or VariableDeclarationKind.Const);

            // If no lexical declarations, just emit statements directly.
            if (!hasLexical)
            {
                // No new lexical scope: use current leaf scope name
                GenerateStatementsForBody(_variables.GetLeafScopeName(), false, blockStatement.Body);
                return;
            }

            // Create a synthetic scope name matching SymbolTableBuilder convention so registry lookups succeed.
            // We rely on the same naming pattern used during symbol table build.
            var scopeName = $"Block_L{blockStatement.Location.Start.Line}C{blockStatement.Location.Start.Column}";

            // Emit inner statements and create the scope instance here if needed
            GenerateStatementsForBody(scopeName, true, blockStatement.Body);
        }

        private void GenerateReturnStatement(ReturnStatement returnStatement)
        {
            if (returnStatement.Argument != null)
            {
                // Special-case: returning a function identifier -> bind closure scopes
                if (returnStatement.Argument is Identifier fid)
                {
                    // Only treat as function if it corresponds to a known function declaration
                    var funcDecl = _dispatchTableGenerator.GetFunctionDeclaration(fid.Name);
                    var fnVar = funcDecl != null ? _variables.FindVariable(fid.Name) : null;
                    if (fnVar != null && funcDecl != null)
                    {
                        // Load the function delegate from its scope field
                        var scopeSlot = _variables.GetScopeLocalSlot(fnVar.ScopeName);
                        if (scopeSlot.Address == -1)
                            throw new InvalidOperationException($"Scope '{fnVar.ScopeName}' not found in local slots");
                        if (scopeSlot.Location == ObjectReferenceLocation.Parameter)
                        {
                            _il.LoadArgument(scopeSlot.Address);
                        }
                        else if (scopeSlot.Location == ObjectReferenceLocation.ScopeArray)
                        {
                            _il.LoadArgument(0);
                            _il.LoadConstantI4(scopeSlot.Address);
                            _il.OpCode(ILOpCode.Ldelem_ref);
                        }
                        else
                        {
                            _il.LoadLocal(scopeSlot.Address);
                        }
                        _il.OpCode(ILOpCode.Ldfld);
                        _il.Token(fnVar.FieldHandle); // stack: target delegate (object)

                        // Build scopes[] to bind: for closures we include global (if any) and the parent local
                        var neededScopeNames = GetScopesForClosureBinding(fnVar).ToList();
                        _il.EmitNewArray(neededScopeNames.Count, _bclReferences.ObjectType, (il, i) =>
                        {
                            var sn = neededScopeNames[i];
                            var refSlot = _variables.GetScopeLocalSlot(sn);
                            if (refSlot.Location == ObjectReferenceLocation.Local)
                            {
                                il.LoadLocal(refSlot.Address);
                            }
                            else if (refSlot.Location == ObjectReferenceLocation.Parameter)
                            {
                                il.LoadArgument(refSlot.Address);
                            }
                            else if (refSlot.Location == ObjectReferenceLocation.ScopeArray)
                            {
                                il.LoadArgument(0);
                                il.LoadConstantI4(refSlot.Address);
                                il.OpCode(ILOpCode.Ldelem_ref);
                            }
                        });

                        // Closure.Bind(object, object[])
                        // Ensure delegate is treated as object for the bind call
                        // (ldfld already leaves it as the specific Func<...>; call will accept object)
                        _runtime.InvokeClosureBindObject();
                    }
                    else
                    {
                        // Fallback to normal emit; boxing handled centrally via TypeCoercion.boxResult
                        _ = _expressionEmitter.Emit(returnStatement.Argument, new TypeCoercion() { boxResult = true });
                    }
                }
                else
                {
                    _ = _expressionEmitter.Emit(returnStatement.Argument, new TypeCoercion() { boxResult = true });
                }
            }
            else
            {
                _il.OpCode(ILOpCode.Ldnull);
            }
            _il.OpCode(ILOpCode.Ret);
        }

        /// <summary>
        /// Determines which scopes to capture when binding a function value for closure.
        /// In a function context, capture [global(if present), caller local]. In Main, capture only the leaf scope.
        /// </summary>
        private IEnumerable<string> GetScopesForClosureBinding(Variable functionVariable)
        {
            var names = _variables.GetAllScopeNames().ToList();
            var slots = names.Select(n => new { Name = n, Slot = _variables.GetScopeLocalSlot(n) }).ToList();

            bool inFunctionContext = slots.Any(e => e.Slot.Location == ObjectReferenceLocation.ScopeArray);
            if (!inFunctionContext)
            {
                yield return _variables.GetLeafScopeName();
                yield break;
            }

            // Global first if available (scopes[0])
            var global = slots.FirstOrDefault(e => e.Slot.Location == ObjectReferenceLocation.ScopeArray && e.Slot.Address == 0);
            if (global != null)
            {
                yield return global.Name;
            }

            // Then the caller/local scope hosting the callee delegate
            if (!string.IsNullOrEmpty(functionVariable.ScopeName))
            {
                var parentLocal = slots.FirstOrDefault(e => e.Name == functionVariable.ScopeName && e.Slot.Location == ObjectReferenceLocation.Local);
                if (parentLocal != null)
                {
                    yield return parentLocal.Name;
                }
            }
        }

        public void GenerateExpressionStatement(Acornima.Ast.ExpressionStatement expressionStatement)
        {
            switch (expressionStatement.Expression)
            {
                case Acornima.Ast.CallExpression callExpression:
                    // Handle CallExpression
                    GenerateCallExpression(callExpression, CallSiteContext.Statement, discardResult: true);
                    break;
                case Acornima.Ast.AssignmentExpression assignmentExpression:
                    // Handle AssignmentExpression with const reassignment guard
                    _expressionEmitter.Emit(assignmentExpression, new TypeCoercion());
                    break;
                case Acornima.Ast.BinaryExpression binaryExpression:
                    // Handle BinaryExpression
                    _binaryOperators.Generate(binaryExpression);
                    break;
                case Acornima.Ast.UpdateExpression updateExpression:
                    // Handle UpdateExpression
                    GenerateUpdateExpression(updateExpression);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported expression type in statement: {expressionStatement.Expression.Type}");
            }
        }

        public void GenerateForStatement(Acornima.Ast.ForStatement forStatement)
        {
            // first lets encode the initalizer
            if (forStatement.Init is Acornima.Ast.VariableDeclaration variableDeclaration)
            {
                DeclareVariable(variableDeclaration);
            }
            else if (forStatement.Init is Acornima.Ast.Expression exprInit)
            {
                // Handle assignment/sequence/etc. as an expression statement
                _ = _expressionEmitter.Emit(exprInit, new TypeCoercion());
            }
            else if (forStatement.Init is null)
            {
                // no-op
            }
            else
            {
                throw new NotSupportedException($"Unsupported for statement initializer type: {forStatement.Init?.Type}");
            }

            // the labels used in the loop flow control
            var loopStartLabel = _il.DefineLabel();
            var loopEndLabel = _il.DefineLabel();
            var loopBodyLabel = _il.DefineLabel();
            var loopUpdateLabel = _il.DefineLabel();

            _il.MarkLabel(loopStartLabel);

            //the test condition in the for loop
            if (forStatement.Test != null)
            {
                this._expressionEmitter.Emit(forStatement.Test, new TypeCoercion(), branching: new ConditionalBranching
                {
                    BranchOnTrue = loopBodyLabel,
                    BranchOnFalse = loopEndLabel
                });
            }

            // Push loop context: continue -> update, break -> end
            _loopStack.Push(new LoopContext(loopUpdateLabel, loopEndLabel));

            // now the body
            _il.MarkLabel(loopBodyLabel);

            GenerateStatement(forStatement.Body);

            if (forStatement.Update != null)
            {
                // Mark update section (continue targets jump here)
                _il.MarkLabel(loopUpdateLabel);
                _expressionEmitter.Emit(forStatement.Update, new TypeCoercion());
            }

            // branch back to the start of the loop
            _il.Branch(ILOpCode.Br, loopStartLabel);

            // here is the end
            _il.MarkLabel(loopEndLabel);

            // Pop loop context
            _loopStack.Pop();
        }

    public void GenerateWhileStatement(WhileStatement whileStatement)
        {
            // Labels for loop control
            var loopStartLabel = _il.DefineLabel();
            var loopBodyLabel = _il.DefineLabel();
            var loopEndLabel = _il.DefineLabel();

            // Start of loop: evaluate test
            _il.MarkLabel(loopStartLabel);
            _expressionEmitter.Emit(whileStatement.Test, new TypeCoercion(), new ConditionalBranching
            {
                BranchOnTrue = loopBodyLabel,
                BranchOnFalse = loopEndLabel
            });

            // Push loop context: continue -> start (re-test), break -> end
            _loopStack.Push(new LoopContext(loopStartLabel, loopEndLabel));

            // Body
            _il.MarkLabel(loopBodyLabel);
            GenerateStatement(whileStatement.Body);

            // Jump back to test
            _il.Branch(ILOpCode.Br, loopStartLabel);

            // End label
            _il.MarkLabel(loopEndLabel);

            // Pop context
            _loopStack.Pop();
        }

        public void GenerateDoWhileStatement(DoWhileStatement doWhileStatement)
        {
            // Labels for loop control
            var loopBodyLabel = _il.DefineLabel();
            var loopEndLabel = _il.DefineLabel();
            var loopContinueLabel = _il.DefineLabel();

            // Body executes at least once
            _il.MarkLabel(loopBodyLabel);
            // Push context: continue -> test (after body), break -> end
            _loopStack.Push(new LoopContext(loopContinueLabel, loopEndLabel));
            GenerateStatement(doWhileStatement.Body);

            // Evaluate test; if true, loop again; else, end
            _il.MarkLabel(loopContinueLabel);
            _expressionEmitter.Emit(doWhileStatement.Test, new TypeCoercion(), new ConditionalBranching
            {
                BranchOnTrue = loopBodyLabel,
                BranchOnFalse = loopEndLabel
            });

            _il.MarkLabel(loopEndLabel);
            _loopStack.Pop();
        }

        private void GenerateIfStatement(IfStatement ifStatement)
        {
            var consequentLabel = _il.DefineLabel();
            var elseLabel = _il.DefineLabel();
            var endLabel = _il.DefineLabel();


            // Actually, we want: if (test) { consequent } else { alternate }
            // So: if test is false, jump to elseLabel, otherwise fall through to consequent
            // Fix: BranchOnTrue = consequentLabel, BranchOnFalse = elseLabel            
            _expressionEmitter.Emit(ifStatement.Test, new TypeCoercion(), new ConditionalBranching
            {
                BranchOnTrue = consequentLabel,
                BranchOnFalse = elseLabel
            });

            // Consequent (if block)\
            _il.MarkLabel(consequentLabel);
            GenerateStatement(ifStatement.Consequent);
            _il.Branch(ILOpCode.Br, endLabel);

            // Else/Alternate
            _il.MarkLabel(elseLabel);
            if (ifStatement.Alternate != null)
            {
                GenerateStatement(ifStatement.Alternate);
            }

            _il.MarkLabel(endLabel);
        }

        private void GenerateThrowStatement(ThrowStatement throwStatement)
        {
            if (throwStatement.Argument == null)
            {
                throw new NotSupportedException("'throw' without an expression is not supported");
            }
            // Evaluate the expression; should yield an object (preferably JavaScriptRuntime.Error or System.Exception)
            _ = _expressionEmitter.Emit(throwStatement.Argument, new TypeCoercion() { boxResult = true });
            // If it's a boxed object that is not an Exception, .NET requires an Exception. We attempt to unbox/cast.
            // Since we cannot know statically, perform 'throw' which expects an Exception reference on stack.
                // Use 'unbox.any object' pattern is invalid; instead rely on the runtime creating Error : Exception.
                _il.OpCode(ILOpCode.Throw);
        }

        private void GenerateTryStatement(TryStatement tryStatement)
        {
            // We only support a single catch (optional) and no finally for this first pass
            var hasCatch = tryStatement.Handler != null;
            var hasFinally = tryStatement.Finalizer != null;

            if (!hasCatch && !hasFinally)
            {
                // Just emit the try block normally
                if (tryStatement.Block is BlockStatement bs)
                {
                    GenerateBlock(bs);
                }
                else
                {
                    GenerateStatement(tryStatement.Block);
                }
                return;
            }

            // Labels
            var tryStart = _il.DefineLabel();
            var tryEnd = _il.DefineLabel();
            var handlerStart = _il.DefineLabel();
            var handlerEnd = _il.DefineLabel();
            var finallyStart = hasFinally ? _il.DefineLabel() : default(LabelHandle);
            var finallyEnd = hasFinally ? _il.DefineLabel() : default(LabelHandle);
            var endLabel = _il.DefineLabel();

            // Begin try
            _il.MarkLabel(tryStart);
            if (tryStatement.Block is BlockStatement tblock)
                GenerateBlock(tblock);
            else
                GenerateStatement(tryStatement.Block);
            // Normal flow leaves try: jump to end beyond handlers
            _il.Branch(ILOpCode.Leave, endLabel);
            _il.MarkLabel(tryEnd);

            if (hasCatch)
            {
                // Catch handler
                _il.MarkLabel(handlerStart);
                // When there's no binding (catch {}), we must consume the exception object on the stack
                var cc = tryStatement.Handler!;
                // Emit body
                if (cc.Param == null)
                {
                    // No binding: ex is on stack; pop it
                    _il.OpCode(ILOpCode.Pop);
                }
                else if (cc.Param is Identifier cid)
                {
                    // Basic binding support: store exception into a temp scope variable if declared; otherwise pop
                    // For now, we don't wire to scopes; minimal viable: pop
                    _il.OpCode(ILOpCode.Pop);
                }

                if (cc.Body is BlockStatement cblock)
                    GenerateBlock(cblock);
                else
                    GenerateStatement(cc.Body);

                _il.Branch(ILOpCode.Leave, endLabel);
                _il.MarkLabel(handlerEnd);
            }

            if (hasFinally)
            {
                // Finally block
                _il.MarkLabel(finallyStart);
                if (tryStatement.Finalizer is BlockStatement fblock)
                    GenerateBlock(fblock);
                else if (tryStatement.Finalizer != null)
                    GenerateStatement(tryStatement.Finalizer);
                // finally handler must terminate with endfinally
                _il.OpCode(ILOpCode.Endfinally);
                _il.MarkLabel(finallyEnd);
            }

            // Register exception regions
            if (hasCatch)
            {
                // Only catch JavaScriptRuntime.Error; other exceptions should crash as js2il bugs
                var errorTypeRef = _runtime.GetErrorTypeRef();
                _cfb.AddCatchRegion(tryStart, tryEnd, handlerStart, handlerEnd, errorTypeRef);
            }
            if (hasFinally)
            {
                _cfb.AddFinallyRegion(tryStart, tryEnd, finallyStart, finallyEnd);
            }

            _il.MarkLabel(endLabel);
        }

        /// <summary>
        /// Attempts to emit an instance method call on a runtime intrinsic object (e.g., require('path') -> Path). Returns true if emitted.
        /// </summary>
        private bool TryEmitIntrinsicInstanceCall(Variable baseVar, string methodName, Acornima.Ast.CallExpression callExpression, bool discardResult)
        {
            // Only applies to runtime intrinsic objects backed by a known CLR type
            if (baseVar.RuntimeIntrinsicType == null)
            {
                return false;
            }

            // Load instance from the variable's scope field
            var scopeRef = _variables.GetScopeLocalSlot(baseVar.ScopeName);
            if (scopeRef.Location == ObjectReferenceLocation.Parameter)
            {
                _il.LoadArgument(scopeRef.Address);
            }
            else if (scopeRef.Location == ObjectReferenceLocation.ScopeArray)
            {
                _il.LoadArgument(0);
                _il.LoadConstantI4(scopeRef.Address);
                _il.OpCode(ILOpCode.Ldelem_ref);
            }
            else
            {
                _il.LoadLocal(scopeRef.Address);
            }
            _il.OpCode(ILOpCode.Ldfld);
            _il.Token(baseVar.FieldHandle); // stack: instance (object)

            // Reflect and select the target method, preferring params object[]
            var rt = baseVar.RuntimeIntrinsicType;
            var methods = rt
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => string.Equals(mi.Name, methodName, StringComparison.Ordinal));

            var chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
            }) ?? methods.FirstOrDefault(mi => mi.GetParameters().Length == callExpression.Arguments.Count);

            if (chosen == null)
            {
                throw new NotSupportedException($"Intrinsic method not found: {rt.FullName}.{methodName} with {callExpression.Arguments.Count} arg(s)");
            }

            var psChosen = chosen.GetParameters();
            var expectsParamsArray = psChosen.Length == 1 && psChosen[0].ParameterType == typeof(object[]);
            var reflectedParamTypes = psChosen.Select(p => p.ParameterType).ToArray();
            var reflectedReturnType = chosen.ReturnType;

            var mrefHandle = _runtime.GetInstanceMethodRef(rt, chosen.Name, reflectedReturnType, reflectedParamTypes);

            // Push arguments as either a packed object[] or individual boxed args
            if (expectsParamsArray)
            {
                _il.EmitNewArray(callExpression.Arguments.Count, _bclReferences.ObjectType, (il, i) =>
                {
                    _expressionEmitter.Emit(callExpression.Arguments[i], new TypeCoercion() { boxResult = true });
                });
            }
            else
            {
                for (int i = 0; i < callExpression.Arguments.Count; i++)
                {
                    _expressionEmitter.Emit(callExpression.Arguments[i], new TypeCoercion() { boxResult = true });
                }
            }

            _il.OpCode(ILOpCode.Callvirt);
            _il.Token(mrefHandle);
            if (discardResult)
            {
                _il.OpCode(ILOpCode.Pop);
            }

            return true;
        }

        private void GenerateObjectExpresion(ObjectExpression objectExpression)
        {
            // first we need to creat a new instance of the expando object
            _il.OpCode(ILOpCode.Newobj);
            _il.Token(_bclReferences.Expando_Ctor_Ref);

            // we create a new object instance for the object expression
            // the generic solution is to use the ExpandoObject
            // will apply optimizations in the future when we can calculate the object schema
            foreach (var property in objectExpression.Properties)
            {
                if (property is not ObjectProperty objectProperty)
                {
                    throw new NotSupportedException($"Unsupported object property type: {property.Type}");
                }

                if (objectProperty.Key is not Identifier propertyKey)
                {
                    throw new NotSupportedException($"Unsupported object property key type: {objectProperty.Key.Type}");
                }

                if (objectProperty.Value is not Expression propertyValue)
                {
                    throw new NotSupportedException($"Unsupported object property value type: {objectProperty.Value.Type}");
                }

                // Duplicate the ExpandoObject reference on the stack
                _il.OpCode(ILOpCode.Dup);

                // in a perfect world we could support any expression for the property name
                // but not feature rich enough to support that yet
                //_expressionEmitter.Emit(objectProperty.Key);
                _il.Ldstr(_metadataBuilder, propertyKey.Name);

                // Load the value of the property (Emit handles boxing by default)
                _ = _expressionEmitter.Emit(propertyValue, new TypeCoercion() { boxResult = true });

                // call set_Item on the ExpandoObject to set the property value
                _il.OpCode(ILOpCode.Callvirt);
                _il.Token(_bclReferences.IDictionary_SetItem_Ref);
            }

            // After all properties are set, the ExpandoObject is on the stack
            // this is the expected behavior.  The consumer for this expression output chooses what to do with it.
        }

        private void GenerateArrayExpression(ArrayExpression arrayExpression)
        {
            // Use JavaScriptRuntime.Array (List<object>-backed) to preserve JS semantics
            // 1) push capacity
            _il.LoadConstantI4(arrayExpression.Elements.Count);
            // 2) invoke runtime array ctor (produces JavaScriptRuntime.Array instance boxed as object)
            _runtime.InvokeArrayCtor();

            // For each element: duplicate array ref, load element (boxed), call Add
            for (int i = 0; i < arrayExpression.Elements.Count; i++)
            {
                var element = arrayExpression.Elements[i];
                _il.OpCode(ILOpCode.Dup); // array instance
                _ = _expressionEmitter.Emit(element!, new TypeCoercion() { boxResult = true });
                _il.OpCode(ILOpCode.Callvirt);
                _il.Token(_bclReferences.Array_Add_Ref);
            }
        }

        private void GenerateUpdateExpression(Acornima.Ast.UpdateExpression updateExpression)
        {
            if ((updateExpression.Operator != Acornima.Operator.Increment && updateExpression.Operator != Acornima.Operator.Decrement) || updateExpression.Prefix)
            {
                throw new NotSupportedException($"Unsupported update expression operator: {updateExpression.Operator} or prefix: {updateExpression.Prefix}");
            }
            // Handle postfix increment (x++) and decrement (x--)
            var variableName = (updateExpression.Argument as Acornima.Ast.Identifier)!.Name;
            var variable = _variables.FindVariable(variableName);

            // If bound variable is const, emit a TypeError and throw
            if (IsConstBinding(variableName))
            {
                EmitConstReassignmentTypeError();
                return;
            }

            // Handle scope field variables
            if (variable == null)
            {
                throw new InvalidOperationException("Variable reference is null.");
            }
            var scopeLocalIndex = _variables.GetScopeLocalSlot(variable.ScopeName);
            if (scopeLocalIndex.Address == -1)
            {
                throw new InvalidOperationException($"Scope '{variable.ScopeName}' not found in local slots");
            }

            // Load scope instance for the store operation later
            if (scopeLocalIndex.Location == ObjectReferenceLocation.Parameter)
            {
                _il.LoadArgument(scopeLocalIndex.Address);
            }
            else
            {
                _il.LoadLocal(scopeLocalIndex.Address);
            }

            // Load the current value from scope field  
            _il.LoadLocal(scopeLocalIndex.Address);
            _il.OpCode(ILOpCode.Ldfld);
            _il.Token(variable.FieldHandle);

            // unbox the variable
            _il.OpCode(ILOpCode.Unbox_any);
            _il.Token(_bclReferences.DoubleType);

            // increment or decrement by 1
            _il.LoadConstantR8(1.0);
            if (updateExpression.Operator == Acornima.Operator.Increment)
            {
                _il.OpCode(ILOpCode.Add);
            }
            else // Decrement
            {
                _il.OpCode(ILOpCode.Sub);
            }

            // box the result back to an object
            _il.OpCode(ILOpCode.Box);
            _il.Token(_bclReferences.DoubleType);

            // Now stack is: [scope_instance] [boxed_result] - perfect for stfld
            _il.OpCode(ILOpCode.Stfld);
            _il.Token(variable.FieldHandle);
        }

        // Emits: throw new TypeError("Assignment to constant variable.")
        private void EmitConstReassignmentTypeError()
        {
            var ctor = _runtime.GetErrorCtorRef("TypeError", 1);
            _il.EmitThrowError(_metadataBuilder, ctor, "Assignment to constant variable.");
        }

        // Consult the registry to see if the current scope has a const binding for this name
        private bool IsConstBinding(string variableName)
        {
            var registry = _variables.GetVariableRegistry();
            var scopeName = _variables.GetLeafScopeName();
            var info = registry?.GetVariableInfo(scopeName, variableName) ?? registry?.FindVariable(variableName);
            return info != null && info.BindingKind == SymbolTables.BindingKind.Const;
        }

        private void GenerateCallExpression(Acornima.Ast.CallExpression callExpression, CallSiteContext context, bool discardResult)
        {
            // General member call: obj.method(...)
            if (callExpression.Callee is Acornima.Ast.MemberExpression mem && !mem.Computed && mem.Property is Acornima.Ast.Identifier mname)
            {
                // Pattern support: String(x).replace(/pattern/flags, replacement)
                if (string.Equals(mname.Name, "replace", StringComparison.Ordinal) && mem.Object is Acornima.Ast.CallExpression strCall && strCall.Callee is Acornima.Ast.Identifier sid && string.Equals(sid.Name, "String", StringComparison.Ordinal))
                {
                    // Expect exactly: String(arg0).replace(regex, replacement)
                    if (strCall.Arguments.Count != 1 || callExpression.Arguments.Count != 2)
                    {
                        throw new NotSupportedException("Only replace(regex, string) with a single String(arg) receiver is supported");
                    }
                    // Extract regex pattern and flags from AST via reflection to avoid taking a hard dependency on node types
                    string? pattern = null;
                    string? flags = null;
                    var rxNode = callExpression.Arguments[0];
                    {
                        var rxType = rxNode?.GetType();
                        if (rxType != null)
                        {
                            // Try nested Regex property (Literal.regex.{pattern,flags}) – case-insensitive
                            object? regexObj = GetPropertyIgnoreCase(rxNode!, "Regex")?.GetValue(rxNode!);
                            if (regexObj != null)
                            {
                                pattern = GetPropertyIgnoreCase(regexObj, "Pattern")?.GetValue(regexObj) as string;
                                flags = GetPropertyIgnoreCase(regexObj, "Flags")?.GetValue(regexObj) as string;
                            }

                            // Fallback: direct properties on the node (Pattern/Flags) – case-insensitive
                            pattern ??= GetPropertyIgnoreCase(rxNode!, "Pattern")?.GetValue(rxNode) as string;
                            flags ??= GetPropertyIgnoreCase(rxNode!, "Flags")?.GetValue(rxNode) as string;

                            // Final fallback: parse Raw string like '/pattern/flags'
                            if (string.IsNullOrEmpty(pattern))
                            {
                                var raw = GetPropertyIgnoreCase(rxNode!, "Raw")?.GetValue(rxNode) as string;
                                if (!string.IsNullOrEmpty(raw))
                                {
                                    (pattern, flags) = ParseRegexRaw(raw!);
                                }
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(pattern))
                    {
                        throw new NotSupportedException("Regex literal pattern not found for String(x).replace");
                    }
                    var global = !string.IsNullOrEmpty(flags) && flags!.IndexOf('g') >= 0;
                    var ignoreCase = !string.IsNullOrEmpty(flags) && flags!.IndexOf('i') >= 0;

                    // Stack: [input, pattern, replacement, global, ignoreCase]
                    // input = ToString(arg0)
                    _ = _expressionEmitter.Emit(strCall.Arguments[0], new TypeCoercion() { toString = true });
                    // pattern
                    _il.Ldstr(_metadataBuilder, pattern!);
                    // replacement as string
                    _ = _expressionEmitter.Emit(callExpression.Arguments[1], new TypeCoercion() { toString = true });
                    // booleans
                    _il.LoadConstantI4(global ? 1 : 0);
                    _il.LoadConstantI4(ignoreCase ? 1 : 0);

                    // Resolve JavaScriptRuntime.String.Replace dynamically like console.log
                    var stringType = JavaScriptRuntime.IntrinsicObjectRegistry.Get("String")
                        ?? throw new NotSupportedException("Host intrinsic 'String' not found");
                    var replaceMethod = stringType
                        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .FirstOrDefault(mi => string.Equals(mi.Name, "Replace", StringComparison.Ordinal)
                                              && mi.GetParameters().Length == 5);
                    if (replaceMethod == null)
                    {
                        throw new NotSupportedException("Host intrinsic method not found: String.Replace(input, pattern, replacement, global, ignoreCase)");
                    }
                    var mref = _runtime.GetStaticMethodRef(stringType, replaceMethod.Name, replaceMethod.ReturnType, replaceMethod.GetParameters().Select(p => p.ParameterType).ToArray());
                    _il.Call(mref);
                    if (discardResult)
                    {
                        _il.OpCode(ILOpCode.Pop);
                    }
                    return;
                }
                if (mem.Object is Acornima.Ast.Identifier baseId)
                {
                    // If the base identifier resolves to a known class, emit a static call without loading an instance.
                    if (_classRegistry.TryGet(baseId.Name, out var classType) && !classType.IsNil)
                    {
                        // Build static method signature: object method(object, ...)
                        var sArgCount = callExpression.Arguments.Count;
                        var sSig = new BlobBuilder();
                        new BlobEncoder(sSig)
                            .MethodSignature(isInstanceMethod: false)
                            .Parameters(sArgCount, r => r.Type().Object(), p => { for (int i = 0; i < sArgCount; i++) p.AddParameter().Type().Object(); });
                        var sMsig = _metadataBuilder.GetOrAddBlob(sSig);
                        var sMref = _metadataBuilder.AddMemberReference(classType, _metadataBuilder.GetOrAddString(mname.Name), sMsig);
                        // Push arguments
                        for (int i = 0; i < callExpression.Arguments.Count; i++)
                        {
                            _expressionEmitter.Emit(callExpression.Arguments[i], new TypeCoercion() { boxResult = true });
                        }
                        _il.Call(sMref);
                        if (discardResult) _il.OpCode(ILOpCode.Pop);
                        return;
                    }
                    // Step 1: Is it a variable?
                    var baseVar = _variables.FindVariable(baseId.Name);
                    if (baseVar != null)
                    {
                        // Step 2: Is it an object/class instance? Try intrinsic first, then class instance fallback
                        if (TryEmitIntrinsicInstanceCall(baseVar, mname.Name, callExpression, discardResult))
                        {
                            return;
                        }

                        // Non-intrinsic instance method call fallback (class instance created via `new`)
                        // Load instance from variable field
                        var scopeRef = _variables.GetScopeLocalSlot(baseVar.ScopeName);
                        if (scopeRef.Location == ObjectReferenceLocation.Parameter) _il.LoadArgument(scopeRef.Address);
                        else if (scopeRef.Location == ObjectReferenceLocation.ScopeArray) { _il.LoadArgument(0); _il.LoadConstantI4(scopeRef.Address); _il.OpCode(ILOpCode.Ldelem_ref); }
                        else _il.LoadLocal(scopeRef.Address);
                        _il.OpCode(ILOpCode.Ldfld);
                        _il.Token(baseVar.FieldHandle); // stack: instance (object)

                        // Resolve the concrete class type if known (based on prior `new` assignment)
                        var argCount = callExpression.Arguments.Count;
                        var sig = new BlobBuilder();
                        new BlobEncoder(sig)
                            .MethodSignature(isInstanceMethod: true)
                            .Parameters(argCount, r => r.Type().Object(), p => { for (int i = 0; i < argCount; i++) p.AddParameter().Type().Object(); });
                        var msig = _metadataBuilder.GetOrAddBlob(sig);

                        TypeDefinitionHandle targetType = default;
                        if (_variableToClass.TryGetValue(baseId.Name, out var cname))
                        {
                            if (_classRegistry.TryGet(cname, out var th)) targetType = th;
                        }
                        if (targetType.IsNil)
                        {
                            throw new NotSupportedException($"Cannot resolve class type for variable '{baseId.Name}' to call method '{mname.Name}'.");
                        }
                        var mrefHandle = _metadataBuilder.AddMemberReference(targetType, _metadataBuilder.GetOrAddString(mname.Name), msig);

                        // Push arguments
                        for (int i = 0; i < callExpression.Arguments.Count; i++)
                        {
                            _expressionEmitter.Emit(callExpression.Arguments[i], new TypeCoercion() { boxResult = true });
                            // Optional: castclass for specific ref types; keep simple for now as literals are already typed
                        }

                        _il.OpCode(ILOpCode.Callvirt);
                        _il.Token(mrefHandle);
                        if (discardResult) _il.OpCode(ILOpCode.Pop);
                        return;
                    }
                    else
                    {
                        // Step 4: Not a variable - try host intrinsic (e.g., console.log)
                        if (TryEmitHostIntrinsicStaticCall(baseId.Name, mname.Name, callExpression, discardResult))
                        {
                            return;
                        }
                        throw new NotSupportedException($"Unsupported member call base identifier: '{baseId.Name}'");
                    }
                }
                // Non-identifier callee under MemberExpression is unsupported beyond the above cases
                throw new NotSupportedException($"Unsupported call expression callee type: {callExpression.Callee.Type}");
            }
            else if (callExpression.Callee is Acornima.Ast.Identifier identifier)
            {
                // Simple function call: f(...)
                EmitFunctionCall(identifier, callExpression, context, discardResult);
                return;
            }
            else
            {
                throw new NotSupportedException($"Unsupported call expression callee type: {callExpression.Callee.Type}");
            }
        }

        // Emits a call to a function identified by an Identifier in the current scope, including scope array construction and delegate dispatch.
        private void EmitFunctionCall(Acornima.Ast.Identifier identifier, Acornima.Ast.CallExpression callExpression, CallSiteContext context, bool discardResult)
        {
            // Node-style require("module") support as a built-in
            if (string.Equals(identifier.Name, "require", StringComparison.Ordinal))
            {
                if (callExpression.Arguments.Count != 1)
                {
                    throw new ArgumentException("require expects exactly one argument");
                }
                // Coerce argument to string (for literals this emits ldstr directly)
                _ = _expressionEmitter.Emit(callExpression.Arguments[0], new TypeCoercion() { toString = true });
                _runtime.InvokeRequire();
                if (discardResult)
                {
                    _il.OpCode(ILOpCode.Pop);
                }
                return;
            }

            var functionVariable = _variables.FindVariable(identifier.Name);
            if (functionVariable == null)
            {
                throw new ArgumentException($"Function {identifier.Name} is not defined.");
            }

            // Load the scope instance as the first parameter
            Action loadScopeInstance;
            var scopeObjectReference = _variables.GetScopeLocalSlot(functionVariable.ScopeName);
            if (scopeObjectReference.Address == -1)
            {
                throw new InvalidOperationException($"Scope '{functionVariable.ScopeName}' not found in local slots");
            }
            if (scopeObjectReference.Location == ObjectReferenceLocation.Local)
            {
                loadScopeInstance = () => _il.LoadLocal(scopeObjectReference.Address);
            }
            else if (scopeObjectReference.Location == ObjectReferenceLocation.Parameter)
            {
                // Load the scope instance from the field
                loadScopeInstance = () => _il.LoadArgument(scopeObjectReference.Address);
            }
            else if (scopeObjectReference.Location == ObjectReferenceLocation.ScopeArray)
            {
                // Load from scope array at index 0
                loadScopeInstance = () =>
                {
                    _il.LoadArgument(0); // Load scope array parameter
                    _il.LoadConstantI4(0); // Index 0 for global scope
                    _il.OpCode(ILOpCode.Ldelem_ref);
                };
            }
            else
            {
                throw new InvalidOperationException($"Unsupported scope object reference location: {scopeObjectReference.Location}");
            }

            // load the delegate to be invoked (from scope field)
            loadScopeInstance();
            _il.OpCode(ILOpCode.Ldfld);
            _il.Token(functionVariable.FieldHandle);

            // First argument: create scope array with appropriate scopes for the function
            // Only include scopes that are actually needed for this function call
            var neededScopeNames = GetNeededScopesForFunction(functionVariable, context).ToList();
            var arraySize = neededScopeNames.Count;

            _il.EmitNewArray(arraySize, _bclReferences.ObjectType, (il, i) =>
            {
                var scopeName = neededScopeNames[i];
                var scopeRef = _variables.GetScopeLocalSlot(scopeName);
                if (scopeRef.Location == ObjectReferenceLocation.Local)
                {
                    il.LoadLocal(scopeRef.Address);
                }
                else if (scopeRef.Location == ObjectReferenceLocation.Parameter)
                {
                    il.LoadArgument(scopeRef.Address);
                }
                else if (scopeRef.Location == ObjectReferenceLocation.ScopeArray)
                {
                    il.LoadArgument(0); // Load scope array parameter
                    il.LoadConstantI4(scopeRef.Address); // Load array index
                    il.OpCode(ILOpCode.Ldelem_ref); // Load scope from array
                }
            });

            // Additional arguments: directly emit each call argument (boxed as needed)
            // If this is a declared function we could validate arity, but for arrow functions or runtime values,
            // we simply pass through the provided arguments.
            for (int i = 0; i < callExpression.Arguments.Count; i++)
            {
                _ = _expressionEmitter.Emit(callExpression.Arguments[i], new TypeCoercion() { boxResult = true });
            }

            // Invoke correct delegate based on parameter count.
            // Select overloads based on call-site context to match historical snapshots.
            var argCount = callExpression.Arguments.Count;
            _il.OpCode(ILOpCode.Callvirt);
            if (context == CallSiteContext.Statement)
            {
                // Statement: array-based Invoke for 0/1 parameters
                if (argCount == 0)
                {
                    _il.Token(_bclReferences.FuncObjectArrayObject_Invoke_Ref);
                }
                else if (argCount == 1)
                {
                    _il.Token(_bclReferences.FuncObjectArrayObjectObject_Invoke_Ref);
                }
                else if (argCount <= 6)
                {
                    _il.Token(_bclReferences.GetFuncArrayParamInvokeRef(argCount));
                }
                else
                {
                    throw new NotSupportedException($"Only up to 6 parameters supported currently (got {argCount})");
                }
            }
            else
            {
                // Expression: non-array-based Invoke for 0/1 parameters; array-based for >=2
                if (argCount == 0)
                {
                    _il.Token(_bclReferences.FuncObjectObject_Invoke_Ref);
                }
                else if (argCount == 1)
                {
                    _il.Token(_bclReferences.FuncObjectObjectObject_Invoke_Ref);
                }
                else if (argCount <= 6)
                {
                    _il.Token(_bclReferences.GetFuncArrayParamInvokeRef(argCount));
                }
                else
                {
                    throw new NotSupportedException($"Only up to 6 parameters supported currently (got {argCount})");
                }
            }
            // Discard result if requested (statement context)
            if (discardResult)
            {
                _il.OpCode(ILOpCode.Pop);
            }
        }

        // Emits a static call on a host intrinsic object (e.g., console.log), discovered via JavaScriptRuntime.IntrinsicObjectAttribute
        private bool TryEmitHostIntrinsicStaticCall(string objectName, string methodName, Acornima.Ast.CallExpression callExpression, bool discardResult)
        {
            var type = JavaScriptRuntime.IntrinsicObjectRegistry.Get(objectName);
            if (type == null)
            {
                return false;
            }

            // Reflect static method; prefer params object[]
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(mi => string.Equals(mi.Name, methodName, StringComparison.OrdinalIgnoreCase));
            var chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
            }) ?? methods.FirstOrDefault(mi => mi.GetParameters().Length == callExpression.Arguments.Count);
            if (chosen == null)
            {
                throw new NotSupportedException($"Host intrinsic method not found: {type.FullName}.{methodName} with {callExpression.Arguments.Count} arg(s)");
            }

            var ps = chosen.GetParameters();
            var expectsParamsArray = ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
            var paramTypes = ps.Select(p => p.ParameterType).ToArray();
            var retType = chosen.ReturnType;

            var mref = _runtime.GetStaticMethodRef(type, chosen.Name, retType, paramTypes);

            if (expectsParamsArray)
            {
                _il.EmitNewArray(callExpression.Arguments.Count, _bclReferences.ObjectType, (il, i) =>
                {
                    _expressionEmitter.Emit(callExpression.Arguments[i], new TypeCoercion() { boxResult = true });
                });
            }
            else
            {
                for (int i = 0; i < callExpression.Arguments.Count; i++)
                {
                    _expressionEmitter.Emit(callExpression.Arguments[i], new TypeCoercion() { boxResult = true });
                }
            }

                _il.Call(mref);
                // Pop only if caller wants to discard and method returns a value
                if (discardResult && retType != typeof(void))
                {
                    _il.OpCode(ILOpCode.Pop);
                }
            return true;
        }

        // Helper to emit a UnaryExpression and return its JavaScript type (or Unknown when control-flow handled)
        private JavascriptType EmitUnaryExpression(UnaryExpression unaryExpression, TypeCoercion typeCoercion, ConditionalBranching? branching)
        {
            // Support logical not: !expr and simple unary negation for numeric literals
            var op = unaryExpression.Operator;
            if (op == Operator.LogicalNot)
            {
                // If we're in a conditional context, invert the branch directly: if (!x) ... => branch on x == false
                if (branching != null)
                {
                    var argType = ((IMethodExpressionEmitter)this).Emit(unaryExpression.Argument, new TypeCoercion() { boxResult = false }, null);

                    if (argType == JavascriptType.Boolean)
                    {
                        // If the argument was not a literal, it's boxed; unbox first
                        if (unaryExpression.Argument is not BooleanLiteral && !(unaryExpression.Argument is Literal lit && lit.Value is bool))
                        {
                            _il.OpCode(ILOpCode.Unbox_any);
                            _il.Token(_bclReferences.BooleanType);
                        }
                        // Brfalse => when arg is false, jump to BranchOnTrue (since !arg is true)
                        _il.Branch(ILOpCode.Brfalse, branching.BranchOnTrue);
                    }
                    else if (argType == JavascriptType.Number)
                    {
                        // ToBoolean(number): 0 => false; so Brfalse when number == 0. Convert to i4 zero-check.
                        // number on stack is double; compare to 0 and branch on equality
                        _il.LoadConstantR8(0);
                        _il.OpCode(ILOpCode.Ceq);
                        _il.Branch(ILOpCode.Brtrue, branching.BranchOnTrue);
                    }
                    else
                    {
                        // Assume boxed boolean; unbox then brfalse
                        _il.OpCode(ILOpCode.Unbox_any);
                        _il.Token(_bclReferences.BooleanType);
                        _il.Branch(ILOpCode.Brfalse, branching.BranchOnTrue);
                    }

                    if (branching.BranchOnFalse.HasValue)
                    {
                        _il.Branch(ILOpCode.Br, branching.BranchOnFalse.Value);
                    }
                    else
                    {
                        // No else block: nothing to pop; branch consumes the tested value.
                    }

                    return JavascriptType.Unknown;
                }
                else
                {
                    // Non-branching context: compute the boolean value and invert it on the stack.
                    var argType = ((IMethodExpressionEmitter)this).Emit(unaryExpression.Argument, new TypeCoercion() { boxResult = false }, null);

                    if (argType == JavascriptType.Boolean)
                    {
                        if (unaryExpression.Argument is not BooleanLiteral && !(unaryExpression.Argument is Literal lit2 && lit2.Value is bool))
                        {
                            _il.OpCode(ILOpCode.Unbox_any);
                            _il.Token(_bclReferences.BooleanType);
                        }
                        _il.OpCode(ILOpCode.Ldc_i4_0);
                        _il.OpCode(ILOpCode.Ceq);
                    }
                    else if (argType == JavascriptType.Number)
                    {
                        _il.LoadConstantR8(0);
                        _il.OpCode(ILOpCode.Ceq);
                    }
                    else
                    {
                        _il.OpCode(ILOpCode.Unbox_any);
                        _il.Token(_bclReferences.BooleanType);
                        _il.OpCode(ILOpCode.Ldc_i4_0);
                        _il.OpCode(ILOpCode.Ceq);
                    }

                    return JavascriptType.Boolean;
                }
            }
            else if (op == Operator.TypeOf)
            {
                // Emit typeof: evaluate argument (boxed), then call JavaScriptRuntime.TypeUtilities.Typeof(object)
                var _ = ((IMethodExpressionEmitter)this).Emit(unaryExpression.Argument, new TypeCoercion() { boxResult = true }, null);
                var mref = _runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.Typeof), typeof(string), typeof(object));
                _il.OpCode(ILOpCode.Call);
                _il.Token(mref);
                return JavascriptType.Object; // string
            }
            else if (op == Operator.UnaryNegation && unaryExpression.Argument is Acornima.Ast.NumericLiteral numericArg)
            {
                if (typeCoercion.toString)
                {
                    var numberAsString = (-numericArg.Value).ToString();
                    _il.Ldstr(_metadataBuilder, numberAsString);
                }
                else
                {
                    _il.LoadConstantR8(-numericArg.Value);
                }
                // Preserve prior behavior: no explicit type assignment beyond emitted value
                return JavascriptType.Unknown;
            }
            else
            {
                throw new NotSupportedException($"Unsupported unary operator: {op}");
            }
        }

        JavascriptType IMethodExpressionEmitter.Emit(Expression expression, TypeCoercion typeCoercion, ConditionalBranching? branching)
        {
            JavascriptType javascriptType = JavascriptType.Unknown;

            switch (expression)
            {
                case ThisExpression thisExpression:
                    if (_inClassMethod)
                    {
                        _il.OpCode(ILOpCode.Ldarg_0);
                        javascriptType = JavascriptType.Object; // 'this' is the instance of the class
                    }
                    else
                    {
                        // TODO - expand support for this to functions
                        throw new NotSupportedException("Unsupported 'this' expression outside of class context");
                    }
                    break;
                case AssignmentExpression assignmentExpression:
                    javascriptType = EmitAssignment(assignmentExpression, typeCoercion);
                    break;
                case CallExpression callExpression:
                    // Use the unified call generator in expression context, preserving the result
                    GenerateCallExpression(callExpression, CallSiteContext.Expression, discardResult: false);
                    javascriptType = JavascriptType.Object;
                    break;
                case ArrowFunctionExpression arrowFunction:
                    {
                        // Generate a static method for the arrow function and create a delegate instance.
                        // Use a deterministic but local owner type for simplicity.
                        var paramNames = arrowFunction.Params.OfType<Identifier>().Select(p => p.Name).ToArray();
                        // Use assignment-target name for registry scope, but keep IL method/type naming location-based to preserve snapshots
                        var registryScopeName = !string.IsNullOrEmpty(_currentAssignmentTarget)
                            ? $"ArrowFunction_{_currentAssignmentTarget}"
                            : $"ArrowFunction_L{arrowFunction.Location.Start.Line}C{arrowFunction.Location.Start.Column}";
                        var ilMethodName = $"ArrowFunction_L{arrowFunction.Location.Start.Line}C{arrowFunction.Location.Start.Column}";
                        var methodHandle = GenerateArrowFunctionMethod(arrowFunction, registryScopeName, ilMethodName, paramNames);

                        // ldnull ; ldftn method ; newobj Func<object[], object, ...>
                        _il.OpCode(ILOpCode.Ldnull);
                        _il.OpCode(ILOpCode.Ldftn);
                        _il.Token(methodHandle);
                        _il.OpCode(ILOpCode.Newobj);
                        var (_, ctorRef) = _bclReferences.GetFuncObjectArrayWithParams(paramNames.Length);
                        _il.Token(ctorRef);
                        javascriptType = JavascriptType.Object;
                    }
                    break;
                case ArrayExpression arrayExpression:
                    // Generate code for ArrayExpression
                    GenerateArrayExpression(arrayExpression);
                    javascriptType = JavascriptType.Object; // Arrays are treated as objects in JavaScript
                    break;
                case NewExpression newExpression:
                    javascriptType = EmitNewExpression(newExpression);
                    break;
                case BinaryExpression binaryExpression:
                    _binaryOperators.Generate(binaryExpression, branching);
                    break;
                case NumericLiteral numericLiteral:
                    // Load numeric literal
                    LoadValue(expression, typeCoercion);

                    javascriptType = JavascriptType.Number;

                    break;
                case BooleanLiteral booleanLiteral:
                    LoadValue(expression, typeCoercion);
                    javascriptType = JavascriptType.Boolean;
                    break;
                case UpdateExpression updateExpression:
                    GenerateUpdateExpression(updateExpression);
                    break;
                case UnaryExpression unaryExpression:
                    javascriptType = EmitUnaryExpression(unaryExpression, typeCoercion, branching);
                    break;
                case ObjectExpression objectExpression:
                    GenerateObjectExpresion(objectExpression);

                    javascriptType = JavascriptType.Object;
                    break;
                case MemberExpression memberExpression:
                    javascriptType = EmitMemberExpression(memberExpression);
                    break;
                case TemplateLiteral template:
                    {
                        // Emit JS template literal: concatenate cooked quasis and expressions via Operators.Add
                        // Start with first quasi text (may be empty)
                        string GetQuasiText(TemplateElement te)
                        {
                            // Try te.Value.Cooked, then te.Value.Raw, else empty
                            var valProp = te.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                            if (valProp != null)
                            {
                                var val = valProp.GetValue(te);
                                if (val != null)
                                {
                                    var cooked = val.GetType().GetProperty("Cooked", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)?.GetValue(val) as string;
                                    if (!string.IsNullOrEmpty(cooked)) return cooked!;
                                    var raw = val.GetType().GetProperty("Raw", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)?.GetValue(val) as string;
                                    if (raw != null) return raw;
                                }
                            }
                            return string.Empty;
                        }

                        var quasis = template.Quasis;
                        var exprs = template.Expressions;

                        // Ensure there's at least one quasi
                        string firstText = quasis.Count > 0 ? GetQuasiText(quasis[0]) : string.Empty;
                        _il.LoadString(_metadataBuilder.GetOrAddUserString(firstText ?? string.Empty));

                        // For each expression, append value and following quasi
                        for (int i = 0; i < exprs.Count; i++)
                        {
                            // current + expr
                            _ = _expressionEmitter.Emit(exprs[i], new TypeCoercion { boxResult = true }, null);
                            _runtime.InvokeOperatorsAdd();

                            // then + next quasi text (quasis are one longer than expressions)
                            string tail = (i + 1) < quasis.Count ? GetQuasiText(quasis[i + 1]) : string.Empty;
                            _il.LoadString(_metadataBuilder.GetOrAddUserString(tail ?? string.Empty));
                            _runtime.InvokeOperatorsAdd();
                        }

                        javascriptType = JavascriptType.Object; // result is a string (object)
                    }
                    break;
                case ConditionalExpression cond:
                    {
                        // Emit expression-level ternary: test ? consequent : alternate
                        var trueLabel = _il.DefineLabel();
                        var falseLabel = _il.DefineLabel();
                        var endLabel = _il.DefineLabel();

                        // Evaluate test with branching (let BinaryOperators or fallback boolean branching handle it)
                        _expressionEmitter.Emit(cond.Test, new TypeCoercion(), new ConditionalBranching
                        {
                            BranchOnTrue = trueLabel,
                            BranchOnFalse = falseLabel
                        });

                        // True arm
                        _il.MarkLabel(trueLabel);
                        // Force result as object to unify stack type across arms; propagate toString if requested
                        var armCoercion = new TypeCoercion { boxResult = true, toString = typeCoercion.toString };
                        _ = _expressionEmitter.Emit(cond.Consequent, armCoercion);
                        _il.Branch(ILOpCode.Br, endLabel);

                        // False arm
                        _il.MarkLabel(falseLabel);
                        _ = _expressionEmitter.Emit(cond.Alternate, armCoercion);

                        _il.MarkLabel(endLabel);
                        javascriptType = JavascriptType.Object;
                    }
                    break;
                case Acornima.Ast.Identifier identifier:
                    {
                        var name = identifier.Name;
                        var variable = _variables.FindVariable(name) ?? throw new InvalidOperationException($"Variable '{name}' not found.");
                        _binaryOperators.LoadVariable(variable); // Load variable using scope field or local fallback

                        // Only unbox when we explicitly know it's a Number && caller didn't request boxing.
                        if (variable.Type == JavascriptType.Number && !typeCoercion.boxResult)
                        {
                            _il.OpCode(ILOpCode.Unbox_any);
                            _il.Token(_bclReferences.DoubleType); // unbox the variable as a double
                        }
                        else
                        {
                            // currently variables are already boxed, so no need to double box (if boxResult is true)
                            typeCoercion.boxResult = false;
                        }

                        javascriptType = variable.Type;
                    }

                    break;
                default:
                    javascriptType = LoadValue(expression, typeCoercion);
                    break;
            }

            // If this emit is for a conditional test (if/for) and wasn't handled by BinaryOperators,
            // branch based on a boolean value now. This consumes the value from the stack.
            if (branching != null && expression is not BinaryExpression)
            {
                if (javascriptType == JavascriptType.Boolean)
                {
                    _il.Branch(ILOpCode.Brtrue, branching.BranchOnTrue);
                    if (branching.BranchOnFalse.HasValue)
                    {
                        _il.Branch(ILOpCode.Br, branching.BranchOnFalse.Value);
                    }
                    else
                    {
                        _il.OpCode(ILOpCode.Pop);
                    }
                    // We've emitted control flow, no result remains
                    return JavascriptType.Unknown;
                }
                else if (javascriptType == JavascriptType.Unknown)
                {
                    // The expression emitter handled branching itself (e.g., unary logical not)
                    return JavascriptType.Unknown;
                }
                else
                {
                    throw new NotSupportedException($"Unsupported conditional test expression type: {javascriptType} for node {expression.Type}");
                }
            }

            // Centralized boxing: if requested, box primitive results
            if (typeCoercion.boxResult)
            {
                if (javascriptType == JavascriptType.Number)
                {
                    _il.OpCode(ILOpCode.Box);
                    _il.Token(_bclReferences.DoubleType);
                }
                else if (javascriptType == JavascriptType.Boolean)
                {
                    _il.OpCode(ILOpCode.Box);
                    _il.Token(_bclReferences.BooleanType);
                }
            }

            return javascriptType;
        }

        // Helper to emit an assignment expression and return its JavaScript type
        private JavascriptType EmitAssignment(AssignmentExpression assignmentExpression, TypeCoercion typeCoercion)
        {
            // Support assignments to identifiers and to this.property within class instance methods
            if (assignmentExpression.Left is Identifier aid)
            {
                // Guard: const reassignment attempts throw at runtime
                if (IsConstBinding(aid.Name))
                {
                    EmitConstReassignmentTypeError();
                    return JavascriptType.Unknown; // unreachable
                }
                var variable = _variables.FindVariable(aid.Name) ?? throw new InvalidOperationException($"Variable '{aid.Name}' not found");

                // Load the appropriate scope instance that holds this field
                var scopeSlot = _variables.GetScopeLocalSlot(variable.ScopeName);
                if (scopeSlot.Address == -1)
                {
                    throw new InvalidOperationException($"Scope '{variable.ScopeName}' not found in local slots");
                }
                // Load scope instance
                if (scopeSlot.Location == ObjectReferenceLocation.Parameter)
                {
                    _il.LoadArgument(scopeSlot.Address);
                }
                else if (scopeSlot.Location == ObjectReferenceLocation.ScopeArray)
                {
                    _il.LoadArgument(0); // Load scope array parameter
                    _il.LoadConstantI4(scopeSlot.Address); // Load array index
                    _il.OpCode(ILOpCode.Ldelem_ref); // Load scope from array
                }
                else
                {
                    _il.LoadLocal(scopeSlot.Address);
                }

                var prevAssignment = _currentAssignmentTarget;
                _currentAssignmentTarget = aid.Name;
                var rhsType = _expressionEmitter.Emit(assignmentExpression.Right, typeCoercion);
                _currentAssignmentTarget = prevAssignment;
                variable.Type = rhsType;
                _il.OpCode(ILOpCode.Stfld);
                _il.Token(variable.FieldHandle);
                return rhsType;
            }
            else if (_inClassMethod && assignmentExpression.Left is MemberExpression me && me.Object is ThisExpression && !me.Computed && me.Property is Identifier pid)
            {
                // this.prop = <expr>
                _il.OpCode(ILOpCode.Ldarg_0); // load 'this'
                var rhsType = _expressionEmitter.Emit(assignmentExpression.Right, new TypeCoercion() { boxResult = true });
                // Lookup field by current class name
                if (string.IsNullOrEmpty(_currentClassName) || !_classRegistry.TryGetField(_currentClassName, pid.Name, out var fieldHandle))
                {
                    throw new NotSupportedException($"Unknown field '{pid.Name}' on class '{_currentClassName}'");
                }
                _il.OpCode(ILOpCode.Stfld);
                _il.Token(fieldHandle);
                return JavascriptType.Object;
            }
            else
            {
                throw new NotSupportedException($"Unsupported assignment target type: {assignmentExpression.Left.Type}");
            }
        }

        // Helper to emit a NewExpression and return its JavaScript type
    private JavascriptType EmitNewExpression(NewExpression newExpression)
        {
            // Support `new Identifier(...)` for classes emitted under Classes namespace
            if (newExpression.Callee is Identifier cid)
            {
                // Try Classes registry first
                if (_classRegistry.TryGet(cid.Name, out var typeHandle) && !typeHandle.IsNil)
                {
                    // Build .ctor signature matching argument count (all object)
                    var argc = newExpression.Arguments.Count;
                    var sig = new BlobBuilder();
                    new BlobEncoder(sig)
                        .MethodSignature(isInstanceMethod: true)
                        .Parameters(argc, r => r.Void(), p => { for (int i = 0; i < argc; i++) p.AddParameter().Type().Object(); });
                    var ctorSig = _metadataBuilder.GetOrAddBlob(sig);
                    var ctorRef = _metadataBuilder.AddMemberReference(typeHandle, _metadataBuilder.GetOrAddString(".ctor"), ctorSig);

                    // Push args
                    for (int i = 0; i < argc; i++)
                    {
                        _expressionEmitter.Emit(newExpression.Arguments[i], new TypeCoercion() { boxResult = true });
                    }
                    _il.OpCode(ILOpCode.Newobj);
                    _il.Token(ctorRef);

                    // Record variable -> class mapping when in a variable initializer or assignment
                    if (!string.IsNullOrEmpty(_currentAssignmentTarget))
                    {
                        _variableToClass[_currentAssignmentTarget] = cid.Name;
                    }
                    return JavascriptType.Object;
                }

                // Built-in Error types from JavaScriptRuntime (Error, TypeError, etc.)
                // Build ctor: choose overload by arg count (we support 0 or 1(param: string) for now)
                var argc2 = newExpression.Arguments.Count;
                if (argc2 > 1)
                {
                    throw new NotSupportedException($"Only up to 1 constructor argument supported for built-in Error types (got {argc2})");
                }
                var ctorRef2 = _runtime.GetErrorCtorRef(cid.Name, argc2);

                // Push args
                for (int i = 0; i < argc2; i++)
                {
                    _expressionEmitter.Emit(newExpression.Arguments[i], new TypeCoercion() { boxResult = true });
                }
                _il.OpCode(ILOpCode.Newobj);
                _il.Token(ctorRef2);
                return JavascriptType.Object;
            }

            throw new NotSupportedException($"Unsupported new-expression callee: {newExpression.Callee.Type}");
        }

        /// <summary>
        /// Emits the IL for a member access expression.
        /// </summary>
        /// <remarks>
        /// A member expression is Object.Property.  Object itself is a expression.
        /// We load the object reference onto the evaluation stack. Then use that as context for property expression.
        /// The harcoded "length" is temporary until we get a proper structure in place for intrinsic objects like the Array.
        /// </remarks>
        private JavascriptType EmitMemberExpression(MemberExpression memberExpression)
        {
            // Handle private instance fields: this.#name inside class methods
            if (!memberExpression.Computed && memberExpression.Object is ThisExpression && memberExpression.Property is Acornima.Ast.PrivateIdentifier ppid)
            {
                if (_inClassMethod && _currentClassName != null && _classRegistry.TryGetPrivateField(_currentClassName, ppid.Name, out var privField))
                {
                    _il.OpCode(ILOpCode.Ldarg_0);
                    _il.OpCode(ILOpCode.Ldfld);
                    _il.Token(privField);
                    return JavascriptType.Object;
                }
            }

            // Special handling: if the object is a known class identifier and property is an identifier,
            // allow static field/method access without evaluating an instance first.
            if (!memberExpression.Computed && memberExpression.Object is Identifier staticBase && memberExpression.Property is Identifier staticProp)
            {
                if (_classRegistry.TryGet(staticBase.Name, out var typeHandle))
                {
                    // Support static field access: ClassName.prop
                    if (_classRegistry.TryGetStaticField(staticBase.Name, staticProp.Name, out var sfield))
                    {
                        _il.OpCode(ILOpCode.Ldsfld);
                        _il.Token(sfield);
                        return JavascriptType.Object;
                    }

                    // Static method invocations are handled in GenerateCallExpression; here we only support fields
                }
            }

            _expressionEmitter.Emit(memberExpression.Object, new TypeCoercion());

            if (!memberExpression.Computed && memberExpression.Property is Identifier propId)
            {
                // First, support array.length
                if (propId.Name == "length")
                {
                    _runtime.InvokeArrayGetCount();
                    return JavascriptType.Number;
                }

                // we currently only support compile time identification of class members
                // this needs to be expanded to support runtime accessing of dynamically added properties
                // this ugly if statement is for 2 cases -
                // this.property and someVar.propery where someVar is a known class type.
                if ((memberExpression.Object is ThisExpression && _inClassMethod && _currentClassName != null && _classRegistry.TryGetField(_currentClassName, propId.Name, out var fieldHandle))
                    || (memberExpression.Object is Identifier baseIdent2 && _variableToClass.TryGetValue(baseIdent2.Name, out var cname) && _classRegistry.TryGetField(cname, propId.Name, out fieldHandle)))
                {
                    // At this point, stack has the instance already. Load its field value.
                    _il.OpCode(ILOpCode.Ldfld);
                    _il.Token(fieldHandle);
                    return JavascriptType.Object;
                }

                // Fallback: dynamic property lookup on runtime object graphs (e.g., ExpandoObject from JSON.parse)
                var getProp = _runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetProperty), typeof(object), typeof(object), typeof(string));
                _il.Ldstr(_metadataBuilder, propId.Name);
                _il.OpCode(ILOpCode.Call);
                _il.Token(getProp);
                return JavascriptType.Object;
            }
            if (memberExpression.Computed)
            {
                // computed means someObject["propertyName"] or someObject[someIndex]
                // arr[expr] -> runtime Object.GetItem(array, doubleIndex)
                var indexType = _expressionEmitter.Emit(memberExpression.Property, new TypeCoercion());
                if (indexType != JavascriptType.Number)
                    // TODO - support strings - object["propertyName"]
                    throw new NotSupportedException("Array index must be numeric expression");
                _runtime.InvokeGetItemFromObject();
                return JavascriptType.Object;
            }
            throw new NotSupportedException("Only 'length', instance fields on known classes, or computed indexing supported.");
        }

        private MethodDefinitionHandle GenerateArrowFunctionMethod(ArrowFunctionExpression arrowFunction, string registryScopeName, string ilMethodName, string[] paramNames)
        {
            // Build method body using a fresh ILMethodGenerator so we never mutate the parent generator's state
            var functionVariables = new Variables(_variables, registryScopeName, paramNames, isNestedFunction: true);
            var pnames = paramNames ?? Array.Empty<string>();
            var childGen = new ILMethodGenerator(functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _dispatchTableGenerator);
            var il = childGen.IL;

            // For arrow functions, we do NOT pre-instantiate a local scope nor initialize parameter fields
            // unless the body actually requires a local scope (e.g., block with declarations). This keeps
            // expression-bodied arrows and simple block-return arrows minimal and matches snapshot baselines.

            // Emit body
            if (arrowFunction.Body is BlockStatement block)
            {
                // Fast-path: handle common pattern `{ const x = <expr>; return x; }`
                if (block.Body.Count == 2 &&
                    block.Body[0] is VariableDeclaration vdecl &&
                    (vdecl.Kind == VariableDeclarationKind.Const || vdecl.Kind == VariableDeclarationKind.Let) &&
                    vdecl.Declarations.Count == 1 &&
                    vdecl.Declarations[0].Id is Identifier vid &&
                    vdecl.Declarations[0].Init is Expression initExpr &&
                    block.Body[1] is ReturnStatement rstmt && rstmt.Argument is Identifier rid && rid.Name == vid.Name)
                {
                    // Optimized pattern: { const x = <expr>; return x; }
                    // If <expr> is a function (arrow/function expression), we must bind closure scopes before returning.
                    bool returnsFunctionInitializer = initExpr is ArrowFunctionExpression || initExpr is FunctionExpression;

                    if (returnsFunctionInitializer)
                    {
                        // If returning a function, ensure this arrow's local scope exists and parameter fields are initialized
                        var registry = functionVariables.GetVariableRegistry();
                        if (registry != null)
                        {
                            var fields = registry.GetVariablesForScope(registryScopeName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
                            var hasAnyFields = fields.Any();
                            if (hasAnyFields)
                            {
                                ScopeInstanceEmitter.EmitCreateLeafScopeInstance(functionVariables, il, _metadataBuilder);
                                // Initialize parameter fields if backing fields exist
                                var localScope = functionVariables.GetLocalScopeSlot();
                                if (localScope.Address >= 0 && pnames.Length > 0)
                                {
                                    var fieldNames = new HashSet<string>(fields.Select(f => f.Name));
                                    ushort jsParamSeq = 1; // arg0 is scopes[]
                                    foreach (var pn in pnames)
                                    {
                                        if (fieldNames.Contains(pn))
                                        {
                                            il.LoadLocal(localScope.Address);
                                            il.LoadArgument(jsParamSeq);
                                            var fh = registry.GetFieldHandle(registryScopeName, pn);
                                            il.OpCode(ILOpCode.Stfld);
                                            il.Token(fh);
                                        }
                                        jsParamSeq++;
                                    }
                                }
                            }
                        }

                        // Emit the initializer expression to produce the delegate on the stack
                        // Make sure inner arrow uses assignment-target-based registry naming (e.g., ArrowFunction_inner)
                        var prevAssignment = childGen._currentAssignmentTarget;
                        childGen._currentAssignmentTarget = vid.Name;
                        try
                        {
                            _ = ((IMethodExpressionEmitter)childGen).Emit(initExpr, new TypeCoercion());
                        }
                        finally
                        {
                            childGen._currentAssignmentTarget = prevAssignment;
                        }

                        // Build scopes[] to bind for closure: prefer [global(if any), this local]
                        var innerVar = functionVariables.FindVariable(vid.Name);
                        if (innerVar != null)
                        {
                            var neededScopeNames = childGen.GetScopesForClosureBinding(innerVar).ToList();
                            il.EmitNewArray(neededScopeNames.Count, _bclReferences.ObjectType, (eil, idx) =>
                            {
                                var sn = neededScopeNames[idx];
                                var refSlot = functionVariables.GetScopeLocalSlot(sn);
                                if (refSlot.Location == ObjectReferenceLocation.Local)
                                {
                                    eil.LoadLocal(refSlot.Address);
                                }
                                else if (refSlot.Location == ObjectReferenceLocation.Parameter)
                                {
                                    eil.LoadArgument(refSlot.Address);
                                }
                                else if (refSlot.Location == ObjectReferenceLocation.ScopeArray)
                                {
                                    eil.LoadArgument(0);
                                    eil.LoadConstantI4(refSlot.Address);
                                    eil.OpCode(ILOpCode.Ldelem_ref);
                                }
                            });
                            // Bind the delegate on stack to the scopes[] we just built
                            childGen._runtime.InvokeClosureBindObject();
                        }
                        il.OpCode(ILOpCode.Ret);
                    }
                    else
                    {
                        // Not returning a function; just evaluate the expression and return it (no binding, no scope instantiation)
                        _ = ((IMethodExpressionEmitter)childGen).Emit(initExpr, new TypeCoercion());
                        il.OpCode(ILOpCode.Ret);
                    }
                }
                else
                {
                    // General fallback: emit statements; ensure a return at end if missing
                    // If the block declares any let/const or uses fields, create a local scope instance now
                    var registry = functionVariables.GetVariableRegistry();
                    if (registry != null)
                    {
                        var fields = registry.GetVariablesForScope(registryScopeName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
                        var hasAnyFields = fields.Any();
                        if (hasAnyFields)
                        {
                            // Create the current arrow function scope instance
                            ScopeInstanceEmitter.EmitCreateLeafScopeInstance(functionVariables, il, _metadataBuilder);

                            // Initialize parameter fields from CLR args when a backing field exists
                            var localScope = functionVariables.GetLocalScopeSlot();
                            if (localScope.Address >= 0 && pnames.Length > 0)
                            {
                                var fieldNames = new HashSet<string>(fields.Select(f => f.Name));
                                ushort jsParamSeq = 1; // arg0 is scopes[]; JS params start at 1
                                foreach (var pn in pnames)
                                {
                                    if (fieldNames.Contains(pn))
                                    {
                                        il.LoadLocal(localScope.Address);
                                        il.LoadArgument(jsParamSeq);
                                        var fh = registry.GetFieldHandle(registryScopeName, pn);
                                        il.OpCode(ILOpCode.Stfld);
                                        il.Token(fh);
                                    }
                                    jsParamSeq++;
                                }
                            }
                        }
                    }

                    // Emit statements using the child generator
                    childGen.GenerateStatementsForBody(functionVariables.GetLeafScopeName(), false, block.Body);
                    // If no explicit return executed, fall through and return null
                    il.OpCode(ILOpCode.Ldnull);
                    il.OpCode(ILOpCode.Ret);
                }
            }
            else
            {
                // Expression-bodied arrow: evaluate via the child generator's expression emitter to keep logic isolated
                var bodyExpr = arrowFunction.Body as Expression ?? throw new NotSupportedException("Arrow function body is not an expression");
                _ = ((IMethodExpressionEmitter)childGen).Emit(bodyExpr, new TypeCoercion());
                il.OpCode(ILOpCode.Ret);
            }

            // Locals signature
            StandaloneSignatureHandle localSignature = default;
            MethodBodyAttributes bodyAttributes = MethodBodyAttributes.None;
            var localCount = functionVariables.GetNumberOfLocals();
            if (localCount > 0)
            {
                var localSig = new BlobBuilder();
                var localEncoder = new BlobEncoder(localSig).LocalVariableSignature(localCount);
                for (int i = 0; i < localCount; i++)
                {
                    localEncoder.AddVariable().Type().Object();
                }
                localSignature = _metadataBuilder.AddStandaloneSignature(_metadataBuilder.GetOrAddBlob(localSig));
                bodyAttributes = MethodBodyAttributes.InitLocals;
            }

            var bodyOffset = _methodBodyStreamEncoder.AddMethodBody(
                il,
                localVariablesSignature: localSignature,
                attributes: bodyAttributes);

            // Build method signature: static object (object[] scopes, object p1, ...)
            var sigBuilder = new BlobBuilder();
            var paramCount = 1 + pnames.Length;
            new BlobEncoder(sigBuilder)
                .MethodSignature()
                .Parameters(paramCount, returnType => returnType.Type().Object(), parameters =>
                {
                    parameters.AddParameter().Type().SZArray().Object();
                    for (int i = 0; i < pnames.Length; i++) parameters.AddParameter().Type().Object();
                });
            var methodSig = _metadataBuilder.GetOrAddBlob(sigBuilder);

            // Add parameter metadata
            var firstParam = _metadataBuilder.AddParameter(ParameterAttributes.None, _metadataBuilder.GetOrAddString("scopes"), sequenceNumber: 1);
            ushort seq = 2;
            foreach (var p in pnames)
            {
                _metadataBuilder.AddParameter(ParameterAttributes.None, _metadataBuilder.GetOrAddString(p), sequenceNumber: seq++);
            }

            // Host the arrow method on its own type under Functions namespace
            var tb = new Js2IL.Utilities.Ecma335.TypeBuilder(_metadataBuilder, "Functions", ilMethodName);
            var mdh = tb.AddMethodDefinition(MethodAttributes.Static | MethodAttributes.Public, ilMethodName, methodSig, bodyOffset, firstParam);
            tb.AddTypeDefinition(TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, _bclReferences.ObjectType);
            return mdh;
        }

        /// <summary>
        /// Determines which scopes are needed for a specific function call.
        /// Rules:
        /// - In Main (no scope-array parameter): pass the current (leaf) scope instance.
        /// - In a function context (scope-array present): pass only the caller's local scope that holds the callee delegate.
        ///   Historical snapshots do not include the global scope alongside the caller local for nested calls.
        /// </summary>
        private enum CallSiteContext { Statement, Expression }

        private IEnumerable<string> GetNeededScopesForFunction(Variable functionVariable, CallSiteContext context)
        {
            var names = _variables.GetAllScopeNames().ToList();
            var slots = names.Select(n => new { Name = n, Slot = _variables.GetScopeLocalSlot(n) }).ToList();

            bool inFunctionContext = slots.Any(e => e.Slot.Location == ObjectReferenceLocation.ScopeArray);
            if (!inFunctionContext)
            {
                // Main: pass only the current (leaf) scope
                var globalName = _variables.GetLeafScopeName();
                yield return globalName;
                yield break;
            }

            // Inside a function: include exactly the scope that owns the callee variable
            // - If the callee is stored on the current local scope: include that local scope only.
            // - If the callee lives on a parent/global scope: include that parent/global scope only.
            if (!string.IsNullOrEmpty(functionVariable.ScopeName))
            {
                var targetSlot = _variables.GetScopeLocalSlot(functionVariable.ScopeName);
                if (targetSlot.Location == ObjectReferenceLocation.Local)
                {
                    // Nested function declared in the current local scope
                    // Statement context snapshots include [global, local]; expression context snapshots include only [local]
                    if (context == CallSiteContext.Statement)
                    {
                        var globalEntry = slots.FirstOrDefault(e => e.Slot.Location == ObjectReferenceLocation.ScopeArray && e.Slot.Address == 0);
                        if (globalEntry != null)
                        {
                            yield return globalEntry.Name; // global first
                        }
                    }
                    yield return functionVariable.ScopeName; // local (caller) scope
                }
                else if (targetSlot.Location == ObjectReferenceLocation.ScopeArray ||
                         targetSlot.Location == ObjectReferenceLocation.Parameter)
                {
                    // Callee lives on a parent/global scope: include only that owning scope
                    yield return functionVariable.ScopeName;
                }
            }
        }

        private static void CollectDeclaredNames(Node node, HashSet<string> declared)
        {
            if (node is VariableDeclaration vardecl)
            {
                foreach (var d in vardecl.Declarations)
                {
                    if (d.Id is Identifier id)
                    {
                        declared.Add(id.Name);
                    }
                }
            }
            else if (node is FunctionDeclaration fdecl)
            {
                if (fdecl.Id is Identifier fid) declared.Add(fid.Name);
                // Do not traverse into nested function bodies for declaration collection beyond adding its name
                return;
            }

            // Recurse over children
            var props = node.GetType().GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(node);
                if (value is Node child)
                {
                    // Skip into nested functions when collecting declarations? We already marked their name; no need to descend
                    if (child is FunctionDeclaration) continue;
                    CollectDeclaredNames(child, declared);
                }
                else if (value is System.Collections.IEnumerable enumerable && value is not string)
                {
                    foreach (var item in enumerable)
                    {
                        if (item is Node childNode)
                        {
                            if (childNode is FunctionDeclaration) continue;
                            CollectDeclaredNames(childNode, declared);
                        }
                    }
                }
            }
        }

        private static bool ContainsGlobalRef(Node node, HashSet<string> globals, HashSet<string> declared)
        {
            // If Identifier and not declared locally/params, treat as potential reference
            if (node is Identifier id)
            {
                if (globals.Contains(id.Name) && !declared.Contains(id.Name))
                {
                    return true;
                }
            }
            else if (node is MemberExpression mex)
            {
                // Visit the object side; skip non-computed property identifiers since they are not variable refs
                if (ContainsGlobalRef(mex.Object, globals, declared)) return true;
                if (mex.Computed && ContainsGlobalRef(mex.Property, globals, declared)) return true;
                return false;
            }
            else if (node is FunctionDeclaration)
            {
                // Do not look into nested function bodies
                return false;
            }

            var props = node.GetType().GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(node);
                if (value is Node child)
                {
                    if (ContainsGlobalRef(child, globals, declared)) return true;
                }
                else if (value is System.Collections.IEnumerable enumerable && value is not string)
                {
                    foreach (var item in enumerable)
                    {
                        if (item is Node childNode)
                        {
                            if (ContainsGlobalRef(childNode, globals, declared)) return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool ContainsNestedFunction(Acornima.Ast.Node node)
        {
            // Walk child nodes; if any FunctionDeclaration is found, return true
            if (node is Acornima.Ast.FunctionDeclaration)
            {
                return true;
            }

            var props = node.GetType().GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(node);
                if (value is Acornima.Ast.Node child)
                {
                    if (ContainsNestedFunction(child)) return true;
                }
                else if (value is System.Collections.IEnumerable enumerable && value is not string)
                {
                    foreach (var item in enumerable)
                    {
                        if (item is Acornima.Ast.Node childNode)
                        {
                            if (ContainsNestedFunction(childNode)) return true;
                        }
                    }
                }
            }
            return false;
        }
        
        /// <summary>
        /// for loading literal expresions onto the IL stack.
        /// i.e. 
        /// x = 5;
        /// x = "hello world";
        /// x = true;
        /// </summary>
        private JavascriptType LoadValue(Expression expression, TypeCoercion typeCoercion)
        {
            JavascriptType type = JavascriptType.Unknown;

            switch (expression)
            {
                case Acornima.Ast.BooleanLiteral booleanLiteral:
                    if (typeCoercion.toString)
                    {
                        _il.Ldstr(_metadataBuilder, booleanLiteral.Value ? "true" : "false");
                        // treat as object/string in this coercion path
                        type = JavascriptType.Object;
                    }
                    else
                    {
                        _il.LoadConstantI4(booleanLiteral.Value ? 1 : 0); // Load boolean literal
                        type = JavascriptType.Boolean;
                    }
                    break;
                case Acornima.Ast.NumericLiteral numericLiteral:
                    if (typeCoercion.toString)
                    {
                        //does dotnet ToString behave the same as JavaScript?
                        var numberAsString = numericLiteral.Value.ToString();
                        _il.Ldstr(_metadataBuilder, numberAsString); // Load numeric literal as string
                    }
                    else
                    {
                        _il.LoadConstantR8(numericLiteral.Value); // Load numeric literal
                    }

                    type = JavascriptType.Number;

                    break;
                case Acornima.Ast.StringLiteral stringLiteral:
                    _il.Ldstr(_metadataBuilder, stringLiteral.Value); // Load string literal
                    break;
                case Acornima.Ast.Literal genericLiteral:
                    // Some literals (especially booleans/null) may come through the generic Literal node
                    if (genericLiteral.Value is bool b)
                    {
                        if (typeCoercion.toString)
                        {
                            _il.Ldstr(_metadataBuilder, b ? "true" : "false");
                            type = JavascriptType.Object;
                        }
                        else
                        {
                            _il.LoadConstantI4(b ? 1 : 0);
                            type = JavascriptType.Boolean;
                        }
                        break;
                    }
                    if (genericLiteral.Value is null)
                    {
                        // JavaScript 'null'
                        _il.OpCode(ILOpCode.Ldnull);
                        type = JavascriptType.Null;
                        break;
                    }
                    throw new NotSupportedException($"Unsupported literal value type: {genericLiteral.Value?.GetType().Name ?? "null"}");
                default:
                    throw new NotSupportedException($"Unsupported expression type: {expression.Type}");
            }

            return type;
        }

    }
}
