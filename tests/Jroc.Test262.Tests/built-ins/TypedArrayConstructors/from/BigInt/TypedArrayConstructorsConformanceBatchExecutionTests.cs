using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.from.BigInt;

public class TypedArrayConstructorsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConstructorsConformanceBatchExecutionTests() : base("built_ins.TypedArrayConstructors.from.BigInt") { }

    [Fact(DisplayName = "arylk-get-length-error.js")]
    public Task arylk_get_length_error() => ExecutionTestFromFile("arylk-get-length-error");

    [Fact(DisplayName = "arylk-to-length-error.js")]
    public Task arylk_to_length_error() => ExecutionTestFromFile("arylk-to-length-error");

    [Fact(DisplayName = "custom-ctor-returns-smaller-instance-throws.js")]
    public Task custom_ctor_returns_smaller_instance_throws() => ExecutionTestFromFile("custom-ctor-returns-smaller-instance-throws");

    [Fact(DisplayName = "custom-ctor.js")]
    public Task custom_ctor() => ExecutionTestFromFile("custom-ctor");

    [Fact(DisplayName = "inherited.js")]
    public Task inherited() => ExecutionTestFromFile("inherited");

    [Fact(DisplayName = "invoked-as-func.js")]
    public Task invoked_as_func() => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "iter-access-error.js")]
    public Task iter_access_error() => ExecutionTestFromFile("iter-access-error");

    [Fact(DisplayName = "iter-invoke-error.js")]
    public Task iter_invoke_error() => ExecutionTestFromFile("iter-invoke-error");

    [Fact(DisplayName = "iter-next-error.js")]
    public Task iter_next_error() => ExecutionTestFromFile("iter-next-error");

    [Fact(DisplayName = "iter-next-value-error.js")]
    public Task iter_next_value_error() => ExecutionTestFromFile("iter-next-value-error");

    [Fact(DisplayName = "mapfn-abrupt-completion.js")]
    public Task mapfn_abrupt_completion() => ExecutionTestFromFile("mapfn-abrupt-completion");

    [Fact(DisplayName = "new-instance-empty.js")]
    public Task new_instance_empty() => ExecutionTestFromFile("new-instance-empty");

    [Fact(DisplayName = "new-instance-from-sparse-array.js")]
    public Task new_instance_from_sparse_array() => ExecutionTestFromFile("new-instance-from-sparse-array");

    [Fact(DisplayName = "property-abrupt-completion.js")]
    public Task property_abrupt_completion() => ExecutionTestFromFile("property-abrupt-completion");

    [Fact(DisplayName = "source-value-is-symbol-throws.js")]
    public Task source_value_is_symbol_throws() => ExecutionTestFromFile("source-value-is-symbol-throws");

    [Fact(DisplayName = "this-is-not-constructor.js")]
    public Task this_is_not_constructor() => ExecutionTestFromFile("this-is-not-constructor");

}
