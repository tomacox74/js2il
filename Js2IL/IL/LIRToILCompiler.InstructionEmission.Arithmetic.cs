using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Services.VariableBindings;
using Js2IL.Utilities.Ecma335;
using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    private bool? TryCompileInstructionToIL_Arithmetic(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
            case LIRAddNumber addNumber:
                TryEmitStackValueInstruction(addNumber, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(addNumber.Result, ilEncoder, allocation);
                break;
            case LIRConcatStrings concatStrings:
                if (!IsMaterialized(concatStrings.Result, allocation))
                {
                    // Stackify will re-emit concat inline at the single use site.
                    break;
                }
                TryEmitStackValueInstruction(concatStrings, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(concatStrings.Result, ilEncoder, allocation);
                break;
            case LIRAddDynamic addDynamic:
                EmitLoadTemp(addDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(addDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsAddObjectObject(ilEncoder);
                EmitStoreTemp(addDynamic.Result, ilEncoder, allocation);
                break;
            case LIRAddDynamicDoubleObject addDynamicDoubleObject:
                EmitLoadTempAsDouble(addDynamicDoubleObject.LeftDouble, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsObject(addDynamicDoubleObject.RightObject, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsAddDoubleObject(ilEncoder);
                EmitStoreTemp(addDynamicDoubleObject.Result, ilEncoder, allocation);
                break;
            case LIRAddDynamicObjectDouble addDynamicObjectDouble:
                EmitLoadTempAsObject(addDynamicObjectDouble.LeftObject, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsDouble(addDynamicObjectDouble.RightDouble, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsAddObjectDouble(ilEncoder);
                EmitStoreTemp(addDynamicObjectDouble.Result, ilEncoder, allocation);
                break;
            case LIRSubNumber subNumber:
                TryEmitStackValueInstruction(subNumber, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(subNumber.Result, ilEncoder, allocation);
                break;
            case LIRMulNumber mulNumber:
                TryEmitStackValueInstruction(mulNumber, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(mulNumber.Result, ilEncoder, allocation);
                break;
            case LIRMulDynamic mulDynamic:
                EmitLoadTemp(mulDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(mulDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsMultiply(ilEncoder);
                EmitStoreTemp(mulDynamic.Result, ilEncoder, allocation);
                break;
            case LIRConstNumber constNumber:
                if (!IsMaterialized(constNumber.Result, allocation))
                {
                    break;
                }
                TryEmitStackValueInstruction(constNumber, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(constNumber.Result, ilEncoder, allocation);
                break;
            case LIRConstString constString:
                if (!IsMaterialized(constString.Result, allocation))
                {
                    break;
                }
                TryEmitStackValueInstruction(constString, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(constString.Result, ilEncoder, allocation);
                break;
            case LIRConstBoolean constBoolean:
                if (!IsMaterialized(constBoolean.Result, allocation))
                {
                    break;
                }
                TryEmitStackValueInstruction(constBoolean, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(constBoolean.Result, ilEncoder, allocation);
                break;
            case LIRConstUndefined:
                if (!IsMaterialized(((LIRConstUndefined)instruction).Result, allocation))
                {
                    break;
                }
                TryEmitStackValueInstruction((LIRConstUndefined)instruction, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(((LIRConstUndefined)instruction).Result, ilEncoder, allocation);
                break;
            case LIRConstNull:
                if (!IsMaterialized(((LIRConstNull)instruction).Result, allocation))
                {
                    break;
                }
                TryEmitStackValueInstruction((LIRConstNull)instruction, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(((LIRConstNull)instruction).Result, ilEncoder, allocation);
                break;

            case LIRConvertToObject convertToObject:
                if (!IsMaterialized(convertToObject.Result, allocation))
                {
                    break;
                }

                TryEmitStackValueInstruction(convertToObject, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(convertToObject.Result, ilEncoder, allocation);
                break;

            case LIRConvertToNumber convertToNumber:
                if (!IsMaterialized(convertToNumber.Result, allocation))
                {
                    break;
                }

                TryEmitStackValueInstruction(convertToNumber, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(convertToNumber.Result, ilEncoder, allocation);
                break;

            case LIRConvertToBoolean convertToBoolean:
                if (!IsMaterialized(convertToBoolean.Result, allocation))
                {
                    break;
                }

                TryEmitStackValueInstruction(convertToBoolean, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(convertToBoolean.Result, ilEncoder, allocation);
                break;

            case LIRConvertToString convertToString:
                if (!IsMaterialized(convertToString.Result, allocation))
                {
                    break;
                }

                TryEmitStackValueInstruction(convertToString, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(convertToString.Result, ilEncoder, allocation);
                break;
            case LIRTypeof:
                if (!IsMaterialized(((LIRTypeof)instruction).Result, allocation))
                {
                    break;
                }
                EmitLoadTemp(((LIRTypeof)instruction).Value, ilEncoder, allocation, methodDescriptor);
                var typeofMref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.Typeof));
                ilEncoder.OpCode(ILOpCode.Call);
                ilEncoder.Token(typeofMref);
                EmitStoreTemp(((LIRTypeof)instruction).Result, ilEncoder, allocation);
                break;
            case LIRNegateNumber:
                if (!IsMaterialized(((LIRNegateNumber)instruction).Result, allocation))
                {
                    break;
                }

                EmitLoadTemp(((LIRNegateNumber)instruction).Value, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Neg);
                EmitStoreTemp(((LIRNegateNumber)instruction).Result, ilEncoder, allocation);
                break;
            case LIRBitwiseNotNumber:
                if (!IsMaterialized(((LIRBitwiseNotNumber)instruction).Result, allocation))
                {
                    break;
                }

                // JS bitwise ops perform ToInt32(ToNumber(x)).
                // Do not apply conv.i4 directly to object refs (it becomes a pointer cast).
                EmitLoadTempAsNumber(((LIRBitwiseNotNumber)instruction).Value, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Not);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                EmitStoreTemp(((LIRBitwiseNotNumber)instruction).Result, ilEncoder, allocation);
                break;
            case LIRLogicalNot logicalNot:
                if (!IsMaterialized(logicalNot.Result, allocation))
                {
                    break;
                }

                // Emit JS ToBoolean(value) and then invert.
                EmitConvertToBooleanCore(logicalNot.Value, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(logicalNot.Result, ilEncoder, allocation);
                break;

            case LIRIsInstanceOf isInstanceOf:
                if (!IsMaterialized(isInstanceOf.Result, allocation))
                {
                    break;
                }

                EmitLoadTempAsObject(isInstanceOf.Value, ilEncoder, allocation, methodDescriptor);
                {
                    var targetType = _typeReferenceRegistry.GetOrAdd(isInstanceOf.TargetType);
                    ilEncoder.OpCode(ILOpCode.Isinst);
                    ilEncoder.Token(targetType);
                }
                EmitStoreTemp(isInstanceOf.Result, ilEncoder, allocation);
                break;
            case LIRCompareNumberLessThan cmpLt:
                if (!IsMaterialized(cmpLt.Result, allocation))
                {
                    break;
                }
                EmitLoadTempAsNumber(cmpLt.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsNumber(cmpLt.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Clt);
                EmitStoreTemp(cmpLt.Result, ilEncoder, allocation);
                break;
            case LIRCompareNumberGreaterThan cmpGt:
                if (!IsMaterialized(cmpGt.Result, allocation))
                {
                    break;
                }
                EmitLoadTempAsNumber(cmpGt.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsNumber(cmpGt.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Cgt);
                EmitStoreTemp(cmpGt.Result, ilEncoder, allocation);
                break;
            case LIRCompareNumberLessThanOrEqual cmpLe:
                if (!IsMaterialized(cmpLe.Result, allocation))
                {
                    break;
                }
                EmitLoadTempAsNumber(cmpLe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsNumber(cmpLe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Cgt);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpLe.Result, ilEncoder, allocation);
                break;
            case LIRCompareNumberGreaterThanOrEqual cmpGe:
                if (!IsMaterialized(cmpGe.Result, allocation))
                {
                    break;
                }
                EmitLoadTempAsNumber(cmpGe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsNumber(cmpGe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Clt);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpGe.Result, ilEncoder, allocation);
                break;
            case LIRCompareNumberEqual cmpEq:
                if (!IsMaterialized(cmpEq.Result, allocation))
                {
                    break;
                }
                EmitLoadTempAsNumber(cmpEq.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsNumber(cmpEq.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpEq.Result, ilEncoder, allocation);
                break;
            case LIRCompareNumberNotEqual cmpNe:
                if (!IsMaterialized(cmpNe.Result, allocation))
                {
                    break;
                }
                EmitLoadTempAsNumber(cmpNe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsNumber(cmpNe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpNe.Result, ilEncoder, allocation);
                break;
            case LIRCompareBooleanEqual cmpBoolEq:
                if (!IsMaterialized(cmpBoolEq.Result, allocation))
                {
                    break;
                }
                EmitLoadTemp(cmpBoolEq.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpBoolEq.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpBoolEq.Result, ilEncoder, allocation);
                break;
            case LIRCompareBooleanNotEqual cmpBoolNe:
                if (!IsMaterialized(cmpBoolNe.Result, allocation))
                {
                    break;
                }
                EmitLoadTemp(cmpBoolNe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpBoolNe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpBoolNe.Result, ilEncoder, allocation);
                break;
            
            // Division
            case LIRDivNumber divNumber:
                TryEmitStackValueInstruction(divNumber, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(divNumber.Result, ilEncoder, allocation);
                break;

            // Remainder (modulo)
            case LIRModNumber modNumber:
                TryEmitStackValueInstruction(modNumber, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(modNumber.Result, ilEncoder, allocation);
                break;

            // Exponentiation (Math.Pow)
            case LIRExpNumber expNumber:
                TryEmitStackValueInstruction(expNumber, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(expNumber.Result, ilEncoder, allocation);
                break;

            // Bitwise AND: convert to int32, and, convert back to double
            case LIRBitwiseAnd bitwiseAnd:
                EmitLoadTempAsNumber(bitwiseAnd.Left, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                EmitLoadTempAsNumber(bitwiseAnd.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.And);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                EmitStoreTemp(bitwiseAnd.Result, ilEncoder, allocation);
                break;

            // Bitwise OR: convert to int32, or, convert back to double
            case LIRBitwiseOr bitwiseOr:
                EmitLoadTempAsNumber(bitwiseOr.Left, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                EmitLoadTempAsNumber(bitwiseOr.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Or);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                EmitStoreTemp(bitwiseOr.Result, ilEncoder, allocation);
                break;

            // Bitwise XOR: convert to int32, xor, convert back to double
            case LIRBitwiseXor bitwiseXor:
                EmitLoadTempAsNumber(bitwiseXor.Left, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                EmitLoadTempAsNumber(bitwiseXor.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Xor);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                EmitStoreTemp(bitwiseXor.Result, ilEncoder, allocation);
                break;

            // Left shift: convert to int32, shift, convert back to double
            case LIRLeftShift leftShift:
                EmitLoadTempAsNumber(leftShift.Left, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                EmitLoadTempAsNumber(leftShift.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Shl);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                EmitStoreTemp(leftShift.Result, ilEncoder, allocation);
                break;

            // Right shift (signed): convert to int32, shift, convert back to double
            case LIRRightShift rightShift:
                EmitLoadTempAsNumber(rightShift.Left, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                EmitLoadTempAsNumber(rightShift.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Shr);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                EmitStoreTemp(rightShift.Result, ilEncoder, allocation);
                break;

            // Unsigned right shift: convert to int32 (to preserve negative values), reinterpret as uint32, shift, convert back to double
            case LIRUnsignedRightShift unsignedRightShift:
                EmitLoadTempAsNumber(unsignedRightShift.Left, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);  // Convert to int32 first (handles negatives)
                ilEncoder.OpCode(ILOpCode.Conv_u4);  // Then reinterpret as uint32 (no value change, just type)
                EmitLoadTempAsNumber(unsignedRightShift.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Shr_un);
                ilEncoder.OpCode(ILOpCode.Conv_r_un); // Convert unsigned to double
                EmitStoreTemp(unsignedRightShift.Result, ilEncoder, allocation);
                break;

            // Call Operators.IsTruthy
            case LIRCallIsTruthy callIsTruthy:
                if (!IsMaterialized(callIsTruthy.Result, allocation))
                {
                    break;
                }
                var truthyInputStorage = GetTempStorage(callIsTruthy.Value);
                EmitLoadTemp(callIsTruthy.Value, ilEncoder, allocation, methodDescriptor);

                if (truthyInputStorage.Kind == ValueStorageKind.UnboxedValue && truthyInputStorage.ClrType == typeof(double))
                {
                    EmitOperatorsIsTruthyDouble(ilEncoder);
                }
                else if (truthyInputStorage.Kind == ValueStorageKind.UnboxedValue && truthyInputStorage.ClrType == typeof(bool))
                {
                    EmitOperatorsIsTruthyBool(ilEncoder);
                }
                else
                {
                    EmitOperatorsIsTruthyObject(ilEncoder);
                }
                EmitStoreTemp(callIsTruthy.Result, ilEncoder, allocation);
                break;

            default:
                return null;
        }

        return true;
    }
}
