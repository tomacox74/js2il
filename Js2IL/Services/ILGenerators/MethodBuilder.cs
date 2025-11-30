using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Acornima.Ast;

namespace Js2IL.Services.ILGenerators
{
    internal static class MethodBuilder
    {
        /// <summary>
        /// Builds a method or constructor signature where parameters are encoded as System.Object,
        /// with an optional leading object[] scopes parameter and optional void return.
        /// Shared by constructors, class methods, functions, and arrow functions.
        /// </summary>
        public static BlobHandle BuildMethodSignature(
            MetadataBuilder metadata,
            bool isInstance,
            int paramCount,
            bool hasScopesParam,
            bool returnsVoid)
        {
            if (hasScopesParam && paramCount == 0)
            {
                throw new ArgumentException("paramCount must be > 0 when hasScopesParam is true.", nameof(paramCount));
            }

            var sig = new BlobBuilder();
            var encoder = new BlobEncoder(sig)
                .MethodSignature(isInstanceMethod: isInstance);

            encoder.Parameters(
                parameterCount: paramCount,
                returnType =>
                {
                    if (returnsVoid)
                    {
                        returnType.Void();
                    }
                    else
                    {
                        returnType.Type().Object();
                    }
                },
                parameters =>
                {
                    var remaining = paramCount;
                    if (hasScopesParam)
                    {
                        parameters.AddParameter().Type().SZArray().Object();
                        remaining--;
                    }

                    for (int i = 0; i < remaining; i++)
                    {
                        parameters.AddParameter().Type().Object();
                    }
                });

            return metadata.GetOrAddBlob(sig);
        }

        /// <summary>
        /// Emit object-pattern parameter destructuring stores for a function/arrow function.
        /// Assumes default parameter initializers have already been applied and any needed scope instance
        /// has been created by the caller when fields exist. This only emits destructuring stores for
        /// bindings that have backing fields in the current scope.
        /// </summary>
        public static void EmitObjectPatternParameterDestructuring(
            MetadataBuilder metadataBuilder,
            InstructionEncoder il,
            Runtime runtime,
            Variables variables,
            string scopeName,
            IReadOnlyList<Node> parameters,
            ushort startingJsParamSeq = 1,
            bool castScopeForStore = true)
        {
            var registry = variables.GetVariableRegistry();
            if (registry == null) return;
            var fields = registry.GetVariablesForScope(scopeName);
            if (fields == null) return;
            var fieldNames = new HashSet<string>();
            foreach (var f in fields) fieldNames.Add(f.Name);

            ushort jsParamSeq = startingJsParamSeq;
            for (int i = 0; i < parameters.Count; i++)
            {
                var pnode = parameters[i];
                if (pnode is ObjectPattern op)
                {
                    foreach (var propNode in op.Properties)
                    {
                        if (propNode is Property p)
                        {
                            var bindId = p.Value as Identifier ?? p.Key as Identifier;
                            if (bindId == null) continue;
                            if (!fieldNames.Contains(bindId.Name)) continue;
                            var propName = (p.Key as Identifier)?.Name
                                ?? (p.Key as Literal)?.Value?.ToString()
                                ?? string.Empty;
                            if (castScopeForStore)
                            {
                                var targetVar = variables.FindVariable(bindId.Name);
                                if (targetVar == null || targetVar.FieldHandle.IsNil) continue;
                                ObjectPatternHelpers.EmitParamDestructuring(il, metadataBuilder, runtime, variables, targetVar, jsParamSeq, propName);
                            }
                            else
                            {
                                var localScope = variables.GetLocalScopeSlot();
                                if (localScope.Address < 0) continue;
                                il.LoadLocal(localScope.Address);
                                il.LoadArgument(jsParamSeq);
                                il.Ldstr(metadataBuilder, propName);
                                var getPropRef = runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetProperty), typeof(object), typeof(object), typeof(string));
                                il.OpCode(System.Reflection.Metadata.ILOpCode.Call); il.Token(getPropRef);
                                var fieldHandle = registry.GetFieldHandle(scopeName, bindId.Name);
                                il.OpCode(System.Reflection.Metadata.ILOpCode.Stfld); il.Token(fieldHandle);
                            }
                        }
                    }
                }
                jsParamSeq++;
            }
        }
    }
}
