using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.export;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\export", "language.export") { }

    [Fact(DisplayName = "escaped-as-export-specifier")]
    public Task escaped_as_export_specifier()
        => CompilationFailureTest("escaped-as-export-specifier");

    [Fact(DisplayName = "escaped-default")]
    public Task escaped_default()
        => CompilationFailureTest("escaped-default");

    [Fact(DisplayName = "escaped-from")]
    public Task escaped_from()
        => CompilationFailureTest("escaped-from");

}
