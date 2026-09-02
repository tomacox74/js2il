using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.filter.BigInt;

public class TypedArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.filter.BigInt") { }

    [Fact(DisplayName = "arraylength-internal.js")]
    public Task arraylength_internal() => ExecutionTestFromFile("arraylength-internal");

    [Fact(DisplayName = "callbackfn-arguments-with-thisarg.js")]
    public Task callbackfn_arguments_with_thisarg() => ExecutionTestFromFile("callbackfn-arguments-with-thisarg");

    [Fact(DisplayName = "callbackfn-arguments-without-thisarg.js")]
    public Task callbackfn_arguments_without_thisarg() => ExecutionTestFromFile("callbackfn-arguments-without-thisarg");

    [Fact(DisplayName = "callbackfn-no-iteration-over-non-integer.js")]
    public Task callbackfn_no_iteration_over_non_integer() => ExecutionTestFromFile("callbackfn-no-iteration-over-non-integer");

    [Fact(DisplayName = "callbackfn-not-callable-throws.js")]
    public Task callbackfn_not_callable_throws() => ExecutionTestFromFile("callbackfn-not-callable-throws");

    [Fact(DisplayName = "callbackfn-not-called-on-empty.js")]
    public Task callbackfn_not_called_on_empty() => ExecutionTestFromFile("callbackfn-not-called-on-empty");

    [Fact(DisplayName = "callbackfn-returns-abrupt.js")]
    public Task callbackfn_returns_abrupt() => ExecutionTestFromFile("callbackfn-returns-abrupt");

    [Fact(DisplayName = "callbackfn-set-value-during-iteration.js")]
    public Task callbackfn_set_value_during_iteration() => ExecutionTestFromFile("callbackfn-set-value-during-iteration");

    [Fact(DisplayName = "callbackfn-this.js")]
    public Task callbackfn_this() => ExecutionTestFromFile("callbackfn-this");

    [Fact(DisplayName = "result-does-not-share-buffer.js")]
    public Task result_does_not_share_buffer() => ExecutionTestFromFile("result-does-not-share-buffer");

    [Fact(DisplayName = "result-empty-callbackfn-returns-false.js")]
    public Task result_empty_callbackfn_returns_false() => ExecutionTestFromFile("result-empty-callbackfn-returns-false");

    [Fact(DisplayName = "result-full-callbackfn-returns-true.js")]
    public Task result_full_callbackfn_returns_true() => ExecutionTestFromFile("result-full-callbackfn-returns-true");

    [Fact(DisplayName = "speciesctor-get-species-use-default-ctor.js")]
    public Task speciesctor_get_species_use_default_ctor() => ExecutionTestFromFile("speciesctor-get-species-use-default-ctor");

    [Fact(DisplayName = "values-are-set.js")]
    public Task values_are_set() => ExecutionTestFromFile("values-are-set");

}
