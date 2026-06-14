using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.class_.accessor_name_inst;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.accessor_name_inst") { }

    [Fact(DisplayName = "computed")]
    public Task computed()
        => ExecutionTest("computed");

    [Fact(DisplayName = "computed-err-evaluation")]
    public Task computed_err_evaluation()
        => ExecutionTest("computed-err-evaluation");

    [Fact(DisplayName = "computed-err-unresolvable")]
    public Task computed_err_unresolvable()
        => ExecutionTest("computed-err-unresolvable");
}
