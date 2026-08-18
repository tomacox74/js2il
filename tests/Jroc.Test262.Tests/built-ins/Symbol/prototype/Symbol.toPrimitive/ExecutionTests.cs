using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol.prototype.Symbol.toPrimitive;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Symbol.prototype.Symbol.toPrimitive") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "redefined-symbol-wrapper-ordinary-toprimitive")]
    public Task redefined_symbol_wrapper_ordinary_toprimitive()
        => ExecutionTestFromFile("redefined-symbol-wrapper-ordinary-toprimitive");

    [Fact(DisplayName = "removed-symbol-wrapper-ordinary-toprimitive")]
    public Task removed_symbol_wrapper_ordinary_toprimitive()
        => ExecutionTestFromFile("removed-symbol-wrapper-ordinary-toprimitive");

    [Fact(DisplayName = "this-val-non-obj")]
    public Task this_val_non_obj()
        => ExecutionTestFromFile("this-val-non-obj");

    [Fact(DisplayName = "this-val-obj-symbol-wrapper")]
    public Task this_val_obj_symbol_wrapper()
        => ExecutionTestFromFile("this-val-obj-symbol-wrapper");
}
