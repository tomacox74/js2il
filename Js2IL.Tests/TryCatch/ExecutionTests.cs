using System.Threading.Tasks;

namespace Js2IL.Tests.TryCatch
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("TryCatch")
        {
        }

        // Try/Catch execution tests
        [Fact]
        public Task TryCatch_NoBinding() { var testName = nameof(TryCatch_NoBinding); return ExecutionTest(testName); }
        [Fact]
        public Task TryCatch_NoBinding_NoThrow() { var testName = nameof(TryCatch_NoBinding_NoThrow); return ExecutionTest(testName); }
        // Try/Finally (no catch)
        [Fact]
        public Task TryFinally_NoCatch() { var testName = nameof(TryFinally_NoCatch); return ExecutionTest(testName); }

        // Try/Finally (no catch) with throw inside try: postpone unhandled-error semantics; allow crash by skipping
        [Fact(Skip = "Unhandled JS Error semantics postponed; allow crash for now")]
        public Task TryFinally_NoCatch_Throw() { var testName = nameof(TryFinally_NoCatch_Throw); return ExecutionTest(testName); }

        // New pipeline regression coverage
        [Fact]
        public Task TryFinally_Return() { var testName = nameof(TryFinally_Return); return ExecutionTest(testName); }

        [Fact]
        public Task TryCatch_ScopedParam() { var testName = nameof(TryCatch_ScopedParam); return ExecutionTest(testName); }

        [Fact]
        public Task TryCatchFinally_ThrowValue() { var testName = nameof(TryCatchFinally_ThrowValue); return ExecutionTest(testName); }

        [Fact]
        public Task TryCatch_NewExpression_BuiltInErrors() { var testName = nameof(TryCatch_NewExpression_BuiltInErrors); return ExecutionTest(testName); }

        [Fact]
        public Task TryCatch_CallMember_MissingMethod_IsTypeError() { var testName = nameof(TryCatch_CallMember_MissingMethod_IsTypeError); return ExecutionTest(testName); }
    }
}
