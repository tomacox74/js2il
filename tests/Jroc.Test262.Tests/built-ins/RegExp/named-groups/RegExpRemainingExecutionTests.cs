namespace Jroc.Test262.Tests.built_ins.RegExp.named_groups;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp.named_groups") { }

    [Fact(DisplayName = "duplicate-names-exec.js")]
    public Task duplicate_names_exec()
        => ExecutionTest("duplicate-names-exec");

    [Fact(DisplayName = "duplicate-names-match.js")]
    public Task duplicate_names_match()
        => ExecutionTest("duplicate-names-match");

    [Fact(DisplayName = "duplicate-names-matchall.js")]
    public Task duplicate_names_matchall()
        => ExecutionTest("duplicate-names-matchall");

    [Fact(DisplayName = "duplicate-names-replace.js")]
    public Task duplicate_names_replace()
        => ExecutionTest("duplicate-names-replace");

    [Fact(DisplayName = "duplicate-names-replaceall.js")]
    public Task duplicate_names_replaceall()
        => ExecutionTest("duplicate-names-replaceall");

    [Fact(DisplayName = "duplicate-names-split.js")]
    public Task duplicate_names_split()
        => ExecutionTest("duplicate-names-split");

    [Fact(DisplayName = "functional-replace-global.js")]
    public Task functional_replace_global()
        => ExecutionTest("functional-replace-global");

    [Fact(DisplayName = "functional-replace-non-global.js")]
    public Task functional_replace_non_global()
        => ExecutionTest("functional-replace-non-global");

    [Fact(DisplayName = "groups-object-subclass-sans.js")]
    public Task groups_object_subclass_sans()
        => ExecutionTest("groups-object-subclass-sans");

    [Fact(DisplayName = "groups-object-subclass.js")]
    public Task groups_object_subclass()
        => ExecutionTest("groups-object-subclass");

    [Fact(DisplayName = "groups-object-unmatched.js")]
    public Task groups_object_unmatched()
        => ExecutionTest("groups-object-unmatched");

    [Fact(DisplayName = "non-unicode-match.js")]
    public Task non_unicode_match()
        => ExecutionTest("non-unicode-match");

    [Fact(DisplayName = "non-unicode-property-names-valid.js")]
    public Task non_unicode_property_names_valid()
        => ExecutionTest("non-unicode-property-names-valid");

    [Fact(DisplayName = "non-unicode-property-names.js")]
    public Task non_unicode_property_names()
        => ExecutionTest("non-unicode-property-names");

    [Fact(DisplayName = "non-unicode-references.js")]
    public Task non_unicode_references()
        => ExecutionTest("non-unicode-references");

    [Fact(DisplayName = "unicode-match.js")]
    public Task unicode_match()
        => ExecutionTest("unicode-match");

    [Fact(DisplayName = "unicode-property-names-valid.js")]
    public Task unicode_property_names_valid()
        => ExecutionTest("unicode-property-names-valid");

    [Fact(DisplayName = "unicode-property-names.js")]
    public Task unicode_property_names()
        => ExecutionTest("unicode-property-names");

    [Fact(DisplayName = "unicode-references.js")]
    public Task unicode_references()
        => ExecutionTest("unicode-references");
}
