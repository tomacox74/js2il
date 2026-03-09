using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Crypto
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Crypto") { }

        [Fact]
        public Task Require_Crypto_CreateHash_And_RandomBytes()
            => GenerateTest(nameof(Require_Crypto_CreateHash_And_RandomBytes));

        [Fact]
        public Task Require_Crypto_WebCrypto_GetRandomValues()
            => GenerateTest(nameof(Require_Crypto_WebCrypto_GetRandomValues));
    }
}
