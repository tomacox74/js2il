using System;
using Js2IL.SymbolTables;

namespace Js2IL.Utilities;

internal static class ScopeNaming
{
    public static string GetModuleName(Scope scope)
    {
        var current = scope;
        while (current.Parent != null)
        {
            current = current.Parent;
        }
        return current.Name;
    }

    // Stable registry key for a scope across the compilation pipeline.
    // - Global scope uses the module name directly.
    // - All other scopes are module-qualified with their full path to prevent collisions.
    public static string GetRegistryScopeName(Scope scope)
    {
        if (scope.Kind == ScopeKind.Global)
        {
            return scope.Name;
        }

        var moduleName = GetModuleName(scope);
        return $"{moduleName}/{scope.GetQualifiedName()}";
    }

    public static string GetRegistryClassName(Scope classScope)
    {
        if (classScope == null) throw new ArgumentNullException(nameof(classScope));
        var ns = classScope.DotNetNamespace ?? "Classes";
        var name = classScope.DotNetTypeName ?? classScope.Name;
        return $"{ns}.{name}";
    }
}
