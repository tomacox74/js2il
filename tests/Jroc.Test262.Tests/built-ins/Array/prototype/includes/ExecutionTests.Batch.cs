namespace Jroc.Test262.Tests.built_ins.Array.prototype.includes;

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
    [Fact(DisplayName = "return-abrupt-tointeger-fromindex-symbol")]
    public Task return_abrupt_tointeger_fromindex_symbol() => ExecutionTestFromFile("return-abrupt-tointeger-fromindex-symbol");
    [Fact(DisplayName = "return-abrupt-tonumber-length-symbol")]
    public Task return_abrupt_tonumber_length_symbol() => ExecutionTestFromFile("return-abrupt-tonumber-length-symbol");
    [Fact(DisplayName = "this-is-not-object")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");
    [Fact(DisplayName = "tointeger-fromindex")]
    public Task tointeger_fromindex() => ExecutionTestFromFile("tointeger-fromindex");
    [Fact(DisplayName = "tolength-length")]
    public Task tolength_length() => ExecutionTestFromFile("tolength-length");
    [Fact(DisplayName = "values-are-not-cached")]
    public Task values_are_not_cached() => ExecutionTestFromFile("values-are-not-cached");
}
