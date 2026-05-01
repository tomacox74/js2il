using Js2IL.Test262.Tests.built_ins;



namespace Js2IL.Test262.Tests.built_ins.Number.isInteger;



public class ExecutionTests : DiskExecutionTestsBase

{

    public ExecutionTests() : base("built_ins.Number.isInteger") { }



    [Fact(DisplayName = "arg-is-not-number", Skip = "Number.isInteger is not implemented.")]

    public Task arg_is_not_number()

        => ExecutionTestFromFile("arg-is-not-number");



    [Fact(DisplayName = "infinity", Skip = "Number.isInteger is not implemented.")]

    public Task infinity()

        => ExecutionTestFromFile("infinity");



    [Fact(DisplayName = "integers", Skip = "Number.isInteger is not implemented.")]

    public Task integers()

        => ExecutionTestFromFile("integers");



    [Fact(DisplayName = "nan", Skip = "Number.isInteger is not implemented.")]

    public Task nan()

        => ExecutionTestFromFile("nan");



    [Fact(DisplayName = "non-integers", Skip = "Number.isInteger is not implemented.")]

    public Task non_integers()

        => ExecutionTestFromFile("non-integers");

}

