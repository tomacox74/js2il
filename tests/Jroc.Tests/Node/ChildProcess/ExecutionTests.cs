using System.Threading.Tasks;

namespace Jroc.Tests.Node.ChildProcess
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

        [Fact(Skip = "Temporarily skipped due to CI flakiness. Tracked by #1517.")]
        public Task Require_ChildProcess_Fork_MessagePassing()
            => ExecutionTest(
                nameof(Require_ChildProcess_Fork_MessagePassing),
                additionalScripts: new[] { "Require_ChildProcess_Fork_MessagePassing_Child" });

        [Fact]
        public Task Require_ChildProcess_Fork_Kill_And_Env()
            => ExecutionTest(
                nameof(Require_ChildProcess_Fork_Kill_And_Env),
                additionalScripts: new[] { "Require_ChildProcess_Fork_Kill_And_Env_Child" });

        [Fact(Skip = "Temporarily skipped due to CI flakiness. Tracked by #1344.")]
        public Task Require_ChildProcess_Fork_Silent()
            => ExecutionTest(
                nameof(Require_ChildProcess_Fork_Silent),
                additionalScripts: new[] { "Require_ChildProcess_Fork_Silent_Child" });

        [Fact]
        public Task Require_ChildProcess_Fork_Unsupported_Options()
            => ExecutionTest(nameof(Require_ChildProcess_Fork_Unsupported_Options));

        [Fact]
        public Task Require_ChildProcess_ExecFileSync_Basic()
            => ExecutionTest(nameof(Require_ChildProcess_ExecFileSync_Basic));

        [Fact]
        public Task Require_ChildProcess_ExecSync_Quoted()
            => ExecutionTest(nameof(Require_ChildProcess_ExecSync_Quoted));
    }
}
