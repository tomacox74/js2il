using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Acornima;
using Acornima.Ast;

namespace Js2IL.Services.ILGenerators
{
    internal class ILMethodGenerator
    {
        private Variables _variables;
        private BaseClassLibraryReferences _bclReferences;
        private MetadataBuilder _metadataBuilder;
        private InstructionEncoder _il;
        private ControlFlowBuilder _cfb;
        private Runtime _runtime;
        private Dispatch.DispatchTableGenerator _dispatchTableGenerator;
        private MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private IMethodExpressionEmitter _expressionEmitter;
        private ClassRegistry _classRegistry;
        private bool _inClassMethod;
        private string? _currentClassName;
        private string? _currentAssignmentTarget;
        private readonly Dictionary<string, string> _variableToClass = new();

        private readonly struct LoopContext
        {
            public readonly LabelHandle ContinueLabel;
            public readonly LabelHandle BreakLabel;
            public LoopContext(LabelHandle continueLabel, LabelHandle breakLabel)
            {
                ContinueLabel = continueLabel;
                BreakLabel = breakLabel;
            }
        }

        private readonly Stack<LoopContext> _loopStack = new();

        /*
        * Temporary exposure of private members until refactoring gets cleaner
        * need to determine what the difference is between generating the main method and generating any generic method
        */
        public Variables Variables => _variables;
        public MetadataBuilder MetadataBuilder => _metadataBuilder;
        public InstructionEncoder IL => _il;
        // Expose limited internals to the expression generator
        internal BaseClassLibraryReferences BclReferences => _bclReferences;
        internal Runtime Runtime => _runtime;
        internal bool InClassMethod => _inClassMethod;
        internal string? CurrentClassName => _currentClassName;
        internal string? CurrentAssignmentTarget { get => _currentAssignmentTarget; set => _currentAssignmentTarget = value; }
        internal ClassRegistry ClassRegistry => _classRegistry;
        internal IMethodExpressionEmitter ExpressionEmitter => _expressionEmitter;

    public ILMethodGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, Dispatch.DispatchTableGenerator dispatchTableGenerator, ClassRegistry? classRegistry = null, bool inClassMethod = false, string? currentClassName = null)
        {
            _variables = variables;
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            var methodIl = new BlobBuilder();
            _cfb = new ControlFlowBuilder();
            _il = new InstructionEncoder(methodIl, _cfb);
            this._runtime = new Runtime(metadataBuilder, _il);
            // Use a dedicated expression generator to avoid circular logic and enable incremental refactors
            this._expressionEmitter = new ILExpressionGenerator(this);

            _methodBodyStreamEncoder = methodBodyStreamEncoder;
            _dispatchTableGenerator = dispatchTableGenerator ?? throw new ArgumentNullException(nameof(dispatchTableGenerator));
            _classRegistry = classRegistry ?? new ClassRegistry();
            _inClassMethod = inClassMethod;
            _currentClassName = currentClassName;
        }

    // Allow expression generator to record variable->class mapping when emitting `new ClassName()` in assignments/initializers
        internal void RecordVariableToClass(string variableName, string className)
        {
            if (!string.IsNullOrEmpty(variableName) && !string.IsNullOrEmpty(className))
            {
                _variableToClass[variableName] = className;
            }
        }

        // Allow expression generator to query variable->class association for instance member access
        internal bool TryGetVariableClass(string variableName, out string className)
        {
            return _variableToClass.TryGetValue(variableName, out className!);
        }

    // GetPropertyIgnoreCase moved to ILExpressionGenerator
    // ParseRegexRaw moved to ILExpressionGenerator

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
                    var initResult = this._expressionEmitter.Emit(variableAST.Init, new TypeCoercion() { boxResult = true });
                    variable.Type = initResult.JsType;
                    variable.RuntimeIntrinsicType = initResult.ClrType;
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
            _ = _expressionEmitter.Emit(expressionStatement.Expression, new TypeCoercion(), CallSiteContext.Statement);
            if (expressionStatement.Expression is NewExpression || expressionStatement.Expression is ConditionalExpression || expressionStatement.Expression is CallExpression)
            {
                _il.OpCode(ILOpCode.Pop);
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
            _expressionEmitter.Emit(whileStatement.Test, new TypeCoercion(), CallSiteContext.Expression, new ConditionalBranching
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
            _expressionEmitter.Emit(doWhileStatement.Test, new TypeCoercion(), CallSiteContext.Expression, new ConditionalBranching
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
            _expressionEmitter.Emit(ifStatement.Test, new TypeCoercion(), CallSiteContext.Expression, new ConditionalBranching
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

    // TryEmitIntrinsicInstanceCall moved to ILExpressionGenerator


        // Emits: throw new TypeError("Assignment to constant variable.")
        internal void EmitConstReassignmentTypeError()
        {
            var ctor = _runtime.GetErrorCtorRef("TypeError", 1);
            _il.EmitThrowError(_metadataBuilder, ctor, "Assignment to constant variable.");
        }

    // TryEmitHostIntrinsicStaticCall moved to ILExpressionGenerator

        internal MethodDefinitionHandle GenerateArrowFunctionMethod(ArrowFunctionExpression arrowFunction, string registryScopeName, string ilMethodName, string[] paramNames)
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
                            _ = childGen.ExpressionEmitter.Emit(initExpr, new TypeCoercion());
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
                        // Not returning a function; evaluate and box primitives so the object return type is satisfied
                        _ = childGen.ExpressionEmitter.Emit(initExpr, new TypeCoercion() { boxResult = true });
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
                _ = childGen.ExpressionEmitter.Emit(bodyExpr, new TypeCoercion() { boxResult = true });
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

        internal MethodDefinitionHandle GenerateFunctionExpressionMethod(FunctionExpression funcExpr, string registryScopeName, string ilMethodName, string[] paramNames)
        {
            var functionVariables = new Variables(_variables, registryScopeName, paramNames, isNestedFunction: true);
            var pnames = paramNames ?? Array.Empty<string>();
            var childGen = new ILMethodGenerator(functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _dispatchTableGenerator);
            var il = childGen.IL;

            // Function expressions use block bodies; create local scope if fields exist and init parameter fields
            var registry = functionVariables.GetVariableRegistry();
            if (registry != null)
            {
                var fields = registry.GetVariablesForScope(registryScopeName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
                if (fields.Any())
                {
                    ScopeInstanceEmitter.EmitCreateLeafScopeInstance(functionVariables, il, _metadataBuilder);
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

            if (funcExpr.Body is BlockStatement block)
            {
                childGen.GenerateStatementsForBody(functionVariables.GetLeafScopeName(), false, block.Body);
                // If control reaches here with no explicit return, return null
                il.OpCode(ILOpCode.Ldnull);
                il.OpCode(ILOpCode.Ret);
            }
            else
            {
                // Function expressions are expected to have block bodies in our parser; return undefined
                il.OpCode(ILOpCode.Ldnull);
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

            // Host the function expression method on its own type under Functions namespace
            var tb = new Js2IL.Utilities.Ecma335.TypeBuilder(_metadataBuilder, "Functions", ilMethodName);
            var mdh = tb.AddMethodDefinition(MethodAttributes.Static | MethodAttributes.Public, ilMethodName, methodSig, bodyOffset, firstParam);
            tb.AddTypeDefinition(TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, _bclReferences.ObjectType);
            return mdh;
        }

    // GetNeededScopesForFunction moved to ILExpressionGenerator

    internal static void CollectDeclaredNames(Node node, HashSet<string> declared)
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

    internal static bool ContainsGlobalRef(Node node, HashSet<string> globals, HashSet<string> declared)
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

    internal static bool ContainsNestedFunction(Acornima.Ast.Node node)
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
        
    // LoadValue moved to ILExpressionGenerator

    }
}
