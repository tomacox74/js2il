using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.sticky;

public sealed class RegExpMetadataExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpMetadataExecutionTests() : base("RegExp.Metadata") { }

    [Fact(DisplayName = "name.js")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "this-val-regexp-prototype.js")]
    public Task this_val_regexp_prototype()
        => ExecutionTestFromFile("this-val-regexp-prototype");

}
