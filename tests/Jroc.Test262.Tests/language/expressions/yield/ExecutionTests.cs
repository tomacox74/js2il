using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.yield_;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.yield") { }

    [Fact(DisplayName = "captured-free-vars")]
    public Task captured_free_vars()
        => ExecutionTest("captured-free-vars");

    [Fact(DisplayName = "formal-parameters")]
    public Task formal_parameters()
        => ExecutionTest("formal-parameters");

    [Fact(DisplayName = "from-catch")]
    public Task from_catch()
        => ExecutionTest("from-catch");

    [Fact(DisplayName = "from-try")]
    public Task from_try()
        => ExecutionTest("from-try");

    [Fact(DisplayName = "iter-value-specified")]
    public Task iter_value_specified()
        => ExecutionTest("iter-value-specified");

    [Fact(DisplayName = "iter-value-unspecified")]
    public Task iter_value_unspecified()
        => ExecutionTest("iter-value-unspecified");

    [Fact(DisplayName = "rhs-yield")]
    public Task rhs_yield()
        => ExecutionTest("rhs-yield");

    [Fact(DisplayName = "rhs-omitted")]
    public Task rhs_omitted()
        => ExecutionTest("rhs-omitted");

    [Fact(DisplayName = "rhs-primitive")]
    public Task rhs_primitive()
        => ExecutionTest("rhs-primitive");

    [Fact(DisplayName = "star-array")]
    public Task star_array()
        => ExecutionTest("star-array");

    [Fact(DisplayName = "star-iterable")]
    public Task star_iterable()
        => ExecutionTest("star-iterable");

    [Fact(DisplayName = "star-string")]
    public Task star_string()
        => ExecutionTest("star-string");

    [Fact(DisplayName = "rhs-iter")]
    public Task rhs_iter()
        => ExecutionTest("rhs-iter");

    [Fact(DisplayName = "rhs-regexp")]
    public Task rhs_regexp()
        => ExecutionTest("rhs-regexp");

    [Fact(DisplayName = "rhs-template-middle")]
    public Task rhs_template_middle()
        => ExecutionTest("rhs-template-middle");

    [Fact(DisplayName = "then-return")]
    public Task then_return()
        => ExecutionTest("then-return");
}
