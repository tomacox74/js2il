using System.Reflection;

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

        // Look through each version folder, e.g., 6.0.0, 7.0.2
        var versionDirs = Directory.GetDirectories(refPackBase)
            .Where(dir => Version.TryParse(Path.GetFileName(dir), out _))
            .OrderByDescending(dir => Version.Parse(Path.GetFileName(dir)!));

        foreach (var versionDir in versionDirs)
        {
            string refDir = Path.Combine(versionDir, "ref");

            if (!Directory.Exists(refDir))
                continue;

            var tfmDirs = Directory.GetDirectories(refDir)
                .Select(Path.GetFileName)
                .Where(name => name != null && name.StartsWith("net") && Version.TryParse(name.Substring(3), out _))
                .OrderByDescending(name => Version.Parse(name!.Substring(3)));

            foreach (var tfm in tfmDirs)
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
