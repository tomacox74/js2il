namespace Jroc.Tests.Abort
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Abort") { }

        [Fact]
        public Task Abort_ExplicitReceiverAbi() { var testName = nameof(Abort_ExplicitReceiverAbi); return GenerateTest(testName); }
    }
}
