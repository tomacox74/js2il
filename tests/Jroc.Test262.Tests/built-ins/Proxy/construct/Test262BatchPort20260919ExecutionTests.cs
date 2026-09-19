using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.construct;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Proxy.construct") { }

    [Fact(DisplayName = "call-parameters-new-target")]
    public Task call_parameters_new_target()
        => ExecutionTestFromFile("call-parameters-new-target");

    [Fact(DisplayName = "call-result")]
    public Task call_result()
        => ExecutionTestFromFile("call-result");

    [Fact(DisplayName = "null-handler")]
    public Task null_handler()
        => ExecutionTestFromFile("null-handler");

    [Fact(DisplayName = "return-is-abrupt")]
    public Task return_is_abrupt()
        => ExecutionTestFromFile("return-is-abrupt");

    [Fact(DisplayName = "return-not-object-throws-boolean")]
    public Task return_not_object_throws_boolean()
        => ExecutionTestFromFile("return-not-object-throws-boolean");

    [Fact(DisplayName = "return-not-object-throws-null")]
    public Task return_not_object_throws_null()
        => ExecutionTestFromFile("return-not-object-throws-null");

    [Fact(DisplayName = "return-not-object-throws-number")]
    public Task return_not_object_throws_number()
        => ExecutionTestFromFile("return-not-object-throws-number");

    [Fact(DisplayName = "return-not-object-throws-string")]
    public Task return_not_object_throws_string()
        => ExecutionTestFromFile("return-not-object-throws-string");

    [Fact(DisplayName = "return-not-object-throws-symbol")]
    public Task return_not_object_throws_symbol()
        => ExecutionTestFromFile("return-not-object-throws-symbol");

    [Fact(DisplayName = "return-not-object-throws-undefined")]
    public Task return_not_object_throws_undefined()
        => ExecutionTestFromFile("return-not-object-throws-undefined");

    [Fact(DisplayName = "trap-is-not-callable")]
    public Task trap_is_not_callable()
        => ExecutionTestFromFile("trap-is-not-callable");

    [Fact(DisplayName = "trap-is-null")]
    public Task trap_is_null()
        => ExecutionTestFromFile("trap-is-null");

    [Fact(DisplayName = "trap-is-undefined-no-property")]
    public Task trap_is_undefined_no_property()
        => ExecutionTestFromFile("trap-is-undefined-no-property");

    [Fact(DisplayName = "trap-is-undefined")]
    public Task trap_is_undefined()
        => ExecutionTestFromFile("trap-is-undefined");

}
