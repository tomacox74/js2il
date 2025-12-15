using Acornima;
using Js2IL.SymbolTables;
using Js2IL.Services;
using System.Linq;
using Xunit;

namespace Js2IL.Tests;

public class SymbolTableTypeInferenceTests
{
    private readonly JavaScriptParser _parser = new();
    private readonly SymbolTableBuilder _scopeBuilder = new();

    [Theory]
    [InlineData(typeof(double), "42")]
    [InlineData(typeof(string), "'hello'")]
    [InlineData(typeof(bool), "true")]
    [InlineData(null, "null")]
    [InlineData(null, "")]
    [InlineData(typeof(double), "1 + 2")]
    [InlineData(typeof(string), "'1' + '2'")]
    [InlineData(typeof(string), "'1' + 2")]
    [InlineData(typeof(string), "1 + '2'")]
    // Bitwise operators always return numbers
    [InlineData(typeof(double), "5 & 3")]       // AND
    [InlineData(typeof(double), "5 | 3")]       // OR
    [InlineData(typeof(double), "5 ^ 3")]       // XOR
    [InlineData(typeof(double), "5 << 2")]      // Left shift
    [InlineData(typeof(double), "5 >> 2")]      // Signed right shift
    [InlineData(typeof(double), "5 >>> 2")]     // Unsigned right shift
    [InlineData(typeof(double), "~5")]          // NOT (unary)
    public void SymbolTable_InferType_Init(Type? expectedType, string initializer)
    {
        var variableName = "testVar";
        var code = $@"var {variableName} {(string.IsNullOrEmpty(initializer) ? "" : "=")} {initializer};";

        var symbolTable = BuildSymbolTable(code);
        var binding = symbolTable.GetBindingInfo(variableName!);
        Assert.NotNull(binding);
        Assert.Equal(expectedType, binding.ClrType);
    }

    [Theory]
    [InlineData(typeof(double), "42", "testVar = 100")]
    [InlineData(null, "'hello'", "testVar = 123")]  // conflicting assignment removes inferred type
    [InlineData(typeof(bool), "true", "testVar = false")]
    [InlineData(null, "null", "testVar = 'now a string'")] // conflicting assignment removes inferred type
    [InlineData(null, "", "testVar = 3.14")] // single assignment, but type is either number or undefined
    [InlineData(typeof(string), "", "testVar = 'first'; testVar = 'second'")] // multiple consistent assignments
    [InlineData(null, "", "testVar = 10; testVar = 'oops'; testVar = true")] // multiple conflicting assignments
    public void SymbolTable_InferType_Assignments(Type? expectedType, string initialValue, string assignments)
    {
        var variableName = "testVar";
        var code = $@"
                var {variableName} {(string.IsNullOrEmpty(initialValue) ? "" : "=")} {initialValue};
                {assignments};
            ";

        var symbolTable = BuildSymbolTable(code);
        var binding = symbolTable.GetBindingInfo(variableName!);
        Assert.NotNull(binding);
        Assert.Equal(expectedType, binding.ClrType);
    }

    [Theory]
    [InlineData(typeof(double), "42", "testVar++")]
    [InlineData(typeof(double), "42", "++testVar")]
    [InlineData(typeof(double), "42", "testVar--")]
    [InlineData(typeof(double), "42", "--testVar")]
    [InlineData(null, "'hello'", "testVar++")]  // invalid operation removes inferred type
    [InlineData(null, "true", "++testVar")]  // invalid operation removes inferred type
    [InlineData(null, "", "testVar++")]  // invalid operation removes inferred type
    public void SymbolTable_InferType_UpdateExpressions(Type? expectedType, string initialValue, string updateExpression)
    {
        var variableName = "testVar";
        var code = $@"
                var {variableName} {(string.IsNullOrEmpty(initialValue) ? "" : "=")} {initialValue};
                {updateExpression};
            ";

        var symbolTable = BuildSymbolTable(code);
        var binding = symbolTable.GetBindingInfo(variableName!);
        Assert.NotNull(binding);
        Assert.Equal(expectedType, binding.ClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_ClassMethod()
    {
        var code = @"
                class MyClass {
                    myMethod() {
                        var localVar = 1 + 2;
                        return localVar;
                    }
                }
            ";

        var symbolTable = BuildSymbolTable(code);
        var binding = symbolTable.GetBindingInfo("MyClass/myMethod/localVar");
        Assert.NotNull(binding);
        Assert.Equal(typeof(double), binding.ClrType);
    }

    [Theory]
    // For loop - iterator variable
    [InlineData("for (let i = 0; i < 10; i++) { }", "i", typeof(double))]
    // For loop - variable in body
    [InlineData("for (let i = 0; i < 10; i++) { let x = 42; }", "x", typeof(double))]
    // For...in loop
    [InlineData("for (let key in {}) { let val = 'test'; }", "val", typeof(string))]
    // For...of loop  
    [InlineData("for (let item of []) { let count = 1; }", "count", typeof(double))]
    // While loop
    [InlineData("while (true) { let counter = 42; }", "counter", typeof(double))]
    // Do-while loop
    [InlineData("do { let flag = true; } while (false);", "flag", typeof(bool))]
    // If block
    [InlineData("if (true) { let value = 'hello'; }", "value", typeof(string))]
    // Else block
    [InlineData("if (false) { } else { let other = 99; }", "other", typeof(double))]
    // Try block
    [InlineData("try { let attempt = 1; } catch (e) { }", "attempt", typeof(double))]
    // Catch block
    [InlineData("try { } catch (e) { let recovered = 'ok'; }", "recovered", typeof(string))]
    // Finally block
    [InlineData("try { } finally { let cleanup = true; }", "cleanup", typeof(bool))]
    // Switch case block (with braces)
    [InlineData("switch (1) { case 1: { let matched = 42; } }", "matched", typeof(double))]
    // NESTED block scopes - these are critical for real-world code like PrimeJavaScript
    // For loop inside if block (2 levels deep)
    [InlineData("if (true) { for (let i = 0; i < 10; i++) { let nested = 42; } }", "nested", typeof(double))]
    // While loop inside for loop (2 levels deep)
    [InlineData("for (let i = 0; i < 1; i++) { while (true) { let deep = 'test'; break; } }", "deep", typeof(string))]
    // If inside while inside if (3 levels deep)
    [InlineData("if (true) { while (true) { if (true) { let veryDeep = 123; } break; } }", "veryDeep", typeof(double))]
    public void SymbolTable_InferTypes_BlockScope(string blockCode, string variableName, Type expectedType)
    {
        var code = $@"
                class MyClass {{
                    myMethod() {{
                        {blockCode}
                    }}
                }}
            ";

        var symbolTable = BuildSymbolTable(code);
        var binding = FindBindingByName(symbolTable.Root, variableName);
        Assert.NotNull(binding);
        Assert.Equal(expectedType, binding.ClrType);
    }

    private BindingInfo? FindBindingByName(Js2IL.SymbolTables.Scope scope, string name)
    {
        // Check current scope
        if (scope.Bindings.TryGetValue(name, out var binding))
        {
            return binding;
        }
        
        // Recursively search children
        foreach (var child in scope.Children)
        {
            var found = FindBindingByName(child, name);
            if (found != null)
            {
                return found;
            }
        }
        
        return null;
    }

    private SymbolTable BuildSymbolTable(string source)
    {
        var ast = _parser.ParseJavaScript(source, "test.js");
        var symbolTable = _scopeBuilder.Build(ast, "test.js");
        return symbolTable;
    }
}