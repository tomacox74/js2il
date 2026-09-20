using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.@try;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.statements.try") { }

    [Fact(DisplayName = "12.14-8")]
    public Task _12_14_8()
        => ExecutionTest("12.14-8");

    [Fact(DisplayName = "12.14-9")]
    public Task _12_14_9()
        => ExecutionTest("12.14-9");

    [Fact(DisplayName = "S12.14_A14")]
    public Task S12_14_A14()
        => ExecutionTest("S12.14_A14");

    [Fact(DisplayName = "S12.14_A15")]
    public Task S12_14_A15()
        => ExecutionTest("S12.14_A15");

    [Fact(DisplayName = "S12.14_A18_T6")]
    public Task S12_14_A18_T6()
        => ExecutionTest("S12.14_A18_T6");

    [Fact(DisplayName = "S12.14_A9_T1")]
    public Task S12_14_A9_T1()
        => ExecutionTest("S12.14_A9_T1");

    [Fact(DisplayName = "S12.14_A9_T5")]
    public Task S12_14_A9_T5()
        => ExecutionTest("S12.14_A9_T5");

    [Fact(DisplayName = "completion-values-fn-finally-abrupt")]
    public Task completion_values_fn_finally_abrupt()
        => ExecutionTest("completion-values-fn-finally-abrupt");

    [Fact(DisplayName = "completion-values-fn-finally-normal")]
    public Task completion_values_fn_finally_normal()
        => ExecutionTest("completion-values-fn-finally-normal");

    [Fact(DisplayName = "optional-catch-binding-lexical")]
    public Task optional_catch_binding_lexical()
        => ExecutionTest("optional-catch-binding-lexical");

}
