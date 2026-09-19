using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.defineProperty;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Reflect.defineProperty") { }

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "return-abrupt-from-attributes")]
    public Task return_abrupt_from_attributes()
        => ExecutionTestFromFile("return-abrupt-from-attributes");

    [Fact(DisplayName = "return-abrupt-from-result")]
    public Task return_abrupt_from_result()
        => ExecutionTestFromFile("return-abrupt-from-result");

    [Fact(DisplayName = "target-is-symbol-throws")]
    public Task target_is_symbol_throws()
        => ExecutionTestFromFile("target-is-symbol-throws");

}
