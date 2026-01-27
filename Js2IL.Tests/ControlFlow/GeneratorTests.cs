using System.Threading.Tasks;

namespace Js2IL.Tests.ControlFlow
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("ControlFlow")
        {
        }

        [Fact]
        public Task ControlFlow_Conditional_Ternary() { var testName = nameof(ControlFlow_Conditional_Ternary); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_Conditional_Ternary_ShortCircuit() { var testName = nameof(ControlFlow_Conditional_Ternary_ShortCircuit); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_Break_AtThree() { var testName = nameof(ControlFlow_DoWhile_Break_AtThree); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_Continue_SkipEven() { var testName = nameof(ControlFlow_DoWhile_Continue_SkipEven); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_CountDownFromFive() { var testName = "ControlFlow_DoWhile_CountDownFromFive"; return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_NestedLet() { var testName = "ControlFlow_DoWhile_NestedLet"; return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_CountUp_AtLeastOnce() { var testName = nameof(ControlFlow_DoWhile_CountUp_AtLeastOnce); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_Break_AtThree() { var testName = nameof(ControlFlow_ForLoop_Break_AtThree); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_Continue_SkipEven() { var testName = nameof(ControlFlow_ForLoop_Continue_SkipEven); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_CountDownFromFive() { var testName = "ControlFlow_ForLoop_CountDownFromFive"; return GenerateTest(testName); }
        
        [Fact]
        public Task ControlFlow_ForLoop_CountToFive() { var testName = "ControlFlow_ForLoop_CountToFive"; return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_GreaterThanOrEqual() { var testName = "ControlFlow_ForLoop_GreaterThanOrEqual"; return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_LessThanOrEqual() { var testName = "ControlFlow_ForLoop_LessThanOrEqual"; return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_LetClosureCapture() { var testName = nameof(ControlFlow_ForLoop_LetClosureCapture); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_LetClosureCapture_Continue() { var testName = nameof(ControlFlow_ForLoop_LetClosureCapture_Continue); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_VarClosureCapture() { var testName = nameof(ControlFlow_ForLoop_VarClosureCapture); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForOf_Array_Basic() { var testName = nameof(ControlFlow_ForOf_Array_Basic); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForOf_ClosureCallback() { var testName = nameof(ControlFlow_ForOf_ClosureCallback); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForOf_Let_PerIterationBinding() { var testName = nameof(ControlFlow_ForOf_Let_PerIterationBinding); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForIn_Let_PerIterationBinding() { var testName = nameof(ControlFlow_ForIn_Let_PerIterationBinding); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForOf_Let_Destructuring_PerIterationBinding() { var testName = nameof(ControlFlow_ForOf_Let_Destructuring_PerIterationBinding); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForIn_Object_Basic() { var testName = nameof(ControlFlow_ForIn_Object_Basic); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForOf_Break() { var testName = nameof(ControlFlow_ForOf_Break); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForOf_Continue() { var testName = nameof(ControlFlow_ForOf_Continue); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForIn_Break() { var testName = nameof(ControlFlow_ForIn_Break); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForIn_Continue() { var testName = nameof(ControlFlow_ForIn_Continue); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForOf_LabeledBreak() { var testName = nameof(ControlFlow_ForOf_LabeledBreak); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForOf_LabeledContinue() { var testName = nameof(ControlFlow_ForOf_LabeledContinue); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForIn_LabeledBreak() { var testName = nameof(ControlFlow_ForIn_LabeledBreak); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForIn_LabeledContinue() { var testName = nameof(ControlFlow_ForIn_LabeledContinue); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_While_LabeledBreak() { var testName = nameof(ControlFlow_While_LabeledBreak); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_While_LabeledContinue() { var testName = nameof(ControlFlow_While_LabeledContinue); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_LabeledBreak() { var testName = nameof(ControlFlow_ForLoop_LabeledBreak); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_LabeledContinue() { var testName = nameof(ControlFlow_ForLoop_LabeledContinue); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_LabeledBreak() { var testName = nameof(ControlFlow_DoWhile_LabeledBreak); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_LabeledContinue() { var testName = nameof(ControlFlow_DoWhile_LabeledContinue); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_If_BooleanLiteral() { var testName = nameof(ControlFlow_If_BooleanLiteral); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_If_LessThan() { var testName = nameof(ControlFlow_If_LessThan); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_If_NotEqual() { var testName = nameof(ControlFlow_If_NotEqual); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_If_NotFlag() { var testName = nameof(ControlFlow_If_NotFlag); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_EmptyStatement() { var testName = nameof(ControlFlow_EmptyStatement); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_If_Truthiness() { var testName = nameof(ControlFlow_If_Truthiness); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_While_Break_AtThree() { var testName = nameof(ControlFlow_While_Break_AtThree); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_While_Continue_SkipEven() { var testName = nameof(ControlFlow_While_Continue_SkipEven); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_While_CountDownFromFive() { var testName = "ControlFlow_While_CountDownFromFive"; return GenerateTest(testName); }

        // Switch statements
        [Fact]
        public Task ControlFlow_Switch_LabeledBreak() { var testName = nameof(ControlFlow_Switch_LabeledBreak); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_Switch_Fallthrough() { var testName = nameof(ControlFlow_Switch_Fallthrough); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_Switch_DefaultInMiddle_Fallthrough() { var testName = nameof(ControlFlow_Switch_DefaultInMiddle_Fallthrough); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_Switch_MultiCaseSharedBody() { var testName = nameof(ControlFlow_Switch_MultiCaseSharedBody); return GenerateTest(testName); }

        [Fact]
        public Task ControlFlow_Switch_NestedBreak() { var testName = nameof(ControlFlow_Switch_NestedBreak); return GenerateTest(testName); }
    }
}
