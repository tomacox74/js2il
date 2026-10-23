namespace Jroc.Test262.Tests.language.import.import_defer.errors.syntax_error;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.import.import_defer.errors.syntax_error") { }

    [Fact(DisplayName = "import-defer-of-syntax-error-fails.js")]
    public Task import_defer_of_syntax_error_fails()
        => CompilationFailureTest("import-defer-of-syntax-error-fails");
}
