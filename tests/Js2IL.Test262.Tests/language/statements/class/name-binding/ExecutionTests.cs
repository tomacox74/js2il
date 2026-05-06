using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.class_.name_binding;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.name_binding") { }

[Fact(DisplayName = "basic")]
    public Task basic()
        => ExecutionTest("basic");

[Fact(DisplayName = "const", Skip = "Known issue: compiler cannot yet compile this test262 scenario")]
    public Task const_()
        => ExecutionTest("const");
}
