using System.Threading.Tasks;

namespace Js2IL.Tests.ControlFlow
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("ControlFlow")
        {
        }

        // Control Flow Tests
        [Fact]
        public Task ControlFlow_ForLoop_CountToFive() { var testName = "ControlFlow_ForLoop_CountToFive"; return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_CountDownFromFive() { var testName = "ControlFlow_ForLoop_CountDownFromFive"; return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_GreaterThanOrEqual() { var testName = "ControlFlow_ForLoop_GreaterThanOrEqual"; return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_LessThanOrEqual() { var testName = "ControlFlow_ForLoop_LessThanOrEqual"; return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_If_LessThan() { var testName = nameof(ControlFlow_If_LessThan); return GenerateTest(testName); }

    [Fact(Skip = "Snapshot pending for ControlFlow_If_BooleanLiteral generator")]
    public Task ControlFlow_If_BooleanLiteral() { var testName = nameof(ControlFlow_If_BooleanLiteral); return GenerateTest(testName); }

    [Fact]
    public Task ControlFlow_If_NotFlag() { var testName = nameof(ControlFlow_If_NotFlag); return GenerateTest(testName); }
    }
}
