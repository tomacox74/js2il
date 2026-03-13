using System.Diagnostics;
using System.IO.Compression;
using System.Xml.Linq;

namespace Js2IL.Tests;

public class Js2ILSdkPackageTests
{
    [Fact]
    public void Pack_Js2ILSdk_ContainsBuildAssetsAndCoreDependency()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "js2il-sdk-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var repoRoot = FindRepoRoot();
            var feedDir = Path.Combine(tempRoot, "feed");
            Directory.CreateDirectory(feedDir);

            var packageVersion = PackLocalFeed(repoRoot, feedDir);
            var sdkPackagePath = Path.Combine(feedDir, $"Js2IL.SDK.{packageVersion}.nupkg");
            Assert.True(File.Exists(sdkPackagePath), $"Expected package was not produced: {sdkPackagePath}");

            using var archive = ZipFile.OpenRead(sdkPackagePath);
            var entryNames = archive.Entries
                .Select(entry => entry.FullName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            Assert.Contains("build/Js2IL.SDK.props", entryNames);
            Assert.Contains("build/Js2IL.SDK.targets", entryNames);
            Assert.Contains("tasks/net10.0/Js2IL.SDK.dll", entryNames);
            Assert.Contains("tasks/net10.0/Js2IL.Compiler.dll", entryNames);
            Assert.Contains("tasks/net10.0/JavaScriptRuntime.dll", entryNames);
            Assert.Contains("README.md", entryNames);
            Assert.Contains("icon.jpg", entryNames);

            var nuspecEntry = archive.Entries.Single(entry => entry.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase));
            using var nuspecStream = nuspecEntry.Open();
            var nuspec = XDocument.Load(nuspecStream);
            XNamespace ns = nuspec.Root!.Name.Namespace;

            var dependencyIds = nuspec
                .Descendants(ns + "dependency")
                .Select(element => (string?)element.Attribute("id"))
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Cast<string>()
                .ToArray();

            Assert.Contains("Js2IL.Core", dependencyIds, StringComparer.Ordinal);
            Assert.DoesNotContain("js2il", dependencyIds, StringComparer.OrdinalIgnoreCase);

            var targetsEntry = archive.GetEntry("build/Js2IL.SDK.targets");
            Assert.NotNull(targetsEntry);
            using var targetsReader = new StreamReader(targetsEntry!.Open());
            var targetsText = targetsReader.ReadToEnd();
            Assert.Contains("Js2ILCompile", targetsText, StringComparison.Ordinal);
            Assert.Contains("ReferenceOutputAssembly", targetsText, StringComparison.Ordinal);
            Assert.Contains("RootModuleId", targetsText, StringComparison.Ordinal);
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }

    [Fact]
    public void Build_WithLocalJs2ILSdkPackage_CompilesAndRunsHostedModule()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "js2il-sdk-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var repoRoot = FindRepoRoot();
            var feedDir = Path.Combine(tempRoot, "feed");
            var projectDir = Path.Combine(tempRoot, "consumer");
            Directory.CreateDirectory(feedDir);
            Directory.CreateDirectory(projectDir);

            var packageVersion = PackLocalFeed(repoRoot, feedDir);
            WriteConsumerProject(projectDir, feedDir, packageVersion);

            var build = RunProcess(
                fileName: "dotnet",
                arguments: "build .\\Consumer.csproj --nologo --ignore-failed-sources",
                workingDirectory: projectDir,
                timeoutSeconds: 180);

            Assert.True(
                build.ExitCode == 0,
                $"dotnet build failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{build.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{build.StdErr}");

            var generatedDir = Path.Combine(projectDir, "obj", "js2il-custom", "HostedMathModule");
            Assert.True(File.Exists(Path.Combine(generatedDir, "HostedMathModule.dll")), $"Missing generated module dll in '{generatedDir}'.");
            Assert.True(File.Exists(Path.Combine(generatedDir, "HostedMathModule.runtimeconfig.json")), $"Missing generated runtimeconfig in '{generatedDir}'.");

            var targetDir = Path.Combine(projectDir, "bin", "Debug", "net10.0");
            Assert.True(File.Exists(Path.Combine(targetDir, "HostedMathModule.dll")), $"Missing referenced module dll in '{targetDir}'.");
            Assert.True(File.Exists(Path.Combine(targetDir, "HostedMathModule.runtimeconfig.json")), $"Missing copied runtimeconfig in '{targetDir}'.");

            var run = RunProcess(
                fileName: "dotnet",
                arguments: "run --no-build --project .\\Consumer.csproj --nologo",
                workingDirectory: projectDir,
                timeoutSeconds: 180);

            Assert.True(
                run.ExitCode == 0,
                $"dotnet run failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{run.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{run.StdErr}");

            var output = run.StdOut.Replace("\r", string.Empty, StringComparison.Ordinal);
            Assert.Contains("hasModuleId=True", output, StringComparison.Ordinal);
            Assert.Contains("version=1.2.3", output, StringComparison.Ordinal);
            Assert.Contains("sum=3", output, StringComparison.Ordinal);
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }

    private static void WriteConsumerProject(string projectDir, string feedDir, string packageVersion)
    {
        File.WriteAllText(
            Path.Combine(projectDir, "NuGet.Config"),
            $$"""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <packageSources>
                <clear />
                <add key="local" value="{{feedDir}}" />
                <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
              </packageSources>
            </configuration>
            """);

        File.WriteAllText(
            Path.Combine(projectDir, "Consumer.csproj"),
            $$"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net10.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
                <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Js2IL.SDK" Version="{{packageVersion}}" />
                <PackageReference Include="JavaScriptRuntime" Version="{{packageVersion}}" />

                <Js2ILCompile Include="JavaScript\HostedMathModule.js"
                              OutputDirectory="$(BaseIntermediateOutputPath)\js2il-custom\HostedMathModule"
                              RootModuleId="sample.math"
                              CopyToOutputDirectory="true" />
              </ItemGroup>
            </Project>
            """);

        Directory.CreateDirectory(Path.Combine(projectDir, "JavaScript"));

        File.WriteAllText(
            Path.Combine(projectDir, "JavaScript", "HostedMathModule.js"),
            """
            "use strict";
            module.exports = {
              version: "1.2.3",
              add(a, b) {
                return a + b;
              }
            };
            """);

        File.WriteAllText(
            Path.Combine(projectDir, "Program.cs"),
            """
            using System.Linq;
            using Js2IL.Runtime;
            using Js2IL.HostedMathModule;

            var moduleIds = JsEngine.GetModuleIds(typeof(IHostedMathModuleExports).Assembly);
            Console.WriteLine($"hasModuleId={moduleIds.Contains("sample.math", StringComparer.Ordinal)}");

            using var exports = JsEngine.LoadModule<IHostedMathModuleExports>();
            Console.WriteLine($"version={exports.Version}");
            Console.WriteLine($"sum={exports.Add(1, 2)}");
            """);
    }

    private static string PackLocalFeed(string repoRoot, string feedDir)
    {
        var packageVersion = CreateLocalTestPackageVersion(ReadPackageVersion(Path.Combine(repoRoot, "src", "Js2IL.SDK", "Js2IL.SDK.csproj")));

        foreach (var relativeProjectPath in new[]
                 {
                     Path.Combine("src", "JavaScriptRuntime", "JavaScriptRuntime.csproj"),
                     Path.Combine("src", "Js2IL.Core", "Js2IL.Core.csproj"),
                     Path.Combine("src", "Js2IL.SDK", "Js2IL.SDK.csproj")
                 })
        {
            var fullProjectPath = Path.Combine(repoRoot, relativeProjectPath);
            var pack = RunProcess(
                fileName: "dotnet",
                arguments: $"pack \"{fullProjectPath}\" -c Release -o \"{feedDir}\" --nologo -p:Version={packageVersion}",
                workingDirectory: repoRoot,
                timeoutSeconds: 180);

            Assert.True(
                pack.ExitCode == 0,
                $"dotnet pack failed for '{relativeProjectPath}'.{Environment.NewLine}STDOUT:{Environment.NewLine}{pack.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{pack.StdErr}");
        }

        return packageVersion;
    }

    private static string CreateLocalTestPackageVersion(string baseVersion)
    {
        return $"{baseVersion}-sdktest{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }

    private static string ReadPackageVersion(string csprojPath)
    {
        var project = XDocument.Load(csprojPath);
        var version = project.Descendants("Version").Select(element => element.Value).FirstOrDefault();
        Assert.False(string.IsNullOrWhiteSpace(version), $"Could not find <Version> in '{csprojPath}'.");
        return version!;
    }

    private static string FindRepoRoot()
    {
        var start = new DirectoryInfo(Path.GetDirectoryName(typeof(Js2ILSdkPackageTests).Assembly.Location)!);
        DirectoryInfo? dir = start;

        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "js2il.sln")))
        {
            dir = dir.Parent;
        }

        Assert.NotNull(dir);
        return dir!.FullName;
    }

    private static (int ExitCode, string StdOut, string StdErr) RunProcess(string fileName, string arguments, string workingDirectory, int timeoutSeconds)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi)!;
        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();

        if (!process.WaitForExit(timeoutSeconds * 1000))
        {
            try { process.Kill(entireProcessTree: true); } catch { /* ignore */ }
            throw new TimeoutException($"Process '{fileName} {arguments}' timed out after {timeoutSeconds} seconds.");
        }

        var stdOut = stdoutTask.GetAwaiter().GetResult();
        var stdErr = stderrTask.GetAwaiter().GetResult();
        return (process.ExitCode, stdOut, stdErr);
    }
}
