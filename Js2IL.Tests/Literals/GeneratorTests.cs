using System.Threading.Tasks;

namespace Js2IL.Tests.Literals
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Literals") { }

        // Literal code generation tests
        [Fact] public Task ArrayLiteral() { var testName = nameof(ArrayLiteral); return GenerateTest(testName); }
        [Fact] public Task ObjectLiteral() { var testName = nameof(ObjectLiteral); return GenerateTest(testName); }
    [Fact(Skip = "Snapshot pending for BooleanLiteral generator")] public Task BooleanLiteral() { var testName = nameof(BooleanLiteral); return GenerateTest(testName); }
    }
}
