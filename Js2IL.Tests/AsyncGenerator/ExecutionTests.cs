namespace Js2IL.Tests.AsyncGenerator
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("AsyncGenerator")
        {
        }

        [Fact]
        public Task AsyncGenerator_BasicNext() { var testName = nameof(AsyncGenerator_BasicNext); return ExecutionTest(testName); }

        [Fact]
        public Task AsyncGenerator_ForAwaitOf() { var testName = nameof(AsyncGenerator_ForAwaitOf); return ExecutionTest(testName); }
    }
}
