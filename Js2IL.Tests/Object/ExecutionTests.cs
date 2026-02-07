using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.Object
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Object")
        {
        }

        [Fact]
        public Task ObjectLiteral_Spread_Basic() { var testName = nameof(ObjectLiteral_Spread_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task ObjectLiteral_ComputedKey_Basic() { var testName = nameof(ObjectLiteral_ComputedKey_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task ObjectLiteral_ComputedKey_EvaluationOrder() { var testName = nameof(ObjectLiteral_ComputedKey_EvaluationOrder); return ExecutionTest(testName); }

        [Fact]
        public Task ObjectLiteral_ShorthandAndMethod() { var testName = nameof(ObjectLiteral_ShorthandAndMethod); return ExecutionTest(testName); }

        // Regression: object literals emitted inline should not introduce invalid type tokens/casts.
        [Fact]
        public Task ObjectLiteral_InlinePropertyInit() { var testName = nameof(ObjectLiteral_InlinePropertyInit); return ExecutionTest(testName); }

        [Fact]
        public Task PrototypeChain_Basic() { var testName = nameof(PrototypeChain_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task ObjectCreate_NullPrototype_And_GetOwnPropertyDescriptor() { var testName = nameof(ObjectCreate_NullPrototype_And_GetOwnPropertyDescriptor); return ExecutionTest(testName); }

        [Fact]
        public Task ObjectDefineProperty_Accessor() { var testName = nameof(ObjectDefineProperty_Accessor); return ExecutionTest(testName); }

        [Fact]
        public Task ObjectDefineProperty_Enumerable_ForIn() { var testName = nameof(ObjectDefineProperty_Enumerable_ForIn); return ExecutionTest(testName); }

        [Fact]
        public Task ObjectCreate_WithPropertyDescriptors() { var testName = nameof(ObjectCreate_WithPropertyDescriptors); return ExecutionTest(testName); }

        // #544: LIRSetItem result temp must be treated as a defined SSA temp;
        // otherwise IL emission can crash when storing the assignment-expression result into a scope field.
        [Fact]
        public Task Object_AssignmentExpression_PropertySet_ResultStoredToScopeField() { var testName = nameof(Object_AssignmentExpression_PropertySet_ResultStoredToScopeField); return ExecutionTest(testName); }

        [Fact]
        public Task Object_GetOwnPropertyNames_Basic() { var testName = nameof(Object_GetOwnPropertyNames_Basic); return ExecutionTest(testName); }

        [Fact]
        public Task Object_Prototype_HasOwnProperty_Basic() { var testName = nameof(Object_Prototype_HasOwnProperty_Basic); return ExecutionTest(testName); }
    }
}
