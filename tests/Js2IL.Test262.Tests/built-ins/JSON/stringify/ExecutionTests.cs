using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.JSON.stringify;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.JSON.stringify") { }

[Fact(DisplayName = "builtin", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task builtin()
        => ExecutionTest("builtin");

[Fact(DisplayName = "length", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task length()
        => ExecutionTest("length");

[Fact(DisplayName = "name", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task name()
        => ExecutionTest("name");

[Fact(DisplayName = "prop-desc", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task prop_desc()
        => ExecutionTest("prop-desc");

[Fact(DisplayName = "property-order", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task property_order()
        => ExecutionTest("property-order");

[Fact(DisplayName = "replacer-array-abrupt")]
    public Task replacer_array_abrupt()
        => ExecutionTest("replacer-array-abrupt");

[Fact(DisplayName = "replacer-array-duplicates", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task replacer_array_duplicates()
        => ExecutionTest("replacer-array-duplicates");

[Fact(DisplayName = "replacer-array-empty", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task replacer_array_empty()
        => ExecutionTest("replacer-array-empty");

[Fact(DisplayName = "builtin", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task builtin()
        => ExecutionTest("builtin");

[Fact(DisplayName = "length", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task length()
        => ExecutionTest("length");

[Fact(DisplayName = "name", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task name()
        => ExecutionTest("name");

[Fact(DisplayName = "prop-desc", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task prop_desc()
        => ExecutionTest("prop-desc");

[Fact(DisplayName = "property-order", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task property_order()
        => ExecutionTest("property-order");

[Fact(DisplayName = "replacer-array-abrupt")]
    public Task replacer_array_abrupt()
        => ExecutionTest("replacer-array-abrupt");

[Fact(DisplayName = "replacer-array-duplicates", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task replacer_array_duplicates()
        => ExecutionTest("replacer-array-duplicates");

[Fact(DisplayName = "replacer-array-empty", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task replacer_array_empty()
        => ExecutionTest("replacer-array-empty");

[Fact(DisplayName = "builtin", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task builtin()
        => ExecutionTest("builtin");

[Fact(DisplayName = "length", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task length()
        => ExecutionTest("length");

[Fact(DisplayName = "name", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task name()
        => ExecutionTest("name");

[Fact(DisplayName = "prop-desc", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task prop_desc()
        => ExecutionTest("prop-desc");

[Fact(DisplayName = "property-order", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task property_order()
        => ExecutionTest("property-order");

[Fact(DisplayName = "replacer-array-abrupt")]
    public Task replacer_array_abrupt()
        => ExecutionTest("replacer-array-abrupt");

[Fact(DisplayName = "replacer-array-duplicates", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task replacer_array_duplicates()
        => ExecutionTest("replacer-array-duplicates");

[Fact(DisplayName = "replacer-array-empty", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task replacer_array_empty()
        => ExecutionTest("replacer-array-empty");
}
