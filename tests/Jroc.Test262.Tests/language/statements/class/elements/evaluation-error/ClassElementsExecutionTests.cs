namespace Jroc.Test262.Tests.language.statements.test_class.elements.evaluation_error;

public sealed class ClassElementsExecutionTests : DiskExecutionTestsBase
{
    public ClassElementsExecutionTests() : base("language.statements.test_class.elements.evaluation_error") { }

    [Fact(DisplayName = "computed-name-referenceerror.js")]
    public Task computed_name_referenceerror()
        => ExecutionTest("computed-name-referenceerror");

    [Fact(DisplayName = "computed-name-toprimitive-err.js")]
    public Task computed_name_toprimitive_err()
        => ExecutionTest("computed-name-toprimitive-err");

    [Fact(DisplayName = "computed-name-toprimitive-returns-noncallable.js")]
    public Task computed_name_toprimitive_returns_noncallable()
        => ExecutionTest("computed-name-toprimitive-returns-noncallable");

    [Fact(DisplayName = "computed-name-toprimitive-returns-nonobject.js")]
    public Task computed_name_toprimitive_returns_nonobject()
        => ExecutionTest("computed-name-toprimitive-returns-nonobject");

    [Fact(DisplayName = "computed-name-tostring-err.js")]
    public Task computed_name_tostring_err()
        => ExecutionTest("computed-name-tostring-err");

    [Fact(DisplayName = "computed-name-valueof-err.js")]
    public Task computed_name_valueof_err()
        => ExecutionTest("computed-name-valueof-err");
}
