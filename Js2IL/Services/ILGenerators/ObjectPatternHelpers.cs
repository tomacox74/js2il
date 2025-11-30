using Acornima.Ast;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.Services.ILGenerators
{
    internal static class ObjectPatternHelpers
    {
        /// <summary>
        /// Emit field stores for properties in an ObjectPattern parameter.
        /// Loads the scope instance for the target variable, fetches the source argument (jsParamSeq),
        /// reads the property via JavaScriptRuntime.Object.GetProperty, and stores into the field.
        /// </summary>
        internal static void EmitParamDestructuring(
            InstructionEncoder il,
            MetadataBuilder metadataBuilder,
            Runtime runtime,
            Variables vars,
            Variable targetVar,
            ushort jsParamSeq,
            string propName)
        {
            // Load scope instance holding the field
            var tslot = vars.GetScopeLocalSlot(targetVar.ScopeName);
            var tScopeType = vars.GetVariableRegistry()?.GetScopeTypeHandle(targetVar.ScopeName) ?? default;
            
            if (tslot.Location == ObjectReferenceLocation.Parameter)
            {
                il.LoadArgument(tslot.Address);
                // Cast needed: parameter is typed as object
                if (!tScopeType.IsNil)
                {
                    il.OpCode(System.Reflection.Metadata.ILOpCode.Castclass);
                    il.Token(tScopeType);
                }
            }
            else if (tslot.Location == ObjectReferenceLocation.ScopeArray)
            {
                il.LoadArgument(0);
                il.LoadConstantI4(tslot.Address);
                il.OpCode(System.Reflection.Metadata.ILOpCode.Ldelem_ref);
                // Cast needed: array element is typed as object
                if (!tScopeType.IsNil)
                {
                    il.OpCode(System.Reflection.Metadata.ILOpCode.Castclass);
                    il.Token(tScopeType);
                }
            }
            else
            {
                il.LoadLocal(tslot.Address);
            }
            // Load incoming argument (object being destructured)
            il.LoadArgument(jsParamSeq);
            il.Ldstr(metadataBuilder, propName);
            var getPropRef = runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetProperty), typeof(object), typeof(object), typeof(string));
            il.OpCode(System.Reflection.Metadata.ILOpCode.Call); il.Token(getPropRef);
            il.OpCode(System.Reflection.Metadata.ILOpCode.Stfld); il.Token(targetVar.FieldHandle);
        }
    }
}
