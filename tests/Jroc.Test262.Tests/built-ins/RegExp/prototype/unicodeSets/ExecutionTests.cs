using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.unicodeSets;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.prototype.unicodeSets") { }

    [Fact(DisplayName = "breaking-change-from-u-to-v-01")]
    public Task breaking_change_from_u_to_v_01()
        => CompilationFailureTest("breaking-change-from-u-to-v-01");

    [Fact(DisplayName = "breaking-change-from-u-to-v-02")]
    public Task breaking_change_from_u_to_v_02()
        => CompilationFailureTest("breaking-change-from-u-to-v-02");

    [Fact(DisplayName = "breaking-change-from-u-to-v-03")]
    public Task breaking_change_from_u_to_v_03()
        => CompilationFailureTest("breaking-change-from-u-to-v-03");

    [Fact(DisplayName = "breaking-change-from-u-to-v-04")]
    public Task breaking_change_from_u_to_v_04()
        => CompilationFailureTest("breaking-change-from-u-to-v-04");

    [Fact(DisplayName = "breaking-change-from-u-to-v-05")]
    public Task breaking_change_from_u_to_v_05()
        => CompilationFailureTest("breaking-change-from-u-to-v-05");

    [Fact(DisplayName = "breaking-change-from-u-to-v-06")]
    public Task breaking_change_from_u_to_v_06()
        => CompilationFailureTest("breaking-change-from-u-to-v-06");

    [Fact(DisplayName = "breaking-change-from-u-to-v-07")]
    public Task breaking_change_from_u_to_v_07()
        => CompilationFailureTest("breaking-change-from-u-to-v-07");

    [Fact(DisplayName = "breaking-change-from-u-to-v-08")]
    public Task breaking_change_from_u_to_v_08()
        => CompilationFailureTest("breaking-change-from-u-to-v-08");

    [Fact(DisplayName = "breaking-change-from-u-to-v-09")]
    public Task breaking_change_from_u_to_v_09()
        => CompilationFailureTest("breaking-change-from-u-to-v-09");

    [Fact(DisplayName = "breaking-change-from-u-to-v-10")]
    public Task breaking_change_from_u_to_v_10()
        => CompilationFailureTest("breaking-change-from-u-to-v-10");
}
