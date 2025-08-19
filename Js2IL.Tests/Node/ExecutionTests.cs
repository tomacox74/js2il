using System.Threading.Tasks;

namespace Js2IL.Tests.Node
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node") { }

        [Fact]
        public Task Require_Path_Join_Basic()
            => ExecutionTest(nameof(Require_Path_Join_Basic));
    }
}
