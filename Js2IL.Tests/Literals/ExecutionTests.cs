using System.Threading.Tasks;

namespace Js2IL.Tests.Literals
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Literals") { }

        // Literal evaluation tests
        [Fact] public Task ArrayLiteral() { var testName = nameof(ArrayLiteral); return ExecutionTest(testName); }
        [Fact] public Task ObjectLiteral() { var testName = nameof(ObjectLiteral); return ExecutionTest(testName); }
    }
}
