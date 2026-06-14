using Jroc.Test262.Tests.built_ins;



namespace Jroc.Test262.Tests.built_ins.Object.keys;



public class ExecutionTests : DiskExecutionTestsBase

{

    public ExecutionTests() : base("built_ins.Object.keys") { }



    [Fact(DisplayName = "15.2.3.14-0-1")]

    public Task _15_2_3_14_0_1()

        => ExecutionTestFromFile("15.2.3.14-0-1");



    [Fact(DisplayName = "15.2.3.14-0-2")]

    public Task _15_2_3_14_0_2()

        => ExecutionTestFromFile("15.2.3.14-0-2");



    [Fact(DisplayName = "15.2.3.14-2-1")]

    public Task _15_2_3_14_2_1()

        => ExecutionTestFromFile("15.2.3.14-2-1");



    [Fact(DisplayName = "15.2.3.14-2-2")]

    public Task _15_2_3_14_2_2()

        => ExecutionTestFromFile("15.2.3.14-2-2");



    [Fact(DisplayName = "15.2.3.14-3-1")]

    public Task _15_2_3_14_3_1()

        => ExecutionTestFromFile("15.2.3.14-3-1");

    [Fact(DisplayName = "15.2.3.14-1-1")]
    public Task _15_2_3_14_1_1()
        => ExecutionTestFromFile("15.2.3.14-1-1");

    [Fact(DisplayName = "15.2.3.14-1-2")]
    public Task _15_2_3_14_1_2()
        => ExecutionTestFromFile("15.2.3.14-1-2");

    [Fact(DisplayName = "15.2.3.14-1-3")]
    public Task _15_2_3_14_1_3()
        => ExecutionTestFromFile("15.2.3.14-1-3");

    [Fact(DisplayName = "15.2.3.14-1-4")]
    public Task _15_2_3_14_1_4()
        => ExecutionTestFromFile("15.2.3.14-1-4");

    [Fact(DisplayName = "15.2.3.14-1-5")]
    public Task _15_2_3_14_1_5()
        => ExecutionTestFromFile("15.2.3.14-1-5");

}
