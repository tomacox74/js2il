using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.slice;

public class NextBatchExecutionTests : DiskExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.slice") { }

    [Fact(DisplayName = "detached-buffer-custom-ctor-other-targettype")]
    public Task detached_buffer_custom_ctor_other_targettype() => ExecutionTestFromFile("detached-buffer-custom-ctor-other-targettype");

    [Fact(DisplayName = "detached-buffer-custom-ctor-same-targettype")]
    public Task detached_buffer_custom_ctor_same_targettype() => ExecutionTestFromFile("detached-buffer-custom-ctor-same-targettype");

    [Fact(DisplayName = "detached-buffer-get-ctor")]
    public Task detached_buffer_get_ctor() => ExecutionTestFromFile("detached-buffer-get-ctor");

}
