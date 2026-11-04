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
    [InlineData("class C extends Object {}")]
    [InlineData("class C { static value = 1; }")]
    [InlineData("@decorator class C {}")]
    public void ClassBindingInitializationIsDistinguishedFromSourceAssignments(string classDeclaration)
    {
        var program = new JavaScriptParser().ParseJavaScript($$"""
            function outer() {
                let value = 0;
                value = 1;
                {{classDeclaration}}
                C = 2;
            }
            """, "class-initialization.js");
        var outer = Assert.IsType<FunctionDeclaration>(Assert.Single(program.Body));
        var module = new ModuleDefinition
        {
            Ast = program,
            Path = "class-initialization.js",
            Name = "class-initialization",
            ModuleId = "class-initialization"
        };
        new SymbolTableBuilder().Build(module);
        var scope = module.SymbolTable!.FindScopeByAstNode(outer)!;
        Assert.True(HIRBuilder.TryParseMethod(
            outer, scope, CallableKind.Function,
            hasScopesParameter: true, out var method));

        var assignments = EnumerateAssignments(method!.Body.Statements).ToArray();
        var initialization = Assert.Single(assignments, assignment => assignment.IsBindingInitialization);
        Assert.Equal("C", initialization.Target.BindingInfo.Name);
        var sourceWrites = assignments.Where(assignment => !assignment.IsBindingInitialization).ToArray();
        Assert.Equal(new[] { "value", "C" }, sourceWrites.Select(assignment => assignment.Target.BindingInfo.Name));
        Assert.Equal(new object?[] { 1d, 2d },
            sourceWrites.Select(assignment => Assert.IsType<HIRLiteralExpression>(assignment.Value).Value));
    }

    private static IEnumerable<HIRAssignmentExpression> EnumerateAssignments(IEnumerable<HIRStatement> statements)
    {
        foreach (var statement in statements)
        {
            if (statement is HIRExpressionStatement { Expression: HIRAssignmentExpression assignment })
            {
                yield return assignment;
            }
            else if (statement is HIRBlock nested)
            {
                foreach (var descendant in EnumerateAssignments(nested.Statements))
                {
                    yield return descendant;
                }
            }
            else if (statement is HIRExpressionStatement { Expression: HIRInitializedUserClassTypeExpression initializedClass })
            {
                foreach (var descendant in EnumerateAssignments(initializedClass.InitializationStatements))
                {
                    yield return descendant;
                }
            }
        }
    }

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
