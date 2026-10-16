using JavaScriptRuntime;
using JsRegExp = JavaScriptRuntime.RegExp;

namespace Jroc.Tests.RegExp;

public sealed class MemberDispatchTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void FailedExecOnlyAllocatesItsResult(int arity)
    {
        WithRealm(() =>
        {
            var expression = new JsRegExp("a[bc]+d", "");
            for (var index = 0; index < 1_000; index++)
                Call(expression, "exec", "miss", arity);

            object? result = null;
            var before = GC.GetAllocatedBytesForCurrentThread();
            for (var index = 0; index < 10_000; index++)
                result = expression.exec("miss");
            var directAllocated = GC.GetAllocatedBytesForCurrentThread() - before;

            before = GC.GetAllocatedBytesForCurrentThread();
            for (var index = 0; index < 10_000; index++)
                result = Call(expression, "exec", "miss", arity);
            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Equal(JsNull.Null, result);
            Assert.Equal(directAllocated, allocated);
        });
    }

    [Theory]
    [InlineData("test", 1)]
    [InlineData("test", 2)]
    [InlineData("exec", 1)]
    [InlineData("exec", 2)]
    public void OwnMethodsKeepReceiverAndAllArguments(string name, int arity)
    {
        WithRealm(() =>
        {
            var expression = new JsRegExp("a", "");
            var expected = new JsObject();
            var method = BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction2)((receiver, input, extra) =>
            {
                Assert.Same(expression, receiver);
                Assert.Equal("input", input);
                Assert.Equal(arity == 2 ? "ignored" : null, extra);
                return expected;
            }));
            ObjectRuntime.SetProperty(expression, name, method);

            Assert.Same(expected, Call(expression, name, "input", arity));
        });
    }

    [Theory]
    [InlineData("test", 1)]
    [InlineData("test", 2)]
    [InlineData("exec", 1)]
    [InlineData("exec", 2)]
    public void AccessorsAreReadOnceAndPreserveTheirReceiver(string name, int arity)
    {
        WithRealm(() =>
        {
            var expression = new JsRegExp("a", "");
            var gets = 0;
            var method = BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction1)((receiver, input) =>
            {
                Assert.Same(expression, receiver);
                return input;
            }));
            var getter = BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction0)(receiver =>
            {
                Assert.Same(expression, receiver);
                gets++;
                return method;
            }));
            JavaScriptRuntime.Object.defineProperty(expression, name, new JsObject
            {
                ["get"] = getter,
                ["configurable"] = true
            });

            Assert.Equal("input", Call(expression, name, "input", arity));
            Assert.Equal(1, gets);
        });
    }

    [Theory]
    [InlineData("test", 1)]
    [InlineData("test", 2)]
    [InlineData("exec", 1)]
    [InlineData("exec", 2)]
    public void PrototypeMutationInvalidatesTheDirectPath(string name, int arity)
    {
        WithRealm(() =>
        {
            var expression = new JsRegExp("a", "");
            _ = Call(expression, name, "a", arity);
            var method = BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction1)((receiver, input) =>
            {
                Assert.Same(expression, receiver);
                return "overridden";
            }));
            ObjectRuntime.SetProperty(JsRegExp.Prototype, name, method);

            Assert.Equal("overridden", Call(expression, name, "a", arity));
        });
    }

    [Theory]
    [InlineData("test", 1)]
    [InlineData("test", 2)]
    [InlineData("exec", 1)]
    [InlineData("exec", 2)]
    public void NonCallableOverridesStillThrow(string name, int arity)
    {
        WithRealm(() =>
        {
            var expression = new JsRegExp("a", "");
            ObjectRuntime.SetProperty(expression, name, 42d);
            Assert.Throws<TypeError>(() => Call(expression, name, "a", arity));
        });
    }

    [Theory]
    [InlineData("test", 1)]
    [InlineData("test", 2)]
    [InlineData("exec", 1)]
    [InlineData("exec", 2)]
    public void NonCallableAccessorIsReadOnlyOnce(string name, int arity)
    {
        WithRealm(() =>
        {
            var expression = new JsRegExp("a", "");
            var gets = 0;
            JavaScriptRuntime.Object.defineProperty(expression, name, new JsObject
            {
                ["get"] = BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction0)(_ =>
                {
                    gets++;
                    return null;
                }))
            });

            Assert.Throws<TypeError>(() => Call(expression, name, "a", arity));
            Assert.Equal(1, gets);
        });
    }

    [Theory]
    [InlineData("test", 1)]
    [InlineData("test", 2)]
    [InlineData("exec", 1)]
    [InlineData("exec", 2)]
    public void DeletedPrototypeMethodsDoNotFallBackToClrMethods(string name, int arity)
    {
        WithRealm(() =>
        {
            var expression = new JsRegExp("a", "");
            Assert.True(ObjectRuntime.DeleteProperty(JsRegExp.Prototype, name));
            Assert.Throws<TypeError>(() => Call(expression, name, "a", arity));
        });
    }

    [Theory]
    [InlineData("test", 1)]
    [InlineData("test", 2)]
    [InlineData("exec", 1)]
    [InlineData("exec", 2)]
    public void PrototypeAccessorsUseTheOriginalReceiver(string name, int arity)
    {
        WithRealm(() =>
        {
            var expression = new JsRegExp("a", "");
            var gets = 0;
            var method = BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction1)((receiver, input) =>
            {
                Assert.Same(expression, receiver);
                return "prototype accessor";
            }));
            JavaScriptRuntime.Object.defineProperty(JsRegExp.Prototype, name, new JsObject
            {
                ["get"] = BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction0)(receiver =>
                {
                    Assert.Same(expression, receiver);
                    gets++;
                    return method;
                })),
                ["configurable"] = true
            });

            Assert.Equal("prototype accessor", Call(expression, name, "a", arity));
            Assert.Equal(1, gets);
        });
    }

    [Theory]
    [InlineData("test", 1)]
    [InlineData("test", 2)]
    [InlineData("exec", 1)]
    [InlineData("exec", 2)]
    public void ChangedPrototypeUsesNormalLookup(string name, int arity)
    {
        WithRealm(() =>
        {
            var expression = new JsRegExp("a", "");
            var method = BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction1)((receiver, input) =>
            {
                Assert.Same(expression, receiver);
                return "custom prototype";
            }));
            JavaScriptRuntime.Object.setPrototypeOf(expression, new JsObject { [name] = method });

            Assert.Equal("custom prototype", Call(expression, name, "a", arity));
        });
    }

    [Theory]
    [InlineData("test", 1)]
    [InlineData("test", 2)]
    [InlineData("exec", 1)]
    [InlineData("exec", 2)]
    public void IgnoredExtraArgumentIsNotCoerced(string name, int arity)
    {
        WithRealm(() =>
        {
            var expression = new JsRegExp("a", "g");
            var extra = new JsObject
            {
                ["toString"] = BuiltinDelegateFunctionAdapter.FromDelegate(
                    (BuiltinFunction0)(_ => throw new InvalidOperationException("extra argument was coerced")))
            };
            var result = arity == 1
                ? ObjectRuntime.CallMember1(expression, name, "a")
                : ObjectRuntime.CallMember2(expression, name, "a", extra);

            if (name == "test")
                Assert.Equal(true, result);
            else
                Assert.Equal("a", Assert.IsType<JavaScriptRuntime.Array>(result)[0]);
            Assert.Equal(1d, TypeUtilities.ToNumber(expression.lastIndex));
        });
    }

    [Theory]
    [InlineData("test", 1)]
    [InlineData("test", 2)]
    [InlineData("exec", 1)]
    [InlineData("exec", 2)]
    public void InputCoercionCannotChangeTheAlreadySelectedMethod(string name, int arity)
    {
        WithRealm(() =>
        {
            var expression = new JsRegExp("a", "");
            var coercions = 0;
            var input = new JsObject
            {
                ["toString"] = BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction0)(_ =>
                {
                    coercions++;
                    ObjectRuntime.SetProperty(JsRegExp.Prototype, name,
                        BuiltinDelegateFunctionAdapter.FromDelegate((BuiltinFunction1)((_, _) => "replaced")));
                    return "a";
                }))
            };
            var result = Call(expression, name, input, arity);
            if (name == "test")
                Assert.Equal(true, result);
            else
                Assert.Equal("a", Assert.IsType<JavaScriptRuntime.Array>(result)[0]);
            Assert.Equal(1, coercions);
            Assert.Equal("replaced", Call(expression, name, "a", arity));
        });
    }

    [Theory]
    [InlineData("test", 1)]
    [InlineData("test", 2)]
    [InlineData("exec", 1)]
    [InlineData("exec", 2)]
    public void ForeignRealmReceiversUseNormalLookup(string name, int arity)
    {
        WithRealm(() =>
        {
            var expression = new JsRegExp("a", "");
            WithRealm(() =>
            {
                Assert.NotSame(JsRegExp.Prototype, PrototypeChain.GetPrototypeOrNull(expression));
                Assert.False(expression.TryInvokeIntrinsicMethod(name, "a", out _));
                var expected = CallableOperations.Call1(ObjectRuntime.GetProperty(expression, name), expression, "a");
                var actual = Call(expression, name, "a", arity);
                if (name == "test")
                {
                    Assert.Equal(expected, actual);
                }
                else
                {
                    var expectedMatch = Assert.IsType<JavaScriptRuntime.Array>(expected);
                    var actualMatch = Assert.IsType<JavaScriptRuntime.Array>(actual);
                    Assert.Equal(expectedMatch[0], actualMatch[0]);
                    Assert.Same(PrototypeChain.GetPrototypeOrNull(expectedMatch),
                        PrototypeChain.GetPrototypeOrNull(actualMatch));
                }
            });
        });
    }

    private static object? Call(JsRegExp expression, string name, object? input, int arity)
        => arity == 1
            ? ObjectRuntime.CallMember1(expression, name, input)
            : ObjectRuntime.CallMember2(expression, name, input, "ignored");

    private static void WithRealm(Action body)
    {
        var services = RuntimeServices.BuildServiceProvider();
        var context = RuntimeExecutionContext.GetOrCreate(services);
        try
        {
            using var scope = context.EnterAsRoot();
            context.GetOrCreateGlobalObject();
            body();
        }
        finally
        {
            services.OwningRealm!.Agent.Cluster.Dispose();
        }
    }
}
