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
    }
}
