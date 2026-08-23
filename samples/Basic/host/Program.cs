namespace Basic;

internal static class Program
{
    private static void Main()
    {
        // Demonstrates:
        // - Referencing the compiled JROC module assembly to use its generated exports contract.
        // - Loading the entry module through its generated facade.
        // - Deterministic shutdown via IDisposable (the exports proxy closes the module runtime).
        using var exports = HostedMathModule.Import();

        Console.WriteLine($"version={exports.Version}");
        Console.WriteLine($"1+2={exports.Add(1, 2)}");
    }
}
