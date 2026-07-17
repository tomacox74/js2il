using JavaScriptRuntime;

namespace Jroc.Tests.Array;

public sealed class RuntimeJsObjectInheritanceTests
{
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
            Assert.Equal(3d, array.length);

            var ordinaryStorage = (IDictionary<string, object?>)array;
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
}
