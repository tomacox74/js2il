namespace Jroc.Test262.Tests.built_ins.String.prototype.Symbol.iterator;

public partial class ExecutionTests
{
    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");
    [Fact(DisplayName = "this-val-non-obj-coercible.js")]
    public Task this_val_non_obj_coercible() => ExecutionTestFromFile("this-val-non-obj-coercible");
    [Fact(DisplayName = "this-val-to-str-err.js")]
    public Task this_val_to_str_err() => ExecutionTestFromFile("this-val-to-str-err");
}
