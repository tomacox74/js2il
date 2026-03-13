using System.Reflection;

public static class RuntimeConfigWriter
{
    public static void WriteRuntimeConfigJson(string outputPath, AssemblyName systemRuntimeAssembly)
    {
        // Extract the major and minor version from the reference assembly
        var version = systemRuntimeAssembly.Version ?? new Version(6, 0, 0, 0);
        string tfm = $"net{version.Major}.{version.Minor}";
        string frameworkVersion = $"{version.Major}.{version.Minor}.0";

        // Generate content
        string content = $@"{{
  ""runtimeOptions"": {{
    ""tfm"": ""{tfm}"",
    ""framework"": {{
      ""name"": ""Microsoft.NETCore.App"",
      ""version"": ""{frameworkVersion}""
    }}
  }}
}}";

        // Save to <outputPath>.runtimeconfig.json
        string configPath = Path.ChangeExtension(outputPath, ".runtimeconfig.json");
        File.WriteAllText(configPath, content);
    }
}
