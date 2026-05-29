using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Map.prototype.set;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.prototype.set") { }

    [Fact(DisplayName = "set")]
    public Task set()
        => ExecutionTestFromFile("set");

    [Fact(DisplayName = "append-new-values-return-map")]
    public Task append_new_values_return_map()
        => ExecutionTestFromFile("append-new-values-return-map");

    [Fact(DisplayName = "replaces-a-value")]
    public Task replaces_a_value()
        => ExecutionTestFromFile("replaces-a-value");

    [Fact(DisplayName = "append-new-values-normalizes-zero-key")]
    public Task append_new_values_normalizes_zero_key()
        => ExecutionTestFromFile("append-new-values-normalizes-zero-key");

    [Fact(DisplayName = "this-not-object-throw")]
    public Task this_not_object_throw()
        => ExecutionTestFromFile("this-not-object-throw");
}
