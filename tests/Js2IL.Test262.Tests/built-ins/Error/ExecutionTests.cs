using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Error;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Error") { }

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "internal-prototype")]
    public Task internal_prototype()
        => ExecutionTestFromFile("internal-prototype");

    [Fact(DisplayName = "prototype/no-error-data")]
    public Task prototype_no_error_data()
        => ExecutionTestFromFile("prototype/no-error-data");

    [Fact(DisplayName = "prototype/S15.11.4_A1")]
    public Task prototype_S15_11_4_A1()
        => ExecutionTestFromFile("prototype/S15.11.4_A1");

    [Fact(DisplayName = "prototype/S15.11.4_A2")]
    public Task prototype_S15_11_4_A2()
        => ExecutionTestFromFile("prototype/S15.11.4_A2");

    [Fact(DisplayName = "prototype/S15.11.4_A3")]
    public Task prototype_S15_11_4_A3()
        => ExecutionTestFromFile("prototype/S15.11.4_A3");

    [Fact(DisplayName = "prototype/S15.11.4_A4")]
    public Task prototype_S15_11_4_A4()
        => ExecutionTestFromFile("prototype/S15.11.4_A4");

}
