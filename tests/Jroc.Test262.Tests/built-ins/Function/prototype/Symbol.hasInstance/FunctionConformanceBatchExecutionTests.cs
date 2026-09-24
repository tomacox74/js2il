using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.prototype.Symbol.hasInstance;

public class FunctionConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public FunctionConformanceBatchExecutionTests() : base("built_ins.Function.prototype.Symbol.hasInstance") { }

    [Fact(DisplayName = "this-val-prototype-non-obj.js")]
    public Task this_val_prototype_non_obj() => ExecutionTestFromFile("this-val-prototype-non-obj");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "this-val-bound-target.js")]
    public Task this_val_bound_target() => ExecutionTestFromFile("this-val-bound-target");

    [Fact(DisplayName = "this-val-not-callable.js")]
    public Task this_val_not_callable() => ExecutionTestFromFile("this-val-not-callable");

    [Fact(DisplayName = "this-val-poisoned-prototype.js")]
    public Task this_val_poisoned_prototype() => ExecutionTestFromFile("this-val-poisoned-prototype");

    [Fact(DisplayName = "value-get-prototype-of-err.js")]
    public Task value_get_prototype_of_err() => ExecutionTestFromFile("value-get-prototype-of-err");

    [Fact(DisplayName = "value-negative.js")]
    public Task value_negative() => ExecutionTestFromFile("value-negative");

    [Fact(DisplayName = "value-non-obj.js")]
    public Task value_non_obj() => ExecutionTestFromFile("value-non-obj");

    [Fact(DisplayName = "value-positive.js")]
    public Task value_positive() => ExecutionTestFromFile("value-positive");

}
