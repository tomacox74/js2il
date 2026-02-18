using System;
using JavaScriptRuntime;
using Xunit;
using JavaScriptRuntime.DependencyInjection;

namespace Js2IL.Tests.Node.Process
{
    public class ProcessExitCodeTests
    {
        [Fact]
        public void Process_exitCode_getter_setter_mirrors_Environment()
        {
            var prev = Environment.ExitCode;
            var serviceProvider = RuntimeServices.BuildServiceProvider();
            try
            {
                GlobalThis.ServiceProvider = serviceProvider;
                Environment.ExitCode = 0;
                GlobalThis.process.exitCode = 7;
                Assert.Equal(7, Environment.ExitCode);
                Assert.Equal(7, GlobalThis.process.exitCode);

                GlobalThis.process.exitCode = 0;
                Assert.Equal(0, Environment.ExitCode);
                Assert.Equal(0, GlobalThis.process.exitCode);
            }
            finally
            {
                Environment.ExitCode = prev;
                GlobalThis.ServiceProvider = null;
            }
        }
    }
}
