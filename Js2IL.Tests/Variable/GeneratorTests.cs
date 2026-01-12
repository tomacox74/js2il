using System.Threading.Tasks;

namespace Js2IL.Tests.Variable
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Variable") { }

        // Variable (const/let) generator tests scaffold
        [Fact] public Task Variable_ConstReassignmentError() { var testName = nameof(Variable_ConstReassignmentError); return GenerateTest(testName); }
        [Fact] public Task Variable_ConstSimple() { var testName = nameof(Variable_ConstSimple); return GenerateTest(testName); }
        [Fact] public Task Variable_LetBlockScope() { var testName = nameof(Variable_LetBlockScope); return GenerateTest(testName); }
        [Fact] public Task Variable_LetFunctionNestedShadowing() { var testName = nameof(Variable_LetFunctionNestedShadowing); return GenerateTest(testName); }
        [Fact] public Task Variable_LetNestedShadowingChain() { var testName = nameof(Variable_LetNestedShadowingChain); return GenerateTest(testName); }
        [Fact] public Task Variable_LetShadowing() { var testName = nameof(Variable_LetShadowing); return GenerateTest(testName); }
        [Fact(Skip = "try/catch + TDZ runtime check not implemented yet")] public Task Variable_TemporalDeadZoneAccess() { var testName = nameof(Variable_TemporalDeadZoneAccess); return GenerateTest(testName); }

        // Object destructuring generator tests
        [Fact] public Task Variable_ObjectDestructuring_Basic() { var testName = nameof(Variable_ObjectDestructuring_Basic); return GenerateTest(testName); }
        [Fact] public Task Variable_ObjectDestructuring_WithDefaults() { var testName = nameof(Variable_ObjectDestructuring_WithDefaults); return GenerateTest(testName); }
        [Fact] public Task Variable_ObjectDestructuring_Captured() { var testName = nameof(Variable_ObjectDestructuring_Captured); return GenerateTest(testName); }

        // Destructuring + assignment targets (PL4.*)
        [Fact] public Task Variable_ArrayDestructuring_Basic() { var testName = nameof(Variable_ArrayDestructuring_Basic); return GenerateTest(testName); }
        [Fact] public Task Variable_ArrayDestructuring_DefaultsAndRest() { var testName = nameof(Variable_ArrayDestructuring_DefaultsAndRest); return GenerateTest(testName); }
        [Fact] public Task Variable_NestedDestructuring_Defaults() { var testName = nameof(Variable_NestedDestructuring_Defaults); return GenerateTest(testName); }
        [Fact] public Task Variable_ObjectDestructuring_Rest() { var testName = nameof(Variable_ObjectDestructuring_Rest); return GenerateTest(testName); }
        [Fact] public Task Variable_AssignmentTargets_MemberAndIndex() { var testName = nameof(Variable_AssignmentTargets_MemberAndIndex); return GenerateTest(testName); }
        [Fact] public Task Variable_DestructuringAssignment_Basic() { var testName = nameof(Variable_DestructuringAssignment_Basic); return GenerateTest(testName); }
    }
}
