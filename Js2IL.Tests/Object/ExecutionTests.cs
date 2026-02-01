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
    }
}
