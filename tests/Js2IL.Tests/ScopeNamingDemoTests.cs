using Acornima;
using Acornima.Ast;
using Js2IL.SymbolTables;
using Js2IL.Services;
using Xunit;
using Xunit.Abstractions;

namespace Js2IL.Tests
{
    public class ScopeNamingDemoTests
    {
        private readonly JavaScriptParser _parser;
        private readonly SymbolTableBuilder _scopeBuilder;
        private readonly ITestOutputHelper _output;

        public ScopeNamingDemoTests(ITestOutputHelper output)
        {
            _parser = new JavaScriptParser();
            _scopeBuilder = new SymbolTableBuilder();
            _output = output;
        }

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

        [Fact]
        public void PrintScopeNames_DemonstrateNamingImprovement()
        {
            // Arrange
            var code = @"
                var assignedArrow = (x) => x * 2;
                
                setTimeout(() => {
                    console.log('timer callback');
                }, 1000);
                
                var assignedFunc = function(a, b) {
                    return a + b;
                };
                
                [1, 2, 3].map(function(item) {
                    return item * 2;
                });
            ";
            var ast = _parser.ParseJavaScript(code, "demo.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "demo.js");

            // Assert & Print
            _output.WriteLine("Generated scope names:");
            foreach (var scope in scopeTree.Root.Children)
            {
                _output.WriteLine($"- {scope.Name}");
            }
            
            // Just verify we have some scopes (adjust expected count based on actual behavior)
            Assert.True(scopeTree.Root.Children.Count > 0, "Should have at least one child scope");
        }
    }
}
