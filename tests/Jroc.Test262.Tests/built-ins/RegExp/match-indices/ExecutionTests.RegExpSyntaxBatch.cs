using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.match_indices;

public partial class ExecutionTests
{
    [Fact(DisplayName = "indices-array-element.js")]
    public Task indices_array_element() => ExecutionTestFromFile("indices-array-element");

    [Fact(DisplayName = "indices-array-matched.js")]
    public Task indices_array_matched() => ExecutionTestFromFile("indices-array-matched");

    [Fact(DisplayName = "indices-array-non-unicode-match.js")]
    public Task indices_array_non_unicode_match() => ExecutionTestFromFile("indices-array-non-unicode-match");

    [Fact(DisplayName = "indices-array-properties.js")]
    public Task indices_array_properties() => ExecutionTestFromFile("indices-array-properties");

    [Fact(DisplayName = "indices-array-unicode-match.js")]
    public Task indices_array_unicode_match() => ExecutionTestFromFile("indices-array-unicode-match");

    [Fact(DisplayName = "indices-array.js")]
    public Task indices_array() => ExecutionTestFromFile("indices-array");

    [Fact(DisplayName = "indices-groups-object-unmatched.js")]
    public Task indices_groups_object_unmatched() => ExecutionTestFromFile("indices-groups-object-unmatched");

    [Fact(DisplayName = "indices-groups-object.js")]
    public Task indices_groups_object() => ExecutionTestFromFile("indices-groups-object");

    [Fact(DisplayName = "indices-property.js")]
    public Task indices_property() => ExecutionTestFromFile("indices-property");

    [Fact(DisplayName = "no-indices-array.js")]
    public Task no_indices_array() => ExecutionTestFromFile("no-indices-array");

}
