using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.set;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Reflect.set") { }

    [Fact(DisplayName = "creates-a-data-descriptor")]
    public Task creates_a_data_descriptor()
        => ExecutionTestFromFile("creates-a-data-descriptor");

    [Fact(DisplayName = "different-property-descriptors")]
    public Task different_property_descriptors()
        => ExecutionTestFromFile("different-property-descriptors");

    [Fact(DisplayName = "receiver-is-not-object")]
    public Task receiver_is_not_object()
        => ExecutionTestFromFile("receiver-is-not-object");

    [Fact(DisplayName = "return-false-if-target-is-not-writable")]
    public Task return_false_if_target_is_not_writable()
        => ExecutionTestFromFile("return-false-if-target-is-not-writable");

    [Fact(DisplayName = "set-value-on-accessor-descriptor-with-receiver")]
    public Task set_value_on_accessor_descriptor_with_receiver()
        => ExecutionTestFromFile("set-value-on-accessor-descriptor-with-receiver");

    [Fact(DisplayName = "set-value-on-data-descriptor")]
    public Task set_value_on_data_descriptor()
        => ExecutionTestFromFile("set-value-on-data-descriptor");

    [Fact(DisplayName = "symbol-property")]
    public Task symbol_property()
        => ExecutionTestFromFile("symbol-property");
}
