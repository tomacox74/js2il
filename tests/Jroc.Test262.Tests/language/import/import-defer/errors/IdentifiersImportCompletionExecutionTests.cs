namespace Jroc.Test262.Tests.language.import.import_defer.errors;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.import.import_defer.errors") { }

    [Fact(DisplayName = "get-self-while-evaluating.js")]
    public Task get_self_while_evaluating()
        => ExecutionTest("get-self-while-evaluating");
}
