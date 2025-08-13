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
        [Fact]
        public Task UnaryOperator_MinusMinusPostfix() { var testName = nameof(UnaryOperator_MinusMinusPostfix); return ExecutionTest(testName); }

        [Fact]
        public Task UnaryOperator_PlusPlusPostfix() { var testName = nameof(UnaryOperator_PlusPlusPostfix); return ExecutionTest(testName); }        

        [Fact(Skip = "process/argv not yet supported")]
        public Task Environment_EnumerateProcessArgV() { var testName = nameof(Environment_EnumerateProcessArgV); return ExecutionTest(testName); }
    }
}
