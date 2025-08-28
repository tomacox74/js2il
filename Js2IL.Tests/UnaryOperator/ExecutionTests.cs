using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.UnaryOperator
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("UnaryOperator") { }

        [Fact]
        public Task UnaryOperator_PlusPlusPostfix() => ExecutionTest(nameof(UnaryOperator_PlusPlusPostfix));

        [Fact]
        public Task UnaryOperator_MinusMinusPostfix() => ExecutionTest(nameof(UnaryOperator_MinusMinusPostfix));

    [Fact]
    public Task UnaryOperator_Typeof() => ExecutionTest(nameof(UnaryOperator_Typeof));
    }
}
