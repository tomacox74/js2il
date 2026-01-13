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
            // Skip allocation for temps that are already mapped to a variable slot.
            if (TryGetDefinedTemp(instruction, out var defined) &&
                defined.Index >= 0 &&
                defined.Index < tempCount &&
                lastUse[defined.Index] >= 0 &&
                (shouldMaterializeTemp is null || shouldMaterializeTemp[defined.Index]) &&
                !CanEmitInline(instruction, methodBody) &&
                !(defined.Index < methodBody.TempVariableSlots.Count && methodBody.TempVariableSlots[defined.Index] >= 0))
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
        if (instruction is LIRConstNumber or LIRConstString or LIRConstBoolean or LIRConstUndefined or LIRConstNull or LIRLoadParameter or LIRLoadThis)
        {
            return true;
        }

        // Pure loads that can safely stay on the stack.
        if (instruction is LIRLoadUserClassInstanceField)
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
            case LIRCallIntrinsic call:
                yield return call.IntrinsicObject;
                yield return call.ArgumentsArray;
                break;

            case LIRCallIntrinsicGlobalFunction callGlobal:
                foreach (var arg in callGlobal.Arguments)
                {
                    yield return arg;
                }
                break;
            case LIRCallInstanceMethod callInstance:
                yield return callInstance.Receiver;
                foreach (var arg in callInstance.Arguments)
                {
                    yield return arg;
                }
                break;
            case LIRCallIntrinsicStatic callStatic:
                foreach (var arg in callStatic.Arguments)
                {
                    yield return arg;
                }
                break;

            case LIRCallIntrinsicStaticVoid callStaticVoid:
                foreach (var arg in callStaticVoid.Arguments)
                {
                    yield return arg;
                }
                break;
            case LIRConvertToObject conv:
                yield return conv.Source;
                break;
            case LIRConvertToNumber convNum:
                yield return convNum.Source;
                break;
            case LIRConvertToBoolean convBool:
                yield return convBool.Source;
                break;
            case LIRConvertToString convString:
                yield return convString.Source;
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
            case LIRLogicalNot logicalNot:
                yield return logicalNot.Value;
                break;

            case LIRIsInstanceOf isInstanceOf:
                yield return isInstanceOf.Value;
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
            case LIRDivNumber div:
                yield return div.Left;
                yield return div.Right;
                break;
            case LIRModNumber mod:
                yield return mod.Left;
                yield return mod.Right;
                break;
            case LIRExpNumber exp:
                yield return exp.Left;
                yield return exp.Right;
                break;
            case LIRBitwiseAnd bitwiseAnd:
                yield return bitwiseAnd.Left;
                yield return bitwiseAnd.Right;
                break;
            case LIRBitwiseOr bitwiseOr:
                yield return bitwiseOr.Left;
                yield return bitwiseOr.Right;
                break;
            case LIRBitwiseXor bitwiseXor:
                yield return bitwiseXor.Left;
                yield return bitwiseXor.Right;
                break;
            case LIRLeftShift leftShift:
                yield return leftShift.Left;
                yield return leftShift.Right;
                break;
            case LIRRightShift rightShift:
                yield return rightShift.Left;
                yield return rightShift.Right;
                break;
            case LIRUnsignedRightShift unsignedRightShift:
                yield return unsignedRightShift.Left;
                yield return unsignedRightShift.Right;
                break;
            case LIRCallIsTruthy callIsTruthy:
                yield return callIsTruthy.Value;
                break;
            case LIRCopyTemp copyTemp:
                yield return copyTemp.Source;
                break;
            case LIRInOperator inOp:
                yield return inOp.Left;
                yield return inOp.Right;
                break;
            case LIREqualDynamic equalDyn:
                yield return equalDyn.Left;
                yield return equalDyn.Right;
                break;
            case LIRNotEqualDynamic notEqualDyn:
                yield return notEqualDyn.Left;
                yield return notEqualDyn.Right;
                break;
            case LIRStrictEqualDynamic strictEqualDyn:
                yield return strictEqualDyn.Left;
                yield return strictEqualDyn.Right;
                break;
            case LIRStrictNotEqualDynamic strictNotEqualDyn:
                yield return strictNotEqualDyn.Left;
                yield return strictNotEqualDyn.Right;
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
            case LIRCallFunctionValue callValue:
                yield return callValue.FunctionValue;
                yield return callValue.ScopesArray;
                yield return callValue.ArgumentsArray;
                break;
            case LIRCallMember callMember:
                yield return callMember.Receiver;
                yield return callMember.ArgumentsArray;
                break;

            case LIRCallUserClassInstanceMethod callUserClass:
                foreach (var arg in callUserClass.Arguments)
                {
                    yield return arg;
                }
                break;
            case LIRBuildScopesArray:
                // LIRBuildScopesArray doesn't consume temps - it loads scope instances from locals/args directly
                break;
            case LIRLoadThis:
                // LIRLoadThis doesn't consume any temps (it loads from IL argument 0)
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
            case LIRNewJsArray newJsArray:
                foreach (var elem in newJsArray.Elements)
                {
                    yield return elem;
                }
                break;
            case LIRNewJsObject newJsObject:
                foreach (var prop in newJsObject.Properties)
                {
                    yield return prop.Value;
                }
                break;
            case LIRGetLength getLength:
                yield return getLength.Object;
                break;
            case LIRGetItem getItem:
                yield return getItem.Object;
                yield return getItem.Index;
                break;
            case LIRSetItem setItem:
                yield return setItem.Object;
                yield return setItem.Index;
                yield return setItem.Value;
                break;
            case LIRArrayPushRange pushRange:
                yield return pushRange.TargetArray;
                yield return pushRange.SourceArray;
                break;
            case LIRArrayAdd arrayAdd:
                yield return arrayAdd.TargetArray;
                yield return arrayAdd.Element;
                break;
            case LIRNewBuiltInError newError:
                if (newError.Message.HasValue)
                {
                    yield return newError.Message.Value;
                }
                break;
            case LIRNewIntrinsicObject newIntrinsic:
                foreach (var arg in newIntrinsic.Arguments)
                {
                    yield return arg;
                }
                break;
            case LIRNewUserClass newUserClass:
                if (newUserClass.NeedsScopes && newUserClass.ScopesArray.HasValue)
                {
                    yield return newUserClass.ScopesArray.Value;
                }
                foreach (var arg in newUserClass.Arguments)
                {
                    yield return arg;
                }
                break;

            case LIRCallDeclaredCallable callDeclared:
                foreach (var arg in callDeclared.Arguments)
                {
                    yield return arg;
                }
                break;

            case LIRCreateBoundArrowFunction createArrow:
                yield return createArrow.ScopesArray;
                break;

            case LIRCreateBoundFunctionExpression createFunc:
                yield return createFunc.ScopesArray;
                break;
            case LIRStoreUserClassInstanceField storeInstanceField:
                yield return storeInstanceField.Value;
                break;

            case LIRStoreUserClassStaticField storeStaticField:
                yield return storeStaticField.Value;
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
            case LIRCallIntrinsicGlobalFunction callGlobal:
                defined = callGlobal.Result;
                return true;
            case LIRCallInstanceMethod callInstance:
                defined = callInstance.Result;
                return true;
            case LIRCallIntrinsicStatic callStatic:
                defined = callStatic.Result;
                return true;

            case LIRCallIntrinsicStaticVoid:
                defined = default;
                return false;
            case LIRConvertToObject conv:
                defined = conv.Result;
                return true;
            case LIRConvertToNumber convNum:
                defined = convNum.Result;
                return true;
            case LIRConvertToBoolean convBool:
                defined = convBool.Result;
                return true;
            case LIRConvertToString convString:
                defined = convString.Result;
                return true;
            case LIRIsInstanceOf isInstanceOf:
                defined = isInstanceOf.Result;
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
            case LIRLogicalNot logicalNot:
                defined = logicalNot.Result;
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
            case LIRDivNumber div:
                defined = div.Result;
                return true;
            case LIRModNumber mod:
                defined = mod.Result;
                return true;
            case LIRExpNumber exp:
                defined = exp.Result;
                return true;
            case LIRBitwiseAnd bitwiseAnd:
                defined = bitwiseAnd.Result;
                return true;
            case LIRBitwiseOr bitwiseOr:
                defined = bitwiseOr.Result;
                return true;
            case LIRBitwiseXor bitwiseXor:
                defined = bitwiseXor.Result;
                return true;
            case LIRLeftShift leftShift:
                defined = leftShift.Result;
                return true;
            case LIRRightShift rightShift:
                defined = rightShift.Result;
                return true;
            case LIRUnsignedRightShift unsignedRightShift:
                defined = unsignedRightShift.Result;
                return true;
            case LIRCallIsTruthy callIsTruthy:
                defined = callIsTruthy.Result;
                return true;
            case LIRCopyTemp copyTemp:
                defined = copyTemp.Destination;
                return true;
            case LIRInOperator inOp:
                defined = inOp.Result;
                return true;
            case LIREqualDynamic equalDyn:
                defined = equalDyn.Result;
                return true;
            case LIRNotEqualDynamic notEqualDyn:
                defined = notEqualDyn.Result;
                return true;
            case LIRStrictEqualDynamic strictEqualDyn:
                defined = strictEqualDyn.Result;
                return true;
            case LIRStrictNotEqualDynamic strictNotEqualDyn:
                defined = strictNotEqualDyn.Result;
                return true;
            case LIRCallFunction callFunc:
                defined = callFunc.Result;
                return true;

            case LIRCallFunctionValue callValue:
                defined = callValue.Result;
                return true;

            case LIRCallMember callMember:
                defined = callMember.Result;
                return true;

            case LIRCallUserClassInstanceMethod callUserClass:
                defined = callUserClass.Result;
                return true;

            case LIRCreateBoundArrowFunction createArrow:
                defined = createArrow.Result;
                return true;

            case LIRCreateBoundFunctionExpression createFunc:
                defined = createFunc.Result;
                return true;
            case LIRBuildScopesArray buildScopes:
                defined = buildScopes.Result;
                return true;
            case LIRLoadThis loadThis:
                defined = loadThis.Result;
                return true;
            case LIRLoadScopesArgument loadScopesArg:
                defined = loadScopesArg.Result;
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
            case LIRNewJsArray newJsArray:
                defined = newJsArray.Result;
                return true;
            case LIRNewJsObject newJsObject:
                defined = newJsObject.Result;
                return true;
            case LIRGetLength getLength:
                defined = getLength.Result;
                return true;
            case LIRGetItem getItem:
                defined = getItem.Result;
                return true;
            case LIRNewBuiltInError newError:
                defined = newError.Result;
                return true;
            case LIRNewIntrinsicObject newIntrinsic:
                defined = newIntrinsic.Result;
                return true;
            case LIRNewUserClass newUserClass:
                defined = newUserClass.Result;
                return true;

            case LIRCallDeclaredCallable callDeclared:
                defined = callDeclared.Result;
                return true;

            case LIRLoadUserClassStaticField loadStaticField:
                defined = loadStaticField.Result;
                return true;

            case LIRLoadUserClassInstanceField loadInstanceField:
                defined = loadInstanceField.Result;
                return true;
            default:
                defined = default;
                return false;
        }
    }
}
