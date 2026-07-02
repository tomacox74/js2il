using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Jroc.Utilities.Ecma335;

namespace Jroc.Services.ILGenerators
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
            bool returnsVoid,
            Type? returnClrType = null,
            EntityHandle returnTypeHandle = default,
            IReadOnlyList<Type?>? jsParameterClrTypes = null,
            TypeReferenceRegistry? typeReferenceRegistry = null)
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
                        if (!returnTypeHandle.IsNil)
                        {
                            // Non-BCL reference return type (e.g., user-defined JS class TypeDef)
                            returnType.Type().Type(returnTypeHandle, isValueType: false);
                            return;
                        }

                        var t = returnClrType ?? typeof(object);
                        if (t == typeof(double))
                        {
                            returnType.Type().Double();
                        }
                        else if (t == typeof(bool))
                        {
                            returnType.Type().Boolean();
                        }
                        else if (t == typeof(string))
                        {
                            returnType.Type().String();
                        }
                        else if (t == typeof(JavaScriptRuntime.Array))
                        {
                            if (typeReferenceRegistry == null)
                            {
                                returnType.Type().Object();
                            }
                            else
                            {
                                returnType.Type().Type(typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Array)), isValueType: false);
                            }
                        }
                        else
                        {
                            returnType.Type().Object();
                        }
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
                        EmitParameterType(parameters.AddParameter().Type(), jsParameterClrTypes != null && i < jsParameterClrTypes.Count
                            ? jsParameterClrTypes[i]
                            : null,
                            typeReferenceRegistry);
                    }
                });

            return metadata.GetOrAddBlob(sig);
        }

        private static void EmitParameterType(SignatureTypeEncoder typeEncoder, Type? clrType, TypeReferenceRegistry? typeReferenceRegistry)
        {
            if (clrType == typeof(double))
            {
                typeEncoder.Double();
            }
            else if (clrType == typeof(bool))
            {
                typeEncoder.Boolean();
            }
            else if (clrType == typeof(string))
            {
                typeEncoder.String();
            }
            else if (clrType == typeof(JavaScriptRuntime.Array))
            {
                if (typeReferenceRegistry == null)
                {
                    typeEncoder.Object();
                }
                else
                {
                    typeEncoder.Type(typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Array)), isValueType: false);
                }
            }
            else
            {
                typeEncoder.Object();
            }
        }
    }
}
