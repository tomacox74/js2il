namespace Jroc.Test262.Tests.language.import.import_defer.errors.module_throws;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.import.import_defer.errors.module_throws") { }

    [Fact(DisplayName = "defer-import-after-evaluation.js")]
    public Task defer_import_after_evaluation()
        => ExecutionTest("defer-import-after-evaluation");

    [Fact(DisplayName = "third-party-evaluation-after-defer-import.js")]
    public Task third_party_evaluation_after_defer_import()
        => ExecutionTest("third-party-evaluation-after-defer-import");

    [Fact(DisplayName = "trigger-evaluation.js")]
    public Task trigger_evaluation()
        => ExecutionTest("trigger-evaluation");
}
