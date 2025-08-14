using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Centralizes emission of a leaf scope instance for the current function/global scope.
    /// Safe to call even when no registry/type is available; it will no-op.
    /// </summary>
    internal static class ScopeInstanceEmitter
    {
        /// <summary>
        /// Emits IL to create the current scope instance (newobj .ctor; stloc scopeLocal),
        /// storing it into the local slot managed by Variables. If the scope type is not
        /// available (e.g., registry lacks it), this method no-ops.
        /// </summary>
        public static void EmitCreateLeafScopeInstance(ILMethodGenerator ilGen)
        {
            if (ilGen == null) throw new ArgumentNullException(nameof(ilGen));

            var variables = ilGen.Variables;
            var registry = variables.GetVariableRegistry();
            if (registry == null)
                return; // Old local variable system; nothing to do

            var currentScopeName = variables.GetLeafScopeName();
            try
            {
                var scopeTypeHandle = registry.GetScopeTypeHandle(currentScopeName);
                if (scopeTypeHandle.IsNil)
                    return;

                // Build parameterless .ctor signature
                var ctorSig = new BlobBuilder();
                new BlobEncoder(ctorSig)
                    .MethodSignature(isInstanceMethod: true)
                    .Parameters(0, r => r.Void(), _ => { });

                var ctorRef = ilGen.MetadataBuilder.AddMemberReference(
                    scopeTypeHandle,
                    ilGen.MetadataBuilder.GetOrAddString(".ctor"),
                    ilGen.MetadataBuilder.GetOrAddBlob(ctorSig));

                // newobj ScopeType::.ctor
                ilGen.IL.OpCode(ILOpCode.Newobj);
                ilGen.IL.Token(ctorRef);

                // stloc (current scope local)
                var scopeLocal = variables.CreateScopeInstance(currentScopeName);
                if (scopeLocal.Address >= 0)
                {
                    ilGen.IL.StoreLocal(scopeLocal.Address);
                }
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                // If type/fields are not present, skip creating a local scope instance.
            }
        }
    }
}
