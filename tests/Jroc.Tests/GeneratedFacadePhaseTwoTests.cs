using System.Reflection;
using Jroc.Runtime;
using Jroc.Services;
using Jroc.Services.Contracts;

namespace Jroc.Tests;

public sealed class GeneratedFacadePhaseTwoTests
{
    [Fact]
    public void ExportShapeAnalyzer_ClassifiesCommonJsAndEsmDeterministically()
    {
        var shapes = AnalyzeShapes(
            """
            import './literal.js';
            import './collision.js';
            import './side.js';
            import './conditional.js';
            export { value as renamed } from './source.js';
            export { chained as chainAlias } from './mid.js';
            export * as everything from './source.js';
            export * from './source.js';
            export { maybe as unknownName } from './dynamic.js';
            """,
            new Dictionary<string, string>
            {
                ["source.js"] = "export const value = 42; export const other = 'text';",
                ["mid.js"] = "export { value as chained } from './source.js';",
                ["dynamic.js"] = "const key = 'late'; exports[key] = true;",
                ["conditional.js"] = "if (true) exports.flag = true;",
                ["literal.js"] = "module.exports = { version: '1.0.0', add: (a, b) => a + b };",
                ["collision.js"] = "module.exports = { fooBar: 1, 'foo-bar': 2 };",
                ["side.js"] = "console.log('side');"
            });

        var entry = shapes["entry"];
        Assert.Equal(PublicExportShapeKind.Unknown, entry.Kind);
        Assert.Contains(entry.Members, member => member.ExportName == "renamed" && member.SourceNode != null);
        Assert.Contains(entry.Members, member => member.ExportName == "chainAlias" && member.SourceNode != null);
        Assert.Contains(entry.Members, member => member.ExportName == "everything" && member.Kind == PublicExportMemberKind.Namespace);
        Assert.Contains(entry.Members, member => member.ExportName == "value");
        Assert.Contains(entry.Members, member => member.ExportName == "other");
        Assert.Contains(entry.Members, member => member.ExportName == "unknownName" && member.HasUnknownSource);

        var literal = shapes["literal"];
        Assert.Equal(PublicExportShapeKind.Known, literal.Kind);
        Assert.Equal(["version", "add"], literal.Members.Select(member => member.ExportName).ToArray());

        Assert.Equal(PublicExportShapeKind.Unknown, shapes["dynamic"].Kind);
        Assert.Equal(PublicExportShapeKind.Unknown, shapes["conditional"].Kind);
        Assert.Equal(PublicExportShapeKind.Unknown, shapes["collision"].Kind);
        Assert.Empty(shapes["collision"].Members);
        Assert.Equal(PublicExportShapeKind.NoExports, shapes["side"].Kind);
    }

    [Fact]
    public void ImportMethods_AreGeneratedOnlyForExportingOrUnknownModules()
    {
        using var sideEffectHarness = new GeneratedAssemblyConsumerHarness(
            "require('./exported.js'); require('./unknown.js'); console.log('entry');",
            "ImportShapeAssembly",
            new Dictionary<string, string>
            {
                ["exported.js"] = "module.exports = { answer: 42 };",
                ["unknown.js"] = "const name = 'dynamic'; exports[name] = 7;"
            });
        using var sideEffectAssembly = JrocInMemoryAssemblyLoader.Load(sideEffectHarness.Artifact);

        var root = sideEffectAssembly.Assembly.GetType("ImportShapeAssembly", throwOnError: true)!;
        var scripts = root.GetNestedType("Scripts", BindingFlags.Public)!;
        var entry = scripts.GetNestedType("entry", BindingFlags.Public)!;
        var exported = scripts.GetNestedType("exported", BindingFlags.Public)!;
        var unknown = scripts.GetNestedType("unknown", BindingFlags.Public)!;

        Assert.Null(root.GetMethod("Import", BindingFlags.Public | BindingFlags.Static));
        Assert.Null(entry.GetMethod("Import", BindingFlags.Public | BindingFlags.Static));
        Assert.NotNull(exported.GetMethod("Import", BindingFlags.Public | BindingFlags.Static));
        Assert.NotNull(unknown.GetMethod("Import", BindingFlags.Public | BindingFlags.Static));

        using var exportingEntryHarness = new GeneratedAssemblyConsumerHarness(
            "module.exports = { answer: 42 };",
            "EntryImportAssembly");
        using var exportingEntryAssembly = JrocInMemoryAssemblyLoader.Load(exportingEntryHarness.Artifact);
        var exportingRoot = exportingEntryAssembly.Assembly.GetType("EntryImportAssembly", throwOnError: true)!;
        var rootImport = exportingRoot.GetMethod("Import", BindingFlags.Public | BindingFlags.Static)!;
        var entryFacade = exportingRoot
            .GetNestedType("Scripts", BindingFlags.Public)!
            .GetNestedType("entry", BindingFlags.Public)!;
        var explicitImport = entryFacade.GetMethod("Import", BindingFlags.Public | BindingFlags.Static)!;

        Assert.Equal(rootImport.ReturnType, explicitImport.ReturnType);
        Assert.Equal("IExports", rootImport.ReturnType.Name);
        Assert.Equal(entryFacade, rootImport.ReturnType.DeclaringType);
        Assert.True(typeof(IDisposable).IsAssignableFrom(rootImport.ReturnType));
        Assert.Equal(
            "entry",
            rootImport.ReturnType.GetCustomAttribute<JsModuleAttribute>()!.ModuleId);
        Assert.DoesNotContain("JavaScriptRuntime", rootImport.ReturnType.FullName);
        Assert.DoesNotContain("Jroc.Runtime", rootImport.ReturnType.FullName);
    }

    [Fact]
    public void CSharpConsumer_ImportsEntryAndDeepModulesThroughGeneratedFacades()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            "require('./api/css.js'); require('./api/math.js'); module.exports = { version: '1.0.0', add: (a, b) => a + b, 'dash-name': 'dash' };",
            "HostedMathModule",
            new Dictionary<string, string>
            {
                ["api/css.js"] = "module.exports = { shade: 'blue' };",
                ["api/math.js"] = "module.exports.multiply = (a, b) => a * b; exports.label = 'module';"
            });

        var result = harness.Build(
            """
            using var root = HostedMathModule.Import();
            using var entry = HostedMathModule.Scripts.entry.Import();
            using var css = HostedMathModule.Scripts.api.css.Import();
            using HostedMathModule.Scripts.api.math.IExports math =
                HostedMathModule.Scripts.api.math.Import();

            Console.WriteLine(root.GetType() == entry.GetType());
            Console.WriteLine(root.Version);
            Console.WriteLine(root.Add(1, 2));
            Console.WriteLine(root.DashName);
            Console.WriteLine(css.Shade);
            Console.WriteLine(math.Multiply(2, 5));
            Console.WriteLine(math.Label);
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            ["True", "1.0.0", "3", "dash", "blue", "10", "module"],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void CSharpConsumer_ImportsEsmDefaultNamedAliasesNamespaceAndReexports()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            """
            export { label as alias } from './source.js';
            export { chained as chainAlias } from './mid.js';
            export * as everything from './source.js';
            export * from './common.js';
            export const local = 4;
            export default function plus(x) { return x + local; }
            """,
            "EsmImportAssembly",
            new Dictionary<string, string>
            {
                ["source.js"] = "export const label = 'from-source'; export const value = 5;",
                ["mid.js"] = "export { label as chained } from './source.js';",
                ["common.js"] = "exports.extra = true;"
            });

        var result = harness.Build(
            """
            using var exports = EsmImportAssembly.Import();
            Console.WriteLine(exports.Alias);
            Console.WriteLine(exports.ChainAlias);
            Console.WriteLine(exports.Local);
            Console.WriteLine(exports.Default(6));
            Console.WriteLine(exports.Extra);
            Console.WriteLine(exports.Everything != null);
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            ["from-source", "from-source", "4", "10", "True", "True"],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void CSharpConsumer_ImportsEsmDefaultOnlyModule()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            "export default 12;",
            "DefaultOnlyAssembly");

        var result = harness.Build(
            """
            using var exports = DefaultOnlyAssembly.Import();
            Console.WriteLine(exports.Default);
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(["12"], OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void CSharpConsumer_ImportsDirectAndUnknownFallbackContracts()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            "require('./function.js'); require('./class.js'); require('./null.js'); require('./undefined.js'); require('./unknown.js'); module.exports = 42;",
            "FallbackAssembly",
            new Dictionary<string, string>
            {
                ["function.js"] = "module.exports = function increment(value) { return value + 1; };",
                ["class.js"] = "module.exports = class Counter { constructor(value) { this.value = value; } read() { return this.value; } };",
                ["null.js"] = "module.exports = null;",
                ["undefined.js"] = "module.exports = undefined;",
                ["unknown.js"] = "const key = 'late'; exports[key] = 9;"
            });

        var result = harness.Build(
            """
            using var primitive = FallbackAssembly.Import();
            using var fn = FallbackAssembly.Scripts.function.Import();
            using var klass = FallbackAssembly.Scripts._class.Import();
            using var nullValue = FallbackAssembly.Scripts._null.Import();
            using var undefinedValue = FallbackAssembly.Scripts.undefined.Import();
            using var unknown = FallbackAssembly.Scripts.unknown.Import();

            Console.WriteLine(primitive.Value);
            Console.WriteLine(fn.Call(41));
            dynamic instance = klass.Construct(7);
            Console.WriteLine(instance.read());
            Console.WriteLine(nullValue.Value != null);
            Console.WriteLine(undefinedValue.Value == null);
            klass.Dispose();
            try { Console.WriteLine(instance.read()); }
            catch (ObjectDisposedException) { Console.WriteLine("nested-disposed"); }
            dynamic dynamicExports = unknown.Value;
            Console.WriteLine(dynamicExports.late);
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            ["42", "42", "7", "True", "True", "nested-disposed", "9"],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void ImportLifecycle_IsIsolatedDisposableAndFailureCleansUp()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            """
            if (false) { require('./failure.js'); require('./pending.js'); }
            globalThis.count = (globalThis.count || 0) + 1;
            module.exports = {
                count: () => count,
                fail: () => { throw new Error('call boom'); }
            };
            """,
            "ImportLifecycleAssembly",
            new Dictionary<string, string>
            {
                ["failure.js"] = "module.exports = { value: 1 }; throw new Error('load boom');",
                ["pending.js"] = "setTimeout(() => console.log('leaked-timer'), 50); module.exports = { ok: () => 'ok' };"
            });

        var result = harness.Build(
            """
            var first = ImportLifecycleAssembly.Import();
            var second = ImportLifecycleAssembly.Import();
            Console.WriteLine(first.Count());
            Console.WriteLine(second.Count());

            first.Dispose();
            first.Dispose();
            try { first.Count(); }
            catch (ObjectDisposedException) { Console.WriteLine("disposed"); }
            second.Dispose();

            using (var pending = ImportLifecycleAssembly.Scripts.pending.Import())
            {
                Console.WriteLine(pending.Ok());
            }
            Thread.Sleep(150);
            Console.WriteLine("after-pending");

            for (var i = 0; i < 2; i++)
            {
                try { using var failure = ImportLifecycleAssembly.Scripts.failure.Import(); }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.GetType().Name);
                    Console.WriteLine(exception.InnerException?.Message?.Contains("load boom") == true);
                }
            }
            """,
            run: true);

        AssertConsumerSucceeded(result);
        Assert.Equal(
            ["1", "1", "disposed", "ok", "after-pending", "JsModuleLoadException", "True", "JsModuleLoadException", "True"],
            OutputLines(result.RunStandardOutput));
    }

    [Fact]
    public void FallbackContractSignatures_UseOnlyGeneratedAndBclTypes()
    {
        using var harness = new GeneratedAssemblyConsumerHarness(
            "const key = 'late'; exports[key] = function(value) { return value; };",
            "UnknownFallbackAssembly");
        using var loaded = JrocInMemoryAssemblyLoader.Load(harness.Artifact);

        var root = loaded.Assembly.GetType("UnknownFallbackAssembly", throwOnError: true)!;
        var import = root.GetMethod("Import", BindingFlags.Public | BindingFlags.Static)!;
        var contract = import.ReturnType;

        AssertPublicSignatureUsesOnlyGeneratedOrBcl(import, loaded.Assembly);
        foreach (var member in contract.GetMembers(BindingFlags.Public | BindingFlags.Instance))
        {
            switch (member)
            {
                case MethodInfo method when !method.IsSpecialName:
                    AssertPublicSignatureUsesOnlyGeneratedOrBcl(method, loaded.Assembly);
                    break;
                case PropertyInfo property:
                    Assert.True(
                        IsGeneratedOrBcl(property.PropertyType, loaded.Assembly),
                        property.ToString());
                    break;
            }
        }
    }

    private static IReadOnlyDictionary<string, PublicModuleExportShape> AnalyzeShapes(
        string entrySource,
        IReadOnlyDictionary<string, string> additionalScripts)
    {
        var root = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "jroc-tests", "phase-two", Guid.NewGuid().ToString("N")));
        var entryPath = Path.Combine(root, "entry.js");
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(entryPath, entrySource);
        foreach (var (relativePath, source) in additionalScripts)
        {
            fileSystem.AddFile(Path.Combine(root, relativePath), source);
        }

        var logger = new TestLogger();
        var loader = new ModuleLoader(
            new CompilerOptions(),
            fileSystem,
            new NodeModuleResolver(fileSystem),
            logger);
        var modules = loader.LoadModules(entryPath);
        Assert.NotNull(modules);
        Assert.True(loader.LinkModules(modules!, logger), logger.Errors);

        return PublicExportShapeAnalyzer.Analyze(modules!);
    }

    private static void AssertPublicSignatureUsesOnlyGeneratedOrBcl(
        MethodInfo method,
        Assembly generatedAssembly)
    {
        Assert.True(IsGeneratedOrBcl(method.ReturnType, generatedAssembly), method.ToString());
        foreach (var parameter in method.GetParameters())
        {
            Assert.True(
                IsGeneratedOrBcl(parameter.ParameterType, generatedAssembly),
                method.ToString());
        }
    }

    private static bool IsGeneratedOrBcl(Type type, Assembly generatedAssembly)
    {
        if (type.IsArray)
        {
            return IsGeneratedOrBcl(type.GetElementType()!, generatedAssembly);
        }

        if (type.Assembly == generatedAssembly || type.Assembly == typeof(object).Assembly)
        {
            return true;
        }

        return false;
    }

    private static void AssertConsumerSucceeded(GeneratedAssemblyConsumerResult result)
    {
        Assert.True(
            result.BuildExitCode == 0,
            $"Consumer build failed.{Environment.NewLine}{result.BuildDiagnostics}");
        Assert.True(
            result.RunExitCode == 0,
            $"Consumer run failed.{Environment.NewLine}" +
            $"{result.RunStandardOutput}{Environment.NewLine}{result.RunStandardError}");
    }

    private static string[] OutputLines(string output) =>
        output.Replace("\r", string.Empty, StringComparison.Ordinal)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);
}
