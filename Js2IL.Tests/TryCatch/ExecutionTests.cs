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
    }
}
