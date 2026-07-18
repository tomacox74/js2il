using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jroc.Runtime;

namespace JavaScriptRuntime
{
    /// <summary>
    /// Minimal Function.prototype helpers for delegate-backed function values.
    ///
    /// JROC models JavaScript functions as CLR delegates; these helpers provide a small subset
    /// of Function.prototype behavior (apply/bind) for those values.
    /// </summary>
[IntrinsicObject("Function")]
public static class Function
{
    private sealed class InvocationMetadataSlot
    {
        public bool RequiresInvocationContext;
    }

    private sealed class UndefinedPrototypeSlot
    {
    }

    private sealed class WithObjectSlot
    {
        public object? Value;
    }

    private static readonly ConditionalWeakTable<Delegate, InvocationMetadataSlot> _invocationMetadata = new();
    private static readonly ConditionalWeakTable<Delegate, UndefinedPrototypeSlot> _undefinedPrototypeFunctions = new();
    private static readonly ConditionalWeakTable<Delegate, WithObjectSlot> _withObjectBindings = new();
    private static readonly Func<object[], object?[], object?> _restrictedPropertyThrower =
        static (_, _) => throw new TypeError("Cannot access restricted function property");

    internal static readonly JsObject Prototype = CreatePrototype();
    internal static readonly JsObject RestrictedPropertiesPrototype = CreateRestrictedPropertiesPrototype();

    private static JsObject CreatePrototype()
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        var prototype = new JsObject();
        DefinePrototypeMethod(prototype, "apply", (Func<object[], object?[]?, object?>)PrototypeApply, 2);
        DefinePrototypeMethod(prototype, "call", (Func<object[], object?[]?, object?>)PrototypeCall, 1);
        DefinePrototypeMethod(prototype, "bind", (Func<object[], object?[]?, object?>)PrototypeBind, 1);
        DefinePrototypeMethod(prototype, "toString", (Func<object[], object?[]?, object?>)PrototypeToString, 0);
        DefineRestrictedProperty(prototype, "caller");
        DefineRestrictedProperty(prototype, "arguments");
        return prototype;
    }

    private static void DefinePrototypeMethod(JsObject prototype, string name, Func<object[], object?[]?, object?> method, double length)
    {
        var value = CreateBuiltinPrototypeFunction(method, length);
        PrototypeChain.SetPrototype(value, prototype);
        PropertyDescriptorStore.DefineOrUpdate(prototype, name, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = true,
            Value = value
        });
    }

    internal static bool TryGetPrototypeValue(string name, out object? value)
    {
        if (PropertyDescriptorStore.TryGetOwn(Prototype, name, out var descriptor)
            && descriptor.Kind == JsPropertyDescriptorKind.Data)
        {
            value = descriptor.Value;
            return true;
        }

        value = null;
        return false;
    }

    private static Func<object[], object?[]?, object?> CreateBuiltinPrototypeFunction(Func<object[], object?[]?, object?> method, double length)
    {
        ConfigureCallableObject(method, hasRestrictedProperties: false);
        PropertyDescriptorStore.DefineOrUpdate(method, "length", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = false,
            Value = length
        });
        PropertyDescriptorStore.DefineOrUpdate(method, "prototype", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = false,
            Writable = false,
            Value = null
        });
        return method;
    }

    private static JsObject CreateRestrictedPropertiesPrototype()
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        var prototype = new JsObject();
        DefineRestrictedProperty(prototype, "caller");
        DefineRestrictedProperty(prototype, "arguments");
        return prototype;
    }

    private static void DefineRestrictedProperty(object target, string propertyName)
    {
        PropertyDescriptorStore.DefineOrUpdate(target, propertyName, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Accessor,
            Enumerable = false,
            Configurable = true,
            Get = _restrictedPropertyThrower,
            Set = _restrictedPropertyThrower
        });
    }

    internal static void ConfigureCallableObject(object functionValue, bool hasRestrictedProperties)
    {
        PrototypeChain.SetPrototype(functionValue, Prototype);
        if (hasRestrictedProperties)
        {
            DefineRestrictedFunctionProperties(functionValue);
        }
    }

    internal static void DefineRestrictedFunctionProperties(object functionValue)
    {
        DefineRestrictedProperty(functionValue, "caller");
        DefineRestrictedProperty(functionValue, "arguments");
    }

    internal static bool HasRestrictedFunctionProperties(object? functionValue)
        => functionValue is not null
            && PropertyDescriptorStore.TryGetOwn(functionValue, "caller", out var caller)
            && caller.Kind == JsPropertyDescriptorKind.Accessor
            && PropertyDescriptorStore.TryGetOwn(functionValue, "arguments", out var arguments)
            && arguments.Kind == JsPropertyDescriptorKind.Accessor;

    internal static object? GetEffectiveThisArg(Delegate target, object? thisArg)
    {
        return (thisArg is null || thisArg is JsNull)
            && Closure.UsesEcmaScriptThisBinding(target)
            && !HasRestrictedFunctionProperties(target)
            ? GlobalThis.globalThis
            : thisArg;
    }

        private static bool IsCallableObject(object? target)
            => target is Delegate || target is Proxy proxy && proxy.IsCallableTarget;

        private static object? PrototypeApply(object[] scopes, object?[]? args)
        {
            var target = RuntimeServices.GetCurrentThis();
            if (!IsCallableObject(target))
            {
                throw new TypeError("Function.prototype.apply called on non-function");
            }

            var thisArg = args != null && args.Length > 0 ? args[0] : null;
            var argArray = args != null && args.Length > 1 ? args[1] : null;
            return Apply(target!, thisArg, argArray);
        }

        private static object? PrototypeCall(object[] scopes, object?[]? args)
        {
            var target = RuntimeServices.GetCurrentThis();
            if (!IsCallableObject(target))
            {
                throw new TypeError("Function.prototype.call called on non-function");
            }

            var thisArg = args != null && args.Length > 0 ? args[0] : null;
            var callArgs = args != null && args.Length > 1
                ? args.Skip(1).ToArray()
                : System.Array.Empty<object?>();

            return Call(target!, thisArg, callArgs);
        }

        private static object? PrototypeBind(object[] scopes, object?[]? args)
        {
            var target = RuntimeServices.GetCurrentThis();
            if (target is ClassConstructorValue classConstructorValue)
            {
                var thisArgForClass = args != null && args.Length > 0 ? args[0] : null;
                var boundClassArgs = args != null && args.Length > 1
                    ? args.Skip(1).ToArray()
                    : System.Array.Empty<object?>();

                return BindClassConstructor(classConstructorValue, thisArgForClass, boundClassArgs);
            }

            if (target is Type classConstructor)
            {
                var thisArgForClass = args != null && args.Length > 0 ? args[0] : null;
                var boundClassArgs = args != null && args.Length > 1
                    ? args.Skip(1).ToArray()
                    : System.Array.Empty<object?>();

                return BindClassConstructor(new ClassConstructorValue(classConstructor, RuntimeServices.EmptyScopes), thisArgForClass, boundClassArgs);
            }

            if (target is not Delegate del)
            {
                throw new TypeError("Function.prototype.bind called on non-function");
            }

            var thisArg = args != null && args.Length > 0 ? args[0] : null;
            var boundArgs = args != null && args.Length > 1
                ? args.Skip(1).ToArray()
                : System.Array.Empty<object?>();

            return Bind(del, thisArg, boundArgs);
        }

        private static object BindClassConstructor(ClassConstructorValue target, object? thisArg, object?[] boundArgs)
        {
            var bound = new JsObject();
            bound["Construct"] = (Func<object[], object?[]?, object?>)((_, args) =>
            {
                var callArgs = boundArgs
                    .Concat(args ?? System.Array.Empty<object?>())
                    .Select(arg => arg!)
                    .ToArray();
                return JavaScriptRuntime.Object.ConstructValue(target, callArgs);
            });
            bound["prototype"] = JavaScriptRuntime.Object.GetProperty(target.Type, "prototype");
            PrototypeChain.SetPrototype(bound, Prototype);
            return bound;
        }

        private static object? PrototypeToString(object[] scopes, object?[]? args)
        {
            var target = RuntimeServices.GetCurrentThis();
            if (target is not Delegate del)
            {
                throw new TypeError("Function.prototype.toString called on non-function");
            }

            return ToSourceString(del);
        }

        private static object?[] NormalizeApplyArguments(object? argArray)
        {
            if (argArray is null || argArray is JsNull)
            {
                return System.Array.Empty<object?>();
            }

            if (argArray is JavaScriptRuntime.Array jsArr)
            {
                return jsArr.ToArray();
            }

            if (argArray is object?[] objArr)
            {
                return objArr;
            }

            if (argArray is IEnumerable enumerable && argArray is not string)
            {
                var list = new List<object?>();
                foreach (var item in enumerable)
                {
                    list.Add(item);
                }
                return list.ToArray();
            }

            throw new TypeError("apply arguments must be an array (or null/undefined)");
        }

        public static object? Apply(Delegate target, object? thisArg, object? argArray)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));

            var argsList = NormalizeApplyArguments(argArray);
            var effectiveThis = GetEffectiveThisArg(target, thisArg);
            var prevThis = RuntimeServices.SetCurrentThis(effectiveThis);
            try
            {
                return Closure.InvokeWithArgs(target, System.Array.Empty<object>(), argsList);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(prevThis);
            }
        }

        public static object? Apply(object target, object? thisArg, object? argArray)
        {
            if (target is Delegate del)
            {
                return Apply(del, thisArg, argArray);
            }

            if (target is Proxy proxy && proxy.IsCallableTarget)
            {
                var argsList = NormalizeApplyArguments(argArray);
                var prevThis = RuntimeServices.SetCurrentThis(thisArg);
                try
                {
                    return Closure.InvokeWithArgs(proxy, System.Array.Empty<object>(), argsList);
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(prevThis);
                }
            }

            throw new TypeError("Function.prototype.apply called on non-function");
        }

        public static object? Call(Delegate target, object? thisArg, object?[] args)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));
            args ??= System.Array.Empty<object?>();

            var effectiveThis = GetEffectiveThisArg(target, thisArg);
            var prevThis = RuntimeServices.SetCurrentThis(effectiveThis);
            try
            {
                return Closure.InvokeWithArgs(target, System.Array.Empty<object>(), args);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(prevThis);
            }
        }

        public static object? Call(object target, object? thisArg, object?[] args)
        {
            if (target is Delegate del)
            {
                return Call(del, thisArg, args);
            }

            if (target is Proxy proxy && proxy.IsCallableTarget)
            {
                args ??= System.Array.Empty<object?>();
                var prevThis = RuntimeServices.SetCurrentThis(thisArg);
                try
                {
                    return Closure.InvokeWithArgs(proxy, System.Array.Empty<object>(), args);
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(prevThis);
                }
            }

            throw new TypeError("Function.prototype.call called on non-function");
        }

        public static Func<object[], object?[], object?> Bind(Delegate target, object? thisArg, object?[] boundArgs)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));
            boundArgs = boundArgs is null || boundArgs.Length == 0
                ? System.Array.Empty<object?>()
                : boundArgs.ToArray();

            // Return a callable delegate that supports arbitrary JS arg counts.
            // Signature uses a trailing object[] parameter which Closure.InvokeWithArgs treats
            // as a params-array.
            Func<object[], object?[], object?> boundDelegate = (scopes, runtimeArgs) =>
            {
                runtimeArgs ??= System.Array.Empty<object?>();
                var finalArgs = boundArgs.Length == 0
                    ? runtimeArgs
                    : boundArgs.Concat(runtimeArgs).ToArray();

                var effectiveThis = GetEffectiveThisArg(target, thisArg);
                var prevThis = RuntimeServices.SetCurrentThis(effectiveThis);
                try
                {
                    return Closure.InvokeWithArgs(target, scopes ?? System.Array.Empty<object>(), finalArgs);
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(prevThis);
                }
            };

            ConfigureCallableObject(boundDelegate, hasRestrictedProperties: false);
            CopyInvocationMetadata(target, boundDelegate);
            Closure.TrackFunctionPrototypeBoundDelegate(boundDelegate, target, boundArgs);
            return boundDelegate;
        }

        public static object InitializeFunctionInstance(object functionValue)
        {
            ArgumentNullException.ThrowIfNull(functionValue);

            if (PrototypeChain.GetPrototypeOrNull(functionValue) == null)
            {
                PrototypeChain.SetPrototype(functionValue, Prototype);
            }

            return functionValue;
        }

        public static object InitializeFunctionInstance(object functionValue, double length, string? name)
        {
            return InitializeFunctionInstance(functionValue, length, name, requiresInvocationContext: true);
        }

        public static object InitializeFunctionInstance(object functionValue, double length, string? name, bool requiresInvocationContext)
            => InitializeFunctionInstance(functionValue, length, name, requiresInvocationContext, hasRestrictedProperties: false);

        public static object InitializeFunctionInstance(object functionValue, double length, string? name, bool requiresInvocationContext, bool hasRestrictedProperties)
        {
            InitializeFunctionInstance(functionValue);
            if (hasRestrictedProperties)
            {
                DefineRestrictedFunctionProperties(functionValue);
            }

            if (functionValue is Delegate del)
            {
                SetRequiresInvocationContext(del, requiresInvocationContext);
                DefineMetadataProperty(del, "length", length);
                DefineMetadataProperty(del, "name", name ?? string.Empty);
            }

            return functionValue;
        }

        internal static bool RequiresInvocationContext(Delegate functionValue)
            => _invocationMetadata.TryGetValue(functionValue, out var slot)
                ? slot.RequiresInvocationContext
                : true;

        internal static void CopyInvocationMetadata(Delegate source, Delegate target)
        {
            SetRequiresInvocationContext(target, RequiresInvocationContext(source));
        }

        internal static void SetRequiresInvocationContext(Delegate functionValue, bool requiresInvocationContext)
        {
            var slot = _invocationMetadata.GetOrCreateValue(functionValue);
            slot.RequiresInvocationContext = requiresInvocationContext;
        }

        public static object MarkUndefinedPrototype(object functionValue)
        {
            if (functionValue is Delegate del)
            {
                MarkUndefinedPrototype(del);
            }

            return functionValue;
        }

        internal static void MarkUndefinedPrototype(Delegate functionValue)
        {
            _undefinedPrototypeFunctions.GetOrCreateValue(functionValue);
            PropertyDescriptorStore.Delete(functionValue, "prototype");
        }

        internal static bool HasUndefinedPrototype(Delegate functionValue)
            => _undefinedPrototypeFunctions.TryGetValue(functionValue, out _);

        public static object BindWithObject(object functionValue, object withObject)
        {
            if (functionValue is Delegate del)
            {
                var slot = _withObjectBindings.GetOrCreateValue(del);
                slot.Value = withObject;
            }

            return functionValue;
        }

        internal static bool TryGetBoundWithObject(Delegate functionValue, out object? withObject)
        {
            if (_withObjectBindings.TryGetValue(functionValue, out var slot))
            {
                withObject = slot.Value;
                return withObject is not null;
            }

            withObject = null;
            return false;
        }

        internal static bool HasBoundWithObject(Delegate functionValue)
            => _withObjectBindings.TryGetValue(functionValue, out var slot) && slot.Value is not null;

        internal static string[] ParseDynamicFunctionParameterNames(object?[] args)
        {
            if (args.Length <= 1)
            {
                return System.Array.Empty<string>();
            }

            return string.Join(",", args.Take(args.Length - 1).Select(DotNet2JSConversions.ToString))
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        public static object? SetInferredNameIfAnonymous(object? functionValue, object? propertyKey)
        {
            if (functionValue is not Delegate del)
            {
                return functionValue;
            }

            if (TryEnsureOwnMetadataPropertyDescriptor(del, "name", out var nameDescriptor)
                && nameDescriptor.Value is string existingName
                && !string.IsNullOrEmpty(existingName))
            {
                return functionValue;
            }

            var functionName = propertyKey is Symbol sym
                ? sym.Description is null ? string.Empty : $"[{sym.Description}]"
                : Object.ToPropertyKeyString(propertyKey);
            DefineMetadataProperty(del, "name", functionName);
            return functionValue;
        }

        public static bool IsConstructorReturnOverride(object? value)
            => TypeUtilities.IsConstructorReturnOverride(value);

        internal static void DefineMetadataProperty(Delegate target, string propName, object? value)
        {
            PropertyDescriptorStore.DefineOrUpdate(target, propName, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = value
            });

            ClearDeletedMetadataProperty(target, propName);
        }

        public static object? Construct(Delegate constructor, object?[]? args)
        {
            if (constructor is null) throw new ArgumentNullException(nameof(constructor));
            return Construct(constructor, args, constructor);
        }

        internal static object? Construct(Delegate constructor, object?[]? args, object? newTarget)
        {
            if (constructor is null) throw new ArgumentNullException(nameof(constructor));
            // JS `new` semantics for function constructors:
            // 1) Create a new instance object
            // 2) Set its [[Prototype]] to ctor.prototype when available
            // 3) Invoke the constructor with `this` bound to the instance
            // 4) If ctor returns an object, use that; otherwise return the instance

            var callArgs = args ?? System.Array.Empty<object?>();

            if (Closure.TryGetFunctionPrototypeBoundMetadata(constructor, out var boundTarget, out var boundArgs))
            {
                var finalArgs = boundArgs.Length == 0
                    ? callArgs
                    : boundArgs.Concat(callArgs).ToArray();
                var effectiveNewTarget = ReferenceEquals(newTarget, constructor)
                    ? boundTarget
                    : newTarget;
                return Construct(boundTarget, finalArgs, effectiveNewTarget);
            }

            if (GeneratorObject.IsGeneratorFunctionValue(constructor))
            {
                throw new TypeError("Generator functions are not constructors");
            }

            if (JavaScriptRuntime.Number.IsNumberConstructor(constructor))
            {
                return JavaScriptRuntime.Number.Construct(callArgs, newTarget);
            }

            if (ReferenceEquals(constructor, GlobalThis.String)
                || (GlobalThis.String is Delegate stringConstructor && constructor.Method == stringConstructor.Method))
            {
                return JavaScriptRuntime.String.Construct(callArgs, newTarget);
            }

            if (!JavaScriptRuntime.Object.IsConstructibleValue(constructor))
            {
                throw new TypeError("Value is not a constructor");
            }

            var instance = JavaScriptRuntime.Object.CreateOrdinaryObject();

            // Override the ordinary Object.prototype default only when ctor.prototype is an object.
            // Null and primitive prototype values use Object.prototype per GetPrototypeFromConstructor.
            var proto = JavaScriptRuntime.ObjectRuntime.GetItem(constructor, "prototype");
            if (TypeUtilities.IsConstructorReturnOverride(proto))
            {
                PrototypeChain.SetPrototype(instance, proto);
            }

            var previousThis = RuntimeServices.SetCurrentThis(instance);
            try
            {
                var result = Closure.InvokeWithArgsWithNewTarget(constructor, System.Array.Empty<object>(), newTarget, callArgs);
                return TypeUtilities.IsConstructorReturnOverride(result) ? result : instance;
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        internal static object? ConstructWithReceiver(Delegate constructor, object receiver, object?[]? args, object? newTarget)
        {
            if (constructor is null) throw new ArgumentNullException(nameof(constructor));
            if (receiver is null) throw new ArgumentNullException(nameof(receiver));

            var callArgs = args ?? System.Array.Empty<object?>();

            if (Closure.TryGetFunctionPrototypeBoundMetadata(constructor, out var boundTarget, out var boundArgs))
            {
                var finalArgs = boundArgs.Length == 0
                    ? callArgs
                    : boundArgs.Concat(callArgs).ToArray();
                var effectiveNewTarget = ReferenceEquals(newTarget, constructor)
                    ? boundTarget
                    : newTarget;
                return ConstructWithReceiver(boundTarget, receiver, finalArgs, effectiveNewTarget);
            }

            var previousThis = RuntimeServices.SetCurrentThis(receiver);
            try
            {
                var result = Closure.InvokeWithArgsWithNewTarget(constructor, System.Array.Empty<object>(), newTarget, callArgs);
                return TypeUtilities.IsConstructorReturnOverride(result) ? result : receiver;
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        private static Delegate ResolveFunctionTarget(Delegate target)
        {
            var current = target;
            while (Closure.TryGetBoundTarget(current, out var original)
                && !Closure.IsFunctionPrototypeBoundDelegate(current))
            {
                current = original;
            }

            return current;
        }

        public static double GetLength(Delegate target)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));
            var resolvedTarget = ResolveFunctionTarget(target);

            if (Closure.TryGetFunctionPrototypeBoundMetadata(resolvedTarget, out var boundTarget, out var boundArgs))
            {
                return global::System.Math.Max(0, GetLength(boundTarget) - boundArgs.Length);
            }

            var invoke = resolvedTarget.GetType().GetMethod("Invoke")
                ?? throw new ArgumentException($"Delegate type '{resolvedTarget.GetType()}' does not define Invoke().", nameof(target));
            var parameters = invoke.GetParameters();
            var abi = JsCallableScopeAbiResolver.Resolve(resolvedTarget);
            bool hasScopes = abi.HasExplicitScopePayload;
            bool hasNewTarget = JsCallableScopeAbiResolver.HasNewTargetParameter(parameters, abi.Kind);
            int jsParamStart = hasScopes
                ? (hasNewTarget ? 2 : 1)
                : (hasNewTarget ? 1 : 0);
            int expectedJsParamCount = parameters.Length - jsParamStart;
            bool hasParamsArray = expectedJsParamCount > 0
                && (
                    Attribute.IsDefined(parameters[^1], typeof(ParamArrayAttribute))
                    || (parameters[^1].ParameterType.IsArray && parameters[^1].ParameterType.GetElementType() == typeof(object))
                );

            return global::System.Math.Max(0, hasParamsArray ? expectedJsParamCount - 1 : expectedJsParamCount);
        }

        public static string GetName(Delegate target)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));
            var resolvedTarget = ResolveFunctionTarget(target);

            if (Closure.TryGetFunctionPrototypeBoundMetadata(resolvedTarget, out var boundTarget, out _))
            {
                return "bound " + GetName(boundTarget);
            }

            var name = resolvedTarget.Method.Name;

            if (string.Equals(name, "__js_call__", StringComparison.Ordinal))
            {
                var declaringTypeName = resolvedTarget.Method.DeclaringType?.Name;
                if (IsSyntheticDynamicFunctionDeclaringTypeName(declaringTypeName))
                {
                    return "anonymous";
                }

                if (!string.IsNullOrEmpty(declaringTypeName) && !declaringTypeName.StartsWith("<", StringComparison.Ordinal))
                {
                    return declaringTypeName;
                }
            }

            return string.IsNullOrEmpty(name) ? string.Empty : name;
        }

        internal static object GetPrototypeObject(Delegate target)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));

            var existingPrototype = PrototypeChain.GetPrototypeOrNull(target);
            if (existingPrototype != null)
            {
                return existingPrototype;
            }

            var prototype = IsGeneratorFunction(target)
                ? GeneratorObject.GeneratorFunctionPrototypeObject
                : Prototype;

            PrototypeChain.SetPrototype(target, prototype);
            return prototype;
        }

        internal static bool TryEnsureOwnMetadataPropertyDescriptor(Delegate target, string propName, out JsPropertyDescriptor descriptor)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));

            if (IsMetadataPropertyName(propName) && PropertyDescriptorStore.IsDeleted(target, propName))
            {
                descriptor = default;
                return false;
            }

            if (PropertyDescriptorStore.TryGetOwn(target, propName, out descriptor))
            {
                return true;
            }

            if (string.Equals(propName, "length", StringComparison.Ordinal))
            {
                descriptor = new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Enumerable = false,
                    Configurable = true,
                    Writable = false,
                    Value = GetLength(target)
                };
                PropertyDescriptorStore.DefineOrUpdate(target, propName, descriptor);
                return true;
            }

            if (string.Equals(propName, "name", StringComparison.Ordinal))
            {
                descriptor = new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Enumerable = false,
                    Configurable = true,
                    Writable = false,
                    Value = GetName(target)
                };
                PropertyDescriptorStore.DefineOrUpdate(target, propName, descriptor);
                return true;
            }

            descriptor = default;
            return false;
        }

        internal static bool DeleteOwnProperty(Delegate target, string propName)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));

            PropertyDescriptorStore.Delete(target, propName);

            return true;
        }

        internal static void ClearDeletedMetadataProperty(Delegate target, string propName)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));
        }

        private static bool IsSyntheticDynamicFunctionDeclaringTypeName(string? declaringTypeName)
        {
            const string prefix = "<>DynamicFunction_L";

            if (string.IsNullOrEmpty(declaringTypeName)
                || !declaringTypeName.StartsWith(prefix, StringComparison.Ordinal))
            {
                return false;
            }

            var index = prefix.Length;
            if (index >= declaringTypeName.Length || !char.IsDigit(declaringTypeName[index]))
            {
                return false;
            }

            while (index < declaringTypeName.Length && char.IsDigit(declaringTypeName[index]))
            {
                index++;
            }

            if (index >= declaringTypeName.Length || declaringTypeName[index] != 'C')
            {
                return false;
            }

            index++;
            if (index >= declaringTypeName.Length || !char.IsDigit(declaringTypeName[index]))
            {
                return false;
            }

            while (index < declaringTypeName.Length && char.IsDigit(declaringTypeName[index]))
            {
                index++;
            }

            return index == declaringTypeName.Length;
        }

        private static bool IsGeneratorFunction(Delegate target)
        {
            var declaringType = target.Method.DeclaringType;
            var scopeType = declaringType?.GetNestedType("Scope", BindingFlags.Public | BindingFlags.NonPublic);
            return scopeType != null && typeof(GeneratorScope).IsAssignableFrom(scopeType);
        }

        private static bool IsMetadataPropertyName(string propName)
            => string.Equals(propName, "length", StringComparison.Ordinal)
                || string.Equals(propName, "name", StringComparison.Ordinal);

        public static string ToSourceString(Delegate target)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));
            var name = GetName(target);
            return string.IsNullOrEmpty(name)
                ? "function () { [native code] }"
                : $"function {name}() {{ [native code] }}";
        }
    }
}
