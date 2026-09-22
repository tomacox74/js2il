using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp;

public class Test262BatchPort20260921Round6ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260921Round6ExecutionTests() : base("built_ins.RegExp") { }

    [Fact(DisplayName = "u180e")]
    public Task u180e()
        => ExecutionTestFromFile("u180e");

    [Fact(DisplayName = "unicode_character_class_backspace_escape")]
    public Task unicode_character_class_backspace_escape()
        => ExecutionTestFromFile("unicode_character_class_backspace_escape");

    [Fact(DisplayName = "unicode_identity_escape")]
    public Task unicode_identity_escape()
        => ExecutionTestFromFile("unicode_identity_escape");

    [Fact(DisplayName = "unicode_restricted_identity_escape_u")]
    public Task unicode_restricted_identity_escape_u()
        => ExecutionTestFromFile("unicode_restricted_identity_escape_u");

    [Fact(DisplayName = "unicode_restricted_identity_escape_x")]
    public Task unicode_restricted_identity_escape_x()
        => ExecutionTestFromFile("unicode_restricted_identity_escape_x");

    [Fact(DisplayName = "unicode_restricted_quantifier_without_atom")]
    public Task unicode_restricted_quantifier_without_atom()
        => ExecutionTestFromFile("unicode_restricted_quantifier_without_atom");

    [Fact(DisplayName = "valid-flags-y")]
    public Task valid_flags_y()
        => ExecutionTestFromFile("valid-flags-y");

}
