using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.module_code.ambiguous_export_bindings;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base("language/module-code/ambiguous-export-bindings", "language.module_code.ambiguous_export_bindings") { }

    [Fact(DisplayName = "import-and-export-propagates-binding")]
    public Task import_and_export_propagates_binding() => ExecutionTest("import-and-export-propagates-binding");
    [Fact(DisplayName = "namespace-unambiguous-if-export-star-as-from-and-import-star-as-and-export")]
    public Task namespace_unambiguous_if_export_star_as_from_and_import_star_as_and_export() => ExecutionTest("namespace-unambiguous-if-export-star-as-from-and-import-star-as-and-export");
    [Fact(DisplayName = "namespace-unambiguous-if-export-star-as-from")]
    public Task namespace_unambiguous_if_export_star_as_from() => ExecutionTest("namespace-unambiguous-if-export-star-as-from");
    [Fact(DisplayName = "namespace-unambiguous-if-import-star-as-and-export")]
    public Task namespace_unambiguous_if_import_star_as_and_export() => ExecutionTest("namespace-unambiguous-if-import-star-as-and-export");
    [Fact(DisplayName = "omitted-from-namespace")]
    public Task omitted_from_namespace() => ExecutionTest("omitted-from-namespace");
}
