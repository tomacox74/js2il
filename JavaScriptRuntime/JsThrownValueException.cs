using System;

namespace JavaScriptRuntime;

/// <summary>
/// Wrapper exception used to model JavaScript semantics where any value can be thrown.
/// If a thrown value is not a CLR Exception, the compiler wraps it in this exception.
/// </summary>
public sealed class JsThrownValueException : Exception
{
    public JsThrownValueException(object? value)
        : base("JavaScript threw a non-exception value")
    {
        Value = value;
    }

    public object? Value { get; }
}
