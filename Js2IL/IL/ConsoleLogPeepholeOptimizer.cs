using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.IR;
using Js2IL.Services;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.IL;

/// <summary>
/// Peephole optimizer for console.log calls.
/// Emits console.log sequences stack-only when possible, avoiding temp locals.
/// </summary>
internal sealed class ConsoleLogPeepholeOptimizer
{
    private readonly MetadataBuilder _metadataBuilder;
    private readonly BaseClassLibraryReferences _bclReferences;
    private readonly MemberReferenceRegistry _memberRefRegistry;
    private readonly TypeReferenceRegistry _typeReferenceRegistry;

    public ConsoleLogPeepholeOptimizer(
        MetadataBuilder metadataBuilder,
        BaseClassLibraryReferences bclReferences,
        MemberReferenceRegistry memberRefRegistry,
        TypeReferenceRegistry typeReferenceRegistry)
    {
        _metadataBuilder = metadataBuilder;
        _bclReferences = bclReferences;
        _memberRefRegistry = memberRefRegistry;
        _typeReferenceRegistry = typeReferenceRegistry;
    }

    /// <summary>
    /// Computes a mask of which temps should be materialized based on console.log peephole analysis.
    /// Returns null if no temps exist, otherwise returns an array where false means "don't materialize".
    /// </summary>
    public bool[]? ComputeStackOnlyMask(MethodBodyIR methodBody)
    {
        int tempCount = methodBody.Temps.Count;
        if (tempCount == 0)
        {
            return null;
        }

        var replaced = new bool[methodBody.Instructions.Count];

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            // Try the new LIRBuildArray pattern first (simpler, 3-instruction sequence)
            if (TryMatchConsoleLogBuildArraySequence(methodBody, i, out var buildArrayIndex, out var buildArrayElements))
            {
                int callIndex = buildArrayIndex + 1;

                // Build set of temps defined in this sequence
                var definedInSequence = new HashSet<TempVariable>();
                for (int j = i; j <= buildArrayIndex; j++)
                {
                    if (TempLocalAllocator.TryGetDefinedTemp(methodBody.Instructions[j], out var def))
                    {
                        definedInSequence.Add(def);
                    }
                }

                // Check if ALL arguments can be emitted stack-only
                bool allArgsStackOnly = true;
                foreach (var element in buildArrayElements)
                {
                    if (!CanEmitTempStackOnly(methodBody, element, definedInSequence))
                    {
                        allArgsStackOnly = false;
                        break;
                    }
                }

                if (allArgsStackOnly)
                {
                    for (int j = i; j <= callIndex; j++)
                    {
                        replaced[j] = true;
                    }
                    i = callIndex;
                    continue;
                }
            }

            if (TryMatchConsoleLogMultiArgSequence(methodBody, i, out var lastStoreIndex, out var storeInfos))
            {
                int callIndex = lastStoreIndex + 1;
                
                // Build set of temps defined in this sequence
                var definedInSequence = new HashSet<TempVariable>();
                for (int j = i; j <= lastStoreIndex; j++)
                {
                    if (TempLocalAllocator.TryGetDefinedTemp(methodBody.Instructions[j], out var def))
                    {
                        definedInSequence.Add(def);
                    }
                }

                // For single-arg case, check update expression pattern
                if (storeInfos.Count == 1)
                {
                    int idx = i + 2;
                    if (methodBody.Instructions[idx] is LIRBeginInitArrayElement)
                    {
                        idx++;
                    }
                    if (TryMatchUpdateExpressionForConsoleArg(methodBody, idx, storeInfos[0].StoreIndex, storeInfos[0].StoredValue, out _, out _, out _))
                    {
                        for (int j = i; j <= callIndex; j++)
                        {
                            replaced[j] = true;
                        }
                        i = callIndex;
                        continue;
                    }
                }

                // Check if ALL arguments can be emitted stack-only
                bool allArgsStackOnly = true;
                foreach (var (_, storedValue) in storeInfos)
                {
                    if (!CanEmitTempStackOnly(methodBody, storedValue, definedInSequence))
                    {
                        allArgsStackOnly = false;
                        break;
                    }
                }

                if (allArgsStackOnly)
                {
                    for (int j = i; j <= callIndex; j++)
                    {
                        replaced[j] = true;
                    }
                    i = callIndex;
                }
            }
        }

        // Determine which temps are used outside replaced regions.
        var usedOutside = new bool[tempCount];
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (replaced[i])
            {
                continue;
            }

            foreach (var used in TempLocalAllocator.EnumerateUsedTemps(methodBody.Instructions[i])
                .Where(u => u.Index >= 0 && u.Index < tempCount))
            {
                usedOutside[used.Index] = true;
            }
        }

        return usedOutside;
    }

    /// <summary>
    /// Tries to emit a console.log sequence using stack-only evaluation.
    /// Returns true if the sequence was handled, with 'consumed' set to the number of instructions consumed.
    /// </summary>
    public bool TryEmitPeephole(
        MethodBodyIR methodBody,
        int startIndex,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        Func<TempVariable, TempLocalAllocation, bool> isMaterialized,
        Action<TempVariable, InstructionEncoder, TempLocalAllocation> emitStoreTemp,
        bool hasScopesParameter,
        bool isInstanceMethod,
        out int consumed)
    {
        consumed = 0;

        // Try the new LIRBuildArray pattern first (simpler, 3-instruction sequence)
        if (TryEmitBuildArrayPeephole(
            methodBody, startIndex, ilEncoder, allocation, isMaterialized, emitStoreTemp, hasScopesParameter, isInstanceMethod, out consumed))
        {
            return true;
        }

        if (startIndex + 3 >= methodBody.Instructions.Count)
        {
            return false;
        }

        if (methodBody.Instructions[startIndex] is not LIRGetIntrinsicGlobal g || !string.Equals(g.Name, "console", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (methodBody.Instructions[startIndex + 1] is not LIRNewObjectArray a || a.ElementCount < 1)
        {
            return false;
        }

        int argCount = a.ElementCount;
        int idx = startIndex + 2;

        // Collect all StoreElementRef instructions for this array
        var storeInfos = new List<(int StoreIndex, TempVariable StoredValue)>();
        int searchIdx = idx;
        
        for (int argIdx = 0; argIdx < argCount; argIdx++)
        {
            // Skip optional BeginInitArrayElement
            if (searchIdx < methodBody.Instructions.Count && 
                methodBody.Instructions[searchIdx] is LIRBeginInitArrayElement begin &&
                begin.Array == a.Result && begin.Index == argIdx)
            {
                searchIdx++;
            }

            // Find the StoreElementRef for this argument index
            int storeIndex = -1;
            for (int j = searchIdx; j < methodBody.Instructions.Count; j++)
            {
                if (methodBody.Instructions[j] is LIRStoreElementRef s && s.Array == a.Result && s.Index == argIdx)
                {
                    storeIndex = j;
                    storeInfos.Add((j, s.Value));
                    searchIdx = j + 1;
                    break;
                }

                // Bail out if we hit another intrinsic/global/array init
                if (methodBody.Instructions[j] is LIRGetIntrinsicGlobal or LIRNewObjectArray)
                {
                    return false;
                }
            }

            if (storeIndex < 0)
            {
                return false;
            }
        }

        if (storeInfos.Count != argCount)
        {
            return false;
        }

        int lastStoreIndex = storeInfos[^1].StoreIndex;

        // Check that CallIntrinsic immediately follows the last store
        if (lastStoreIndex + 1 >= methodBody.Instructions.Count)
        {
            return false;
        }

        if (methodBody.Instructions[lastStoreIndex + 1] is not LIRCallIntrinsic call || 
            call.IntrinsicObject != g.Result || 
            call.ArgumentsArray != a.Result || 
            !string.Equals(call.Name, "log", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Build the set of temps defined in this sequence (for stack-only analysis)
        var definedInSequence = new HashSet<TempVariable>();
        for (int i = startIndex; i <= lastStoreIndex; i++)
        {
            if (TempLocalAllocator.TryGetDefinedTemp(methodBody.Instructions[i], out var def))
            {
                definedInSequence.Add(def);
            }
        }

        // For single-arg case, check for update expression pattern first
        if (argCount == 1)
        {
            var (storeIndex, storedValue) = storeInfos[0];
            if (TryMatchUpdateExpressionForConsoleArg(methodBody, idx, storeIndex, storedValue, out var updatedVarSlot, out var isDecrement, out var isPrefix))
            {
                EmitLoadIntrinsicGlobalVariable("console", ilEncoder);
                ilEncoder.LoadConstantI4(1);
                ilEncoder.OpCode(ILOpCode.Newarr);
                ilEncoder.Token(_bclReferences.ObjectType);
                ilEncoder.OpCode(ILOpCode.Dup);
                ilEncoder.LoadConstantI4(0);

                EmitStackOnlyUpdateExpressionValue(ilEncoder, updatedVarSlot, isDecrement, isPrefix);
                ilEncoder.OpCode(ILOpCode.Stelem_ref);

                EmitInvokeIntrinsicMethod(typeof(JavaScriptRuntime.Console), "log", ilEncoder);

                if (isMaterialized(call.Result, allocation))
                {
                    emitStoreTemp(call.Result, ilEncoder, allocation);
                }
                else
                {
                    ilEncoder.OpCode(ILOpCode.Pop);
                }

                consumed = (lastStoreIndex + 1) - startIndex + 1;
                return true;
            }
        }

        // Check if ALL arguments can be emitted stack-only
        bool allArgsStackOnly = true;
        foreach (var (_, storedValue) in storeInfos)
        {
            if (!CanEmitTempStackOnly(methodBody, storedValue, definedInSequence))
            {
                allArgsStackOnly = false;
                break;
            }
        }

        if (allArgsStackOnly)
        {
            EmitLoadIntrinsicGlobalVariable("console", ilEncoder);
            ilEncoder.LoadConstantI4(argCount);
            ilEncoder.OpCode(ILOpCode.Newarr);
            ilEncoder.Token(_bclReferences.ObjectType);

            for (int argIdx = 0; argIdx < argCount; argIdx++)
            {
                ilEncoder.OpCode(ILOpCode.Dup);
                ilEncoder.LoadConstantI4(argIdx);
                EmitTempStackOnly(methodBody, storeInfos[argIdx].StoredValue, ilEncoder, hasScopesParameter, isInstanceMethod);
                ilEncoder.OpCode(ILOpCode.Stelem_ref);
            }

            EmitInvokeIntrinsicMethod(typeof(JavaScriptRuntime.Console), "log", ilEncoder);

            if (isMaterialized(call.Result, allocation))
            {
                emitStoreTemp(call.Result, ilEncoder, allocation);
            }
            else
            {
                ilEncoder.OpCode(ILOpCode.Pop);
            }

            consumed = (lastStoreIndex + 1) - startIndex + 1;
            return true;
        }

        // If we can't handle it stack-only, don't consume the sequence.
        return false;
    }

    /// <summary>
    /// Tries to emit a console.log sequence using LIRBuildArray (3-instruction pattern) stack-only.
    /// </summary>
    private bool TryEmitBuildArrayPeephole(
        MethodBodyIR methodBody,
        int startIndex,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        Func<TempVariable, TempLocalAllocation, bool> isMaterialized,
        Action<TempVariable, InstructionEncoder, TempLocalAllocation> emitStoreTemp,
        bool hasScopesParameter,
        bool isInstanceMethod,
        out int consumed)
    {
        consumed = 0;

        if (!TryMatchConsoleLogBuildArraySequence(methodBody, startIndex, out var buildArrayIndex, out var elements))
        {
            return false;
        }

        var buildArray = (LIRBuildArray)methodBody.Instructions[buildArrayIndex];
        var call = (LIRCallIntrinsic)methodBody.Instructions[buildArrayIndex + 1];
        int argCount = elements.Count;

        // Build set of temps defined in this sequence
        var definedInSequence = new HashSet<TempVariable>();
        for (int j = startIndex; j <= buildArrayIndex; j++)
        {
            if (TempLocalAllocator.TryGetDefinedTemp(methodBody.Instructions[j], out var def))
            {
                definedInSequence.Add(def);
            }
        }

        // Check if ALL arguments can be emitted stack-only
        bool allArgsStackOnly = true;
        foreach (var element in elements)
        {
            if (!CanEmitTempStackOnly(methodBody, element, definedInSequence))
            {
                allArgsStackOnly = false;
                break;
            }
        }

        if (allArgsStackOnly)
        {
            EmitLoadIntrinsicGlobalVariable("console", ilEncoder);
            ilEncoder.LoadConstantI4(argCount);
            ilEncoder.OpCode(ILOpCode.Newarr);
            ilEncoder.Token(_bclReferences.ObjectType);

            for (int argIdx = 0; argIdx < argCount; argIdx++)
            {
                ilEncoder.OpCode(ILOpCode.Dup);
                ilEncoder.LoadConstantI4(argIdx);
                EmitTempStackOnly(methodBody, elements[argIdx], ilEncoder, hasScopesParameter, isInstanceMethod);
                ilEncoder.OpCode(ILOpCode.Stelem_ref);
            }

            EmitInvokeIntrinsicMethod(typeof(JavaScriptRuntime.Console), "log", ilEncoder);

            if (isMaterialized(call.Result, allocation))
            {
                emitStoreTemp(call.Result, ilEncoder, allocation);
            }
            else
            {
                ilEncoder.OpCode(ILOpCode.Pop);
            }

            consumed = 3; // LIRGetIntrinsicGlobal, LIRBuildArray, LIRCallIntrinsic
            return true;
        }

        return false;
    }

    /// <summary>
    /// Matches a console.log sequence using LIRBuildArray (3-instruction pattern):
    /// LIRGetIntrinsicGlobal("console"), LIRBuildArray, LIRCallIntrinsic
    /// </summary>
    private static bool TryMatchConsoleLogBuildArraySequence(
        MethodBodyIR methodBody,
        int startIndex,
        out int buildArrayIndex,
        out IReadOnlyList<TempVariable> elements)
    {
        buildArrayIndex = -1;
        elements = Array.Empty<TempVariable>();

        if (startIndex + 2 >= methodBody.Instructions.Count)
        {
            return false;
        }

        if (methodBody.Instructions[startIndex] is not LIRGetIntrinsicGlobal g || !string.Equals(g.Name, "console", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (methodBody.Instructions[startIndex + 1] is not LIRBuildArray buildArray || buildArray.Elements.Count < 1)
        {
            return false;
        }

        if (methodBody.Instructions[startIndex + 2] is not LIRCallIntrinsic call ||
            call.IntrinsicObject != g.Result ||
            call.ArgumentsArray != buildArray.Result ||
            !string.Equals(call.Name, "log", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        buildArrayIndex = startIndex + 1;
        elements = buildArray.Elements;
        return true;
    }

    /// <summary>
    /// Matches a console.log sequence with N arguments (N >= 1).
    /// </summary>
    private static bool TryMatchConsoleLogMultiArgSequence(
        MethodBodyIR methodBody,
        int startIndex,
        out int lastStoreIndex,
        out List<(int StoreIndex, TempVariable StoredValue)> storeInfos)
    {
        lastStoreIndex = -1;
        storeInfos = new List<(int StoreIndex, TempVariable StoredValue)>();

        if (startIndex + 3 >= methodBody.Instructions.Count)
        {
            return false;
        }

        if (methodBody.Instructions[startIndex] is not LIRGetIntrinsicGlobal g || !string.Equals(g.Name, "console", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (methodBody.Instructions[startIndex + 1] is not LIRNewObjectArray a || a.ElementCount < 1)
        {
            return false;
        }

        int argCount = a.ElementCount;
        int searchIdx = startIndex + 2;

        for (int argIdx = 0; argIdx < argCount; argIdx++)
        {
            // Skip optional BeginInitArrayElement
            if (searchIdx < methodBody.Instructions.Count &&
                methodBody.Instructions[searchIdx] is LIRBeginInitArrayElement begin &&
                begin.Array == a.Result && begin.Index == argIdx)
            {
                searchIdx++;
            }

            // Find the StoreElementRef for this argument index
            int storeIndex = -1;
            for (int j = searchIdx; j < methodBody.Instructions.Count; j++)
            {
                if (methodBody.Instructions[j] is LIRStoreElementRef s && s.Array == a.Result && s.Index == argIdx)
                {
                    storeIndex = j;
                    storeInfos.Add((j, s.Value));
                    searchIdx = j + 1;
                    break;
                }

                // Bail out if we hit another intrinsic/global/array init
                if (methodBody.Instructions[j] is LIRGetIntrinsicGlobal or LIRNewObjectArray)
                {
                    return false;
                }
            }

            if (storeIndex < 0)
            {
                return false;
            }
        }

        if (storeInfos.Count != argCount)
        {
            return false;
        }

        lastStoreIndex = storeInfos[^1].StoreIndex;

        // Check that CallIntrinsic immediately follows the last store
        if (lastStoreIndex + 1 >= methodBody.Instructions.Count)
        {
            return false;
        }

        if (methodBody.Instructions[lastStoreIndex + 1] is not LIRCallIntrinsic call ||
            call.IntrinsicObject != g.Result ||
            call.ArgumentsArray != a.Result ||
            !string.Equals(call.Name, "log", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    private bool CanEmitTempStackOnly(MethodBodyIR methodBody, TempVariable temp, HashSet<TempVariable> definedInSequence)
    {
        // Check for variable-mapped temps FIRST.
        if (temp.Index >= 0 &&
            temp.Index < methodBody.TempVariableSlots.Count &&
            methodBody.TempVariableSlots[temp.Index] >= 0)
        {
            return true;
        }

        var def = TryFindDefInstruction(methodBody, temp);
        if (def == null)
        {
            return false;
        }

        return def switch
        {
            LIRConstNumber => true,
            LIRConstString => true,
            LIRConstBoolean => true,
            LIRConstUndefined => true,
            LIRConstNull => true,
            LIRLoadParameter => true,
            LIRConvertToObject conv => CanEmitTempStackOnly(methodBody, conv.Source, definedInSequence),
            LIRTypeof t => CanEmitTempStackOnly(methodBody, t.Value, definedInSequence),
            LIRNegateNumber neg => CanEmitTempStackOnly(methodBody, neg.Value, definedInSequence),
            LIRBitwiseNotNumber not => CanEmitTempStackOnly(methodBody, not.Value, definedInSequence),
            // Comparison operators
            LIRCompareNumberLessThan cmp => CanEmitTempStackOnly(methodBody, cmp.Left, definedInSequence) && CanEmitTempStackOnly(methodBody, cmp.Right, definedInSequence),
            LIRCompareNumberGreaterThan cmp => CanEmitTempStackOnly(methodBody, cmp.Left, definedInSequence) && CanEmitTempStackOnly(methodBody, cmp.Right, definedInSequence),
            LIRCompareNumberLessThanOrEqual cmp => CanEmitTempStackOnly(methodBody, cmp.Left, definedInSequence) && CanEmitTempStackOnly(methodBody, cmp.Right, definedInSequence),
            LIRCompareNumberGreaterThanOrEqual cmp => CanEmitTempStackOnly(methodBody, cmp.Left, definedInSequence) && CanEmitTempStackOnly(methodBody, cmp.Right, definedInSequence),
            LIRCompareNumberEqual cmp => CanEmitTempStackOnly(methodBody, cmp.Left, definedInSequence) && CanEmitTempStackOnly(methodBody, cmp.Right, definedInSequence),
            LIRCompareNumberNotEqual cmp => CanEmitTempStackOnly(methodBody, cmp.Left, definedInSequence) && CanEmitTempStackOnly(methodBody, cmp.Right, definedInSequence),
            LIRCompareBooleanEqual cmp => CanEmitTempStackOnly(methodBody, cmp.Left, definedInSequence) && CanEmitTempStackOnly(methodBody, cmp.Right, definedInSequence),
            LIRCompareBooleanNotEqual cmp => CanEmitTempStackOnly(methodBody, cmp.Left, definedInSequence) && CanEmitTempStackOnly(methodBody, cmp.Right, definedInSequence),
            // Dynamic operators
            LIRAddDynamic add => CanEmitTempStackOnly(methodBody, add.Left, definedInSequence) && CanEmitTempStackOnly(methodBody, add.Right, definedInSequence),
            LIRMulDynamic mul => CanEmitTempStackOnly(methodBody, mul.Left, definedInSequence) && CanEmitTempStackOnly(methodBody, mul.Right, definedInSequence),
            // Array ops that are part of this sequence are fine
            LIRGetIntrinsicGlobal g when definedInSequence.Contains(g.Result) => true,
            LIRNewObjectArray a when definedInSequence.Contains(a.Result) => true,
            _ => false,
        };
    }

    private bool TryMatchUpdateExpressionForConsoleArg(
        MethodBodyIR methodBody,
        int start,
        int storeIndex,
        TempVariable storedValue,
        out int updatedVarSlot,
        out bool isDecrement,
        out bool isPrefix)
    {
        updatedVarSlot = -1;
        isDecrement = false;
        isPrefix = false;

        if (TryFindDefInstruction(methodBody, storedValue) is not LIRConvertToObject valueConv)
        {
            return false;
        }

        TempVariable updateResult;
        TempVariable updateLeft;
        TempVariable updateRight;
        bool foundUpdate = false;
        updateResult = default!;
        updateLeft = default!;
        updateRight = default!;

        for (int i = start; i < storeIndex; i++)
        {
            if (methodBody.Instructions[i] is LIRAddNumber add)
            {
                updateResult = add.Result;
                updateLeft = add.Left;
                updateRight = add.Right;
                isDecrement = false;
                foundUpdate = true;
                break;
            }
            if (methodBody.Instructions[i] is LIRSubNumber sub)
            {
                updateResult = sub.Result;
                updateLeft = sub.Left;
                updateRight = sub.Right;
                isDecrement = true;
                foundUpdate = true;
                break;
            }
        }
        if (!foundUpdate)
        {
            return false;
        }

        if (updateResult.Index >= 0 && updateResult.Index < methodBody.TempVariableSlots.Count)
        {
            updatedVarSlot = methodBody.TempVariableSlots[updateResult.Index];
        }
        if (updatedVarSlot < 0)
        {
            return false;
        }

        var rightDef = TryFindDefInstruction(methodBody, updateRight!);
        if (rightDef is not LIRConstNumber cn || cn.Value is not 1.0)
        {
            return false;
        }

        bool prefix = valueConv.Source == updateResult;
        bool postfix = valueConv.Source == updateLeft;
        if (!prefix && !postfix)
        {
            return false;
        }

        isPrefix = prefix;
        return true;
    }

    private void EmitStackOnlyUpdateExpressionValue(InstructionEncoder ilEncoder, int updatedVarSlot, bool isDecrement, bool isPrefix)
    {
        if (!isPrefix)
        {
            // postfix: value is old
            ilEncoder.LoadLocal(updatedVarSlot);
            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.LoadConstantR8(1.0);
            ilEncoder.OpCode(isDecrement ? ILOpCode.Sub : ILOpCode.Add);
            ilEncoder.StoreLocal(updatedVarSlot);
            ilEncoder.OpCode(ILOpCode.Box);
            ilEncoder.Token(_bclReferences.DoubleType);
        }
        else
        {
            // prefix: value is new
            ilEncoder.LoadLocal(updatedVarSlot);
            ilEncoder.LoadConstantR8(1.0);
            ilEncoder.OpCode(isDecrement ? ILOpCode.Sub : ILOpCode.Add);
            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.StoreLocal(updatedVarSlot);
            ilEncoder.OpCode(ILOpCode.Box);
            ilEncoder.Token(_bclReferences.DoubleType);
        }
    }

    /// <summary>
    /// Emits a pure expression chain stack-only (no locals). The value ends up boxed on the stack.
    /// </summary>
    private void EmitTempStackOnly(MethodBodyIR methodBody, TempVariable temp, InstructionEncoder ilEncoder, bool hasScopesParameter, bool isInstanceMethod)
    {
        // Handle variable-mapped temps FIRST.
        if (temp.Index >= 0 && temp.Index < methodBody.TempVariableSlots.Count)
        {
            var varSlot = methodBody.TempVariableSlots[temp.Index];
            if (varSlot >= 0)
            {
                ilEncoder.LoadLocal(varSlot);
                // Get the storage type to determine if boxing is needed
                var storage = methodBody.TempStorages[temp.Index];
                if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(double))
                {
                    ilEncoder.OpCode(ILOpCode.Box);
                    ilEncoder.Token(_bclReferences.DoubleType);
                }
                else if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(bool))
                {
                    ilEncoder.OpCode(ILOpCode.Box);
                    ilEncoder.Token(_bclReferences.BooleanType);
                }
                // Reference types (object, string, etc.) are already boxed, no boxing needed
                return;
            }
        }

        var def = TryFindDefInstruction(methodBody, temp);
        if (def == null)
        {
            throw new InvalidOperationException($"EmitTempStackOnly: temp {temp.Index} has no definition and is not variable-mapped");
        }

        switch (def)
        {
            case LIRConstNumber cn:
                ilEncoder.LoadConstantR8(cn.Value);
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_bclReferences.DoubleType);
                break;

            case LIRConstString cs:
                ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString(cs.Value));
                break;

            case LIRConstBoolean cb:
                ilEncoder.LoadConstantI4(cb.Value ? 1 : 0);
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_bclReferences.BooleanType);
                break;

            case LIRConstUndefined:
                ilEncoder.OpCode(ILOpCode.Ldnull);
                break;

            case LIRConstNull:
                ilEncoder.LoadConstantI4((int)JavaScriptRuntime.JsNull.Null);
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsNull)));
                break;

            case LIRTypeof t:
                EmitTempStackOnly(methodBody, t.Value, ilEncoder, hasScopesParameter, isInstanceMethod);
                var typeofMethod = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.Typeof));
                ilEncoder.OpCode(ILOpCode.Call);
                ilEncoder.Token(typeofMethod);
                break;

            case LIRNegateNumber neg:
                EmitTempStackOnlyUnboxed(methodBody, neg.Value, ilEncoder, hasScopesParameter, isInstanceMethod);
                ilEncoder.OpCode(ILOpCode.Neg);
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_bclReferences.DoubleType);
                break;

            case LIRBitwiseNotNumber not:
                EmitTempStackOnlyUnboxed(methodBody, not.Value, ilEncoder, hasScopesParameter, isInstanceMethod);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Not);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_bclReferences.DoubleType);
                break;

            case LIRConvertToObject conv:
                EmitTempStackOnlyUnboxed(methodBody, conv.Source, ilEncoder, hasScopesParameter, isInstanceMethod);
                if (conv.SourceType == typeof(bool))
                {
                    ilEncoder.OpCode(ILOpCode.Box);
                    ilEncoder.Token(_bclReferences.BooleanType);
                }
                else if (conv.SourceType == typeof(JavaScriptRuntime.JsNull))
                {
                    ilEncoder.OpCode(ILOpCode.Box);
                    ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsNull)));
                }
                else
                {
                    ilEncoder.OpCode(ILOpCode.Box);
                    ilEncoder.Token(_bclReferences.DoubleType);
                }
                break;

            case LIRLoadParameter lp:
                // Parameters are already object (boxed), just load the argument
                // JS param 0 -> IL arg 1 when hasScopesParameter or instance method (arg0 is 'this'), else IL arg 0
                int argIndex = (hasScopesParameter || isInstanceMethod) ? lp.ParameterIndex + 1 : lp.ParameterIndex;
                ilEncoder.LoadArgument(argIndex);
                break;

            case LIRAddDynamic add:
                EmitTempStackOnly(methodBody, add.Left, ilEncoder, hasScopesParameter, isInstanceMethod);
                EmitTempStackOnly(methodBody, add.Right, ilEncoder, hasScopesParameter, isInstanceMethod);
                {
                    var addMethod = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), "Add");
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(addMethod);
                }
                break;

            case LIRMulDynamic mul:
                EmitTempStackOnly(methodBody, mul.Left, ilEncoder, hasScopesParameter, isInstanceMethod);
                EmitTempStackOnly(methodBody, mul.Right, ilEncoder, hasScopesParameter, isInstanceMethod);
                {
                    var mulMethod = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), "Multiply");
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(mulMethod);
                }
                break;

            default:
                throw new InvalidOperationException($"EmitTempStackOnly: unexpected instruction {def.GetType().Name}");
        }
    }

    /// <summary>
    /// Emits a pure expression chain stack-only, leaving the raw (unboxed) value on the stack.
    /// </summary>
    private void EmitTempStackOnlyUnboxed(MethodBodyIR methodBody, TempVariable temp, InstructionEncoder ilEncoder, bool hasScopesParameter, bool isInstanceMethod)
    {
        if (temp.Index >= 0 && temp.Index < methodBody.TempVariableSlots.Count)
        {
            var varSlot = methodBody.TempVariableSlots[temp.Index];
            if (varSlot >= 0)
            {
                ilEncoder.LoadLocal(varSlot);
                return;
            }
        }

        var def = TryFindDefInstruction(methodBody, temp);
        if (def == null)
        {
            throw new InvalidOperationException($"EmitTempStackOnlyUnboxed: temp {temp.Index} has no definition and is not variable-mapped");
        }

        switch (def)
        {
            case LIRConstNumber cn:
                ilEncoder.LoadConstantR8(cn.Value);
                break;

            case LIRConstBoolean cb:
                ilEncoder.LoadConstantI4(cb.Value ? 1 : 0);
                break;

            case LIRConstNull:
                ilEncoder.LoadConstantI4((int)JavaScriptRuntime.JsNull.Null);
                break;

            case LIRNegateNumber neg:
                EmitTempStackOnlyUnboxed(methodBody, neg.Value, ilEncoder, hasScopesParameter, isInstanceMethod);
                ilEncoder.OpCode(ILOpCode.Neg);
                break;

            case LIRBitwiseNotNumber not:
                EmitTempStackOnlyUnboxed(methodBody, not.Value, ilEncoder, hasScopesParameter, isInstanceMethod);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Not);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                break;

            default:
                throw new InvalidOperationException($"EmitTempStackOnlyUnboxed: unexpected instruction {def.GetType().Name}");
        }
    }

    private void EmitLoadIntrinsicGlobalVariable(string variableName, InstructionEncoder ilEncoder)
    {
        var gvType = typeof(JavaScriptRuntime.GlobalThis);
        var gvProp = gvType.GetProperty(variableName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
        var getterDecl = gvProp?.GetMethod?.DeclaringType!;
        var getterMref = _memberRefRegistry.GetOrAddMethod(getterDecl!, gvProp!.GetMethod!.Name);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(getterMref);
    }

    private void EmitInvokeIntrinsicMethod(Type declaringType, string methodName, InstructionEncoder ilEncoder)
    {
        var methodMref = _memberRefRegistry.GetOrAddMethod(declaringType, methodName);
        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(methodMref);
    }

    private static LIRInstruction? TryFindDefInstruction(MethodBodyIR methodBody, TempVariable temp)
    {
        foreach (var instr in methodBody.Instructions
            .Where(i => TempLocalAllocator.TryGetDefinedTemp(i, out var defined) && defined == temp))
        {
            return instr;
        }
        return null;
    }
}
