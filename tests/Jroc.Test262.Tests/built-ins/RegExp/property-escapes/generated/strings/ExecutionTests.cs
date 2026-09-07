using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.property_escapes.generated.strings;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.property_escapes.generated.strings") { }

    [Fact(DisplayName = "Basic_Emoji-negative-CharacterClass.js")]
    public Task Basic_Emoji_negative_CharacterClass()
        => CompilationFailureTest("Basic_Emoji-negative-CharacterClass");

    [Fact(DisplayName = "Basic_Emoji-negative-P.js")]
    public Task Basic_Emoji_negative_P()
        => CompilationFailureTest("Basic_Emoji-negative-P");

    [Fact(DisplayName = "Basic_Emoji-negative-u.js")]
    public Task Basic_Emoji_negative_u()
        => CompilationFailureTest("Basic_Emoji-negative-u");

    [Fact(DisplayName = "Emoji_Keycap_Sequence-negative-CharacterClass.js")]
    public Task Emoji_Keycap_Sequence_negative_CharacterClass()
        => CompilationFailureTest("Emoji_Keycap_Sequence-negative-CharacterClass");

    [Fact(DisplayName = "Emoji_Keycap_Sequence-negative-P.js")]
    public Task Emoji_Keycap_Sequence_negative_P()
        => CompilationFailureTest("Emoji_Keycap_Sequence-negative-P");

    [Fact(DisplayName = "Emoji_Keycap_Sequence-negative-u.js")]
    public Task Emoji_Keycap_Sequence_negative_u()
        => CompilationFailureTest("Emoji_Keycap_Sequence-negative-u");

    [Fact(DisplayName = "RGI_Emoji-negative-CharacterClass.js")]
    public Task RGI_Emoji_negative_CharacterClass()
        => CompilationFailureTest("RGI_Emoji-negative-CharacterClass");

    [Fact(DisplayName = "RGI_Emoji-negative-P.js")]
    public Task RGI_Emoji_negative_P()
        => CompilationFailureTest("RGI_Emoji-negative-P");

    [Fact(DisplayName = "RGI_Emoji-negative-u.js")]
    public Task RGI_Emoji_negative_u()
        => CompilationFailureTest("RGI_Emoji-negative-u");

    [Fact(DisplayName = "RGI_Emoji_Flag_Sequence-negative-CharacterClass.js")]
    public Task RGI_Emoji_Flag_Sequence_negative_CharacterClass()
        => CompilationFailureTest("RGI_Emoji_Flag_Sequence-negative-CharacterClass");

    [Fact(DisplayName = "RGI_Emoji_Flag_Sequence-negative-P.js")]
    public Task RGI_Emoji_Flag_Sequence_negative_P()
        => CompilationFailureTest("RGI_Emoji_Flag_Sequence-negative-P");

    [Fact(DisplayName = "RGI_Emoji_Flag_Sequence-negative-u.js")]
    public Task RGI_Emoji_Flag_Sequence_negative_u()
        => CompilationFailureTest("RGI_Emoji_Flag_Sequence-negative-u");

    [Fact(DisplayName = "RGI_Emoji_Modifier_Sequence-negative-CharacterClass.js")]
    public Task RGI_Emoji_Modifier_Sequence_negative_CharacterClass()
        => CompilationFailureTest("RGI_Emoji_Modifier_Sequence-negative-CharacterClass");

    [Fact(DisplayName = "RGI_Emoji_Modifier_Sequence-negative-P.js")]
    public Task RGI_Emoji_Modifier_Sequence_negative_P()
        => CompilationFailureTest("RGI_Emoji_Modifier_Sequence-negative-P");

    [Fact(DisplayName = "RGI_Emoji_Modifier_Sequence-negative-u.js")]
    public Task RGI_Emoji_Modifier_Sequence_negative_u()
        => CompilationFailureTest("RGI_Emoji_Modifier_Sequence-negative-u");

    [Fact(DisplayName = "RGI_Emoji_Tag_Sequence-negative-CharacterClass.js")]
    public Task RGI_Emoji_Tag_Sequence_negative_CharacterClass()
        => CompilationFailureTest("RGI_Emoji_Tag_Sequence-negative-CharacterClass");

    [Fact(DisplayName = "RGI_Emoji_Tag_Sequence-negative-P.js")]
    public Task RGI_Emoji_Tag_Sequence_negative_P()
        => CompilationFailureTest("RGI_Emoji_Tag_Sequence-negative-P");

    [Fact(DisplayName = "RGI_Emoji_Tag_Sequence-negative-u.js")]
    public Task RGI_Emoji_Tag_Sequence_negative_u()
        => CompilationFailureTest("RGI_Emoji_Tag_Sequence-negative-u");

    [Fact(DisplayName = "RGI_Emoji_ZWJ_Sequence-negative-CharacterClass.js")]
    public Task RGI_Emoji_ZWJ_Sequence_negative_CharacterClass()
        => CompilationFailureTest("RGI_Emoji_ZWJ_Sequence-negative-CharacterClass");

    [Fact(DisplayName = "RGI_Emoji_ZWJ_Sequence-negative-P.js")]
    public Task RGI_Emoji_ZWJ_Sequence_negative_P()
        => CompilationFailureTest("RGI_Emoji_ZWJ_Sequence-negative-P");

    [Fact(DisplayName = "RGI_Emoji_ZWJ_Sequence-negative-u.js")]
    public Task RGI_Emoji_ZWJ_Sequence_negative_u()
        => CompilationFailureTest("RGI_Emoji_ZWJ_Sequence-negative-u");

}
