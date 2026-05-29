using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Object.@is;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.is") { }

    [Fact(DisplayName = "not-same-value-x-y-boolean")]
    public Task not_same_value_x_y_boolean()
        => ExecutionTestFromFile("not-same-value-x-y-boolean");

    [Fact(DisplayName = "not-same-value-x-y-null")]
    public Task not_same_value_x_y_null()
        => ExecutionTestFromFile("not-same-value-x-y-null");

    [Fact(DisplayName = "not-same-value-x-y-number")]
    public Task not_same_value_x_y_number()
        => ExecutionTestFromFile("not-same-value-x-y-number");

    [Fact(DisplayName = "not-same-value-x-y-object")]
    public Task not_same_value_x_y_object()
        => ExecutionTestFromFile("not-same-value-x-y-object");

    [Fact(DisplayName = "not-same-value-x-y-string")]
    public Task not_same_value_x_y_string()
        => ExecutionTestFromFile("not-same-value-x-y-string");

    [Fact(DisplayName = "not-same-value-x-y-symbol")]
    public Task not_same_value_x_y_symbol()
        => ExecutionTestFromFile("not-same-value-x-y-symbol");

    [Fact(DisplayName = "not-same-value-x-y-type")]
    public Task not_same_value_x_y_type()
        => ExecutionTestFromFile("not-same-value-x-y-type");

    [Fact(DisplayName = "not-same-value-x-y-undefined")]
    public Task not_same_value_x_y_undefined()
        => ExecutionTestFromFile("not-same-value-x-y-undefined");

    [Fact(DisplayName = "same-value-x-y-boolean")]
    public Task same_value_x_y_boolean()
        => ExecutionTestFromFile("same-value-x-y-boolean");

    [Fact(DisplayName = "same-value-x-y-empty")]
    public Task same_value_x_y_empty()
        => ExecutionTestFromFile("same-value-x-y-empty");

    [Fact(DisplayName = "same-value-x-y-null")]
    public Task same_value_x_y_null()
        => ExecutionTestFromFile("same-value-x-y-null");

    [Fact(DisplayName = "same-value-x-y-number")]
    public Task same_value_x_y_number()
        => ExecutionTestFromFile("same-value-x-y-number");

    [Fact(DisplayName = "same-value-x-y-object")]
    public Task same_value_x_y_object()
        => ExecutionTestFromFile("same-value-x-y-object");

    [Fact(DisplayName = "same-value-x-y-string")]
    public Task same_value_x_y_string()
        => ExecutionTestFromFile("same-value-x-y-string");

    [Fact(DisplayName = "same-value-x-y-symbol")]
    public Task same_value_x_y_symbol()
        => ExecutionTestFromFile("same-value-x-y-symbol");

    [Fact(DisplayName = "same-value-x-y-undefined")]
    public Task same_value_x_y_undefined()
        => ExecutionTestFromFile("same-value-x-y-undefined");

    [Fact(DisplayName = "symbol-object-is-same-value")]
    public Task symbol_object_is_same_value()
        => ExecutionTestFromFile("symbol-object-is-same-value");

}
