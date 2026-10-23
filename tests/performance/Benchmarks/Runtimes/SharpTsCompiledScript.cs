using System.Reflection;
using System.Runtime.Loader;
using SharpTS.Compilation;
using SharpTS.Diagnostics;
using SharpTS.Modules;
using SharpTS.TypeSystem;

namespace Benchmarks.Runtimes;

/// <summary>Compiles and loads once; measured calls only invoke the prepared IL.</summary>
internal sealed class SharpTsCompiledScript : IDisposable
{
    private AssemblyLoadContext? _loadContext;
    private Type? _program;
    private Action? _main;

    public SharpTsCompiledScript(string source, string fileName)
    {
        // Check as JavaScript using SharpTS's CommonJS lenient checker. Emit a
        // single script so repeated calls do not hit the module initializer's
        // run-once guard. Benchmark source text is passed through unchanged.
        var sourcePath = Path.Combine(Path.GetTempPath(), $"sharpts-bench-{Guid.NewGuid():N}", Path.ChangeExtension(Path.GetFileName(fileName), ".cjs"));
        var resolver = new ModuleResolver(sourcePath, new Dictionary<string, string> { [sourcePath] = source });
        var module = resolver.LoadModule(sourcePath);
        var modules = resolver.GetModulesInOrder(module);
        var checker = new TypeChecker();
        var typeMap = checker.CheckModules(modules, resolver);
        var errors = checker.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToArray();
        if (errors.Length != 0)
        {
            throw new InvalidOperationException(
                $"SharpTS compilation failed for '{fileName}':{Environment.NewLine}" +
                string.Join(Environment.NewLine, errors.Select(error => error.ToString())));
        }
        var deadCode = new DeadCodeAnalyzer(typeMap).Analyze(module.Statements);
        var compiler = new ILCompiler($"sharpts_bench_{Guid.NewGuid():N}");
        compiler.Compile(module.Statements, typeMap, deadCode);
        var assemblyBytes = compiler.SaveToBytes();

        _loadContext = new AssemblyLoadContext($"sharpts-bench-{Guid.NewGuid():N}", isCollectible: true);
        try
        {
            using var stream = new MemoryStream(assemblyBytes, writable: false);
            var assembly = _loadContext.LoadFromStream(stream);
            _program = assembly.GetType("$Program", throwOnError: true)!;
            _main = _program.GetMethod("Main", BindingFlags.Public | BindingFlags.Static)!
                .CreateDelegate<Action>();
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public void Execute()
    {
        // SharpTS Main installs an event-loop context on the calling thread.
        var previousContext = SynchronizationContext.Current;
        try
        {
            (_main ?? throw new ObjectDisposedException(nameof(SharpTsCompiledScript)))();
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previousContext);
        }
    }

    public Func<object> GetFunction(string name)
    {
        var program = _program ?? throw new ObjectDisposedException(nameof(SharpTsCompiledScript));
        return (program.GetMethod(name, BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException($"SharpTS compiled function '{name}' was not found."))
            .CreateDelegate<Func<object>>();
    }

    public void Dispose()
    {
        _main = null;
        _program = null;
        _loadContext?.Unload();
        _loadContext = null;
    }
}
