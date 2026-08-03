using System.Threading.Tasks;

namespace Jroc.Tests.Node.Console
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Console") { }

        [Fact]
        public Task Console_Undici_Transform_Table()
            => GenerateTest(nameof(Console_Undici_Transform_Table));

        [Fact]
        public Task Require_NodeConsole_Log_Variadic()
            => GenerateTest(nameof(Require_NodeConsole_Log_Variadic));
    }
}
