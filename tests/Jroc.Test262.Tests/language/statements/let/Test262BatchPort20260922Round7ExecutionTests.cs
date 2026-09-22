using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.let;

public class Test262BatchPort20260922Round7ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260922Round7ExecutionTests() : base("language.statements.let") { }

    [Fact(DisplayName = "fn-name-arrow")]
    public Task fn_name_arrow()
        => ExecutionTest("fn-name-arrow");

    [Fact(DisplayName = "fn-name-class")]
    public Task fn_name_class()
        => ExecutionTest("fn-name-class");

    [Fact(DisplayName = "fn-name-cover")]
    public Task fn_name_cover()
        => ExecutionTest("fn-name-cover");

    [Fact(DisplayName = "fn-name-fn")]
    public Task fn_name_fn()
        => ExecutionTest("fn-name-fn");

    [Fact(DisplayName = "fn-name-gen")]
    public Task fn_name_gen()
        => ExecutionTest("fn-name-gen");

}
