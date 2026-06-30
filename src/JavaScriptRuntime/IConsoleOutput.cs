using System;

namespace JavaScriptRuntime
{
    public interface IConsoleOutput
    {
        void Write(string text);

        void WriteLine(string line);
    }

    public class DefaultConsoleOutput : IConsoleOutput
    {
        public void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }

        public void Write(string text)
        {
            System.Console.Write(text);
        }
    }

    public class DefaultErrorConsoleOutput : IConsoleOutput
    {
        public void WriteLine(string line)
        {
            System.Console.Error.WriteLine(line);
        }

        public void Write(string text)
        {
            System.Console.Error.Write(text);
        }
    }
}
