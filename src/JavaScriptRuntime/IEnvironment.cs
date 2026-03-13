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
        /// <summary>
        /// Simulates terminating the process with a specific exit code. Test implementations should not actually terminate.
        /// </summary>
        void Exit(int code);
        /// <summary>
        /// Simulates terminating the process using the current exit code. Test implementations should not actually terminate.
        /// </summary>
        void Exit();
        // Add more members here as needed (e.g., CurrentDirectory) when used by runtime code.
    }

    public sealed class DefaultEnvironment : IEnvironment
    {
        public int ExitCode
        {
            get => System.Environment.ExitCode;
            set => System.Environment.ExitCode = value;
        }

        public void Exit(int code)
        {
            // In production/default, terminate the process with the specified code
            System.Environment.ExitCode = code;
            if (!EnvironmentProvider.SuppressExit)
            {
                System.Environment.Exit(code);
            }
        }

        public void Exit()
        {
            // Use the current exit code when exiting without an explicit code
            if (!EnvironmentProvider.SuppressExit)
            {
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }

        public string[] GetCommandLineArgs()
        {
            return System.Environment.GetCommandLineArgs();
        }
    }

    /// <summary>
    /// Non-terminating environment for tests. Records exit code but does not terminate the host process.
    /// </summary>
    public sealed class NonTerminatingEnvironment : IEnvironment
    {
        public int ExitCode { get; set; }

        public void Exit(int code)
        {
            ExitCode = code;
            // Do not terminate during tests
        }

        public void Exit()
        {
            // Do not terminate during tests
        }

        public string[] GetCommandLineArgs()
        {
            return System.Environment.GetCommandLineArgs();
        }
    }

    /// <summary>
    /// Capturing environment for tests that records whether Exit was called and with what code, without terminating.
    /// </summary>
    public sealed class CapturingEnvironment : IEnvironment
    {
        public int ExitCode { get; set; }
        public bool ExitCalled { get; private set; }
        public int? ExitCalledWithCode { get; private set; }

        public void Exit(int code)
        {
            ExitCalled = true;
            ExitCalledWithCode = code;
            ExitCode = code;
            // Do not terminate
        }

        public void Exit()
        {
            ExitCalled = true;
            ExitCalledWithCode = ExitCode;
            // Do not terminate
        }

        public string[] GetCommandLineArgs()
        {
            return System.Environment.GetCommandLineArgs();
        }
    }
}
