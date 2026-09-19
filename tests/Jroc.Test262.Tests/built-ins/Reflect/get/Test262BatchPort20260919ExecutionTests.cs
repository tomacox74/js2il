using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.get;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Reflect.get") { }

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "return-abrupt-from-property-key")]
    public Task return_abrupt_from_property_key()
        => ExecutionTestFromFile("return-abrupt-from-property-key");

    [Fact(DisplayName = "return-abrupt-from-result")]
    public Task return_abrupt_from_result()
        => ExecutionTestFromFile("return-abrupt-from-result");

    [Fact(DisplayName = "target-is-symbol-throws")]
    public Task target_is_symbol_throws()
        => ExecutionTestFromFile("target-is-symbol-throws");

}
