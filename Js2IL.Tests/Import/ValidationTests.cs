using Xunit;
using Js2IL.Validation;

namespace Js2IL.Tests.Import
{
    public class ValidationTests
    {
        [Fact]
        public void Import_NonLiteral_ShouldPass()
        {
            var js = @"
                'use strict';
                const moduleName = './some-module';
                import(moduleName);
            ";

            var parser = new Services.JavaScriptParser();
            var ast = parser.ParseJavaScript(js, "test.js");

            var validator = new JavaScriptAstValidator();
            var result = validator.Validate(ast);

            Assert.True(result.IsValid, $"Validation should allow non-literal import() specifier. Errors: {string.Join(" | ", result.Errors)}");
            Assert.DoesNotContain(result.Errors, e => e.Contains("non-literal") && e.Contains("import"));
        }
    }
}
