using Xunit;

namespace Jroc.Test262.Tests.annexB.built_ins.Date.prototype.toGMTString;

public class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("annexB/built-ins/Date/prototype/toGMTString") { }

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTest("prop-desc");

    [Fact(DisplayName = "value.js")]
    public Task value() => ExecutionTest("value");
}
