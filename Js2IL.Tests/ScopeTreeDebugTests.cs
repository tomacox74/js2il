using Acornima;
using Js2IL.Scoping;
using Js2IL.Services;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Js2IL.Tests
{
    public class ScopeTreeDebugTests
    {
        private readonly ITestOutputHelper _output;
        private readonly JavaScriptParser _parser;
        private readonly ScopeTreeBuilder _scopeBuilder;

        public ScopeTreeDebugTests(ITestOutputHelper output)
        {
            _output = output;
            _parser = new JavaScriptParser();
            _scopeBuilder = new ScopeTreeBuilder();
        }

        [Fact]
        public void Debug_FunctionDeclaration_ShowStructure()
        {
            // Arrange
            var code = @"
                function myFunction(param1) {
                    var localVar = 10;
                }
            ";
            var ast = _parser.ParseJavaScript(code);

            // Act
            var scopeTree = _scopeBuilder.Build(ast, "test.js");

            // Debug output
            _output.WriteLine($"Root scope: {scopeTree.Root.Name}");
            _output.WriteLine($"Root bindings: {string.Join(", ", scopeTree.Root.Bindings.Keys)}");
            _output.WriteLine($"Root children count: {scopeTree.Root.Children.Count}");
            
            for (int i = 0; i < scopeTree.Root.Children.Count; i++)
            {
                var child = scopeTree.Root.Children[i];
                _output.WriteLine($"Child {i}: {child.Name} ({child.Kind})");
                _output.WriteLine($"Child {i} bindings: {string.Join(", ", child.Bindings.Keys)}");
                _output.WriteLine($"Child {i} children count: {child.Children.Count}");
                
                // Show grandchildren too
                for (int j = 0; j < child.Children.Count; j++)
                {
                    var grandchild = child.Children[j];
                    _output.WriteLine($"  Grandchild {j}: {grandchild.Name} ({grandchild.Kind})");
                    _output.WriteLine($"  Grandchild {j} bindings: {string.Join(", ", grandchild.Bindings.Keys)}");
                }
            }
        }

        [Fact]
        public void Debug_ArrowFunction_ShowStructure()
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

            // Debug output
            _output.WriteLine($"Root scope: {scopeTree.Root.Name}");
            _output.WriteLine($"Root bindings: {string.Join(", ", scopeTree.Root.Bindings.Keys)}");
            _output.WriteLine($"Root children count: {scopeTree.Root.Children.Count}");
            
            for (int i = 0; i < scopeTree.Root.Children.Count; i++)
            {
                var child = scopeTree.Root.Children[i];
                _output.WriteLine($"Child {i}: {child.Name} ({child.Kind})");
                _output.WriteLine($"Child {i} bindings: {string.Join(", ", child.Bindings.Keys)}");
            }
        }
    }
}
