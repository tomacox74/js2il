using System.Threading.Tasks;

namespace Js2IL.Tests.Generator
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Generator")
        {
        }

        [Fact]
        public Task Generator_BasicNext() { var testName = nameof(Generator_BasicNext); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_ClassMethod_SimpleYield() { var testName = nameof(Generator_ClassMethod_SimpleYield); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_ClassMethod_YieldAssign() { var testName = nameof(Generator_ClassMethod_YieldAssign); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_ClassMethod_WithThis() { var testName = nameof(Generator_ClassMethod_WithThis); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_StaticMethod_SimpleYield() { var testName = nameof(Generator_StaticMethod_SimpleYield); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_Inheritance_SuperIteratorMethod() { var testName = nameof(Generator_Inheritance_SuperIteratorMethod); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_YieldStar_ArrayBasic() { var testName = nameof(Generator_YieldStar_ArrayBasic); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_YieldStar_NestedGenerator() { var testName = nameof(Generator_YieldStar_NestedGenerator); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_YieldStar_PassNextValue() { var testName = nameof(Generator_YieldStar_PassNextValue); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_YieldStar_ReturnForwards() { var testName = nameof(Generator_YieldStar_ReturnForwards); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_TryFinally_ReturnWhileSuspended() { var testName = nameof(Generator_TryFinally_ReturnWhileSuspended); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_TryFinally_ThrowWhileSuspended() { var testName = nameof(Generator_TryFinally_ThrowWhileSuspended); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_TryFinally_Nested_ReturnWhileSuspended() { var testName = nameof(Generator_TryFinally_Nested_ReturnWhileSuspended); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_TryCatchFinally_ThrowWhileSuspended_CatchYields() { var testName = nameof(Generator_TryCatchFinally_ThrowWhileSuspended_CatchYields); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_TryCatchFinally_ThrowWhileSuspended_Rethrow() { var testName = nameof(Generator_TryCatchFinally_ThrowWhileSuspended_Rethrow); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_TryCatchFinally_ThrowWhileSuspended_Nested() { var testName = nameof(Generator_TryCatchFinally_ThrowWhileSuspended_Nested); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_TryCatchFinally_ReturnWhileSuspended() { var testName = nameof(Generator_TryCatchFinally_ReturnWhileSuspended); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_Prototype_ToStringTag() { var testName = nameof(Generator_Prototype_ToStringTag); return ExecutionTest(testName); }

        [Fact]
        public Task Generator_Prototype_Constructor() { var testName = nameof(Generator_Prototype_Constructor); return ExecutionTest(testName); }
    }
}
