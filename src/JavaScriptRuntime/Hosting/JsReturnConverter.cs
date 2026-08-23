using JavaScriptRuntime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Jroc.Runtime;

internal static class JsReturnConverter
{
    private static readonly MethodInfo PromiseToTaskOpenGeneric = typeof(JsPromiseTaskInterop)
        .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        .Single(m => m.Name == nameof(JsPromiseTaskInterop.ToTask)
                     && m.IsGenericMethodDefinition
                     && m.GetParameters().Length == 4
                     && m.GetParameters()[0].ParameterType == typeof(JsRuntimeInstance)
                     && m.GetParameters()[1].ParameterType == typeof(Promise));

    private static readonly MethodInfo TaskFromResultOpenGeneric = typeof(Task)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .Single(m => m.Name == nameof(Task.FromResult)
                     && m.IsGenericMethodDefinition
                     && m.GetParameters().Length == 1);

    private static readonly ConditionalWeakTable<Type, ResultConversionMethods> ResultConversions = new();

    internal static object? ConvertReturn(
        JsRuntimeInstance runtime,
        object? value,
        Type returnType,
        string? memberName = null,
        Type? contractType = null)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(returnType);
        if (!GeneratedContractMetadata.IsGeneratedContractType(contractType))
        {
            memberName = null;
            contractType = null;
        }

        if (returnType == typeof(Task))
        {
            if (value is Promise p)
            {
                return JsPromiseTaskInterop.ToTask(runtime, p, memberName, contractType);
            }

            // If the JS side returns a non-promise value but the contract expects a Task,
            // treat it as already-completed.
            return Task.CompletedTask;
        }

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var resultType = returnType.GetGenericArguments()[0];
            var conversionMethods = ResultConversions.GetValue(
                resultType,
                static type => new ResultConversionMethods(
                    PromiseToTaskOpenGeneric.MakeGenericMethod(type),
                    TaskFromResultOpenGeneric.MakeGenericMethod(type)));

            if (value is Promise p)
            {
                return conversionMethods.PromiseToTask.Invoke(
                    null,
                    new object?[] { runtime, p, memberName, contractType });
            }

            var converted = ConvertReturn(runtime, value, resultType, memberName, contractType);

            return conversionMethods.TaskFromResult.Invoke(
                null,
                new[] { converted });
        }

        if (returnType == typeof(void))
        {
            return null;
        }

        if (value == null)
        {
            return returnType.IsValueType ? Activator.CreateInstance(returnType) : null;
        }

        if (IsJsConstructorType(returnType))
        {
            return runtime.GetOrCreateConstructorProxy(returnType, value);
        }

        if (JavaScriptRuntime.CallableOperations.IsCallable(value))
        {
            var callable = runtime.GetOrCreateCallableWrapper(value);
            if (returnType == typeof(object)
                || returnType == typeof(JsCallable)
                || returnType.IsInstanceOfType(callable))
            {
                return callable;
            }
        }

        if (returnType == typeof(object))
        {
            // An object-typed host contract erases whether the value is a JS reference.
            // Keep runtime objects behind the dynamic hosting boundary instead of leaking them.
            return JsDynamicValueProxy.Wrap(runtime, value);
        }

        if (returnType.IsInstanceOfType(value))
        {
            return value;
        }

        if (typeof(IJsHandle).IsAssignableFrom(returnType) || IsGeneratedHandleType(returnType))
        {
            return runtime.GetOrCreateHandleProxy(returnType, value);
        }

        if (returnType.IsEnum)
        {
            try
            {
                return Enum.ToObject(returnType, value);
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidCastException or FormatException or OverflowException)
            {
                throw new InvalidCastException(
                    $"Failed to convert return value '{value}' ({value.GetType().FullName}) to enum '{returnType.FullName}'.",
                    ex);
            }
        }

        try
        {
            return Convert.ChangeType(value, returnType);
        }
        catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
        {
            throw new InvalidCastException(
                $"Failed to convert return value '{value}' ({value.GetType().FullName}) to '{returnType.FullName}'.",
                ex);
        }
    }

    private static bool IsJsConstructorType(Type returnType)
    {
        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(IJsConstructor<>))
        {
            return true;
        }

        return returnType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IJsConstructor<>))
            || IsGeneratedConstructorType(returnType);
    }

    private static bool IsGeneratedConstructorType(Type returnType)
        => GeneratedContractMetadata.IsGeneratedContractType(returnType)
           && returnType.IsInterface
           && typeof(IDisposable).IsAssignableFrom(returnType)
           && returnType.GetMethods().Any(method => method.Name == "Construct");

    private static bool IsGeneratedHandleType(Type returnType)
        => GeneratedContractMetadata.IsGeneratedContractType(returnType)
           && returnType.IsInterface
           && typeof(IDisposable).IsAssignableFrom(returnType);

    private sealed record ResultConversionMethods(
        MethodInfo PromiseToTask,
        MethodInfo TaskFromResult);
}
