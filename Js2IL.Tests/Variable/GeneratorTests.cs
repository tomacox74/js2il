using System.Threading.Tasks;

namespace Js2IL.Tests.Variable
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Variable") { }

        // Variable (const/let) generator tests scaffold
        [Fact] public Task Variable_ConstSimple() { var testName = nameof(Variable_ConstSimple); return GenerateTest(testName); }
        [Fact] public Task Variable_LetBlockScope() { var testName = nameof(Variable_LetBlockScope); return GenerateTest(testName); }
    [Fact(Skip = "try/catch + const reassignment runtime error not implemented yet")] public Task Variable_ConstReassignmentError() { var testName = nameof(Variable_ConstReassignmentError); return GenerateTest(testName); }
        [Fact] public Task Variable_LetShadowing() { var testName = nameof(Variable_LetShadowing); return GenerateTest(testName); }
        [Fact] public Task Variable_TemporalDeadZoneAccess() { var testName = nameof(Variable_TemporalDeadZoneAccess); return GenerateTest(testName); }
    }
}
