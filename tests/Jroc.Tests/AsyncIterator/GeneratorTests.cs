namespace Jroc.Tests.AsyncIterator
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("AsyncIterator") { }

        [Fact]
        public Task AsyncIterator_ExplicitReceiverAbi() { var testName = nameof(AsyncIterator_ExplicitReceiverAbi); return GenerateTest(testName); }
    }
}
