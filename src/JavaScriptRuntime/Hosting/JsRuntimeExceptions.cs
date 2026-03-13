using System.Reflection;
using JavaScriptRuntime;

namespace Js2IL.Runtime;

/// <summary>
/// Base type for all exceptions intentionally surfaced by the hosting API.
/// </summary>
public class JsRuntimeException : Exception
{
    public string? ModuleId { get; }
    public string? MemberName { get; }
    public Type? ContractType { get; }
    public string? CompiledAssemblyName { get; }

    public JsRuntimeException(
        string message,
        Exception? innerException = null,
        string? moduleId = null,
        string? memberName = null,
        Type? contractType = null,
        string? compiledAssemblyName = null)
        : base(message, innerException)
    {
        ModuleId = moduleId;
        MemberName = memberName;
        ContractType = contractType;
        CompiledAssemblyName = compiledAssemblyName;
    }
}

public sealed class JsModuleLoadException : JsRuntimeException
{
    public JsModuleLoadException(
        string message,
        Exception? innerException = null,
        string? moduleId = null,
        Type? contractType = null,
        string? compiledAssemblyName = null)
        : base(message, innerException, moduleId: moduleId, contractType: contractType, compiledAssemblyName: compiledAssemblyName)
    {
    }
}

public sealed class JsContractProjectionException : JsRuntimeException
{
    public JsContractProjectionException(
        string message,
        Exception? innerException = null,
        string? moduleId = null,
        string? memberName = null,
        Type? contractType = null,
        string? compiledAssemblyName = null)
        : base(message, innerException, moduleId: moduleId, memberName: memberName, contractType: contractType, compiledAssemblyName: compiledAssemblyName)
    {
    }
}

public sealed class JsInvocationException : JsRuntimeException
{
    public JsInvocationException(
        string message,
        Exception? innerException = null,
        string? moduleId = null,
        string? memberName = null,
        Type? contractType = null,
        string? compiledAssemblyName = null)
        : base(message, innerException, moduleId: moduleId, memberName: memberName, contractType: contractType, compiledAssemblyName: compiledAssemblyName)
    {
    }
}

public sealed class JsErrorException : JsRuntimeException
{
    public string? JsName { get; }
    public string? JsMessage { get; }
    public string? JsStack { get; }
    public object? ThrownValue { get; }

    public JsErrorException(
        string message,
        Exception? innerException = null,
        string? moduleId = null,
        string? memberName = null,
        Type? contractType = null,
        string? compiledAssemblyName = null,
        string? jsName = null,
        string? jsMessage = null,
        string? jsStack = null,
        object? thrownValue = null)
        : base(message, innerException, moduleId: moduleId, memberName: memberName, contractType: contractType, compiledAssemblyName: compiledAssemblyName)
    {
        JsName = jsName;
        JsMessage = jsMessage;
        JsStack = jsStack;
        ThrownValue = thrownValue;
    }
}

internal static class JsHostingExceptionTranslator
{
    internal static Exception TranslateModuleLoad(Exception exception, Assembly compiledAssembly, string moduleId, Type? contractType = null)
    {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentNullException.ThrowIfNull(compiledAssembly);
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);

        moduleId = moduleId.Trim();

        var ex = Unwrap(exception);

        // Keep common argument/lifetime exceptions as-is.
        if (ex is ArgumentException or ObjectDisposedException)
        {
            return ex;
        }

        var assemblyName = compiledAssembly.GetName().Name;
        var cause = TranslateCause(ex, moduleId: moduleId, memberName: null, contractType: contractType, compiledAssemblyName: assemblyName);

        return new JsModuleLoadException(
            $"Failed to load/evaluate module '{moduleId}'.",
            innerException: cause,
            moduleId: moduleId,
            contractType: contractType,
            compiledAssemblyName: assemblyName);
    }

    internal static Exception TranslateProxyCall(Exception exception, JsRuntimeInstance runtime, string? memberName, Type? contractType)
    {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentNullException.ThrowIfNull(runtime);

        var ex = Unwrap(exception);

        if (ex is JsRuntimeException)
        {
            return ex;
        }

        // Keep common argument/lifetime exceptions as-is.
        if (ex is ArgumentException or ObjectDisposedException)
        {
            return ex;
        }

        var moduleId = runtime.ModuleId;
        var assemblyName = runtime.CompiledAssemblyName;

        if (IsProjectionFailure(ex))
        {
            return new JsContractProjectionException(
                memberName == null
                    ? $"Failed to project module exports onto '{contractType?.FullName ?? "<unknown contract>"}'."
                    : $"Failed to project export member '{memberName}'.",
                innerException: ex,
                moduleId: moduleId,
                memberName: memberName,
                contractType: contractType,
                compiledAssemblyName: assemblyName);
        }

        var cause = TranslateCause(ex, moduleId: moduleId, memberName: memberName, contractType: contractType, compiledAssemblyName: assemblyName);

        return new JsInvocationException(
            memberName == null ? "JavaScript invocation failed." : $"JavaScript invocation of '{memberName}' failed.",
            innerException: cause,
            moduleId: moduleId,
            memberName: memberName,
            contractType: contractType,
            compiledAssemblyName: assemblyName);
    }

    private static Exception TranslateCause(Exception ex, string? moduleId, string? memberName, Type? contractType, string? compiledAssemblyName)
    {
        if (ex is Error jsError)
        {
            return new JsErrorException(
                memberName == null
                    ? $"JavaScript Error thrown while evaluating module '{moduleId}'."
                    : $"JavaScript Error thrown while invoking '{memberName}'.",
                innerException: ex,
                moduleId: moduleId,
                memberName: memberName,
                contractType: contractType,
                compiledAssemblyName: compiledAssemblyName,
                jsName: jsError.Name,
                jsMessage: jsError.Message,
                jsStack: jsError.stack);
        }

        if (ex is JsThrownValueException thrown)
        {
            return new JsErrorException(
                memberName == null
                    ? $"JavaScript threw a non-error value while evaluating module '{moduleId}'."
                    : $"JavaScript threw a non-error value while invoking '{memberName}'.",
                innerException: ex,
                moduleId: moduleId,
                memberName: memberName,
                contractType: contractType,
                compiledAssemblyName: compiledAssemblyName,
                thrownValue: thrown.Value);
        }

        return ex;
    }

    private static Exception Unwrap(Exception exception)
    {
        if (exception is TargetInvocationException tie && tie.InnerException != null)
        {
            return tie.InnerException;
        }

        return exception;
    }

    private static bool IsProjectionFailure(Exception ex)
    {
        return ex is MissingMemberException
            or MissingMethodException
            or InvalidCastException
            or NotSupportedException
            or InvalidOperationException;
    }
}
