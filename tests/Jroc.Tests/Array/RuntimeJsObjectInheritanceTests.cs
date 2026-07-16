using JavaScriptRuntime;

namespace Jroc.Tests.Array;

public sealed class RuntimeJsObjectInheritanceTests
{
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
