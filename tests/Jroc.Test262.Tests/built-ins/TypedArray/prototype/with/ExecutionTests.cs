using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.with;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype.with") { }

    [Fact(DisplayName = "early-type-coercion")]
    public Task early_type_coercion() => ExecutionTestFromFile("early-type-coercion");

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

    [Fact(DisplayName = "index-throw-completion")]
    public Task index_throw_completion() => ExecutionTestFromFile("index-throw-completion");

    [Fact(DisplayName = "length-property-ignored")]
    public Task length_property_ignored() => ExecutionTestFromFile("length-property-ignored");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-fractional-index-truncated-to-zero")]
    public Task negative_fractional_index_truncated_to_zero() => ExecutionTestFromFile("negative-fractional-index-truncated-to-zero");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "order-of-evaluation")]
    public Task order_of_evaluation() => ExecutionTestFromFile("order-of-evaluation");

    [Fact(DisplayName = "value-throw-completion")]
    public Task value_throw_completion() => ExecutionTestFromFile("value-throw-completion");
}
