using System.Diagnostics;
using System.Security;

namespace Jroc.Tests;

public sealed record GeneratedAssemblyConsumerResult(
    int BuildExitCode,
    string BuildStandardOutput,
    string BuildStandardError,
    int? RunExitCode,
    string RunStandardOutput,
    string RunStandardError)
{
    public string BuildDiagnostics => BuildStandardOutput + Environment.NewLine + BuildStandardError;
}

public sealed class GeneratedAssemblyConsumerHarness : IDisposable
{
    private readonly JrocMaterializedAssembly _materializedAssembly;
    private int _disposed;

    public GeneratedAssemblyConsumerHarness(
        string javaScript,
        string assemblyName = "ConsumerFixture",
        IReadOnlyDictionary<string, string>? additionalScripts = null,
        string entryFileName = "entry.js")
    {
        ArgumentNullException.ThrowIfNull(javaScript);

        WorkingDirectory = Path.Combine(
            Path.GetTempPath(),
            "Jroc.Tests",
            "GeneratedAssemblyConsumer",
            Guid.NewGuid().ToString("N"));

        try
        {
            var sourceDirectory = Path.Combine(WorkingDirectory, "javascript");
            var entryPath = Path.Combine(sourceDirectory, entryFileName);
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(entryPath, javaScript);
            foreach (var (relativePath, source) in additionalScripts ?? new Dictionary<string, string>())
            {
                fileSystem.AddFile(Path.Combine(sourceDirectory, relativePath), source);
            }

            Artifact = JrocInMemoryCompiler.Compile(
                new JrocInMemoryCompileRequest(entryPath)
                {
                    AssemblyName = assemblyName,
                    FileSystem = fileSystem,
                    EmitPdb = true
                });
            _materializedAssembly = Artifact.Materialize(Path.Combine(WorkingDirectory, "generated"));
        }
        catch
        {
            DeleteWorkingDirectory();
            throw;
        }
    }

    public string WorkingDirectory { get; }

    public JrocCompiledAssemblyArtifact Artifact { get; }

    public GeneratedAssemblyConsumerResult Build(string consumerSource, bool run = false)
    {
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(consumerSource);

        var projectDirectory = Path.Combine(WorkingDirectory, "consumer");
        Directory.CreateDirectory(projectDirectory);
        var projectText = CreateProjectText(_materializedAssembly.AssemblyPath);
        AssertNoDirectRuntimeReference(consumerSource, projectText);

        File.WriteAllText(Path.Combine(projectDirectory, "Program.cs"), consumerSource);
        File.WriteAllText(Path.Combine(projectDirectory, "Consumer.csproj"), projectText);

        var build = RunProcess(
            "dotnet",
            ["build", "Consumer.csproj", "--nologo", "--verbosity:minimal"],
            projectDirectory);

        if (!run || build.ExitCode != 0)
        {
            return new GeneratedAssemblyConsumerResult(
                build.ExitCode,
                build.StandardOutput,
                build.StandardError,
                null,
                string.Empty,
                string.Empty);
        }

        var outputDirectory = Path.Combine(projectDirectory, "bin", "Debug", "net10.0");
        CopyRuntimeImplementation(outputDirectory);
        var runResult = RunProcess(
            "dotnet",
            [Path.Combine(outputDirectory, "Consumer.dll")],
            projectDirectory);

        return new GeneratedAssemblyConsumerResult(
            build.ExitCode,
            build.StandardOutput,
            build.StandardError,
            runResult.ExitCode,
            runResult.StandardOutput,
            runResult.StandardError);
    }

    public static void AssertNoDirectRuntimeReference(string consumerSource, string projectText)
    {
        ArgumentNullException.ThrowIfNull(consumerSource);
        ArgumentNullException.ThrowIfNull(projectText);

        foreach (var forbiddenReference in new[] { "Jroc.Runtime", "JavaScriptRuntime" })
        {
            if (consumerSource.Contains(forbiddenReference, StringComparison.Ordinal)
                || projectText.Contains(forbiddenReference, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Consumer source and project files must not directly reference '{forbiddenReference}'.");
            }
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        DeleteWorkingDirectory();
    }

    private static string CreateProjectText(string assemblyPath)
    {
        var escapedAssemblyPath = SecurityElement.Escape(assemblyPath)
            ?? throw new InvalidOperationException("Could not encode the generated assembly path.");

        return $$"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net10.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
                <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
              </PropertyGroup>
              <ItemGroup>
                <Reference Include="GeneratedJavaScript">
                  <HintPath>{{escapedAssemblyPath}}</HintPath>
                  <Private>true</Private>
                </Reference>
              </ItemGroup>
            </Project>
            """;
    }

    private static void CopyRuntimeImplementation(string outputDirectory)
    {
        var runtimePath = typeof(JavaScriptRuntime.ObjectRuntime).Assembly.Location;
        if (string.IsNullOrWhiteSpace(runtimePath) || !File.Exists(runtimePath))
        {
            throw new FileNotFoundException("Could not locate JavaScriptRuntime.dll.", runtimePath);
        }

        File.Copy(
            runtimePath,
            Path.Combine(outputDirectory, Path.GetFileName(runtimePath)),
            overwrite: true);
    }

    private static ProcessResult RunProcess(
        string fileName,
        IReadOnlyList<string> arguments,
        string workingDirectory)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Could not start '{fileName}'.");
        var standardOutput = process.StandardOutput.ReadToEndAsync();
        var standardError = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(120_000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException($"Process '{fileName}' timed out.");
        }

        return new ProcessResult(
            process.ExitCode,
            standardOutput.GetAwaiter().GetResult(),
            standardError.GetAwaiter().GetResult());
    }

    private void DeleteWorkingDirectory()
    {
        if (!Directory.Exists(WorkingDirectory))
        {
            return;
        }

        try
        {
            Directory.Delete(WorkingDirectory, recursive: true);
        }
        catch (IOException)
        {
            Thread.Sleep(100);
            Directory.Delete(WorkingDirectory, recursive: true);
        }
    }

    private sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);
}
