using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.matchAll;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.matchAll") { }

    [Fact(DisplayName = "cstm-matchall-on-bigint-primitive.js")]
    public Task cstm_matchall_on_bigint_primitive()
        => ExecutionTestFromFile("cstm-matchall-on-bigint-primitive");

    [Fact(DisplayName = "cstm-matchall-on-number-primitive.js")]
    public Task cstm_matchall_on_number_primitive()
        => ExecutionTestFromFile("cstm-matchall-on-number-primitive");

    [Fact(DisplayName = "cstm-matchall-on-string-primitive.js")]
    public Task cstm_matchall_on_string_primitive()
        => ExecutionTestFromFile("cstm-matchall-on-string-primitive");
}
