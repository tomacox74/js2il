using Jroc.IL;

namespace Jroc.IR;

internal static class LIRTaggedNumericNormalization
{
    private enum CandidateState : byte
    {
        Unused,
        Used,
        Ineligible
    }

    public static void Normalize(MethodBodyIR body)
    {
        var definitions = new LIRInstruction?[body.TempStorages.Count];
        var states = new CandidateState[definitions.Length];
        foreach (var instruction in body.Instructions)
        {
            if (LIRInstructionInfo.TryGetDefinedTemp(instruction, out var result)
                && result.Index >= 0 && result.Index < definitions.Length)
            {
                if (definitions[result.Index] != null)
                {
                    states[result.Index] = CandidateState.Ineligible;
                }
                definitions[result.Index] = instruction;
            }
        }

        var candidates = new List<int>();
        for (var index = 0; index < definitions.Length; index++)
        {
            if (states[index] != CandidateState.Ineligible
                && definitions[index] is LIRGetItem or LIRGetJsArrayElement
                && body.TempStorages[index].ClrType == typeof(object)
                && (index >= body.TempVariableSlots.Count || body.TempVariableSlots[index] < 0))
            {
                candidates.Add(index);
            }
            else
            {
                states[index] = CandidateState.Ineligible;
            }
        }

        if (candidates.Count == 0)
        {
            return;
        }

        var comparisonOperands = new List<int>?[definitions.Length];
        foreach (var instruction in body.Instructions)
        {
            var visitor = new UseVisitor(states, comparisonOperands, instruction);
            LIRInstructionInfo.VisitUsedTemps(instruction, ref visitor);
        }

        var candidateCount = 0;
        for (var index = 0; index < candidates.Count; index++)
        {
            var candidate = candidates[index];
            if (states[candidate] == CandidateState.Used)
            {
                candidates[candidateCount++] = candidate;
            }
        }
        candidates.RemoveRange(candidateCount, candidates.Count - candidateCount);

        // Both relational operands must have a tagged/double overload. Removing one candidate
        // can make the other ineligible, so finish the proof before changing any storage types.
        bool changed;
        do
        {
            changed = false;
            foreach (var index in candidates)
            {
                if (states[index] != CandidateState.Used
                    || comparisonOperands[index] is not { } operands)
                {
                    continue;
                }
                foreach (var operand in operands)
                {
                    if (!SupportsTaggedComparisonOperand(body, states, operand))
                    {
                        states[index] = CandidateState.Ineligible;
                        changed = true;
                        break;
                    }
                }
            }
        } while (changed);

        foreach (var index in candidates)
        {
            if (states[index] == CandidateState.Used)
            {
                body.TempStorages[index] = new ValueStorage(
                    ValueStorageKind.UnboxedValue, typeof(JavaScriptRuntime.JsValue));
            }
        }
    }

    private static bool SupportsTaggedComparisonOperand(
        MethodBodyIR body, CandidateState[] states, int operand)
        => operand >= 0 && operand < body.TempStorages.Count
            && (states[operand] == CandidateState.Used
                || body.TempStorages[operand] is
                { Kind: ValueStorageKind.UnboxedValue, ClrType: var type }
                    && type == typeof(double));

    private readonly struct UseVisitor(
        CandidateState[] states,
        List<int>?[] comparisonOperands,
        LIRInstruction instruction) : ITempUseVisitor
    {
        public void Visit(TempVariable temp)
        {
            if (temp.Index < 0 || temp.Index >= states.Length
                || states[temp.Index] == CandidateState.Ineligible)
            {
                return;
            }

            if (instruction is LIRConvertToNumber)
            {
                states[temp.Index] = CandidateState.Used;
            }
            else if (instruction is LIRBinaryDynamicOperator
            {
                Operator: DynamicBinaryOperatorKind.LessThan
                        or DynamicBinaryOperatorKind.GreaterThan
                        or DynamicBinaryOperatorKind.LessThanOrEqual
                        or DynamicBinaryOperatorKind.GreaterThanOrEqual
            } comparison)
            {
                states[temp.Index] = CandidateState.Used;
                var operand = comparison.Left.Index == temp.Index ? comparison.Right : comparison.Left;
                (comparisonOperands[temp.Index] ??= new List<int>()).Add(operand.Index);
            }
            else
            {
                states[temp.Index] = CandidateState.Ineligible;
                comparisonOperands[temp.Index] = null;
            }
        }
    }
}
