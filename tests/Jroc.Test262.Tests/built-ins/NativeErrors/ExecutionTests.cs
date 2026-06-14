using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.NativeErrors;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.NativeErrors") { }

    [Fact(DisplayName = "EvalError/constructor")]
    public Task EvalError_constructor()
        => ExecutionTestFromFile("EvalError/constructor");

    [Fact(DisplayName = "ReferenceError/constructor")]
    public Task ReferenceError_constructor()
        => ExecutionTestFromFile("ReferenceError/constructor");

    [Fact(DisplayName = "RangeError/constructor")]
    public Task RangeError_constructor()
        => ExecutionTestFromFile("RangeError/constructor");

    [Fact(DisplayName = "SyntaxError/constructor")]
    public Task SyntaxError_constructor()
        => ExecutionTestFromFile("SyntaxError/constructor");

    [Fact(DisplayName = "TypeError/constructor")]
    public Task TypeError_constructor()
        => ExecutionTestFromFile("TypeError/constructor");

    [Fact(DisplayName = "URIError/constructor")]
    public Task URIError_constructor()
        => ExecutionTestFromFile("URIError/constructor");

    [Fact(DisplayName = "EvalError/instance-proto")]
    public Task EvalError_instance_proto()
        => ExecutionTestFromFile("EvalError/instance-proto");

    [Fact(DisplayName = "TypeError/instance-proto")]
    public Task TypeError_instance_proto()
        => ExecutionTestFromFile("TypeError/instance-proto");

}
