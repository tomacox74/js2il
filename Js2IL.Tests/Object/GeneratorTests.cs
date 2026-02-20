using System.Threading.Tasks;
using Xunit;

namespace Js2IL.Tests.Object
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Object")
        {
        }

        [Fact]
        public Task ObjectLiteral_Spread_Basic() { var testName = nameof(ObjectLiteral_Spread_Basic); return GenerateTest(testName); }

        [Fact]
        public Task ObjectLiteral_Spread_Multiple() { var testName = nameof(ObjectLiteral_Spread_Multiple); return GenerateTest(testName); }

        [Fact]
        public Task ObjectLiteral_Spread_Clone() { var testName = nameof(ObjectLiteral_Spread_Clone); return GenerateTest(testName); }

        [Fact]
        public Task ObjectLiteral_Spread_Empty() { var testName = nameof(ObjectLiteral_Spread_Empty); return GenerateTest(testName); }

        [Fact]
        public Task ObjectLiteral_Spread_SymbolProperties() { var testName = nameof(ObjectLiteral_Spread_SymbolProperties); return GenerateTest(testName); }

        [Fact]
        public Task ObjectLiteral_Spread_NestedObjects() { var testName = nameof(ObjectLiteral_Spread_NestedObjects); return GenerateTest(testName); }

        [Fact]
        public Task ObjectLiteral_Spread_SkipsNonEnumerable() { var testName = nameof(ObjectLiteral_Spread_SkipsNonEnumerable); return GenerateTest(testName); }

        [Fact]
        public Task ObjectLiteral_ComputedKey_Basic() { var testName = nameof(ObjectLiteral_ComputedKey_Basic); return GenerateTest(testName); }

        [Fact]
        public Task ObjectLiteral_ComputedKey_EvaluationOrder() { var testName = nameof(ObjectLiteral_ComputedKey_EvaluationOrder); return GenerateTest(testName); }

        [Fact]
        public Task ObjectLiteral_ShorthandAndMethod() { var testName = nameof(ObjectLiteral_ShorthandAndMethod); return GenerateTest(testName); }

        // Regression: object literals emitted inline should not introduce invalid type tokens/casts.
        [Fact]
        public Task ObjectLiteral_InlinePropertyInit() { var testName = nameof(ObjectLiteral_InlinePropertyInit); return GenerateTest(testName); }

        [Fact]
        public Task PrototypeChain_Basic() { var testName = nameof(PrototypeChain_Basic); return GenerateTest(testName); }

        [Fact]
        public Task ObjectCreate_NullPrototype_And_GetOwnPropertyDescriptor() { var testName = nameof(ObjectCreate_NullPrototype_And_GetOwnPropertyDescriptor); return GenerateTest(testName); }

        [Fact]
        public Task ObjectDefineProperty_Accessor() { var testName = nameof(ObjectDefineProperty_Accessor); return GenerateTest(testName); }

        [Fact]
        public Task ObjectDefineProperty_Enumerable_ForIn() { var testName = nameof(ObjectDefineProperty_Enumerable_ForIn); return GenerateTest(testName); }

        [Fact]
        public Task ObjectCreate_WithPropertyDescriptors() { var testName = nameof(ObjectCreate_WithPropertyDescriptors); return GenerateTest(testName); }

        // #544: LIRSetItem result temp must be treated as a defined SSA temp;
        // otherwise IL emission can crash when storing the assignment-expression result into a scope field.
        [Fact]
        public Task Object_AssignmentExpression_PropertySet_ResultStoredToScopeField() { var testName = nameof(Object_AssignmentExpression_PropertySet_ResultStoredToScopeField); return GenerateTest(testName); }

        [Fact]
        public Task Object_GetOwnPropertyNames_Basic() { var testName = nameof(Object_GetOwnPropertyNames_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Object_Prototype_HasOwnProperty_Basic() { var testName = nameof(Object_Prototype_HasOwnProperty_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Object_Keys_Basic() { var testName = nameof(Object_Keys_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Object_Values_Basic() { var testName = nameof(Object_Values_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Object_Is_SameValue() { var testName = nameof(Object_Is_SameValue); return GenerateTest(testName); }

        [Fact]
        public Task Object_Entries_Basic() { var testName = nameof(Object_Entries_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Object_Assign_Basic() { var testName = nameof(Object_Assign_Basic); return GenerateTest(testName); }

        [Fact]
        public Task Object_FromEntries_Basic() { var testName = nameof(Object_FromEntries_Basic); return GenerateTest(testName); }
    }
}
