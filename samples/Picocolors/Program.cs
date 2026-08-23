namespace Picocolors;

internal static class Program
{
    private static void Main()
    {
        using var pc = global::picocolors.Import();

        // Call a representative selection of picocolors color/style functions.
        // When ANSI color is supported the strings include ANSI escape codes;
        // when running without a TTY they are returned as plain text.
        string red    = Convert.ToString(pc.Red("ERROR: something went wrong")) ?? string.Empty;
        string green  = Convert.ToString(pc.Green("OK: all systems go"))        ?? string.Empty;
        string yellow = Convert.ToString(pc.Yellow("WARN: check your config"))  ?? string.Empty;
        string cyan   = Convert.ToString(pc.Cyan("INFO: picocolors via JROC"))  ?? string.Empty;
        string bold   = Convert.ToString(pc.Bold("Bold text"))                  ?? string.Empty;

        Console.WriteLine($"red={red}");
        Console.WriteLine($"green={green}");
        Console.WriteLine($"yellow={yellow}");
        Console.WriteLine($"cyan={cyan}");
        Console.WriteLine($"bold={bold}");
        Console.WriteLine("done");
    }
}
