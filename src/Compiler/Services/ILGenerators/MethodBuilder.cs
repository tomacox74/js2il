using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

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
            bool returnsVoid,
            Type? returnClrType = null,
            EntityHandle returnTypeHandle = default)
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
                        parameters.AddParameter().Type().Object();
                    }
                });

            return metadata.GetOrAddBlob(sig);
        }
    }
}
