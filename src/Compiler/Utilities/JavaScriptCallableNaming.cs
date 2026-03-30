namespace Js2IL.Utilities;

internal static class JavaScriptCallableNaming
{
    public static string MakeClassMethodCallableName(string className, string methodName)
    {
        if (string.IsNullOrWhiteSpace(className)) throw new ArgumentException("Class name must be provided.", nameof(className));
        if (string.IsNullOrWhiteSpace(methodName)) throw new ArgumentException("Method name must be provided.", nameof(methodName));
        return $"{className}.{methodName}";
    }

    public static string MakeClassAccessorCallableName(string className, string accessorKind, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(className)) throw new ArgumentException("Class name must be provided.", nameof(className));
        if (string.IsNullOrWhiteSpace(accessorKind)) throw new ArgumentException("Accessor kind must be provided.", nameof(accessorKind));
        if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException("Property name must be provided.", nameof(propertyName));

        // Convention: "ClassName.get:prop" / "ClassName.set:prop".
        // This stays simple, stable, and disambiguates get vs set which otherwise collide.
        return $"{className}.{accessorKind}:{propertyName}";
    }

    public static bool TrySplitClassMethodCallableName(string? callableName, out string className, out string methodName)
    {
        className = string.Empty;
        methodName = string.Empty;

        if (string.IsNullOrWhiteSpace(callableName))
        {
            return false;
        }

        // CallableId convention for class methods: "ClassName.methodName"
        // Keep this intentionally simple and deterministic.
        var dot = callableName.IndexOf('.');
        if (dot <= 0 || dot >= callableName.Length - 1)
        {
            return false;
        }

        className = callableName[..dot];
        methodName = callableName[(dot + 1)..];
        return true;
    }
}
