using Xunit;

namespace Jroc.Test262.Tests.annexB.built_ins.Date.prototype.getYear;

public class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("annexB/built-ins/Date/prototype/getYear") { }

    [Fact(DisplayName = "B.2.4.js")]
    public Task B_2_4() => ExecutionTest("B.2.4");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTest("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTest("name");

    [Fact(DisplayName = "nan.js")]
    public Task nan() => ExecutionTest("nan");

    [Fact(DisplayName = "return-value.js")]
    public Task return_value() => ExecutionTest("return-value");

    [Fact(DisplayName = "this-not-date.js")]
    public Task this_not_date() => ExecutionTest("this-not-date");
}
