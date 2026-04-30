using Js2IL.Test262.Tests.built_ins;



namespace Js2IL.Test262.Tests.built_ins.isNaN;



public class ExecutionTests : DiskExecutionTestsBase

{

    public ExecutionTests() : base("built_ins.isNaN") { }



    [Fact(DisplayName = "return-false-not-nan-numbers", Skip = "Global isNaN is not yet supported.")]

    public Task return_false_not_nan_numbers()

        => ExecutionTestFromFile("return-false-not-nan-numbers");



    [Fact(DisplayName = "S15.1.2.4_A2.6", Skip = "Global isNaN is not yet supported.")]

    public Task S15_1_2_4_A2_6()

        => ExecutionTestFromFile("S15.1.2.4_A2.6");



    [Fact(DisplayName = "tonumber-operations", Skip = "Global isNaN is not yet supported.")]

    public Task tonumber_operations()

        => ExecutionTestFromFile("tonumber-operations");



    [Fact(DisplayName = "return-abrupt-from-tonumber-number-symbol", Skip = "Global isNaN is not yet supported.")]

    public Task return_abrupt_from_tonumber_number_symbol()

        => ExecutionTestFromFile("return-abrupt-from-tonumber-number-symbol");



    [Fact(DisplayName = "toprimitive-valid-result", Skip = "Global isNaN is not yet supported.")]

    public Task toprimitive_valid_result()

        => ExecutionTestFromFile("toprimitive-valid-result");

}

