using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors.typedarray_arg;

public class TypedArrayConstructorsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConstructorsConformanceBatchExecutionTests() : base("built_ins.TypedArrayConstructors.ctors.typedarray_arg") { }

    [Fact(DisplayName = "custom-proto-access-throws.js")]
    public Task custom_proto_access_throws() => ExecutionTestFromFile("custom-proto-access-throws");

    [Fact(DisplayName = "new-instance-extensibility.js")]
    public Task new_instance_extensibility() => ExecutionTestFromFile("new-instance-extensibility");

    [Fact(DisplayName = "other-ctor-returns-new-typedarray.js")]
    public Task other_ctor_returns_new_typedarray() => ExecutionTestFromFile("other-ctor-returns-new-typedarray");

    [Fact(DisplayName = "returns-new-instance.js")]
    public Task returns_new_instance() => ExecutionTestFromFile("returns-new-instance");

    [Fact(DisplayName = "same-ctor-buffer-ctor-species-null.js")]
    public Task same_ctor_buffer_ctor_species_null() => ExecutionTestFromFile("same-ctor-buffer-ctor-species-null");

    [Fact(DisplayName = "same-ctor-buffer-ctor-species-undefined.js")]
    public Task same_ctor_buffer_ctor_species_undefined() => ExecutionTestFromFile("same-ctor-buffer-ctor-species-undefined");

    [Fact(DisplayName = "same-ctor-returns-new-cloned-typedarray.js")]
    public Task same_ctor_returns_new_cloned_typedarray() => ExecutionTestFromFile("same-ctor-returns-new-cloned-typedarray");

    [Fact(DisplayName = "src-typedarray-big-throws.js")]
    public Task src_typedarray_big_throws() => ExecutionTestFromFile("src-typedarray-big-throws");

    [Fact(DisplayName = "use-custom-proto-if-object.js")]
    public Task use_custom_proto_if_object() => ExecutionTestFromFile("use-custom-proto-if-object");

    [Fact(DisplayName = "use-default-proto-if-custom-proto-is-not-object.js")]
    public Task use_default_proto_if_custom_proto_is_not_object() => ExecutionTestFromFile("use-default-proto-if-custom-proto-is-not-object");

}
