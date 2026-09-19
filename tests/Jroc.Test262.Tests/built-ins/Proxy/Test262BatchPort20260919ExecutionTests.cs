using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Proxy") { }

    [Fact(DisplayName = "create-handler-not-object-throw-boolean")]
    public Task create_handler_not_object_throw_boolean()
        => ExecutionTestFromFile("create-handler-not-object-throw-boolean");

    [Fact(DisplayName = "create-handler-not-object-throw-null")]
    public Task create_handler_not_object_throw_null()
        => ExecutionTestFromFile("create-handler-not-object-throw-null");

    [Fact(DisplayName = "create-handler-not-object-throw-number")]
    public Task create_handler_not_object_throw_number()
        => ExecutionTestFromFile("create-handler-not-object-throw-number");

    [Fact(DisplayName = "create-handler-not-object-throw-string")]
    public Task create_handler_not_object_throw_string()
        => ExecutionTestFromFile("create-handler-not-object-throw-string");

    [Fact(DisplayName = "create-handler-not-object-throw-symbol")]
    public Task create_handler_not_object_throw_symbol()
        => ExecutionTestFromFile("create-handler-not-object-throw-symbol");

    [Fact(DisplayName = "create-handler-not-object-throw-undefined")]
    public Task create_handler_not_object_throw_undefined()
        => ExecutionTestFromFile("create-handler-not-object-throw-undefined");

    [Fact(DisplayName = "create-target-is-not-callable")]
    public Task create_target_is_not_callable()
        => ExecutionTestFromFile("create-target-is-not-callable");

    [Fact(DisplayName = "create-target-not-object-throw-boolean")]
    public Task create_target_not_object_throw_boolean()
        => ExecutionTestFromFile("create-target-not-object-throw-boolean");

    [Fact(DisplayName = "create-target-not-object-throw-null")]
    public Task create_target_not_object_throw_null()
        => ExecutionTestFromFile("create-target-not-object-throw-null");

    [Fact(DisplayName = "create-target-not-object-throw-number")]
    public Task create_target_not_object_throw_number()
        => ExecutionTestFromFile("create-target-not-object-throw-number");

    [Fact(DisplayName = "create-target-not-object-throw-string")]
    public Task create_target_not_object_throw_string()
        => ExecutionTestFromFile("create-target-not-object-throw-string");

    [Fact(DisplayName = "create-target-not-object-throw-symbol")]
    public Task create_target_not_object_throw_symbol()
        => ExecutionTestFromFile("create-target-not-object-throw-symbol");

    [Fact(DisplayName = "create-target-not-object-throw-undefined")]
    public Task create_target_not_object_throw_undefined()
        => ExecutionTestFromFile("create-target-not-object-throw-undefined");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

}
