using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.NativeErrors.URIError;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.NativeErrors.URIError") { }

    [Fact(DisplayName = "instance-proto")]
    public Task instance_proto()
        => ExecutionTestFromFile("instance-proto");

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor()
        => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "is-error-object")]
    public Task is_error_object()
        => ExecutionTestFromFile("is-error-object");

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
