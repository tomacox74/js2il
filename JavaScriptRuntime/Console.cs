using System;
using System.Text;

namespace JavaScriptRuntime
{
    public class ConsoleOutputSinks
    {
        public IConsoleOutput? Output { get; init; }

        public IConsoleOutput? ErrorOutput { get; init; }
    }

    public class Console
    {        
        private IConsoleOutput _output = new DefaultConsoleOutput();

        private IConsoleOutput _errorOutput = new DefaultErrorConsoleOutput();

        public Console(ConsoleOutputSinks consoleOutputSinks)
        {
            _output = consoleOutputSinks.Output ?? new DefaultConsoleOutput();
            _errorOutput = consoleOutputSinks.ErrorOutput ?? new DefaultErrorConsoleOutput();
        }

        /// <summary>
        /// Instance methods to support the console object, forwarding to the static API.
        /// </summary>
        public object? log(params object?[] args) => Log(args);

        public object? error(params object?[] args) => Error(args);

        public object? warn(params object?[] args) => Warn(args);

        public object? Log(params object?[] args)
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
                if (i > 0)
                {
                    sb.Append(' ');
                }

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

                        AppendConsoleArrayValue(sb, arr[j]);
                    }

                    sb.Append(' ');
                    sb.Append(']');
                    continue;
                }

                sb.Append(DotNet2JSConversions.ToString(arg));
            }

            _output.WriteLine(sb.ToString());
            return null;
        }

        public object? Error(params object?[] args)
        {
            if (args == null || args.Length == 0)
            {
                _errorOutput.WriteLine(string.Empty);
                return null;
            }

            var sb = new StringBuilder();

            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(' ');
                }

                sb.Append(DotNet2JSConversions.ToString(args[i]));
            }

            _errorOutput.WriteLine(sb.ToString());
            return null;
        }

        public object? Warn(params object?[] args)
        {
            if (args == null || args.Length == 0)
            {
                _errorOutput.WriteLine(string.Empty);
                return null;
            }

            var sb = new StringBuilder();

            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(' ');
                }

                sb.Append(DotNet2JSConversions.ToString(args[i]));
            }

            _errorOutput.WriteLine(sb.ToString());
            return null;
        }

        private static void AppendConsoleArrayValue(StringBuilder sb, object? value)
        {
            if (value is string str)
            {
                sb.Append('\'');
                sb.Append(EscapeConsoleString(str));
                sb.Append('\'');
                return;
            }

            sb.Append(DotNet2JSConversions.ToString(value));
        }

        private static string EscapeConsoleString(string value)
        {
            var sb = new StringBuilder(value.Length);

            foreach (var ch in value)
            {
                switch (ch)
                {
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\'':
                        sb.Append("\\'");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    default:
                        if (char.IsControl(ch))
                        {
                            sb.Append("\\u");
                            sb.Append(((int)ch).ToString("x4"));
                        }
                        else
                        {
                            sb.Append(ch);
                        }

                        break;
                }
            }

            return sb.ToString();
        }
    }
}
