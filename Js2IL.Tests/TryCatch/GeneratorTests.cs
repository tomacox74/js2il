using System.Threading.Tasks;

namespace Js2IL.Tests.TryCatch
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("TryCatch")
        {
        }

        // Add try/catch generator tests here
    [Fact]
    public Task TryCatch_NoBinding() { var testName = nameof(TryCatch_NoBinding); return GenerateTest(testName); }
    }
}
