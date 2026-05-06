using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.class_.super;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.super") { }

[Fact(DisplayName = "in-constructor-superproperty-evaluation", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task in_constructor_superproperty_evaluation()
        => ExecutionTest("in-constructor-superproperty-evaluation");

[Fact(DisplayName = "in-constructor", Skip = "Known issue: compiler cannot yet compile this test262 scenario")]
    public Task in_constructor()
        => ExecutionTest("in-constructor");
}
