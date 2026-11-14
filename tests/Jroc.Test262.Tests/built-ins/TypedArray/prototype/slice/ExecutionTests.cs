using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.slice;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.slice") { }

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");


    [Fact(DisplayName = "detached-buffer-speciesctor-get-species-custom-ctor-throws.js")]
    public Task Port400_detached_buffer_speciesctor_get_species_custom_ctor_throws()
        => ExecutionTestFromFile("detached-buffer-speciesctor-get-species-custom-ctor-throws");

    [Fact(DisplayName = "detached-buffer-zero-count-custom-ctor-other-targettype.js")]
    public Task Port400_detached_buffer_zero_count_custom_ctor_other_targettype()
        => ExecutionTestFromFile("detached-buffer-zero-count-custom-ctor-other-targettype");

    [Fact(DisplayName = "detached-buffer-zero-count-custom-ctor-same-targettype.js")]
    public Task Port400_detached_buffer_zero_count_custom_ctor_same_targettype()
        => ExecutionTestFromFile("detached-buffer-zero-count-custom-ctor-same-targettype");
}
