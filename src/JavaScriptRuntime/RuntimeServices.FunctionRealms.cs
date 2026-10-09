namespace JavaScriptRuntime;

public partial class RuntimeServices
{
    internal static IDisposable? EnterFunctionRealm(JsFunctionObject function)
    {
        var intrinsics = function.OwningIntrinsics;
        return ReferenceEquals(RuntimeExecutionContext.CurrentOrOverride?.Realm.Intrinsics, intrinsics)
            || intrinsics.ExecutionContext is not { } context
                ? null
                : context.Enter();
    }
}
