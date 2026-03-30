using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Https
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Https") { }

        [Fact]
        public Task Tls_CreateServer_Connect_Basic()
            => GenerateTest(nameof(Tls_CreateServer_Connect_Basic));

        [Fact]
        public Task Tls_CreateSecureContext_Server_Handshake()
            => GenerateTest(nameof(Tls_CreateSecureContext_Server_Handshake));

        [Fact]
        public Task Tls_CreateServer_Unsupported_RequestCert_Fails_Clearly()
            => GenerateTest(nameof(Tls_CreateServer_Unsupported_RequestCert_Fails_Clearly));

        [Fact]
        public Task Https_Get_Loopback_SelfSigned()
            => GenerateTest(nameof(Https_Get_Loopback_SelfSigned));

        [Fact]
        public Task Https_Request_Post_Basic()
            => GenerateTest(nameof(Https_Request_Post_Basic));
    }
}
