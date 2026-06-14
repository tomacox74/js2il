using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayIteratorPrototype.next;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.ArrayIteratorPrototype.next") { }

    [Fact(DisplayName = "Uint8Array")]
    public Task Uint8Array()
        => ExecutionTestFromFile("Uint8Array");

    [Fact(DisplayName = "Int8Array")]
    public Task Int8Array()
        => ExecutionTestFromFile("Int8Array");

    [Fact(DisplayName = "Int16Array")]
    public Task Int16Array()
        => ExecutionTestFromFile("Int16Array");

    [Fact(DisplayName = "Int32Array")]
    public Task Int32Array()
        => ExecutionTestFromFile("Int32Array");

    [Fact(DisplayName = "Float32Array")]
    public Task Float32Array()
        => ExecutionTestFromFile("Float32Array");

    [Fact(DisplayName = "Float64Array")]
    public Task Float64Array()
        => ExecutionTestFromFile("Float64Array");

    [Fact(DisplayName = "iteration")]
    public Task iteration()
        => ExecutionTestFromFile("iteration");

    [Fact(DisplayName = "property-descriptor")]
    public Task property_descriptor()
        => ExecutionTestFromFile("property-descriptor");
}
