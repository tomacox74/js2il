using Jroc.IL;

namespace Jroc.IR;

internal static class LIRTaggedNumericNormalization
{
    public static void Normalize(MethodBodyIR body)
    {
        var definitions = new LIRInstruction?[body.TempStorages.Count];
        var multipleDefinitions = new bool[definitions.Length];
        foreach (var instruction in body.Instructions)
        {
            if (LIRInstructionInfo.TryGetDefinedTemp(instruction, out var result)
                && result.Index >= 0 && result.Index < definitions.Length)
            {
                multipleDefinitions[result.Index] |= definitions[result.Index] != null;
                definitions[result.Index] = instruction;
            }
        }

        if (!definitions.Any(instruction => instruction is LIRGetItem or LIRGetJsArrayElement))
        {
            return;
        }

        var uses = new List<LIRInstruction>?[definitions.Length];
        foreach (var instruction in body.Instructions)
        {
            var visitor = new UseVisitor(definitions, uses, instruction);
            LIRInstructionInfo.VisitUsedTemps(instruction, ref visitor);
        }

        var candidates = new HashSet<int>();
        for (var index = 0; index < definitions.Length; index++)
        {
            if (!multipleDefinitions[index]
                && definitions[index] is LIRGetItem or LIRGetJsArrayElement
                && body.TempStorages[index].ClrType == typeof(object)
                && (index >= body.TempVariableSlots.Count || body.TempVariableSlots[index] < 0)
                && uses[index] is { Count: > 0 } consumers
                && consumers.All(IsTaggedConsumer))
            {
                candidates.Add(index);
            }
        }

        // Both relational operands must have a tagged/double overload. Removing one candidate
        // can make the other ineligible, so finish the proof before changing any storage types.
        bool changed;
        do
        {
            changed = false;
            foreach (var index in candidates.ToArray())
            {
                if (uses[index]!.OfType<LIRBinaryDynamicOperator>().Any(comparison =>
                    !SupportsTaggedComparisonOperand(body, candidates,
                        comparison.Left.Index == index ? comparison.Right : comparison.Left)))
                {
                    candidates.Remove(index);
                    changed = true;
                }
            }
        } while (changed);

        foreach (var index in candidates)
        {
            body.TempStorages[index] = new ValueStorage(
                ValueStorageKind.UnboxedValue, typeof(JavaScriptRuntime.JsValue));
        }
    }

    private static bool IsTaggedConsumer(LIRInstruction instruction)
        => instruction is LIRConvertToNumber
            || instruction is LIRBinaryDynamicOperator
            {
                Operator: DynamicBinaryOperatorKind.LessThan
                    or DynamicBinaryOperatorKind.GreaterThan
                    or DynamicBinaryOperatorKind.LessThanOrEqual
                    or DynamicBinaryOperatorKind.GreaterThanOrEqual
            };

    private static bool SupportsTaggedComparisonOperand(
        MethodBodyIR body, HashSet<int> candidates, TempVariable operand)
        => operand.Index >= 0 && operand.Index < body.TempStorages.Count
            && (candidates.Contains(operand.Index)
                || body.TempStorages[operand.Index] is
                { Kind: ValueStorageKind.UnboxedValue, ClrType: var type }
                    && type == typeof(double));

    private readonly struct UseVisitor(
        LIRInstruction?[] definitions,
        List<LIRInstruction>?[] uses,
        LIRInstruction instruction) : ITempUseVisitor
    {
        public void Visit(TempVariable temp)
        {
            if (temp.Index >= 0 && temp.Index < uses.Length
                && definitions[temp.Index] is LIRGetItem or LIRGetJsArrayElement)
            {
                (uses[temp.Index] ??= new List<LIRInstruction>()).Add(instruction);
            }
        }
    }
}
