using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Convenience helpers for emitting common IL instruction patterns using System.Reflection.Metadata encoders.
    /// Keep these generic and dependency-free so they can be reused across generators.
    /// </summary>
    internal static class ILEmitHelpers
    {
        // Load a managed string literal
        public static void Ldstr(this InstructionEncoder il, MetadataBuilder md, string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            il.LoadString(md.GetOrAddUserString(value));
        }

        // throw new <Error>(message)
        public static void EmitThrowError(this InstructionEncoder il, MetadataBuilder md, EntityHandle errorCtor, string message)
        {
            il.Ldstr(md, message);
            il.OpCode(ILOpCode.Newobj);
            il.Token(errorCtor);
            il.OpCode(ILOpCode.Throw);
        }

        /// <summary>
        /// Emit: new T[length] given the element type handle, and fill elements using the provided callback.
        /// The callback must leave the element value on the stack for each index.
        /// </summary>
        public static void EmitNewArray(this InstructionEncoder il, int length, EntityHandle elementType, Action<InstructionEncoder,int> emitElementAt)
        {
            if (emitElementAt is null) throw new ArgumentNullException(nameof(emitElementAt));
            il.LoadConstantI4(length);
            il.OpCode(ILOpCode.Newarr);
            il.Token(elementType);

            for (int i = 0; i < length; i++)
            {
                il.OpCode(ILOpCode.Dup);
                il.LoadConstantI4(i);
                emitElementAt(il, i);
                il.OpCode(ILOpCode.Stelem_ref);
            }
        }

        /// <summary>
        /// Emit: new object[length] and fill elements using the provided callback. The callback must leave
        /// the element value on the stack for each index.
        /// </summary>
        public static void EmitNewObjectArray(this InstructionEncoder il, int length, EntityHandle objectType, Action<InstructionEncoder,int> emitElementAt)
        {
            il.EmitNewArray(length, objectType, emitElementAt);
        }
    }
}
