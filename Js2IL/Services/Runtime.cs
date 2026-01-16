using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Acornima.Ast;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services
{
    internal sealed class Runtime
    {
        private readonly Dictionary<string, Type> _runtimeTypeNameCache = new(StringComparer.Ordinal);
        private readonly InstructionEncoder _il;
        private readonly TypeReferenceRegistry _typeRefRegistry;
        private readonly MemberReferenceRegistry _memberRefRegistry;

        public Runtime(InstructionEncoder il, TypeReferenceRegistry typeRefRegistry, MemberReferenceRegistry memberRefRegistry)
        {
            _il = il;
            _typeRefRegistry = typeRefRegistry;
            _memberRefRegistry = memberRefRegistry;
        }

        public void InvokeArrayCtor()
        {
            // we assume the size of the array is already on the stack
            var ctorRef = _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.Array), new[] { typeof(int) });
            _il.OpCode(ILOpCode.Newobj);
            _il.Token(ctorRef);
        }

        public void InvokeEngineCtor()
        {
            // creates a new instance of JavaScriptRuntime.Engine
            var ctorRef = _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.Engine), Type.EmptyTypes);
            _il.OpCode(ILOpCode.Newobj);
            _il.Token(ctorRef);
        }

        public void InvokeGetItemFromObject()
        {
            // we assume the object and index are already on the stack
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetItem));
            _il.Call(mref);
        }

        public void InvokeGetLengthFromObject()
        {
            // we assume the object is already on the stack
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetLength));
            _il.Call(mref);
        }

        public void InvokeNormalizeForOfIterable()
        {
            // we assume the object is already on the stack
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.NormalizeForOfIterable));
            _il.Call(mref);
        }

        public void InvokeGetEnumerableKeysFromObject()
        {
            // we assume the object is already on the stack
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetEnumerableKeys));
            _il.Call(mref);
        }

        public void InvokeOperatorsAdd()
        {
            // assumes two object operands are on the stack
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.Add));
            _il.Call(mref);
        }

        public void InvokeOperatorsSubtract()
        {
            // assumes two object operands are on the stack
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.Subtract));
            _il.Call(mref);
        }

        public void InvokeClosureBindObject()
        {
            // assumes [delegateAsObject] [scopesArray] are on the stack
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), nameof(JavaScriptRuntime.Closure.Bind), new[] { typeof(object), typeof(object[]) });
            _il.Call(mref);
        }

        public void InvokeClosureInvokeWithArgs()
        {
            // assumes [delegateAsObject] [scopesArray] [argsArray] are on the stack
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), nameof(JavaScriptRuntime.Closure.InvokeWithArgs), new[] { typeof(object), typeof(object[]), typeof(object[]) });
            _il.Call(mref);
        }

        // Legacy intrinsic invocation helper (used by the removed legacy IL expression emitter) was deleted.

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

            // Get or cache the Type for this error type name
            if (!_runtimeTypeNameCache.TryGetValue(errorTypeName, out var errorType))
            {
                // Use reflection to find the type in JavaScriptRuntime assembly
                var runtimeAssembly = typeof(JavaScriptRuntime.Console).Assembly;
                var fullTypeName = $"JavaScriptRuntime.{errorTypeName}";
                errorType = runtimeAssembly.GetType(fullTypeName)
                    ?? throw new InvalidOperationException($"Could not resolve error type '{fullTypeName}' in JavaScriptRuntime assembly");
                _runtimeTypeNameCache[errorTypeName] = errorType;
            }

            // Build parameter types array based on argumentCount
            Type[] parameterTypes = argumentCount switch
            {
                0 => Type.EmptyTypes,
                1 => new[] { typeof(string) },
                _ => throw new NotSupportedException($"Unsupported argument count: {argumentCount}")
            };

            // Use MemberReferenceRegistry to get the constructor
            return _memberRefRegistry.GetOrAddConstructor(errorType, parameterTypes);
        }

        /// <summary>
        /// Returns a TypeReferenceHandle for JavaScriptRuntime.Error (base JS error type).
        /// </summary>
        public TypeReferenceHandle GetErrorTypeRef()
        {
            return _typeRefRegistry.GetOrAdd(typeof(JavaScriptRuntime.Error));
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

        public MemberReferenceHandle GetConstructorRef(Type runtimeType, params Type[] parameterTypes)
        {
            return _memberRefRegistry.GetOrAddConstructor(runtimeType, parameterTypes);
        }

        public MemberReferenceHandle GetInstanceMethodRef(Type runtimeType, string methodName, int notUsed, params Type[] parameterTypes)
        {
            // Delegate to MemberReferenceRegistry which uses reflection to discover the signature automatically
            return _memberRefRegistry.GetOrAddMethod(runtimeType, methodName, parameterTypes);
        }

        public MemberReferenceHandle GetStaticMethodRef(Type runtimeType, string methodName, int notUsed, params Type[] parameterTypes)
        {
            // Delegate to MemberReferenceRegistry which uses reflection to discover the signature automatically
            return _memberRefRegistry.GetOrAddMethod(runtimeType, methodName, parameterTypes);
        }

        public void InvokeRequire()
        {
            var mref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.CommonJS.RequireDelegate), nameof(JavaScriptRuntime.CommonJS.RequireDelegate.Invoke));
            _il.OpCode(ILOpCode.Callvirt);
            _il.Token(mref);
        }
    }
}
