using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.GeneratorFunction;

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

    [Fact(DisplayName = "instance-construct-throws")]
    public Task instance_construct_throws()
        => ExecutionTestFromFile("instance-construct-throws");

    [Fact(DisplayName = "instance-length")]
    public Task instance_length()
        => ExecutionTestFromFile("instance-length");

    [Fact(DisplayName = "instance-name")]
    public Task instance_name()
        => ExecutionTestFromFile("instance-name");

    [Fact(DisplayName = "instance-restricted-properties")]
    public Task instance_restricted_properties()
        => ExecutionTestFromFile("instance-restricted-properties");

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor()
        => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");
}
