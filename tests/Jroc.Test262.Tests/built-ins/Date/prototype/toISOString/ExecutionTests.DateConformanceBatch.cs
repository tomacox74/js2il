using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.toISOString;

public partial class ExecutionTests
{
    [Fact(DisplayName = "15.9.5.43-0-13.js")]
    public Task _15_9_5_43_0_13() => ExecutionTestFromFile("15.9.5.43-0-13");

    [Fact(DisplayName = "15.9.5.43-0-14.js")]
    public Task _15_9_5_43_0_14() => ExecutionTestFromFile("15.9.5.43-0-14");

    [Fact(DisplayName = "15.9.5.43-0-15.js")]
    public Task _15_9_5_43_0_15() => ExecutionTestFromFile("15.9.5.43-0-15");

    [Fact(DisplayName = "15.9.5.43-0-2.js")]
    public Task _15_9_5_43_0_2() => ExecutionTestFromFile("15.9.5.43-0-2");

    [Fact(DisplayName = "15.9.5.43-0-3.js")]
    public Task _15_9_5_43_0_3() => ExecutionTestFromFile("15.9.5.43-0-3");

    [Fact(DisplayName = "15.9.5.43-0-4.js")]
    public Task _15_9_5_43_0_4() => ExecutionTestFromFile("15.9.5.43-0-4");

    [Fact(DisplayName = "15.9.5.43-0-8.js")]
    public Task _15_9_5_43_0_8() => ExecutionTestFromFile("15.9.5.43-0-8");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

}
