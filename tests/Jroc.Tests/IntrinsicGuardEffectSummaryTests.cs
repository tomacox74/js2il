using Acornima;
using Jroc.Services;
using Jroc.SymbolTables;
using Xunit;

namespace Jroc.Tests;

public sealed class IntrinsicGuardEffectSummaryTests
{
    [Fact]
    public void Build_PropagatesMutationThroughDirectCallGraph()
    {
        var symbolTable = Build(
            """
            const mutate = function () {
                if (true) {
                    String.prototype.charAt = function () { return "x"; };
                }
            };
            const caller = function () {
                mutate();
            };
            caller();
            """);

        var callableScopes = symbolTable.Root.Children
            .Where(scope => scope.Kind == ScopeKind.Function)
            .ToArray();
        var mutateInitializer = Assert.IsAssignableFrom<
            Acornima.Ast.VariableDeclarator>(
            symbolTable.Root.Bindings["mutate"].DeclarationNode).Init;
        var mutate = Assert.Single(
            callableScopes,
            scope => ReferenceEquals(
                scope.AstNode,
                mutateInitializer));
        var caller = Assert.Single(
            callableScopes,
            scope => !ReferenceEquals(scope, mutate)
                && scope.AstNode is
                    Acornima.Ast.FunctionExpression);

        Assert.True(
            mutate.IntrinsicGuardEffects.Effects.HasFlag(
                IntrinsicGuardEffects
                    .MutatesIntrinsicPrototypeOrLink));
        Assert.True(
            caller.IntrinsicGuardEffects.Effects.HasFlag(
                IntrinsicGuardEffects
                    .MutatesIntrinsicPrototypeOrLink),
            $"caller effects: {caller.IntrinsicGuardEffects.Effects}; "
            + $"mutate materialization: "
            + $"{symbolTable.Root.Bindings["mutate"].CallableMaterialization}");
    }

    [Fact]
    public void Build_PropagatesPureDirectCallAsGuardHoistSafe()
    {
        var symbolTable = Build(
            """
            const increment = value => value + 1;
            const caller = value => increment(value);
            caller(4);
            """);

        var callableScopes = symbolTable.Root.Children
            .Where(scope => scope.Kind == ScopeKind.Function)
            .ToArray();
        Assert.Equal(2, callableScopes.Length);

        Assert.All(
            callableScopes,
            scope => Assert.True(
                scope.IntrinsicGuardEffects.IsGuardHoistSafe,
                $"effects: {scope.IntrinsicGuardEffects.Effects}; "
                + $"increment materialization: "
                + $"{symbolTable.Root.Bindings["increment"].CallableMaterialization}"));
    }

    [Theory]
    [InlineData(
        "async function work() { await 1; }",
        (int)IntrinsicGuardEffects.MaySuspendOrYield)]
    [InlineData(
        "function work(value) { Object.setPrototypeOf(value, null); }",
        (int)IntrinsicGuardEffects.MutatesIntrinsicPrototypeOrLink)]
    [InlineData(
        "function work(value) { unknown(value); }",
        (int)IntrinsicGuardEffects.InvokesUnknownOrEscapedCode)]
    public void Build_RecordsConservativeBarriers(
        string source,
        int expectedValue)
    {
        var symbolTable = Build(source);
        var work = Assert.Single(
            symbolTable.Root.Children,
            scope => scope.Name == "work");

        Assert.True(
            work.IntrinsicGuardEffects.Effects.HasFlag(
                (IntrinsicGuardEffects)expectedValue));
    }

    [Fact]
    public void Build_TreatsObservablePropertyReadAsUnknownCode()
    {
        var symbolTable = Build(
            "function work(value) { return value.member; }");
        var work = Assert.Single(
            symbolTable.Root.Children,
            scope => scope.Name == "work");

        Assert.True(
            work.IntrinsicGuardEffects.Effects.HasFlag(
                IntrinsicGuardEffects
                    .InvokesUnknownOrEscapedCode));
    }

    [Fact]
    public void Build_PropagatesClassStaticBlockEffects()
    {
        var symbolTable = Build(
            """
            function work() {
                class Mutation {
                    static {
                        Array.prototype.push = function () {};
                    }
                }
            }
            """);
        var work = Assert.Single(
            symbolTable.Root.Children,
            scope => scope.Name == "work");

        Assert.True(
            work.IntrinsicGuardEffects.Effects.HasFlag(
                IntrinsicGuardEffects
                    .MutatesIntrinsicPrototypeOrLink));
    }

    private static SymbolTable Build(string source)
    {
        var parser = new JavaScriptParser();
        var program = parser.ParseJavaScript(
            source,
            "intrinsic-guard-effects.js");
        var module = new ModuleDefinition
        {
            Ast = program,
            Path = "intrinsic-guard-effects.js",
            Name = "IntrinsicGuardEffects",
            ModuleId = "IntrinsicGuardEffects"
        };
        new SymbolTableBuilder().Build(module);
        return module.SymbolTable!;
    }
}
