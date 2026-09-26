using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.Symbol.split;

public class ExecutionTests : InMemoryExecutionTestsBase
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

    [Fact(DisplayName = "coerce-flags-err.js")]
    public Task ported_coerce_flags_err() => ExecutionTestFromFile("coerce-flags-err");

    [Fact(DisplayName = "coerce-flags.js")]
    public Task ported_coerce_flags() => ExecutionTestFromFile("coerce-flags");

    [Fact(DisplayName = "coerce-string-err.js")]
    public Task ported_coerce_string_err() => ExecutionTestFromFile("coerce-string-err");

    [Fact(DisplayName = "get-flags-err.js")]
    public Task ported_get_flags_err() => ExecutionTestFromFile("get-flags-err");

    [Fact(DisplayName = "last-index-exceeds-str-size.js")]
    public Task ported_last_index_exceeds_str_size() => ExecutionTestFromFile("last-index-exceeds-str-size");

    [Fact(DisplayName = "limit-0-bail.js")]
    public Task ported_limit_0_bail() => ExecutionTestFromFile("limit-0-bail");

    [Fact(DisplayName = "name.js")]
    public Task ported_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "species-ctor-ctor-get-err.js")]
    public Task ported_species_ctor_ctor_get_err() => ExecutionTestFromFile("species-ctor-ctor-get-err");

    [Fact(DisplayName = "species-ctor-ctor-non-obj.js")]
    public Task ported_species_ctor_ctor_non_obj() => ExecutionTestFromFile("species-ctor-ctor-non-obj");

    [Fact(DisplayName = "species-ctor-err.js")]
    public Task ported_species_ctor_err() => ExecutionTestFromFile("species-ctor-err");

    [Fact(DisplayName = "species-ctor-species-get-err.js")]
    public Task ported_species_ctor_species_get_err() => ExecutionTestFromFile("species-ctor-species-get-err");

    [Fact(DisplayName = "species-ctor-species-non-ctor.js")]
    public Task ported_species_ctor_species_non_ctor() => ExecutionTestFromFile("species-ctor-species-non-ctor");

    [Fact(DisplayName = "species-ctor-y.js")]
    public Task ported_species_ctor_y() => ExecutionTestFromFile("species-ctor-y");

    [Fact(DisplayName = "species-ctor.js")]
    public Task ported_species_ctor() => ExecutionTestFromFile("species-ctor");

    [Fact(DisplayName = "str-coerce-lastindex-err.js")]
    public Task ported_str_coerce_lastindex_err() => ExecutionTestFromFile("str-coerce-lastindex-err");

    [Fact(DisplayName = "str-coerce-lastindex.js")]
    public Task ported_str_coerce_lastindex() => ExecutionTestFromFile("str-coerce-lastindex");

    [Fact(DisplayName = "str-empty-match-err.js")]
    public Task ported_str_empty_match_err() => ExecutionTestFromFile("str-empty-match-err");
}
