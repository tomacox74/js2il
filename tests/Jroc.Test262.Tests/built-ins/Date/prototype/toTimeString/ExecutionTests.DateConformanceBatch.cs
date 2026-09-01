using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.toTimeString;

public partial class ExecutionTests
{
    [Fact(DisplayName = "format.js")]
    public Task format() => ExecutionTestFromFile("format");

    [Fact(DisplayName = "invalid-date.js")]
    public Task invalid_date() => ExecutionTestFromFile("invalid-date");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

}
