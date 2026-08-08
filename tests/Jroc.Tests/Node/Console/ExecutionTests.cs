using System.Threading.Tasks;

namespace Jroc.Tests.Node.Console
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Console") { }

        [Fact]
        public Task Console_Undici_Transform_Table()
            => ExecutionTest(nameof(Console_Undici_Transform_Table));

        [Fact]
        public Task Require_NodeConsole_Log_Variadic()
            => ExecutionTest(nameof(Require_NodeConsole_Log_Variadic));

        [Fact]
        public Task Console_GlobalLog_DirectIntrinsic()
            => ExecutionTest(nameof(Console_GlobalLog_DirectIntrinsic));

        [Fact]
        public Task Console_GlobalLog_LocalBindingShadowing()
            => ExecutionTest(nameof(Console_GlobalLog_LocalBindingShadowing));

        [Fact]
        public Task Console_GlobalLog_GlobalPropertyReplacement()
            => ExecutionTest(nameof(Console_GlobalLog_GlobalPropertyReplacement));

        [Fact]
        public Task Console_GlobalLog_GlobalThisAliasReplacement()
            => ExecutionTest(nameof(Console_GlobalLog_GlobalThisAliasReplacement));

        [Fact]
        public Task Console_GlobalLog_ComputedGlobalPropertyReplacement()
            => ExecutionTest(nameof(Console_GlobalLog_ComputedGlobalPropertyReplacement));

        [Fact]
        public Task Console_GlobalLog_DescriptorReplacement()
            => ExecutionTest(nameof(Console_GlobalLog_DescriptorReplacement));

        [Fact]
        public Task Console_GlobalLog_Deletion()
            => ExecutionTest(nameof(Console_GlobalLog_Deletion));
    }
}
