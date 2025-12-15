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
        /// Creates a local variable signature for the specified variables context.
        /// Scope locals are typed as their specific scope class; other locals are typed as System.Object.
        /// Returns the signature handle and body attributes (InitLocals if locals > 0, None otherwise).
        /// </summary>
        public static (StandaloneSignatureHandle signature, MethodBodyAttributes attributes) CreateLocalVariableSignature(
            MetadataBuilder metadata,
            Variables variables,
            BaseClassLibraryReferences bclReferences)
        {
            int numberOfLocals = variables.GetNumberOfLocals();
            if (numberOfLocals <= 0)
            {
                return (default, MethodBodyAttributes.None);
            }

            var localSig = new BlobBuilder();
            var localEncoder = new BlobEncoder(localSig).LocalVariableSignature(numberOfLocals);
            for (int i = 0; i < numberOfLocals; i++)
            {
                var typeHandle = variables.GetLocalVariableType(i, bclReferences);
                
                if (typeHandle.HasValue)
                {
                    if (typeHandle.Value == bclReferences.DoubleType)
                    {
                        localEncoder.AddVariable().Type().Double();
                    }
                    else if (typeHandle.Value.Kind == HandleKind.TypeDefinition)
                    {
                        localEncoder.AddVariable().Type().Type((TypeDefinitionHandle)typeHandle.Value, isValueType: false);
                    }
                    else if (typeHandle.Value.Kind == HandleKind.TypeReference)
                    {
                        localEncoder.AddVariable().Type().Type((TypeReferenceHandle)typeHandle.Value, isValueType: false);
                    }
                }
                else
                {
                    // Fallback to Object type if the handle is null or neither type
                    localEncoder.AddVariable().Type().Object();
                }
            }

            var signature = metadata.AddStandaloneSignature(metadata.GetOrAddBlob(localSig));
            return (signature, MethodBodyAttributes.InitLocals);
        }

        /// <summary>
        /// Emit object-pattern parameter destructuring stores with support for default values.
        /// Default values in patterns like { host = "localhost" } are properly handled.
        /// </summary>
        public static void EmitObjectPatternParameterDestructuring(
            MetadataBuilder metadataBuilder,
            InstructionEncoder il,
            Runtime runtime,
            Variables variables,
            string scopeName,
            IReadOnlyList<Node> parameters,
            IMethodExpressionEmitter expressionEmitter,
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
                            // Extract identifier and check for default value
                            // Property.Value can be:
                            // - Identifier: simple binding like { host }
                            // - AssignmentPattern: binding with default like { host = "localhost" }
                            Identifier? bindId = null;
                            Expression? defaultValue = null;
                            
                            if (p.Value is Identifier id)
                            {
                                bindId = id;
                            }
                            else if (p.Value is AssignmentPattern ap)
                            {
                                bindId = ap.Left as Identifier;
                                defaultValue = ap.Right;
                            }
                            else if (p.Key is Identifier keyId)
                            {
                                // Fallback: shorthand property
                                bindId = keyId;
                            }
                            
                            if (bindId == null) continue;
                            if (!fieldNames.Contains(bindId.Name)) continue;
                            
                            var propName = (p.Key as Identifier)?.Name
                                ?? (p.Key as Literal)?.Value?.ToString()
                                ?? string.Empty;
                                
                            if (castScopeForStore)
                            {
                                var targetVar = variables.FindVariable(bindId.Name);
                                if (targetVar == null || (targetVar.FieldHandle.IsNil && targetVar.LocalSlot < 0)) continue;
                                
                                if (defaultValue != null)
                                {
                                    // With default value and castScopeForStore: check if property is null/undefined
                                    // Pattern: temp = GetProperty(param, propName)
                                    //          if (temp != null) scope.field = temp; else scope.field = defaultValue;
                                    
                                    int tempLocal = variables.AllocateBlockScopeLocal($"DestructTemp_{propName}_L{p.Location.Start.Line}");
                                    
                                    // Get property value into temp
                                    il.LoadArgument(jsParamSeq);
                                    il.Ldstr(metadataBuilder, propName);
                                    var getPropRef = runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetProperty), typeof(object), typeof(object), typeof(string));
                                    il.OpCode(System.Reflection.Metadata.ILOpCode.Call); il.Token(getPropRef);
                                    il.StoreLocal(tempLocal);
                                    
                                    // Check if temp is null
                                    il.LoadLocal(tempLocal);
                                    var labelUseDefault = il.DefineLabel();
                                    var labelEnd = il.DefineLabel();
                                    il.Branch(System.Reflection.Metadata.ILOpCode.Brfalse, labelUseDefault);
                                    
                                    // Not null: load scope (if needed), load temp value, then store to variable
                                    if (!targetVar.FieldHandle.IsNil)
                                    {
                                        var localScope = variables.GetLocalScopeSlot();
                                        if (localScope.Address >= 0)
                                        {
                                            il.LoadLocal(localScope.Address);
                                        }
                                    }
                                    il.LoadLocal(tempLocal);
                                    ILEmitHelpers.EmitStoreVariable(il, targetVar, variables, scopeAlreadyLoaded: true);
                                    il.Branch(System.Reflection.Metadata.ILOpCode.Br, labelEnd);
                                    
                                    // Null: load scope (if needed), emit default expression, then store to variable
                                    il.MarkLabel(labelUseDefault);
                                    if (!targetVar.FieldHandle.IsNil)
                                    {
                                        var localScope = variables.GetLocalScopeSlot();
                                        if (localScope.Address >= 0)
                                        {
                                            il.LoadLocal(localScope.Address);
                                        }
                                    }
                                    expressionEmitter.Emit(defaultValue, new TypeCoercion { boxResult = true });
                                    ILEmitHelpers.EmitStoreVariable(il, targetVar, variables, scopeAlreadyLoaded: true);
                                    
                                    il.MarkLabel(labelEnd);
                                }
                                else
                                {
                                    // No default: use existing helper
                                    ObjectPatternHelpers.EmitParamDestructuring(il, metadataBuilder, runtime, variables, targetVar, jsParamSeq, propName);
                                }
                            }
                            else
                            {
                                var localScope = variables.GetLocalScopeSlot();
                                if (localScope.Address < 0) continue;
                                var fieldHandle = registry.GetFieldHandle(scopeName, bindId.Name);

                                if (defaultValue != null)
                                {
                                    // With default value: check if property is null/undefined, use default if so
                                    // Pattern: 
                                    //   temp = Object.GetProperty(param, propName)
                                    //   if (temp != null) scope.field = temp; else scope.field = defaultValue;
                                    
                                    // Get the property value into a temporary local
                                    int tempLocal = variables.AllocateBlockScopeLocal($"DestructTemp_{propName}_L{p.Location.Start.Line}");
                                    il.LoadArgument(jsParamSeq);
                                    il.Ldstr(metadataBuilder, propName);
                                    var getPropRef = runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetProperty), typeof(object), typeof(object), typeof(string));
                                    il.OpCode(System.Reflection.Metadata.ILOpCode.Call); 
                                    il.Token(getPropRef);
                                    il.StoreLocal(tempLocal);
                                    
                                    // Check if temp is null
                                    il.LoadLocal(tempLocal);
                                    var labelUseDefault = il.DefineLabel();
                                    var labelEnd = il.DefineLabel();
                                    il.Branch(System.Reflection.Metadata.ILOpCode.Brfalse, labelUseDefault);
                                    
                                    // Not null: store the extracted value
                                    il.LoadLocal(localScope.Address);
                                    il.LoadLocal(tempLocal);
                                    il.OpCode(System.Reflection.Metadata.ILOpCode.Stfld); 
                                    il.Token(fieldHandle);
                                    il.Branch(System.Reflection.Metadata.ILOpCode.Br, labelEnd);
                                    
                                    // Null/undefined: use default value
                                    il.MarkLabel(labelUseDefault);
                                    il.LoadLocal(localScope.Address);
                                    expressionEmitter.Emit(defaultValue, new TypeCoercion { boxResult = true });
                                    il.OpCode(System.Reflection.Metadata.ILOpCode.Stfld); 
                                    il.Token(fieldHandle);
                                    il.MarkLabel(labelEnd);
                                }
                                else
                                {
                                    // No default value: simple extraction
                                    il.LoadLocal(localScope.Address);
                                    il.LoadArgument(jsParamSeq);
                                    il.Ldstr(metadataBuilder, propName);
                                    var getPropRef = runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetProperty), typeof(object), typeof(object), typeof(string));
                                    il.OpCode(System.Reflection.Metadata.ILOpCode.Call); 
                                    il.Token(getPropRef);
                                    il.OpCode(System.Reflection.Metadata.ILOpCode.Stfld); 
                                    il.Token(fieldHandle);
                                }
                            }
                        }
                    }
                }
                jsParamSeq++;
            }
        }
    }
}
