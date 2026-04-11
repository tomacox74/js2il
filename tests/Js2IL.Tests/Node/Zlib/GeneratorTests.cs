using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Zlib
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Zlib") { }

        [Fact]
        public Task Require_Zlib_GzipSync_GunzipSync_RoundTrip()
            => GenerateTest(nameof(Require_Zlib_GzipSync_GunzipSync_RoundTrip));

        [Fact]
        public Task Require_Zlib_Stream_RoundTrip()
            => GenerateTest(nameof(Require_Zlib_Stream_RoundTrip));

        [Fact]
        public Task Require_Zlib_ErrorPaths()
            => GenerateTest(nameof(Require_Zlib_ErrorPaths));
    }
}
