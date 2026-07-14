using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Jroc.IR;

namespace Jroc.IL;

internal interface ITempUseVisitor
{
    void Visit(TempVariable temp);
}

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
    private readonly record struct StorageKey(ValueStorageKind Kind, Type? ClrType, EntityHandle TypeHandle, string? ScopeName);

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
            var visitor = new LastUseVisitor(lastUse, tempCount, shouldMaterializeTemp, i);
            VisitUsedTemps(methodBody.Instructions[i], ref visitor);
        }

        // Second pass: linear-scan allocation with reuse after last use.
        var tempToSlot = new int[tempCount];
        Array.Fill(tempToSlot, -1);

        // Def-instruction lookup built once so CanEmitInline doesn't rescan the
        // instruction list per query (O(N^2) on large method bodies, issue #1415).
        var defInstructions = BuildDefInstructionMap(methodBody);

        var slotStorages = new List<ValueStorage>();
        var freeByKey = new Dictionary<StorageKey, Stack<int>>();

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            var instruction = methodBody.Instructions[i];

            // Free dead operands before allocating the result so we can reuse within the same instruction.
            var releaseVisitor = new ReleaseDeadTempsVisitor(
                methodBody,
                lastUse,
                tempToSlot,
                tempCount,
                i,
                freeByKey);
            VisitUsedTemps(instruction, ref releaseVisitor);

            // Allocate a slot for result if it will be used later.
            // Skip allocation for constant temps that can be emitted inline.
            // Skip allocation for temps that are already mapped to a variable slot.
            if (TryGetDefinedTemp(instruction, out var defined) &&
                defined.Index >= 0 &&
                defined.Index < tempCount &&
                lastUse[defined.Index] >= 0 &&
                (shouldMaterializeTemp is null || shouldMaterializeTemp[defined.Index]) &&
                !CanEmitInline(instruction, methodBody, defInstructions) &&
                !(defined.Index < methodBody.TempVariableSlots.Count && methodBody.TempVariableSlots[defined.Index] >= 0))
            {
                var storage = GetTempStorage(methodBody, defined);
                var key = new StorageKey(storage.Kind, storage.ClrType, storage.TypeHandle, storage.ScopeName);

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
    private static bool CanEmitInline(LIRInstruction instruction, MethodBodyIR methodBody, Dictionary<int, LIRInstruction> defInstructions)
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
            var sourceDefinition = defInstructions.TryGetValue(convertToObject.Source.Index, out var def) ? def : null;
            return sourceDefinition != null && CanEmitInline(sourceDefinition, methodBody, defInstructions);
        }

        return false;
    }

    /// <summary>
    /// Builds a temp-index → defining-instruction map in a single pass.
    /// Keeps the first definition per temp to match the previous first-match scan semantics.
    /// </summary>
    private static Dictionary<int, LIRInstruction> BuildDefInstructionMap(MethodBodyIR methodBody)
    {
        var map = new Dictionary<int, LIRInstruction>(methodBody.Instructions.Count);
        foreach (var inst in methodBody.Instructions)
        {
            if (TryGetDefinedTemp(inst, out var def))
            {
                map.TryAdd(def.Index, inst);
            }
        }
        return map;
    }

    private static ValueStorage GetTempStorage(MethodBodyIR methodBody, TempVariable temp)
    {
        if (temp.Index >= 0 && temp.Index < methodBody.TempVariableSlots.Count)
        {
            var variableSlot = methodBody.TempVariableSlots[temp.Index];
            if (variableSlot >= 0 && variableSlot < methodBody.VariableStorages.Count)
            {
                return methodBody.VariableStorages[variableSlot];
            }
        }

        if (temp.Index >= 0 && temp.Index < methodBody.TempStorages.Count)
        {
            return methodBody.TempStorages[temp.Index];
        }

        return new ValueStorage(ValueStorageKind.Unknown);
    }

    private struct LastUseVisitor : ITempUseVisitor
    {
        private readonly int[] _lastUse;
        private readonly int _tempCount;
        private readonly bool[]? _shouldMaterializeTemp;
        private readonly int _instructionIndex;

        public LastUseVisitor(int[] lastUse, int tempCount, bool[]? shouldMaterializeTemp, int instructionIndex)
        {
            _lastUse = lastUse;
            _tempCount = tempCount;
            _shouldMaterializeTemp = shouldMaterializeTemp;
            _instructionIndex = instructionIndex;
        }

        public void Visit(TempVariable temp)
        {
            if (temp.Index >= 0
                && temp.Index < _tempCount
                && (_shouldMaterializeTemp is null || _shouldMaterializeTemp[temp.Index]))
            {
                _lastUse[temp.Index] = _instructionIndex;
            }
        }
    }

    private struct ReleaseDeadTempsVisitor : ITempUseVisitor
    {
        private readonly MethodBodyIR _methodBody;
        private readonly int[] _lastUse;
        private readonly int[] _tempToSlot;
        private readonly int _tempCount;
        private readonly int _instructionIndex;
        private readonly Dictionary<StorageKey, Stack<int>> _freeByKey;

        public ReleaseDeadTempsVisitor(
            MethodBodyIR methodBody,
            int[] lastUse,
            int[] tempToSlot,
            int tempCount,
            int instructionIndex,
            Dictionary<StorageKey, Stack<int>> freeByKey)
        {
            _methodBody = methodBody;
            _lastUse = lastUse;
            _tempToSlot = tempToSlot;
            _tempCount = tempCount;
            _instructionIndex = instructionIndex;
            _freeByKey = freeByKey;
        }

        public void Visit(TempVariable temp)
        {
            if (temp.Index < 0
                || temp.Index >= _tempCount
                || _lastUse[temp.Index] != _instructionIndex)
            {
                return;
            }

            var usedSlot = _tempToSlot[temp.Index];
            if (usedSlot < 0)
            {
                return;
            }

            var usedStorage = GetTempStorage(_methodBody, temp);
            var key = new StorageKey(usedStorage.Kind, usedStorage.ClrType, usedStorage.TypeHandle, usedStorage.ScopeName);
            if (!_freeByKey.TryGetValue(key, out var stack))
            {
                stack = new Stack<int>();
                _freeByKey[key] = stack;
            }

            stack.Push(usedSlot);
        }
    }

    /// <summary>
    /// Visits all temps read by an instruction without allocating an iterator object.
    /// </summary>
    internal static void VisitUsedTemps<TVisitor>(LIRInstruction instruction, ref TVisitor visitor)
        where TVisitor : struct, ITempUseVisitor
    {
        switch (instruction)
        {
            case LIRAddNumber value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRConcatStrings value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRAddDynamic value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRAddDynamicDoubleObject value:
                visitor.Visit(value.LeftDouble); visitor.Visit(value.RightObject); break;
            case LIRAddDynamicObjectDouble value:
                visitor.Visit(value.LeftObject); visitor.Visit(value.RightDouble); break;
            case LIRAddAndToNumber value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRSubNumber value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRMulNumber value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRMulDynamic value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRCallIntrinsic value:
                visitor.Visit(value.IntrinsicObject); visitor.Visit(value.ArgumentsArray); break;
            case LIRCallIntrinsicGlobalFunction value:
                VisitList(value.Arguments, ref visitor); break;
            case LIRCallInstanceMethod value:
                visitor.Visit(value.Receiver); VisitList(value.Arguments, ref visitor); break;
            case LIRCallIntrinsicStatic value:
                VisitList(value.Arguments, ref visitor); break;
            case LIRCallIntrinsicStaticVoid value:
                VisitList(value.Arguments, ref visitor); break;
            case LIRCallIntrinsicStaticWithArgsArray value:
                visitor.Visit(value.ArgumentsArray); break;
            case LIRCallIntrinsicStaticVoidWithArgsArray value:
                visitor.Visit(value.ArgumentsArray); break;
            case LIRCallRuntimeServicesStatic value:
                VisitList(value.Arguments, ref visitor); break;
            case LIRConvertToObject value:
                visitor.Visit(value.Source); break;
            case LIRConvertToNumber value:
                visitor.Visit(value.Source); break;
            case LIRConvertToNumberDiscard value:
                visitor.Visit(value.Source); break;
            case LIRConvertToBoolean value:
                visitor.Visit(value.Source); break;
            case LIRConvertToString value:
                visitor.Visit(value.Source); break;
            case LIRTypeof value:
                visitor.Visit(value.Value); break;
            case LIRNegateNumber value:
                visitor.Visit(value.Value); break;
            case LIRNegateNumberDynamic value:
                visitor.Visit(value.Value); break;
            case LIRBitwiseNotNumber value:
                visitor.Visit(value.Value); break;
            case LIRBitwiseNotDynamic value:
                visitor.Visit(value.Value); break;
            case LIRLogicalNot value:
                visitor.Visit(value.Value); break;
            case LIRIsInstanceOf value:
                visitor.Visit(value.Value); break;
            case LIRCompareNumberLessThan value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRCompareNumberGreaterThan value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRCompareNumberLessThanOrEqual value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRCompareNumberGreaterThanOrEqual value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRCompareNumberEqual value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRCompareNumberNotEqual value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRCompareBooleanEqual value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRCompareBooleanNotEqual value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRDivNumber value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRModNumber value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRExpNumber value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRBitwiseAnd value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRBitwiseOr value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRBitwiseXor value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRLeftShift value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRRightShift value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRUnsignedRightShift value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRCallIsTruthy value:
                visitor.Visit(value.Value); break;
            case LIRCallIsTruthyDouble value:
                visitor.Visit(value.Value); break;
            case LIRCallIsTruthyBool value:
                visitor.Visit(value.Value); break;
            case LIRCopyTemp value:
                visitor.Visit(value.Source); break;
            case LIRInOperator value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRInstanceOfOperator value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIREqualDynamic value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRNotEqualDynamic value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRStrictEqualDynamic value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRStrictNotEqualDynamic value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRBinaryDynamicOperator value:
                visitor.Visit(value.Left); visitor.Visit(value.Right); break;
            case LIRReturn value:
                visitor.Visit(value.ReturnValue); break;
            case LIRBranchIfFalse value:
                visitor.Visit(value.Condition); break;
            case LIRBranchIfTrue value:
                visitor.Visit(value.Condition); break;
            case LIRCallFunction value:
                visitor.Visit(value.ScopesArray); VisitList(value.Arguments, ref visitor); break;
            case LIRTailCallFunctionReturn value:
                visitor.Visit(value.ScopesArray); VisitList(value.Arguments, ref visitor); break;
            case LIRCallFunctionWithArgsArray value:
                visitor.Visit(value.ScopesArray); visitor.Visit(value.ArgumentsArray); break;
            case LIRCallFunctionValue value:
                visitor.Visit(value.FunctionValue); visitor.Visit(value.ScopesArray); visitor.Visit(value.ArgumentsArray); break;
            case LIRCallFunctionValue0 value:
                visitor.Visit(value.FunctionValue); visitor.Visit(value.ScopesArray); break;
            case LIRCallFunctionValue1 value:
                visitor.Visit(value.FunctionValue); visitor.Visit(value.ScopesArray); visitor.Visit(value.A0); break;
            case LIRCallFunctionValue2 value:
                visitor.Visit(value.FunctionValue); visitor.Visit(value.ScopesArray); visitor.Visit(value.A0); visitor.Visit(value.A1); break;
            case LIRCallFunctionValue3 value:
                visitor.Visit(value.FunctionValue); visitor.Visit(value.ScopesArray); visitor.Visit(value.A0); visitor.Visit(value.A1); visitor.Visit(value.A2); break;
            case LIRCallRequire value:
                visitor.Visit(value.RequireValue); visitor.Visit(value.ModuleId); break;
            case LIRCallImport value:
                visitor.Visit(value.ModuleSpecifier); visitor.Visit(value.CurrentModuleId); break;
            case LIRConstructValue value:
                visitor.Visit(value.ConstructorValue); visitor.Visit(value.ArgumentsArray); break;
            case LIRCallFunctionBaseConstructor value:
                visitor.Visit(value.ConstructorValue); visitor.Visit(value.ArgumentsArray); break;
            case LIRCallMember value:
                visitor.Visit(value.Receiver); visitor.Visit(value.ArgumentsArray); break;
            case LIRCallMember0 value:
                visitor.Visit(value.Receiver); break;
            case LIRCallMember1 value:
                visitor.Visit(value.Receiver); visitor.Visit(value.A0); break;
            case LIRCallMember2 value:
                visitor.Visit(value.Receiver); visitor.Visit(value.A0); visitor.Visit(value.A1); break;
            case LIRCallMember3 value:
                visitor.Visit(value.Receiver); visitor.Visit(value.A0); visitor.Visit(value.A1); visitor.Visit(value.A2); break;
            case LIRCallTypedMember value:
                visitor.Visit(value.Receiver); VisitList(value.Arguments, ref visitor); break;
            case LIRCallTypedMemberWithFallback value:
                visitor.Visit(value.Receiver); VisitList(value.Arguments, ref visitor); break;
            case LIRCallUserClassInstanceMethod value:
                VisitList(value.Arguments, ref visitor); break;
            case LIRCallUserClassBaseConstructor value:
                VisitList(value.Arguments, ref visitor); VisitList(value.AllJsArguments, ref visitor); break;
            case LIRCallIntrinsicBaseConstructor value:
                VisitList(value.Arguments, ref visitor); break;
            case LIRCallUserClassBaseInstanceMethod value:
                VisitList(value.Arguments, ref visitor); break;
            case LIRBuildScopesArray value:
                VisitScopeSlots(value.Slots, ref visitor); break;
            case LIRStoreParameter value:
                visitor.Visit(value.Value); break;
            case LIRStoreLeafScopeField value:
                visitor.Visit(value.Value); break;
            case LIRStoreParentScopeField value:
                visitor.Visit(value.Value); break;
            case LIRStoreScopeFieldByName value:
                visitor.Visit(value.Value); break;
            case LIRLoadScopeField value:
                visitor.Visit(value.ScopeInstance); break;
            case LIRStoreScopeField value:
                visitor.Visit(value.ScopeInstance); visitor.Visit(value.Value); break;
            case LIRAwait value:
                visitor.Visit(value.AwaitedValue); break;
            case LIRYield value:
                visitor.Visit(value.YieldedValue); break;
            case LIRAsyncCallMoveNext value:
                visitor.Visit(value.ScopesArray); break;
            case LIRAsyncResolve value:
                visitor.Visit(value.Value); break;
            case LIRAsyncReject value:
                visitor.Visit(value.Reason); break;
            case LIRAsyncStateSwitch value:
                visitor.Visit(value.StateValue); break;
            case LIRAsyncStoreAwaitedResult value:
                visitor.Visit(value.Value); break;
            case LIRBuildArray value:
                VisitList(value.Elements, ref visitor); break;
            case LIRNewJsArray value:
                VisitList(value.Elements, ref visitor); break;
            case LIRNewJsObject value:
                VisitObjectPropertyValues(value.Properties, ref visitor); break;
            case LIRNewInferredJsObject value:
                VisitInferredObjectPropertyValues(value.Properties, ref visitor); break;
            case LIRGetInferredMember value:
                visitor.Visit(value.Receiver); break;
            case LIRSetInferredMember value:
                visitor.Visit(value.Receiver); visitor.Visit(value.Value); break;
            case LIRGetLength value:
                visitor.Visit(value.Object); break;
            case LIRGetStringLength value:
                visitor.Visit(value.Receiver); break;
            case LIRGetJsArrayLength value:
                visitor.Visit(value.Receiver); break;
            case LIRGetInt32ArrayLength value:
                visitor.Visit(value.Receiver); break;
            case LIRGetItem value:
                visitor.Visit(value.Object); visitor.Visit(value.Index); break;
            case LIRGetItemAsNumber value:
                visitor.Visit(value.Object); visitor.Visit(value.Index); break;
            case LIRGetItemAsNumberString value:
                visitor.Visit(value.Object); visitor.Visit(value.Index); break;
            case LIRSetItem value:
                visitor.Visit(value.Object); visitor.Visit(value.Index); visitor.Visit(value.Value); break;
            case LIRSetJsArrayLength value:
                visitor.Visit(value.Receiver); visitor.Visit(value.Value); break;
            case LIRGetJsArrayElement value:
                visitor.Visit(value.Receiver); visitor.Visit(value.Index); break;
            case LIRSetJsArrayElement value:
                visitor.Visit(value.Receiver); visitor.Visit(value.Index); visitor.Visit(value.Value); break;
            case LIRGetInt32ArrayElement value:
                visitor.Visit(value.Receiver); visitor.Visit(value.Index); break;
            case LIRSetInt32ArrayElement value:
                visitor.Visit(value.Receiver); visitor.Visit(value.Index); visitor.Visit(value.Value); break;
            case LIRArrayPushRange value:
                visitor.Visit(value.TargetArray); visitor.Visit(value.SourceArray); break;
            case LIRArrayAdd value:
                visitor.Visit(value.TargetArray); visitor.Visit(value.Element); break;
            case LIRNewBuiltInError value when value.Message.HasValue:
                visitor.Visit(value.Message.Value); break;
            case LIRNewIntrinsicObject value:
                VisitList(value.Arguments, ref visitor); break;
            case LIRNewUserClass value:
                if (value.NeedsScopes && value.ScopesArray.HasValue)
                {
                    visitor.Visit(value.ScopesArray.Value);
                }
                VisitList(value.Arguments, ref visitor);
                break;
            case LIRCallDeclaredCallable value:
                VisitList(value.Arguments, ref visitor); break;
            case LIRCreateBoundArrowFunction value:
                visitor.Visit(value.ScopesArray); break;
            case LIRCreateBoundFunctionExpression value:
                visitor.Visit(value.ScopesArray); break;
            case LIRStoreUserClassInstanceField value:
                visitor.Visit(value.Value); break;
            case LIRStoreUserClassStaticField value:
                visitor.Visit(value.Value); break;
        }
    }

    private static void VisitList<TVisitor>(IReadOnlyList<TempVariable> temps, ref TVisitor visitor)
        where TVisitor : struct, ITempUseVisitor
    {
        for (int i = 0; i < temps.Count; i++)
        {
            visitor.Visit(temps[i]);
        }
    }

    private static void VisitScopeSlots<TVisitor>(IReadOnlyList<ScopeSlotSource> slots, ref TVisitor visitor)
        where TVisitor : struct, ITempUseVisitor
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (slot.Source == ScopeInstanceSource.Temp && slot.SourceIndex >= 0)
            {
                visitor.Visit(new TempVariable(slot.SourceIndex));
            }
        }
    }

    private static void VisitObjectPropertyValues<TVisitor>(IReadOnlyList<ObjectProperty> properties, ref TVisitor visitor)
        where TVisitor : struct, ITempUseVisitor
    {
        for (int i = 0; i < properties.Count; i++)
        {
            visitor.Visit(properties[i].Value);
        }
    }

    private static void VisitInferredObjectPropertyValues<TVisitor>(IReadOnlyList<InferredObjectProperty> properties, ref TVisitor visitor)
        where TVisitor : struct, ITempUseVisitor
    {
        for (int i = 0; i < properties.Count; i++)
        {
            visitor.Visit(properties[i].Value);
        }
    }

    internal static bool UsesTemp(LIRInstruction instruction, TempVariable target)
    {
        var visitor = new TempMatchVisitor(target.Index);
        VisitUsedTemps(instruction, ref visitor);
        return visitor.Found;
    }

    internal static TempUseSummary GetTempUseSummary(LIRInstruction instruction, TempVariable target)
    {
        var visitor = new TempUseSummaryVisitor(target.Index);
        VisitUsedTemps(instruction, ref visitor);
        return visitor.ToSummary();
    }

    internal readonly record struct TempUseSummary(int Count, TempVariable First, int TargetIndex);

    private struct TempMatchVisitor : ITempUseVisitor
    {
        private readonly int _targetIndex;

        public TempMatchVisitor(int targetIndex)
        {
            _targetIndex = targetIndex;
        }

        public bool Found { get; private set; }

        public void Visit(TempVariable temp)
        {
            Found |= temp.Index == _targetIndex;
        }
    }

    private struct TempUseSummaryVisitor : ITempUseVisitor
    {
        private readonly int _targetIndex;
        private TempVariable _first;

        public TempUseSummaryVisitor(int targetIndex)
        {
            _targetIndex = targetIndex;
            _first = default;
            TargetIndex = -1;
        }

        public int Count { get; private set; }
        public int TargetIndex { get; private set; }

        public void Visit(TempVariable temp)
        {
            if (Count == 0)
            {
                _first = temp;
            }

            if (temp.Index == _targetIndex && TargetIndex < 0)
            {
                TargetIndex = Count;
            }

            Count++;
        }

        public TempUseSummary ToSummary()
            => new(Count, _first, TargetIndex);
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
            case LIRGetUserClassType t:
                defined = t.Result;
                return true;
            case LIRGetIntrinsicGlobal g:
                defined = g.Result;
                return true;
            case LIRGetIntrinsicGlobalFunction gf:
                defined = gf.Result;
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
            case LIRAddDynamicDoubleObject addDynDoubleObject:
                defined = addDynDoubleObject.Result;
                return true;
            case LIRAddDynamicObjectDouble addDynObjectDouble:
                defined = addDynObjectDouble.Result;
                return true;
            case LIRAddAndToNumber addAndToNumber:
                defined = addAndToNumber.Result;
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
            
            case LIRCallRuntimeServicesStatic callRuntimeServices:
                defined = callRuntimeServices.Result;
                return true;

            case LIRConstructValue constructValue:
                defined = constructValue.Result;
                return true;

            case LIRCallIntrinsicStaticVoid:
                defined = default;
                return false;

            case LIRCallIntrinsicStaticVoidWithArgsArray:
                defined = default;
                return false;

            case LIRLoadScopeFieldByName loadScopeByName:
                defined = loadScopeByName.Result;
                return true;

            case LIRCreateScopeInstance createScope:
                defined = createScope.Result;
                return true;

            case LIRLoadScopeField loadScopeField:
                defined = loadScopeField.Result;
                return true;

            // Async / await state machine instructions
            case LIRAwait awaitInstr:
                defined = awaitInstr.Result;
                return true;

            // Generator yield: the yield-expression result temp is populated on resume.
            // We treat it as a defined temp for allocation purposes so the IL emitter has
            // a local slot to store the resumed value into.
            case LIRYield yieldInstr:
                defined = yieldInstr.Result;
                return true;
            case LIRAsyncLoadState loadState:
                defined = loadState.Result;
                return true;
            case LIRAsyncLoadAwaitedResult loadAwaited:
                defined = loadAwaited.Result;
                return true;

            // Instructions that do not define temps
            case LIRAsyncInitialize:
            case LIRAsyncStoreState:
            case LIRAsyncCallMoveNext:
            case LIRAsyncReturnPromise:
            case LIRAsyncResolve:
            case LIRAsyncReject:
            case LIRAsyncStateSwitch:
            case LIRAsyncStoreAwaitedResult:
            case LIRConvertToNumberDiscard:
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
            case LIRNegateNumberDynamic negDyn:
                defined = negDyn.Result;
                return true;
            case LIRBitwiseNotNumber not:
                defined = not.Result;
                return true;
            case LIRBitwiseNotDynamic notDyn:
                defined = notDyn.Result;
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
            case LIRCallIsTruthyDouble callIsTruthyDouble:
                defined = callIsTruthyDouble.Result;
                return true;
            case LIRCallIsTruthyBool callIsTruthyBool:
                defined = callIsTruthyBool.Result;
                return true;
            case LIRCopyTemp copyTemp:
                defined = copyTemp.Destination;
                return true;
            case LIRInOperator inOp:
                defined = inOp.Result;
                return true;
            case LIRInstanceOfOperator instOf:
                defined = instOf.Result;
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
            case LIRBinaryDynamicOperator binaryDynamic:
                defined = binaryDynamic.Result;
                return true;
            case LIRCallFunction callFunc:
                defined = callFunc.Result;
                return true;

            case LIRCallFunctionWithArgsArray callWithArgsArray:
                defined = callWithArgsArray.Result;
                return true;

            case LIRCallFunctionValue callValue:
                defined = callValue.Result;
                return true;
            case LIRCallFunctionValue0 callValue0:
                defined = callValue0.Result;
                return true;
            case LIRCallFunctionValue1 callValue1:
                defined = callValue1.Result;
                return true;
            case LIRCallFunctionValue2 callValue2:
                defined = callValue2.Result;
                return true;
            case LIRCallFunctionValue3 callValue3:
                defined = callValue3.Result;
                return true;

            case LIRCallRequire callRequire:
                defined = callRequire.Result;
                return true;

            case LIRCallImport callImport:
                defined = callImport.Result;
                return true;

            case LIRCallMember callMember:
                defined = callMember.Result;
                return true;
            case LIRCallMember0 callMember0:
                defined = callMember0.Result;
                return true;
            case LIRCallMember1 callMember1:
                defined = callMember1.Result;
                return true;
            case LIRCallMember2 callMember2:
                defined = callMember2.Result;
                return true;
            case LIRCallMember3 callMember3:
                defined = callMember3.Result;
                return true;

            case LIRCallIntrinsicStaticWithArgsArray callStaticWithArgsArray:
                defined = callStaticWithArgsArray.Result;
                return true;

            case LIRCallTypedMember callTyped:
                defined = callTyped.Result;
                return true;

            case LIRCallTypedMemberWithFallback callTypedFallback:
                defined = callTypedFallback.Result;
                return true;

            case LIRCallUserClassInstanceMethod callUserClass:
                defined = callUserClass.Result;
                return true;

            case LIRCallUserClassBaseInstanceMethod callBaseMethod:
                defined = callBaseMethod.Result;
                return true;

            case LIRCallUserClassBaseConstructor:
                defined = default;
                return false;

            case LIRCallIntrinsicBaseConstructor:
                defined = default;
                return false;

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
            case LIRLoadNewTarget loadNewTarget:
                defined = loadNewTarget.Result;
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
            case LIRNewInferredJsObject newInferredJsObject:
                defined = newInferredJsObject.Result;
                return true;
            case LIRGetInferredMember getInferredMember:
                defined = getInferredMember.Result;
                return true;
            case LIRGetLength getLength:
                defined = getLength.Result;
                return true;
            case LIRGetStringLength getStringLength:
                defined = getStringLength.Result;
                return true;
            case LIRGetJsArrayLength getJsArrayLength:
                defined = getJsArrayLength.Result;
                return true;
            case LIRGetInt32ArrayLength getInt32ArrayLength:
                defined = getInt32ArrayLength.Result;
                return true;
            case LIRGetItem getItem:
                defined = getItem.Result;
                return true;
            case LIRGetItemAsNumber getItemAsNumber:
                defined = getItemAsNumber.Result;
                return true;
            case LIRGetItemAsNumberString getItemAsNumberString:
                defined = getItemAsNumberString.Result;
                return true;

            case LIRSetItem setItem:
                defined = setItem.Result;
                return true;

            case LIRSetJsArrayLength setJsArrayLength:
                defined = setJsArrayLength.Result;
                return true;

            case LIRSetJsArrayElement setJsArray:
                defined = setJsArray.Result;
                return true;

            case LIRSetInt32ArrayElement setInt32Array:
                defined = setInt32Array.Result;
                return true;
            case LIRGetJsArrayElement getJsArray:
                defined = getJsArray.Result;
                return true;
            case LIRGetInt32ArrayElement getInt32Array:
                defined = getInt32Array.Result;
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
