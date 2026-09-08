using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.filter.BigInt;

public class FailingBatchExecutionTests : DiskExecutionTestsBase
{
    public FailingBatchExecutionTests() : base("built_ins.TypedArray.prototype.filter.BigInt") { }

    [Fact(DisplayName = "callbackfn-called-before-ctor.js")]
    public Task callbackfn_called_before_ctor() => ExecutionTestFromFile("callbackfn-called-before-ctor");

    [Fact(DisplayName = "callbackfn-called-before-species.js")]
    public Task callbackfn_called_before_species() => ExecutionTestFromFile("callbackfn-called-before-species");

    [Fact(DisplayName = "callbackfn-detachbuffer.js")]
    public Task callbackfn_detachbuffer() => ExecutionTestFromFile("callbackfn-detachbuffer");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "speciesctor-destination-resizable.js")]
    public Task speciesctor_destination_resizable() => ExecutionTestFromFile("speciesctor-destination-resizable");

    [Fact(DisplayName = "speciesctor-get-ctor-abrupt.js")]
    public Task speciesctor_get_ctor_abrupt() => ExecutionTestFromFile("speciesctor-get-ctor-abrupt");

    [Fact(DisplayName = "speciesctor-get-ctor-inherited.js")]
    public Task speciesctor_get_ctor_inherited() => ExecutionTestFromFile("speciesctor-get-ctor-inherited");

    [Fact(DisplayName = "speciesctor-get-ctor-returns-throws.js")]
    public Task speciesctor_get_ctor_returns_throws() => ExecutionTestFromFile("speciesctor-get-ctor-returns-throws");

    [Fact(DisplayName = "speciesctor-get-ctor.js")]
    public Task speciesctor_get_ctor() => ExecutionTestFromFile("speciesctor-get-ctor");

    [Fact(DisplayName = "speciesctor-get-species-abrupt.js")]
    public Task speciesctor_get_species_abrupt() => ExecutionTestFromFile("speciesctor-get-species-abrupt");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-invocation.js")]
    public Task speciesctor_get_species_custom_ctor_invocation() => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-invocation");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-length-throws-resizable-arraybuffer.js")]
    public Task speciesctor_get_species_custom_ctor_length_throws_resizable_arraybuffer() => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-length-throws-resizable-arraybuffer");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-length-throws.js")]
    public Task speciesctor_get_species_custom_ctor_length_throws() => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-length-throws");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-length.js")]
    public Task speciesctor_get_species_custom_ctor_length() => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-length");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-returns-another-instance.js")]
    public Task speciesctor_get_species_custom_ctor_returns_another_instance() => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-returns-another-instance");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-throws.js")]
    public Task speciesctor_get_species_custom_ctor_throws() => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-throws");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor.js")]
    public Task speciesctor_get_species_custom_ctor() => ExecutionTestFromFile("speciesctor-get-species-custom-ctor");

    [Fact(DisplayName = "speciesctor-get-species-returns-throws.js")]
    public Task speciesctor_get_species_returns_throws() => ExecutionTestFromFile("speciesctor-get-species-returns-throws");

    [Fact(DisplayName = "speciesctor-get-species.js")]
    public Task speciesctor_get_species() => ExecutionTestFromFile("speciesctor-get-species");
}
