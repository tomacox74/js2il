namespace Jroc.Test262.Tests.language.expressions.dynamic_import.import_defer.sync;

public sealed class DynamicImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public DynamicImportCompletionExecutionTests() : base("language.expressions.dynamic_import.import_defer.sync") { }

    [Fact(DisplayName = "main.js")]
    public Task main()
        => ExecutionTest("main");
}
