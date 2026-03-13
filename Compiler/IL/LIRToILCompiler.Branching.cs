using Js2IL.IR;
using Js2IL.Services;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Services.VariableBindings;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    #region Branch Condition Handling

    /// <summary>
    /// Emits the condition for a branch instruction. If the condition is a non-materialized
    /// comparison, emits the comparison inline. Otherwise loads the temp normally.
    /// </summary>
    private void EmitBranchCondition(
        TempVariable condition,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        Dictionary<int, LIRInstruction> tempDefinitions,
        MethodDescriptor methodDescriptor)
    {
        // Check if the condition is a non-materialized comparison that we should inline
        if (!IsMaterialized(condition, allocation) &&
            condition.Index >= 0 &&
            tempDefinitions.TryGetValue(condition.Index, out var definingInstruction) &&
            BranchConditionOptimizer.IsComparisonInstruction(definingInstruction))
        {
            var emitLoad = definingInstruction is LIRCompareNumberLessThan
                or LIRCompareNumberGreaterThan
                or LIRCompareNumberLessThanOrEqual
                or LIRCompareNumberGreaterThanOrEqual
                or LIRCompareNumberEqual
                or LIRCompareNumberNotEqual
                ? (Action<TempVariable, InstructionEncoder>)((temp, encoder) => EmitLoadTempAsNumber(temp, encoder, allocation, methodDescriptor))
                : (temp, encoder) => EmitLoadTemp(temp, encoder, allocation, methodDescriptor);

            // Emit the comparison inline without storing to a local
            BranchConditionOptimizer.EmitInlineComparison(
                definingInstruction,
                ilEncoder,
                emitLoad);
        }
        else
        {
            // Load the condition from its local normally
            EmitLoadTemp(condition, ilEncoder, allocation, methodDescriptor);
        }
    }

    private bool TryGetSameILLocalSlot(TempVariable source, TempVariable destination, TempLocalAllocation allocation, out int slot)
    {
        slot = -1;

        // Variable-slot backed temps.
        int srcVarSlot = (source.Index >= 0 && source.Index < MethodBody.TempVariableSlots.Count)
            ? MethodBody.TempVariableSlots[source.Index]
            : -1;
        int dstVarSlot = (destination.Index >= 0 && destination.Index < MethodBody.TempVariableSlots.Count)
            ? MethodBody.TempVariableSlots[destination.Index]
            : -1;

        if (srcVarSlot >= 0 || dstVarSlot >= 0)
        {
            if (srcVarSlot >= 0 && dstVarSlot >= 0 && srcVarSlot == dstVarSlot)
            {
                slot = srcVarSlot;
                return true;
            }

            return false;
        }

        // Temp-local backed temps.
        if (allocation.IsMaterialized(source) && allocation.IsMaterialized(destination))
        {
            var srcTempSlot = allocation.GetSlot(source);
            var dstTempSlot = allocation.GetSlot(destination);
            if (srcTempSlot == dstTempSlot)
            {
                slot = srcTempSlot;
                return true;
            }
        }

        return false;
    }

    #endregion
}