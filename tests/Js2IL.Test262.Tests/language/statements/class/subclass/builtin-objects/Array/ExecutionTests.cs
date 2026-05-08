using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.statements.class_.subclass.builtin_objects.Array;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.subclass.builtin_objects.Array") { }

    [Fact(DisplayName = "contructor-calls-super-multiple-arguments")]
    public Task contructor_calls_super_multiple_arguments()
        => ExecutionTest("contructor-calls-super-multiple-arguments");
}
