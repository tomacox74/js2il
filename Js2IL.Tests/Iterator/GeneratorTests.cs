using System.Threading.Tasks;

namespace Js2IL.Tests.Iterator
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Iterator") { }

        [Fact]
        public Task Iterator_From_HelperChain() { var testName = nameof(Iterator_From_HelperChain); return GenerateTest(testName); }

        [Fact]
        public Task Iterator_Helper_Next_Return() { var testName = nameof(Iterator_Helper_Next_Return); return GenerateTest(testName); }
    }
}
