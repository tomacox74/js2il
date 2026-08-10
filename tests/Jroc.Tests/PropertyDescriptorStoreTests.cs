using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests;

public class PropertyDescriptorStoreTests
{
    [Fact]
    public void Descriptor_IsValueType()
        => Assert.True(typeof(JsPropertyDescriptor).IsValueType);

    [Fact]
    public void Descriptor_UsesCompactFieldLayout()
        => Assert.Equal(
            IntPtr.Size == 8 ? 32 : 20,
            System.Runtime.CompilerServices.Unsafe.SizeOf<JsPropertyDescriptor>());

    [Fact]
    public void OrdinaryDefaultDataProperties_UseOnlyShapeAndValueStorage()
    {
        var target = new JsObject();

        target.SetNumber("number", 42d);
        target.SetBoolean("boolean", true);
        target.SetString("string", "value");
        target.SetObject("object", new object());
        ObjectRuntime.DefineObjectLiteralDataProperty(target, "literal", 7d);
        ObjectRuntime.SetProperty(target, "assignment", 8d);

        Assert.False(target.HasInlineDescriptorState);
        Assert.False(target.HasNonDataDescriptors);
        Assert.False(PropertyDescriptorStore.HasExternalDescriptorStateForTests(target));

        Assert.True(PropertyDescriptorStore.TryGetOwn(target, "number", out var descriptor));
        Assert.Equal(42d, descriptor.Value);
        Assert.True(descriptor.Writable);
        Assert.True(descriptor.Enumerable);
        Assert.True(descriptor.Configurable);
    }

    [Fact]
    public void OrdinaryCustomDataAndAccessorDescriptors_UseLazyInlineState()
    {
        var target = new JsObject();
        Func<object[], object?[]?, object?> getter = static (_, _) => "accessor";
        Action<object?> setter = static _ => { };

        target.DefineOwnProperty(
            "restricted",
            DataDescriptor(1d, enumerable: false, writable: false, configurable: true));
        target.DefineOwnProperty("accessor", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Accessor,
            Get = getter,
            Set = setter,
            Enumerable = true,
            Configurable = true
        });

        Assert.True(target.HasInlineDescriptorState);
        Assert.True(target.HasNonDataDescriptors);
        Assert.False(PropertyDescriptorStore.HasExternalDescriptorStateForTests(target));

        Assert.True(PropertyDescriptorStore.TryGetOwn(target, "restricted", out var data));
        Assert.Equal(1d, data.Value);
        Assert.False(data.Writable);
        Assert.False(data.Enumerable);

        Assert.True(PropertyDescriptorStore.TryGetOwn(target, "accessor", out var accessor));
        Assert.Same(
            BuiltinDelegateFunctionAdapter.FromDelegate(getter),
            accessor.Get);
        Assert.Same(
            BuiltinDelegateFunctionAdapter.FromDelegate(setter),
            accessor.Set);
        Assert.Equal("accessor", ObjectRuntime.GetProperty(target, "accessor"));
    }

    [Fact]
    public void OrdinaryDescriptorTransitions_KeepOneCanonicalValueSlot()
    {
        var target = new JsObject();
        target.DefineOwnProperty(
            "value",
            DataDescriptor("data", writable: false, configurable: true));

        Func<object[], object?[]?, object?> getter = static (_, _) => "getter";
        target.DefineOwnProperty("value", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Accessor,
            Get = getter,
            Enumerable = true,
            Configurable = true
        });
        Assert.Equal("getter", ObjectRuntime.GetProperty(target, "value"));

        target.DefineOwnProperty("value", DataDescriptor("restored"));

        Assert.Equal("restored", ObjectRuntime.GetProperty(target, "value"));
        Assert.True(target.TryGetBoxedValue("value", out var storedValue));
        Assert.Equal("restored", storedValue);
        Assert.True(PropertyDescriptorStore.TryGetOwn(target, "value", out var descriptor));
        Assert.Equal(JsPropertyDescriptorKind.Data, descriptor.Kind);
        Assert.True(descriptor.Writable);
        Assert.True(descriptor.Enumerable);
        Assert.True(descriptor.Configurable);
        Assert.False(PropertyDescriptorStore.HasExternalDescriptorStateForTests(target));
    }

    [Theory]
    [InlineData("first")]
    [InlineData("middle")]
    [InlineData("last")]
    public void OrdinaryDescriptorDeletion_CompactsShapeValuesAndMetadata(string deletedKey)
    {
        var target = new JsObject();
        target.DefineOwnProperty(
            "first",
            DataDescriptor("first-value", enumerable: false, configurable: true));
        target.SetString("middle", "middle-value");
        target.DefineOwnProperty("last", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Accessor,
            Get = (Func<object[], object?[]?, object?>)(static (_, _) => "last-value"),
            Enumerable = true,
            Configurable = true
        });

        Assert.True(ObjectRuntime.DeleteProperty(target, deletedKey));
        Assert.False(PropertyDescriptorStore.TryGetOwn(target, deletedKey, out _));

        if (deletedKey != "first")
        {
            Assert.True(PropertyDescriptorStore.TryGetOwn(target, "first", out var first));
            Assert.Equal("first-value", first.Value);
            Assert.False(first.Enumerable);
        }

        if (deletedKey != "middle")
        {
            Assert.Equal("middle-value", ObjectRuntime.GetProperty(target, "middle"));
        }

        if (deletedKey != "last")
        {
            Assert.Equal("last-value", ObjectRuntime.GetProperty(target, "last"));
        }
    }

    [Fact]
    public void OrdinaryDescriptorState_GrowsAndClearsWithShapeStorage()
    {
        var target = new JsObject();
        target.DefineOwnProperty(
            "custom",
            DataDescriptor(1d, enumerable: false, configurable: true));
        target.SetNumber("defaultA", 2d);
        target.SetNumber("defaultB", 3d);
        target.DefineOwnProperty("accessor", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Accessor,
            Get = (Func<object[], object?[]?, object?>)(static (_, _) => 4d),
            Enumerable = true,
            Configurable = true
        });

        Assert.Equal(4, target.Count);
        Assert.Equal(3d, ObjectRuntime.GetProperty(target, "defaultB"));
        Assert.Equal(4d, ObjectRuntime.GetProperty(target, "accessor"));

        target.Clear();

        Assert.Empty(target);
        Assert.Empty(target.GetOwnPropertyNames());
        Assert.False(target.HasInlineDescriptorState);
        Assert.False(target.HasNonDataDescriptors);
        Assert.False(PropertyDescriptorStore.HasAny(target));

        target.DefineOwnProperty(
            "custom",
            DataDescriptor(5d, enumerable: false, configurable: true));
        PropertyDescriptorStore.Clear(target);

        Assert.Empty(target);
        Assert.False(target.HasInlineDescriptorState);
        Assert.False(PropertyDescriptorStore.HasAny(target));
    }

    [Fact]
    public void SharedShape_DoesNotShareDescriptorAttributes()
    {
        var first = new JsObject();
        var second = new JsObject();
        first.SetNumber("value", 1d);
        second.SetNumber("value", 2d);

        second.DefineOwnProperty(
            "value",
            DataDescriptor(2d, writable: false, configurable: true));

        Assert.True(PropertyDescriptorStore.TryGetOwn(first, "value", out var firstDescriptor));
        Assert.True(PropertyDescriptorStore.TryGetOwn(second, "value", out var secondDescriptor));
        Assert.True(firstDescriptor.Writable);
        Assert.False(secondDescriptor.Writable);
        Assert.False(first.HasInlineDescriptorState);
        Assert.True(second.HasInlineDescriptorState);
    }

    [Fact]
    public void OrdinaryDeleteAndReAdd_AppendsShapeKeyOrder()
    {
        var target = new JsObject();
        target.SetNumber("first", 1d);
        target.SetNumber("second", 2d);
        target.SetNumber("third", 3d);

        Assert.True(ObjectRuntime.DeleteProperty(target, "second"));
        target.SetNumber("second", 4d);

        Assert.Equal(
            new[] { "first", "third", "second" },
            target.GetOwnPropertyKeys());
    }

    [Fact]
    public void JsObjectIntrinsicBaseline_IsInlineWhileRuntimeOverrideUsesOverlay()
    {
        var target = new JsObject();
        using (PropertyDescriptorStore.BeginIntrinsicInitialization())
        {
            PropertyDescriptorStore.DefineOrUpdate(
                target,
                "baseline",
                DataDescriptor("base", enumerable: false));
        }

        Assert.True(target.HasSharedIntrinsicBaseline);
        Assert.True(target.HasInlineDescriptorState);
        Assert.False(PropertyDescriptorStore.HasExternalDescriptorStateForTests(target));

        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            PropertyDescriptorStore.DefineOrUpdate(target, "baseline", DataDescriptor("override"));

            Assert.True(PropertyDescriptorStore.HasExternalDescriptorStateForTests(target));
            Assert.True(PropertyDescriptorStore.TryGetOwn(target, "baseline", out var overridden));
            Assert.Equal("override", overridden.Value);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }

        Assert.True(PropertyDescriptorStore.TryGetOwn(target, "baseline", out var baseline));
        Assert.Equal("base", baseline.Value);
        Assert.False(baseline.Enumerable);
    }

    [Fact]
    public void NonJsObjectTargets_KeepConditionalWeakTableFallback()
    {
        var target = new Dictionary<string, object?>();
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            PropertyDescriptorStore.DefineOrUpdate(target, "value", DataDescriptor(42d));

            Assert.True(PropertyDescriptorStore.HasExternalDescriptorStateForTests(target));
            Assert.True(PropertyDescriptorStore.TryGetOwn(target, "value", out var descriptor));
            Assert.Equal(42d, descriptor.Value);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void RuntimeStore_FallsBackToIntrinsicDescriptor_AndKeepsOverrideIsolated()
    {
        var target = new JsObject();
        using (PropertyDescriptorStore.BeginIntrinsicInitialization())
        {
            PropertyDescriptorStore.DefineOrUpdate(target, "answer", DataDescriptor(42d, enumerable: false));
        }

        var firstRuntime = RuntimeServices.BuildServiceProvider();
        var secondRuntime = RuntimeServices.BuildServiceProvider();

        try
        {
            GlobalThis.ServiceProvider = firstRuntime;
            Assert.True(PropertyDescriptorStore.TryGetOwn(target, "answer", out var firstBase));
            Assert.Equal(42d, firstBase.Value);
            Assert.False(firstBase.Enumerable);

            PropertyDescriptorStore.DefineOrUpdate(target, "answer", DataDescriptor(84d, enumerable: true));
            Assert.True(PropertyDescriptorStore.TryGetOwn(target, "answer", out var firstOverride));
            Assert.Equal(84d, firstOverride.Value);
            Assert.True(firstOverride.Enumerable);

            GlobalThis.ServiceProvider = secondRuntime;
            Assert.True(PropertyDescriptorStore.TryGetOwn(target, "answer", out var secondRuntimeDescriptor));
            Assert.Equal(42d, secondRuntimeDescriptor.Value);
            Assert.False(secondRuntimeDescriptor.Enumerable);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void RuntimeStore_DeleteOverride_MasksIntrinsicDescriptor()
    {
        var target = new JsObject();
        using (PropertyDescriptorStore.BeginIntrinsicInitialization())
        {
            PropertyDescriptorStore.DefineOrUpdate(target, "intrinsic", DataDescriptor("base", enumerable: true));
        }

        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;

            Assert.True(PropertyDescriptorStore.Delete(target, "intrinsic"));
            Assert.False(PropertyDescriptorStore.TryGetOwn(target, "intrinsic", out _));
            Assert.DoesNotContain("intrinsic", PropertyDescriptorStore.GetOwnKeys(target));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }

        Assert.True(PropertyDescriptorStore.TryGetOwn(target, "intrinsic", out var intrinsicDescriptor));
        Assert.Equal("base", intrinsicDescriptor.Value);
    }

    [Fact]
    public void RuntimeStore_MergesIntrinsicAndOverrideKeyOrder()
    {
        var target = new JsObject();
        using (PropertyDescriptorStore.BeginIntrinsicInitialization())
        {
            PropertyDescriptorStore.DefineOrUpdate(target, "baseA", DataDescriptor(1d));
            PropertyDescriptorStore.DefineOrUpdate(target, "baseB", DataDescriptor(2d));
        }

        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;

            PropertyDescriptorStore.DefineOrUpdate(target, "runtimeA", DataDescriptor(3d));
            PropertyDescriptorStore.DefineOrUpdate(target, "baseA", DataDescriptor(4d));
            PropertyDescriptorStore.DefineOrUpdate(target, "runtimeB", DataDescriptor(5d));
            Assert.True(PropertyDescriptorStore.Delete(target, "baseB"));

            Assert.Equal(new[] { "baseA", "runtimeA", "runtimeB" }, PropertyDescriptorStore.GetOwnKeys(target));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void RuntimeStore_RedefinedIntrinsicKeyUsesRuntimeInsertionOrderAfterDelete()
    {
        var target = new JsObject();
        using (PropertyDescriptorStore.BeginIntrinsicInitialization())
        {
            PropertyDescriptorStore.DefineOrUpdate(target, "baseA", DataDescriptor(1d));
            PropertyDescriptorStore.DefineOrUpdate(target, "baseB", DataDescriptor(2d));
        }

        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;

            Assert.True(PropertyDescriptorStore.Delete(target, "baseA"));
            PropertyDescriptorStore.DefineOrUpdate(target, "runtimeA", DataDescriptor(3d));
            PropertyDescriptorStore.DefineOrUpdate(target, "baseA", DataDescriptor(4d));

            Assert.Equal(new[] { "baseB", "runtimeA", "baseA" }, PropertyDescriptorStore.GetOwnKeys(target));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void RuntimeStore_FunctionMetadataDelete_IsScopedToCurrentRuntime()
    {
        Func<object[], object?[]?, object?> functionValue = static (_, _) => null;
        var firstRuntime = RuntimeServices.BuildServiceProvider();
        var secondRuntime = RuntimeServices.BuildServiceProvider();

        try
        {
            GlobalThis.ServiceProvider = firstRuntime;
            Assert.NotNull(JavaScriptRuntime.Object.getOwnPropertyDescriptor(functionValue, "name"));

            Assert.True(ObjectRuntime.DeleteProperty(functionValue, "name"));
            Assert.Null(JavaScriptRuntime.Object.getOwnPropertyDescriptor(functionValue, "name"));

            GlobalThis.ServiceProvider = secondRuntime;
            Assert.NotNull(JavaScriptRuntime.Object.getOwnPropertyDescriptor(functionValue, "name"));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void RuntimeStore_FunctionPrototypeDelete_DoesNotMutateSharedBackingDictionary()
    {
        var firstRuntime = RuntimeServices.BuildServiceProvider();
        var secondRuntime = RuntimeServices.BuildServiceProvider();

        try
        {
            GlobalThis.ServiceProvider = firstRuntime;
            Assert.True(JavaScriptRuntime.Function.TryGetPrototypeValue("bind", out _));

            Assert.True(ObjectRuntime.DeleteProperty(JavaScriptRuntime.Function.Prototype, "bind"));
            Assert.False(JavaScriptRuntime.Function.TryGetPrototypeValue("bind", out _));

            GlobalThis.ServiceProvider = secondRuntime;
            Assert.True(JavaScriptRuntime.Function.TryGetPrototypeValue("bind", out var bindValue));
            Assert.NotNull(bindValue);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void RuntimeStore_WritesCopyIncomingDescriptors()
    {
        var target = new JsObject();
        using (PropertyDescriptorStore.BeginIntrinsicInitialization())
        {
            PropertyDescriptorStore.DefineOrUpdate(target, "value", DataDescriptor("original"));
        }

        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;

            // Writes copy the incoming descriptor, so later caller-side mutation
            // of the written descriptor must not leak into the store.
            var written = DataDescriptor("updated");
            PropertyDescriptorStore.DefineOrUpdate(target, "value", written);
            written.Value = "mutated after write";

            Assert.True(PropertyDescriptorStore.TryGetOwn(target, "value", out var reread));
            Assert.Equal("updated", reread.Value);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void RuntimeStore_ReadsReturnIndependentDescriptorValues()
    {
        var target = new JsObject();
        Func<object[], object?[]?, object?> getter = static (_, _) => "value";
        Action<object?> setter = static _ => { };
        var runtime = RuntimeServices.BuildServiceProvider();

        try
        {
            GlobalThis.ServiceProvider = runtime;
            PropertyDescriptorStore.DefineOrUpdate(target, "value", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Get = getter,
                Set = setter,
                Enumerable = true,
                Configurable = true
            });

            Assert.True(PropertyDescriptorStore.TryGetOwn(target, "value", out var firstRead));
            var getterAdapter =
                BuiltinDelegateFunctionAdapter.FromDelegate(getter);
            var setterAdapter =
                BuiltinDelegateFunctionAdapter.FromDelegate(setter);
            Assert.Same(getterAdapter, firstRead.Get);
            Assert.Same(setterAdapter, firstRead.Set);
            firstRead.Get = null;
            firstRead.Enumerable = false;

            Assert.True(PropertyDescriptorStore.TryGetOwn(target, "value", out var secondRead));
            Assert.Same(getterAdapter, secondRead.Get);
            Assert.Same(setterAdapter, secondRead.Set);
            Assert.True(secondRead.Enumerable);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void IntrinsicStore_AllowsConcurrentRuntimeReads()
    {
        var target = new JsObject();
        using (PropertyDescriptorStore.BeginIntrinsicInitialization())
        {
            PropertyDescriptorStore.DefineOrUpdate(target, "stable", DataDescriptor("base"));
        }

        var exceptions = new List<Exception>();
        var sync = new object();

        Parallel.For(0, 16, _ =>
        {
            try
            {
                var runtime = RuntimeServices.BuildServiceProvider();
                GlobalThis.ServiceProvider = runtime;
                for (var i = 0; i < 500; i++)
                {
                    if (!PropertyDescriptorStore.TryGetOwn(target, "stable", out var descriptor)
                        || !Equals("base", descriptor.Value))
                    {
                        throw new InvalidOperationException("Intrinsic descriptor lookup returned an unexpected value.");
                    }
                }
            }
            catch (Exception ex)
            {
                lock (sync)
                {
                    exceptions.Add(ex);
                }
            }
            finally
            {
                GlobalThis.ServiceProvider = null;
            }
        });

        Assert.Empty(exceptions);
    }

    private static JsPropertyDescriptor DataDescriptor(
        object? value,
        bool enumerable = true,
        bool writable = true,
        bool configurable = true)
        => new()
        {
            Kind = JsPropertyDescriptorKind.Data,
            Value = value,
            Writable = writable,
            Enumerable = enumerable,
            Configurable = configurable
        };
}
