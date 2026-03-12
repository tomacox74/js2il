using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Net
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Net") { }

        [Fact]
        public Task Net_CreateServer_Connect_Basic()
            => GenerateTest(nameof(Net_CreateServer_Connect_Basic));
    }
}
