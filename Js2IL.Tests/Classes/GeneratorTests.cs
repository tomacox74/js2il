using System.Threading.Tasks;

namespace Js2IL.Tests.Classes
{
    public class GeneratorTests : GeneratorTestsBase
    {
        public GeneratorTests() : base("Classes") { }

        // Classes generator tests
        [Fact] public Task Classes_DeclareEmptyClass() { var testName = nameof(Classes_DeclareEmptyClass); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassWithMethod_NoInstantiation() { var testName = nameof(Classes_ClassWithMethod_NoInstantiation); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassWithMethod_HelloWorld() { var testName = nameof(Classes_ClassWithMethod_HelloWorld); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassWithStaticMethod_HelloWorld() { var testName = nameof(Classes_ClassWithStaticMethod_HelloWorld); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassProperty_DefaultAndLog() { var testName = nameof(Classes_ClassProperty_DefaultAndLog); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassConstructor_Param_Field_Log() { var testName = nameof(Classes_ClassConstructor_Param_Field_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassConstructor_WithMultipleParameters() { var testName = nameof(Classes_ClassConstructor_WithMultipleParameters); return GenerateTest(testName); }
    // Pseudo-private underscore fields are public in JS; prefer true private (#) tests only
        // True JS private field using # (not yet supported)
        [Fact(Skip = "ECMAScript private fields (#) not yet supported")] public Task Classes_ClassPrivateField_HelperMethod_Log() { var testName = nameof(Classes_ClassPrivateField_HelperMethod_Log); return GenerateTest(testName); }
    }
}
