namespace Jroc.Tests.Abort
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Abort") { }

        [Fact]
        public Task Abort_ExplicitReceiverAbi() { var testName = nameof(Abort_ExplicitReceiverAbi); return ExecutionTest(testName); }
    }
}
