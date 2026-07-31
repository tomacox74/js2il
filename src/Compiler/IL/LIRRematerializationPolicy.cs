using Jroc.IR;

namespace Jroc.IL;

/// <summary>
/// Central entry point for deciding whether a definition may be suppressed and
/// safely reproduced at a later use. Rematerialization does not decide
/// evaluation-stack residency or instruction order.
/// </summary>
internal static class LIRRematerializationPolicy
{
    internal static bool CanRematerializeForStackify(
        LIRInstruction instruction,
        MethodBodyIR methodBody,
        LIRInstruction?[] definitions)
        => Stackify.EvaluateLegacyRematerialization(
            instruction,
            methodBody,
            definitions);

    internal static bool CanRematerializeForAllocation(
        LIRInstruction instruction,
        MethodBodyIR methodBody,
        IReadOnlyDictionary<int, LIRInstruction> definitions)
    {
        if (instruction is LIRConstNumber
            or LIRConstString
            or LIRConstBoolean
            or LIRConstUndefined
            or LIRConstNull
            or LIRLoadParameter
            or LIRLoadThis
            or LIRLoadUserClassInstanceField)
        {
            return true;
        }

        if (instruction is not LIRConvertToObject convertToObject)
        {
            return false;
        }

        var sourceIndex = convertToObject.Source.Index;
        if (sourceIndex >= 0
            && sourceIndex < methodBody.TempVariableSlots.Count
            && methodBody.TempVariableSlots[sourceIndex] >= 0)
        {
            return false;
        }

        return definitions.TryGetValue(sourceIndex, out var sourceDefinition)
            && CanRematerializeForAllocation(
                sourceDefinition,
                methodBody,
                definitions);
    }
}
