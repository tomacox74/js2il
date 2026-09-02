using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.copyWithin;

public class TypedArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.copyWithin") { }

    [Fact(DisplayName = "return-abrupt-from-end-is-symbol.js")]
    public Task return_abrupt_from_end_is_symbol() => ExecutionTestFromFile("return-abrupt-from-end-is-symbol");

    [Fact(DisplayName = "return-abrupt-from-end.js")]
    public Task return_abrupt_from_end() => ExecutionTestFromFile("return-abrupt-from-end");

    [Fact(DisplayName = "return-abrupt-from-start-is-symbol.js")]
    public Task return_abrupt_from_start_is_symbol() => ExecutionTestFromFile("return-abrupt-from-start-is-symbol");

    [Fact(DisplayName = "return-abrupt-from-start.js")]
    public Task return_abrupt_from_start() => ExecutionTestFromFile("return-abrupt-from-start");

    [Fact(DisplayName = "return-abrupt-from-target-is-symbol.js")]
    public Task return_abrupt_from_target_is_symbol() => ExecutionTestFromFile("return-abrupt-from-target-is-symbol");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

    [Fact(DisplayName = "this-is-not-typedarray-instance.js")]
    public Task this_is_not_typedarray_instance() => ExecutionTestFromFile("this-is-not-typedarray-instance");

}
