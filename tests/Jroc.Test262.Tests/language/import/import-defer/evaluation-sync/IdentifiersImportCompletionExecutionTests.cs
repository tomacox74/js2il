namespace Jroc.Test262.Tests.language.import.import_defer.evaluation_sync;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.import.import_defer.evaluation_sync") { }

    [Fact(DisplayName = "import-defer-does-not-evaluate.js")]
    public Task import_defer_does_not_evaluate()
        => ExecutionTest("import-defer-does-not-evaluate");

    [Fact(DisplayName = "module-imported-defer-and-eager.js")]
    public Task module_imported_defer_and_eager()
        => ExecutionTest("module-imported-defer-and-eager");
}
