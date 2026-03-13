using System;

namespace JavaScriptRuntime;

/// <summary>
/// A tagged value type that stores JavaScript primitive values (number, boolean, string)
/// without boxing on the heap. Reference values (object, string) are stored as object references.
/// This avoids allocations when building <see cref="JsObject"/> property stores.
/// </summary>
public readonly struct JsValue
{
    private const byte TagUndefined = 0;
    private const byte TagNull = 1;
    private const byte TagNumber = 2;
    private const byte TagBoolean = 3;
    private const byte TagString = 4;
    private const byte TagObject = 5;

    private readonly byte _tag;
    private readonly double _dblValue;   // used for number (tag=2) and boolean (tag=3, 0.0=false 1.0=true)
    private readonly object? _refValue;  // used for string (tag=4) and object (tag=5)

    private JsValue(byte tag, double dblValue, object? refValue)
    {
        _tag = tag;
        _dblValue = dblValue;
        _refValue = refValue;
    }

    public static readonly JsValue Undefined = new(TagUndefined, 0.0, null);
    public static readonly JsValue Null = new(TagNull, 0.0, null);

    public static JsValue FromNumber(double value) => new(TagNumber, value, null);
    public static JsValue FromBoolean(bool value) => new(TagBoolean, value ? 1.0 : 0.0, null);
    public static JsValue FromString(string? value) => value is null ? Null : new(TagString, 0.0, value);

    /// <summary>
    /// Creates a <see cref="JsValue"/> from an arbitrary CLR object.
    /// Unwraps boxed doubles and booleans to avoid redundant heap allocation.
    /// </summary>
    public static JsValue FromObject(object? value) => value switch
    {
        null => Undefined,
        JsNull => Null,
        double d => FromNumber(d),
        bool b => FromBoolean(b),
        string s => new JsValue(TagString, 0.0, s),
        _ => new JsValue(TagObject, 0.0, value)
    };

    public bool IsUndefined => _tag == TagUndefined;
    public bool IsNull => _tag == TagNull;
    public bool IsNumber => _tag == TagNumber;
    public bool IsBoolean => _tag == TagBoolean;
    public bool IsString => _tag == TagString;
    public bool IsObject => _tag == TagObject;

    public double AsNumber => _dblValue;
    public bool AsBoolean => _dblValue != 0.0;
    public string? AsString => (string?)_refValue;

    /// <summary>
    /// Converts the <see cref="JsValue"/> to a CLR <see cref="object"/>.
    /// Numbers and booleans are boxed here (heap allocation for these types).
    /// Use the typed accessors (<see cref="AsNumber"/>, <see cref="AsBoolean"/>) to avoid boxing.
    /// </summary>
    public object? ToObject() => _tag switch
    {
        TagUndefined => null,
        TagNull => JsNull.Null,
        TagNumber => (object)_dblValue,
        TagBoolean => (object)(_dblValue != 0.0),
        TagString => _refValue,
        TagObject => _refValue,
        _ => null
    };
}
