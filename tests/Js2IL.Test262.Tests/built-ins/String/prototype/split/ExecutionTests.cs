using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.String.prototype.split;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.split") { }

    [Fact(DisplayName = "argument-is-new-reg-exp-and-instance-is-string-hello", Skip = "String.prototype.split RegExp separator handling is incomplete.")]
    public Task argument_is_new_reg_exp_and_instance_is_string_hello()
        => ExecutionTest("argument-is-new-reg-exp-and-instance-is-string-hello");

    [Fact(DisplayName = "argument-is-null-and-instance-is-function-call-that-returned-string")]
    public Task argument_is_null_and_instance_is_function_call_that_returned_string()
        => ExecutionTest("argument-is-null-and-instance-is-function-call-that-returned-string");

    [Fact(DisplayName = "argument-is-reg-exp-a-z-and-instance-is-string-abc")]
    public Task argument_is_reg_exp_a_z_and_instance_is_string_abc()
        => ExecutionTest("argument-is-reg-exp-a-z-and-instance-is-string-abc");

    [Fact(DisplayName = "argument-is-regexp-a-z-and-instance-is-string-abc")]
    public Task argument_is_regexp_a_z_and_instance_is_string_abc()
        => ExecutionTest("argument-is-regexp-a-z-and-instance-is-string-abc");
}
