using Js2IL.Runtime;
using Js2IL.HostedMathModule;

namespace Hosting.Basic;

internal static class Program
{
    private static void Main()
    {
        // Demonstrates:
        // - Referencing the compiled JS2IL module assembly to use its generated exports contract.
        // - Loading a module via JsEngine.LoadModule<TExports>() using the [JsModule] metadata on the generated contract.
        // - Deterministic shutdown via IDisposable (the exports proxy closes the module runtime).
        using var exports = JsEngine.LoadModule<IHostedMathModuleExports>();

        Console.WriteLine($"version={exports.Version}");
        Console.WriteLine($"1+2={exports.Add(1, 2)}");
    }
}
