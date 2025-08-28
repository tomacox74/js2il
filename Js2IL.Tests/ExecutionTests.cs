using Js2IL.Services;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Js2IL.Tests
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("ExecutionTests")
        {
        }


        // Sorted test methods
    // moved to UnaryOperator.ExecutionTests

    // moved to UnaryOperator.ExecutionTests

        [Fact(Skip = "process/argv not yet supported")]
        public Task Environment_EnumerateProcessArgV() { var testName = nameof(Environment_EnumerateProcessArgV); return ExecutionTest(testName); }

    }
}
