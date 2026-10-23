namespace Jroc.Test262.Tests.language.import.import_defer.errors.resolution_error;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.import.import_defer.errors.resolution_error") { }

    [Fact(DisplayName = "import-defer-of-missing-module-fails.js")]
    public Task import_defer_of_missing_module_fails()
        => ExecutionTest("import-defer-of-missing-module-fails");
}
