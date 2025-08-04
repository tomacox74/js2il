using Acornima;
using Js2IL.Scoping;
using Js2IL.Services;
using Xunit;

namespace Js2IL.Tests
{
    public class ScopeNamingTests
    {
        private readonly JavaScriptParser _parser;
        private readonly ScopeTreeBuilder _scopeBuilder;

        public ScopeNamingTests()
        {
            _parser = new JavaScriptParser();
            _scopeBuilder = new ScopeTreeBuilder();
        }

        [Fact]
        public void Build_ArrowFunctionWithAssignment_UsesDescriptiveName()
        {
            // Arrange
            var code = @"
                var func = () => {
                    console.log('test');
                };
            ";
            var ast = _parser.ParseJavaScript(code);

            // Act
            var scopeTree = _scopeBuilder.Build(ast, "test.js");

            // Assert
            var arrowScope = scopeTree.Root.Children[0];
            Assert.Equal("ArrowFunction_func", arrowScope.Name);
        }

        [Fact]
        public void Build_FunctionExpressionWithAssignment_UsesDescriptiveName()
        {
            // Arrange
            var code = @"
                var callback = function() {
                    return 42;
                };
            ";
            var ast = _parser.ParseJavaScript(code);

            // Act
            var scopeTree = _scopeBuilder.Build(ast, "test.js");

            // Assert
            var funcScope = scopeTree.Root.Children[0];
            Assert.Equal("Function_callback", funcScope.Name);
        }

        [Fact]
        public void Build_ArrowFunctionNoAssignment_UsesGenericName()
        {
            // Arrange
            var code = @"
                setTimeout(() => {
                    console.log('test');
                }, 1000);
            ";
            var ast = _parser.ParseJavaScript(code);

            // Act
            var scopeTree = _scopeBuilder.Build(ast, "test.js");

            // Assert
            var arrowScope = scopeTree.Root.Children[0];
            Assert.StartsWith("ArrowFunction", arrowScope.Name);
            Assert.Contains("_L", arrowScope.Name); // Should contain line number like ArrowFunction1_L2C17
            Assert.Contains("C", arrowScope.Name); // Should contain column number
        }
    }
}
