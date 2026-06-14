using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.reserved_words;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.reserved_words") { }

    [Fact(DisplayName = "ident-name-global-property-accessor")]
    public Task ident_name_global_property_accessor()
        => ExecutionTest("ident-name-global-property-accessor");

    [Fact(DisplayName = "ident-name-global-property-memberexpr")]
    public Task ident_name_global_property_memberexpr()
        => ExecutionTest("ident-name-global-property-memberexpr");

    [Fact(DisplayName = "ident-name-global-property-memberexpr-str")]
    public Task ident_name_global_property_memberexpr_str()
        => ExecutionTest("ident-name-global-property-memberexpr-str");

    [Fact(DisplayName = "ident-name-global-property-prop-name")]
    public Task ident_name_global_property_prop_name()
        => ExecutionTest("ident-name-global-property-prop-name");

    [Fact(DisplayName = "ident-name-reserved-word-literal-accessor")]
    public Task ident_name_reserved_word_literal_accessor()
        => ExecutionTest("ident-name-reserved-word-literal-accessor");

    [Fact(DisplayName = "ident-name-reserved-word-literal-memberexpr")]
    public Task ident_name_reserved_word_literal_memberexpr()
        => ExecutionTest("ident-name-reserved-word-literal-memberexpr");

    [Fact(DisplayName = "ident-name-reserved-word-literal-memberexpr-str")]
    public Task ident_name_reserved_word_literal_memberexpr_str()
        => ExecutionTest("ident-name-reserved-word-literal-memberexpr-str");

    [Fact(DisplayName = "ident-name-reserved-word-literal-prop-name")]
    public Task ident_name_reserved_word_literal_prop_name()
        => ExecutionTest("ident-name-reserved-word-literal-prop-name");
}
