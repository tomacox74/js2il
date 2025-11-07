using System;
using System.Threading.Tasks;
using JavaScriptRuntime;
using Xunit;

namespace Js2IL.Tests.Node
{
    public class ProcessExitCodeTests
    {
        [Fact]
        public void Process_exitCode_getter_setter_mirrors_Environment()
        {
            // arrange
            var prev = Environment.ExitCode;
            var prevEnv = EnvironmentProvider.Current;
            try
            {
                // Ensure we're using the default environment that mirrors System.Environment.ExitCode
                EnvironmentProvider.SetEnvironment(new DefaultEnvironment());
                Environment.ExitCode = 0;
                GlobalVariables.process.exitCode = 7;
                Assert.Equal(7, Environment.ExitCode);
                Assert.Equal(7, GlobalVariables.process.exitCode);

                GlobalVariables.process.exitCode = 0;
                Assert.Equal(0, Environment.ExitCode);
                Assert.Equal(0, GlobalVariables.process.exitCode);
            }
            finally
            {
                Environment.ExitCode = prev;
                EnvironmentProvider.SetEnvironment(prevEnv);
            }
        }
    }
}
