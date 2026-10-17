namespace Jroc.Test262.Tests.language.import.import_defer.errors.get_other_while_evaluating_async;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.import.import_defer.errors.get_other_while_evaluating_async") { }

    [Fact(DisplayName = "main.js")]
    public Task main()
        => ExecutionTest("main");
}
