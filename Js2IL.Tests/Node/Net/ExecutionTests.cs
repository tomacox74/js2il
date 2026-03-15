using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Net
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Net") { }

        [Fact]
        public Task Net_CreateServer_Connect_Basic()
            => ExecutionTest(nameof(Net_CreateServer_Connect_Basic));

        [Fact]
        public Task Net_Socket_Binary_Data_Defaults_To_Buffer()
            => ExecutionTest(nameof(Net_Socket_Binary_Data_Defaults_To_Buffer));

        [Fact]
        public Task Net_Socket_SetEncoding_Utf8()
            => ExecutionTest(nameof(Net_Socket_SetEncoding_Utf8));

        [Fact]
        public Task Net_Socket_Timeout_Allows_Graceful_End()
            => ExecutionTest(nameof(Net_Socket_Timeout_Allows_Graceful_End));

        [Fact]
        public Task Net_CreateServer_AllowHalfOpen_Delayed_Response()
            => ExecutionTest(nameof(Net_CreateServer_AllowHalfOpen_Delayed_Response));

        [Fact]
        public Task Net_Socket_KeepAlive_And_Unsupported_Options()
            => ExecutionTest(nameof(Net_Socket_KeepAlive_And_Unsupported_Options));
    }
}
