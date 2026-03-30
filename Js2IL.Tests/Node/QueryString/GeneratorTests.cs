using System.Threading.Tasks;

namespace Js2IL.Tests.Node.QueryString
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/QueryString") { }

        [Fact]
        public Task Require_QueryString_Parse_And_Stringify()
            => GenerateTest(nameof(Require_QueryString_Parse_And_Stringify));
    }
}
