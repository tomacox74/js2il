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
                Name = Path.GetFileNameWithoutExtension(fileName),
                ModuleId = Path.GetFileNameWithoutExtension(fileName)
            };

            _scopeBuilder.Build(module);

            return module.SymbolTable!;
        }

        [Fact]
        public void Build_ArrowFunctionWithAssignment_UsesLocationBasedName()
        {
            // Arrange
            var code = @"
                var func = () => {
                    console.log('test');
                };
            ";
            var ast = _parser.ParseJavaScript(code, "test.js");

            var varDecl = (VariableDeclaration)ast.Body[0];
            var declarator = (VariableDeclarator)varDecl.Declarations[0];
            var arrowExpr = (ArrowFunctionExpression)declarator.Init!;
            var expectedName = $"ArrowFunction_L{arrowExpr.Location.Start.Line}C{arrowExpr.Location.Start.Column + 1}";

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            var arrowScope = scopeTree.Root.Children[0];
            Assert.Equal(expectedName, arrowScope.Name);
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

            var exprStmt = (ExpressionStatement)ast.Body[0];
            var call = (CallExpression)exprStmt.Expression;
            var arrowExpr = (ArrowFunctionExpression)call.Arguments[0];
            var expectedName = $"ArrowFunction_L{arrowExpr.Location.Start.Line}C{arrowExpr.Location.Start.Column + 1}";

            // Act
            var scopeTree = BuildSymbolTable(ast, "test.js");

            // Assert
            var arrowScope = scopeTree.Root.Children[0];
            Assert.Equal(expectedName, arrowScope.Name);
        }
    }
}
