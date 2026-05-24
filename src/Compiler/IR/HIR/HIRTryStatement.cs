namespace Js2IL.HIR;

public sealed class HIRTryStatement : HIRStatement
{
    public HIRTryStatement(HIRStatement tryBlock, Js2IL.SymbolTables.BindingInfo? catchParamBinding, HIRPattern? catchParamPattern, HIRStatement? catchBody, HIRStatement? finallyBody)
    {
        TryBlock = tryBlock;
        CatchParamBinding = catchParamBinding;
        CatchParamPattern = catchParamPattern;
        CatchBody = catchBody;
        FinallyBody = finallyBody;
    }

    public HIRStatement TryBlock { get; }
    public Js2IL.SymbolTables.BindingInfo? CatchParamBinding { get; }
    public HIRPattern? CatchParamPattern { get; }
    public HIRStatement? CatchBody { get; }
    public HIRStatement? FinallyBody { get; }
}
