using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.subclass_builtins;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class_.subclass-builtins") { }

    [Fact(DisplayName = "subclass-Function.js")]
    public Task ported_subclass_Function() => ExecutionTest("subclass-Function");

}
