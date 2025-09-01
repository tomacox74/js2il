using System;
using System.Threading.Tasks;
using JavaScriptRuntime.Node;
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
            try
            {
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
            }
        }
    }
}
