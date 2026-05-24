using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.GeneratorFunction;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.GeneratorFunction") { }

    [Fact(DisplayName = "GeneratorFunction_length")]
    public Task GeneratorFunction_length()
        => ExecutionTestFromFile("GeneratorFunction_length");

    [Fact(DisplayName = "extensibility")]
    public Task extensibility()
        => ExecutionTestFromFile("extensibility");

    [Fact(DisplayName = "has-instance")]
    public Task has_instance()
        => ExecutionTestFromFile("has-instance");

    [Fact(DisplayName = "invoked-as-constructor-no-arguments")]
    public Task invoked_as_constructor_no_arguments()
        => ExecutionTestFromFile("invoked-as-constructor-no-arguments");

    [Fact(DisplayName = "invoked-as-function-multiple-arguments")]
    public Task invoked_as_function_multiple_arguments()
        => ExecutionTestFromFile("invoked-as-function-multiple-arguments");

    [Fact(DisplayName = "invoked-as-function-no-arguments")]
    public Task invoked_as_function_no_arguments()
        => ExecutionTestFromFile("invoked-as-function-no-arguments");

    [Fact(DisplayName = "invoked-as-function-single-argument")]
    public Task invoked_as_function_single_argument()
        => ExecutionTestFromFile("invoked-as-function-single-argument");
}
