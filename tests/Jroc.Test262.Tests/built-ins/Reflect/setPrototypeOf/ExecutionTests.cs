using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.setPrototypeOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Reflect.setPrototypeOf") { }

    [Fact(DisplayName = "return-true-if-new-prototype-is-set")]
    public Task return_true_if_new_prototype_is_set()
        => ExecutionTestFromFile("return-true-if-new-prototype-is-set");
}
