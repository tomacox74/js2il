using Acornima.Ast;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
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
        private readonly CallableRegistry _callableRegistry;

        public FunctionRegistry FunctionRegistry => _functionRegistry;

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
            _callableRegistry = serviceProvider.GetRequiredService<CallableRegistry>();
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
                var paramNames = ExtractParameterNames(functionDeclaration.Params).ToArray();
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
                        var nestedParamNames = ExtractParameterNames(nestedDecl.Params).ToArray();
                        // Pre-register parameter count before generating method body
                        _functionRegistry.PreRegisterParameterCount(nestedName, nestedParamNames.Length);
                        var nestedRegistryScopeName = $"{moduleName}/{nestedName}";
                        var nestedVars = new Variables(functionVariables, nestedRegistryScopeName, nestedParamNames, isNestedFunction: true);
                        var nestedMethod = GenerateMethodForFunction(nestedDecl, nestedScope, nestedTb);
                        if (firstNestedMethod.IsNil) firstNestedMethod = nestedMethod;
                        if (this._firstMethod.IsNil) _firstMethod = nestedMethod;
                        _functionRegistry.Register(nestedName, nestedMethod, nestedParamNames.Length);

                        // Two-phase: register the token in the canonical CallableRegistry
                        _callableRegistry.SetDeclaredTokenForAstNode(nestedDecl, nestedMethod);
                    }
                }

                // Now generate the outer function method (nested handles already registered)
                var methodDefinition = GenerateMethodForFunction(functionDeclaration, funcScope, moduleTb);
                if (this._firstMethod.IsNil) _firstMethod = methodDefinition;
                _functionRegistry.Register(functionName, methodDefinition, paramNames.Length);

                // Two-phase: register the token in the canonical CallableRegistry
                _callableRegistry.SetDeclaredTokenForAstNode(functionDeclaration, methodDefinition);

                globalMethods.Add((functionName, methodDefinition, funcScope, functionVariables, nestedTb, firstNestedMethod));
            }
        }

        public MethodDefinitionHandle GenerateMethodForFunction(FunctionDeclaration functionDeclaration, Scope functionScope, TypeBuilder typeBuilder)
        {
            var functionName = (functionDeclaration.Id as Acornima.Ast.Identifier)!.Name;

            var methodCompiler = _serviceProvider.GetRequiredService<JsMethodCompiler>();
            var compiledMethod = methodCompiler.TryCompileMethod(typeBuilder, functionName, functionDeclaration, functionScope, _methodBodyStreamEncoder);
            IR.IRPipelineMetrics.RecordFunctionAttempt(!compiledMethod.IsNil);
            if (!compiledMethod.IsNil)
            {
                return compiledMethod;
            }

            throw new NotSupportedException(
                $"IR pipeline could not compile function '{functionName}' in scope '{functionScope.GetQualifiedName()}'.");
        }

        private static IEnumerable<string> ExtractParameterNames(IReadOnlyList<Node> parameters)
        {
            if (parameters == null)
            {
                yield break;
            }

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
                    yield return "param" + (index + 1);
                }

                index++;
            }
        }
    }
}
