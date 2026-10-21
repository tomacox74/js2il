namespace Jroc.Test262.Tests.language.statements.test_class.subclass.builtin_objects.Array;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.subclass.builtin_objects.Array") { }

    [Fact(DisplayName = "contructor-calls-super-single-argument.js")]
    public Task contructor_calls_super_single_argument()
        => ExecutionTest("contructor-calls-super-single-argument");

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTest("length");

    [Fact(DisplayName = "regular-subclassing.js")]
    public Task regular_subclassing()
        => ExecutionTest("regular-subclassing");
}
