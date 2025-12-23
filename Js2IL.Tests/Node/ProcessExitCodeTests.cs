using System;
using System.Threading.Tasks;
using JavaScriptRuntime;
using Xunit;
using JavaScriptRuntime.DependencyInjection;

namespace Js2IL.Tests.Node
{
    public class ProcessExitCodeTests
    {
        [Fact]
        public void Process_exitCode_getter_setter_mirrors_Environment()
        {
            // arrange
            var prev = Environment.ExitCode;
            var serviceProvider = RuntimeServices.BuildServiceProvider();
            try
            {
                // Ensure we're using the default environment that mirrors System.Environment.ExitCode
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
