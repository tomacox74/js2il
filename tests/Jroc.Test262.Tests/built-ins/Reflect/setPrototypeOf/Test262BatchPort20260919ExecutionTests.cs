using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.setPrototypeOf;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Reflect.setPrototypeOf") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "proto-is-symbol-throws")]
    public Task proto_is_symbol_throws()
        => ExecutionTestFromFile("proto-is-symbol-throws");

    [Fact(DisplayName = "return-abrupt-from-result")]
    public Task return_abrupt_from_result()
        => ExecutionTestFromFile("return-abrupt-from-result");

    [Fact(DisplayName = "return-false-if-target-and-proto-are-the-same")]
    public Task return_false_if_target_and_proto_are_the_same()
        => ExecutionTestFromFile("return-false-if-target-and-proto-are-the-same");

    [Fact(DisplayName = "return-false-if-target-is-not-extensible")]
    public Task return_false_if_target_is_not_extensible()
        => ExecutionTestFromFile("return-false-if-target-is-not-extensible");

    [Fact(DisplayName = "return-false-if-target-is-prototype-of-proto")]
    public Task return_false_if_target_is_prototype_of_proto()
        => ExecutionTestFromFile("return-false-if-target-is-prototype-of-proto");

    [Fact(DisplayName = "return-true-if-proto-is-current")]
    public Task return_true_if_proto_is_current()
        => ExecutionTestFromFile("return-true-if-proto-is-current");

    [Fact(DisplayName = "setPrototypeOf")]
    public Task setPrototypeOf()
        => ExecutionTestFromFile("setPrototypeOf");

    [Fact(DisplayName = "target-is-not-object-throws")]
    public Task target_is_not_object_throws()
        => ExecutionTestFromFile("target-is-not-object-throws");

    [Fact(DisplayName = "target-is-symbol-throws")]
    public Task target_is_symbol_throws()
        => ExecutionTestFromFile("target-is-symbol-throws");

}
