using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors_bigint.no_args;

public class TypedArrayConstructorsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConstructorsConformanceBatchExecutionTests() : base("built_ins.TypedArrayConstructors.ctors_bigint.no_args") { }

    [Fact(DisplayName = "custom-proto-access-throws.js")]
    public Task custom_proto_access_throws() => ExecutionTestFromFile("custom-proto-access-throws");

    [Fact(DisplayName = "new-instance-extensibility.js")]
    public Task new_instance_extensibility() => ExecutionTestFromFile("new-instance-extensibility");

    [Fact(DisplayName = "returns-object.js")]
    public Task returns_object() => ExecutionTestFromFile("returns-object");

    [Fact(DisplayName = "use-custom-proto-if-object.js")]
    public Task use_custom_proto_if_object() => ExecutionTestFromFile("use-custom-proto-if-object");

    [Fact(DisplayName = "use-default-proto-if-custom-proto-is-not-object.js")]
    public Task use_default_proto_if_custom_proto_is_not_object() => ExecutionTestFromFile("use-default-proto-if-custom-proto-is-not-object");

}
