using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Js2IL.Runtime;

internal class JsHandleProxy : DispatchProxy
{
    private JsRuntimeInstance? _runtime;
    private object? _target;
    private int _disposed;

    internal void Initialize(JsRuntimeInstance runtime, object? target)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(target);

        _runtime = runtime;
        _target = target;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null)
        {
            throw new ArgumentNullException(nameof(targetMethod));
        }

        if (targetMethod.DeclaringType == typeof(IDisposable) && targetMethod.Name == nameof(IDisposable.Dispose))
        {
            DisposeHandle();
            return null;
        }

        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(targetMethod.DeclaringType?.FullName ?? nameof(JsHandleProxy));
        }

        var runtime = _runtime ?? throw new ObjectDisposedException(nameof(JsHandleProxy));
        var target = _target ?? throw new ObjectDisposedException(nameof(JsHandleProxy));

        if (targetMethod.DeclaringType == typeof(object))
        {
            return HandleObjectMethod(targetMethod, args);
        }

        if (targetMethod.IsSpecialName)
        {
            if (targetMethod.Name.StartsWith("get_", StringComparison.Ordinal))
            {
                var name = targetMethod.Name.Substring(4);
                return runtime.Invoke(() =>
                {
                    var value = ExportMemberResolver.GetExportMember(target, name);
                    return JsReturnConverter.ConvertReturn(runtime, value, targetMethod.ReturnType);
                });
            }

            if (targetMethod.Name.StartsWith("set_", StringComparison.Ordinal))
            {
                throw new NotSupportedException("Handle properties are read-only via the hosting API.");
            }
        }

        return runtime.Invoke(() =>
        {
            var result = ExportMemberResolver.InvokeInstanceMethod(target, targetMethod.Name, args ?? Array.Empty<object?>());
            return JsReturnConverter.ConvertReturn(runtime, result, targetMethod.ReturnType);
        });
    }

    private void DisposeHandle()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        Interlocked.Exchange(ref _runtime, null);
        Interlocked.Exchange(ref _target, null);
    }

    private object? HandleObjectMethod(MethodInfo targetMethod, object?[]? args)
    {
        return targetMethod.Name switch
        {
            nameof(ToString) => nameof(JsHandleProxy),
            nameof(GetHashCode) => base.GetHashCode(),
            nameof(Equals) => ReferenceEquals(this, args != null && args.Length > 0 ? args[0] : null),
            _ => null,
        };
    }
}

internal class JsConstructorProxy : DispatchProxy
{
    private JsRuntimeInstance? _runtime;
    private object? _constructor;
    private int _disposed;

    internal void Initialize(JsRuntimeInstance runtime, object? constructor)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(constructor);

        _runtime = runtime;
        _constructor = constructor;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null)
        {
            throw new ArgumentNullException(nameof(targetMethod));
        }

        if (targetMethod.DeclaringType == typeof(IDisposable) && targetMethod.Name == nameof(IDisposable.Dispose))
        {
            DisposeHandle();
            return null;
        }

        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(targetMethod.DeclaringType?.FullName ?? nameof(JsConstructorProxy));
        }

        var runtime = _runtime ?? throw new ObjectDisposedException(nameof(JsConstructorProxy));
        var constructor = _constructor ?? throw new ObjectDisposedException(nameof(JsConstructorProxy));

        if (targetMethod.DeclaringType == typeof(object))
        {
            return HandleObjectMethod(targetMethod, args);
        }

        if (targetMethod.Name != nameof(IJsConstructor<IJsHandle>.Construct))
        {
            throw new MissingMethodException($"Constructor proxy does not support method '{targetMethod.Name}'.");
        }

        return runtime.Invoke(() =>
        {
            // DispatchProxy passes method arguments as an object?[] with one entry per parameter.
            // For a params method like Construct(params object?[] args), that means:
            //   args.Length == 1 and args[0] is the actual params array.
            var ctorArgs = (args != null && args.Length == 1 && args[0] is object?[] a)
                ? a
                : (args ?? Array.Empty<object?>());

            var result = ExportMemberResolver.Construct(constructor, ctorArgs);
            return JsReturnConverter.ConvertReturn(runtime, result, targetMethod.ReturnType);
        });
    }

    private void DisposeHandle()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        Interlocked.Exchange(ref _runtime, null);
        Interlocked.Exchange(ref _constructor, null);
    }

    private object? HandleObjectMethod(MethodInfo targetMethod, object?[]? args)
    {
        return targetMethod.Name switch
        {
            nameof(ToString) => nameof(JsConstructorProxy),
            nameof(GetHashCode) => base.GetHashCode(),
            nameof(Equals) => ReferenceEquals(this, args != null && args.Length > 0 ? args[0] : null),
            _ => null,
        };
    }
}
