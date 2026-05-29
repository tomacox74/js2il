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

    [Fact(DisplayName = "AsyncFunction-length")]
    public Task AsyncFunction_length()
        => ExecutionTestFromFile("AsyncFunction-length");

    [Fact(DisplayName = "AsyncFunction-name")]
    public Task AsyncFunction_name()
        => ExecutionTestFromFile("AsyncFunction-name");

    [Fact(DisplayName = "AsyncFunction-prototype")]
    public Task AsyncFunction_prototype()
        => ExecutionTestFromFile("AsyncFunction-prototype");

    [Fact(DisplayName = "AsyncFunctionPrototype-is-not-callable")]
    public Task AsyncFunctionPrototype_is_not_callable()
        => ExecutionTestFromFile("AsyncFunctionPrototype-is-not-callable");

    [Fact(DisplayName = "AsyncFunctionPrototype-to-string")]
    public Task AsyncFunctionPrototype_to_string()
        => ExecutionTestFromFile("AsyncFunctionPrototype-to-string");

    [Fact(DisplayName = "instance-construct-throws")]
    public Task instance_construct_throws()
        => ExecutionTestFromFile("instance-construct-throws");

    [Fact(DisplayName = "instance-has-name")]
    public Task instance_has_name()
        => ExecutionTestFromFile("instance-has-name");

    [Fact(DisplayName = "instance-length")]
    public Task instance_length()
        => ExecutionTestFromFile("instance-length");

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor()
        => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "is-not-a-global")]
    public Task is_not_a_global()
        => ExecutionTestFromFile("is-not-a-global");
}
