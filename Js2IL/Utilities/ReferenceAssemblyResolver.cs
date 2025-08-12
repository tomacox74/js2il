using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

public static class ReferenceAssemblyResolver
{
    public static bool TryFindSystemRuntime(out AssemblyName systemRuntimeAssembly)
    {
        string? dotnetRoot =
            System.Environment.GetEnvironmentVariable("DOTNET_ROOT") ??
            (OperatingSystem.IsWindows()
                ? Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles), "dotnet")
                : "/usr/share/dotnet");

        string refPackBase = Path.Combine(dotnetRoot, "packs", "Microsoft.NETCore.App.Ref");

        if (!Directory.Exists(refPackBase))
        {
            systemRuntimeAssembly = null!;
            return false;
        }

        // Determine preferred major.minor from the executing assembly's TargetFramework (e.g., v8.0)
        Version? preferred = null;
        try
        {
            var asm = Assembly.GetExecutingAssembly();
            var tfmAttr = asm.GetCustomAttribute<TargetFrameworkAttribute>();
            if (tfmAttr != null)
            {
                // Example: ".NETCoreApp,Version=v8.0"
                var name = tfmAttr.FrameworkName;
                var verIdx = name.IndexOf("Version=v", StringComparison.OrdinalIgnoreCase);
                if (verIdx >= 0)
                {
                    var verStr = name.Substring(verIdx + "Version=v".Length);
                    if (Version.TryParse(verStr, out var v))
                    {
                        preferred = new Version(v.Major, v.Minor);
                    }
                }
            }
        }
        catch { }

        // Fall back to current host runtime if we couldn't detect from TFM
        var current = Environment.Version; // e.g., 8.0.x
        var currentMajorMinor = preferred ?? new Version(current.Major, current.Minor);

        // Look through each version folder, e.g., 6.0.0, 7.0.2, sorted descending
        var versionDirs = Directory.GetDirectories(refPackBase)
            .Where(dir => Version.TryParse(Path.GetFileName(dir), out _))
            .OrderByDescending(dir => Version.Parse(Path.GetFileName(dir)!));

        // Try to find a pack version with the same major.minor as current runtime first
        var preferredVersionDir = versionDirs.FirstOrDefault(dir =>
        {
            var v = Version.Parse(Path.GetFileName(dir)!);
            return v.Major == currentMajorMinor.Major && v.Minor == currentMajorMinor.Minor;
        });

        IEnumerable<string> searchDirs = preferredVersionDir != null
            ? new[] { preferredVersionDir }
            : versionDirs;

        foreach (var versionDir in searchDirs)
        {
            string refDir = Path.Combine(versionDir, "ref");

            if (!Directory.Exists(refDir))
                continue;

            var tfmDirs = Directory.GetDirectories(refDir)
                .Select(Path.GetFileName)
                .Where(name => name != null && name.StartsWith("net") && Version.TryParse(name.Substring(3), out _))
                .OrderByDescending(name => Version.Parse(name!.Substring(3)));

            // Prefer TFM that matches the test assembly's major.minor (or host) e.g., net8.0
            string preferredTfm = $"net{currentMajorMinor.Major}.{currentMajorMinor.Minor}";
            var orderedTfms = tfmDirs
                .OrderByDescending(name => string.Equals(name, preferredTfm, StringComparison.OrdinalIgnoreCase))
                .ThenByDescending(name => Version.Parse(name!.Substring(3)));
            foreach (var tfm in orderedTfms)
            {
                string candidate = Path.Combine(refDir, tfm!, "System.Runtime.dll");
                if (File.Exists(candidate))
                {
                    systemRuntimeAssembly = AssemblyName.GetAssemblyName(candidate);
                    return true;
                }
            }
        }

        systemRuntimeAssembly = null!;
        return false;
    }
}
