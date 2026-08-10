using System;

namespace JavaScriptRuntime.Node;

[NodeModule("assert")]
public sealed partial class AssertModule : JsFunctionObject
{
    private readonly bool _strictMode;
    private readonly AssertionErrorConstructor _assertionErrorConstructor = new();
    private AssertModule? _strict;

    public AssertModule() : this(false)
    {
    }

    private AssertModule(bool strictMode)
    {
        _strictMode = strictMode;
        this["ok"] = this;
    }

    public object AssertionError => _assertionErrorConstructor;

    public object strict => _strict ??= new AssertModule(strictMode: true);

    protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
    {
        ok(arguments.GetArgument(0), arguments.Count > 1 ? arguments.GetArgument(1) : null);
        return null;
    }

    public object? ok(object? value) => ok(value, null);

    public object? ok(object? value, object? message)
    {
        if (!TypeUtilities.ToBoolean(value))
        {
            ThrowAssertion(value, true, message, "==");
        }

        return null;
    }

    public object? equal(object? actual, object? expected) => equal(actual, expected, null);

    public object? equal(object? actual, object? expected, object? message)
    {
        var equal = _strictMode
            ? Operators.SameValue(actual, expected)
            : Operators.Equal(actual, expected);
        if (!equal)
        {
            ThrowAssertion(actual, expected, message, _strictMode ? "strictEqual" : "==");
        }

        return null;
    }

    public object? notEqual(object? actual, object? expected) => notEqual(actual, expected, null);

    public object? notEqual(object? actual, object? expected, object? message)
    {
        var equal = _strictMode
            ? Operators.SameValue(actual, expected)
            : Operators.Equal(actual, expected);
        if (equal)
        {
            ThrowAssertion(actual, expected, message, _strictMode ? "notStrictEqual" : "!=");
        }

        return null;
    }

    public object? strictEqual(object? actual, object? expected)
        => strictEqual(actual, expected, null);

    public object? strictEqual(object? actual, object? expected, object? message)
    {
        if (!Operators.SameValue(actual, expected))
        {
            ThrowAssertion(actual, expected, message, "strictEqual");
        }

        return null;
    }

    public object? notStrictEqual(object? actual, object? expected)
        => notStrictEqual(actual, expected, null);

    public object? notStrictEqual(object? actual, object? expected, object? message)
    {
        if (Operators.SameValue(actual, expected))
        {
            ThrowAssertion(actual, expected, message, "notStrictEqual");
        }

        return null;
    }

    public object? match(string value, RegExp regexp) => match(value, regexp, null);

    public object? match(string value, RegExp regexp, object? message)
    {
        if (!TypeUtilities.ToBoolean(regexp.test(value)))
        {
            ThrowAssertion(value, regexp, message, "match");
        }

        return null;
    }

    public object? doesNotMatch(string value, RegExp regexp) => doesNotMatch(value, regexp, null);

    public object? doesNotMatch(string value, RegExp regexp, object? message)
    {
        if (TypeUtilities.ToBoolean(regexp.test(value)))
        {
            ThrowAssertion(value, regexp, message, "doesNotMatch");
        }

        return null;
    }

    public object? throws(object? fn) => throws(fn, null, null);

    public object? throws(object? fn, object? error)
        => error is string message ? throws(fn, null, message) : throws(fn, error, null);

    public object? throws(object? fn, object? error, string? message)
    {
        try
        {
            CallableOperations.Call0(fn, null);
        }
        catch (Exception exception)
        {
            if (MatchesExpectedError(exception, error))
            {
                return null;
            }

            throw;
        }

        ThrowAssertion(null, error, message, "throws");
        return null;
    }

    public object? doesNotThrow(object? fn) => doesNotThrow(fn, null, null);

    public object? doesNotThrow(object? fn, object? error)
        => error is string message ? doesNotThrow(fn, null, message) : doesNotThrow(fn, error, null);

    public object? doesNotThrow(object? fn, object? error, string? message)
    {
        try
        {
            CallableOperations.Call0(fn, null);
        }
        catch (Exception exception)
        {
            if (error is null || MatchesExpectedError(exception, error))
            {
                ThrowAssertion(exception, null, message, "doesNotThrow");
            }

            throw;
        }

        return null;
    }

    public object? fail() => fail("Failed");

    public object? fail(object? message)
    {
        ThrowAssertion(null, null, message, "fail");
        return null;
    }

    public object? fail(object? actual, object? expected)
        => fail(actual, expected, null, "!=");

    public object? fail(object? actual, object? expected, object? message)
        => fail(actual, expected, message, "!=");

    public object? fail(object? actual, object? expected, object? message, string @operator)
    {
        ThrowAssertion(actual, expected, message, @operator);
        return null;
    }

    public object? fail(
        object? actual,
        object? expected,
        object? message,
        string @operator,
        object? stackStartFn)
        => fail(actual, expected, message, @operator);

    public object? ifError(object? value)
    {
        if (value is not null and not JsNull)
        {
            ThrowAssertion(value, null, value is Exception exception ? exception.Message : null, "ifError");
        }

        return null;
    }

    private void ThrowAssertion(object? actual, object? expected, object? message, string @operator)
    {
        if (message is Exception exception)
        {
            throw exception;
        }

        var generatedMessage = message is null;
        var text = generatedMessage
            ? $"Expected values to satisfy '{@operator}'"
            : DotNet2JSConversions.ToErrorMessageString(message);
        throw new AssertionError(
            text,
            actual,
            expected,
            @operator,
            generatedMessage,
            _assertionErrorConstructor.Prototype);
    }

    private static bool MatchesExpectedError(Exception exception, object? expected)
    {
        if (expected is null)
        {
            return true;
        }

        if (expected is RegExp regexp)
        {
            return TypeUtilities.ToBoolean(regexp.test(exception.Message));
        }

        return true;
    }
}

public sealed class AssertionError : Error
{
    public AssertionError(
        string message,
        object? actual,
        object? expected,
        string @operator,
        bool generatedMessage,
        object prototype)
        : base(message)
    {
        Name = "AssertionError";
        this.actual = actual;
        this.expected = expected;
        this.@operator = @operator;
        this.generatedMessage = generatedMessage;
        InitializeIntrinsicSurface(prototype);
    }

    public object? actual { get; }

    public object? expected { get; }

    public bool generatedMessage { get; }

    public string code => "ERR_ASSERTION";

    public string @operator { get; }
}

internal sealed class AssertionErrorConstructor : JsFunctionObject
{
    public AssertionErrorConstructor()
    {
        Prototype = new JsObject();
        this["prototype"] = Prototype;
    }

    public JsObject Prototype { get; }

    public override bool IsConstructor => true;

    protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
        => ConstructCore(arguments, this);

    protected override object? ConstructCore(in JsCallArguments arguments, object? newTarget)
        => new AssertionError(
            arguments.Count > 0
                ? DotNet2JSConversions.ToErrorMessageString(arguments.GetArgument(0))
                : string.Empty,
            null,
            null,
            "fail",
            arguments.Count == 0,
            Prototype);
}
