using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.defineProperty;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Reflect.defineProperty") { }

    [Fact(DisplayName = "define-properties")]
    public Task define_properties()
        => ExecutionTestFromFile("define-properties");

    [Fact(DisplayName = "define-symbol-properties")]
    public Task define_symbol_properties()
        => ExecutionTestFromFile("define-symbol-properties");

    [Fact(DisplayName = "defineProperty")]
    public Task defineProperty()
        => ExecutionTestFromFile("defineProperty");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");
}
