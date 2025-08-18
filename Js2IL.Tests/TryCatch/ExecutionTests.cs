using System.Threading.Tasks;

namespace Js2IL.Tests.TryCatch
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("TryCatch")
        {
        }

    // Try/Catch execution tests
    [Fact(Skip = "try/catch not yet implemented")]
    public Task TryCatch_NoBinding() { var testName = nameof(TryCatch_NoBinding); return ExecutionTest(testName); }
    }
}
