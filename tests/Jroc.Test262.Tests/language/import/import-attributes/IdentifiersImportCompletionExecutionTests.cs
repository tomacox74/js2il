namespace Jroc.Test262.Tests.language.import.import_attributes;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.import.import_attributes") { }

    [Fact(DisplayName = "json-extensibility-array.js")]
    public Task json_extensibility_array()
        => ExecutionTest("json-extensibility-array");

    [Fact(DisplayName = "json-extensibility-object.js")]
    public Task json_extensibility_object()
        => ExecutionTest("json-extensibility-object");

    [Fact(DisplayName = "json-idempotency.js")]
    public Task json_idempotency()
        => ExecutionTest("json-idempotency");

    [Fact(DisplayName = "json-invalid.js")]
    public Task json_invalid()
        => CompilationFailureTest("json-invalid");

    [Fact(DisplayName = "json-named-bindings.js")]
    public Task json_named_bindings()
        => CompilationFailureTest("json-named-bindings");

    [Fact(DisplayName = "json-value-array.js")]
    public Task json_value_array()
        => ExecutionTest("json-value-array");

    [Fact(DisplayName = "json-value-boolean.js")]
    public Task json_value_boolean()
        => ExecutionTest("json-value-boolean");

    [Fact(DisplayName = "json-value-null.js")]
    public Task json_value_null()
        => ExecutionTest("json-value-null");

    [Fact(DisplayName = "json-value-number.js")]
    public Task json_value_number()
        => ExecutionTest("json-value-number");

    [Fact(DisplayName = "json-value-object.js")]
    public Task json_value_object()
        => ExecutionTest("json-value-object");

    [Fact(DisplayName = "json-value-string.js")]
    public Task json_value_string()
        => ExecutionTest("json-value-string");

    [Fact(DisplayName = "json-via-namespace.js")]
    public Task json_via_namespace()
        => ExecutionTest("json-via-namespace");

    [Fact(DisplayName = "text-empty.js")]
    public Task text_empty()
        => ExecutionTest("text-empty");

    [Fact(DisplayName = "text-javascript.js")]
    public Task text_javascript()
        => ExecutionTest("text-javascript");

    [Fact(DisplayName = "text-self.js")]
    public Task text_self()
        => ExecutionTest("text-self");

    [Fact(DisplayName = "text-string.js")]
    public Task text_string()
        => ExecutionTest("text-string");

    [Fact(DisplayName = "text-via-namespace.js")]
    public Task text_via_namespace()
        => ExecutionTest("text-via-namespace");
}
