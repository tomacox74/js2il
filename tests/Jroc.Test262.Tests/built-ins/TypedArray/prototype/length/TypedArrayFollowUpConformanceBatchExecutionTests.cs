using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.length;

public class TypedArrayFollowUpConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayFollowUpConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.length") { }

    [Fact(DisplayName = "invoked-as-accessor.js")]
    public Task invoked_as_accessor() => ExecutionTestFromFile("invoked-as-accessor");

    [Fact(DisplayName = "invoked-as-func.js")]
    public Task invoked_as_func() => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "resizable-array-buffer-auto.js")]
    public Task resizable_array_buffer_auto() => ExecutionTestFromFile("resizable-array-buffer-auto");

    [Fact(DisplayName = "resizable-array-buffer-fixed.js")]
    public Task resizable_array_buffer_fixed() => ExecutionTestFromFile("resizable-array-buffer-fixed");

    [Fact(DisplayName = "return-length.js")]
    public Task return_length() => ExecutionTestFromFile("return-length");

    [Fact(DisplayName = "this-has-no-typedarrayname-internal.js")]
    public Task this_has_no_typedarrayname_internal() => ExecutionTestFromFile("this-has-no-typedarrayname-internal");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

}
