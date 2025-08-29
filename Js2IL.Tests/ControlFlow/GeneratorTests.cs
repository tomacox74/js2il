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
        public Task ControlFlow_While_CountDownFromFive() { var testName = "ControlFlow_While_CountDownFromFive"; return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_CountDownFromFive() { var testName = "ControlFlow_DoWhile_CountDownFromFive"; return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_GreaterThanOrEqual() { var testName = "ControlFlow_ForLoop_GreaterThanOrEqual"; return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_LessThanOrEqual() { var testName = "ControlFlow_ForLoop_LessThanOrEqual"; return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_If_LessThan() { var testName = nameof(ControlFlow_If_LessThan); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_If_BooleanLiteral() { var testName = nameof(ControlFlow_If_BooleanLiteral); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_If_NotFlag() { var testName = nameof(ControlFlow_If_NotFlag); return GenerateTest(testName); }

        // Pending feature: continue statement support
        [Fact]
        public Task ControlFlow_ForLoop_Continue_SkipEven() { var testName = nameof(ControlFlow_ForLoop_Continue_SkipEven); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_While_Continue_SkipEven() { var testName = nameof(ControlFlow_While_Continue_SkipEven); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_Continue_SkipEven() { var testName = nameof(ControlFlow_DoWhile_Continue_SkipEven); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_Break_AtThree() { var testName = nameof(ControlFlow_ForLoop_Break_AtThree); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_While_Break_AtThree() { var testName = nameof(ControlFlow_While_Break_AtThree); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_Break_AtThree() { var testName = nameof(ControlFlow_DoWhile_Break_AtThree); return GenerateTest(testName); }

    // Conditional operator (?:) – pending implementation
    [Fact(Skip = "Conditional (ternary) operator codegen not implemented yet")]
    public Task ControlFlow_Conditional_Ternary() { var testName = nameof(ControlFlow_Conditional_Ternary); return GenerateTest(testName); }
    }
}
