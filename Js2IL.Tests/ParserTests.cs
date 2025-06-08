using Xunit;
using Js2IL.Services;
using Acornima.Ast;

namespace Js2IL.Tests;

public class ParserTests
{
    private readonly JavaScriptParser _parser;

    public ParserTests()
    {
        _parser = new JavaScriptParser();
    }

    [Fact]
    public void Parse_SimpleFunction_ReturnsValidAst()
    {
        // Arrange
        var js = "function add(a, b) { return a + b; }";

        // Act
        var ast = _parser.ParseJavaScript(js);

        // Assert
        Assert.NotNull(ast);
        Assert.Equal(NodeType.Program, ast.Type);
    }

    [Fact]
    public void Parse_InvalidJavaScript_ThrowsException()
    {
        // Arrange
        var js = "function add(a, b) { return a + b; // missing closing brace";

        // Act & Assert
        Assert.Throws<Exception>(() => _parser.ParseJavaScript(js));
    }

    [Fact]
    public void VisitAst_SimpleFunction_VisitsAllNodes()
    {
        // Arrange
        var js = "function add(a, b) { return a + b; }";
        var ast = _parser.ParseJavaScript(js);
        var visitedNodes = new List<NodeType>();

        // Act
        _parser.VisitAst(ast, node => visitedNodes.Add(node.Type));

        // Assert
        Assert.Contains(NodeType.FunctionDeclaration, visitedNodes);
        Assert.Contains(NodeType.BlockStatement, visitedNodes);
        Assert.Contains(NodeType.ReturnStatement, visitedNodes);
        Assert.Contains(NodeType.BinaryExpression, visitedNodes);
    }
} 