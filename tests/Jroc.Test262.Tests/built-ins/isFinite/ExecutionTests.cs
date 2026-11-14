using Jroc.Test262.Tests.built_ins;



namespace Jroc.Test262.Tests.built_ins.isFinite;



public class ExecutionTests : InMemoryExecutionTestsBase

{

    public ExecutionTests() : base("built_ins.isFinite") { }



    [Fact(DisplayName = "return-false-on-nan-or-infinities")]

    public Task return_false_on_nan_or_infinities()

        => ExecutionTestFromFile("return-false-on-nan-or-infinities");



    [Fact(DisplayName = "return-true-for-valid-finite-numbers")]

    public Task return_true_for_valid_finite_numbers()

        => ExecutionTestFromFile("return-true-for-valid-finite-numbers");



    [Fact(DisplayName = "S15.1.2.5_A2.6")]

    public Task S15_1_2_5_A2_6()

        => ExecutionTestFromFile("S15.1.2.5_A2.6");



    [Fact(DisplayName = "tonumber-operations")]

    public Task tonumber_operations()

        => ExecutionTestFromFile("tonumber-operations");



    [Fact(DisplayName = "toprimitive-valid-result")]

    public Task toprimitive_valid_result()

        => ExecutionTestFromFile("toprimitive-valid-result");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "return-abrupt-from-tonumber-number")]
    public Task return_abrupt_from_tonumber_number()
        => ExecutionTestFromFile("return-abrupt-from-tonumber-number");

    [Fact(DisplayName = "toprimitive-call-abrupt")]
    public Task toprimitive_call_abrupt()
        => ExecutionTestFromFile("toprimitive-call-abrupt");

    [Fact(DisplayName = "toprimitive-get-abrupt")]
    public Task toprimitive_get_abrupt()
        => ExecutionTestFromFile("toprimitive-get-abrupt");


    [Fact(DisplayName = "prop-desc.js")]
    public Task Port400_prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "return-abrupt-from-tonumber-number-symbol.js")]
    public Task Port400_return_abrupt_from_tonumber_number_symbol()
        => ExecutionTestFromFile("return-abrupt-from-tonumber-number-symbol");

    [Fact(DisplayName = "S15.1.2.5_A2.7.js")]
    public Task Port400_S15_1_2_5_A2_7()
        => ExecutionTestFromFile("S15.1.2.5_A2.7");

    [Fact(DisplayName = "toprimitive-not-callable-throws.js")]
    public Task Port400_toprimitive_not_callable_throws()
        => ExecutionTestFromFile("toprimitive-not-callable-throws");

    [Fact(DisplayName = "toprimitive-result-is-object-throws.js")]
    public Task Port400_toprimitive_result_is_object_throws()
        => ExecutionTestFromFile("toprimitive-result-is-object-throws");

    [Fact(DisplayName = "toprimitive-result-is-symbol-throws.js")]
    public Task Port400_toprimitive_result_is_symbol_throws()
        => ExecutionTestFromFile("toprimitive-result-is-symbol-throws");
}
