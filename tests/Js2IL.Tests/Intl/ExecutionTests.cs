namespace Js2IL.Tests.Intl
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Intl")
        {
        }

        [Fact]
        public Task Intl_NumberFormat_And_Segmenter_Basic() { var testName = nameof(Intl_NumberFormat_And_Segmenter_Basic); return ExecutionTest(testName); }
    }
}
