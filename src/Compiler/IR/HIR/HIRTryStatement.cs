namespace Jroc.HIR;

public sealed class HIRTryStatement : HIRStatement
{
    public HIRTryStatement(HIRStatement tryBlock, Jroc.SymbolTables.BindingInfo? catchParamBinding, HIRPattern? catchParamPattern, HIRStatement? catchBody, HIRStatement? finallyBody)
    {
        TryBlock = tryBlock;
        CatchParamBinding = catchParamBinding;
        CatchParamPattern = catchParamPattern;
        CatchBody = catchBody;
        FinallyBody = finallyBody;
    }

    public HIRStatement TryBlock { get; }
    public Jroc.SymbolTables.BindingInfo? CatchParamBinding { get; }
    public HIRPattern? CatchParamPattern { get; }
    public HIRStatement? CatchBody { get; }
    public HIRStatement? FinallyBody { get; }
}
