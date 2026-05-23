using Js2IL.Test262.Tests.built_ins;



namespace Js2IL.Test262.Tests.built_ins.Number.isInteger;



public class ExecutionTests : DiskExecutionTestsBase

{

    public ExecutionTests() : base("built_ins.Number.isInteger") { }



    [Fact(DisplayName = "arg-is-not-number")]

    public Task arg_is_not_number()

        => ExecutionTestFromFile("arg-is-not-number");



    [Fact(DisplayName = "infinity")]

    public Task infinity()

        => ExecutionTestFromFile("infinity");



    [Fact(DisplayName = "integers")]

    public Task integers()

        => ExecutionTestFromFile("integers");



    [Fact(DisplayName = "nan")]

    public Task nan()

        => ExecutionTestFromFile("nan");



    [Fact(DisplayName = "non-integers")]

    public Task non_integers()

        => ExecutionTestFromFile("non-integers");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}

