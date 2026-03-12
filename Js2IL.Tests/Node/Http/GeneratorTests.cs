using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Http
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Http") { }

        [Fact]
        public Task Http_CreateServer_Get_Loopback()
            => GenerateTest(nameof(Http_CreateServer_Get_Loopback));

        [Fact]
        public Task Http_Request_Post_Basic()
            => GenerateTest(nameof(Http_Request_Post_Basic));
    }
}
