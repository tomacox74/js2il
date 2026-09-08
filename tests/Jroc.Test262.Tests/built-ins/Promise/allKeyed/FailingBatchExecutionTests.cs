using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.allKeyed;

public class PromiseBatchExecutionTests : DiskExecutionTestsBase
{
    public PromiseBatchExecutionTests() : base("built_ins.Promise.allKeyed") { }

    [Fact(DisplayName = "extensible.js")]
    public Task extensible()
        => ExecutionTestFromFile("extensible");

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto()
        => ExecutionTestFromFile("proto");

}
