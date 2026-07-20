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

    [Fact]
    public void SequentialIndexWrites_DoNotAllocateBackingArrayPerElement()
    {
        JavaScriptRuntime.Array.ResetPrototypeForTests();

        try
        {
            var value = new object();
            var warmup = new JavaScriptRuntime.Array();
            Assert.True(warmup.TrySetIndexValue(0, value, throwOnError: true));

            const int elementCount = 1024;
            var array = new JavaScriptRuntime.Array();
            var before = GC.GetAllocatedBytesForCurrentThread();

            for (var index = 0; index < elementCount; index++)
            {
                if (!array.TrySetIndexValue(index, value, throwOnError: true))
                {
                    throw new InvalidOperationException($"Failed to set index {index}.");
                }
            }

            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Equal((double)elementCount, array.length);
            Assert.Same(value, array[0]);
            Assert.Same(value, array[elementCount - 1]);
            Assert.InRange(allocated, 0, 512 * 1024);
        }
        finally
        {
            JavaScriptRuntime.Array.ResetPrototypeForTests();
        }
    }

    [Fact]
    public void DenseNumericWritesAndReads_AvoidPerElementBoxing()
    {
        JavaScriptRuntime.Array.ResetPrototypeForTests();

        try
        {
            var warmup = new JavaScriptRuntime.Array(1);
            Assert.True(warmup.TrySetIndexNumber(0, 1d, throwOnError: true));
            Assert.Equal(1d, ObjectRuntime.GetItemAsNumber(warmup, 0d));

            const int elementCount = 4096;
            var array = new JavaScriptRuntime.Array(elementCount);
            var beforeWrites = GC.GetAllocatedBytesForCurrentThread();
            for (var index = 0; index < elementCount; index++)
            {
                if (!array.TrySetIndexNumber(index, index, throwOnError: true))
                {
                    throw new InvalidOperationException($"Failed to set index {index}.");
                }
            }
            var writeAllocations = GC.GetAllocatedBytesForCurrentThread() - beforeWrites;

            var beforeReads = GC.GetAllocatedBytesForCurrentThread();
            var sum = 0d;
            for (var index = 0; index < elementCount; index++)
            {
                sum += ObjectRuntime.GetItemAsNumber(array, index);
            }
            var readAllocations = GC.GetAllocatedBytesForCurrentThread() - beforeReads;

            Assert.Equal((elementCount - 1d) * elementCount / 2d, sum);
            Assert.InRange(writeAllocations, 0, 128 * 1024);
            Assert.Equal(0, readAllocations);
        }
        finally
        {
            JavaScriptRuntime.Array.ResetPrototypeForTests();
        }
    }

    [Fact]
    public void NumericIndexObjectWrites_AvoidIndexBoxingAndKeyStrings()
    {
        JavaScriptRuntime.Array.ResetPrototypeForTests();

        try
        {
            var value = new object();
            var warmup = JavaScriptRuntime.Array.Construct(new object[] { 1d });
            ObjectRuntime.SetItem(warmup, 0d, value, throwOnError: true);

            const int elementCount = 4096;
            var array = JavaScriptRuntime.Array.Construct(new object[] { (double)elementCount });
            var before = GC.GetAllocatedBytesForCurrentThread();
            for (var index = 0; index < elementCount; index++)
            {
                ObjectRuntime.SetItem(array, (double)index, value, throwOnError: true);
            }
            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Equal((double)elementCount, array.length);
            Assert.Same(value, array[0]);
            Assert.Same(value, array[elementCount - 1]);
            Assert.InRange(allocated, 0, 128 * 1024);
        }
        finally
        {
            JavaScriptRuntime.Array.ResetPrototypeForTests();
        }
    }

    [Fact]
    public void NumericStorage_TransitionsForMixedValuesAndHoles()
    {
        var array = new JavaScriptRuntime.Array(32);
        array.AddNumber(1d);
        array.AddNumber(2d);
        Assert.True(array.TrySetIndexValue(1, "mixed", throwOnError: true));
        Assert.True(array.TrySetIndexNumber(2, 3d, throwOnError: true));

        Assert.True(array.DeleteOwnProperty("0"));

        Assert.False(JavaScriptRuntime.Object.hasOwn(array, "0"));
        Assert.Equal("mixed", array[1]);
        Assert.Equal(3d, ObjectRuntime.GetItemAsNumber(array, 2d));
        Assert.Equal(3d, array.length);
    }

    [Fact]
    public void GenericNumericReads_TransitionOnceAndReuseBoxedValues()
    {
        var array = new JavaScriptRuntime.Array(32);
        array.AddNumber(1d);
        array.AddNumber(2d);

        var first = array[0];
        object? last = null;
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 10_000; i++)
        {
            last = array[0];
        }
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Same(first, last);
        Assert.Equal(0, allocated);

        object boxed = 3d;
        Assert.True(array.TrySetIndexValue(1, boxed, throwOnError: true));
        Assert.Same(boxed, array[1]);
    }

    [Fact]
    public void NumericLengthConstruction_DoesNotMaterializeHoles()
    {
        _ = JavaScriptRuntime.Array.Construct(new object[] { 1d });

        var before = GC.GetAllocatedBytesForCurrentThread();
        var array = JavaScriptRuntime.Array.Construct(new object[] { 10_000_000d });
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(10_000_000d, array.length);
        Assert.False(JavaScriptRuntime.Object.hasOwn(array, "0"));
        Assert.False(JavaScriptRuntime.Object.hasOwn(array, "9999999"));
        Assert.InRange(allocated, 0, 16 * 1024);

        var beforeFirstWrite = GC.GetAllocatedBytesForCurrentThread();
        Assert.True(array.TrySetIndexNumber(0, 1d, throwOnError: true));
        var firstWriteAllocations = GC.GetAllocatedBytesForCurrentThread() - beforeFirstWrite;

        Assert.True(JavaScriptRuntime.Object.hasOwn(array, "0"));
        Assert.False(JavaScriptRuntime.Object.hasOwn(array, "1"));
        Assert.InRange(firstWriteAllocations, 0, 1024 * 1024);
    }

    [Fact]
    public void DenseGrowth_PreservesHolesSparseJumpsAndLengthTruncation()
    {
        var array = JavaScriptRuntime.Array.Construct(new object[] { 8d });
        var first = new object();
        var last = new object();

        Assert.True(array.TrySetIndexValue(0, first, throwOnError: true));
        Assert.True(array.TrySetIndexValue(12, last, throwOnError: true));

        Assert.Equal(13d, array.length);
        Assert.Same(first, array[0]);
        Assert.False(JavaScriptRuntime.Object.hasOwn(array, "1"));
        Assert.False(JavaScriptRuntime.Object.hasOwn(array, "8"));
        Assert.Same(last, array[12]);

        array.length = 2;

        Assert.Equal(2d, array.length);
        Assert.Same(first, array[0]);
        Assert.False(JavaScriptRuntime.Object.hasOwn(array, "1"));
        Assert.False(JavaScriptRuntime.Object.hasOwn(array, "12"));
    }
}
