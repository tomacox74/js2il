using JavaScriptRuntime;
using JsObjectConstructor = JavaScriptRuntime.Object;

namespace Jroc.Tests;

public sealed class RegExpObjectRepresentationTests
{
    [Theory]
    [InlineData("Script=Hrkt")]
    [InlineData("sc=Katakana_Or_Hiragana")]
    [InlineData("Script_Extensions=Hrkt")]
    [InlineData("scx=Katakana_Or_Hiragana")]
    public void UnicodeScriptProperties_RejectCompositeIsoScriptAliases(string property)
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        foreach (var flags in new[] { "u", "v" })
        {
            Assert.Throws<JavaScriptRuntime.SyntaxError>(
                () => new JavaScriptRuntime.RegExp($"\\p{{{property}}}", flags));
        }
    }

    [Theory]
    [InlineData("iu", "Script=Greek", "\u03B9", true)]
    [InlineData("iv", "Script=Greek", "\u03B9", false)]
    [InlineData("iu", "Script=Greek", "\u03BC", true)]
    [InlineData("iv", "Script=Greek", "\u03BC", false)]
    [InlineData("iu", "Script_Extensions=Greek", "\u03B9", false)]
    [InlineData("iv", "Script_Extensions=Greek", "\u03B9", false)]
    [InlineData("iu", "Script_Extensions=Greek", "\u03BC", true)]
    [InlineData("iv", "Script_Extensions=Greek", "\u03BC", false)]
    [InlineData("iu", "ASCII", "K", true)]
    [InlineData("iv", "ASCII", "K", false)]
    [InlineData("iu", "Lowercase_Letter", "a", true)]
    [InlineData("iv", "Lowercase_Letter", "a", false)]
    [InlineData("iu", "General_Category=Uppercase_Letter", "A", true)]
    [InlineData("iv", "General_Category=Uppercase_Letter", "A", false)]
    [InlineData("iu", "gc=Ll", "\U00010400", true)]
    [InlineData("iv", "gc=Ll", "\U00010400", false)]
    [InlineData("iu", "Changes_When_Casefolded", "a", true)]
    [InlineData("iv", "Changes_When_Casefolded", "a", false)]
    public void UnicodePropertyComplements_RespectModeSpecificCaseFolding(
        string flags, string property, string input, bool expected)
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        Assert.Equal(expected, new JavaScriptRuntime.RegExp($"^\\P{{{property}}}$", flags).test(input));
        Assert.Equal(expected, new JavaScriptRuntime.RegExp($"^[\\P{{{property}}}]$", flags).test(input));
    }

    [Theory]
    [InlineData("u", "^(?i:\\p{Script=Greek})$", "\u0345", true)]
    [InlineData("u", "^(?i:\\P{Script=Greek})$", "\u03B9", true)]
    [InlineData("v", "^(?i:\\P{Script=Greek})$", "\u03B9", false)]
    [InlineData("iu", "^(?-i:\\p{Script=Greek})$", "\u0345", false)]
    [InlineData("iv", "^(?-i:\\P{Script=Greek})$", "\u0345", true)]
    [InlineData("u", "^(?i:\\p{sc=Greek})\\p{sc=Greek}$", "\u0345\u0345", false)]
    [InlineData("iu", "^(?-i:\\p{sc=Greek})\\p{sc=Greek}$", "\u03B9\u0345", true)]
    public void UnicodeScriptProperties_RespectScopedCaseFolding(
        string flags, string pattern, string input, bool expected)
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        Assert.Equal(expected, new JavaScriptRuntime.RegExp(pattern, flags).test(input));
    }

    [Fact]
    public void RegExpWrappers_UseInlineJsObjectStorage()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var regExp = new JavaScriptRuntime.RegExp("a", "g");
        var customPrototype = new JsObject();

        Assert.IsAssignableFrom<JsObject>(regExp);
        Assert.Same(JavaScriptRuntime.RegExp.Prototype, JsObjectConstructor.getPrototypeOf(regExp));
        Assert.Equal("a", ObjectRuntime.GetProperty(regExp, "source"));
        Assert.Equal("g", ObjectRuntime.GetProperty(regExp, "flags"));

        var lastIndex = Assert.IsAssignableFrom<JsObject>(
            JsObjectConstructor.getOwnPropertyDescriptor(regExp, "lastIndex"));
        Assert.False((bool)ObjectRuntime.GetProperty(lastIndex, "enumerable")!);
        Assert.False((bool)ObjectRuntime.GetProperty(lastIndex, "configurable")!);
        Assert.True((bool)ObjectRuntime.GetProperty(lastIndex, "writable")!);

        ObjectRuntime.SetProperty(regExp, "custom", 42d);
        Assert.Equal(42d, ObjectRuntime.GetProperty(regExp, "custom"));
        Assert.True(JsObjectConstructor.hasOwn(regExp, "custom"));

        ObjectRuntime.SetProperty(regExp, "lastIndex", 2d);
        Assert.Equal(2d, regExp.lastIndex);

        JsObjectConstructor.setPrototypeOf(regExp, customPrototype);
        Assert.Same(customPrototype, JsObjectConstructor.getPrototypeOf(regExp));

        JsObjectConstructor.freeze(regExp);
        Assert.True(JsObjectConstructor.isFrozen(regExp));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(regExp, "custom", 0d));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(regExp, "lastIndex", 0d));
    }

    [Fact]
    public void RegExpPrototypeDescriptorMutations_AreIsolatedAcrossRuntimes()
    {
        var mutationResult = InMemoryTestCompiler.CompileAndExecute(
            "mutate-regexp-prototype-descriptors",
            "RegExp.PrototypeIsolation",
            GetDescriptorIsolationScript);
        var readResult = InMemoryTestCompiler.CompileAndExecute(
            "read-regexp-prototype-descriptors",
            "RegExp.PrototypeIsolation",
            GetDescriptorIsolationScript);

        Assert.Equal($"runtime-one{Environment.NewLine}", mutationResult.Output);
        Assert.Equal($"true{Environment.NewLine}", readResult.Output);
    }

    [Fact]
    public void MatchAll_CustomExecReturningPrimitive_ThrowsTypeError()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "regexp-match-all-primitive-exec",
            "RegExp.MatchAll",
            static _ => ("""
                var iterator = /./g[Symbol.matchAll]('a');
                RegExp.prototype.exec = function() {
                  return 1;
                };

                try {
                  iterator.next();
                  console.log(false);
                } catch (error) {
                  console.log(error instanceof TypeError);
                }
                """, null));

        Assert.Equal($"true{Environment.NewLine}", result.Output);
    }

    [Fact]
    public void UnicodeExec_LastIndexInsideSurrogatePair_NormalizesToCodePointBoundary()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "regexp-unicode-last-index-surrogate-pair",
            "RegExp.UnicodeLastIndex",
            static _ => ("""
                const regexp = /./gu;
                regexp.lastIndex = 1;
                const match = regexp.exec('𝌆');
                console.log(match.index + ":" + match[0] + ":" + regexp.lastIndex);
                """, null));

        Assert.Equal($"0:𝌆:2{Environment.NewLine}", result.Output);
    }

    [Fact]
    public void Exec_QuantifiedLookaheadCapture_RemainsParticipating()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "regexp-quantified-lookahead-capture",
            "RegExp.LookaheadCapture",
            static _ => ("""
                const match = /((?=(ab))a)+/.exec('ab');
                console.log(match[1] + ":" + match[2]);
                """, null));

        Assert.Equal($"a:ab{Environment.NewLine}", result.Output);
    }

    private static (string Script, string? SourcePath) GetDescriptorIsolationScript(string testName)
        => testName switch
        {
            "mutate-regexp-prototype-descriptors" => ("""
                Object.defineProperty(RegExp.prototype, "descriptorLeakCheck", {
                  value: "runtime-one",
                  enumerable: true,
                  configurable: true,
                  writable: true
                });
                console.log(new RegExp("a").descriptorLeakCheck);
                """, null),
            "read-regexp-prototype-descriptors" => ("""
                console.log(Object.getOwnPropertyDescriptor(
                  RegExp.prototype,
                  "descriptorLeakCheck"
                ) === undefined);
                """, null),
            _ => throw new ArgumentOutOfRangeException(nameof(testName), testName, "Unknown descriptor isolation script.")
        };
}
