namespace Picocolors;

internal static class Program
{
    private static void Main()
    {
        using var pc = global::picocolors.Import();

        // Call a representative selection of picocolors color/style functions.
        // When ANSI color is supported the strings include ANSI escape codes;
        // when running without a TTY they are returned as plain text.
        string red = pc.Red("ERROR: something went wrong");
        string green = pc.Green("OK: all systems go");
        string yellow = pc.Yellow("WARN: check your config");
        string cyan = pc.Cyan("INFO: picocolors via JROC");
        string bold = pc.Bold("Bold text");

        Console.WriteLine($"red={red}");
        Console.WriteLine($"green={green}");
        Console.WriteLine($"yellow={yellow}");
        Console.WriteLine($"cyan={cyan}");
        Console.WriteLine($"bold={bold}");
        Console.WriteLine("done");
    }
}
