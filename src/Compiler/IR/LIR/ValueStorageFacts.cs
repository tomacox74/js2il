namespace Jroc.IR;

internal static class ValueStorageFacts
{
    internal static bool IsSameRuntimeRepresentation(ValueStorage left, ValueStorage right)
    {
        if (left.Kind != right.Kind || left.ClrType != right.ClrType)
        {
            return false;
        }

        if (!left.TypeHandle.IsNil || !right.TypeHandle.IsNil)
        {
            return left.TypeHandle.Equals(right.TypeHandle);
        }

        return string.Equals(left.ScopeName, right.ScopeName, StringComparison.Ordinal);
    }

    internal static bool CanFlowTo(ValueStorage source, ValueStorage target)
    {
        if (IsSameRuntimeRepresentation(source, target))
        {
            return true;
        }

        if (target.Kind == ValueStorageKind.Reference && target.ClrType == typeof(object))
        {
            return source.Kind is ValueStorageKind.UnboxedValue or ValueStorageKind.BoxedValue or ValueStorageKind.Reference;
        }

        if (source.Kind == ValueStorageKind.Reference
            && target.Kind == ValueStorageKind.Reference
            && source.ClrType != null
            && target.ClrType != null
            && target.ClrType.IsAssignableFrom(source.ClrType)
            && source.TypeHandle.IsNil
            && target.TypeHandle.IsNil)
        {
            return true;
        }

        return false;
    }

}
