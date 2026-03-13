using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.IR;

namespace Js2IL.IL;

/// <summary>
/// Optimizer for branch conditions.
/// Inlines comparison operations directly into branch instructions when the comparison result
/// is only used by that single branch, eliminating an unnecessary local variable.
/// </summary>
internal static class BranchConditionOptimizer
{
    /// <summary>
    /// Builds a map from temp index to the instruction that defines it.
    /// </summary>
    public static Dictionary<int, LIRInstruction> BuildTempDefinitionMap(MethodBodyIR methodBody)
    {
        var result = new Dictionary<int, LIRInstruction>();
        foreach (var instruction in methodBody.Instructions
            .Where(instr => TempLocalAllocator.TryGetDefinedTemp(instr, out var d) && d.Index >= 0))
        {
            TempLocalAllocator.TryGetDefinedTemp(instruction, out var defined);
            result[defined.Index] = instruction;
        }
        return result;
    }

    /// <summary>
    /// Marks comparison temps that are only used by branch instructions as non-materialized.
    /// This allows us to emit the comparison inline with the branch, eliminating the local.
    /// </summary>
    public static void MarkBranchOnlyComparisonTemps(
        MethodBodyIR methodBody,
        bool[]? shouldMaterializeTemp,
        Dictionary<int, LIRInstruction> tempDefinitions)
    {
        int tempCount = methodBody.Temps.Count;
        if (tempCount == 0) return;

        // Count how many times each temp is used
        var useCount = new int[tempCount];
        var usedByBranchOnly = new bool[tempCount];

        // Initialize: assume all temps are used only by branches
        for (int i = 0; i < tempCount; i++)
        {
            usedByBranchOnly[i] = true;
        }

        foreach (var instruction in methodBody.Instructions)
        {
            bool isBranch = instruction is LIRBranchIfFalse or LIRBranchIfTrue;
            
            foreach (var used in TempLocalAllocator.EnumerateUsedTemps(instruction)
                .Where(u => u.Index >= 0 && u.Index < tempCount))
            {
                useCount[used.Index]++;
                if (!isBranch)
                {
                    usedByBranchOnly[used.Index] = false;
                }
            }
        }

        // Mark comparison temps that are only used once by a branch as non-materialized
        if (shouldMaterializeTemp == null)
        {
            return; // Will be created fresh, no way to mark
        }

        for (int i = 0; i < tempCount; i++)
        {
            if (useCount[i] == 1
                && usedByBranchOnly[i]
                && tempDefinitions.TryGetValue(i, out var definingInstruction)
                && IsComparisonInstruction(definingInstruction))
            {
                // Mark this temp as not needing materialization (false = not used outside)
                shouldMaterializeTemp[i] = false;
            }
        }
    }

    /// <summary>
    /// Returns true if the instruction is a comparison that produces a boolean result.
    /// </summary>
    public static bool IsComparisonInstruction(LIRInstruction instruction)
    {
        return instruction is LIRCompareNumberLessThan
            or LIRCompareNumberGreaterThan
            or LIRCompareNumberLessThanOrEqual
            or LIRCompareNumberGreaterThanOrEqual
            or LIRCompareNumberEqual
            or LIRCompareNumberNotEqual
            or LIRCompareBooleanEqual
            or LIRCompareBooleanNotEqual;
    }

    /// <summary>
    /// Emits a comparison instruction inline without storing the result.
    /// Used when the comparison is only consumed by a branch instruction.
    /// </summary>
    public static void EmitInlineComparison(
        LIRInstruction comparison,
        InstructionEncoder ilEncoder,
        Action<TempVariable, InstructionEncoder> emitLoadTemp)
    {
        switch (comparison)
        {
            case LIRCompareNumberLessThan cmp:
                emitLoadTemp(cmp.Left, ilEncoder);
                emitLoadTemp(cmp.Right, ilEncoder);
                ilEncoder.OpCode(ILOpCode.Clt);
                break;
            case LIRCompareNumberGreaterThan cmp:
                emitLoadTemp(cmp.Left, ilEncoder);
                emitLoadTemp(cmp.Right, ilEncoder);
                ilEncoder.OpCode(ILOpCode.Cgt);
                break;
            case LIRCompareNumberLessThanOrEqual cmp:
                emitLoadTemp(cmp.Left, ilEncoder);
                emitLoadTemp(cmp.Right, ilEncoder);
                ilEncoder.OpCode(ILOpCode.Cgt);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareNumberGreaterThanOrEqual cmp:
                emitLoadTemp(cmp.Left, ilEncoder);
                emitLoadTemp(cmp.Right, ilEncoder);
                ilEncoder.OpCode(ILOpCode.Clt);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareNumberEqual cmp:
                emitLoadTemp(cmp.Left, ilEncoder);
                emitLoadTemp(cmp.Right, ilEncoder);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareNumberNotEqual cmp:
                emitLoadTemp(cmp.Left, ilEncoder);
                emitLoadTemp(cmp.Right, ilEncoder);
                ilEncoder.OpCode(ILOpCode.Ceq);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareBooleanEqual cmp:
                emitLoadTemp(cmp.Left, ilEncoder);
                emitLoadTemp(cmp.Right, ilEncoder);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareBooleanNotEqual cmp:
                emitLoadTemp(cmp.Left, ilEncoder);
                emitLoadTemp(cmp.Right, ilEncoder);
                ilEncoder.OpCode(ILOpCode.Ceq);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
        }
    }
}
