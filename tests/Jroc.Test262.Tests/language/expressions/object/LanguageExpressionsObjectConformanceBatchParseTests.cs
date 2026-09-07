using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.object_;

public class LanguageExpressionsObjectConformanceBatchParseTests : FileSystemExecutionTestsBase
{
    public LanguageExpressionsObjectConformanceBatchParseTests() : base("language/expressions/object", "language.expressions.object") { }

    [Fact(DisplayName = "11.1.5-1gs.js")]
    public Task _11_1_5_1gs() => CompilationFailureTest("11.1.5-1gs");

    [Fact(DisplayName = "__proto__-duplicate.js")]
    public Task __proto___duplicate() => CompilationFailureTest("__proto__-duplicate");

    [Fact(DisplayName = "cover-initialized-name.js")]
    public Task cover_initialized_name() => CompilationFailureTest("cover-initialized-name");

    [Fact(DisplayName = "getter-body-strict-inside.js")]
    public Task getter_body_strict_inside() => CompilationFailureTest("getter-body-strict-inside");

    [Fact(DisplayName = "getter-body-strict-outside.js")]
    public Task getter_body_strict_outside() => CompilationFailureTest("getter-body-strict-outside");

    [Fact(DisplayName = "getter-param-dflt.js")]
    public Task getter_param_dflt() => CompilationFailureTest("getter-param-dflt");

}
