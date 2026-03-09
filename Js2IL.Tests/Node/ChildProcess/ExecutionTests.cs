using System.Threading.Tasks;

namespace Js2IL.Tests.Node.ChildProcess
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/ChildProcess") { }

        [Fact]
        public Task Require_ChildProcess_Spawn_Basic()
            => ExecutionTest(nameof(Require_ChildProcess_Spawn_Basic));

        [Fact]
        public Task Require_ChildProcess_Spawn_Ignore()
            => ExecutionTest(nameof(Require_ChildProcess_Spawn_Ignore));

        [Fact]
        public Task Require_ChildProcess_Exec_Callback()
            => ExecutionTest(nameof(Require_ChildProcess_Exec_Callback));

        [Fact]
        public Task Require_ChildProcess_ExecFile_NonZero()
            => ExecutionTest(nameof(Require_ChildProcess_ExecFile_NonZero));
    }
}
