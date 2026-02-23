namespace JavaScriptRuntime;

internal readonly struct JsValue
{
    private readonly byte _kind;
    private readonly double _numberValue;
    private readonly bool _boolValue;
    private readonly object? _objectValue;

    private JsValue(byte kind, double numberValue, bool boolValue, object? objectValue)
    {
        _kind = kind;
        _numberValue = numberValue;
        _boolValue = boolValue;
        _objectValue = objectValue;
    }

    public static JsValue FromNumber(double value) => new(kind: 1, numberValue: value, boolValue: false, objectValue: null);
    public static JsValue FromBoolean(bool value) => new(kind: 2, numberValue: 0d, boolValue: value, objectValue: null);
    public static JsValue FromObject(object? value) => value switch
    {
        double d => FromNumber(d),
        bool b => FromBoolean(b),
        _ => new JsValue(kind: 3, numberValue: 0d, boolValue: false, objectValue: value)
    };

    public object? ToObject() => _kind switch
    {
        1 => _numberValue,
        2 => _boolValue,
        _ => _objectValue
    };
}
