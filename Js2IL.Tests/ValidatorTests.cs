using Xunit;
using Js2IL.Services;
using Acornima.Ast;

namespace Js2IL.Tests;

public class ValidatorTests
{
    private readonly JavaScriptParser _parser;
    private readonly JavaScriptAstValidator _validator;

    public ValidatorTests()
    {
        _parser = new JavaScriptParser();
        _validator = new JavaScriptAstValidator();
    }

    [Fact]
    public void Validate_SimpleAddition_ReturnsValid()
    {
        // Arrange
        var js = @"var x = 1 + 2;
            console.log('X is',x);
        ";
        var ast = _parser.ParseJavaScript(js, "test.js");
        // Act
        var result = _validator.Validate(ast);
        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void Validate_SimpleFunction_ReturnsValid()
    {
        // Arrange
        var js = "function add(a, b) { return a + b; }";
        var ast = _parser.ParseJavaScript(js, "test.js");

        // Act
        var result = _validator.Validate(ast);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void Validate_ClassDeclaration_ReturnsInvalid()
    {
        // Arrange
        var js = "class Test { constructor() { } }";
        var ast = _parser.ParseJavaScript(js, "test.js");

        // Act
        var result = _validator.Validate(ast);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Class declarations are not yet supported", result.Errors[0]);
    }

    [Fact]
    public void Validate_ArrowFunction_ReturnsWarning()
    {
        // Arrange
        var js = "const add = (a, b) => a + b;";
        var ast = _parser.ParseJavaScript(js, "test.js");

        // Act
        var result = _validator.Validate(ast);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Contains("Arrow functions are experimental", result.Warnings[0]);
    }

    [Fact]
    public void Validate_MultipleIssues_ReturnsAllErrorsAndWarnings()
    {
        // Arrange
        var js = @"
            class Test { }
            const add = (a, b) => a + b;
            /* import { something } from 'module'; */
        ";
        var ast = _parser.ParseJavaScript(js, "test.js");

        // Act
        var result = _validator.Validate(ast);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Class declarations are not yet supported (line 2)", result.Errors);
        /* Assert.Contains("ES6 modules are not yet supported", result.Errors); */
        Assert.Contains("Arrow functions are experimental (line 3)", result.Warnings);
    }
} 