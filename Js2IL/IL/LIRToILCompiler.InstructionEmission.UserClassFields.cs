using Js2IL.IR;
using Js2IL.Services;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Services.VariableBindings;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    private bool? TryCompileInstructionToIL_UserClassFields(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
            case LIRStoreUserClassInstanceField storeInstanceField:
                {
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry == null)
                    {
                        return false;
                    }

                    FieldDefinitionHandle fieldHandle;
                    if (storeInstanceField.IsPrivateField)
                    {
                        if (!classRegistry.TryGetPrivateField(storeInstanceField.RegistryClassName, storeInstanceField.FieldName, out fieldHandle))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!classRegistry.TryGetField(storeInstanceField.RegistryClassName, storeInstanceField.FieldName, out fieldHandle))
                        {
                            return false;
                        }
                    }

                    // Instance fields are stored on the runtime `this`.
                    // - In instance methods (class methods/ctors): receiver is IL arg0.
                    // - In static JS callables (functions/arrows): receiver is RuntimeServices.CurrentThis.
                    if (methodDescriptor.IsStatic)
                    {
                        var getThisRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis));
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getThisRef);

                        if (!classRegistry.TryGet(storeInstanceField.RegistryClassName, out var thisTypeHandle))
                        {
                            return false;
                        }

                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(thisTypeHandle);
                    }
                    else
                    {
                        ilEncoder.LoadArgument(0);
                    }
                    var fieldClrType = GetDeclaredUserClassFieldClrType(
                        classRegistry,
                        storeInstanceField.RegistryClassName,
                        storeInstanceField.FieldName,
                        storeInstanceField.IsPrivateField,
                        isStaticField: false);

                    if (fieldClrType == typeof(double))
                    {
                        EmitLoadTempAsDouble(storeInstanceField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else if (fieldClrType == typeof(bool))
                    {
                        EmitLoadTempAsBoolean(storeInstanceField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else if (fieldClrType == typeof(string))
                    {
                        EmitLoadTempAsString(storeInstanceField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(storeInstanceField.Value, ilEncoder, allocation, methodDescriptor);
                    }

                    // If the field is declared as a user class type (not object/string), cast before stfld.
                    // This keeps IL verification correct since `object` is not assignable to a specific class type.
                    if (TryGetDeclaredUserClassFieldTypeHandle(
                        classRegistry,
                        storeInstanceField.RegistryClassName,
                        storeInstanceField.FieldName,
                        storeInstanceField.IsPrivateField,
                        isStaticField: false,
                        out var declaredTypeHandle))
                    {
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(declaredTypeHandle);
                    }
                    else if (fieldClrType != typeof(object)
                        && fieldClrType != typeof(string)
                        && fieldClrType != typeof(double)
                        && fieldClrType != typeof(bool)
                        && !fieldClrType.IsValueType)
                    {
                        // Typed CLR reference field (e.g., JavaScriptRuntime.Int32Array). If the value is currently
                        // flowing as object (common in our temps), cast to keep IL verification correct.
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(fieldClrType));
                    }
                    ilEncoder.OpCode(ILOpCode.Stfld);
                    ilEncoder.Token(fieldHandle);
                    break;
                }

            case LIRStoreUserClassStaticField storeStaticField:
                {
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry == null)
                    {
                        return false;
                    }

                    if (!classRegistry.TryGetStaticField(storeStaticField.RegistryClassName, storeStaticField.FieldName, out var fieldHandle))
                    {
                        return false;
                    }

                    var fieldClrType = GetDeclaredUserClassFieldClrType(
                        classRegistry,
                        storeStaticField.RegistryClassName,
                        storeStaticField.FieldName,
                        isPrivateField: false,
                        isStaticField: true);

                    if (fieldClrType == typeof(double))
                    {
                        EmitLoadTempAsDouble(storeStaticField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else if (fieldClrType == typeof(bool))
                    {
                        EmitLoadTempAsBoolean(storeStaticField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else if (fieldClrType == typeof(string))
                    {
                        EmitLoadTempAsString(storeStaticField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(storeStaticField.Value, ilEncoder, allocation, methodDescriptor);
                    }

                    if (TryGetDeclaredUserClassFieldTypeHandle(
                        classRegistry,
                        storeStaticField.RegistryClassName,
                        storeStaticField.FieldName,
                        isPrivateField: false,
                        isStaticField: true,
                        out var declaredTypeHandle))
                    {
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(declaredTypeHandle);
                    }
                    else if (fieldClrType != typeof(object)
                        && fieldClrType != typeof(string)
                        && fieldClrType != typeof(double)
                        && fieldClrType != typeof(bool)
                        && !fieldClrType.IsValueType)
                    {
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(fieldClrType));
                    }
                    ilEncoder.OpCode(ILOpCode.Stsfld);
                    ilEncoder.Token(fieldHandle);
                    break;
                }

            case LIRLoadUserClassStaticField loadStaticField:
                {
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry == null)
                    {
                        return false;
                    }

                    if (!classRegistry.TryGetStaticField(loadStaticField.RegistryClassName, loadStaticField.FieldName, out var fieldHandle))
                    {
                        return false;
                    }

                    ilEncoder.OpCode(ILOpCode.Ldsfld);
                    ilEncoder.Token(fieldHandle);

                    if (IsMaterialized(loadStaticField.Result, allocation))
                    {
                        EmitStoreTemp(loadStaticField.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }

                    break;
                }

            case LIRLoadUserClassInstanceField loadInstanceField:
                {
                    if (!IsMaterialized(loadInstanceField.Result, allocation))
                    {
                        // This temp will be emitted inline at its use site (EmitLoadTemp).
                        break;
                    }

                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry == null)
                    {
                        return false;
                    }

                    FieldDefinitionHandle fieldHandle;
                    if (loadInstanceField.IsPrivateField)
                    {
                        if (!classRegistry.TryGetPrivateField(loadInstanceField.RegistryClassName, loadInstanceField.FieldName, out fieldHandle))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!classRegistry.TryGetField(loadInstanceField.RegistryClassName, loadInstanceField.FieldName, out fieldHandle))
                        {
                            return false;
                        }
                    }

                    // Instance fields are loaded from the runtime `this`.
                    // - In instance methods (class methods/ctors): receiver is IL arg0.
                    // - In static JS callables (functions/arrows): receiver is RuntimeServices.CurrentThis.
                    if (methodDescriptor.IsStatic)
                    {
                        var getThisRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis));
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getThisRef);

                        if (!classRegistry.TryGet(loadInstanceField.RegistryClassName, out var thisTypeHandle))
                        {
                            return false;
                        }

                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(thisTypeHandle);
                    }
                    else
                    {
                        ilEncoder.LoadArgument(0);
                    }
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredUserClassFieldClrType(
                        classRegistry,
                        loadInstanceField.RegistryClassName,
                        loadInstanceField.FieldName,
                        loadInstanceField.IsPrivateField,
                        isStaticField: false);
                    EmitBoxIfNeededForTypedUserClassFieldLoad(fieldClrType, GetTempStorage(loadInstanceField.Result), ilEncoder);

                    EmitStoreTemp(loadInstanceField.Result, ilEncoder, allocation);

                    break;
                }

            default:
                return null;
        }

        return true;
    }
}
