namespace Jroc.Test262.Tests.built_ins.String.prototype.normalize;

public partial class ExecutionTests
{
    [Fact(DisplayName = "return-abrupt-from-this-as-symbol.js")]
    public Task return_abrupt_from_this_as_symbol() => ExecutionTestFromFile("return-abrupt-from-this-as-symbol");
    [Fact(DisplayName = "return-abrupt-from-this.js")]
    public Task return_abrupt_from_this() => ExecutionTestFromFile("return-abrupt-from-this");
    [Fact(DisplayName = "this-is-null-throws.js")]
    public Task this_is_null_throws() => ExecutionTestFromFile("this-is-null-throws");
    [Fact(DisplayName = "this-is-undefined-throws.js")]
    public Task this_is_undefined_throws() => ExecutionTestFromFile("this-is-undefined-throws");
}
