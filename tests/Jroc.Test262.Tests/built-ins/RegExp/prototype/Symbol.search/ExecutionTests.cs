using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.Symbol.search;

public class ExecutionTests : InMemoryExecutionTestsBase
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

    [Fact(DisplayName = "coerce-string-err.js")]
    public Task ported_coerce_string_err() => ExecutionTestFromFile("coerce-string-err");

    [Fact(DisplayName = "cstm-exec-return-index.js")]
    public Task ported_cstm_exec_return_index() => ExecutionTestFromFile("cstm-exec-return-index");

    [Fact(DisplayName = "get-lastindex-err.js")]
    public Task ported_get_lastindex_err() => ExecutionTestFromFile("get-lastindex-err");

    [Fact(DisplayName = "lastindex-no-restore.js")]
    public Task ported_lastindex_no_restore() => ExecutionTestFromFile("lastindex-no-restore");

    [Fact(DisplayName = "match-err.js")]
    public Task ported_match_err() => ExecutionTestFromFile("match-err");

    [Fact(DisplayName = "name.js")]
    public Task ported_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "set-lastindex-init-err.js")]
    public Task ported_set_lastindex_init_err() => ExecutionTestFromFile("set-lastindex-init-err");

    [Fact(DisplayName = "set-lastindex-init-samevalue.js")]
    public Task ported_set_lastindex_init_samevalue() => ExecutionTestFromFile("set-lastindex-init-samevalue");

    [Fact(DisplayName = "set-lastindex-init.js")]
    public Task ported_set_lastindex_init() => ExecutionTestFromFile("set-lastindex-init");

    [Fact(DisplayName = "set-lastindex-restore-err.js")]
    public Task ported_set_lastindex_restore_err() => ExecutionTestFromFile("set-lastindex-restore-err");

    [Fact(DisplayName = "set-lastindex-restore-samevalue.js")]
    public Task ported_set_lastindex_restore_samevalue() => ExecutionTestFromFile("set-lastindex-restore-samevalue");

    [Fact(DisplayName = "set-lastindex-restore.js")]
    public Task ported_set_lastindex_restore() => ExecutionTestFromFile("set-lastindex-restore");

    [Fact(DisplayName = "success-get-index-err.js")]
    public Task ported_success_get_index_err() => ExecutionTestFromFile("success-get-index-err");
}
