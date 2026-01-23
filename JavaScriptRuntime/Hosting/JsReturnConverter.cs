using JavaScriptRuntime;
using System.Reflection;
using System.Threading.Tasks;

namespace Js2IL.Runtime;

internal static class JsReturnConverter
{
    internal static object? ConvertReturn(JsRuntimeInstance runtime, object? value, Type returnType)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(returnType);

        if (returnType == typeof(Task))
        {
            if (value is Promise p)
            {
                return JsPromiseTaskInterop.ToTask(p);
            }

            // If the JS side returns a non-promise value but the contract expects a Task,
            // treat it as already-completed.
            return Task.CompletedTask;
        }

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var resultType = returnType.GetGenericArguments()[0];

            if (value is Promise p)
            {
                var method = typeof(JsPromiseTaskInterop)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Single(m => m.Name == nameof(JsPromiseTaskInterop.ToTask)
                                 && m.IsGenericMethodDefinition
                                 && m.GetParameters().Length == 2
                                 && m.GetParameters()[0].ParameterType == typeof(JsRuntimeInstance)
                                 && m.GetParameters()[1].ParameterType == typeof(Promise))
                    .MakeGenericMethod(resultType);
                return method.Invoke(null, new object?[] { runtime, p });
            }

            var converted = ConvertReturn(runtime, value, resultType);
            var fromResult = typeof(Task)
                .GetMethods()
                .Single(m => m.Name == nameof(Task.FromResult) && m.IsGenericMethodDefinition)
                .MakeGenericMethod(resultType);
            return fromResult.Invoke(null, new[] { converted });
        }

        if (returnType == typeof(void))
        {
            return null;
        }

        if (value == null)
        {
            return returnType.IsValueType ? Activator.CreateInstance(returnType) : null;
        }

        if (returnType.IsInstanceOfType(value))
        {
            return value;
        }

        if (IsJsConstructorType(returnType))
        {
            var proxy = JsProxyFactory.CreateProxy(returnType, typeof(JsConstructorProxy));
            ((JsConstructorProxy)proxy).Initialize(runtime, value);
            return proxy;
        }

        if (typeof(IJsHandle).IsAssignableFrom(returnType))
        {
            var proxy = JsProxyFactory.CreateProxy(returnType, typeof(JsHandleProxy));
            ((JsHandleProxy)proxy).Initialize(runtime, value);
            return proxy;
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

        return returnType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IJsConstructor<>));
    }
}
