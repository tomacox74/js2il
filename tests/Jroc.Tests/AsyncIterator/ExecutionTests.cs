namespace Jroc.Tests.AsyncIterator
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("AsyncIterator") { }

        [Fact]
        public Task AsyncIterator_ExplicitReceiverAbi() { var testName = nameof(AsyncIterator_ExplicitReceiverAbi); return ExecutionTest(testName); }
    }
}
