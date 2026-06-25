using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.fill;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.fill") { }

    [Fact(DisplayName = "fill-values")]
    public Task fill_values()
        => ExecutionTestFromFile("fill-values");

    [Fact(DisplayName = "fill-values-custom-start-and-end")]
    public Task fill_values_custom_start_and_end()
        => ExecutionTestFromFile("fill-values-custom-start-and-end");

    [Fact(DisplayName = "fill-values-relative-end")]
    public Task fill_values_relative_end()
        => ExecutionTestFromFile("fill-values-relative-end");

    [Fact(DisplayName = "fill-values-relative-start")]
    public Task fill_values_relative_start()
        => ExecutionTestFromFile("fill-values-relative-start");


}
