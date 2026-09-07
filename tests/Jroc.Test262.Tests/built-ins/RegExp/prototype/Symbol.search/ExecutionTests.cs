using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.Symbol.search;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.prototype.Symbol.search") { }

    [Fact(DisplayName = "coerce-string.js")]
    public Task coerce_string()
        => ExecutionTestFromFile("coerce-string");

    [Fact(DisplayName = "cstm-exec-return-invalid.js")]
    public Task cstm_exec_return_invalid()
        => ExecutionTestFromFile("cstm-exec-return-invalid");

    [Fact(DisplayName = "failure-return-val.js")]
    public Task failure_return_val()
        => ExecutionTestFromFile("failure-return-val");

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "success-return-val.js")]
    public Task success_return_val()
        => ExecutionTestFromFile("success-return-val");

    [Fact(DisplayName = "this-val-non-obj.js")]
    public Task this_val_non_obj()
        => ExecutionTestFromFile("this-val-non-obj");

    [Fact(DisplayName = "u-lastindex-advance.js")]
    public Task u_lastindex_advance()
        => ExecutionTestFromFile("u-lastindex-advance");

    [Fact(DisplayName = "y-fail-return.js")]
    public Task y_fail_return()
        => ExecutionTestFromFile("y-fail-return");

}
