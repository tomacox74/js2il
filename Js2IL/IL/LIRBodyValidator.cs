using Js2IL.IR;

namespace Js2IL.IL;

/// <summary>
/// Validates invariants on a lowered <see cref="MethodBodyIR"/> before IL emission.
/// Catches lowering bugs early with actionable diagnostics instead of silent mis-compilation.
/// </summary>
/// <remarks>
/// Two categories of invariants are checked:
/// <list type="bullet">
///   <item><description>
///     <b>Slot materialization</b>: every temp tagged with a variable slot
///     (<see cref="MethodBodyIR.TempVariableSlots"/><c>[i] &gt;= 0</c>) must be the result of at least
///     one instruction in the method body.  A slot with no defining instruction would produce an
///     uninitialized-local read at runtime.
///   </description></item>
///   <item><description>
///     <b>Numeric lowering</b>: instructions that emit raw IL arithmetic (<c>add</c>, <c>sub</c>,
///     <c>mul</c>, <c>div</c>, <c>rem</c>, <c>neg</c>) and native numeric comparison instructions
///     require their operand temps to be proven unboxed <see cref="double"/> values
///     (<see cref="ValueStorageKind.UnboxedValue"/> with <c>ClrType == typeof(double)</c>).
///     Operands that are not already unboxed doubles must first be converted via
///     <c>EnsureNumber()</c> before being fed to these instructions.
///   </description></item>
/// </list>
/// Registered in the pipeline via <c>#if DEBUG</c> guards in
/// <see cref="HIRToLIRLowerer.TryLower"/>.
/// </remarks>
internal static class LIRBodyValidator
{
    /// <summary>
    /// Validates all supported invariants on the supplied method body.
    /// Throws <see cref="InvalidOperationException"/> with a diagnostic message on the first violation.
    /// </summary>
    internal static void Validate(MethodBodyIR methodBody)
    {
        ValidateSlotMaterialization(methodBody);
        ValidateNumericLowering(methodBody);
    }

    // -----------------------------------------------------------------------
    // Slot materialization
    // -----------------------------------------------------------------------

    /// <summary>
    /// Checks that every temp tagged with a variable slot has at least one defining
    /// instruction in the method body.  A "tag-only" slot mapping (no defining instruction)
    /// would cause the corresponding IL local to be read uninitialized.
    /// </summary>
    private static void ValidateSlotMaterialization(MethodBodyIR methodBody)
    {
        if (methodBody.TempVariableSlots.Count == 0)
        {
            return;
        }

        // Build the set of temps that are defined by at least one instruction.
        var definedTemps = new HashSet<int>(capacity: methodBody.Instructions.Count);
        foreach (var instr in methodBody.Instructions)
        {
            if (TryGetDefinedTempExtended(instr, out var defined) && defined.Index >= 0)
            {
                definedTemps.Add(defined.Index);
            }
        }

        // Every slot-tagged temp must appear in the defined set.
        for (int i = 0; i < methodBody.TempVariableSlots.Count; i++)
        {
            if (methodBody.TempVariableSlots[i] >= 0 && !definedTemps.Contains(i))
            {
                throw new InvalidOperationException(
                    $"LIR slot materialization invariant violated: temp {i} is mapped to " +
                    $"variable slot {methodBody.TempVariableSlots[i]} but has no defining " +
                    "instruction in the method body. This is a tag-only slot mapping that " +
                    "will cause an uninitialized local read at runtime. " +
                    "Use EnsureTempMappedToSlot() instead of calling SetTempVariableSlot() directly " +
                    "on a temp that has no defining instruction.");
            }
        }
    }

    // -----------------------------------------------------------------------
    // Numeric lowering
    // -----------------------------------------------------------------------

    /// <summary>
    /// Checks that native arithmetic and numeric comparison instructions only receive
    /// operand temps that are proven to be unboxed doubles.
    /// </summary>
    private static void ValidateNumericLowering(MethodBodyIR methodBody)
    {
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            switch (methodBody.Instructions[i])
            {
                case LIRAddNumber op:
                    AssertUnboxedDouble(methodBody, op.Left, i, nameof(LIRAddNumber), "Left");
                    AssertUnboxedDouble(methodBody, op.Right, i, nameof(LIRAddNumber), "Right");
                    break;
                case LIRSubNumber op:
                    AssertUnboxedDouble(methodBody, op.Left, i, nameof(LIRSubNumber), "Left");
                    AssertUnboxedDouble(methodBody, op.Right, i, nameof(LIRSubNumber), "Right");
                    break;
                case LIRMulNumber op:
                    AssertUnboxedDouble(methodBody, op.Left, i, nameof(LIRMulNumber), "Left");
                    AssertUnboxedDouble(methodBody, op.Right, i, nameof(LIRMulNumber), "Right");
                    break;
                case LIRDivNumber op:
                    AssertUnboxedDouble(methodBody, op.Left, i, nameof(LIRDivNumber), "Left");
                    AssertUnboxedDouble(methodBody, op.Right, i, nameof(LIRDivNumber), "Right");
                    break;
                case LIRModNumber op:
                    AssertUnboxedDouble(methodBody, op.Left, i, nameof(LIRModNumber), "Left");
                    AssertUnboxedDouble(methodBody, op.Right, i, nameof(LIRModNumber), "Right");
                    break;
                case LIRExpNumber op:
                    AssertUnboxedDouble(methodBody, op.Left, i, nameof(LIRExpNumber), "Left");
                    AssertUnboxedDouble(methodBody, op.Right, i, nameof(LIRExpNumber), "Right");
                    break;
                case LIRNegateNumber op:
                    AssertUnboxedDouble(methodBody, op.Value, i, nameof(LIRNegateNumber), "Value");
                    break;
                case LIRCompareNumberLessThan op:
                    AssertUnboxedDouble(methodBody, op.Left, i, nameof(LIRCompareNumberLessThan), "Left");
                    AssertUnboxedDouble(methodBody, op.Right, i, nameof(LIRCompareNumberLessThan), "Right");
                    break;
                case LIRCompareNumberGreaterThan op:
                    AssertUnboxedDouble(methodBody, op.Left, i, nameof(LIRCompareNumberGreaterThan), "Left");
                    AssertUnboxedDouble(methodBody, op.Right, i, nameof(LIRCompareNumberGreaterThan), "Right");
                    break;
                case LIRCompareNumberLessThanOrEqual op:
                    AssertUnboxedDouble(methodBody, op.Left, i, nameof(LIRCompareNumberLessThanOrEqual), "Left");
                    AssertUnboxedDouble(methodBody, op.Right, i, nameof(LIRCompareNumberLessThanOrEqual), "Right");
                    break;
                case LIRCompareNumberGreaterThanOrEqual op:
                    AssertUnboxedDouble(methodBody, op.Left, i, nameof(LIRCompareNumberGreaterThanOrEqual), "Left");
                    AssertUnboxedDouble(methodBody, op.Right, i, nameof(LIRCompareNumberGreaterThanOrEqual), "Right");
                    break;
                case LIRCompareNumberEqual op:
                    AssertUnboxedDouble(methodBody, op.Left, i, nameof(LIRCompareNumberEqual), "Left");
                    AssertUnboxedDouble(methodBody, op.Right, i, nameof(LIRCompareNumberEqual), "Right");
                    break;
                case LIRCompareNumberNotEqual op:
                    AssertUnboxedDouble(methodBody, op.Left, i, nameof(LIRCompareNumberNotEqual), "Left");
                    AssertUnboxedDouble(methodBody, op.Right, i, nameof(LIRCompareNumberNotEqual), "Right");
                    break;
            }
        }
    }

    private static void AssertUnboxedDouble(
        MethodBodyIR methodBody,
        TempVariable temp,
        int instrIndex,
        string instrType,
        string operandName)
    {
        var storage = GetTempStorage(methodBody, temp);
        if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(double))
        {
            return;
        }

        throw new InvalidOperationException(
            $"LIR numeric lowering invariant violated at instruction #{instrIndex} ({instrType}): " +
            $"operand '{operandName}' (temp {temp.Index}) must be a proven unboxed double, " +
            $"but has storage Kind={storage.Kind}, ClrType={storage.ClrType?.Name ?? "null"}. " +
            "Pass the operand through EnsureNumber() before using it in a native numeric instruction.");
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static ValueStorage GetTempStorage(MethodBodyIR methodBody, TempVariable temp)
    {
        if (temp.Index >= 0 && temp.Index < methodBody.TempStorages.Count)
        {
            return methodBody.TempStorages[temp.Index];
        }
        return new ValueStorage(ValueStorageKind.Unknown);
    }

    /// <summary>
    /// Like <see cref="TempLocalAllocator.TryGetDefinedTemp"/> but also covers instruction types
    /// that define result temps without being listed in the allocator's switch
    /// (e.g., <see cref="LIRStoreException"/> and <see cref="LIRUnwrapCatchException"/>
    /// which populate their result from the CLR evaluation stack rather than via a prior LIR operand).
    /// </summary>
    private static bool TryGetDefinedTempExtended(LIRInstruction instruction, out TempVariable defined)
    {
        // Handle instructions that define a temp but are not in TempLocalAllocator.TryGetDefinedTemp.
        if (instruction is LIRStoreException storeEx)
        {
            defined = storeEx.Result;
            return true;
        }

        if (instruction is LIRUnwrapCatchException unwrap)
        {
            defined = unwrap.Result;
            return true;
        }

        return TempLocalAllocator.TryGetDefinedTemp(instruction, out defined);
    }
}
