namespace Jroc.Test262.Tests.built_ins.RegExp.regexp_modifiers;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp.regexp_modifiers") { }

    [Fact(DisplayName = "add-dotAll-does-not-affect-alternatives-outside.js")]
    public Task add_dotAll_does_not_affect_alternatives_outside()
        => ExecutionTest("add-dotAll-does-not-affect-alternatives-outside");

    [Fact(DisplayName = "add-dotAll-does-not-affect-ignoreCase-flag.js")]
    public Task add_dotAll_does_not_affect_ignoreCase_flag()
        => ExecutionTest("add-dotAll-does-not-affect-ignoreCase-flag");

    [Fact(DisplayName = "add-dotAll-does-not-affect-multiline-flag.js")]
    public Task add_dotAll_does_not_affect_multiline_flag()
        => ExecutionTest("add-dotAll-does-not-affect-multiline-flag");

    [Fact(DisplayName = "add-dotAll.js")]
    public Task add_dotAll()
        => ExecutionTest("add-dotAll");

    [Fact(DisplayName = "add-ignoreCase-affects-slash-upper-p.js")]
    public Task add_ignoreCase_affects_slash_upper_p()
        => ExecutionTest("add-ignoreCase-affects-slash-upper-p");

    [Fact(DisplayName = "add-multiline.js")]
    public Task add_multiline()
        => ExecutionTest("add-multiline");

    [Fact(DisplayName = "changing-dotAll-flag-does-not-affect-dotAll-modifier.js")]
    public Task changing_dotAll_flag_does_not_affect_dotAll_modifier()
        => ExecutionTest("changing-dotAll-flag-does-not-affect-dotAll-modifier");

    [Fact(DisplayName = "nesting-dotAll-does-not-affect-alternatives-outside.js")]
    public Task nesting_dotAll_does_not_affect_alternatives_outside()
        => ExecutionTest("nesting-dotAll-does-not-affect-alternatives-outside");

    [Fact(DisplayName = "remove-dotAll.js")]
    public Task remove_dotAll()
        => ExecutionTest("remove-dotAll");

    [Fact(DisplayName = "remove-ignoreCase-affects-slash-lower-b.js")]
    public Task remove_ignoreCase_affects_slash_lower_b()
        => ExecutionTest("remove-ignoreCase-affects-slash-lower-b");

    [Fact(DisplayName = "remove-ignoreCase-affects-slash-lower-w.js")]
    public Task remove_ignoreCase_affects_slash_lower_w()
        => ExecutionTest("remove-ignoreCase-affects-slash-lower-w");

    [Fact(DisplayName = "remove-ignoreCase-affects-slash-upper-b.js")]
    public Task remove_ignoreCase_affects_slash_upper_b()
        => ExecutionTest("remove-ignoreCase-affects-slash-upper-b");

    [Fact(DisplayName = "remove-ignoreCase-affects-slash-upper-w.js")]
    public Task remove_ignoreCase_affects_slash_upper_w()
        => ExecutionTest("remove-ignoreCase-affects-slash-upper-w");

    [Fact(DisplayName = "remove-multiline-does-not-affect-dotAll-flag.js")]
    public Task remove_multiline_does_not_affect_dotAll_flag()
        => ExecutionTest("remove-multiline-does-not-affect-dotAll-flag");
}
