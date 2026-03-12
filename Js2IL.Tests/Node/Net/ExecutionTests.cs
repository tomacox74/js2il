using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Net
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Net") { }

        [Fact]
        public Task Net_CreateServer_Connect_Basic()
            => ExecutionTest(nameof(Net_CreateServer_Connect_Basic));
    }
}
