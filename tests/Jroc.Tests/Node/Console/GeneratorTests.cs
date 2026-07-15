using System.Threading.Tasks;

namespace Jroc.Tests.Node.Console
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Console") { }

        [Fact]
        public Task Console_Undici_Transform_Table()
            => GenerateTest(nameof(Console_Undici_Transform_Table));
    }
}
