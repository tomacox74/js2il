using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.UnaryOperator
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("UnaryOperator") { }

        [Fact]
        public Task UnaryOperator_MinusMinusPostfix() => GenerateTest(nameof(UnaryOperator_MinusMinusPostfix), assertOnIRPipelineFailure: true);

        [Fact]
        public Task UnaryOperator_MinusMinusPrefix() => GenerateTest(nameof(UnaryOperator_MinusMinusPrefix), assertOnIRPipelineFailure: true);

        [Fact]
        public Task UnaryOperator_PlusPlusPostfix() => GenerateTest(nameof(UnaryOperator_PlusPlusPostfix), assertOnIRPipelineFailure: true);

        [Fact]
        public Task UnaryOperator_PlusPlusPrefix() => GenerateTest(nameof(UnaryOperator_PlusPlusPrefix), assertOnIRPipelineFailure: true);

        [Fact]
        public Task UnaryOperator_PlusPlusMinusMinusCapturedFromNestedFunction() => GenerateTest(nameof(UnaryOperator_PlusPlusMinusMinusCapturedFromNestedFunction), assertOnIRPipelineFailure: true);

        [Fact]
        public Task UnaryOperator_Typeof() => GenerateTest(nameof(UnaryOperator_Typeof), assertOnIRPipelineFailure: true);

        [Fact]
        public Task UnaryOperator_BitwiseNot() => GenerateTest(nameof(UnaryOperator_BitwiseNot), assertOnIRPipelineFailure: true);

        [Fact]
        public Task UnaryOperator_LogicalNot() => GenerateTest(nameof(UnaryOperator_LogicalNot), assertOnIRPipelineFailure: true);
    }
}
