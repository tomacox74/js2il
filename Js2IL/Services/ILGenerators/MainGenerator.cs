using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Acornima.Ast;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Generates the Main function which is teh entry point for execution
    /// </summary>
    internal class MainGenerator
    {
        private ILMethodGenerator _ilGenerator;
        private JavaScriptFunctionGenerator _functionGenerator;
        private ClassesGenerator _classesGenerator;
        private MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private SymbolTable _symbolTable;

        private readonly Variables _rootVariables;
        private readonly ILogger _logger;
        private readonly bool _verbose;

        private BaseClassLibraryReferences _bclReferences;

        private readonly ClassRegistry _classRegistry = new();
        
        private readonly TwoPhaseCompilationCoordinator? _twoPhaseCoordinator;
        private readonly IServiceProvider _serviceProvider;

        public MainGenerator(IServiceProvider serviceProvider, Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, SymbolTable symbolTable)
        {
            _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));

            _rootVariables = variables ?? throw new ArgumentNullException(nameof(variables));

            if (bclReferences == null) throw new ArgumentNullException(nameof(bclReferences));
            if (metadataBuilder == null) throw new ArgumentNullException(nameof(metadataBuilder));
            
            _serviceProvider = serviceProvider;
            _bclReferences = bclReferences;
            _methodBodyStreamEncoder = methodBodyStreamEncoder;
            var compilerOptions = serviceProvider.GetRequiredService<CompilerOptions>();
            _verbose = compilerOptions.Verbose;
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _functionGenerator = new JavaScriptFunctionGenerator(serviceProvider, variables, bclReferences, metadataBuilder, methodBodyStreamEncoder, _classRegistry, symbolTable);
            _ilGenerator = new ILMethodGenerator(serviceProvider, variables, bclReferences, metadataBuilder, methodBodyStreamEncoder, _classRegistry, _functionGenerator.FunctionRegistry, symbolTable: symbolTable);
            _classesGenerator = new ClassesGenerator(serviceProvider,metadataBuilder, bclReferences, methodBodyStreamEncoder, _classRegistry, variables);
            
            // Initialize two-phase coordinator if enabled
            if (compilerOptions.TwoPhaseCompilation)
            {
                _twoPhaseCoordinator = serviceProvider.GetRequiredService<TwoPhaseCompilationCoordinator>();
            }
        }

        private void DeclarePhase1AnonymousCallables(IReadOnlyList<CallableId> callables, MetadataBuilder metadataBuilder)
        {
            // Milestone 1 (Option B): Phase 1 is declare-only.
            // We emit MemberRef tokens for arrow functions and function expressions so that
            // expression emission can always do lookup-only (no compilation).

            var registry = _twoPhaseCoordinator?.Registry;
            if (registry == null)
            {
                return;
            }

            // Use Module scope for TypeRef resolution so we can reference types that will be emitted later.
            // The single module row is always index 1.
            var moduleHandle = MetadataTokens.EntityHandle(TableIndex.Module, 1);

            // Cache TypeRefs by "namespace/name" to avoid duplicates.
            var typeRefCache = new System.Collections.Generic.Dictionary<string, TypeReferenceHandle>(StringComparer.Ordinal);

            foreach (var callable in callables)
            {
                switch (callable.Kind)
                {
                    case CallableKind.Arrow:
                        // Skip if already has a token (idempotent); Phase 2 may overwrite MemberRef with MethodDef.
                        if (registry.TryGetDeclaredToken(callable, out var existingToken) && !existingToken.IsNil)
                        {
                            continue;
                        }

                        // Build a MemberRef to Functions.<ILMethodName>::<ILMethodName>(object[] scopes, object...)
                        var signature = registry.GetSignature(callable);
                        if (signature == null)
                        {
                            continue;
                        }

                        var ilMethodName = signature.ILMethodName;
                        var typeKey = $"Functions/{ilMethodName}";
                        if (!typeRefCache.TryGetValue(typeKey, out var typeRef))
                        {
                            typeRef = metadataBuilder.AddTypeReference(
                                moduleHandle,
                                metadataBuilder.GetOrAddString("Functions"),
                                metadataBuilder.GetOrAddString(ilMethodName));
                            typeRefCache[typeKey] = typeRef;
                        }

                        var paramCount = 1 + callable.JsParamCount;
                        var methodSig = MethodBuilder.BuildMethodSignature(
                            metadataBuilder,
                            isInstance: false,
                            paramCount: paramCount,
                            hasScopesParam: true,
                            returnsVoid: false);

                        var memberRef = metadataBuilder.AddMemberReference(
                            typeRef,
                            metadataBuilder.GetOrAddString(ilMethodName),
                            methodSig);

                        registry.SetToken(callable, (EntityHandle)memberRef);
                        break;

                    case CallableKind.FunctionExpression:
                        if (registry.TryGetDeclaredToken(callable, out var existingToken2) && !existingToken2.IsNil)
                        {
                            continue;
                        }

                        var signature2 = registry.GetSignature(callable);
                        if (signature2 == null)
                        {
                            continue;
                        }

                        var ilMethodName2 = signature2.ILMethodName;
                        var typeKey2 = $"Functions/{ilMethodName2}";
                        if (!typeRefCache.TryGetValue(typeKey2, out var typeRef2))
                        {
                            typeRef2 = metadataBuilder.AddTypeReference(
                                moduleHandle,
                                metadataBuilder.GetOrAddString("Functions"),
                                metadataBuilder.GetOrAddString(ilMethodName2));
                            typeRefCache[typeKey2] = typeRef2;
                        }

                        var paramCount2 = 1 + callable.JsParamCount;
                        var methodSig2 = MethodBuilder.BuildMethodSignature(
                            metadataBuilder,
                            isInstance: false,
                            paramCount: paramCount2,
                            hasScopesParam: true,
                            returnsVoid: false);

                        var memberRef2 = metadataBuilder.AddMemberReference(
                            typeRef2,
                            metadataBuilder.GetOrAddString(ilMethodName2),
                            methodSig2);

                        registry.SetToken(callable, (EntityHandle)memberRef2);
                        break;
                }
            }
        }

        private void CompilePhase2AnonymousCallables(IReadOnlyList<CallableId> callables, MetadataBuilder metadataBuilder)
        {
            foreach (var callable in callables)
            {
                switch (callable.Kind)
                {
                    case CallableKind.Arrow:
                        if (callable.AstNode is not ArrowFunctionExpression arrowExpr)
                        {
                            continue;
                        }
                        DeclareArrowFunction(callable, arrowExpr, metadataBuilder);
                        break;

                    case CallableKind.FunctionExpression:
                        if (callable.AstNode is not FunctionExpression funcExpr)
                        {
                            continue;
                        }
                        DeclareFunctionExpression(callable, funcExpr, metadataBuilder);
                        break;
                }
            }
        }

        private void DeclareArrowFunction(CallableId callable, ArrowFunctionExpression arrowExpr, MetadataBuilder metadataBuilder)
        {
            var paramNames = ILMethodGenerator.ExtractParameterNames(arrowExpr.Params).ToArray();
            var moduleName = _symbolTable.Root.Name;
            var arrowBaseScopeName = callable.Name != null
                ? $"ArrowFunction_{callable.Name}"
                : $"ArrowFunction_L{arrowExpr.Location.Start.Line}C{arrowExpr.Location.Start.Column}";
            var registryScopeName = $"{moduleName}/{arrowBaseScopeName}";
            var ilMethodName = $"ArrowFunction_L{arrowExpr.Location.Start.Line}C{arrowExpr.Location.Start.Column}";

            var arrowGen = new JavaScriptArrowFunctionGenerator(
                _serviceProvider,
                _rootVariables,
                _bclReferences,
                metadataBuilder,
                _methodBodyStreamEncoder,
                _classRegistry,
                _functionGenerator.FunctionRegistry,
                _symbolTable);

            arrowGen.GenerateArrowFunctionMethod(arrowExpr, registryScopeName, ilMethodName, paramNames);

            if (_verbose)
            {
                _logger.WriteLine($"[TwoPhase] Phase 1: Declared arrow: {ilMethodName}");
            }
        }

        private void DeclareFunctionExpression(CallableId callable, FunctionExpression funcExpr, MetadataBuilder metadataBuilder)
        {
            var paramNames = ILMethodGenerator.ExtractParameterNames(funcExpr.Params).ToArray();
            string baseScopeName;
            if (funcExpr.Id is Identifier fid && !string.IsNullOrEmpty(fid.Name))
            {
                baseScopeName = fid.Name;
            }
            else if (callable.Name != null)
            {
                baseScopeName = $"FunctionExpression_{callable.Name}";
            }
            else
            {
                baseScopeName = $"FunctionExpression_L{funcExpr.Location.Start.Line}C{funcExpr.Location.Start.Column}";
            }

            var moduleName = _symbolTable.Root.Name;
            var registryScopeName = $"{moduleName}/{baseScopeName}";
            var ilMethodName = $"FunctionExpression_L{funcExpr.Location.Start.Line}C{funcExpr.Location.Start.Column}";

            var methodGen = new ILMethodGenerator(
                _serviceProvider,
                _rootVariables,
                _bclReferences,
                metadataBuilder,
                _methodBodyStreamEncoder,
                _classRegistry,
                _functionGenerator.FunctionRegistry,
                symbolTable: _symbolTable);

            methodGen.GenerateFunctionExpressionMethod(funcExpr, registryScopeName, ilMethodName, paramNames);

            if (_verbose)
            {
                _logger.WriteLine($"[TwoPhase] Phase 1: Declared function expression: {ilMethodName}");
            }
        }

        private bool IsInsideClassScope(CallableId callable)
        {
            if (callable.AstNode == null)
            {
                return false;
            }

            var scope = _symbolTable.FindScopeByAstNode(callable.AstNode);
            while (scope != null)
            {
                if (scope.Kind == ScopeKind.Class)
                {
                    return true;
                }
                scope = scope.Parent;
            }
            return false;
        }

        /// <summary>
        /// Determines if the global scope instance needs to be created.
        /// The instance is only needed when:
        /// 1. Any global variable is captured (referenced from nested functions/classes), OR
        /// 2. Any class or function needs parent scope access (ReferencesParentScopeVariables), OR
        /// 3. There are function declarations at global scope (stored as scope fields)
        /// </summary>
        private bool ShouldCreateGlobalScopeInstance()
        {
            var globalScope = _symbolTable.Root;
            
            // Check if any global binding is captured (accessed from nested scope)
            // OR if any binding is a function declaration (stored as scope field)
            foreach (var binding in globalScope.Bindings.Values)
            {
                if (binding.IsCaptured)
                {
                    return true;
                }
                // Function declarations are stored as scope fields
                if (binding.Kind == BindingKind.Function)
                {
                    return true;
                }
            }
            
            // Check if any child scope (function or class) references parent scope variables
            foreach (var child in globalScope.Children)
            {
                if (child.ReferencesParentScopeVariables)
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Creates the global scope instance.
        /// The instance is stored in a local variable that can be accessed by variable operations.
        /// </summary>
        private void CreateGlobalScopeInstance(Variables variables)
        {
            // Delegate to shared helper; safe no-op if no registry or scope type is available
            ScopeInstanceEmitter.EmitCreateLeafScopeInstance(variables, _ilGenerator.IL, _ilGenerator.MetadataBuilder);
        }

        /// <summary>
        /// Declares classes and functions, populating the CompiledMethodCache.
        /// This must be called before attempting IR compilation of the main method,
        /// because the IR pipeline relies on CompiledMethodCache to obtain compiled function
        /// method handles when emitting function call instructions.
        /// </summary>
        public void DeclareClassesAndFunctions(SymbolTable symbolTable)
        {
            if (_twoPhaseCoordinator != null)
            {
                // Two-phase path: discover callables, then declare all in Phase 1
                _twoPhaseCoordinator.RunPhase1Discovery(symbolTable);
                
                // Phase 1: Declaration (declare-only; no bodies)
                var discoveredCallables = _twoPhaseCoordinator.DiscoveredCallables;
                if (discoveredCallables != null)
                {
                    // Phase 1: declare all arrow functions and function expressions (MemberRef tokens)
                    DeclarePhase1AnonymousCallables(discoveredCallables, _ilGenerator.MetadataBuilder);
                }

                // Enable strict mode before any body compilation so that expression emission cannot compile.
                _twoPhaseCoordinator.EnableStrictMode();

                // Phase 2: Compile bodies using existing generators.
                _twoPhaseCoordinator.RunPhase2BodyCompilation(() =>
                {
                    if (discoveredCallables != null)
                    {
                        CompilePhase2AnonymousCallables(discoveredCallables, _ilGenerator.MetadataBuilder);
                    }

                    _classesGenerator.DeclareClasses(symbolTable);
                    _functionGenerator.DeclareFunctions(symbolTable);
                });
            }
            else
            {
                // Legacy path: declare classes first so their types exist under the Classes namespace
                _classesGenerator.DeclareClasses(symbolTable);

                // Declare functions (emits static method definitions and populates CompiledMethodCache)
                _functionGenerator.DeclareFunctions(symbolTable);
            }
        }

        /// <summary>
        /// Generates the main method body using the legacy IL emitter.
        /// Call DeclareClassesAndFunctions first.
        /// </summary>
        public int GenerateMethodBody(Acornima.Ast.Program ast)
        {
            var metadataBuilder = _ilGenerator.MetadataBuilder;
            var variables = _ilGenerator.Variables;

            // Step 1: Only create the global scope instance if it's actually needed
            // (i.e., variables are captured or classes/functions need parent scope access)
            if (ShouldCreateGlobalScopeInstance())
            {
                CreateGlobalScopeInstance(variables);
            }

            // Initialize top-level function variables directly (no dispatch table indirection)
            _ilGenerator.InitializeLocalFunctionVariables(ast.Body.OfType<Acornima.Ast.FunctionDeclaration>());

            _ilGenerator.GenerateStatementsForBody(variables.GetLeafScopeName(), false, ast.Body);

            _ilGenerator.IL.OpCode(ILOpCode.Ret);

            // local variables
            var (localSignature, methodBodyAttributes) = MethodBuilder.CreateLocalVariableSignature(metadataBuilder, variables, this._bclReferences);

            return _methodBodyStreamEncoder.AddMethodBody(
                _ilGenerator.IL,
                maxStack: 32,
                localVariablesSignature: localSignature,
                attributes: methodBodyAttributes);
        }

        /// <summary>
        /// Generates the complete main method including class/function declarations and method body.
        /// This is the original combined method for backward compatibility.
        /// </summary>
        public int GenerateMethod(Acornima.Ast.Program ast)
        {
            DeclareClassesAndFunctions(_symbolTable);
            return GenerateMethodBody(ast);
        }
    }
}
