using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.preventExtensions;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Reflect.preventExtensions") { }

    [Fact(DisplayName = "always-return-true-from-ordinary-object")]
    public Task always_return_true_from_ordinary_object()
        => ExecutionTestFromFile("always-return-true-from-ordinary-object");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "preventExtensions")]
    public Task preventExtensions()
        => ExecutionTestFromFile("preventExtensions");

    [Fact(DisplayName = "return-abrupt-from-result")]
    public Task return_abrupt_from_result()
        => ExecutionTestFromFile("return-abrupt-from-result");

    [Fact(DisplayName = "return-boolean-from-proxy-object")]
    public Task return_boolean_from_proxy_object()
        => ExecutionTestFromFile("return-boolean-from-proxy-object");

    [Fact(DisplayName = "target-is-not-object-throws")]
    public Task target_is_not_object_throws()
        => ExecutionTestFromFile("target-is-not-object-throws");

    [Fact(DisplayName = "target-is-symbol-throws")]
    public Task target_is_symbol_throws()
        => ExecutionTestFromFile("target-is-symbol-throws");

}
