using Js2IL.Test262.Tests.built_ins;



namespace Js2IL.Test262.Tests.built_ins.parseInt;



public class ExecutionTests : DiskExecutionTestsBase

{

    public ExecutionTests() : base("built_ins.parseInt") { }



    [Fact(DisplayName = "15.1.2.2-2-1")]

    public Task _15_1_2_2_2_1()

        => ExecutionTestFromFile("15.1.2.2-2-1");



    [Fact(DisplayName = "S15.1.2.2_A1_T1")]

    public Task S15_1_2_2_A1_T1()

        => ExecutionTestFromFile("S15.1.2.2_A1_T1");



    [Fact(DisplayName = "S15.1.2.2_A1_T2")]

    public Task S15_1_2_2_A1_T2()

        => ExecutionTestFromFile("S15.1.2.2_A1_T2");



    [Fact(DisplayName = "S15.1.2.2_A1_T3")]

    public Task S15_1_2_2_A1_T3()

        => ExecutionTestFromFile("S15.1.2.2_A1_T3");



    [Fact(DisplayName = "S15.1.2.2_A2_T1")]

    public Task S15_1_2_2_A2_T1()

        => ExecutionTestFromFile("S15.1.2.2_A2_T1");

}

