using System.Threading.Tasks;

namespace Jroc.Tests.Iterator
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Iterator") { }

        [Fact]
        public Task Iterator_From_HelperChain() { var testName = nameof(Iterator_From_HelperChain); return GenerateTest(testName); }

        [Fact]
        public Task Iterator_Helper_Cleanup() { var testName = nameof(Iterator_Helper_Cleanup); return GenerateTest(testName); }

        [Fact]
        public Task Iterator_Helper_Next_Return() { var testName = nameof(Iterator_Helper_Next_Return); return GenerateTest(testName); }

        [Fact]
        public Task Iterator_Result_Object_Shape() { var testName = nameof(Iterator_Result_Object_Shape); return GenerateTest(testName); }
    }
}
