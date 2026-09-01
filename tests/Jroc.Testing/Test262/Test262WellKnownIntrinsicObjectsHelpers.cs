using JavaScriptRuntime;

namespace Jroc.Tests;

internal static class Test262WellKnownIntrinsicObjectsHelpers
{
    public static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
    {
        builder.AddGlobalFactory("WellKnownIntrinsicObjects", Create);
    }

    private static object Create()
    {
        return new JavaScriptRuntime.Array(
        new object?[]
        {
            CreateRecord("%ThrowTypeError%", null)
        });
    }

    private static JsObject CreateRecord(string name, object? value)
    {
        var record = new JsObject();
        ObjectRuntime.SetItem(record, "name", name);
        ObjectRuntime.SetItem(record, "value", value);
        return record;
    }
}
