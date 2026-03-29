using System.Reflection;

namespace JavaScriptRuntime;

internal static class CompiledAssemblyPathResolver
{
    internal static string? Resolve(
        Assembly modulesAssembly,
        string? configuredAssemblyPath,
        bool allowAssemblyLocationFallback = true)
    {
        if (!string.IsNullOrWhiteSpace(configuredAssemblyPath))
        {
            return configuredAssemblyPath;
        }

        if (!allowAssemblyLocationFallback)
        {
            return null;
        }

        try
        {
            var location = modulesAssembly.Location;
            return string.IsNullOrWhiteSpace(location) ? null : location;
        }
        catch
        {
            return null;
        }
    }
}
