namespace Jroc.Test262.Tests.language.expressions.dynamic_import.import_defer.import_defer_async_module;

public sealed class DynamicImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public DynamicImportCompletionExecutionTests() : base("language.expressions.dynamic_import.import_defer.import_defer_async_module") { }

    [Fact(DisplayName = "main.js")]
    public Task main()
        => ExecutionTest("main");
}
