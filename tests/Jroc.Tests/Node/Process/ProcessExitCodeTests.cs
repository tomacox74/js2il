using System;
using JavaScriptRuntime;
using Xunit;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests.Node.Process
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
                Assert.Null(GlobalThis.process.exitCode);

                GlobalThis.process.exitCode = 7d;
                Assert.Equal(7, Environment.ExitCode);
                Assert.Equal(7d, GlobalThis.process.exitCode);

                GlobalThis.process.exitCode = "8";
                Assert.Equal(8, Environment.ExitCode);
                Assert.Equal(8d, GlobalThis.process.exitCode);

                GlobalThis.process.exitCode = null;
                Assert.Equal(0, Environment.ExitCode);
                Assert.Null(GlobalThis.process.exitCode);

                Assert.Throws<TypeError>(() => GlobalThis.process.exitCode = true);
                Assert.Throws<RangeError>(() => GlobalThis.process.exitCode = 1.5d);
            }
            finally
            {
                Environment.ExitCode = prev;
                GlobalThis.ServiceProvider = null;
            }
        }
    }
}
