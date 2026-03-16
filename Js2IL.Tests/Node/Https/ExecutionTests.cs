using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Https
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Https") { }

        [Fact]
        public Task Tls_CreateServer_Connect_Basic()
            => ExecutionTest(nameof(Tls_CreateServer_Connect_Basic));

        [Fact]
        public Task Tls_CreateSecureContext_Server_Handshake()
            => ExecutionTest(nameof(Tls_CreateSecureContext_Server_Handshake));

        [Fact]
        public Task Tls_CreateServer_Unsupported_RequestCert_Fails_Clearly()
            => ExecutionTest(nameof(Tls_CreateServer_Unsupported_RequestCert_Fails_Clearly));

        [Fact]
        public Task Https_Get_Loopback_SelfSigned()
            => ExecutionTest(nameof(Https_Get_Loopback_SelfSigned));

        [Fact]
        public Task Https_Request_Post_Basic()
            => ExecutionTest(nameof(Https_Request_Post_Basic));
    }
}
