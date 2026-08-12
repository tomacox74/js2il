namespace JavaScriptRuntime;

/// <summary>
/// Allocation-conscious JavaScript argument transport for dynamic calls.
/// </summary>
public readonly struct JsCallArguments
{
    public const int InlineCapacity = 5;

    private readonly object?[]? _array;
    private readonly object? _argument0;
    private readonly object? _argument1;
    private readonly object? _argument2;
    private readonly object? _argument3;
    private readonly object? _argument4;
    private readonly byte _inlineCount;

    private JsCallArguments(
        byte inlineCount,
        object? argument0 = null,
        object? argument1 = null,
        object? argument2 = null,
        object? argument3 = null,
        object? argument4 = null)
    {
        _array = null;
        _argument0 = argument0;
        _argument1 = argument1;
        _argument2 = argument2;
        _argument3 = argument3;
        _argument4 = argument4;
        _inlineCount = inlineCount;
    }

    private JsCallArguments(object?[] arguments)
    {
        _array = arguments;
        _argument0 = null;
        _argument1 = null;
        _argument2 = null;
        _argument3 = null;
        _argument4 = null;
        _inlineCount = 0;
    }

    public static JsCallArguments Empty => default;

    public int Count => _array?.Length ?? _inlineCount;

    public bool UsesArrayStorage => _array is not null;

    public static JsCallArguments From(object? argument0)
        => new(1, argument0);

    public static JsCallArguments From(object? argument0, object? argument1)
        => new(2, argument0, argument1);

    public static JsCallArguments From(object? argument0, object? argument1, object? argument2)
        => new(3, argument0, argument1, argument2);

    public static JsCallArguments From(
        object? argument0,
        object? argument1,
        object? argument2,
        object? argument3)
        => new(4, argument0, argument1, argument2, argument3);

    public static JsCallArguments From(
        object? argument0,
        object? argument1,
        object? argument2,
        object? argument3,
        object? argument4)
        => new(5, argument0, argument1, argument2, argument3, argument4);

    public static JsCallArguments FromArray(object?[]? arguments)
        => arguments is null || arguments.Length == 0
            ? Empty
            : new JsCallArguments(arguments);

    public static JsCallArguments Prepend(
        object?[] prefix,
        in JsCallArguments suffix)
    {
        ArgumentNullException.ThrowIfNull(prefix);
        if (prefix.Length == 0)
        {
            return suffix;
        }

        var count = prefix.Length + suffix.Count;
        if (count <= 5)
        {
            var suffixCopy = suffix;
            object? Get(int index)
                => index < prefix.Length
                    ? prefix[index]
                    : suffixCopy.GetArgument(index - prefix.Length);

            return count switch
            {
                1 => From(Get(0)),
                2 => From(Get(0), Get(1)),
                3 => From(Get(0), Get(1), Get(2)),
                4 => From(Get(0), Get(1), Get(2), Get(3)),
                5 => From(Get(0), Get(1), Get(2), Get(3), Get(4)),
                _ => Empty
            };
        }

        var arguments = new object?[count];
        System.Array.Copy(prefix, arguments, prefix.Length);
        for (var index = 0; index < suffix.Count; index++)
        {
            arguments[prefix.Length + index] = suffix.GetArgument(index);
        }
        return FromArray(arguments);
    }

    public object? GetArgument(int index)
    {
        if ((uint)index >= (uint)Count)
        {
            return null;
        }

        if (_array is not null)
        {
            return _array[index];
        }

        return index switch
        {
            0 => _argument0,
            1 => _argument1,
            2 => _argument2,
            3 => _argument3,
            4 => _argument4,
            _ => null
        };
    }

    /// <summary>
    /// Returns the existing arbitrary-argument array or materializes inline arguments.
    /// </summary>
    public object?[] ToArray()
    {
        if (_array is not null)
        {
            return _array;
        }

        return _inlineCount switch
        {
            0 => System.Array.Empty<object?>(),
            1 => [_argument0],
            2 => [_argument0, _argument1],
            3 => [_argument0, _argument1, _argument2],
            4 => [_argument0, _argument1, _argument2, _argument3],
            5 => [_argument0, _argument1, _argument2, _argument3, _argument4],
            _ => throw new InvalidOperationException("Invalid inline JavaScript argument count.")
        };
    }
}
