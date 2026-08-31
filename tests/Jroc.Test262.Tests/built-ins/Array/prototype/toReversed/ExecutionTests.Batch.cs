namespace Jroc.Test262.Tests.built_ins.Array.prototype.toReversed;

public partial class ExecutionTests
{
    [Fact(DisplayName = "frozen-this-value")]
    public Task frozen_this_value() => ExecutionTestFromFile("frozen-this-value");
    [Fact(DisplayName = "get-descending-order")]
    public Task get_descending_order() => ExecutionTestFromFile("get-descending-order");
    [Fact(DisplayName = "ignores-species")]
    public Task ignores_species() => ExecutionTestFromFile("ignores-species");
    [Fact(DisplayName = "length-casted-to-zero")]
    public Task length_casted_to_zero() => ExecutionTestFromFile("length-casted-to-zero");
    [Fact(DisplayName = "length-exceeding-array-length-limit")]
    public Task length_exceeding_array_length_limit() => ExecutionTestFromFile("length-exceeding-array-length-limit");
    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "length-tolength")]
    public Task length_tolength() => ExecutionTestFromFile("length-tolength");
    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "property-descriptor")]
    public Task property_descriptor() => ExecutionTestFromFile("property-descriptor");
    [Fact(DisplayName = "this-value-nullish")]
    public Task this_value_nullish() => ExecutionTestFromFile("this-value-nullish");
}
