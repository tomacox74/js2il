namespace Jroc.IR;

/// <summary>
/// Defines global object members that can use a direct CLR instance call once source
/// analysis proves that their global binding has not been modified or exposed.
/// </summary>
internal static class GlobalMemberIntrinsicRegistry
{
    private static readonly GlobalMemberIntrinsicDescriptor[] Entries =
    [
        new(
            GlobalName: nameof(JavaScriptRuntime.GlobalThis.console),
            MemberName: nameof(JavaScriptRuntime.Console.log),
            ReceiverType: typeof(JavaScriptRuntime.Console))
    ];

    public static bool TryGet(string globalName, string memberName, out GlobalMemberIntrinsicDescriptor descriptor)
    {
        foreach (var entry in Entries)
        {
            if (string.Equals(entry.GlobalName, globalName, StringComparison.Ordinal)
                && string.Equals(entry.MemberName, memberName, StringComparison.Ordinal))
            {
                descriptor = entry;
                return true;
            }
        }

        descriptor = default;
        return false;
    }
}

internal readonly record struct GlobalMemberIntrinsicDescriptor(
    string GlobalName,
    string MemberName,
    Type ReceiverType);
