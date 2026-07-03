namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private TempVariable EmitMarkUndefinedPrototype(TempVariable functionValueTemp)
    {
        var markedTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            IntrinsicName: nameof(JavaScriptRuntime.Function),
            MethodName: nameof(JavaScriptRuntime.Function.MarkUndefinedPrototype),
            Arguments: new List<TempVariable> { EnsureObject(functionValueTemp) },
            Result: markedTemp));
        DefineTempStorage(markedTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return markedTemp;
    }    
}