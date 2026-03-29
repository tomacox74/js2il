using System.Threading.Tasks;

namespace Js2IL.Tests.Iterator
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Iterator") { }

        [Fact]
        public Task Iterator_From_HelperChain() { var testName = nameof(Iterator_From_HelperChain); return ExecutionTest(testName); }

        [Fact]
        public Task Iterator_Helper_Next_Return() { var testName = nameof(Iterator_Helper_Next_Return); return ExecutionTest(testName); }
    }
}
