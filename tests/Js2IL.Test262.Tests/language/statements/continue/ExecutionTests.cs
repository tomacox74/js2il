using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.continue_;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\continue", "language.statements.continue_") { }

    [Fact(DisplayName = "12.7-1")]
    public Task _12_7_1()
        => ExecutionTest("12.7-1");

    [Fact(DisplayName = "labeled-continue")]
    public Task labeled_continue()
        => ExecutionTest("labeled-continue");

    [Fact(DisplayName = "no-label-continue")]
    public Task no_label_continue()
        => ExecutionTest("no-label-continue");

    [Fact(DisplayName = "simple-and-labeled")]
    public Task simple_and_labeled()
        => ExecutionTest("simple-and-labeled");
}
