using JavaScriptRuntime;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jroc.Tests.RegExp;

/// <summary>
/// Regular expression construction reuses cached, pattern-derived compilation artifacts.
/// These tests pin the observable behaviour that sharing must not change.
/// </summary>
public sealed class CompiledPatternCacheTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ColdPatternsDoNotRetainInputSizedClrRunnerBuffers(bool literal)
    {
        var suffix = Guid.NewGuid().ToString("N");
        var source = "^(?<letters>\\P{Script=Adlam}+)" + suffix + "$";
        var input = string.Concat(Enumerable.Repeat("\U0001F600", 100_000)) + suffix;
        var weak = MatchColdPattern(source, input, literal);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Assert.False(weak.TryGetTarget(out _));

        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var recreated = literal
            ? JavaScriptRuntime.RegExp.CreateLiteral(source, "u")
            : new JavaScriptRuntime.RegExp(source, "u");
        var result = Assert.IsType<JavaScriptRuntime.Array>(recreated.exec(input));
        Assert.Equal(input[..^suffix.Length], ObjectRuntime.GetProperty(ObjectRuntime.GetProperty(result, "groups")!, "letters"));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference<System.Text.RegularExpressions.Regex> MatchColdPattern(
        string source, string input, bool literal)
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var expression = literal
            ? JavaScriptRuntime.RegExp.CreateLiteral(source, "u")
            : new JavaScriptRuntime.RegExp(source, "u");
        Assert.True(Matches(expression, input));
        var regex = (System.Text.RegularExpressions.Regex)typeof(JavaScriptRuntime.RegExp)
            .GetField("_regex", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(expression)!;
        return new WeakReference<System.Text.RegularExpressions.Regex>(regex);
    }

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

    [Fact]
    public void IgnoreCaseCompilationDoesNotDependOnCurrentCulture()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var previousCulture = CultureInfo.CurrentCulture;

        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
            var regExp = new JavaScriptRuntime.RegExp("^(?:i)$", "i");

            Assert.False(Matches(regExp, "\u0130"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }
}
