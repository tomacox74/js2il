using System.Threading.Tasks;

namespace Js2IL.Tests.Literals
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Literals") { }

        // Literal evaluation tests
        [Fact] public Task ArrayLiteral() { var testName = nameof(ArrayLiteral); return ExecutionTest(testName); }
        [Fact] public Task ObjectLiteral() { var testName = nameof(ObjectLiteral); return ExecutionTest(testName); }
        // Execution should now succeed and print value via console.log
        [Fact] public Task ObjectLiteral_NumericKey() { var testName = nameof(ObjectLiteral_NumericKey); return ExecutionTest(testName); }
        [Fact] public Task BooleanLiteral() { var testName = nameof(BooleanLiteral); return ExecutionTest(testName); }
        [Fact] public Task Literals_NullAndUndefined() { var testName = nameof(Literals_NullAndUndefined); return ExecutionTest(testName); }
        [Fact] public Task Array_Spread_Copy() { var testName = nameof(Array_Spread_Copy); return ExecutionTest(testName); }
    }
}
