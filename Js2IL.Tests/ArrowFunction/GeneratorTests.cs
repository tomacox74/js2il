using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.ArrowFunction
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("ArrowFunction") { }

        [Fact]
        public Task ArrowFunction_BlockBody_Return() { var testName = nameof(ArrowFunction_BlockBody_Return); return GenerateTest(testName); }

        [Fact]
        public Task ArrowFunction_CapturesOuterVariable() { var testName = nameof(ArrowFunction_CapturesOuterVariable); return GenerateTest(testName); }

        [Fact]
        public Task ArrowFunction_DefaultParameterExpression() { var testName = nameof(ArrowFunction_DefaultParameterExpression); return GenerateTest(testName); }

        [Fact]
        public Task ArrowFunction_DefaultParameterValue() { var testName = nameof(ArrowFunction_DefaultParameterValue); return GenerateTest(testName); }

        [Fact]
        public Task ArrowFunction_GlobalFunctionCallsGlobalFunction() { var testName = nameof(ArrowFunction_GlobalFunctionCallsGlobalFunction); return GenerateTest(testName); }

        [Fact]
        public Task ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal() { var testName = nameof(ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal); return GenerateTest(testName); }

        [Fact]
        public Task ArrowFunction_GlobalFunctionWithMultipleParameters() { var testName = nameof(ArrowFunction_GlobalFunctionWithMultipleParameters); return GenerateTest(testName); }

        [Fact]
        public Task ArrowFunction_NestedFunctionAccessesMultipleScopes() { var testName = nameof(ArrowFunction_NestedFunctionAccessesMultipleScopes); return GenerateTest(testName); }

        [Fact]
        public Task ArrowFunction_LexicalThis_ConstructorAssigned() { var testName = nameof(ArrowFunction_LexicalThis_ConstructorAssigned); return GenerateTest(testName); }

        [Fact]
        public Task ArrowFunction_LexicalThis_CreatedInMethod() { var testName = nameof(ArrowFunction_LexicalThis_CreatedInMethod); return GenerateTest(testName); }

        // New: parameter destructuring (object)
        [Fact]
        public Task ArrowFunction_ParameterDestructuring_Object() { var testName = nameof(ArrowFunction_ParameterDestructuring_Object); return GenerateTest(testName); }

        [Fact]
        public Task ArrowFunction_SimpleExpression() { var testName = nameof(ArrowFunction_SimpleExpression); return GenerateTest(testName); }

        [Fact]
        public Task ArrowFunction_ClosureMutatesOuterVariable()
        {
            var testName = nameof(ArrowFunction_ClosureMutatesOuterVariable);

            return GenerateTest(testName, verifyAssembly: (generatedAssembly) => {
                var globalScript = generatedAssembly.GetType("Modules.ArrowFunction_ClosureMutatesOuterVariable", throwOnError: true)!;
                Assert.True(globalScript.IsClass, "Expected globalScript to be a class.");
                
                var globalScope = globalScript.GetNestedType("Scope", BindingFlags.Public | BindingFlags.NonPublic)!;
                Assert.True(globalScope.IsClass, "Expected globalScope to be a class");

                var createCounterClass = globalScript.GetNestedType("createCounter", BindingFlags.Public | BindingFlags.NonPublic)!;
                Assert.True(createCounterClass.IsClass, "Expected createCounter to be a class");

                var createCounterScope = createCounterClass.GetNestedType("Scope", BindingFlags.Public | BindingFlags.NonPublic)!;
                Assert.True(createCounterScope.IsClass);

                var nestedArrowFunction = createCounterClass.GetNestedType("ArrowFunction_L7C23", BindingFlags.Public | BindingFlags.NonPublic)!;
                Assert.True(nestedArrowFunction.IsClass, "Expected ArrowFunction_L7C23 to be a class");

                // make sure there is no old Function.* types
                var previousType = generatedAssembly.GetType("Functions.ArrowFunction_L7C23", throwOnError: false);
                Assert.Null(previousType);
            });
        }
    }
}
