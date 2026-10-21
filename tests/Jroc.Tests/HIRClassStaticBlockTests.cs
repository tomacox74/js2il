using Acornima;
using Acornima.Ast;
using Jroc.HIR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using Jroc.SymbolTables;

namespace Jroc.Tests;

public sealed class HIRClassStaticBlockTests
{
    [Theory]
    [InlineData("() => new.target", true)]
    [InlineData("() => { return new.target; }", true)]
    [InlineData("function() { return new.target; }", false)]
    public void NewTargetRespectsTheStaticBlockAndOrdinaryFunctionBoundaries(
        string callable, bool isUndefined)
    {
        var program = new JavaScriptParser().ParseJavaScript($$"""
            function outer() {
                class C {
                    static { const read = {{callable}}; }
                }
            }
            """, "static-block.js");
        var outer = Assert.IsType<FunctionDeclaration>(Assert.Single(program.Body));
        var declaration = Assert.IsType<ClassDeclaration>(
            Assert.Single(Assert.IsAssignableFrom<BlockStatement>(outer.Body).Body));
        var block = Assert.IsType<StaticBlock>(Assert.Single(declaration.Body.Body));
        var variable = Assert.IsType<VariableDeclaration>(Assert.Single(block.Body));
        var function = Assert.IsAssignableFrom<Expression>(
            Assert.Single(variable.Declarations).Init);
        var module = new ModuleDefinition
        {
            Ast = program,
            Path = "static-block.js",
            Name = "static-block",
            ModuleId = "static-block"
        };
        new SymbolTableBuilder().Build(module);
        var scope = module.SymbolTable!.FindScopeByAstNode(function)!;

        Assert.True(HIRBuilder.TryParseMethod(
            function, scope, CallableKind.Function,
            hasScopesParameter: true, out var method));
        var result = Assert.Single(method!.Body.Statements.OfType<HIRReturnStatement>()).Expression;

        if (isUndefined)
        {
            Assert.Null(Assert.IsType<HIRLiteralExpression>(result).Value);
        }
        else
        {
            Assert.IsType<HIRNewTargetExpression>(result);
        }
    }
}
