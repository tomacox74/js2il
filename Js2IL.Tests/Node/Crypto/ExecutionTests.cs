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
        public Task Require_Crypto_WebCrypto_GetRandomValues()
            => ExecutionTest(nameof(Require_Crypto_WebCrypto_GetRandomValues));
    }
}
