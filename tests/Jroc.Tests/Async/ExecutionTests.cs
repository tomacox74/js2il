namespace Jroc.Tests.Async
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
        public Task Async_TopLevelAwait_ResolvedPromise() { var testName = nameof(Async_TopLevelAwait_ResolvedPromise); return ExecutionTest(testName); }

        [Fact]
        public Task Async_TopLevelAwait_PendingPromise() { var testName = nameof(Async_TopLevelAwait_PendingPromise); return ExecutionTest(testName); }

        [Fact]
        public Task Async_TopLevelAwait_RejectionCaught() { var testName = nameof(Async_TopLevelAwait_RejectionCaught); return ExecutionTest(testName); }

        [Fact]
        public Task Async_TopLevelAwait_ForOfClosureCapture() { var testName = nameof(Async_TopLevelAwait_ForOfClosureCapture); return ExecutionTest(testName); }

        [Fact]
        public Task Async_TopLevelAwait_DefaultDestructuringFunctionExport() { var testName = nameof(Async_TopLevelAwait_DefaultDestructuringFunctionExport); return ExecutionTest(testName); }

        [Fact]
        public Task Async_TopLevelAwait_ImportedNamedFunction()
        {
            var testName = nameof(Async_TopLevelAwait_ImportedNamedFunction);
            return ExecutionTest(testName, additionalScripts: new[] { "Async_TopLevelAwait_ImportedNamedFunction_Dependency" });
        }

        [Fact]
        public Task Async_Calls_Module_Function_Before_Await() { var testName = nameof(Async_Calls_Module_Function_Before_Await); return ExecutionTest(testName); }

        [Fact]
        public Task Async_RealSuspension_SetTimeout() { var testName = nameof(Async_RealSuspension_SetTimeout); return ExecutionTest(testName); }

        [Fact]
        public Task Async_ArrowFunction_SimpleAwait() { var testName = nameof(Async_ArrowFunction_SimpleAwait); return ExecutionTest(testName); }

        [Fact]
        public Task Async_ArrowFunction_LexicalThis() { var testName = nameof(Async_ArrowFunction_LexicalThis); return ExecutionTest(testName); }

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

        [Fact]
        public Task Async_ClassMethod_SimpleAwait() { var testName = nameof(Async_ClassMethod_SimpleAwait); return ExecutionTest(testName); }

        [Fact]
        public Task Async_ClassMethod_WithThis() { var testName = nameof(Async_ClassMethod_WithThis); return ExecutionTest(testName); }

        [Fact]
        public Task Async_StaticMethod_SimpleAwait() { var testName = nameof(Async_StaticMethod_SimpleAwait); return ExecutionTest(testName); }

        [Fact]
        public Task Async_ClassMethod_MultipleAwaits() { var testName = nameof(Async_ClassMethod_MultipleAwaits); return ExecutionTest(testName); }

        [Fact]
        public Task Async_ClassMethod_CallsOtherAsync() { var testName = nameof(Async_ClassMethod_CallsOtherAsync); return ExecutionTest(testName); }

        [Fact]
        public Task Async_Inheritance_SuperAsyncMethod() { var testName = nameof(Async_Inheritance_SuperAsyncMethod); return ExecutionTest(testName); }

        [Fact]
        public Task Async_ForAwaitOf_Array() { var testName = nameof(Async_ForAwaitOf_Array); return ExecutionTest(testName); }

        [Fact]
        public Task Async_ForOf_AwaitInBody() { var testName = nameof(Async_ForOf_AwaitInBody); return ExecutionTest(testName); }

        [Fact]
        public Task Async_ForAwaitOf_AsyncIterator_BreakCloses() { var testName = nameof(Async_ForAwaitOf_AsyncIterator_BreakCloses); return ExecutionTest(testName); }

        [Fact]
        public Task Async_ForAwaitOf_SyncIteratorFallback_BreakCloses() { var testName = nameof(Async_ForAwaitOf_SyncIteratorFallback_BreakCloses); return ExecutionTest(testName); }
    }
}
