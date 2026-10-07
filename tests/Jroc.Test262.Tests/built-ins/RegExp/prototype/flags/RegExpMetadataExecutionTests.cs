using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.flags;

public sealed class RegExpMetadataExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpMetadataExecutionTests() : base("RegExp.Metadata") { }

    [Fact(DisplayName = "name.js")]
    public Task name()
        => ExecutionTestFromFile("name");

}
