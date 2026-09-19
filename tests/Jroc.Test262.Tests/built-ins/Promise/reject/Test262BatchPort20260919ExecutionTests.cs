using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.reject;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Promise.reject") { }

    [Fact(DisplayName = "S25.4.4.4_A3.1_T1")]
    public Task S25_4_4_4_A3_1_T1()
        => ExecutionTestFromFile("S25.4.4.4_A3.1_T1");

    [Fact(DisplayName = "ctx-ctor-throws")]
    public Task ctx_ctor_throws()
        => ExecutionTestFromFile("ctx-ctor-throws");

    [Fact(DisplayName = "ctx-non-ctor")]
    public Task ctx_non_ctor()
        => ExecutionTestFromFile("ctx-non-ctor");

    [Fact(DisplayName = "ctx-non-object")]
    public Task ctx_non_object()
        => ExecutionTestFromFile("ctx-non-object");

}
