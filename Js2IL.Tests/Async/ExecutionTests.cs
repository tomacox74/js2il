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
        public Task Async_ReturnValue() { var testName = nameof(Async_ReturnValue); return ExecutionTest(testName); }

        [Fact]
        public Task Async_SimpleAwait() { var testName = nameof(Async_SimpleAwait); return ExecutionTest(testName); }

        [Fact(Skip = "Pending promise await not implemented yet")]
        public Task Async_PendingPromiseAwait() { var testName = nameof(Async_PendingPromiseAwait); return ExecutionTest(testName); }

        [Fact(Skip = "await not fully implemented yet")]
        public Task Async_RealSuspension_SetTimeout() { var testName = nameof(Async_RealSuspension_SetTimeout); return ExecutionTest(testName); }
    }
}