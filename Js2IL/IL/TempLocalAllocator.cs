using System.Reflection.Metadata.Ecma335;
using Js2IL.IR;

namespace Js2IL.IL;

/// <summary>
/// Result of temp local allocation - maps SSA temps to IL local slots.
/// </summary>
internal readonly record struct TempLocalAllocation(int[] TempToSlot, IReadOnlyList<ValueStorage> SlotStorages)
{
    public bool IsMaterialized(TempVariable temp)
        => temp.Index >= 0 && temp.Index < TempToSlot.Length && TempToSlot[temp.Index] >= 0;

    public int GetSlot(TempVariable temp)
    {
        if (temp.Index < 0 || temp.Index >= TempToSlot.Length)
        {
            throw new InvalidOperationException($"Temp index out of range: {temp.Index}");
        }

        var slot = TempToSlot[temp.Index];
        if (slot < 0)
        {
            throw new InvalidOperationException($"Temp {temp.Index} was not materialized into an IL local slot.");
        }

        return slot;
    }
}

/// <summary>
/// Linear-scan register allocator for SSA temps → IL locals.
/// Reuses local slots when temps are no longer live.
/// </summary>
internal static class TempLocalAllocator
{
    private readonly record struct StorageKey(ValueStorageKind Kind, Type? ClrType);

    public static TempLocalAllocation Allocate(MethodBodyIR methodBody, bool[]? shouldMaterializeTemp = null)
    {
        int tempCount = methodBody.Temps.Count;
        if (tempCount == 0)
        {
            return new TempLocalAllocation(Array.Empty<int>(), Array.Empty<ValueStorage>());
        }

        var lastUse = new int[tempCount];
        Array.Fill(lastUse, -1);

        // First pass: determine last use for each temp.
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            foreach (var used in EnumerateUsedTemps(methodBody.Instructions[i])
                .Where(u => u.Index >= 0 && u.Index < tempCount &&
                    (shouldMaterializeTemp is null || shouldMaterializeTemp[u.Index])))
            {
                lastUse[used.Index] = i;
            }
        }

        // Second pass: linear-scan allocation with reuse after last use.
        var tempToSlot = new int[tempCount];
        Array.Fill(tempToSlot, -1);

        var slotStorages = new List<ValueStorage>();
        var freeByKey = new Dictionary<StorageKey, Stack<int>>();

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            var instruction = methodBody.Instructions[i];

            // Free dead operands before allocating the result so we can reuse within the same instruction.
            foreach (var used in EnumerateUsedTemps(instruction))
            {
                if (used.Index < 0 || used.Index >= tempCount)
                {
                    continue;
                }

                if (lastUse[used.Index] != i)
                {
                    continue;
                }

                var usedSlot = tempToSlot[used.Index];
                if (usedSlot < 0)
                {
                    continue;
                }

                var usedStorage = GetTempStorage(methodBody, used);
                var key = new StorageKey(usedStorage.Kind, usedStorage.ClrType);
                if (!freeByKey.TryGetValue(key, out var stack))
                {
                    stack = new Stack<int>();
                    freeByKey[key] = stack;
                }
                stack.Push(usedSlot);
            }

            // Allocate a slot for result if it will be used later.
            // Skip allocation for constant temps that can be emitted inline.
            if (TryGetDefinedTemp(instruction, out var defined) &&
                defined.Index >= 0 &&
                defined.Index < tempCount &&
                lastUse[defined.Index] >= 0 &&
                (shouldMaterializeTemp is null || shouldMaterializeTemp[defined.Index]) &&
                !CanEmitInline(instruction, methodBody))
            {
                var storage = GetTempStorage(methodBody, defined);
                var key = new StorageKey(storage.Kind, storage.ClrType);

                int slot;
                if (freeByKey.TryGetValue(key, out var stack) && stack.Count > 0)
                {
                    slot = stack.Pop();
                }
                else
                {
                    slot = slotStorages.Count;
                    slotStorages.Add(storage);
                }

                tempToSlot[defined.Index] = slot;
            }
        }

        return new TempLocalAllocation(tempToSlot, slotStorages);
    }

    /// <summary>
    /// Returns true if the instruction defines a constant that can be emitted inline
    /// without needing a local variable slot.
    /// </summary>
    private static bool CanEmitInline(LIRInstruction instruction, MethodBodyIR methodBody)
    {
        if (instruction is LIRConstNumber or LIRConstString or LIRConstBoolean or LIRConstUndefined or LIRConstNull or LIRLoadParameter)
        {
            return true;
        }

        // LIRConvertToObject can be emitted inline if its source is an inline constant
        // AND the source is not backed by a variable slot. If the source is backed by a
        // variable slot, that slot may be overwritten by a later SSA value before the
        // box is consumed (e.g., postfix increment: x++ must capture the old value before
        // the slot is updated, so the box must materialize).
        if (instruction is LIRConvertToObject convertToObject)
        {
            var sourceIdx = convertToObject.Source.Index;
            // If the source is backed by a variable slot, don't inline - the slot may be modified
            if (sourceIdx >= 0 && sourceIdx < methodBody.TempVariableSlots.Count && methodBody.TempVariableSlots[sourceIdx] >= 0)
            {
                return false;
            }
            var sourceDefinition = TryFindDefInstruction(methodBody, convertToObject.Source);
            return sourceDefinition != null && CanEmitInline(sourceDefinition, methodBody);
        }

        return false;
    }

    /// <summary>
    /// Finds the instruction that defines the given temp variable.
    /// </summary>
    private static LIRInstruction? TryFindDefInstruction(MethodBodyIR methodBody, TempVariable temp)
    {
        foreach (var inst in methodBody.Instructions)
        {
            if (TryGetDefinedTemp(inst, out var def) && def.Index == temp.Index)
            {
                return inst;
            }
        }
        return null;
    }

    private static ValueStorage GetTempStorage(MethodBodyIR methodBody, TempVariable temp)
    {
        if (temp.Index >= 0 && temp.Index < methodBody.TempStorages.Count)
        {
            return methodBody.TempStorages[temp.Index];
        }

        return new ValueStorage(ValueStorageKind.Unknown);
    }

    /// <summary>
    /// Enumerates all temps used (read) by an instruction.
    /// </summary>
    internal static IEnumerable<TempVariable> EnumerateUsedTemps(LIRInstruction instruction)
    {
        switch (instruction)
        {
            case LIRAddNumber add:
                yield return add.Left;
                yield return add.Right;
                break;
            case LIRConcatStrings concat:
                yield return concat.Left;
                yield return concat.Right;
                break;
            case LIRAddDynamic addDyn:
                yield return addDyn.Left;
                yield return addDyn.Right;
                break;
            case LIRSubNumber sub:
                yield return sub.Left;
                yield return sub.Right;
                break;
            case LIRMulNumber mul:
                yield return mul.Left;
                yield return mul.Right;
                break;
            case LIRMulDynamic mulDyn:
                yield return mulDyn.Left;
                yield return mulDyn.Right;
                break;
            case LIRBeginInitArrayElement begin:
                yield return begin.Array;
                break;
            case LIRCallIntrinsic call:
                yield return call.IntrinsicObject;
                yield return call.ArgumentsArray;
                break;
            case LIRConvertToObject conv:
                yield return conv.Source;
                break;
            case LIRTypeof t:
                yield return t.Value;
                break;
            case LIRNegateNumber neg:
                yield return neg.Value;
                break;
            case LIRBitwiseNotNumber not:
                yield return not.Value;
                break;
            case LIRCompareNumberLessThan cmp:
                yield return cmp.Left;
                yield return cmp.Right;
                break;
            case LIRCompareNumberGreaterThan cmp:
                yield return cmp.Left;
                yield return cmp.Right;
                break;
            case LIRCompareNumberLessThanOrEqual cmp:
                yield return cmp.Left;
                yield return cmp.Right;
                break;
            case LIRCompareNumberGreaterThanOrEqual cmp:
                yield return cmp.Left;
                yield return cmp.Right;
                break;
            case LIRCompareNumberEqual cmp:
                yield return cmp.Left;
                yield return cmp.Right;
                break;
            case LIRCompareNumberNotEqual cmp:
                yield return cmp.Left;
                yield return cmp.Right;
                break;
            case LIRCompareBooleanEqual cmp:
                yield return cmp.Left;
                yield return cmp.Right;
                break;
            case LIRCompareBooleanNotEqual cmp:
                yield return cmp.Left;
                yield return cmp.Right;
                break;
            case LIRStoreElementRef store:
                yield return store.Array;
                yield return store.Value;
                break;
            case LIRReturn ret:
                yield return ret.ReturnValue;
                break;
            case LIRBranchIfFalse branchFalse:
                yield return branchFalse.Condition;
                break;
            case LIRBranchIfTrue branchTrue:
                yield return branchTrue.Condition;
                break;
            case LIRCallFunction callFunc:
                yield return callFunc.ScopesArray;
                foreach (var arg in callFunc.Arguments)
                {
                    yield return arg;
                }
                break;
            case LIRCreateScopesArray:
                // The GlobalScope field on LIRCreateScopesArray does not currently correspond to any temp,
                // so no temps are consumed from it here.
                break;
            case LIRLoadParameter:
                // LIRLoadParameter doesn't consume any temps (it loads from IL argument)
                break;
            case LIRStoreLeafScopeField storeLeaf:
                yield return storeLeaf.Value;
                break;
            case LIRStoreParentScopeField storeParent:
                yield return storeParent.Value;
                break;
            case LIRLoadLeafScopeField:
            case LIRLoadParentScopeField:
                // Load instructions don't consume temps (they load from scope fields)
                break;
            case LIRBuildArray buildArray:
                foreach (var elem in buildArray.Elements)
                {
                    yield return elem;
                }
                break;
            // LIRLabel and LIRBranch don't use temps
        }
    }

    /// <summary>
    /// Gets the temp defined (written) by an instruction, if any.
    /// </summary>
    internal static bool TryGetDefinedTemp(LIRInstruction instruction, out TempVariable defined)
    {
        switch (instruction)
        {
            case LIRConstNumber c:
                defined = c.Result;
                return true;
            case LIRConstString c:
                defined = c.Result;
                return true;
            case LIRConstBoolean c:
                defined = c.Result;
                return true;
            case LIRConstUndefined c:
                defined = c.Result;
                return true;
            case LIRConstNull c:
                defined = c.Result;
                return true;
            case LIRGetIntrinsicGlobal g:
                defined = g.Result;
                return true;
            case LIRNewObjectArray n:
                defined = n.Result;
                return true;
            case LIRAddNumber add:
                defined = add.Result;
                return true;
            case LIRConcatStrings concat:
                defined = concat.Result;
                return true;
            case LIRAddDynamic addDyn:
                defined = addDyn.Result;
                return true;
            case LIRSubNumber sub:
                defined = sub.Result;
                return true;
            case LIRMulNumber mul:
                defined = mul.Result;
                return true;
            case LIRMulDynamic mulDyn:
                defined = mulDyn.Result;
                return true;
            case LIRCallIntrinsic call:
                defined = call.Result;
                return true;
            case LIRConvertToObject conv:
                defined = conv.Result;
                return true;
            case LIRTypeof t:
                defined = t.Result;
                return true;
            case LIRNegateNumber neg:
                defined = neg.Result;
                return true;
            case LIRBitwiseNotNumber not:
                defined = not.Result;
                return true;
            case LIRCompareNumberLessThan cmp:
                defined = cmp.Result;
                return true;
            case LIRCompareNumberGreaterThan cmp:
                defined = cmp.Result;
                return true;
            case LIRCompareNumberLessThanOrEqual cmp:
                defined = cmp.Result;
                return true;
            case LIRCompareNumberGreaterThanOrEqual cmp:
                defined = cmp.Result;
                return true;
            case LIRCompareNumberEqual cmp:
                defined = cmp.Result;
                return true;
            case LIRCompareNumberNotEqual cmp:
                defined = cmp.Result;
                return true;
            case LIRCompareBooleanEqual cmp:
                defined = cmp.Result;
                return true;
            case LIRCompareBooleanNotEqual cmp:
                defined = cmp.Result;
                return true;
            case LIRCallFunction callFunc:
                defined = callFunc.Result;
                return true;
            case LIRCreateScopesArray createScopes:
                defined = createScopes.Result;
                return true;
            case LIRLoadParameter loadParam:
                defined = loadParam.Result;
                return true;
            case LIRLoadLeafScopeField loadLeaf:
                defined = loadLeaf.Result;
                return true;
            case LIRLoadParentScopeField loadParent:
                defined = loadParent.Result;
                return true;
            case LIRBuildArray buildArray:
                defined = buildArray.Result;
                return true;
            default:
                defined = default;
                return false;
        }
    }
}
