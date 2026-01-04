using Xunit;
using Js2IL.Services;
using Js2IL.Validation;
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

    [Fact]
    public void Validate_Require_DynamicArgument_ReportsError()
    {
        var js = "const name = './b'; const m = require(name);";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Dynamic require() with non-literal argument is not supported"));
    }

    #region Unsupported Feature Validation Tests

    [Fact]
    public void Validate_RestParameters_ReportsError()
    {
        var js = "function foo(...args) { console.log(args); }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Rest parameters"));
    }

    [Fact]
    public void Validate_SpreadInFunctionCall_ReportsError()
    {
        var js = "const arr = [1, 2, 3]; console.log(...arr);";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Spread in function calls"));
    }

    [Fact]
    public void Validate_DestructuringAssignment_ReportsError()
    {
        var js = "let x, y; [x, y] = [1, 2];";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Destructuring assignment"));
    }

    [Fact]
    public void Validate_ArrayDestructuring_ReportsError()
    {
        var js = "const [a, b] = [1, 2];";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Array destructuring"));
    }

    [Fact]
    public void Validate_ForInLoop_ReportsError()
    {
        var js = "const obj = {a: 1}; for (const k in obj) { console.log(k); }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("for...in"));
    }

    [Fact]
    public void Validate_SwitchStatement_ReportsError()
    {
        var js = "const x = 1; switch(x) { case 1: break; default: break; }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Switch statements"));
    }

    [Fact]
    public void Validate_ComputedPropertyNames_ReportsError()
    {
        var js = "const key = 'foo'; const obj = { [key]: 123 };";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Computed property names"));
    }

    [Fact]
    public void Validate_ObjectRestProperties_ReportsError()
    {
        var js = "const obj = {a: 1, b: 2, c: 3}; const {a, ...rest} = obj;";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Rest parameters/properties"));
    }

    [Fact]
    public void Validate_WithStatement_ReportsError()
    {
        // Note: 'with' only works in non-strict mode
        var js = "var obj = {a: 1}; with(obj) { console.log(a); }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("'with' statement"));
    }

    [Fact]
    public void Validate_LabeledStatement_ReportsError()
    {
        var js = "outer: for (let i = 0; i < 3; i++) { break outer; }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Labeled statements"));
    }

    [Fact]
    public void Validate_DebuggerStatement_ReportsError()
    {
        var js = "function test() { debugger; }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("'debugger' statement"));
    }

    [Fact]
    public void Validate_NestedDestructuring_ReportsError()
    {
        var js = "const obj = {inner: {x: 1}}; const {inner: {x}} = obj;";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Nested destructuring"));
    }

    [Fact]
    public void Validate_NewTarget_ReportsError()
    {
        var js = "function Foo() { console.log(new.target); }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("new.target"));
    }

    [Fact]
    public void Validate_SuperExpression_ReportsError()
    {
        var js = "class Parent { foo() {} } class Child extends Parent { foo() { super.foo(); } }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("super"));
    }

    [Fact]
    public void Validate_Getter_ReportsError()
    {
        var js = "const obj = { get foo() { return 42; } };";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Getter"));
    }

    [Fact]
    public void Validate_Setter_ReportsError()
    {
        var js = "const obj = { set foo(v) { this._foo = v; } };";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Setter"));
    }

    [Fact]
    public void Validate_ClassGetter_ReportsError()
    {
        var js = "class Foo { get bar() { return 42; } }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Getter"));
    }

    [Fact]
    public void Validate_ThisExpression_ReportsError()
    {
        // Issue #218: 'this' is not yet supported
        var js = "function foo() { console.log(this); }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("'this' keyword is not yet supported"));
    }

    [Fact]
    public void Validate_ThisInArrowFunction_ReportsError()
    {
        // Issue #218: 'this' in arrow functions is not yet supported
        var js = "const foo = () => this.bar;";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("'this' keyword is not yet supported"));
    }

    [Fact]
    public void Validate_FunctionWithMoreThan6Parameters_ReportsError()
    {
        // Issue #220: Functions with >6 parameters are not supported
        var js = "function foo(a, b, c, d, e, f, g) { return a + b + c + d + e + f + g; }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("more than 6 parameters"));
    }

    [Fact]
    public void Validate_ArrowFunctionWithMoreThan6Parameters_ReportsError()
    {
        // Issue #220: Arrow functions with >6 parameters are not supported
        var js = "const foo = (a, b, c, d, e, f, g) => a + b + c + d + e + f + g;";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("more than 6 parameters"));
    }

    [Fact]
    public void Validate_CallWithMoreThan6Arguments_ReportsError()
    {
        // Issue #220: Call expressions with >6 arguments are not supported
        var js = "function test() { console.log(1, 2, 3, 4, 5, 6, 7); }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("more than 6 arguments"));
    }

    [Fact]
    public void Validate_FunctionWith6Parameters_Valid()
    {
        // Issue #220: Functions with exactly 6 parameters should be valid
        var js = "function foo(a, b, c, d, e, f) { return a + b + c + d + e + f; }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_CallWith6Arguments_Valid()
    {
        // Issue #220: Call expressions with exactly 6 arguments should be valid
        var js = "function test() { console.log(1, 2, 3, 4, 5, 6); }";
        var ast = _parser.ParseJavaScript(js, "test.js");
        var result = _validator.Validate(ast);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    #endregion
} 