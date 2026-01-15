using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.UnaryOperator
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("UnaryOperator") { }

        [Fact]
        public Task UnaryOperator_MinusMinusPostfix() => ExecutionTest(nameof(UnaryOperator_MinusMinusPostfix));

        [Fact]
        public Task UnaryOperator_MinusMinusPrefix() => ExecutionTest(nameof(UnaryOperator_MinusMinusPrefix));

        [Fact]
        public Task UnaryOperator_PlusPlusPostfix() => ExecutionTest(nameof(UnaryOperator_PlusPlusPostfix));

        [Fact]
        public Task UnaryOperator_PlusPlusPrefix() => ExecutionTest(nameof(UnaryOperator_PlusPlusPrefix));

        [Fact]
        public Task UnaryOperator_PlusPlusMinusMinusCapturedFromNestedFunction() => ExecutionTest(nameof(UnaryOperator_PlusPlusMinusMinusCapturedFromNestedFunction));

        [Fact]
        public Task UnaryOperator_Typeof() => ExecutionTest(nameof(UnaryOperator_Typeof));

        [Fact]
        public Task UnaryOperator_BitwiseNot() => ExecutionTest(nameof(UnaryOperator_BitwiseNot));

        [Fact]
        public Task UnaryOperator_LogicalNot() => ExecutionTest(nameof(UnaryOperator_LogicalNot));

        [Fact]
        public Task UnaryOperator_DoubleNot_NaNTruthiness() => ExecutionTest(nameof(UnaryOperator_DoubleNot_NaNTruthiness));
    }
}
