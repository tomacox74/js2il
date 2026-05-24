using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.class_.accessor_name_inst;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class_.accessor_name_inst") { }

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
