using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Zlib
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Zlib") { }

        [Fact]
        public Task Require_Zlib_GzipSync_GunzipSync_RoundTrip()
            => ExecutionTest(nameof(Require_Zlib_GzipSync_GunzipSync_RoundTrip));

        [Fact]
        public Task Require_Zlib_Stream_RoundTrip()
            => ExecutionTest(nameof(Require_Zlib_Stream_RoundTrip));

        [Fact]
        public Task Require_Zlib_ErrorPaths()
            => ExecutionTest(nameof(Require_Zlib_ErrorPaths));
    }
}
