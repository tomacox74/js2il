using System;
using System.Collections.Generic;
using System.Text;
using JavaScriptRuntime.Node;

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

        private bool _useColors;

        public Console(ConsoleOutputSinks consoleOutputSinks)
        {
            _output = consoleOutputSinks.Output ?? new DefaultConsoleOutput();
            _errorOutput = consoleOutputSinks.ErrorOutput ?? new DefaultErrorConsoleOutput();
        }

        public Console()
        {
        }

        public Console(object? options)
        {
            if (options == null || options is JsNull)
            {
                throw new TypeError("Console options must specify a writable stdout stream.");
            }

            _output = CreateStreamOutput(Object.GetProperty(options, "stdout"), "stdout");

            var stderr = Object.GetProperty(options, "stderr");
            _errorOutput = stderr == null || stderr is JsNull
                ? _output
                : CreateStreamOutput(stderr, "stderr");

            var inspectOptions = Object.GetProperty(options, "inspectOptions");
            _useColors = Object.GetProperty(inspectOptions ?? JsNull.Null, "colors") is bool colors && colors;
        }

        /// <summary>
        /// Instance methods to support the console object, forwarding to the static API.
        /// </summary>
        public object? log(params object?[] args) => Log(args);

        public object? error(params object?[] args) => Error(args);

        public object? warn(params object?[] args) => Warn(args);

        public object? table(object? tabularData)
        {
            if (tabularData is not Array rows || rows.Count == 0)
            {
                return null;
            }

            var columns = GetTableColumns(rows);
            if (columns.Count == 0)
            {
                return null;
            }

            var widths = new int[columns.Count + 1];
            widths[0] = "index".Length;
            for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
            {
                widths[columnIndex + 1] = columns[columnIndex].Length;
            }

            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                widths[0] = System.Math.Max(widths[0], rowIndex.ToString().Length);
                var row = rows[rowIndex] ?? JsNull.Null;
                for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
                {
                    widths[columnIndex + 1] = System.Math.Max(
                        widths[columnIndex + 1],
                        FormatTableValue(Object.GetProperty(row, columns[columnIndex])).Length);
                }
            }

            var table = new StringBuilder();
            AppendTableRow(table, widths, "index", columns);
            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var values = new string[columns.Count];
                var row = rows[rowIndex] ?? JsNull.Null;
                for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
                {
                    values[columnIndex] = FormatTableValue(Object.GetProperty(row, columns[columnIndex]));
                }

                table.Append('\n');
                AppendTableRow(table, widths, rowIndex.ToString(), values);
            }

            _output.WriteLine(table.ToString());
            return null;
        }

        // Arity-specific overloads to avoid object[] allocations for common cases (0-3 args).
        // These inline the formatting logic to avoid creating arrays.

        public object? log()
        {
            _output.WriteLine(string.Empty);
            return null;
        }

        public object? log(object? a0)
        {
            var sb = new StringBuilder();
            if (a0 is Array arr)
            {
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
            }
            else
            {
                AppendConsoleValue(sb, a0);
            }
            _output.WriteLine(sb.ToString());
            return null;
        }

        public object? log(object? a0, object? a1)
        {
            var sb = new StringBuilder();

            if (a0 is Array arr0)
            {
                sb.Append('[');
                sb.Append(' ');
                for (int j = 0; j < arr0.Count; j++)
                {
                    if (j > 0)
                    {
                        sb.Append(',');
                        sb.Append(' ');
                    }
                    AppendConsoleArrayValue(sb, arr0[j]);
                }
                sb.Append(' ');
                sb.Append(']');
            }
            else
            {
                AppendConsoleValue(sb, a0);
            }

            sb.Append(' ');

            if (a1 is Array arr1)
            {
                sb.Append('[');
                sb.Append(' ');
                for (int j = 0; j < arr1.Count; j++)
                {
                    if (j > 0)
                    {
                        sb.Append(',');
                        sb.Append(' ');
                    }
                    AppendConsoleArrayValue(sb, arr1[j]);
                }
                sb.Append(' ');
                sb.Append(']');
            }
            else
            {
                AppendConsoleValue(sb, a1);
            }

            _output.WriteLine(sb.ToString());
            return null;
        }

        public object? log(object? a0, object? a1, object? a2)
        {
            var sb = new StringBuilder();

            if (a0 is Array arr0)
            {
                sb.Append('[');
                sb.Append(' ');
                for (int j = 0; j < arr0.Count; j++)
                {
                    if (j > 0)
                    {
                        sb.Append(',');
                        sb.Append(' ');
                    }
                    AppendConsoleArrayValue(sb, arr0[j]);
                }
                sb.Append(' ');
                sb.Append(']');
            }
            else
            {
                AppendConsoleValue(sb, a0);
            }

            sb.Append(' ');

            if (a1 is Array arr1)
            {
                sb.Append('[');
                sb.Append(' ');
                for (int j = 0; j < arr1.Count; j++)
                {
                    if (j > 0)
                    {
                        sb.Append(',');
                        sb.Append(' ');
                    }
                    AppendConsoleArrayValue(sb, arr1[j]);
                }
                sb.Append(' ');
                sb.Append(']');
            }
            else
            {
                AppendConsoleValue(sb, a1);
            }

            sb.Append(' ');

            if (a2 is Array arr2)
            {
                sb.Append('[');
                sb.Append(' ');
                for (int j = 0; j < arr2.Count; j++)
                {
                    if (j > 0)
                    {
                        sb.Append(',');
                        sb.Append(' ');
                    }
                    AppendConsoleArrayValue(sb, arr2[j]);
                }
                sb.Append(' ');
                sb.Append(']');
            }
            else
            {
                AppendConsoleValue(sb, a2);
            }

            _output.WriteLine(sb.ToString());
            return null;
        }

        public object? error()
        {
            _errorOutput.WriteLine(string.Empty);
            return null;
        }

        public object? error(object? a0)
        {
            var sb = new StringBuilder();
            sb.Append(DotNet2JSConversions.ToString(a0));
            _errorOutput.WriteLine(sb.ToString());
            return null;
        }

        public object? error(object? a0, object? a1)
        {
            var sb = new StringBuilder();
            sb.Append(DotNet2JSConversions.ToString(a0));
            sb.Append(' ');
            sb.Append(DotNet2JSConversions.ToString(a1));
            _errorOutput.WriteLine(sb.ToString());
            return null;
        }

        public object? error(object? a0, object? a1, object? a2)
        {
            var sb = new StringBuilder();
            sb.Append(DotNet2JSConversions.ToString(a0));
            sb.Append(' ');
            sb.Append(DotNet2JSConversions.ToString(a1));
            sb.Append(' ');
            sb.Append(DotNet2JSConversions.ToString(a2));
            _errorOutput.WriteLine(sb.ToString());
            return null;
        }

        public object? warn()
        {
            _errorOutput.WriteLine(string.Empty);
            return null;
        }

        public object? warn(object? a0)
        {
            var sb = new StringBuilder();
            sb.Append(DotNet2JSConversions.ToString(a0));
            _errorOutput.WriteLine(sb.ToString());
            return null;
        }

        public object? warn(object? a0, object? a1)
        {
            var sb = new StringBuilder();
            sb.Append(DotNet2JSConversions.ToString(a0));
            sb.Append(' ');
            sb.Append(DotNet2JSConversions.ToString(a1));
            _errorOutput.WriteLine(sb.ToString());
            return null;
        }

        public object? warn(object? a0, object? a1, object? a2)
        {
            var sb = new StringBuilder();
            sb.Append(DotNet2JSConversions.ToString(a0));
            sb.Append(' ');
            sb.Append(DotNet2JSConversions.ToString(a1));
            sb.Append(' ');
            sb.Append(DotNet2JSConversions.ToString(a2));
            _errorOutput.WriteLine(sb.ToString());
            return null;
        }

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

                AppendConsoleValue(sb, arg);
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

            AppendConsoleValue(sb, value);
        }

        private static void AppendConsoleValue(StringBuilder sb, object? value)
        {
            if (value is not null
                && (PropertyDescriptorStore.TryGetOwn(value, JavaScriptRuntime.String.StringDataPropertyName, out _)
                    || PropertyDescriptorStore.TryGetOwn(value, JavaScriptRuntime.Number.NumberDataPropertyName, out _)))
            {
                sb.Append(DotNet2JSConversions.ToString(value));
                return;
            }

            if (value is JavaScriptRuntime.Array)
            {
                sb.Append(DotNet2JSConversions.ToString(value));
                return;
            }

            if (value is System.Collections.Generic.IDictionary<string, object?> dict)
            {
                sb.Append('{');
                if (dict.Count > 0)
                {
                    sb.Append(' ');
                    var first = true;
                    foreach (var entry in dict)
                    {
                        if (!first)
                        {
                            sb.Append(',');
                            sb.Append(' ');
                        }

                        first = false;
                        sb.Append(entry.Key);
                        sb.Append(':');
                        sb.Append(' ');
                        if (entry.Value is string str)
                        {
                            sb.Append('\'');
                            sb.Append(EscapeConsoleString(str));
                            sb.Append('\'');
                        }
                        else
                        {
                            AppendConsoleValue(sb, entry.Value);
                        }
                    }

                    sb.Append(' ');
                }

                sb.Append('}');
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

        private static IConsoleOutput CreateStreamOutput(object? stream, string streamName)
        {
            if (stream is not Writable writable)
            {
                throw new TypeError($"Console {streamName} must be a writable stream.");
            }

            return new StreamConsoleOutput(writable);
        }

        private static List<string> GetTableColumns(Array rows)
        {
            var columns = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var row = rows[rowIndex];
                if (row == null || row is JsNull)
                {
                    continue;
                }

                foreach (var key in Object.GetEnumerableKeys(row))
                {
                    var name = DotNet2JSConversions.ToString(key);
                    if (seen.Add(name))
                    {
                        columns.Add(name);
                    }
                }
            }

            return columns;
        }

        private string FormatTableValue(object? value)
        {
            var builder = new StringBuilder();
            AppendConsoleValue(builder, value);
            var formatted = builder.ToString();
            if (!_useColors)
            {
                return formatted;
            }

            return value switch
            {
                string => $"\u001b[32m{formatted}\u001b[39m",
                bool or byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal
                    => $"\u001b[33m{formatted}\u001b[39m",
                _ => formatted
            };
        }

        private static void AppendTableRow(StringBuilder line, int[] widths, string index, IReadOnlyList<string> values)
        {
            AppendTableCell(line, index, widths[0]);
            for (var columnIndex = 0; columnIndex < values.Count; columnIndex++)
            {
                line.Append(" | ");
                AppendTableCell(line, values[columnIndex], widths[columnIndex + 1]);
            }
        }

        private static void AppendTableCell(StringBuilder builder, string value, int width)
        {
            builder.Append(value);
            builder.Append(' ', width - value.Length);
        }

        private sealed class StreamConsoleOutput : IConsoleOutput
        {
            private readonly Writable _stream;

            public StreamConsoleOutput(Writable stream)
            {
                _stream = stream;
            }

            public void Write(string text)
            {
                _stream.write(text);
            }

            public void WriteLine(string line)
            {
                _stream.write(line + "\n");
            }
        }
    }
}
