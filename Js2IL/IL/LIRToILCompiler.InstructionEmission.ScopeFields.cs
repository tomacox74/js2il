using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities.Ecma335;
using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    private bool? TryCompileInstructionToIL_ScopeFields(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
            case LIRLoadScopeField loadScopeFieldTemp:
                {
                    if (!IsMaterialized(loadScopeFieldTemp.Result, allocation))
                    {
                        break;
                    }

                    var scopeTypeHandle = ResolveScopeTypeHandle(
                        loadScopeFieldTemp.Scope.Name,
                        "LIRLoadScopeField instruction (castclass)");
                    var fieldHandle = ResolveFieldToken(
                        loadScopeFieldTemp.Field.ScopeName,
                        loadScopeFieldTemp.Field.FieldName,
                        "LIRLoadScopeField instruction");

                    EmitLoadTemp(loadScopeFieldTemp.ScopeInstance, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Castclass);
                    ilEncoder.Token(scopeTypeHandle);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(loadScopeFieldTemp.Field.ScopeName, loadScopeFieldTemp.Field.FieldName);
                    EmitBoxIfNeededForTypedScopeFieldLoad(fieldClrType, GetTempStorage(loadScopeFieldTemp.Result), ilEncoder);
                    EmitStoreTemp(loadScopeFieldTemp.Result, ilEncoder, allocation);
                    break;
                }

            case LIRStoreScopeField storeScopeFieldTemp:
                {
                    var scopeTypeHandle = ResolveScopeTypeHandle(
                        storeScopeFieldTemp.Scope.Name,
                        "LIRStoreScopeField instruction (castclass)");
                    var fieldHandle = ResolveFieldToken(
                        storeScopeFieldTemp.Field.ScopeName,
                        storeScopeFieldTemp.Field.FieldName,
                        "LIRStoreScopeField instruction");

                    EmitLoadTemp(storeScopeFieldTemp.ScopeInstance, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Castclass);
                    ilEncoder.Token(scopeTypeHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(storeScopeFieldTemp.Field.ScopeName, storeScopeFieldTemp.Field.FieldName);
                    if (fieldClrType == typeof(double))
                    {
                        EmitLoadTempAsDouble(storeScopeFieldTemp.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else if (fieldClrType == typeof(bool))
                    {
                        EmitLoadTempAsBoolean(storeScopeFieldTemp.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else if (fieldClrType == typeof(string))
                    {
                        EmitLoadTempAsString(storeScopeFieldTemp.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(storeScopeFieldTemp.Value, ilEncoder, allocation, methodDescriptor);

                        if (fieldClrType != typeof(object) && !fieldClrType.IsValueType)
                        {
                            ilEncoder.OpCode(ILOpCode.Castclass);
                            ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(fieldClrType));
                        }
                    }
                    ilEncoder.OpCode(ILOpCode.Stfld);
                    ilEncoder.Token(fieldHandle);
                    break;
                }

            case LIRLoadLeafScopeField loadLeafField:
                {
                    if (!IsMaterialized(loadLeafField.Result, allocation))
                    {
                        break;
                    }

                    // Emit: ldloc.0 (scope instance), ldfld (field handle)
                    var fieldHandle = ResolveFieldToken(
                        loadLeafField.Field.ScopeName,
                        loadLeafField.Field.FieldName,
                        "LIRLoadLeafScopeField instruction");
                    ilEncoder.LoadLocal(0); // Scope instance is always in local 0
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(loadLeafField.Field.ScopeName, loadLeafField.Field.FieldName);
                    EmitBoxIfNeededForTypedScopeFieldLoad(fieldClrType, GetTempStorage(loadLeafField.Result), ilEncoder);
                    EmitStoreTemp(loadLeafField.Result, ilEncoder, allocation);
                    break;
                }

            case LIRLoadScopeFieldByName loadScopeField:
                {
                    if (!IsMaterialized(loadScopeField.Result, allocation))
                    {
                        break;
                    }

                    var fieldHandle = ResolveFieldToken(
                        loadScopeField.ScopeName,
                        loadScopeField.FieldName,
                        "LIRLoadScopeFieldByName instruction");
                    ilEncoder.LoadLocal(0);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(loadScopeField.ScopeName, loadScopeField.FieldName);
                    EmitBoxIfNeededForTypedScopeFieldLoad(fieldClrType, GetTempStorage(loadScopeField.Result), ilEncoder);
                    EmitStoreTemp(loadScopeField.Result, ilEncoder, allocation);
                    break;
                }

            case LIRStoreLeafScopeField storeLeafField:
                {
                    // Emit: ldloc.0 (scope instance), ldarg/ldloc Value, stfld (field handle)
                    var fieldHandle = ResolveFieldToken(
                        storeLeafField.Field.ScopeName,
                        storeLeafField.Field.FieldName,
                        "LIRStoreLeafScopeField instruction");
                    ilEncoder.LoadLocal(0); // Scope instance is always in local 0

                    var fieldClrType = GetDeclaredScopeFieldClrType(storeLeafField.Field.ScopeName, storeLeafField.Field.FieldName);
                    if (fieldClrType == typeof(double))
                    {
                        EmitLoadTempAsDouble(storeLeafField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else if (fieldClrType == typeof(bool))
                    {
                        EmitLoadTempAsBoolean(storeLeafField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else if (fieldClrType == typeof(string))
                    {
                        EmitLoadTempAsString(storeLeafField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(storeLeafField.Value, ilEncoder, allocation, methodDescriptor);

                        if (fieldClrType != typeof(object) && !fieldClrType.IsValueType)
                        {
                            ilEncoder.OpCode(ILOpCode.Castclass);
                            ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(fieldClrType));
                        }
                    }
                    ilEncoder.OpCode(ILOpCode.Stfld);
                    ilEncoder.Token(fieldHandle);
                    break;
                }

            case LIRStoreScopeFieldByName storeScopeField:
                {
                    var fieldHandle = ResolveFieldToken(
                        storeScopeField.ScopeName,
                        storeScopeField.FieldName,
                        "LIRStoreScopeFieldByName instruction");
                    ilEncoder.LoadLocal(0);

                    var fieldClrType = GetDeclaredScopeFieldClrType(storeScopeField.ScopeName, storeScopeField.FieldName);
                    if (fieldClrType == typeof(double))
                    {
                        EmitLoadTempAsDouble(storeScopeField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else if (fieldClrType == typeof(bool))
                    {
                        EmitLoadTempAsBoolean(storeScopeField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else if (fieldClrType == typeof(string))
                    {
                        EmitLoadTempAsString(storeScopeField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(storeScopeField.Value, ilEncoder, allocation, methodDescriptor);

                        if (fieldClrType != typeof(object) && !fieldClrType.IsValueType)
                        {
                            ilEncoder.OpCode(ILOpCode.Castclass);
                            ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(fieldClrType));
                        }
                    }
                    ilEncoder.OpCode(ILOpCode.Stfld);
                    ilEncoder.Token(fieldHandle);
                    break;
                }

            default:
                return null;
        }

        return true;
    }
}
