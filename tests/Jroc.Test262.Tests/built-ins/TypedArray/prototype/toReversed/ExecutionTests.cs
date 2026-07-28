using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.toReversed;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype.toReversed") { }

    [Fact(DisplayName = "immutable")]
    public Task immutable() => ExecutionTestFromFile("immutable");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "property-descriptor")]
    public Task property_descriptor() => ExecutionTestFromFile("property-descriptor");

    [Fact(DisplayName = "reverses")]
    public Task reverses() => ExecutionTestFromFile("reverses");
}
