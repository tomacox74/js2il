using Acornima;
using Acornima.Ast;
using Js2IL.SymbolTables;
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
        private readonly SymbolTableBuilder _scopeBuilder;

        public ScopeTreeDebugTests(ITestOutputHelper output)
        {
            _output = output;
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
        public void Debug_FunctionDeclaration_ShowStructure()
        {
            // Arrange
            var code = @"
                function myFunction(param1) {
                    var localVar = 10;
                }
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            if (ast is Acornima.Ast.Program p2 && p2.Body[0] is Acornima.Ast.FunctionDeclaration fd2)
            {
                _output.WriteLine($"Pre-Build param count: {fd2.Params.Count}");
            }
            else if (ast is Acornima.Ast.Program p3)
            {
                var first = p3.Body[0];
                _output.WriteLine($"First body node type: {first.GetType().FullName}");
                var props = first.GetType().GetProperties();
                foreach (var pr in props)
                {
                    try
                    {
                        var val = pr.GetValue(first);
                        string valDesc = val == null ? "null" : (val is System.Collections.IEnumerable e && val is not string ? $"Enumerable({string.Join(';', System.Linq.Enumerable.Cast<object>(e).Select(o=>o.GetType().Name))})" : val.GetType().Name);
                        _output.WriteLine($"Prop {pr.Name}: {valDesc}");
                    }
                    catch (Exception ex)
                    {
                        _output.WriteLine($"Prop {pr.Name}: <error {ex.Message}>");
                    }
                }
            }

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Inspect raw AST parameter node types
            if (ast is Acornima.Ast.Program prog && prog.Body[0] is Acornima.Ast.FunctionDeclaration fd)
            {
                foreach (var p in fd.Params)
                {
                    _output.WriteLine($"Param node type: {p.GetType().FullName}");
                }
            }

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
            var ast = _parser.ParseJavaScript(code, "test.js");

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

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
