using Acornima;
using Acornima.Ast;
using Js2IL.SymbolTables;
using Js2IL.Services;
using Xunit;

namespace Js2IL.Tests
{
    public class ScopeNamingTests
    {
        private readonly JavaScriptParser _parser;
        private readonly SymbolTableBuilder _scopeBuilder;

        public ScopeNamingTests()
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
                Path = fileName,
                Name = Path.GetFileNameWithoutExtension(fileName)
            };

            _scopeBuilder.Build(module);

            return module.SymbolTable!;
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
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

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
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            var funcScope = scopeTree.Root.Children[0];
            Assert.Equal("FunctionExpression_callback", funcScope.Name);
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
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            var arrowScope = scopeTree.Root.Children[0];
            Assert.StartsWith("ArrowFunction", arrowScope.Name);
            Assert.Contains("_L", arrowScope.Name); // Should contain line number like ArrowFunction1_L2C17
            Assert.Contains("C", arrowScope.Name); // Should contain column number
        }
    }
}
