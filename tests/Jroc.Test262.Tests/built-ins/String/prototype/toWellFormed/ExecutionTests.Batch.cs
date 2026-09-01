namespace Jroc.Test262.Tests.built_ins.String.prototype.toWellFormed;

public partial class ExecutionTests
{
    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");
    [Fact(DisplayName = "return-abrupt-from-this.js")]
    public Task return_abrupt_from_this() => ExecutionTestFromFile("return-abrupt-from-this");
    [Fact(DisplayName = "returns-well-formed-string.js")]
    public Task returns_well_formed_string() => ExecutionTestFromFile("returns-well-formed-string");
    [Fact(DisplayName = "to-string-primitive.js")]
    public Task to_string_primitive() => ExecutionTestFromFile("to-string-primitive");
    [Fact(DisplayName = "to-string.js")]
    public Task to_string() => ExecutionTestFromFile("to-string");
}
