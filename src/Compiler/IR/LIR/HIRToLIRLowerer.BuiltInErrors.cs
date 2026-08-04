namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private static ValueStorage GetBuiltInErrorStorage(string errorTypeName)
        => new(ValueStorageKind.Reference, BuiltInErrorTypes.GetRuntimeErrorClrType(errorTypeName));
}
