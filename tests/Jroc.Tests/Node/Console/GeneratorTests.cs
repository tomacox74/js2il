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

        [Fact]
        public Task Console_GlobalLog_DirectIntrinsic()
            => GenerateTest(nameof(Console_GlobalLog_DirectIntrinsic));

        [Fact]
        public Task Console_GlobalLog_LocalBindingShadowing()
            => GenerateTest(nameof(Console_GlobalLog_LocalBindingShadowing));

        [Fact]
        public Task Console_GlobalLog_GlobalPropertyReplacement()
            => GenerateTest(nameof(Console_GlobalLog_GlobalPropertyReplacement));

        [Fact]
        public Task Console_GlobalLog_GlobalThisAliasReplacement()
            => GenerateTest(nameof(Console_GlobalLog_GlobalThisAliasReplacement));

        [Fact]
        public Task Console_GlobalLog_ComputedGlobalPropertyReplacement()
            => GenerateTest(nameof(Console_GlobalLog_ComputedGlobalPropertyReplacement));

        [Fact]
        public Task Console_GlobalLog_DescriptorReplacement()
            => GenerateTest(nameof(Console_GlobalLog_DescriptorReplacement));

        [Fact]
        public Task Console_GlobalLog_Deletion()
            => GenerateTest(nameof(Console_GlobalLog_Deletion));
    }
}
