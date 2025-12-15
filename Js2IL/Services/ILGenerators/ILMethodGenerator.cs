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
    internal partial class ILMethodGenerator
    {
        private Variables _variables;
        private BaseClassLibraryReferences _bclReferences;
        private MetadataBuilder _metadataBuilder;
        private InstructionEncoder _il;
        private ControlFlowBuilder _cfb;
        private Runtime _runtime;
        // Dispatch table removed; functions bound directly to static method delegates.
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
        internal FunctionRegistry? FunctionRegistry => _functionRegistry;

        private readonly FunctionRegistry? _functionRegistry;

        public ILMethodGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, ClassRegistry? classRegistry = null, FunctionRegistry? functionRegistry = null, bool inClassMethod = false, string? currentClassName = null)
        {
            _variables = variables;
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            var methodIl = new BlobBuilder();
            _cfb = new ControlFlowBuilder();
            _il = new InstructionEncoder(methodIl, _cfb);
            this._runtime = new Runtime(_il, bclReferences.TypeRefRegistry, bclReferences.MemberRefRegistry);
            // Use a dedicated expression generator to avoid circular logic and enable incremental refactors
            this._expressionEmitter = new ILExpressionGenerator(this);

            _methodBodyStreamEncoder = methodBodyStreamEncoder;
            _classRegistry = classRegistry ?? new ClassRegistry();
            _functionRegistry = functionRegistry;
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

        // Allow expression generator to register arrow functions with parameter count
        internal void RegisterFunction(string name, MethodDefinitionHandle handle, int paramCount)
        {
            _functionRegistry?.Register(name, handle, paramCount);
        }

        // GetPropertyIgnoreCase moved to ILExpressionGenerator
        // ParseRegexRaw moved to ILExpressionGenerator

        public void DeclareVariable(VariableDeclaration variableDeclaraion)
        {
            // Handle one declarator at a time; support destructuring and simple identifiers
            var variableAST = variableDeclaraion.Declarations.FirstOrDefault()!;

            // Object destructuring: const { a } = <init>;
            if (variableAST.Id is Acornima.Ast.ObjectPattern objPattern)
            {
                if (variableAST.Init == null)
                {
                    ILEmitHelpers.ThrowNotSupported("Object destructuring without initializer is not supported", variableAST);
                }

                // Choose a synthetic temp field name that SymbolTable added. Prefer 'perf' for perf_hooks pattern when available.
                string tempName = "perf";
                var tempVar = _variables.FindVariable(tempName);
                if (tempVar == null)
                {
                    tempName = "__obj";
                    tempVar = _variables.FindVariable(tempName);
                }
                if (tempVar == null)
                {
                    // As a fallback, pick the first property name as a valid binding scope to host the init temporarily
                    // though snapshots expect 'perf' for perf_hooks, so this path should rarely execute.
                    throw new InvalidOperationException("Destructuring temp binding not found (expected 'perf' or '__obj')");
                }

                // Check if temp is an uncaptured variable (uses local slot instead of field)
                bool tempIsLocal = tempVar.LocalSlot >= 0;
                var tempScopeSlot = _variables.GetScopeLocalSlot(tempVar.ScopeName);
                var tempScopeType = _variables.GetVariableRegistry()?.GetScopeTypeHandle(tempVar.ScopeName) ?? default;

                // 1) temp = <init>;
                if (!tempIsLocal)
                {
                    // Field-backed: load scope instance for stfld
                    if (tempScopeSlot.Address == -1) throw new InvalidOperationException($"Scope '{tempVar.ScopeName}' not found in local slots");
                    if (tempScopeSlot.Location == ObjectReferenceLocation.Parameter) _il.LoadArgument(tempScopeSlot.Address);
                    else if (tempScopeSlot.Location == ObjectReferenceLocation.ScopeArray) { _il.LoadArgument(0); _il.LoadConstantI4(tempScopeSlot.Address); _il.OpCode(ILOpCode.Ldelem_ref); }
                    else _il.LoadLocal(tempScopeSlot.Address);
                    // Cast to scope type for verifiable field store
                    if (!tempScopeType.IsNil) { _il.OpCode(ILOpCode.Castclass); _il.Token(tempScopeType); }
                }

                var prevAssignmentTarget = _currentAssignmentTarget;
                try
                {
                    _currentAssignmentTarget = tempName;
                    var initRes = _expressionEmitter.Emit(variableAST.Init, new TypeCoercion() { boxResult = true });
                    tempVar.Type = initRes.JsType;
                    if (!tempVar.IsStableType)
                    {
                        tempVar.ClrType = initRes.ClrType;
                    }
                    try { _variables.GetVariableRegistry()?.SetClrType(tempVar.ScopeName, tempVar.Name, initRes.ClrType); } catch { }
                }
                finally { _currentAssignmentTarget = prevAssignmentTarget; }

                // Store temp value
                if (tempIsLocal)
                {
                    // Store to local variable slot
                    _il.StoreLocal(tempVar.LocalSlot);
                }
                else
                {
                    // Store to field
                    _il.OpCode(ILOpCode.Stfld);
                    _il.Token(tempVar.FieldHandle);
                }

                // 2) For each property: name = temp.<prop> (via direct getter when known, else runtime Object.GetProperty)
                foreach (var p in objPattern.Properties)
                {
                    if (p is Acornima.Ast.Property prop && prop.Value is Acornima.Ast.Identifier bid)
                    {
                        var targetVar = _variables.FindVariable(bid.Name) ?? throw new InvalidOperationException($"Variable '{bid.Name}' not found.");
                        bool targetIsLocal = targetVar.LocalSlot >= 0;

                        if (!targetIsLocal)
                        {
                            // Load target scope for stfld
                            var tslot = _variables.GetScopeLocalSlot(targetVar.ScopeName);
                            if (tslot.Location == ObjectReferenceLocation.Parameter) _il.LoadArgument(tslot.Address);
                            else if (tslot.Location == ObjectReferenceLocation.ScopeArray) { _il.LoadArgument(0); _il.LoadConstantI4(tslot.Address); _il.OpCode(ILOpCode.Ldelem_ref); }
                            else _il.LoadLocal(tslot.Address);
                            var tScopeType = _variables.GetVariableRegistry()?.GetScopeTypeHandle(targetVar.ScopeName) ?? default;
                            if (!tScopeType.IsNil) { _il.OpCode(ILOpCode.Castclass); _il.Token(tScopeType); }
                        }

                        // Load temp value (either from local or field)
                        if (tempIsLocal)
                        {
                            _il.LoadLocal(tempVar.LocalSlot);
                        }
                        else
                        {
                            // Load temp receiver from scope field
                            if (tempScopeSlot.Location == ObjectReferenceLocation.Parameter) _il.LoadArgument(tempScopeSlot.Address);
                            else if (tempScopeSlot.Location == ObjectReferenceLocation.ScopeArray) { _il.LoadArgument(0); _il.LoadConstantI4(tempScopeSlot.Address); _il.OpCode(ILOpCode.Ldelem_ref); }
                            else _il.LoadLocal(tempScopeSlot.Address);
                            // Cast to concrete scope type for verifiable ldfld
                            if (!tempScopeType.IsNil)
                            {
                                _il.OpCode(ILOpCode.Castclass);
                                _il.Token(tempScopeType);
                            }
                            _il.OpCode(ILOpCode.Ldfld);
                            _il.Token(tempVar.FieldHandle);
                        }

                        // If tempVar has a known runtime intrinsic type with a matching property getter, prefer that
                        var rtType = tempVar.ClrType;
                        bool emittedDirectGetter = false;
                        if (rtType != null)
                        {
                            var propName = (prop.Key as Acornima.Ast.Identifier)?.Name ?? (prop.Key as Acornima.Ast.Literal)?.Value?.ToString();
                            if (!string.IsNullOrEmpty(propName))
                            {
                                var clrProp = rtType.GetProperty(propName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreCase);
                                if (clrProp?.GetMethod != null)
                                {
                                    var mref = _runtime.GetInstanceMethodRef(rtType, clrProp.GetMethod.Name, clrProp.PropertyType);
                                    _il.OpCode(ILOpCode.Callvirt);
                                    _il.Token(mref);
                                    // Record the runtime intrinsic CLR type for the target variable so downstream member calls can bind directly
                                    if (!targetVar.IsStableType)
                                    {
                                        targetVar.ClrType = clrProp.PropertyType;
                                    }
                                    try { _variables.GetVariableRegistry()?.SetClrType(targetVar.ScopeName, targetVar.Name, clrProp.PropertyType); } catch { }
                                    emittedDirectGetter = true;
                                }
                            }
                        }
                        if (!emittedDirectGetter)
                        {
                            // Fallback to runtime Object.GetProperty(temp, name)
                            var propName = (prop.Key as Acornima.Ast.Identifier)?.Name ?? (prop.Key as Acornima.Ast.Literal)?.Value?.ToString() ?? string.Empty;
                            _il.Ldstr(_metadataBuilder, propName);
                            var getPropRef = _runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetProperty), typeof(object), typeof(object), typeof(string));
                            _il.OpCode(ILOpCode.Call);
                            _il.Token(getPropRef);
                        }

                        // Store target value
                        if (targetIsLocal)
                        {
                            // Store to local variable slot
                            _il.StoreLocal(targetVar.LocalSlot);
                        }
                        else
                        {
                            // Store to field (the target scope instance was already loaded and cast before value evaluation)
                            _il.OpCode(ILOpCode.Stfld);
                            _il.Token(targetVar.FieldHandle);
                        }
                    }
                }
                return;
            }

            // Simple identifier declaration path (original)
            var variableName = (variableAST.Id as Acornima.Ast.Identifier)!.Name;
            var variable = _variables.FindVariable(variableName) ?? throw new InvalidOperationException($"Variable '{variableName}' not found.");

            if (variableAST.Init != null)
            {
                // Check if this is an uncaptured variable (stored as local, not field)
                bool isLocalVariable = variable.LocalSlot >= 0;

                // Only load scope if it's a field variable
                if (!isLocalVariable)
                {
                    var scopeLocalIndex = _variables.GetScopeLocalSlot(variable.ScopeName);
                    if (scopeLocalIndex.Address == -1)
                    {
                        throw new InvalidOperationException($"Scope '{variable.ScopeName}' not found in local slots");
                    }

                    // Load scope instance onto stack
                    if (scopeLocalIndex.Location == ObjectReferenceLocation.Parameter)
                    {
                        _il.LoadArgument(scopeLocalIndex.Address);
                        // Cast needed: parameter is typed as object
                        var regVar = _variables.GetVariableRegistry();
                        var tdefVar = regVar?.GetScopeTypeHandle(variable.ScopeName) ?? default;
                        if (!tdefVar.IsNil)
                        {
                            _il.OpCode(ILOpCode.Castclass);
                            _il.Token(tdefVar);
                        }
                    }
                    else if (scopeLocalIndex.Location == ObjectReferenceLocation.ScopeArray)
                    {
                        _il.LoadArgument(0);
                        _il.LoadConstantI4(scopeLocalIndex.Address);
                        _il.OpCode(ILOpCode.Ldelem_ref);
                        // Cast needed: array element is typed as object
                        var regVar = _variables.GetVariableRegistry();
                        var tdefVar = regVar?.GetScopeTypeHandle(variable.ScopeName) ?? default;
                        if (!tdefVar.IsNil)
                        {
                            _il.OpCode(ILOpCode.Castclass);
                            _il.Token(tdefVar);
                        }
                    }
                    else
                    {
                        _il.LoadLocal(scopeLocalIndex.Address);
                    }
                }

                // Evaluate initializer and leave value on stack
                var prevAssignmentTarget2 = _currentAssignmentTarget;
                try
                {
                    _currentAssignmentTarget = variableName;
                    var boxResult = variable.ClrType != typeof(double);
                    var initResult = this._expressionEmitter.Emit(variableAST.Init, new TypeCoercion() { boxResult = boxResult });
                    variable.Type = initResult.JsType;
                    if (!variable.IsStableType)
                    {
                        // unstable types are variables which can change types during their lifetime
                        //  i.e. first a number, then a string.. etc..
                        variable.ClrType = initResult.ClrType;
                    }
                    // else if stable type we are trusting that the type inference logic was correct

                    try { _variables.GetVariableRegistry()?.SetClrType(variable.ScopeName, variableName, initResult.ClrType); } catch { }
                }
                finally { _currentAssignmentTarget = prevAssignmentTarget2; }

                // Store value into variable using helper
                // For local variables: Stack is [value]
                // For field variables: Stack is [scope instance, value]
                _il.EmitStoreVariable(variable, _variables, scopeAlreadyLoaded: !isLocalVariable);
            }
        }

        public void GenerateStatementsForBody(string scopeName, bool createScopeInstance, NodeList<Statement> statements)
        {
            int? createdLocalIndex = null;
            bool lexicalScopePushed = false;
            
            if (createScopeInstance)
            {
                var registry = _variables.GetVariableRegistry();
                if (registry != null)
                {
                    // Check if this scope actually has any field-backed variables
                    // If all variables are uncaptured (use locals), we don't need a scope instance
                    bool hasFieldBackedVariables = registry.ScopeHasFieldBackedVariables(scopeName);
                    
                    if (hasFieldBackedVariables)
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

                            // For the function's own scope, use CreateScopeInstance so it maps to local 0
                            var scopeRef = _variables.CreateScopeInstance(scopeName);
                            createdLocalIndex = scopeRef.Address >= 0 ? scopeRef.Address : (int?)null;
                            if (createdLocalIndex.HasValue)
                            {
                                _il.OpCode(ILOpCode.Newobj);
                                _il.Token(ctorRef);
                                _il.StoreLocal(createdLocalIndex.Value);
                            }
                        }
                    }
                    
                    // Always track lexical scope for variable resolution (shadowing), even if no instance created
                    _variables.PushLexicalScope(scopeName);
                    lexicalScopePushed = true;
                }
            }

            // Iterate through each statement in the block
            foreach (var statement in statements.Where(s => s is not FunctionDeclaration))
            {
                GenerateStatement(statement);
            }

            if (lexicalScopePushed)
            {
                _variables.PopLexicalScope(scopeName);
                // Do not emit any instructions after potential explicit returns; rely on GC to collect the scope instance.
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
            // Resolve emitted method for this function via function registry
            var methodHandle = _functionRegistry?.Get(functionName) ?? default;
            if (methodHandle.IsNil)
            {
                // If not found, skip (leave null) – may be emitted later (nested ordering). TODO: revisit ordering guarantees.
            }

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
                // Cast needed: parameter is typed as object
                var regF = _variables.GetVariableRegistry();
                var tdefF = regF?.GetScopeTypeHandle(functionVariable.ScopeName) ?? default;
                if (!tdefF.IsNil)
                {
                    _il.OpCode(ILOpCode.Castclass);
                    _il.Token(tdefF);
                }
            }
            else if (scopeLocalIndex.Location == ObjectReferenceLocation.ScopeArray)
            {
                _il.LoadArgument(0); // Load scope array parameter
                _il.LoadConstantI4(scopeLocalIndex.Address); // Load array index
                _il.OpCode(ILOpCode.Ldelem_ref); // Load scope from array
                // Cast needed: array element is typed as object
                var regF = _variables.GetVariableRegistry();
                var tdefF = regF?.GetScopeTypeHandle(functionVariable.ScopeName) ?? default;
                if (!tdefF.IsNil)
                {
                    _il.OpCode(ILOpCode.Castclass);
                    _il.Token(tdefF);
                }
            }
            else
            {
                _il.LoadLocal(scopeLocalIndex.Address);
            }
            if (!methodHandle.IsNil)
            {
                // Create delegate: ldnull, ldftn <method>, newobj Func<object[][, params], object>
                _il.OpCode(ILOpCode.Ldnull);
                _il.OpCode(ILOpCode.Ldftn);
                _il.Token(methodHandle);
                // Count parameters including any synthetically represented destructuring patterns.
                int paramCount = CountRuntimeParameters(functionDeclaration.Params);
                var ctorRef = _bclReferences.GetFuncCtorRef(paramCount);
                _il.OpCode(ILOpCode.Newobj);
                _il.Token(ctorRef);
            }
            else
            {
                // Fallback: ldnull to preserve stack correctness
                _il.OpCode(ILOpCode.Ldnull);
            }
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
                case ForOfStatement forOfStatement:
                    GenerateForOfStatement(forOfStatement);
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
                    ILEmitHelpers.ThrowNotSupported($"Unsupported statement type: {statement.Type}", statement);
                    break; // unreachable
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
                (s is VariableDeclaration vd && vd.Kind is VariableDeclarationKind.Let or VariableDeclarationKind.Const)
                || (s is ForOfStatement fof && fof.Left is VariableDeclaration fovd && (fovd.Kind is VariableDeclarationKind.Let or VariableDeclarationKind.Const))
            );

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
                    // Only treat as function if it corresponds to a known function declaration.
                    // Guard: some identifiers refer to arrays/objects/etc. Returning those must NOT invoke Closure.Bind.
                    var fnVar = _variables.FindVariable(fid.Name);
                    bool isFunctionVariable = false;
                    if (fnVar != null)
                    {
                        // Consult registry for variable classification (Function vs Variable/Parameter)
                        try
                        {
                            var regInfo = _variables.GetVariableRegistry()?.GetVariableInfo(fnVar.ScopeName, fnVar.Name)
                                ?? _variables.GetVariableRegistry()?.FindVariable(fnVar.Name);
                            if (regInfo != null && regInfo.VariableType == VariableBindings.VariableType.Function)
                            {
                                isFunctionVariable = true;
                            }
                        }
                        catch
                        {
                            // Best-effort; if lookup fails, fall back to non-function behavior
                            isFunctionVariable = false;
                        }
                    }

                    if (fnVar != null && isFunctionVariable && !_inClassMethod) // skip closure binding logic inside class instance methods
                    {
                        // Load the function delegate from its scope field
                        var scopeSlot = _variables.GetScopeLocalSlot(fnVar.ScopeName);
                        if (scopeSlot.Address == -1)
                            throw new InvalidOperationException($"Scope '{fnVar.ScopeName}' not found in local slots");
                        if (scopeSlot.Location == ObjectReferenceLocation.Parameter)
                        {
                            _il.LoadArgument(scopeSlot.Address);
                            // Cast needed: parameter is typed as object
                            var regFn = _variables.GetVariableRegistry();
                            var tdefFn = regFn?.GetScopeTypeHandle(fnVar.ScopeName) ?? default;
                            if (!tdefFn.IsNil)
                            {
                                _il.OpCode(ILOpCode.Castclass);
                                _il.Token(tdefFn);
                            }
                        }
                        else if (scopeSlot.Location == ObjectReferenceLocation.ScopeArray)
                        {
                            _il.LoadArgument(0);
                            _il.LoadConstantI4(scopeSlot.Address);
                            _il.OpCode(ILOpCode.Ldelem_ref);
                            // Cast needed: array element is typed as object
                            var regFn = _variables.GetVariableRegistry();
                            var tdefFn = regFn?.GetScopeTypeHandle(fnVar.ScopeName) ?? default;
                            if (!tdefFn.IsNil)
                            {
                                _il.OpCode(ILOpCode.Castclass);
                                _il.Token(tdefFn);
                            }
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
        internal IEnumerable<string> GetScopesForClosureBinding(Variable functionVariable)
        {
            var names = _variables.GetAllScopeNames().ToList(); // Updated to ensure proper context
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
            // Ignore directive prologue string literals like "use strict".
            // A standalone string literal as an expression statement in JS is a directive, not a value-producing expression.
            // Emitting it would leave an unconsumed value on the stack and cause InvalidProgramException at method return.
            if (expressionStatement.Expression is Acornima.Ast.Literal lit && lit.Value is string)
            {
                return;
            }

                // Emit the expression in statement context. Discard the result only if a value was produced.
            var exprResult = _expressionEmitter.Emit(expressionStatement.Expression, new TypeCoercion(), CallSiteContext.Statement);
            // Assignments and updates store directly and leave the stack empty.
            // Also skip Pop when the expression is a call that returned void (no value on the stack).
            if (expressionStatement.Expression is not Acornima.Ast.AssignmentExpression
                && expressionStatement.Expression is not Acornima.Ast.UpdateExpression
                && exprResult.ClrType != typeof(void))
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
                var initResult = _expressionEmitter.Emit(exprInit, new TypeCoercion(), CallSiteContext.Statement);
                if (exprInit is not Acornima.Ast.AssignmentExpression
                    && exprInit is not Acornima.Ast.UpdateExpression
                    && initResult.ClrType != typeof(void))
                {
                    _il.OpCode(ILOpCode.Pop);
                }
            }
            else if (forStatement.Init is null)
            {
                // no-op
            }
            else
            {
                ILEmitHelpers.ThrowNotSupported($"Unsupported for statement initializer type: {forStatement.Init?.Type}", forStatement.Init);
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
                // Emit update in statement context and discard result to avoid leaving values on the stack
                var updResult = _expressionEmitter.Emit(forStatement.Update, new TypeCoercion(), CallSiteContext.Statement);
                if (forStatement.Update is not Acornima.Ast.AssignmentExpression
                    && forStatement.Update is not Acornima.Ast.UpdateExpression
                    && updResult.ClrType != typeof(void))
                {
                    _il.OpCode(ILOpCode.Pop);
                }
            }

            // branch back to the start of the loop
            _il.Branch(ILOpCode.Br, loopStartLabel);

            // here is the end
            _il.MarkLabel(loopEndLabel);

            // Pop loop context
            _loopStack.Pop();
        }

        public void GenerateForOfStatement(Acornima.Ast.ForOfStatement forOf)
        {
            // Desugar: for (x of iterable) { body }
            //   -> let iter = <Right>;
            //      let len = Object.GetLength(iter);
            //      let i = 0;
            //      while (i < len) { let x = Object.GetItem(iter, i); body; i++; }

            // Allocate labels
            var loopStartLabel = _il.DefineLabel();
            var loopBodyLabel = _il.DefineLabel();
            var loopEndLabel = _il.DefineLabel();
            var loopContinueLabel = _il.DefineLabel();

            // Determine binding target name (const/let/identifier)
            string? iterVarName = null;
            bool bindTargetIsConst = false;
            if (forOf.Left is VariableDeclaration vdecl && vdecl.Declarations.Count == 1 && vdecl.Declarations[0].Id is Identifier vid)
            {
                iterVarName = vid.Name;
                bindTargetIsConst = (vdecl.Kind == VariableDeclarationKind.Const);
            }
            else if (forOf.Left is Identifier id)
            {
                iterVarName = id.Name;
            }

            // Allocate three temp locals: iter (object), len (boxed double), i (boxed double)
            // Reuse block-scope local allocator to reserve object-typed local slots
            int iterLocal = _variables.AllocateBlockScopeLocal($"ForOfTemp_Iter_L{forOf.Location.Start.Line}C{forOf.Location.Start.Column}");
            int lenLocal = _variables.AllocateBlockScopeLocal($"ForOfTemp_Len_L{forOf.Location.Start.Line}C{forOf.Location.Start.Column}");
            int idxLocal = _variables.AllocateBlockScopeLocal($"ForOfTemp_Idx_L{forOf.Location.Start.Line}C{forOf.Location.Start.Column}");

            // Store iterable into iterLocal
            _ = _expressionEmitter.Emit(forOf.Right, new TypeCoercion() { boxResult = true });
            _il.StoreLocal(iterLocal);

            // Compute length and store in lenLocal (boxed)
            _il.LoadLocal(iterLocal);
            _runtime.InvokeGetLengthFromObject();
            _il.OpCode(ILOpCode.Box);
            _il.Token(_bclReferences.DoubleType);
            _il.StoreLocal(lenLocal);

            // Initialize index = 0 (boxed)
            _il.LoadConstantR8(0);
            _il.OpCode(ILOpCode.Box);
            _il.Token(_bclReferences.DoubleType);
            _il.StoreLocal(idxLocal);

            // Labels
            _il.MarkLabel(loopStartLabel);
            // Test: idx < len
            _il.LoadLocal(idxLocal);
            _il.OpCode(ILOpCode.Unbox_any);
            _il.Token(_bclReferences.DoubleType);
            _il.LoadLocal(lenLocal);
            _il.OpCode(ILOpCode.Unbox_any);
            _il.Token(_bclReferences.DoubleType);
            _il.Branch(ILOpCode.Blt, loopBodyLabel);
            _il.Branch(ILOpCode.Br, loopEndLabel);

            // Push loop context: continue -> continueLabel, break -> end
            _loopStack.Push(new LoopContext(loopContinueLabel, loopEndLabel));

            // Body
            _il.MarkLabel(loopBodyLabel);

            // Bind current element to target variable
            if (!string.IsNullOrEmpty(iterVarName))
            {
                // Try resolve via Variables; if unavailable (e.g., for-of header const not pre-registered), fall back to registry
                var targetVar = _variables.FindVariable(iterVarName!);
                if (targetVar == null)
                {
                    var registry = _variables.GetVariableRegistry();
                    var leafScope = _variables.GetLeafScopeName();
                    var vinfo = registry?.GetVariableInfo(leafScope, iterVarName!);
                    if (vinfo == null)
                    {
                        // As a last resort, attempt to declare (will throw if truly unknown)
                        if (forOf.Left is VariableDeclaration vdecl2)
                        {
                            DeclareVariable(vdecl2);
                            targetVar = _variables.FindVariable(iterVarName!);
                            if (targetVar == null)
                                throw new InvalidOperationException($"Variable '{iterVarName}' not found.");
                        }
                        else
                        {
                            ILEmitHelpers.ThrowNotSupported($"for-of target '{iterVarName}' could not be resolved.", forOf.Left);
                        }
                    }
                    else
                    {
                        // Create Variable from registry info for use with helpers
                        targetVar = new LocalVariable 
                        { 
                            Name = vinfo.Name, 
                            ScopeName = vinfo.ScopeName, 
                            FieldHandle = vinfo.FieldHandle, 
                            ClrType = vinfo.ClrType,
                            IsStableType = vinfo.IsStableType
                        };
                    }
                }

                // Check if it's a field variable and we need to load scope first
                bool isFieldVariable = targetVar.LocalSlot < 0;
                
                // Load scope instance if storing to field (must be before value on stack)
                if (isFieldVariable)
                {
                    var tslot = _variables.GetScopeLocalSlot(targetVar.ScopeName);
                    var regFO = _variables.GetVariableRegistry();
                    var tdefFO = regFO?.GetScopeTypeHandle(targetVar.ScopeName) ?? default;
                    
                    if (tslot.Location == ObjectReferenceLocation.Parameter)
                    {
                        _il.LoadArgument(tslot.Address);
                        if (!tdefFO.IsNil)
                        {
                            _il.OpCode(ILOpCode.Castclass);
                            _il.Token(tdefFO);
                        }
                    }
                    else if (tslot.Location == ObjectReferenceLocation.ScopeArray)
                    {
                        _il.LoadArgument(0);
                        _il.LoadConstantI4(tslot.Address);
                        _il.OpCode(ILOpCode.Ldelem_ref);
                        if (!tdefFO.IsNil)
                        {
                            _il.OpCode(ILOpCode.Castclass);
                            _il.Token(tdefFO);
                        }
                    }
                    else
                    {
                        _il.LoadLocal(tslot.Address);
                    }
                }

                // Load iterable and index to get item (leaves value on stack)
                _il.LoadLocal(iterLocal);
                _il.LoadLocal(idxLocal);
                _runtime.InvokeGetItemFromObject();

                // Store using helper (handles both local and field cases)
                _il.EmitStoreVariable(targetVar, _variables, scopeAlreadyLoaded: isFieldVariable);
            }

            // Emit loop body
            GenerateStatement(forOf.Body);

            // continue label: increment index
            _il.MarkLabel(loopContinueLabel);
            // idx = (double)idx + 1
            _il.LoadLocal(idxLocal);
            _il.OpCode(ILOpCode.Unbox_any);
            _il.Token(_bclReferences.DoubleType);
            _il.LoadConstantR8(1);
            _il.OpCode(ILOpCode.Add);
            _il.OpCode(ILOpCode.Box);
            _il.Token(_bclReferences.DoubleType);
            _il.StoreLocal(idxLocal);

            _il.Branch(ILOpCode.Br, loopStartLabel);

            _il.MarkLabel(loopEndLabel);
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
                ILEmitHelpers.ThrowNotSupported("'throw' without an expression is not supported", throwStatement);
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

        internal MethodDefinitionHandle GenerateArrowFunctionMethod(ArrowFunctionExpression arrowFunction, string registryScopeName, string ilMethodName, string[] paramNames)
        {
            var arrowGen = new JavaScriptArrowFunctionGenerator(_variables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _classRegistry, _functionRegistry);
            return arrowGen.GenerateArrowFunctionMethod(arrowFunction, registryScopeName, ilMethodName, paramNames);
        }

        internal MethodDefinitionHandle GenerateFunctionExpressionMethod(FunctionExpression funcExpr, string registryScopeName, string ilMethodName, string[] paramNames)
        {
            var functionVariables = new Variables(_variables, registryScopeName, paramNames, isNestedFunction: true);
            var pnames = paramNames ?? Array.Empty<string>();
            // Share the parent ClassRegistry and FunctionRegistry so nested functions can resolve declared classes
            // and register their methods for lazy self-binding (recursion) support.
            var childGen = new ILMethodGenerator(functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _classRegistry, _functionRegistry);
            var il = childGen.IL;

            // If this is a named function expression (e.g., function walk(node) { ... }), JS specifies
            // that the name is bound inside the function body to the function object itself (for recursion).
            // Strategy: after (optionally) creating the scope instance and before emitting the body, store
            // the delegate into the scope field matching the name (if one exists in the registry scope).
            Identifier? internalNameId = funcExpr.Id as Identifier;

            // Pre-register the function's parameter count BEFORE generating the body.
            // This allows recursive calls within the function body to find the correct parameter count.
            if (internalNameId != null && _functionRegistry != null)
            {
                _functionRegistry.PreRegisterParameterCount(internalNameId.Name, pnames.Length);
            }

            // Function expressions use block bodies; create local scope if fields exist and init parameter fields.
            // We also eagerly emit self-binding for named function expressions (internal name) BEFORE body so
            // recursive calls resolve. We need the eventual MethodDefinitionHandle for ldftn, so we build the
            // method body in two phases: capture IL so far, but the handle (mdh) is only known after adding the
            // method definition. Strategy: emit a placeholder sequence we can patch is complicated; instead we
            // construct the delegate via reflection to our own method after mdh is known by re-opening the IL
            // stream is not supported. Simpler: emit delegate creation AFTER adding method definition by
            // building a tiny prologue first, then body referencing a local temp field. To keep changes minimal
            // we resolve self-binding using a lazy pattern: during first recursive call if field is null, load
            // ldftn of this method and store. (Cheap check, avoids complex two-pass.)
            // Implementation: if named function expression and internal binding field exists, we pre-store null
            // (already default), and inject at top of body: if(field==null){ field = new Func(..., thisMethod); }

            var registry = functionVariables.GetVariableRegistry();
            if (registry != null)
            {
                var fields = registry.GetVariablesForScope(registryScopeName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
                if (fields.Any())
                {
                    ScopeInstanceEmitter.EmitCreateLeafScopeInstance(functionVariables, il, _metadataBuilder);
                    // Initialize default parameter values before field initialization
                    childGen.EmitDefaultParameterInitializers(funcExpr.Params, parameterStartIndex: 1);
                    var localScope = functionVariables.GetLocalScopeSlot();
                    if (localScope.Address >= 0 && funcExpr.Params.Count > 0)
                    {
                        var fieldNames = new HashSet<string>(fields.Select(f => f.Name));
                        ushort jsParamSeq = 1; // arg0 is scopes[]
                        for (int i = 0; i < funcExpr.Params.Count; i++)
                        {
                            var paramNode = funcExpr.Params[i];
                            // Extract identifier from Identifier or AssignmentPattern
                            Identifier? pid = paramNode as Identifier;
                            if (pid == null && paramNode is AssignmentPattern ap)
                            {
                                pid = ap.Left as Identifier;
                            }
                            
                            if (pid != null && fieldNames.Contains(pid.Name))
                            {
                                il.LoadLocal(localScope.Address);
                                childGen.EmitLoadParameterWithDefault(paramNode, jsParamSeq);
                                var fh = registry.GetFieldHandle(registryScopeName, pid.Name);
                                il.OpCode(ILOpCode.Stfld);
                                il.Token(fh);
                            }
                            jsParamSeq++;
                        }
                        // Destructure object-pattern parameters
                        jsParamSeq = 1;
                        for (int i = 0; i < funcExpr.Params.Count; i++)
                        {
                            var pnode = funcExpr.Params[i];
                            if (pnode is ObjectPattern op)
                            {
                                foreach (var propNode in op.Properties)
                                {
                                    if (propNode is Property p)
                                    {
                                        // Support alias ({x: y}) and shorthand ({a}) forms
                                        var bindId = p.Value as Identifier ?? p.Key as Identifier;
                                        if (bindId == null) continue;
                                        var targetVar = functionVariables.FindVariable(bindId.Name);
                                        if (targetVar == null) continue;
                                        var propName = (p.Key as Identifier)?.Name ?? (p.Key as Literal)?.Value?.ToString() ?? string.Empty;
                                        ObjectPatternHelpers.EmitParamDestructuring(il, _metadataBuilder, _runtime, functionVariables, targetVar, jsParamSeq, propName);
                                    }
                                }
                            }
                            jsParamSeq++;
                        }
                    }
                    // Eager self-binding prologue for named function expressions
                    if (internalNameId != null)
                    {
                        // Only bind if the internal binding field exists in this scope
                        FieldDefinitionHandle selfFieldHandle = default;
                        try { selfFieldHandle = registry.GetFieldHandle(registryScopeName, internalNameId.Name); } catch { selfFieldHandle = default; }
                        if (!selfFieldHandle.IsNil && localScope.Address >= 0)
                        {
                            // Load scope instance (no cast needed: local is strongly-typed)
                            il.LoadLocal(localScope.Address);
                            // Duplicate for ldfld test and potential stfld target
                            il.OpCode(ILOpCode.Dup);
                            il.OpCode(ILOpCode.Ldfld); il.Token(selfFieldHandle);
                            var alreadyBound = il.DefineLabel();
                            var endBind = il.DefineLabel();
                            il.Branch(ILOpCode.Brtrue_s, alreadyBound);
                            // Not bound yet: stack has the scope instance
                            // Create delegate to current method using reflection: GetCurrentMethod(), paramCount
                            // GetCurrentMethod returns MethodBase for this function method (we are in the function body)
                            il.OpCode(ILOpCode.Call); il.Token(_bclReferences.MethodBase_GetCurrentMethod_Ref);
                            // paramCount for delegate arity
                            il.LoadConstantI4(pnames.Length);
                            // Call runtime helper to create correct Func<object[],...,object>
                            // Build a MemberReference to JavaScriptRuntime.Closure.CreateSelfDelegate(MethodBase, int)
                            var closureTypeRef = _runtime.GetRuntimeTypeHandle(typeof(JavaScriptRuntime.Closure));
                            var makeSelfSig = new BlobBuilder();
                            new BlobEncoder(makeSelfSig)
                                .MethodSignature(isInstanceMethod: false)
                                .Parameters(2,
                                    rt => rt.Type().Object(),
                                    p => {
                                        p.AddParameter().Type().Type(_bclReferences.MethodBaseType, isValueType: false);
                                        p.AddParameter().Type().Int32();
                                    });
                            var makeSelfRef = _metadataBuilder.AddMemberReference(
                                closureTypeRef,
                                _metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime.Closure.CreateSelfDelegate)),
                                _metadataBuilder.GetOrAddBlob(makeSelfSig));
                            il.OpCode(ILOpCode.Call); il.Token(makeSelfRef);
                            // Store into internal binding field
                            il.OpCode(ILOpCode.Stfld); il.Token(selfFieldHandle);
                            il.Branch(ILOpCode.Br, endBind);
                            // Already bound: pop the duplicated scope instance to balance stack
                            il.MarkLabel(alreadyBound); il.OpCode(ILOpCode.Pop);
                            il.MarkLabel(endBind);
                        }
                    }
                }
            }

            if (funcExpr.Body is BlockStatement block)
            {
                // We'll emit body into childGen; but first, if named function expression, inject lazy self-binding check.
                // We need the field handle; after mdh is created we'll patch? Instead, emit code that on first recursive
                // call (when identifier resolved) will find null and bind. For simplicity, we rely on EmitFunctionCall
                // having already loaded delegate field; if null we throw. To avoid modifying call logic now, implement
                // eager binding AFTER mdh creation by buffering body IL then prepending prologue.
                // Ensure lexical scope is active for resolution of local bindings inside function expression
                childGen._variables.PushLexicalScope(functionVariables.GetLeafScopeName());
                try
                {
                    childGen.GenerateStatementsForBody(functionVariables.GetLeafScopeName(), false, block.Body);
                }
                finally
                {
                    childGen._variables.PopLexicalScope(functionVariables.GetLeafScopeName());
                }
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
            var (localSignature, bodyAttributes) = MethodBuilder.CreateLocalVariableSignature(_metadataBuilder, functionVariables, this._bclReferences);

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
            var mdh = tb.AddMethodDefinition(MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.HideBySig, ilMethodName, methodSig, bodyOffset, firstParam);
            // Register named function expression in function registry so recursive calls can lazy-bind.
            if (internalNameId != null && _functionRegistry != null)
            {
                var plen = paramNames?.Length ?? -1;
                // Only register if not already registered (avoid duplicate registration exception)
                if (_functionRegistry.Get(internalNameId.Name).IsNil)
                {
                    _functionRegistry.Register(internalNameId.Name, mdh, plen);
                }
            }
            // NOTE: Self-binding eager insertion not implemented due to single-pass encoder limitations.
            // For recursion support, EmitFunctionCall must locate the internal binding. Since we authored an
            // internal binding field in SymbolTable for named function expressions, the delegate field remains
            // null unless assigned externally. TODO: future enhancement - restructure to build body after mdh
            // known so we can ldftn this method and store delegate before executing body. Current change ensures
            // internal binding exists so outer recursion pattern can be updated next.
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

namespace Js2IL.Services.ILGenerators
{
    internal partial class ILMethodGenerator
    {
        internal static int CountRuntimeParameters(IReadOnlyList<Node> parameters)
        {
            // Each declared parameter (identifier or pattern) is one runtime argument.
            if (parameters == null) return 0;
            int count = 0;
            foreach (var _ in parameters) count++;
            return count;
        }

        /// <summary>
        /// Counts the number of required parameters (those without default values).
        /// Parameters with defaults (AssignmentPattern) are optional.
        /// </summary>
        internal static int CountRequiredParameters(IReadOnlyList<Node> parameters)
        {
            if (parameters == null) return 0;
            
            // Count parameters that are not AssignmentPattern (i.e., required parameters)
            return parameters.Count(param => param is not AssignmentPattern);
        }

        /// <summary>
        /// Extracts parameter names from a parameter list, handling both simple identifiers and assignment patterns (default parameters).
        /// For AssignmentPattern nodes (e.g., a = 10), extracts the identifier from the left side.
        /// </summary>
        internal static IEnumerable<string> ExtractParameterNames(IReadOnlyList<Node> parameters)
        {
            if (parameters == null) yield break;
            
            int index = 0;
            foreach (var param in parameters)
            {
                if (param is Identifier id)
                {
                    yield return id.Name;
                }
                else if (param is AssignmentPattern ap && ap.Left is Identifier apId)
                {
                    yield return apId.Name;
                }
                else if (param is ObjectPattern or ArrayPattern)
                {
                    // Destructuring parameters map to a single IL parameter (the object to destructure)
                    // Use a generic placeholder name since the actual field names are extracted during destructuring
                    yield return "param" + (index + 1);
                }
                index++;
            }
        }

        /// <summary>
        /// Emits IL to load a parameter value.
        /// Default parameters are handled by EmitDefaultParameterInitializers using starg,
        /// so this method simply loads the argument (which already contains the correct value).
        /// </summary>
        /// <param name="paramNode">The parameter AST node (reserved for future use, currently unused)</param>
        /// <param name="argIndex">The argument index to load from</param>
        internal void EmitLoadParameterWithDefault(Node paramNode, ushort argIndex)
        {
            // Default parameters are handled by EmitDefaultParameterInitializers using starg
            // So we just load the parameter directly (it already has the default if it was null)
            // Note: paramNode is currently unused but kept for potential future enhancements
            _il.LoadArgument(argIndex);
        }

        /// <summary>
        /// Emits IL to initialize default parameter values using the starg pattern.
        /// For each parameter with a default value (AssignmentPattern), emits:
        /// - Check if parameter is null (brtrue)
        /// - If null, evaluate default expression and store back to parameter (starg)
        /// This must execute before field initialization so captured parameters receive correct default values.
        /// </summary>
        /// <param name="parameters">The parameter list to process</param>
        /// <param name="parameterStartIndex">The starting argument index (accounts for 'this' in instance methods)</param>
        internal void EmitDefaultParameterInitializers(IReadOnlyList<Node> parameters, ushort parameterStartIndex)
        {
            // Early exit if there are no parameters with defaults
            if (!parameters.Any(param => param is AssignmentPattern ap && ap.Left is not (ObjectPattern or ArrayPattern)))
            {
                return;  // No default parameters to process
            }
            
            // For parameters with defaults, use starg to update the parameter value if null
            // This must happen BEFORE field initialization so captured parameters get the correct value
            ushort argIndex = parameterStartIndex;
            for (int i = 0; i < parameters.Count; i++)
            {
                var param = parameters[i];
                
                // Skip destructuring parameters - they don't support defaults yet
                if (param is ObjectPattern or ArrayPattern)
                {
                    argIndex++;  // Still increment - destructuring params consume an IL argument slot
                    continue;
                }
                
                if (param is AssignmentPattern ap)
                {
                    // Check if the pattern itself is a destructuring pattern
                    if (ap.Left is ObjectPattern or ArrayPattern)
                    {
                        // Destructuring with default - not yet supported
                        argIndex++;  // Still increment - destructuring params consume an IL argument slot
                        continue;
                    }
                    
                    // Parameter has a default value
                    var notNullLabel = _il.DefineLabel();
                    
                    // Load and check if parameter is null
                    _il.LoadArgument(argIndex);
                    _il.Branch(ILOpCode.Brtrue, notNullLabel);
                    
                    // Parameter is null - emit default value and store back to parameter using starg
                    _ = _expressionEmitter.Emit(ap.Right, new TypeCoercion() { boxResult = true });
                    _il.StoreArgument(argIndex);
                    
                    // Mark the not-null label
                    _il.MarkLabel(notNullLabel);
                }
                argIndex++;
            }
        }
    }
}
