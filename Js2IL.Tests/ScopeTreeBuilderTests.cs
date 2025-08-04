using Acornima;
using Js2IL.Scoping;
using Js2IL.Services;
using System.Linq;
using Xunit;

namespace Js2IL.Tests
{
    public class ScopeTreeBuilderTests
    {
        private readonly JavaScriptParser _parser;
        private readonly ScopeTreeBuilder _scopeBuilder;

        public ScopeTreeBuilderTests()
        {
            _parser = new JavaScriptParser();
            _scopeBuilder = new ScopeTreeBuilder();
        }

        [Fact]
        public void Build_SimpleVariableDeclaration_CreatesGlobalScopeWithBinding()
        {
            // Arrange
            var code = "var x = 5;";
            var ast = _parser.ParseJavaScript(code);

            // Act
            var scopeTree = _scopeBuilder.Build(ast, "test.js");

            // Assert
            Assert.Equal("test", scopeTree.Root.Name);
            Assert.Equal(ScopeKind.Global, scopeTree.Root.Kind);
            Assert.True(scopeTree.Root.Bindings.ContainsKey("x"));
            Assert.Equal(BindingKind.Var, scopeTree.Root.Bindings["x"].Kind);
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
            var ast = _parser.ParseJavaScript(code);

            // Act
            var scopeTree = _scopeBuilder.Build(ast, "test.js");

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
            var ast = _parser.ParseJavaScript(code);

            // Act
            var scopeTree = _scopeBuilder.Build(ast, "test.js");

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
            var ast = _parser.ParseJavaScript(code);

            // Act
            var scopeTree = _scopeBuilder.Build(ast, "test.js");

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
            var ast = _parser.ParseJavaScript(code);

            // Act
            var scopeTree = _scopeBuilder.Build(ast, "test.js");

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
            var ast = _parser.ParseJavaScript(code);

            // Act
            var scopeTree = _scopeBuilder.Build(ast, "test.js");

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
            var ast = _parser.ParseJavaScript(code);

            // Act
            var scopeTree = _scopeBuilder.Build(ast, "test.js");

            // Assert
            // Should have one child scope with descriptive name based on assignment
            Assert.Single(scopeTree.Root.Children);
            var funcScope = scopeTree.Root.Children[0];
            Assert.Equal("Function_callback", funcScope.Name);
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
            var ast = _parser.ParseJavaScript(code);

            // Act
            var scopeTree = _scopeBuilder.Build(ast, "complex.js");

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
            Assert.True(blockScope.Bindings.ContainsKey("innerFunc"));

            var arrowScope = blockScope.Children[0];
            Assert.StartsWith("ArrowFunction", arrowScope.Name);
            Assert.True(arrowScope.Bindings.ContainsKey("arrowVar"));
        }
    }
}
