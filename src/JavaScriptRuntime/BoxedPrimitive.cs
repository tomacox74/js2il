using System.Runtime.CompilerServices;

namespace JavaScriptRuntime;

/// <summary>Immutable primitive boxes shared across realms, never JavaScript wrapper objects.</summary>
public static class BoxedPrimitive
{
    public static readonly object True = true;
    public static readonly object False = false;
    public static readonly object Null = JsNull.Null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Box(bool value) => value ? True : False;
}
