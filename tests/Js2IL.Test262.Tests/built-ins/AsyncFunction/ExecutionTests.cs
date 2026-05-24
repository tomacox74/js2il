using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.AsyncFunction;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncFunction") { }

    [Fact(DisplayName = "AsyncFunction-construct")]
    public Task AsyncFunction_construct()
        => ExecutionTestFromFile("AsyncFunction-construct");

    [Fact(DisplayName = "AsyncFunction-is-extensible")]
    public Task AsyncFunction_is_extensible()
        => ExecutionTestFromFile("AsyncFunction-is-extensible");

    [Fact(DisplayName = "AsyncFunction-is-subclass")]
    public Task AsyncFunction_is_subclass()
        => ExecutionTestFromFile("AsyncFunction-is-subclass");

    [Fact(DisplayName = "AsyncFunction")]
    public Task AsyncFunction()
        => ExecutionTestFromFile("AsyncFunction");

    [Fact(DisplayName = "AsyncFunctionPrototype-is-extensible")]
    public Task AsyncFunctionPrototype_is_extensible()
        => ExecutionTestFromFile("AsyncFunctionPrototype-is-extensible");

    [Fact(DisplayName = "AsyncFunctionPrototype-prototype")]
    public Task AsyncFunctionPrototype_prototype()
        => ExecutionTestFromFile("AsyncFunctionPrototype-prototype");

    [Fact(DisplayName = "AsyncFunction_intrinsic")]
    public Task AsyncFunction_intrinsic()
        => ExecutionTestFromFile("AsyncFunction_intrinsic");

    [Fact(DisplayName = "instance-prototype-property")]
    public Task instance_prototype_property()
        => ExecutionTestFromFile("instance-prototype-property");
}
