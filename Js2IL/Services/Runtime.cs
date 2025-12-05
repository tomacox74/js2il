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
        private readonly Dictionary<string, MemberReferenceHandle> _runtimeMethodCache = new(StringComparer.Ordinal);
        private readonly InstructionEncoder _il;
        private readonly TypeReferenceRegistry _typeRefRegistry;
        private readonly MemberReferenceRegistry _memberRefRegistry;
        private MemberReferenceHandle _arrayCtorRef;
        private MemberReferenceHandle _engineCtorRef;

        public Runtime(MetadataBuilder metadataBuilder, InstructionEncoder il, TypeReferenceRegistry typeRefRegistry)
        {
            _il = il;
            _metadataBuilder = metadataBuilder;
            _typeRefRegistry = typeRefRegistry;
            _memberRefRegistry = new MemberReferenceRegistry(metadataBuilder, typeRefRegistry);

            // Initialize JavaScriptRuntime.Array method references
            InitializeArray();

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

        public void InvokeGetItemFromObject()
        {
            // we assume the object and index are already on the stack
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Object), "GetItem");
            _il.Call(mref);
        }

        public void InvokeGetLengthFromObject()
        {
            // we assume the object is already on the stack
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Object), "GetLength");
            _il.Call(mref);
        }

        public void InvokeOperatorsAdd()
        {
            // assumes two object operands are on the stack
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), "Add");
            _il.Call(mref);
        }

        public void InvokeOperatorsSubtract()
        {
            // assumes two object operands are on the stack
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), "Subtract");
            _il.Call(mref);
        }

        public void InvokeClosureBindObject()
        {
            // assumes [delegateAsObject] [scopesArray] are on the stack
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), "Bind", new[] { typeof(object), typeof(object[]) });
            _il.Call(mref);
        }

        public void InvokeClosureInvokeWithArgs()
        {
            // assumes [delegateAsObject] [scopesArray] [argsArray] are on the stack
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), "InvokeWithArgs", new[] { typeof(object), typeof(object[]), typeof(object[]) });
            _il.Call(mref);
        }

        /// <summary>
        /// Initializes references for JavaScriptRuntime.Array (.ctor and get_length).
        /// </summary>
        private void InitializeArray()
        {
            var arrayType = _typeRefRegistry.GetOrAdd(typeof(JavaScriptRuntime.Array));

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
            // Build the full type name and use reflection to get the Type
            var fullTypeName = $"JavaScriptRuntime.{typeName}, JavaScriptRuntime";
            var type = Type.GetType(fullTypeName) ?? throw new InvalidOperationException($"Could not resolve type {fullTypeName}");
            var tref = _typeRefRegistry.GetOrAdd(type);
            _runtimeTypeCache[typeName] = tref;
            return tref;
        }

        /// <summary>
        /// Returns a MemberReferenceHandle for the .ctor of a JavaScriptRuntime Error type (Error, TypeError, etc.).
        /// Supports 0 or 1 string parameter.
        /// TODO: Needs to be changed to resolve in the same fashion as other intrinsic objects such as Array, Object, etc. 
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
                var actionTypeRef = _typeRefRegistry.GetOrAdd(typeof(Action));
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
            // Use TypeReferenceRegistry which handles both top-level and nested types
            return _typeRefRegistry.GetOrAdd(runtimeType);
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

        // Convenience: Emit a call to JavaScriptRuntime.Require.require(string) with string already on stack
        public void InvokeRequire()
        {
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Require), "require");
            _il.OpCode(ILOpCode.Call);
            _il.Token(mref);
        }
    }
}
