using JavaScriptRuntime;
using JsString = JavaScriptRuntime.String;

namespace Jroc.Tests;

public sealed class IntrinsicPrototypeEpochTests
{
    [Fact]
    public void StringEpochRemainsStableDuringInitializationAndUnrelatedMutation()
    {
        WithRealm(
            () =>
            {
                var initial =
                    IntrinsicPrototypeEpochs.Read(
                        IntrinsicPrototypeFamily.String);
                Assert.Equal(
                    IntrinsicPrototypeEpochs.PristineEpoch,
                    initial);
                Assert.True(
                    IntrinsicPrototypeEpochs.IsPristine(
                        IntrinsicPrototypeFamily.String));

                _ = GlobalThis.globalThis;
                _ = ObjectRuntime.GetProperty(
                    JsString.Prototype,
                    "charAt");
                ObjectRuntime.SetProperty(
                    new JsObject(),
                    "unrelated",
                    true);
                ObjectRuntime.SetProperty(
                    JavaScriptRuntime.Array.Prototype,
                    "unrelatedFamilyMutation",
                    true);

                Assert.Equal(
                    initial,
                    IntrinsicPrototypeEpochs.Read(
                        IntrinsicPrototypeFamily.String));
                Assert.True(
                    IntrinsicPrototypeEpochs.IsCurrent(
                        IntrinsicPrototypeFamily.String,
                        initial));
            });
    }

    [Fact]
    public void StringEpochInvalidatesForEveryRelevantMutationShape()
    {
        WithRealm(
            () =>
            {
                _ = GlobalThis.globalThis;
                var stringPrototype = JsString.Prototype;
                var objectPrototype =
                    GlobalThis.ObjectPrototypeValue;

                AssertAdvances(
                    () => ObjectRuntime.SetProperty(
                        stringPrototype,
                        "charAt",
                        new EpochFunction()));
                AssertAdvances(
                    () => JavaScriptRuntime.Object.defineProperty(
                        stringPrototype,
                        "epochData",
                        DataDescriptor(
                            "defined",
                            enumerable: true)));
                AssertAdvances(
                    () => JavaScriptRuntime.Object.defineProperty(
                        stringPrototype,
                        "epochData",
                        DataDescriptor(
                            "reconfigured",
                            enumerable: false)));
                AssertAdvances(
                    () => JavaScriptRuntime.Object.defineProperty(
                        stringPrototype,
                        "epochAccessor",
                        AccessorDescriptor(new EpochFunction())));
                AssertAdvances(
                    () => Assert.True(
                        ObjectRuntime.DeleteProperty(
                            stringPrototype,
                            "epochAccessor")));
                AssertAdvances(
                    () => ObjectRuntime.SetProperty(
                        objectPrototype,
                        "stringAncestorMutation",
                        true));
                AssertAdvances(
                    () => JavaScriptRuntime.Object.setPrototypeOf(
                        objectPrototype,
                        new JsObject()));
                AssertAdvances(
                    () => JavaScriptRuntime.Object.setPrototypeOf(
                        stringPrototype,
                        new JsObject()));
            });
    }

    [Fact]
    public void StringEpochIsIsolatedPerRealm()
    {
        var firstServices = RuntimeServices.BuildServiceProvider();
        var secondServices = RuntimeServices.BuildServiceProvider();
        var firstContext =
            RuntimeExecutionContext.GetOrCreate(firstServices);
        var secondContext =
            RuntimeExecutionContext.GetOrCreate(secondServices);
        long firstMutatedEpoch;

        try
        {
            using (firstContext.EnterAsRoot())
            {
                _ = GlobalThis.globalThis;
                var initial = IntrinsicPrototypeEpochs.Read(
                    IntrinsicPrototypeFamily.String);
                ObjectRuntime.SetProperty(
                    JsString.Prototype,
                    "realmMutation",
                    "first");
                firstMutatedEpoch = IntrinsicPrototypeEpochs.Read(
                    IntrinsicPrototypeFamily.String);
                Assert.True(firstMutatedEpoch > initial);
            }

            using (secondContext.EnterAsRoot())
            {
                _ = GlobalThis.globalThis;
                Assert.Equal(
                    0,
                    IntrinsicPrototypeEpochs.Read(
                        IntrinsicPrototypeFamily.String));
                Assert.Null(
                    ObjectRuntime.GetProperty(
                        JsString.Prototype,
                        "realmMutation"));
            }

            using (firstContext.EnterAsRoot())
            {
                Assert.Equal(
                    firstMutatedEpoch,
                    IntrinsicPrototypeEpochs.Read(
                        IntrinsicPrototypeFamily.String));
                Assert.Equal(
                    "first",
                    ObjectRuntime.GetProperty(
                        JsString.Prototype,
                        "realmMutation"));
            }
        }
        finally
        {
            firstServices.OwningRealm!.Agent.Cluster.Dispose();
            secondServices.OwningRealm!.Agent.Cluster.Dispose();
        }
    }

    [Fact]
    public void EpochReadAndValidationAllocateNothing()
    {
        WithRealm(
            () =>
            {
                _ = GlobalThis.globalThis;
                var expected = IntrinsicPrototypeEpochs.Read(
                    IntrinsicPrototypeFamily.String);

                for (var index = 0; index < 1_000; index++)
                {
                    _ = IntrinsicPrototypeEpochs.IsCurrent(
                        IntrinsicPrototypeFamily.String,
                        expected);
                    _ = IntrinsicPrototypeEpochs.IsPristine(
                        IntrinsicPrototypeFamily.String);
                }

                var before =
                    GC.GetAllocatedBytesForCurrentThread();
                var valid = true;
                long observed = 0;
                for (var index = 0; index < 100_000; index++)
                {
                    observed ^= IntrinsicPrototypeEpochs.Read(
                        IntrinsicPrototypeFamily.String);
                    valid &= IntrinsicPrototypeEpochs.IsCurrent(
                        IntrinsicPrototypeFamily.String,
                        expected);
                    valid &= IntrinsicPrototypeEpochs.IsPristine(
                        IntrinsicPrototypeFamily.String);
                }
                var allocated =
                    GC.GetAllocatedBytesForCurrentThread() - before;

                GC.KeepAlive(observed);
                Assert.True(valid);
                Assert.Equal(0, allocated);
            });
    }

    private static void AssertAdvances(Action mutation)
    {
        var before = IntrinsicPrototypeEpochs.Read(
            IntrinsicPrototypeFamily.String);

        mutation();

        var after = IntrinsicPrototypeEpochs.Read(
            IntrinsicPrototypeFamily.String);
        Assert.True(after > before);
        Assert.False(
            IntrinsicPrototypeEpochs.IsCurrent(
                IntrinsicPrototypeFamily.String,
                before));
        Assert.False(
            IntrinsicPrototypeEpochs.IsPristine(
                IntrinsicPrototypeFamily.String));
    }

    private static JsObject DataDescriptor(
        object? value,
        bool enumerable)
        => new()
        {
            ["value"] = value,
            ["writable"] = true,
            ["enumerable"] = enumerable,
            ["configurable"] = true
        };

    private static JsObject AccessorDescriptor(
        JsFunctionObject getter)
        => new()
        {
            ["get"] = getter,
            ["enumerable"] = false,
            ["configurable"] = true
        };

    private static void WithRealm(Action body)
    {
        var services = RuntimeServices.BuildServiceProvider();
        var context = RuntimeExecutionContext.GetOrCreate(services);
        try
        {
            using var scope = context.EnterAsRoot();
            body();
        }
        finally
        {
            services.OwningRealm!.Agent.Cluster.Dispose();
        }
    }

    private sealed class EpochFunction : JsFunctionObject
    {
        protected override object? CallCore(
            object? thisArgument,
            in JsCallArguments arguments)
            => null;
    }
}
