using Js2IL.Test262.Tests.built_ins;



namespace Js2IL.Test262.Tests.built_ins.isFinite;



public class ExecutionTests : DiskExecutionTestsBase

{

    public ExecutionTests() : base("built_ins.isFinite") { }



    [Fact(DisplayName = "return-false-on-nan-or-infinities")]

    public Task return_false_on_nan_or_infinities()

        => ExecutionTestFromFile("return-false-on-nan-or-infinities");



    [Fact(DisplayName = "return-true-for-valid-finite-numbers")]

    public Task return_true_for_valid_finite_numbers()

        => ExecutionTestFromFile("return-true-for-valid-finite-numbers");



    [Fact(DisplayName = "S15.1.2.5_A2.6", Skip = "isFinite.prototype is not undefined.")]

    public Task S15_1_2_5_A2_6()

        => ExecutionTestFromFile("S15.1.2.5_A2.6");



    [Fact(DisplayName = "tonumber-operations", Skip = "isFinite ToNumber coercion is incorrect.")]

    public Task tonumber_operations()

        => ExecutionTestFromFile("tonumber-operations");



    [Fact(DisplayName = "toprimitive-valid-result")]

    public Task toprimitive_valid_result()

        => ExecutionTestFromFile("toprimitive-valid-result");

}

