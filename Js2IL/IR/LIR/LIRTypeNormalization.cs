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

        for (int i = 0; i < methodBody.Instructions.Count - 1; i++)
        {
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
