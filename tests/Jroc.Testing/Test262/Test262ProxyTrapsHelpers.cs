using JavaScriptRuntime;

namespace Jroc.Tests;

internal static class Test262ProxyTrapsHelpers
{
    private static readonly (string Name, string ErrorMessage)[] TrapDefinitions =
    [
        ("getPrototypeOf", "[[GetPrototypeOf]] trap called"),
        ("setPrototypeOf", "[[SetPrototypeOf]] trap called"),
        ("isExtensible", "[[IsExtensible]] trap called"),
        ("preventExtensions", "[[PreventExtensions]] trap called"),
        ("getOwnPropertyDescriptor", "[[GetOwnProperty]] trap called"),
        ("has", "[[HasProperty]] trap called"),
        ("get", "[[Get]] trap called"),
        ("set", "[[Set]] trap called"),
        ("deleteProperty", "[[Delete]] trap called"),
        ("defineProperty", "[[DefineOwnProperty]] trap called"),
        ("ownKeys", "[[OwnPropertyKeys]] trap called"),
        ("apply", "[[Call]] trap called"),
        ("construct", "[[Construct]] trap called")
    ];

    public static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
    {
        builder.AddGlobalFactory("allowProxyTraps", () => Test262HostRuntimeIntrinsics.CreateFunction(
            (Func<object?, object?>)AllowProxyTraps,
            "allowProxyTraps",
            1));
    }

    private static object AllowProxyTraps(object? overrides)
    {
        var handlers = new JsObject();
        var source = TypeUtilities.ToBoolean(overrides) ? overrides : new JsObject();

        foreach (var (name, errorMessage) in TrapDefinitions)
        {
            ObjectRuntime.SetItem(handlers, name, SelectTrap(source, name, errorMessage));
        }

        ObjectRuntime.SetItem(
            handlers,
            "enumerate",
            ThrowingTrap("[[Enumerate]] trap called: this trap has been removed"));

        return handlers;
    }

    private static object? SelectTrap(object? overrides, string name, string errorMessage)
    {
        var candidate = ObjectRuntime.GetItem(overrides!, name);
        return TypeUtilities.ToBoolean(candidate) ? candidate : ThrowingTrap(errorMessage);
    }

    private static object ThrowingTrap(string message)
        => Test262HostRuntimeIntrinsics.CreateFunction(
            (Action)(() => throw Test262HostRuntimeIntrinsics.CreateTest262Error(message)),
            string.Empty,
            0);
}
