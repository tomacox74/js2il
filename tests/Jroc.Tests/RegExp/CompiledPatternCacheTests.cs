using JavaScriptRuntime;

namespace Jroc.Tests.RegExp;

/// <summary>
/// Regular expression construction reuses cached, pattern-derived compilation artifacts.
/// These tests pin the observable behaviour that sharing must not change.
/// </summary>
public sealed class CompiledPatternCacheTests
{
    [Fact]
    public void RepeatedConstructionKeepsIndependentInstanceState()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();

        var first = new JavaScriptRuntime.RegExp("a", "g");
        var second = new JavaScriptRuntime.RegExp("a", "g");

        Assert.NotSame(first, second);

        first.lastIndex = 5d;

        Assert.Equal(5d, TypeUtilities.ToNumber(first.lastIndex));
        Assert.Equal(0d, TypeUtilities.ToNumber(second.lastIndex));
        Assert.True(Matches(first, "banana"));
        Assert.True(Matches(second, "banana"));
    }

    [Fact]
    public void FlagsThatAffectCompilationAreNotShared()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();

        var plain = new JavaScriptRuntime.RegExp("a.b", string.Empty);
        var ignoreCase = new JavaScriptRuntime.RegExp("a.b", "i");
        var dotAll = new JavaScriptRuntime.RegExp("a.b", "s");

        Assert.True(Matches(plain, "axb"));
        Assert.False(Matches(plain, "AxB"));
        Assert.True(Matches(ignoreCase, "AxB"));
        Assert.False(Matches(plain, "a\nb"));
        Assert.True(Matches(dotAll, "a\nb"));
    }

    private static bool Matches(JavaScriptRuntime.RegExp regExp, string input)
        => TypeUtilities.ToBoolean(regExp.test(input));

    [Fact]
    public void NamedGroupsRemainAvailableForCachedPatterns()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();

        for (var attempt = 0; attempt < 3; attempt++)
        {
            var regExp = new JavaScriptRuntime.RegExp("(?<year>\\d{4})-(?<month>\\d{2})", string.Empty);
            var match = regExp.exec("2026-09") as JavaScriptRuntime.Array;

            Assert.NotNull(match);
            var groups = ObjectRuntime.GetProperty(match!, "groups");
            Assert.NotNull(groups);
            Assert.Equal("2026", ObjectRuntime.GetProperty(groups!, "year"));
            Assert.Equal("09", ObjectRuntime.GetProperty(groups!, "month"));
        }
    }

    [Fact]
    public void InvalidPatternsKeepThrowingSyntaxError()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();

        Assert.Throws<SyntaxError>(() => new JavaScriptRuntime.RegExp("(", string.Empty));
        Assert.Throws<SyntaxError>(() => new JavaScriptRuntime.RegExp("(", string.Empty));
    }
}
