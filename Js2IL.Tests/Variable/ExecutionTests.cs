using System.Threading.Tasks;

namespace Js2IL.Tests.Variable
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Variable") { }

        // Variable (const/let) tests scaffold
        [Fact] public Task Variable_ConstSimple() { var testName = nameof(Variable_ConstSimple); return ExecutionTest(testName); }
        [Fact] public Task Variable_LetBlockScope() { var testName = nameof(Variable_LetBlockScope); return ExecutionTest(testName); }
    [Fact] public Task Variable_ConstReassignmentError() { var testName = nameof(Variable_ConstReassignmentError); return ExecutionTest(testName); }
        [Fact] public Task Variable_LetShadowing() { var testName = nameof(Variable_LetShadowing); return ExecutionTest(testName); }
    [Fact(Skip = "try/catch + TDZ runtime check not implemented yet")] public Task Variable_TemporalDeadZoneAccess() { var testName = nameof(Variable_TemporalDeadZoneAccess); return ExecutionTest(testName); }
        [Fact] public Task Variable_LetNestedShadowingChain() { var testName = nameof(Variable_LetNestedShadowingChain); return ExecutionTest(testName); }
        [Fact] public Task Variable_LetFunctionNestedShadowing() { var testName = nameof(Variable_LetFunctionNestedShadowing); return ExecutionTest(testName); }

    }
}
