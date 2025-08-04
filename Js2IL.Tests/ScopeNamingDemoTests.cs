using Acornima;
using Js2IL.Scoping;
using Js2IL.Services;
using Xunit;
using Xunit.Abstractions;

namespace Js2IL.Tests
{
    public class ScopeNamingDemoTests
    {
        private readonly JavaScriptParser _parser;
        private readonly ScopeTreeBuilder _scopeBuilder;
        private readonly ITestOutputHelper _output;

        public ScopeNamingDemoTests(ITestOutputHelper output)
        {
            _parser = new JavaScriptParser();
            _scopeBuilder = new ScopeTreeBuilder();
            _output = output;
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
            var ast = _parser.ParseJavaScript(code);

            // Act
            var scopeTree = _scopeBuilder.Build(ast, "demo.js");

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
