namespace Jroc.Test262.Tests.language.expressions.assignmenttargettype;

public class AssignmentTargetTypeCompletionExecutionTests : DiskExecutionTestsBase
{
    public AssignmentTargetTypeCompletionExecutionTests() : base("language.expressions.assignmenttargettype") { }

    [Fact(DisplayName = "direct-importcall-defer.js")]
    public Task direct_importcall_defer() => CompilationFailureTest("direct-importcall-defer", "Failed to parse JavaScript");

    [Fact(DisplayName = "direct-importcall-source.js")]
    public Task direct_importcall_source() => CompilationFailureTest("direct-importcall-source", "Failed to parse JavaScript");

    [Fact(DisplayName = "direct-importcall.js")]
    public Task direct_importcall() => CompilationFailureTest("direct-importcall", "Failed to parse JavaScript");

    [Fact(DisplayName = "parenthesized-importcall-defer.js")]
    public Task parenthesized_importcall_defer() => CompilationFailureTest("parenthesized-importcall-defer", "Failed to parse JavaScript");

    [Fact(DisplayName = "parenthesized-importcall-source.js")]
    public Task parenthesized_importcall_source() => CompilationFailureTest("parenthesized-importcall-source", "Failed to parse JavaScript");

    [Fact(DisplayName = "parenthesized-importcall.js")]
    public Task parenthesized_importcall() => CompilationFailureTest("parenthesized-importcall", "Failed to parse JavaScript");
}
