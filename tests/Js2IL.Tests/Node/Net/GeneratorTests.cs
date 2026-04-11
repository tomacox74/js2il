using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Net
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Net") { }

        [Fact]
        public Task Net_CreateServer_Connect_Basic()
            => GenerateTest(nameof(Net_CreateServer_Connect_Basic));

        [Fact]
        public Task Net_Socket_Binary_Data_Defaults_To_Buffer()
            => GenerateTest(nameof(Net_Socket_Binary_Data_Defaults_To_Buffer));

        [Fact]
        public Task Net_Socket_SetEncoding_Utf8()
            => GenerateTest(nameof(Net_Socket_SetEncoding_Utf8));

        [Fact]
        public Task Net_Socket_Timeout_Allows_Graceful_End()
            => GenerateTest(nameof(Net_Socket_Timeout_Allows_Graceful_End));

        [Fact]
        public Task Net_CreateServer_AllowHalfOpen_Delayed_Response()
            => GenerateTest(nameof(Net_CreateServer_AllowHalfOpen_Delayed_Response));

        [Fact]
        public Task Net_Socket_KeepAlive_And_Unsupported_Options()
            => GenerateTest(nameof(Net_Socket_KeepAlive_And_Unsupported_Options));
    }
}
