using System.Reflection;
using Acornima;
using JavaScriptRuntime;
using JavaScriptRuntime.Modules.CommonJS;
using Jroc;
using Jroc.Runtime;

namespace Jroc.Tests;

internal sealed class Test262ScriptHelpers : IDisposable
{
    private readonly List<JrocLoadedAssembly> _assemblies = [];

    internal object? Evaluate(object? source)
    {
        var text = DotNet2JSConversions.ToString(source);
        var path = Path.Combine(Path.GetTempPath(), $"test262-script-{Guid.NewGuid():N}.js");
        try
        {
            new Parser(new ParserOptions
            {
                EcmaVersion = EcmaVersion.Latest,
                ExperimentalESFeatures = ExperimentalESFeatures.Decorators
            }).ParseScript(text, path);
        }
        catch (ParseErrorException error)
        {
            throw new JavaScriptRuntime.SyntaxError(error.Message);
        }
        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(path)
        {
            SourceText = text,
            ParseAsScript = true
        });
        var loaded = JrocInMemoryAssemblyLoader.Load(artifact);
        _assemblies.Add(loaded);
        var mapping = loaded.Assembly.GetCustomAttributes<JsCompiledModuleTypeAttribute>()
            .Single(attribute => attribute.CanonicalModuleId == artifact.EntryModuleId);
        var entry = loaded.Assembly.GetType(mapping.TypeName, throwOnError: true)!
            .GetMethod("__js_module_init__", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("Compiled script has no module initializer.");
        var run = entry.CreateDelegate<ModuleMainDelegate>();
        var previousThis = RuntimeServices.SetCurrentThis(GlobalThis.globalThis);
        try
        {
            run(null, _ => throw new TypeError("require is unavailable in $262.evalScript"), null, path, Path.GetDirectoryName(path)!);
            return null;
        }
        finally
        {
            RuntimeServices.SetCurrentThis(previousThis);
        }
    }

    public void Dispose()
    {
        foreach (var assembly in _assemblies)
        {
            assembly.Dispose();
        }
        _assemblies.Clear();
    }
}
