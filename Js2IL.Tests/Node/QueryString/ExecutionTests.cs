using System.Threading.Tasks;

namespace Js2IL.Tests.Node.QueryString
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/QueryString") { }

        [Fact]
        public Task Require_QueryString_Parse_And_Stringify()
            => ExecutionTest(nameof(Require_QueryString_Parse_And_Stringify));
    }
}
