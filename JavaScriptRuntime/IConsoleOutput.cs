using System;

namespace JavaScriptRuntime
{
    public interface IConsoleOutput
    {
        void WriteLine(string line);
    }

    public class DefaultConsoleOutput : IConsoleOutput
    {
        public void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }
    }

    public class DefaultErrorConsoleOutput : IConsoleOutput
    {
        public void WriteLine(string line)
        {
            System.Console.Error.WriteLine(line);
        }
    }
}
