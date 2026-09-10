using JavaScriptRuntime;
using JsObjectConstructor = JavaScriptRuntime.Object;
using JsIterator = JavaScriptRuntime.Iterator;

namespace Jroc.Tests;

public sealed class IteratorReviewRegressionTests
{
    [Fact]
    public void IteratorStepFailures_DoNotCloseTheSource()
    {
        WithRealm(() =>
        {
            var closeCalls = 0;
            var throwingNext = CreateIterator(
                _ => throw new StepFailure(),
                _ =>
                {
                    closeCalls++;
                    return IteratorResult.Create(null, true);
                });
            var every = ObjectRuntime.GetProperty(JsIterator.Prototype, "every");
            var predicate = BuiltinDelegateFunctionAdapter.FromDelegate(
                (BuiltinFunction2)((_, _, _) => true));

            Assert.Throws<StepFailure>(
                () => CallableOperations.Call1(every, throwingNext, predicate));
            Assert.Equal(0, closeCalls);

            var throwingDoneResult = new JsObject();
            PropertyDescriptorStore.DefineOrUpdate(
                throwingDoneResult,
                "done",
                new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Accessor,
                    Enumerable = true,
                    Configurable = true,
                    Get = (BuiltinFunction0)(_ => throw new DoneFailure())
                });
            var throwingDone = CreateIterator(
                _ => throwingDoneResult,
                _ =>
                {
                    closeCalls++;
                    return IteratorResult.Create(null, true);
                });
            var filter = ObjectRuntime.GetProperty(JsIterator.Prototype, "filter");
            var helper = Assert.IsAssignableFrom<IJavaScriptIterator>(
                CallableOperations.Call1(filter, throwingDone, predicate));

            Assert.Throws<DoneFailure>(() => helper.Next());
            Assert.Equal(0, closeCalls);

            var throwingValueResult = new JsObject();
            ObjectRuntime.SetProperty(throwingValueResult, "done", false);
            PropertyDescriptorStore.DefineOrUpdate(
                throwingValueResult,
                "value",
                new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Accessor,
                    Enumerable = true,
                    Configurable = true,
                    Get = (BuiltinFunction0)(_ => throw new ValueFailure())
                });
            var throwingValue = CreateIterator(
                _ => throwingValueResult,
                _ =>
                {
                    closeCalls++;
                    return IteratorResult.Create(null, true);
                });
            var drop = ObjectRuntime.GetProperty(JsIterator.Prototype, "drop");
            helper = Assert.IsAssignableFrom<IJavaScriptIterator>(
                CallableOperations.Call1(drop, throwingValue, 0d));

            Assert.Throws<ValueFailure>(() => helper.Next());
            Assert.Equal(0, closeCalls);
        });
    }

    [Fact]
    public void IteratorCallbackFailure_ClosesTheSource()
    {
        WithRealm(() =>
        {
            var closeCalls = 0;
            var source = CreateIterator(
                _ => IteratorResult.Create(1d, false),
                _ =>
                {
                    closeCalls++;
                    return IteratorResult.Create(null, true);
                });
            var filter = ObjectRuntime.GetProperty(JsIterator.Prototype, "filter");
            var predicate = BuiltinDelegateFunctionAdapter.FromDelegate(
                (BuiltinFunction2)((_, _, _) => throw new CallbackFailure()));
            var helper = Assert.IsAssignableFrom<IJavaScriptIterator>(
                CallableOperations.Call1(filter, source, predicate));

            Assert.Throws<CallbackFailure>(() => helper.Next());
            Assert.Equal(1, closeCalls);
        });
    }

    [Fact]
    public void FlatMapReturn_ClosesOuterWhenInnerCloseThrows_AndPreservesInnerError()
    {
        WithRealm(() =>
        {
            var outerCloseCalls = 0;
            var outer = CreateIterator(
                _ => IteratorResult.Create(1d, false),
                _ =>
                {
                    outerCloseCalls++;
                    throw new OuterCloseFailure();
                });
            var inner = CreateIterator(
                _ => IteratorResult.Create(2d, false),
                _ => throw new InnerCloseFailure());
            var mapper = BuiltinDelegateFunctionAdapter.FromDelegate(
                (BuiltinFunction2)((_, _, _) => inner));
            var flatMap = ObjectRuntime.GetProperty(JsIterator.Prototype, "flatMap");
            var iterable = new JsObject();
            ObjectRuntime.SetProperty(
                iterable,
                Symbol.iterator.DebugId,
                (BuiltinFunction0)(_ => outer));
            var wrappedOuter = JsIterator.From(iterable);
            var helper = Assert.IsAssignableFrom<IJavaScriptIterator>(
                CallableOperations.Call1(flatMap, wrappedOuter, mapper));

            Assert.Equal(2d, helper.Next().value);
            Assert.Throws<InnerCloseFailure>(() => helper.Return());
            Assert.Equal(1, outerCloseCalls);
        });

        AssertFlatMapInnerStepFailureClosesOuter();
    }

    private static void AssertFlatMapInnerStepFailureClosesOuter()
    {
        WithRealm(() =>
        {
            var outerCloseCalls = 0;
            var innerCloseCalls = 0;
            var outer = CreateIterator(
                _ => IteratorResult.Create(1d, false),
                _ =>
                {
                    outerCloseCalls++;
                    return IteratorResult.Create(null, true);
                });
            var inner = CreateIterator(
                _ => throw new InnerStepFailure(),
                _ =>
                {
                    innerCloseCalls++;
                    return IteratorResult.Create(null, true);
                });
            var mapper = BuiltinDelegateFunctionAdapter.FromDelegate(
                (BuiltinFunction2)((_, _, _) => inner));
            var flatMap = ObjectRuntime.GetProperty(JsIterator.Prototype, "flatMap");
            var iterable = new JsObject();
            ObjectRuntime.SetProperty(
                iterable,
                Symbol.iterator.DebugId,
                (BuiltinFunction0)(_ => outer));
            var wrappedOuter = JsIterator.From(iterable);
            var helper = Assert.IsAssignableFrom<IJavaScriptIterator>(
                CallableOperations.Call1(flatMap, wrappedOuter, mapper));

            Assert.Throws<InnerStepFailure>(() => helper.Next());
            Assert.Equal(1, outerCloseCalls);
            Assert.Equal(0, innerCloseCalls);
            helper.Return();
            Assert.Equal(1, outerCloseCalls);
        });
    }

    private static void AssertIteratorHelperPrototypeMethodsRequireIteratorHelperBrand()
    {
        WithRealm(() =>
        {
            var source = CreateIterator(
                _ => IteratorResult.Create(1d, false));
            var mapper = BuiltinDelegateFunctionAdapter.FromDelegate(
                (BuiltinFunction2)((_, value, _) => value));
            var map = ObjectRuntime.GetProperty(JsIterator.Prototype, "map");
            var helper = Assert.IsAssignableFrom<IJavaScriptIterator>(
                CallableOperations.Call1(map, source, mapper));
            var next = ObjectRuntime.GetProperty(JsIterator.HelperPrototype, "next");
            var @return = ObjectRuntime.GetProperty(JsIterator.HelperPrototype, "return");

            Assert.Equal(
                1d,
                ObjectRuntime.GetProperty(
                    CallableOperations.Call0(next, helper)!,
                    "value"));
            Assert.Throws<TypeError>(() => CallableOperations.Call0(next, source));
            Assert.Throws<TypeError>(() => CallableOperations.Call0(@return, source));
        });
    }

    [Fact]
    public void IteratorFromWrapper_IsBrandedLazyAndExactlyForwardsResults()
    {
        WithRealm(() =>
        {
            var nextResult = IteratorResult.Create("next", false);
            var returnResult = IteratorResult.Create("return", true);
            var source = CreateIterator(_ => nextResult, _ => returnResult);
            var wrapper = JsIterator.From(source);
            var next = ObjectRuntime.GetProperty(wrapper, "next");
            var @return = ObjectRuntime.GetProperty(wrapper, "return");

            Assert.Same(nextResult, CallableOperations.Call0(next, wrapper));
            Assert.Same(returnResult, CallableOperations.Call0(@return, wrapper));
            Assert.Throws<TypeError>(() => CallableOperations.Call0(next, source));
            Assert.Throws<TypeError>(() => CallableOperations.Call0(@return, source));

            var nonCallableNext = new JsObject();
            ObjectRuntime.SetProperty(nonCallableNext, "next", 0d);
            var lazyWrapper = JsIterator.From(nonCallableNext);
            var lazyNext = ObjectRuntime.GetProperty(lazyWrapper, "next");
            Assert.Throws<TypeError>(
                () => CallableOperations.Call0(lazyNext, lazyWrapper));

            var primitiveSource = CreateIterator(_ => 17d, _ => "closed");
            var primitiveWrapper = JsIterator.From(primitiveSource);
            var primitiveNext =
                ObjectRuntime.GetProperty(primitiveWrapper, "next");
            var primitiveReturn =
                ObjectRuntime.GetProperty(primitiveWrapper, "return");

            Assert.Equal(
                17d,
                CallableOperations.Call0(primitiveNext, primitiveWrapper));
            Assert.Equal(
                "closed",
                CallableOperations.Call0(
                    primitiveReturn,
                    primitiveWrapper));
            Assert.Throws<TypeError>(() => primitiveWrapper.Next());
            Assert.Throws<TypeError>(() => primitiveWrapper.Return());
        });

        AssertIteratorHelperPrototypeMethodsRequireIteratorHelperBrand();
        AssertConcreteIteratorPrototypesDefineBrandedOwnNextMethods();
    }

    [Fact]
    public void IteratorFrom_UsesIteratorMethodBeforeReadingNext_AndReturnsIteratorInstances()
    {
        WithRealm(() =>
        {
            var nextGets = 0;
            var source = new JsObject();
            PropertyDescriptorStore.DefineOrUpdate(
                source,
                "next",
                new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Accessor,
                    Enumerable = true,
                    Configurable = true,
                    Get = (BuiltinFunction0)(_ =>
                    {
                        nextGets++;
                        throw new NextGetterFailure();
                    })
                });
            var inner = CreateIterator(
                _ => IteratorResult.Create(7d, false));
            ObjectRuntime.SetProperty(
                source,
                Symbol.iterator.DebugId,
                (BuiltinFunction0)(_ => inner));

            var wrapper = JsIterator.From(source);
            Assert.Equal(0, nextGets);
            Assert.Equal(7d, wrapper.Next().value);

            var direct = CreateIterator(
                _ => IteratorResult.Create(null, true));
            PrototypeChain.SetPrototype(direct, JsIterator.Prototype);
            var from = ObjectRuntime.GetProperty(GlobalThis.Iterator, "from");
            Assert.Same(
                direct,
                CallableOperations.Call1(from, GlobalThis.Iterator, direct));
        });

        AssertPrimitiveStringCustomIteratorForwardsRawResults();
        AssertBorrowedIteratorFromUsesOwningRealm();
    }

    private static void AssertPrimitiveStringCustomIteratorForwardsRawResults()
    {
        WithRealm(() =>
        {
            var iterator = CreateIterator(_ => 23d, _ => "string-return");
            ObjectRuntime.SetProperty(
                JavaScriptRuntime.String.Prototype,
                Symbol.iterator.DebugId,
                (BuiltinFunction0)(_ => iterator));

            var wrapper = JsIterator.From("abc");
            var next = ObjectRuntime.GetProperty(wrapper, "next");
            var @return = ObjectRuntime.GetProperty(wrapper, "return");

            Assert.Equal(23d, CallableOperations.Call0(next, wrapper));
            Assert.Equal(
                "string-return",
                CallableOperations.Call0(@return, wrapper));
            Assert.Throws<TypeError>(() => wrapper.Next());
            Assert.Throws<TypeError>(() => wrapper.Return());
        });
    }

    private static void AssertBorrowedIteratorFromUsesOwningRealm()
    {
        object fromFirstRealm = null!;
        object iteratorPrototypeFromFirstRealm = null!;
        object wrapperPrototypeFromFirstRealm = null!;
        JsObject directIteratorFromFirstRealm = null!;
        object wrapperFromFirstRealm = null!;

        WithRealm(() =>
        {
            fromFirstRealm =
                ObjectRuntime.GetProperty(GlobalThis.Iterator, "from")!;
            iteratorPrototypeFromFirstRealm = JsIterator.Prototype;
            wrapperPrototypeFromFirstRealm = JsIterator.WrapperPrototype;
            directIteratorFromFirstRealm = CreateIterator(
                _ => IteratorResult.Create(null, true));
            PrototypeChain.SetPrototype(
                directIteratorFromFirstRealm,
                iteratorPrototypeFromFirstRealm);
        });

        WithRealm(() =>
        {
            var secondRealmWrapperPrototype = JsIterator.WrapperPrototype;
            Assert.NotSame(
                wrapperPrototypeFromFirstRealm,
                secondRealmWrapperPrototype);
            Assert.Same(
                directIteratorFromFirstRealm,
                CallableOperations.Call1(
                    fromFirstRealm,
                    null,
                    directIteratorFromFirstRealm));

            var source = CreateIterator(
                _ => IteratorResult.Create(null, true));
            wrapperFromFirstRealm = CallableOperations.Call1(
                fromFirstRealm,
                null,
                source)!;
            Assert.Same(
                wrapperPrototypeFromFirstRealm,
                JsObjectConstructor.getPrototypeOf(wrapperFromFirstRealm));
        });

        WithRealm(() =>
        {
            Assert.Same(
                wrapperPrototypeFromFirstRealm,
                JsObjectConstructor.getPrototypeOf(wrapperFromFirstRealm));
        });
    }

    private static void AssertConcreteIteratorPrototypesDefineBrandedOwnNextMethods()
    {
        WithRealm(() =>
        {
            var arrayIterator =
                new JavaScriptRuntime.Array(new object?[] { 1d }).values();
            var typedArrayIterator =
                new JavaScriptRuntime.Int32Array(1d).values();

            var map = new JavaScriptRuntime.Map();
            map.set("key", "value");
            var mapIterator = map.entries();

            var set = new JavaScriptRuntime.Set();
            set.add("value");
            var setIterator = set.values();

            var searchParams =
                new JavaScriptRuntime.Node.URLSearchParams("key=value");
            var searchParamsIterator = searchParams.entries();

            AssertBrandedOwnNext(arrayIterator, mapIterator);
            Assert.Same(
                JsObjectConstructor.getPrototypeOf(arrayIterator),
                JsObjectConstructor.getPrototypeOf(typedArrayIterator));
            AssertBrandedOwnNext(typedArrayIterator, mapIterator);
            AssertBrandedOwnNext(mapIterator, setIterator);
            AssertBrandedOwnNext(setIterator, mapIterator);
            AssertBrandedOwnNext(searchParamsIterator, mapIterator);
        });
    }

    [Fact]
    public void IteratorDispose_RejectsOnlyNullishAndLooksUpBoxedPrimitiveMethods()
    {
        WithRealm(() =>
        {
            object? receivedThis = null;
            ObjectRuntime.SetProperty(
                GlobalThis.NumberPrototypeValue,
                "return",
                (BuiltinFunction0)(thisArgument =>
                {
                    receivedThis = thisArgument;
                    return IteratorResult.Create(null, true);
                }));
            var dispose = ObjectRuntime.GetProperty(
                JsIterator.Prototype,
                Symbol.dispose.DebugId);

            Assert.Null(CallableOperations.Call0(dispose, 42d));
            Assert.Equal(42d, receivedThis);
            Assert.Null(CallableOperations.Call0(dispose, true));
            Assert.Throws<TypeError>(() => CallableOperations.Call0(dispose, null));
            Assert.Throws<TypeError>(() => CallableOperations.Call0(dispose, JsNull.Null));
        });
    }

    [Fact]
    public void IteratorConstructor_UsesNewTargetPrototypeAndActiveConstructorRealm()
    {
        WithRealm(() =>
        {
            var iteratorConstructor =
                BuiltinDelegateFunctionAdapter.NormalizeJavaScriptObject(
                    GlobalThis.Iterator);
            Assert.Throws<TypeError>(
                () => CallableOperations.Construct0(
                    iteratorConstructor,
                    iteratorConstructor));

            var customPrototype = new JsObject();
            var newTarget = CreateConstructor(customPrototype);
            var result = Assert.IsAssignableFrom<JsObject>(
                CallableOperations.Construct0(iteratorConstructor, newTarget));
            Assert.Same(
                customPrototype,
                JsObjectConstructor.getPrototypeOf(result));

            ObjectRuntime.SetProperty(newTarget, "prototype", 1d);
            result = Assert.IsAssignableFrom<JsObject>(
                CallableOperations.Construct0(iteratorConstructor, newTarget));
            Assert.Same(
                JsIterator.Prototype,
                JsObjectConstructor.getPrototypeOf(result));
        });

        object iteratorConstructorFromFirstRealm = null!;
        object iteratorPrototypeFromSecondRealm = null!;
        WithRealm(() =>
        {
            iteratorConstructorFromFirstRealm =
                BuiltinDelegateFunctionAdapter.NormalizeJavaScriptObject(
                    GlobalThis.Iterator);
        });

        WithRealm(() =>
        {
            var newTarget = CreateConstructor(1d);
            iteratorPrototypeFromSecondRealm = JsIterator.Prototype;
            var result = Assert.IsAssignableFrom<JsObject>(
                CallableOperations.Construct0(
                    iteratorConstructorFromFirstRealm,
                    newTarget));
            Assert.Same(
                iteratorPrototypeFromSecondRealm,
                JsObjectConstructor.getPrototypeOf(result));
        });
    }

    private static JsObject CreateIterator(
        BuiltinFunction0 next,
        BuiltinFunction0? @return = null)
    {
        var iterator = new JsObject();
        ObjectRuntime.SetProperty(iterator, "next", next);
        if (@return != null)
        {
            ObjectRuntime.SetProperty(iterator, "return", @return);
        }

        return iterator;
    }

    private static object CreateConstructor(object? prototype)
    {
        Func<object[], object?[], object?> constructor = static (_, _) => null;
        JavaScriptRuntime.Function.InitializeFunctionInstance(
            constructor,
            0d,
            "IteratorNewTarget",
            requiresInvocationContext: true);
        JavaScriptRuntime.Function.MarkConstructible(constructor);
        var constructorObject =
            BuiltinDelegateFunctionAdapter.NormalizeJavaScriptObject(
                constructor);
        ObjectRuntime.SetProperty(constructorObject, "prototype", prototype);
        return constructorObject;
    }

    private static void AssertBrandedOwnNext(
        IJavaScriptIterator iterator,
        IJavaScriptIterator incompatibleReceiver)
    {
        var prototype = JsObjectConstructor.getPrototypeOf(iterator);
        Assert.NotNull(prototype);
        Assert.True(ObjectRuntime.hasOwn(prototype!, "next"));

        var next = ObjectRuntime.GetProperty(prototype!, "next");
        Assert.Equal("next", ObjectRuntime.GetProperty(next!, "name"));
        Assert.Equal(0d, ObjectRuntime.GetProperty(next!, "length"));
        Assert.IsAssignableFrom<IIteratorResult>(
            CallableOperations.Call0(next, iterator));
        Assert.Throws<TypeError>(
            () => CallableOperations.Call0(next, incompatibleReceiver));
    }

    private static void WithRealm(Action body)
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).EnterAsRoot();
        _ = GlobalThis.globalThis;
        body();
    }

    private sealed class StepFailure : Exception;
    private sealed class DoneFailure : Exception;
    private sealed class ValueFailure : Exception;
    private sealed class CallbackFailure : Exception;
    private sealed class InnerCloseFailure : Exception;
    private sealed class InnerStepFailure : Exception;
    private sealed class OuterCloseFailure : Exception;
    private sealed class NextGetterFailure : Exception;
}
