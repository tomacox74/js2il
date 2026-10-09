using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.Test_class.decorator.syntax.class_valid;

public sealed class ClassCompletionExecutionTests : DiskExecutionTestsBase
{
    public ClassCompletionExecutionTests() : base("Jroc.Test262.Tests.language.expressions.Test_class.decorator.syntax.class_valid") { }

    [Fact(DisplayName = "decorator-member-expr-private-identifier.js")]
    public Task decorator_member_expr_private_identifier()
        => ExecutionTestFromFile("decorator-member-expr-private-identifier");
}
