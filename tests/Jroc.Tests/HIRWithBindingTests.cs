using Acornima;
using Acornima.Ast;
using Jroc.HIR;
using Jroc.IR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using Jroc.SymbolTables;

namespace Jroc.Tests;

public sealed class HIRWithBindingTests
{
    [Theory]
    [InlineData("undefined")]
    [InlineData("NaN")]
    [InlineData("Infinity")]
    public void GlobalConstantsRemainDynamicInsideWithAndItsClosures(string name)
    {
        var (declaration, scope, table) = ParseFunction($$"""
            function read(object) {
                {{name}};
                with (object) {
                    {{name}};
                    var captured = function() { return {{name}}; };
                }
                {{name}};
            }
            """);
        Assert.True(HIRBuilder.TryParseMethod(
            declaration, scope, CallableKind.Function,
            hasScopesParameter: true, out var method));
        var outside = method!.Body.Statements.OfType<HIRExpressionStatement>().ToArray();
        Assert.Equal(2, outside.Length);
        Assert.All(outside, statement => Assert.IsType<HIRLiteralExpression>(statement.Expression));
        var with = Assert.Single(method.Body.Statements.OfType<HIRWithStatement>());
        var body = Assert.IsType<HIRBlock>(with.Body);
        var read = Assert.IsType<HIRBlock>(body.Statements[0]);
        Assert.IsType<HIRVariableExpression>(
            Assert.Single(read.Statements.OfType<HIRExpressionStatement>()).Expression);

        var withAst = Assert.Single(
            Assert.IsAssignableFrom<BlockStatement>(declaration.Body).Body.OfType<WithStatement>());
        var variable = Assert.Single(
            Assert.IsAssignableFrom<BlockStatement>(withAst.Body).Body.OfType<VariableDeclaration>());
        var function = Assert.IsType<FunctionExpression>(Assert.Single(variable.Declarations).Init);
        var functionScope = table.FindScopeByAstNode(function)!;
        Assert.True(functionScope.MayUseBoundWithObject);
        Assert.True(HIRBuilder.TryParseMethod(
            function, functionScope, CallableKind.Function,
            hasScopesParameter: true, out var closure));
        Assert.IsType<HIRVariableExpression>(
            Assert.Single(closure!.Body.Statements.OfType<HIRReturnStatement>()).Expression);
    }

    [Fact]
    public void WithObjectIsCoercedOnceBeforeProbingBindings()
    {
        var (declaration, scope, _) = ParseFunction("function read() { with (1) { value; } }");
        Assert.True(HIRBuilder.TryParseMethod(
            declaration, scope, CallableKind.Function,
            hasScopesParameter: true, out var method));
        Assert.True(HIRToLIRLowerer.TryLower(
            method!, scope, null, CallableKind.Function,
            hasScopesParameter: true, classRegistry: null, out var lir));
        var calls = lir!.Instructions.OfType<LIRCallIntrinsicStatic>().ToArray();
        var toObject = Assert.Single(calls,
            call => call.MethodName == nameof(JavaScriptRuntime.ObjectRuntime.ToObject));
        var probe = Assert.Single(calls,
            call => call.MethodName == nameof(JavaScriptRuntime.ObjectRuntime.HasWithBinding));
        Assert.True(lir.Instructions.IndexOf(toObject) < lir.Instructions.IndexOf(probe));
        Assert.Contains(toObject.Result, probe.Arguments);
    }

    private static (FunctionDeclaration Declaration, Jroc.SymbolTables.Scope Scope, SymbolTable Table) ParseFunction(string source)
    {
        var program = new JavaScriptParser().ParseJavaScript(source, "with.js");
        var declaration = Assert.IsType<FunctionDeclaration>(Assert.Single(program.Body));
        var module = new ModuleDefinition
        {
            Ast = program,
            Path = "with.js",
            Name = "with",
            ModuleId = "with"
        };
        new SymbolTableBuilder().Build(module);
        return (declaration, module.SymbolTable!.FindScopeByAstNode(declaration)!, module.SymbolTable);
    }
}
