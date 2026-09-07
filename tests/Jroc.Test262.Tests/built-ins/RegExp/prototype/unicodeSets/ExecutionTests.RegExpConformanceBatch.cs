using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.unicodeSets;

public partial class ExecutionTests
{
    [Fact(DisplayName = "breaking-change-from-u-to-v-11.js")]
    public Task breaking_change_from_u_to_v_11()
        => CompilationFailureTest("breaking-change-from-u-to-v-11");

    [Fact(DisplayName = "breaking-change-from-u-to-v-12.js")]
    public Task breaking_change_from_u_to_v_12()
        => CompilationFailureTest("breaking-change-from-u-to-v-12");

    [Fact(DisplayName = "breaking-change-from-u-to-v-13.js")]
    public Task breaking_change_from_u_to_v_13()
        => CompilationFailureTest("breaking-change-from-u-to-v-13");

    [Fact(DisplayName = "breaking-change-from-u-to-v-14.js")]
    public Task breaking_change_from_u_to_v_14()
        => CompilationFailureTest("breaking-change-from-u-to-v-14");

    [Fact(DisplayName = "breaking-change-from-u-to-v-15.js")]
    public Task breaking_change_from_u_to_v_15()
        => CompilationFailureTest("breaking-change-from-u-to-v-15");

    [Fact(DisplayName = "breaking-change-from-u-to-v-16.js")]
    public Task breaking_change_from_u_to_v_16()
        => CompilationFailureTest("breaking-change-from-u-to-v-16");

    [Fact(DisplayName = "breaking-change-from-u-to-v-17.js")]
    public Task breaking_change_from_u_to_v_17()
        => CompilationFailureTest("breaking-change-from-u-to-v-17");

    [Fact(DisplayName = "breaking-change-from-u-to-v-18.js")]
    public Task breaking_change_from_u_to_v_18()
        => CompilationFailureTest("breaking-change-from-u-to-v-18");

    [Fact(DisplayName = "breaking-change-from-u-to-v-19.js")]
    public Task breaking_change_from_u_to_v_19()
        => CompilationFailureTest("breaking-change-from-u-to-v-19");

    [Fact(DisplayName = "breaking-change-from-u-to-v-20.js")]
    public Task breaking_change_from_u_to_v_20()
        => CompilationFailureTest("breaking-change-from-u-to-v-20");

    [Fact(DisplayName = "breaking-change-from-u-to-v-21.js")]
    public Task breaking_change_from_u_to_v_21()
        => CompilationFailureTest("breaking-change-from-u-to-v-21");

    [Fact(DisplayName = "breaking-change-from-u-to-v-22.js")]
    public Task breaking_change_from_u_to_v_22()
        => CompilationFailureTest("breaking-change-from-u-to-v-22");

    [Fact(DisplayName = "breaking-change-from-u-to-v-23.js")]
    public Task breaking_change_from_u_to_v_23()
        => CompilationFailureTest("breaking-change-from-u-to-v-23");

    [Fact(DisplayName = "breaking-change-from-u-to-v-24.js")]
    public Task breaking_change_from_u_to_v_24()
        => CompilationFailureTest("breaking-change-from-u-to-v-24");

    [Fact(DisplayName = "breaking-change-from-u-to-v-25.js")]
    public Task breaking_change_from_u_to_v_25()
        => CompilationFailureTest("breaking-change-from-u-to-v-25");

    [Fact(DisplayName = "breaking-change-from-u-to-v-26.js")]
    public Task breaking_change_from_u_to_v_26()
        => CompilationFailureTest("breaking-change-from-u-to-v-26");

    [Fact(DisplayName = "breaking-change-from-u-to-v-27.js")]
    public Task breaking_change_from_u_to_v_27()
        => CompilationFailureTest("breaking-change-from-u-to-v-27");

    [Fact(DisplayName = "breaking-change-from-u-to-v-28.js")]
    public Task breaking_change_from_u_to_v_28()
        => CompilationFailureTest("breaking-change-from-u-to-v-28");

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "this-val-invalid-obj.js")]
    public Task this_val_invalid_obj()
        => ExecutionTestFromFile("this-val-invalid-obj");

    [Fact(DisplayName = "this-val-non-obj.js")]
    public Task this_val_non_obj()
        => ExecutionTestFromFile("this-val-non-obj");

    [Fact(DisplayName = "uv-flags-constructor.js")]
    public Task uv_flags_constructor()
        => ExecutionTestFromFile("uv-flags-constructor");

    [Fact(DisplayName = "uv-flags.js")]
    public Task uv_flags()
        => CompilationFailureTest("uv-flags");

}
