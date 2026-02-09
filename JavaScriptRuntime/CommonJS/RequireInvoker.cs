using System;

namespace JavaScriptRuntime.CommonJS;

/// <summary>
/// Helper for invoking the injected CommonJS <c>require</c> binding.
///
/// The compiled module wrapper stores <c>require</c> as a dynamic JS value (typically <see cref="RequireDelegate"/>).
/// This helper provides a small, non-reflective type check so generated IL can avoid routing through
/// <see cref="JavaScriptRuntime.Closure.InvokeWithArgs"/>.
/// </summary>
public static class RequireInvoker
{
    public static RequireDelegate EnsureRequireDelegate(object? value)
    {
        if (value is RequireDelegate d)
        {
            return d;
        }

        throw new ArgumentException(
            $"Unsupported callable type for function call: target type = {value?.GetType() ?? typeof(object)}, args length = 1",
            nameof(value));
    }
}
