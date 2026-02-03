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

        [Fact]
        public Task ArrowFunction_LexicalThis_ObjectLiteralProperty() { var testName = nameof(ArrowFunction_LexicalThis_ObjectLiteralProperty); return GenerateTest(testName); }

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

                var nestedArrowFunctions = createCounterClass
                    .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(t => t.Name.StartsWith("ArrowFunction_L", StringComparison.Ordinal))
                    .ToArray();

                Assert.True(nestedArrowFunctions.Length > 0, "Expected at least one nested ArrowFunction_L* type.");

                foreach (var nestedArrowFunction in nestedArrowFunctions)
                {
                    Assert.True(nestedArrowFunction.IsClass, $"Expected {nestedArrowFunction.Name} to be a class");

                    // Make sure there is no old Function.* type for this arrow function
                    var previousType = generatedAssembly.GetType($"Functions.{nestedArrowFunction.Name}", throwOnError: false);
                    Assert.Null(previousType);
                }
            });
        }

        [Fact]
        public Task ArrowFunction_MaxParameters_32() { var testName = nameof(ArrowFunction_MaxParameters_32); return GenerateTest(testName); }
    }
}
