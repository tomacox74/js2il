namespace Jroc.Test262.Tests.built_ins.Array.of;

public partial class ExecutionTests
{
    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "return-abrupt-from-data-property")]
    public Task return_abrupt_from_data_property() => ExecutionTestFromFile("return-abrupt-from-data-property");

    [Fact(DisplayName = "return-abrupt-from-data-property-using-proxy.js")]
    public Task return_abrupt_from_data_property_using_proxy()
        => ExecutionTestFromFile("return-abrupt-from-data-property-using-proxy");
}
