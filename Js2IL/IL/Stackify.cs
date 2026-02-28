using Js2IL.IR;
using System.Linq;

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
/// </summary>
/// <remarks>
/// For a detailed explanation (with examples), see docs/compiler/Stackify.md.
/// </remarks>
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

            // Typed numeric binary ops (LIR*Number) are safe to stackify only when the use is
            // immediately after the definition.
            //
            // Rationale: these ops can be re-emitted at the use site by Stackify, but the main
            // emitter must *not* also emit them at the def site (otherwise we'd compute + pop,
            // then compute again at the load site). The current emitter supports this pattern
            // for immediate chaining; we keep the analysis conservative to reduce churn.
            var use = uses[0];
            if (instr is LIRAddNumber or LIRSubNumber or LIRMulNumber or LIRDivNumber or LIRModNumber or LIRExpNumber)
            {
                if (use != def + 1)
                {
                    continue;
                }
            }

            // Only instructions that can be emitted inline are candidates.
            // Exception: certain side-effectful instructions (currently: LIRCallTypedMember)
            // can be stackified when the single use is *immediately* after the definition,
            // and the IL emitter defers execution to that use site (so it is not re-emitted).
            if (!CanEmitInline(instr, methodBody, defInstruction) && instr is not LIRCallTypedMember)
            {
                continue;
            }

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

        // IMPORTANT: for side-effectful instructions like calls, stackifying is only safe when
        // the use immediately follows the definition. Otherwise we'd be delaying execution until
        // the load site, which can reorder side effects relative to intervening instructions.
        // The direct adjacency case above is safe because the call would execute at the same
        // program point (just without an intervening local store/load).
        var defInstrForTemp = methodBody.Instructions[defIndex];
        if (defInstrForTemp is LIRCallTypedMember)
        {
            return false;
        }

        // Special-case: receiver temps for intrinsic instance calls (e.g., console.log).
        // Restrict this to receivers defined by LIRGetIntrinsicGlobal (e.g., GlobalThis.console)
        // which are expected to be pure/singleton lookups.
        // This avoids materializing receivers like GlobalThis.get_console() into locals.
        {
            var useInstr = methodBody.Instructions[useIndex];
            var defInstr = methodBody.Instructions[defIndex];
            if (defInstr is LIRGetIntrinsicGlobal &&
                useInstr is LIRCallIntrinsic callIntrinsic &&
                callIntrinsic.IntrinsicObject.Index == targetTemp.Index)
            {
                for (int i = defIndex + 1; i < useIndex; i++)
                {
                    var instr = methodBody.Instructions[i];
                    if (IsControlFlowInstruction(instr))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (defInstr is LIRGetIntrinsicGlobal &&
                useInstr is LIRCallInstanceMethod callInstanceMethod &&
                callInstanceMethod.Receiver.Index == targetTemp.Index)
            {
                for (int i = defIndex + 1; i < useIndex; i++)
                {
                    var instr = methodBody.Instructions[i];
                    if (IsControlFlowInstruction(instr))
                    {
                        return false;
                    }
                }

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
        // Treat leaf-scope creation as a barrier for stackification: it overwrites the
        // leaf scope local (ldloc.0) and can change the result of re-emitted scope field loads.
        return instruction is LIRLabel or LIRBranch or LIRBranchIfFalse or LIRBranchIfTrue or LIRCreateLeafScopeInstance or LIRCreateScopeInstance;
    }

    /// <summary>
    /// Returns true if the instruction can be emitted inline without storing to a local.
    /// 
    /// IMPORTANT: This method determines which instructions can be safely re-emitted
    /// when loading a stackable temp. Only instructions that are:
    /// 1. Side-effect free (no state changes)
    /// 2. Trivially cheap to re-execute (constants, parameter loads)
    /// should return true.
    /// 
    /// Binary operations (LIRAddDynamic, LIRMulDynamic, etc.) must NOT be included here
    /// because re-emitting them would cause duplicate computation of the entire operation.
    ///
    /// Typed numeric binary operations (LIR*Number) *may* be included when their operands are
    /// inlineable, because they lower to pure IL ops (add/mul/etc) and stackification only
    /// applies when a temp has a single use.
    /// </summary>
    private static bool CanEmitInline(LIRInstruction instruction, MethodBodyIR methodBody, LIRInstruction?[] defInstruction)
    {
        switch (instruction)
        {
            // Simple constants - trivially cheap to re-emit
            case LIRConstNumber:
            case LIRConstString:
            case LIRConstBoolean:
            case LIRConstUndefined:
            case LIRConstNull:
                return true;

            // Parameter loads - just ldarg, trivially cheap
            case LIRLoadParameter:
            case LIRLoadThis:
                return true;

            // Scope field loads - side-effect free field reads, similar to parameter loads
            case LIRLoadLeafScopeField:
            case LIRLoadParentScopeField:
                return true;

            case LIRLoadScopeField loadScopeField:
                // Only inline if the scope-instance temp is stable.
                // If the scope instance temp is backed by a variable slot that can be reassigned
                // (e.g., per-iteration environment recreation), re-emitting the load could observe
                // a different scope instance.
                var scopeTempIdx = loadScopeField.ScopeInstance.Index;
                if (scopeTempIdx >= 0 && scopeTempIdx < methodBody.TempVariableSlots.Count && methodBody.TempVariableSlots[scopeTempIdx] >= 0)
                {
                    var scopeVarSlot = methodBody.TempVariableSlots[scopeTempIdx];
                    if (!methodBody.SingleAssignmentSlots.Contains(scopeVarSlot))
                    {
                        return false;
                    }
                }
                return true;

            // Intrinsic global loads - just a dictionary lookup, no side effects
            case LIRGetIntrinsicGlobal:
                return true;

            // Intrinsic global function-value loads - pure lookup of cached delegate
            case LIRGetIntrinsicGlobalFunction:
                return true;

            // Scopes array creation - creates a small array, safe to inline
            // Used once per closure creation, no observable side effects
            case LIRBuildScopesArray:
                return true;

            // Unary operators - cheap runtime calls, no side effects
            case LIRBitwiseNotNumber:
            case LIRTypeof:
            case LIRNegateNumber:
                return true;

            // Type checks - side-effect free IL 'isinst'
            case LIRIsInstanceOf isInstanceOf:
                return IsInlineableOperand(isInstanceOf.Value);

            // LIRConvertToNumber can be emitted inline if its source can be emitted inline
            // AND the source is not backed by a variable slot that could be modified.
            // This mirrors the postfix increment safety rule used for LIRConvertToObject.
            case LIRConvertToNumber convertToNumber:
                var numSourceIdx = convertToNumber.Source.Index;
                if (numSourceIdx >= 0 && numSourceIdx < methodBody.TempVariableSlots.Count && methodBody.TempVariableSlots[numSourceIdx] >= 0)
                {
                    var varSlot = methodBody.TempVariableSlots[numSourceIdx];
                    if (!methodBody.SingleAssignmentSlots.Contains(varSlot))
                    {
                        return false;
                    }
                }
                if (numSourceIdx >= 0 && numSourceIdx < defInstruction.Length && defInstruction[numSourceIdx] != null)
                {
                    return CanEmitInline(defInstruction[numSourceIdx]!, methodBody, defInstruction);
                }
                return false;

            // LIRConcatStrings (System.String::Concat) is side-effect free.
            // It can be emitted inline only if both operands are safely loadable inline.
            // This avoids creating new inlining paths that could fail at IL emission time.
            case LIRConcatStrings concatStrings:
                return IsInlineableOperand(concatStrings.Left) && IsInlineableOperand(concatStrings.Right);

            // Typed comparisons are side-effect free and compile to pure IL.
            // Safe to inline when both operands are inlineable.
            case LIRCompareNumberLessThan cmpLt:
                return IsInlineableOperand(cmpLt.Left) && IsInlineableOperand(cmpLt.Right);
            case LIRCompareNumberGreaterThan cmpGt:
                return IsInlineableOperand(cmpGt.Left) && IsInlineableOperand(cmpGt.Right);
            case LIRCompareNumberLessThanOrEqual cmpLe:
                return IsInlineableOperand(cmpLe.Left) && IsInlineableOperand(cmpLe.Right);
            case LIRCompareNumberGreaterThanOrEqual cmpGe:
                return IsInlineableOperand(cmpGe.Left) && IsInlineableOperand(cmpGe.Right);
            case LIRCompareNumberEqual cmpEq:
                return IsInlineableOperand(cmpEq.Left) && IsInlineableOperand(cmpEq.Right);
            case LIRCompareNumberNotEqual cmpNe:
                return IsInlineableOperand(cmpNe.Left) && IsInlineableOperand(cmpNe.Right);
            case LIRCompareBooleanEqual cmpBoolEq:
                return IsInlineableOperand(cmpBoolEq.Left) && IsInlineableOperand(cmpBoolEq.Right);
            case LIRCompareBooleanNotEqual cmpBoolNe:
                return IsInlineableOperand(cmpBoolNe.Left) && IsInlineableOperand(cmpBoolNe.Right);

            // Typed numeric binary ops compile to pure IL.
            // Safe to inline when both operands are inlineable.
            case LIRAddNumber addNumber:
                return IsInlineableOperand(addNumber.Left) && IsInlineableOperand(addNumber.Right);
            case LIRSubNumber subNumber:
                return IsInlineableOperand(subNumber.Left) && IsInlineableOperand(subNumber.Right);
            case LIRMulNumber mulNumber:
                return IsInlineableOperand(mulNumber.Left) && IsInlineableOperand(mulNumber.Right);
            case LIRDivNumber divNumber:
                return IsInlineableOperand(divNumber.Left) && IsInlineableOperand(divNumber.Right);
            case LIRModNumber modNumber:
                return IsInlineableOperand(modNumber.Left) && IsInlineableOperand(modNumber.Right);
            case LIRExpNumber expNumber:
                return IsInlineableOperand(expNumber.Left) && IsInlineableOperand(expNumber.Right);

            // LIRBuildArray creates an array and initializes elements inline.
            // Safe to inline if all element temps can be emitted inline.
            // Used for call arguments (e.g., console.log) where array is consumed immediately.
            case LIRBuildArray buildArray:
                foreach (var elemIdx in buildArray.Elements.Select(e => e.Index))
                {
                    if (elemIdx < 0 || elemIdx >= defInstruction.Length || defInstruction[elemIdx] == null)
                        return false;
                    if (!CanEmitInline(defInstruction[elemIdx]!, methodBody, defInstruction))
                        return false;
                }
                return true;

            // LIRNewJsArray creates a JavaScriptRuntime.Array and initializes elements inline.
            // Safe to inline if all element temps can be emitted inline.
            case LIRNewJsArray newJsArray:
                foreach (var elemIdx in newJsArray.Elements.Select(e => e.Index))
                {
                    if (elemIdx < 0 || elemIdx >= defInstruction.Length || defInstruction[elemIdx] == null)
                        return false;
                    if (!CanEmitInline(defInstruction[elemIdx]!, methodBody, defInstruction))
                        return false;
                }
                return true;

            // LIRNewJsObject creates a JsObject and initializes properties inline.
            // Safe to inline if all property value temps can be emitted inline.
            case LIRNewJsObject newJsObject:
                foreach (var valueIdx in newJsObject.Properties.Select(p => p.Value.Index))
                {
                    if (valueIdx < 0 || valueIdx >= defInstruction.Length || defInstruction[valueIdx] == null)
                        return false;
                    if (!CanEmitInline(defInstruction[valueIdx]!, methodBody, defInstruction))
                        return false;
                }
                return true;

            // LIRGetLength and LIRGetItem are pure runtime calls
            case LIRGetLength:
            case LIRGetItem:
            case LIRGetItemAsNumber:
                return true;

            // Proven Array/Int32Array length gets are side-effect free.
            case LIRGetJsArrayLength:
            case LIRGetInt32ArrayLength:
                return true;

            // Proven Array element get is side-effect free.
            case LIRGetJsArrayElement:
                return true;

            // Typed intrinsic element get is a side-effect free callvirt get_Item(double).
            // Safe to inline/stackify.
            case LIRGetInt32ArrayElement:
                return true;

            // Typed intrinsic element set mutates the array and must never be re-emitted.
            case LIRSetInt32ArrayElement:
                return false;

            // Proven Array length set mutates the array and must never be re-emitted.
            case LIRSetJsArrayLength:
                return false;

            // Proven Array element set mutates the array and must never be re-emitted.
            case LIRSetJsArrayElement:
                return false;

            // LIRArrayPushRange and LIRArrayAdd have side effects (mutate the array)
            // but don't produce results, so they're not candidates for inlining
            case LIRArrayPushRange:
            case LIRArrayAdd:
                return false;


            // LIRCallIntrinsic calls an intrinsic method (e.g., console.log).
            // Calls are not safe to inline/stackify because the emitter will still
            // execute them in the main pass when the result temp is not materialized.
            // If the temp is later loaded (stackified), the call would be emitted again.
            case LIRCallIntrinsic:
                return false;

            // LIRCallInstanceMethod calls a known CLR instance method on a typed receiver.
            case LIRCallInstanceMethod:
                // Instance method calls may have side effects (and may mutate the receiver).
                // They must never be inlined/re-emitted by Stackify.
                return false;

            // LIRCallIntrinsicStatic calls a static method on an intrinsic type (e.g., Array.isArray).
            case LIRCallIntrinsicStatic:
                return false;

            case LIRCallIntrinsicStaticWithArgsArray:
                return false;

            // Statement-level intrinsic static calls must never be inlined/re-emitted.
            case LIRCallIntrinsicStaticVoid:
                return false;

            case LIRCallIntrinsicStaticVoidWithArgsArray:
                return false;

            // LIRCallFunction calls a user-defined function.
            case LIRCallFunction:
                return false;

            case LIRCallFunctionWithArgsArray:
                return false;

            // LIRCallFunctionValue calls a function value via runtime dispatch.
            case LIRCallFunctionValue:
            case LIRCallFunctionValue0:
            case LIRCallFunctionValue1:
            case LIRCallFunctionValue2:
            case LIRCallFunctionValue3:
                return false;

            // LIRCallRequire is a direct callvirt to RequireDelegate and must never be inlined/re-emitted.
            case LIRCallRequire:
            // LIRCallImport is a direct call to DynamicImport.Import and must never be inlined/re-emitted.
            case LIRCallImport:
                return false;

            // LIRCallMember calls a member via runtime dispatcher.
            case LIRCallMember:
            case LIRCallMember0:
            case LIRCallMember1:
            case LIRCallMember2:
            case LIRCallMember3:
                return false;

            // LIRCallTypedMember is side-effectful and must never be considered inlineable/re-emittable.
            // Stackification support for immediate fluent chaining is handled explicitly in Analyze
            // (and must be paired with IL emission that defers execution to the single use site).
            case LIRCallTypedMember:
                return false;

            // Fallback form expands to a small control-flow sequence (labels/branches), not safe to inline.
            case LIRCallTypedMemberWithFallback:
                return false;

            // LIRCallUserClassInstanceMethod performs a direct callvirt to a generated class method.
            // Like other calls, it must never be inlined/re-emitted by Stackify.
            case LIRCallUserClassInstanceMethod:
                return false;

            // LIRCallDeclaredCallable performs a direct call to a declared method.
            // Like other calls, it must never be inlined/re-emitted by Stackify.
            case LIRCallDeclaredCallable:
                return false;

            // LIRConvertToObject can be emitted inline if its source can be emitted inline
            // AND the source is not backed by a variable slot that could be modified.
            // If the source is backed by a variable slot, that slot may be overwritten by a later
            // SSA value before the box is consumed (e.g., postfix increment: x++ must capture 
            // the old value before the slot is updated, so the box must materialize).
            // However, if the slot is marked as single-assignment (e.g., const variables),
            // the value is guaranteed to never change, so we can safely inline.
            case LIRConvertToObject convertToObject:
                var sourceIdx = convertToObject.Source.Index;
                // Check variable slot first - if source is backed by a variable slot
                if (sourceIdx >= 0 && sourceIdx < methodBody.TempVariableSlots.Count && methodBody.TempVariableSlots[sourceIdx] >= 0)
                {
                    var varSlot = methodBody.TempVariableSlots[sourceIdx];
                    // If the variable slot is single-assignment, it's safe to inline
                    if (!methodBody.SingleAssignmentSlots.Contains(varSlot))
                    {
                        return false;
                    }
                }
                // Now check if the source's defining instruction can be inlined
                if (sourceIdx >= 0 && sourceIdx < defInstruction.Length && defInstruction[sourceIdx] != null)
                {
                    return CanEmitInline(defInstruction[sourceIdx]!, methodBody, defInstruction);
                }
                return false;

            // NOTE: LIRAddDynamic, LIRMulDynamic, and other binary operations are intentionally
            // NOT included here. While the IL emitter can technically emit them inline, doing so
            // would cause the entire computation to be duplicated each time the temp is loaded.
            // This was the bug found by the code reviewer - expressions like "Hello, " + name + "!"
            // were being computed multiple times.

            default:
                return false;
        }

        bool IsInlineableOperand(TempVariable temp)
        {
            var idx = temp.Index;
            if (idx < 0)
            {
                return false;
            }

            // Temps backed by variable slots are always materialized into stable locals,
            // so they are safe operands for inline concat.
            if (idx < methodBody.TempVariableSlots.Count && methodBody.TempVariableSlots[idx] >= 0)
            {
                return true;
            }

            // If we have a defining instruction for the operand, it must itself be inlineable.
            if (idx < defInstruction.Length && defInstruction[idx] != null)
            {
                return CanEmitInline(defInstruction[idx]!, methodBody, defInstruction);
            }

            // No def instruction and not variable-backed: conservatively disallow.
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
            case LIRGetIntrinsicGlobalFunction:
            case LIRLoadUserClassStaticField:
            case LIRLoadParameter:
            case LIRLoadThis:
            case LIRBuildScopesArray:
                return (0, 1);

            // LIRBuildArray: consumes N element temps, produces 1 array reference
            // The dup pattern keeps array on stack internally, net effect is: pop N, push 1
            case LIRBuildArray buildArray:
                return (buildArray.Elements.Count, 1);

            // LIRNewJsArray: consumes N element temps, produces 1 JavaScriptRuntime.Array reference
            // The dup pattern keeps array on stack internally, net effect is: pop N, push 1
            case LIRNewJsArray newJsArray:
                return (newJsArray.Elements.Count, 1);

            // LIRNewJsObject: consumes N property value temps, produces 1 JsObject reference
            // The dup pattern keeps object on stack internally, net effect is: pop N, push 1
            case LIRNewJsObject newJsObject:
                return (newJsObject.Properties.Count, 1);

            // LIRGetLength: consumes 1 object, produces 1 double
            case LIRGetLength:
                return (1, 1);

            // LIRGetJsArrayLength: consumes 1 receiver, produces 1 double
            case LIRGetJsArrayLength:
                return (1, 1);

            // LIRGetInt32ArrayLength: consumes 1 receiver, produces 1 double
            case LIRGetInt32ArrayLength:
                return (1, 1);

            // LIRGetItem: consumes 2 (object + index), produces 1 value
            case LIRGetItem:
                return (2, 1);

            // LIRGetItemAsNumber: consumes 2 (object + index), produces 1 unboxed double
            case LIRGetItemAsNumber:
                return (2, 1);

            // LIRGetJsArrayElement: consumes 2 (receiver + index), produces 1 value
            case LIRGetJsArrayElement:
                return (2, 1);

            // LIRGetInt32ArrayElement: consumes 2 (receiver + index), produces 1 double
            case LIRGetInt32ArrayElement:
                return (2, 1);

            // LIRSetInt32ArrayElement: consumes receiver + index + value; side effects, no net value left on stack
            // (its SSA result is materialized into a local when needed).
            case LIRSetInt32ArrayElement:
                return (3, 0);

            // LIRSetJsArrayLength: consumes receiver + value; side effects, no net value left on stack
            // (its SSA result is materialized into a local when needed).
            case LIRSetJsArrayLength:
                return (2, 0);

            // LIRSetJsArrayElement: consumes receiver + index + value; side effects, no net value left on stack
            // (its SSA result is materialized into a local when needed).
            case LIRSetJsArrayElement:
                return (3, 0);

            // Direct instance method call on a user-defined class: consumes N args, produces 1 result.
            case LIRCallUserClassInstanceMethod callUserClass:
                return (callUserClass.Arguments.Count, 1);

            // LIRArrayPushRange: consumes 2 (target array + source), produces 0 (void return)
            case LIRArrayPushRange:
                return (2, 0);

            // LIRArrayAdd: consumes 2 (target array + element), produces 0 (void return)
            case LIRArrayAdd:
                return (2, 0);


            // Scope field loads: produce 1 value (field is loaded from memory, not from stack temps)
            case LIRLoadLeafScopeField:
            case LIRLoadParentScopeField:
                return (0, 1);

            // Scope field stores: consume 1 value (the value being stored, not counting the scope instance/array which comes from local/arg)
            case LIRStoreLeafScopeField:
            case LIRStoreParentScopeField:
                return (1, 0);

            // Binary ops: consume 2, produce 1
            case LIRAddNumber:
            case LIRAddDynamic:
            case LIRAddDynamicDoubleObject:
            case LIRAddDynamicObjectDouble:
            case LIRAddAndToNumber:
            case LIRSubNumber:
            case LIRMulNumber:
            case LIRMulDynamic:
            case LIRConcatStrings:
            case LIRDivNumber:
            case LIRModNumber:
            case LIRExpNumber:
            case LIRBitwiseAnd:
            case LIRBitwiseOr:
            case LIRBitwiseXor:
            case LIRLeftShift:
            case LIRRightShift:
            case LIRUnsignedRightShift:
            case LIRInOperator:
            case LIRInstanceOfOperator:
            case LIREqualDynamic:
            case LIRNotEqualDynamic:
            case LIRStrictEqualDynamic:
            case LIRStrictNotEqualDynamic:
            case LIRBinaryDynamicOperator:
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
            case LIRConvertToNumber:
            case LIRIsInstanceOf:
            case LIRTypeof:
            case LIRNegateNumber:
            case LIRNegateNumberDynamic:
            case LIRBitwiseNotNumber:
            case LIRBitwiseNotDynamic:
            case LIRCallIsTruthy:
            case LIRCallIsTruthyDouble:
            case LIRCallIsTruthyBool:
            case LIRCopyTemp:
                return (1, 1);

            // Call: consumes args + object, produces result
            case LIRCallIntrinsic:
                return (2, 1);

            // Instance call: consumes receiver + N args, produces result
            case LIRCallInstanceMethod callInstance:
                return (1 + callInstance.Arguments.Count, 1);

            // Intrinsic static call: consumes N args, produces 1 result
            case LIRCallIntrinsicStatic callStatic:
                return (callStatic.Arguments.Count, 1);

            // Intrinsic static call (args array): consumes argsArray, produces 1 result
            case LIRCallIntrinsicStaticWithArgsArray:
                return (1, 1);

            // Statement-level intrinsic static call: consumes N args, produces 0
            case LIRCallIntrinsicStaticVoid callStaticVoid:
                return (callStaticVoid.Arguments.Count, 0);

            // Statement-level intrinsic static call (args array): consumes argsArray, produces 0
            case LIRCallIntrinsicStaticVoidWithArgsArray:
                return (1, 0);

            // Declared callable direct call: consumes N args, produces 1 result
            case LIRCallDeclaredCallable callDeclared:
                return (callDeclared.Arguments.Count, 1);

            case LIRCallFunction call:
                return (1 + call.Arguments.Count, 1);

            case LIRCallFunctionWithArgsArray:
                // scopesArray + argsArray -> result
                return (2, 1);

            case LIRCallFunctionValue:
                // target + scopesArray + argsArray -> result
                return (3, 1);
            case LIRCallFunctionValue0:
                // target + scopesArray -> result
                return (2, 1);
            case LIRCallFunctionValue1:
                // target + scopesArray + a0 -> result
                return (3, 1);
            case LIRCallFunctionValue2:
                // target + scopesArray + a0 + a1 -> result
                return (4, 1);
            case LIRCallFunctionValue3:
                // target + scopesArray + a0 + a1 + a2 -> result
                return (5, 1);

            case LIRCallRequire:
                // requireValue + moduleId -> result
                return (2, 1);

            case LIRCallImport:
                // specifier + currentModuleId -> result
                return (2, 1);

            case LIRCallMember:
                // receiver + methodName + argsArray -> result
                return (3, 1);
            case LIRCallMember0:
                // receiver + methodName -> result
                return (2, 1);
            case LIRCallMember1:
                // receiver + methodName + a0 -> result
                return (3, 1);
            case LIRCallMember2:
                // receiver + methodName + a0 + a1 -> result
                return (4, 1);
            case LIRCallMember3:
                // receiver + methodName + a0 + a1 + a2 -> result
                return (5, 1);

            // Typed member call without fallback: receiver + N args -> result
            case LIRCallTypedMember callTyped:
                return (1 + callTyped.Arguments.Count, 1);

            // Typed member call with fallback: receiver + N args -> result
            // (The fallback path allocates an args array, but the LIR-level operands are still receiver+args.)
            case LIRCallTypedMemberWithFallback callTypedFallback:
                return (1 + callTypedFallback.Arguments.Count, 1);

            // Return: consumes return value
            case LIRReturn:
                return (1, 0);

            // StoreParameter: consumes value
            case LIRStoreParameter:
                return (1, 0);

            // Hints and control flow have no stack effect at LIR level
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
}
