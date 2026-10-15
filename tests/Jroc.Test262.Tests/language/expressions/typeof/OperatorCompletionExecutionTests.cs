using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.typeof_;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.typeof") { }

    [Fact(DisplayName = "bigint.js")]
    public Task test_bigint() => ExecutionTest("bigint");

    [Fact(DisplayName = "boolean.js")]
    public Task test_boolean() => ExecutionTest("boolean");

    [Fact(DisplayName = "built-in-exotic-objects-no-call.js")]
    public Task test_built_in_exotic_objects_no_call() => ExecutionTest("built-in-exotic-objects-no-call");

    [Fact(DisplayName = "built-in-functions.js")]
    public Task test_built_in_functions() => ExecutionTest("built-in-functions");

    [Fact(DisplayName = "built-in-ordinary-objects-no-call.js")]
    public Task test_built_in_ordinary_objects_no_call() => ExecutionTest("built-in-ordinary-objects-no-call");

    [Fact(DisplayName = "get-value-ref-err.js")]
    public Task test_get_value_ref_err() => ExecutionTest("get-value-ref-err");

    [Fact(DisplayName = "get-value.js")]
    public Task test_get_value() => ExecutionTest("get-value");

    [Fact(DisplayName = "native-call.js")]
    public Task test_native_call() => ExecutionTest("native-call");

    [Fact(DisplayName = "null.js")]
    public Task test_null() => ExecutionTest("null");

    [Fact(DisplayName = "number.js")]
    public Task test_number() => ExecutionTest("number");

    [Fact(DisplayName = "string.js")]
    public Task test_string() => ExecutionTest("string");

    [Fact(DisplayName = "symbol.js")]
    public Task test_symbol() => ExecutionTest("symbol");

    [Fact(DisplayName = "syntax.js", Skip = "eval is not supported.")]
    public Task test_syntax() => ExecutionTest("syntax");

    [Fact(DisplayName = "undefined.js")]
    public Task test_undefined() => ExecutionTest("undefined");

    [Fact(DisplayName = "unresolvable-reference.js")]
    public Task test_unresolvable_reference() => ExecutionTest("unresolvable-reference");
}
