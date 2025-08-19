using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaScriptRuntime
{
    [IntrinsicObject("console")]
    public class Console
    {
        private static IConsoleOutput _output = new DefaultConsoleOutput();

    public static void SetOutput(IConsoleOutput output)
        {
            _output = output ?? new DefaultConsoleOutput();
        }

    public static void Log(params object?[] args)
        {
            var parts = args.Select(arg => DotNet2JSConversions.ToString(arg));
            var line = string.Join(" ", parts);
            _output.WriteLine(line);
        }
    }
}
