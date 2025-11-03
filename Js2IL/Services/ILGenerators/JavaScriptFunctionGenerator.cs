using Acornima.Ast;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services.ILGenerators
{
    internal class JavaScriptFunctionGenerator
    {
        private Variables _variables;
        private BaseClassLibraryReferences _bclReferences;
        private MetadataBuilder _metadataBuilder;
        private MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private readonly ClassRegistry _classRegistry;
        private MethodDefinitionHandle _firstMethod = default;
        private readonly FunctionRegistry _functionRegistry = new();
    public FunctionRegistry FunctionRegistry => _functionRegistry;

        // Tracks owner types in the Functions namespace
        private readonly Dictionary<string, TypeDefinitionHandle> _globalFunctionOwnerTypes = new();
        private TypeDefinitionHandle _moduleOwnerType = default;
        public JavaScriptFunctionGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, ClassRegistry? classRegistry = null)
        {
            _variables = variables;
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            _methodBodyStreamEncoder = methodBodyStreamEncoder;
            _classRegistry = classRegistry ?? new ClassRegistry();
        }

        public void DeclareFunctions(SymbolTable symbolTable)
        {
            // New hosting model: one owner type per module under the Functions namespace.
            // All top-level functions are static methods on Functions.<RootName>.
            // Nested functions get their own nested owner type under Functions.<RootName>.

            var root = symbolTable.Root;
            var moduleName = root.Name;

            // 1) Plan and emit top-level methods via a TypeBuilder per hosting strategy
            var topLevelFunctions = root.Children.Where(c => c.Kind == ScopeKind.Function && c.AstNode is FunctionDeclaration).ToList();
            var globalMethods = new List<(string Name, MethodDefinitionHandle Handle, Scope Scope, Variables Vars, TypeBuilder? NestedOwnerBuilder, MethodDefinitionHandle FirstNestedMethod)>();
            var pendingNestedOwnerFinalization = new List<(string OuterName, TypeBuilder NestedBuilder, MethodDefinitionHandle FirstNestedMethod)>();

            if (topLevelFunctions.Count > 1)
            {
                // Define module owner type first so method tokens reference an existing type row
                var moduleTb = new TypeBuilder(_metadataBuilder, "Functions", moduleName);
                _moduleOwnerType = moduleTb.AddTypeDefinition(
                    TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                    _bclReferences.ObjectType);

                foreach (var funcScope in topLevelFunctions)
                {
                    var functionDeclaration = (FunctionDeclaration)funcScope.AstNode!;
                    var functionName = (functionDeclaration.Id as Identifier)!.Name;

                    // Prepare variables for the outer function
                    var paramNames = functionDeclaration.Params.OfType<Identifier>().Select(p => p.Name).ToArray();
                    var functionVariables = new Variables(_variables, functionName, paramNames, isNestedFunction: false);

                    // Pre-generate nested function methods (depth-first) so their handles exist before outer initialization.
                    TypeBuilder? nestedTb = null;
                    MethodDefinitionHandle firstNestedMethod = default;
                    var nestedFunctionScopes = funcScope.Children.Where(c => c.Kind == ScopeKind.Function && c.AstNode is FunctionDeclaration).ToList();
                    if (nestedFunctionScopes.Count > 0)
                    {
                        nestedTb = new TypeBuilder(_metadataBuilder, "Functions", functionName + "_Nested");
                        // Define nested owner type early
                        var nestedOwnerHandle = nestedTb.AddTypeDefinition(
                            TypeAttributes.NestedPublic | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                            _bclReferences.ObjectType);
                        _metadataBuilder.AddNestedType(nestedOwnerHandle, _moduleOwnerType);
                        foreach (var nestedScope in nestedFunctionScopes)
                        {
                            var nestedDecl = (FunctionDeclaration)nestedScope.AstNode!;
                            var nestedName = (nestedDecl.Id as Identifier)!.Name;
                            var nestedParamNames = nestedDecl.Params.OfType<Identifier>().Select(p => p.Name).ToArray();
                            var nestedVars = new Variables(functionVariables, nestedName, nestedParamNames, isNestedFunction: true);
                            var nestedGen = new ILMethodGenerator(nestedVars, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _classRegistry, _functionRegistry);
                            var nestedMethod = GenerateMethodForFunction(nestedDecl, nestedVars, nestedGen, nestedScope, symbolTable, nestedTb);
                            if (firstNestedMethod.IsNil) firstNestedMethod = nestedMethod;
                            if (this._firstMethod.IsNil) _firstMethod = nestedMethod;
                            _functionRegistry.Register(nestedName, nestedMethod);
                        }
                        // (No deferred finalization needed now)
                    }

                    // Now generate the outer function method (nested handles already registered)
                    var methodGenerator = new ILMethodGenerator(functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _classRegistry, _functionRegistry);
                    var methodDefinition = GenerateMethodForFunction(functionDeclaration, functionVariables, methodGenerator, funcScope, symbolTable, moduleTb);
                    if (this._firstMethod.IsNil) _firstMethod = methodDefinition;
                    _functionRegistry.Register(functionName, methodDefinition);

                    globalMethods.Add((functionName, methodDefinition, funcScope, functionVariables, nestedTb, firstNestedMethod));
                }

                // (Type already defined at top of branch)
            }
            else if (topLevelFunctions.Count == 1)
            {
                // Single top-level function: create and define owner type before methods
                var funcScope = topLevelFunctions[0];
                var functionDeclaration = (FunctionDeclaration)funcScope.AstNode!;
                var functionName = (functionDeclaration.Id as Identifier)!.Name;

                var tb = new TypeBuilder(_metadataBuilder, "Functions", functionName);
                var globalOwnerType = tb.AddTypeDefinition(
                    TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                    _bclReferences.ObjectType);
                var paramNames = functionDeclaration.Params.OfType<Identifier>().Select(p => p.Name).ToArray();
                var functionVariables = new Variables(_variables, functionName, paramNames, isNestedFunction: false);

                // Generate nested functions first (if any)
                TypeBuilder? nestedTb = null;
                MethodDefinitionHandle firstNestedMethod = default;
                var nestedFunctionScopes = funcScope.Children.Where(c => c.Kind == ScopeKind.Function && c.AstNode is FunctionDeclaration).ToList();
                if (nestedFunctionScopes.Count > 0)
                {
                    nestedTb = new TypeBuilder(_metadataBuilder, "Functions", functionName + "_Nested");
                    var nestedHandle = nestedTb.AddTypeDefinition(
                        TypeAttributes.NestedPublic | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                        _bclReferences.ObjectType);
                    _metadataBuilder.AddNestedType(nestedHandle, globalOwnerType);
                    foreach (var nestedScope in nestedFunctionScopes)
                    {
                        var nestedDecl = (FunctionDeclaration)nestedScope.AstNode!;
                        var nestedName = (nestedDecl.Id as Identifier)!.Name;
                        var nestedParamNames = nestedDecl.Params.OfType<Identifier>().Select(p => p.Name).ToArray();
                        var nestedVars = new Variables(functionVariables, nestedName, nestedParamNames, isNestedFunction: true);
                        var nestedGen = new ILMethodGenerator(nestedVars, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _classRegistry, _functionRegistry);
                        var nestedMethod = GenerateMethodForFunction(nestedDecl, nestedVars, nestedGen, nestedScope, symbolTable, nestedTb);
                        if (firstNestedMethod.IsNil) firstNestedMethod = nestedMethod;
                        if (this._firstMethod.IsNil) _firstMethod = nestedMethod;
                        _functionRegistry.Register(nestedName, nestedMethod);
                    }
                }

                // Now generate outer method
                var methodGenerator = new ILMethodGenerator(functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _classRegistry, _functionRegistry);
                var methodDefinition = GenerateMethodForFunction(functionDeclaration, functionVariables, methodGenerator, funcScope, symbolTable, tb);
                if (this._firstMethod.IsNil) _firstMethod = methodDefinition;
                _functionRegistry.Register(functionName, methodDefinition);

                globalMethods.Add((functionName, methodDefinition, funcScope, functionVariables, nestedTb, firstNestedMethod));

                _globalFunctionOwnerTypes[functionName] = globalOwnerType;
                _moduleOwnerType = default; // not used in this mode
            }
            // For multi-function module case finalize nested owners handled earlier.
        }

        private void DeclareFunctionsRecursive(Scope scope, Variables parentVars, SymbolTable symbolTable)
        {
            // Generate methods for each function declared directly in this scope
            foreach (var funcScope in scope.Children.Where(c => c.Kind == ScopeKind.Function && c.AstNode is FunctionDeclaration))
            {
                var functionDeclaration = (FunctionDeclaration)funcScope.AstNode!;
                var functionName = (functionDeclaration.Id as Identifier)!.Name;
                var paramNames = functionDeclaration.Params
                    .OfType<Identifier>()
                    .Select(p => p.Name)
                    .ToArray();

                bool isNested = scope.Kind == ScopeKind.Function; // nested if parent is a function
                var functionVariables = new Variables(parentVars, functionName, paramNames, isNestedFunction: isNested);
                    var methodGenerator = new ILMethodGenerator(functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _classRegistry, _functionRegistry);
                var methodDefinition = GenerateMethodForFunction(functionDeclaration, functionVariables, methodGenerator, funcScope, symbolTable);
                if (this._firstMethod.IsNil)
                {
                    this._firstMethod = methodDefinition;
                }
                if (_functionRegistry.Get(functionName).IsNil)
                {
                    _functionRegistry.Register(functionName, methodDefinition);
                }

                // Recurse into nested functions with this function's Variables as the new parent
                DeclareFunctionsRecursive(funcScope, functionVariables, symbolTable);
            }
        }

    public MethodDefinitionHandle GenerateMethodForFunction(FunctionDeclaration functionDeclaration, Variables functionVariables, ILMethodGenerator methodGenerator, Scope? functionScope = null, SymbolTable? symbolTable = null, TypeBuilder? typeBuilder = null)
        {
            var functionName = (functionDeclaration.Id as Acornima.Ast.Identifier)!.Name;

            if (functionDeclaration.Body is not BlockStatement)
            {
                ILEmitHelpers.ThrowNotSupported($"Unsupported function body type: {functionDeclaration.Body.Type}", functionDeclaration.Body);
                throw new InvalidOperationException(); // unreachable, satisfies definite assignment
            }
            var blockStatement = (BlockStatement)functionDeclaration.Body;

            var variables = functionVariables;
            var il = methodGenerator.IL;

            // Parameters are already registered in Variables constructor

            // Create a scope instance for the function itself so that local vars (declared within the function)
            // have a backing scope object to store their fields. Only allocate if this function scope has fields.
            var registry = variables.GetVariableRegistry();
            if (registry != null)
            {
                var fields = registry.GetVariablesForScope(functionName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
                var hasAnyFields = fields.Any();
                if (hasAnyFields)
                {
                    // Create the leaf scope instance using the shared helper
                    ScopeInstanceEmitter.EmitCreateLeafScopeInstance(variables, il, _metadataBuilder);

                    // Initialize parameter fields on the scope from CLR arguments only when a backing field exists for the parameter.
                    var localScope = variables.GetLocalScopeSlot();
                    if (localScope.Address >= 0)
                    {
                        // Build a fast lookup of field-backed names in this scope
                        var fieldNames = new HashSet<string>(fields.Select(f => f.Name));

                        // JS parameters start at arg1 (arg0 is scopes[])
                        ushort jsParamSeq = 1;
                        foreach (var param in functionDeclaration.Params.OfType<Acornima.Ast.Identifier>())
                        {
                            if (!fieldNames.Contains(param.Name))
                            {
                                // No field backing for this parameter (e.g., no nested functions). Skip.
                                jsParamSeq++;
                                continue;
                            }

                            // Load scope instance (target for stfld)
                            il.LoadLocal(localScope.Address);
                            // Load CLR arg for this parameter (object already)
                            il.LoadArgument(jsParamSeq);
                            // Store to the corresponding field on the scope
                            var fieldHandle = registry.GetFieldHandle(functionName, param.Name);
                            il.OpCode(ILOpCode.Stfld);
                            il.Token(fieldHandle);
                            jsParamSeq++;
                        }
                    }
                }
            }

            // Emit body statements
            var hasExplicitReturn = blockStatement.Body.Any(s => s is ReturnStatement);
            
            // Initialize nested function variables before generating other statements
            if (functionScope != null)
            {
                // Get nested functions from the function's child scopes
                var nestedFunctions = functionScope.Children
                    .Where(scope => scope.Kind == ScopeKind.Function && scope.AstNode is FunctionDeclaration)
                    .Select(scope => (FunctionDeclaration)scope.AstNode!)
                    .ToList();
                methodGenerator.InitializeLocalFunctionVariables(nestedFunctions);
            }
            
            methodGenerator.GenerateStatementsForBody(functionVariables.GetLeafScopeName(), false, blockStatement.Body);
            if (!hasExplicitReturn)
            {
                // Implicit return undefined => null
                il.OpCode(ILOpCode.Ldnull);
                il.OpCode(ILOpCode.Ret);
            }

            // Add method body including any scope locals we created (function scope instance)
            StandaloneSignatureHandle localSignature = default;
            MethodBodyAttributes bodyAttributes = MethodBodyAttributes.None;
            var localCount = variables.GetNumberOfLocals();
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

            var bodyoffset = _methodBodyStreamEncoder.AddMethodBody(
                il,
                maxStack: 32, // todo - keep track of the pops and pushes so as to provide a accurate value for maxStack
                localVariablesSignature: localSignature,
                attributes: bodyAttributes);
                
            // Build method signature: static object (object[] scopes, object param1, ...)
            var sigBuilder = new BlobBuilder();
            var paramCount = 1 + functionDeclaration.Params.Count; // scope array + declared params
            new BlobEncoder(sigBuilder)
                .MethodSignature()
                .Parameters(paramCount, returnType => returnType.Type().Object(), parameters =>
                {
                    // scope array parameter
                    parameters.AddParameter().Type().SZArray().Object();
                    // each JS parameter as System.Object for now
                    foreach (var p in functionDeclaration.Params)
                    {
                        parameters.AddParameter().Type().Object();
                    }
                });
            var methodSig = _metadataBuilder.GetOrAddBlob(sigBuilder);

            // Add parameters with names
            var scopeParamName = "scopes";
            ParameterHandle firstParamHandle = _metadataBuilder.AddParameter(
                ParameterAttributes.None,
                _metadataBuilder.GetOrAddString(scopeParamName),
                sequenceNumber: 1);
            // subsequent params
            ushort seq = 2;
            foreach (var p in functionDeclaration.Params)
            {
                if (p is Acornima.Ast.Identifier pid)
                {
                    _metadataBuilder.AddParameter(ParameterAttributes.None, _metadataBuilder.GetOrAddString(pid.Name), sequenceNumber: seq++);
                }
                else
                {
                    _metadataBuilder.AddParameter(ParameterAttributes.None, _metadataBuilder.GetOrAddString($"param{seq-1}"), sequenceNumber: seq++);
                }
            }

            if (typeBuilder is null)
            {
                throw new InvalidOperationException("TypeBuilder is required for method emission.");
            }

                var methodDefinition = typeBuilder.AddMethodDefinition(
                MethodAttributes.Static | MethodAttributes.Public,
                functionName,
                methodSig,
                bodyoffset,
                parameterList: firstParamHandle);

            return methodDefinition;
        }
    }
}
