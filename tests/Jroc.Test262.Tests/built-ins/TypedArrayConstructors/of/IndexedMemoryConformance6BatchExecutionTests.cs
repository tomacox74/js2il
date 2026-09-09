using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.of;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArrayConstructors.of") { }

    [Fact(DisplayName = "argument-is-symbol-throws.js")]
    public Task argument_is_symbol_throws() => ExecutionTestFromFile("argument-is-symbol-throws");

    [Fact(DisplayName = "argument-number-value-throws.js")]
    public Task argument_number_value_throws() => ExecutionTestFromFile("argument-number-value-throws");

    [Fact(DisplayName = "custom-ctor-returns-other-instance.js")]
    public Task custom_ctor_returns_other_instance() => ExecutionTestFromFile("custom-ctor-returns-other-instance");

    [Fact(DisplayName = "custom-ctor.js")]
    public Task custom_ctor() => ExecutionTestFromFile("custom-ctor");

    [Fact(DisplayName = "inherited.js")]
    public Task inherited() => ExecutionTestFromFile("inherited");

    [Fact(DisplayName = "invoked-as-func.js")]
    public Task invoked_as_func() => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "nan-conversion.js")]
    public Task nan_conversion() => ExecutionTestFromFile("nan-conversion");

    [Fact(DisplayName = "new-instance-empty.js")]
    public Task new_instance_empty() => ExecutionTestFromFile("new-instance-empty");

    [Fact(DisplayName = "new-instance-from-zero.js")]
    public Task new_instance_from_zero() => ExecutionTestFromFile("new-instance-from-zero");

    [Fact(DisplayName = "new-instance-using-custom-ctor.js")]
    public Task new_instance_using_custom_ctor() => ExecutionTestFromFile("new-instance-using-custom-ctor");

    [Fact(DisplayName = "new-instance.js")]
    public Task new_instance() => ExecutionTestFromFile("new-instance");

    [Fact(DisplayName = "this-is-not-constructor.js")]
    public Task this_is_not_constructor() => ExecutionTestFromFile("this-is-not-constructor");
}
