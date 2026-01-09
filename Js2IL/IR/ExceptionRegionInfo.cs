namespace Js2IL.IR;

public enum ExceptionRegionKind
{
    Catch,
    Finally
}

public sealed record ExceptionRegionInfo(
    ExceptionRegionKind Kind,
    int TryStartLabelId,
    int TryEndLabelId,
    int HandlerStartLabelId,
    int HandlerEndLabelId,
    Type? CatchType = null);
