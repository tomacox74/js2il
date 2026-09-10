using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.map;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype.map") { }

    [Fact(DisplayName = "speciesctor-get-ctor")]
    public Task speciesctor_get_ctor() => ExecutionTestFromFile("speciesctor-get-ctor");

    [Fact(DisplayName = "speciesctor-get-ctor-inherited")]
    public Task speciesctor_get_ctor_inherited() => ExecutionTestFromFile("speciesctor-get-ctor-inherited");

    [Fact(DisplayName = "speciesctor-get-species")]
    public Task speciesctor_get_species() => ExecutionTestFromFile("speciesctor-get-species");

    [Fact(DisplayName = "speciesctor-get-species-use-default-ctor")]
    public Task speciesctor_get_species_use_default_ctor() => ExecutionTestFromFile("speciesctor-get-species-use-default-ctor");

    [Fact(DisplayName = "resizable-buffer-grow-mid-iteration.js")]
    public Task resizable_buffer_grow_mid_iteration() => ExecutionTestFromFile("resizable-buffer-grow-mid-iteration");
    [Fact(DisplayName = "resizable-buffer-shrink-mid-iteration.js")]
    public Task resizable_buffer_shrink_mid_iteration() => ExecutionTestFromFile("resizable-buffer-shrink-mid-iteration");
    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");
    [Fact(DisplayName = "speciesctor-resizable-buffer-grow.js")]
    public Task speciesctor_resizable_buffer_grow() => ExecutionTestFromFile("speciesctor-resizable-buffer-grow");
    [Fact(DisplayName = "speciesctor-resizable-buffer-shrink.js")]
    public Task speciesctor_resizable_buffer_shrink() => ExecutionTestFromFile("speciesctor-resizable-buffer-shrink");
}
