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
        private MemberReferenceHandle _objectGetItem;
        private MemberReferenceHandle _arrayCtorRef;
        private MemberReferenceHandle _arrayLengthRef;
        private MemberReferenceHandle _closureBindObjectRef;
    private InstructionEncoder _il;
    private MemberReferenceHandle _operatorsAddRef;

        public Runtime(MetadataBuilder metadataBuilder, InstructionEncoder il) 
        { 
            _il = il;

            var runtimeAssembly = typeof(JavaScriptRuntime.Console).Assembly;
            var runtimeAssemblyName = runtimeAssembly.GetName();
            var runtimeAssemblyVersion = runtimeAssemblyName.Version;

            var runtimeAssemblyReference = metadataBuilder.AddAssemblyReference(
                metadataBuilder.GetOrAddString(runtimeAssemblyName.Name!),
                version: runtimeAssemblyVersion!,
                culture: default,
                publicKeyOrToken: default,
                flags: 0,
                hashValue: default
            );

            var dotNet2JsType = metadataBuilder.AddTypeReference(
                runtimeAssemblyReference,
                metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime)),
                metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime.DotNet2JSConversions))
            );

            var operatorsType = metadataBuilder.AddTypeReference(
                runtimeAssemblyReference,
                metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime)),
                metadataBuilder.GetOrAddString("Operators")
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

            // JavaScriptRuntime.Operators.Add(object, object) -> object
            var addSigBuilder = new BlobBuilder();
            new BlobEncoder(addSigBuilder)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(2,
                    returnType => returnType.Type().Object(),
                    parameters => {
                        parameters.AddParameter().Type().Object();
                        parameters.AddParameter().Type().Object();
                    });
            var addSig = metadataBuilder.GetOrAddBlob(addSigBuilder);
            _operatorsAddRef = metadataBuilder.AddMemberReference(
                operatorsType,
                metadataBuilder.GetOrAddString("Add"),
                addSig);

            // create the method body for Console.Log(params object[] args)
            var consoleLogSigBuilder = new BlobBuilder();
            new BlobEncoder(consoleLogSigBuilder)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(1,
                    returnType => returnType.Void(),
                    parameters => {
                        parameters.AddParameter().Type().SZArray().Object();
                    });
            var consoleLogSig = metadataBuilder.GetOrAddBlob(consoleLogSigBuilder);

            var consoleType = metadataBuilder.AddTypeReference(
                runtimeAssemblyReference,
                metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime)),
                metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime.Console))
            );

            _consoleLogMethodRef = metadataBuilder.AddMemberReference(
                consoleType,
                metadataBuilder.GetOrAddString("Log"),
                consoleLogSig);

            // Array type ---------
            var arrayType = metadataBuilder.AddTypeReference(
                runtimeAssemblyReference,
                metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime)),
                metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime.Array)));

            //  - Constructor
            var arraySigBuilder = new BlobBuilder();
            new BlobEncoder(arraySigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(1, returnType => returnType.Void(), parameters => 
                 { 
                     parameters.AddParameter().Type().Int32(); 
                 });
            var arrayCtorSig = metadataBuilder.GetOrAddBlob(arraySigBuilder);
            _arrayCtorRef = metadataBuilder.AddMemberReference(
                arrayType,
                metadataBuilder.GetOrAddString(".ctor"),
                arrayCtorSig);

            // - length
            var arrayLengthSigBuilder = new BlobBuilder();
            new BlobEncoder(arrayLengthSigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(0, returnType => returnType.Type().Double(), parameters => { });
            var arrayLengthSig = metadataBuilder.GetOrAddBlob(arrayLengthSigBuilder);
            _arrayLengthRef = metadataBuilder.AddMemberReference(
                arrayType,
                metadataBuilder.GetOrAddString("get_length"),
                arrayLengthSig);


            var objectType = metadataBuilder.AddTypeReference(
                runtimeAssemblyReference,
                metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime)),
                metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime.Object)));

            var objectGetItemSigBuilder = new BlobBuilder();
            new BlobEncoder(objectGetItemSigBuilder)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(2, returnType => returnType.Type().Object(), parameters => 
                { 
                    parameters.AddParameter().Type().Object();
                    parameters.AddParameter().Type().Double();
                });
            var objectGetItemSig = metadataBuilder.GetOrAddBlob(objectGetItemSigBuilder);
            _objectGetItem = metadataBuilder.AddMemberReference(
                objectType,
                metadataBuilder.GetOrAddString("GetItem"),
                objectGetItemSig);

            // JavaScriptRuntime.Closure.Bind(object, object[])
            var runtimeAsmName = typeof(JavaScriptRuntime.Closure).Assembly.GetName();
            var rtAssembly = metadataBuilder.AddAssemblyReference(
                name: metadataBuilder.GetOrAddString(runtimeAsmName.Name!),
                version: runtimeAsmName.Version!,
                culture: default,
                publicKeyOrToken: default,
                flags: 0,
                hashValue: default
            );
            var closureType = metadataBuilder.AddTypeReference(
                rtAssembly,
                metadataBuilder.GetOrAddString("JavaScriptRuntime"),
                metadataBuilder.GetOrAddString("Closure")
            );
            var bindSig = new BlobBuilder();
            new BlobEncoder(bindSig)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(2,
                    returnType => returnType.Type().Object(),
                    parameters => {
                        parameters.AddParameter().Type().Object();
                        parameters.AddParameter().Type().SZArray().Object();
                    });
            var bindSigHandle = metadataBuilder.GetOrAddBlob(bindSig);
            _closureBindObjectRef = metadataBuilder.AddMemberReference(
                closureType,
                metadataBuilder.GetOrAddString("Bind"),
                bindSigHandle);
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

        public void InvokeArrayCtor()
        {
            // we assume the size of the array is already on the stack
            _il.OpCode(ILOpCode.Newobj);
            _il.Token(_arrayCtorRef);
        }

        public void InvokeArrayGetCount()
        {
            // we assume the array is already on the stack
            _il.OpCode(ILOpCode.Callvirt);
            // this can be moved into the runtime as a length property in the future
            _il.Token(_arrayLengthRef);            
        }

        public void InvokeGetItemFromObject()
        {
            // we assume the object and index are already on the stack
            _il.OpCode(ILOpCode.Call);
            _il.Token(_objectGetItem);
        }

        public void InvokeOperatorsAdd()
        {
            // assumes two object operands are on the stack
            _il.OpCode(ILOpCode.Call);
            _il.Token(_operatorsAddRef);
        }

        public void InvokeClosureBindObject()
        {
            // assumes [delegateAsObject] [scopesArray] are on the stack
            _il.OpCode(ILOpCode.Call);
            _il.Token(_closureBindObjectRef);
        }
    }
}
