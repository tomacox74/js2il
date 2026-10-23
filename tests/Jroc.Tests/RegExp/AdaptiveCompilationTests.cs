using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Globalization;
using JavaScriptRuntime;

namespace Jroc.Tests.RegExp;

[Collection(Jroc.Tests.String.BuiltinAdapterRuntimeTestsCollection.CollectionName)]
public sealed class AdaptiveCompilationTests
{
    [Fact]
    public void ColdPatternsRemainInterpretedAndThirdConstructionPromotes()
    {
        var budget = new RegExpCompilationBudget(1, enabled: true);
        var tier = new RegExpAdaptiveCompilation(budget);
        var interpreted = new Regex("a[bc]+d", RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2));

        Assert.Same(interpreted, tier.GetRegexForConstruction(interpreted));
        Assert.Same(interpreted, tier.GetRegexForConstruction(interpreted));
        var compiled = tier.GetRegexForConstruction(interpreted);

        Assert.NotSame(interpreted, compiled);
        Assert.True(compiled.Options.HasFlag(RegexOptions.Compiled));
        Assert.Equal(interpreted.MatchTimeout, compiled.MatchTimeout);
        Assert.Equal(interpreted.Options, compiled.Options & ~RegexOptions.Compiled);
        Assert.Same(compiled, tier.GetRegexForConstruction(interpreted));
        Assert.Matches(interpreted, "abbd");
        Assert.Matches(compiled, "abbd");
        Assert.Equal(1, budget.Count);
    }

    [Fact]
    public void PromotionFailureIsReportedAndNeverRetried()
    {
        var attempts = 0;
        var budget = new RegExpCompilationBudget(1, enabled: true);
        var interpreted = new Regex("a");
        var tier = new RegExpAdaptiveCompilation(budget, _ =>
        {
            attempts++;
            throw new NotSupportedException("simulated compilation failure");
        });

        for (var index = 0; index < 100; index++)
        {
            Assert.Same(interpreted, tier.GetRegexForConstruction(interpreted));
        }

        Assert.Equal(1, attempts);
        Assert.Equal("simulated compilation failure", tier.PromotionFailure);
        Assert.Equal(0, budget.Count);
        tier.Retire();
        Assert.Equal(0, budget.Count);
    }

    [Fact]
    public void UnexpectedCompilerErrorsPropagateWithoutLeakingReservations()
    {
        var budget = new RegExpCompilationBudget(1, enabled: true);
        var interpreted = new Regex("a");
        var tier = new RegExpAdaptiveCompilation(budget, _ => throw new InvalidOperationException("unexpected"));
        tier.GetRegexForConstruction(interpreted);
        tier.GetRegexForConstruction(interpreted);

        Assert.Throws<InvalidOperationException>(() => tier.GetRegexForConstruction(interpreted));
        Assert.Equal(0, budget.Count);
        Assert.Same(interpreted, tier.GetRegexForConstruction(interpreted));
    }

    [Fact]
    public void DisabledDynamicCodeNeverAttemptsPromotion()
    {
        var budget = new RegExpCompilationBudget(1, enabled: false);
        var tier = new RegExpAdaptiveCompilation(budget, _ => throw new InvalidOperationException());
        var interpreted = new Regex("a");
        for (var index = 0; index < 100; index++)
        {
            Assert.Same(interpreted, tier.GetRegexForConstruction(interpreted));
        }
        Assert.Equal(0, budget.Count);
    }

    [Fact]
    public void MatchingAnExistingInstanceDoesNotCountAsConstruction()
    {
        var budget = new RegExpCompilationBudget(1, enabled: true);
        var tier = new RegExpAdaptiveCompilation(budget);
        var selected = tier.GetRegexForConstruction(new Regex("a+"));
        for (var index = 0; index < 10_000; index++)
        {
            Assert.Matches(selected, "aaa");
        }
        Assert.False(selected.Options.HasFlag(RegexOptions.Compiled));
        Assert.Equal(0, budget.Count);
    }

    [Fact]
    public async Task ConcurrentThresholdCrossingCompilesOnce()
    {
        var attempts = 0;
        var budget = new RegExpCompilationBudget(1, enabled: true);
        var interpreted = new Regex("a+");
        var tier = new RegExpAdaptiveCompilation(budget, regex =>
        {
            Interlocked.Increment(ref attempts);
            Thread.Sleep(10);
            return new Regex(regex.ToString(), regex.Options | RegexOptions.Compiled);
        });
        var results = await Task.WhenAll(Enumerable.Range(0, 64)
            .Select(_ => Task.Run(() => tier.GetRegexForConstruction(interpreted))));

        Assert.Equal(1, attempts);
        Assert.All(results, regex => Assert.Matches(regex, "aaa"));
        Assert.True(tier.GetRegexForConstruction(interpreted).Options.HasFlag(RegexOptions.Compiled));
        tier.Retire();
        tier.Retire();
        Assert.Equal(0, budget.Count);
    }

    [Fact]
    public async Task EvictedInFlightPromotionKeepsReservationUntilCompletion()
    {
        var budget = new RegExpCompilationBudget(1, enabled: true);
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        var interpreted = new Regex("a+");
        var tier = new RegExpAdaptiveCompilation(budget, regex =>
        {
            entered.Set();
            Assert.True(release.Wait(TimeSpan.FromSeconds(10)));
            return new Regex(regex.ToString(), RegexOptions.Compiled);
        });
        tier.GetRegexForConstruction(interpreted);
        tier.GetRegexForConstruction(interpreted);
        var promotion = Task.Run(() => tier.GetRegexForConstruction(interpreted));
        try
        {
            Assert.True(entered.Wait(TimeSpan.FromSeconds(10)));
            Assert.Same(interpreted, tier.GetRegexForConstruction(interpreted));
            tier.Retire();
            tier.Retire();
            Assert.Equal(1, budget.Count);
            Assert.False(budget.TryReserve());
        }
        finally
        {
            release.Set();
        }
        Assert.Same(interpreted, await promotion);
        Assert.Equal(0, budget.Count);
        Assert.Same(interpreted, tier.GetRegexForConstruction(interpreted));
    }

    [Fact]
    public void CompiledCapacityIsBoundedAndRetirementReleasesExactlyOnce()
    {
        var budget = new RegExpCompilationBudget(2, enabled: true);
        var interpreted = new Regex("a+");
        var entries = Enumerable.Range(0, 1_000).Select(_ => new RegExpAdaptiveCompilation(budget)).ToArray();
        foreach (var tier in entries)
        {
            for (var construction = 0; construction < 3; construction++)
            {
                tier.GetRegexForConstruction(interpreted);
            }
            Assert.InRange(budget.Count, 0, 2);
        }
        Assert.Equal(2, budget.Count);
        Assert.False(entries[2].GetRegexForConstruction(interpreted).Options.HasFlag(RegexOptions.Compiled));
        foreach (var tier in entries)
        {
            tier.Retire();
            tier.Retire();
            Assert.Same(interpreted, tier.GetRegexForConstruction(interpreted));
        }
        Assert.Equal(0, budget.Count);
    }

    [Fact]
    public async Task ConcurrentPromotionAndEvictionRespectTheSharedBudget()
    {
        var budget = new RegExpCompilationBudget(4, enabled: true);
        var interpreted = new Regex("a+");
        await Task.WhenAll(Enumerable.Range(0, 128).Select(_ => Task.Run(async () =>
        {
            var tier = new RegExpAdaptiveCompilation(budget);
            tier.GetRegexForConstruction(interpreted);
            tier.GetRegexForConstruction(interpreted);
            var promotion = Task.Run(() => tier.GetRegexForConstruction(interpreted));
            await Task.Yield();
            tier.Retire();
            await promotion;
            tier.Retire();
            Assert.InRange(budget.Count, 0, 4);
        })));
        Assert.Equal(0, budget.Count);
    }

    [Fact]
    public void DynamicAndLiteralEvaluationsShareTheirConstructionCounter()
    {
        WithFreshCache(() =>
        {
            var source = "(?<letter>a)(b)?" + Guid.NewGuid().ToString("N");
            var first = new JavaScriptRuntime.RegExp(source, "g");
            var second = JavaScriptRuntime.RegExp.CreateLiteral(source, "g");
            var promoted = PromoteLiteral(source, "g");
            Assert.False(first.Regex.Options.HasFlag(RegexOptions.Compiled));
            Assert.Same(first.Regex, second.Regex);
            Assert.Equal(RuntimeFeature.IsDynamicCodeCompiled, promoted.Regex.Options.HasFlag(RegexOptions.Compiled));
            Assert.NotSame(first, promoted);
            first.lastIndex = 123d;
            Assert.Equal(0d, TypeUtilities.ToNumber(promoted.lastIndex));

            var input = "ab" + source["(?<letter>a)(b)?".Length..];
            first.lastIndex = 0d;
            var oldResult = Assert.IsType<JavaScriptRuntime.Array>(first.exec(input));
            var newResult = Assert.IsType<JavaScriptRuntime.Array>(promoted.exec(input));
            Assert.Equal(oldResult[1], newResult[1]);
            Assert.Equal(oldResult[2], newResult[2]);
            Assert.Equal("a", ObjectRuntime.GetItem(ObjectRuntime.GetItem(newResult, "groups")!, "letter"));
        });
    }

    [Theory]
    [InlineData("i")]
    [InlineData("m")]
    [InlineData("s")]
    [InlineData("u")]
    [InlineData("v")]
    public void CompilationAffectingFlagsKeepIndependentTiers(string flags)
    {
        WithFreshCache(() =>
        {
            var source = "a.b" + Guid.NewGuid().ToString("N");
            var first = new JavaScriptRuntime.RegExp(source, "");
            var different = new JavaScriptRuntime.RegExp(source, flags);
            _ = new JavaScriptRuntime.RegExp(source, "");
            JavaScriptRuntime.RegExp promoted = null!;
            for (var construction = 3; construction <= JavaScriptRuntime.RegExp.CompiledRegexPromotionThreshold; construction++)
            {
                promoted = new JavaScriptRuntime.RegExp(source, "");
            }
            Assert.False(first.Regex.Options.HasFlag(RegexOptions.Compiled));
            Assert.False(different.Regex.Options.HasFlag(RegexOptions.Compiled));
            Assert.Equal(RuntimeFeature.IsDynamicCodeCompiled, promoted.Regex.Options.HasFlag(RegexOptions.Compiled));
        });
    }

    [Fact]
    public void PromotedIgnoreCaseMatchingRemainsCultureInvariant()
    {
        WithFreshCache(() =>
        {
            var previous = CultureInfo.CurrentCulture;
            try
            {
                var source = "^(?:i|" + Guid.NewGuid().ToString("N") + ")$";
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                var first = JavaScriptRuntime.RegExp.CreateLiteral(source, "i");
                _ = JavaScriptRuntime.RegExp.CreateLiteral(source, "i");
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
                var promoted = PromoteLiteral(source, "i");

                Assert.False(TypeUtilities.ToBoolean(first.test("\u0130")));
                Assert.False(TypeUtilities.ToBoolean(promoted.test("\u0130")));
                Assert.True(TypeUtilities.ToBoolean(promoted.test("I")));
                Assert.Equal(RuntimeFeature.IsDynamicCodeCompiled, promoted.Regex.Options.HasFlag(RegexOptions.Compiled));
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        });
    }

    [Fact]
    public async Task ConcurrentRealmsShareOnlyPatternArtifacts()
    {
        var source = "a[bc]+" + Guid.NewGuid().ToString("N");
        WithFreshCache(() => { });
        var expressions = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => Task.Run(() =>
        {
            var context = RuntimeExecutionContext.GetOrCreate(RuntimeServices.BuildServiceProvider());
            using var scope = context.EnterAsRoot();
            JavaScriptRuntime.RegExp result = null!;
            for (var index = 0; index < JavaScriptRuntime.RegExp.CompiledRegexPromotionThreshold; index++)
            {
                result = JavaScriptRuntime.RegExp.CreateLiteral(source, "g");
            }
            return result;
        })));

        Assert.All(expressions, expression => Assert.Equal(0d, TypeUtilities.ToNumber(expression.lastIndex)));
        expressions[0].lastIndex = 42d;
        Assert.All(expressions.Skip(1), expression => Assert.Equal(0d, TypeUtilities.ToNumber(expression.lastIndex)));
        var selected = expressions.Select(expression => expression.Regex)
            .Where(regex => regex.Options.HasFlag(RegexOptions.Compiled)).Distinct().ToArray();
        Assert.Equal(RuntimeFeature.IsDynamicCodeCompiled ? 1 : 0, selected.Length);
    }

    [Theory]
    [InlineData("")]
    [InlineData("g")]
    [InlineData("y")]
    [InlineData("u")]
    [InlineData("v")]
    [InlineData("d")]
    [InlineData("s")]
    [InlineData("i")]
    public void PromotedMatchingPreservesCaptureResetAndLastIndex(string flags)
    {
        WithFreshCache(() =>
        {
            var suffix = Guid.NewGuid().ToString("N");
            var source = "(a|(b))+" + suffix;
            var first = JavaScriptRuntime.RegExp.CreateLiteral(source, flags);
            _ = JavaScriptRuntime.RegExp.CreateLiteral(source, flags);
            var promoted = PromoteLiteral(source, flags);
            var input = "ba" + suffix;
            var oldResult = Assert.IsType<JavaScriptRuntime.Array>(first.exec(input));
            var newResult = Assert.IsType<JavaScriptRuntime.Array>(promoted.exec(input));
            Assert.Equal(oldResult.Count, newResult.Count);
            Assert.Equal("a", newResult[1]);
            Assert.Null(newResult[2]);
            Assert.Equal(first.lastIndex, promoted.lastIndex);
            Assert.Equal(RuntimeFeature.IsDynamicCodeCompiled, promoted.Regex.Options.HasFlag(RegexOptions.Compiled));
        });
    }

    [Fact]
    public void InvalidDynamicAndLiteralPatternsThrowFreshErrors()
    {
        WithFreshCache(() =>
        {
            var source = "(" + Guid.NewGuid().ToString("N");
            var first = Assert.Throws<SyntaxError>(() => new JavaScriptRuntime.RegExp(source));
            var second = Assert.Throws<SyntaxError>(() => new JavaScriptRuntime.RegExp(source));
            var third = Assert.Throws<SyntaxError>(() => JavaScriptRuntime.RegExp.CreateLiteral(source, ""));
            var fourth = Assert.Throws<SyntaxError>(() => JavaScriptRuntime.RegExp.CreateLiteral(source, ""));
            Assert.NotSame(first, second);
            Assert.NotSame(third, fourth);
        });
    }

    [Fact]
    public void EvictedLiteralTemplatesCannotKeepAnUnaccountedCompiledTier()
    {
        WithFreshCache(() =>
        {
            var source = "a+" + Guid.NewGuid().ToString("N");
            var first = JavaScriptRuntime.RegExp.CreateLiteral(source, "");
            _ = JavaScriptRuntime.RegExp.CreateLiteral(source, "");
            var promoted = PromoteLiteral(source, "");
            Assert.Equal(RuntimeFeature.IsDynamicCodeCompiled, promoted.Regex.Options.HasFlag(RegexOptions.Compiled));
            FloodColdPatterns();
            var retired = JavaScriptRuntime.RegExp.CreateLiteral(source, "");
            Assert.Same(first.Regex, retired.Regex);
            Assert.Equal(RuntimeFeature.IsDynamicCodeCompiled, promoted.Regex.Options.HasFlag(RegexOptions.Compiled));
        });
    }

    [Fact]
    public void ColdLiteralRetiredBeforeTierCreationCannotPromoteLater()
    {
        WithFreshCache(() =>
        {
            var source = "a+" + Guid.NewGuid().ToString("N");
            var first = JavaScriptRuntime.RegExp.CreateLiteral(source, "");
            FloodColdPatterns();
            for (var index = 0; index < JavaScriptRuntime.RegExp.CompiledRegexPromotionThreshold * 2; index++)
            {
                var expression = JavaScriptRuntime.RegExp.CreateLiteral(source, "");
                Assert.Same(first.Regex, expression.Regex);
            }
            Assert.Equal(0, JavaScriptRuntime.RegExp.GetPatternCacheStatistics().CompiledEntries);
        });
    }

    [Fact]
    public void DistinctPatternFloodKeepsBothCacheBoundsAndDoesNotCompile()
    {
        WithFreshCache(() =>
        {
            var prefix = new string('x', 8_192) + Guid.NewGuid().ToString("N");
            for (var index = 0; index < 2_000; index++)
            {
                var expression = new JavaScriptRuntime.RegExp(prefix + index);
                Assert.False(expression.Regex.Options.HasFlag(RegexOptions.Compiled));
            }
            var statistics = JavaScriptRuntime.RegExp.GetPatternCacheStatistics();
            Assert.InRange(statistics.Entries, 0, 1_024);
            Assert.InRange(statistics.SourceCharacters, 0, 4 * 1_024 * 1_024);
            Assert.Equal(0, statistics.CompiledEntries);
        });
    }

    private static void WithFreshCache(Action body)
    {
        var context = RuntimeExecutionContext.GetOrCreate(RuntimeServices.BuildServiceProvider());
        using var scope = context.EnterAsRoot();
        FloodColdPatterns();
        body();
    }

    private static JavaScriptRuntime.RegExp PromoteLiteral(string source, string flags)
    {
        JavaScriptRuntime.RegExp result = null!;
        for (var construction = 3; construction <= JavaScriptRuntime.RegExp.CompiledRegexPromotionThreshold; construction++)
        {
            result = JavaScriptRuntime.RegExp.CreateLiteral(source, flags);
            if (construction < JavaScriptRuntime.RegExp.CompiledRegexPromotionThreshold)
            {
                Assert.False(result.Regex.Options.HasFlag(RegexOptions.Compiled));
            }
        }
        return result;
    }

    private static void FloodColdPatterns()
    {
        var prefix = "cold" + Guid.NewGuid().ToString("N");
        for (var index = 0; index < 1_025; index++)
        {
            _ = new JavaScriptRuntime.RegExp(prefix + index);
        }
    }
}
