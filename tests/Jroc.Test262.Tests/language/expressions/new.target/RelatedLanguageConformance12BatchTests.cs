using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.new_target;

public sealed class RelatedLanguageConformance12BatchTests : DiskExecutionTestsBase
{
    public RelatedLanguageConformance12BatchTests() : base("language.expressions.new_target") { }

    [Fact(DisplayName = "language/expressions/new.target/asi.js")]
    public Task test_asi()
        => ExecutionTest("asi");

    [Fact(DisplayName = "language/expressions/new.target/escaped-new.js")]
    public Task test_escaped_new()
        => CompilationFailureTest("escaped-new", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/new.target/escaped-target.js")]
    public Task test_escaped_target()
        => CompilationFailureTest("escaped-target", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/new.target/value-via-call.js")]
    public Task test_value_via_call()
        => ExecutionTest("value-via-call");

    [Fact(DisplayName = "language/expressions/new.target/value-via-fpapply.js")]
    public Task test_value_via_fpapply()
        => ExecutionTest("value-via-fpapply");

    [Fact(DisplayName = "language/expressions/new.target/value-via-fpcall.js")]
    public Task test_value_via_fpcall()
        => ExecutionTest("value-via-fpcall");

    [Fact(DisplayName = "language/expressions/new.target/value-via-member.js")]
    public Task test_value_via_member()
        => ExecutionTest("value-via-member");

    [Fact(DisplayName = "language/expressions/new.target/value-via-new.js")]
    public Task test_value_via_new()
        => ExecutionTest("value-via-new");

    [Fact(DisplayName = "language/expressions/new.target/value-via-reflect-apply.js")]
    public Task test_value_via_reflect_apply()
        => ExecutionTest("value-via-reflect-apply");

    [Fact(DisplayName = "language/expressions/new.target/value-via-reflect-construct.js")]
    public Task test_value_via_reflect_construct()
        => ExecutionTest("value-via-reflect-construct");

    [Fact(DisplayName = "language/expressions/new.target/value-via-tagged-template.js")]
    public Task test_value_via_tagged_template()
        => ExecutionTest("value-via-tagged-template");
}
