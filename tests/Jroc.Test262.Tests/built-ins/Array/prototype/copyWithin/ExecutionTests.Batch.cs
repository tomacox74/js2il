namespace Jroc.Test262.Tests.built_ins.Array.prototype.copyWithin;

public partial class ExecutionTests
{
    [Fact(DisplayName = "coerced-values-start")]
    public Task coerced_values_start() => ExecutionTestFromFile("coerced-values-start");
    [Fact(DisplayName = "coerced-values-target")]
    public Task coerced_values_target() => ExecutionTestFromFile("coerced-values-target");
    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");
    [Fact(DisplayName = "return-abrupt-from-delete-proxy-target")]
    public Task return_abrupt_from_delete_proxy_target() => ExecutionTestFromFile("return-abrupt-from-delete-proxy-target");
    [Fact(DisplayName = "return-abrupt-from-delete-target")]
    public Task return_abrupt_from_delete_target() => ExecutionTestFromFile("return-abrupt-from-delete-target");
    [Fact(DisplayName = "return-abrupt-from-get-start-value")]
    public Task return_abrupt_from_get_start_value() => ExecutionTestFromFile("return-abrupt-from-get-start-value");
    [Fact(DisplayName = "return-abrupt-from-has-start")]
    public Task return_abrupt_from_has_start() => ExecutionTestFromFile("return-abrupt-from-has-start");
    [Fact(DisplayName = "return-abrupt-from-set-target-value")]
    public Task return_abrupt_from_set_target_value() => ExecutionTestFromFile("return-abrupt-from-set-target-value");
    [Fact(DisplayName = "return-abrupt-from-this")]
    public Task return_abrupt_from_this() => ExecutionTestFromFile("return-abrupt-from-this");
    [Fact(DisplayName = "return-abrupt-from-this-length-as-symbol")]
    public Task return_abrupt_from_this_length_as_symbol() => ExecutionTestFromFile("return-abrupt-from-this-length-as-symbol");
    [Fact(DisplayName = "return-abrupt-from-this-length")]
    public Task return_abrupt_from_this_length() => ExecutionTestFromFile("return-abrupt-from-this-length");
    [Fact(DisplayName = "return-this")]
    public Task return_this() => ExecutionTestFromFile("return-this");
}
