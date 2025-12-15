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
        public Task UnaryOperator_PlusPlusPostfix() => GenerateTest(nameof(UnaryOperator_PlusPlusPostfix));

        [Fact]
        public Task UnaryOperator_Typeof() => GenerateTest(nameof(UnaryOperator_Typeof));

        [Fact]
        public Task UnaryOperator_BitwiseNot() => GenerateTest(nameof(UnaryOperator_BitwiseNot));
    }
}
