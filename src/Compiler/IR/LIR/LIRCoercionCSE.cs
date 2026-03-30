namespace Js2IL.IR;

/// <summary>
/// Common Subexpression Elimination (CSE) pass for repeated coercion instructions.
///
/// Within each basic block (instructions between two consecutive <see cref="LIRLabel"/> markers),
/// if the same pure coercion (<see cref="LIRConvertToNumber"/>, <see cref="LIRConvertToBoolean"/>,
/// or <see cref="LIRCallIsTruthy"/>) is applied to the same source temp more than once, the
/// second and subsequent occurrences are replaced with a <see cref="LIRCopyTemp"/> from the first result.
///
/// Coercions are treated as pure only when their source is a known primitive:
/// <see cref="ValueStorageKind.UnboxedValue"/> of <see cref="double"/> or <see cref="bool"/>.
/// Coercions on <see cref="ValueStorageKind.Reference"/> (object) sources are left unchanged to
/// preserve observable <c>valueOf</c>/<c>toString</c> side-effects on user-defined objects.
/// </summary>
internal static class LIRCoercionCSE
{
    private enum CoercionKind { ToNumber, ToBoolean, IsTruthy }

    /// <summary>
    /// Runs the CSE pass over all instructions in <paramref name="methodBody"/>.
    /// </summary>
    public static void Optimize(MethodBodyIR methodBody)
    {
        // Within a basic block: (coercion kind, source temp index) → result temp index of first occurrence.
        var available = new Dictionary<(CoercionKind kind, int sourceIdx), int>();

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            var instruction = methodBody.Instructions[i];

            // A label marks a potential branch target and thus the start of a new basic block.
            // Reset the CSE map to avoid propagating values across control-flow joins.
            if (instruction is LIRLabel)
            {
                available.Clear();
                continue;
            }

            switch (instruction)
            {
                case LIRConvertToNumber toNum:
                    if (IsPrimitive(GetTempStorage(methodBody, toNum.Source)))
                    {
                        TryCse(methodBody, ref i, CoercionKind.ToNumber, toNum.Source, toNum.Result, available);
                    }
                    break;

                case LIRConvertToBoolean toBool:
                    if (IsPrimitive(GetTempStorage(methodBody, toBool.Source)))
                    {
                        TryCse(methodBody, ref i, CoercionKind.ToBoolean, toBool.Source, toBool.Result, available);
                    }
                    break;

                case LIRCallIsTruthy isTruthy:
                    if (IsPrimitive(GetTempStorage(methodBody, isTruthy.Value)))
                    {
                        TryCse(methodBody, ref i, CoercionKind.IsTruthy, isTruthy.Value, isTruthy.Result, available);
                    }
                    break;
            }
        }
    }

    private static void TryCse(
        MethodBodyIR methodBody,
        ref int instructionIndex,
        CoercionKind kind,
        TempVariable source,
        TempVariable result,
        Dictionary<(CoercionKind, int), int> available)
    {
        var key = (kind, source.Index);
        if (available.TryGetValue(key, out var existingResultIdx))
        {
            // Replace duplicate coercion with a copy from the first-computed result.
            var existingResult = new TempVariable(existingResultIdx);
            methodBody.Instructions[instructionIndex] = new LIRCopyTemp(existingResult, result);

            // Keep storage metadata in sync so downstream passes and the IL emitter see
            // the correct CLR type for the result temp.
            var existingStorage = GetTempStorage(methodBody, existingResult);
            SetTempStorage(methodBody, result, existingStorage);
        }
        else
        {
            available[key] = result.Index;
        }
    }

    /// <summary>
    /// Returns true when <paramref name="storage"/> represents an unboxed primitive value
    /// (either <see cref="double"/> or <see cref="bool"/>), meaning the corresponding
    /// coercion is free of observable side-effects and safe to CSE.
    /// </summary>
    private static bool IsPrimitive(ValueStorage storage)
        => storage.Kind == ValueStorageKind.UnboxedValue
            && (storage.ClrType == typeof(double) || storage.ClrType == typeof(bool));

    private static ValueStorage GetTempStorage(MethodBodyIR methodBody, TempVariable temp)
    {
        if (temp.Index >= 0 && temp.Index < methodBody.TempStorages.Count)
        {
            return methodBody.TempStorages[temp.Index];
        }

        return new ValueStorage(ValueStorageKind.Unknown);
    }

    private static void SetTempStorage(MethodBodyIR methodBody, TempVariable temp, ValueStorage storage)
    {
        if (temp.Index >= 0 && temp.Index < methodBody.TempStorages.Count)
        {
            methodBody.TempStorages[temp.Index] = storage;
        }
    }
}
