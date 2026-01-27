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
        var ast = _parser.ParseJavaScript(js, "test.js");

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
        Assert.Throws<Exception>(() => _parser.ParseJavaScript(js, "test.js"));
    }

    [Fact]
    public void VisitAst_SimpleFunction_VisitsAllNodes()
    {
        // Arrange
        var js = "function add(a, b) { return a + b; }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var visitedNodes = new List<NodeType>();

        // Act
        _parser.VisitAst(ast, node => visitedNodes.Add(node.Type));

        // Assert
        Assert.Contains(NodeType.FunctionDeclaration, visitedNodes);
        Assert.Contains(NodeType.BlockStatement, visitedNodes);
        Assert.Contains(NodeType.ReturnStatement, visitedNodes);
        Assert.Contains(NodeType.BinaryExpression, visitedNodes);
    }

    [Fact]
    public void Parse_ContinueToNonLoopLabel_ThrowsException()
    {
        var js = "while (false) { notALoop: { continue notALoop; } }";
        var ex = Assert.Throws<Exception>(() => _parser.ParseJavaScript(js, "test.js"));
        Assert.Contains("does not denote an iteration statement", ex.Message);
    }

    [Fact]
    public void Parse_BreakToUndefinedLabel_ThrowsException()
    {
        var js = "while (false) { break missing; }";
        var ex = Assert.Throws<Exception>(() => _parser.ParseJavaScript(js, "test.js"));
        Assert.Contains("Undefined label", ex.Message);
        Assert.Contains("missing", ex.Message);
    }

    [Fact]
    public void Parse_ForOfWithMultipleDeclarators_ThrowsException()
    {
        var js = "for (let a, b of [1,2,3]) { }";
        var ex = Assert.Throws<Exception>(() => _parser.ParseJavaScript(js, "test.js"));
        Assert.Contains("for-of", ex.Message);
        Assert.Contains("single binding", ex.Message);
    }

    [Fact]
    public void Parse_ForOfWithInitializer_ThrowsException()
    {
        var js = "for (let x = 0 of [1,2,3]) { }";
        var ex = Assert.Throws<Exception>(() => _parser.ParseJavaScript(js, "test.js"));
        Assert.Contains("for-of", ex.Message);
        Assert.Contains("initializer", ex.Message);
    }

    [Fact]
    public void VisitAst_LabeledStatement_VisitsNestedNodes()
    {
        var js = "label: { var x = 1; }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var visitedNodes = new List<NodeType>();

        _parser.VisitAst(ast, node => visitedNodes.Add(node.Type));

        Assert.Contains(NodeType.LabeledStatement, visitedNodes);
        Assert.Contains(NodeType.VariableDeclaration, visitedNodes);
    }
} 