using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.of.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArrayConstructors.of.BigInt") { }

    [Fact(DisplayName = "argument-is-symbol-throws.js")]
    public Task argument_is_symbol_throws() => ExecutionTestFromFile("argument-is-symbol-throws");

    [Fact(DisplayName = "argument-number-value-throws.js")]
    public Task argument_number_value_throws() => ExecutionTestFromFile("argument-number-value-throws");

    [Fact(DisplayName = "custom-ctor-returns-smaller-instance-throws.js")]
    public Task custom_ctor_returns_smaller_instance_throws() => ExecutionTestFromFile("custom-ctor-returns-smaller-instance-throws");

    [Fact(DisplayName = "custom-ctor.js")]
    public Task custom_ctor() => ExecutionTestFromFile("custom-ctor");

    [Fact(DisplayName = "inherited.js")]
    public Task inherited() => ExecutionTestFromFile("inherited");

    [Fact(DisplayName = "invoked-as-func.js")]
    public Task invoked_as_func() => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "new-instance-empty.js")]
    public Task new_instance_empty() => ExecutionTestFromFile("new-instance-empty");

    [Fact(DisplayName = "new-instance-using-custom-ctor.js")]
    public Task new_instance_using_custom_ctor() => ExecutionTestFromFile("new-instance-using-custom-ctor");

    [Fact(DisplayName = "new-instance.js")]
    public Task new_instance() => ExecutionTestFromFile("new-instance");

    [Fact(DisplayName = "this-is-not-constructor.js")]
    public Task this_is_not_constructor() => ExecutionTestFromFile("this-is-not-constructor");
}
