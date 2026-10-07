using Xunit;

namespace Jroc.Test262.Tests.annexB.built_ins.String.prototype.@fixed;

public class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("annexB/built-ins/String/prototype/fixed")
    {
    }

    [Fact(DisplayName = "B.2.3.6.js")]
    public Task B_2_3_6() => ExecutionTest("B.2.3.6");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTest("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTest("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTest("prop-desc");

    [Fact(DisplayName = "this-val-tostring-err.js")]
    public Task this_val_tostring_err() => ExecutionTest("this-val-tostring-err");
}
