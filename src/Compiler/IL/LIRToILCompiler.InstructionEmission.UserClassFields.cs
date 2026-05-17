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
    private void EmitStoreInstanceFieldValue(
        LIRStoreUserClassInstanceField storeInstanceField,
        Type fieldClrType,
        ClassRegistry classRegistry,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
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
            ilEncoder.OpCode(ILOpCode.Castclass);
            ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(fieldClrType));
        }
    }

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

                    var fieldClrType = GetDeclaredUserClassFieldClrType(
                        classRegistry,
                        storeInstanceField.RegistryClassName,
                        storeInstanceField.FieldName,
                        storeInstanceField.IsPrivateField,
                        isStaticField: false);

                    if (methodDescriptor.IsDerivedConstructor && !storeInstanceField.IsPrivateField)
                    {
                        if (!classRegistry.TryGet(storeInstanceField.RegistryClassName, out var classTypeHandle))
                        {
                            return false;
                        }

                        var directStore = ilEncoder.DefineLabel();
                        var done = ilEncoder.DefineLabel();

                        var getThisRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis));
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getThisRef);

                        var resolveThisRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.ResolveLexicalThis),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(resolveThisRef);

                        ilEncoder.OpCode(ILOpCode.Dup);
                        ilEncoder.LoadArgument(0);
                        ilEncoder.Branch(ILOpCode.Beq, directStore);

                        ilEncoder.Ldstr(_metadataBuilder, storeInstanceField.FieldName);
                        EmitLoadTempAsObject(storeInstanceField.Value, ilEncoder, allocation, methodDescriptor);
                        var defineClassField = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.ObjectRuntime),
                            nameof(JavaScriptRuntime.ObjectRuntime.DefineClassFieldDataProperty),
                            parameterTypes: new[] { typeof(object), typeof(string), typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(defineClassField);
                        ilEncoder.OpCode(ILOpCode.Pop);
                        ilEncoder.Branch(ILOpCode.Br, done);

                        ilEncoder.MarkLabel(directStore);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(classTypeHandle);
                        EmitStoreInstanceFieldValue(storeInstanceField, fieldClrType, classRegistry, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Stfld);
                        ilEncoder.Token(fieldHandle);

                        ilEncoder.MarkLabel(done);
                        break;
                    }

                    // Instance fields are stored on the runtime `this`.
                    // - In instance methods (class methods/ctors): receiver is IL arg0.
                    // - In static JS callables (functions/arrows): use ObjectRuntime.SetItem because
                    //   `this` may be a ClassConstructorValue (the class itself, e.g. for static accessors),
                    //   not a CLR instance, so castclass+stfld would fail.
                    if (methodDescriptor.IsStatic)
                    {
                        var getThisRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis));
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getThisRef);

                        ilEncoder.Ldstr(_metadataBuilder, storeInstanceField.FieldName);
                        EmitLoadTempAsObject(storeInstanceField.Value, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.LoadConstantI4(0); // non-strict
                        var setItemMethod = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.ObjectRuntime),
                            nameof(JavaScriptRuntime.ObjectRuntime.SetItem),
                            parameterTypes: new[] { typeof(object), typeof(string), typeof(object), typeof(bool) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(setItemMethod);
                        ilEncoder.OpCode(ILOpCode.Pop); // pop SetItem's return value
                        break;
                    }

                    ilEncoder.LoadArgument(0);

                    EmitStoreInstanceFieldValue(storeInstanceField, fieldClrType, classRegistry, ilEncoder, allocation, methodDescriptor);
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

                    var fieldClrType = GetDeclaredUserClassFieldClrType(
                        classRegistry,
                        loadInstanceField.RegistryClassName,
                        loadInstanceField.FieldName,
                        loadInstanceField.IsPrivateField,
                        isStaticField: false);

                    // Instance fields are loaded from the runtime `this`.
                    // - In instance methods (class methods/ctors): receiver is IL arg0.
                    // - In static JS callables (functions/arrows): use runtime helpers because
                    //   `this` may be a ClassConstructorValue (the class itself, e.g. for static accessors),
                    //   not a CLR instance, so castclass+ldfld would fail.
                    if (methodDescriptor.IsStatic)
                    {
                        var getThisRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis));
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getThisRef);

                        ilEncoder.Ldstr(_metadataBuilder, loadInstanceField.FieldName);
                        if (fieldClrType == typeof(double))
                        {
                            var getItemAsNumberMethod = _memberRefRegistry.GetOrAddMethod(
                                typeof(JavaScriptRuntime.ObjectRuntime),
                                nameof(JavaScriptRuntime.ObjectRuntime.GetItemAsNumber),
                                parameterTypes: new[] { typeof(object), typeof(string) });
                            ilEncoder.OpCode(ILOpCode.Call);
                            ilEncoder.Token(getItemAsNumberMethod);
                            EmitBoxIfNeededForTypedUserClassFieldLoad(fieldClrType, GetTempStorage(loadInstanceField.Result), ilEncoder);
                        }
                        else
                        {
                            var getItemMethod = _memberRefRegistry.GetOrAddMethod(
                                typeof(JavaScriptRuntime.ObjectRuntime),
                                nameof(JavaScriptRuntime.ObjectRuntime.GetItem),
                                parameterTypes: new[] { typeof(object), typeof(string) });
                            ilEncoder.OpCode(ILOpCode.Call);
                            ilEncoder.Token(getItemMethod);
                        }
                        EmitStoreTemp(loadInstanceField.Result, ilEncoder, allocation);
                        break;
                    }

                    ilEncoder.LoadArgument(0);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);
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
