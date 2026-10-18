namespace Jroc.Test262.Tests.built_ins.RegExp.property_escapes.generated.strings;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp.property_escapes.generated.strings") { }

    [Fact(DisplayName = "Basic_Emoji.js")]
    public Task Basic_Emoji()
        => ExecutionTest("Basic_Emoji");

    [Fact(DisplayName = "Emoji_Keycap_Sequence.js")]
    public Task Emoji_Keycap_Sequence()
        => ExecutionTest("Emoji_Keycap_Sequence");

    [Fact(DisplayName = "RGI_Emoji.js")]
    public Task RGI_Emoji()
        => ExecutionTest("RGI_Emoji");

    [Fact(DisplayName = "RGI_Emoji_Flag_Sequence.js")]
    public Task RGI_Emoji_Flag_Sequence()
        => ExecutionTest("RGI_Emoji_Flag_Sequence");

    [Fact(DisplayName = "RGI_Emoji_Modifier_Sequence.js")]
    public Task RGI_Emoji_Modifier_Sequence()
        => ExecutionTest("RGI_Emoji_Modifier_Sequence");

    [Fact(DisplayName = "RGI_Emoji_Tag_Sequence.js")]
    public Task RGI_Emoji_Tag_Sequence()
        => ExecutionTest("RGI_Emoji_Tag_Sequence");

    [Fact(DisplayName = "RGI_Emoji_ZWJ_Sequence.js")]
    public Task RGI_Emoji_ZWJ_Sequence()
        => ExecutionTest("RGI_Emoji_ZWJ_Sequence");
}
