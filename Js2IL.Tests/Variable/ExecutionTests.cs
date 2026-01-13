using System.Threading.Tasks;

namespace Js2IL.Tests.Variable
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Variable") { }

        // Variable (const/let) tests scaffold
        [Fact] public Task Variable_ConstReassignmentError() { var testName = nameof(Variable_ConstReassignmentError); return ExecutionTest(testName); }
        [Fact] public Task Variable_ConstSimple() { var testName = nameof(Variable_ConstSimple); return ExecutionTest(testName); }
        [Fact] public Task Variable_CapturedConst_NumberFieldType() { var testName = nameof(Variable_CapturedConst_NumberFieldType); return ExecutionTest(testName); }
        [Fact] public Task Variable_LetBlockScope() { var testName = nameof(Variable_LetBlockScope); return ExecutionTest(testName); }
        [Fact] public Task Variable_LetFunctionNestedShadowing() { var testName = nameof(Variable_LetFunctionNestedShadowing); return ExecutionTest(testName); }
        [Fact] public Task Variable_LetNestedShadowingChain() { var testName = nameof(Variable_LetNestedShadowingChain); return ExecutionTest(testName); }
        [Fact] public Task Variable_LetShadowing() { var testName = nameof(Variable_LetShadowing); return ExecutionTest(testName); }
        [Fact(Skip = "try/catch + TDZ runtime check not implemented yet")] public Task Variable_TemporalDeadZoneAccess() { var testName = nameof(Variable_TemporalDeadZoneAccess); return ExecutionTest(testName); }

        // Object destructuring tests
        [Fact] public Task Variable_ObjectDestructuring_Basic() { var testName = nameof(Variable_ObjectDestructuring_Basic); return ExecutionTest(testName); }
        [Fact] public Task Variable_ObjectDestructuring_WithDefaults() { var testName = nameof(Variable_ObjectDestructuring_WithDefaults); return ExecutionTest(testName); }
        [Fact] public Task Variable_ObjectDestructuring_Captured() { var testName = nameof(Variable_ObjectDestructuring_Captured); return ExecutionTest(testName); }

        // Destructuring + assignment targets (PL4.*)
        [Fact] public Task Variable_ArrayDestructuring_Basic() { var testName = nameof(Variable_ArrayDestructuring_Basic); return ExecutionTest(testName); }
        [Fact] public Task Variable_ArrayDestructuring_DefaultsAndRest() { var testName = nameof(Variable_ArrayDestructuring_DefaultsAndRest); return ExecutionTest(testName); }
        [Fact] public Task Variable_NestedDestructuring_Defaults() { var testName = nameof(Variable_NestedDestructuring_Defaults); return ExecutionTest(testName); }
        [Fact] public Task Variable_ObjectDestructuring_Rest() { var testName = nameof(Variable_ObjectDestructuring_Rest); return ExecutionTest(testName); }
        [Fact] public Task Variable_AssignmentTargets_MemberAndIndex() { var testName = nameof(Variable_AssignmentTargets_MemberAndIndex); return ExecutionTest(testName); }
        [Fact] public Task Variable_DestructuringAssignment_Basic() { var testName = nameof(Variable_DestructuringAssignment_Basic); return ExecutionTest(testName); }
        [Fact] public Task Variable_Destructuring_NullOrUndefined_ThrowsNodeMessage() { var testName = nameof(Variable_Destructuring_NullOrUndefined_ThrowsNodeMessage); return ExecutionTest(testName); }
    }
}
