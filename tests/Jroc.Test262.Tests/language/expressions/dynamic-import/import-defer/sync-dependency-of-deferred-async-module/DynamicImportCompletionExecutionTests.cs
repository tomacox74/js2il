namespace Jroc.Test262.Tests.language.expressions.dynamic_import.import_defer.sync_dependency_of_deferred_async_module;

public sealed class DynamicImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public DynamicImportCompletionExecutionTests() : base("language.expressions.dynamic_import.import_defer.sync_dependency_of_deferred_async_module") { }

    [Fact(DisplayName = "main.js")]
    public Task main()
        => ExecutionTest("main");
}
