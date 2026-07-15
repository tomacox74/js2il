using System.Threading.Tasks;

namespace Jroc.Tests.Node.Console
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Console") { }

        [Fact]
        public Task Console_Undici_Transform_Table()
            => ExecutionTest(nameof(Console_Undici_Transform_Table));
    }
}
