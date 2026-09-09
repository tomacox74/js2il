using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.slice.BigInt;

public class NextBatchExecutionTests : DiskExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.slice.BigInt") { }

    [Fact(DisplayName = "detached-buffer-custom-ctor-other-targettype")]
    public Task detached_buffer_custom_ctor_other_targettype() => ExecutionTestFromFile("detached-buffer-custom-ctor-other-targettype");

    [Fact(DisplayName = "detached-buffer-custom-ctor-same-targettype")]
    public Task detached_buffer_custom_ctor_same_targettype() => ExecutionTestFromFile("detached-buffer-custom-ctor-same-targettype");

    [Fact(DisplayName = "detached-buffer-get-ctor")]
    public Task detached_buffer_get_ctor() => ExecutionTestFromFile("detached-buffer-get-ctor");

    [Fact(DisplayName = "detached-buffer-speciesctor-get-species-custom-ctor-throws")]
    public Task detached_buffer_speciesctor_get_species_custom_ctor_throws() => ExecutionTestFromFile("detached-buffer-speciesctor-get-species-custom-ctor-throws");

    [Fact(DisplayName = "detached-buffer-zero-count-custom-ctor-other-targettype")]
    public Task detached_buffer_zero_count_custom_ctor_other_targettype() => ExecutionTestFromFile("detached-buffer-zero-count-custom-ctor-other-targettype");

    [Fact(DisplayName = "detached-buffer-zero-count-custom-ctor-same-targettype")]
    public Task detached_buffer_zero_count_custom_ctor_same_targettype() => ExecutionTestFromFile("detached-buffer-zero-count-custom-ctor-same-targettype");

    [Fact(DisplayName = "detached-buffer")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

}
