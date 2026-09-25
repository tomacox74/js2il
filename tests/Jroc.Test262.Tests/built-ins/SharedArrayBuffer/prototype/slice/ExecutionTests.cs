using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer.prototype.slice;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.SharedArrayBuffer.prototype.slice") { }

    [Fact(DisplayName = "context-is-not-arraybuffer-object")]
    public Task context_is_not_arraybuffer_object()
        => ExecutionTest("context-is-not-arraybuffer-object");

    [Fact(DisplayName = "context-is-not-object")]
    public Task context_is_not_object()
        => ExecutionTest("context-is-not-object");

    [Fact(DisplayName = "descriptor")]
    public Task descriptor()
        => ExecutionTest("descriptor");

    [Fact(DisplayName = "extensible")]
    public Task extensible()
        => ExecutionTest("extensible");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTest("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "species-constructor-is-not-object.js")]
    public Task ported_species_constructor_is_not_object() => ExecutionTest("species-constructor-is-not-object");

    [Fact(DisplayName = "species-is-not-constructor.js")]
    public Task ported_species_is_not_constructor() => ExecutionTest("species-is-not-constructor");

    [Fact(DisplayName = "species-is-not-object.js")]
    public Task ported_species_is_not_object() => ExecutionTest("species-is-not-object");
}
