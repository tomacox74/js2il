namespace Jroc.Tests.Intl
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Intl")
        {
        }

        [Fact]
        public Task Intl_NumberFormat_And_Segmenter_Basic() { var testName = nameof(Intl_NumberFormat_And_Segmenter_Basic); return GenerateTest(testName); }
    }
}
