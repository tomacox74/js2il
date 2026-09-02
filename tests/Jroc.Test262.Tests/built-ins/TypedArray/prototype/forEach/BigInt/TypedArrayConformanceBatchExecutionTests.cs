using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.forEach.BigInt;

public class TypedArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.forEach.BigInt") { }

    [Fact(DisplayName = "arraylength-internal.js")]
    public Task arraylength_internal() => ExecutionTestFromFile("arraylength-internal");

    [Fact(DisplayName = "callbackfn-arguments-with-thisarg.js")]
    public Task callbackfn_arguments_with_thisarg() => ExecutionTestFromFile("callbackfn-arguments-with-thisarg");

    [Fact(DisplayName = "callbackfn-arguments-without-thisarg.js")]
    public Task callbackfn_arguments_without_thisarg() => ExecutionTestFromFile("callbackfn-arguments-without-thisarg");

    [Fact(DisplayName = "callbackfn-is-not-callable.js")]
    public Task callbackfn_is_not_callable() => ExecutionTestFromFile("callbackfn-is-not-callable");

    [Fact(DisplayName = "callbackfn-no-interaction-over-non-integer.js")]
    public Task callbackfn_no_interaction_over_non_integer() => ExecutionTestFromFile("callbackfn-no-interaction-over-non-integer");

    [Fact(DisplayName = "callbackfn-not-called-on-empty.js")]
    public Task callbackfn_not_called_on_empty() => ExecutionTestFromFile("callbackfn-not-called-on-empty");

    [Fact(DisplayName = "callbackfn-returns-abrupt.js")]
    public Task callbackfn_returns_abrupt() => ExecutionTestFromFile("callbackfn-returns-abrupt");

    [Fact(DisplayName = "callbackfn-set-value-during-interaction.js")]
    public Task callbackfn_set_value_during_interaction() => ExecutionTestFromFile("callbackfn-set-value-during-interaction");

    [Fact(DisplayName = "callbackfn-this.js")]
    public Task callbackfn_this() => ExecutionTestFromFile("callbackfn-this");

    [Fact(DisplayName = "returns-undefined.js")]
    public Task returns_undefined() => ExecutionTestFromFile("returns-undefined");

}
