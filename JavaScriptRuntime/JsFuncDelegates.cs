namespace JavaScriptRuntime;

internal static class JsFuncDelegates
{
    private static HashSet<Type> _registeredDelegateTypes = new();

    static JsFuncDelegates()
    {
        // Register all JsFuncN delegate types
        for (int i = 0; i <= 32; i++)
        {
            var delegateType = Type.GetType($"JavaScriptRuntime.JsFunc{i}");
            if (delegateType != null)
            {
                _registeredDelegateTypes.Add(delegateType);
            }
        }
    }

    internal static bool IsJsFuncDelegateType(Type type)
    {
        return _registeredDelegateTypes.Contains(type);
    }
}

// Custom delegate types used by js2il to bypass System.Func<> arity limits.
// Signature convention: (object[] scopes, object? newTarget, object? a1..aN) -> object?
public delegate object? JsFunc0(object[] scopes, object? newTarget);
public delegate object? JsFunc1(object[] scopes, object? newTarget, object? a1);
public delegate object? JsFunc2(object[] scopes, object? newTarget, object? a1, object? a2);
public delegate object? JsFunc3(object[] scopes, object? newTarget, object? a1, object? a2, object? a3);
public delegate object? JsFunc4(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4);
public delegate object? JsFunc5(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5);
public delegate object? JsFunc6(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6);
public delegate object? JsFunc7(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7);
public delegate object? JsFunc8(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8);
public delegate object? JsFunc9(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9);
public delegate object? JsFunc10(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10);
public delegate object? JsFunc11(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11);
public delegate object? JsFunc12(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12);
public delegate object? JsFunc13(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13);
public delegate object? JsFunc14(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14);
public delegate object? JsFunc15(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15);
public delegate object? JsFunc16(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16);
public delegate object? JsFunc17(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17);
public delegate object? JsFunc18(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18);
public delegate object? JsFunc19(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19);
public delegate object? JsFunc20(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20);
public delegate object? JsFunc21(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21);
public delegate object? JsFunc22(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22);
public delegate object? JsFunc23(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23);
public delegate object? JsFunc24(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24);
public delegate object? JsFunc25(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25);
public delegate object? JsFunc26(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25, object? a26);
public delegate object? JsFunc27(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25, object? a26, object? a27);
public delegate object? JsFunc28(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25, object? a26, object? a27, object? a28);
public delegate object? JsFunc29(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25, object? a26, object? a27, object? a28, object? a29);
public delegate object? JsFunc30(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25, object? a26, object? a27, object? a28, object? a29, object? a30);
public delegate object? JsFunc31(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25, object? a26, object? a27, object? a28, object? a29, object? a30, object? a31);
public delegate object? JsFunc32(object[] scopes, object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25, object? a26, object? a27, object? a28, object? a29, object? a30, object? a31, object? a32);

// No-scopes delegate types (optimization for functions that don't reference parent scopes).
// Signature convention: (object? newTarget, object? a1..aN) -> object?
// newTarget is retained for proper `new` vs call semantics.
public delegate object? JsFuncNoScopes0(object? newTarget);
public delegate object? JsFuncNoScopes1(object? newTarget, object? a1);
public delegate object? JsFuncNoScopes2(object? newTarget, object? a1, object? a2);
public delegate object? JsFuncNoScopes3(object? newTarget, object? a1, object? a2, object? a3);
public delegate object? JsFuncNoScopes4(object? newTarget, object? a1, object? a2, object? a3, object? a4);
public delegate object? JsFuncNoScopes5(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5);
public delegate object? JsFuncNoScopes6(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6);
public delegate object? JsFuncNoScopes7(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7);
public delegate object? JsFuncNoScopes8(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8);
public delegate object? JsFuncNoScopes9(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9);
public delegate object? JsFuncNoScopes10(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10);
public delegate object? JsFuncNoScopes11(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11);
public delegate object? JsFuncNoScopes12(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12);
public delegate object? JsFuncNoScopes13(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13);
public delegate object? JsFuncNoScopes14(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14);
public delegate object? JsFuncNoScopes15(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15);
public delegate object? JsFuncNoScopes16(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16);
public delegate object? JsFuncNoScopes17(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17);
public delegate object? JsFuncNoScopes18(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18);
public delegate object? JsFuncNoScopes19(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19);
public delegate object? JsFuncNoScopes20(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20);
public delegate object? JsFuncNoScopes21(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21);
public delegate object? JsFuncNoScopes22(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22);
public delegate object? JsFuncNoScopes23(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23);
public delegate object? JsFuncNoScopes24(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24);
public delegate object? JsFuncNoScopes25(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25);
public delegate object? JsFuncNoScopes26(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25, object? a26);
public delegate object? JsFuncNoScopes27(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25, object? a26, object? a27);
public delegate object? JsFuncNoScopes28(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25, object? a26, object? a27, object? a28);
public delegate object? JsFuncNoScopes29(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25, object? a26, object? a27, object? a28, object? a29);
public delegate object? JsFuncNoScopes30(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25, object? a26, object? a27, object? a28, object? a29, object? a30);
public delegate object? JsFuncNoScopes31(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25, object? a26, object? a27, object? a28, object? a29, object? a30, object? a31);
public delegate object? JsFuncNoScopes32(object? newTarget, object? a1, object? a2, object? a3, object? a4, object? a5, object? a6, object? a7, object? a8, object? a9, object? a10, object? a11, object? a12, object? a13, object? a14, object? a15, object? a16, object? a17, object? a18, object? a19, object? a20, object? a21, object? a22, object? a23, object? a24, object? a25, object? a26, object? a27, object? a28, object? a29, object? a30, object? a31, object? a32);
