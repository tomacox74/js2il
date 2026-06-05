using Js2IL.Test262.Tests.language.modules;

namespace Js2IL.Test262.Tests.language.import;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\import", "language.import") { }

    [Fact(DisplayName = "dup-bound-names")]
    public Task dup_bound_names()
        => CompilationFailureTest("dup-bound-names");

    [Fact(DisplayName = "escaped-as-import-specifier")]
    public Task escaped_as_import_specifier()
        => CompilationFailureTest("escaped-as-import-specifier");

    [Fact(DisplayName = "escaped-as-namespace-import")]
    public Task escaped_as_namespace_import()
        => CompilationFailureTest("escaped-as-namespace-import");

    [Fact(DisplayName = "escaped-from")]
    public Task escaped_from()
        => CompilationFailureTest("escaped-from");

    [Fact(DisplayName = "import-defer/syntax/invalid-default-and-defer-namespace")]
    public Task import_defer_syntax_invalid_default_and_defer_namespace()
        => CompilationFailureTest("import-defer/syntax/invalid-default-and-defer-namespace");

    [Fact(DisplayName = "import-defer/syntax/invalid-defer-as-with-no-asterisk")]
    public Task import_defer_syntax_invalid_defer_as_with_no_asterisk()
        => CompilationFailureTest("import-defer/syntax/invalid-defer-as-with-no-asterisk");

    [Fact(DisplayName = "import-defer/syntax/invalid-defer-default-and-namespace")]
    public Task import_defer_syntax_invalid_defer_default_and_namespace()
        => CompilationFailureTest("import-defer/syntax/invalid-defer-default-and-namespace");

    [Fact(DisplayName = "import-defer/syntax/invalid-defer-default")]
    public Task import_defer_syntax_invalid_defer_default()
        => CompilationFailureTest("import-defer/syntax/invalid-defer-default");

    [Fact(DisplayName = "import-defer/syntax/invalid-defer-named")]
    public Task import_defer_syntax_invalid_defer_named()
        => CompilationFailureTest("import-defer/syntax/invalid-defer-named");

    [Fact(DisplayName = "import-defer/syntax/invalid-export-defer-namespace")]
    public Task import_defer_syntax_invalid_export_defer_namespace()
        => CompilationFailureTest("import-defer/syntax/invalid-export-defer-namespace");

}
