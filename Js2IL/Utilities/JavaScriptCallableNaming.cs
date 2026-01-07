namespace Js2IL.Utilities;

internal static class JavaScriptCallableNaming
{
    public static string MakeClassMethodCallableName(string className, string methodName)
    {
        if (string.IsNullOrWhiteSpace(className)) throw new ArgumentException("Class name must be provided.", nameof(className));
        if (string.IsNullOrWhiteSpace(methodName)) throw new ArgumentException("Method name must be provided.", nameof(methodName));
        return $"{className}.{methodName}";
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
