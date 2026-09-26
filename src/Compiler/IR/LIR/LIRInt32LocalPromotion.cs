using Jroc.IL;

namespace Jroc.IR;

/// <summary>
/// Keeps signed bitwise values and Int32Array element reads in native int32 locals.
/// An unsigned shift is eligible only when its constant shift count guarantees
/// the result fits in a signed int32; other JS numbers remain doubles.
/// </summary>
internal static class LIRInt32LocalPromotion
{
    public static void Optimize(MethodBodyIR body)
    {
        if (body.VariableStorages.Count == 0)
        {
            return;
        }

        var definitions = new LIRInstruction?[body.TempStorages.Count];
        var multipleDefinitions = new bool[definitions.Length];
        var uses = new List<LIRInstruction>?[definitions.Length];
        foreach (var instruction in body.Instructions)
        {
            if (LIRInstructionInfo.TryGetDefinedTemp(instruction, out var result)
                && result.Index >= 0 && result.Index < definitions.Length)
            {
                multipleDefinitions[result.Index] |= definitions[result.Index] != null;
                definitions[result.Index] = instruction;
            }

            var visitor = new UseVisitor(uses, instruction);
            LIRInstructionInfo.VisitUsedTemps(instruction, ref visitor);
        }

        var slotTemps = new List<int>?[body.VariableStorages.Count];
        for (var index = 0; index < body.TempVariableSlots.Count; index++)
        {
            var slot = body.TempVariableSlots[index];
            if (slot >= 0 && slot < slotTemps.Length)
            {
                (slotTemps[slot] ??= new List<int>()).Add(index);
            }
        }

        for (var slot = 0; slot < body.VariableStorages.Count; slot++)
        {
            if (body.VariableStorages[slot] is not { Kind: ValueStorageKind.UnboxedValue, ClrType: var type }
                || type != typeof(double))
            {
                continue;
            }

            if (slotTemps[slot] is not { Count: > 0 } mappedTemps)
            {
                continue;
            }

            var producers = new HashSet<int>();
            if (!mappedTemps.All(index => TryCollectInt32Definitions(
                    body, definitions, multipleDefinitions, index, slot, producers, new HashSet<int>()))
                || !producers.All(index => UsesSupportInt32(body, uses[index]))
                // A typed-array read may become undefined; only coerce it early if every use would coerce it to int32 anyway.
                || !producers.All(index => definitions[index] is not LIRGetInt32ArrayElement
                    || GetterUsedOnlyForInt32Operations(index, uses, producers)))
            {
                continue;
            }

            body.VariableStorages[slot] = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(int));
            foreach (var index in producers)
            {
                body.TempStorages[index] = new ValueStorage(ValueStorageKind.UnboxedValue, typeof(int));
            }
        }
    }

    private static bool TryCollectInt32Definitions(
        MethodBodyIR body,
        LIRInstruction?[] definitions,
        bool[] multipleDefinitions,
        int index,
        int slot,
        HashSet<int> producers,
        HashSet<int> visiting)
    {
        if (producers.Contains(index))
        {
            return true;
        }

        if (index < 0 || index >= body.TempVariableSlots.Count
            || (body.TempVariableSlots[index] >= 0 && body.TempVariableSlots[index] != slot)
            || !visiting.Add(index))
        {
            return false;
        }

        var definition = definitions[index];
        var valid = !multipleDefinitions[index] && definition != null
            && (definition is LIRCopyTemp copy
                ? TryCollectInt32Definitions(body, definitions, multipleDefinitions,
                    copy.Source.Index, slot, producers, visiting)
                : IsInt32Producer(body, definitions, multipleDefinitions, definition));
        visiting.Remove(index);
        if (valid)
        {
            producers.Add(index);
        }
        return valid;
    }

    private static bool IsInt32Producer(
        MethodBodyIR body,
        LIRInstruction?[] definitions,
        bool[] multipleDefinitions,
        LIRInstruction definition)
        => definition is LIRBitwiseAnd or LIRBitwiseOr or LIRBitwiseXor
            or LIRLeftShift or LIRRightShift or LIRGetInt32ArrayElement
            || definition is LIRUnsignedRightShift shift
                && shift.Right.Index >= 0 && shift.Right.Index < definitions.Length
                && !multipleDefinitions[shift.Right.Index]
                && (body.TempVariableSlots[shift.Right.Index] is var rightSlot
                    && (rightSlot < 0 || body.SingleAssignmentSlots.Contains(rightSlot)))
                && definitions[shift.Right.Index] is LIRConstNumber constant
                && constant.Value is >= 1 and <= 31
                && constant.Value == Math.Truncate(constant.Value);

    private static bool UsesSupportInt32(
        MethodBodyIR body,
        List<LIRInstruction>? instructions)
    {
        if (instructions == null)
        {
            return true;
        }

        foreach (var instruction in instructions)
        {
            if (instruction is LIRCopyTemp copy)
            {
                var destination = copy.Destination.Index;
                var slot = body.TempVariableSlots[destination];
                var storage = slot >= 0
                    ? body.VariableStorages[slot]
                    : body.TempStorages[destination];
                if (storage.Kind == ValueStorageKind.UnboxedValue
                    && (storage.ClrType == typeof(double) || storage.ClrType == typeof(int))
                    || storage.Kind is ValueStorageKind.Reference or ValueStorageKind.BoxedValue
                        && storage.ClrType == typeof(object))
                {
                    continue;
                }
                return false;
            }

            if (instruction is LIRBitwiseAnd or LIRBitwiseOr or LIRBitwiseXor
                or LIRLeftShift or LIRRightShift or LIRUnsignedRightShift
                or LIRGetInt32ArrayElement or LIRSetInt32ArrayElement)
            {
                continue;
            }

            return false;
        }

        return true;
    }

    private static bool GetterUsedOnlyForInt32Operations(
        int index,
        List<LIRInstruction>?[] uses,
        HashSet<int> producers)
    {
        var visited = new HashSet<int>();
        var pending = new Stack<int>();
        pending.Push(index);
        while (pending.Count != 0)
        {
            var current = pending.Pop();
            if (!visited.Add(current) || uses[current] == null)
            {
                continue;
            }

            foreach (var instruction in uses[current]!)
            {
                if (instruction is LIRCopyTemp copy && producers.Contains(copy.Destination.Index))
                {
                    pending.Push(copy.Destination.Index);
                }
                else if (instruction is LIRSetInt32ArrayElement set && set.Value.Index == current)
                {
                    continue;
                }
                else if (instruction is not (LIRBitwiseAnd or LIRBitwiseOr or LIRBitwiseXor
                    or LIRLeftShift or LIRRightShift or LIRUnsignedRightShift))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private readonly struct UseVisitor(
        List<LIRInstruction>?[] uses,
        LIRInstruction instruction) : ITempUseVisitor
    {
        public void Visit(TempVariable temp)
        {
            if (temp.Index >= 0 && temp.Index < uses.Length)
            {
                (uses[temp.Index] ??= new List<LIRInstruction>()).Add(instruction);
            }
        }
    }
}
