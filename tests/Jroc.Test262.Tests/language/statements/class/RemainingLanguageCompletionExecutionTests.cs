namespace Jroc.Test262.Tests.language.statements.class_;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.class_") { }

    [Fact(DisplayName = "static-init-arguments-methods.js")]
    public Task static_init_arguments_methods()
        => ExecutionTest("static-init-arguments-methods");

    [Fact(DisplayName = "static-init-super-property.js")]
    public Task static_init_super_property()
        => ExecutionTest("static-init-super-property");
}
