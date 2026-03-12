using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Http
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Http") { }

        [Fact]
        public Task Http_CreateServer_Get_Loopback()
            => ExecutionTest(nameof(Http_CreateServer_Get_Loopback));

        [Fact]
        public Task Http_Request_Post_Basic()
            => ExecutionTest(nameof(Http_Request_Post_Basic));
    }
}
