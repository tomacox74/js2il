using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.getPrototypeOf;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Proxy.getPrototypeOf") { }

    [Fact(DisplayName = "call-parameters")]
    public Task call_parameters()
        => ExecutionTestFromFile("call-parameters");

    [Fact(DisplayName = "extensible-target-return-handlerproto")]
    public Task extensible_target_return_handlerproto()
        => ExecutionTestFromFile("extensible-target-return-handlerproto");

    [Fact(DisplayName = "not-extensible-same-proto")]
    public Task not_extensible_same_proto()
        => ExecutionTestFromFile("not-extensible-same-proto");

    [Fact(DisplayName = "null-handler")]
    public Task null_handler()
        => ExecutionTestFromFile("null-handler");

    [Fact(DisplayName = "return-is-abrupt")]
    public Task return_is_abrupt()
        => ExecutionTestFromFile("return-is-abrupt");

    [Fact(DisplayName = "trap-is-not-callable")]
    public Task trap_is_not_callable()
        => ExecutionTestFromFile("trap-is-not-callable");

    [Fact(DisplayName = "trap-is-undefined")]
    public Task trap_is_undefined()
        => ExecutionTestFromFile("trap-is-undefined");

    [Fact(DisplayName = "trap-result-neither-object-nor-null-throws-boolean")]
    public Task trap_result_neither_object_nor_null_throws_boolean()
        => ExecutionTestFromFile("trap-result-neither-object-nor-null-throws-boolean");

    [Fact(DisplayName = "trap-result-neither-object-nor-null-throws-number")]
    public Task trap_result_neither_object_nor_null_throws_number()
        => ExecutionTestFromFile("trap-result-neither-object-nor-null-throws-number");

    [Fact(DisplayName = "trap-result-neither-object-nor-null-throws-string")]
    public Task trap_result_neither_object_nor_null_throws_string()
        => ExecutionTestFromFile("trap-result-neither-object-nor-null-throws-string");

    [Fact(DisplayName = "trap-result-neither-object-nor-null-throws-symbol")]
    public Task trap_result_neither_object_nor_null_throws_symbol()
        => ExecutionTestFromFile("trap-result-neither-object-nor-null-throws-symbol");

    [Fact(DisplayName = "trap-result-neither-object-nor-null-throws-undefined")]
    public Task trap_result_neither_object_nor_null_throws_undefined()
        => ExecutionTestFromFile("trap-result-neither-object-nor-null-throws-undefined");

}
