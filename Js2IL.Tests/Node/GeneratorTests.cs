using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Js2IL.Tests.Node
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Node") { }

        [Fact]
        public Task Environment_EnumerateProcessArgV() => GenerateTest(
            nameof(Environment_EnumerateProcessArgV));

        [Fact]
        public Task FS_ReadWrite_Utf8() => GenerateTest(
            nameof(FS_ReadWrite_Utf8));

        [Fact]
        public Task Global___dirname_PrintsDirectory() => GenerateTest(
            nameof(Global___dirname_PrintsDirectory));

        [Fact]
        public Task PerfHooks_PerformanceNow_Basic() => GenerateTest(
            nameof(PerfHooks_PerformanceNow_Basic));

        [Fact]
        public Task ClearTimeout_MultipleZeroDelay_ClearSecondTimer() => GenerateTest(nameof(ClearTimeout_MultipleZeroDelay_ClearSecondTimer));

        [Fact]
        public Task ClearTimeout_ZeroDelay() => GenerateTest(nameof(ClearTimeout_ZeroDelay));

        [Fact]
        public Task SetInterval_ExecutesThreeTimes_ThenClears() => GenerateTest(nameof(SetInterval_ExecutesThreeTimes_ThenClears));

        [Fact]
        public Task Process_Exit_NoArg_GeneratesCall() => GenerateTest(
            nameof(Process_Exit_NoArg_GeneratesCall));

        [Fact]
        public Task Process_Exit_WithCode_GeneratesCall() => GenerateTest(
            nameof(Process_Exit_WithCode_GeneratesCall));

        [Fact]
        public Task Require_Path_Join_Basic() => GenerateTest(
            nameof(Require_Path_Join_Basic));

        [Fact]
        public Task Require_Path_Join_NestedFunction() => GenerateTest(
            nameof(Require_Path_Join_NestedFunction));

        [Fact]
        public Task SetTimeout_MultipleZeroDelay_ExecutedInOrder() => GenerateTest(nameof(SetTimeout_MultipleZeroDelay_ExecutedInOrder));

        [Fact]
        public Task SetTimeout_OneSecondDelay() => GenerateTest(nameof(SetTimeout_OneSecondDelay));

        [Fact]
        public Task SetTimeout_ZeroDelay() => GenerateTest(nameof(SetTimeout_ZeroDelay));

        [Fact]
        public Task SetTimeout_ZeroDelay_WithArgs() => GenerateTest(nameof(SetTimeout_ZeroDelay_WithArgs));

        [Fact]
        public Task SetImmediate_ExecutesCallback() => GenerateTest(nameof(SetImmediate_ExecutesCallback));

        [Fact]
        public Task SetImmediate_WithArgs_PassesCorrectly() => GenerateTest(nameof(SetImmediate_WithArgs_PassesCorrectly));

        [Fact]
        public Task SetImmediate_Multiple_ExecuteInOrder() => GenerateTest(nameof(SetImmediate_Multiple_ExecuteInOrder));

        [Fact]
        public Task ClearImmediate_CancelsCallback() => GenerateTest(nameof(ClearImmediate_CancelsCallback));

        [Fact]
        public Task SetImmediate_ExecutesBeforeSetTimeout() => GenerateTest(nameof(SetImmediate_ExecutesBeforeSetTimeout));

        [Fact]
        public Task SetImmediate_Nested_ExecutesInNextIteration() => GenerateTest(nameof(SetImmediate_Nested_ExecutesInNextIteration));
    }
}
