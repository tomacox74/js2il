using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities.Ecma335;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    private bool? TryCompileInstructionToIL_Scopes(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
            case LIRCreateScopeInstance createScopeTemp:
                {
                    if (!IsMaterialized(createScopeTemp.Result, allocation))
                    {
                        break;
                    }

                    var scopeTypeHandle = ResolveScopeTypeHandle(
                        createScopeTemp.Scope.Name,
                        "LIRCreateScopeInstance instruction");
                    var ctorRef = GetScopeConstructorRef(scopeTypeHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(ctorRef);
                    EmitStoreTemp(createScopeTemp.Result, ilEncoder, allocation);
                    break;
                }

            case LIRLoadParentScopeField loadParentField:
                {
                    if (!IsMaterialized(loadParentField.Result, allocation))
                    {
                        break;
                    }

                    // Emit IL to load parent scope field
                    // For static methods with scopes parameter: ldarg.0 (scopes array)
                    // For instance methods: ldarg.0 (this), ldfld _scopes
                    var scopeTypeHandle = ResolveScopeTypeHandle(
                        loadParentField.Scope.Name,
                        "LIRLoadParentScopeField instruction (castclass)");
                    var fieldHandle = ResolveFieldToken(
                        loadParentField.Field.ScopeName,
                        loadParentField.Field.FieldName,
                        "LIRLoadParentScopeField instruction");
                    EmitLoadScopesArray(ilEncoder, methodDescriptor);
                    ilEncoder.LoadConstantI4(loadParentField.ParentScopeIndex);
                    ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                    ilEncoder.OpCode(ILOpCode.Castclass);
                    ilEncoder.Token(scopeTypeHandle);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(loadParentField.Field.ScopeName, loadParentField.Field.FieldName);
                    EmitBoxIfNeededForTypedScopeFieldLoad(fieldClrType, GetTempStorage(loadParentField.Result), ilEncoder);
                    EmitStoreTemp(loadParentField.Result, ilEncoder, allocation);
                    break;
                }

            case LIRStoreParentScopeField storeParentField:
                {
                    // Emit IL to store to parent scope field
                    // For static methods with scopes parameter: ldarg.0 (scopes array)
                    // For instance methods: ldarg.0 (this), ldfld _scopes
                    var scopeTypeHandle = ResolveScopeTypeHandle(
                        storeParentField.Scope.Name,
                        "LIRStoreParentScopeField instruction (castclass)");
                    var fieldHandle = ResolveFieldToken(
                        storeParentField.Field.ScopeName,
                        storeParentField.Field.FieldName,
                        "LIRStoreParentScopeField instruction");
                    EmitLoadScopesArray(ilEncoder, methodDescriptor);
                    ilEncoder.LoadConstantI4(storeParentField.ParentScopeIndex);
                    ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                    ilEncoder.OpCode(ILOpCode.Castclass);
                    ilEncoder.Token(scopeTypeHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(storeParentField.Field.ScopeName, storeParentField.Field.FieldName);
                    if (fieldClrType == typeof(double))
                    {
                        EmitLoadTempAsDouble(storeParentField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else if (fieldClrType == typeof(bool))
                    {
                        EmitLoadTempAsBoolean(storeParentField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else if (fieldClrType == typeof(string))
                    {
                        EmitLoadTempAsString(storeParentField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(storeParentField.Value, ilEncoder, allocation, methodDescriptor);

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
