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
