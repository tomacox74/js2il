using JavaScriptRuntime;
using JsObjectConstructor = JavaScriptRuntime.Object;

namespace Jroc.Tests;

public sealed class RegExpObjectRepresentationTests
{
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
