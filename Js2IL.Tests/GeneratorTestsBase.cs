using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using JavaScriptRuntime.CommonJS;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Js2IL.Tests
{
    public abstract class GeneratorTestsBase
    {
        private readonly string _outputPath;
        private readonly VerifySettings _verifySettings = new();

        protected GeneratorTestsBase(string testCategory)
        {
            _verifySettings.DisableDiff();

            // Create a temp directory for the generated assemblies.
            // Use a unique per-run directory to avoid file locks from Assembly.LoadFile() causing
            // intermittent failures when re-running tests on Windows.
            var root = Path.Combine(Path.GetTempPath(), "Js2IL.Tests");
            var runId = Guid.NewGuid().ToString("N");
            _outputPath = Path.Combine(root, $"{testCategory}.GeneratorTests", runId);
            Directory.CreateDirectory(_outputPath);
        }

        protected Task GenerateTest(string testName, string[]? additionalScripts, [CallerFilePath] string sourceFilePath = "")
            => GenerateTest(testName, configureSettings: null, additionalScripts: additionalScripts, sourceFilePath: sourceFilePath);

        protected Task GenerateTest(string testName, Action<VerifySettings>? configureSettings = null, string[]? additionalScripts = null, [CallerFilePath] string sourceFilePath = "", Action<System.Reflection.Assembly>? verifyAssembly = null)
        {
            async Task RunAsync()
            {
                var js = GetJavaScript(testName);
                var testFilePath = Path.Combine(_outputPath, $"{testName}.js");

                var mockFileSystem = new MockFileSystem();
                mockFileSystem.AddFile(testFilePath, js);

                // Add additional scripts to the mock file system
                if (additionalScripts != null)
                {
                    foreach (var scriptName in additionalScripts)
                    {
                        var scriptContent = GetJavaScript(scriptName);
                        var scriptPath = Path.Combine(_outputPath, $"{scriptName}.js");
                        mockFileSystem.AddFile(scriptPath, scriptContent);
                    }
                }

                var options = new CompilerOptions
                {
                    OutputDirectory = _outputPath,
                    EmitPdb = true
                };

                var testLogger = new TestLogger();
                var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFileSystem, testLogger);
                var compiler = serviceProvider.GetRequiredService<Compiler>();

                if (!compiler.Compile(testFilePath))
                {
                    var compileDetails = string.IsNullOrWhiteSpace(testLogger.Errors)
                        ? string.Empty
                        : $"\nErrors:\n{testLogger.Errors}";
                    var warnings = string.IsNullOrWhiteSpace(testLogger.Warnings)
                        ? string.Empty
                        : $"\nWarnings:\n{testLogger.Warnings}";
                    throw new InvalidOperationException($"Compilation failed for test {testName}.{compileDetails}{warnings}");
                }

                // Compiler outputs <entryFileBasename>.dll into OutputDirectory.
                // For nested-path test names (e.g. "CommonJS_Require_X/a"), the DLL will be "a.dll".
                var assemblyName = Path.GetFileNameWithoutExtension(testFilePath);
                var expectedPath = Path.Combine(_outputPath, $"{assemblyName}.dll");

                AssertCompiledModuleManifest(expectedPath, testFilePath, additionalScripts);

                var il = Utilities.AssemblyToText.ConvertToText(expectedPath);

                if (verifyAssembly is not null)
                {
                    var assembly = Assembly.LoadFile(expectedPath);
                    verifyAssembly(assembly);
                }

                var settings = new VerifySettings(_verifySettings);
                var directory = Path.GetDirectoryName(sourceFilePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    var snapshotsDirectory = Path.Combine(directory, "Snapshots");
                    Directory.CreateDirectory(snapshotsDirectory);
                    settings.UseDirectory(snapshotsDirectory);
                }
                configureSettings?.Invoke(settings);

                await Verify(il, settings);
            }

            return RunAsync();
        }

        private string GetJavaScript(string testName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var category = GetCategoryFromNamespace();
            // Support nested module paths in tests (e.g., "CommonJS_Require_X/helpers/b").
            // Embedded resource names use '.' separators, so normalize path separators to '.'.
            var resourceKey = testName.Replace('\\', '.').Replace('/', '.');
            var categorySpecific = string.IsNullOrEmpty(category)
                ? null
                : $"Js2IL.Tests.{category}.JavaScript.{resourceKey}.js";
            var legacy = $"Js2IL.Tests.JavaScript.{resourceKey}.js";
            using (var stream = (categorySpecific != null ? assembly.GetManifestResourceStream(categorySpecific) : null)
                               ?? assembly.GetManifestResourceStream(legacy))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Resource '{categorySpecific}' or '{legacy}' not found in assembly '{assembly.FullName}'.");
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private void AssertCompiledModuleManifest(string assemblyPath, string rootScriptPath, string[]? additionalScripts)
        {
            var expected = new HashSet<string>(StringComparer.Ordinal)
            {
                GetExpectedModuleId(rootScriptPath, rootScriptPath)
            };

            expected.UnionWith((additionalScripts ?? System.Array.Empty<string>())
                .Select(scriptName => Path.Combine(_outputPath, $"{scriptName}.js"))
                .Select(scriptPath => GetExpectedModuleId(scriptPath, rootScriptPath)));

            var actual = ReadCompiledModuleIdsFromManifest(assemblyPath);

            Assert.NotEmpty(actual);
            Assert.All(expected, moduleId => Assert.Contains(moduleId, actual));
        }

        private static string GetExpectedModuleId(string modulePath, string rootScriptPath)
        {
            if (TryGetPackageIdentity(modulePath, out var packageName, out var withinPackageNoExt))
            {
                return withinPackageNoExt.Length == 0
                    ? packageName
                    : packageName + "/" + withinPackageNoExt;
            }

            return ModuleName.GetModuleIdForManifestFromPath(modulePath, rootScriptPath);
        }

        private static bool TryGetPackageIdentity(string modulePath, out string packageName, out string withinPackageNoExt)
        {
            packageName = string.Empty;
            withinPackageNoExt = string.Empty;
            var full = Path.GetFullPath(modulePath);

            var segments = full.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();

            var nodeModulesIndex = System.Array.FindLastIndex(segments, s => string.Equals(s, "node_modules", StringComparison.OrdinalIgnoreCase));
            if (nodeModulesIndex < 0 || nodeModulesIndex + 1 >= segments.Length)
            {
                return false;
            }

            var pkgStart = nodeModulesIndex + 1;
            if (segments[pkgStart].StartsWith("@", StringComparison.Ordinal) && pkgStart + 1 < segments.Length)
            {
                packageName = segments[pkgStart] + "/" + segments[pkgStart + 1];
                pkgStart += 2;
            }
            else
            {
                packageName = segments[pkgStart];
                pkgStart += 1;
            }

            var withinSegments = segments.Skip(pkgStart).ToArray();
            if (withinSegments.Length == 0)
            {
                withinPackageNoExt = string.Empty;
                return true;
            }

            var last = withinSegments[^1];
            last = last.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
                ? last.Substring(0, last.Length - 3)
                : Path.ChangeExtension(last, null) ?? last;
            withinSegments[^1] = last;

            withinPackageNoExt = string.Join("/", withinSegments);
            return true;
        }

        private static IReadOnlyCollection<string> ReadCompiledModuleIdsFromManifest(string assemblyPath)
        {
            using var stream = File.OpenRead(assemblyPath);
            using var peReader = new PEReader(stream);
            var reader = peReader.GetMetadataReader();

            var assemblyDef = reader.GetAssemblyDefinition();
            var result = new HashSet<string>(StringComparer.Ordinal);

            foreach (var caHandle in assemblyDef.GetCustomAttributes())
            {
                var ca = reader.GetCustomAttribute(caHandle);
                if (!IsJsCompiledModuleAttribute(reader, ca.Constructor))
                {
                    continue;
                }

                var br = reader.GetBlobReader(ca.Value);
                var prolog = br.ReadUInt16();
                if (prolog != 0x0001)
                {
                    throw new InvalidOperationException($"Invalid custom attribute prolog for JsCompiledModuleAttribute: 0x{prolog:X4}");
                }

                var moduleId = br.ReadSerializedString();
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
                var memberRef = reader.GetMemberReference((MemberReferenceHandle)ctorHandle);
                var parent = memberRef.Parent;
                if (parent.Kind == HandleKind.TypeReference)
                {
                    var typeRef = reader.GetTypeReference((TypeReferenceHandle)parent);
                    return string.Equals(reader.GetString(typeRef.Namespace), ExpectedNamespace, StringComparison.Ordinal)
                        && string.Equals(reader.GetString(typeRef.Name), ExpectedName, StringComparison.Ordinal);
                }
            }

            return false;
        }

        private string GetCategoryFromNamespace()
        {
            var ns = GetType().Namespace ?? string.Empty;
            const string rootNs = "Js2IL.Tests.";
            if (ns.StartsWith(rootNs, StringComparison.Ordinal))
            {
                var category = ns.Substring(rootNs.Length);
                if (!string.IsNullOrWhiteSpace(category))
                {
                    return category;
                }
            }

            return ns.Split('.').LastOrDefault() ?? string.Empty;
        }

    }
}
