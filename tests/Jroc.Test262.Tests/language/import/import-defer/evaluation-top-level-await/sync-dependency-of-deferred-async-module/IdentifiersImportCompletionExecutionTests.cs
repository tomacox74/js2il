namespace Jroc.Test262.Tests.language.import.import_defer.evaluation_top_level_await.sync_dependency_of_deferred_async_module;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.import.import_defer.evaluation_top_level_await.sync_dependency_of_deferred_async_module") { }

    [Fact(DisplayName = "main.js")]
    public Task main()
        => ExecutionTest("main");
}
