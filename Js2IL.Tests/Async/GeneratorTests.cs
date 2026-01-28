namespace Js2IL.Tests.Async
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Async") { }

        [Fact]
        public Task Async_HelloWorld() { var testName = nameof(Async_HelloWorld); return GenerateTest(testName); }

        [Fact]
        public Task Async_ReturnValue() { var testName = nameof(Async_ReturnValue); return GenerateTest(testName); }

        [Fact]
        public Task Async_SimpleAwait() { var testName = nameof(Async_SimpleAwait); return GenerateTest(testName); }

        [Fact]
        public Task Async_PendingPromiseAwait() { var testName = nameof(Async_PendingPromiseAwait); return GenerateTest(testName); }

        [Fact]
        public Task Async_RealSuspension_SetTimeout() { var testName = nameof(Async_RealSuspension_SetTimeout); return GenerateTest(testName); }

        [Fact]
        public Task Async_ArrowFunction_SimpleAwait() { var testName = nameof(Async_ArrowFunction_SimpleAwait); return GenerateTest(testName); }

        [Fact]
        public Task Async_FunctionExpression_SimpleAwait() { var testName = nameof(Async_FunctionExpression_SimpleAwait); return GenerateTest(testName); }

        [Fact]
        public Task Async_TryCatch_AwaitReject() { var testName = nameof(Async_TryCatch_AwaitReject); return GenerateTest(testName); }

        [Fact]
        public Task Async_TryFinally_AwaitInFinally_Normal() { var testName = nameof(Async_TryFinally_AwaitInFinally_Normal); return GenerateTest(testName); }

        [Fact]
        public Task Async_TryCatchFinally_AwaitInFinally_OnReject() { var testName = nameof(Async_TryCatchFinally_AwaitInFinally_OnReject); return GenerateTest(testName); }

        [Fact]
        public Task Async_TryFinally_PreservesExceptionThroughAwait() { var testName = nameof(Async_TryFinally_PreservesExceptionThroughAwait); return GenerateTest(testName); }

        [Fact]
        public Task Async_TryFinally_FinallyThrowOverridesOriginal() { var testName = nameof(Async_TryFinally_FinallyThrowOverridesOriginal); return GenerateTest(testName); }

        [Fact]
        public Task Async_TryFinally_ReturnPreservedThroughAwait() { var testName = nameof(Async_TryFinally_ReturnPreservedThroughAwait); return GenerateTest(testName); }

        [Fact]
        public Task Async_ClassMethod_SimpleAwait() { var testName = nameof(Async_ClassMethod_SimpleAwait); return GenerateTest(testName); }

        [Fact]
        public Task Async_ClassMethod_WithThis() { var testName = nameof(Async_ClassMethod_WithThis); return GenerateTest(testName); }

        [Fact]
        public Task Async_StaticMethod_SimpleAwait() { var testName = nameof(Async_StaticMethod_SimpleAwait); return GenerateTest(testName); }

        [Fact]
        public Task Async_ClassMethod_MultipleAwaits() { var testName = nameof(Async_ClassMethod_MultipleAwaits); return GenerateTest(testName); }

        [Fact]
        public Task Async_ClassMethod_CallsOtherAsync() { var testName = nameof(Async_ClassMethod_CallsOtherAsync); return GenerateTest(testName); }

        [Fact]
        public Task Async_Inheritance_SuperAsyncMethod() { var testName = nameof(Async_Inheritance_SuperAsyncMethod); return GenerateTest(testName); }

        [Fact]
        public Task Async_ForAwaitOf_Array() { var testName = nameof(Async_ForAwaitOf_Array); return GenerateTest(testName); }

        [Fact]
        public Task Async_ForAwaitOf_AsyncIterator_BreakCloses() { var testName = nameof(Async_ForAwaitOf_AsyncIterator_BreakCloses); return GenerateTest(testName); }

        [Fact]
        public Task Async_ForAwaitOf_SyncIteratorFallback_BreakCloses() { var testName = nameof(Async_ForAwaitOf_SyncIteratorFallback_BreakCloses); return GenerateTest(testName); }
    }
}