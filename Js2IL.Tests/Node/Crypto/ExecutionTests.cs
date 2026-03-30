using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Crypto
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Crypto") { }

        [Fact]
        public Task Require_Crypto_CreateHash_And_RandomBytes()
            => ExecutionTest(nameof(Require_Crypto_CreateHash_And_RandomBytes));

        [Fact]
        public Task Require_Crypto_CreateHmac()
            => ExecutionTest(nameof(Require_Crypto_CreateHmac));

        [Fact]
        public Task Require_Crypto_WebCrypto_GetRandomValues()
            => ExecutionTest(nameof(Require_Crypto_WebCrypto_GetRandomValues));

        [Fact]
        public Task Require_Crypto_WebCrypto_Subtle()
            => ExecutionTest(nameof(Require_Crypto_WebCrypto_Subtle));

        [Fact]
        public Task Require_Crypto_ErrorPaths()
            => ExecutionTest(nameof(Require_Crypto_ErrorPaths));
    }
}
