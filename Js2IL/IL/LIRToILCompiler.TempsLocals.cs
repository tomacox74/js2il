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
    #region Temp/Local Variable Management

    private void EmitLocalType(SignatureTypeEncoder typeEncoder, ValueStorage storage, bool allowUnboxedJsNull)
    {
        if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(bool))
        {
            typeEncoder.Boolean();
            return;
        }

        if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(double))
        {
            typeEncoder.Double();
            return;
        }

        if (allowUnboxedJsNull && storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(JavaScriptRuntime.JsNull))
        {
            var typeRef = _typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsNull));
            typeEncoder.Type(typeRef, false);
            return;
        }

        if (storage.Kind == ValueStorageKind.Reference && storage.ClrType == typeof(string))
        {
            typeEncoder.String();
            return;
        }

        if (storage.Kind == ValueStorageKind.Reference
            && storage.ClrType != null
            && storage.ClrType.IsArray
            && storage.ClrType.GetElementType() == typeof(object))
        {
            typeEncoder.SZArray().Object();
            return;
        }

        if (storage.Kind == ValueStorageKind.Reference && !string.IsNullOrWhiteSpace(storage.ScopeName))
        {
            var scopeTypeHandle = ResolveScopeTypeHandle(storage.ScopeName!, "local variable signature creation (scope instance local)");
            typeEncoder.Type(scopeTypeHandle, false);
            return;
        }

        if (storage.Kind == ValueStorageKind.Reference && !storage.TypeHandle.IsNil)
        {
            // Preserve known reference types represented only by metadata handles
            // (e.g., user-defined JS classes compiled as TypeDefinitionHandles).
            typeEncoder.Type(storage.TypeHandle, false);
            return;
        }

        if (storage.Kind == ValueStorageKind.Reference && storage.ClrType != null && storage.ClrType != typeof(object))
        {
            // Preserve known runtime reference types for declared variables (e.g., JavaScriptRuntime.Array)
            // so later lowering/emission can take advantage of typed locals.
            var typeRef = _typeReferenceRegistry.GetOrAdd(storage.ClrType);
            typeEncoder.Type(typeRef, false);
            return;
        }

        typeEncoder.Object();
    }

    private StandaloneSignatureHandle CreateLocalVariablesSignature(TempLocalAllocation allocation)
    {
        int varCount = MethodBody.VariableNames.Count;
        int tempLocals = allocation.SlotStorages.Count;
        bool hasLeafScope = MethodBody.NeedsLeafScopeLocal && !MethodBody.LeafScopeId.IsNil;
        
        // If we need a leaf scope local, it goes in slot 0
        int scopeLocalCount = hasLeafScope ? 1 : 0;
        int totalLocals = scopeLocalCount + varCount + tempLocals;

        if (totalLocals == 0)
        {
            return default;
        }

        var localSig = new BlobBuilder();
        var localEncoder = new BlobEncoder(localSig).LocalVariableSignature(totalLocals);

        // Local 0: Scope instance (if needed)
        if (hasLeafScope)
        {
            var typeEncoder = localEncoder.AddVariable().Type();
            var leafScopeTypeHandle = ResolveScopeTypeHandle(
                MethodBody.LeafScopeId.Name,
                "local variable signature creation (leaf scope local)");
            typeEncoder.Type(leafScopeTypeHandle, false);
        }

        // Variable locals (shifted by scope local count)
        for (int i = 0; i < varCount; i++)
        {
            var storage = MethodBody.VariableStorages[i];
            EmitLocalType(localEncoder.AddVariable().Type(), storage, allowUnboxedJsNull: false);
        }

        // Then temp locals
        for (int i = 0; i < allocation.SlotStorages.Count; i++)
        {
            var storage = allocation.SlotStorages[i];
            EmitLocalType(localEncoder.AddVariable().Type(), storage, allowUnboxedJsNull: true);
        }

        var signature = _metadataBuilder.AddStandaloneSignature(_metadataBuilder.GetOrAddBlob(localSig));
        return signature;
    }

    private void EmitLoadTemp(TempVariable temp, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        // Check if materialized - if so, load from local
        if (IsMaterialized(temp, allocation))
        {
            var slot = GetSlotForTemp(temp, allocation);
            ilEncoder.LoadLocal(slot);
            return;
        }

        // Not materialized - try to emit inline
        var def = TryFindDefInstruction(temp);
        if (def == null)
        {
            // Try to include a little more context: which instruction attempted to use this temp.
            // This typically indicates a lowering/SSA bug where a temp is referenced without being defined.
            var firstUse = MethodBody.Instructions
                .Select((instr, idx) => (instr, idx))
                .FirstOrDefault(t => TempLocalAllocator.EnumerateUsedTemps(t.instr).Any(u => u == temp));

            if (firstUse.instr != null)
            {
                throw new InvalidOperationException(
                    $"Cannot emit unmaterialized temp {temp.Index} - no definition found. " +
                    $"First use at instruction #{firstUse.idx}: {firstUse.instr.GetType().Name}");
            }

            throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - no definition found (and no uses found in method body?)");
        }

        // Emit the constant/expression inline
        if (TryEmitStackValueInstruction(def, ilEncoder, allocation, methodDescriptor))
        {
            return;
        }

        switch (def)
        {
            case LIRCreateScopeInstance createScope:
                {
                    var scopeTypeHandle = ResolveScopeTypeHandle(
                        createScope.Scope.Name,
                        "inline LIRCreateScopeInstance emission");
                    var ctorRef = GetScopeConstructorRef(scopeTypeHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(ctorRef);
                    break;
                }
            case LIRNewBuiltInError newError:
                {
                    var errorClrType = Js2IL.IR.BuiltInErrorTypes.GetRuntimeErrorClrType(newError.ErrorTypeName);

                    if (newError.Message.HasValue)
                    {
                        EmitLoadTempAsObject(newError.Message.Value, ilEncoder, allocation, methodDescriptor);
                        var toString = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.DotNet2JSConversions),
                            nameof(JavaScriptRuntime.DotNet2JSConversions.ToString),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(toString);
                        var ctor = _memberRefRegistry.GetOrAddConstructor(errorClrType, parameterTypes: new[] { typeof(string) });
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(ctor);
                        break;
                    }

                    var defaultCtor = _memberRefRegistry.GetOrAddConstructor(errorClrType, parameterTypes: Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(defaultCtor);
                    break;
                }
            case LIRCompareNumberLessThan cmpLt:
                EmitLoadTemp(cmpLt.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpLt.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Clt);
                break;
            case LIRCompareNumberGreaterThan cmpGt:
                EmitLoadTemp(cmpGt.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpGt.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Cgt);
                break;
            case LIRCompareNumberLessThanOrEqual cmpLe:
                EmitLoadTemp(cmpLe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpLe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Cgt);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareNumberGreaterThanOrEqual cmpGe:
                EmitLoadTemp(cmpGe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpGe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Clt);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareNumberEqual cmpEq:
                EmitLoadTemp(cmpEq.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpEq.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareNumberNotEqual cmpNe:
                EmitLoadTemp(cmpNe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpNe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareBooleanEqual cmpBoolEq:
                EmitLoadTemp(cmpBoolEq.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpBoolEq.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareBooleanNotEqual cmpBoolNe:
                EmitLoadTemp(cmpBoolNe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpBoolNe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;

            case LIRNewIntrinsicObject newIntrinsic:
                {
                    EmitNewIntrinsicObjectCore(newIntrinsic, ilEncoder, allocation, methodDescriptor);
                    break;
                }

            case LIRNewUserClass newUserClass:
                {
                    // Emit inline user-defined class construction (newobj) with optional scopes array.
                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    if (reader == null)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing ICallableDeclarationReader");
                    }

                    if (!reader.TryGetDeclaredToken(newUserClass.ConstructorCallableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing declared constructor token for class {newUserClass.ClassName}");
                    }

                    var ctorDef = (MethodDefinitionHandle)token;

                    int argc = newUserClass.Arguments.Count;
                    if (argc < newUserClass.MinArgCount || argc > newUserClass.MaxArgCount)
                    {
                        var expectedMinArgs = newUserClass.MinArgCount;
                        var expectedMaxArgs = newUserClass.MaxArgCount;

                        if (expectedMinArgs == expectedMaxArgs)
                        {
                            ILEmitHelpers.ThrowNotSupported(
                                $"Constructor for class '{newUserClass.ClassName}' expects {expectedMinArgs} argument(s) but call site has {argc}.");
                        }

                        ILEmitHelpers.ThrowNotSupported(
                            $"Constructor for class '{newUserClass.ClassName}' expects {expectedMinArgs}-{expectedMaxArgs} argument(s) but call site has {argc}.");
                    }

                    if (newUserClass.NeedsScopes)
                    {
                        if (newUserClass.ScopesArray is not { } scopesTemp)
                        {
                            throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing scopes array temp for class {newUserClass.ClassName}");
                        }
                        EmitLoadTemp(scopesTemp, ilEncoder, allocation, methodDescriptor);
                    }

                    foreach (var arg in newUserClass.Arguments)
                    {
                        EmitLoadTemp(arg, ilEncoder, allocation, methodDescriptor);
                    }

                    int paddingNeeded = newUserClass.MaxArgCount - argc;
                    for (int i = 0; i < paddingNeeded; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(ctorDef);
                    // Result stays on stack
                    break;
                }

            case LIRLoadUserClassStaticField loadStaticField:
                {
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry == null)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing ClassRegistry for static field load {loadStaticField.RegistryClassName}::{loadStaticField.FieldName}");
                    }

                    if (!classRegistry.TryGetStaticField(loadStaticField.RegistryClassName, loadStaticField.FieldName, out var fieldHandle))
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing registered static field {loadStaticField.RegistryClassName}::{loadStaticField.FieldName}");
                    }

                    ilEncoder.OpCode(ILOpCode.Ldsfld);
                    ilEncoder.Token(fieldHandle);
                    break;
                }

            case LIRMulDynamic mulDynamic:
                // Emit inline dynamic multiplication
                EmitLoadTemp(mulDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(mulDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsMultiply(ilEncoder);
                break;
            case LIRAddDynamic addDynamic:
                // Emit inline dynamic addition
                EmitLoadTemp(addDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(addDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsAddObjectObject(ilEncoder);
                break;
            case LIRAddDynamicDoubleObject addDynamicDoubleObject:
                // Mixed dynamic addition: left is unboxed double, right is boxed object
                EmitLoadTemp(addDynamicDoubleObject.LeftDouble, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(addDynamicDoubleObject.RightObject, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsAddDoubleObject(ilEncoder);
                break;
            case LIRAddDynamicObjectDouble addDynamicObjectDouble:
                // Mixed dynamic addition: left is boxed object, right is unboxed double
                EmitLoadTemp(addDynamicObjectDouble.LeftObject, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(addDynamicObjectDouble.RightDouble, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsAddObjectDouble(ilEncoder);
                break;
            case LIRLoadLeafScopeField loadLeafField:
                // Emit inline: ldloc.0 (scope instance), ldfld (field handle)
                {
                    var fieldHandle = ResolveFieldToken(
                        loadLeafField.Field.ScopeName,
                        loadLeafField.Field.FieldName,
                        "inline LIRLoadLeafScopeField emission");
                    ilEncoder.LoadLocal(0);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(loadLeafField.Field.ScopeName, loadLeafField.Field.FieldName);
                    EmitBoxIfNeededForTypedScopeFieldLoad(fieldClrType, GetTempStorage(temp), ilEncoder);
                }
                break;
            case LIRLoadScopeFieldByName loadScopeField:
                // Emit inline: ldloc.0 (scope instance), ldfld (field handle)
                {
                    var fieldHandle = ResolveFieldToken(
                        loadScopeField.ScopeName,
                        loadScopeField.FieldName,
                        "inline LIRLoadScopeFieldByName emission");
                    ilEncoder.LoadLocal(0);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(loadScopeField.ScopeName, loadScopeField.FieldName);
                    EmitBoxIfNeededForTypedScopeFieldLoad(fieldClrType, GetTempStorage(temp), ilEncoder);
                }
                break;
            case LIRLoadParentScopeField loadParentField:
                // Emit inline: load scopes array, index, cast, ldfld
                {
                    var scopeTypeHandle = ResolveScopeTypeHandle(
                        loadParentField.Scope.Name,
                        "inline LIRLoadParentScopeField emission (castclass)");
                    var fieldHandle = ResolveFieldToken(
                        loadParentField.Field.ScopeName,
                        loadParentField.Field.FieldName,
                        "inline LIRLoadParentScopeField emission");
                    EmitLoadScopesArray(ilEncoder, methodDescriptor);
                    ilEncoder.LoadConstantI4(loadParentField.ParentScopeIndex);
                    ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                    ilEncoder.OpCode(ILOpCode.Castclass);
                    ilEncoder.Token(scopeTypeHandle);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(loadParentField.Field.ScopeName, loadParentField.Field.FieldName);
                    EmitBoxIfNeededForTypedScopeFieldLoad(fieldClrType, GetTempStorage(temp), ilEncoder);
                }
                break;
            case LIRLoadScopeField loadScopeFieldTemp:
                // Emit inline: load scope temp, cast, ldfld
                {
                    var scopeTypeHandle = ResolveScopeTypeHandle(
                        loadScopeFieldTemp.Scope.Name,
                        "inline LIRLoadScopeField emission (castclass)");
                    var fieldHandle = ResolveFieldToken(
                        loadScopeFieldTemp.Field.ScopeName,
                        loadScopeFieldTemp.Field.FieldName,
                        "inline LIRLoadScopeField emission");

                    EmitLoadTemp(loadScopeFieldTemp.ScopeInstance, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Castclass);
                    ilEncoder.Token(scopeTypeHandle);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(loadScopeFieldTemp.Field.ScopeName, loadScopeFieldTemp.Field.FieldName);
                    EmitBoxIfNeededForTypedScopeFieldLoad(fieldClrType, GetTempStorage(temp), ilEncoder);
                }
                break;
            case LIRGetIntrinsicGlobal getIntrinsicGlobal:
                // Emit inline: call IntrinsicObjectRegistry.GetOrDefault
                EmitLoadIntrinsicGlobalVariable(getIntrinsicGlobal.Name, ilEncoder);
                break;
            case LIRBuildScopesArray buildScopes:
                // Emit inline: create scopes array with scope instances
                if (buildScopes.Slots.Count == 0)
                {
                    EmitEmptyScopesArray(ilEncoder);
                }
                else
                {
                    EmitPopulateScopesArray(ilEncoder, buildScopes.Slots, methodDescriptor, allocation);
                }
                // Array reference stays on stack
                break;
            case LIRBitwiseNotNumber bitwiseNot:
                // Emit inline: load value, convert to int, bitwise not, convert back to double
                EmitLoadTemp(bitwiseNot.Value, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Not);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                break;
            case LIRLogicalNot logicalNot:
                // Emit inline: load as object, call ToBoolean, invert
                EmitLoadTempAsObject(logicalNot.Value, ilEncoder, allocation, methodDescriptor);
                {
                    var toBooleanMref = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.TypeUtilities),
                        nameof(JavaScriptRuntime.TypeUtilities.ToBoolean),
                        parameterTypes: new[] { typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(toBooleanMref);
                }
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRTypeof typeofInstr:
                // Emit inline: load value, call TypeUtilities.Typeof
                EmitLoadTemp(typeofInstr.Value, ilEncoder, allocation, methodDescriptor);
                var typeofMref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.Typeof));
                ilEncoder.OpCode(ILOpCode.Call);
                ilEncoder.Token(typeofMref);
                break;
            case LIRBuildArray buildArray:
                // Emit inline array construction using dup pattern
                ilEncoder.LoadConstantI4(buildArray.Elements.Count);
                ilEncoder.OpCode(ILOpCode.Newarr);
                ilEncoder.Token(_bclReferences.ObjectType);
                for (int i = 0; i < buildArray.Elements.Count; i++)
                {
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.LoadConstantI4(i);
                    EmitLoadTempAsObject(buildArray.Elements[i], ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Stelem_ref);
                }
                // Array reference stays on stack
                break;
            case LIRNewJsArray newJsArray:
                {
                    // Emit inline JavaScriptRuntime.Array construction using dup pattern
                    ilEncoder.LoadConstantI4(newJsArray.Elements.Count);
                    var arrayCtor = _memberRefRegistry.GetOrAddConstructor(
                        typeof(JavaScriptRuntime.Array),
                        parameterTypes: new[] { typeof(int) });
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(arrayCtor);

                    // For each element: dup, load element value, callvirt Add
                    var addMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(System.Collections.Generic.List<object>),
                        nameof(System.Collections.Generic.List<object>.Add));
                    for (int i = 0; i < newJsArray.Elements.Count; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Dup);
                        EmitLoadTemp(newJsArray.Elements[i], ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Callvirt);
                        ilEncoder.Token(addMethod);
                    }
                    // Array reference stays on stack
                }
                break;
            case LIRNewJsObject newJsObject:
                {
                    // Emit inline ExpandoObject construction
                    var expandoCtor = _memberRefRegistry.GetOrAddConstructor(
                        typeof(System.Dynamic.ExpandoObject),
                        parameterTypes: Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(expandoCtor);

                    // For each property: dup, ldstr key, load value, callvirt IDictionary.set_Item
                    var setItemMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(System.Collections.Generic.IDictionary<string, object>),
                        "set_Item");
                    foreach (var prop in newJsObject.Properties)
                    {
                        ilEncoder.OpCode(ILOpCode.Dup);
                        ilEncoder.Ldstr(_metadataBuilder, prop.Key);
                        EmitLoadTemp(prop.Value, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Callvirt);
                        ilEncoder.Token(setItemMethod);
                    }
                    // Object reference stays on stack
                }
                break;
            case LIRGetLength getLength:
                // Emit inline: call JavaScriptRuntime.Object.GetLength(object)
                EmitLoadTempAsObject(getLength.Object, ilEncoder, allocation, methodDescriptor);
                {
                    var getLengthMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.GetLength),
                        parameterTypes: new[] { typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(getLengthMethod);
                }
                break;

            case LIRGetJsArrayLength getJsArrayLength:
                {
                    // Inline: receiver, callvirt get_Count, conv.r8
                    var receiverStorage = GetTempStorage(getJsArrayLength.Receiver);
                    if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Array))
                    {
                        EmitLoadTemp(getJsArrayLength.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(getJsArrayLength.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Array)));
                    }

                    var getCountMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(System.Collections.Generic.List<object>),
                        "get_Count",
                        parameterTypes: Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(getCountMethod);
                    ilEncoder.OpCode(ILOpCode.Conv_r8);
                    break;
                }

            case LIRGetInt32ArrayLength getInt32ArrayLength:
                {
                    // Inline: receiver, callvirt get_length
                    var receiverStorage = GetTempStorage(getInt32ArrayLength.Receiver);
                    if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Int32Array))
                    {
                        EmitLoadTemp(getInt32ArrayLength.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(getInt32ArrayLength.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Int32Array)));
                    }

                    var getLengthMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Int32Array),
                        "get_length",
                        parameterTypes: Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(getLengthMethod);
                    break;
                }
            case LIRGetItem getItem:
                {
                    var indexStorage = GetTempStorage(getItem.Index);
                    var resultStorage = GetTempStorage(getItem.Result);
                    if (indexStorage.Kind == ValueStorageKind.UnboxedValue && indexStorage.ClrType == typeof(double))
                    {
                        // Emit inline: call JavaScriptRuntime.Object.GetItem(object, double)
                        EmitLoadTempAsObject(getItem.Object, ilEncoder, allocation, methodDescriptor);
                        EmitLoadTemp(getItem.Index, ilEncoder, allocation, methodDescriptor);
                        var getItemMethod = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Object),
                            nameof(JavaScriptRuntime.Object.GetItem),
                            parameterTypes: new[] { typeof(object), typeof(double) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getItemMethod);

                        // If the temp storage expects an unboxed double, coerce the object result to a number.
                        if (resultStorage.Kind == ValueStorageKind.UnboxedValue && resultStorage.ClrType == typeof(double))
                        {
                            var toNumberMref = _memberRefRegistry.GetOrAddMethod(
                                typeof(JavaScriptRuntime.TypeUtilities),
                                nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                                parameterTypes: new[] { typeof(object) });
                            ilEncoder.OpCode(ILOpCode.Call);
                            ilEncoder.Token(toNumberMref);
                        }
                    }
                    else
                    {
                        // Emit inline: call JavaScriptRuntime.Object.GetItem(object, object)
                        EmitLoadTempAsObject(getItem.Object, ilEncoder, allocation, methodDescriptor);
                        EmitLoadTempAsObject(getItem.Index, ilEncoder, allocation, methodDescriptor);
                        var getItemMethod = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Object),
                            nameof(JavaScriptRuntime.Object.GetItem),
                            parameterTypes: new[] { typeof(object), typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getItemMethod);

                        // If the temp storage expects an unboxed double, coerce the object result to a number.
                        if (resultStorage.Kind == ValueStorageKind.UnboxedValue && resultStorage.ClrType == typeof(double))
                        {
                            var toNumberMref = _memberRefRegistry.GetOrAddMethod(
                                typeof(JavaScriptRuntime.TypeUtilities),
                                nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                                parameterTypes: new[] { typeof(object) });
                            ilEncoder.OpCode(ILOpCode.Call);
                            ilEncoder.Token(toNumberMref);
                        }
                    }
                }
                break;

            case LIRGetJsArrayElement getArray:
                {
                    // Inline: receiver, index, callvirt Array.get_Item(double)
                    var receiverStorage = GetTempStorage(getArray.Receiver);
                    if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Array))
                    {
                        EmitLoadTemp(getArray.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(getArray.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Array)));
                    }

                    EmitLoadTemp(getArray.Index, ilEncoder, allocation, methodDescriptor);

                    var arrayGetter = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Array),
                        "get_Item",
                        parameterTypes: new[] { typeof(double) });
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(arrayGetter);

                    // If the temp storage expects an unboxed double, coerce the object result to a number.
                    var resultStorage = GetTempStorage(getArray.Result);
                    if (resultStorage.Kind == ValueStorageKind.UnboxedValue && resultStorage.ClrType == typeof(double))
                    {
                        var toNumberMref = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.TypeUtilities),
                            nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(toNumberMref);
                    }

                    break;
                }

            case LIRGetInt32ArrayElement getI32:
                {
                    // Inline: receiver, index, callvirt get_Item(double)
                    var receiverStorage = GetTempStorage(getI32.Receiver);
                    if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Int32Array))
                    {
                        EmitLoadTemp(getI32.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(getI32.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Int32Array)));
                    }

                    EmitLoadTemp(getI32.Index, ilEncoder, allocation, methodDescriptor);

                    var int32ArrayGetter = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Int32Array),
                        "get_Item",
                        parameterTypes: new[] { typeof(double) });
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(int32ArrayGetter);

                    // Leave as double on stack; caller will box if it needs object.
                    break;
                }
            case LIRCallIntrinsic callIntrinsic:
                // Emit inline intrinsic call (e.g., console.log)
                EmitLoadTemp(callIntrinsic.IntrinsicObject, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(callIntrinsic.ArgumentsArray, ilEncoder, allocation, methodDescriptor);
                EmitInvokeIntrinsicMethod(typeof(JavaScriptRuntime.Console), callIntrinsic.Name, ilEncoder);
                // Result stays on stack (caller will handle it)
                break;

            case LIRCallIntrinsicGlobalFunction callGlobalFunc:
                EmitIntrinsicGlobalFunctionCallInline(callGlobalFunc, ilEncoder, allocation, methodDescriptor);
                // Result stays on stack
                break;
            case LIRCallInstanceMethod callInstance:
                // Emit inline instance method call - result stays on stack
                EmitInstanceMethodCallInline(callInstance, ilEncoder, allocation, methodDescriptor);
                break;
            case LIRCallIntrinsicStatic callIntrinsicStatic:
                // Emit inline intrinsic static call (e.g., Array.isArray)
                // We reuse the main EmitIntrinsicStaticCall but need to handle unmaterialized result
                EmitIntrinsicStaticCallInline(callIntrinsicStatic, ilEncoder, allocation, methodDescriptor);
                // Result stays on stack (caller will handle it)
                break;
            case LIRNegateNumber negateNumber:
                // Emit inline: load value, negate
                EmitLoadTemp(negateNumber.Value, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Neg);
                break;
            case LIRCallFunction callFunc:
                {
                    if (callFunc.CallableId is not { } callableId)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing CallableId for LIRCallFunction");
                    }

                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    if (reader == null || !reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing declared token for callable {callableId.DisplayName}");
                    }

                    var methodHandle = (MethodDefinitionHandle)token;

                    // IMPORTANT: use the callee's declared parameter count, not the call-site argument count.
                    // The call-site may omit args (default parameters), but the delegate signature must match
                    // the target method signature, otherwise the JIT can crash the process.
                    int jsParamCount = callableId.JsParamCount;
                    int argsToPass = Math.Min(callFunc.Arguments.Count, jsParamCount);

                    // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                    // Load scopes array
                    EmitLoadTemp(callFunc.ScopesArray, ilEncoder, allocation, methodDescriptor);

                    // Load all arguments
                    for (int i = 0; i < argsToPass; i++)
                    {
                        EmitLoadTemp(callFunc.Arguments[i], ilEncoder, allocation, methodDescriptor);
                    }

                    // Pad missing parameters with null (supports default parameter initialization).
                    for (int i = argsToPass; i < jsParamCount; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    // Invoke: callvirt Func<object[], [object, ...], object>::Invoke
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(_bclReferences.GetFuncInvokeRef(jsParamCount));
                    // Result stays on stack
                    break;
                }

            case LIRCallFunctionValue callValue:
                {
                    // Inline emission uses the same lowering as the main pass for calls.
                    EmitLoadTemp(callValue.FunctionValue, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(callValue.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(callValue.ArgumentsArray, ilEncoder, allocation, methodDescriptor);

                    var invokeRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Closure),
                        nameof(JavaScriptRuntime.Closure.InvokeWithArgs),
                        new[] { typeof(object), typeof(object[]), typeof(object[]) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(invokeRef);

                    if (IsMaterialized(callValue.Result, allocation))
                    {
                        EmitStoreTemp(callValue.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }

            case LIRCallMember callMember:
                {
                    // Inline emission of member call via runtime dispatcher.
                    EmitLoadTempAsObject(callMember.Receiver, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.Ldstr(_metadataBuilder, callMember.MethodName);
                    EmitLoadTemp(callMember.ArgumentsArray, ilEncoder, allocation, methodDescriptor);

                    var callMemberRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.CallMember),
                        new[] { typeof(object), typeof(string), typeof(object[]) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(callMemberRef);
                    break;
                }

            case LIRCallTypedMember callTyped:
                {
                    // Inline the typed call (no fallback) and leave the result on the stack.
                    EmitCallTypedMemberNoFallbackCore(callTyped, ilEncoder, allocation, methodDescriptor);
                    EmitBoxIfNeededForTypedCallResult(temp, callTyped.ReturnClrType, ilEncoder);
                    break;
                }

            case LIRCallTypedMemberWithFallback callTypedFallback:
                {
                    EmitCallTypedMemberWithFallback(callTypedFallback, ilEncoder, allocation, methodDescriptor);
                    break;
                }

            case LIRCallUserClassInstanceMethod callUserClass:
                {
                    if (callUserClass.MethodHandle.IsNil)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing method token for '{callUserClass.RegistryClassName}.{callUserClass.MethodName}'");
                    }

                    ilEncoder.OpCode(ILOpCode.Ldarg_0);

                    if (callUserClass.HasScopesParameter)
                    {
                        EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
                    }

                    int jsParamCount = callUserClass.MaxParamCount;
                    int argsToPass = Math.Min(callUserClass.Arguments.Count, jsParamCount);
                    for (int i = 0; i < argsToPass; i++)
                    {
                        EmitLoadTemp(callUserClass.Arguments[i], ilEncoder, allocation, methodDescriptor);
                    }

                    for (int i = argsToPass; i < jsParamCount; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(callUserClass.MethodHandle);
                    break;
                }

            case LIRCallUserClassBaseConstructor callBaseCtor:
                {
                    if (callBaseCtor.ConstructorHandle.IsNil)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized base constructor call for '{callBaseCtor.BaseRegistryClassName}' - missing ctor token");
                    }

                    ilEncoder.OpCode(ILOpCode.Ldarg_0);

                    if (callBaseCtor.HasScopesParameter)
                    {
                        EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
                    }

                    int jsParamCount = callBaseCtor.MaxParamCount;
                    int argsToPass = Math.Min(callBaseCtor.Arguments.Count, jsParamCount);
                    for (int i = 0; i < argsToPass; i++)
                    {
                        EmitLoadTemp(callBaseCtor.Arguments[i], ilEncoder, allocation, methodDescriptor);
                    }

                    for (int i = argsToPass; i < jsParamCount; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(callBaseCtor.ConstructorHandle);
                    break;
                }

            case LIRCallIntrinsicBaseConstructor callIntrinsicBaseCtor:
                {
                    if (!string.Equals(callIntrinsicBaseCtor.IntrinsicName, "Array", StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException($"Unsupported intrinsic base constructor: '{callIntrinsicBaseCtor.IntrinsicName}'");
                    }

                    ilEncoder.OpCode(ILOpCode.Ldarg_0);
                    var baseCtor = _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.Array), System.Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(baseCtor);

                    ilEncoder.OpCode(ILOpCode.Ldarg_0);

                    int argc = callIntrinsicBaseCtor.Arguments.Count;
                    ilEncoder.LoadConstantI4(argc);
                    ilEncoder.OpCode(ILOpCode.Newarr);
                    ilEncoder.Token(_bclReferences.ObjectType);

                    for (int i = 0; i < argc; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Dup);
                        ilEncoder.LoadConstantI4(i);
                        EmitLoadTempAsObject(callIntrinsicBaseCtor.Arguments[i], ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Stelem_ref);
                    }

                    var initRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Array),
                        nameof(JavaScriptRuntime.Array.ConstructInto),
                        new[] { typeof(object[]) });

                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(initRef);
                    break;
                }

            case LIRCallUserClassBaseInstanceMethod callBaseMethod:
                {
                    if (callBaseMethod.MethodHandle.IsNil)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized base method call for '{callBaseMethod.BaseRegistryClassName}.{callBaseMethod.MethodName}' - missing method token");
                    }

                    ilEncoder.OpCode(ILOpCode.Ldarg_0);

                    if (callBaseMethod.HasScopesParameter)
                    {
                        EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
                    }

                    int jsParamCount = callBaseMethod.MaxParamCount;
                    int argsToPass = Math.Min(callBaseMethod.Arguments.Count, jsParamCount);
                    for (int i = 0; i < argsToPass; i++)
                    {
                        EmitLoadTemp(callBaseMethod.Arguments[i], ilEncoder, allocation, methodDescriptor);
                    }

                    for (int i = argsToPass; i < jsParamCount; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(callBaseMethod.MethodHandle);
                    break;
                }

            case LIRCallDeclaredCallable callDeclared:
                {
                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    if (reader == null || !reader.TryGetDeclaredToken(callDeclared.CallableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing declared token for callable {callDeclared.CallableId.DisplayName}");
                    }

                    var methodHandle = (MethodDefinitionHandle)token;

                    foreach (var arg in callDeclared.Arguments)
                    {
                        EmitLoadTemp(arg, ilEncoder, allocation, methodDescriptor);
                    }

                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(methodHandle);
                    // Result stays on stack
                    break;
                }

            case LIRCreateBoundArrowFunction createArrow:
                {
                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    var callableId = createArrow.CallableId;
                    if (reader == null || !reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing declared token for callable {callableId.DisplayName}");
                    }

                    var methodHandle = (MethodDefinitionHandle)token;
                    int jsParamCount = createArrow.CallableId.JsParamCount;

                    // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                    // Bind delegate to scopes array: Closure.Bind(object, object[])
                    EmitLoadTemp(createArrow.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Call);
                    var bindRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), nameof(JavaScriptRuntime.Closure.Bind), new[] { typeof(object), typeof(object[]) });
                    ilEncoder.Token(bindRef);
                    // Result stays on stack
                    break;
                }

            case LIRCreateBoundFunctionExpression createFunc:
                {
                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    var callableId = createFunc.CallableId;
                    if (reader == null || !reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing declared token for callable {callableId.DisplayName}");
                    }

                    var methodHandle = (MethodDefinitionHandle)token;
                    int jsParamCount = createFunc.CallableId.JsParamCount;

                    // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                    // Bind delegate to scopes array: Closure.Bind(object, object[])
                    EmitLoadTemp(createFunc.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Call);
                    var bindRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), nameof(JavaScriptRuntime.Closure.Bind), new[] { typeof(object), typeof(object[]) });
                    ilEncoder.Token(bindRef);
                    // Result stays on stack
                    break;
                }

            case LIRLoadUserClassInstanceField loadInstanceField:
                {
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry == null)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - ClassRegistry service missing");
                    }

                    FieldDefinitionHandle fieldHandle;
                    if (loadInstanceField.IsPrivateField)
                    {
                        if (!classRegistry.TryGetPrivateField(loadInstanceField.RegistryClassName, loadInstanceField.FieldName, out fieldHandle))
                        {
                            throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing private field '{loadInstanceField.FieldName}' on '{loadInstanceField.RegistryClassName}'");
                        }
                    }
                    else
                    {
                        if (!classRegistry.TryGetField(loadInstanceField.RegistryClassName, loadInstanceField.FieldName, out fieldHandle))
                        {
                            throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing field '{loadInstanceField.FieldName}' on '{loadInstanceField.RegistryClassName}'");
                        }
                    }

                    // Inline instance-field load from runtime `this`.
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
                            throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing class '{loadInstanceField.RegistryClassName}' for this-cast");
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
                    EmitBoxIfNeededForTypedUserClassFieldLoad(fieldClrType, GetTempStorage(temp), ilEncoder);
                    break;
                }

            case LIRIsInstanceOf isInstanceOf:
                {
                    // Inline: <value as object>; isinst <TargetType>
                    EmitLoadTempAsObject(isInstanceOf.Value, ilEncoder, allocation, methodDescriptor);
                    var targetType = _typeReferenceRegistry.GetOrAdd(isInstanceOf.TargetType);
                    ilEncoder.OpCode(ILOpCode.Isinst);
                    ilEncoder.Token(targetType);
                    break;
                }
            default:
                throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - unsupported instruction {def.GetType().Name}");
        }
    }

    private bool TryEmitStackValueInstruction(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
            case LIRConstNumber constNum:
                ilEncoder.LoadConstantR8(constNum.Value);
                return true;
            case LIRConstString constStr:
                ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString(constStr.Value));
                return true;
            case LIRConstBoolean constBool:
                ilEncoder.LoadConstantI4(constBool.Value ? 1 : 0);
                return true;
            case LIRConstUndefined:
                ilEncoder.OpCode(ILOpCode.Ldnull);
                return true;
            case LIRConstNull:
                ilEncoder.LoadConstantI4((int)JavaScriptRuntime.JsNull.Null);
                return true;
            case LIRLoadThis:
                if (methodDescriptor.IsStatic)
                {
                    var getThisRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.RuntimeServices),
                        nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis));
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(getThisRef);
                    return true;
                }
                ilEncoder.LoadArgument(0);
                return true;
            case LIRLoadScopesArgument:
                if (!methodDescriptor.HasScopesParameter)
                {
                    throw new InvalidOperationException("Cannot emit scopes argument when method has no scopes parameter");
                }
                // Static functions: scopes is arg0. Instance constructors: scopes is arg1.
                ilEncoder.LoadArgument(methodDescriptor.IsStatic ? 0 : 1);
                return true;
            case LIRLoadParameter loadParam:
                // Emit ldarg.X inline - no local slot needed
                int ilArgIndex = GetIlArgIndexForJsParameter(methodDescriptor, loadParam.ParameterIndex);
                ilEncoder.LoadArgument(ilArgIndex);
                return true;
            case LIRConvertToObject convertToObject:
                EmitLoadTemp(convertToObject.Source, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Box);
                if (convertToObject.SourceType == typeof(bool))
                {
                    ilEncoder.Token(_bclReferences.BooleanType);
                }
                else if (convertToObject.SourceType == typeof(JavaScriptRuntime.JsNull))
                {
                    ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsNull)));
                }
                else
                {
                    ilEncoder.Token(_bclReferences.DoubleType);
                }
                return true;

            case LIRConvertToNumber convertToNumber:
                // If already an unboxed numeric value, skip boxing + ToNumber.
                // In this compiler pipeline, the only non-numeric unboxed values are bool and JsNull.
                if (GetTempStorage(convertToNumber.Source) is { Kind: ValueStorageKind.UnboxedValue, ClrType: var clrType }
                    && clrType != typeof(bool)
                    && clrType != typeof(JavaScriptRuntime.JsNull))
                {
                    EmitLoadTemp(convertToNumber.Source, ilEncoder, allocation, methodDescriptor);
                    return true;
                }

                EmitLoadTempAsObject(convertToNumber.Source, ilEncoder, allocation, methodDescriptor);
                {
                    var toNumberMref = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.TypeUtilities),
                        nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                        parameterTypes: new[] { typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(toNumberMref);
                }
                return true;

            case LIRConvertToBoolean convertToBoolean:
                EmitConvertToBooleanCore(convertToBoolean.Source, ilEncoder, allocation, methodDescriptor);
                return true;

            case LIRConvertToString convertToString:
                EmitConvertToStringCore(convertToString.Source, ilEncoder, allocation, methodDescriptor);
                return true;

            case LIRConcatStrings concatStrings:
                EmitLoadTemp(concatStrings.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(concatStrings.Right, ilEncoder, allocation, methodDescriptor);
                EmitStringConcat(ilEncoder);
                return true;

            case LIRAddNumber addNumber:
                EmitLoadTemp(addNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(addNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Add);
                return true;
            case LIRSubNumber subNumber:
                EmitLoadTemp(subNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(subNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Sub);
                return true;
            case LIRMulNumber mulNumber:
                EmitLoadTemp(mulNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(mulNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Mul);
                return true;
            case LIRDivNumber divNumber:
                EmitLoadTemp(divNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(divNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Div);
                return true;
            case LIRModNumber modNumber:
                EmitLoadTemp(modNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(modNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Rem);
                return true;
            case LIRExpNumber expNumber:
                EmitLoadTemp(expNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(expNumber.Right, ilEncoder, allocation, methodDescriptor);
                EmitMathPow(ilEncoder);
                return true;
            default:
                return false;
        }
    }

    private bool IsMaterialized(TempVariable temp, TempLocalAllocation allocation)
    {
        // Variable-mapped temps always materialize into their stable variable local slot.
        if (temp.Index >= 0 &&
            temp.Index < MethodBody.TempVariableSlots.Count &&
            MethodBody.TempVariableSlots[temp.Index] >= 0)
        {
            return true;
        }

        return allocation.IsMaterialized(temp);
    }

    private void EmitStoreTemp(TempVariable temp, InstructionEncoder ilEncoder, TempLocalAllocation allocation)
    {
        if (!IsMaterialized(temp, allocation))
        {
            ilEncoder.OpCode(ILOpCode.Pop);
            return;
        }

        var slot = GetSlotForTemp(temp, allocation);
        ilEncoder.StoreLocal(slot);
    }

    /// <summary>
    /// Emits IL to load a temp value as an object reference.
    /// If the temp's storage is an unboxed value type, emits a box instruction.
    /// </summary>
    private void EmitLoadTempAsObject(TempVariable temp, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        // Load the temp value
        EmitLoadTemp(temp, ilEncoder, allocation, methodDescriptor);

        // Check if boxing is needed based on storage type
        var storage = GetTempStorage(temp);
        if (storage.Kind == ValueStorageKind.UnboxedValue)
        {
            ilEncoder.OpCode(ILOpCode.Box);
            if (storage.ClrType == typeof(double))
            {
                ilEncoder.Token(_bclReferences.DoubleType);
            }
            else if (storage.ClrType == typeof(bool))
            {
                ilEncoder.Token(_bclReferences.BooleanType);
            }
            else if (storage.ClrType == typeof(JavaScriptRuntime.JsNull))
            {
                ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsNull)));
            }
            else
            {
                // Default to double for unknown numeric types
                ilEncoder.Token(_bclReferences.DoubleType);
            }
        }
    }

    private void EmitConvertToBooleanCore(TempVariable source, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        var sourceStorage = GetTempStorage(source);

        // If the value is already a typed primitive, avoid boxing.
        if (sourceStorage.Kind == ValueStorageKind.UnboxedValue)
        {
            if (sourceStorage.ClrType == typeof(bool))
            {
                // JS ToBoolean(bool) is identity.
                EmitLoadTemp(source, ilEncoder, allocation, methodDescriptor);
                return;
            }

            if (sourceStorage.ClrType == typeof(double))
            {
                EmitLoadTemp(source, ilEncoder, allocation, methodDescriptor);
                var toBooleanDoubleMref = _memberRefRegistry.GetOrAddMethod(
                    typeof(JavaScriptRuntime.TypeUtilities),
                    nameof(JavaScriptRuntime.TypeUtilities.ToBoolean),
                    parameterTypes: new[] { typeof(double) });
                ilEncoder.OpCode(ILOpCode.Call);
                ilEncoder.Token(toBooleanDoubleMref);
                return;
            }
        }

        // Fallback: box and call object-based coercion.
        EmitLoadTempAsObject(source, ilEncoder, allocation, methodDescriptor);
        var toBooleanObjectMref = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.TypeUtilities),
            nameof(JavaScriptRuntime.TypeUtilities.ToBoolean),
            parameterTypes: new[] { typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(toBooleanObjectMref);
    }

    private void EmitConvertToStringCore(TempVariable source, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        EmitLoadTempAsObject(source, ilEncoder, allocation, methodDescriptor);
        var toStringMref = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.DotNet2JSConversions),
            nameof(JavaScriptRuntime.DotNet2JSConversions.ToString),
            parameterTypes: new[] { typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(toStringMref);
    }

    private void EmitNewIntrinsicObjectCore(LIRNewIntrinsicObject newIntrinsic, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(newIntrinsic.IntrinsicName)
            ?? throw new InvalidOperationException($"Unknown intrinsic type: {newIntrinsic.IntrinsicName}");

        bool isStaticClass = intrinsicType.IsAbstract && intrinsicType.IsSealed;
        if (isStaticClass)
        {
            throw new InvalidOperationException($"Intrinsic '{newIntrinsic.IntrinsicName}' is not constructible (static class). ");
        }

        var argc = newIntrinsic.Arguments.Count;
        ConstructorInfo? chosenCtor = argc switch
        {
            0 => intrinsicType.GetConstructor(Type.EmptyTypes),
            1 => intrinsicType.GetConstructor(new[] { typeof(object) }),
            2 => intrinsicType.GetConstructor(new[] { typeof(object), typeof(object) }),
            _ => null
        };

        if (chosenCtor == null)
        {
            throw new InvalidOperationException(
                $"No matching intrinsic constructor found: {intrinsicType.FullName} with {argc} argument(s)");
        }

        foreach (var arg in newIntrinsic.Arguments)
        {
            EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
        }

        var ctorParamTypes = chosenCtor.GetParameters().Select(p => p.ParameterType).ToArray();
        var ctorRef = _memberRefRegistry.GetOrAddConstructor(intrinsicType, ctorParamTypes);
        ilEncoder.OpCode(ILOpCode.Newobj);
        ilEncoder.Token(ctorRef);
    }

    /// <summary>
    /// Gets the storage type for a temp variable.
    /// </summary>
    private ValueStorage GetTempStorage(TempVariable temp)
    {
        if (temp.Index >= 0 && temp.Index < MethodBody.TempStorages.Count)
        {
            return MethodBody.TempStorages[temp.Index];
        }
        return new ValueStorage(ValueStorageKind.Unknown);
    }

    /// <summary>
    /// Emits IL to load the scopes array onto the stack.
    /// For static methods with scopes parameter: ldarg.0 (scopes array is first parameter)
    /// For instance methods with scopes parameter: ldarg.1 (scopes array is second parameter, after this)
    /// For instance methods: ldarg.0 (this), ldfld _scopes (scopes stored in instance field)
    /// </summary>
    private void EmitLoadScopesArray(InstructionEncoder ilEncoder, MethodDescriptor methodDescriptor)
    {
        if (methodDescriptor.IsStatic && methodDescriptor.HasScopesParameter)
        {
            // Static function with scopes parameter - scopes is arg 0
            ilEncoder.LoadArgument(0);
        }
        else if (!methodDescriptor.IsStatic && methodDescriptor.HasScopesParameter)
        {
            // Instance method (e.g., constructor) with scopes parameter - scopes is arg 1
            ilEncoder.LoadArgument(1);
        }
        else if (!methodDescriptor.IsStatic && methodDescriptor.ScopesFieldHandle.HasValue)
        {
            // Instance method with _scopes field
            // ldarg.0 (this), ldfld _scopes
            ilEncoder.LoadArgument(0);
            ilEncoder.OpCode(ILOpCode.Ldfld);
            ilEncoder.Token(methodDescriptor.ScopesFieldHandle.Value);
        }
        else
        {
            // Static method without scopes parameter (e.g., module Main) - shouldn't have parent scope access
            throw new InvalidOperationException("Cannot load scopes array - method has no scopes parameter and no _scopes field");
        }
    }

            private void EmitLoadScopesArrayOrEmpty(InstructionEncoder ilEncoder, MethodDescriptor methodDescriptor)
            {
                if (methodDescriptor.HasScopesParameter || (!methodDescriptor.IsStatic && methodDescriptor.ScopesFieldHandle.HasValue))
                {
                    EmitLoadScopesArray(ilEncoder, methodDescriptor);
                    return;
                }

                // If this method has a leaf scope instance in local 0, use that as the parent
                // scopes array. This is important for resumables (async/generators) because
                // their leaf scope is prepended to the parent scopes, and the runtime expects
                // at least the global/module scope to be present at index 1 after prepending.
                if (MethodBody.NeedsLeafScopeLocal)
                {
                    ilEncoder.LoadConstantI4(1);
                    ilEncoder.OpCode(ILOpCode.Newarr);
                    ilEncoder.Token(_bclReferences.ObjectType);
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.LoadConstantI4(0);
                    ilEncoder.LoadLocal(0);
                    ilEncoder.OpCode(ILOpCode.Stelem_ref);
                    return;
                }

                // ABI fallback: 1-element array with null.
                EmitEmptyScopesArray(ilEncoder);
            }

    /// <summary>
    /// Emits IL to load a scope instance from the specified source.
    /// </summary>
    private void EmitLoadScopeInstance(InstructionEncoder ilEncoder, ScopeSlotSource slotSource, MethodDescriptor methodDescriptor, TempLocalAllocation allocation)
    {
        switch (slotSource.Source)
        {
            case ScopeInstanceSource.LeafLocal:
                // Load from local 0 (the leaf scope instance)
                ilEncoder.LoadLocal(0);
                break;

            case ScopeInstanceSource.ScopesArgument:
                // Load from scopes argument: ldarg.0 (scopes), ldc.i4 index, ldelem.ref
                if (!methodDescriptor.HasScopesParameter)
                {
                    throw new InvalidOperationException("Cannot load from ScopesArgument - method has no scopes parameter");
                }
                ilEncoder.LoadArgument(methodDescriptor.IsStatic ? 0 : 1); // scopes arg position
                ilEncoder.LoadConstantI4(slotSource.SourceIndex);
                ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                break;

            case ScopeInstanceSource.ThisScopes:
                // Load from this._scopes: ldarg.0 (this), ldfld _scopes, ldc.i4 index, ldelem.ref
                if (methodDescriptor.IsStatic || !methodDescriptor.ScopesFieldHandle.HasValue)
                {
                    throw new InvalidOperationException("Cannot load from ThisScopes - method is static or has no _scopes field");
                }
                ilEncoder.LoadArgument(0); // this
                ilEncoder.OpCode(ILOpCode.Ldfld);
                ilEncoder.Token(methodDescriptor.ScopesFieldHandle.Value);
                ilEncoder.LoadConstantI4(slotSource.SourceIndex);
                ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                break;

            case ScopeInstanceSource.Temp:
                // Load from a temp local (TempVariable index encoded in SourceIndex)
                EmitLoadTemp(new TempVariable(slotSource.SourceIndex), ilEncoder, allocation, methodDescriptor);
                break;

            default:
                throw new ArgumentException($"Unknown ScopeInstanceSource: {slotSource.Source}");
        }
    }

    /// <summary>
    /// Emits IL to create a 1-element scopes array with null.
    /// Used for ABI compatibility when callee doesn't need scopes.
    /// </summary>
    private void EmitEmptyScopesArray(InstructionEncoder ilEncoder)
    {
        ilEncoder.LoadConstantI4(1);
        ilEncoder.OpCode(ILOpCode.Newarr);
        ilEncoder.Token(_bclReferences.ObjectType);
        ilEncoder.OpCode(ILOpCode.Dup);
        ilEncoder.LoadConstantI4(0);
        ilEncoder.OpCode(ILOpCode.Ldnull);
        ilEncoder.OpCode(ILOpCode.Stelem_ref);
    }

    /// <summary>
    /// Emits IL to create and populate a scopes array with scope instances.
    /// </summary>
    private void EmitPopulateScopesArray(InstructionEncoder ilEncoder, IReadOnlyList<ScopeSlotSource> slots, MethodDescriptor methodDescriptor, TempLocalAllocation allocation)
    {
        // Create array with proper size
        ilEncoder.LoadConstantI4(slots.Count);
        ilEncoder.OpCode(ILOpCode.Newarr);
        ilEncoder.Token(_bclReferences.ObjectType);
        
        // Populate each slot
        foreach (var slotSource in slots)
        {
            ilEncoder.OpCode(ILOpCode.Dup); // Keep array reference for next stelem
            ilEncoder.LoadConstantI4(slotSource.Slot.Index);
            
            // Load the scope instance from the appropriate source
            EmitLoadScopeInstance(ilEncoder, slotSource, methodDescriptor, allocation);
            
            ilEncoder.OpCode(ILOpCode.Stelem_ref);
        }
    }

    private int GetSlotForTemp(TempVariable temp, TempLocalAllocation allocation)
    {
        // Calculate offset for scope local (if present)
        int scopeLocalOffset = (MethodBody.NeedsLeafScopeLocal && !MethodBody.LeafScopeId.IsNil) ? 1 : 0;
        
        // Variable-mapped temps always go to their stable variable slot (after scope local).
        if (temp.Index >= 0 && temp.Index < MethodBody.TempVariableSlots.Count)
        {
            int varSlot = MethodBody.TempVariableSlots[temp.Index];
            if (varSlot >= 0)
            {
                return scopeLocalOffset + varSlot;
            }
        }

        // Other temps go after variable locals (and scope local).
        var slot = allocation.GetSlot(temp);
        return scopeLocalOffset + MethodBody.VariableNames.Count + slot;
    }

    private LIRInstruction? TryFindDefInstruction(TempVariable temp)
    {
        foreach (var instr in MethodBody.Instructions
            .Where(i => TempLocalAllocator.TryGetDefinedTemp(i, out var defined) && defined == temp))
        {
            return instr;
        }
        return null;
    }

    /// <summary>
    /// Marks stackifiable temps as non-materialized in the peephole mask.
    /// This prevents TempLocalAllocator from allocating IL local slots for temps that can stay on the stack.
    /// </summary>
    private void MarkStackifiableTemps(StackifyResult stackifyResult, bool[]? shouldMaterializeTemp)
    {
        if (shouldMaterializeTemp == null || stackifyResult.CanStackify.Length == 0)
        {
            return;
        }

        for (int i = 0; i < Math.Min(stackifyResult.CanStackify.Length, shouldMaterializeTemp.Length); i++)
        {
            if (stackifyResult.CanStackify[i])
            {
                // This temp can stay on the stack - mark it as not needing materialization
                shouldMaterializeTemp[i] = false;
            }
        }
    }

    #endregion
}