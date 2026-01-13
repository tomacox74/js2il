using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Convenience helpers for emitting common IL instruction patterns using System.Reflection.Metadata encoders.
    /// Keep these generic and dependency-free so they can be reused across IR and declaration phases.
    /// </summary>
    internal static class ILEmitHelpers
    {
        public static void Ldstr(this InstructionEncoder il, MetadataBuilder md, string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            il.LoadString(md.GetOrAddUserString(value));
        }

        public static void EmitThrowError(this InstructionEncoder il, MetadataBuilder md, EntityHandle errorCtor, string message)
        {
            il.Ldstr(md, message);
            il.OpCode(ILOpCode.Newobj);
            il.Token(errorCtor);
            il.OpCode(ILOpCode.Throw);
        }

        public static void EmitNewArray(this InstructionEncoder il, int length, EntityHandle elementType, Action<InstructionEncoder, int> emitElementAt)
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

        public static void EmitNewObjectArray(this InstructionEncoder il, int length, EntityHandle objectType, Action<InstructionEncoder, int> emitElementAt)
            => il.EmitNewArray(length, objectType, emitElementAt);

        public static void EmitNewObjectArray(
            this InstructionEncoder il,
            int length,
            EntityHandle objectType,
            Utilities.Ecma335.MemberReferenceRegistry memberRefRegistry,
            Action<InstructionEncoder, int>? emitElementAt)
        {
            if (length == 0)
            {
                var arrayEmptyRef = memberRefRegistry.GetOrAddArrayEmptyObject();
                il.Call(arrayEmptyRef);
                return;
            }

            if (emitElementAt is null) throw new ArgumentNullException(nameof(emitElementAt));
            il.EmitNewArray(length, objectType, emitElementAt);
        }

        [DoesNotReturn]
        public static void ThrowNotSupported(string message, Acornima.Ast.Node? node = null)
        {
            if (node == null)
            {
                throw new NotSupportedException(message);
            }

            var loc = node.Location.Start;
            var src = string.IsNullOrEmpty(node.Location.SourceFile) ? "<unknown>" : node.Location.SourceFile;
            throw new NotSupportedException($"{src}:{loc.Line}:{loc.Column}: {message}");
        }

        public static NotSupportedException NotSupported(string message, Acornima.Ast.Node? node = null)
        {
            if (node == null)
            {
                return new NotSupportedException(message);
            }

            var loc = node.Location.Start;
            var src = string.IsNullOrEmpty(node.Location.SourceFile) ? "<unknown>" : node.Location.SourceFile;
            return new NotSupportedException($"{src}:{loc.Line}:{loc.Column}: {message}");
        }
    }
}
