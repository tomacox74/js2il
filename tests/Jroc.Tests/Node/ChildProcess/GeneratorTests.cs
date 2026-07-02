using System.Threading.Tasks;

namespace Jroc.Tests.Node.ChildProcess
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/ChildProcess") { }

        [Fact]
        public Task Require_ChildProcess_Spawn_Basic()
            => GenerateTest(nameof(Require_ChildProcess_Spawn_Basic));

        [Fact]
        public Task Require_ChildProcess_Spawn_Ignore()
            => GenerateTest(nameof(Require_ChildProcess_Spawn_Ignore));

        [Fact]
        public Task Require_ChildProcess_Exec_Callback()
            => GenerateTest(nameof(Require_ChildProcess_Exec_Callback));

        [Fact]
        public Task Require_ChildProcess_ExecFile_NonZero()
            => GenerateTest(nameof(Require_ChildProcess_ExecFile_NonZero));

        [Fact]
        public Task Require_ChildProcess_Fork_MessagePassing()
            => GenerateTest(
                nameof(Require_ChildProcess_Fork_MessagePassing),
                new[] { "Require_ChildProcess_Fork_MessagePassing_Child" });

        [Fact]
        public Task Require_ChildProcess_Fork_Kill_And_Env()
            => GenerateTest(
                nameof(Require_ChildProcess_Fork_Kill_And_Env),
                new[] { "Require_ChildProcess_Fork_Kill_And_Env_Child" });

        [Fact]
        public Task Require_ChildProcess_Fork_Silent()
            => GenerateTest(
                nameof(Require_ChildProcess_Fork_Silent),
                new[] { "Require_ChildProcess_Fork_Silent_Child" });

        [Fact]
        public Task Require_ChildProcess_Fork_Unsupported_Options()
            => GenerateTest(nameof(Require_ChildProcess_Fork_Unsupported_Options));

        [Fact]
        public Task Require_ChildProcess_ExecFileSync_Basic()
            => GenerateTest(nameof(Require_ChildProcess_ExecFileSync_Basic));

        [Fact]
        public Task Require_ChildProcess_ExecSync_Quoted()
            => GenerateTest(nameof(Require_ChildProcess_ExecSync_Quoted));
    }
}
