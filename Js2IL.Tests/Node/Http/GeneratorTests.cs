using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Http
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Http") { }

        [Fact]
        public Task Http_CreateServer_Get_Loopback()
            => GenerateTest(nameof(Http_CreateServer_Get_Loopback));

        [Fact]
        public Task Http_Request_Post_Basic()
            => GenerateTest(nameof(Http_Request_Post_Basic));

        [Fact]
        public Task Http_Request_Streaming_Chunked_RequestBody()
            => GenerateTest(nameof(Http_Request_Streaming_Chunked_RequestBody));

        [Fact]
        public Task Http_Response_Streaming_Chunked_ResponseBody()
            => GenerateTest(nameof(Http_Response_Streaming_Chunked_ResponseBody));

        [Fact]
        public Task Http_Agent_KeepAlive_Reuses_Connection()
            => GenerateTest(nameof(Http_Agent_KeepAlive_Reuses_Connection));

        [Fact]
        public Task Http_Request_Unsupported_Connect_Fails_Clearly()
            => GenerateTest(nameof(Http_Request_Unsupported_Connect_Fails_Clearly));
    }
}
