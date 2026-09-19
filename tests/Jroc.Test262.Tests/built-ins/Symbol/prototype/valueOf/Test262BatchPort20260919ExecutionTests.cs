using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol.prototype.valueOf;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Symbol.prototype.valueOf") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "this-val-non-obj")]
    public Task this_val_non_obj()
        => ExecutionTestFromFile("this-val-non-obj");

    [Fact(DisplayName = "this-val-obj-non-symbol")]
    public Task this_val_obj_non_symbol()
        => ExecutionTestFromFile("this-val-obj-non-symbol");

    [Fact(DisplayName = "this-val-obj-symbol")]
    public Task this_val_obj_symbol()
        => ExecutionTestFromFile("this-val-obj-symbol");

}
