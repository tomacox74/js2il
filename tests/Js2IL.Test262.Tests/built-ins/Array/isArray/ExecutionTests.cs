using Js2IL.Test262.Tests.built_ins;



namespace Js2IL.Test262.Tests.built_ins.Array.isArray;



public class ExecutionTests : DiskExecutionTestsBase

{

    public ExecutionTests() : base("built_ins.Array.isArray") { }



    [Fact(DisplayName = "15.4.3.2-0-1")]

    public Task _15_4_3_2_0_1()

        => ExecutionTestFromFile("15.4.3.2-0-1");



    [Fact(DisplayName = "15.4.3.2-0-2")]

    public Task _15_4_3_2_0_2()

        => ExecutionTestFromFile("15.4.3.2-0-2");



    [Fact(DisplayName = "15.4.3.2-0-3")]

    public Task _15_4_3_2_0_3()

        => ExecutionTestFromFile("15.4.3.2-0-3");



    [Fact(DisplayName = "15.4.3.2-0-4")]

    public Task _15_4_3_2_0_4()

        => ExecutionTestFromFile("15.4.3.2-0-4");



    [Fact(DisplayName = "15.4.3.2-1-1")]

    public Task _15_4_3_2_1_1()

        => ExecutionTestFromFile("15.4.3.2-1-1");

}

