using Jroc.Services;
using Jroc.Services.TwoPhaseCompilation;
using Jroc.SymbolTables;
using System.Reflection.Metadata.Ecma335;

namespace Jroc.Tests.Integration;

public sealed class GeneratedFunctionObjectPlanningTests
{
    [Fact]
    public void PlannerShapesCapturesLexicalStateAndConstructabilityPerCallee()
    {
        const string source = """
            let outerValue = 1;
            const captureFree = value => value;
            const capturing = () => outerValue;
            const lexical = () => this;

            function ordinary(value) {
                return value;
            }

            class Example {
                method(value) {
                    return value;
                }
            }
            """;

        var (symbolTable, coordinator, registry) = Build(source);
        coordinator.RunPhase1Discovery(symbolTable);
        coordinator.RunPhase1Discovery(symbolTable);

        var plans = registry.GetPlansInStableOrder();
        Assert.Equal(plans.Count, plans.Select(plan => plan.Callable).Distinct().Count());

        var arrows = plans
            .Where(plan => plan.Callable.Kind == CallableKind.Arrow)
            .ToArray();
        Assert.Equal(3, arrows.Length);

        var captureFree = Assert.Single(
            arrows,
            plan => plan.Captures.Count == 0 && plan.StateFields.Count == 0);
        Assert.False(captureFree.IsConstructable);

        var capturing = Assert.Single(arrows, plan => plan.Captures.Count == 1);
        var capture = Assert.Single(capturing.Captures);
        Assert.Contains(symbolTable.Root.Name, capture.ScopeName, StringComparison.Ordinal);
        Assert.StartsWith("_environment", capture.FieldName, StringComparison.Ordinal);

        var lexical = Assert.Single(
            arrows,
            plan => plan.StateFields.Any(
                field => field.Kind == GeneratedFunctionStateKind.LexicalThis));
        Assert.Equal(
            ["_lexicalThis"],
            lexical.StateFields.Select(field => field.FieldName));
        Assert.True(lexical.RequiresInvocationContext);

        var ordinary = Assert.Single(
            plans,
            plan => plan.Callable.Kind == CallableKind.FunctionDeclaration
                && plan.Callable.Name == "ordinary");
        Assert.True(ordinary.IsConstructable);
        Assert.Empty(ordinary.StateFields);

        var method = Assert.Single(
            plans,
            plan => plan.Callable.Kind == CallableKind.ClassMethod
                && plan.Callable.Name == "Example.method");
        Assert.True(method.Signature.IsInstanceMethod);
        Assert.False(method.IsConstructable);
    }

    [Fact]
    public void PlannerDistinguishesJavaScriptVisibleReturnFamilies()
    {
        const string source = """
            async function promiseValue() { return 1; }
            function* generatorValue() { yield 1; }
            async function* asyncGeneratorValue() { yield 1; }
            function ordinaryValue() { return 1; }
            """;

        var (symbolTable, coordinator, registry) = Build(source);
        coordinator.RunPhase1Discovery(symbolTable);

        var byName = registry.GetPlansInStableOrder()
            .Where(plan => plan.Callable.Name is not null)
            .ToDictionary(plan => plan.Callable.Name!, StringComparer.Ordinal);

        Assert.Equal(GeneratedFunctionReturnKind.Promise, byName["promiseValue"].ReturnKind);
        Assert.Equal(GeneratedFunctionReturnKind.Generator, byName["generatorValue"].ReturnKind);
        Assert.Equal(
            GeneratedFunctionReturnKind.AsyncGenerator,
            byName["asyncGeneratorValue"].ReturnKind);
        Assert.Equal(GeneratedFunctionReturnKind.Value, byName["ordinaryValue"].ReturnKind);

        Assert.False(byName["promiseValue"].IsConstructable);
        Assert.False(byName["generatorValue"].IsConstructable);
        Assert.False(byName["asyncGeneratorValue"].IsConstructable);
        Assert.True(byName["ordinaryValue"].IsConstructable);
    }

    [Fact]
    public void SpecializedEntryPointsRemainOnCanonicalGeneratedType()
    {
        var (symbolTable, coordinator, registry) = Build(
            "function value(input) { return input; }");
        coordinator.RunPhase1Discovery(symbolTable);
        var plan = Assert.Single(registry.GetPlansInStableOrder());
        var typeHandle = MetadataTokens.TypeDefinitionHandle(1);
        registry.SetMetadata(new GeneratedFunctionObjectMetadata
        {
            Plan = plan,
            TypeHandle = typeHandle,
            CanonicalOwnerTypeHandle = MetadataTokens.TypeDefinitionHandle(2),
            ConstructorHandle = MetadataTokens.MethodDefinitionHandle(1),
            IsConstructorGetterHandle = MetadataTokens.MethodDefinitionHandle(2),
            RequiresInvocationContextGetterHandle = MetadataTokens.MethodDefinitionHandle(3),
            CallAdapterHandle = MetadataTokens.MethodDefinitionHandle(4),
            ConstructAdapterHandle = MetadataTokens.MethodDefinitionHandle(5),
            FieldHandles = new Dictionary<string, System.Reflection.Metadata.FieldDefinitionHandle>(),
            EntryPoints =
            [
                new GeneratedFunctionEntryPointPlan(
                    "__js_call__",
                    [null],
                    null,
                    MetadataTokens.MethodDefinitionHandle(6))
            ]
        });

        registry.AddSpecializedEntryPoint(
            plan.Callable,
            new GeneratedFunctionEntryPointPlan(
                "__js_call__number",
                [typeof(double)],
                typeof(double),
                MetadataTokens.MethodDefinitionHandle(7)));

        var metadata = registry.GetMetadata(plan.Callable);
        Assert.Equal(typeHandle, metadata.TypeHandle);
        Assert.Equal(2, metadata.EntryPoints.Count);
        Assert.Equal(
            ["__js_call__", "__js_call__number"],
            metadata.EntryPoints.Select(entryPoint => entryPoint.Name));
    }

    private static (
        SymbolTable SymbolTable,
        TwoPhaseCompilationCoordinator Coordinator,
        GeneratedFunctionObjectRegistry Registry) Build(string source)
    {
        var parser = new JavaScriptParser();
        var module = new ModuleDefinition
        {
            Ast = parser.ParseJavaScript(source, "generated-function-objects.js"),
            Name = "GeneratedFunctionObjects",
            Path = "generated-function-objects.js",
            ModuleId = "GeneratedFunctionObjects"
        };

        new SymbolTableBuilder().Build(module);
        var symbolTable = module.SymbolTable
            ?? throw new InvalidOperationException("Symbol table was not built.");
        var registry = new GeneratedFunctionObjectRegistry();
        var coordinator = new TwoPhaseCompilationCoordinator(
            new CompilerOptions(),
            new CallableRegistry(),
            generatedFunctionObjectRegistry: registry);
        return (symbolTable, coordinator, registry);
    }
}
