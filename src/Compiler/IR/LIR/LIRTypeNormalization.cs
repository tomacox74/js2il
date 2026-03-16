using Js2IL.Services;
using System.Reflection;

namespace Js2IL.IR;

/// <summary>
/// Normalizes generic LIR patterns into more explicit typed forms when provably safe.
///
/// Goal: keep LIR->IL focused on IL mechanics (stackify/locals/metadata) by pushing
/// type-directed rewrites earlier in the pipeline.
///
/// This pass is intentionally conservative.
/// </summary>
internal static class LIRTypeNormalization
{
    public static void Normalize(MethodBodyIR methodBody, ClassRegistry? classRegistry)
    {
        // Specialize IsTruthy calls regardless of whether a ClassRegistry is present.
        SpecializeIsTruthyCalls(methodBody);
        RewriteTypeofFunctionBranchComparisons(methodBody);

        if (classRegistry == null)
        {
            return;
        }

        // Stamp result temps from `new <UserClass>(...)` with the constructed CLR type handle when safe.
        // This enables strongly-typed locals for materialized temps and allows the IL emitter to
        // avoid redundant castclass instructions.
        //
        // Skip classes with PL5.4a ctor-return override semantics: the result temp may be overwritten
        // with an arbitrary value (not necessarily an instance of the constructed CLR type).
        foreach (var instr in methodBody.Instructions)
        {
            if (instr is not LIRNewUserClass newUserClass)
            {
                continue;
            }

            if (newUserClass.Result.Index < 0 || newUserClass.Result.Index >= methodBody.TempStorages.Count)
            {
                continue;
            }

            if (!classRegistry.TryGet(newUserClass.RegistryClassName, out var constructedTypeHandle) || constructedTypeHandle.IsNil)
            {
                continue;
            }

            if (classRegistry.TryGetPrivateField(newUserClass.RegistryClassName, "__js2il_ctorReturn", out _))
            {
                continue;
            }

            var storage = GetTempStorage(methodBody, newUserClass.Result);
            if (storage.Kind == ValueStorageKind.Reference)
            {
                storage = storage with { TypeHandle = constructedTypeHandle };
                SetTempStorage(methodBody, newUserClass.Result, storage);

                // If this temp is pinned to a declared variable slot, keep the variable storage in sync
                // so the local signature also becomes strongly typed.
                var slot = GetTempVariableSlot(methodBody, newUserClass.Result);
                if (slot >= 0 && slot < methodBody.VariableStorages.Count)
                {
                    methodBody.VariableStorages[slot] = storage;
                }
            }
        }

        for (int i = 0; i < methodBody.Instructions.Count - 1; i++)
        {
            // Peephole: Object.NormalizeForOfIterable(x) where x is already a known iterable.
            // Today we only prove this for JavaScriptRuntime.Array.
            // Rewrite to a direct copy to avoid an unnecessary runtime call.
            if (methodBody.Instructions[i] is LIRCallIntrinsicStatic normalizeForOf &&
                string.Equals(normalizeForOf.IntrinsicName, "Object", StringComparison.Ordinal) &&
                string.Equals(normalizeForOf.MethodName, "NormalizeForOfIterable", StringComparison.Ordinal) &&
                normalizeForOf.Arguments.Count == 1)
            {
                var source = normalizeForOf.Arguments[0];
                var sourceStorage = GetTempStorage(methodBody, source);

                if (sourceStorage.Kind == ValueStorageKind.Reference && sourceStorage.ClrType == typeof(JavaScriptRuntime.Array))
                {
                    // Preserve the stable loop-carry variable slot (created in lowering) without
                    // materializing an extra temp local + copy in IL.
                    //
                    // If the NormalizeForOfIterable result temp is pinned to a variable slot,
                    // pin the proven-array source temp to the same slot. This causes the source
                    // temp to store directly into that slot at its definition site, and downstream
                    // loads of the result temp will read the same local.
                    var resultSlot = GetTempVariableSlot(methodBody, normalizeForOf.Result);
                    if (resultSlot >= 0)
                    {
                        var sourceSlot = GetTempVariableSlot(methodBody, source);
                        if (sourceSlot >= 0)
                        {
                            // Prefer reusing an existing stable variable slot (e.g., `arr` in
                            // `for (x of arr)`) by mapping the iterator temp onto it.
                            // This avoids introducing a redundant `$forOf_iter` copy.
                            if (sourceSlot != resultSlot)
                            {
                                SetTempVariableSlot(methodBody, normalizeForOf.Result, sourceSlot);
                            }
                        }
                        else
                        {
                            // Otherwise, map the source temp into the loop-carry slot so its
                            // definition stores directly into the iterator local.
                            SetTempVariableSlot(methodBody, source, resultSlot);
                        }
                    }

                    methodBody.Instructions[i] = new LIRCopyTemp(source, normalizeForOf.Result);
                    SetTempStorage(methodBody, normalizeForOf.Result, sourceStorage);

                    if (resultSlot >= 0 && resultSlot < methodBody.VariableStorages.Count)
                    {
                        methodBody.VariableStorages[resultSlot] = sourceStorage;
                    }

                    continue;
                }
            }

            // Peephole: ConvertToObject(double/bool) feeding a typed user-class field store.
            // Rewrite the store to use the unboxed source temp directly.
            // This avoids sequences like: double -> box -> ToNumber(object) -> store double.
            if (methodBody.Instructions[i] is not LIRConvertToObject convert)
            {
                continue;
            }

            if (convert.SourceType != typeof(double) && convert.SourceType != typeof(bool))
            {
                continue;
            }

            if (methodBody.Instructions[i + 1] is not LIRStoreUserClassInstanceField store)
            {
                continue;
            }

            if (store.Value.Index != convert.Result.Index)
            {
                continue;
            }

            var fieldClrType = GetDeclaredUserClassFieldClrType(
                classRegistry,
                store.RegistryClassName,
                store.FieldName,
                store.IsPrivateField,
                isStaticField: false);

            if (fieldClrType != convert.SourceType)
            {
                continue;
            }

            // Rewrite: store(Value=objTemp) -> store(Value=sourceTemp)
            methodBody.Instructions[i + 1] = store with { Value = convert.Source };

            // If the boxed temp is not used elsewhere, remove the conversion instruction.
            if (!IsTempUsedOutside(methodBody, convert.Result, ignoreInstructionIndex: i))
            {
                methodBody.Instructions.RemoveAt(i);
                i--; // account for removed instruction
            }
        }

        // After rewrites, some anonymous variable slots (e.g., `$forOf_iter`) may become unused
        // if we coalesce temps onto existing slots. Compact variable slots to avoid emitting
        // unused IL locals and to keep local indices stable/meaningful.
        CompactUnusedVariableSlots(methodBody);
    }

    /// <summary>
    /// Rewrites <see cref="LIRCallIsTruthy"/> to its typed variant when the operand type is statically known,
    /// enabling the IL emitter to select the correct <c>Operators.IsTruthy</c> overload without
    /// inspecting storage at IL-emit time.
    /// </summary>
    private static void SpecializeIsTruthyCalls(MethodBodyIR methodBody)
    {
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (methodBody.Instructions[i] is not LIRCallIsTruthy isTruthy)
            {
                continue;
            }

            var valueStorage = GetTempStorage(methodBody, isTruthy.Value);

            if (valueStorage.Kind == ValueStorageKind.UnboxedValue && valueStorage.ClrType == typeof(double))
            {
                methodBody.Instructions[i] = new LIRCallIsTruthyDouble(isTruthy.Value, isTruthy.Result);
            }
            else if (valueStorage.Kind == ValueStorageKind.UnboxedValue && valueStorage.ClrType == typeof(bool))
            {
                methodBody.Instructions[i] = new LIRCallIsTruthyBool(isTruthy.Value, isTruthy.Result);
            }
            // Otherwise leave as LIRCallIsTruthy (object overload).
        }
    }

    private static void RewriteTypeofFunctionBranchComparisons(MethodBodyIR methodBody)
    {
        var knownTypeofOperands = new Dictionary<int, TempVariable>();
        var knownConstStrings = new Dictionary<int, string>();

        foreach (var instruction in methodBody.Instructions)
        {
            switch (instruction)
            {
                case LIRTypeof typeofInstruction when typeofInstruction.Result.Index >= 0:
                    knownTypeofOperands[typeofInstruction.Result.Index] = typeofInstruction.Value;
                    break;

                case LIRConstString constString when constString.Result.Index >= 0:
                    knownConstStrings[constString.Result.Index] = constString.Value;
                    break;

                case LIRCopyTemp copyTemp:
                    if (copyTemp.Destination.Index >= 0
                        && knownTypeofOperands.TryGetValue(copyTemp.Source.Index, out var typeofOperand))
                    {
                        knownTypeofOperands[copyTemp.Destination.Index] = typeofOperand;
                    }

                    if (copyTemp.Destination.Index >= 0
                        && knownConstStrings.TryGetValue(copyTemp.Source.Index, out var constStringValue))
                    {
                        knownConstStrings[copyTemp.Destination.Index] = constStringValue;
                    }
                    break;
            }
        }

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            bool invertBranches;
            TempVariable typeofSource;
            TempVariable typeofResultTemp;
            TempVariable constStringTemp;
            TempVariable comparisonResult;

            switch (methodBody.Instructions[i])
            {
                case LIRStrictEqualDynamic strictEqual:
                    invertBranches = false;
                    if (!TryMatchTypeofFunctionComparison(strictEqual.Left, strictEqual.Right, knownTypeofOperands, knownConstStrings, out typeofSource, out typeofResultTemp, out constStringTemp))
                    {
                        continue;
                    }
 
                    comparisonResult = strictEqual.Result;
                    break;

                case LIRStrictNotEqualDynamic strictNotEqual:
                    invertBranches = true;
                    if (!TryMatchTypeofFunctionComparison(strictNotEqual.Left, strictNotEqual.Right, knownTypeofOperands, knownConstStrings, out typeofSource, out typeofResultTemp, out constStringTemp))
                    {
                        continue;
                    }

                    comparisonResult = strictNotEqual.Result;
                    break;

                default:
                    continue;
            }

            if (!TryGetBranchOnlyUses(methodBody, comparisonResult, ignoreInstructionIndex: i, out var branchUseIndices))
            {
                continue;
            }

            var isFunctionTemp = CreateTemp(methodBody, new ValueStorage(ValueStorageKind.Reference, typeof(Delegate)));
            methodBody.Instructions[i] = new LIRIsInstanceOf(typeof(Delegate), typeofSource, isFunctionTemp);

            foreach (var branchIndex in branchUseIndices)
            {
                methodBody.Instructions[branchIndex] = RewriteBranchCondition(methodBody.Instructions[branchIndex], isFunctionTemp, invertBranches);
            }

            RemoveDeadDefinitionChain(methodBody, typeofResultTemp);
            RemoveDeadDefinitionChain(methodBody, constStringTemp);
        }
    }

    private static void CompactUnusedVariableSlots(MethodBodyIR methodBody)
    {
        int varCount = methodBody.VariableNames.Count;
        if (varCount == 0)
        {
            return;
        }

        var used = new bool[varCount];
        foreach (var slot in methodBody.TempVariableSlots)
        {
            if (slot >= 0 && slot < varCount)
            {
                used[slot] = true;
            }
        }

        // Fast path: everything is used.
        bool anyUnused = false;
        for (int i = 0; i < used.Length; i++)
        {
            if (!used[i])
            {
                anyUnused = true;
                break;
            }
        }
        if (!anyUnused)
        {
            return;
        }

        var oldToNew = new int[varCount];
        Array.Fill(oldToNew, -1);

        var newNames = new List<string>(varCount);
        var newStorages = new List<ValueStorage>(varCount);
        int next = 0;
        for (int i = 0; i < varCount; i++)
        {
            if (!used[i])
            {
                continue;
            }

            oldToNew[i] = next++;
            newNames.Add(methodBody.VariableNames[i]);
            newStorages.Add(methodBody.VariableStorages[i]);
        }

        // Rewrite temp -> variable slot mappings.
        for (int i = 0; i < methodBody.TempVariableSlots.Count; i++)
        {
            var slot = methodBody.TempVariableSlots[i];
            if (slot >= 0)
            {
                methodBody.TempVariableSlots[i] = oldToNew[slot];
            }
        }

        // Rewrite single-assignment slot set.
        if (methodBody.SingleAssignmentSlots.Count > 0)
        {
            var remapped = new HashSet<int>();
            foreach (var slot in methodBody.SingleAssignmentSlots)
            {
                if (slot >= 0 && slot < oldToNew.Length)
                {
                    var mapped = oldToNew[slot];
                    if (mapped >= 0)
                    {
                        remapped.Add(mapped);
                    }
                }
            }

            methodBody.SingleAssignmentSlots.Clear();
            foreach (var s in remapped)
            {
                methodBody.SingleAssignmentSlots.Add(s);
            }
        }

        // Replace variable slot tables.
        methodBody.VariableNames.Clear();
        methodBody.VariableNames.AddRange(newNames);
        methodBody.VariableStorages.Clear();
        methodBody.VariableStorages.AddRange(newStorages);
    }

    private static ValueStorage GetTempStorage(MethodBodyIR methodBody, TempVariable temp)
    {
        if (temp.Index >= 0 && temp.Index < methodBody.TempStorages.Count)
        {
            return methodBody.TempStorages[temp.Index];
        }

        return new ValueStorage(ValueStorageKind.Unknown);
    }

    private static void SetTempStorage(MethodBodyIR methodBody, TempVariable temp, ValueStorage storage)
    {
        if (temp.Index >= 0 && temp.Index < methodBody.TempStorages.Count)
        {
            methodBody.TempStorages[temp.Index] = storage;
        }
    }

    private static int GetTempVariableSlot(MethodBodyIR methodBody, TempVariable temp)
    {
        if (temp.Index >= 0 && temp.Index < methodBody.TempVariableSlots.Count)
        {
            return methodBody.TempVariableSlots[temp.Index];
        }

        return -1;
    }

    private static void SetTempVariableSlot(MethodBodyIR methodBody, TempVariable temp, int slot)
    {
        if (temp.Index >= 0 && temp.Index < methodBody.TempVariableSlots.Count)
        {
            methodBody.TempVariableSlots[temp.Index] = slot;
        }
    }

    private static bool IsTempUsedOutside(MethodBodyIR methodBody, TempVariable temp, int ignoreInstructionIndex)
    {
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (i == ignoreInstructionIndex)
            {
                continue;
            }

            foreach (var used in EnumerateTemps(methodBody.Instructions[i]))
            {
                if (used.Index == temp.Index)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool TryMatchTypeofFunctionComparison(
        TempVariable left,
        TempVariable right,
        IReadOnlyDictionary<int, TempVariable> knownTypeofOperands,
        IReadOnlyDictionary<int, string> knownConstStrings,
        out TempVariable typeofSource,
        out TempVariable typeofResultTemp,
        out TempVariable constStringTemp)
    {
        if (TryMatchTypeofFunctionOperand(left, right, knownTypeofOperands, knownConstStrings, out typeofSource, out typeofResultTemp, out constStringTemp))
        {
            return true;
        }

        return TryMatchTypeofFunctionOperand(right, left, knownTypeofOperands, knownConstStrings, out typeofSource, out typeofResultTemp, out constStringTemp);
    }

    private static bool TryMatchTypeofFunctionOperand(
        TempVariable typeofCandidate,
        TempVariable stringCandidate,
        IReadOnlyDictionary<int, TempVariable> knownTypeofOperands,
        IReadOnlyDictionary<int, string> knownConstStrings,
        out TempVariable typeofSource,
        out TempVariable typeofResultTemp,
        out TempVariable constStringTemp)
    {
        typeofSource = default;
        typeofResultTemp = default;
        constStringTemp = default;

        if (!knownTypeofOperands.TryGetValue(typeofCandidate.Index, out typeofSource))
        {
            return false;
        }

        if (!knownConstStrings.TryGetValue(stringCandidate.Index, out var constString)
            || !string.Equals(constString, "function", StringComparison.Ordinal))
        {
            return false;
        }

        typeofResultTemp = typeofCandidate;
        constStringTemp = stringCandidate;
        return true;
    }

    private static bool TryGetBranchOnlyUses(MethodBodyIR methodBody, TempVariable temp, int ignoreInstructionIndex, out List<int> branchUseIndices)
    {
        branchUseIndices = new List<int>();

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (i == ignoreInstructionIndex)
            {
                continue;
            }

            var instruction = methodBody.Instructions[i];
            var usesTemp = false;
            foreach (var used in EnumerateTemps(instruction))
            {
                if (used.Index == temp.Index)
                {
                    usesTemp = true;
                    break;
                }
            }

            if (!usesTemp)
            {
                continue;
            }

            if (instruction is LIRBranchIfTrue or LIRBranchIfFalse)
            {
                branchUseIndices.Add(i);
                continue;
            }

            branchUseIndices.Clear();
            return false;
        }

        return branchUseIndices.Count > 0;
    }

    private static LIRInstruction RewriteBranchCondition(LIRInstruction instruction, TempVariable condition, bool invert)
    {
        return instruction switch
        {
            LIRBranchIfTrue branchIfTrue when invert => new LIRBranchIfFalse(condition, branchIfTrue.TargetLabel),
            LIRBranchIfFalse branchIfFalse when invert => new LIRBranchIfTrue(condition, branchIfFalse.TargetLabel),
            LIRBranchIfTrue branchIfTrue => branchIfTrue with { Condition = condition },
            LIRBranchIfFalse branchIfFalse => branchIfFalse with { Condition = condition },
            _ => instruction
        };
    }

    private static TempVariable CreateTemp(MethodBodyIR methodBody, ValueStorage storage)
    {
        var temp = new TempVariable(methodBody.Temps.Count);
        methodBody.Temps.Add(temp);
        methodBody.TempStorages.Add(storage);
        methodBody.TempVariableSlots.Add(-1);
        return temp;
    }

    private static void RemoveDeadDefinitionChain(MethodBodyIR methodBody, TempVariable temp)
    {
        while (TryFindInstructionDefiningTemp(methodBody, temp, out var definitionIndex))
        {
            if (IsTempUsedOutside(methodBody, temp, definitionIndex))
            {
                return;
            }

            var instruction = methodBody.Instructions[definitionIndex];
            TempVariable nextTemp = default;
            bool continueChain = false;

            switch (instruction)
            {
                case LIRCopyTemp copyTemp:
                    nextTemp = copyTemp.Source;
                    continueChain = true;
                    break;

                case LIRTypeof:
                case LIRConstString:
                    break;

                default:
                    return;
            }

            methodBody.Instructions.RemoveAt(definitionIndex);

            if (!continueChain)
            {
                return;
            }

            temp = nextTemp;
        }
    }

    private static bool TryFindInstructionDefiningTemp(MethodBodyIR methodBody, TempVariable temp, out int definitionIndex)
    {
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            switch (methodBody.Instructions[i])
            {
                case LIRTypeof typeofInstruction when typeofInstruction.Result.Index == temp.Index:
                case LIRConstString constString when constString.Result.Index == temp.Index:
                case LIRCopyTemp copyTemp when copyTemp.Destination.Index == temp.Index:
                    definitionIndex = i;
                    return true;
            }
        }

        definitionIndex = -1;
        return false;
    }

    private static IEnumerable<TempVariable> EnumerateTemps(LIRInstruction instruction)
    {
        // Conservative reflection-based enumerator for TempVariable-bearing instruction properties.
        // This avoids having to manually maintain a giant switch over all instruction shapes.
        var type = instruction.GetType();
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            object? value;
            try
            {
                value = prop.GetValue(instruction);
            }
            catch
            {
                continue;
            }

            if (value == null)
            {
                continue;
            }

            if (value is TempVariable tv)
            {
                yield return tv;
                continue;
            }

            if (prop.PropertyType == typeof(TempVariable?))
            {
                var ntv = (TempVariable?)value;
                if (ntv.HasValue)
                {
                    yield return ntv.Value;
                }

                continue;
            }

            if (value is IEnumerable<TempVariable> list)
            {
                foreach (var t in list)
                {
                    yield return t;
                }

                continue;
            }
        }
    }

    private static Type GetDeclaredUserClassFieldClrType(
        ClassRegistry classRegistry,
        string registryClassName,
        string fieldName,
        bool isPrivateField,
        bool isStaticField)
    {
        if (isStaticField)
        {
            return classRegistry.TryGetStaticFieldClrType(registryClassName, fieldName, out var t)
                ? t
                : typeof(object);
        }

        if (isPrivateField)
        {
            return classRegistry.TryGetPrivateFieldClrType(registryClassName, fieldName, out var t)
                ? t
                : typeof(object);
        }

        return classRegistry.TryGetFieldClrType(registryClassName, fieldName, out var t2)
            ? t2
            : typeof(object);
    }
}
