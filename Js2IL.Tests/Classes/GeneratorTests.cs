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
        [Fact] public Task Classes_ClassConstructor_TwoParams_AddMethod() { var testName = nameof(Classes_ClassConstructor_TwoParams_AddMethod); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassConstructor_TwoParams_SubtractMethod() { var testName = nameof(Classes_ClassConstructor_TwoParams_SubtractMethod); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassPrivateField_HelperMethod_Log() { var testName = nameof(Classes_ClassPrivateField_HelperMethod_Log); return GenerateTest(testName); }
    [Fact] public Task Classes_ClassPrivateProperty_HelperMethod_Log() { var testName = nameof(Classes_ClassPrivateProperty_HelperMethod_Log); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassMethod_CallsAnotherMethod() { var testName = nameof(Classes_ClassMethod_CallsAnotherMethod); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassMethod_ForLoop_CallsAnotherMethod() { var testName = nameof(Classes_ClassMethod_ForLoop_CallsAnotherMethod); return GenerateTest(testName); }

        // Repro: new ClassName() inside an arrow function fails to resolve the declared class in nested generator context
        // Current behavior (bug): codegen in arrow function cannot see ClassRegistry entry and may reference a non-existent runtime type,
        // leading to a TypeLoadException at execution time. Keep skipped until the bug is fixed.
        [Fact]
        public Task Classes_ClassConstructor_New_In_ArrowFunction()
        {
            var testName = nameof(Classes_ClassConstructor_New_In_ArrowFunction);
            return GenerateTest(testName);
        }

        // Repro for UpdateExpression inside a class method while-loop (postfix and prefix)
        [Fact] public Task Classes_ClassMethod_While_Increment_Postfix() { var testName = nameof(Classes_ClassMethod_While_Increment_Postfix); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassMethod_While_Increment_Prefix() { var testName = nameof(Classes_ClassMethod_While_Increment_Prefix); return GenerateTest(testName); }

        // Repro for UpdateExpression applied to a method parameter inside while-loop
        [Fact] public Task Classes_ClassMethod_While_Increment_Param_Postfix() { var testName = nameof(Classes_ClassMethod_While_Increment_Param_Postfix); return GenerateTest(testName); }
        [Fact] public Task Classes_ClassMethod_While_Increment_Param_Prefix() { var testName = nameof(Classes_ClassMethod_While_Increment_Param_Prefix); return GenerateTest(testName); }
    }
}
