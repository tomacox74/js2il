using System.Threading.Tasks;

namespace Js2IL.Tests.Generator
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Generator")
        {
        }

        [Fact]
        public Task Generator_BasicNext() { var testName = nameof(Generator_BasicNext); return GenerateTest(testName); }

        [Fact]
        public Task Generator_ClassMethod_SimpleYield() { var testName = nameof(Generator_ClassMethod_SimpleYield); return GenerateTest(testName); }

        [Fact]
        public Task Generator_ClassMethod_YieldAssign() { var testName = nameof(Generator_ClassMethod_YieldAssign); return GenerateTest(testName); }

        [Fact]
        public Task Generator_ClassMethod_WithThis() { var testName = nameof(Generator_ClassMethod_WithThis); return GenerateTest(testName); }

        [Fact]
        public Task Generator_StaticMethod_SimpleYield() { var testName = nameof(Generator_StaticMethod_SimpleYield); return GenerateTest(testName); }

        [Fact]
        public Task Generator_Inheritance_SuperIteratorMethod() { var testName = nameof(Generator_Inheritance_SuperIteratorMethod); return GenerateTest(testName); }

        [Fact]
        public Task Generator_YieldStar_ArrayBasic() { var testName = nameof(Generator_YieldStar_ArrayBasic); return GenerateTest(testName); }

        [Fact]
        public Task Generator_YieldStar_NestedGenerator() { var testName = nameof(Generator_YieldStar_NestedGenerator); return GenerateTest(testName); }

        [Fact]
        public Task Generator_YieldStar_PassNextValue() { var testName = nameof(Generator_YieldStar_PassNextValue); return GenerateTest(testName); }

        [Fact]
        public Task Generator_YieldStar_ReturnForwards() { var testName = nameof(Generator_YieldStar_ReturnForwards); return GenerateTest(testName); }

        [Fact]
        public Task Generator_TryFinally_ReturnWhileSuspended() { var testName = nameof(Generator_TryFinally_ReturnWhileSuspended); return GenerateTest(testName); }

        [Fact]
        public Task Generator_TryFinally_ThrowWhileSuspended() { var testName = nameof(Generator_TryFinally_ThrowWhileSuspended); return GenerateTest(testName); }

        [Fact]
        public Task Generator_TryFinally_Nested_ReturnWhileSuspended() { var testName = nameof(Generator_TryFinally_Nested_ReturnWhileSuspended); return GenerateTest(testName); }
    }
}
