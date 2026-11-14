using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer.prototype.slice;

public class Port400ExecutionTests : InMemoryExecutionTestsBase
{
    public Port400ExecutionTests() : base("built_ins.SharedArrayBuffer.prototype.slice") { }

    [Fact(DisplayName = "species-returns-larger-arraybuffer.js")]
    public Task Port400_species_returns_larger_arraybuffer()
        => ExecutionTestFromFile("species-returns-larger-arraybuffer");

    [Fact(DisplayName = "species-returns-not-arraybuffer.js")]
    public Task Port400_species_returns_not_arraybuffer()
        => ExecutionTestFromFile("species-returns-not-arraybuffer");

    [Fact(DisplayName = "species-returns-same-arraybuffer.js")]
    public Task Port400_species_returns_same_arraybuffer()
        => ExecutionTestFromFile("species-returns-same-arraybuffer");

    [Fact(DisplayName = "species-returns-smaller-arraybuffer.js")]
    public Task Port400_species_returns_smaller_arraybuffer()
        => ExecutionTestFromFile("species-returns-smaller-arraybuffer");

    [Fact(DisplayName = "species.js")]
    public Task Port400_species()
        => ExecutionTestFromFile("species");
}
