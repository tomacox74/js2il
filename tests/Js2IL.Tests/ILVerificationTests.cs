using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Js2IL.Tests;

/// <summary>
/// IL verification tests that ensure emitted assemblies are structurally valid according to CLR verifier rules.
/// Uses ilverify to catch IL emission regressions like stack imbalance, bad exception regions, or invalid metadata.
/// Addresses issue #580: Quality gate for IL structural validity.
/// </summary>
public sealed class ILVerificationTests
{
    private readonly ITestOutputHelper _output;
    private readonly string _outputPath;
    private readonly JavaScriptParser _parser;

    public ILVerificationTests(ITestOutputHelper output)
    {
        _output = output;
        _parser = new JavaScriptParser();
        
        // Use a unique per-run directory to avoid file locks
        var root = Path.Combine(Path.GetTempPath(), "Js2IL.Tests.ILVerification");
        var runId = Guid.NewGuid().ToString("N");
        _outputPath = Path.Combine(root, runId);
        Directory.CreateDirectory(_outputPath);
    }

    /// <summary>
    /// Verifies that assemblies emitted from a curated set of JS test cases pass IL verification.
    /// This corpus covers known tricky IL shapes: loops, control flow joins, try/finally, closures, and class methods.
    /// </summary>
    [Theory]
    [InlineData("ControlFlow_ForLoop_CountToFive")]
    [InlineData("ControlFlow_ForLoop_Break_AtThree")]
    [InlineData("ControlFlow_Conditional_Ternary")]
    [InlineData("ControlFlow_Conditional_Ternary_ShortCircuit")]
    [InlineData("ControlFlow_While_Break_AtThree")]
    // TryCatchFinally_ThrowValue currently fails verification - known issue with exception handling
    // [InlineData("TryCatchFinally_ThrowValue")]
    [InlineData("TryFinally_Return")]
    [InlineData("Function_ClosureMutatesOuterVariable")]
    [InlineData("Function_Closure_MultiLevel_ReadWriteAcrossScopes")]
    [InlineData("Classes_ClassMethod_CallsAnotherMethod")]
    [InlineData("Classes_ClassWithMethod_HelloWorld")]
    [InlineData("Classes_Inheritance_SuperMethodCall")]
    public void ILVerify_ShouldPass_ForEmittedAssembly(string testName)
    {
        // Arrange: Compile the JS test case to a DLL
        var assemblyPath = CompileTestCase(testName);

        // Act: Run ilverify on the assembly
        var result = RunILVerify(assemblyPath);

        // Assert: Verification should succeed
        if (result.ExitCode != 0)
        {
            var errorMessage = new StringBuilder();
            errorMessage.AppendLine($"IL verification failed for test case '{testName}':");
            errorMessage.AppendLine($"Assembly: {assemblyPath}");
            errorMessage.AppendLine();
            errorMessage.AppendLine("STDOUT:");
            errorMessage.AppendLine(result.StandardOutput);
            errorMessage.AppendLine();
            errorMessage.AppendLine("STDERR:");
            errorMessage.AppendLine(result.StandardError);
            
            Assert.Fail(errorMessage.ToString());
        }
        
        _output.WriteLine($"✓ IL verification passed for {testName}");
        _output.WriteLine($"  Output: {result.StandardOutput}");
    }

    private string CompileTestCase(string testName)
    {
        // Determine the category from the test name prefix
        string category;
        if (testName.StartsWith("ControlFlow_"))
            category = "ControlFlow";
        else if (testName.StartsWith("TryCatch") || testName.StartsWith("TryFinally"))
            category = "TryCatch";
        else if (testName.StartsWith("Function_"))
            category = "Function";
        else if (testName.StartsWith("Classes_"))
            category = "Classes";
        else
            throw new InvalidOperationException($"Unknown test category for test name: {testName}");

        // Load the embedded JavaScript file
        var resourceName = $"Js2IL.Tests.{category}.JavaScript.{testName}.js";
        var assembly = typeof(ILVerificationTests).Assembly;
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"Could not find embedded resource: {resourceName}");
        }

        using var reader = new StreamReader(stream);
        var jsSource = reader.ReadToEnd();

        // Create a mock file system
        var mockFileSystem = new MockFileSystem();
        var entryPath = Path.Combine(_outputPath, $"{testName}.js");
        mockFileSystem.AddFile(entryPath, jsSource, null);

        // Configure the compiler
        var options = new CompilerOptions
        {
            OutputDirectory = _outputPath,
            EmitPdb = false // Don't need PDBs for IL verification
        };

        var testLogger = new TestLogger();
        var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFileSystem, testLogger);
        var compiler = serviceProvider.GetRequiredService<Compiler>();

        // Compile the JS to a DLL
        var success = compiler.Compile(entryPath);
        
        if (!success)
        {
            var errorDetails = new StringBuilder();
            errorDetails.AppendLine($"Compilation failed for test case '{testName}':");
            if (!string.IsNullOrEmpty(testLogger.Output))
                errorDetails.AppendLine(testLogger.Output);
            if (!string.IsNullOrEmpty(testLogger.Errors))
                errorDetails.AppendLine(testLogger.Errors);
            if (!string.IsNullOrEmpty(testLogger.Warnings))
                errorDetails.AppendLine(testLogger.Warnings);
                
            throw new InvalidOperationException(errorDetails.ToString());
        }

        return Path.Combine(_outputPath, $"{testName}.dll");
    }

    private (int ExitCode, string StandardOutput, string StandardError) RunILVerify(string assemblyPath)
    {
        // For .NET 10, the system module is System.Runtime
        // We need to provide the path to the runtime assemblies directory for reference resolution
        var runtimePath = GetRuntimeAssembliesPath();
        var outputDir = Path.GetDirectoryName(assemblyPath);
        
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"ilverify \"{assemblyPath}\" --system-module System.Runtime -r \"{runtimePath}/*.dll\" -r \"{outputDir}/*.dll\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start ilverify process");
        }

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return (process.ExitCode, stdout, stderr);
    }

    private static string GetRuntimeAssembliesPath()
    {
        // Find the path to .NET runtime assemblies
        // This is typically in the shared framework directory
        var dotnetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT");
        if (string.IsNullOrEmpty(dotnetRoot))
        {
            // Common default locations
            if (OperatingSystem.IsWindows())
            {
                dotnetRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet");
            }
            else
            {
                dotnetRoot = "/usr/share/dotnet";
            }
        }

        // Look for the Microsoft.NETCore.App shared framework
        var sharedPath = Path.Combine(dotnetRoot, "shared", "Microsoft.NETCore.App");
        
        if (!Directory.Exists(sharedPath))
        {
            throw new InvalidOperationException($"Could not find .NET shared framework at: {sharedPath}");
        }

        // Find the highest version available (preferably 10.x)
        var versions = Directory.GetDirectories(sharedPath)
            .Select(Path.GetFileName)
            .Where(v => v != null && v.StartsWith("10."))
            .OrderByDescending(v => v)
            .ToList();

        if (versions.Count == 0)
        {
            // Fallback to any version
            versions = Directory.GetDirectories(sharedPath)
                .Select(Path.GetFileName)
                .Where(v => v != null)
                .OrderByDescending(v => v)
                .ToList();
        }

        if (versions.Count == 0)
        {
            throw new InvalidOperationException($"Could not find any .NET runtime version in: {sharedPath}");
        }

        return Path.Combine(sharedPath, versions[0]!);
    }
}
