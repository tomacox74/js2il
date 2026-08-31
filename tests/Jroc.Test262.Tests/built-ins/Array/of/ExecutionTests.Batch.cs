namespace Jroc.Test262.Tests.built_ins.Array.of;

public partial class ExecutionTests
{
    [Fact(DisplayName = "return-abrupt-from-data-property")]
    public Task return_abrupt_from_data_property() => ExecutionTestFromFile("return-abrupt-from-data-property");
}
