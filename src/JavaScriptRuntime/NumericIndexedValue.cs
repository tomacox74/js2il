namespace JavaScriptRuntime;

/// <summary>
/// Carries an indexed-read result to its numeric coercion point without boxing numeric values.
/// Non-numeric values remain deferred so observable ToNumber behavior keeps JavaScript ordering.
/// </summary>
public readonly struct NumericIndexedValue
{
    private static readonly object Undefined = new();

    private readonly object? _value;
    private readonly double _number;

    private NumericIndexedValue(double number)
    {
        _value = null;
        _number = number;
    }

    private NumericIndexedValue(object? value)
    {
        _value = value ?? Undefined;
        _number = default;
    }

    internal static NumericIndexedValue FromValue(object? value)
        => value is double number
            ? new NumericIndexedValue(number)
            : new NumericIndexedValue(value);

    internal static NumericIndexedValue FromNumber(double number)
        => new(number);

    public static double ToNumber(NumericIndexedValue value)
    {
        if (value._value == null)
        {
            return value._number;
        }

        return TypeUtilities.ToNumber(ReferenceEquals(value._value, Undefined) ? null : value._value);
    }
}
