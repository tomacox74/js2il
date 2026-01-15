using Acornima;
using Acornima.Ast;
using Js2IL.SymbolTables;
using Js2IL.Services;
using System.Linq;
using Xunit;

namespace Js2IL.Tests;

public class SymbolTableTypeInferenceTests
{
    private readonly JavaScriptParser _parser = new();
    private readonly SymbolTableBuilder _scopeBuilder = new();

    // Helper method to adapt old test API to new ModuleDefinition-based API
    private SymbolTable BuildSymbolTable(Acornima.Ast.Program ast, string fileName)
    {
        var module = new ModuleDefinition
        {
            Ast = ast,
            Path = fileName,
            Name = Path.GetFileNameWithoutExtension(fileName)
        };
        _scopeBuilder.Build(module);
        return module.SymbolTable!;
    }
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

    [Theory]
    [InlineData("X", "42", typeof(double), "return X + 1;")]
    [InlineData("FLAG", "true", typeof(bool), "return FLAG ? 1 : 0;")]
    [InlineData("NAME", "'hello'", typeof(string), "return NAME;")]
    public void SymbolTable_InferTypes_CapturedConst(string name, string initializer, Type expectedType, string usage)
    {
        var code = $@"
                const {name} = {initializer};

                function run() {{
                    {usage}
                }}

                run();
            ";

        var symbolTable = BuildSymbolTable(code);
        var binding = symbolTable.GetBindingInfo(name);
        Assert.NotNull(binding);
        Assert.Equal(BindingKind.Const, binding.Kind);
        Assert.True(binding.IsCaptured);
        Assert.True(binding.IsStableType);
        Assert.Equal(expectedType, binding.ClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_ClassInstanceFields_Primitives()
    {
        var code = @"
                class Counter {
                    constructor() {
                        this.value = 0;
                        this.name = 'test';
                        this.active = true;
                    }

                    increment() {
                        this.value = this.value + 1;
                    }
                }
            ";

        var symbolTable = BuildSymbolTable(code);
        var classScope = FindClassScope(symbolTable.Root, "Counter");
        Assert.NotNull(classScope);
        Assert.Equal(ScopeKind.Class, classScope!.Kind);

        Assert.True(classScope.StableInstanceFieldClrTypes.TryGetValue("value", out var valueType));
        Assert.Equal(typeof(double), valueType);

        Assert.True(classScope.StableInstanceFieldClrTypes.TryGetValue("name", out var nameType));
        Assert.Equal(typeof(string), nameType);

        Assert.True(classScope.StableInstanceFieldClrTypes.TryGetValue("active", out var activeType));
        Assert.Equal(typeof(bool), activeType);
    }

    [Fact]
    public void SymbolTable_InferTypes_ClassInstanceFields_ConflictingAssignments_NotStable()
    {
        var code = @"
                class C {
                    constructor() {
                        this.x = 0;
                    }
                    m() {
                        this.x = 'oops';
                    }
                }
            ";

        var symbolTable = BuildSymbolTable(code);
        var classScope = FindClassScope(symbolTable.Root, "C");
        Assert.NotNull(classScope);

        Assert.False(classScope!.StableInstanceFieldClrTypes.ContainsKey("x"));
    }

    [Fact]
    public void SymbolTable_InferTypes_ClassInstanceFields_ComputedAssignment_Ignored()
    {
        var code = @"
                class C {
                    constructor() {
                        this['x'] = 1;
                        this.y = 2;
                    }
                }
            ";

        var symbolTable = BuildSymbolTable(code);
        var classScope = FindClassScope(symbolTable.Root, "C");
        Assert.NotNull(classScope);

        Assert.False(classScope!.StableInstanceFieldClrTypes.ContainsKey("x"));
        Assert.True(classScope.StableInstanceFieldClrTypes.TryGetValue("y", out var yType));
        Assert.Equal(typeof(double), yType);
    }

    [Fact]
    public void SymbolTable_InferTypes_ClassInstanceFields_UpdateExpression_InfersNumber()
    {
        var code = @"
                class C {
                    m() {
                        this.count++;
                    }
                }
            ";

        var symbolTable = BuildSymbolTable(code);
        var classScope = FindClassScope(symbolTable.Root, "C");
        Assert.NotNull(classScope);

        Assert.True(classScope!.StableInstanceFieldClrTypes.TryGetValue("count", out var t));
        Assert.Equal(typeof(double), t);
    }

    [Fact]
    public void SymbolTable_InferTypes_ClassInstanceFields_UserClass_NewExpression()
    {
        var code = @"
                class Child {
                    constructor() { }
                }

                class Parent {
                    constructor() {
                        this.child = new Child();
                    }
                }
            ";

        var symbolTable = BuildSymbolTable(code);
        var classScope = FindClassScope(symbolTable.Root, "Parent");
        Assert.NotNull(classScope);

        Assert.True(classScope!.StableInstanceFieldUserClassNames.TryGetValue("child", out var inferred));
        Assert.Equal("Child", inferred);
    }

    [Theory]
    [InlineData("function f() { return 1 + 2; }", "f", typeof(double))]
    [InlineData("function f() { return true; }", "f", typeof(bool))]
    public void SymbolTable_InferTypes_StableReturn_FunctionDeclaration(string source, string functionName, Type expectedReturnType)
    {
        var symbolTable = BuildSymbolTable(source);

        var functionScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function &&
            s.Parent?.Kind == ScopeKind.Global &&
            string.Equals(s.Name, functionName, StringComparison.Ordinal) &&
            s.AstNode is FunctionDeclaration);

        Assert.NotNull(functionScope);
        Assert.Equal(expectedReturnType, functionScope!.StableReturnClrType);
    }

    [Theory]
    [InlineData("const a = () => 1 + 2;", typeof(double))]
    [InlineData("const a = () => true;", typeof(bool))]
    [InlineData("const a = () => { return 1 + 2; };", typeof(double))]
    [InlineData("const a = () => { return true; };", typeof(bool))]
    public void SymbolTable_InferTypes_StableReturn_ArrowFunction(string source, Type expectedReturnType)
    {
        var symbolTable = BuildSymbolTable(source);

        var arrowScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function &&
            s.Parent?.Kind == ScopeKind.Global &&
            s.AstNode is ArrowFunctionExpression);

        Assert.NotNull(arrowScope);
        Assert.Equal(expectedReturnType, arrowScope!.StableReturnClrType);
    }

    [Theory]
    [InlineData(@"
                class C {
                    m() { return 1 + 2; }
                }
            ", "C", "m", typeof(double))]
    [InlineData(@"
                class C {
                    m() { return true; }
                }
            ", "C", "m", typeof(bool))]
    public void SymbolTable_InferTypes_StableReturn_ClassMethod(string source, string className, string methodName, Type expectedReturnType)
    {
        var symbolTable = BuildSymbolTable(source);
        var classScope = FindClassScope(symbolTable.Root, className);
        Assert.NotNull(classScope);

        var methodScope = FindFirstScope(classScope!, s =>
            s.Kind == ScopeKind.Function &&
            s.Parent?.Kind == ScopeKind.Class &&
            string.Equals(s.Name, methodName, StringComparison.Ordinal));

        Assert.NotNull(methodScope);
        Assert.Equal(expectedReturnType, methodScope!.StableReturnClrType);
    }

    private static Js2IL.SymbolTables.Scope? FindFirstScope(Js2IL.SymbolTables.Scope scope, Func<Js2IL.SymbolTables.Scope, bool> predicate)
    {
        if (predicate(scope))
        {
            return scope;
        }

        foreach (var child in scope.Children)
        {
            var found = FindFirstScope(child, predicate);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private static Js2IL.SymbolTables.Scope? FindClassScope(Js2IL.SymbolTables.Scope scope, string className)
    {
        if (scope.Kind == ScopeKind.Class && string.Equals(scope.Name, className, StringComparison.Ordinal))
        {
            return scope;
        }

        foreach (var child in scope.Children)
        {
            var found = FindClassScope(child, className);
            if (found != null)
            {
                return found;
            }
        }

        return null;
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
        var symbolTable = BuildSymbolTable(ast, "test.js");
        return symbolTable;
    }
}
