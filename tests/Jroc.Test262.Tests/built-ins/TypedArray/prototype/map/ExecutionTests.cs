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
}
