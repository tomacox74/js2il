using System;

namespace JavaScriptRuntime
{
    // Abstraction over System.Environment for testing and substitution.
    public interface IEnvironment
    {
        int ExitCode { get; set; }
        /// <summary>
        /// Returns the command-line arguments for the current process.
        /// Mirrors System.Environment.GetCommandLineArgs().
        /// </summary>
        string[] GetCommandLineArgs();
        // Add more members here as needed (e.g., CurrentDirectory) when used by runtime code.
    }

    public sealed class DefaultEnvironment : IEnvironment
    {
        public int ExitCode
        {
            get => System.Environment.ExitCode;
            set => System.Environment.ExitCode = value;
        }

        public string[] GetCommandLineArgs()
        {
            return System.Environment.GetCommandLineArgs();
        }
    }
}
