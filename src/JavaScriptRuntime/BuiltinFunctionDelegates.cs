namespace JavaScriptRuntime;

internal delegate object? BuiltinFunction0(object? thisArgument);
internal delegate object? BuiltinFunction1(object? thisArgument, object? argument0);
internal delegate object? BuiltinFunction2(object? thisArgument, object? argument0, object? argument1);
internal delegate object? BuiltinFunction3(object? thisArgument, object? argument0, object? argument1, object? argument2);
internal delegate object? BuiltinFunction4(object? thisArgument, object? argument0, object? argument1, object? argument2, object? argument3);
internal delegate object? BuiltinFunction5(object? thisArgument, object? argument0, object? argument1, object? argument2, object? argument3, object? argument4);
internal delegate object? BuiltinFunctionVariadic(object? thisArgument, in JsCallArguments arguments);

internal static class BuiltinFunctionDelegates
{
    internal static bool IsReceiverAware(Delegate target)
        => target is BuiltinFunction0
            or BuiltinFunction1
            or BuiltinFunction2
            or BuiltinFunction3
            or BuiltinFunction4
            or BuiltinFunction5
            or BuiltinFunctionVariadic;

    internal static bool TryGetLength(Delegate target, out double length)
    {
        length = target switch
        {
            BuiltinFunction0 => 0d,
            BuiltinFunction1 => 1d,
            BuiltinFunction2 => 2d,
            BuiltinFunction3 => 3d,
            BuiltinFunction4 => 4d,
            BuiltinFunction5 => 5d,
            BuiltinFunctionVariadic => 0d,
            _ => -1d
        };
        return length >= 0d;
    }

    internal static bool TryInvoke(
        Delegate target,
        object? thisArgument,
        in JsCallArguments arguments,
        out object? result)
    {
        switch (target)
        {
            case BuiltinFunction0 function:
                result = function(thisArgument);
                return true;
            case BuiltinFunction1 function:
                result = function(thisArgument, arguments.GetArgument(0));
                return true;
            case BuiltinFunction2 function:
                result = function(
                    thisArgument,
                    arguments.GetArgument(0),
                    arguments.GetArgument(1));
                return true;
            case BuiltinFunction3 function:
                result = function(
                    thisArgument,
                    arguments.GetArgument(0),
                    arguments.GetArgument(1),
                    arguments.GetArgument(2));
                return true;
            case BuiltinFunction4 function:
                result = function(
                    thisArgument,
                    arguments.GetArgument(0),
                    arguments.GetArgument(1),
                    arguments.GetArgument(2),
                    arguments.GetArgument(3));
                return true;
            case BuiltinFunction5 function:
                result = function(
                    thisArgument,
                    arguments.GetArgument(0),
                    arguments.GetArgument(1),
                    arguments.GetArgument(2),
                    arguments.GetArgument(3),
                    arguments.GetArgument(4));
                return true;
            case BuiltinFunctionVariadic function:
                result = function(thisArgument, arguments);
                return true;
            default:
                result = null;
                return false;
        }
    }
}
