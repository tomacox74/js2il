namespace Jroc.Test262.Tests.built_ins.RegExp.property_escapes;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp.property_escapes") { }

    [Fact(DisplayName = "character-class.js")]
    public Task character_class()
        => ExecutionTest("character-class");

    [Fact(DisplayName = "special-property-value-Script_Extensions-Unknown.js")]
    public Task special_property_value_Script_Extensions_Unknown()
        => ExecutionTest("special-property-value-Script_Extensions-Unknown");
}
