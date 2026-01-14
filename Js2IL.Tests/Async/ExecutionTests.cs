namespace Js2IL.Tests.Async
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Async")
        {
        }

        [Fact]
        public Task Async_HelloWorld() { var testName = nameof(Async_HelloWorld); return ExecutionTest(testName); }        

        [Fact]
        public Task Async_RealSuspension_SetTimeout() { var testName = nameof(Async_RealSuspension_SetTimeout); return ExecutionTest(testName); }
    }
}