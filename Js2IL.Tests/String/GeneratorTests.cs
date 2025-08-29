using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.String
{
    public class GeneratorTests : GeneratorTestsBase
    {
    public GeneratorTests() : base("String") {}
        [Fact]
        public Task String_Replace_Regex_Global()
        {
            var testName = nameof(String_Replace_Regex_Global);
            return GenerateTest(testName);
        }
    }
}
