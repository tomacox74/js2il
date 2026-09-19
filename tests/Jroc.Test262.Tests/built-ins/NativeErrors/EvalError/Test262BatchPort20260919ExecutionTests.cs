using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.NativeErrors.EvalError;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.NativeErrors.EvalError") { }

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor()
        => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto")]
    public Task proto()
        => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "prototype")]
    public Task prototype()
        => ExecutionTestFromFile("prototype");

}
