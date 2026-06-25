using System.Diagnostics;
using System.IO.Compression;
using System.Xml.Linq;

namespace Jroc.Tests;

[Collection("RepoDotnetCli")]
public class JrocSdkPackageTests
{
    [Fact]
    public void Pack_JrocSdk_ContainsBuildAssetsSamplesAndCoreDependency()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "jroc-sdk-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var repoRoot = FindRepoRoot();
            var feedDir = Path.Combine(tempRoot, "feed");
            Directory.CreateDirectory(feedDir);

            var packageVersion = PackLocalFeed(repoRoot, feedDir);
            var sdkPackagePath = Path.Combine(feedDir, $"Jroc.SDK.{packageVersion}.nupkg");
            Assert.True(File.Exists(sdkPackagePath), $"Expected package was not produced: {sdkPackagePath}");

            var package = ReadPackedPackage(sdkPackagePath);
            var entryNames = package.EntryNames;

            Assert.Contains("build/Jroc.SDK.props", entryNames);
            Assert.Contains("build/Jroc.SDK.targets", entryNames);
            Assert.Contains("tasks/net10.0/Jroc.SDK.dll", entryNames);
            Assert.Contains("tasks/net10.0/Jroc.Compiler.dll", entryNames);
            Assert.Contains("tasks/net10.0/JavaScriptRuntime.dll", entryNames);
            Assert.Contains("README.md", entryNames);
            Assert.Contains("icon.jpg", entryNames);
            Assert.Contains("samples/Directory.Build.props", entryNames);
            Assert.Contains("samples/Hosting.Basic/host/Hosting.Basic.csproj", entryNames);
            Assert.Contains("samples/Hosting.Basic/compiler/JavaScript/HostedMathModule.js", entryNames);
            Assert.DoesNotContain("samples/Hosting.Basic/compiler/HostedMathModule.proj", entryNames);
            Assert.Contains("samples/Hosting.Typed/host/Hosting.Typed.csproj", entryNames);
            Assert.Contains("samples/Hosting.Typed/compiler/JavaScript/HostedCounterModule.js", entryNames);
            Assert.DoesNotContain("samples/Hosting.Typed/compiler/HostedCounterModule.proj", entryNames);
            Assert.Contains("samples/Hosting.Domino/host/Hosting.Domino.csproj", entryNames);
            Assert.Contains("samples/Hosting.Domino/host/package.json", entryNames);
            Assert.Contains("samples/Hosting.Domino/host/package-lock.json", entryNames);
            Assert.DoesNotContain("samples/Hosting.Domino/compiler/package.json", entryNames);
            Assert.DoesNotContain("samples/Hosting.Domino/compiler/package-lock.json", entryNames);
            Assert.DoesNotContain(entryNames, name => name.Contains("/jroc/", StringComparison.OrdinalIgnoreCase));

            AssertPackagePageMetadata(
                package,
                expectedId: "Jroc.SDK",
                expectedDescription: "MSBuild SDK package for compiling JavaScript sources into .NET assemblies during dotnet build.",
                expectedProjectUrl: "https://github.com/tomacox74/jroc/blob/master/docs/sdk/Index.md",
                requiredTags:
                [
                    "compiler",
                    "msbuild",
                    "sdk",
                    "hosting"
                ],
                requiredReadmeLinks:
                [
                    "https://www.nuget.org/packages/jroc",
                    "https://www.nuget.org/packages/Jroc.Core",
                    "https://www.nuget.org/packages/Jroc.SDK",
                    "https://www.nuget.org/packages/Jroc.Runtime"
                ]);

            var dependencyIds = GetDependencyIds(package.Nuspec);

            Assert.Contains("Jroc.Core", dependencyIds, StringComparer.Ordinal);
            Assert.DoesNotContain("jroc", dependencyIds, StringComparer.OrdinalIgnoreCase);
            Assert.Contains("<PackageReference Include=\"Jroc.Runtime\" Version=\"VERSION\" />", package.ReadmeText, StringComparison.Ordinal);

            using var archive = ZipFile.OpenRead(sdkPackagePath);
            var targetsEntry = archive.GetEntry("build/Jroc.SDK.targets");
            Assert.NotNull(targetsEntry);
            using var targetsReader = new StreamReader(targetsEntry!.Open());
            var targetsText = targetsReader.ReadToEnd();
            Assert.Contains("JrocCompile", targetsText, StringComparison.Ordinal);
            Assert.Contains("ModuleResolutionBaseDirectory", targetsText, StringComparison.Ordinal);
            Assert.Contains("ReferenceOutputAssembly", targetsText, StringComparison.Ordinal);
            Assert.Contains("RootModuleId", targetsText, StringComparison.Ordinal);

            var dominoSampleEntry = archive.GetEntry("samples/Hosting.Domino/host/Hosting.Domino.csproj");
            Assert.NotNull(dominoSampleEntry);
            using var dominoSampleReader = new StreamReader(dominoSampleEntry!.Open());
            var dominoSampleText = dominoSampleReader.ReadToEnd();
            Assert.Contains("<JrocCompile Include=\"@mixmark-io/domino\"", dominoSampleText, StringComparison.Ordinal);
            Assert.DoesNotContain("DominoCompilerDir", dominoSampleText, StringComparison.Ordinal);
            Assert.DoesNotContain("JrocModuleResolutionBaseDirectory", dominoSampleText, StringComparison.Ordinal);
            Assert.DoesNotContain("ModuleResolutionBaseDirectory=", dominoSampleText, StringComparison.Ordinal);
            Assert.DoesNotContain("node_modules','@mixmark-io','domino','lib','index.js", dominoSampleText, StringComparison.Ordinal);
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }

    [Fact]
    public void Pack_JrocTool_DoesNotShipHostingSamples()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "jroc-sdk-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var repoRoot = FindRepoRoot();
            var feedDir = Path.Combine(tempRoot, "feed");
            Directory.CreateDirectory(feedDir);

            var packageVersion = CreateLocalTestPackageVersion(ReadPackageVersion(Path.Combine(repoRoot, "src", "Cli", "Jroc.csproj")));
            PackProject(repoRoot, Path.Combine("src", "Cli", "Jroc.csproj"), feedDir, packageVersion);

            var toolPackagePath = Path.Combine(feedDir, $"jroc.{packageVersion}.nupkg");
            Assert.True(File.Exists(toolPackagePath), $"Expected package was not produced: {toolPackagePath}");

            var package = ReadPackedPackage(toolPackagePath);
            var entryNames = package.EntryNames.ToArray();

            Assert.DoesNotContain(entryNames, name => name.StartsWith("samples/", StringComparison.OrdinalIgnoreCase));
            Assert.Contains("README.md", entryNames);
            Assert.Contains("icon.jpg", entryNames);
            Assert.Contains("https://www.nuget.org/packages/Jroc.Core", package.ReadmeText, StringComparison.Ordinal);
            Assert.Contains("https://www.nuget.org/packages/Jroc.SDK", package.ReadmeText, StringComparison.Ordinal);
            Assert.Contains("https://www.nuget.org/packages/Jroc.Runtime", package.ReadmeText, StringComparison.Ordinal);
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }

    [Fact]
    public void Pack_JrocCore_ContainsReadmeIconAndDiscoverabilityMetadata()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "jroc-sdk-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var repoRoot = FindRepoRoot();
            var feedDir = Path.Combine(tempRoot, "feed");
            Directory.CreateDirectory(feedDir);

            var packageVersion = PackLocalFeed(repoRoot, feedDir);
            var corePackagePath = Path.Combine(feedDir, $"Jroc.Core.{packageVersion}.nupkg");
            Assert.True(File.Exists(corePackagePath), $"Expected package was not produced: {corePackagePath}");

            var package = ReadPackedPackage(corePackagePath);

            AssertPackagePageMetadata(
                package,
                expectedId: "Jroc.Core",
                expectedDescription: "Reusable jroc compiler library for embedding JavaScript-to-.NET compilation in custom .NET tools and hosts.",
                expectedProjectUrl: "https://github.com/tomacox74/jroc/blob/master/docs/sdk/Index.md",
                requiredTags:
                [
                    "compiler",
                    "library",
                    "hosting"
                ],
                requiredReadmeLinks:
                [
                    "https://www.nuget.org/packages/jroc",
                    "https://www.nuget.org/packages/Jroc.Core",
                    "https://www.nuget.org/packages/Jroc.SDK",
                    "https://www.nuget.org/packages/Jroc.Runtime"
                ]);

            var dependencyIds = GetDependencyIds(package.Nuspec);
            Assert.Contains("Jroc.Runtime", dependencyIds, StringComparer.Ordinal);
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }

    [Fact]
    public void Pack_JrocRuntime_ContainsReadmeIconAndDiscoverabilityMetadata()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "jroc-sdk-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var repoRoot = FindRepoRoot();
            var feedDir = Path.Combine(tempRoot, "feed");
            Directory.CreateDirectory(feedDir);

            var packageVersion = PackLocalFeed(repoRoot, feedDir);
            var runtimePackagePath = Path.Combine(feedDir, $"Jroc.Runtime.{packageVersion}.nupkg");
            Assert.True(File.Exists(runtimePackagePath), $"Expected package was not produced: {runtimePackagePath}");

            var package = ReadPackedPackage(runtimePackagePath);

            AssertPackagePageMetadata(
                package,
                expectedId: "Jroc.Runtime",
                expectedDescription: "Runtime support library for executing and hosting JROC-compiled assemblies from .NET.",
                expectedProjectUrl: "https://github.com/tomacox74/jroc/blob/master/docs/sdk/Index.md",
                requiredTags:
                [
                    "runtime",
                    "hosting"
                ],
                requiredReadmeLinks:
                [
                    "https://www.nuget.org/packages/jroc",
                    "https://www.nuget.org/packages/Jroc.Core",
                    "https://www.nuget.org/packages/Jroc.SDK",
                    "https://www.nuget.org/packages/Jroc.Runtime"
                ]);

            Assert.Contains("<PackageReference Include=\"Jroc.Runtime\" Version=\"VERSION\" />", package.ReadmeText, StringComparison.Ordinal);
            Assert.Contains("JavaScriptRuntime.dll", package.ReadmeText, StringComparison.Ordinal);
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }

    [Fact]
    public void Build_WithLocalJrocSdkPackage_CompilesAndRunsHostedModule()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "jroc-sdk-tests", Guid.NewGuid().ToString("N"));
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
                arguments: "build Consumer.csproj --nologo --ignore-failed-sources",
                workingDirectory: projectDir,
                timeoutSeconds: 180);

            Assert.True(
                build.ExitCode == 0,
                $"dotnet build failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{build.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{build.StdErr}");

            var generatedDir = Path.Combine(projectDir, "obj", "jroc-custom", "HostedMathModule");
            Assert.True(File.Exists(Path.Combine(generatedDir, "HostedMathModule.dll")), $"Missing generated module dll in '{generatedDir}'.");
            Assert.True(File.Exists(Path.Combine(generatedDir, "HostedMathModule.runtimeconfig.json")), $"Missing generated runtimeconfig in '{generatedDir}'.");

            var targetDir = Path.Combine(projectDir, "bin", "Debug", "net10.0");
            Assert.True(File.Exists(Path.Combine(targetDir, "HostedMathModule.dll")), $"Missing referenced module dll in '{targetDir}'.");
            Assert.True(File.Exists(Path.Combine(targetDir, "HostedMathModule.runtimeconfig.json")), $"Missing copied runtimeconfig in '{targetDir}'.");

            var run = RunProcess(
                fileName: "dotnet",
                arguments: "run --no-build --project Consumer.csproj --nologo",
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

    [Fact]
    public void Build_WithLocalJrocSdkPackage_CompilesPackageEntrypointByModuleId()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "jroc-sdk-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var repoRoot = FindRepoRoot();
            var feedDir = Path.Combine(tempRoot, "feed");
            var projectDir = Path.Combine(tempRoot, "consumer");
            Directory.CreateDirectory(feedDir);
            Directory.CreateDirectory(projectDir);

            var packageVersion = PackLocalFeed(repoRoot, feedDir);
            WriteModuleIdConsumerProject(projectDir, feedDir, packageVersion);

            var build = RunProcess(
                fileName: "dotnet",
                arguments: "build Consumer.csproj --nologo --ignore-failed-sources",
                workingDirectory: projectDir,
                timeoutSeconds: 180);

            Assert.True(
                build.ExitCode == 0,
                $"dotnet build failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{build.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{build.StdErr}");

            var generatedDir = Path.Combine(projectDir, "obj", "jroc-custom", "pkg");
            Assert.True(File.Exists(Path.Combine(generatedDir, "index.dll")), $"Missing generated package module dll in '{generatedDir}'.");
            Assert.True(File.Exists(Path.Combine(generatedDir, "index.runtimeconfig.json")), $"Missing generated package runtimeconfig in '{generatedDir}'.");

            var targetDir = Path.Combine(projectDir, "bin", "Debug", "net10.0");
            Assert.True(File.Exists(Path.Combine(targetDir, "index.dll")), $"Missing copied package module dll in '{targetDir}'.");

            var run = RunProcess(
                fileName: "dotnet",
                arguments: "run --no-build --project Consumer.csproj --nologo",
                workingDirectory: projectDir,
                timeoutSeconds: 180);

            Assert.True(
                run.ExitCode == 0,
                $"dotnet run failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{run.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{run.StdErr}");

            var output = run.StdOut.Replace("\r", string.Empty, StringComparison.Ordinal);
            Assert.Contains("hasModuleId=True", output, StringComparison.Ordinal);
            Assert.Contains("value=42", output, StringComparison.Ordinal);
            Assert.Contains("message=hello from package", output, StringComparison.Ordinal);
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }

    [Fact]
    public void Build_ExtractedHostingBasicSample_WithLocalJrocSdkPackage_CompilesAndRuns()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "jroc-sdk-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var repoRoot = FindRepoRoot();
            var feedDir = Path.Combine(tempRoot, "feed");
            var extractDir = Path.Combine(tempRoot, "sdk-package");
            Directory.CreateDirectory(feedDir);

            var packageVersion = PackLocalFeed(repoRoot, feedDir);
            var sdkPackagePath = Path.Combine(feedDir, $"Jroc.SDK.{packageVersion}.nupkg");
            Assert.True(File.Exists(sdkPackagePath), $"Expected package was not produced: {sdkPackagePath}");

            ZipFile.ExtractToDirectory(sdkPackagePath, extractDir);
            WriteNuGetConfig(extractDir, feedDir);

            var hostDir = Path.Combine(extractDir, "samples", "Hosting.Basic", "host");
            var build = RunProcess(
                fileName: "dotnet",
                arguments: $"build Hosting.Basic.csproj -c Release --nologo --ignore-failed-sources -p:JrocPackageVersion={packageVersion}",
                workingDirectory: hostDir,
                timeoutSeconds: 180);

            Assert.True(
                build.ExitCode == 0,
                $"dotnet build failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{build.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{build.StdErr}");

            var generatedDir = Path.Combine(hostDir, "obj", "Release", "net10.0", "jroc", "HostedMathModule");
            Assert.True(File.Exists(Path.Combine(generatedDir, "HostedMathModule.dll")), $"Missing generated module dll in '{generatedDir}'.");
            Assert.False(Directory.Exists(Path.Combine(hostDir, "jroc")), $"Expected generated outputs to stay under obj, but found '{Path.Combine(hostDir, "jroc")}'.");

            var run = RunProcess(
                fileName: "dotnet",
                arguments: $"run --project Hosting.Basic.csproj -c Release --no-build --nologo -p:JrocPackageVersion={packageVersion}",
                workingDirectory: hostDir,
                timeoutSeconds: 180);

            Assert.True(
                run.ExitCode == 0,
                $"dotnet run failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{run.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{run.StdErr}");

            var output = run.StdOut.Replace("\r", string.Empty, StringComparison.Ordinal);
            Assert.Contains("version=", output, StringComparison.Ordinal);
            Assert.Contains("1+2=", output, StringComparison.Ordinal);
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }

    [Fact]
    public void Build_ExtractedHostingTypedSample_WithLocalJrocSdkPackage_CompilesAndRuns()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "jroc-sdk-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var repoRoot = FindRepoRoot();
            var feedDir = Path.Combine(tempRoot, "feed");
            var extractDir = Path.Combine(tempRoot, "sdk-package");
            Directory.CreateDirectory(feedDir);

            var packageVersion = PackLocalFeed(repoRoot, feedDir);
            var sdkPackagePath = Path.Combine(feedDir, $"Jroc.SDK.{packageVersion}.nupkg");
            Assert.True(File.Exists(sdkPackagePath), $"Expected package was not produced: {sdkPackagePath}");

            ZipFile.ExtractToDirectory(sdkPackagePath, extractDir);
            WriteNuGetConfig(extractDir, feedDir);

            var hostDir = Path.Combine(extractDir, "samples", "Hosting.Typed", "host");
            var build = RunProcess(
                fileName: "dotnet",
                arguments: $"build Hosting.Typed.csproj -c Release --nologo --ignore-failed-sources -p:JrocPackageVersion={packageVersion}",
                workingDirectory: hostDir,
                timeoutSeconds: 180);

            Assert.True(
                build.ExitCode == 0,
                $"dotnet build failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{build.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{build.StdErr}");

            var generatedDir = Path.Combine(hostDir, "obj", "Release", "net10.0", "jroc", "HostedCounterModule");
            Assert.True(File.Exists(Path.Combine(generatedDir, "HostedCounterModule.dll")), $"Missing generated module dll in '{generatedDir}'.");

            var run = RunProcess(
                fileName: "dotnet",
                arguments: $"run --project Hosting.Typed.csproj -c Release --no-build --nologo -p:JrocPackageVersion={packageVersion}",
                workingDirectory: hostDir,
                timeoutSeconds: 180);

            Assert.True(
                run.ExitCode == 0,
                $"dotnet run failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{run.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{run.StdErr}");

            var output = run.StdOut.Replace("\r", string.Empty, StringComparison.Ordinal);
            Assert.Contains("version=", output, StringComparison.Ordinal);
            Assert.Contains("add(1,2)=", output, StringComparison.Ordinal);
            Assert.Contains("counter.add(5)=", output, StringComparison.Ordinal);
            Assert.Contains("created.add(1)=", output, StringComparison.Ordinal);
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }

    private static void WriteConsumerProject(string projectDir, string feedDir, string packageVersion)
    {
        WriteNuGetConfig(projectDir, feedDir);

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
                <PackageReference Include="Jroc.SDK" Version="{{packageVersion}}" />
                <PackageReference Include="Jroc.Runtime" Version="{{packageVersion}}" />

                <JrocCompile Include="JavaScript\HostedMathModule.js"
                              OutputDirectory="$(BaseIntermediateOutputPath)\jroc-custom\HostedMathModule"
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
            using Jroc.Runtime;
            using Jroc.HostedMathModule;

            var moduleIds = JsEngine.GetModuleIds(typeof(IHostedMathModuleExports).Assembly);
            Console.WriteLine($"hasModuleId={moduleIds.Contains("sample.math", StringComparer.Ordinal)}");

            using var exports = JsEngine.LoadModule<IHostedMathModuleExports>();
            Console.WriteLine($"version={exports.Version}");
            Console.WriteLine($"sum={exports.Add(1, 2)}");
            """);
    }

    private static void WriteModuleIdConsumerProject(string projectDir, string feedDir, string packageVersion)
    {
        WriteNuGetConfig(projectDir, feedDir);

        var packageRoot = Path.Combine(projectDir, "node_modules", "@scope", "pkg");
        Directory.CreateDirectory(Path.Combine(packageRoot, "lib"));

        File.WriteAllText(
            Path.Combine(packageRoot, "package.json"),
            """
            {
              "name": "@scope/pkg",
              "main": "lib/index.js"
            }
            """);

        File.WriteAllText(
            Path.Combine(packageRoot, "lib", "index.js"),
            """
            "use strict";
            module.exports = {
              value: 42,
              message: "hello from package"
            };
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
                <PackageReference Include="Jroc.SDK" Version="{{packageVersion}}" />
                <PackageReference Include="Jroc.Runtime" Version="{{packageVersion}}" />

                <JrocCompile Include="@scope/pkg"
                              OutputDirectory="$(BaseIntermediateOutputPath)\jroc-custom\pkg"
                              CopyToOutputDirectory="true" />
              </ItemGroup>
            </Project>
            """);

        File.WriteAllText(
            Path.Combine(projectDir, "Program.cs"),
            """
            using System.Linq;
            using System.Reflection;
            using Jroc.Runtime;

            var compiledModulePath = Path.Combine(AppContext.BaseDirectory, "index.dll");
            var asm = Assembly.LoadFrom(compiledModulePath);
            var moduleIds = JsEngine.GetModuleIds(asm);
            Console.WriteLine($"hasModuleId={moduleIds.Contains("@scope/pkg")}");

            using dynamic exports = JsEngine.LoadModule(asm, moduleId: "@scope/pkg");
            Console.WriteLine($"value={exports.value}");
            Console.WriteLine($"message={exports.message}");
            """);
    }

    private static void WriteNuGetConfig(string directory, string feedDir)
    {
        File.WriteAllText(
            Path.Combine(directory, "NuGet.Config"),
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
    }

    private static string PackLocalFeed(string repoRoot, string feedDir)
    {
        var packageVersion = CreateLocalTestPackageVersion(ReadPackageVersion(Path.Combine(repoRoot, "src", "Jroc.SDK", "Jroc.SDK.csproj")));

        foreach (var relativeProjectPath in new[]
                 {
                      Path.Combine("src", "JavaScriptRuntime", "JavaScriptRuntime.csproj"),
                      Path.Combine("src", "Jroc.Core", "Jroc.Core.csproj"),
                      Path.Combine("src", "Jroc.SDK", "Jroc.SDK.csproj")
                   })
        {
            PackProject(repoRoot, relativeProjectPath, feedDir, packageVersion);
        }

        return packageVersion;
    }

    private static void PackProject(string repoRoot, string relativeProjectPath, string feedDir, string packageVersion)
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

    private static PackedPackage ReadPackedPackage(string packagePath)
    {
        using var archive = ZipFile.OpenRead(packagePath);

        var entryNames = archive.Entries
            .Select(entry => entry.FullName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var nuspecEntry = archive.Entries.Single(entry => entry.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase));
        using var nuspecStream = nuspecEntry.Open();
        var nuspec = XDocument.Load(nuspecStream);

        var readmeEntry = archive.GetEntry("README.md");
        Assert.NotNull(readmeEntry);

        using var readmeReader = new StreamReader(readmeEntry!.Open());
        var readmeText = readmeReader.ReadToEnd();

        return new PackedPackage(entryNames, readmeText, nuspec);
    }

    private static void AssertPackagePageMetadata(
        PackedPackage package,
        string expectedId,
        string expectedDescription,
        string expectedProjectUrl,
        string[] requiredTags,
        string[] requiredReadmeLinks)
    {
        Assert.Contains("README.md", package.EntryNames);
        Assert.Contains("icon.jpg", package.EntryNames);

        Assert.Equal(expectedId, GetMetadataValue(package.Nuspec, "id"));
        Assert.Equal(expectedDescription, GetMetadataValue(package.Nuspec, "description"));
        Assert.Equal("README.md", GetMetadataValue(package.Nuspec, "readme"));
        Assert.Equal("icon.jpg", GetMetadataValue(package.Nuspec, "icon"));
        Assert.Equal(expectedProjectUrl, GetMetadataValue(package.Nuspec, "projectUrl"));
        Assert.Equal("https://github.com/tomacox74/jroc", GetRepositoryUrl(package.Nuspec));
        Assert.Equal("git", GetRepositoryType(package.Nuspec));

        var tags = GetMetadataValue(package.Nuspec, "tags");
        foreach (var tag in requiredTags)
        {
            Assert.Contains(tag, tags, StringComparison.OrdinalIgnoreCase);
        }

        foreach (var link in requiredReadmeLinks)
        {
            Assert.Contains(link, package.ReadmeText, StringComparison.Ordinal);
        }
    }

    private static string[] GetDependencyIds(XDocument nuspec)
    {
        XNamespace ns = nuspec.Root!.Name.Namespace;
        return nuspec
            .Descendants(ns + "dependency")
            .Select(element => (string?)element.Attribute("id"))
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Cast<string>()
            .ToArray();
    }

    private static string GetMetadataValue(XDocument nuspec, string elementName)
    {
        XNamespace ns = nuspec.Root!.Name.Namespace;
        var value = nuspec
            .Descendants(ns + elementName)
            .Select(element => element.Value)
            .FirstOrDefault();

        Assert.False(string.IsNullOrWhiteSpace(value), $"Could not find <{elementName}> in nuspec.");
        return value!;
    }

    private static string GetRepositoryUrl(XDocument nuspec)
    {
        XNamespace ns = nuspec.Root!.Name.Namespace;
        var repository = nuspec.Descendants(ns + "repository").FirstOrDefault();
        Assert.NotNull(repository);

        var url = (string?)repository!.Attribute("url");
        Assert.False(string.IsNullOrWhiteSpace(url), "Could not find repository url in nuspec.");
        return url!;
    }

    private static string GetRepositoryType(XDocument nuspec)
    {
        XNamespace ns = nuspec.Root!.Name.Namespace;
        var repository = nuspec.Descendants(ns + "repository").FirstOrDefault();
        Assert.NotNull(repository);

        var type = (string?)repository!.Attribute("type");
        Assert.False(string.IsNullOrWhiteSpace(type), "Could not find repository type in nuspec.");
        return type!;
    }

    private static string FindRepoRoot()
    {
        var start = new DirectoryInfo(Path.GetDirectoryName(typeof(JrocSdkPackageTests).Assembly.Location)!);
        DirectoryInfo? dir = start;

        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "jroc.sln")))
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

    private sealed record PackedPackage(HashSet<string> EntryNames, string ReadmeText, XDocument Nuspec);
}
