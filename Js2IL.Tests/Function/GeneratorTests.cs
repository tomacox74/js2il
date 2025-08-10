using System.Threading.Tasks;

namespace Js2IL.Tests.Function
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Function")
        {
        }

        // Function Tests
        [Fact]
        public Task Function_HelloWorld()
        {
            var testName = nameof(Function_HelloWorld);
            // Ensure GenerateTest uses the correct input and expected output for HelloWorld
            // If GenerateTest is implemented in GeneratorTestsBase, verify its logic and the source files it uses.
            return GenerateTest(testName);
        }

    [Fact]
    public Task Function_GlobalFunctionCallsGlobalFunction() { var testName = nameof(Function_GlobalFunctionCallsGlobalFunction); return GenerateTest(testName); }

    [Fact]
    public Task Function_GlobalFunctionWithParameter() { var testName = nameof(Function_GlobalFunctionWithParameter); return GenerateTest(testName); }

    [Fact]
    public Task Function_ReturnsStaticValueAndLogs() { var testName = nameof(Function_ReturnsStaticValueAndLogs); return GenerateTest(testName); }

    [Fact]
    public Task Function_GlobalFunctionWithArrayIteration() { var testName = nameof(Function_GlobalFunctionWithArrayIteration); return GenerateTest(testName); }

    [Fact]
    public Task Function_GlobalFunctionLogsGlobalVariable() { var testName = nameof(Function_GlobalFunctionLogsGlobalVariable); return GenerateTest(testName); }

    [Fact]
    public Task Function_GlobalFunctionChangesGlobalVariableValue() { var testName = nameof(Function_GlobalFunctionChangesGlobalVariableValue); return GenerateTest(testName); }

    [Fact]
    public Task Function_GlobalFunctionDeclaresAndCallsNestedFunction() { var testName = nameof(Function_GlobalFunctionDeclaresAndCallsNestedFunction); return GenerateTest(testName); }

    [Fact]
    public Task Function_NestedFunctionAccessesMultipleScopes() { var testName = nameof(Function_NestedFunctionAccessesMultipleScopes); return GenerateTest(testName); }

    [Fact]
    public Task Function_NestedFunctionLogsOuterParameter() { var testName = nameof(Function_NestedFunctionLogsOuterParameter); return GenerateTest(testName); }
    }
}
