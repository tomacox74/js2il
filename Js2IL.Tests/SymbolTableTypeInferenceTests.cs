using Acornima;
using Js2IL.SymbolTables;
using Js2IL.Services;
using System.Linq;
using Xunit;

namespace Js2IL.Tests;

public class SymbolTableTypeInferenceTests
{
        private readonly JavaScriptParser _parser = new();
        private readonly SymbolTableBuilder _scopeBuilder = new();

        [Theory]
        [InlineData(typeof(double), "42")]
        [InlineData(typeof(string), "'hello'")]  
        [InlineData(typeof(bool), "true")]
        [InlineData(null, "null")]
        [InlineData(null, "")]
        [InlineData(typeof(double), "1 + 2")]
        [InlineData(typeof(string), "'1' + '2'")]
        [InlineData(typeof(string), "'1' + 2")]
        [InlineData(typeof(string), "1 + '2'")]
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
        [InlineData(null, "'hello'", "testVar = 123")]  // conflicting assignment removes inferred type
        [InlineData(typeof(bool), "true", "testVar = false")]
        [InlineData(null, "null", "testVar = 'now a string'")] // conflicting assignment removes inferred type
        [InlineData(null, "", "testVar = 3.14")] // single assignment, but type is either number or undefined
        [InlineData(typeof(string), "", "testVar = 'first'; testVar = 'second'")] // multiple consistent assignments
        [InlineData(null, "", "testVar = 10; testVar = 'oops'; testVar = true")] // multiple conflicting assignments
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

        private SymbolTable BuildSymbolTable(string source)
        {
            var ast = _parser.ParseJavaScript(source, "test.js");
            var symbolTable = _scopeBuilder.Build(ast, "test.js");
            return symbolTable;
        }

}