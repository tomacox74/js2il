using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.toString;

public partial class ExecutionTests
{
    [Fact(DisplayName = "invalid-date.js")]
    public Task invalid_date() => ExecutionTestFromFile("invalid-date");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "non-date-receiver.js")]
    public Task non_date_receiver() => ExecutionTestFromFile("non-date-receiver");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

}
