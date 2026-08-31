namespace Jroc.Test262.Tests.built_ins.String.prototype.at;

public partial class ExecutionTests
{
    [Fact(DisplayName = "index-argument-tointeger.js")]
    public Task index_argument_tointeger() => ExecutionTestFromFile("index-argument-tointeger");
    [Fact(DisplayName = "index-non-numeric-argument-tointeger-invalid.js")]
    public Task index_non_numeric_argument_tointeger_invalid() => ExecutionTestFromFile("index-non-numeric-argument-tointeger-invalid");
    [Fact(DisplayName = "index-non-numeric-argument-tointeger.js")]
    public Task index_non_numeric_argument_tointeger() => ExecutionTestFromFile("index-non-numeric-argument-tointeger");
    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");
    [Fact(DisplayName = "return-abrupt-from-this.js")]
    public Task return_abrupt_from_this() => ExecutionTestFromFile("return-abrupt-from-this");
    [Fact(DisplayName = "returns-code-unit.js")]
    public Task returns_code_unit() => ExecutionTestFromFile("returns-code-unit");
    [Fact(DisplayName = "returns-undefined-for-out-of-range-index.js")]
    public Task returns_undefined_for_out_of_range_index() => ExecutionTestFromFile("returns-undefined-for-out-of-range-index");
}
