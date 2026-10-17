namespace Jroc.Test262.Tests.language.expressions.dynamic_import.import_defer.import_defer_transitive_async_module;

public sealed class DynamicImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public DynamicImportCompletionExecutionTests() : base("language.expressions.dynamic_import.import_defer.import_defer_transitive_async_module") { }

    [Fact(DisplayName = "main.js")]
    public Task main()
        => ExecutionTest("main");

    [Fact(DisplayName = "promise-prototype-then-not-called.js")]
    public Task promise_prototype_then_not_called()
        => ExecutionTest("promise-prototype-then-not-called");
}
