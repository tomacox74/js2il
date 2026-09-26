namespace Jroc.IR;

internal static class LIRVectorNormalization
{
    public static void Normalize(MethodBodyIR method)
    {
        var instructions = method.Instructions;
        for (var index = 0; index < instructions.Count; index++)
        {
            if (instructions[index] is not LIRVectorInt32Range range)
            {
                continue;
            }

            if (range.Load.Array != range.Store.Array)
            {
                throw new InvalidOperationException("A vector range must load and store the same array.");
            }

            instructions[index] = new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.RuntimeServices),
                nameof(JavaScriptRuntime.RuntimeServices.TryVectorOrRange),
                [range.Load.Array, range.Load.Start, range.Load.End, range.Operation.Mask],
                range.Applied);
        }
    }
}
