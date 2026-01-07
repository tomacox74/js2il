using Acornima.Ast;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using Js2IL.IL;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;

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
        private readonly SymbolTable? _symbolTable;
        private readonly CompiledMethodCache _compiledMethodCache;
        private readonly TwoPhaseCompilationCoordinator _twoPhaseCoordinator;

        public FunctionRegistry FunctionRegistry => _functionRegistry;

        // Tracks owner types in the Functions namespace
        private readonly Dictionary<string, TypeDefinitionHandle> _globalFunctionOwnerTypes = new();
        private TypeDefinitionHandle _moduleOwnerType = default;

        private IServiceProvider _serviceProvider;

        public JavaScriptFunctionGenerator(IServiceProvider serviceProvider, Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, ClassRegistry? classRegistry = null, SymbolTable? symbolTable = null)
        {
            _variables = variables;
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            _methodBodyStreamEncoder = methodBodyStreamEncoder;
            _classRegistry = classRegistry ?? new ClassRegistry();
            _symbolTable = symbolTable;
            _compiledMethodCache = serviceProvider.GetRequiredService<CompiledMethodCache>();
            _twoPhaseCoordinator = serviceProvider.GetRequiredService<TwoPhaseCompilationCoordinator>();
            this._serviceProvider = serviceProvider;
        }

        public void DeclareFunctions(SymbolTable symbolTable)
        {
            // New hosting model: one owner type per module under the Functions namespace.
            // All top-level functions are static methods on Functions.<RootName>.
            // Nested functions get their own nested owner type under Functions.<RootName>.

            var root = symbolTable.Root;
            var moduleName = root.Name;

            // 1) Emit top-level methods on a module owner type under Functions.<ModuleName>
            var topLevelFunctions = root.Children.Where(c => c.Kind == ScopeKind.Function && c.AstNode is FunctionDeclaration).ToList();
            var globalMethods = new List<(string Name, MethodDefinitionHandle Handle, Scope Scope, Variables Vars, TypeBuilder? NestedOwnerBuilder, MethodDefinitionHandle FirstNestedMethod)>();
            var pendingNestedOwnerFinalization = new List<(string OuterName, TypeBuilder NestedBuilder, MethodDefinitionHandle FirstNestedMethod)>();

            if (topLevelFunctions.Count == 0)
            {
                return;
            }

            // Define module owner type first so method tokens reference an existing type row
            var moduleTb = new TypeBuilder(_metadataBuilder, "Functions", moduleName);
            _moduleOwnerType = moduleTb.AddTypeDefinition(
                TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                _bclReferences.ObjectType);

            foreach (var funcScope in topLevelFunctions)
            {
                var functionDeclaration = (FunctionDeclaration)funcScope.AstNode!;
                var functionName = (functionDeclaration.Id as Identifier)!.Name;

                var registryScopeName = $"{moduleName}/{functionName}";

                // Prepare variables for the outer function
                var paramNames = ILMethodGenerator.ExtractParameterNames(functionDeclaration.Params).ToArray();
                var functionVariables = new Variables(_variables, registryScopeName, paramNames, isNestedFunction: false);

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
                        var nestedParamNames = ILMethodGenerator.ExtractParameterNames(nestedDecl.Params).ToArray();
                        // Pre-register parameter count before generating method body
                        _functionRegistry.PreRegisterParameterCount(nestedName, nestedParamNames.Length);
                        var nestedRegistryScopeName = $"{moduleName}/{nestedName}";
                        var nestedVars = new Variables(functionVariables, nestedRegistryScopeName, nestedParamNames, isNestedFunction: true);
                        var nestedGen = new ILMethodGenerator(_serviceProvider, nestedVars, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _classRegistry, _functionRegistry, symbolTable: _symbolTable);
                        var nestedMethod = GenerateMethodForFunction(nestedDecl, nestedVars, nestedGen, nestedScope, symbolTable, nestedTb, nestedRegistryScopeName);
                        if (firstNestedMethod.IsNil) firstNestedMethod = nestedMethod;
                        if (this._firstMethod.IsNil) _firstMethod = nestedMethod;
                        _functionRegistry.Register(nestedName, nestedMethod, nestedParamNames.Length);
                        
                        // Register nested function in CompiledMethodCache for IR pipeline function call emission
                        // The nested function's binding is in the parent function's scope (funcScope)
                        if (funcScope.Bindings.TryGetValue(nestedName, out var nestedBinding))
                        {
                            _compiledMethodCache.Add(nestedBinding, nestedMethod);
                        }

                        // Two-phase: register the token in the canonical CallableRegistry
                        _twoPhaseCoordinator.Registry.SetDeclaredTokenForAstNode(nestedDecl, (EntityHandle)nestedMethod);
                    }
                }

                // Now generate the outer function method (nested handles already registered)
                var methodGenerator = new ILMethodGenerator(_serviceProvider, functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _classRegistry, _functionRegistry, symbolTable: _symbolTable);
                var methodDefinition = GenerateMethodForFunction(functionDeclaration, functionVariables, methodGenerator, funcScope, symbolTable, moduleTb, registryScopeName);
                if (this._firstMethod.IsNil) _firstMethod = methodDefinition;
                _functionRegistry.Register(functionName, methodDefinition, paramNames.Length);

                // Register in CompiledMethodCache for IR pipeline function call emission
                if (root.Bindings.TryGetValue(functionName, out var binding))
                {
                    _compiledMethodCache.Add(binding, methodDefinition);
                }

                // Two-phase: register the token in the canonical CallableRegistry
                _twoPhaseCoordinator.Registry.SetDeclaredTokenForAstNode(functionDeclaration, (EntityHandle)methodDefinition);

                globalMethods.Add((functionName, methodDefinition, funcScope, functionVariables, nestedTb, firstNestedMethod));
            }
        }

        public MethodDefinitionHandle GenerateMethodForFunction(FunctionDeclaration functionDeclaration, Variables functionVariables, ILMethodGenerator methodGenerator, Scope functionScope, SymbolTable symbolTable, TypeBuilder typeBuilder, string? registryScopeNameOverride = null)
        {
            var functionName = (functionDeclaration.Id as Acornima.Ast.Identifier)!.Name;
            var registryScopeName = registryScopeNameOverride ?? functionVariables.GetCurrentScopeName();

            if (functionDeclaration.Body is not BlockStatement)
            {
                ILEmitHelpers.ThrowNotSupported($"Unsupported function body type: {functionDeclaration.Body.Type}", functionDeclaration.Body);
                throw new InvalidOperationException(); // unreachable, satisfies definite assignment
            }
            var blockStatement = (BlockStatement)functionDeclaration.Body;

            var methodCompiler = _serviceProvider.GetRequiredService<JsMethodCompiler>();
            var compiledMethod = methodCompiler.TryCompileMethod(typeBuilder, functionName, functionDeclaration, functionScope, _methodBodyStreamEncoder);
            IR.IRPipelineMetrics.RecordFunctionAttempt(!compiledMethod.IsNil);
            if (!compiledMethod.IsNil)
            {
                return compiledMethod;
            }
            // Generate method body normally

            var variables = functionVariables;
            var il = methodGenerator.IL;
            // Runtime helper to reference JavaScriptRuntime methods (e.g., Object.GetProperty)
            var runtime = new Js2IL.Services.Runtime(il, _serviceProvider.GetRequiredService<TypeReferenceRegistry>(), _serviceProvider.GetRequiredService<MemberReferenceRegistry>());

            // Parameters are already registered in Variables constructor

            // Create a scope instance for the function itself so that local vars (declared within the function)
            // have a backing scope object to store their fields. Only allocate if this function scope has fields.
            var registry = variables.GetVariableRegistry();
            
            // Initialize default parameter values FIRST (using starg), regardless of whether there are fields
            // This must happen before field initialization so captured parameters get the correct value
            methodGenerator.EmitDefaultParameterInitializers(functionDeclaration.Params, parameterStartIndex: 1);
            
            if (registry != null)
            {
                var fields = registry.GetVariablesForScope(registryScopeName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
                var hasAnyFields = fields.Any();
                if (hasAnyFields)
                {
                    // Create the leaf scope instance using the shared helper
                    ScopeInstanceEmitter.EmitCreateLeafScopeInstance(variables, il, _metadataBuilder);

                    // Initialize parameter fields on the scope from CLR arguments only when a backing field exists for the parameter.
                    var localScope = variables.GetLocalScopeSlot();
                    // Build a fast lookup of field-backed names in this scope (used for both field init and destructuring)
                    var fieldNames = localScope.Address >= 0 ? new HashSet<string>(fields.Select(f => f.Name)) : new HashSet<string>();
                    ushort jsParamSeq;
                    
                    if (localScope.Address >= 0)
                    {
                        // JS parameters start at arg1 (arg0 is scopes[])
                        jsParamSeq = 1;
                        // Initialize simple identifier and assignment pattern parameters into fields when applicable
                        for (int i = 0; i < functionDeclaration.Params.Count; i++)
                        {
                            var paramNode = functionDeclaration.Params[i];
                            // Extract identifier from Identifier or AssignmentPattern
                            Acornima.Ast.Identifier? pid = paramNode as Acornima.Ast.Identifier;
                            if (pid == null && paramNode is Acornima.Ast.AssignmentPattern ap)
                            {
                                pid = ap.Left as Acornima.Ast.Identifier;
                            }
                            
                            if (pid != null && fieldNames.Contains(pid.Name))
                            {
                                il.LoadLocal(localScope.Address);
                                methodGenerator.EmitLoadParameterWithDefault(paramNode, jsParamSeq);
                                var fieldHandle = registry.GetFieldHandle(registryScopeName, pid.Name);
                                il.OpCode(ILOpCode.Stfld);
                                il.Token(fieldHandle);
                            }
                            jsParamSeq++;
                        }
                    
                        // Destructure object-pattern parameters into their bound fields (shared helper)
                        MethodBuilder.EmitObjectPatternParameterDestructuring(
                            _metadataBuilder,
                            il,
                            runtime,
                            variables,
                            registryScopeName,
                            functionDeclaration.Params,
                            methodGenerator.ExpressionEmitter,
                            startingJsParamSeq: 1,
                            castScopeForStore: false);
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
            var (localSignature, bodyAttributes) = MethodBuilder.CreateLocalVariableSignature(_metadataBuilder, variables, this._bclReferences);

            var bodyoffset = _methodBodyStreamEncoder.AddMethodBody(
                il,
                maxStack: 32, // todo - keep track of the pops and pushes so as to provide a accurate value for maxStack
                localVariablesSignature: localSignature,
                attributes: bodyAttributes);

            // Build method signature: static object (object[] scopes, object param1, ...)
            // paramCount: scope array + declared params
            var paramCount = 1 + functionDeclaration.Params.Count;
            var methodSig = MethodBuilder.BuildMethodSignature(
                _metadataBuilder,
                isInstance: false,
                paramCount: paramCount,
                hasScopesParam: true,
                returnsVoid: false);

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
                MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.HideBySig,
                functionName,
                methodSig,
                bodyoffset,
                parameterList: firstParamHandle);

            return methodDefinition;
        }
    }
}
