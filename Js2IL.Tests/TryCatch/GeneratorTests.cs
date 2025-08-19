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

    // Try/Catch where no exception is thrown inside try
    [Fact]
    public Task TryCatch_NoBinding_NoThrow() { var testName = nameof(TryCatch_NoBinding_NoThrow); return GenerateTest(testName); }

    // Try/Finally (no catch) generator test
    [Fact]
    public Task TryFinally_NoCatch() { var testName = nameof(TryFinally_NoCatch); return GenerateTest(testName); }

    // Try/Finally (no catch) with throw inside try
    [Fact]
    public Task TryFinally_NoCatch_Throw() { var testName = nameof(TryFinally_NoCatch_Throw); return GenerateTest(testName); }
    }
}
