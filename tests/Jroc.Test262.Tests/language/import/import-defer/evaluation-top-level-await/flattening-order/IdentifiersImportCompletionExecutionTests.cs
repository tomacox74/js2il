namespace Jroc.Test262.Tests.language.import.import_defer.evaluation_top_level_await.flattening_order;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.import.import_defer.evaluation_top_level_await.flattening_order") { }

    [Fact(DisplayName = "main.js")]
    public Task main()
        => ExecutionTest("main");
}
