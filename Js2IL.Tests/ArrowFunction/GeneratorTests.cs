using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.ArrowFunction
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("ArrowFunction") { }

        [Fact]
    public Task Generate_ArrowFunction_SimpleExpression() { var testName = "ArrowFunction_SimpleExpression"; return GenerateTest(testName); }

        [Fact]
    public Task Generate_ArrowFunction_BlockBody_Return() { var testName = "ArrowFunction_BlockBody_Return"; return GenerateTest(testName); }

        [Fact]
    public Task Generate_ArrowFunction_CapturesOuterVariable() { var testName = "ArrowFunction_CapturesOuterVariable"; return GenerateTest(testName); }
    }
}
