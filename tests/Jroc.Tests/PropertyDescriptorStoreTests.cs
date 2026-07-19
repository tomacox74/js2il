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
            Assert.Same(getter, firstRead.Get);
            Assert.Same(setter, firstRead.Set);
            firstRead.Get = null;
            firstRead.Enumerable = false;

            Assert.True(PropertyDescriptorStore.TryGetOwn(target, "value", out var secondRead));
            Assert.Same(getter, secondRead.Get);
            Assert.Same(setter, secondRead.Set);
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

    private static JsPropertyDescriptor DataDescriptor(object? value, bool enumerable = true)
        => new()
        {
            Kind = JsPropertyDescriptorKind.Data,
            Value = value,
            Writable = true,
            Enumerable = enumerable,
            Configurable = true
        };
}
