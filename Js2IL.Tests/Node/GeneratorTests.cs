using System.Threading.Tasks;

namespace Js2IL.Tests.Node
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node") { }

        [Fact]
        public Task Require_Path_Join_Basic() => GenerateTest(nameof(Require_Path_Join_Basic));
    }
}
