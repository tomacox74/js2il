using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services
{
    internal class Runtime
    {
        private static readonly string RuntimeAssemblyName = typeof(JavaScriptRuntime.Console).Assembly.GetName().Name!;

        private readonly MetadataBuilder _metadataBuilder;
        private readonly Dictionary<string, TypeReferenceHandle> _runtimeTypeCache = new(StringComparer.Ordinal);
        private readonly Dictionary<string, TypeReferenceHandle> _runtimeTypeCacheByFullName = new(StringComparer.Ordinal);
        private readonly Dictionary<string, MemberReferenceHandle> _runtimeMethodCache = new(StringComparer.Ordinal);
        private readonly InstructionEncoder _il;
        private MemberReferenceHandle _objectGetItem;
        private MemberReferenceHandle _objectGetLength;
        private MemberReferenceHandle _arrayCtorRef;
        private MemberReferenceHandle _arrayLengthRef;
        private MemberReferenceHandle _closureBindObjectRef;
        private MemberReferenceHandle _closureInvokeWithArgsRef;
        private MemberReferenceHandle _operatorsAddRef;
        private MemberReferenceHandle _operatorsSubtractRef;
        private MemberReferenceHandle _engineCtorRef;
        private readonly AssemblyReferenceRegistry _assemblyRefRegistry;

        public Runtime(MetadataBuilder metadataBuilder, InstructionEncoder il, AssemblyReferenceRegistry assemblyRefRegistry) 
        { 
            _il = il;
            _metadataBuilder = metadataBuilder;
            _assemblyRefRegistry = assemblyRefRegistry ?? throw new ArgumentNullException(nameof(assemblyRefRegistry));
            // Initialize references to JavaScriptRuntime.Operators methods
            InitializeOperators();

            // Initialize JavaScriptRuntime.Array method references
            InitializeArray();


            // Initialize JavaScriptRuntime.Object.GetItem
            InitializeObject();

            // Initialize JavaScriptRuntime.Closure.Bind
            InitializeClosure();

            // Initialize JavaScriptRuntime.Engine
            InitializeEngine();
        }

        public void InvokeArrayCtor()
        {
            // we assume the size of the array is already on the stack
            _il.OpCode(ILOpCode.Newobj);
            _il.Token(_arrayCtorRef);
        }

        public void InvokeEngineCtor()
        {
            // creates a new instance of JavaScriptRuntime.Engine
            _il.OpCode(ILOpCode.Newobj);
            _il.Token(_engineCtorRef);
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
            _il.Call(_objectGetItem);
        }

        public void InvokeGetLengthFromObject()
        {
            // we assume the object is already on the stack
            _il.Call(_objectGetLength);
        }

        public void InvokeOperatorsAdd()
        {
            // assumes two object operands are on the stack
            _il.Call(_operatorsAddRef);
        }

        public void InvokeOperatorsSubtract()
        {
            // assumes two object operands are on the stack
            _il.Call(_operatorsSubtractRef);
        }

        public void InvokeClosureBindObject()
        {
            // assumes [delegateAsObject] [scopesArray] are on the stack
            _il.Call(_closureBindObjectRef);
        }

        public void InvokeClosureInvokeWithArgs()
        {
            // assumes [delegateAsObject] [scopesArray] [argsArray] are on the stack
            _il.Call(_closureInvokeWithArgsRef);
        }

        /// <summary>
        /// Initializes references for JavaScriptRuntime.Operators methods (Add/Subtract).
        /// </summary>
        private void InitializeOperators()
        {
            var runtimeAsmRef = _assemblyRefRegistry.GetOrAdd(RuntimeAssemblyName);
            var operatorsType = _metadataBuilder.AddTypeReference(
                runtimeAsmRef,
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
        /// Initializes reference for JavaScriptRuntime.Object.GetItem(object, object) -> object.
        /// </summary>
        private void InitializeObject()
        {
            var objectType = this.GetRuntimeTypeHandle(typeof(JavaScriptRuntime.Object));

            var objectGetItemSigBuilder = new BlobBuilder();
            new BlobEncoder(objectGetItemSigBuilder)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(2, rt => rt.Type().Object(), p =>
                {
                    p.AddParameter().Type().Object();
                    p.AddParameter().Type().Object();
                });
            var objectGetItemSig = _metadataBuilder.GetOrAddBlob(objectGetItemSigBuilder);
            _objectGetItem = _metadataBuilder.AddMemberReference(
                objectType,
                _metadataBuilder.GetOrAddString("GetItem"),
                objectGetItemSig);

            // JavaScriptRuntime.Object.GetLength(object) -> double
            var objectGetLengthSigBuilder = new BlobBuilder();
            new BlobEncoder(objectGetLengthSigBuilder)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(1, rt => rt.Type().Double(), p =>
                {
                    p.AddParameter().Type().Object();
                });
            var objectGetLengthSig = _metadataBuilder.GetOrAddBlob(objectGetLengthSigBuilder);
            _objectGetLength = _metadataBuilder.AddMemberReference(
                objectType,
                _metadataBuilder.GetOrAddString("GetLength"),
                objectGetLengthSig);
        }

        /// <summary>
        /// Initializes reference for JavaScriptRuntime.Closure.Bind(object, object[]) -> object.
        /// </summary>
        private void InitializeClosure()
        {
            var runtimeAsmRef = _assemblyRefRegistry.GetOrAdd(RuntimeAssemblyName);
            var closureType = _metadataBuilder.AddTypeReference(
                runtimeAsmRef,
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

            // Add reference for InvokeWithArgs(object, object[], params object[]) -> object
            var invokeWithArgsSig = new BlobBuilder();
            new BlobEncoder(invokeWithArgsSig)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(3,
                    rt => rt.Type().Object(),
                    p =>
                    {
                        p.AddParameter().Type().Object(); // target
                        p.AddParameter().Type().SZArray().Object(); // scopes
                        p.AddParameter().Type().SZArray().Object(); // args (params)
                    });
            var invokeWithArgsSigHandle = _metadataBuilder.GetOrAddBlob(invokeWithArgsSig);
            _closureInvokeWithArgsRef = _metadataBuilder.AddMemberReference(
                closureType,
                _metadataBuilder.GetOrAddString("InvokeWithArgs"),
                invokeWithArgsSigHandle);
        }

        /// <summary>
        /// Initializes references for JavaScriptRuntime.Array (.ctor and get_length).
        /// </summary>
        private void InitializeArray()
        {
            var runtimeAsmRef = _assemblyRefRegistry.GetOrAdd(RuntimeAssemblyName);
            var arrayType = _metadataBuilder.AddTypeReference(
                runtimeAsmRef,
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
        /// Initializes reference for JavaScriptRuntime.Engine (.ctor).
        /// </summary>
        private void InitializeEngine()
        {
            var engineType = GetRuntimeTypeHandle(typeof(JavaScriptRuntime.Engine));

            // Constructor: .ctor()
            var engineCtorSigBuilder = new BlobBuilder();
            new BlobEncoder(engineCtorSigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(0, rt => rt.Void(), p => { });
            var engineCtorSig = _metadataBuilder.GetOrAddBlob(engineCtorSigBuilder);
            _engineCtorRef = _metadataBuilder.AddMemberReference(
                engineType,
                _metadataBuilder.GetOrAddString(".ctor"),
                engineCtorSig);
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
            var runtimeAsmRef = _assemblyRefRegistry.GetOrAdd(RuntimeAssemblyName);
            var tref = _metadataBuilder.AddTypeReference(
                runtimeAsmRef,
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

        private void EncodeSignatureType(SignatureTypeEncoder enc, Type type)
        {
            if (type == typeof(object)) enc.Object();
            else if (type == typeof(string)) enc.String();
            else if (type == typeof(double)) enc.Double();
            else if (type == typeof(bool)) enc.Boolean();
            else if (type == typeof(int)) enc.Int32();
            else if (type == typeof(object[])) enc.SZArray().Object();
            else if (type == typeof(Action))
            {
                // System.Action is from System.Runtime, not JavaScriptRuntime
                var systemRuntimeRef = _assemblyRefRegistry.GetOrAdd("System.Runtime");
                var actionTypeRef = _metadataBuilder.AddTypeReference(
                    systemRuntimeRef,
                    _metadataBuilder.GetOrAddString("System"),
                    _metadataBuilder.GetOrAddString("Action"));
                enc.Type(actionTypeRef, isValueType: false);
            }
            else if (type.Namespace?.StartsWith("JavaScriptRuntime", StringComparison.Ordinal) == true)
            {
                // Map JavaScriptRuntime reference types (e.g., JavaScriptRuntime.Array, JavaScriptRuntime.Node.Process)
                var tref = GetRuntimeTypeRef(type);
                enc.Type(tref, isValueType: type.IsValueType);
            }
            else
            {
                throw new NotSupportedException($"Type '{type.FullName ?? type.Name}' from namespace '{type.Namespace}' is not supported in method signatures. Only JavaScriptRuntime types and primitive BCL types (object, string, double, bool, int, object[], Action) are supported.");
            }
        }

        private void EncodeReturnType(ReturnTypeEncoder enc, Type type)
        {
            if (type == typeof(void)) enc.Void();
            else EncodeSignatureType(enc.Type(), type);
        }

        private string MakeMethodKey(string fullTypeName, string methodName, bool isInstance, Type ret, IReadOnlyList<Type> parms)
        {
            var ps = string.Join(",", parms.Select(p => p.FullName));
            return $"{fullTypeName}::{methodName}|{(isInstance ? "inst" : "static")}|ret={ret.FullName}|params=[{ps}]";
        }

        // Nested-type aware resolver for JavaScriptRuntime types (e.g., JavaScriptRuntime.Node.PerfHooks+Performance)
        private TypeReferenceHandle GetRuntimeTypeRef(Type runtimeType)
        {
            var fullName = runtimeType.FullName ?? ((runtimeType.Namespace ?? "") + "." + runtimeType.Name);
            if (_runtimeTypeCacheByFullName.TryGetValue(fullName, out var existing)) return existing;

            TypeReferenceHandle tref;
            if (runtimeType.DeclaringType == null)
            {
                // Top-level type under JavaScriptRuntime assembly
                var runtimeAsmRef = _assemblyRefRegistry.GetOrAdd(RuntimeAssemblyName);
                tref = _metadataBuilder.AddTypeReference(
                    runtimeAsmRef,
                    _metadataBuilder.GetOrAddString(runtimeType.Namespace ?? "JavaScriptRuntime"),
                    _metadataBuilder.GetOrAddString(runtimeType.Name));
            }
            else
            {
                // Nested type: resolution scope is the declaring type TypeReference
                var parentRef = GetRuntimeTypeRef(runtimeType.DeclaringType);
                tref = _metadataBuilder.AddTypeReference(
                    parentRef,
                    // Nested types in metadata do not carry a namespace; use empty here and set only the nested name
                    _metadataBuilder.GetOrAddString(string.Empty),
                    _metadataBuilder.GetOrAddString(runtimeType.Name));
            }

            _runtimeTypeCacheByFullName[fullName] = tref;
            return tref;
        }

        // Public helper to get a type reference handle for a JavaScriptRuntime type
        public TypeReferenceHandle GetRuntimeTypeHandle(Type runtimeType)
        {
            return GetRuntimeTypeRef(runtimeType);
        }

        public MemberReferenceHandle GetInstanceMethodRef(Type runtimeType, string methodName, Type returnType, params Type[] parameterTypes)
        {
            // Use FullName so nested types (which use '+') are uniquely identified
            var full = runtimeType.FullName ?? ((runtimeType.Namespace ?? "JavaScriptRuntime") + "." + runtimeType.Name);
            var key = MakeMethodKey(full, methodName, isInstance: true, ret: returnType, parms: parameterTypes);
            if (_runtimeMethodCache.TryGetValue(key, out var cached)) return cached;

            // Resolve type reference with nested-type awareness
            var typeRef = GetRuntimeTypeRef(runtimeType);
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(parameterTypes.Length, rt => EncodeReturnType(rt, returnType), p =>
                {
                    foreach (var pt in parameterTypes)
                    {
                        EncodeSignatureType(p.AddParameter().Type(), pt);
                    }
                });
            var sigHandle = _metadataBuilder.GetOrAddBlob(sigBuilder);
            var mref = _metadataBuilder.AddMemberReference(typeRef, _metadataBuilder.GetOrAddString(methodName), sigHandle);
            _runtimeMethodCache[key] = mref;
            return mref;
        }

        public MemberReferenceHandle GetStaticMethodRef(Type runtimeType, string methodName, Type returnType, params Type[] parameterTypes)
        {
            // Use FullName so nested types (which use '+') are uniquely identified
            var full = runtimeType.FullName ?? ((runtimeType.Namespace ?? "JavaScriptRuntime") + "." + runtimeType.Name);
            var key = MakeMethodKey(full, methodName, isInstance: false, ret: returnType, parms: parameterTypes);
            if (_runtimeMethodCache.TryGetValue(key, out var cached)) return cached;

            // Resolve type reference with nested-type awareness
            var typeRef = GetRuntimeTypeRef(runtimeType);
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature(isInstanceMethod: false)
                .Parameters(parameterTypes.Length, rt => EncodeReturnType(rt, returnType), p =>
                {
                    foreach (var pt in parameterTypes)
                    {
                        EncodeSignatureType(p.AddParameter().Type(), pt);
                    }
                });
            var sigHandle = _metadataBuilder.GetOrAddBlob(sigBuilder);
            var mref = _metadataBuilder.AddMemberReference(typeRef, _metadataBuilder.GetOrAddString(methodName), sigHandle);
            _runtimeMethodCache[key] = mref;
            return mref;
        }

        public MemberReferenceHandle GetInstanceMethodRef(System.Reflection.MethodInfo method)
        {
            var dt = method.DeclaringType ?? throw new ArgumentException("Method has no declaring type", nameof(method));
            var ret = method.ReturnType;
            var pars = method.GetParameters().Select(p => p.ParameterType).ToArray();
            return GetInstanceMethodRef(dt, method.Name, ret, pars);
        }

        // Convenience: Emit a call to JavaScriptRuntime.Require.require(string) with string already on stack
        public void InvokeRequire()
        {
            var mref = GetStaticMethodRef(typeof(JavaScriptRuntime.Require), "require", typeof(object), typeof(string));
            _il.OpCode(ILOpCode.Call);
            _il.Token(mref);
        }
    }
}
