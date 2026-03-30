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

        // Join materialization - ?:
        [Fact]
        public Task ShapeCoverage_JoinMaterialization_TernaryFeedsArithmetic() { var testName = nameof(ShapeCoverage_JoinMaterialization_TernaryFeedsArithmetic); return GenerateTest(testName); }

        // Join materialization - &&
        [Fact]
        public Task ShapeCoverage_JoinMaterialization_LogicalAndFeedsArithmetic() { var testName = nameof(ShapeCoverage_JoinMaterialization_LogicalAndFeedsArithmetic); return GenerateTest(testName); }

        // Join materialization - ||
        [Fact]
        public Task ShapeCoverage_JoinMaterialization_LogicalOrFeedsArithmetic() { var testName = nameof(ShapeCoverage_JoinMaterialization_LogicalOrFeedsArithmetic); return GenerateTest(testName); }

        // Loop-carried variables
        [Fact]
        public Task ShapeCoverage_LoopCarried_UpdatedEveryIteration() { var testName = nameof(ShapeCoverage_LoopCarried_UpdatedEveryIteration); return GenerateTest(testName); }

        [Fact]
        public Task ShapeCoverage_LoopCarried_ConditionalUpdateInLoop() { var testName = nameof(ShapeCoverage_LoopCarried_ConditionalUpdateInLoop); return GenerateTest(testName); }

        // Mixed numeric representations
        [Fact]
        public Task ShapeCoverage_MixedNumeric_BoxedArithmetic() { var testName = nameof(ShapeCoverage_MixedNumeric_BoxedArithmetic); return GenerateTest(testName); }

        [Fact]
        public Task ShapeCoverage_MixedNumeric_RuntimeCoercion() { var testName = nameof(ShapeCoverage_MixedNumeric_RuntimeCoercion); return GenerateTest(testName); }

        // Combined shapes
        [Fact]
        public Task ShapeCoverage_Combined_TernaryInsideLoop() { var testName = nameof(ShapeCoverage_Combined_TernaryInsideLoop); return GenerateTest(testName); }
    }
}
