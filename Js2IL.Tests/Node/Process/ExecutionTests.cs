using System;
using System.Threading.Tasks;

namespace Js2IL.Tests.Node.Process
{
    public class ExecutionTests : ExecutionTestsBase
    {
        public ExecutionTests() : base("Node/Process") { }

        [Fact]
        public Task Environment_EnumerateProcessArgV()
            => ExecutionTest(
                nameof(Environment_EnumerateProcessArgV),
                configureSettings: s =>
                {
                    s.AddScrubber(sb => sb.Replace('\\', '/'));
                    var temp = System.IO.Path.GetTempPath().Replace('\\', '/');
                    s.AddScrubber(sb => sb.Replace(temp, "{TempPath}"));
                    s.AddScrubber(sb =>
                    {
                        var text = sb.ToString();
                        var nl = text.IndexOf('\n');
                        var firstLine = nl >= 0 ? text.Substring(0, nl + 1) : text;
                        const string prefix = "argv length is ";
                        sb.Clear();
                        if (firstLine.StartsWith(prefix))
                        {
                            sb.Append(prefix).Append("{N}").Append('\n');
                        }
                        else
                        {
                            sb.Append(firstLine);
                        }
                    });
                });

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

        [Fact]
        public Task Process_Platform_Versions_And_Env_Basics()
            => ExecutionTest(nameof(Process_Platform_Versions_And_Env_Basics));

        [Fact]
        public Task Process_Chdir_And_NextTick_Basics()
            => ExecutionTest(nameof(Process_Chdir_And_NextTick_Basics));

        [Fact]
        public Task Process_Versions_Expanded()
            => ExecutionTest(nameof(Process_Versions_Expanded));
    }
}
