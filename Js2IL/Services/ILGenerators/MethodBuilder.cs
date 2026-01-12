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

            for (int localIndex = 0; localIndex < numberOfLocals; localIndex++)
            {
                var typeEncoder = localEncoder.AddVariable().Type();

                var localTypeHandle = variables.GetLocalVariableType(localIndex, bclReferences);
                if (!localTypeHandle.HasValue || localTypeHandle.Value.IsNil)
                {
                    typeEncoder.Object();
                    continue;
                }

                // Prefer primitive encoders for known BCL value types.
                if (localTypeHandle.Value.Equals(bclReferences.DoubleType))
                {
                    typeEncoder.Double();
                }
                else if (localTypeHandle.Value.Equals(bclReferences.BooleanType))
                {
                    typeEncoder.Boolean();
                }
                else if (localTypeHandle.Value.Equals(bclReferences.Int32Type))
                {
                    typeEncoder.Int32();
                }
                else if (localTypeHandle.Value.Equals(bclReferences.StringType))
                {
                    typeEncoder.String();
                }
                else if (localTypeHandle.Value.Equals(bclReferences.ObjectType))
                {
                    typeEncoder.Object();
                }
                else
                {
                    typeEncoder.Type(localTypeHandle.Value, isValueType: false);
                }
            }

            var signature = metadata.AddStandaloneSignature(metadata.GetOrAddBlob(localSig));
            return (signature, MethodBodyAttributes.InitLocals);
        }
    }
}
