using Jroc.IL;

namespace Jroc.IR;

/// <summary>
/// Coalesces a temp that is produced by an instruction and immediately copied into a JavaScript
/// binding's stable variable slot, so the producing instruction stores directly into that slot.
///
/// <para>
/// HIR-to-LIR lowering emits a value-producing instruction into a fresh temp and then calls
/// <c>EnsureTempMappedToSlot</c>, which appends an <see cref="LIRCopyTemp"/> into the binding's
/// stable variable slot. Because the IL local allocator keeps temp-local allocation separate from
/// variable-slot allocation, that leaves a redundant temp local plus a
/// <c>stloc temp / ldloc temp / stloc variable</c> round-trip (issue #1604).
/// </para>
///
/// <para>
/// This pass pins the producer's result temp to the destination variable slot. The IL emitter then
/// stores the produced value straight into the variable local
/// (<c>LIRToILCompiler.GetSlotForTemp</c> routes slot-pinned temps to the variable local), and the
/// now-degenerate copy is skipped because source and destination resolve to the same IL local
/// (<c>LIRToILCompiler.TryGetSameILLocalSlot</c>). No temp local is allocated for the producer,
/// because <see cref="TempLocalAllocator"/> skips slot-pinned temps.
/// </para>
///
/// <para>
/// The rewrite only moves the write to the variable local one instruction earlier, from the copy to
/// the producer that immediately precedes it. Every eligibility rule below exists to keep that move
/// unobservable.
/// </para>
/// </summary>
internal static class LIRVariableSlotCoalescing
{
    /// <summary>
    /// Runs the coalescing pass over all instructions in <paramref name="methodBody"/>.
    /// </summary>
    public static void Optimize(MethodBodyIR methodBody)
    {
        var instructions = methodBody.Instructions;
        if (instructions.Count < 2)
        {
            return;
        }

        var tempCount = methodBody.Temps.Count;
        if (tempCount == 0)
        {
            return;
        }

        var defCount = new int[tempCount];
        var defIndex = new int[tempCount];
        var useCount = new int[tempCount];
        Array.Fill(defIndex, -1);

        for (int i = 0; i < instructions.Count; i++)
        {
            var instruction = instructions[i];

            if (LIRInstructionInfo.TryGetDefinedTemp(instruction, out var defined)
                && defined.Index >= 0
                && defined.Index < tempCount)
            {
                defCount[defined.Index]++;
                defIndex[defined.Index] = i;
            }

            var visitor = new UseCountVisitor(useCount, tempCount);
            LIRInstructionInfo.VisitUsedTemps(instruction, ref visitor);
        }

        for (int i = 1; i < instructions.Count; i++)
        {
            if (instructions[i] is not LIRCopyTemp copy)
            {
                continue;
            }

            if (!IsCoalesceable(methodBody, copy, i, tempCount, defCount, defIndex, useCount))
            {
                continue;
            }

            methodBody.TempVariableSlots[copy.Source.Index] = methodBody.TempVariableSlots[copy.Destination.Index];
        }
    }

    private static bool IsCoalesceable(
        MethodBodyIR methodBody,
        LIRCopyTemp copy,
        int copyIndex,
        int tempCount,
        int[] defCount,
        int[] defIndex,
        int[] useCount)
    {
        var source = copy.Source;
        var destination = copy.Destination;

        if (source.Index < 0 || source.Index >= tempCount)
        {
            return false;
        }

        if (destination.Index < 0 || destination.Index >= tempCount)
        {
            return false;
        }

        if (source.Index == destination.Index)
        {
            return false;
        }

        var slots = methodBody.TempVariableSlots;
        if (source.Index >= slots.Count || destination.Index >= slots.Count)
        {
            return false;
        }

        // The destination must be the stable variable slot we are coalescing into, and the source
        // must not already be pinned. Repinning an already-pinned source would retroactively change
        // the IL local that earlier instructions read and write.
        var destinationSlot = slots[destination.Index];
        if (destinationSlot < 0 || slots[source.Index] >= 0)
        {
            return false;
        }

        // The producer must sit immediately before the copy. Adjacency is what makes moving the
        // write to the variable local unobservable: nothing can read the slot in between, no label
        // or branch can split the pair, and no exception region boundary can fall between them.
        var producerIndex = copyIndex - 1;
        if (defCount[source.Index] != 1 || defIndex[source.Index] != producerIndex)
        {
            return false;
        }

        // The copy must be the source's only consumer. Any other read would observe the variable
        // local, which later assignments to the same binding are free to overwrite.
        if (useCount[source.Index] != 1)
        {
            return false;
        }

        // The copy must be the destination's only producer, otherwise the destination temp is not a
        // simple alias for the value we are coalescing.
        if (defCount[destination.Index] != 1 || defIndex[destination.Index] != copyIndex)
        {
            return false;
        }

        // Once pinned, the source is emitted with the variable slot's storage, so the copy must be
        // a pure representation-preserving move. Anything that would box, unbox, or cast has to keep
        // its own temp.
        if (destinationSlot >= methodBody.VariableStorages.Count)
        {
            return false;
        }

        var slotStorage = methodBody.VariableStorages[destinationSlot];
        if (slotStorage.Kind == ValueStorageKind.Unknown)
        {
            return false;
        }

        var sourceStorage = GetTempStorage(methodBody, source);
        var destinationStorage = GetTempStorage(methodBody, destination);

        return ValueStorageFacts.IsSameRuntimeRepresentation(sourceStorage, slotStorage)
            && ValueStorageFacts.IsSameRuntimeRepresentation(destinationStorage, slotStorage);
    }

    private static ValueStorage GetTempStorage(MethodBodyIR methodBody, TempVariable temp)
    {
        if (temp.Index >= 0 && temp.Index < methodBody.TempStorages.Count)
        {
            return methodBody.TempStorages[temp.Index];
        }

        return new ValueStorage(ValueStorageKind.Unknown);
    }

    private struct UseCountVisitor : ITempUseVisitor
    {
        private readonly int[] _useCount;
        private readonly int _tempCount;

        public UseCountVisitor(int[] useCount, int tempCount)
        {
            _useCount = useCount;
            _tempCount = tempCount;
        }

        public void Visit(TempVariable temp)
        {
            if (temp.Index >= 0 && temp.Index < _tempCount)
            {
                _useCount[temp.Index]++;
            }
        }
    }
}
