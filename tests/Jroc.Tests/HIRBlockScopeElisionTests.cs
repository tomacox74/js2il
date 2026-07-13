using Acornima;
using Acornima.Ast;
using Jroc.HIR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using Jroc.SymbolTables;
using AstNode = Acornima.Ast.Node;
using SymbolScope = Jroc.SymbolTables.Scope;

namespace Jroc.Tests;

public class HIRBlockScopeElisionTests
{
    [Fact]
    public void TryParseMethod_EmptyLoopAndIfBlocks_DoNotRequestScopeInstances()
    {
        var (method, _) = ParseFunctionExpression("""
            var findGraphNode = function(obj) {
                for (var i = 0; i < this.length; i++) {
                    if (this[i].pos == obj.pos) { return this[i]; }
                }
                return false;
            };
            """);

        var blocks = EnumerateBlocks(method.Body).ToArray();

        Assert.True(blocks.Length >= 3);
        Assert.All(blocks, block => Assert.Null(block.ScopeName));
    }

    [Fact]
    public void TryParseMethod_BlockWithLexicalBinding_RequestsScopeInstance()
    {
        var (method, _) = ParseFunctionExpression("""
            var findGraphNode = function(obj) {
                for (var i = 0; i < this.length; i++) {
                    let current = this[i];
                    if (current.pos == obj.pos) { return current; }
                }
                return false;
            };
            """);

        Assert.Contains(EnumerateBlocks(method.Body), block => block.ScopeName != null);
    }

    [Fact]
    public void TryParseMethod_EmptySwitchScope_DoesNotRequestScopeInstance()
    {
        var (method, _) = ParseFunctionDeclaration("""
            function select(value) {
                switch (value) {
                    case 1:
                        return "one";
                    default:
                        return "other";
                }
            }
            """);

        var switchStatement = Assert.Single(method.Body.Statements.OfType<HIRSwitchStatement>());
        Assert.Null(switchStatement.ScopeName);
    }

    private static (HIRMethod Method, SymbolScope Scope) ParseFunctionExpression(string source)
    {
        var parser = new JavaScriptParser();
        var program = parser.ParseJavaScript(source, "scope-elision.js");
        var functionExpression = Assert.IsType<FunctionExpression>(
            Assert.IsType<VariableDeclaration>(Assert.Single(program.Body)).Declarations[0].Init);

        return ParseMethod(program, functionExpression);
    }

    private static (HIRMethod Method, SymbolScope Scope) ParseFunctionDeclaration(string source)
    {
        var parser = new JavaScriptParser();
        var program = parser.ParseJavaScript(source, "scope-elision.js");
        var functionDeclaration = Assert.IsType<FunctionDeclaration>(Assert.Single(program.Body));

        return ParseMethod(program, functionDeclaration);
    }

    private static (HIRMethod Method, SymbolScope Scope) ParseMethod(Program program, AstNode function)
    {
        var module = new ModuleDefinition
        {
            Ast = program,
            Path = "scope-elision.js",
            Name = "scope-elision",
            ModuleId = "scope-elision"
        };

        new SymbolTableBuilder().Build(module);
        var scope = Assert.Single(module.SymbolTable!.Root.Children);

        Assert.True(HIRBuilder.TryParseMethod(
            function,
            scope,
            CallableKind.Function,
            hasScopesParameter: false,
            out var method));

        return (method!, scope);
    }

    private static IEnumerable<HIRBlock> EnumerateBlocks(HIRStatement statement)
    {
        switch (statement)
        {
            case HIRBlock block:
                yield return block;
                foreach (var nested in block.Statements)
                {
                    foreach (var descendant in EnumerateBlocks(nested))
                    {
                        yield return descendant;
                    }
                }
                yield break;

            case HIRIfStatement ifStatement:
                foreach (var descendant in EnumerateBlocks(ifStatement.Consequent))
                {
                    yield return descendant;
                }
                if (ifStatement.Alternate != null)
                {
                    foreach (var descendant in EnumerateBlocks(ifStatement.Alternate))
                    {
                        yield return descendant;
                    }
                }
                yield break;

            case HIRForStatement forStatement:
                if (forStatement.Init != null)
                {
                    foreach (var descendant in EnumerateBlocks(forStatement.Init))
                    {
                        yield return descendant;
                    }
                }
                foreach (var descendant in EnumerateBlocks(forStatement.Body))
                {
                    yield return descendant;
                }
                yield break;
        }
    }
}
