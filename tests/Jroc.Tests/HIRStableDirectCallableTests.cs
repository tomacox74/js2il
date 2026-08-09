using Acornima;
using Acornima.Ast;
using Jroc.HIR;
using Jroc.Services;
using Jroc.Services.TwoPhaseCompilation;
using Jroc.SymbolTables;
using AstNode = Acornima.Ast.Node;
using ScopesCallableKind = Jroc.Services.ScopesAbi.CallableKind;

namespace Jroc.Tests;

public class HIRStableDirectCallableTests
{
    [Theory]
    [InlineData("const target = value => value + 1; target(2);")]
    [InlineData("const target = function (value, ...rest) { return value + rest.length; }; target(2);")]
    public void StableConstCallableCall_CarriesCanonicalDirectTarget(string source)
    {
        var (program, module, method) = ParseProgram(source);
        var call = GetTopLevelCall(method);
        var target = Assert.IsType<HIRStableDirectCallableTarget>(
            call.StableDirectCallableTarget);
        var declaration = Assert.Single(
            method.Body.Statements.OfType<HIRVariableDeclaration>());
        var callableValue = Assert.IsAssignableFrom<HIRExpression>(
            declaration.Initializer);
        var materialization = callableValue switch
        {
            HIRArrowFunctionExpression arrow => arrow.MaterializationDecision,
            HIRFunctionExpression function => function.MaterializationDecision,
            _ => throw new Xunit.Sdk.XunitException(
                $"Unexpected callable HIR value {callableValue.GetType().Name}.")
        };
        Assert.Equal(CallableMaterializationKind.DirectOnly, materialization.Kind);
        var declarator = Assert.IsType<VariableDeclaration>(program.Body[0]).Declarations[0];
        var initializer = Assert.IsAssignableFrom<AstNode>(declarator.Init);
        var discoveredCallable = Assert.Single(
            new CallableDiscovery(module.SymbolTable!)
                .DiscoverAll(),
            callable => ReferenceEquals(callable.AstNode, initializer));

        AssertCallableMatchesPhase1(discoveredCallable, target.CallableId);
        Assert.Same(initializer, target.CallableId.AstNode);
        Assert.Same(initializer, target.CallableScope.AstNode);
    }

    [Theory]
    [InlineData("target(); const target = () => 1;")]
    [InlineData("const target = (left, right) => left + right; target(...[1, 2]);")]
    [InlineData("const target = () => this; target();")]
    [InlineData("const target = function () { \"use strict\"; return 1; }; target();")]
    [InlineData("const target = async () => 1; target();")]
    [InlineData("const target = function* () { yield 1; }; target();")]
    [InlineData("const target = function self() { return self; }; target();")]
    public void UnsafeStableConstCallableCall_DoesNotCarryDirectTarget(string source)
    {
        var (_, _, method) = ParseProgram(source);

        Assert.Null(GetTopLevelCall(method).StableDirectCallableTarget);
    }

    [Fact]
    public void CallableCreatedInWithEnvironment_DoesNotCarryDirectTarget()
    {
        var (_, _, method) = ParseProgram(
            "with ({}) { const target = () => 1; target(); }");
        var withStatement = Assert.Single(method.Body.Statements.OfType<HIRWithStatement>());
        var call = Assert.Single(EnumerateCalls(withStatement.Body));

        Assert.Null(call.StableDirectCallableTarget);
    }

    [Fact]
    public void CallableResolvedThroughActiveWithEnvironment_DoesNotCarryDirectTarget()
    {
        var (_, _, method) = ParseProgram(
            "const target = () => 1; with ({ target: () => 2 }) target();");
        var withStatement = Assert.Single(method.Body.Statements.OfType<HIRWithStatement>());
        var call = Assert.Single(EnumerateCalls(withStatement.Body));
        var declaration = Assert.Single(
            method.Body.Statements.OfType<HIRVariableDeclaration>());
        var arrow = Assert.IsType<HIRArrowFunctionExpression>(declaration.Initializer);

        Assert.Null(call.StableDirectCallableTarget);
        Assert.Equal(
            CallableMaterializationKind.UnknownMaterialize,
            arrow.MaterializationDecision.Kind);
        Assert.Equal(
            CallableMaterializationReason.WithEnvironment,
            arrow.MaterializationDecision.Reasons);
    }

    [Fact]
    public void HoistedFunctionCallBeforeConstInitialization_DoesNotCarryDirectTarget()
    {
        const string source = """
            callHoisted();
            const target = () => 1;
            function callHoisted() {
                return target();
            }
            """;
        var parser = new JavaScriptParser();
        var program = parser.ParseJavaScript(source, "hir-stable-call.js");
        var module = CreateModule(program);
        new SymbolTableBuilder().Build(module);
        var function = Assert.IsType<FunctionDeclaration>(program.Body[2]);
        var functionScope = Assert.Single(
            module.SymbolTable!.Root.Children,
            scope => ReferenceEquals(scope.AstNode, function));

        Assert.True(HIRBuilder.TryParseMethod(
            function,
            functionScope,
            ScopesCallableKind.Function,
            hasScopesParameter: true,
            out var method));

        var returnStatement = Assert.Single(method!.Body.Statements.OfType<HIRReturnStatement>());
        var call = Assert.IsType<HIRCallExpression>(returnStatement.Expression);
        Assert.Null(call.StableDirectCallableTarget);
    }

    [Fact]
    public void RecursiveArrowCall_CarriesDirectTarget()
    {
        const string source = "const target = () => target();";
        var parser = new JavaScriptParser();
        var program = parser.ParseJavaScript(source, "hir-stable-call.js");
        var module = CreateModule(program);
        new SymbolTableBuilder().Build(module);
        var declaration = Assert.IsType<VariableDeclaration>(Assert.Single(program.Body));
        var arrow = Assert.IsType<ArrowFunctionExpression>(declaration.Declarations[0].Init);
        var arrowScope = Assert.Single(
            module.SymbolTable!.Root.Children,
            scope => ReferenceEquals(scope.AstNode, arrow));

        Assert.True(HIRBuilder.TryParseMethod(
            arrow,
            arrowScope,
            ScopesCallableKind.Function,
            hasScopesParameter: true,
            out var method));

        var returnStatement = Assert.Single(method!.Body.Statements.OfType<HIRReturnStatement>());
        var call = Assert.IsType<HIRCallExpression>(returnStatement.Expression);
        Assert.NotNull(call.StableDirectCallableTarget);
    }

    private static (Program Program, ModuleDefinition Module, HIRMethod Method) ParseProgram(
        string source)
    {
        var parser = new JavaScriptParser();
        var program = parser.ParseJavaScript(source, "hir-stable-call.js");
        var module = CreateModule(program);
        new SymbolTableBuilder().Build(module);

        Assert.True(HIRBuilder.TryParseMethod(
            program,
            module.SymbolTable!.Root,
            ScopesCallableKind.ModuleMain,
            hasScopesParameter: false,
            out var method));

        return (program, module, method!);
    }

    private static ModuleDefinition CreateModule(Program program)
        => new()
        {
            Ast = program,
            Path = "hir-stable-call.js",
            Name = "hir-stable-call",
            ModuleId = "hir-stable-call"
        };

    private static HIRCallExpression GetTopLevelCall(HIRMethod method)
        => Assert.IsType<HIRCallExpression>(
            Assert.Single(method.Body.Statements.OfType<HIRExpressionStatement>()).Expression);

    private static void AssertCallableMatchesPhase1(
        CallableId expected,
        CallableId actual)
    {
        Assert.Equal(expected, actual);
        Assert.Equal(expected.NeedsArgumentsObject, actual.NeedsArgumentsObject);
        Assert.Equal(expected.HasRestParameters, actual.HasRestParameters);
        Assert.Equal(expected.UsesMappedArgumentsObject, actual.UsesMappedArgumentsObject);
        Assert.Equal(expected.ArgumentsParameterNames, actual.ArgumentsParameterNames);
        Assert.Equal(expected.IncludeCalleeInArgumentsObject, actual.IncludeCalleeInArgumentsObject);
        Assert.Equal(expected.HasRestrictedFunctionProperties, actual.HasRestrictedFunctionProperties);
        Assert.Equal(expected.IsMethodDefinition, actual.IsMethodDefinition);
        Assert.Equal(expected.IsAccessorDefinition, actual.IsAccessorDefinition);
    }

    private static IEnumerable<HIRCallExpression> EnumerateCalls(HIRStatement statement)
    {
        switch (statement)
        {
            case HIRExpressionStatement { Expression: HIRCallExpression call }:
                yield return call;
                yield break;
            case HIRBlock block:
                foreach (var nestedStatement in block.Statements)
                {
                    foreach (var nestedCall in EnumerateCalls(nestedStatement))
                    {
                        yield return nestedCall;
                    }
                }
                yield break;
            case HIRWithStatement withStatement:
                foreach (var nestedCall in EnumerateCalls(withStatement.Body))
                {
                    yield return nestedCall;
                }
                yield break;
        }
    }
}
