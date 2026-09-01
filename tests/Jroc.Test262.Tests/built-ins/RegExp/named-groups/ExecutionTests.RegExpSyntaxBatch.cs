using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.named_groups;

public partial class ExecutionTests
{
    [Fact(DisplayName = "duplicate-names-group-property-enumeration-order.js")]
    public Task duplicate_names_group_property_enumeration_order() => ExecutionTestFromFile("duplicate-names-group-property-enumeration-order");

    [Fact(DisplayName = "duplicate-names-match-indices.js")]
    public Task duplicate_names_match_indices() => ExecutionTestFromFile("duplicate-names-match-indices");

    [Fact(DisplayName = "duplicate-names-search.js")]
    public Task duplicate_names_search() => ExecutionTestFromFile("duplicate-names-search");

    [Fact(DisplayName = "duplicate-names-test.js")]
    public Task duplicate_names_test() => ExecutionTestFromFile("duplicate-names-test");

    [Fact(DisplayName = "groups-object.js")]
    public Task groups_object() => ExecutionTestFromFile("groups-object");

    [Fact(DisplayName = "lookbehind.js")]
    public Task lookbehind() => ExecutionTestFromFile("lookbehind");

    [Fact(DisplayName = "non-unicode-property-names-invalid.js")]
    public Task non_unicode_property_names_invalid() => ExecutionTestFromFile("non-unicode-property-names-invalid");

    [Fact(DisplayName = "string-replace-escaped.js")]
    public Task string_replace_escaped() => ExecutionTestFromFile("string-replace-escaped");

    [Fact(DisplayName = "string-replace-nocaptures.js")]
    public Task string_replace_nocaptures() => ExecutionTestFromFile("string-replace-nocaptures");

    [Fact(DisplayName = "string-replace-numbered.js")]
    public Task string_replace_numbered() => ExecutionTestFromFile("string-replace-numbered");

    [Fact(DisplayName = "string-replace-unclosed.js")]
    public Task string_replace_unclosed() => ExecutionTestFromFile("string-replace-unclosed");

    [Fact(DisplayName = "unicode-property-names-invalid.js")]
    public Task unicode_property_names_invalid() => ExecutionTestFromFile("unicode-property-names-invalid");

}
