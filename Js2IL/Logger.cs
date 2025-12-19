namespace Js2IL;

internal class Logger
{
    public static void WriteLineWarning(string message)
    {
        var prev = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
        }
        finally
        {
            Console.ForegroundColor = prev;
        }
    }

    public static void WriteLineError(string message)
    {
        var prev = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
        }
        finally
        {
            Console.ForegroundColor = prev;
        }
    }    
}