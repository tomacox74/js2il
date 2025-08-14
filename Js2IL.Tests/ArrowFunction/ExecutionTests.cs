using System.Threading.Tasks;

namespace Js2IL.Tests.ArrowFunction
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("ArrowFunction") { }

        // Arrow Function tests (initial scaffolding)
    [Fact]
    public Task ArrowFunction_SimpleExpression() { var testName = nameof(ArrowFunction_SimpleExpression); return ExecutionTest(testName); }

    [Fact]
    public Task ArrowFunction_BlockBody_Return() { var testName = nameof(ArrowFunction_BlockBody_Return); return ExecutionTest(testName); }

    [Fact]
    public Task ArrowFunction_CapturesOuterVariable() { var testName = nameof(ArrowFunction_CapturesOuterVariable); return ExecutionTest(testName); }
    }
}
