using Jroc.Test262.Tests.built_ins;



namespace Jroc.Test262.Tests.built_ins.isNaN;



public class ExecutionTests : DiskExecutionTestsBase

{

    public ExecutionTests() : base("built_ins.isNaN") { }



    [Fact(DisplayName = "return-false-not-nan-numbers")]

    public Task return_false_not_nan_numbers()

        => ExecutionTestFromFile("return-false-not-nan-numbers");



    [Fact(DisplayName = "S15.1.2.4_A2.6")]

    public Task S15_1_2_4_A2_6()

        => ExecutionTestFromFile("S15.1.2.4_A2.6");



    [Fact(DisplayName = "tonumber-operations")]

    public Task tonumber_operations()

        => ExecutionTestFromFile("tonumber-operations");



    [Fact(DisplayName = "return-abrupt-from-tonumber-number-symbol")]

    public Task return_abrupt_from_tonumber_number_symbol()

        => ExecutionTestFromFile("return-abrupt-from-tonumber-number-symbol");



    [Fact(DisplayName = "toprimitive-valid-result")]

    public Task toprimitive_valid_result()

        => ExecutionTestFromFile("toprimitive-valid-result");

    [Fact(DisplayName = "return-true-nan")]
    public Task return_true_nan()
        => ExecutionTestFromFile("return-true-nan");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "toprimitive-call-abrupt")]
    public Task toprimitive_call_abrupt()
        => ExecutionTestFromFile("toprimitive-call-abrupt");

    [Fact(DisplayName = "toprimitive-get-abrupt")]
    public Task toprimitive_get_abrupt()
        => ExecutionTestFromFile("toprimitive-get-abrupt");

}
