namespace Jroc.Test262.Tests.built_ins.Array.prototype.with;

public partial class ExecutionTests
{
    [Fact(DisplayName = "holes-not-preserved")]
    public Task holes_not_preserved() => ExecutionTestFromFile("holes-not-preserved");

    [Fact(DisplayName = "ignores-species")]
    public Task ignores_species() => ExecutionTestFromFile("ignores-species");

    [Fact(DisplayName = "immutable")]
    public Task immutable() => ExecutionTestFromFile("immutable");

    [Fact(DisplayName = "index-bigger-or-eq-than-length")]
    public Task index_bigger_or_eq_than_length() => ExecutionTestFromFile("index-bigger-or-eq-than-length");

    [Fact(DisplayName = "index-casted-to-number")]
    public Task index_casted_to_number() => ExecutionTestFromFile("index-casted-to-number");

    [Fact(DisplayName = "index-negative")]
    public Task index_negative() => ExecutionTestFromFile("index-negative");

    [Fact(DisplayName = "index-smaller-than-minus-length")]
    public Task index_smaller_than_minus_length() => ExecutionTestFromFile("index-smaller-than-minus-length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-fractional-index-truncated-to-zero")]
    public Task negative_fractional_index_truncated_to_zero() => ExecutionTestFromFile("negative-fractional-index-truncated-to-zero");

    [Fact(DisplayName = "no-get-replaced-index")]
    public Task no_get_replaced_index() => ExecutionTestFromFile("no-get-replaced-index");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "property-descriptor")]
    public Task property_descriptor() => ExecutionTestFromFile("property-descriptor");

    [Fact(DisplayName = "this-value-boolean")]
    public Task this_value_boolean() => ExecutionTestFromFile("this-value-boolean");

    [Fact(DisplayName = "this-value-nullish")]
    public Task this_value_nullish() => ExecutionTestFromFile("this-value-nullish");
}
