using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.rest_parameters;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.rest_parameters") { }

    [Fact(DisplayName = "arrow-function")]
    public Task arrow_function()
        => ExecutionTest("arrow-function");

    [Fact(DisplayName = "expected-argument-count")]
    public Task expected_argument_count()
        => ExecutionTest("expected-argument-count");

    [Fact(DisplayName = "no-alias-arguments")]
    public Task no_alias_arguments()
        => ExecutionTest("no-alias-arguments");

    [Fact(DisplayName = "rest-index")]
    public Task rest_index()
        => ExecutionTest("rest-index");

    [Fact(DisplayName = "rest-parameters-apply")]
    public Task rest_parameters_apply()
        => ExecutionTest("rest-parameters-apply");

    [Fact(DisplayName = "rest-parameters-call")]
    public Task rest_parameters_call()
        => ExecutionTest("rest-parameters-call");

    [Fact(DisplayName = "rest-parameters-produce-an-array", Skip = "Product defect: rest parameter array constructor is incorrect at runtime")]
    public Task rest_parameters_produce_an_array()
        => ExecutionTest("rest-parameters-produce-an-array");
}
