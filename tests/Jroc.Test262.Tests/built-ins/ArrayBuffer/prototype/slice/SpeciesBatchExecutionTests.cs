using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.slice;

public class SpeciesBatchExecutionTests : DiskExecutionTestsBase
{
    public SpeciesBatchExecutionTests() : base("built_ins.ArrayBuffer.prototype.slice") { }

    [Fact(DisplayName = "species-constructor-is-not-object.js")]
    public Task species_constructor_is_not_object() => ExecutionTestFromFile("species-constructor-is-not-object");

    [Fact(DisplayName = "species-is-not-constructor.js")]
    public Task species_is_not_constructor() => ExecutionTestFromFile("species-is-not-constructor");

    [Fact(DisplayName = "species-is-not-object.js")]
    public Task species_is_not_object() => ExecutionTestFromFile("species-is-not-object");

    [Fact(DisplayName = "species.js")]
    public Task species() => ExecutionTestFromFile("species");

    [Fact(DisplayName = "species-returns-larger-arraybuffer.js")]
    public Task species_returns_larger_arraybuffer() => ExecutionTestFromFile("species-returns-larger-arraybuffer");

    [Fact(DisplayName = "species-returns-not-arraybuffer.js")]
    public Task species_returns_not_arraybuffer() => ExecutionTestFromFile("species-returns-not-arraybuffer");

    [Fact(DisplayName = "species-returns-same-arraybuffer.js")]
    public Task species_returns_same_arraybuffer() => ExecutionTestFromFile("species-returns-same-arraybuffer");

    [Fact(DisplayName = "species-returns-smaller-arraybuffer.js")]
    public Task species_returns_smaller_arraybuffer() => ExecutionTestFromFile("species-returns-smaller-arraybuffer");

}
