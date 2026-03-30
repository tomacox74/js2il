using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Js2IL.Tests.CommonJS;

public class NodeModulesExecutionTests
{
    private readonly VerifySettings _verifySettings = new();

    public NodeModulesExecutionTests()
    {
        _verifySettings.DisableDiff();
    }

    [Fact]
    public Task CommonJS_Require_NodeModules_PackageJson_Exports_And_NestedDependency()
    {
        var output = CompileAndRunTempProject();
        return VerifyWithSnapshot(output);
    }

    [Fact]
    public Task CommonJS_Require_NodeModules_DualMode_Exports_Imports_TypeModule_And_MjsEntry()
    {
        using var project = NodeModulesTestProjectSupport.CreateDualModeExportsImportsProject();
        var compiled = NodeModulesTestProjectSupport.Compile(project);
        if (!compiled.Success || string.IsNullOrWhiteSpace(compiled.AssemblyPath))
        {
            throw new InvalidOperationException("Compilation failed.\nErrors:\n" + compiled.Logger.Errors);
        }

        var output = NodeModulesTestProjectSupport.ExecuteGeneratedAssembly(compiled.AssemblyPath);
        return VerifyWithSnapshot(output);
    }

    [Fact]
    public void CommonJS_Require_NodeModules_UnsupportedConditions_ReportDiagnostic()
    {
        using var project = NodeModulesTestProjectSupport.CreateUnsupportedExportsConditionsProject();
        var compiled = NodeModulesTestProjectSupport.Compile(project);

        Assert.False(compiled.Success);
        Assert.Contains("Unsupported package.json exports conditions", compiled.Logger.Errors);
        Assert.Contains("Supported conditions: import, require, node, default", compiled.Logger.Errors);
    }

    private string CompileAndRunTempProject()
    {
        var root = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "Issue783", Guid.NewGuid().ToString("N"));
        var projectRoot = Path.Combine(root, "proj");
        var outputDir = Path.Combine(root, "out");

        Directory.CreateDirectory(projectRoot);
        Directory.CreateDirectory(outputDir);

        var entryPath = Path.Combine(projectRoot, "src", "main.js");
        Directory.CreateDirectory(Path.GetDirectoryName(entryPath)!);

        // Project entry: require a bare package id and an exports subpath.
        File.WriteAllText(entryPath,
            "\"use strict\";\n" +
            "const pkg1 = require('pkg');\n" +
            "const pkg2 = require('pkg');\n" +
            "console.log('pkg.name', pkg1.name);\n" +
            "console.log('pkg.execCount', pkg1.execCount);\n" +
            "console.log('pkg.depValue', pkg1.depValue);\n" +
            "console.log('same', pkg1 === pkg2);\n" +
            "const feature = require('pkg/feature');\n" +
            "console.log('feature', feature);\n");

        var pkgRoot = Path.Combine(projectRoot, "node_modules", "pkg");
        Directory.CreateDirectory(Path.Combine(pkgRoot, "lib"));

        File.WriteAllText(
            Path.Combine(pkgRoot, "package.json"),
            "{\"name\":\"pkg\",\"main\":\"lib/index.js\",\"exports\":{\".\":\"./lib/index.js\",\"./feature\":\"./dist/feature.js\"}}"
        );

        File.WriteAllText(
            Path.Combine(pkgRoot, "lib", "index.js"),
            "\"use strict\";\n" +
            "const dep = require('dep');\n" +
            "globalThis.__pkgExecCount = (globalThis.__pkgExecCount || 0) + 1;\n" +
            "module.exports = { name: 'pkg', execCount: globalThis.__pkgExecCount, depValue: dep.value };\n"
        );

        Directory.CreateDirectory(Path.Combine(pkgRoot, "dist"));
        File.WriteAllText(
            Path.Combine(pkgRoot, "dist", "feature.js"),
            "\"use strict\";\n" +
            "module.exports = 'feature-value';\n"
        );

        var depRoot = Path.Combine(projectRoot, "node_modules", "dep");
        Directory.CreateDirectory(depRoot);
        File.WriteAllText(
            Path.Combine(depRoot, "package.json"),
            "{\"name\":\"dep\",\"main\":\"index.js\"}"
        );
        File.WriteAllText(
            Path.Combine(depRoot, "index.js"),
            "\"use strict\";\n" +
            "module.exports = { value: 'dep-value' };\n"
        );

        var options = new CompilerOptions
        {
            OutputDirectory = outputDir,
            EmitPdb = true
        };

        try
        {
            var testLogger = new TestLogger();
            using var serviceProvider = CompilerServices.BuildServiceProvider(options, fileSystem: new FileSystem(), compilerOutput: testLogger);
            var compiler = serviceProvider.GetRequiredService<Compiler>();

            if (!compiler.Compile(entryPath))
            {
                var details = string.IsNullOrWhiteSpace(testLogger.Errors) ? string.Empty : "\nErrors:\n" + testLogger.Errors;
                var warnings = string.IsNullOrWhiteSpace(testLogger.Warnings) ? string.Empty : "\nWarnings:\n" + testLogger.Warnings;
                throw new InvalidOperationException("Compilation failed." + details + warnings);
            }

            var assemblyName = Path.GetFileNameWithoutExtension(entryPath) + ".dll";
            var assemblyPath = Path.Combine(outputDir, assemblyName);
            if (!File.Exists(assemblyPath))
            {
                throw new InvalidOperationException($"Expected compiled assembly not found at '{assemblyPath}'.");
            }

            return ExecuteGeneratedAssembly(assemblyPath);
        }
        finally
        {
            try { Directory.Delete(root, recursive: true); } catch { /* ignore cleanup errors */ }
        }
    }

    private static string ExecuteGeneratedAssembly(string assemblyPath, int timeoutMs = 30000)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = assemblyPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processInfo);
        bool exited = process!.WaitForExit(timeoutMs);

        if (!exited)
        {
            process.Kill();
            throw new TimeoutException($"Test execution timed out after {timeoutMs}ms. Test may have an infinite loop.");
        }

        string stdOut = process.StandardOutput.ReadToEnd();
        string stdErr = process.StandardError.ReadToEnd();

        if (process.ExitCode != 0)
        {
            throw new Exception($"dotnet execution failed (exit {process.ExitCode}):\nSTDERR:\n{stdErr}\nSTDOUT:\n{stdOut}");
        }

        return stdOut;
    }

    private Task VerifyWithSnapshot(string value, [CallerFilePath] string sourceFilePath = "")
    {
        var settings = new VerifySettings(_verifySettings);
        var directory = Path.GetDirectoryName(sourceFilePath)!;
        var snapshotsDirectory = Path.Combine(directory, "Snapshots");
        Directory.CreateDirectory(snapshotsDirectory);
        settings.UseDirectory(snapshotsDirectory);
        return Verify(value, settings);
    }
}
