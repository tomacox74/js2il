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
            // Empty prints empty line
            if (args == null || args.Length == 0)
            {
                _output.WriteLine(string.Empty);
                return null;
            }
            // Build output without LINQ to avoid capturing implementation details in edge cases
            var sb = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0) sb.Append(' ');
                var arg = args[i];
                if (arg is Array arr)
                {
                    // Node-like console formatting for arrays: [ 1, 2, 3 ]
                    sb.Append('[');
                    sb.Append(' ');
                    for (int j = 0; j < arr.Count; j++)
                    {
                        if (j > 0)
                        {
                            sb.Append(',');
                            sb.Append(' ');
                        }
                        sb.Append(DotNet2JSConversions.ToString(arr[j]));
                    }
                    sb.Append(' ');
                    sb.Append(']');
                }
                else
                {
                    sb.Append(DotNet2JSConversions.ToString(arg));
                }
            }
            _output.WriteLine(sb.ToString());
            return null;
        }

        public static object? Error(params object?[] args)
        {
            if (args == null || args.Length == 0)
            {
                _errorOutput.WriteLine(string.Empty);
                return null;
            }
            var sb = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0) sb.Append(' ');
                sb.Append(DotNet2JSConversions.ToString(args[i]));
            }
            _errorOutput.WriteLine(sb.ToString());
            return null;
        }

        public static object? Warn(params object?[] args)
        {
            if (args == null || args.Length == 0)
            {
                _errorOutput.WriteLine(string.Empty);
                return null;
            }
            var sb = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0) sb.Append(' ');
                sb.Append(DotNet2JSConversions.ToString(args[i]));
            }
            _errorOutput.WriteLine(sb.ToString());
            return null;
        }
    }
}
