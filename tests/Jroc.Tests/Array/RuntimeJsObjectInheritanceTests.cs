using JavaScriptRuntime;

namespace Jroc.Tests.Array;

public sealed class RuntimeJsObjectInheritanceTests
{
    private sealed class DescriptorCountingArray : JavaScriptRuntime.Array
    {
        public DescriptorCountingArray(object?[] values)
            : base(values)
        {
        }

        public int DescriptorLookupCount { get; private set; }

        public void ResetDescriptorLookupCount()
            => DescriptorLookupCount = 0;

        internal override PropertyDescriptorLookup GetOwnPropertyDescriptor(
            string key,
            out JsPropertyDescriptor descriptor)
        {
            DescriptorLookupCount++;
            return base.GetOwnPropertyDescriptor(key, out descriptor);
        }
    }

    [Fact]
    public void ArrayPrototypeAndUnscopables_UseJsObjectRepresentation()
    {
        JavaScriptRuntime.Array.ResetPrototypeForTests();

        try
        {
            var prototype = Assert.IsType<JsObject>(JavaScriptRuntime.Array.Prototype);
            var unscopables = Assert.IsType<JsObject>(
                ObjectRuntime.GetItem(prototype, Symbol.unscopables.DebugId));

            Assert.Equal(JsNull.Null, PrototypeChain.GetPrototypeOrNull(unscopables));
            Assert.Equal(true, ObjectRuntime.GetItem(unscopables, "at"));
        }
        finally
        {
            JavaScriptRuntime.Array.ResetPrototypeForTests();
        }
    }

    [Fact]
    public void DenseTruncation_DoesNotMaterializeDescriptorState()
    {
        var values = Enumerable.Range(0, 4096).Select(value => (object?)(double)value);
        var truncated = new JavaScriptRuntime.Array(values);

        truncated.length = 0;

        Assert.Equal(0d, truncated.length);
        Assert.False(PropertyDescriptorStore.HasAny(truncated));

        var drained = new JavaScriptRuntime.Array(values);
        while (drained.length > 0)
        {
            drained.pop();
        }

        Assert.Equal(0d, drained.length);
        Assert.False(PropertyDescriptorStore.HasAny(drained));
    }

    [Fact]
    public void EmptyAddRange_PreservesVirtualLength()
    {
        var array = new JavaScriptRuntime.Array();
        array.length = 4294967295d;

        array.AddRange(System.Array.Empty<object?>());

        Assert.Equal(4294967295d, array.length);
    }

    [Fact]
    public void Array_UsesInheritedStorageOnlyForOrdinaryProperties()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            var array = new JavaScriptRuntime.Array(new object?[] { 1d, 2d });
            object target = array;

            ObjectRuntime.SetProperty(target, "custom", 3d);
            ObjectRuntime.SetItem(target, "2", 4d);

            Assert.IsAssignableFrom<JsObject>(array);
            Assert.Equal(3d, ObjectRuntime.GetProperty(target, "custom"));
            Assert.Equal(4d, ObjectRuntime.GetItem(target, "2"));
            Assert.Equal(3d, ObjectRuntime.GetProperty(target, "length"));
            Assert.Equal(3d, array.length);

            var ordinaryStorage = (IDictionary<string, object?>)array;
            Assert.False(((JsObject)array).TryGetBoxedValue("length", out _));
            Assert.False(((JsObject)array).TryGetBoxedValue("0", out _));
            Assert.Equal(3d, ordinaryStorage["custom"]);
            Assert.DoesNotContain("0", ((JsObject)array).GetOwnPropertyNames());
            Assert.DoesNotContain("2", ((JsObject)array).GetOwnPropertyNames());
            Assert.DoesNotContain("length", ((JsObject)array).GetOwnPropertyNames());

            ordinaryStorage["4"] = 5d;
            Assert.Equal(5d, array[4]);
            Assert.Equal(5d, array.length);
            Assert.DoesNotContain("4", ((JsObject)array).GetOwnPropertyNames());

            JsObject baseTyped = array;
            baseTyped["custom"] = 4d;
            baseTyped["6"] = 7d;
            baseTyped.SetNumber("7", 8d);
            baseTyped.Add("8", 9d);
            Assert.Equal(4d, ObjectRuntime.GetProperty(target, "custom"));
            Assert.Equal(9d, array.length);
            Assert.Equal(7d, array[6]);
            Assert.Equal(8d, array[7]);
            Assert.Equal(9d, array[8]);
            Assert.DoesNotContain("6", baseTyped.GetOwnPropertyNames());
            Assert.DoesNotContain("7", baseTyped.GetOwnPropertyNames());
            Assert.DoesNotContain("8", baseTyped.GetOwnPropertyNames());

            ordinaryStorage.Clear();
            Assert.False(ordinaryStorage.ContainsKey("custom"));
            Assert.Null(ObjectRuntime.GetProperty(target, "custom"));
            Assert.Equal(9d, array.length);
            Assert.Equal(5d, array[4]);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void ValueReads_BypassSyntheticArrayDescriptors()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            var array = new DescriptorCountingArray(new object?[] { 1d, 2d });
            array.ResetDescriptorLookupCount();

            Assert.Equal(2d, ObjectRuntime.GetProperty(array, "length"));
            Assert.Equal(1d, ObjectRuntime.GetProperty(array, "0"));
            Assert.Equal(0, array.DescriptorLookupCount);

            Func<object[], object?[]?, object?> getter = static (_, _) => "accessor";
            Assert.True(array.DefineOwnProperty("0", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Get = getter,
                Enumerable = true,
                Configurable = true
            }));
            var descriptorLookupCount = array.DescriptorLookupCount;
            Assert.Equal("accessor", ObjectRuntime.GetProperty(array, "0"));
            Assert.Equal(descriptorLookupCount, array.DescriptorLookupCount);

            Assert.True(array.DefineOwnProperty("length", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = 2d,
                Writable = false,
                Enumerable = false,
                Configurable = false
            }));
            descriptorLookupCount = array.DescriptorLookupCount;
            Assert.Equal(2d, ObjectRuntime.GetProperty(array, "length"));
            Assert.Equal(descriptorLookupCount, array.DescriptorLookupCount);

            Assert.True(array.DeleteOwnProperty("0"));
            descriptorLookupCount = array.DescriptorLookupCount;
            Assert.Null(ObjectRuntime.GetProperty(array, "0"));
            Assert.Equal(descriptorLookupCount, array.DescriptorLookupCount);

            Assert.IsType<JsObject>(
                JavaScriptRuntime.Object.getOwnPropertyDescriptor(array, "length"));
            Assert.Equal(descriptorLookupCount + 1, array.DescriptorLookupCount);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void LengthValueReads_AllocateOnlyTheBoxedResult()
    {
        var runtime = RuntimeServices.BuildServiceProvider();
        try
        {
            GlobalThis.ServiceProvider = runtime;
            var array = new JavaScriptRuntime.Array(new object?[] { 1d, 2d, 3d });

            _ = ObjectRuntime.GetProperty(array, "length");
            const int iterations = 10_000;
            object? value = null;
            var before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < iterations; i++)
            {
                value = ObjectRuntime.GetProperty(array, "length");
            }

            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Equal(3d, value);
            Assert.InRange(allocated, 0, iterations * 32L);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }
}
