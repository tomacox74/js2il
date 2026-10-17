namespace Jroc.Test262.Tests.language.import.import_defer.errors.get_self_while_defer_evaluating;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.import.import_defer.errors.get_self_while_defer_evaluating") { }

    [Fact(DisplayName = "main.js")]
    public Task main()
        => ExecutionTest("main");
}
