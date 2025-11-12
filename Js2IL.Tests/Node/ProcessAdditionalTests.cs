using System.Threading.Tasks;
using System;

namespace Js2IL.Tests.Node
{
    public class ProcessAdditionalTests : ExecutionTestsBase
    {
        public ProcessAdditionalTests() : base("Node") { }

        [Fact]
        public Task Process_Exit_Code_Sets_ExitCode()
            => ExecutionTest(
                nameof(Process_Exit_Code_Sets_ExitCode),
                allowUnhandledException: true,
                configureSettings: _ => { },
                preferOutOfProc: false);

        [Fact]
        public Task Process_Exit_Uses_Current_ExitCode()
            => ExecutionTest(
                nameof(Process_Exit_Uses_Current_ExitCode),
                allowUnhandledException: true,
                configureSettings: _ => { },
                preferOutOfProc: false);
    }
}
