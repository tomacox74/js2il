using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.reduceRight;

public class TypedArraySecondFollowUpConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArraySecondFollowUpConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.reduceRight") { }

    [Fact(DisplayName = "callbackfn-is-not-callable-throws.js")]
    public Task callbackfn_is_not_callable_throws() => ExecutionTestFromFile("callbackfn-is-not-callable-throws");

    [Fact(DisplayName = "empty-instance-with-no-initialvalue-throws.js")]
    public Task empty_instance_with_no_initialvalue_throws() => ExecutionTestFromFile("empty-instance-with-no-initialvalue-throws");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

    [Fact(DisplayName = "this-is-not-typedarray-instance.js")]
    public Task this_is_not_typedarray_instance() => ExecutionTestFromFile("this-is-not-typedarray-instance");

}
