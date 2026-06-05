using Js2IL.Test262.Tests.language.modules;

namespace Js2IL.Test262.Tests.language.expressions.await_;

public class ExecutionTests : Js2IL.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\expressions\await", "language.expressions.await_") { }

    [Fact(DisplayName = "await-BindingIdentifier-nested")]
    public Task await_BindingIdentifier_nested()
        => CompilationFailureTest("await-BindingIdentifier-nested");



    [Fact(DisplayName = "early-errors-await-not-simple-assignment-target")]
    public Task early_errors_await_not_simple_assignment_target()
        => CompilationFailureTest("early-errors-await-not-simple-assignment-target");

    [Fact(DisplayName = "no-operand")]
    public Task no_operand()
        => CompilationFailureTest("no-operand");

}
