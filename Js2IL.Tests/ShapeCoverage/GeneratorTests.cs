using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.ShapeCoverage
{
    /// <summary>
    /// Generator (IL snapshot) tests for the shape-coverage micro-test suite.
    /// </summary>
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("ShapeCoverage")
        {
        }

        private static void ScrubILSpyVolatileMethodHeader(StringBuilder sb)
        {
            // ILSpy includes non-deterministic disassembly metadata like RVA and code size.
            // Scrub it so generator snapshots are stable across builds and platforms.
            var text = sb.ToString();
            var normalized = string.Join("\n",
                text.Replace("\r\n", "\n")
                    .Split('\n')
                    .Where(line =>
                    {
                        var trimmed = line.TrimStart();
                        return !trimmed.StartsWith("// Method begins at RVA ", StringComparison.Ordinal)
                            && !trimmed.StartsWith("// Code size: ", StringComparison.Ordinal);
                    }));

            sb.Clear();
            sb.Append(normalized);
        }

        private Task GenerateStableIL(string testName) => GenerateTest(testName, settings =>
        {
            settings.AddScrubber(ScrubILSpyVolatileMethodHeader);
        });

        // Join materialization - ?:
        [Fact]
        public Task ShapeCoverage_JoinMaterialization_TernaryFeedsArithmetic() => GenerateStableIL(nameof(ShapeCoverage_JoinMaterialization_TernaryFeedsArithmetic));

        // Join materialization - &&
        [Fact]
        public Task ShapeCoverage_JoinMaterialization_LogicalAndFeedsArithmetic() => GenerateStableIL(nameof(ShapeCoverage_JoinMaterialization_LogicalAndFeedsArithmetic));

        // Join materialization - ||
        [Fact]
        public Task ShapeCoverage_JoinMaterialization_LogicalOrFeedsArithmetic() => GenerateStableIL(nameof(ShapeCoverage_JoinMaterialization_LogicalOrFeedsArithmetic));

        // Loop-carried variables
        [Fact]
        public Task ShapeCoverage_LoopCarried_UpdatedEveryIteration() => GenerateStableIL(nameof(ShapeCoverage_LoopCarried_UpdatedEveryIteration));

        [Fact]
        public Task ShapeCoverage_LoopCarried_ConditionalUpdateInLoop() => GenerateStableIL(nameof(ShapeCoverage_LoopCarried_ConditionalUpdateInLoop));

        // Mixed numeric representations
        [Fact]
        public Task ShapeCoverage_MixedNumeric_BoxedArithmetic() => GenerateStableIL(nameof(ShapeCoverage_MixedNumeric_BoxedArithmetic));

        [Fact]
        public Task ShapeCoverage_MixedNumeric_RuntimeCoercion() => GenerateStableIL(nameof(ShapeCoverage_MixedNumeric_RuntimeCoercion));

        // Combined shapes
        [Fact]
        public Task ShapeCoverage_Combined_TernaryInsideLoop() => GenerateStableIL(nameof(ShapeCoverage_Combined_TernaryInsideLoop));
    }
}
