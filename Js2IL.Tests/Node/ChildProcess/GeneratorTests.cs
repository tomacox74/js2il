using System.Threading.Tasks;

namespace Js2IL.Tests.Node.ChildProcess
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
    }
}
