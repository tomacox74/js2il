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

        [Fact]
        public Task Async_PendingPromiseAwait() { var testName = nameof(Async_PendingPromiseAwait); return ExecutionTest(testName); }

        [Fact]
        public Task Async_RealSuspension_SetTimeout() { var testName = nameof(Async_RealSuspension_SetTimeout); return ExecutionTest(testName); }

        [Fact]
        public Task Async_ArrowFunction_SimpleAwait() { var testName = nameof(Async_ArrowFunction_SimpleAwait); return ExecutionTest(testName); }

        [Fact]
        public Task Async_FunctionExpression_SimpleAwait() { var testName = nameof(Async_FunctionExpression_SimpleAwait); return ExecutionTest(testName); }

        [Fact]
        public Task Async_TryCatch_AwaitReject() { var testName = nameof(Async_TryCatch_AwaitReject); return ExecutionTest(testName); }

        [Fact]
        public Task Async_TryFinally_AwaitInFinally_Normal() { var testName = nameof(Async_TryFinally_AwaitInFinally_Normal); return ExecutionTest(testName); }

        [Fact]
        public Task Async_TryCatchFinally_AwaitInFinally_OnReject() { var testName = nameof(Async_TryCatchFinally_AwaitInFinally_OnReject); return ExecutionTest(testName); }

        [Fact]
        public Task Async_TryFinally_PreservesExceptionThroughAwait() { var testName = nameof(Async_TryFinally_PreservesExceptionThroughAwait); return ExecutionTest(testName); }

        [Fact]
        public Task Async_TryFinally_FinallyThrowOverridesOriginal() { var testName = nameof(Async_TryFinally_FinallyThrowOverridesOriginal); return ExecutionTest(testName); }

        [Fact]
        public Task Async_TryFinally_ReturnPreservedThroughAwait() { var testName = nameof(Async_TryFinally_ReturnPreservedThroughAwait); return ExecutionTest(testName); }
    }
}