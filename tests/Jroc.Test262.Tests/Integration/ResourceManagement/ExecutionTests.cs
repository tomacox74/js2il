namespace Jroc.Test262.Tests.Integration.ResourceManagement;

public sealed class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("Integration.ResourceManagement") { }

    [Fact(DisplayName = "resourceDeclarationCompletions.js")]
    public Task resource_declaration_completions()
        => ExecutionTest("resourceDeclarationCompletions");
}
