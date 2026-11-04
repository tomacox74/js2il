using System.Collections.Generic;
using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class ObjectRuntimeOrdinaryObjectTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NumericPropertyAssignmentAndReads_DoNotAllocatePerIteration(bool genericKey)
    {
        var target = new JsObject();
        target.SetNumber("value", 0d);
        object key = "value";
        for (var i = 0; i < 1_024; i++)
        {
            ObjectRuntime.SetPropertyNumber(target, "value", i, throwOnError: true);
            _ = Read();
        }

        double sum = 0;
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 10_000; i++)
        {
            ObjectRuntime.SetPropertyNumber(target, "value", i, throwOnError: true);
            sum += Read();
        }
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(49_995_000d, sum);
        Assert.Equal(9_999d, ObjectRuntime.GetItem(target, "value"));
        Assert.InRange(allocated, 0L, 1_024L);

        double Read() => genericKey
            ? ObjectRuntime.GetItemAsNumber(target, key)
            : ObjectRuntime.GetItemAsNumber(target, "value");
    }

    [Fact]
    public void NumericPropertyAssignment_PreservesTaggedTransitionsAndNumberBits()
    {
        var target = new JsObject();
        target.SetNumber("value", 7d);
        object?[] values = ["12.5", true, JsNull.Null, null];
        double[] expected = [12.5d, 1d, 0d, double.NaN];
        for (var index = 0; index < values.Length; index++)
        {
            ObjectRuntime.SetItem(target, "value", values[index]);
            Assert.Equal(expected[index], ObjectRuntime.GetItemAsNumber(target, "value"));
            ObjectRuntime.SetPropertyNumber(target, "value", 9.25d, throwOnError: true);
            Assert.Equal(9.25d, ObjectRuntime.GetItem(target, "value"));
        }

        double[] numbers = [0d, -0d, double.NaN, double.PositiveInfinity, double.NegativeInfinity, -12.75d];
        foreach (var number in numbers)
        {
            ObjectRuntime.SetPropertyNumber(target, "value", number, throwOnError: true);
            Assert.Equal(BitConverter.DoubleToInt64Bits(number),
                BitConverter.DoubleToInt64Bits(ObjectRuntime.GetItemAsNumber(target, "value")));
            Assert.Equal(BitConverter.DoubleToInt64Bits(number),
                BitConverter.DoubleToInt64Bits(Assert.IsType<double>(ObjectRuntime.GetItem(target, "value"))));
        }
    }

    [Fact]
    public void NumericPropertyRead_PreservesNullishAndConversionErrors()
    {
        var target = new JsObject();
        Assert.True(double.IsNaN(ObjectRuntime.GetItemAsNumber(target, "missing")));
        target.SetObject("value", null);
        Assert.True(double.IsNaN(ObjectRuntime.GetItemAsNumber(target, "value")));
        target.SetObject("value", new Symbol("numeric-read"));
        Assert.Throws<TypeError>(() => ObjectRuntime.GetItemAsNumber(target, "value"));
        target.SetObject("value", new System.Numerics.BigInteger(7));
        Assert.Throws<TypeError>(() => ObjectRuntime.GetItemAsNumber(target, "value"));
        Assert.Throws<TypeError>(() => ObjectRuntime.GetItemAsNumber(null!, "value"));
        Assert.Throws<TypeError>(() => ObjectRuntime.GetItemAsNumber(JsNull.Null, "value"));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetPropertyNumber(null, "value", 1d, false));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetPropertyNumber(JsNull.Null, "value", 1d, false));
    }

    [Fact]
    public void NumericPropertyAssignment_RespectsDescriptorsAndNonextensibility()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).EnterAsRoot();
        var target = new JsObject();
        target.SetNumber("existing", 1d);
        JavaScriptRuntime.Object.preventExtensions(target);
        ObjectRuntime.SetPropertyNumber(target, "existing", 2d, true);
        Assert.Equal(2d, ObjectRuntime.GetItemAsNumber(target, "existing"));
        ObjectRuntime.SetPropertyNumber(target, "new", 3d, false);
        Assert.False(JavaScriptRuntime.Object.hasOwn(target, "new"));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetPropertyNumber(target, "new", 3d, true));

        JavaScriptRuntime.Object.freeze(target);
        ObjectRuntime.SetPropertyNumber(target, "existing", 4d, false);
        Assert.Equal(2d, ObjectRuntime.GetItemAsNumber(target, "existing"));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetPropertyNumber(target, "existing", 4d, true));

        var overlay = new JsObject();
        overlay.SetNumber("value", 5d);
        PropertyDescriptorStore.DefineOrUpdate(overlay, "value", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Value = 6d,
            Writable = true,
            Enumerable = false,
            Configurable = true
        });
        Assert.Equal(6d, ObjectRuntime.GetItemAsNumber(overlay, "value"));
        ObjectRuntime.SetPropertyNumber(overlay, "value", 8d, true);
        var descriptor = Assert.IsType<JsObject>(
            JavaScriptRuntime.Object.getOwnPropertyDescriptor(overlay, "value"));
        Assert.Equal(8d, ObjectRuntime.GetProperty(descriptor, "value"));
        Assert.Equal(false, ObjectRuntime.GetProperty(descriptor, "enumerable"));
    }

    [Fact]
    public void NumericPropertyAssignment_SharedIntrinsicDataKeepsRealmOverlay()
    {
        var target = new JsObject();
        using (PropertyDescriptorStore.BeginIntrinsicInitialization())
        {
            PropertyDescriptorStore.DefineOrUpdate(target, "value", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = 7d,
                Writable = true,
                Enumerable = false,
                Configurable = true
            });
        }

        var firstRealm = RuntimeExecutionContext.GetOrCreate(RuntimeServices.BuildServiceProvider());
        var secondRealm = RuntimeExecutionContext.GetOrCreate(RuntimeServices.BuildServiceProvider());
        using (firstRealm.EnterAsRoot())
        {
            ObjectRuntime.SetPropertyNumber(target, "value", 9d, true);
            Assert.Equal(9d, ObjectRuntime.GetItemAsNumber(target, "value"));
        }
        using (secondRealm.EnterAsRoot())
        {
            Assert.Equal(7d, ObjectRuntime.GetItemAsNumber(target, "value"));
            ObjectRuntime.SetPropertyNumber(target, "value", 11d, true);
            Assert.Equal(11d, ObjectRuntime.GetItemAsNumber(target, "value"));
        }
        using (firstRealm.EnterAsRoot())
        {
            Assert.Equal(9d, ObjectRuntime.GetItemAsNumber(target, "value"));
        }
    }

    [Fact]
    public void NumericPropertyAccess_InheritedAccessorsKeepReceiverAndCallCounts()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).EnterAsRoot();
        var target = new JsObject();
        var prototype = new JsObject();
        var reads = 0;
        var writes = 0;
        object? getterReceiver = null;
        object? setterReceiver = null;
        object? assigned = null;
        ObjectRuntime.DefineObjectLiteralAccessorProperty(prototype, "value",
            (BuiltinFunction0)(receiver =>
            {
                reads++;
                getterReceiver = receiver;
                return "17.5";
            }),
            (BuiltinFunction1)((receiver, value) =>
            {
                writes++;
                setterReceiver = receiver;
                assigned = value;
                return null;
            }));
        JavaScriptRuntime.Object.setPrototypeOf(target, prototype);

        Assert.Equal(17.5d, ObjectRuntime.GetItemAsNumber(target, "value"));
        ObjectRuntime.SetPropertyNumber(target, "value", 23d, true);
        Assert.Equal(1, reads);
        Assert.Equal(1, writes);
        Assert.Same(target, getterReceiver);
        Assert.Same(target, setterReceiver);
        Assert.Equal(23d, assigned);
        Assert.False(JavaScriptRuntime.Object.hasOwn(target, "value"));

        prototype.SetNumber("data", 4d);
        ObjectRuntime.SetPropertyNumber(target, "data", 9d, true);
        Assert.Equal(9d, ObjectRuntime.GetItemAsNumber(target, "data"));
        Assert.Equal(4d, ObjectRuntime.GetItemAsNumber(prototype, "data"));
        Assert.True(JavaScriptRuntime.Object.hasOwn(target, "data"));

        PropertyDescriptorStore.DefineOrUpdate(prototype, "readonly", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Value = 5d,
            Writable = false,
            Enumerable = true,
            Configurable = true
        });
        ObjectRuntime.SetPropertyNumber(target, "readonly", 10d, false);
        Assert.False(JavaScriptRuntime.Object.hasOwn(target, "readonly"));
        Assert.Equal(5d, ObjectRuntime.GetItemAsNumber(target, "readonly"));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetPropertyNumber(target, "readonly", 10d, true));

        target.SetNumber("ownAccessor", 99d);
        ObjectRuntime.DefineObjectLiteralAccessorProperty(target, "ownAccessor",
            (BuiltinFunction0)(_ => { reads++; return "31"; }),
            (BuiltinFunction1)((_, value) => { writes++; assigned = value; return null; }));
        Assert.Equal(31d, ObjectRuntime.GetItemAsNumber(target, "ownAccessor"));
        ObjectRuntime.SetPropertyNumber(target, "ownAccessor", 37d, true);
        Assert.Equal(2, reads);
        Assert.Equal(2, writes);
        Assert.Equal(37d, assigned);
    }

    [Fact]
    public void NumericPropertyAccess_ExoticAndIndexedReceiversUseFallback()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).EnterAsRoot();
        var exotic = new ExoticObjectForTest();
        exotic.SetNumber("value", 99d);
        exotic.Seed("value", "12");
        Assert.Equal(12d, ObjectRuntime.GetItemAsNumber(exotic, "value"));
        Assert.Equal(1, exotic.PropertyReadCount);
        ObjectRuntime.SetPropertyNumber(exotic, "value", 13d, true);
        Assert.Equal(13d, ObjectRuntime.GetItemAsNumber(exotic, "value"));
        Assert.Equal(2, exotic.PropertyReadCount);

        var array = new JavaScriptRuntime.Array(new object?[] { "6" });
        ObjectRuntime.SetPropertyNumber(array, "0", 8d, true);
        Assert.Equal(8d, ObjectRuntime.GetItemAsNumber(array, "0"));
        Assert.Equal(8d, ObjectRuntime.GetItemAsNumber(array, 0d));
        Assert.Equal(8d, ObjectRuntime.GetItemAsNumber(array, (object)0d));
        Assert.Equal(1d, ObjectRuntime.GetItemAsNumber(array, "length"));
        var typed = new Int32Array(1d);
        ObjectRuntime.SetPropertyNumber(typed, "0", 9.75d, true);
        Assert.Equal(9d, ObjectRuntime.GetItemAsNumber(typed, "0"));
        Assert.Equal(9d, ObjectRuntime.GetItemAsNumber(typed, 0d));
        Assert.Equal(9d, ObjectRuntime.GetItemAsNumber(typed, (object)0d));
    }

    [Fact]
    public void NumericPropertyAccess_StringCharactersAndFunctionRestrictionsPrecedeSlots()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).EnterAsRoot();
        var boxedString = new JsObject();
        boxedString.SetString(JavaScriptRuntime.String.StringDataPropertyName, "12");
        boxedString.SetNumber("0", 99d);
        Assert.Equal(1d, ObjectRuntime.GetItemAsNumber(boxedString, "0"));
        ObjectRuntime.SetPropertyNumber(boxedString, "0", 7d, false);
        Assert.Equal(1d, ObjectRuntime.GetItemAsNumber(boxedString, "0"));
        Assert.Throws<TypeError>(() => ObjectRuntime.SetPropertyNumber(boxedString, "0", 7d, true));
        boxedString.SetNumber("01", 42d);
        Assert.Equal(42d, ObjectRuntime.GetItemAsNumber(boxedString, "01"));

        var functionPrototype = new JsObject();
        functionPrototype.SetNumber("caller", 77d);
        functionPrototype.SetNumber("arguments", 88d);
        functionPrototype.MarkFunctionPrototype();
        Assert.Throws<TypeError>(() => ObjectRuntime.GetItemAsNumber(functionPrototype, "caller"));
        Assert.Throws<TypeError>(() => ObjectRuntime.GetItemAsNumber(functionPrototype, "arguments"));
    }

    private sealed class CoercibleReference;

    private sealed class ExoticObjectForTest : JsObject, IExoticJsObject
    {
        private readonly Dictionary<string, object?> _values = new(StringComparer.Ordinal);

        public bool RejectDefinitions { get; set; }

        public bool RejectDeletions { get; set; }

        public int PropertyReadCount { get; private set; }

        public void Seed(string key, object? value)
            => _values[key] = value;

        internal override bool TryGetBoxedValue(
            string key,
            object receiverForAccessors,
            out object? value)
        {
            PropertyReadCount++;
            return base.TryGetBoxedValue(key, receiverForAccessors, out value);
        }

        internal override bool TryGetOwnPropertyValue(string key, out object? value)
            => _values.TryGetValue(key, out value);

        internal override bool SetOwnPropertyValue(string key, object? value)
        {
            _values[key] = value;
            return true;
        }

        internal override bool DefineOwnProperty(string key, JsPropertyDescriptor descriptor)
            => !RejectDefinitions && base.DefineOwnProperty(key, descriptor);

        internal override bool DeleteOwnProperty(string key)
        {
            if (RejectDeletions)
            {
                return false;
            }

            _values.Remove(key);
            PropertyDescriptorStore.Delete(this, key);
            return true;
        }

        internal override IEnumerable<string> GetOwnPropertyKeys()
        {
            foreach (var key in _values.Keys)
            {
                yield return key;
            }

            foreach (var key in PropertyDescriptorStore.GetOwnKeys(this))
            {
                if (!_values.ContainsKey(key))
                {
                    yield return key;
                }
            }
        }
    }

    [Fact]
    public void RequireObjectCoercible_PreservesTypedReference()
    {
        var value = new CoercibleReference();

        CoercibleReference result = ObjectRuntime.RequireObjectCoercible(value);

        Assert.Same(value, result);
    }

    [Fact]
    public void RequireObjectCoercible_RejectsNullAndJsNull()
    {
        Assert.Throws<TypeError>(() => ObjectRuntime.RequireObjectCoercible<object?>(null));
        Assert.Throws<TypeError>(() => ObjectRuntime.RequireObjectCoercible<object>(JsNull.Null));
    }

    [Fact]
    public void StaticClassDispatch_PacksParamsArrayArguments()
    {
        var math = typeof(JavaScriptRuntime.Math);

        Assert.Equal(3d, ObjectRuntime.CallMember2(math, "max", 1d, 3d));
        Assert.Equal(6d, ObjectRuntime.CallMember(math, "max", [2d, 6d, 4d]));
        Assert.Equal(double.NegativeInfinity, ObjectRuntime.CallMember0(math, "max"));
    }

    [Fact]
    public void OwnPropertyDescriptor_DoesNotExposeRuntimeClrFields()
    {
        var stream = new JavaScriptRuntime.Node.Writable();

        Assert.Null(JavaScriptRuntime.Object.getOwnPropertyDescriptor(stream, "_write"));
        Assert.Null(JavaScriptRuntime.Object.getOwnPropertyDescriptor(stream, "_WRITE"));
        Assert.Null(JavaScriptRuntime.Object.getOwnPropertyDescriptor(
            typeof(RuntimeServices),
            nameof(RuntimeServices.EmptyScopes)));
    }

    [Fact]
    public void OwnPropertyDescriptor_OnlyMatchesDeclaredClassFieldName()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "class-field-descriptor-name",
            "Classes",
            _ => ("""
                const assert = require("assert");
                class Example { foo = 42; }
                const instance = new Example();
                assert.strictEqual(Object.getOwnPropertyDescriptor(instance, "foo").value, 42);
                assert.strictEqual(Object.getOwnPropertyDescriptor(instance, "FOO"), undefined);
                console.log("ok");
                """, null));

        Assert.Equal($"ok{Environment.NewLine}", result.Output);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetProperty_NamedReadsDoNotBoxUnrelatedStringData(bool hasCustomDescriptor)
    {
        var target = new JsObject();
        var expected = new object();
        target.SetNumber(JavaScriptRuntime.String.StringDataPropertyName, 42d);
        target.SetObject("value", expected);
        if (hasCustomDescriptor)
        {
            PropertyDescriptorStore.DefineOrUpdate(target, "value", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = expected,
                Writable = false,
                Enumerable = true,
                Configurable = true
            });
        }

        for (var i = 0; i < 256; i++)
        {
            _ = ObjectRuntime.GetProperty(target, "value");
        }

        object? actual = null;
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 1_024; i++)
        {
            actual = ObjectRuntime.GetProperty(target, "value");
        }
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Same(expected, actual);
        Assert.InRange(allocated, 0L, 1_024L);
    }

    [Fact]
    public void GetProperty_OwnReadTracksDescriptorAndPrototypeChanges()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).EnterAsRoot();
        var target = new JsObject();
        var prototype = new JsObject();
        prototype.SetString("value", "inherited");
        JavaScriptRuntime.Object.setPrototypeOf(target, prototype);
        target.SetString("value", "own");

        Assert.Equal("own", ObjectRuntime.GetProperty(target, "value"));
        target.SetObject("value", null);
        Assert.Null(ObjectRuntime.GetProperty(target, "value"));

        var reads = 0;
        BuiltinFunction0 getter = receiver =>
        {
            reads++;
            return receiver;
        };
        ObjectRuntime.DefineObjectLiteralAccessorProperty(target, "value", getter, null);

        Assert.Same(target, ObjectRuntime.GetProperty(target, "value"));
        Assert.Equal(1, reads);
        Assert.True(ObjectRuntime.DeleteProperty(target, "value"));
        Assert.Equal("inherited", ObjectRuntime.GetProperty(target, "value"));
        target.SetString("value", "restored");
        Assert.Equal("restored", ObjectRuntime.GetProperty(target, "value"));
    }

    [Theory]
    [InlineData("0", "a")]
    [InlineData("1", "b")]
    [InlineData("2", "own")]
    [InlineData("01", "own")]
    [InlineData("-0", "own")]
    [InlineData("", "own")]
    public void GetProperty_StringIndicesTakePrecedenceOverOrdinarySlots(string key, string expected)
    {
        var target = new JsObject();
        target.SetString(JavaScriptRuntime.String.StringDataPropertyName, "ab");
        target.SetString(key, "own");

        Assert.Equal(expected, ObjectRuntime.GetProperty(target, key));
    }

    [Theory]
    [InlineData("caller")]
    [InlineData("arguments")]
    public void GetProperty_FunctionPrototypeRestrictionsPrecedeOrdinarySlots(string key)
    {
        var target = new JsObject();
        target.SetString(key, "ordinary");
        Assert.Equal("ordinary", ObjectRuntime.GetProperty(target, key));

        target.MarkFunctionPrototype();

        Assert.Throws<TypeError>(() => ObjectRuntime.GetProperty(target, key));
    }

    [Fact]
    public void CoreDispatch_PreservesJsObjectBehavior()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            object target = new JsObject();
            var prototype = new JsObject();

            ObjectRuntime.SetProperty(target, "second", 2d);
            ObjectRuntime.SetProperty(target, "1", "index");
            ObjectRuntime.SetProperty(target, "first", 1d);
            ObjectRuntime.SetProperty(prototype, "inherited", 3d);
            JavaScriptRuntime.Object.setPrototypeOf(target, prototype);

            Assert.Equal(2d, ObjectRuntime.GetProperty(target, "second"));
            Assert.Equal(3d, ObjectRuntime.GetProperty(target, "inherited"));
            Assert.True(JavaScriptRuntime.Object.hasOwn(target, "first"));
            Assert.True(Operators.In("inherited", target));

            Assert.Equal(
                new object?[] { "1", "second", "first" },
                Assert.IsType<JavaScriptRuntime.Array>(JavaScriptRuntime.Object.getOwnPropertyNames(target)).ToArray());

            PropertyDescriptorStore.DefineOrUpdate(target, "descriptorOnly", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = 4d,
                Writable = true,
                Enumerable = true,
                Configurable = true
            });
            Assert.Equal(4d, ObjectRuntime.GetProperty(target, "descriptorOnly"));

            Assert.True(ObjectRuntime.DeleteProperty(target, "second"));
            Assert.False(JavaScriptRuntime.Object.hasOwn(target, "second"));

            JavaScriptRuntime.Object.freeze(target);
            Assert.True(JavaScriptRuntime.Object.isFrozen(target));
            Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(target, "first", 4d));

            var rejectingTarget = new ExoticObjectForTest { RejectDefinitions = true };
            Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(rejectingTarget, "rejected", 1d));
            Assert.Equal(
                1d,
                ObjectRuntime.SetProperty(rejectingTarget, "rejected", 1d, throwOnError: false));
            Assert.False(JavaScriptRuntime.Object.hasOwn(rejectingTarget, "rejected"));

            var deleteRejectingTarget = new ExoticObjectForTest();
            ObjectRuntime.SetProperty(deleteRejectingTarget, "retained", 1d);
            deleteRejectingTarget.RejectDeletions = true;
            Assert.Throws<TypeError>(() =>
                ObjectRuntime.DeleteProperty(deleteRejectingTarget, "retained"));
            Assert.False(ObjectRuntime.DeletePropertyNonStrict(deleteRejectingTarget, "retained"));
            Assert.True(JavaScriptRuntime.Object.hasOwn(deleteRejectingTarget, "retained"));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void PropertyReadContract_PreservesInheritedAccessorReceiver()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            var target = new JsObject();
            var prototype = new JsObject();

            ObjectRuntime.SetProperty(target, "marker", "target");
            Func<object[], object?[]?, object?> getter = static (_, _) =>
                ObjectRuntime.GetProperty(RuntimeServices.GetCurrentThis()!, "marker");
            ObjectRuntime.DefineObjectLiteralAccessorProperty(
                prototype,
                "computed",
                getter,
                null);
            JavaScriptRuntime.Object.setPrototypeOf(target, prototype);

            Assert.Equal("target", ObjectRuntime.GetProperty(target, "computed"));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void ProxyFallback_PreservesJsObjectTargetSemantics()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            object target = new JsObject();
            var proxy = new JavaScriptRuntime.Proxy(target, new JsObject());

            ObjectRuntime.SetProperty(proxy, "value", 42d);

            Assert.Equal(42d, ObjectRuntime.GetProperty(proxy, "value"));
            Assert.True(Operators.In("value", proxy));
            Assert.True(ObjectRuntime.DeleteProperty(proxy, "value"));
            Assert.False(Operators.In("value", proxy));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void IntegrityChecksSkipKeysDeletedDuringDescriptorLookup()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope =
            RuntimeExecutionContext.GetOrCreate(services).EnterAsRoot();
        RuntimeExecutionContext.Current!.GetOrCreateGlobalObject();

        Assert.True(JavaScriptRuntime.Object.isSealed(
            CreateDeletingDescriptorProxy()));
        Assert.True(JavaScriptRuntime.Object.isFrozen(
            CreateDeletingDescriptorProxy()));

        static JavaScriptRuntime.Proxy CreateDeletingDescriptorProxy()
        {
            var target = new JsObject();
            ObjectRuntime.SetProperty(target, "removed", 1d);
            JavaScriptRuntime.Object.preventExtensions(target);

            var handler = new JsObject();
            ObjectRuntime.SetProperty(
                handler,
                "getOwnPropertyDescriptor",
                (BuiltinFunction2)((_, _, key) =>
                {
                    ObjectRuntime.DeleteProperty(target, key);
                    return null;
                }));
            return new JavaScriptRuntime.Proxy(target, handler);
        }
    }

    [Fact]
    public void DeletedIntrinsicProperty_IsMaskedAcrossCoreDispatch()
    {
        object target = new JsObject();
        ObjectRuntime.TrySetOwnValue(target, "hidden", 42d);
        using (PropertyDescriptorStore.BeginIntrinsicInitialization())
        {
            PropertyDescriptorStore.DefineOrUpdate(target, "hidden", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = 42d,
                Writable = true,
                Enumerable = true,
                Configurable = true
            });
        }

        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;

            Assert.True(ObjectRuntime.DeleteProperty(target, "hidden"));
            Assert.Null(ObjectRuntime.GetProperty(target, "hidden"));
            Assert.Null(JavaScriptRuntime.Object.getOwnPropertyDescriptor(target, "hidden"));
            Assert.False(JavaScriptRuntime.Object.hasOwn(target, "hidden"));
            Assert.False(Operators.In("hidden", target));
            Assert.Empty(Assert.IsType<JavaScriptRuntime.Array>(
                JavaScriptRuntime.Object.getOwnPropertyNames(target)));
            Assert.True(JavaScriptRuntime.ObjectRuntime.CreateForInIterator(target).Next().done);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void RuntimeCreatedOrdinaryRecords_AreJsObjects()
    {
        var services = RuntimeServices.BuildServiceProvider();
        using var scope = RuntimeExecutionContext.GetOrCreate(services).Enter();
        var moduleId = $"module-{Guid.NewGuid():N}.js";
        var importMeta = Assert.IsType<JsObject>(RuntimeServices.GetImportMeta(moduleId));
        var revocable = Assert.IsType<JsObject>(
            JavaScriptRuntime.Proxy.revocable(new JsObject(), new JsObject()));

        Assert.Same(importMeta, RuntimeServices.GetImportMeta(moduleId));
        Assert.IsType<string>(ObjectRuntime.GetProperty(importMeta, "url"));
        Assert.IsType<JavaScriptRuntime.Proxy>(ObjectRuntime.GetProperty(revocable, "proxy"));
        Assert.IsType<BuiltinDelegateFunctionAdapter>(
            ObjectRuntime.GetProperty(revocable, "revoke"));
    }

    [Fact]
    public void ExternalDictionary_RemainsAHostObject()
    {
        var hostObject = new Dictionary<string, object?>();

        ObjectRuntime.SetProperty(hostObject, "value", 42d);

        Assert.Equal(42d, ObjectRuntime.GetProperty(hostObject, "value"));
        Assert.True(JavaScriptRuntime.Object.hasOwn(hostObject, "value"));
        Assert.Equal(42d, hostObject["value"]);
    }

    [Fact]
    public void NumericGetItem_PreservesNonCanonicalArrayProperties()
    {
        var target = new JavaScriptRuntime.Array(new object?[] { "zero" });
        ObjectRuntime.SetProperty(target, "1.5", "fractional");
        ObjectRuntime.SetProperty(target, "NaN", "not-a-number");
        ObjectRuntime.SetProperty(target, "-1", "negative");

        Assert.Equal("fractional", ObjectRuntime.GetItem(target, 1.5d));
        Assert.Equal("not-a-number", ObjectRuntime.GetItem(target, double.NaN));
        Assert.Equal("negative", ObjectRuntime.GetItem(target, -1d));
    }

    [Fact]
    public void IteratorProtocolInvocation_PreservesWithBindingContext()
    {
        var withObject = new JsObject();
        ObjectRuntime.SetProperty(withObject, "marker", "bound");

        var iterator = new JsObject();
        ObjectRuntime.SetProperty(
            iterator,
            "next",
            (Func<object[], object?[]?, object?>)((_, _) => IteratorResult.Create(null, done: true)));

        object? observed = null;
        Func<object[], object?[]?, object?> iteratorMethod = (_, _) =>
        {
            observed = RuntimeServices.ResolveWithBindingOrDefault("marker", "fallback");
            return iterator;
        };
        JavaScriptRuntime.Function.BindWithObject(iteratorMethod, withObject);

        var iterable = new JsObject();
        ObjectRuntime.DefineObjectLiteralDataProperty(
            iterable,
            Symbol.iterator,
            iteratorMethod);

        _ = ObjectRuntime.GetIterator(iterable);
        Assert.Equal("bound", observed);
    }

    [Fact]
    public void ExoticSubclass_UsesInternalOperationsAcrossGenericObjectSemantics()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            var target = new ExoticObjectForTest();
            var prototype = new JsObject();

            target.Seed("seed", 0d);
            ObjectRuntime.SetProperty(target, "second", 2d);
            ObjectRuntime.SetProperty(target, "1", "index");
            ObjectRuntime.SetProperty(target, "first", 1d);
            ObjectRuntime.SetProperty(prototype, "inherited", 3d);
            JavaScriptRuntime.Object.setPrototypeOf(target, prototype);

            Assert.Equal(2d, ObjectRuntime.GetProperty(target, "second"));
            Assert.Equal(3d, ObjectRuntime.GetProperty(target, "inherited"));
            Assert.True(target.PropertyReadCount >= 2);
            Assert.True(JavaScriptRuntime.Object.hasOwn(target, "first"));
            Assert.True(Operators.In("inherited", target));
            Assert.Equal(
                new object?[] { "1", "seed", "second", "first" },
                Assert.IsType<JavaScriptRuntime.Array>(
                    JavaScriptRuntime.Object.getOwnPropertyNames(target)).ToArray());

            var descriptor = Assert.IsType<JsObject>(
                JavaScriptRuntime.Object.getOwnPropertyDescriptor(target, "first"));
            Assert.Equal(1d, ObjectRuntime.GetProperty(descriptor, "value"));

            Func<object[], object?[]?, object?> getter = static (_, _) => "accessor";
            ObjectRuntime.DefineObjectLiteralAccessorProperty(
                target,
                "computed",
                getter,
                null);
            Assert.Equal("accessor", ObjectRuntime.GetProperty(target, "computed"));

            ObjectRuntime.DefineObjectLiteralDataProperty(target, "helper", 5d);
            JavaScriptRuntime.ObjectRuntime.SetPropertyNumber(target, "typed", 6d);
            Assert.Equal(5d, ObjectRuntime.GetProperty(target, "helper"));
            Assert.Equal(6d, ObjectRuntime.GetProperty(target, "typed"));

            PropertyDescriptorStore.DefineOrUpdate(target, "descriptorOnly", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = 7d,
                Writable = true,
                Enumerable = true,
                Configurable = true
            });
            Assert.Contains(
                "descriptorOnly",
                Assert.IsType<JavaScriptRuntime.Array>(
                    JavaScriptRuntime.Object.getOwnPropertyNames(target)).ToArray());

            var proxy = new JavaScriptRuntime.Proxy(target, new JsObject());
            Assert.Equal(1d, ObjectRuntime.GetProperty(proxy, "first"));

            Assert.True(ObjectRuntime.DeleteProperty(target, "second"));
            Assert.False(JavaScriptRuntime.Object.hasOwn(target, "second"));

            JavaScriptRuntime.Object.freeze(target);
            Assert.True(JavaScriptRuntime.Object.isFrozen(target));
            Assert.Throws<TypeError>(() => ObjectRuntime.SetProperty(target, "first", 4d));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Theory]
    [InlineData("0", true, 0u)]
    [InlineData("2147483648", true, 2147483648u)]
    [InlineData("4294967294", true, 4294967294u)]
    [InlineData("4294967295", false, 0u)]
    [InlineData("01", false, 0u)]
    [InlineData("-0", false, 0u)]
    [InlineData("1.0", false, 0u)]
    public void CanonicalArrayIndex_UsesEcma262PropertyKeyRange(
        string key,
        bool expected,
        uint expectedIndex)
    {
        Assert.Equal(expected, CanonicalArrayIndex.TryParse(key, out var index));
        Assert.Equal(expectedIndex, index);
    }
}
