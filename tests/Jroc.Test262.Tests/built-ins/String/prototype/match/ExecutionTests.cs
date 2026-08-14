using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.match;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.match") { }

    [Fact(DisplayName = "cstm-matcher-on-bigint-primitive.js")]
    public Task cstm_matcher_on_bigint_primitive()
        => ExecutionTestFromFile("cstm-matcher-on-bigint-primitive");

    [Fact(DisplayName = "cstm-matcher-on-boolean-primitive.js")]
    public Task cstm_matcher_on_boolean_primitive()
        => ExecutionTestFromFile("cstm-matcher-on-boolean-primitive");

    [Fact(DisplayName = "cstm-matcher-on-number-primitive.js")]
    public Task cstm_matcher_on_number_primitive()
        => ExecutionTestFromFile("cstm-matcher-on-number-primitive");

    [Fact(DisplayName = "cstm-matcher-on-string-primitive.js")]
    public Task cstm_matcher_on_string_primitive()
        => ExecutionTestFromFile("cstm-matcher-on-string-primitive");
}
