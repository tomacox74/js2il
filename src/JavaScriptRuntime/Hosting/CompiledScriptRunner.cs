using System.ComponentModel;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;
using JavaScriptRuntime.Modules.CommonJS;
using JavaScriptRuntime.Node;

namespace Jroc.Runtime;

/// <summary>
/// Runtime entry point used by compiler-generated script facades.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class CompiledScriptRunner
{
    public static object Import(
        Type exportsContractType,
        ModuleMainDelegate scriptEntryPoint,
        string moduleId)
    {
        ArgumentNullException.ThrowIfNull(exportsContractType);
        ArgumentNullException.ThrowIfNull(scriptEntryPoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);

        var compiledAssembly = scriptEntryPoint.Method.Module.Assembly;
        return JsEngine.LoadModule(exportsContractType, compiledAssembly, moduleId);
    }

    public static void Run(
        ModuleMainDelegate scriptEntryPoint,
        string moduleId,
        string[] args)
    {
        ArgumentNullException.ThrowIfNull(scriptEntryPoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);
        ArgumentNullException.ThrowIfNull(args);

        for (var index = 0; index < args.Length; index++)
        {
            if (args[index] is null)
            {
                throw new ArgumentException(
                    $"Run argument at index {index} cannot be null.",
                    nameof(args));
            }
        }

        var compiledAssembly = scriptEntryPoint.Method.Module.Assembly;
        var compiledAssemblyPath = string.IsNullOrWhiteSpace(compiledAssembly.Location)
            ? null
            : compiledAssembly.Location;
        var environment = new ScriptRunEnvironment(moduleId, args);
        var existingServices = Engine._serviceProviderOverride.Value;

        try
        {
            using var lifecycle = RuntimeLifecycle.Create(
                compiledAssembly,
                isHostedExecution: true,
                compiledAssemblyPath: compiledAssemblyPath,
                existingServices: existingServices,
                configureServices: existingServices is null
                    ? services => services.RegisterInstance<IEnvironment>(environment)
                    : null,
                suppressInheritedExecutionContext: true);

            try
            {
                lifecycle.Execute(
                    services =>
                    {
                        Engine.ConfigureChildProcessIpc(services);
                        var moduleExecutor = new ModuleExecutor(services);
                        var forkEntryModule = System.Environment.GetEnvironmentVariable(
                            ChildProcessRuntimeOptions.ForkEntryModuleEnvVar);
                        if (!string.IsNullOrWhiteSpace(forkEntryModule))
                        {
                            moduleExecutor.Execute(scriptEntryPoint, forkEntryModule);
                        }
                        else
                        {
                            moduleExecutor.Execute(scriptEntryPoint);
                        }
                    },
                    waitForTimers: true);
                lifecycle.Services
                    .Resolve<UnhandledPromiseRejectionTracker>()
                    .ThrowIfUnhandled();
            }
            catch (ScriptProcessExitException)
            {
                // process.exit() intentionally stops evaluation and event-loop draining.
            }

            if (environment.ExitCode != 0)
            {
                throw new JsScriptRunException(
                    $"Module '{moduleId}' completed with exit code {environment.ExitCode}.",
                    exitCode: environment.ExitCode,
                    moduleId: moduleId,
                    compiledAssemblyName: compiledAssembly.GetName().Name);
            }
        }
        catch (JsScriptRunException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw JsHostingExceptionTranslator.TranslateScriptRun(
                exception,
                compiledAssembly,
                moduleId);
        }
        finally
        {
            ScriptProcessExitControl.Clear();
        }
    }

    private sealed class ScriptRunEnvironment :
        IEnvironment,
        INodeProcessArgumentsEnvironment,
        IUnhandledPromiseRejectionEnvironment
    {
        private readonly string[] _arguments;

        internal ScriptRunEnvironment(string moduleId, IReadOnlyList<string> args)
        {
            _arguments = new string[args.Count + 2];
            _arguments[0] = "jroc";
            _arguments[1] = moduleId;
            for (var index = 0; index < args.Count; index++)
            {
                _arguments[index + 2] = args[index];
            }
        }

        public int ExitCode { get; set; }

        public string[] GetCommandLineArgs() => (string[])_arguments.Clone();

        string[] INodeProcessArgumentsEnvironment.GetNodeProcessArguments() =>
            (string[])_arguments.Clone();

        public void Exit(int code)
        {
            ExitCode = code;
            ScriptProcessExitControl.RequestExit();
        }

        public void Exit()
        {
            ScriptProcessExitControl.RequestExit();
        }
    }
}
