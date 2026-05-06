using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Object.entries;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.entries") { }

[Fact(DisplayName = "exception-during-enumeration", Skip = "Global RangeError constructor exposure is incomplete.")]
    public Task exception_during_enumeration()
        => ExecutionTest("exception-during-enumeration");

[Fact(DisplayName = "exception-not-object-coercible")]
    public Task exception_not_object_coercible()
        => ExecutionTest("exception-not-object-coercible");

[Fact(DisplayName = "function-length")]
    public Task function_length()
        => ExecutionTest("function-length");

[Fact(DisplayName = "function-name", Skip = "Object.entries intrinsic metadata support is incomplete.")]
    public Task function_name()
        => ExecutionTest("function-name");

[Fact(DisplayName = "function-property-descriptor")]
    public Task function_property_descriptor()
        => ExecutionTest("function-property-descriptor");

[Fact(DisplayName = "getter-adding-key")]
    public Task getter_adding_key()
        => ExecutionTest("getter-adding-key");

[Fact(DisplayName = "getter-making-future-key-nonenumerable")]
    public Task getter_making_future_key_nonenumerable()
        => ExecutionTest("getter-making-future-key-nonenumerable");

[Fact(DisplayName = "getter-removing-future-key")]
    public Task getter_removing_future_key()
        => ExecutionTest("getter-removing-future-key");

[Fact(DisplayName = "inherited-properties-omitted")]
    public Task inherited_properties_omitted()
        => ExecutionTest("inherited-properties-omitted");

[Fact(DisplayName = "exception-during-enumeration", Skip = "Global RangeError constructor exposure is incomplete.")]
    public Task exception_during_enumeration()
        => ExecutionTest("exception-during-enumeration");

[Fact(DisplayName = "exception-not-object-coercible")]
    public Task exception_not_object_coercible()
        => ExecutionTest("exception-not-object-coercible");

[Fact(DisplayName = "function-length")]
    public Task function_length()
        => ExecutionTest("function-length");

[Fact(DisplayName = "function-name", Skip = "Object.entries intrinsic metadata support is incomplete.")]
    public Task function_name()
        => ExecutionTest("function-name");

[Fact(DisplayName = "function-property-descriptor")]
    public Task function_property_descriptor()
        => ExecutionTest("function-property-descriptor");

[Fact(DisplayName = "getter-adding-key")]
    public Task getter_adding_key()
        => ExecutionTest("getter-adding-key");

[Fact(DisplayName = "getter-making-future-key-nonenumerable")]
    public Task getter_making_future_key_nonenumerable()
        => ExecutionTest("getter-making-future-key-nonenumerable");

[Fact(DisplayName = "getter-removing-future-key")]
    public Task getter_removing_future_key()
        => ExecutionTest("getter-removing-future-key");

[Fact(DisplayName = "inherited-properties-omitted")]
    public Task inherited_properties_omitted()
        => ExecutionTest("inherited-properties-omitted");
}
