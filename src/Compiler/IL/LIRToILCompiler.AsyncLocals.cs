using Js2IL.IR;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities.Ecma335;
using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    private int GetScopeLocalOffset()
    {
        // Local 0 is reserved for the leaf scope instance when present.
        return (MethodBody.NeedsLeafScopeLocal && !MethodBody.LeafScopeId.IsNil) ? 1 : 0;
    }

    private EntityHandle GetBoxingTypeToken(Type clrType)
    {
        if (clrType == typeof(double)) return _bclReferences.DoubleType;
        if (clrType == typeof(bool)) return _bclReferences.BooleanType;
        if (clrType == typeof(int)) return _bclReferences.Int32Type;
        if (clrType == typeof(string)) return _bclReferences.StringType;
        if (clrType == typeof(JavaScriptRuntime.JsNull)) return _typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsNull));

        return _typeReferenceRegistry.GetOrAdd(clrType);
    }

    private void EmitEnsureAsyncLocalsArray(InstructionEncoder ilEncoder)
    {
        if (MethodBody.VariableNames.Count == 0)
        {
            return;
        }

        var scopeName = MethodBody.LeafScopeId.Name;

        // if (scope._locals != null) goto hasLocals;
        // scope._locals = new object[VariableNames.Count];
        var hasLocalsLabel = ilEncoder.DefineLabel();

        ilEncoder.LoadLocal(0);
        EmitLoadFieldByName(ilEncoder, scopeName, "_locals");
        ilEncoder.Branch(ILOpCode.Brtrue, hasLocalsLabel);

        ilEncoder.LoadLocal(0);
        ilEncoder.LoadConstantI4(MethodBody.VariableNames.Count);
        ilEncoder.OpCode(ILOpCode.Newarr);
        ilEncoder.Token(_bclReferences.ObjectType);
        EmitStoreFieldByName(ilEncoder, scopeName, "_locals");

        ilEncoder.MarkLabel(hasLocalsLabel);
    }

    private void EmitSpillVariableSlotsToAsyncLocalsArray(InstructionEncoder ilEncoder)
    {
        if (MethodBody.VariableNames.Count == 0)
        {
            return;
        }

        var scopeName = MethodBody.LeafScopeId.Name;
        var scopeLocalOffset = GetScopeLocalOffset();

        // locals = scope._locals;
        ilEncoder.LoadLocal(0);
        EmitLoadFieldByName(ilEncoder, scopeName, "_locals");

        for (int i = 0; i < MethodBody.VariableNames.Count; i++)
        {
            var storage = MethodBody.VariableStorages[i];

            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.LoadConstantI4(i);

            // Load variable local (after scope local).
            ilEncoder.LoadLocal(scopeLocalOffset + i);

            // Box unboxed values for object[] storage.
            if (storage.Kind == ValueStorageKind.UnboxedValue)
            {
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(GetBoxingTypeToken(storage.ClrType ?? typeof(object)));
            }

            ilEncoder.OpCode(ILOpCode.Stelem_ref);
        }

        ilEncoder.OpCode(ILOpCode.Pop); // pop locals array
    }

    private void EmitRestoreVariableSlotsFromAsyncLocalsArray(InstructionEncoder ilEncoder)
    {
        if (MethodBody.VariableNames.Count == 0)
        {
            return;
        }

        var scopeName = MethodBody.LeafScopeId.Name;
        var scopeLocalOffset = GetScopeLocalOffset();

        // locals = scope._locals;
        ilEncoder.LoadLocal(0);
        EmitLoadFieldByName(ilEncoder, scopeName, "_locals");

        for (int i = 0; i < MethodBody.VariableNames.Count; i++)
        {
            var storage = MethodBody.VariableStorages[i];

            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.LoadConstantI4(i);
            ilEncoder.OpCode(ILOpCode.Ldelem_ref);

            // Convert from object back to expected local type.
            if (storage.Kind == ValueStorageKind.UnboxedValue)
            {
                ilEncoder.OpCode(ILOpCode.Unbox_any);
                ilEncoder.Token(GetBoxingTypeToken(storage.ClrType ?? typeof(object)));
            }
            else if (storage.Kind == ValueStorageKind.Reference && storage.ClrType != typeof(object))
            {
                ilEncoder.OpCode(ILOpCode.Castclass);
                ilEncoder.Token(GetBoxingTypeToken(storage.ClrType ?? typeof(object)));
            }

            ilEncoder.StoreLocal(scopeLocalOffset + i);
        }

        ilEncoder.OpCode(ILOpCode.Pop); // pop locals array
    }
}
