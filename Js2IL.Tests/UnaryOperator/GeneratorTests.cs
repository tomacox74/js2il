using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.UnaryOperator
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("UnaryOperator") { }

        [Fact]
        public Task UnaryOperator_MinusMinusPostfix() => GenerateTest(nameof(UnaryOperator_MinusMinusPostfix));

        [Fact]
        public Task UnaryOperator_MinusMinusPostfix_InFunctionSwitch() => GenerateTest(nameof(UnaryOperator_MinusMinusPostfix_InFunctionSwitch));

        [Fact]
        public Task UnaryOperator_MinusMinusPostfix_InFunctionSwitch_ObjectLocal() => GenerateTest(nameof(UnaryOperator_MinusMinusPostfix_InFunctionSwitch_ObjectLocal));

        [Fact]
        public Task UnaryOperator_MinusMinusPrefix() => GenerateTest(nameof(UnaryOperator_MinusMinusPrefix));

        [Fact]
        public Task UnaryOperator_MinusMinusPrefix_InFunctionSwitch() => GenerateTest(nameof(UnaryOperator_MinusMinusPrefix_InFunctionSwitch));

        [Fact]
        public Task UnaryOperator_MinusMinusPrefix_InFunctionSwitch_ObjectLocal() => GenerateTest(nameof(UnaryOperator_MinusMinusPrefix_InFunctionSwitch_ObjectLocal));

        [Fact]
        public Task UnaryOperator_PlusPlusPostfix() => GenerateTest(nameof(UnaryOperator_PlusPlusPostfix));

        [Fact]
        public Task UnaryOperator_PlusPlusPostfix_InFunctionSwitch() => GenerateTest(nameof(UnaryOperator_PlusPlusPostfix_InFunctionSwitch));

        [Fact]
        public Task UnaryOperator_PlusPlusPostfix_InFunctionSwitch_ObjectLocal() => GenerateTest(nameof(UnaryOperator_PlusPlusPostfix_InFunctionSwitch_ObjectLocal));

        [Fact]
        public Task UnaryOperator_PlusPlusPrefix() => GenerateTest(nameof(UnaryOperator_PlusPlusPrefix));

        [Fact]
        public Task UnaryOperator_PlusPlusPrefix_InFunctionSwitch() => GenerateTest(nameof(UnaryOperator_PlusPlusPrefix_InFunctionSwitch));

        [Fact]
        public Task UnaryOperator_PlusPlusPrefix_InFunctionSwitch_ObjectLocal() => GenerateTest(nameof(UnaryOperator_PlusPlusPrefix_InFunctionSwitch_ObjectLocal));

        [Fact]
        public Task UnaryOperator_PlusPlusMinusMinusCapturedFromNestedFunction() => GenerateTest(nameof(UnaryOperator_PlusPlusMinusMinusCapturedFromNestedFunction));

        [Fact]
        public Task UnaryOperator_PlusPlusMinusMinus_MemberAndIndexTargets() => GenerateTest(nameof(UnaryOperator_PlusPlusMinusMinus_MemberAndIndexTargets));

        [Fact]
        public Task UnaryOperator_Typeof() => GenerateTest(nameof(UnaryOperator_Typeof));

        [Fact]
        public Task UnaryOperator_BitwiseNot() => GenerateTest(nameof(UnaryOperator_BitwiseNot));

        [Fact]
        public Task UnaryOperator_LogicalNot() => GenerateTest(nameof(UnaryOperator_LogicalNot));

        [Fact]
        public Task UnaryOperator_DoubleNot_NaNTruthiness() => GenerateTest(nameof(UnaryOperator_DoubleNot_NaNTruthiness));

        [Fact]
        public Task UnaryOperator_VoidOperator() => GenerateTest(nameof(UnaryOperator_VoidOperator));

        [Fact]
        public Task UnaryOperator_UnaryNegation_CoercesToNumber() => GenerateTest(nameof(UnaryOperator_UnaryNegation_CoercesToNumber));
    }
}
