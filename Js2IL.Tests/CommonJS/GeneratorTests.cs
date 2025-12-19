using System.Threading.Tasks;

namespace Js2IL.Tests.CommonJS
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("CommonJS")
        {
        }

        [Fact]
        public Task CommonJS_Require_Basic()
        {
            var testName = nameof(CommonJS_Require_Basic);
            return GenerateTest(testName, new[] { "CommonJS_Require_Dependency" });
        }
    }
}
