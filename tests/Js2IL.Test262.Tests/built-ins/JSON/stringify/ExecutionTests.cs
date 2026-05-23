using Js2IL.Test262.Tests.built_ins;



namespace Js2IL.Test262.Tests.built_ins.JSON.stringify;



public class ExecutionTests : DiskExecutionTestsBase

{

    public ExecutionTests() : base("built_ins.JSON.stringify") { }



    [Fact(DisplayName = "builtin")]

    public Task builtin()

        => ExecutionTestFromFile("builtin");



    [Fact(DisplayName = "length")]

    public Task length()

        => ExecutionTestFromFile("length");



    [Fact(DisplayName = "name")]

    public Task name()

        => ExecutionTestFromFile("name");



    [Fact(DisplayName = "prop-desc")]

    public Task prop_desc()

        => ExecutionTestFromFile("prop-desc");



    [Fact(DisplayName = "property-order")]
    public Task property_order()

        => ExecutionTestFromFile("property-order");



    [Fact(DisplayName = "replacer-array-abrupt")]
    public Task replacer_array_abrupt()

        => ExecutionTestFromFile("replacer-array-abrupt");



    [Fact(DisplayName = "replacer-array-duplicates")]
    public Task replacer_array_duplicates()

        => ExecutionTestFromFile("replacer-array-duplicates");



    [Fact(DisplayName = "replacer-array-empty")]
    public Task replacer_array_empty()

        => ExecutionTestFromFile("replacer-array-empty");



    [Fact(DisplayName = "replacer-array-number-object")]
    public Task replacer_array_number_object()

        => ExecutionTestFromFile("replacer-array-number-object");



    [Fact(DisplayName = "replacer-array-number")]

    public Task replacer_array_number()

        => ExecutionTestFromFile("replacer-array-number");



    [Fact(DisplayName = "replacer-array-order")]
    public Task replacer_array_order()

        => ExecutionTestFromFile("replacer-array-order");



    [Fact(DisplayName = "replacer-array-proxy-revoked-realm")]
    public Task replacer_array_proxy_revoked_realm()

        => ExecutionTestFromFile("replacer-array-proxy-revoked-realm");



    [Fact(DisplayName = "replacer-array-proxy-revoked")]

    public Task replacer_array_proxy_revoked()

        => ExecutionTestFromFile("replacer-array-proxy-revoked");



    [Fact(DisplayName = "replacer-array-proxy")]
    public Task replacer_array_proxy()

        => ExecutionTestFromFile("replacer-array-proxy");



    [Fact(DisplayName = "replacer-array-string-object")]
    public Task replacer_array_string_object()

        => ExecutionTestFromFile("replacer-array-string-object");



    [Fact(DisplayName = "replacer-array-undefined")]
    public Task replacer_array_undefined()

        => ExecutionTestFromFile("replacer-array-undefined");

    [Fact(DisplayName = "replacer-function-abrupt", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task replacer_function_abrupt()
        => ExecutionTestFromFile("replacer-function-abrupt");

    [Fact(DisplayName = "replacer-function-arguments", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task replacer_function_arguments()
        => ExecutionTestFromFile("replacer-function-arguments");

    [Fact(DisplayName = "replacer-function-array-circular", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task replacer_function_array_circular()
        => ExecutionTestFromFile("replacer-function-array-circular");

    [Fact(DisplayName = "replacer-function-object-circular", Skip = "Tracked by #1093: built-ins port is sound but JS2IL does not yet match expected runtime behavior.")]
    public Task replacer_function_object_circular()
        => ExecutionTestFromFile("replacer-function-object-circular");

}
