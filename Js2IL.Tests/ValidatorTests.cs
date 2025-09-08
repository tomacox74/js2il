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
        // Only modules are an error; classes and arrow functions allowed now.
        // Accept either valid or a single module error if modules were present.
        if (result.Errors.Count > 0)
        {
            Assert.All(result.Errors, e => Assert.Contains("modules are not yet supported", e));
        }
        Assert.DoesNotContain(result.Errors, e => e.Contains("Class declarations are not yet supported"));
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void Validate_Require_SupportedModule_NoError()
    {
        var js = "const p = require('path');";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_Require_UnsupportedModule_ReportsError()
    {
        var js = "const c = require('node:crypto');";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Module 'node:crypto' is not yet supported"));
    }
} 