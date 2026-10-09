using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.Test_class.elements.evaluation_error;

public sealed class ClassCompletionExecutionTests : DiskExecutionTestsBase
{
    public ClassCompletionExecutionTests() : base("Jroc.Test262.Tests.language.expressions.Test_class.elements.evaluation_error") { }

    [Fact(DisplayName = "computed-name-referenceerror.js")]
    public Task computed_name_referenceerror()
        => ExecutionTestFromFile("computed-name-referenceerror");

    [Fact(DisplayName = "computed-name-toprimitive-err.js")]
    public Task computed_name_toprimitive_err()
        => ExecutionTestFromFile("computed-name-toprimitive-err");

    [Fact(DisplayName = "computed-name-toprimitive-returns-noncallable.js")]
    public Task computed_name_toprimitive_returns_noncallable()
        => ExecutionTestFromFile("computed-name-toprimitive-returns-noncallable");

    [Fact(DisplayName = "computed-name-toprimitive-returns-nonobject.js")]
    public Task computed_name_toprimitive_returns_nonobject()
        => ExecutionTestFromFile("computed-name-toprimitive-returns-nonobject");

    [Fact(DisplayName = "computed-name-tostring-err.js")]
    public Task computed_name_tostring_err()
        => ExecutionTestFromFile("computed-name-tostring-err");

    [Fact(DisplayName = "computed-name-valueof-err.js")]
    public Task computed_name_valueof_err()
        => ExecutionTestFromFile("computed-name-valueof-err");
}
