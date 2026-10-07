using Xunit;

namespace Jroc.Test262.Tests.annexB.built_ins.String.prototype.bold;

public class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("annexB/built-ins/String/prototype/bold") { }

    [Fact(DisplayName = "B.2.3.5.js")]
    public Task B_2_3_5() => ExecutionTest("B.2.3.5");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTest("length");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTest("prop-desc");

    [Fact(DisplayName = "this-val-tostring-err.js")]
    public Task this_val_tostring_err() => ExecutionTest("this-val-tostring-err");
}
