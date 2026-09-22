using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.assignmenttargettype;

public class Test262BatchPort20260922Round7ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260922Round7ExecutionTests() : base("language.expressions.assignmenttargettype") { }

    [Fact(DisplayName = "simple-basic-identifierreference-arguments")]
    public Task simple_basic_identifierreference_arguments()
        => ExecutionTest("simple-basic-identifierreference-arguments");

    [Fact(DisplayName = "simple-basic-identifierreference-await")]
    public Task simple_basic_identifierreference_await()
        => ExecutionTest("simple-basic-identifierreference-await");

    [Fact(DisplayName = "simple-basic-identifierreference-eval")]
    public Task simple_basic_identifierreference_eval()
        => ExecutionTest("simple-basic-identifierreference-eval");

    [Fact(DisplayName = "simple-basic-identifierreference-yield")]
    public Task simple_basic_identifierreference_yield()
        => ExecutionTest("simple-basic-identifierreference-yield");

    [Fact(DisplayName = "simple-complex-callexpression-expression")]
    public Task simple_complex_callexpression_expression()
        => ExecutionTest("simple-complex-callexpression-expression");

    [Fact(DisplayName = "simple-complex-callexpression.identifiername")]
    public Task simple_complex_callexpression_identifiername()
        => ExecutionTest("simple-complex-callexpression.identifiername");

    [Fact(DisplayName = "simple-complex-memberexpression-expression")]
    public Task simple_complex_memberexpression_expression()
        => ExecutionTest("simple-complex-memberexpression-expression");

    [Fact(DisplayName = "simple-complex-memberexpression.identifiername")]
    public Task simple_complex_memberexpression_identifiername()
        => ExecutionTest("simple-complex-memberexpression.identifiername");

}
