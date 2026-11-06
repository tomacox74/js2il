using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.module_code.import_attributes;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base("language/module-code/import-attributes", "language.module_code.import_attributes") { }

    [Fact(DisplayName = "import-attribute-empty")]
    public Task import_attribute_empty() => ExecutionTest("import-attribute-empty");
}
