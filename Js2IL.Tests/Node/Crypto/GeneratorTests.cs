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
        public Task Require_Crypto_CreateHmac()
            => GenerateTest(nameof(Require_Crypto_CreateHmac));

        [Fact]
        public Task Require_Crypto_Pbkdf2Sync()
            => GenerateTest(nameof(Require_Crypto_Pbkdf2Sync));

        [Fact]
        public Task Require_Crypto_WebCrypto_GetRandomValues()
            => GenerateTest(nameof(Require_Crypto_WebCrypto_GetRandomValues));

        [Fact]
        public Task Require_Crypto_WebCrypto_Subtle()
            => GenerateTest(nameof(Require_Crypto_WebCrypto_Subtle));

        [Fact]
        public Task Require_Crypto_ErrorPaths()
            => GenerateTest(nameof(Require_Crypto_ErrorPaths));
    }
}
