using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.ShapeCoverage
{
    /// <summary>
    /// Micro-tests that deliberately construct high-risk IR/control-flow shapes:
    /// join materialization, loop back-edges, and mixed numeric representations.
    /// These act as tripwires for compiler invariants around tempslot stores,
    /// numeric coercion paths, and control-flow join materialization.
    /// </summary>
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("ShapeCoverage")
        {
        }

        // Join materialization - ?:
        [Fact]
        public Task ShapeCoverage_JoinMaterialization_TernaryFeedsArithmetic() { var testName = nameof(ShapeCoverage_JoinMaterialization_TernaryFeedsArithmetic); return ExecutionTest(testName); }

        // Join materialization - &&
        [Fact]
        public Task ShapeCoverage_JoinMaterialization_LogicalAndFeedsArithmetic() { var testName = nameof(ShapeCoverage_JoinMaterialization_LogicalAndFeedsArithmetic); return ExecutionTest(testName); }

        // Join materialization - ||
        [Fact]
        public Task ShapeCoverage_JoinMaterialization_LogicalOrFeedsArithmetic() { var testName = nameof(ShapeCoverage_JoinMaterialization_LogicalOrFeedsArithmetic); return ExecutionTest(testName); }

        // Loop-carried variables
        [Fact]
        public Task ShapeCoverage_LoopCarried_UpdatedEveryIteration() { var testName = nameof(ShapeCoverage_LoopCarried_UpdatedEveryIteration); return ExecutionTest(testName); }

        [Fact]
        public Task ShapeCoverage_LoopCarried_ConditionalUpdateInLoop() { var testName = nameof(ShapeCoverage_LoopCarried_ConditionalUpdateInLoop); return ExecutionTest(testName); }

        // Mixed numeric representations
        [Fact]
        public Task ShapeCoverage_MixedNumeric_BoxedArithmetic() { var testName = nameof(ShapeCoverage_MixedNumeric_BoxedArithmetic); return ExecutionTest(testName); }

        [Fact]
        public Task ShapeCoverage_MixedNumeric_RuntimeCoercion() { var testName = nameof(ShapeCoverage_MixedNumeric_RuntimeCoercion); return ExecutionTest(testName); }

        // Combined shapes
        [Fact]
        public Task ShapeCoverage_Combined_TernaryInsideLoop() { var testName = nameof(ShapeCoverage_Combined_TernaryInsideLoop); return ExecutionTest(testName); }
    }
}
