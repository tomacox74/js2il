using Xunit;

namespace Jroc.Test262.Tests.annexB.built_ins.String.prototype.fontcolor;

public class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("annexB/built-ins/String/prototype/fontcolor")
    {
    }

    [Fact(DisplayName = "B.2.3.7.js")]
    public Task B_2_3_7() => ExecutionTest("B.2.3.7");

    [Fact(DisplayName = "attr-tostring-err.js")]
    public Task attr_tostring_err() => ExecutionTest("attr-tostring-err");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTest("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTest("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTest("prop-desc");

    [Fact(DisplayName = "this-val-tostring-err.js")]
    public Task this_val_tostring_err() => ExecutionTest("this-val-tostring-err");
}
