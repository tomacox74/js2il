using Js2IL.Services;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Js2IL.Tests.ControlFlow
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("ControlFlow")
        {
        }

        // Control Flow Tests
        // Conditional operator (?:)
        [Fact]
        public Task ControlFlow_Conditional_Ternary() { var testName = nameof(ControlFlow_Conditional_Ternary); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_Conditional_Ternary_ShortCircuit() { var testName = nameof(ControlFlow_Conditional_Ternary_ShortCircuit); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_Break_AtThree() { var testName = nameof(ControlFlow_DoWhile_Break_AtThree); return ExecutionTest(testName); }
        [Fact]
        public Task ControlFlow_DoWhile_Continue_SkipEven() { var testName = nameof(ControlFlow_DoWhile_Continue_SkipEven); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_CountDownFromFive() { var testName = nameof(ControlFlow_DoWhile_CountDownFromFive); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_NestedLet() { var testName = nameof(ControlFlow_DoWhile_NestedLet); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_CountUp_AtLeastOnce() { var testName = nameof(ControlFlow_DoWhile_CountUp_AtLeastOnce); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_Break_AtThree() { var testName = nameof(ControlFlow_ForLoop_Break_AtThree); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_Continue_SkipEven() { var testName = nameof(ControlFlow_ForLoop_Continue_SkipEven); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_CountDownFromFive() { var testName = nameof(ControlFlow_ForLoop_CountDownFromFive); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_CountToFive() { var testName = nameof(ControlFlow_ForLoop_CountToFive); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_GreaterThanOrEqual() { var testName = nameof(ControlFlow_ForLoop_GreaterThanOrEqual); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_LessThanOrEqual() { var testName = nameof(ControlFlow_ForLoop_LessThanOrEqual); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForOf_Array_Basic() { var testName = nameof(ControlFlow_ForOf_Array_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForOf_ClosureCallback() { var testName = nameof(ControlFlow_ForOf_ClosureCallback); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForIn_Object_Basic() { var testName = nameof(ControlFlow_ForIn_Object_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForOf_Break() { var testName = nameof(ControlFlow_ForOf_Break); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForOf_Continue() { var testName = nameof(ControlFlow_ForOf_Continue); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForIn_Break() { var testName = nameof(ControlFlow_ForIn_Break); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForIn_Continue() { var testName = nameof(ControlFlow_ForIn_Continue); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForOf_LabeledBreak() { var testName = nameof(ControlFlow_ForOf_LabeledBreak); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForOf_LabeledContinue() { var testName = nameof(ControlFlow_ForOf_LabeledContinue); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForIn_LabeledBreak() { var testName = nameof(ControlFlow_ForIn_LabeledBreak); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForIn_LabeledContinue() { var testName = nameof(ControlFlow_ForIn_LabeledContinue); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_While_LabeledBreak() { var testName = nameof(ControlFlow_While_LabeledBreak); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_While_LabeledContinue() { var testName = nameof(ControlFlow_While_LabeledContinue); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_LabeledBreak() { var testName = nameof(ControlFlow_ForLoop_LabeledBreak); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_LabeledContinue() { var testName = nameof(ControlFlow_ForLoop_LabeledContinue); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_LabeledBreak() { var testName = nameof(ControlFlow_DoWhile_LabeledBreak); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_LabeledContinue() { var testName = nameof(ControlFlow_DoWhile_LabeledContinue); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_If_BooleanLiteral() { var testName = nameof(ControlFlow_If_BooleanLiteral); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_If_LessThan() { var testName = nameof(ControlFlow_If_LessThan); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_If_NotEqual() { var testName = nameof(ControlFlow_If_NotEqual); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_If_NotFlag() { var testName = nameof(ControlFlow_If_NotFlag); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_EmptyStatement() { var testName = nameof(ControlFlow_EmptyStatement); return ExecutionTest(testName); }
        
        [Fact]
        public Task ControlFlow_If_Truthiness() { var testName = nameof(ControlFlow_If_Truthiness); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_While_Break_AtThree() { var testName = nameof(ControlFlow_While_Break_AtThree); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_While_Continue_SkipEven() { var testName = nameof(ControlFlow_While_Continue_SkipEven); return ExecutionTest(testName); }
            
        [Fact]
        public Task ControlFlow_While_CountDownFromFive() { var testName = nameof(ControlFlow_While_CountDownFromFive); return ExecutionTest(testName); }

        // Switch statements
        [Fact]
        public Task ControlFlow_Switch_LabeledBreak() { var testName = nameof(ControlFlow_Switch_LabeledBreak); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_Switch_Fallthrough() { var testName = nameof(ControlFlow_Switch_Fallthrough); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_Switch_DefaultInMiddle_Fallthrough() { var testName = nameof(ControlFlow_Switch_DefaultInMiddle_Fallthrough); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_Switch_MultiCaseSharedBody() { var testName = nameof(ControlFlow_Switch_MultiCaseSharedBody); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_Switch_NestedBreak() { var testName = nameof(ControlFlow_Switch_NestedBreak); return ExecutionTest(testName); }
     }
}
