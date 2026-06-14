using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.class_.name_binding;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.name_binding") { }

    [Fact(DisplayName = "basic")]
    public Task basic()
        => ExecutionTest("basic");

    [Fact(DisplayName = "const")]
    public Task const_()
        => ExecutionTest("const");
}
