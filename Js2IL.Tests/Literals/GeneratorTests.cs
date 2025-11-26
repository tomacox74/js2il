using System.Threading.Tasks;

namespace Js2IL.Tests.Literals
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Literals") { }

        // Literal code generation tests
        [Fact] public Task ArrayLiteral() { var testName = nameof(ArrayLiteral); return GenerateTest(testName); }
        [Fact] public Task Array_Spread_Copy() { var testName = nameof(Array_Spread_Copy); return GenerateTest(testName); }
        [Fact] public Task BooleanLiteral() { var testName = nameof(BooleanLiteral); return GenerateTest(testName); }
        [Fact] public Task ObjectLiteral() { var testName = nameof(ObjectLiteral); return GenerateTest(testName); }
        // Now supported: generate IL for numeric-key object literal
        [Fact] public Task ObjectLiteral_NumericKey() { var testName = nameof(ObjectLiteral_NumericKey); return GenerateTest(testName); }

        // Repro for property assignment on object literal (expected to fail until supported)
        [Fact] public Task ObjectLiteral_PropertyAssign() { var testName = nameof(ObjectLiteral_PropertyAssign); return GenerateTest(testName); }

    // No helper needed; GenerateTest loads embedded JS by convention
    }
}
