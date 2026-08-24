using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;

namespace Jroc.Tests;

[Collection("RepoDotnetCli")]
public class JrocSdkPackageTests
{
    [Fact]
    public void Pack_JrocSdk_ContainsBuildAssetsSamplesAndRuntimeDependency()
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
            Assert.Contains("samples/Basic/host/Basic.csproj", entryNames);
            Assert.Contains("samples/Basic/compiler/JavaScript/HostedMathModule.js", entryNames);
            Assert.DoesNotContain("samples/Basic/compiler/HostedMathModule.proj", entryNames);
            Assert.Contains("samples/Typed/host/Typed.csproj", entryNames);
            Assert.Contains("samples/Typed/compiler/JavaScript/HostedCounterModule.js", entryNames);
            Assert.DoesNotContain("samples/Typed/compiler/HostedCounterModule.proj", entryNames);
            Assert.Contains("samples/Picocolors/Picocolors.csproj", entryNames);
            Assert.Contains("samples/Picocolors/Program.cs", entryNames);
            Assert.Contains("samples/NpmRunAll2/NpmRunAll2.csproj", entryNames);
            Assert.Contains("samples/NpmRunAll2/Program.cs", entryNames);
            Assert.Contains("samples/NpmRunAll2/index.js", entryNames);
            Assert.Contains("samples/Domino/Domino.csproj", entryNames);
            Assert.Contains("samples/Domino/Program.cs", entryNames);
            Assert.Contains("samples/Domino/sample.html", entryNames);
            Assert.Contains("samples/Domino/package.json", entryNames);
            Assert.Contains("samples/Domino/package-lock.json", entryNames);
            Assert.DoesNotContain("samples/Domino/compiler/package.json", entryNames);
            Assert.DoesNotContain("samples/Domino/compiler/package-lock.json", entryNames);
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

            Assert.Contains("Jroc.Runtime", dependencyIds, StringComparer.Ordinal);
            Assert.DoesNotContain("Jroc.Core", dependencyIds, StringComparer.Ordinal);
            Assert.DoesNotContain("jroc", dependencyIds, StringComparer.OrdinalIgnoreCase);
            Assert.DoesNotContain("<PackageReference Include=\"Jroc.Runtime\"", package.ReadmeText, StringComparison.Ordinal);
            Assert.Contains("restores and deploys the compatible runtime automatically", package.ReadmeText, StringComparison.Ordinal);

            using var archive = ZipFile.OpenRead(sdkPackagePath);
            var targetsEntry = archive.GetEntry("build/Jroc.SDK.targets");
            Assert.NotNull(targetsEntry);
            using var targetsReader = new StreamReader(targetsEntry!.Open());
            var targetsText = targetsReader.ReadToEnd();
            Assert.Contains("JrocCompile", targetsText, StringComparison.Ordinal);
            Assert.Contains("ModuleResolutionBaseDirectory", targetsText, StringComparison.Ordinal);
            Assert.Contains("ReferenceOutputAssembly", targetsText, StringComparison.Ordinal);
            Assert.Contains("RootModuleId", targetsText, StringComparison.Ordinal);

            var dominoSampleEntry = archive.GetEntry("samples/Domino/Domino.csproj");
            Assert.NotNull(dominoSampleEntry);
            using var dominoSampleReader = new StreamReader(dominoSampleEntry!.Open());
            var dominoSampleText = dominoSampleReader.ReadToEnd();
            Assert.Contains("<JrocCompile Include=\"@mixmark-io/domino\"", dominoSampleText, StringComparison.Ordinal);
            Assert.DoesNotContain("DominoCompilerDir", dominoSampleText, StringComparison.Ordinal);
            Assert.DoesNotContain("JrocModuleResolutionBaseDirectory", dominoSampleText, StringComparison.Ordinal);
            Assert.DoesNotContain("ModuleResolutionBaseDirectory=", dominoSampleText, StringComparison.Ordinal);
            Assert.DoesNotContain("node_modules','@mixmark-io','domino','lib','index.js", dominoSampleText, StringComparison.Ordinal);
            Assert.DoesNotContain("PackageReference Include=\"Jroc.Runtime\"", dominoSampleText, StringComparison.Ordinal);
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
    public void Build_WithLocalJrocSdkPackage_CompilesRunsAndPublishesWithoutDirectRuntimeReference()
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
            var generatedAssemblyPath = Path.Combine(generatedDir, "HostedMathAssembly.dll");
            Assert.True(File.Exists(generatedAssemblyPath), $"Missing generated module dll in '{generatedDir}'.");
            Assert.True(File.Exists(Path.Combine(generatedDir, "HostedMathAssembly.runtimeconfig.json")), $"Missing generated runtimeconfig in '{generatedDir}'.");
            Assert.Equal("HostedMathAssembly", AssemblyName.GetAssemblyName(generatedAssemblyPath).Name);

            var targetDir = Path.Combine(projectDir, "bin", "Debug", "net10.0");
            Assert.True(File.Exists(Path.Combine(targetDir, "HostedMathAssembly.dll")), $"Missing referenced module dll in '{targetDir}'.");
            Assert.True(File.Exists(Path.Combine(targetDir, "JavaScriptRuntime.dll")), $"Missing transitive runtime dll in '{targetDir}'.");
            Assert.False(File.Exists(Path.Combine(targetDir, "Jroc.Compiler.dll")), $"Compiler implementation leaked into '{targetDir}'.");

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

            var publishDir = Path.Combine(projectDir, "publish");
            var publish = RunProcess(
                fileName: "dotnet",
                arguments: $"publish Consumer.csproj -c Debug --no-build --nologo -o \"{publishDir}\"",
                workingDirectory: projectDir,
                timeoutSeconds: 180);

            Assert.True(
                publish.ExitCode == 0,
                $"dotnet publish failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{publish.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{publish.StdErr}");
            Assert.True(File.Exists(Path.Combine(publishDir, "HostedMathAssembly.dll")));
            Assert.True(File.Exists(Path.Combine(publishDir, "HostedMathAssembly.pdb")));
            Assert.True(File.Exists(Path.Combine(publishDir, "JavaScriptRuntime.dll")));
            Assert.False(File.Exists(Path.Combine(publishDir, "Jroc.Compiler.dll")));

            var publishedRun = RunProcess(
                fileName: Path.Combine(publishDir, "Consumer"),
                arguments: string.Empty,
                workingDirectory: publishDir,
                timeoutSeconds: 180);
            Assert.True(
                publishedRun.ExitCode == 0,
                $"published consumer failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{publishedRun.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{publishedRun.StdErr}");
            Assert.Contains("sum=3", publishedRun.StdOut, StringComparison.Ordinal);

            using var assets = JsonDocument.Parse(File.ReadAllText(Path.Combine(projectDir, "obj", "project.assets.json")));
            var libraries = assets.RootElement.GetProperty("libraries").EnumerateObject().Select(property => property.Name).ToArray();
            Assert.Contains(libraries, name => name.StartsWith("Jroc.SDK/", StringComparison.Ordinal));
            Assert.Contains(libraries, name => name.StartsWith("Jroc.Runtime/", StringComparison.Ordinal));
            Assert.DoesNotContain(libraries, name => name.StartsWith("Jroc.Core/", StringComparison.Ordinal));
            var directDependencies = assets.RootElement
                .GetProperty("project")
                .GetProperty("frameworks")
                .GetProperty("net10.0")
                .GetProperty("dependencies");
            Assert.True(directDependencies.TryGetProperty("Jroc.SDK", out _));
            Assert.False(directDependencies.TryGetProperty("Jroc.Runtime", out _));
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }

    [Fact]
    public void Build_WithMultipleJrocAssemblies_UsesOneTransitiveRuntimeForRunAndPublish()
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
            WriteMultipleAssemblyConsumerProject(projectDir, feedDir, packageVersion);

            var publishDir = Path.Combine(projectDir, "publish");
            var publish = RunProcess(
                fileName: "dotnet",
                arguments: $"publish Consumer.csproj -c Release --nologo --ignore-failed-sources -o \"{publishDir}\"",
                workingDirectory: projectDir,
                timeoutSeconds: 180);

            Assert.True(
                publish.ExitCode == 0,
                $"dotnet publish failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{publish.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{publish.StdErr}");

            Assert.True(File.Exists(Path.Combine(publishDir, "FirstModule.dll")));
            Assert.True(File.Exists(Path.Combine(publishDir, "SecondModule.dll")));
            Assert.Single(Directory.GetFiles(publishDir, "JavaScriptRuntime.dll"));
            Assert.False(File.Exists(Path.Combine(publishDir, "Jroc.Compiler.dll")));

            var run = RunProcess(
                fileName: Path.Combine(publishDir, "Consumer"),
                arguments: string.Empty,
                workingDirectory: publishDir,
                timeoutSeconds: 180);
            Assert.True(
                run.ExitCode == 0,
                $"published consumer failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{run.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{run.StdErr}");
            Assert.Contains("total=42", run.StdOut, StringComparison.Ordinal);
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }

    [Fact]
    public void Build_WithMissingOrIncompatibleRuntimeAsset_ReportsActionableSdkDiagnostic()
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
            WriteDiagnosticConsumerProject(projectDir, feedDir, packageVersion);

            var restore = RunProcess(
                fileName: "dotnet",
                arguments: "restore Consumer.csproj --nologo --ignore-failed-sources",
                workingDirectory: projectDir,
                timeoutSeconds: 180);
            Assert.True(restore.ExitCode == 0, restore.StdOut + Environment.NewLine + restore.StdErr);

            var runtimePath = Directory.GetFiles(
                    Path.Combine(projectDir, "packages", "jroc.runtime"),
                    "JavaScriptRuntime.dll",
                    SearchOption.AllDirectories)
                .Single(path => path.Contains($"{Path.DirectorySeparatorChar}lib{Path.DirectorySeparatorChar}net10.0{Path.DirectorySeparatorChar}", StringComparison.Ordinal));
            var compatibleRuntime = File.ReadAllBytes(runtimePath);
            File.Delete(runtimePath);

            var missingBuild = RunProcess(
                fileName: "dotnet",
                arguments: "build Consumer.csproj --no-restore --nologo",
                workingDirectory: projectDir,
                timeoutSeconds: 180);
            Assert.NotEqual(0, missingBuild.ExitCode);
            Assert.Contains("JROCSDK1001", missingBuild.StdOut + missingBuild.StdErr, StringComparison.Ordinal);
            Assert.Contains("no compatible runtime reference was resolved", missingBuild.StdOut + missingBuild.StdErr, StringComparison.OrdinalIgnoreCase);

            File.WriteAllBytes(runtimePath, compatibleRuntime);
            var incompatibleDir = Path.Combine(tempRoot, "incompatible-runtime");
            Directory.CreateDirectory(incompatibleDir);
            File.WriteAllText(
                Path.Combine(incompatibleDir, "IncompatibleRuntime.csproj"),
                """
                    <Project Sdk="Microsoft.NET.Sdk">
                      <PropertyGroup>
                        <TargetFramework>net10.0</TargetFramework>
                        <AssemblyName>JavaScriptRuntime</AssemblyName>
                        <AssemblyVersion>99.0.0.0</AssemblyVersion>
                      </PropertyGroup>
                    </Project>
                    """);
            File.WriteAllText(Path.Combine(incompatibleDir, "Placeholder.cs"), "public sealed class Placeholder;");
            var incompatibleBuild = RunProcess(
                fileName: "dotnet",
                arguments: "build IncompatibleRuntime.csproj -c Release --nologo",
                workingDirectory: incompatibleDir,
                timeoutSeconds: 180);
            Assert.True(
                incompatibleBuild.ExitCode == 0,
                $"incompatible runtime build failed.{Environment.NewLine}{incompatibleBuild.StdOut}{Environment.NewLine}{incompatibleBuild.StdErr}");
            File.Copy(
                Path.Combine(incompatibleDir, "bin", "Release", "net10.0", "JavaScriptRuntime.dll"),
                runtimePath,
                overwrite: true);

            var mismatchedBuild = RunProcess(
                fileName: "dotnet",
                arguments: "build Consumer.csproj --no-restore --nologo",
                workingDirectory: projectDir,
                timeoutSeconds: 180);
            Assert.NotEqual(0, mismatchedBuild.ExitCode);
            Assert.Contains("JROCSDK1002", mismatchedBuild.StdOut + mismatchedBuild.StdErr, StringComparison.Ordinal);
            Assert.Contains("Align the Jroc.SDK and Jroc.Runtime package versions", mismatchedBuild.StdOut + mismatchedBuild.StdErr, StringComparison.Ordinal);
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
            var generatedAssemblyPath = Path.Combine(generatedDir, "scope.pkg.dll");
            Assert.True(File.Exists(generatedAssemblyPath), $"Missing generated package module dll in '{generatedDir}'.");
            Assert.True(File.Exists(Path.Combine(generatedDir, "scope.pkg.runtimeconfig.json")), $"Missing generated package runtimeconfig in '{generatedDir}'.");
            Assert.Equal("scope.pkg", AssemblyName.GetAssemblyName(generatedAssemblyPath).Name);

            var targetDir = Path.Combine(projectDir, "bin", "Debug", "net10.0");
            Assert.True(File.Exists(Path.Combine(targetDir, "scope.pkg.dll")), $"Missing copied package module dll in '{targetDir}'.");

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
    public void Build_ExtractedBasicSample_WithLocalJrocSdkPackage_CompilesAndRuns()
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

            var hostDir = Path.Combine(extractDir, "samples", "Basic", "host");
            var build = RunProcess(
                fileName: "dotnet",
                arguments: $"build Basic.csproj -c Release --nologo --ignore-failed-sources -p:JrocPackageVersion={packageVersion}",
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
                arguments: $"run --project Basic.csproj -c Release --no-build --nologo -p:JrocPackageVersion={packageVersion}",
                workingDirectory: hostDir,
                timeoutSeconds: 180);

            Assert.True(
                run.ExitCode == 0,
                $"dotnet run failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{run.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{run.StdErr}");

            var output = run.StdOut.Replace("\r", string.Empty, StringComparison.Ordinal);
            Assert.Contains("version=1.0.0\n", output, StringComparison.Ordinal);
            Assert.Contains("1+2=3\n", output, StringComparison.Ordinal);
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }

    [Fact]
    public void Build_ExtractedTypedSample_WithLocalJrocSdkPackage_CompilesAndRuns()
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

            var hostDir = Path.Combine(extractDir, "samples", "Typed", "host");
            var build = RunProcess(
                fileName: "dotnet",
                arguments: $"build Typed.csproj -c Release --nologo --ignore-failed-sources -p:JrocPackageVersion={packageVersion}",
                workingDirectory: hostDir,
                timeoutSeconds: 180);

            Assert.True(
                build.ExitCode == 0,
                $"dotnet build failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{build.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{build.StdErr}");

            var generatedDir = Path.Combine(hostDir, "obj", "Release", "net10.0", "jroc", "HostedCounterModule");
            Assert.True(File.Exists(Path.Combine(generatedDir, "HostedCounterModule.dll")), $"Missing generated module dll in '{generatedDir}'.");

            var run = RunProcess(
                fileName: "dotnet",
                arguments: $"run --project Typed.csproj -c Release --no-build --nologo -p:JrocPackageVersion={packageVersion}",
                workingDirectory: hostDir,
                timeoutSeconds: 180);

            Assert.True(
                run.ExitCode == 0,
                $"dotnet run failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{run.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{run.StdErr}");

            var output = run.StdOut.Replace("\r", string.Empty, StringComparison.Ordinal);
            Assert.Contains("version=1.2.3\n", output, StringComparison.Ordinal);
            Assert.Contains("add(1,2)=3\n", output, StringComparison.Ordinal);
            Assert.Contains("counter.add(5)=15\n", output, StringComparison.Ordinal);
            Assert.Contains("counter.value=15\n", output, StringComparison.Ordinal);
            Assert.Contains("addAsync(1,2)=3\n", output, StringComparison.Ordinal);
            Assert.Contains("created.add(1)=3\n", output, StringComparison.Ordinal);
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }

    [Fact]
    public void Build_ExtractedNpmSamples_WithLocalJrocSdkPackage_CompileAndRun()
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

            var samples = new[]
            {
                (
                    Directory: Path.Combine(extractDir, "samples", "Picocolors"),
                    Project: "Picocolors.csproj",
                    Expected: new[] { "red=", "green=", "yellow=", "cyan=", "bold=", "done" }),
                (
                    Directory: Path.Combine(extractDir, "samples", "NpmRunAll2"),
                    Project: "NpmRunAll2.csproj",
                    Expected: new[] { "=== task headers ===", "> build", "=== pattern matching ===", "test:unit,test:integration,test:e2e", "done" }),
                (
                    Directory: Path.Combine(extractDir, "samples", "Domino"),
                    Project: "Domino.csproj",
                    Expected: new[] { "title=JROC Domino Sample", "elements=12", "links=2" })
            };

            foreach (var sample in samples)
            {
                var build = RunProcess(
                    fileName: "dotnet",
                    arguments: $"build {sample.Project} -c Release --nologo --ignore-failed-sources -p:JrocPackageVersion={packageVersion}",
                    workingDirectory: sample.Directory,
                    timeoutSeconds: 600);

                Assert.True(
                    build.ExitCode == 0,
                    $"{sample.Project} build failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{build.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{build.StdErr}");

                var run = RunProcess(
                    fileName: "dotnet",
                    arguments: $"run --project {sample.Project} -c Release --no-build --nologo -p:JrocPackageVersion={packageVersion}",
                    workingDirectory: sample.Directory,
                    timeoutSeconds: 180);

                Assert.True(
                    run.ExitCode == 0,
                    $"{sample.Project} run failed.{Environment.NewLine}STDOUT:{Environment.NewLine}{run.StdOut}{Environment.NewLine}STDERR:{Environment.NewLine}{run.StdErr}");

                foreach (var expected in sample.Expected)
                {
                    Assert.Contains(expected, run.StdOut, StringComparison.Ordinal);
                }
            }

            var dominoDirectory = Path.Combine(extractDir, "samples", "Domino");
            var missingHtmlPath = Path.Combine(dominoDirectory, "missing-sample.html");
            var diagnosticRun = RunProcess(
                fileName: "dotnet",
                arguments: $"run --project Domino.csproj -c Release --no-build --nologo -p:JrocPackageVersion={packageVersion}",
                workingDirectory: dominoDirectory,
                timeoutSeconds: 180,
                environmentVariables: new Dictionary<string, string?>
                {
                    ["JROC_DOMINO_DIAG"] = "1",
                    ["JROC_DOMINO_HTML_PATH"] = missingHtmlPath
                });
            Assert.NotEqual(0, diagnosticRun.ExitCode);
            Assert.Contains("[diag] Domino failure", diagnosticRun.StdOut, StringComparison.Ordinal);
            Assert.Contains("missing-sample.html", diagnosticRun.StdOut + diagnosticRun.StdErr, StringComparison.Ordinal);
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { /* ignore */ }
        }
    }

    [Fact]
    public void ShippingSampleSources_UseSupportedFacadePatterns()
    {
        var repoRoot = FindRepoRoot();
        var basic = File.ReadAllText(Path.Combine(repoRoot, "samples", "Basic", "host", "Program.cs"));
        var typed = File.ReadAllText(Path.Combine(repoRoot, "samples", "Typed", "host", "Program.cs"));
        var picocolors = File.ReadAllText(Path.Combine(repoRoot, "samples", "Picocolors", "Program.cs"));
        var npmRunAll2 = File.ReadAllText(Path.Combine(repoRoot, "samples", "NpmRunAll2", "Program.cs"));
        var domino = File.ReadAllText(Path.Combine(repoRoot, "samples", "Domino", "Program.cs"));

        Assert.Contains("HostedMathModule.Import()", basic, StringComparison.Ordinal);
        Assert.Contains("HostedCounterModule.Import()", typed, StringComparison.Ordinal);
        Assert.Contains("await exports.AddAsync", typed, StringComparison.Ordinal);
        Assert.Contains("exports.Counter.Construct", typed, StringComparison.Ordinal);
        Assert.Contains("picocolors.Import()", picocolors, StringComparison.Ordinal);
        Assert.Contains("mixmark_io_domino.Import()", domino, StringComparison.Ordinal);

        foreach (var source in new[] { basic, typed, picocolors, npmRunAll2, domino })
        {
            Assert.DoesNotContain("JsEngine", source, StringComparison.Ordinal);
            Assert.DoesNotContain("Jroc.Runtime", source, StringComparison.Ordinal);
            Assert.DoesNotContain("dynamic", source, StringComparison.Ordinal);
            Assert.DoesNotContain("Convert.", source, StringComparison.Ordinal);
            Assert.DoesNotContain("Assembly.Load", source, StringComparison.Ordinal);
        }

        Assert.Contains("NpmRunAll2Module.Import()", npmRunAll2, StringComparison.Ordinal);
        Assert.False(File.Exists(Path.Combine(repoRoot, "samples", "Picocolors", "index.js")));
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
                <JrocCompile Include="JavaScript\HostedMathModule.js"
                              OutputDirectory="$(BaseIntermediateOutputPath)\jroc-custom\HostedMathModule"
                              AssemblyName="HostedMathAssembly"
                              RootModuleId="sample.math" />
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

            var moduleIds = JsEngine.GetModuleIds(typeof(HostedMathAssembly).Assembly);
            Console.WriteLine($"hasModuleId={moduleIds.Contains("sample.math", StringComparer.Ordinal)}");

            using var exports = HostedMathAssembly.Import();
            Console.WriteLine($"version={exports.Version}");
            Console.WriteLine($"sum={exports.Add(1, 2)}");
            """);
    }

    private static void WriteMultipleAssemblyConsumerProject(string projectDir, string feedDir, string packageVersion)
    {
        WriteNuGetConfig(projectDir, feedDir);
        Directory.CreateDirectory(Path.Combine(projectDir, "JavaScript"));

        File.WriteAllText(
            Path.Combine(projectDir, "Consumer.csproj"),
            $$"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net10.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="Jroc.SDK" Version="{{packageVersion}}" />
                <JrocCompile Include="JavaScript\first.js" AssemblyName="FirstModule" />
                <JrocCompile Include="JavaScript\second.js" AssemblyName="SecondModule" />
              </ItemGroup>
            </Project>
            """);
        File.WriteAllText(Path.Combine(projectDir, "JavaScript", "first.js"), "module.exports = { value: 20 };");
        File.WriteAllText(Path.Combine(projectDir, "JavaScript", "second.js"), "module.exports = { value: 22 };");
        File.WriteAllText(
            Path.Combine(projectDir, "Program.cs"),
            """
            using var first = FirstModule.Import();
            using var second = SecondModule.Import();
            Console.WriteLine($"total={first.Value + second.Value}");
            """);
    }

    private static void WriteDiagnosticConsumerProject(string projectDir, string feedDir, string packageVersion)
    {
        WriteNuGetConfig(projectDir, feedDir);
        Directory.CreateDirectory(Path.Combine(projectDir, "JavaScript"));

        File.WriteAllText(
            Path.Combine(projectDir, "Consumer.csproj"),
            $$"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net10.0</TargetFramework>
                <RestorePackagesPath>$(MSBuildProjectDirectory)\packages</RestorePackagesPath>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="Jroc.SDK" Version="{{packageVersion}}" />
                <JrocCompile Include="JavaScript\module.js" AssemblyName="DiagnosticModule" />
              </ItemGroup>
            </Project>
            """);
        File.WriteAllText(Path.Combine(projectDir, "JavaScript", "module.js"), "module.exports = { value: 42 };");
        File.WriteAllText(Path.Combine(projectDir, "Program.cs"), "using var exports = DiagnosticModule.Import();");
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

            var compiledModulePath = Path.Combine(AppContext.BaseDirectory, "scope.pkg.dll");
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

    private static (int ExitCode, string StdOut, string StdErr) RunProcess(
        string fileName,
        string arguments,
        string workingDirectory,
        int timeoutSeconds,
        IReadOnlyDictionary<string, string?>? environmentVariables = null)
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
        psi.Environment["DOTNET_CLI_USE_MSBUILD_SERVER"] = "0";
        psi.Environment["MSBUILDDISABLENODEREUSE"] = "1";
        psi.Environment["UseSharedCompilation"] = "false";
        if (environmentVariables != null)
        {
            foreach (var (name, value) in environmentVariables)
            {
                psi.Environment[name] = value;
            }
        }

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
