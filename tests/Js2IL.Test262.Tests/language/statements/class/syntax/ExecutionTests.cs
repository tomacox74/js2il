using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.statements.class_.syntax;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.syntax") { }

    [Fact(DisplayName = "class-body-has-direct-super-class-heritage")]
    public Task class_body_has_direct_super_class_heritage()
        => ExecutionTest("class-body-has-direct-super-class-heritage");
}
