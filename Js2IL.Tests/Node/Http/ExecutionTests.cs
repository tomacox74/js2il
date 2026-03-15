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

        [Fact]
        public Task Http_Request_Streaming_Chunked_RequestBody()
            => ExecutionTest(nameof(Http_Request_Streaming_Chunked_RequestBody));

        [Fact]
        public Task Http_Response_Streaming_Chunked_ResponseBody()
            => ExecutionTest(nameof(Http_Response_Streaming_Chunked_ResponseBody));

        [Fact]
        public Task Http_Agent_KeepAlive_Reuses_Connection()
            => ExecutionTest(nameof(Http_Agent_KeepAlive_Reuses_Connection));

        [Fact]
        public Task Http_Request_Unsupported_Connect_Fails_Clearly()
            => ExecutionTest(nameof(Http_Request_Unsupported_Connect_Fails_Clearly));
    }
}
