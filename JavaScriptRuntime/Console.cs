using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaScriptRuntime
{
    public class Console
    {
        private static IConsoleOutput _output = new DefaultConsoleOutput();
        private static IConsoleOutput _errorOutput = new DefaultErrorConsoleOutput();

        // Instance methods to support a global console object (e.g., console.log)
        // These delegate to the static implementations so both patterns work.
        public object? log(params object?[] args) => Log(args);
        public object? error(params object?[] args) => Error(args);
        public object? warn(params object?[] args) => Warn(args);

        public static void SetOutput(IConsoleOutput output)
        {
            _output = output ?? new DefaultConsoleOutput();
        }

        public static void SetErrorOutput(IConsoleOutput output)
        {
            _errorOutput = output ?? new DefaultErrorConsoleOutput();
        }

        public static object? Log(params object?[] args)
        {
            var parts = args.Select(arg => DotNet2JSConversions.ToString(arg));
            var line = string.Join(" ", parts);
            _output.WriteLine(line);
            return null;
        }

        public static object? Error(params object?[] args)
        {
            var parts = args.Select(arg => DotNet2JSConversions.ToString(arg));
            var line = string.Join(" ", parts);
            _errorOutput.WriteLine(line);
            return null;
        }

        public static object? Warn(params object?[] args)
        {
            var parts = args.Select(arg => DotNet2JSConversions.ToString(arg));
            var line = string.Join(" ", parts);
            _errorOutput.WriteLine(line);
            return null;
        }
    }
}
