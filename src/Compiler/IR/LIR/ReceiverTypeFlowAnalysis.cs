using Jroc.IL;
using Jroc.SymbolTables;

namespace Jroc.IR;

internal sealed record ReceiverTypeFlowValue(
    bool IncludesUnknown,
    bool IncludesNonCandidate,
    IReadOnlySet<Type> CandidateClrTypes)
{
    public bool Contains(Type type) => CandidateClrTypes.Contains(type);
}

internal sealed class ReceiverTypeFlowFacts
{
    private readonly Dictionary<(int InstructionIndex, int TempIndex), ReceiverTypeFlowValue>
        _beforeTemps = [];
    private readonly Dictionary<(int InstructionIndex, int TempIndex), ReceiverTypeFlowValue>
        _afterTemps = [];
    private readonly Dictionary<(int InstructionIndex, BindingInfo Binding), ReceiverTypeFlowValue>
        _beforeBindings = [];
    private readonly Dictionary<(int InstructionIndex, BindingInfo Binding), ReceiverTypeFlowValue>
        _afterBindings = [];

    public ReceiverTypeFlowValue GetTempBefore(int instructionIndex, TempVariable temp)
        => _beforeTemps.TryGetValue((instructionIndex, temp.Index), out var value)
            ? value
            : FlowValue.Unknown.ToPublic();

    public ReceiverTypeFlowValue GetTempAfter(int instructionIndex, TempVariable temp)
        => _afterTemps.TryGetValue((instructionIndex, temp.Index), out var value)
            ? value
            : FlowValue.Unknown.ToPublic();

    public ReceiverTypeFlowValue GetBindingBefore(int instructionIndex, BindingInfo binding)
        => _beforeBindings.TryGetValue((instructionIndex, binding), out var value)
            ? value
            : FlowValue.Unknown.ToPublic();

    public ReceiverTypeFlowValue GetBindingAfter(int instructionIndex, BindingInfo binding)
        => _afterBindings.TryGetValue((instructionIndex, binding), out var value)
            ? value
            : FlowValue.Unknown.ToPublic();

    internal void RecordTempBefore(int instructionIndex, TempVariable temp, FlowValue value)
    {
        if (value.HasCandidates)
        {
            _beforeTemps[(instructionIndex, temp.Index)] = value.ToPublic();
        }
    }

    internal void RecordTempAfter(int instructionIndex, TempVariable temp, FlowValue value)
    {
        if (value.HasCandidates)
        {
            _afterTemps[(instructionIndex, temp.Index)] = value.ToPublic();
        }
    }

    internal void RecordBindingBefore(int instructionIndex, BindingInfo binding, FlowValue value)
    {
        if (value.HasCandidates)
        {
            _beforeBindings[(instructionIndex, binding)] = value.ToPublic();
        }
    }

    internal void RecordBindingAfter(int instructionIndex, BindingInfo binding, FlowValue value)
    {
        if (value.HasCandidates)
        {
            _afterBindings[(instructionIndex, binding)] = value.ToPublic();
        }
    }
}

internal static class ReceiverTypeFlowAnalysis
{
    public static bool RequiresAnalysis(MethodBodyIR methodBody)
    {
        foreach (var instruction in methodBody.Instructions)
        {
            if (TryGetReceiver(instruction, out var receiver)
                && receiver.Index >= 0
                && receiver.Index < methodBody.TempStorages.Count)
            {
                var storage = methodBody.TempStorages[receiver.Index];
                if (storage.Kind == ValueStorageKind.Unknown
                    || storage.ClrType == null
                    || storage.ClrType == typeof(object))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static ReceiverTypeFlowFacts Analyze(MethodBodyIR methodBody)
    {
        var facts = new ReceiverTypeFlowFacts();
        if (methodBody.Instructions.Count == 0)
        {
            return facts;
        }

        var slice = BuildAnalysisSlice(methodBody);
        var blocks = BuildBlocks(methodBody);
        var hasFinallyRegion = methodBody.ExceptionRegions.Any(
            static region => region.Kind == ExceptionRegionKind.Finally);
        var blockInputs = new FlowState?[blocks.Count];
        var worklist = new Queue<int>();
        var queued = new bool[blocks.Count];

        EnqueueWithInput(0, CreateEntryState());
        foreach (var region in methodBody.ExceptionRegions)
        {
            if (TryGetLabelBlock(blocks, region.HandlerStartLabelId, out var handlerBlock))
            {
                EnqueueWithInput(handlerBlock, new FlowState());
            }
        }

        while (worklist.Count > 0)
        {
            var blockIndex = worklist.Dequeue();
            queued[blockIndex] = false;
            var state = blockInputs[blockIndex]!.Clone();
            var block = blocks[blockIndex];

            for (var instructionIndex = block.Start; instructionIndex < block.End; instructionIndex++)
            {
                Transfer(
                    methodBody,
                    methodBody.Instructions[instructionIndex],
                    state,
                    slice,
                    hasFinallyRegion);
            }

            foreach (var successor in block.Successors)
            {
                EnqueueWithInput(successor, state);
            }
        }

        for (var blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
        {
            if (blockInputs[blockIndex] == null)
            {
                continue;
            }

            var state = blockInputs[blockIndex]!.Clone();
            var block = blocks[blockIndex];
            for (var instructionIndex = block.Start; instructionIndex < block.End; instructionIndex++)
            {
                var instruction = methodBody.Instructions[instructionIndex];
                var visitor = new TempFactRecorder(
                    facts,
                    state,
                    methodBody,
                    slice,
                    instructionIndex);
                LIRInstructionInfo.VisitUsedTemps(instruction, ref visitor);
                RecordBindingBefore(instruction, state, facts, instructionIndex);

                Transfer(
                    methodBody,
                    instruction,
                    state,
                    slice,
                    hasFinallyRegion);

                if (LIRInstructionInfo.TryGetDefinedTemp(instruction, out var defined)
                    && defined.Index >= 0)
                {
                    facts.RecordTempAfter(
                        instructionIndex,
                        defined,
                        state.GetTemp(methodBody, defined));
                }
                RecordBindingAfter(instruction, state, facts, instructionIndex);
            }
        }

        return facts;

        FlowState CreateEntryState()
        {
            var state = new FlowState();
            foreach (var (parameter, summary) in
                     methodBody.ReceiverParameterTypeSummaries)
            {
                if (slice.Parameters.Contains(parameter))
                {
                    state.SetParameter(
                        parameter,
                        FlowValue.FromSummary(summary));
                }
            }

            foreach (var (binding, summary) in
                     methodBody.ReceiverCapturedEntryTypeSummaries)
            {
                if (slice.Bindings.Contains(binding))
                {
                    state.SetBinding(
                        binding,
                        FlowValue.FromSummary(summary));
                }
            }

            return state;
        }

        void EnqueueWithInput(int blockIndex, FlowState incoming)
        {
            if (blockInputs[blockIndex] == null)
            {
                blockInputs[blockIndex] = incoming.Clone();
            }
            else if (!blockInputs[blockIndex]!.MergeFrom(incoming))
            {
                return;
            }

            if (!queued[blockIndex])
            {
                queued[blockIndex] = true;
                worklist.Enqueue(blockIndex);
            }
        }
    }

    private static void Transfer(
        MethodBodyIR methodBody,
        LIRInstruction instruction,
        FlowState state,
        AnalysisSlice slice,
        bool hasFinallyRegion = false)
    {
        var effects = LIRInstructionInfo.GetEffectsForScheduling(instruction);
        if ((effects & LIRInstructionEffects.UnsupportedBarrier) != 0)
        {
            state.InvalidateMutableLocations();
        }
        else if ((effects & (LIRInstructionEffects.Calls
                     | LIRInstructionEffects.Suspension
                     | LIRInstructionEffects.ScopeReplacement)) != 0
                 || MayInvokeDynamicAccessor(instruction))
        {
            state.InvalidateCapturedBindings();
        }

        if (hasFinallyRegion && instruction is LIRLeave)
        {
            state.InvalidateMutableLocations();
        }

        switch (instruction)
        {
            case LIRStoreScopeField store:
                if (slice.Bindings.Contains(store.Binding))
                {
                    state.SetBinding(
                        store.Binding,
                        state.GetTemp(methodBody, store.Value));
                }
                break;
            case LIRStoreLeafScopeField store:
                if (slice.Bindings.Contains(store.Binding))
                {
                    state.SetBinding(
                        store.Binding,
                        state.GetTemp(methodBody, store.Value));
                }
                break;
            case LIRStoreParentScopeField store:
                if (slice.Bindings.Contains(store.Binding))
                {
                    state.SetBinding(
                        store.Binding,
                        state.GetTemp(methodBody, store.Value));
                }
                break;
            case LIRStoreParameter store:
                if (slice.Parameters.Contains(store.ParameterIndex))
                {
                    state.SetParameter(
                        store.ParameterIndex,
                        state.GetTemp(methodBody, store.Value));
                }
                break;
        }

        if (!LIRInstructionInfo.TryGetDefinedTemp(instruction, out var defined)
            || defined.Index < 0
            || !slice.IsTempRelevant(methodBody, defined))
        {
            return;
        }

        var value = instruction switch
        {
            LIRCopyTemp copy => state.GetTemp(methodBody, copy.Source),
            LIRLoadScopeField load => ResolveLoadValue(
                state.GetBinding(load.Binding),
                ClassifyStorage(methodBody, defined)),
            LIRLoadLeafScopeField load => ResolveLoadValue(
                state.GetBinding(load.Binding),
                ClassifyStorage(methodBody, defined)),
            LIRLoadParentScopeField load => ResolveLoadValue(
                state.GetBinding(load.Binding),
                ClassifyStorage(methodBody, defined)),
            LIRLoadParameter load => ResolveLoadValue(
                state.GetParameter(load.ParameterIndex),
                ClassifyStorage(methodBody, defined)),
            LIRConvertToObject convert => state.GetTemp(methodBody, convert.Source),
            LIRCallIntrinsicStatic
            {
                IntrinsicName: nameof(JavaScriptRuntime.ObjectRuntime),
                MethodName: nameof(JavaScriptRuntime.ObjectRuntime.RequireObjectCoercible),
                Arguments.Count: 1
            } requireObjectCoercible
                => state.GetTemp(methodBody, requireObjectCoercible.Arguments[0]),
            LIRCallIntrinsicStatic
            {
                IntrinsicName: nameof(JavaScriptRuntime.Array),
                MethodName: "Construct"
            } => FlowValue.ForCandidate(typeof(JavaScriptRuntime.Array)),
            LIRConstString => FlowValue.ForCandidate(typeof(string)),
            LIRConvertToString => FlowValue.ForCandidate(typeof(string)),
            LIRConcatStrings => FlowValue.ForCandidate(typeof(string)),
            LIRConstNumber or LIRConstBoolean or LIRConstNull or LIRConstUndefined
                => FlowValue.NonCandidate,
            _ => methodBody.ReceiverTempTypeSummaries.TryGetValue(
                defined.Index,
                out var summary)
                    ? FlowValue.FromSummary(summary)
                    : ClassifyStorage(methodBody, defined)
        };

        state.SetTemp(methodBody, defined, value);
    }

    private static bool MayInvokeDynamicAccessor(LIRInstruction instruction)
        => instruction is LIRGetItem
            or LIRGetItemAsNumber
            or LIRGetItemAsNumberString
            or LIRGetJsArrayElement
            or LIRGetLength
            or LIRSetItem;

    private static FlowValue ResolveLoadValue(
        FlowValue flowValue,
        FlowValue storageValue)
        => flowValue.Equals(FlowValue.Unknown)
            ? storageValue
            : flowValue;

    private static FlowValue ClassifyStorage(
        MethodBodyIR methodBody,
        TempVariable temp)
    {
        if (temp.Index < 0 || temp.Index >= methodBody.TempStorages.Count)
        {
            return FlowValue.Unknown;
        }

        var storage = methodBody.TempStorages[temp.Index];
        if (IsReceiverCandidateType(storage.ClrType))
        {
            return FlowValue.ForCandidate(storage.ClrType!);
        }

        if (storage.Kind == ValueStorageKind.Unknown
            || storage.ClrType == null
            || storage.ClrType == typeof(object)
            || !storage.TypeHandle.IsNil)
        {
            return FlowValue.Unknown;
        }

        return FlowValue.NonCandidate;
    }

    private static bool IsReceiverCandidateType(Type? type)
        => type == typeof(string)
           || type is
           {
               IsAbstract: false,
               Namespace: { } namespaceName
           }
           && namespaceName.StartsWith("JavaScriptRuntime", StringComparison.Ordinal);

    private static AnalysisSlice BuildAnalysisSlice(MethodBodyIR methodBody)
    {
        var slice = new AnalysisSlice();
        var definitions = new Dictionary<int, List<LIRInstruction>>();
        var bindingStores =
            new Dictionary<BindingInfo, List<TempVariable>>();
        var parameterStores = new Dictionary<int, List<TempVariable>>();
        var locationWorklist = new Queue<int>();
        var bindingWorklist = new Queue<BindingInfo>();
        var parameterWorklist = new Queue<int>();

        foreach (var instruction in methodBody.Instructions)
        {
            if (TryGetReceiver(instruction, out var receiver))
            {
                AddTemp(receiver);
            }

            if (LIRInstructionInfo.TryGetDefinedTemp(
                    instruction,
                    out var defined)
                && defined.Index >= 0)
            {
                AddToList(
                    definitions,
                    GetLocationKey(methodBody, defined),
                    instruction);
            }

            switch (instruction)
            {
                case LIRStoreScopeField store:
                    AddToList(bindingStores, store.Binding, store.Value);
                    break;
                case LIRStoreLeafScopeField store:
                    AddToList(bindingStores, store.Binding, store.Value);
                    break;
                case LIRStoreParentScopeField store:
                    AddToList(bindingStores, store.Binding, store.Value);
                    break;
                case LIRStoreParameter store:
                    AddToList(
                        parameterStores,
                        store.ParameterIndex,
                        store.Value);
                    break;
            }
        }

        while (locationWorklist.Count > 0
               || bindingWorklist.Count > 0
               || parameterWorklist.Count > 0)
        {
            while (locationWorklist.TryDequeue(out var location))
            {
                if (!definitions.TryGetValue(location, out var locationDefinitions))
                {
                    continue;
                }

                foreach (var definition in locationDefinitions)
                {
                    switch (definition)
                    {
                        case LIRCopyTemp copy:
                            AddTemp(copy.Source);
                            break;
                        case LIRConvertToObject convert:
                            AddTemp(convert.Source);
                            break;
                        case LIRCallIntrinsicStatic
                            {
                                IntrinsicName:
                                    nameof(JavaScriptRuntime.ObjectRuntime),
                                MethodName:
                                    nameof(JavaScriptRuntime.ObjectRuntime
                                        .RequireObjectCoercible),
                                Arguments.Count: 1
                            } requireObjectCoercible:
                            AddTemp(requireObjectCoercible.Arguments[0]);
                            break;
                        case LIRLoadScopeField load:
                            AddBinding(load.Binding);
                            break;
                        case LIRLoadLeafScopeField load:
                            AddBinding(load.Binding);
                            break;
                        case LIRLoadParentScopeField load:
                            AddBinding(load.Binding);
                            break;
                        case LIRLoadParameter load:
                            AddParameter(load.ParameterIndex);
                            break;
                    }
                }
            }

            while (bindingWorklist.TryDequeue(out var binding))
            {
                if (bindingStores.TryGetValue(binding, out var storedValues))
                {
                    foreach (var storedValue in storedValues)
                    {
                        AddTemp(storedValue);
                    }
                }
            }

            while (parameterWorklist.TryDequeue(out var parameter))
            {
                if (parameterStores.TryGetValue(parameter, out var storedValues))
                {
                    foreach (var storedValue in storedValues)
                    {
                        AddTemp(storedValue);
                    }
                }
            }
        }

        return slice;

        void AddTemp(TempVariable temp)
        {
            if (slice.AddTemp(methodBody, temp))
            {
                locationWorklist.Enqueue(GetLocationKey(methodBody, temp));
            }
        }

        void AddBinding(BindingInfo binding)
        {
            if (slice.Bindings.Add(binding))
            {
                bindingWorklist.Enqueue(binding);
            }
        }

        void AddParameter(int parameter)
        {
            if (slice.Parameters.Add(parameter))
            {
                parameterWorklist.Enqueue(parameter);
            }
        }
    }

    private static int GetLocationKey(
        MethodBodyIR methodBody,
        TempVariable temp)
    {
        var slot = AnalysisSlice.GetVariableSlot(methodBody, temp);
        return slot >= 0 ? -slot - 1 : temp.Index;
    }

    private static void AddToList<TKey, TValue>(
        Dictionary<TKey, List<TValue>> lookup,
        TKey key,
        TValue value)
        where TKey : notnull
    {
        if (!lookup.TryGetValue(key, out var values))
        {
            values = [];
            lookup.Add(key, values);
        }

        values.Add(value);
    }

    private static bool TryGetReceiver(
        LIRInstruction instruction,
        out TempVariable receiver)
    {
        receiver = instruction switch
        {
            LIRCallMember call => call.Receiver,
            LIRCallMember0 call => call.Receiver,
            LIRCallMember1 call => call.Receiver,
            LIRCallMember2 call => call.Receiver,
            LIRCallMember3 call => call.Receiver,
            LIRCallMember4 call => call.Receiver,
            LIRCallMember5 call => call.Receiver,
            LIRCallGuardedStringIntrinsic call => call.Receiver,
            LIRGetLength getLength => getLength.Object,
            LIRGetItem getItem => getItem.Object,
            _ => default
        };
        return instruction is LIRCallMember
            or LIRCallMember0
            or LIRCallMember1
            or LIRCallMember2
            or LIRCallMember3
            or LIRCallMember4
            or LIRCallMember5
            or LIRCallGuardedStringIntrinsic
            or LIRGetLength
            or LIRGetItem;
    }

    private static void RecordBindingBefore(
        LIRInstruction instruction,
        FlowState state,
        ReceiverTypeFlowFacts facts,
        int instructionIndex)
    {
        if (TryGetBinding(instruction, out var binding))
        {
            facts.RecordBindingBefore(
                instructionIndex,
                binding,
                state.GetBinding(binding));
        }
    }

    private static void RecordBindingAfter(
        LIRInstruction instruction,
        FlowState state,
        ReceiverTypeFlowFacts facts,
        int instructionIndex)
    {
        if (TryGetBinding(instruction, out var binding))
        {
            facts.RecordBindingAfter(
                instructionIndex,
                binding,
                state.GetBinding(binding));
        }
    }

    private static bool TryGetBinding(
        LIRInstruction instruction,
        out BindingInfo binding)
    {
        binding = instruction switch
        {
            LIRLoadScopeField load => load.Binding,
            LIRStoreScopeField store => store.Binding,
            LIRLoadLeafScopeField load => load.Binding,
            LIRStoreLeafScopeField store => store.Binding,
            LIRLoadParentScopeField load => load.Binding,
            LIRStoreParentScopeField store => store.Binding,
            _ => null!
        };
        return binding != null;
    }

    private static List<BasicBlock> BuildBlocks(MethodBodyIR methodBody)
    {
        var instructions = methodBody.Instructions;
        var leaders = new SortedSet<int> { 0 };
        var labelInstructionIndices = new Dictionary<int, int>();

        for (var index = 0; index < instructions.Count; index++)
        {
            if (instructions[index] is LIRLabel label)
            {
                leaders.Add(index);
                labelInstructionIndices[label.LabelId] = index;
            }

            if (IsBlockEndingInstruction(instructions[index])
                && index + 1 < instructions.Count)
            {
                leaders.Add(index + 1);
            }
        }

        var starts = leaders.ToArray();
        var blocks = new List<BasicBlock>(starts.Length);
        var blockByInstruction = new int[instructions.Count];
        for (var blockIndex = 0; blockIndex < starts.Length; blockIndex++)
        {
            var start = starts[blockIndex];
            var end = blockIndex + 1 < starts.Length
                ? starts[blockIndex + 1]
                : instructions.Count;
            var block = new BasicBlock(start, end);
            if (instructions[start] is LIRLabel startLabel)
            {
                block.StartLabelId = startLabel.LabelId;
            }
            blocks.Add(block);
            for (var index = start; index < end; index++)
            {
                blockByInstruction[index] = blockIndex;
            }
        }

        var blockByLabel = labelInstructionIndices.ToDictionary(
            pair => pair.Key,
            pair => blockByInstruction[pair.Value]);
        var resumeBlockByLabel = new Dictionary<int, int>();
        for (var blockIndex = 0; blockIndex + 1 < blocks.Count; blockIndex++)
        {
            switch (instructions[blocks[blockIndex].End - 1])
            {
                case LIRAwait awaitInstruction:
                    resumeBlockByLabel[awaitInstruction.ResumeLabelId] =
                        blockIndex + 1;
                    break;
                case LIRYield yieldInstruction:
                    resumeBlockByLabel[yieldInstruction.ResumeLabelId] =
                        blockIndex + 1;
                    break;
            }
        }

        for (var blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
        {
            var block = blocks[blockIndex];
            var last = instructions[block.End - 1];

            void AddLabelSuccessor(int labelId)
            {
                if (resumeBlockByLabel.TryGetValue(labelId, out var resumeBlock))
                {
                    block.Successors.Add(resumeBlock);
                }
                else if (blockByLabel.TryGetValue(labelId, out var successor))
                {
                    block.Successors.Add(successor);
                }
            }

            void AddFallthrough()
            {
                if (blockIndex + 1 < blocks.Count)
                {
                    block.Successors.Add(blockIndex + 1);
                }
            }

            switch (last)
            {
                case LIRBranch branch:
                    AddLabelSuccessor(branch.TargetLabel);
                    break;
                case LIRLeave leave:
                    AddLabelSuccessor(leave.TargetLabel);
                    break;
                case LIRBranchIfTrue branch:
                    AddLabelSuccessor(branch.TargetLabel);
                    AddFallthrough();
                    break;
                case LIRBranchIfFalse branch:
                    AddLabelSuccessor(branch.TargetLabel);
                    AddFallthrough();
                    break;
                case LIRAsyncStateSwitch stateSwitch:
                    foreach (var label in stateSwitch.StateToLabel.Values)
                    {
                        AddLabelSuccessor(label);
                    }
                    AddLabelSuccessor(stateSwitch.DefaultLabel);
                    break;
                case LIRGeneratorStateSwitch stateSwitch:
                    foreach (var label in stateSwitch.StateToLabel.Values)
                    {
                        AddLabelSuccessor(label);
                    }
                    AddLabelSuccessor(stateSwitch.DefaultLabel);
                    break;
                case LIRAwait awaitInstruction:
                    AddFallthrough();
                    if (awaitInstruction.RejectResumeStateId is int rejectState
                        && methodBody.AsyncInfo?.ResumeLabels.TryGetValue(
                            rejectState,
                            out var rejectLabel) == true)
                    {
                        AddLabelSuccessor(rejectLabel);
                    }
                    break;
                case LIRYield yieldInstruction:
                    AddFallthrough();
                    break;
                case LIRReturn:
                case LIRReturnUndefinedImmediate:
                case LIRTailCallFunctionReturn:
                case LIRAsyncReturnPromise:
                case LIRThrow:
                case LIRThrowNewTypeError:
                case LIREndFinally:
                    break;
                default:
                    AddFallthrough();
                    break;
            }
        }

        return blocks;
    }

    private static bool TryGetLabelBlock(
        IReadOnlyList<BasicBlock> blocks,
        int labelId,
        out int blockIndex)
    {
        for (var index = 0; index < blocks.Count; index++)
        {
            if (blocks[index].StartLabelId == labelId)
            {
                blockIndex = index;
                return true;
            }
        }

        blockIndex = -1;
        return false;
    }

    private static bool IsBlockEndingInstruction(LIRInstruction instruction)
        => instruction is LIRBranch
            or LIRLeave
            or LIRBranchIfTrue
            or LIRBranchIfFalse
            or LIRAsyncStateSwitch
            or LIRGeneratorStateSwitch
            or LIRAwait
            or LIRYield
            or LIRReturn
            or LIRReturnUndefinedImmediate
            or LIRTailCallFunctionReturn
            or LIRAsyncReturnPromise
            or LIRThrow
            or LIRThrowNewTypeError
            or LIREndFinally;

    private sealed class BasicBlock(int start, int end)
    {
        public int Start { get; } = start;
        public int End { get; } = end;
        public HashSet<int> Successors { get; } = [];
        public int? StartLabelId { get; set; }
    }

    private sealed class AnalysisSlice
    {
        public HashSet<int> Temps { get; } = [];
        public HashSet<int> Slots { get; } = [];
        public HashSet<int> Parameters { get; } = [];
        public HashSet<BindingInfo> Bindings { get; } = [];

        public bool AddTemp(MethodBodyIR methodBody, TempVariable temp)
        {
            var slot = GetVariableSlot(methodBody, temp);
            return slot >= 0
                ? Slots.Add(slot)
                : Temps.Add(temp.Index);
        }

        public bool IsTempRelevant(
            MethodBodyIR methodBody,
            TempVariable temp)
        {
            var slot = GetVariableSlot(methodBody, temp);
            return slot >= 0
                ? Slots.Contains(slot)
                : Temps.Contains(temp.Index);
        }

        public static int GetVariableSlot(
            MethodBodyIR methodBody,
            TempVariable temp)
            => temp.Index >= 0
                && temp.Index < methodBody.TempVariableSlots.Count
                    ? methodBody.TempVariableSlots[temp.Index]
                    : -1;
    }

    private sealed class FlowState
    {
        private readonly Dictionary<int, FlowValue> _temps = [];
        private readonly Dictionary<int, FlowValue> _slots = [];
        private readonly Dictionary<int, FlowValue> _parameters = [];
        private readonly Dictionary<BindingInfo, FlowValue> _bindings = [];

        public FlowState Clone()
        {
            var clone = new FlowState();
            CopyTo(_temps, clone._temps);
            CopyTo(_slots, clone._slots);
            CopyTo(_parameters, clone._parameters);
            CopyTo(_bindings, clone._bindings);
            return clone;
        }

        public FlowValue GetTemp(MethodBodyIR methodBody, TempVariable temp)
        {
            var slot = GetVariableSlot(methodBody, temp);
            return slot >= 0
                ? Get(_slots, slot)
                : Get(_temps, temp.Index);
        }

        public void SetTemp(MethodBodyIR methodBody, TempVariable temp, FlowValue value)
        {
            var slot = GetVariableSlot(methodBody, temp);
            if (slot >= 0)
            {
                Set(_slots, slot, value);
            }
            else
            {
                Set(_temps, temp.Index, value);
            }
        }

        public FlowValue GetParameter(int parameterIndex)
            => Get(_parameters, parameterIndex);

        public void SetParameter(int parameterIndex, FlowValue value)
            => Set(_parameters, parameterIndex, value);

        public FlowValue GetBinding(BindingInfo binding)
            => Get(_bindings, binding);

        public void SetBinding(BindingInfo binding, FlowValue value)
            => Set(_bindings, binding, value);

        public void InvalidateCapturedBindings()
            => _bindings.Clear();

        public void InvalidateMutableLocations()
        {
            _slots.Clear();
            _parameters.Clear();
            _bindings.Clear();
        }

        public bool MergeFrom(FlowState incoming)
        {
            var changed = false;
            changed |= MergeDictionary(_temps, incoming._temps);
            changed |= MergeDictionary(_slots, incoming._slots);
            changed |= MergeDictionary(_parameters, incoming._parameters);
            changed |= MergeDictionary(_bindings, incoming._bindings);
            return changed;
        }

        private static int GetVariableSlot(
            MethodBodyIR methodBody,
            TempVariable temp)
            => temp.Index >= 0
                && temp.Index < methodBody.TempVariableSlots.Count
                    ? methodBody.TempVariableSlots[temp.Index]
                    : -1;

        private static FlowValue Get<TKey>(
            IReadOnlyDictionary<TKey, FlowValue> dictionary,
            TKey key)
            where TKey : notnull
            => dictionary.TryGetValue(key, out var value)
                ? value
                : FlowValue.Unknown;

        private static void Set<TKey>(
            Dictionary<TKey, FlowValue> dictionary,
            TKey key,
            FlowValue value)
            where TKey : notnull
        {
            if (value.Equals(FlowValue.Unknown))
            {
                dictionary.Remove(key);
            }
            else
            {
                dictionary[key] = value;
            }
        }

        private static bool MergeDictionary<TKey>(
            Dictionary<TKey, FlowValue> target,
            IReadOnlyDictionary<TKey, FlowValue> incoming)
            where TKey : notnull
        {
            var changed = false;
            foreach (var key in target.Keys.Union(incoming.Keys).ToArray())
            {
                var merged = Get(target, key).Union(Get(incoming, key));
                if (merged.Equals(Get(target, key)))
                {
                    continue;
                }

                Set(target, key, merged);
                changed = true;
            }
            return changed;
        }

        private static void CopyTo<TKey>(
            IReadOnlyDictionary<TKey, FlowValue> source,
            Dictionary<TKey, FlowValue> destination)
            where TKey : notnull
        {
            foreach (var pair in source)
            {
                destination.Add(pair.Key, pair.Value);
            }
        }
    }

    private readonly record struct TempFactRecorder(
        ReceiverTypeFlowFacts Facts,
        FlowState State,
        MethodBodyIR MethodBody,
        AnalysisSlice Slice,
        int InstructionIndex) : ITempUseVisitor
    {
        public void Visit(TempVariable temp)
        {
            if (Slice.IsTempRelevant(MethodBody, temp))
            {
                Facts.RecordTempBefore(
                    InstructionIndex,
                    temp,
                    State.GetTemp(MethodBody, temp));
            }
        }
    }
}

internal sealed class FlowValue : IEquatable<FlowValue>
{
    public static FlowValue Unknown { get; } = new(
        includesUnknown: true,
        includesNonCandidate: true,
        []);

    public static FlowValue NonCandidate { get; } = new(
        includesUnknown: false,
        includesNonCandidate: true,
        []);

    private readonly HashSet<Type> _candidateClrTypes;
    private ReceiverTypeFlowValue? _publicValue;

    private FlowValue(
        bool includesUnknown,
        bool includesNonCandidate,
        IEnumerable<Type> candidateClrTypes)
    {
        IncludesUnknown = includesUnknown;
        IncludesNonCandidate = includesNonCandidate;
        _candidateClrTypes = [.. candidateClrTypes];
    }

    public bool IncludesUnknown { get; }
    public bool IncludesNonCandidate { get; }
    public bool HasCandidates => _candidateClrTypes.Count > 0;

    public static FlowValue ForCandidate(Type type)
        => new(
            includesUnknown: false,
            includesNonCandidate: false,
            [type]);

    public static FlowValue FromSummary(ReceiverTypeSummary summary)
        => new(
            summary.IncludesUnknown,
            summary.IncludesNonCandidate,
            summary.CandidateClrTypes);

    public FlowValue Union(FlowValue other)
        => new(
            IncludesUnknown || other.IncludesUnknown,
            IncludesNonCandidate || other.IncludesNonCandidate,
            _candidateClrTypes.Union(other._candidateClrTypes));

    public ReceiverTypeFlowValue ToPublic()
        => _publicValue ??= new(
            IncludesUnknown,
            IncludesNonCandidate,
            new HashSet<Type>(_candidateClrTypes));

    public bool Equals(FlowValue? other)
        => other != null
            && IncludesUnknown == other.IncludesUnknown
            && IncludesNonCandidate == other.IncludesNonCandidate
            && _candidateClrTypes.SetEquals(other._candidateClrTypes);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(IncludesUnknown);
        hash.Add(IncludesNonCandidate);
        foreach (var type in _candidateClrTypes.OrderBy(static type => type.FullName, StringComparer.Ordinal))
        {
            hash.Add(type);
        }
        return hash.ToHashCode();
    }
}
