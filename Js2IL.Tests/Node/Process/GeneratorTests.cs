using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Process
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node/Process") { }

        [Fact]
        public Task Environment_EnumerateProcessArgV() => GenerateTest(
            nameof(Environment_EnumerateProcessArgV));

        [Fact]
        public Task Process_Exit_NoArg_GeneratesCall() => GenerateTest(
            nameof(Process_Exit_NoArg_GeneratesCall));

        [Fact]
        public Task Process_Exit_WithCode_GeneratesCall() => GenerateTest(
            nameof(Process_Exit_WithCode_GeneratesCall));

        [Fact]
        public Task Process_Platform_Versions_And_Env_Basics() => GenerateTest(
            nameof(Process_Platform_Versions_And_Env_Basics));

        [Fact]
        public Task Process_Chdir_And_NextTick_Basics() => GenerateTest(
            nameof(Process_Chdir_And_NextTick_Basics));

        [Fact]
        public Task Process_Versions_Expanded() => GenerateTest(
            nameof(Process_Versions_Expanded));
    }
}
