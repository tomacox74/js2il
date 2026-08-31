namespace Jroc.Test262.Tests.built_ins.Array.prototype.fill;

public partial class ExecutionTests
{
    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");
    [Fact(DisplayName = "return-abrupt-from-setting-property-value")]
    public Task return_abrupt_from_setting_property_value() => ExecutionTestFromFile("return-abrupt-from-setting-property-value");
    [Fact(DisplayName = "return-abrupt-from-this")]
    public Task return_abrupt_from_this() => ExecutionTestFromFile("return-abrupt-from-this");
    [Fact(DisplayName = "return-abrupt-from-this-length-as-symbol")]
    public Task return_abrupt_from_this_length_as_symbol() => ExecutionTestFromFile("return-abrupt-from-this-length-as-symbol");
    [Fact(DisplayName = "return-abrupt-from-this-length")]
    public Task return_abrupt_from_this_length() => ExecutionTestFromFile("return-abrupt-from-this-length");
    [Fact(DisplayName = "return-this")]
    public Task return_this() => ExecutionTestFromFile("return-this");
}
