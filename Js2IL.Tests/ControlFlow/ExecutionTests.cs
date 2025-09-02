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
        [Fact]
        public Task ControlFlow_ForLoop_CountDownFromFive() { var testName = nameof(ControlFlow_ForLoop_CountDownFromFive); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_CountToFive() { var testName = nameof(ControlFlow_ForLoop_CountToFive); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_GreaterThanOrEqual() { var testName = nameof(ControlFlow_ForLoop_GreaterThanOrEqual); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_LessThanOrEqual() { var testName = nameof(ControlFlow_ForLoop_LessThanOrEqual); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_If_LessThan() { var testName = nameof(ControlFlow_If_LessThan); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_If_BooleanLiteral() { var testName = nameof(ControlFlow_If_BooleanLiteral); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_If_NotFlag() { var testName = nameof(ControlFlow_If_NotFlag); return ExecutionTest(testName); }
        
    [Fact]
    public Task ControlFlow_If_Truthiness() { var testName = nameof(ControlFlow_If_Truthiness); return ExecutionTest(testName); }
            
        [Fact]
        public Task ControlFlow_While_CountDownFromFive() { var testName = nameof(ControlFlow_While_CountDownFromFive); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_CountDownFromFive() { var testName = nameof(ControlFlow_DoWhile_CountDownFromFive); return ExecutionTest(testName); }

        // Pending feature: continue statement support
        [Fact]
        public Task ControlFlow_ForLoop_Continue_SkipEven() { var testName = nameof(ControlFlow_ForLoop_Continue_SkipEven); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_While_Continue_SkipEven() { var testName = nameof(ControlFlow_While_Continue_SkipEven); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_DoWhile_Continue_SkipEven() { var testName = nameof(ControlFlow_DoWhile_Continue_SkipEven); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_ForLoop_Break_AtThree() { var testName = nameof(ControlFlow_ForLoop_Break_AtThree); return ExecutionTest(testName); }

        [Fact]
        public Task ControlFlow_While_Break_AtThree() { var testName = nameof(ControlFlow_While_Break_AtThree); return ExecutionTest(testName); }
        
        [Fact]
        public Task ControlFlow_DoWhile_Break_AtThree() { var testName = nameof(ControlFlow_DoWhile_Break_AtThree); return ExecutionTest(testName); }

    // Conditional operator (?:)
    [Fact]
    public Task ControlFlow_Conditional_Ternary() { var testName = nameof(ControlFlow_Conditional_Ternary); return ExecutionTest(testName); }

    [Fact]
    public Task ControlFlow_ForOf_Array_Basic() { var testName = nameof(ControlFlow_ForOf_Array_Basic); return ExecutionTest(testName); }
     }
}
