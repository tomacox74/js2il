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
            // Load the scope instance if this is a field variable
            if (!targetVar.FieldHandle.IsNil)
            {
                var localScope = vars.GetLocalScopeSlot();
                if (localScope.Address >= 0)
                {
                    il.LoadLocal(localScope.Address);
                }
            }
            
            // Load incoming argument (object being destructured) and extract property
            il.LoadArgument(jsParamSeq);
            il.Ldstr(metadataBuilder, propName);
            var getPropRef = runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetProperty), 0, typeof(object), typeof(string));
            il.OpCode(System.Reflection.Metadata.ILOpCode.Call); il.Token(getPropRef);
            
            // Store the property value to the variable (field or local)
            ILEmitHelpers.EmitStoreVariable(il, targetVar, vars, scopeAlreadyLoaded: true);
        }
    }
}
