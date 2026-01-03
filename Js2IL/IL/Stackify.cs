using Js2IL.IR;

namespace Js2IL.IL;

/// <summary>
/// Result of stackify analysis - indicates which temps can stay on the evaluation stack
/// without being stored to IL local variables.
/// </summary>
internal readonly record struct StackifyResult(bool[] CanStackify)
{
    /// <summary>
    /// Returns true if the given temp can be kept on the stack instead of stored to a local.
    /// </summary>
    public bool IsStackable(TempVariable temp)
        => temp.Index >= 0 && temp.Index < CanStackify.Length && CanStackify[temp.Index];
}

/// <summary>
/// Performs stack-friendliness analysis on LIR instructions.
/// 
/// Identifies temps that can remain on the evaluation stack (avoiding store/load pairs)
/// when the following conditions are met:
/// 1. The temp has exactly one use
/// 2. The use immediately follows the definition (no intervening instructions that push/pop the stack)
/// 3. No control flow (branches/labels) between definition and use
/// 4. The temp is consumed in the correct stack order (LIFO)
/// 5. The defining instruction can be emitted inline (i.e., CanEmitInline returns true)
/// 
/// Additionally, attempts to reorder instructions to make more values stack-friendly
/// when safe to do so.
/// </summary>
internal static class Stackify
{
    /// <summary>
    /// Analyzes the method body to determine which temps can stay on the stack.
    /// Returns a result indicating which temps are stackable.
    /// </summary>
    public static StackifyResult Analyze(MethodBodyIR methodBody)
    {
        int tempCount = methodBody.Temps.Count;
        if (tempCount == 0)
        {
            return new StackifyResult(Array.Empty<bool>());
        }

        var canStackify = new bool[tempCount];

        // Build def-use information
        var defIndex = new int[tempCount];
        var defInstruction = new LIRInstruction?[tempCount];
        var useIndices = new List<int>[tempCount];
        Array.Fill(defIndex, -1);

        for (int i = 0; i < tempCount; i++)
        {
            useIndices[i] = new List<int>();
        }

        // First pass: gather def and use sites for each temp
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            var instruction = methodBody.Instructions[i];

            if (TempLocalAllocator.TryGetDefinedTemp(instruction, out var def) &&
                def.Index >= 0 && def.Index < tempCount)
            {
                defIndex[def.Index] = i;
                defInstruction[def.Index] = instruction;
            }

            foreach (var used in TempLocalAllocator.EnumerateUsedTemps(instruction))
            {
                if (used.Index >= 0 && used.Index < tempCount)
                {
                    useIndices[used.Index].Add(i);
                }
            }
        }

        // Second pass: check if temps meet stackify criteria
        for (int tempIdx = 0; tempIdx < tempCount; tempIdx++)
        {
            var def = defIndex[tempIdx];
            var uses = useIndices[tempIdx];
            var instr = defInstruction[tempIdx];

            // Must have exactly one definition and one use
            if (def < 0 || uses.Count != 1 || instr == null)
            {
                continue;
            }

            // Only instructions that can be emitted inline are candidates
            // This prevents marking temps that require materialization (like LIRCreateScopesArray)
            if (!CanEmitInline(instr, methodBody, defInstruction))
            {
                continue;
            }

            var use = uses[0];

            // Use must come after definition
            if (use <= def)
            {
                continue;
            }

            // Check if the value can stay on the stack between def and use
            if (CanStackifyBetween(methodBody, def, use, new TempVariable(tempIdx)))
            {
                canStackify[tempIdx] = true;
            }
        }

        return new StackifyResult(canStackify);
    }

    /// <summary>
    /// Checks if a temp defined at defIndex can remain on the stack until used at useIndex.
    /// This requires:
    /// 1. No control flow between def and use
    /// 2. All intervening instructions that produce values also consume them before use
    /// 3. The temp is used in LIFO stack order
    /// </summary>
    private static bool CanStackifyBetween(MethodBodyIR methodBody, int defIndex, int useIndex, TempVariable targetTemp)
    {
        // Direct adjacency: def immediately followed by use - always stackable
        if (useIndex == defIndex + 1)
        {
            // But only if target temp is the first operand consumed
            var useInstr = methodBody.Instructions[useIndex];
            var operands = TempLocalAllocator.EnumerateUsedTemps(useInstr).ToList();
            if (operands.Count > 0 && operands[0].Index == targetTemp.Index)
            {
                return true;
            }
            // If it's the only operand, also stackable
            if (operands.Count == 1 && operands[0].Index == targetTemp.Index)
            {
                return true;
            }
        }

        // Check for intervening control flow
        for (int i = defIndex + 1; i < useIndex; i++)
        {
            var instr = methodBody.Instructions[i];
            if (IsControlFlowInstruction(instr))
            {
                return false;
            }
        }

        // Check stack discipline: track what's on the stack between def and use
        // The target temp is pushed onto the stack at defIndex
        // We need to ensure it's still on the stack (and at the right position) at useIndex
        var stackDepth = 1; // After def, target temp is on stack
        var targetStackPosition = 0; // Target is at position 0 (top of stack after def)

        for (int i = defIndex + 1; i < useIndex; i++)
        {
            var instr = methodBody.Instructions[i];

            // Count how many values this instruction pops and pushes
            var (pops, pushes) = GetStackEffect(instr);

            // Can't pop our value
            if (pops > stackDepth - 1) // -1 because we can't pop our target
            {
                return false;
            }

            // Update stack depth and target position
            stackDepth = stackDepth - pops + pushes;
            targetStackPosition += pushes; // Target moves down as more things are pushed

            // If stack depth goes negative or target falls off, not stackable
            if (stackDepth < 1 || targetStackPosition >= stackDepth)
            {
                return false;
            }
        }

        // At use site, verify target is consumed correctly
        var useInstr2 = methodBody.Instructions[useIndex];
        var operands2 = TempLocalAllocator.EnumerateUsedTemps(useInstr2).ToList();

        // Find which operand position our target is at
        int targetOperandIndex = -1;
        for (int j = 0; j < operands2.Count; j++)
        {
            if (operands2[j].Index == targetTemp.Index)
            {
                targetOperandIndex = j;
                break;
            }
        }

        if (targetOperandIndex < 0)
        {
            return false; // Target not used at use site
        }

        // For stack-based consumption:
        // - First operand is pushed first (deeper on stack)
        // - Last operand is pushed last (top of stack)
        // So if we're the first operand and there are more operands, we need to be deeper on stack
        // Actually, in our LIR, operands are loaded in order (left then right)
        // So left operand should be on stack first (deeper), right operand on top
        // If targetStackPosition matches what's expected for this operand index, it's okay

        // Simplified check: if there are intervening values on the stack and target is not at top,
        // we need to ensure the instructions between def and use properly consume those values
        // For now, be conservative and only allow if immediately consumed or if no intervening pushes
        if (targetStackPosition != 0 && targetOperandIndex != 0)
        {
            // Target is not at top of stack, and not the first operand - might be unsafe
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns true if the instruction is a control flow instruction (branch or label).
    /// </summary>
    private static bool IsControlFlowInstruction(LIRInstruction instruction)
    {
        return instruction is LIRLabel or LIRBranch or LIRBranchIfFalse or LIRBranchIfTrue;
    }

    /// <summary>
    /// Returns true if the instruction can be emitted inline without storing to a local.
    /// This must match what LIRToILCompiler.EmitLoadTemp supports for inline emission.
    /// </summary>
    private static bool CanEmitInline(LIRInstruction instruction, MethodBodyIR methodBody, LIRInstruction?[] defInstruction)
    {
        switch (instruction)
        {
            // Simple constants and parameter loads
            case LIRConstNumber:
            case LIRConstString:
            case LIRConstBoolean:
            case LIRConstUndefined:
            case LIRConstNull:
            case LIRLoadParameter:
                return true;

            // LIRConvertToObject can be emitted inline if its source can be emitted inline
            case LIRConvertToObject convertToObject:
                var sourceIdx = convertToObject.Source.Index;
                if (sourceIdx >= 0 && sourceIdx < defInstruction.Length && defInstruction[sourceIdx] != null)
                {
                    return CanEmitInline(defInstruction[sourceIdx]!, methodBody, defInstruction);
                }
                return false;

            // Dynamic operations can be emitted inline if their operands can be
            case LIRMulDynamic:
            case LIRAddDynamic:
                return true; // These are supported in EmitLoadTemp

            default:
                return false;
        }
    }

    /// <summary>
    /// Gets the stack effect of an instruction (values popped, values pushed).
    /// This is for the intermediate values, not the temps themselves.
    /// </summary>
    private static (int Pops, int Pushes) GetStackEffect(LIRInstruction instruction)
    {
        // Most LIR instructions pop their operands and push their result
        // But since we're tracking at the LIR level where temps are explicit,
        // the "stack effect" here is about intermediate values that might be left on stack

        // Instructions that produce values but we're checking if they get stored
        switch (instruction)
        {
            // These instructions define a temp result (push 1)
            case LIRConstNumber:
            case LIRConstString:
            case LIRConstBoolean:
            case LIRConstUndefined:
            case LIRConstNull:
            case LIRGetIntrinsicGlobal:
            case LIRNewObjectArray:
            case LIRLoadParameter:
            case LIRCreateScopesArray:
                return (0, 1);

            // Binary ops: consume 2, produce 1
            case LIRAddNumber:
            case LIRAddDynamic:
            case LIRSubNumber:
            case LIRMulNumber:
            case LIRMulDynamic:
            case LIRConcatStrings:
            case LIRCompareNumberLessThan:
            case LIRCompareNumberGreaterThan:
            case LIRCompareNumberLessThanOrEqual:
            case LIRCompareNumberGreaterThanOrEqual:
            case LIRCompareNumberEqual:
            case LIRCompareNumberNotEqual:
            case LIRCompareBooleanEqual:
            case LIRCompareBooleanNotEqual:
                return (2, 1);

            // Unary ops: consume 1, produce 1
            case LIRConvertToObject:
            case LIRTypeof:
            case LIRNegateNumber:
            case LIRBitwiseNotNumber:
                return (1, 1);

            // Call: consumes args + object, produces result
            case LIRCallIntrinsic:
                return (2, 1);

            case LIRCallFunction call:
                return (1 + call.Arguments.Count, 1);

            // StoreElementRef: consumes array + value, produces nothing
            case LIRStoreElementRef:
                return (2, 0);

            // Return: consumes return value
            case LIRReturn:
                return (1, 0);

            // StoreParameter: consumes value
            case LIRStoreParameter:
                return (1, 0);

            // Hints and control flow have no stack effect at LIR level
            case LIRBeginInitArrayElement:
            case LIRLabel:
            case LIRBranch:
                return (0, 0);

            // Branches consume condition
            case LIRBranchIfFalse:
            case LIRBranchIfTrue:
                return (1, 0);

            default:
                return (0, 0);
        }
    }

    /// <summary>
    /// Attempts to reorder instructions to make more values stack-friendly.
    /// This is an optional optimization pass that can be run before the main analysis.
    /// </summary>
    /// <remarks>
    /// Currently performs these reorderings:
    /// 1. Move constant definitions closer to their single use
    /// 2. Reorder commutative binary operations if it improves stackability
    /// </remarks>
    public static void OptimizeInstructionOrder(MethodBodyIR methodBody)
    {
        // Phase 1: Move single-use constant definitions to just before their use
        MoveConstantsToUse(methodBody);

        // Phase 2: Reorder binary operands for better stack discipline
        // (This modifies the instruction list in place to swap operand order where beneficial)
        OptimizeBinaryOperandOrder(methodBody);
    }

    /// <summary>
    /// Moves constant definitions to immediately before their single use site.
    /// </summary>
    private static void MoveConstantsToUse(MethodBodyIR methodBody)
    {
        int tempCount = methodBody.Temps.Count;
        if (tempCount == 0) return;

        // Build use count and single use location
        var useCount = new int[tempCount];
        var singleUseIndex = new int[tempCount];
        Array.Fill(singleUseIndex, -1);

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            foreach (var used in TempLocalAllocator.EnumerateUsedTemps(methodBody.Instructions[i]))
            {
                if (used.Index >= 0 && used.Index < tempCount)
                {
                    useCount[used.Index]++;
                    singleUseIndex[used.Index] = i;
                }
            }
        }

        // Find constants with single use that can be moved
        var instructionsToMove = new List<(int FromIndex, int ToIndex, LIRInstruction Instruction)>();

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            var instr = methodBody.Instructions[i];

            // Only consider constant instructions
            if (!IsConstantInstruction(instr)) continue;

            if (!TempLocalAllocator.TryGetDefinedTemp(instr, out var def)) continue;
            if (def.Index < 0 || def.Index >= tempCount) continue;

            // Single use only
            if (useCount[def.Index] != 1) continue;

            var use = singleUseIndex[def.Index];
            if (use <= i + 1) continue; // Already adjacent or before

            // Check no control flow between def and use
            bool hasControlFlow = false;
            for (int j = i + 1; j < use; j++)
            {
                if (IsControlFlowInstruction(methodBody.Instructions[j]))
                {
                    hasControlFlow = true;
                    break;
                }
            }
            if (hasControlFlow) continue;

            // Can move this constant to just before its use
            instructionsToMove.Add((i, use, instr));
        }

        // Apply moves in reverse order to preserve indices
        foreach (var (fromIndex, toIndex, instr) in instructionsToMove.OrderByDescending(m => m.FromIndex))
        {
            methodBody.Instructions.RemoveAt(fromIndex);
            // Adjust toIndex since we removed an element before it
            var adjustedToIndex = toIndex - 1;
            methodBody.Instructions.Insert(adjustedToIndex, instr);
        }
    }

    /// <summary>
    /// For commutative binary operations, swaps operand order if it improves stackability.
    /// </summary>
    private static void OptimizeBinaryOperandOrder(MethodBodyIR methodBody)
    {
        int tempCount = methodBody.Temps.Count;
        if (tempCount == 0) return;

        // Build definition locations
        var defIndex = new int[tempCount];
        Array.Fill(defIndex, -1);

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (TempLocalAllocator.TryGetDefinedTemp(methodBody.Instructions[i], out var def) &&
                def.Index >= 0 && def.Index < tempCount)
            {
                defIndex[def.Index] = i;
            }
        }

        // Check each binary operation
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            var instr = methodBody.Instructions[i];

            // Only consider commutative operations
            if (!TryGetCommutativeBinaryOperands(instr, out var left, out var right)) continue;

            // Check if swapping would improve things
            // If right operand is defined immediately before (i-1), it's already on stack top
            // If left operand is defined immediately before but right is not, swap them
            var leftDef = left.Index >= 0 && left.Index < tempCount ? defIndex[left.Index] : -1;
            var rightDef = right.Index >= 0 && right.Index < tempCount ? defIndex[right.Index] : -1;

            if (leftDef == i - 1 && rightDef != i - 1 && rightDef >= 0)
            {
                // Left is on stack top but should be loaded first - swap
                // This means right should be loaded first (pushed deeper)
                var swapped = SwapBinaryOperands(instr);
                if (swapped != null)
                {
                    methodBody.Instructions[i] = swapped;
                }
            }
        }
    }

    private static bool IsConstantInstruction(LIRInstruction instruction)
    {
        return instruction is LIRConstNumber or LIRConstString or LIRConstBoolean
            or LIRConstUndefined or LIRConstNull;
    }

    private static bool TryGetCommutativeBinaryOperands(LIRInstruction instruction,
        out TempVariable left, out TempVariable right)
    {
        // NOTE: Only numeric operations are truly commutative.
        // LIRAddDynamic and LIRConcatStrings are NOT commutative because they may perform
        // string concatenation, which is order-dependent ("Hello" + "World" != "World" + "Hello").
        switch (instruction)
        {
            case LIRAddNumber add:
                left = add.Left;
                right = add.Right;
                return true;
            case LIRMulNumber mul:
                left = mul.Left;
                right = mul.Right;
                return true;
            case LIRMulDynamic mulDyn:
                // Multiplication is always numeric and commutative
                left = mulDyn.Left;
                right = mulDyn.Right;
                return true;
            case LIRCompareNumberEqual cmpEq:
                left = cmpEq.Left;
                right = cmpEq.Right;
                return true;
            case LIRCompareNumberNotEqual cmpNe:
                left = cmpNe.Left;
                right = cmpNe.Right;
                return true;
            case LIRCompareBooleanEqual cmpBoolEq:
                left = cmpBoolEq.Left;
                right = cmpBoolEq.Right;
                return true;
            case LIRCompareBooleanNotEqual cmpBoolNe:
                left = cmpBoolNe.Left;
                right = cmpBoolNe.Right;
                return true;
            // NOT included: LIRAddDynamic, LIRConcatStrings - string concatenation is not commutative
            default:
                left = default;
                right = default;
                return false;
        }
    }

    private static LIRInstruction? SwapBinaryOperands(LIRInstruction instruction)
    {
        // Only swap for truly commutative operations (not LIRAddDynamic)
        return instruction switch
        {
            LIRAddNumber add => new LIRAddNumber(add.Right, add.Left, add.Result),
            LIRMulNumber mul => new LIRMulNumber(mul.Right, mul.Left, mul.Result),
            LIRMulDynamic mulDyn => new LIRMulDynamic(mulDyn.Right, mulDyn.Left, mulDyn.Result),
            LIRCompareNumberEqual cmpEq => new LIRCompareNumberEqual(cmpEq.Right, cmpEq.Left, cmpEq.Result),
            LIRCompareNumberNotEqual cmpNe => new LIRCompareNumberNotEqual(cmpNe.Right, cmpNe.Left, cmpNe.Result),
            LIRCompareBooleanEqual cmpBoolEq => new LIRCompareBooleanEqual(cmpBoolEq.Right, cmpBoolEq.Left, cmpBoolEq.Result),
            LIRCompareBooleanNotEqual cmpBoolNe => new LIRCompareBooleanNotEqual(cmpBoolNe.Right, cmpBoolNe.Left, cmpBoolNe.Result),
            _ => null
        };
    }
}
