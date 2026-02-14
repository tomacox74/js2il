using Xunit;
using Js2IL.Validation;

namespace Js2IL.Tests.Import
{
    public class ValidationTests
    {
        [Fact]
        public void Import_NonLiteral_ShouldFail()
        {
            var js = @"
                const moduleName = './some-module';
                import(moduleName);
            ";

            var parser = new Services.JavaScriptParser();
            var ast = parser.ParseJavaScript(js, "test.js");

            var validator = new JavaScriptAstValidator();
            var result = validator.Validate(ast);

            Assert.False(result.IsValid, "Validation should fail for non-literal import() specifier");
            Assert.Contains(result.Errors, e => e.Contains("non-literal") && e.Contains("import"));
        }
    }
}
