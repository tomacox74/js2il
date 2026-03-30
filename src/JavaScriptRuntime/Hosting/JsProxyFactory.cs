using System.Reflection;

namespace Js2IL.Runtime;

internal static class JsProxyFactory
{
    private static readonly MethodInfo CreateMethod = typeof(DispatchProxy)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Single(m => m.Name == nameof(DispatchProxy.Create)
            && m.IsGenericMethodDefinition
            && m.GetGenericArguments().Length == 2
            && m.GetParameters().Length == 0);

    internal static object CreateProxy(Type interfaceType, Type proxyType)
    {
        ArgumentNullException.ThrowIfNull(interfaceType);
        ArgumentNullException.ThrowIfNull(proxyType);

        var method = CreateMethod.MakeGenericMethod(interfaceType, proxyType);
        return method.Invoke(null, null)
            ?? throw new InvalidOperationException($"Failed to create proxy for '{interfaceType.FullName}'.");
    }
}
