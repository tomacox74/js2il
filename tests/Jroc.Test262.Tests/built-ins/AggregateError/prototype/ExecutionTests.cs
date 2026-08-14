using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AggregateError.prototype;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AggregateError.prototype") { }

    [Fact(DisplayName = "constructor.js")]
    public Task constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "errors-absent-on-prototype.js")]
    public Task errors_absent_on_prototype()
        => ExecutionTestFromFile("errors-absent-on-prototype");

    [Fact(DisplayName = "message.js")]
    public Task message() => ExecutionTestFromFile("message");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");
}
