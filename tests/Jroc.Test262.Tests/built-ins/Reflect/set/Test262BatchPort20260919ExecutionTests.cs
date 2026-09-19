using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.set;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Reflect.set") { }

    [Fact(DisplayName = "call-prototype-property-set")]
    public Task call_prototype_property_set()
        => ExecutionTestFromFile("call-prototype-property-set");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "return-abrupt-from-property-key")]
    public Task return_abrupt_from_property_key()
        => ExecutionTestFromFile("return-abrupt-from-property-key");

    [Fact(DisplayName = "return-abrupt-from-result")]
    public Task return_abrupt_from_result()
        => ExecutionTestFromFile("return-abrupt-from-result");

    [Fact(DisplayName = "return-false-if-receiver-is-not-writable")]
    public Task return_false_if_receiver_is_not_writable()
        => ExecutionTestFromFile("return-false-if-receiver-is-not-writable");

    [Fact(DisplayName = "set-value-on-accessor-descriptor")]
    public Task set_value_on_accessor_descriptor()
        => ExecutionTestFromFile("set-value-on-accessor-descriptor");

    [Fact(DisplayName = "set")]
    public Task set()
        => ExecutionTestFromFile("set");

    [Fact(DisplayName = "target-is-not-object-throws")]
    public Task target_is_not_object_throws()
        => ExecutionTestFromFile("target-is-not-object-throws");

    [Fact(DisplayName = "target-is-symbol-throws")]
    public Task target_is_symbol_throws()
        => ExecutionTestFromFile("target-is-symbol-throws");

}
