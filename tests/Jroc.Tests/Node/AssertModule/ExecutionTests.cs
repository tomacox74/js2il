using System.Threading.Tasks;

namespace Jroc.Tests.Node.AssertModule
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/AssertModule") { }

        [Fact]
        public Task Require_Assert_Callable_And_Core_Methods()
            => ExecutionTest(nameof(Require_Assert_Callable_And_Core_Methods));

        [Fact]
        public Task Require_Assert_AssertionError_Metadata()
            => ExecutionTest(nameof(Require_Assert_AssertionError_Metadata));
    }
}
