using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.property_accessors;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.property_accessors") { }

    [Fact(DisplayName = "S11.2.1_A4_T3")]
    public Task S11_2_1_A4_T3()
        => ExecutionTest("S11.2.1_A4_T3");
}
