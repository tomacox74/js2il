using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.setPrototypeOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Reflect.setPrototypeOf") { }

    [Fact(DisplayName = "return-true-if-new-prototype-is-set")]
    public Task return_true_if_new_prototype_is_set()
        => ExecutionTestFromFile("return-true-if-new-prototype-is-set");

    [Fact(DisplayName = "proto-is-not-object-and-not-null-throws")]
    public Task proto_is_not_object_and_not_null_throws()
        => ExecutionTestFromFile("proto-is-not-object-and-not-null-throws");
}
