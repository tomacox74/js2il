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

            Console.Log("Hello", "World", 42, JavaScriptRuntime.JsNull.Null);

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

        [Fact]
        public void Log_PrintsBlankLineWhenNoArguments()
        {
            var testOutput = new TestConsoleOutput();
            Console.SetOutput(testOutput);

            Console.Log();

            Assert.Single(testOutput.Output);
            Assert.Equal(string.Empty, testOutput.Output[0]);
        }

        [Fact]
        public void Log_PrintsStringAndFloat()
        {
            var testOutput = new TestConsoleOutput();
            Console.SetOutput(testOutput);

            // Use a double literal to match JS number semantics (R8)
            Console.Log("Value:", 42d);

            Assert.Single(testOutput.Output);
            Assert.Equal("Value: 42", testOutput.Output[0]);
        }

        [Fact]
        public void Log_PrintsExpandoObjectProperties()
        {
            var testOutput = new TestConsoleOutput();
            Console.SetOutput(testOutput);

            dynamic expando = new System.Dynamic.ExpandoObject();
            expando.name = "Alice";
            expando.age = 31;

            Console.Log("x is", expando);

            Assert.Single(testOutput.Output);
            // The expected output should match the Node.js style: x is { name: 'Alice', age: 31 }
            Assert.Equal("x is { name: 'Alice', age: 31 }", testOutput.Output[0]);
        }
    }
}
