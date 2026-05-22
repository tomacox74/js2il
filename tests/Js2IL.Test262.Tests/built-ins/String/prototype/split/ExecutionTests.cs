using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.String.prototype.split;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.split") { }

    [Fact(DisplayName = "argument-is-new-reg-exp-and-instance-is-string-hello")]
    public Task argument_is_new_reg_exp_and_instance_is_string_hello()
        => ExecutionTestFromFile("argument-is-new-reg-exp-and-instance-is-string-hello");

    [Fact(DisplayName = "argument-is-null-and-instance-is-function-call-that-returned-string")]
    public Task argument_is_null_and_instance_is_function_call_that_returned_string()
        => ExecutionTestFromFile("argument-is-null-and-instance-is-function-call-that-returned-string");

    [Fact(DisplayName = "argument-is-reg-exp-a-z-and-instance-is-string-abc")]
    public Task argument_is_reg_exp_a_z_and_instance_is_string_abc()
        => ExecutionTestFromFile("argument-is-reg-exp-a-z-and-instance-is-string-abc");

    [Fact(DisplayName = "argument-is-regexp-a-z-and-instance-is-string-abc")]
    public Task argument_is_regexp_a_z_and_instance_is_string_abc()
        => ExecutionTestFromFile("argument-is-regexp-a-z-and-instance-is-string-abc");

    [Fact(DisplayName = "argument-is-regexp-and-instance-is-number")]
    public Task argument_is_regexp_and_instance_is_number()
        => ExecutionTestFromFile("argument-is-regexp-and-instance-is-number");

    [Fact(DisplayName = "argument-is-regexp-d-and-instance-is-string-dfe23iu-34-65")]
    public Task argument_is_regexp_d_and_instance_is_string_dfe23iu_34_65()
        => ExecutionTestFromFile("argument-is-regexp-d-and-instance-is-string-dfe23iu-34-65");

    [Fact(DisplayName = "argument-is-regexp-l-and-instance-is-string-hello")]
    public Task argument_is_regexp_l_and_instance_is_string_hello()
        => ExecutionTestFromFile("argument-is-regexp-l-and-instance-is-string-hello");

    [Fact(DisplayName = "argument-is-regexp-reg-exp-d-and-instance-is-string-dfe23iu-34-65")]
    public Task argument_is_regexp_reg_exp_d_and_instance_is_string_dfe23iu_34_65()
        => ExecutionTestFromFile("argument-is-regexp-reg-exp-d-and-instance-is-string-dfe23iu-34-65");

    [Fact(DisplayName = "argument-is-regexp-s-and-instance-is-string-a-b-c-de-f")]
    public Task argument_is_regexp_s_and_instance_is_string_a_b_c_de_f()
        => ExecutionTestFromFile("argument-is-regexp-s-and-instance-is-string-a-b-c-de-f");

    [Fact(DisplayName = "argument-is-regexp-x-and-instance-is-string-a-b-c-de-f")]
    public Task argument_is_regexp_x_and_instance_is_string_a_b_c_de_f()
        => ExecutionTestFromFile("argument-is-regexp-x-and-instance-is-string-a-b-c-de-f");

    [Fact(DisplayName = "argument-is-undefined-and-instance-is-string")]
    public Task argument_is_undefined_and_instance_is_string()
        => ExecutionTestFromFile("argument-is-undefined-and-instance-is-string");

    [Fact(DisplayName = "argument-is-void-0-and-instance-is-string-object-object-have-overrided-to-string-function")]
    public Task argument_is_void_0_and_instance_is_string_object_object_have_overrided_to_string_function()
        => ExecutionTestFromFile("argument-is-void-0-and-instance-is-string-object-object-have-overrided-to-string-function");

    [Fact(DisplayName = "arguments-are-boolean-expression-function-call-and-null-and-instance-is-boolean")]
    public Task arguments_are_boolean_expression_function_call_and_null_and_instance_is_boolean()
        => ExecutionTestFromFile("arguments-are-boolean-expression-function-call-and-null-and-instance-is-boolean");

    [Fact(DisplayName = "arguments-are-false-and-true-and-instance-is-object")]
    public Task arguments_are_false_and_true_and_instance_is_object()
        => ExecutionTestFromFile("arguments-are-false-and-true-and-instance-is-object");

    [Fact(DisplayName = "arguments-are-new-reg-exp-and-0-and-instance-is-string-hello")]
    public Task arguments_are_new_reg_exp_and_0_and_instance_is_string_hello()
        => ExecutionTestFromFile("arguments-are-new-reg-exp-and-0-and-instance-is-string-hello");

    [Fact(DisplayName = "arguments-are-new-reg-exp-and-1-and-instance-is-string-hello")]
    public Task arguments_are_new_reg_exp_and_1_and_instance_is_string_hello()
        => ExecutionTestFromFile("arguments-are-new-reg-exp-and-1-and-instance-is-string-hello");
}
