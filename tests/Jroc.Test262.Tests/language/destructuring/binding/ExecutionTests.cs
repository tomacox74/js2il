using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.destructuring.binding;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.destructuring.binding") { }

    [Fact(DisplayName = "initialization-returns-normal-completion-for-empty-objects")]
    public Task initialization_returns_normal_completion_for_empty_objects()
        => ExecutionTest("initialization-returns-normal-completion-for-empty-objects");

    [Fact(DisplayName = "initialization-requires-object-coercible-null")]
    public Task initialization_requires_object_coercible_null()
        => ExecutionTest("initialization-requires-object-coercible-null");

    [Fact(DisplayName = "initialization-requires-object-coercible-undefined")]
    public Task initialization_requires_object_coercible_undefined()
        => ExecutionTest("initialization-requires-object-coercible-undefined");

    [Fact(DisplayName = "keyed-destructuring-property-reference-target-evaluation-order-with-bindings")]
    public Task keyed_destructuring_property_reference_target_evaluation_order_with_bindings()
        => ExecutionTest("keyed-destructuring-property-reference-target-evaluation-order-with-bindings");
}
