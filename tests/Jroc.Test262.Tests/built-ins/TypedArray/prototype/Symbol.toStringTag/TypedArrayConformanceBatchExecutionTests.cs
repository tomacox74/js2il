using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.Symbol.toStringTag;

public class TypedArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.Symbol.toStringTag") { }

    [Fact(DisplayName = "invoked-as-accessor.js")]
    public Task invoked_as_accessor() => ExecutionTestFromFile("invoked-as-accessor");

    [Fact(DisplayName = "invoked-as-func.js")]
    public Task invoked_as_func() => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "this-has-no-typedarrayname-internal.js")]
    public Task this_has_no_typedarrayname_internal() => ExecutionTestFromFile("this-has-no-typedarrayname-internal");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

}
