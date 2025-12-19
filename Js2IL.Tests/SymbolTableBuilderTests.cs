using Acornima;
using Acornima.Ast;
using Js2IL.SymbolTables;
using Js2IL.Services;
using System.Linq;
using Xunit;

namespace Js2IL.Tests
{
    public class SymbolTableBuilderTests
    {
        private readonly JavaScriptParser _parser;
        private readonly SymbolTableBuilder _scopeBuilder;

        public SymbolTableBuilderTests()
        {
            _parser = new JavaScriptParser();
            _scopeBuilder = new SymbolTableBuilder();
        }

        // Helper method to adapt old test API to new ModuleDefinition-based API
        private SymbolTable BuildSymbolTable(Acornima.Ast.Program ast, string fileName)
        {
            var module = new ModuleDefinition
            {
                Ast = ast,
                Path = fileName
            };
            return _scopeBuilder.Build(module);
        }

        [Fact]
        public void Build_SimpleVariableDeclaration_CreatesGlobalScopeWithBinding()
        {
            // Arrange
            var code = "var x = 5;";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            Assert.Equal("test", scopeTree.Root.Name);
            Assert.Equal(ScopeKind.Global, scopeTree.Root.Kind);
            Assert.True(scopeTree.Root.Bindings.ContainsKey("x"));
            Assert.Equal(BindingKind.Var, scopeTree.Root.Bindings["x"].Kind);
            Assert.False(scopeTree.Root.Bindings["x"].IsCaptured);
        }

        [Fact]
        public void Build_LetAndConstDeclarations_CreatesCorrectBindingKinds()
        {
            // Arrange
            var code = @"
                let y = 10;
                const z = 20;
                var x = 5;
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            Assert.Equal(BindingKind.Let, scopeTree.Root.Bindings["y"].Kind);
            Assert.Equal(BindingKind.Const, scopeTree.Root.Bindings["z"].Kind);
            Assert.Equal(BindingKind.Var, scopeTree.Root.Bindings["x"].Kind);
        }

        [Fact]
        public void Build_FunctionDeclaration_CreatesNestedScope()
        {
            // Arrange
            var code = @"
                function myFunction(param1, param2) {
                    var localVar = 10;
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have the function binding
            Assert.True(scopeTree.Root.Bindings.ContainsKey("myFunction"));
            Assert.Equal(BindingKind.Function, scopeTree.Root.Bindings["myFunction"].Kind);

            // Should have one child scope (the function)
            Assert.Single(scopeTree.Root.Children);
            var funcScope = scopeTree.Root.Children[0];
            Assert.Equal("myFunction", funcScope.Name);
            Assert.Equal(ScopeKind.Function, funcScope.Kind);

            // Function scope should have parameters and local variable
            Assert.True(funcScope.Bindings.ContainsKey("param1"));
            Assert.True(funcScope.Bindings.ContainsKey("param2"));
            Assert.True(funcScope.Bindings.ContainsKey("localVar"));
            Assert.Equal(BindingKind.Var, funcScope.Bindings["param1"].Kind);
            Assert.Equal(BindingKind.Var, funcScope.Bindings["localVar"].Kind);
        }

        [Fact]
        public void Build_NestedFunctions_CreatesCorrectHierarchy()
        {
            // Arrange
            var code = @"
                function outer(x) {
                    function inner(y) {
                        var innerVar = y;
                    }
                    var outerVar = x;
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            var outerScope = scopeTree.Root.Children[0];
            Assert.Equal("outer", outerScope.Name);
            Assert.Single(outerScope.Children);

            var innerScope = outerScope.Children[0];
            Assert.Equal("inner", innerScope.Name);
            Assert.True(innerScope.Bindings.ContainsKey("y"));
            Assert.True(innerScope.Bindings.ContainsKey("innerVar"));
        }

        [Fact]
        public void Build_BlockStatement_CreatesBlockScope()
        {
            // Arrange
            var code = @"
                var x = 1;
                {
                    let blockVar = 2;
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have x
            Assert.True(scopeTree.Root.Bindings.ContainsKey("x"));

            // Should have one child scope (the block)
            Assert.Single(scopeTree.Root.Children);
            var blockScope = scopeTree.Root.Children[0];
            Assert.Equal(ScopeKind.Block, blockScope.Kind);
            Assert.StartsWith("Block_", blockScope.Name);
            Assert.True(blockScope.Bindings.ContainsKey("blockVar"));
            Assert.Equal(BindingKind.Let, blockScope.Bindings["blockVar"].Kind);
        }

        [Fact]
        public void Build_ArrowFunction_CreatesArrowFunctionScope()
        {
            // Arrange
            var code = @"
                var func = (a, b) => {
                    var result = a + b;
                    return result;
                };
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have func variable
            Assert.True(scopeTree.Root.Bindings.ContainsKey("func"));

            // Should have one child scope (the arrow function)
            Assert.Single(scopeTree.Root.Children);
            var arrowScope = scopeTree.Root.Children[0];
            Assert.Equal(ScopeKind.Function, arrowScope.Kind);
            Assert.StartsWith("ArrowFunction", arrowScope.Name);
            
            // Arrow function should have parameters and local variable
            Assert.True(arrowScope.Bindings.ContainsKey("a"));
            Assert.True(arrowScope.Bindings.ContainsKey("b"));
            Assert.True(arrowScope.Bindings.ContainsKey("result"));
        }

        [Fact]
        public void Build_AnonymousFunction_GetsGeneratedName()
        {
            // Arrange
            var code = @"
                var callback = function(x) {
                    return x * 2;
                };
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Should have one child scope with descriptive name based on assignment
            Assert.Single(scopeTree.Root.Children);
            var funcScope = scopeTree.Root.Children[0];
            Assert.Equal("FunctionExpression_callback", funcScope.Name);
            Assert.True(funcScope.Bindings.ContainsKey("x"));
        }

        [Fact]
        public void Build_ComplexNestedStructure_CreatesCorrectTree()
        {
            // Arrange
            var code = @"
                var globalVar = 1;
                function outerFunc(param1) {
                    let outerLet = 2;
                    {
                        const blockConst = 3;
                        var innerFunc = () => {
                            var arrowVar = 4;
                        };
                    }
                }
            ";
            var ast = _parser.ParseJavaScript(code, "complex.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "complex.js");

            // Assert
            Assert.Equal("complex", scopeTree.Root.Name);
            Assert.True(scopeTree.Root.Bindings.ContainsKey("globalVar"));
            Assert.True(scopeTree.Root.Bindings.ContainsKey("outerFunc"));

            var outerFuncScope = scopeTree.Root.Children[0];
            Assert.Equal("outerFunc", outerFuncScope.Name);
            Assert.True(outerFuncScope.Bindings.ContainsKey("param1"));
            Assert.True(outerFuncScope.Bindings.ContainsKey("outerLet"));

            var blockScope = outerFuncScope.Children[0];
            Assert.Equal(ScopeKind.Block, blockScope.Kind);
            Assert.True(blockScope.Bindings.ContainsKey("blockConst"));
            // var-declared identifiers inside a block hoist to the nearest function scope
            Assert.True(outerFuncScope.Bindings.ContainsKey("innerFunc"));
            Assert.DoesNotContain("innerFunc", blockScope.Bindings.Keys);

            var arrowScope = blockScope.Children[0];
            Assert.StartsWith("ArrowFunction", arrowScope.Name);
            Assert.True(arrowScope.Bindings.ContainsKey("arrowVar"));
        }

        [Fact]
        public void Build_ForLoopWithArray_CreatesCorrectScopes()
        {
            // Arrange - Similar to ArrayLiteral.js
            var code = @"
                var x = [ 'cat', 'dog', 'dotnet bot' ];
                for (var index = 0; index < x.length; index++) {
                    console.log('', x[index]);
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have x
            Assert.True(scopeTree.Root.Bindings.ContainsKey("x"));
            Assert.Equal(BindingKind.Var, scopeTree.Root.Bindings["x"].Kind);

            // Global scope should also have index (var declarations in for loop hoist to global)
            Assert.True(scopeTree.Root.Bindings.ContainsKey("index"));
            Assert.Equal(BindingKind.Var, scopeTree.Root.Bindings["index"].Kind);

            // Should have one child scope (the for loop block)
            Assert.Single(scopeTree.Root.Children);
            var forBlockScope = scopeTree.Root.Children[0];
            Assert.Equal(ScopeKind.Block, forBlockScope.Kind);
            Assert.StartsWith("Block_", forBlockScope.Name);

            // The for loop block should not have any bindings (console.log is just an expression)
            Assert.Empty(forBlockScope.Bindings);
        }

        [Fact]
        public void Build_IIFE_CreatesFunctionExpressionScope_WithLocationBasedName()
        {
            // Arrange - anonymous function expression invoked immediately (IIFE)
            var code = @"(function(x) { return x; })(42);";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have no binding for the anonymous function (not assigned)
            Assert.DoesNotContain(scopeTree.Root.Bindings, kvp => kvp.Value.Kind == BindingKind.Function);

            // Should have one child scope for the function expression
            Assert.Single(scopeTree.Root.Children);
            var iifeScope = scopeTree.Root.Children[0];
            Assert.Equal(ScopeKind.Function, iifeScope.Kind);
            Assert.StartsWith("FunctionExpression_", iifeScope.Name);
            Assert.Contains("_L", iifeScope.Name); // location-based naming
            Assert.Contains("C", iifeScope.Name);

            // Parameter 'x' should be a binding in the function scope
            Assert.True(iifeScope.Bindings.ContainsKey("x"));
            Assert.Equal(BindingKind.Var, iifeScope.Bindings["x"].Kind);
        }

        [Fact]
        public void Build_NamedFunctionExpressionIIFE_InternalBindingOnlyInFunction()
        {
            // Arrange - named function expression invoked immediately
            var code = @"(function walk(n) { return n; })(0);";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should not contain a binding for the internal name 'walk'
            Assert.DoesNotContain(scopeTree.Root.Bindings, kvp => kvp.Key == "walk");

            // There should be exactly one child scope (the function expression)
            Assert.Single(scopeTree.Root.Children);
            var funcScope = scopeTree.Root.Children[0];
            Assert.Equal(ScopeKind.Function, funcScope.Kind);
            // Named function expression uses its internal name for the scope name
            Assert.Equal("walk", funcScope.Name);

            // Internal binding 'walk' is available inside the function scope only
            Assert.True(funcScope.Bindings.ContainsKey("walk"));
            Assert.Equal(BindingKind.Function, funcScope.Bindings["walk"].Kind);

            // Parameter 'n' should also be present in the function scope
            Assert.True(funcScope.Bindings.ContainsKey("n"));
            Assert.Equal(BindingKind.Var, funcScope.Bindings["n"].Kind);
        }

        [Fact]
        public void SymbolTable_GlobalVariable_CreatesCorrectBindings()
        {
            // Arrange
            var code = @"
                var globalVar = 1;
                function myFunc() {
                    return 'globalVar';
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have globalVar and myFunc bindings
            Assert.True(scopeTree.Root.Bindings.ContainsKey("globalVar"));
            Assert.True(scopeTree.Root.Bindings.ContainsKey("myFunc"));

            var funcScope = scopeTree.Root.Children[0];
            Assert.Equal("myFunc", funcScope.Name);
            // Function scope should not have its own binding for globalVar
            Assert.False(funcScope.Bindings.ContainsKey("globalVar"));

            // However, globalVar should be marked as captured
            var globalVarBinding = scopeTree.Root.Bindings["globalVar"];
            Assert.False(globalVarBinding.IsCaptured);
        }

        [Fact]
        public void SymbolTable_GlobalVariableCapturedInFunction_CreatesCorrectBindings()
        {
            // Arrange
            var code = @"
                var globalVar = 1;
                function myFunc() {
                    return globalVar;
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have globalVar and myFunc bindings
            Assert.True(scopeTree.Root.Bindings.ContainsKey("globalVar"));
            Assert.True(scopeTree.Root.Bindings.ContainsKey("myFunc"));

            var funcScope = scopeTree.Root.Children[0];
            Assert.Equal("myFunc", funcScope.Name);
            // Function scope should not have its own binding for globalVar
            Assert.False(funcScope.Bindings.ContainsKey("globalVar"));

            // However, globalVar should be marked as captured
            var globalVarBinding = scopeTree.Root.Bindings["globalVar"];
            Assert.True(globalVarBinding.IsCaptured);
        }

        [Fact]
        public void SymbolTable_GlobalVariableCapturedInNestedFunction_CreatesCorrectBindings()
        {
            // Arrange
            var code = @"
                var globalVar = 1;
                function myFunc() {
                    function innerFunc() {
                        return globalVar;
                    }
                    return innerFunc();
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have globalVar and myFunc bindings
            Assert.True(scopeTree.Root.Bindings.ContainsKey("globalVar"));

            // However, globalVar should be marked as captured
            var globalVarBinding = scopeTree.Root.Bindings["globalVar"];
            Assert.True(globalVarBinding.IsCaptured);
        }

        [Fact]
        public void SymbolTable_GlobalVariableCapturedInArrowFunction_CreatesCorrectBindings()
        {
            // Arrange
            var code = @"
                var globalVar = 1;
                const myFunc = () => {
                    return globalVar;
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have globalVar and myFunc bindings
            Assert.True(scopeTree.Root.Bindings.ContainsKey("globalVar"));

            // However, globalVar should be marked as captured
            var globalVarBinding = scopeTree.Root.Bindings["globalVar"];
            Assert.True(globalVarBinding.IsCaptured);
        }

        [Fact]
        public void SymbolTable_GlobalVariableCapturedInNestedArrowFunction_CreatesCorrectBindings()
        {
            // Arrange
            var code = @"
                var globalVar = 1;
                const myFunc = () => {
                    const innerFunc = () => {
                        return globalVar;
                    }
                    return innerFunc();
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have globalVar and myFunc bindings
            Assert.True(scopeTree.Root.Bindings.ContainsKey("globalVar"));

            // However, globalVar should be marked as captured
            var globalVarBinding = scopeTree.Root.Bindings["globalVar"];
            Assert.True(globalVarBinding.IsCaptured);
        }

        [Fact]
        public void SymbolTable_GlobalVariableCapturedInClassConstructor_CreatesCorrectBindings()
        {
            // Arrange
            var code = @"
                var globalVar = 1;
                class MyClass {
                    constructor() {
                        this.var = globalVar;
                    }
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have globalVar and myFunc bindings
            Assert.True(scopeTree.Root.Bindings.ContainsKey("globalVar"));

            // However, globalVar should be marked as captured
            var globalVarBinding = scopeTree.Root.Bindings["globalVar"];
            Assert.True(globalVarBinding.IsCaptured);
        }

        [Fact]
        public void SymbolTable_GlobalVariableCapturedInClassMethod_CreatesCorrectBindings()
        {
            // Arrange
            var code = @"
                var globalVar = 1;
                class MyClass {
                    someMethod() {
                        this.var = globalVar;
                    }
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have globalVar and myFunc bindings
            Assert.True(scopeTree.Root.Bindings.ContainsKey("globalVar"));

            // However, globalVar should be marked as captured
            var globalVarBinding = scopeTree.Root.Bindings["globalVar"];
            Assert.True(globalVarBinding.IsCaptured);
        }

        [Fact]
        public void SymbolTable_GlobalVariableCapturedInClassInstanceField_CreatesCorrectBindings()
        {
            // Arrange
            var code = @"
                var globalVar = 1;
                class MyClass {
                    instanceField = globalVar;
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have globalVar and myFunc bindings
            Assert.True(scopeTree.Root.Bindings.ContainsKey("globalVar"));

            // However, globalVar should be marked as captured
            var globalVarBinding = scopeTree.Root.Bindings["globalVar"];
            Assert.True(globalVarBinding.IsCaptured);
        }

        [Fact]
        public void SymbolTable_GlobalVariableCapturedInClassInstanceFieldExpression_CreatesCorrectBindings()
        {
            // Arrange
            var code = @"
                var globalVar = 1;
                class MyClass {
                    instanceField = globalVar + 10;
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have globalVar and myFunc bindings
            Assert.True(scopeTree.Root.Bindings.ContainsKey("globalVar"));

            // However, globalVar should be marked as captured
            var globalVarBinding = scopeTree.Root.Bindings["globalVar"];
            Assert.True(globalVarBinding.IsCaptured);
        }

                [Fact]
        public void SymbolTable_ShadowedVariableCapturedInNestedFunction_CreatesCorrectBindings()
        {
            // Arrange
            var code = @"
                var someValue = 1;
                function outerFunc() {
                    var someValue = 2;
                    function innerFunc() {
                        return someValue;
                    }
                    return innerFunc();
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope variable is NOT captured
            Assert.True(scopeTree.Root.Bindings.ContainsKey("someValue"));
            var globalVarBinding = scopeTree.Root.Bindings["someValue"];
            Assert.False(globalVarBinding.IsCaptured);

            // Outer function variable IS captured
            var outerFuncScope = scopeTree.Root.Children[0];
            Assert.True(outerFuncScope.Bindings.ContainsKey("someValue"));
            var outerFuncVarBinding = outerFuncScope.Bindings["someValue"];
            Assert.True(outerFuncVarBinding.IsCaptured);
        }

        [Fact]
        public void Build_ArrowFunctionWithObjectPatternParameter_CreatesBindingsAndParameters()
        {
            // Arrange
            var code = @"
                const arrow = ({a, b}) => {
                    console.log(a, b);
                };
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Global scope should have the arrow function binding
            Assert.True(scopeTree.Root.Bindings.ContainsKey("arrow"));

            // Should have one child scope (the arrow function)
            Assert.Single(scopeTree.Root.Children);
            var arrowScope = scopeTree.Root.Children[0];
            Assert.Equal(ScopeKind.Function, arrowScope.Kind);

            // Arrow function scope should have bindings for destructured parameters
            Assert.True(arrowScope.Bindings.ContainsKey("a"));
            Assert.True(arrowScope.Bindings.ContainsKey("b"));
            Assert.Equal(BindingKind.Var, arrowScope.Bindings["a"].Kind);
            Assert.Equal(BindingKind.Var, arrowScope.Bindings["b"].Kind);

            // Both should be marked as parameters
            Assert.Contains("a", arrowScope.Parameters);
            Assert.Contains("b", arrowScope.Parameters);
        }

        [Fact]
        public void Build_ArrowFunctionWithObjectPatternParameterAndDefaults_CreatesBindingsAndParameters()
        {
            // Arrange
            var code = @"
                const arrow = ({host = 'localhost', port = 8080}) => {
                    console.log(host, port);
                };
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Should have one child scope (the arrow function)
            Assert.Single(scopeTree.Root.Children);
            var arrowScope = scopeTree.Root.Children[0];
            Assert.Equal(ScopeKind.Function, arrowScope.Kind);

            // Arrow function scope should have bindings for destructured parameters with defaults
            Assert.True(arrowScope.Bindings.ContainsKey("host"));
            Assert.True(arrowScope.Bindings.ContainsKey("port"));
            Assert.Equal(BindingKind.Var, arrowScope.Bindings["host"].Kind);
            Assert.Equal(BindingKind.Var, arrowScope.Bindings["port"].Kind);

            // Both should be marked as parameters
            Assert.Contains("host", arrowScope.Parameters);
            Assert.Contains("port", arrowScope.Parameters);
        }

        [Fact]
        public void Build_ArrowFunctionWithMixedObjectPatternParameter_CreatesBindingsAndParameters()
        {
            // Arrange
            var code = @"
                const arrow = ({a, b, host = 'localhost', port = 8080, secure}) => {
                    console.log(a, b, host, port, secure);
                };
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Should have one child scope (the arrow function)
            Assert.Single(scopeTree.Root.Children);
            var arrowScope = scopeTree.Root.Children[0];
            Assert.Equal(ScopeKind.Function, arrowScope.Kind);

            // Arrow function scope should have bindings for all destructured parameters
            Assert.True(arrowScope.Bindings.ContainsKey("a"));
            Assert.True(arrowScope.Bindings.ContainsKey("b"));
            Assert.True(arrowScope.Bindings.ContainsKey("host"));
            Assert.True(arrowScope.Bindings.ContainsKey("port"));
            Assert.True(arrowScope.Bindings.ContainsKey("secure"));

            // All should be marked as parameters
            Assert.Contains("a", arrowScope.Parameters);
            Assert.Contains("b", arrowScope.Parameters);
            Assert.Contains("host", arrowScope.Parameters);
            Assert.Contains("port", arrowScope.Parameters);
            Assert.Contains("secure", arrowScope.Parameters);
        }

        [Fact]
        public void Build_ClassConstructorWithDestructuredParameters_AddsToParameters()
        {
            // Arrange
            var code = @"
                class Point {
                    constructor({ x, y }) {
                        this.x = x;
                        this.y = y;
                    }
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Should have Point class scope
            var pointScope = scopeTree.Root.Children[0];
            Assert.Equal("Point", pointScope.Name);
            Assert.Equal(ScopeKind.Class, pointScope.Kind);

            // Should have constructor scope as child
            var constructorScope = pointScope.Children.FirstOrDefault(c => c.Name == "constructor");
            Assert.NotNull(constructorScope);
            Assert.Equal(ScopeKind.Function, constructorScope.Kind);

            // Constructor should have bindings for destructured parameters
            Assert.True(constructorScope.Bindings.ContainsKey("x"));
            Assert.True(constructorScope.Bindings.ContainsKey("y"));

            // CRITICAL: Destructured properties must be in Parameters set
            Assert.Contains("x", constructorScope.Parameters);
            Assert.Contains("y", constructorScope.Parameters);
        }

        [Fact]
        public void Build_ClassMethodWithDestructuredParameters_AddsToParameters()
        {
            // Arrange
            var code = @"
                class Config {
                    update({ host, port, secure = false }) {
                        this.host = host;
                        this.port = port;
                        this.secure = secure;
                    }
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Should have Config class scope
            var configScope = scopeTree.Root.Children[0];
            Assert.Equal("Config", configScope.Name);
            Assert.Equal(ScopeKind.Class, configScope.Kind);

            // Should have update method scope as child
            var updateScope = configScope.Children.FirstOrDefault(c => c.Name == "update");
            Assert.NotNull(updateScope);
            Assert.Equal(ScopeKind.Function, updateScope.Kind);

            // Method should have bindings for destructured parameters
            Assert.True(updateScope.Bindings.ContainsKey("host"));
            Assert.True(updateScope.Bindings.ContainsKey("port"));
            Assert.True(updateScope.Bindings.ContainsKey("secure"));

            // CRITICAL: Destructured properties must be in Parameters set
            Assert.Contains("host", updateScope.Parameters);
            Assert.Contains("port", updateScope.Parameters);
            Assert.Contains("secure", updateScope.Parameters);
        }

        [Fact]
        public void Build_FunctionWithDestructuredParameters_AddsToParameters()
        {
            // Arrange
            var code = @"
                function createPoint({ x, y }) {
                    return { x, y };
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            // Should have function scope
            var funcScope = scopeTree.Root.Children[0];
            Assert.Equal("createPoint", funcScope.Name);
            Assert.Equal(ScopeKind.Function, funcScope.Kind);

            // Function should have bindings for destructured parameters
            Assert.True(funcScope.Bindings.ContainsKey("x"));
            Assert.True(funcScope.Bindings.ContainsKey("y"));

            // CRITICAL: Destructured properties must be in Parameters set
            Assert.Contains("x", funcScope.Parameters);
            Assert.Contains("y", funcScope.Parameters);
        }
    }
}

