using System.Threading.Tasks;

namespace Js2IL.Tests.Date
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Date") { }

        [Fact]
        public Task Date_Construct_FromMs_GetTime_ToISOString() { var testName = nameof(Date_Construct_FromMs_GetTime_ToISOString); return GenerateTest(testName, assertOnIRPipelineFailure: true); }

        [Fact]
        public Task Date_Parse_IsoString() { var testName = nameof(Date_Parse_IsoString); return GenerateTest(testName, assertOnIRPipelineFailure: true); }
    }
}
