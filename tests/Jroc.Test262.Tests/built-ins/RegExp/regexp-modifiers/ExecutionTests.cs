using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.regexp_modifiers;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.regexp_modifiers") { }

    [Fact(DisplayName = "add-dotAll-does-not-affect-dotAll-property.js")]
    public Task add_dotAll_does_not_affect_dotAll_property()
        => ExecutionTestFromFile("add-dotAll-does-not-affect-dotAll-property");

    [Fact(DisplayName = "add-ignoreCase-affects-backreferences.js")]
    public Task add_ignoreCase_affects_backreferences()
        => ExecutionTestFromFile("add-ignoreCase-affects-backreferences");

    [Fact(DisplayName = "add-ignoreCase-affects-characterClasses.js")]
    public Task add_ignoreCase_affects_characterClasses()
        => ExecutionTestFromFile("add-ignoreCase-affects-characterClasses");

    [Fact(DisplayName = "add-ignoreCase-affects-characterEscapes.js")]
    public Task add_ignoreCase_affects_characterEscapes()
        => ExecutionTestFromFile("add-ignoreCase-affects-characterEscapes");

    [Fact(DisplayName = "add-ignoreCase-affects-slash-lower-b.js")]
    public Task add_ignoreCase_affects_slash_lower_b()
        => ExecutionTestFromFile("add-ignoreCase-affects-slash-lower-b");

    [Fact(DisplayName = "add-ignoreCase-affects-slash-lower-p.js")]
    public Task add_ignoreCase_affects_slash_lower_p()
        => ExecutionTestFromFile("add-ignoreCase-affects-slash-lower-p");

    [Fact(DisplayName = "add-ignoreCase-affects-slash-lower-w.js")]
    public Task add_ignoreCase_affects_slash_lower_w()
        => ExecutionTestFromFile("add-ignoreCase-affects-slash-lower-w");

    [Fact(DisplayName = "add-ignoreCase-affects-slash-upper-b.js")]
    public Task add_ignoreCase_affects_slash_upper_b()
        => ExecutionTestFromFile("add-ignoreCase-affects-slash-upper-b");

    [Fact(DisplayName = "add-ignoreCase-affects-slash-upper-w.js")]
    public Task add_ignoreCase_affects_slash_upper_w()
        => ExecutionTestFromFile("add-ignoreCase-affects-slash-upper-w");

    [Fact(DisplayName = "add-ignoreCase-does-not-affect-alternatives-outside.js")]
    public Task add_ignoreCase_does_not_affect_alternatives_outside()
        => ExecutionTestFromFile("add-ignoreCase-does-not-affect-alternatives-outside");

    [Fact(DisplayName = "add-ignoreCase-does-not-affect-dotAll-flag.js")]
    public Task add_ignoreCase_does_not_affect_dotAll_flag()
        => ExecutionTestFromFile("add-ignoreCase-does-not-affect-dotAll-flag");

    [Fact(DisplayName = "add-ignoreCase-does-not-affect-ignoreCase-property.js")]
    public Task add_ignoreCase_does_not_affect_ignoreCase_property()
        => ExecutionTestFromFile("add-ignoreCase-does-not-affect-ignoreCase-property");

    [Fact(DisplayName = "add-ignoreCase-does-not-affect-multiline-flag.js")]
    public Task add_ignoreCase_does_not_affect_multiline_flag()
        => ExecutionTestFromFile("add-ignoreCase-does-not-affect-multiline-flag");

    [Fact(DisplayName = "add-ignoreCase.js")]
    public Task add_ignoreCase()
        => ExecutionTestFromFile("add-ignoreCase");

    [Fact(DisplayName = "add-multiline-does-not-affect-alternatives-outside.js")]
    public Task add_multiline_does_not_affect_alternatives_outside()
        => ExecutionTestFromFile("add-multiline-does-not-affect-alternatives-outside");

    [Fact(DisplayName = "add-multiline-does-not-affect-dotAll-flag.js")]
    public Task add_multiline_does_not_affect_dotAll_flag()
        => ExecutionTestFromFile("add-multiline-does-not-affect-dotAll-flag");

    [Fact(DisplayName = "add-multiline-does-not-affect-ignoreCase-flag.js")]
    public Task add_multiline_does_not_affect_ignoreCase_flag()
        => ExecutionTestFromFile("add-multiline-does-not-affect-ignoreCase-flag");

    [Fact(DisplayName = "add-multiline-does-not-affect-multiline-property.js")]
    public Task add_multiline_does_not_affect_multiline_property()
        => ExecutionTestFromFile("add-multiline-does-not-affect-multiline-property");

    [Fact(DisplayName = "add-remove-modifiers.js")]
    public Task add_remove_modifiers()
        => ExecutionTestFromFile("add-remove-modifiers");

    [Fact(DisplayName = "changing-ignoreCase-flag-does-not-affect-ignoreCase-modifier.js")]
    public Task changing_ignoreCase_flag_does_not_affect_ignoreCase_modifier()
        => ExecutionTestFromFile("changing-ignoreCase-flag-does-not-affect-ignoreCase-modifier");

    [Fact(DisplayName = "changing-multiline-flag-does-not-affect-multiline-modifier.js")]
    public Task changing_multiline_flag_does_not_affect_multiline_modifier()
        => ExecutionTestFromFile("changing-multiline-flag-does-not-affect-multiline-modifier");

    [Fact(DisplayName = "nested-add-remove-modifiers.js")]
    public Task nested_add_remove_modifiers()
        => ExecutionTestFromFile("nested-add-remove-modifiers");

    [Fact(DisplayName = "nesting-add-dotAll-within-remove-dotAll.js")]
    public Task nesting_add_dotAll_within_remove_dotAll()
        => ExecutionTestFromFile("nesting-add-dotAll-within-remove-dotAll");

    [Fact(DisplayName = "nesting-add-ignoreCase-within-remove-ignoreCase.js")]
    public Task nesting_add_ignoreCase_within_remove_ignoreCase()
        => ExecutionTestFromFile("nesting-add-ignoreCase-within-remove-ignoreCase");

    [Fact(DisplayName = "nesting-add-multiline-within-remove-multiline.js")]
    public Task nesting_add_multiline_within_remove_multiline()
        => ExecutionTestFromFile("nesting-add-multiline-within-remove-multiline");

    [Fact(DisplayName = "nesting-ignoreCase-does-not-affect-alternatives-outside.js")]
    public Task nesting_ignoreCase_does_not_affect_alternatives_outside()
        => ExecutionTestFromFile("nesting-ignoreCase-does-not-affect-alternatives-outside");

    [Fact(DisplayName = "nesting-multiline-does-not-affect-alternatives-outside.js")]
    public Task nesting_multiline_does_not_affect_alternatives_outside()
        => ExecutionTestFromFile("nesting-multiline-does-not-affect-alternatives-outside");

    [Fact(DisplayName = "nesting-remove-dotAll-within-add-dotAll.js")]
    public Task nesting_remove_dotAll_within_add_dotAll()
        => ExecutionTestFromFile("nesting-remove-dotAll-within-add-dotAll");

    [Fact(DisplayName = "nesting-remove-ignoreCase-within-add-ignoreCase.js")]
    public Task nesting_remove_ignoreCase_within_add_ignoreCase()
        => ExecutionTestFromFile("nesting-remove-ignoreCase-within-add-ignoreCase");

    [Fact(DisplayName = "nesting-remove-multiline-within-add-multiline.js")]
    public Task nesting_remove_multiline_within_add_multiline()
        => ExecutionTestFromFile("nesting-remove-multiline-within-add-multiline");

    [Fact(DisplayName = "remove-dotAll-does-not-affect-alternatives-outside.js")]
    public Task remove_dotAll_does_not_affect_alternatives_outside()
        => ExecutionTestFromFile("remove-dotAll-does-not-affect-alternatives-outside");

    [Fact(DisplayName = "remove-dotAll-does-not-affect-dotAll-property.js")]
    public Task remove_dotAll_does_not_affect_dotAll_property()
        => ExecutionTestFromFile("remove-dotAll-does-not-affect-dotAll-property");

    [Fact(DisplayName = "remove-dotAll-does-not-affect-ignoreCase-flag.js")]
    public Task remove_dotAll_does_not_affect_ignoreCase_flag()
        => ExecutionTestFromFile("remove-dotAll-does-not-affect-ignoreCase-flag");

    [Fact(DisplayName = "remove-dotAll-does-not-affect-multiline-flag.js")]
    public Task remove_dotAll_does_not_affect_multiline_flag()
        => ExecutionTestFromFile("remove-dotAll-does-not-affect-multiline-flag");

    [Fact(DisplayName = "remove-ignoreCase-affects-backreferences.js")]
    public Task remove_ignoreCase_affects_backreferences()
        => ExecutionTestFromFile("remove-ignoreCase-affects-backreferences");

    [Fact(DisplayName = "remove-ignoreCase-affects-characterClasses.js")]
    public Task remove_ignoreCase_affects_characterClasses()
        => ExecutionTestFromFile("remove-ignoreCase-affects-characterClasses");

    [Fact(DisplayName = "remove-ignoreCase-affects-characterEscapes.js")]
    public Task remove_ignoreCase_affects_characterEscapes()
        => ExecutionTestFromFile("remove-ignoreCase-affects-characterEscapes");

    [Fact(DisplayName = "remove-ignoreCase-affects-slash-lower-p.js")]
    public Task remove_ignoreCase_affects_slash_lower_p()
        => ExecutionTestFromFile("remove-ignoreCase-affects-slash-lower-p");

    [Fact(DisplayName = "remove-ignoreCase-affects-slash-upper-p.js")]
    public Task remove_ignoreCase_affects_slash_upper_p()
        => ExecutionTestFromFile("remove-ignoreCase-affects-slash-upper-p");

    [Fact(DisplayName = "remove-ignoreCase-does-not-affect-alternatives-outside.js")]
    public Task remove_ignoreCase_does_not_affect_alternatives_outside()
        => ExecutionTestFromFile("remove-ignoreCase-does-not-affect-alternatives-outside");

    [Fact(DisplayName = "remove-ignoreCase-does-not-affect-dotAll-flag.js")]
    public Task remove_ignoreCase_does_not_affect_dotAll_flag()
        => ExecutionTestFromFile("remove-ignoreCase-does-not-affect-dotAll-flag");

    [Fact(DisplayName = "remove-ignoreCase-does-not-affect-ignoreCase-property.js")]
    public Task remove_ignoreCase_does_not_affect_ignoreCase_property()
        => ExecutionTestFromFile("remove-ignoreCase-does-not-affect-ignoreCase-property");

    [Fact(DisplayName = "remove-ignoreCase-does-not-affect-multiline-flag.js")]
    public Task remove_ignoreCase_does_not_affect_multiline_flag()
        => ExecutionTestFromFile("remove-ignoreCase-does-not-affect-multiline-flag");

    [Fact(DisplayName = "remove-ignoreCase.js")]
    public Task remove_ignoreCase()
        => ExecutionTestFromFile("remove-ignoreCase");

    [Fact(DisplayName = "remove-multiline-does-not-affect-alternatives-outside.js")]
    public Task remove_multiline_does_not_affect_alternatives_outside()
        => ExecutionTestFromFile("remove-multiline-does-not-affect-alternatives-outside");

    [Fact(DisplayName = "remove-multiline-does-not-affect-ignoreCase-flag.js")]
    public Task remove_multiline_does_not_affect_ignoreCase_flag()
        => ExecutionTestFromFile("remove-multiline-does-not-affect-ignoreCase-flag");

    [Fact(DisplayName = "remove-multiline-does-not-affect-multiline-property.js")]
    public Task remove_multiline_does_not_affect_multiline_property()
        => ExecutionTestFromFile("remove-multiline-does-not-affect-multiline-property");

    [Fact(DisplayName = "remove-multiline.js")]
    public Task remove_multiline()
        => ExecutionTestFromFile("remove-multiline");

}
