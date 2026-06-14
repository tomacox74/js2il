using System;
using System.Collections.Generic;
using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;
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

        private class DualTestConsoleOutput : IConsoleOutput
        {
            public List<string> StdOut = new();
            public List<string> StdErr = new();
            private readonly bool _isErr;
            public DualTestConsoleOutput(bool isErr) { _isErr = isErr; }
            public void WriteLine(string line)
            {
                if (_isErr) StdErr.Add(line); else StdOut.Add(line);
            }
        }

        [Fact]
        public void Log_PrintsAllArgumentsWithSpaces()
        {
            var testOutput = new TestConsoleOutput();
            var console = CreateConsole(testOutput);

            console.Log("Hello", "World", 42, JavaScriptRuntime.JsNull.Null);

            Assert.Single(testOutput.Output);
            Assert.Equal("Hello World 42 null", testOutput.Output[0]);
        }

        [Fact]
        public void Log_PrintsLiteralBraces()
        {
            var testOutput = new TestConsoleOutput();
            var console = CreateConsole(testOutput);

            console.Log("Hello, {0}", "World");

            Assert.Single(testOutput.Output);
            Assert.Equal("Hello, {0} World", testOutput.Output[0]);
        }

        [Fact]
        public void Log_PrintsBlankLineWhenNoArguments()
        {
            var testOutput = new TestConsoleOutput();
            var console = CreateConsole(testOutput);

            console.Log();

            Assert.Single(testOutput.Output);
            Assert.Equal(string.Empty, testOutput.Output[0]);
        }

        [Fact]
        public void Log_PrintsStringAndFloat()
        {
            var testOutput = new TestConsoleOutput();
            var console = CreateConsole(testOutput);

            // Use a double literal to match JS number semantics (R8)
            console.Log("Value:", 42d);

            Assert.Single(testOutput.Output);
            Assert.Equal("Value: 42", testOutput.Output[0]);
        }

        [Fact]
        public void Error_PrintsAllArgumentsWithSpaces_ToStdErr()
        {
            var stdout = new DualTestConsoleOutput(false);
            var stderr = new DualTestConsoleOutput(true);
            var console = CreateConsole(stdout, stderr);

            console.Error("Hello", "World", 42d, JavaScriptRuntime.JsNull.Null);

            Assert.Empty(stdout.StdOut);
            Assert.Single(stderr.StdErr);
            Assert.Equal("Hello World 42 null", stderr.StdErr[0]);
        }

        [Fact]
        public void Warn_PrintsAllArgumentsWithSpaces_ToStdErr()
        {
            var stdout = new DualTestConsoleOutput(false);
            var stderr = new DualTestConsoleOutput(true);
            var console = CreateConsole(stdout, stderr);

            console.Warn("Be", "careful", 7d);

            Assert.Empty(stdout.StdOut);
            Assert.Single(stderr.StdErr);
            Assert.Equal("Be careful 7", stderr.StdErr[0]);
        }

        [Fact]
        public void Log_PrintsExpandoObjectProperties()
        {
            var testOutput = new TestConsoleOutput();
            var console = CreateConsole(testOutput);

            dynamic expando = new System.Dynamic.ExpandoObject();
            expando.name = "Alice";
            expando.age = 31;

            console.Log("x is", expando);

            Assert.Single(testOutput.Output);
            // The expected output should match the Node.js style: x is { name: 'Alice', age: 31 }
            Assert.Equal("x is { name: 'Alice', age: 31 }", testOutput.Output[0]);
        }

        private static Console CreateConsole(IConsoleOutput? output = null, IConsoleOutput? errorOutput = null)
        {
            var container = new ServiceContainer();
            var sinks = new ConsoleOutputSinks
            {
                Output = output,
                ErrorOutput = errorOutput
            };
            container.RegisterInstance(sinks);

            return container.Resolve<Console>();
        }
    }
}
