namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.Symbol_split;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp.prototype.Symbol_split") { }

    [Fact(DisplayName = "splitter-proto-from-ctor-realm.js")]
    public Task splitter_proto_from_ctor_realm()
        => ExecutionTest("splitter-proto-from-ctor-realm");

    [Fact(DisplayName = "str-get-lastindex-err.js")]
    public Task str_get_lastindex_err()
        => ExecutionTest("str-get-lastindex-err");

    [Fact(DisplayName = "str-match-err.js")]
    public Task str_match_err()
        => ExecutionTest("str-match-err");

    [Fact(DisplayName = "str-result-coerce-length-err.js")]
    public Task str_result_coerce_length_err()
        => ExecutionTest("str-result-coerce-length-err");

    [Fact(DisplayName = "str-result-coerce-length.js")]
    public Task str_result_coerce_length()
        => ExecutionTest("str-result-coerce-length");

    [Fact(DisplayName = "str-result-get-capture-err.js")]
    public Task str_result_get_capture_err()
        => ExecutionTest("str-result-get-capture-err");

    [Fact(DisplayName = "str-result-get-length-err.js")]
    public Task str_result_get_length_err()
        => ExecutionTest("str-result-get-length-err");

    [Fact(DisplayName = "str-set-lastindex-err.js")]
    public Task str_set_lastindex_err()
        => ExecutionTest("str-set-lastindex-err");

    [Fact(DisplayName = "str-set-lastindex-match.js")]
    public Task str_set_lastindex_match()
        => ExecutionTest("str-set-lastindex-match");

    [Fact(DisplayName = "str-set-lastindex-no-match.js")]
    public Task str_set_lastindex_no_match()
        => ExecutionTest("str-set-lastindex-no-match");
}
