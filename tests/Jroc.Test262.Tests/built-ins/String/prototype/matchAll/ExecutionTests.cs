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

    [Fact(DisplayName = "regexp-prototype-has-no-matchAll.js")]
    public Task regexp_prototype_has_no_matchAll()
        => ExecutionTestFromFile("regexp-prototype-has-no-matchAll");

    [Fact(DisplayName = "regexp-prototype-matchAll-invocation.js")]
    public Task regexp_prototype_matchAll_invocation()
        => ExecutionTestFromFile("regexp-prototype-matchAll-invocation");

    [Fact(DisplayName = "regexp-prototype-matchAll-throws.js")]
    public Task regexp_prototype_matchAll_throws()
        => ExecutionTestFromFile("regexp-prototype-matchAll-throws");

    [Fact(DisplayName = "regexp-matchAll-throws.js")]
    public Task regexp_matchAll_throws()
        => ExecutionTestFromFile("regexp-matchAll-throws");

    [Fact(DisplayName = "regexp-get-matchAll-throws.js")]
    public Task regexp_get_matchAll_throws()
        => ExecutionTestFromFile("regexp-get-matchAll-throws");
}
