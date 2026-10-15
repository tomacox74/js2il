using JavaScriptRuntime;

namespace Jroc.Tests.String;

[Collection(BuiltinAdapterRuntimeTestsCollection.CollectionName)]
public sealed class RegExpFastPathRuntimeTests
{
    [Theory]
    [InlineData("(?:)", 600_000)]
    [InlineData("a", 150_000)]
    [InlineData("a+", 30_000)]
    [InlineData(".*", 30_000)]
    public void IntrinsicSpeciesSplitAvoidsPerPositionMatchResults(string pattern, long allocationLimit)
    {
        WithRealm(() =>
        {
            var input = new string('b', 65_538);
            var expression = new JavaScriptRuntime.RegExp(pattern, "");
            var constructor = ObjectRuntime.GetItem(expression, "constructor");
            Assert.Same(constructor, ObjectRuntime.GetItem(constructor!, Symbol.species));

            JavaScriptRuntime.String.SplitWithRegExp(input, expression, null);
            var before = GC.GetAllocatedBytesForCurrentThread();
            var result = JavaScriptRuntime.String.SplitWithRegExp(input, expression, null);
            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Equal(pattern == "(?:)" ? input.Length : pattern == ".*" ? 2 : 1, result.Count);
            Assert.True(allocated < allocationLimit, $"Split allocated {allocated:N0} bytes (limit {allocationLimit:N0}).");
            if (pattern == "(?:)")
            {
                Assert.Same(result[0], result[1]);
            }
        });
    }

    [Fact]
    public void NoMatchReplacementReturnsInputWithoutCopying()
    {
        WithRealm(() =>
        {
            var input = new string('b', 65_538);
            var expression = new JavaScriptRuntime.RegExp("a", "g");
            JavaScriptRuntime.String.ReplaceWithRegExp(input, expression, "replacement");

            var before = GC.GetAllocatedBytesForCurrentThread();
            var result = JavaScriptRuntime.String.ReplaceWithRegExp(input, expression, "replacement");
            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Same(input, result);
            Assert.Equal(0d, TypeUtilities.ToNumber(expression.lastIndex));
            Assert.True(allocated < 10_000, $"No-match replace allocated {allocated:N0} bytes.");
        });
    }

    [Theory]
    [InlineData("(?:)", "", "", null)]
    [InlineData("(?:)", "", "abc", null)]
    [InlineData("(?:)", "u", "\uD83D\uDE00a\uD800", null)]
    [InlineData("(?:)", "v", "\uD83D\uDE00a\uD800", null)]
    [InlineData("(?:)", "", "\uD83D\uDE00a\uD800", 2d)]
    [InlineData("(?:)", "u", "\uD83D\uDE00a\uD800", 2d)]
    [InlineData("a", "", "", null)]
    [InlineData("a", "", "aba", null)]
    [InlineData("a", "y", "baba", null)]
    [InlineData("a", "g", "baba", 2d)]
    [InlineData("a", "i", "bAbA", null)]
    [InlineData("a", "u", "baba", null)]
    [InlineData("a+", "d", "baba", null)]
    [InlineData("a", "", "baba", 0d)]
    [InlineData("a", "", "baba", 1d)]
    [InlineData("a", "", "baba", -1d)]
    [InlineData(".*", "", "abc", null)]
    [InlineData(".*", "", "", null)]
    [InlineData("a*", "", "baab", null)]
    [InlineData("a*?", "", "aba", null)]
    [InlineData("(?=b)", "", "abcb", null)]
    [InlineData("(?<=b)", "", "abcb", null)]
    [InlineData("^|$", "", "abc", null)]
    [InlineData("^", "m", "a\nb", null)]
    [InlineData(".+", "s", "a\nb", null)]
    [InlineData("(a)", "", "aba", null)]
    [InlineData("(a)?b", "", "abc", null)]
    public void OptimizedSplitMatchesObservableExecFallback(string pattern, string flags, string input, double? limit)
    {
        WithRealm(() =>
        {
            var expression = new JavaScriptRuntime.RegExp(pattern, flags) { lastIndex = 7d };
            var optimized = JavaScriptRuntime.String.SplitWithRegExp(input, expression, limit);
            var originalExec = ObjectRuntime.GetItem(JavaScriptRuntime.RegExp.Prototype, "exec");
            var calls = 0;
            ObjectRuntime.SetItem(JavaScriptRuntime.RegExp.Prototype, "exec",
                new BuiltinDelegateFunctionAdapter((BuiltinFunction1)((receiver, value) =>
                {
                    calls++;
                    return CallableOperations.Call1(originalExec, receiver, value);
                })));
            var fallback = JavaScriptRuntime.String.SplitWithRegExp(input, expression, limit);

            Assert.Equal(fallback.Count, optimized.Count);
            for (var index = 0; index < fallback.Count; index++)
            {
                Assert.Equal(fallback[index], optimized[index]);
            }
            Assert.Equal(7d, expression.lastIndex);
            Assert.True(calls > 0 || limit == 0d);
        });
    }

    [Fact]
    public void LimitCoercionCanReplaceExecBeforeFastPathSelection()
    {
        WithRealm(() =>
        {
            var calls = 0;
            var limit = new JsObject();
            ObjectRuntime.SetItem(limit, "valueOf",
                new BuiltinDelegateFunctionAdapter((BuiltinFunction0)(_ =>
                {
                    ObjectRuntime.SetItem(JavaScriptRuntime.RegExp.Prototype, "exec",
                        new BuiltinDelegateFunctionAdapter((BuiltinFunction1)((_, _) =>
                        {
                            calls++;
                            return JsNull.Null;
                        })));
                    return 10d;
                })));

            var result = JavaScriptRuntime.String.SplitWithRegExp(
                "abc", new JavaScriptRuntime.RegExp("(?:)", ""), limit);
            Assert.Single(result);
            Assert.Equal("abc", result[0]);
            Assert.Equal(3, calls);
        });
    }

    [Fact]
    public void IntrinsicSpeciesGetterIsObservedBeforeLimitCoercion()
    {
        WithRealm(() =>
        {
            var observations = new List<string>();
            var constructor = BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.RegExp);
            PropertyDescriptorStore.DefineOrUpdate(constructor, Symbol.species.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Get = new BuiltinDelegateFunctionAdapter((BuiltinFunction0)(receiver =>
                {
                    Assert.Same(constructor, receiver);
                    observations.Add("species");
                    return constructor;
                })),
                Configurable = true
            });
            var limit = new JsObject();
            ObjectRuntime.SetItem(limit, "valueOf",
                new BuiltinDelegateFunctionAdapter((BuiltinFunction0)(_ =>
                {
                    observations.Add("limit");
                    return 2d;
                })));

            var result = JavaScriptRuntime.String.SplitWithRegExp(
                "abc", new JavaScriptRuntime.RegExp("(?:)", ""), limit);
            Assert.Equal(2, result.Count);
            Assert.Equal(new[] { "species", "limit" }, observations);
        });
    }

    [Fact]
    public void BuiltinExecGuardRejectsChangedPrototypeWithoutInvokingGetter()
    {
        WithRealm(() =>
        {
            var expression = new JavaScriptRuntime.RegExp("(?:)", "");
            Assert.True(expression.CanUseBuiltinExecFastPath());
            var prototype = new JsObject();
            PrototypeChain.SetPrototype(prototype, JavaScriptRuntime.RegExp.Prototype);
            PropertyDescriptorStore.DefineOrUpdate(prototype, "exec", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Get = new BuiltinDelegateFunctionAdapter((BuiltinFunction0)(_ =>
                    throw new InvalidOperationException("The guard must not invoke getters.")))
            });
            PrototypeChain.SetPrototype(expression, prototype);

            Assert.False(expression.CanUseBuiltinExecFastPath());
        });
    }

    [Fact]
    public void CustomSpeciesSplitterRemainsObservableAfterLimitCoercion()
    {
        WithRealm(() =>
        {
            JavaScriptRuntime.RegExp? splitter = null;
            var constructorCalls = 0;
            var execCalls = 0;
            var species = new BuiltinDelegateFunctionAdapter(
                (BuiltinFunction2)((_, pattern, flags) =>
                {
                    constructorCalls++;
                    Assert.Equal("y", flags);
                    splitter = new JavaScriptRuntime.RegExp("(?:)", "y");
                    return splitter;
                }), isConstructor: true);
            var constructor = new JsObject();
            ObjectRuntime.SetItem(constructor, Symbol.species, species);
            var expression = new JavaScriptRuntime.RegExp("(?:)", "");
            ObjectRuntime.SetItem(expression, "constructor", constructor);
            var limit = new JsObject();
            ObjectRuntime.SetItem(limit, "valueOf",
                new BuiltinDelegateFunctionAdapter((BuiltinFunction0)(_ =>
                {
                    Assert.NotNull(splitter);
                    ObjectRuntime.SetItem(splitter, "exec",
                        new BuiltinDelegateFunctionAdapter((BuiltinFunction1)((_, _) =>
                        {
                            execCalls++;
                            return JsNull.Null;
                        })));
                    return 10d;
                })));

            var result = JavaScriptRuntime.String.SplitWithRegExp("abc", expression, limit);
            Assert.Equal(1, constructorCalls);
            Assert.Equal(3, execCalls);
            Assert.Single(result);
            Assert.Equal("abc", result[0]);
        });
    }

    [Fact]
    public void NoMatchReplacementStillCoercesReplacementAndCallsExec()
    {
        WithRealm(() =>
        {
            var observations = new List<string>();
            var replacement = new JsObject();
            ObjectRuntime.SetItem(replacement, "toString",
                new BuiltinDelegateFunctionAdapter((BuiltinFunction0)(_ =>
                {
                    observations.Add("replacement");
                    return "unused";
                })));
            var expression = new JavaScriptRuntime.RegExp("a", "g") { lastIndex = 10d };
            ObjectRuntime.SetItem(expression, "exec",
                new BuiltinDelegateFunctionAdapter((BuiltinFunction1)((receiver, _) =>
                {
                    Assert.Same(expression, receiver);
                    Assert.Equal(0d, TypeUtilities.ToNumber(expression.lastIndex));
                    observations.Add("exec");
                    return JsNull.Null;
                })));

            Assert.Equal("bbb", JavaScriptRuntime.String.ReplaceWithRegExp("bbb", expression, replacement));
            Assert.Equal(new[] { "replacement", "exec" }, observations);
        });
    }

    private static void WithRealm(Action body)
    {
        var context = RuntimeExecutionContext.GetOrCreate(RuntimeServices.BuildServiceProvider());
        using var scope = context.EnterAsRoot();
        _ = BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.RegExp);
        body();
    }
}
