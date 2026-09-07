using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.literals.regexp.named_groups;

public class LiteralsConformanceBatchParseTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public LiteralsConformanceBatchParseTests() : base("language/literals/regexp/named-groups", "language.literals.regexp.named-groups") { }

    [Fact(DisplayName = "invalid-dangling-groupname-2-u.js")]
    public Task invalid_dangling_groupname_2_u()
        => CompilationFailureTest("invalid-dangling-groupname-2-u");

    [Fact(DisplayName = "invalid-dangling-groupname-2.js")]
    public Task invalid_dangling_groupname_2()
        => CompilationFailureTest("invalid-dangling-groupname-2");

    [Fact(DisplayName = "invalid-dangling-groupname-3-u.js")]
    public Task invalid_dangling_groupname_3_u()
        => CompilationFailureTest("invalid-dangling-groupname-3-u");

    [Fact(DisplayName = "invalid-dangling-groupname-3.js")]
    public Task invalid_dangling_groupname_3()
        => CompilationFailureTest("invalid-dangling-groupname-3");

    [Fact(DisplayName = "invalid-dangling-groupname-4-u.js")]
    public Task invalid_dangling_groupname_4_u()
        => CompilationFailureTest("invalid-dangling-groupname-4-u");

    [Fact(DisplayName = "invalid-dangling-groupname-4.js")]
    public Task invalid_dangling_groupname_4()
        => CompilationFailureTest("invalid-dangling-groupname-4");

    [Fact(DisplayName = "invalid-dangling-groupname-5.js")]
    public Task invalid_dangling_groupname_5()
        => CompilationFailureTest("invalid-dangling-groupname-5");

    [Fact(DisplayName = "invalid-dangling-groupname-u.js")]
    public Task invalid_dangling_groupname_u()
        => CompilationFailureTest("invalid-dangling-groupname-u");

    [Fact(DisplayName = "invalid-dangling-groupname-without-group-u.js")]
    public Task invalid_dangling_groupname_without_group_u()
        => CompilationFailureTest("invalid-dangling-groupname-without-group-u");

    [Fact(DisplayName = "invalid-dangling-groupname.js")]
    public Task invalid_dangling_groupname()
        => CompilationFailureTest("invalid-dangling-groupname");

    [Fact(DisplayName = "invalid-duplicate-groupspecifier-2-u.js")]
    public Task invalid_duplicate_groupspecifier_2_u()
        => CompilationFailureTest("invalid-duplicate-groupspecifier-2-u");

    [Fact(DisplayName = "invalid-duplicate-groupspecifier-2.js")]
    public Task invalid_duplicate_groupspecifier_2()
        => CompilationFailureTest("invalid-duplicate-groupspecifier-2");

    [Fact(DisplayName = "invalid-duplicate-groupspecifier-u.js")]
    public Task invalid_duplicate_groupspecifier_u()
        => CompilationFailureTest("invalid-duplicate-groupspecifier-u");

    [Fact(DisplayName = "invalid-duplicate-groupspecifier.js")]
    public Task invalid_duplicate_groupspecifier()
        => CompilationFailureTest("invalid-duplicate-groupspecifier");

    [Fact(DisplayName = "invalid-empty-groupspecifier-u.js")]
    public Task invalid_empty_groupspecifier_u()
        => CompilationFailureTest("invalid-empty-groupspecifier-u");

    [Fact(DisplayName = "invalid-empty-groupspecifier.js")]
    public Task invalid_empty_groupspecifier()
        => CompilationFailureTest("invalid-empty-groupspecifier");

    [Fact(DisplayName = "invalid-identity-escape-in-capture-u.js")]
    public Task invalid_identity_escape_in_capture_u()
        => CompilationFailureTest("invalid-identity-escape-in-capture-u");

    [Fact(DisplayName = "invalid-incomplete-groupname-2-u.js")]
    public Task invalid_incomplete_groupname_2_u()
        => CompilationFailureTest("invalid-incomplete-groupname-2-u");

    [Fact(DisplayName = "invalid-incomplete-groupname-2.js")]
    public Task invalid_incomplete_groupname_2()
        => CompilationFailureTest("invalid-incomplete-groupname-2");

    [Fact(DisplayName = "invalid-incomplete-groupname-3-u.js")]
    public Task invalid_incomplete_groupname_3_u()
        => CompilationFailureTest("invalid-incomplete-groupname-3-u");

    [Fact(DisplayName = "invalid-incomplete-groupname-3.js")]
    public Task invalid_incomplete_groupname_3()
        => CompilationFailureTest("invalid-incomplete-groupname-3");

    [Fact(DisplayName = "invalid-incomplete-groupname-4.js")]
    public Task invalid_incomplete_groupname_4()
        => CompilationFailureTest("invalid-incomplete-groupname-4");

    [Fact(DisplayName = "invalid-incomplete-groupname-5.js")]
    public Task invalid_incomplete_groupname_5()
        => CompilationFailureTest("invalid-incomplete-groupname-5");

    [Fact(DisplayName = "invalid-incomplete-groupname-6.js")]
    public Task invalid_incomplete_groupname_6()
        => CompilationFailureTest("invalid-incomplete-groupname-6");

    [Fact(DisplayName = "invalid-incomplete-groupname-u.js")]
    public Task invalid_incomplete_groupname_u()
        => CompilationFailureTest("invalid-incomplete-groupname-u");

    [Fact(DisplayName = "invalid-incomplete-groupname-without-group-2-u.js")]
    public Task invalid_incomplete_groupname_without_group_2_u()
        => CompilationFailureTest("invalid-incomplete-groupname-without-group-2-u");

    [Fact(DisplayName = "invalid-incomplete-groupname-without-group-3-u.js")]
    public Task invalid_incomplete_groupname_without_group_3_u()
        => CompilationFailureTest("invalid-incomplete-groupname-without-group-3-u");

    [Fact(DisplayName = "invalid-incomplete-groupname-without-group-u.js")]
    public Task invalid_incomplete_groupname_without_group_u()
        => CompilationFailureTest("invalid-incomplete-groupname-without-group-u");

    [Fact(DisplayName = "invalid-incomplete-groupname.js")]
    public Task invalid_incomplete_groupname()
        => CompilationFailureTest("invalid-incomplete-groupname");

    [Fact(DisplayName = "invalid-non-id-continue-groupspecifier-4-u.js")]
    public Task invalid_non_id_continue_groupspecifier_4_u()
        => CompilationFailureTest("invalid-non-id-continue-groupspecifier-4-u");

    [Fact(DisplayName = "invalid-non-id-continue-groupspecifier-4.js")]
    public Task invalid_non_id_continue_groupspecifier_4()
        => CompilationFailureTest("invalid-non-id-continue-groupspecifier-4");

    [Fact(DisplayName = "invalid-non-id-continue-groupspecifier.js")]
    public Task invalid_non_id_continue_groupspecifier()
        => CompilationFailureTest("invalid-non-id-continue-groupspecifier");

    [Fact(DisplayName = "invalid-non-id-start-groupspecifier-2-u.js")]
    public Task invalid_non_id_start_groupspecifier_2_u()
        => CompilationFailureTest("invalid-non-id-start-groupspecifier-2-u");

    [Fact(DisplayName = "invalid-non-id-start-groupspecifier-2.js")]
    public Task invalid_non_id_start_groupspecifier_2()
        => CompilationFailureTest("invalid-non-id-start-groupspecifier-2");

    [Fact(DisplayName = "invalid-non-id-start-groupspecifier-3.js")]
    public Task invalid_non_id_start_groupspecifier_3()
        => CompilationFailureTest("invalid-non-id-start-groupspecifier-3");

    [Fact(DisplayName = "invalid-non-id-start-groupspecifier-4-u.js")]
    public Task invalid_non_id_start_groupspecifier_4_u()
        => CompilationFailureTest("invalid-non-id-start-groupspecifier-4-u");

    [Fact(DisplayName = "invalid-non-id-start-groupspecifier-4.js")]
    public Task invalid_non_id_start_groupspecifier_4()
        => CompilationFailureTest("invalid-non-id-start-groupspecifier-4");

    [Fact(DisplayName = "invalid-non-id-start-groupspecifier-5-u.js")]
    public Task invalid_non_id_start_groupspecifier_5_u()
        => CompilationFailureTest("invalid-non-id-start-groupspecifier-5-u");

    [Fact(DisplayName = "invalid-non-id-start-groupspecifier-5.js")]
    public Task invalid_non_id_start_groupspecifier_5()
        => CompilationFailureTest("invalid-non-id-start-groupspecifier-5");

    [Fact(DisplayName = "invalid-non-id-start-groupspecifier-6.js")]
    public Task invalid_non_id_start_groupspecifier_6()
        => CompilationFailureTest("invalid-non-id-start-groupspecifier-6");

    [Fact(DisplayName = "invalid-non-id-start-groupspecifier-7.js")]
    public Task invalid_non_id_start_groupspecifier_7()
        => CompilationFailureTest("invalid-non-id-start-groupspecifier-7");

    [Fact(DisplayName = "invalid-non-id-start-groupspecifier-8-u.js")]
    public Task invalid_non_id_start_groupspecifier_8_u()
        => CompilationFailureTest("invalid-non-id-start-groupspecifier-8-u");

    [Fact(DisplayName = "invalid-non-id-start-groupspecifier-8.js")]
    public Task invalid_non_id_start_groupspecifier_8()
        => CompilationFailureTest("invalid-non-id-start-groupspecifier-8");

    [Fact(DisplayName = "invalid-non-id-start-groupspecifier-9-u.js")]
    public Task invalid_non_id_start_groupspecifier_9_u()
        => CompilationFailureTest("invalid-non-id-start-groupspecifier-9-u");

    [Fact(DisplayName = "invalid-non-id-start-groupspecifier-u.js")]
    public Task invalid_non_id_start_groupspecifier_u()
        => CompilationFailureTest("invalid-non-id-start-groupspecifier-u");

    [Fact(DisplayName = "invalid-non-id-start-groupspecifier.js")]
    public Task invalid_non_id_start_groupspecifier()
        => CompilationFailureTest("invalid-non-id-start-groupspecifier");

    [Fact(DisplayName = "invalid-numeric-groupspecifier-u.js")]
    public Task invalid_numeric_groupspecifier_u()
        => CompilationFailureTest("invalid-numeric-groupspecifier-u");

    [Fact(DisplayName = "invalid-numeric-groupspecifier.js")]
    public Task invalid_numeric_groupspecifier()
        => CompilationFailureTest("invalid-numeric-groupspecifier");

    [Fact(DisplayName = "invalid-punctuator-starting-groupspecifier-u.js")]
    public Task invalid_punctuator_starting_groupspecifier_u()
        => CompilationFailureTest("invalid-punctuator-starting-groupspecifier-u");

    [Fact(DisplayName = "invalid-punctuator-starting-groupspecifier.js")]
    public Task invalid_punctuator_starting_groupspecifier()
        => CompilationFailureTest("invalid-punctuator-starting-groupspecifier");

    [Fact(DisplayName = "invalid-punctuator-within-groupspecifier-u.js")]
    public Task invalid_punctuator_within_groupspecifier_u()
        => CompilationFailureTest("invalid-punctuator-within-groupspecifier-u");

    [Fact(DisplayName = "invalid-punctuator-within-groupspecifier.js")]
    public Task invalid_punctuator_within_groupspecifier()
        => CompilationFailureTest("invalid-punctuator-within-groupspecifier");

    [Fact(DisplayName = "invalid-unterminated-groupspecifier-u.js")]
    public Task invalid_unterminated_groupspecifier_u()
        => CompilationFailureTest("invalid-unterminated-groupspecifier-u");

    [Fact(DisplayName = "invalid-unterminated-groupspecifier.js")]
    public Task invalid_unterminated_groupspecifier()
        => CompilationFailureTest("invalid-unterminated-groupspecifier");

}
