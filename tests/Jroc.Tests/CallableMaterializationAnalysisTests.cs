using Jroc.Services;
using Jroc.SymbolTables;

namespace Jroc.Tests;

public sealed class CallableMaterializationAnalysisTests
{
    [Theory]
    [InlineData("const target = value => value + 1; target(2);")]
    [InlineData("const target = function (value) { return value + 1; }; target(2);")]
    public void DirectOnlyBindingsAreClassifiedWithoutBailouts(string source)
    {
        var binding = Analyze(source, "target");

        var decision = GetDecision(binding);
        Assert.Equal(CallableMaterializationKind.DirectOnly, decision.Kind);
        Assert.Equal(CallableMaterializationReason.None, decision.Reasons);
        Assert.Equal(1, decision.RuntimeUseCount);
        Assert.Equal(1, decision.DirectCallCount);
    }

    [Theory]
    [InlineData("const target = () => 1; { target(); }")]
    [InlineData("const target = () => 1; for (let index = 0; index < 1; index++) target();")]
    [InlineData("const target = () => 1; switch (0) { case 0: target(); break; }")]
    [InlineData("const target = () => 1; try { target(); } catch (error) {}")]
    public void OrdinaryDescendantLexicalScopesRemainDirectOnly(string source)
    {
        var decision = GetDecision(Analyze(source, "target"));

        Assert.Equal(CallableMaterializationKind.DirectOnly, decision.Kind);
        Assert.False(decision.Reasons.HasFlag(CallableMaterializationReason.CapturedValueRead));
        Assert.Equal(1, decision.RuntimeUseCount);
        Assert.Equal(1, decision.DirectCallCount);
    }

    [Theory]
    [InlineData("const target = () => 1; const invoke = () => target(); invoke();")]
    [InlineData(
        "const target = () => 1; const Holder = class { read() { return target(); } }; new Holder().read();")]
    public void NestedCallableReadCrossesCaptureBoundary(string source)
    {
        var decision = GetDecision(Analyze(source, "target"));

        Assert.Equal(CallableMaterializationKind.IdentityObservable, decision.Kind);
        Assert.Equal(CallableMaterializationReason.CapturedValueRead, decision.Reasons);
        Assert.Equal(1, decision.RuntimeUseCount);
        Assert.Equal(1, decision.DirectCallCount);
    }

    [Fact]
    public void ActiveWithCallSiteRequiresMaterialization()
    {
        var decision = GetDecision(Analyze(
            "const target = () => 1; with ({ target: () => 2 }) target();",
            "target"));

        Assert.Equal(CallableMaterializationKind.UnknownMaterialize, decision.Kind);
        Assert.Equal(CallableMaterializationReason.WithEnvironment, decision.Reasons);
        Assert.Equal(1, decision.RuntimeUseCount);
        Assert.Equal(0, decision.DirectCallCount);
    }

    [Fact]
    public void ForLoopHeadAndBodyCallsUseTheLoopBindingScope()
    {
        var decision = GetDecision(Analyze(
            "for (const target = () => false; target();) target();",
            "target"));

        Assert.Equal(CallableMaterializationKind.DirectOnly, decision.Kind);
        Assert.Equal(CallableMaterializationReason.None, decision.Reasons);
        Assert.Equal(2, decision.RuntimeUseCount);
        Assert.Equal(2, decision.DirectCallCount);
    }

    [Fact]
    public void SwitchDiscriminantAndCaseCallsUseTheirRespectiveBindings()
    {
        const string source = """
            const target = () => 0;
            switch (target()) {
                case 0:
                    const target = () => 1;
                    target();
                    break;
            }
            """;
        var root = Build(source).SymbolTable!.Root;
        var switchScope = Assert.Single(
            root.Children,
            scope => scope.AstNode is Acornima.Ast.SwitchStatement);
        var outerDecision = GetDecision(root.Bindings["target"]);
        var caseDecision = GetDecision(switchScope.Bindings["target"]);

        Assert.Equal(CallableMaterializationKind.DirectOnly, outerDecision.Kind);
        Assert.Equal(1, outerDecision.RuntimeUseCount);
        Assert.Equal(1, outerDecision.DirectCallCount);
        Assert.Equal(CallableMaterializationKind.DirectOnly, caseDecision.Kind);
        Assert.Equal(1, caseDecision.RuntimeUseCount);
        Assert.Equal(1, caseDecision.DirectCallCount);
    }

    [Theory]
    [InlineData(
        "const target = () => 1; module.exports = target;",
        CallableMaterializationReason.Export)]
    [InlineData(
        "const target = () => 1; consume(target);",
        CallableMaterializationReason.UnknownArgument)]
    [InlineData(
        "const target = () => 1; const holder = { value: target };",
        CallableMaterializationReason.PropertyStorage)]
    [InlineData(
        "const target = () => 1; const holder = [target];",
        CallableMaterializationReason.ArrayStorage)]
    [InlineData(
        "function outer() { const target = () => 1; return target; } outer();",
        CallableMaterializationReason.Return)]
    [InlineData(
        "const target = () => 1; const alias = target;",
        CallableMaterializationReason.Alias)]
    [InlineData(
        "const target = () => 1; target.bind(null);",
        CallableMaterializationReason.CallApplyBind)]
    [InlineData(
        "const target = () => 1; target.call(null);",
        CallableMaterializationReason.CallApplyBind)]
    [InlineData(
        "const target = () => 1; target.apply(null, []);",
        CallableMaterializationReason.CallApplyBind)]
    [InlineData(
        "const target = () => 1; console.log(target.name);",
        CallableMaterializationReason.Reflection)]
    [InlineData(
        "const target = () => target(); target();",
        CallableMaterializationReason.RecursiveReference)]
    [InlineData(
        "const target = () => 1; target?.();",
        CallableMaterializationReason.OptionalCall)]
    [InlineData(
        "const target = value => value; target(...[1]);",
        CallableMaterializationReason.SpreadCall)]
    [InlineData(
        "let target = () => 1; target = () => 2; target();",
        CallableMaterializationReason.Reassigned)]
    [InlineData(
        "const target = () => 1; function invoke() { return target(); } invoke();",
        CallableMaterializationReason.CapturedValueRead)]
    public void ObservableOrUncertainUsesRequireMaterialization(
        string source,
        CallableMaterializationReason expectedReason)
    {
        var binding = Analyze(source, "target");

        var decision = GetDecision(binding);
        Assert.NotEqual(CallableMaterializationKind.DirectOnly, decision.Kind);
        Assert.True(
            decision.Reasons.HasFlag(expectedReason),
            $"Expected {expectedReason}, actual: {decision.ToDiagnosticText()}");
    }

    [Fact]
    public void MutuallyRecursiveConstArrowsRequireMaterialization()
    {
        const string source = """
            const left = () => right();
            const right = () => left();
            left();
            """;

        var left = Analyze(source, "left");
        var right = Analyze(source, "right");

        Assert.All(
            new[] { left, right },
            binding =>
            {
                var decision = GetDecision(binding);
                Assert.NotEqual(CallableMaterializationKind.DirectOnly, decision.Kind);
                Assert.True(
                    decision.Reasons.HasFlag(
                        CallableMaterializationReason.MutuallyRecursiveScc));
            });
    }

    [Fact]
    public void DiagnosticTextIsDeterministic()
    {
        var binding = Analyze(
            "const target = () => 1; const alias = target;",
            "target");

        Assert.Equal(
            "IdentityObservable; uses=1; direct-calls=0; reasons=Alias",
            binding.CallableMaterialization!.ToDiagnosticText());
    }

    private static BindingInfo Analyze(string source, string bindingName)
    {
        var module = Build(source);

        return EnumerateScopes(module.SymbolTable!.Root)
            .SelectMany(scope => scope.Bindings.Values)
            .First(binding => string.Equals(
                binding.Name,
                bindingName,
                StringComparison.Ordinal)
                && binding.CallableMaterialization != null);
    }

    private static ModuleDefinition Build(string source)
    {
        var parser = new JavaScriptParser();
        var program = parser.ParseJavaScript(source, "callable-materialization.js");
        var module = new ModuleDefinition
        {
            Ast = program,
            Path = "callable-materialization.js",
            Name = "callable-materialization",
            ModuleId = "callable-materialization"
        };
        new SymbolTableBuilder().Build(module);
        return module;
    }

    private static CallableMaterializationDecision GetDecision(BindingInfo binding)
        => Assert.IsType<CallableMaterializationDecision>(
            binding.CallableMaterialization);

    private static IEnumerable<Jroc.SymbolTables.Scope> EnumerateScopes(
        Jroc.SymbolTables.Scope scope)
    {
        yield return scope;
        foreach (var child in scope.Children)
        {
            foreach (var descendant in EnumerateScopes(child))
            {
                yield return descendant;
            }
        }
    }
}
