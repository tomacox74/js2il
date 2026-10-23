namespace Jroc.Test262.Tests.language.import.import_defer.deferred_namespace_object;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.import.import_defer.deferred_namespace_object") { }

    [Fact(DisplayName = "exotic-object-behavior.js")]
    public Task exotic_object_behavior()
        => ExecutionTest("exotic-object-behavior");

    [Fact(DisplayName = "identity.js")]
    public Task identity()
        => ExecutionTest("identity");

    [Fact(DisplayName = "reexport-deferred-ns-evaluation.js")]
    public Task reexport_deferred_ns_evaluation()
        => ExecutionTest("reexport-deferred-ns-evaluation");

    [Fact(DisplayName = "to-string-tag.js")]
    public Task to_string_tag()
        => ExecutionTest("to-string-tag");
}
