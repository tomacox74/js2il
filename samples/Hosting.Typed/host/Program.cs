using Js2IL.Runtime;
using Js2IL.HostedCounterModule;
using System.Threading.Tasks;

namespace Hosting.Typed;

internal static class Program
{
    private static async Task Main()
    {
        // Demonstrates:
        // - Using only JS2IL-generated contracts (no handwritten interfaces).
        // - Exported functions and values projected onto a strongly-typed exports interface.
        // - Exported ES class projected as IJsConstructor<T> and instances as IJsHandle (dispose when done).
        // - Invoking an async JS export that returns a Promise, then bridging it to Task.
        using var exports = JsEngine.LoadModule<IHostedCounterModuleExports>();

        Console.WriteLine($"version={exports.Version}");
        Console.WriteLine($"add(1,2)={exports.Add(1, 2)}");

        using var counter = exports.Counter.Construct(10);
        Console.WriteLine($"counter.add(5)={counter.Add(5)}");
        Console.WriteLine($"counter.value={counter.GetValue()}");

        // The JS async export returns a Promise at runtime, projected as a Task in the contract.
        var asyncSum = await exports.AddAsync(1d, 2d);
        Console.WriteLine($"addAsync(1,2)={asyncSum}");

        using var created = exports.CreateCounter(2);
        Console.WriteLine($"created.add(1)={created.Add(1)}");
    }
}
