using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.JSON.stringify;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.JSON.stringify") { }

    [Fact(DisplayName = "builtin", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task builtin()
        => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "length", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "property-order", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task property_order()
        => ExecutionTestFromFile("property-order");

    [Fact(DisplayName = "replacer-array-abrupt")]
    public Task replacer_array_abrupt()
        => ExecutionTestFromFile("replacer-array-abrupt");

    [Fact(DisplayName = "replacer-array-duplicates", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task replacer_array_duplicates()
        => ExecutionTestFromFile("replacer-array-duplicates");

    [Fact(DisplayName = "replacer-array-empty", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task replacer_array_empty()
        => ExecutionTestFromFile("replacer-array-empty");

    [Fact(DisplayName = "replacer-array-number-object", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task replacer_array_number_object()
        => ExecutionTestFromFile("replacer-array-number-object");

    [Fact(DisplayName = "replacer-array-number", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task replacer_array_number()
        => ExecutionTestFromFile("replacer-array-number");

    [Fact(DisplayName = "replacer-array-order", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task replacer_array_order()
        => ExecutionTestFromFile("replacer-array-order");

    [Fact(DisplayName = "replacer-array-proxy-revoked-realm", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task replacer_array_proxy_revoked_realm()
        => ExecutionTestFromFile("replacer-array-proxy-revoked-realm");

    [Fact(DisplayName = "replacer-array-proxy-revoked")]
    public Task replacer_array_proxy_revoked()
        => ExecutionTestFromFile("replacer-array-proxy-revoked");

    [Fact(DisplayName = "replacer-array-proxy", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task replacer_array_proxy()
        => ExecutionTestFromFile("replacer-array-proxy");

    [Fact(DisplayName = "replacer-array-string-object", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task replacer_array_string_object()
        => ExecutionTestFromFile("replacer-array-string-object");

    [Fact(DisplayName = "replacer-array-undefined", Skip = "Known issue: unstable timeout in this test262 scenario")]
    public Task replacer_array_undefined()
        => ExecutionTestFromFile("replacer-array-undefined");
}
