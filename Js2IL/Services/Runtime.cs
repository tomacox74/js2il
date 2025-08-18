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
        private readonly MetadataBuilder _metadataBuilder;
        private AssemblyReferenceHandle _runtimeAssemblyReference;
        private readonly Dictionary<string, TypeReferenceHandle> _runtimeTypeCache = new(StringComparer.Ordinal);
        private MemberReferenceHandle _consoleLogMethodRef;
        private MemberReferenceHandle _objectGetItem;
        private MemberReferenceHandle _arrayCtorRef;
        private MemberReferenceHandle _arrayLengthRef;
        private MemberReferenceHandle _closureBindObjectRef;
        private InstructionEncoder _il;
        private MemberReferenceHandle _operatorsAddRef;
        private MemberReferenceHandle _operatorsSubtractRef;

        public Runtime(MetadataBuilder metadataBuilder, InstructionEncoder il) 
        { 
            _il = il;
            _metadataBuilder = metadataBuilder;

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
            _runtimeAssemblyReference = runtimeAssemblyReference;

            var dotNet2JsType = metadataBuilder.AddTypeReference(
                runtimeAssemblyReference,
                metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime)),
                metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime.DotNet2JSConversions))
            );
            // Initialize references to JavaScriptRuntime.Operators methods
            InitializeOperators();

            // Initialize JavaScriptRuntime.Console.Log
            InitializeConsole();

            // Initialize JavaScriptRuntime.Array method references
            InitializeArray();


            // Initialize JavaScriptRuntime.Object.GetItem
            InitializeObject();

            // Initialize JavaScriptRuntime.Closure.Bind
            InitializeClosure();
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

        public void InvokeOperatorsSubtract()
        {
            // assumes two object operands are on the stack
            _il.OpCode(ILOpCode.Call);
            _il.Token(_operatorsSubtractRef);
        }

        public void InvokeClosureBindObject()
        {
            // assumes [delegateAsObject] [scopesArray] are on the stack
            _il.OpCode(ILOpCode.Call);
            _il.Token(_closureBindObjectRef);
        }

        /// <summary>
        /// Initializes references for JavaScriptRuntime.Operators methods (Add/Subtract).
        /// </summary>
        private void InitializeOperators()
        {
            var operatorsType = _metadataBuilder.AddTypeReference(
                _runtimeAssemblyReference,
                _metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime)),
                _metadataBuilder.GetOrAddString("Operators")
            );

            // JavaScriptRuntime.Operators.Add(object, object) -> object
            var addSigBuilder = new BlobBuilder();
            new BlobEncoder(addSigBuilder)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(2,
                    returnType => returnType.Type().Object(),
                    parameters =>
                    {
                        parameters.AddParameter().Type().Object();
                        parameters.AddParameter().Type().Object();
                    });
            var addSig = _metadataBuilder.GetOrAddBlob(addSigBuilder);
            _operatorsAddRef = _metadataBuilder.AddMemberReference(
                operatorsType,
                _metadataBuilder.GetOrAddString("Add"),
                addSig);

            // JavaScriptRuntime.Operators.Subtract(object, object) -> object
            var subSigBuilder = new BlobBuilder();
            new BlobEncoder(subSigBuilder)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(2,
                    returnType => returnType.Type().Object(),
                    parameters =>
                    {
                        parameters.AddParameter().Type().Object();
                        parameters.AddParameter().Type().Object();
                    });
            var subSig = _metadataBuilder.GetOrAddBlob(subSigBuilder);
            _operatorsSubtractRef = _metadataBuilder.AddMemberReference(
                operatorsType,
                _metadataBuilder.GetOrAddString("Subtract"),
                subSig);
        }

        /// <summary>
        /// Initializes reference for JavaScriptRuntime.Console.Log(object[]).
        /// </summary>
        private void InitializeConsole()
        {
            var consoleLogSigBuilder = new BlobBuilder();
            new BlobEncoder(consoleLogSigBuilder)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(1,
                    returnType => returnType.Void(),
                    parameters =>
                    {
                        parameters.AddParameter().Type().SZArray().Object();
                    });
            var consoleLogSig = _metadataBuilder.GetOrAddBlob(consoleLogSigBuilder);

            var consoleType = _metadataBuilder.AddTypeReference(
                _runtimeAssemblyReference,
                _metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime)),
                _metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime.Console))
            );

            _consoleLogMethodRef = _metadataBuilder.AddMemberReference(
                consoleType,
                _metadataBuilder.GetOrAddString("Log"),
                consoleLogSig);
        }

        /// <summary>
        /// Initializes reference for JavaScriptRuntime.Object.GetItem(object, double) -> object.
        /// </summary>
        private void InitializeObject()
        {
            var objectType = _metadataBuilder.AddTypeReference(
                _runtimeAssemblyReference,
                _metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime)),
                _metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime.Object)));

            var objectGetItemSigBuilder = new BlobBuilder();
            new BlobEncoder(objectGetItemSigBuilder)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(2, rt => rt.Type().Object(), p =>
                {
                    p.AddParameter().Type().Object();
                    p.AddParameter().Type().Double();
                });
            var objectGetItemSig = _metadataBuilder.GetOrAddBlob(objectGetItemSigBuilder);
            _objectGetItem = _metadataBuilder.AddMemberReference(
                objectType,
                _metadataBuilder.GetOrAddString("GetItem"),
                objectGetItemSig);
        }

        /// <summary>
        /// Initializes reference for JavaScriptRuntime.Closure.Bind(object, object[]) -> object.
        /// </summary>
        private void InitializeClosure()
        {
            // Use the same runtime assembly reference we already captured
            var closureType = _metadataBuilder.AddTypeReference(
                _runtimeAssemblyReference,
                _metadataBuilder.GetOrAddString("JavaScriptRuntime"),
                _metadataBuilder.GetOrAddString("Closure")
            );

            var bindSig = new BlobBuilder();
            new BlobEncoder(bindSig)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(2,
                    rt => rt.Type().Object(),
                    p =>
                    {
                        p.AddParameter().Type().Object();
                        p.AddParameter().Type().SZArray().Object();
                    });
            var bindSigHandle = _metadataBuilder.GetOrAddBlob(bindSig);
            _closureBindObjectRef = _metadataBuilder.AddMemberReference(
                closureType,
                _metadataBuilder.GetOrAddString("Bind"),
                bindSigHandle);
        }

        /// <summary>
        /// Initializes references for JavaScriptRuntime.Array (.ctor and get_length).
        /// </summary>
        private void InitializeArray()
        {
            var arrayType = _metadataBuilder.AddTypeReference(
                _runtimeAssemblyReference,
                _metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime)),
                _metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime.Array)));

            // Constructor: .ctor(int capacity)
            var arraySigBuilder = new BlobBuilder();
            new BlobEncoder(arraySigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(1, rt => rt.Void(), p =>
                {
                    p.AddParameter().Type().Int32();
                });
            var arrayCtorSig = _metadataBuilder.GetOrAddBlob(arraySigBuilder);
            _arrayCtorRef = _metadataBuilder.AddMemberReference(
                arrayType,
                _metadataBuilder.GetOrAddString(".ctor"),
                arrayCtorSig);

            // Property getter: double get_length()
            var arrayLengthSigBuilder = new BlobBuilder();
            new BlobEncoder(arrayLengthSigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(0, rt => rt.Type().Double(), p => { });
            var arrayLengthSig = _metadataBuilder.GetOrAddBlob(arrayLengthSigBuilder);
            _arrayLengthRef = _metadataBuilder.AddMemberReference(
                arrayType,
                _metadataBuilder.GetOrAddString("get_length"),
                arrayLengthSig);
        }

        /// <summary>
        /// Gets (and caches) a TypeReferenceHandle to a type in the JavaScriptRuntime assembly.
        /// </summary>
        private TypeReferenceHandle GetJavaScriptRuntimeType(string typeName)
        {
            if (_runtimeTypeCache.TryGetValue(typeName, out var handle))
            {
                return handle;
            }
            var tref = _metadataBuilder.AddTypeReference(
                _runtimeAssemblyReference,
                _metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime)),
                _metadataBuilder.GetOrAddString(typeName));
            _runtimeTypeCache[typeName] = tref;
            return tref;
        }

        /// <summary>
        /// Returns a MemberReferenceHandle for the .ctor of a JavaScriptRuntime Error type (Error, TypeError, etc.).
        /// Supports 0 or 1 string parameter.
        /// </summary>
        public MemberReferenceHandle GetErrorCtorRef(string errorTypeName, int argumentCount)
        {
            if (argumentCount < 0 || argumentCount > 1)
            {
                throw new NotSupportedException($"Only up to 1 constructor argument supported for built-in Error types (got {argumentCount})");
            }

            var errorTypeRef = GetJavaScriptRuntimeType(errorTypeName);

            var sig = new BlobBuilder();
            new BlobEncoder(sig)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(argumentCount,
                    r => r.Void(),
                    p =>
                    {
                        for (int i = 0; i < argumentCount; i++)
                        {
                            // Error(string message)
                            p.AddParameter().Type().String();
                        }
                    });
            var ctorSig = _metadataBuilder.GetOrAddBlob(sig);
            return _metadataBuilder.AddMemberReference(errorTypeRef, _metadataBuilder.GetOrAddString(".ctor"), ctorSig);
        }

        /// <summary>
        /// Returns a TypeReferenceHandle for JavaScriptRuntime.Error (base JS error type).
        /// </summary>
        public TypeReferenceHandle GetErrorTypeRef()
        {
            return GetJavaScriptRuntimeType("Error");
        }
    }
}
