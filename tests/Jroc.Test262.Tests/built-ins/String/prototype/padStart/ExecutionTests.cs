using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.padStart;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.padStart") { }

    [Fact(DisplayName = "exception-fill-string-symbol")] public Task exception_fill_string_symbol() => ExecutionTestFromFile("exception-fill-string-symbol");
    [Fact(DisplayName = "exception-not-object-coercible")] public Task exception_not_object_coercible() => ExecutionTestFromFile("exception-not-object-coercible");
    [Fact(DisplayName = "exception-symbol")] public Task exception_symbol() => ExecutionTestFromFile("exception-symbol");
    [Fact(DisplayName = "fill-string-empty")] public Task fill_string_empty() => ExecutionTestFromFile("fill-string-empty");
    [Fact(DisplayName = "fill-string-non-strings")] public Task fill_string_non_strings() => ExecutionTestFromFile("fill-string-non-strings");
    [Fact(DisplayName = "fill-string-omitted")] public Task fill_string_omitted() => ExecutionTestFromFile("fill-string-omitted");
    [Fact(DisplayName = "function-length")] public Task function_length() => ExecutionTestFromFile("function-length");
    [Fact(DisplayName = "function-name")] public Task function_name() => ExecutionTestFromFile("function-name");
    [Fact(DisplayName = "function-property-descriptor")] public Task function_property_descriptor() => ExecutionTestFromFile("function-property-descriptor");
    [Fact(DisplayName = "normal-operation")] public Task normal_operation() => ExecutionTestFromFile("normal-operation");
}
