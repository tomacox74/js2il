using Acornima;
using Acornima.Ast;
using Jroc.SymbolTables;
using Jroc.Services;
using System.IO;
using System.Reflection;
using System.Linq;
using Xunit;

namespace Jroc.Tests;

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
            Name = Path.GetFileNameWithoutExtension(fileName),
            ModuleId = Path.GetFileNameWithoutExtension(fileName)
        };
        _scopeBuilder.Build(module);
        return module.SymbolTable!;
    }
    [Theory]
    [InlineData(typeof(double), "42")]
    [InlineData(typeof(string), "'hello'")]
    [InlineData(typeof(bool), "true")]
    [InlineData(typeof(JavaScriptRuntime.JsNull), "null")]
    [InlineData(typeof(JavaScriptRuntime.Array), "[]")]
    [InlineData(typeof(JavaScriptRuntime.Array), "[1, 2, 3]")]
    [InlineData(typeof(JavaScriptRuntime.Array), "new Array()")]
    [InlineData(typeof(JavaScriptRuntime.Array), "Array.of(1, 2)")]
    [InlineData(typeof(JavaScriptRuntime.Array), "Array.from([1, 2])")]
    [InlineData(typeof(JavaScriptRuntime.RegExp), "/a/")]
    [InlineData(typeof(JavaScriptRuntime.RegExp), "new RegExp('a', 'g')")]
    [InlineData(null, "")]
    [InlineData(typeof(double), "1 + 2")]
    [InlineData(typeof(double), "2 ** 3")]
    [InlineData(typeof(double), "5 % 2")]
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
    // Array static methods
    [InlineData(typeof(bool), "Array.isArray([])")]
    [InlineData(typeof(bool), "Array.isArray(null)")]
    // Array instance methods that return boolean
    [InlineData(typeof(bool), "[1, 2, 3].some(x => x === 2)")]
    [InlineData(typeof(bool), "['a', 'b'].some(x => x === 'c')")]
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
    [InlineData(typeof(double), "42", "testVar = testVar ** 3")]
    [InlineData(typeof(double), "42", "testVar = testVar % 5")]
    [InlineData(null, "'hello'", "testVar = 123")]  // conflicting assignment removes inferred type
    [InlineData(typeof(bool), "true", "testVar = false")]
    [InlineData(null, "null", "testVar = 'now a string'")] // conflicting assignment removes inferred type
    [InlineData(null, "", "testVar = 3.14")] // single assignment, but type is either number or undefined
    [InlineData(typeof(string), "", "testVar = 'first'; testVar = 'second'")] // multiple consistent assignments
    [InlineData(null, "", "testVar = 10; testVar = 'oops'; testVar = true")] // multiple conflicting assignments
    [InlineData(typeof(JavaScriptRuntime.Array), "[]", "testVar = [1, 2]")]
    [InlineData(null, "[]", "testVar = [1]; testVar = 123")]
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

    [Fact]
    public void SymbolTable_InferType_AssignmentAlias_WidensWhenSourceBecomesNull()
    {
        var code = @"
                var first = 0;
                var second = 1;
                first = second;
                second = null;
            ";

        var symbolTable = BuildSymbolTable(code);
        var firstBinding = symbolTable.GetBindingInfo("first");
        var secondBinding = symbolTable.GetBindingInfo("second");
        Assert.NotNull(firstBinding);
        Assert.NotNull(secondBinding);
        Assert.Null(firstBinding.ClrType);
        Assert.Null(secondBinding.ClrType);
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
    public void SymbolTable_InferTypes_ArithBenchmarkLocals_StayStableDoubles()
    {
        var code = @"
                function arith() {
                    var s = 0;
                    for (var i = 0; i < 20; i++) {
                        var temp = 0;
                        temp += 1;
                        temp = temp ** 3;
                        temp *= 7;
                        temp -= 11;
                        temp /= 2;
                        s = s + temp;
                    }
                    return s;
                }
            ";

        var symbolTable = BuildSymbolTable(code);

        foreach (var variableName in new[] { "arith/s", "arith/i", "arith/temp" })
        {
            var binding = symbolTable.GetBindingInfo(variableName);
            Assert.NotNull(binding);
            Assert.True(binding.IsStableType, $"Expected {variableName} to be stable.");
            Assert.Equal(typeof(double), binding.ClrType);
        }
    }

    [Fact]
    public void SymbolTable_InferTypes_NumericVarLocals_ProvesInitializedAndLoopBindings()
    {
        var symbolTable = BuildSymbolTable(@"
            function calculate(limit) {
                var total = 0;
                for (var i = 0; i < limit; i++) {
                    total += i;
                }

                var direction;
                if (total >= 0) {
                    direction = 1;
                } else {
                    direction = -1;
                }

                return total + direction;
            }
            calculate(5);
        ");

        foreach (var variableName in new[]
                 {
                     "calculate/total",
                     "calculate/i",
                     "calculate/direction"
                 })
        {
            var binding = symbolTable.GetBindingInfo(variableName);
            Assert.NotNull(binding);
            Assert.True(binding.IsStableType, $"Expected {variableName} to be stable.");
            Assert.Equal(typeof(double), binding.ClrType);
            Assert.True(binding.CanUseUnboxedLocal, $"Expected {variableName} to use an unboxed local.");
        }
    }

    [Theory]
    [InlineData("console.log(value); value = 1;")]
    [InlineData("if (flag) value = 1; console.log(value);")]
    [InlineData("value = 1; if (flag) value = 'text';")]
    [InlineData("value = 1; var object = { property: (value = 'text') };")]
    [InlineData("label: { if (flag) break label; value = 1; } console.log(value);")]
    [InlineData("var callback = null; callback?.(value = 1); console.log(value);")]
    public void SymbolTable_InferTypes_NumericVarLocal_RejectsUnsafeControlFlow(string statements)
    {
        var symbolTable = BuildSymbolTable($@"
            function calculate(flag) {{
                var value;
                {statements}
            }}
        ");

        var binding = symbolTable.GetBindingInfo("calculate/value");
        Assert.NotNull(binding);
        Assert.False(binding.CanUseUnboxedLocal);
    }

    [Fact]
    public void SymbolTable_InferTypes_NumericVarLocal_RejectsHoistedNumericDependency()
    {
        var symbolTable = BuildSymbolTable(@"
            function calculate() {
                var result = value;
                var value = 1;
                return result;
            }
        ");

        var resultBinding = symbolTable.GetBindingInfo("calculate/result");
        var valueBinding = symbolTable.GetBindingInfo("calculate/value");
        Assert.NotNull(resultBinding);
        Assert.NotNull(valueBinding);
        Assert.False(resultBinding.CanUseUnboxedLocal);
        Assert.False(valueBinding.CanUseUnboxedLocal);
        Assert.Null(resultBinding.ClrType);
        var functionScope = FindFirstScope(
            symbolTable.Root,
            candidate => candidate.Kind == ScopeKind.Function
                         && string.Equals(candidate.Name, "calculate", StringComparison.Ordinal));
        Assert.NotNull(functionScope);
        Assert.Null(functionScope!.StableReturnClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_NumericVarLocal_RejectsShadowedNonnumericSource()
    {
        var symbolTable = BuildSymbolTable(@"
            function calculate() {
                var source = 1;
                var target;
                {
                    let source = 'text';
                    target = source;
                }
                return target;
            }
        ");

        var targetBinding = symbolTable.GetBindingInfo("calculate/target");
        Assert.NotNull(targetBinding);
        Assert.False(targetBinding.CanUseUnboxedLocal);
        Assert.Null(targetBinding.ClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_NumericVarLocal_RejectsCapturedBinding()
    {
        var symbolTable = BuildSymbolTable(@"
            function calculate() {
                var value = 1;
                return function () {
                    return ++value;
                };
            }
        ");

        var binding = symbolTable.GetBindingInfo("calculate/value");
        Assert.NotNull(binding);
        Assert.True(binding.IsCaptured);
        Assert.False(binding.CanUseUnboxedLocal);
    }

    [Fact]
    public void SymbolTable_InferTypes_NumericVarLocalSpecializationFixture_ProvesHotLocals()
    {
        const string resourceName = "Jroc.Tests.Variable.JavaScript.Variable_NumericVarLocalSpecialization.js";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);
        using var reader = new StreamReader(stream);
        var symbolTable = BuildSymbolTable(reader.ReadToEnd());

        foreach (var variableName in new[] { "calculate/total", "calculate/i", "calculate/direction" })
        {
            var binding = symbolTable.GetBindingInfo(variableName);
            Assert.NotNull(binding);
            Assert.True(binding.CanUseUnboxedLocal, $"Expected {variableName} to use an unboxed local.");
        }
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
    public void SymbolTable_InferTypes_CapturedLetArray_WithClosureWrites_IsStableArray()
    {
        var code = @"
                let ret = [];
                function test(fn) { fn(); }

                test(() => {
                    ret = [];
                    ret = new Array(8);
                });
            ";

        var symbolTable = BuildSymbolTable(code);
        var binding = symbolTable.GetBindingInfo("ret");
        Assert.NotNull(binding);
        Assert.Equal(BindingKind.Let, binding!.Kind);
        Assert.True(binding.IsCaptured);
        Assert.True(binding.IsStableType);
        Assert.Equal(typeof(JavaScriptRuntime.Array), binding.ClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_CapturedLetArray_WithConflictingClosureWrite_IsNotStable()
    {
        var code = @"
                let ret = [];
                function test(fn) { fn(); }

                test(() => {
                    ret = 1;
                });
            ";

        var symbolTable = BuildSymbolTable(code);
        var binding = symbolTable.GetBindingInfo("ret");
        Assert.NotNull(binding);
        Assert.Equal(BindingKind.Let, binding!.Kind);
        Assert.True(binding.IsCaptured);
        Assert.False(binding.IsStableType);
        Assert.Null(binding.ClrType);
    }

    [Fact]
    public void Compiler_CapturedLetArray_EmitsObjectScopeFieldWhenTdzChecksRequired()
    {
        var source = @"
                'use strict';
                let read = () => ret;
                let ret = [];

                read();
            ";

        var outputDir = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "CapturedLetArray", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDir);

        var compiled = TestCompiler.Compile(
            testName: "CapturedLetArray",
            testCategory: "SymbolTable",
            outputDirectory: outputDir,
            getJavaScriptAndSourcePath: _ => (source, null),
            additionalScripts: null);

        var il = Utilities.AssemblyToText.ConvertToText(compiled.AssemblyPath);
        Assert.Contains("object 'ret'", il);
        Assert.DoesNotContain("JavaScriptRuntime.Array 'ret'", il);
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
    [InlineData("const arr = [1, 2, 3]; const result = arr.some(x => x === 2);", "result", typeof(bool))]
    [InlineData("const arr = ['a', 'b']; const result = arr.some(x => x === 'c');", "result", typeof(bool))]
    [InlineData("let arr = [1, 2, 3]; let result = arr.some(x => x > 1);", "result", typeof(bool))]
    public void SymbolTable_InferType_ArraySomeWithVariableReference(string source, string variableName, Type expectedType)
    {
        var symbolTable = BuildSymbolTable(source);
        var binding = symbolTable.GetBindingInfo(variableName);
        Assert.NotNull(binding);
        Assert.Equal(expectedType, binding.ClrType);
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

    [Fact]
    public void SymbolTable_InferTypes_StableParameters_FunctionDeclaration_DirectCallsInferStableParameters()
    {
        var source = @"
                function format(a, b, enabled, prefix) {
                    return enabled ? prefix + (a + b) : 'off';
                }

                format(1, 2, true, 'sum=');
                format(4, 5, true, 'sum=');
            ";

        var symbolTable = BuildSymbolTable(source);
        var functionScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Global
            && string.Equals(s.Name, "format", StringComparison.Ordinal));

        Assert.NotNull(functionScope);
        AssertStableParameterTypes(
            functionScope!,
            ("a", typeof(double)),
            ("b", typeof(double)),
            ("enabled", typeof(bool)),
            ("prefix", typeof(string)));
    }

    [Fact]
    public void SymbolTable_InferTypes_StableParameters_FunctionDeclaration_DirectArrayCallsInferArrayParameter()
    {
        var source = @"
                function rotate(m, phi) {
                    return m[0][0] + phi;
                }

                const matrix = [[1, 0], [0, 1]];
                rotate(matrix, 30);
                rotate(matrix, 45);
            ";

        var symbolTable = BuildSymbolTable(source);
        var functionScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Global
            && string.Equals(s.Name, "rotate", StringComparison.Ordinal));

        Assert.NotNull(functionScope);
        AssertStableParameterTypes(
            functionScope!,
            ("m", typeof(JavaScriptRuntime.Array)),
            ("phi", typeof(double)));
    }

    [Fact]
    public void SymbolTable_InferTypes_StableParameters_FunctionDeclaration_PartialEvidenceInfersStablePositions()
    {
        var source = @"
                function translate(matrix, dx, dy, dz) {
                    return matrix[0][0] + dx + dy + dz;
                }

                const identity = [[1, 0], [0, 1]];
                const origin = { v: [1, 2, 3] };

                translate(identity, -origin.v[0], -origin.v[1], -origin.v[2]);
                translate(identity, origin.v[0], origin.v[1], origin.v[2]);
            ";

        var symbolTable = BuildSymbolTable(source);
        var functionScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Global
            && string.Equals(s.Name, "translate", StringComparison.Ordinal));

        Assert.NotNull(functionScope);
        Assert.Single(functionScope!.StableParameterClrTypes);
        AssertStableParameterType(functionScope, 0, "matrix", typeof(JavaScriptRuntime.Array));
        AssertObjectParameter(functionScope, "dx");
        AssertObjectParameter(functionScope, "dy");
        AssertObjectParameter(functionScope, "dz");
    }

    [Fact]
    public void SymbolTable_InferTypes_StableParameters_EscapedArrowKeepsObjectParameters()
    {
        var source = @"
                const add = (a, b) => {
                    return a + b;
                };

                module.exports = add;
                add(1, 2);
            ";

        var symbolTable = BuildSymbolTable(source);
        var functionScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Global
            && s.AstNode is ArrowFunctionExpression);

        Assert.NotNull(functionScope);
        Assert.Empty(functionScope!.StableParameterClrTypes);
        AssertObjectParameter(functionScope, "a");
        AssertObjectParameter(functionScope, "b");
    }

    [Fact]
    public void SymbolTable_InferTypes_StableParameters_ShadowedArrayDoesNotInferArrayParameter()
    {
        var source = @"
                const Array = {
                    from() { return 1; }
                };

                function consume(value) {
                    return value;
                }

                consume(Array.from());
            ";

        var symbolTable = BuildSymbolTable(source);
        var functionScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Global
            && string.Equals(s.Name, "consume", StringComparison.Ordinal));

        Assert.NotNull(functionScope);
        Assert.Empty(functionScope!.StableParameterClrTypes);
        AssertObjectParameter(functionScope, "value");
    }

    [Fact]
    public void SymbolTable_InferTypes_StableParameters_ShadowedNewArrayDoesNotInferArrayParameter()
    {
        var source = @"
                function Array() {
                }

                function consume(value) {
                    return value;
                }

                consume(new Array());
            ";

        var symbolTable = BuildSymbolTable(source);
        var functionScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Global
            && string.Equals(s.Name, "consume", StringComparison.Ordinal));

        Assert.NotNull(functionScope);
        Assert.Empty(functionScope!.StableParameterClrTypes);
        AssertObjectParameter(functionScope, "value");
    }

    [Fact]
    public void SymbolTable_InferTypes_StableParameters_MutableArrayIdentifierDoesNotInferArrayParameter()
    {
        var source = @"
                let value = [1];
                value = 0;

                function consume(value) {
                    return value;
                }

                consume(value);
            ";

        var symbolTable = BuildSymbolTable(source);
        var functionScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Global
            && string.Equals(s.Name, "consume", StringComparison.Ordinal));

        Assert.NotNull(functionScope);
        Assert.Empty(functionScope!.StableParameterClrTypes);
        AssertObjectParameter(functionScope, "value");
    }

    [Fact]
    public void SymbolTable_InferTypes_StableParameters_DirectArrowCallsInferArrayAfterStableReturnAssignments()
    {
        var source = @"
                const makeMatrix = () => [[1, 0], [0, 1]];
                let matrix = [];
                matrix = makeMatrix();

                const rotate = (m, phi) => {
                    return m[0][0] + phi;
                };

                rotate(matrix, 30);
            ";

        var symbolTable = BuildSymbolTable(source);
        var functionScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Global
            && s.AstNode is ArrowFunctionExpression
            && s.Parameters.Contains("m"));

        Assert.NotNull(functionScope);
        AssertStableParameterTypes(
            functionScope!,
            ("m", typeof(JavaScriptRuntime.Array)),
            ("phi", typeof(double)));
    }

    [Fact]
    public void SymbolTable_InferTypes_StableParameters_FunctionDeclaration_EscapedFunctionKeepsObjectParameters()
    {
        var source = @"
                function add(a, b) {
                    return a + b;
                }

                module.exports = add;
                add(1, 2);
            ";

        var symbolTable = BuildSymbolTable(source);
        var functionScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Global
            && string.Equals(s.Name, "add", StringComparison.Ordinal));

        Assert.NotNull(functionScope);
        Assert.Empty(functionScope!.StableParameterClrTypes);
        AssertObjectParameter(functionScope, "a");
        AssertObjectParameter(functionScope, "b");
    }

    [Fact]
    public void SymbolTable_InferTypes_StableParameters_ClassMethod_DirectThisCalls()
    {
        var source = @"
                class Accumulator {
                    run() {
                        this.add(2, 3);
                        this.add(10, 20);
                    }

                    add(a, b) {
                        return a + b;
                    }
                }

                new Accumulator().run();
            ";

        var symbolTable = BuildSymbolTable(source);
        var classScope = FindClassScope(symbolTable.Root, "Accumulator");
        Assert.NotNull(classScope);

        var methodScope = FindFirstScope(classScope!, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Class
            && string.Equals(s.Name, "add", StringComparison.Ordinal));

        Assert.NotNull(methodScope);
        AssertStableParameterTypes(
            methodScope!,
            ("a", typeof(double)),
            ("b", typeof(double)));
    }

    [Fact]
    public void SymbolTable_InferTypes_Dromaeo3dCube_TranslateAndMMultiInferPartialArrayParameters()
    {
        const string resourceName = "Jroc.Tests.Integration.JavaScript.Compile_Resources_Dromaeo_3d_Cube.js";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using var reader = new StreamReader(stream!);
        var source = reader.ReadToEnd();
        var symbolTable = BuildSymbolTable(source);

        var translateScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Global
            && string.Equals(s.Name, "Translate", StringComparison.Ordinal));
        var mMultiScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Global
            && string.Equals(s.Name, "MMulti", StringComparison.Ordinal));

        Assert.NotNull(translateScope);
        Assert.NotNull(mMultiScope);

        AssertStableParameterType(translateScope!, 0, "M", typeof(JavaScriptRuntime.Array));
        AssertObjectParameter(translateScope!, "Dx");
        AssertObjectParameter(translateScope!, "Dy");
        AssertObjectParameter(translateScope!, "Dz");

        AssertStableParameterType(mMultiScope!, 0, "M1", typeof(JavaScriptRuntime.Array));
        AssertStableParameterType(mMultiScope!, 1, "M2", typeof(JavaScriptRuntime.Array));
    }

    [Fact]
    public void SymbolTable_InferTypes_DromaeoGenerateTestStrings_ReturnType_IsArray()
    {
        const string resourceName = "Jroc.Tests.Integration.JavaScript.Compile_Performance_Dromaeo_Object_Regexp.js";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using var reader = new StreamReader(stream!);
        var source = reader.ReadToEnd();
        var symbolTable = BuildSymbolTable(source);

        var functionScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Global
            && string.Equals(s.Name, "generateTestStrings", StringComparison.Ordinal));

        Assert.NotNull(functionScope);
        Assert.Equal(typeof(JavaScriptRuntime.Array), functionScope!.StableReturnClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_DromaeoGenerateTestStrings_PropagatesArrayToTmp()
    {
        const string resourceName = "Jroc.Tests.Integration.JavaScript.Compile_Performance_Dromaeo_Object_Regexp.js";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using var reader = new StreamReader(stream!);
        var source = reader.ReadToEnd();
        var symbolTable = BuildSymbolTable(source);

        var tmpBinding = symbolTable.GetBindingInfo("tmp");
        Assert.NotNull(tmpBinding);
        Assert.True(tmpBinding!.IsStableType);
        Assert.Equal(typeof(JavaScriptRuntime.Array), tmpBinding.ClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_StableReturnIsThis_PrimeJavaScript_RunSieve()
    {
        const string resourceName = "Jroc.Tests.Integration.JavaScript.Compile_Performance_PrimeJavaScript.js";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using var reader = new StreamReader(stream!);
        var source = reader.ReadToEnd();
        var symbolTable = BuildSymbolTable(source);

        var classScope = FindClassScope(symbolTable.Root, "PrimeSieve");
        Assert.NotNull(classScope);

        var methodScope = FindFirstScope(classScope!, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Class
            && string.Equals(s.Name, "runSieve", StringComparison.Ordinal));

        Assert.NotNull(methodScope);
        Assert.True(methodScope!.StableReturnIsThis);
    }

    [Fact]
    public void SymbolTable_InferTypes_DromaeoObjectRegexp_GenerateTestStrings_T_IsString()
    {
        const string resourceName = "Jroc.Tests.Integration.JavaScript.Compile_Performance_Dromaeo_Object_Regexp.js";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using var reader = new StreamReader(stream!);
        var source = reader.ReadToEnd();
        var symbolTable = BuildSymbolTable(source);

        var functionScope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Global
            && string.Equals(s.Name, "generateTestStrings", StringComparison.Ordinal)
            && s.AstNode is FunctionDeclaration);

        Assert.NotNull(functionScope);
        Assert.True(functionScope!.Bindings.TryGetValue("t", out var tBinding));
        Assert.True(tBinding.IsStableType);
        Assert.Equal(typeof(string), tBinding.ClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_DromaeoObjectRegexp_TestStrings_ElementType_IsString()
    {
        const string resourceName = "Jroc.Tests.Integration.JavaScript.Compile_Performance_Dromaeo_Object_Regexp.js";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using var reader = new StreamReader(stream!);
        var source = reader.ReadToEnd();
        var symbolTable = BuildSymbolTable(source);

        var binding = symbolTable.GetBindingInfo("testStrings");
        Assert.NotNull(binding);
        Assert.True(binding!.IsStableType);
        Assert.Equal(typeof(JavaScriptRuntime.Array), binding.ClrType);
        Assert.Equal(typeof(string), binding.StableElementClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_DromaeoObjectRegexp_Tmp_ElementType_IsString()
    {
        const string resourceName = "Jroc.Tests.Integration.JavaScript.Compile_Performance_Dromaeo_Object_Regexp.js";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using var reader = new StreamReader(stream!);
        var source = reader.ReadToEnd();
        var symbolTable = BuildSymbolTable(source);

        var binding = symbolTable.GetBindingInfo("tmp");
        Assert.NotNull(binding);
        Assert.True(binding!.IsStableType);
        Assert.Equal(typeof(JavaScriptRuntime.Array), binding.ClrType);
        Assert.Equal(typeof(string), binding.StableElementClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_ArrayElementType_MixedWrites_IsNotStable()
    {
        var source = @"
                var foo = [];
                foo[0] = '1';
                foo[1] = 1.1;
            ";

        var symbolTable = BuildSymbolTable(source);
        var binding = symbolTable.GetBindingInfo("foo");
        Assert.NotNull(binding);
        Assert.True(binding!.IsStableType);
        Assert.Equal(typeof(JavaScriptRuntime.Array), binding.ClrType);
        Assert.Null(binding.StableElementClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_ArrayElementType_AliasedMixedWrite_IsNotStable()
    {
        var source = @"
                var foo = [];
                foo[0] = 'a';
                foo[1] = 'b';
                var foo2 = foo;
                foo2[0] = 0.0;
            ";

        var symbolTable = BuildSymbolTable(source);
        var binding = symbolTable.GetBindingInfo("foo");
        Assert.NotNull(binding);
        Assert.True(binding!.IsStableType);
        Assert.Equal(typeof(JavaScriptRuntime.Array), binding.ClrType);
        Assert.Null(binding.StableElementClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_ArrayElementType_AliasedConsistentWrite_RemainsStableString()
    {
        var source = @"
                var foo = [];
                foo[0] = 'a';
                var foo2 = foo;
                foo2[1] = 'b';
            ";

        var symbolTable = BuildSymbolTable(source);
        var binding = symbolTable.GetBindingInfo("foo");
        Assert.NotNull(binding);
        Assert.True(binding!.IsStableType);
        Assert.Equal(typeof(JavaScriptRuntime.Array), binding.ClrType);
        Assert.Equal(typeof(string), binding.StableElementClrType);
    }

    [Fact]
    public void SymbolTable_StableElementClrType_Guard_RequiresStableArrayBinding()
    {
        var symbolTable = BuildSymbolTable("var x = 1;");
        var binding = symbolTable.GetBindingInfo("x");
        Assert.NotNull(binding);

        binding!.IsStableType = true;
        binding.ClrType = typeof(string);
        binding.StableElementClrType = typeof(string);
        Assert.Null(binding.StableElementClrType);

        binding.ClrType = typeof(JavaScriptRuntime.Array);
        binding.StableElementClrType = typeof(string);
        Assert.Equal(typeof(string), binding.StableElementClrType);

        binding.IsStableType = false;
        Assert.Null(binding.StableElementClrType);
    }

    [Fact]
    public void SymbolTable_StableReturnArrayElementClrType_Guard_RequiresArrayReturn()
    {
        var symbolTable = BuildSymbolTable("function f() { return 1; }");
        var scope = FindFirstScope(symbolTable.Root, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Global
            && string.Equals(s.Name, "f", StringComparison.Ordinal));
        Assert.NotNull(scope);

        scope!.StableReturnClrType = typeof(string);
        scope.StableReturnArrayElementClrType = typeof(string);
        Assert.Null(scope.StableReturnArrayElementClrType);

        scope.StableReturnClrType = typeof(JavaScriptRuntime.Array);
        scope.StableReturnArrayElementClrType = typeof(string);
        Assert.Equal(typeof(string), scope.StableReturnArrayElementClrType);

        scope.StableReturnClrType = typeof(double);
        Assert.Null(scope.StableReturnArrayElementClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_PrimeJavaScript_RunSieve_InfersQIsNumber()
    {
        const string resourceName = "Jroc.Tests.Integration.JavaScript.Compile_Performance_PrimeJavaScript.js";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using var reader = new StreamReader(stream!);
        var source = reader.ReadToEnd();
        var symbolTable = BuildSymbolTable(source);

        var binding = symbolTable.GetBindingInfo("PrimeSieve/runSieve/q");
        Assert.NotNull(binding);
        Assert.True(binding!.IsStableType);
        Assert.Equal(typeof(double), binding.ClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_PrimeJavaScript_RunSieve_InfersStepAndStartAreNumbers()
    {
        const string resourceName = "Jroc.Tests.Integration.JavaScript.Compile_Performance_PrimeJavaScript.js";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using var reader = new StreamReader(stream!);
        var source = reader.ReadToEnd();
        var symbolTable = BuildSymbolTable(source);

        var classScope = FindClassScope(symbolTable.Root, "PrimeSieve");
        Assert.NotNull(classScope);

        var methodScope = FindFirstScope(classScope!, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Class
            && string.Equals(s.Name, "runSieve", StringComparison.Ordinal));
        Assert.NotNull(methodScope);

        // step/start are declared inside the `while` body block scope.
        var step = FindBindingByName(methodScope!, "step");
        Assert.NotNull(step);
        Assert.True(step!.IsStableType);
        Assert.Equal(typeof(double), step.ClrType);

        var start = FindBindingByName(methodScope!, "start");
        Assert.NotNull(start);
        Assert.True(start!.IsStableType);
        Assert.Equal(typeof(double), start.ClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_PrimeJavaScript_SetBitsTrue_InfersNumericParameters()
    {
        const string resourceName = "Jroc.Tests.Integration.JavaScript.Compile_Performance_PrimeJavaScript.js";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using var reader = new StreamReader(stream!);
        var source = reader.ReadToEnd();
        var symbolTable = BuildSymbolTable(source);

        var classScope = FindClassScope(symbolTable.Root, "BitArray");
        Assert.NotNull(classScope);

        var methodScope = FindFirstScope(classScope!, s =>
            s.Kind == ScopeKind.Function
            && s.Parent?.Kind == ScopeKind.Class
            && string.Equals(s.Name, "setBitsTrue", StringComparison.Ordinal));

        Assert.NotNull(methodScope);
        AssertStableParameterTypes(
            methodScope!,
            ("range_start", typeof(double)),
            ("step", typeof(double)),
            ("range_stop", typeof(double)));

        var index = FindBindingByName(methodScope!, "index");
        Assert.NotNull(index);
        Assert.True(index!.IsStableType);
        Assert.Equal(typeof(double), index.ClrType);
    }

    [Fact]
    public void SymbolTable_InferTypes_PrimeStyle_TypedArrayElementRead_InfersWordValueIsNumber()
    {
        // This reproduces the PrimeJavaScript pattern:
        // let wordValue = this.wordArray[wordOffset];
        var source = @"
                'use strict';

                const WORD_SIZE = 32;

                class BitArray {
                    constructor(size) {
                        this.wordArray = new Int32Array(1 + (size >>> 5));
                    }

                    setBitsTrue(range_start, step, range_stop) {
                        let index = range_start;
                        let wordOffset = index >>> 5;
                        let wordValue = this.wordArray[wordOffset];

                        while (index < range_stop) {
                            const bitOffset = index & 31;
                            wordValue |= (1 << bitOffset);
                            index += step;
                            const newwordOffset = index >>> 5;
                            if (newwordOffset != wordOffset) {
                                this.wordArray[wordOffset] = wordValue;
                                wordOffset = newwordOffset;
                                wordValue = this.wordArray[wordOffset];
                            }
                        }

                        this.wordArray[wordOffset] = wordValue;
                    }
                }

                new BitArray(64).setBitsTrue(4, 3, 64);
            ";

        var symbolTable = BuildSymbolTable(source);
        var binding = symbolTable.GetBindingInfo("BitArray/setBitsTrue/wordValue");
        Assert.NotNull(binding);
        Assert.True(binding!.IsStableType);
        Assert.Equal(typeof(double), binding.ClrType);

    }

    private static Jroc.SymbolTables.Scope? FindFirstScope(Jroc.SymbolTables.Scope scope, Func<Jroc.SymbolTables.Scope, bool> predicate)
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

    private static Jroc.SymbolTables.Scope? FindClassScope(Jroc.SymbolTables.Scope scope, string className)
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

    private BindingInfo? FindBindingByName(Jroc.SymbolTables.Scope scope, string name)
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

    private static void AssertStableParameterTypes(
        Jroc.SymbolTables.Scope scope,
        params (string Name, Type ClrType)[] expected)
    {
        Assert.Equal(expected.Length, scope.StableParameterClrTypes.Count);

        for (var i = 0; i < expected.Length; i++)
        {
            var (name, clrType) = expected[i];
            Assert.True(scope.StableParameterClrTypes.TryGetValue(i, out var actualType), $"Expected stable parameter type for '{name}' at index {i}.");
            Assert.Equal(clrType, actualType);

            Assert.True(scope.Bindings.TryGetValue(name, out var binding), $"Expected binding for parameter '{name}'.");
            Assert.True(binding!.IsStableType);
            Assert.Equal(clrType, binding.ClrType);
        }
    }

    private static void AssertStableParameterType(
        Jroc.SymbolTables.Scope scope,
        int index,
        string name,
        Type clrType)
    {
        Assert.True(scope.StableParameterClrTypes.TryGetValue(index, out var actualType), $"Expected stable parameter type for '{name}' at index {index}.");
        Assert.Equal(clrType, actualType);

        Assert.True(scope.Bindings.TryGetValue(name, out var binding), $"Expected binding for parameter '{name}'.");
        Assert.True(binding!.IsStableType);
        Assert.Equal(clrType, binding.ClrType);
    }

    private static void AssertObjectParameter(Jroc.SymbolTables.Scope scope, string parameterName)
    {
        Assert.True(scope.Bindings.TryGetValue(parameterName, out var binding), $"Expected binding for parameter '{parameterName}'.");
        Assert.False(binding!.IsStableType);
        Assert.Null(binding.ClrType);
    }

    private SymbolTable BuildSymbolTable(string source)
    {
        var ast = _parser.ParseJavaScript(source, "test.js");
        var symbolTable = BuildSymbolTable(ast, "test.js");
        return symbolTable;
    }
}
