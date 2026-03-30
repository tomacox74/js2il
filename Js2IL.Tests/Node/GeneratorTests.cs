using System.Threading.Tasks;

namespace Js2IL.Tests.Node
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node") { }

        [Fact]
        public Task Global___dirname_PrintsDirectory() => GenerateTest(
            nameof(Global___dirname_PrintsDirectory));

        [Fact]
        public Task PerfHooks_PerformanceNow_Basic() => GenerateTest(
            nameof(PerfHooks_PerformanceNow_Basic));

    }
}
