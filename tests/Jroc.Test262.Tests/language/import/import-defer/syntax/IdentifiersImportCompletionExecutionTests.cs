namespace Jroc.Test262.Tests.language.import.import_defer.syntax;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.import.import_defer.syntax") { }

    [Fact(DisplayName = "import-attributes.js")]
    public Task import_attributes()
        => ExecutionTest("import-attributes");

    [Fact(DisplayName = "valid-default-binding-named-defer.js")]
    public Task valid_default_binding_named_defer()
        => ExecutionTest("valid-default-binding-named-defer");

    [Fact(DisplayName = "valid-defer-namespace.js")]
    public Task valid_defer_namespace()
        => ExecutionTest("valid-defer-namespace");
}
