using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Url
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Url") { }

        [Fact]
        public Task Require_Url_Parse_And_Base()
            => GenerateTest(nameof(Require_Url_Parse_And_Base));

        [Fact]
        public Task Require_Url_SearchParams_Mutate()
            => GenerateTest(nameof(Require_Url_SearchParams_Mutate));

        [Fact]
        public Task Require_Url_Invalid_With_Base_Throws()
            => GenerateTest(nameof(Require_Url_Invalid_With_Base_Throws));

        [Fact]
        public Task Require_Url_File_Helpers()
            => GenerateTest(nameof(Require_Url_File_Helpers));

        [Fact]
        public Task Global_Url_And_SearchParams()
            => GenerateTest(nameof(Global_Url_And_SearchParams));
    }
}
