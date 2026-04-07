using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Js2IL.Tests.CommonJS;

internal sealed class TempNodeModulesProject : IDisposable
{
    public TempNodeModulesProject(string root, string projectRoot, string outputDir, string entryPath)
    {
        Root = root;
        ProjectRoot = projectRoot;
        OutputDir = outputDir;
        EntryPath = entryPath;
    }

    public string Root { get; }

    public string ProjectRoot { get; }

    public string OutputDir { get; }

    public string EntryPath { get; }

    public void Dispose()
    {
        if (!Directory.Exists(Root))
        {
            return;
        }

        try
        {
            Directory.Delete(Root, recursive: true);
        }
        catch (DirectoryNotFoundException)
        {
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}

internal sealed class CompiledNodeModulesProject
{
    public CompiledNodeModulesProject(bool success, string? assemblyPath, TestLogger logger)
    {
        Success = success;
        AssemblyPath = assemblyPath;
        Logger = logger;
    }

    public bool Success { get; }

    public string? AssemblyPath { get; }

    public TestLogger Logger { get; }
}

internal static class NodeModulesTestProjectSupport
{
    public static TempNodeModulesProject CreateDualModeExportsImportsProject()
    {
        var project = CreateProject("Issue869DualMode", Path.Combine("src", "main.mjs"));

        WriteFile(project.ProjectRoot, Path.Combine("src", "main.mjs"),
            "\"use strict\";\n" +
            "import pkg, { sharedKind } from 'pkg';\n" +
            "import feature from 'pkg/feature';\n" +
            "import cjsView from './require-view.cjs';\n" +
            "console.log('esm.entry', pkg.entry);\n" +
            "console.log('esm.shared', sharedKind);\n" +
            "console.log('esm.feature', feature.feature);\n" +
            "console.log('cjs.entry', cjsView.entry);\n" +
            "console.log('cjs.shared', cjsView.sharedKind);\n" +
            "console.log('cjs.feature', cjsView.feature);\n");

        WriteFile(project.ProjectRoot, Path.Combine("src", "require-view.cjs"),
            "\"use strict\";\n" +
            "const pkg = require('pkg');\n" +
            "const feature = require('pkg/feature');\n" +
            "module.exports = {\n" +
            "  entry: pkg.entry,\n" +
            "  sharedKind: pkg.shared.kind,\n" +
            "  feature: feature.feature\n" +
            "};\n");

        var packageRoot = Path.Combine(project.ProjectRoot, "node_modules", "pkg");
        WriteFile(packageRoot, "package.json",
            "{\n" +
            "  \"name\": \"pkg\",\n" +
            "  \"type\": \"module\",\n" +
            "  \"exports\": {\n" +
            "    \".\": { \"import\": \"./esm/index.js\", \"require\": \"./cjs/index.cjs\" },\n" +
            "    \"./feature\": { \"import\": \"./esm/feature.js\", \"require\": \"./cjs/feature.cjs\" }\n" +
            "  },\n" +
            "  \"imports\": {\n" +
            "    \"#shared\": { \"import\": \"./esm/shared.js\", \"require\": \"./cjs/shared.cjs\" }\n" +
            "  }\n" +
            "}\n");

        WriteFile(packageRoot, Path.Combine("esm", "shared.js"),
            "\"use strict\";\n" +
            "export default { kind: 'imports:import' };\n" +
            "export const kind = 'imports:import';\n");

        WriteFile(packageRoot, Path.Combine("esm", "index.js"),
            "\"use strict\";\n" +
            "import shared, { kind as sharedKind } from '#shared';\n" +
            "export { sharedKind };\n" +
            "export default { entry: 'esm-entry', shared };\n");

        WriteFile(packageRoot, Path.Combine("esm", "feature.js"),
            "\"use strict\";\n" +
            "import shared from '#shared';\n" +
            "export default { feature: 'esm-feature:' + shared.kind };\n");

        WriteFile(packageRoot, Path.Combine("cjs", "shared.cjs"),
            "\"use strict\";\n" +
            "module.exports = { kind: 'imports:require' };\n");

        WriteFile(packageRoot, Path.Combine("cjs", "index.cjs"),
            "\"use strict\";\n" +
            "const shared = require('#shared');\n" +
            "module.exports = { entry: 'cjs-entry', shared };\n");

        WriteFile(packageRoot, Path.Combine("cjs", "feature.cjs"),
            "\"use strict\";\n" +
            "const shared = require('#shared');\n" +
            "module.exports = { feature: 'cjs-feature:' + shared.kind };\n");

        return project;
    }

    public static TempNodeModulesProject CreateBarePackageImportsAliasProject()
    {
        var project = CreateProject("Issue952BarePackageImportsAlias", Path.Combine("src", "main.mjs"));

        WriteFile(project.ProjectRoot, "package.json",
            "{\n" +
            "  \"name\": \"app\",\n" +
            "  \"type\": \"module\",\n" +
            "  \"imports\": {\n" +
            "    \"#dep\": \"dep\",\n" +
            "    \"#dep/*\": \"dep/*\"\n" +
            "  }\n" +
            "}\n");

        WriteFile(project.ProjectRoot, Path.Combine("src", "main.mjs"),
            "\"use strict\";\n" +
            "import pkg from '#dep';\n" +
            "import feature from '#dep/feature';\n" +
            "import cjsView from './require-view.cjs';\n" +
            "console.log('esm.entry', pkg.entry);\n" +
            "console.log('esm.feature', feature.feature);\n" +
            "console.log('cjs.entry', cjsView.entry);\n" +
            "console.log('cjs.feature', cjsView.feature);\n");

        WriteFile(project.ProjectRoot, Path.Combine("src", "require-view.cjs"),
            "\"use strict\";\n" +
            "const pkg = require('#dep');\n" +
            "const feature = require('#dep/feature');\n" +
            "module.exports = {\n" +
            "  entry: pkg.entry,\n" +
            "  feature: feature.feature\n" +
            "};\n");

        var packageRoot = Path.Combine(project.ProjectRoot, "node_modules", "dep");
        WriteFile(packageRoot, "package.json",
            "{\n" +
            "  \"name\": \"dep\",\n" +
            "  \"type\": \"module\",\n" +
            "  \"exports\": {\n" +
            "    \".\": { \"import\": \"./esm/index.js\", \"require\": \"./cjs/index.cjs\" },\n" +
            "    \"./feature\": { \"import\": \"./esm/feature.js\", \"require\": \"./cjs/feature.cjs\" }\n" +
            "  }\n" +
            "}\n");

        WriteFile(packageRoot, Path.Combine("esm", "index.js"),
            "\"use strict\";\n" +
            "export default { entry: 'esm-dep-entry' };\n");

        WriteFile(packageRoot, Path.Combine("esm", "feature.js"),
            "\"use strict\";\n" +
            "export default { feature: 'esm-dep-feature' };\n");

        WriteFile(packageRoot, Path.Combine("cjs", "index.cjs"),
            "\"use strict\";\n" +
            "module.exports = { entry: 'cjs-dep-entry' };\n");

        WriteFile(packageRoot, Path.Combine("cjs", "feature.cjs"),
            "\"use strict\";\n" +
            "module.exports = { feature: 'cjs-dep-feature' };\n");

        return project;
    }

    public static TempNodeModulesProject CreateUnsupportedExportsConditionsProject()
    {
        var project = CreateProject("Issue869UnsupportedConditions", Path.Combine("src", "main.mjs"));

        WriteFile(project.ProjectRoot, Path.Combine("src", "main.mjs"),
            "\"use strict\";\n" +
            "import value from 'badpkg';\n" +
            "console.log(value);\n");

        var packageRoot = Path.Combine(project.ProjectRoot, "node_modules", "badpkg");
        WriteFile(packageRoot, "package.json",
            "{\n" +
            "  \"name\": \"badpkg\",\n" +
            "  \"exports\": {\n" +
            "    \".\": { \"browser\": \"./browser.js\" }\n" +
            "  }\n" +
            "}\n");

        WriteFile(packageRoot, "browser.js", "export default 'browser-only';\n");

        return project;
    }

    public static CompiledNodeModulesProject Compile(TempNodeModulesProject project)
    {
        var options = new CompilerOptions
        {
            OutputDirectory = project.OutputDir,
            EmitPdb = true
        };

        var testLogger = new TestLogger();
        using var serviceProvider = CompilerServices.BuildServiceProvider(options, fileSystem: new FileSystem(), compilerOutput: testLogger);
        var compiler = serviceProvider.GetRequiredService<Compiler>();

        if (!compiler.Compile(project.EntryPath))
        {
            return new CompiledNodeModulesProject(success: false, assemblyPath: null, logger: testLogger);
        }

        var assemblyName = Path.GetFileNameWithoutExtension(project.EntryPath) + ".dll";
        var assemblyPath = Path.Combine(project.OutputDir, assemblyName);
        if (!File.Exists(assemblyPath))
        {
            throw new InvalidOperationException($"Expected compiled assembly not found at '{assemblyPath}'.");
        }

        return new CompiledNodeModulesProject(success: true, assemblyPath, testLogger);
    }

    public static string ExecuteGeneratedAssembly(string assemblyPath, int timeoutMs = 30000)
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
        var exited = process!.WaitForExit(timeoutMs);

        if (!exited)
        {
            process.Kill();
            throw new TimeoutException($"Test execution timed out after {timeoutMs}ms. Test may have an infinite loop.");
        }

        var stdOut = process.StandardOutput.ReadToEnd();
        var stdErr = process.StandardError.ReadToEnd();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"dotnet execution failed (exit {process.ExitCode}):\nSTDERR:\n{stdErr}\nSTDOUT:\n{stdOut}");
        }

        return stdOut;
    }

    public static IReadOnlyCollection<string> ReadCompiledModuleIdsFromManifest(string assemblyPath)
    {
        using var stream = File.OpenRead(assemblyPath);
        using var peReader = new PEReader(stream);
        var reader = peReader.GetMetadataReader();

        var assemblyDef = reader.GetAssemblyDefinition();
        var result = new HashSet<string>(StringComparer.Ordinal);

        foreach (var customAttributeHandle in assemblyDef.GetCustomAttributes())
        {
            var customAttribute = reader.GetCustomAttribute(customAttributeHandle);
            if (!IsJsCompiledModuleAttribute(reader, customAttribute.Constructor))
            {
                continue;
            }

            var blobReader = reader.GetBlobReader(customAttribute.Value);
            var prolog = blobReader.ReadUInt16();
            if (prolog != 0x0001)
            {
                throw new InvalidOperationException($"Invalid custom attribute prolog for JsCompiledModuleAttribute: 0x{prolog:X4}");
            }

            var moduleId = blobReader.ReadSerializedString();
            if (!string.IsNullOrWhiteSpace(moduleId))
            {
                result.Add(moduleId);
            }
        }

        return result;
    }

    private static bool IsJsCompiledModuleAttribute(MetadataReader reader, EntityHandle ctorHandle)
    {
        const string ExpectedNamespace = "Js2IL.Runtime";
        const string ExpectedName = "JsCompiledModuleAttribute";

        if (ctorHandle.Kind == HandleKind.MethodDefinition)
        {
            var method = reader.GetMethodDefinition((MethodDefinitionHandle)ctorHandle);
            var declaringType = reader.GetTypeDefinition(method.GetDeclaringType());
            return string.Equals(reader.GetString(declaringType.Namespace), ExpectedNamespace, StringComparison.Ordinal)
                && string.Equals(reader.GetString(declaringType.Name), ExpectedName, StringComparison.Ordinal);
        }

        if (ctorHandle.Kind == HandleKind.MemberReference)
        {
            var memberReference = reader.GetMemberReference((MemberReferenceHandle)ctorHandle);
            var parent = memberReference.Parent;
            if (parent.Kind == HandleKind.TypeReference)
            {
                var typeReference = reader.GetTypeReference((TypeReferenceHandle)parent);
                return string.Equals(reader.GetString(typeReference.Namespace), ExpectedNamespace, StringComparison.Ordinal)
                    && string.Equals(reader.GetString(typeReference.Name), ExpectedName, StringComparison.Ordinal);
            }
        }

        return false;
    }

    private static TempNodeModulesProject CreateProject(string scenarioName, string entryRelativePath)
    {
        var root = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", scenarioName, Guid.NewGuid().ToString("N"));
        var projectRoot = Path.Combine(root, "proj");
        var outputDir = Path.Combine(root, "out");
        var entryPath = Path.Combine(projectRoot, entryRelativePath);

        Directory.CreateDirectory(projectRoot);
        Directory.CreateDirectory(outputDir);
        Directory.CreateDirectory(Path.GetDirectoryName(entryPath)!);

        return new TempNodeModulesProject(root, projectRoot, outputDir, entryPath);
    }

    private static void WriteFile(string root, string relativePath, string contents)
    {
        var fullPath = Path.Combine(root, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, contents);
    }
}
