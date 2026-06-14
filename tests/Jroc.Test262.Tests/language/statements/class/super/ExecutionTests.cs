using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.class_.super;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.super") { }

    [Fact(DisplayName = "in-constructor-superproperty-evaluation")]
    public Task in_constructor_superproperty_evaluation()
        => ExecutionTest("in-constructor-superproperty-evaluation");

    [Fact(DisplayName = "in-constructor")]
    public Task in_constructor()
        => ExecutionTest("in-constructor");
}
