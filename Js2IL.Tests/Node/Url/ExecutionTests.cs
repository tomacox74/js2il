using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Url
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Url") { }

        [Fact]
        public Task Require_Url_Parse_And_Base()
            => ExecutionTest(nameof(Require_Url_Parse_And_Base));

        [Fact]
        public Task Require_Url_SearchParams_Mutate()
            => ExecutionTest(nameof(Require_Url_SearchParams_Mutate));

        [Fact]
        public Task Require_Url_Invalid_With_Base_Throws()
            => ExecutionTest(nameof(Require_Url_Invalid_With_Base_Throws));

        [Fact]
        public Task Require_Url_File_Helpers()
            => ExecutionTest(nameof(Require_Url_File_Helpers));
    }
}
