using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.Symbol.split;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.prototype.Symbol.split") { }

    [Fact(DisplayName = "coerce-limit-err.js")]
    public Task coerce_limit_err()
        => ExecutionTestFromFile("coerce-limit-err");

    [Fact(DisplayName = "coerce-limit.js")]
    public Task coerce_limit()
        => ExecutionTestFromFile("coerce-limit");

    [Fact(DisplayName = "coerce-string.js")]
    public Task coerce_string()
        => ExecutionTestFromFile("coerce-string");

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "species-ctor-ctor-undef.js")]
    public Task species_ctor_ctor_undef()
        => ExecutionTestFromFile("species-ctor-ctor-undef");

    [Fact(DisplayName = "species-ctor-species-undef.js")]
    public Task species_ctor_species_undef()
        => ExecutionTestFromFile("species-ctor-species-undef");

    [Fact(DisplayName = "str-adv-thru-empty-match.js")]
    public Task str_adv_thru_empty_match()
        => ExecutionTestFromFile("str-adv-thru-empty-match");

    [Fact(DisplayName = "str-empty-match.js")]
    public Task str_empty_match()
        => ExecutionTestFromFile("str-empty-match");

    [Fact(DisplayName = "str-empty-no-match.js")]
    public Task str_empty_no_match()
        => ExecutionTestFromFile("str-empty-no-match");

    [Fact(DisplayName = "str-limit-capturing.js")]
    public Task str_limit_capturing()
        => ExecutionTestFromFile("str-limit-capturing");

    [Fact(DisplayName = "str-limit.js")]
    public Task str_limit()
        => ExecutionTestFromFile("str-limit");

    [Fact(DisplayName = "str-trailing-chars.js")]
    public Task str_trailing_chars()
        => ExecutionTestFromFile("str-trailing-chars");

    [Fact(DisplayName = "this-val-non-obj.js")]
    public Task this_val_non_obj()
        => ExecutionTestFromFile("this-val-non-obj");

    [Fact(DisplayName = "u-lastindex-adv-thru-failure.js")]
    public Task u_lastindex_adv_thru_failure()
        => ExecutionTestFromFile("u-lastindex-adv-thru-failure");

    [Fact(DisplayName = "u-lastindex-adv-thru-match.js")]
    public Task u_lastindex_adv_thru_match()
        => ExecutionTestFromFile("u-lastindex-adv-thru-match");

}
