using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Js2IL.Services
{
    internal class Runtime
    {
        private MemberReferenceHandle _toStringMethodRef;
        private MemberReferenceHandle _consoleLogMethodRef;
        private InstructionEncoder _il;

        public Runtime(MetadataBuilder metadataBuilder, InstructionEncoder il) 
        { 
            _il = il;

            var thisAssembly = typeof(Runtime).Assembly;
            var thisAssemblyName = thisAssembly.GetName();
            var thisAssemblyVersion = thisAssemblyName.Version;

            var thisAssemblyReference = metadataBuilder.AddAssemblyReference(
                metadataBuilder.GetOrAddString(thisAssemblyName.Name!),
                version: thisAssemblyVersion!,
                culture: default,
                publicKeyOrToken: default,
                flags: 0,
                hashValue: default
            );

            var dotNet2JsType = metadataBuilder.AddTypeReference(
                thisAssemblyReference,
                metadataBuilder.GetOrAddString("Js2IL.Runtime"),
                metadataBuilder.GetOrAddString("DotNet2JSConversions")
            );


            // Create method signature: void ToString(object)
            var toStringSigBuilder = new BlobBuilder();
            new BlobEncoder(toStringSigBuilder)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(1,
                    returnType => returnType.Type().String(),
                    parameters => {
                        parameters.AddParameter().Type().Object();
                    });
            var toStringSig = metadataBuilder.GetOrAddBlob(toStringSigBuilder);


            _toStringMethodRef = metadataBuilder.AddMemberReference(
                dotNet2JsType,
                metadataBuilder.GetOrAddString("ToString"),
                toStringSig);

            // create the method body for Console.Log(string, object)
            var consoleLogSigBuilder = new BlobBuilder();
            new BlobEncoder(consoleLogSigBuilder)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(2,
                    returnType => returnType.Void(),
                    parameters => {
                        parameters.AddParameter().Type().String();
                        parameters.AddParameter().Type().Object();
                    });
            var consoleLogSig = metadataBuilder.GetOrAddBlob(consoleLogSigBuilder);

            var consoleType = metadataBuilder.AddTypeReference(
                thisAssemblyReference,
                metadataBuilder.GetOrAddString("Js2IL.Runtime"),
                metadataBuilder.GetOrAddString("Console")
            );

            _consoleLogMethodRef = metadataBuilder.AddMemberReference(
                consoleType,
                metadataBuilder.GetOrAddString("Log"),
                consoleLogSig);
        }

        /// <summary>
        /// Inserts IL to convert what ever is on the stack to a string representation.
        /// </summary>
        /// <remarks>
        /// Currently takes a dependency on a external libary.  Stretch goal is to append runtime il to the generated assembly.
        /// </remarks>
        public void InvokeToString()
        {
            // we assume the object to covert to a string is already on the stack
            _il.OpCode(ILOpCode.Call);
            _il.Token(_toStringMethodRef);
        }

        /// <summary>
        /// Inserts IL to call Console.Log(string, object) with the string representation of the object on the stack.
        /// </summary>
        /// <remarks>
        /// Assumes the parameters for Console.Log are already on the stack
        /// </remarks>
        public void InvokeConsoleLog()
        {
            _il.OpCode(ILOpCode.Call);
            _il.Token(_consoleLogMethodRef);
        }
    }
}
