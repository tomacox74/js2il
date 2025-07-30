using System;
using System.Collections.Generic;
using JavaScriptRuntime;
using Xunit;

namespace JavaScriptRuntime.Tests
{
    public class ConsoleTests
    {
        private class TestConsoleOutput : IConsoleOutput
        {
            public List<string> Output = new List<string>();
            public void WriteLine(string line)
            {
                Output.Add(line);
            }
        }

        [Fact]
        public void Log_PrintsAllArgumentsWithSpaces()
        {
            var testOutput = new TestConsoleOutput();
            Console.SetOutput(testOutput);

            Console.Log("Hello", "World", 42, (object?)null);

            Assert.Single(testOutput.Output);
            Assert.Equal("Hello World 42 null", testOutput.Output[0]);
        }

        [Fact]
        public void Log_PrintsLiteralBraces()
        {
            var testOutput = new TestConsoleOutput();
            Console.SetOutput(testOutput);

            Console.Log("Hello, {0}", "World");

            Assert.Single(testOutput.Output);
            Assert.Equal("Hello, {0} World", testOutput.Output[0]);
        }
    }
}
