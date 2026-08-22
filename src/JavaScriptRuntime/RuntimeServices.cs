using JavaScriptRuntime.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime;

public class RuntimeServices
{
    private static readonly System.Threading.AsyncLocal<InvocationFrame?> _currentInvocation = new();
    [ThreadStatic] private static Stack<object?[]?>? _constructorArgStack;
    [ThreadStatic] private static Stack<GeneratedFunctionDirectCallState>? _generatedFunctionDirectCallStack;
    [ThreadStatic] private static Stack<object?>? _constructorNewTargetStack;
    [ThreadStatic] private static Stack<object?>? _derivedConstructorThisStack;
    private static ConditionalWeakTable<Type, LazyClassMetadataSlot> _lazyClassMetadata
        => GetCurrentRealmValueCaches().LazyClassMetadata;
    private static ConcurrentDictionary<ClassConstructorCacheKey, JsClassConstructorObject> _classConstructorValues
        => GetCurrentRealmValueCaches().MaterializedClassConstructors;

    // ABI compatibility: when a callee doesn't need scopes, we still pass a 1-element scopes array.
    // NOTE: Consumers must treat scopes arrays as immutable.
    public static readonly object[] EmptyScopes = new object[1];
    public static readonly object TemporalDeadZoneSentinel = new();

    public static object[] GetEmptyScopes() => EmptyScopes;

    public static double NumberCall(object? value)
        => Number.FromNumberConstructorArgument(value);

    internal sealed record InvocationFrame(
        object? CurrentThis = null,
        object? CurrentLexicalSuperReceiver = null,
        object[]? CurrentLexicalSuperScopes = null,
        object?[]? CurrentArguments = null,
        JsCallArguments? CurrentCallArguments = null,
        object? CurrentNewTarget = null,
        object? CurrentCallee = null)
    {
        internal bool IsEmpty =>
            CurrentThis is null
            && CurrentLexicalSuperReceiver is null
            && CurrentLexicalSuperScopes is null
            && CurrentArguments is null
            && CurrentCallArguments is null
            && CurrentNewTarget is null
            && CurrentCallee is null;
    }

    internal readonly record struct InvocationFrameState(
        InvocationFrame? PreviousFrame,
        bool WasPublished);

    private readonly record struct AmbientInvocationState(
        InvocationFrame? InvocationFrame,
        Stack<object?[]?>? ConstructorArgStack,
        Stack<GeneratedFunctionDirectCallState>? GeneratedFunctionDirectCallStack,
        Stack<object?>? ConstructorNewTargetStack,
        Stack<object?>? DerivedConstructorThisStack);

    private sealed class DerivedConstructorThisBinding
    {
        public object? Value = TemporalDeadZoneSentinel;
    }

    internal sealed class LazyClassMetadataSlot
    {
        public readonly List<LazyClassMethodDataProperty> Methods = new();
    }

    internal sealed class ClassConstructorCacheKey : IEquatable<ClassConstructorCacheKey>
    {
        private readonly Type _type;
        private readonly int _formalParameterCount;
        private readonly object?[] _scopes;
        private readonly int _hashCode;

        public ClassConstructorCacheKey(Type type, object?[] scopes, int formalParameterCount)
        {
            _type = type;
            _formalParameterCount = formalParameterCount;
            _scopes = (object?[])scopes.Clone();

            var hash = new HashCode();
            hash.Add(type);
            hash.Add(formalParameterCount);
            hash.Add(_scopes.Length);
            foreach (var scope in _scopes)
            {
                hash.Add(scope == null ? 0 : RuntimeHelpers.GetHashCode(scope));
            }

            _hashCode = hash.ToHashCode();
        }

        public bool Equals(ClassConstructorCacheKey? other)
        {
            if (other == null
                || !ReferenceEquals(_type, other._type)
                || _formalParameterCount != other._formalParameterCount
                || _scopes.Length != other._scopes.Length)
            {
                return false;
            }

            for (var i = 0; i < _scopes.Length; i++)
            {
                if (!ReferenceEquals(_scopes[i], other._scopes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object? obj) => Equals(obj as ClassConstructorCacheKey);

        public override int GetHashCode() => _hashCode;
    }

    internal sealed record LazyClassMethodDataProperty(
        string PropertyKey,
        string ClrMethodName,
        double Length,
        string FunctionName,
        bool IsStatic,
        bool IsPrivate,
        bool IsGenerator,
        bool IsAsync,
        object[] Scopes);

    private static JsPropertyDescriptor CloneDescriptor(JsPropertyDescriptor descriptor)
    {
        return new JsPropertyDescriptor
        {
            Kind = descriptor.Kind,
            Enumerable = descriptor.Enumerable,
            Configurable = descriptor.Configurable,
            Writable = descriptor.Writable,
            Value = descriptor.Value,
            Get = descriptor.Get,
            Set = descriptor.Set
        };
    }

#if DEBUG
    public static void AssertEmptyScopesUnmodified()
    {
        if (EmptyScopes[0] != null)
        {
            throw new InvalidOperationException("RuntimeServices.EmptyScopes was mutated (expected [0] == null).");
        }
    }
#endif

    public static object? GetCurrentThis()
    {
        return _currentInvocation.Value?.CurrentThis;
    }

    public static object? SetCurrentThis(object? value)
    {
        var current = _currentInvocation.Value;
        var previous = current?.CurrentThis;
        PublishInvocationFrame(
            (current ?? new InvocationFrame()) with
            {
                CurrentThis = value
            });
        return previous;
    }

    internal static object CaptureAndClearAmbientInvocationState()
    {
        var state = new AmbientInvocationState(
            _currentInvocation.Value,
            _constructorArgStack,
            _generatedFunctionDirectCallStack,
            _constructorNewTargetStack,
            _derivedConstructorThisStack);

        PublishInvocationFrame(null);
        _constructorArgStack = null;
        _generatedFunctionDirectCallStack = null;
        _constructorNewTargetStack = null;
        _derivedConstructorThisStack = null;
        return state;
    }

    internal static void RestoreAmbientInvocationState(object state)
    {
        if (state is not AmbientInvocationState invocationState)
        {
            throw new ArgumentException(
                "The invocation state was not created by this runtime.",
                nameof(state));
        }

        PublishInvocationFrame(invocationState.InvocationFrame);
        _constructorArgStack = invocationState.ConstructorArgStack;
        _generatedFunctionDirectCallStack = invocationState.GeneratedFunctionDirectCallStack;
        _constructorNewTargetStack = invocationState.ConstructorNewTargetStack;
        _derivedConstructorThisStack = invocationState.DerivedConstructorThisStack;
    }

    public static object? GetCurrentLexicalSuperReceiver()
    {
        var current = _currentInvocation.Value;
        return current?.CurrentLexicalSuperReceiver
            ?? ResolveLexicalThis(current?.CurrentThis);
    }

    public static object? ResolveGeneratedDirectCallThis(
        JsFunctionObject? functionObject,
        bool usesNonStrictThisBinding,
        object? lexicalThis)
    {
        if (functionObject is not null)
        {
            return functionObject.ResolveThisArgument(null);
        }

        return usesNonStrictThisBinding
            ? GlobalThis.globalThis
            : lexicalThis;
    }

    public static object? ResolveGeneratedDirectCallNewTarget(
        JsFunctionObject? functionObject,
        object? lexicalNewTarget)
        => functionObject is not null
            ? functionObject.ResolveCallNewTarget()
            : lexicalNewTarget;

    public static object? GetCurrentLexicalSuperPropertyReceiver()
    {
        return ResolveLexicalThis(_currentInvocation.Value?.CurrentThis);
    }

    public static object? SetCurrentLexicalSuperReceiver(object? value)
    {
        var current = _currentInvocation.Value;
        var previous = current?.CurrentLexicalSuperReceiver;
        PublishInvocationFrame(
            (current ?? new InvocationFrame()) with
            {
                CurrentLexicalSuperReceiver = value
            });
        return previous;
    }

    public static object[] GetCurrentLexicalSuperScopes()
    {
        return _currentInvocation.Value?.CurrentLexicalSuperScopes
            ?? EmptyScopes;
    }

    public static object[]? SetCurrentLexicalSuperScopes(object[]? value)
    {
        var current = _currentInvocation.Value;
        var previous = current?.CurrentLexicalSuperScopes;
        PublishInvocationFrame(
            (current ?? new InvocationFrame()) with
            {
                CurrentLexicalSuperScopes = value
            });
        return previous;
    }

    internal static InvocationFrameState PushInvocationFrame(
        InvocationContextRequirements requirements,
        object? currentThis,
        in JsCallArguments arguments,
        object? currentCallee,
        object? currentNewTarget,
        object? lexicalSuperReceiver = null,
        object[]? lexicalSuperScopes = null)
    {
        var previous = _currentInvocation.Value;
        var publishesThis =
            (requirements & InvocationContextRequirements.This) != 0;
        var publishesArguments =
            (requirements & InvocationContextRequirements.Arguments) != 0;
        var publishesCallee =
            (requirements & InvocationContextRequirements.Callee) != 0;
        var publishesNewTarget =
            (requirements & InvocationContextRequirements.NewTarget) != 0;
        var publishesLexicalSuper =
            (requirements & InvocationContextRequirements.LexicalSuper) != 0
            && lexicalSuperScopes is not null;
        if (!publishesThis
            && !publishesArguments
            && !publishesCallee
            && !publishesNewTarget
            && !publishesLexicalSuper)
        {
            return default;
        }

        var next = new InvocationFrame(
            publishesThis
                ? currentThis
                : previous?.CurrentThis,
            publishesLexicalSuper
                ? lexicalSuperReceiver
                : previous?.CurrentLexicalSuperReceiver,
            publishesLexicalSuper
                ? lexicalSuperScopes
                : previous?.CurrentLexicalSuperScopes,
            publishesArguments
                ? null
                : previous?.CurrentArguments,
            publishesArguments
                ? arguments
                : previous?.CurrentCallArguments,
            publishesNewTarget
                ? currentNewTarget
                : previous?.CurrentNewTarget,
            publishesCallee
                ? currentCallee
                : previous?.CurrentCallee);
        PublishInvocationFrame(next);
        return new InvocationFrameState(previous, WasPublished: true);
    }

    internal static void RestoreInvocationFrame(
        InvocationFrameState state)
    {
        if (state.WasPublished)
        {
            PublishInvocationFrame(state.PreviousFrame);
        }
    }

    private static void PublishInvocationFrame(InvocationFrame? frame)
    {
        var next = frame is { IsEmpty: true } ? null : frame;
        if (!ReferenceEquals(_currentInvocation.Value, next))
        {
            _currentInvocation.Value = next;
        }
    }

    public static void PushDerivedConstructorThisBinding()
    {
        _derivedConstructorThisStack ??= new Stack<object?>();
        _derivedConstructorThisStack.Push(
            _currentInvocation.Value?.CurrentThis);
        SetCurrentThis(new DerivedConstructorThisBinding());
    }

    public static void InitializeDerivedConstructorThisBinding(object? value)
    {
        if (_currentInvocation.Value?.CurrentThis
            is DerivedConstructorThisBinding binding)
        {
            if (!ReferenceEquals(binding.Value, TemporalDeadZoneSentinel))
            {
                throw new ReferenceError("Super constructor may only be called once");
            }

            binding.Value = value;
            return;
        }

        SetCurrentThis(value);
    }

    public static void ConstructDerivedFunctionBase(object receiver, object constructor, object[] args)
    {
        var newTarget = GetCurrentNewTarget() ?? constructor;
        object? constructed;
        if (constructor is JsFunctionObject functionObject
            && functionObject.IsConstructor)
        {
            constructed = CallableOperations.ConstructWithReceiver(
                functionObject,
                receiver,
                args,
                newTarget);
        }
        else if (constructor is JavaScriptRuntime.Proxy)
        {
            constructed = ObjectRuntime.ConstructValue(constructor, args);
        }
        else
        {
            throw new TypeError($"Class extends value is not a constructor: it has type {TypeUtilities.Typeof(constructor)}.");
        }

        InitializeDerivedConstructorThisBinding(constructed);
    }

    public static void PopDerivedConstructorThisBinding()
    {
        if (_derivedConstructorThisStack is { Count: > 0 } stack)
        {
            SetCurrentThis(stack.Pop());
            return;
        }

        SetCurrentThis(null);
    }

    public static object? ResolveLexicalThis(object? boundThis)
    {
        var value = boundThis is DerivedConstructorThisBinding binding
            ? binding.Value
            : boundThis;

        if (ReferenceEquals(value, TemporalDeadZoneSentinel))
        {
            throw new ReferenceError("Cannot access 'this' before super()");
        }

        return value;
    }

    public static JsClassConstructorObject InitializeClassConstructorObject(
        JsClassConstructorObject constructor,
        Type type,
        object[] scopes,
        double formalParameterCount,
        bool freshIdentity)
    {
        ArgumentNullException.ThrowIfNull(constructor);
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(scopes);

        var length = (int)formalParameterCount;
        constructor.Initialize(type, scopes, length);
        var cacheKey = new ClassConstructorCacheKey(type, scopes, length);
        var materialized = freshIdentity
            ? constructor
            : _classConstructorValues.GetOrAdd(cacheKey, constructor);
        CopyStaticClassDescriptors(type, materialized);
        _ = TryEnsureClassConstructorMetadataPropertyDescriptor(
            materialized,
            "prototype",
            out _);
        return materialized;
    }

    private static void CopyStaticClassDescriptors(
        Type type,
        JsClassConstructorObject constructor)
    {
        foreach (var key in PropertyDescriptorStore.GetOwnKeys(type))
        {
            if (string.Equals(key, "prototype", StringComparison.Ordinal)
                || string.Equals(key, "length", StringComparison.Ordinal))
            {
                continue;
            }

            if (PropertyDescriptorStore.TryGetOwn(type, key, out var descriptor))
            {
                if (string.Equals(key, "name", StringComparison.Ordinal)
                    && descriptor.Kind == JsPropertyDescriptorKind.Data
                    && descriptor.Value is null)
                {
                    continue;
                }

                PropertyDescriptorStore.DefineOrUpdate(
                    constructor,
                    key,
                    CloneDescriptor(descriptor));
            }
        }
    }

    public static object RefreshClassConstructorDescriptors(
        object constructorValue)
    {
        if (constructorValue is not JsClassConstructorObject constructor)
        {
            return constructorValue;
        }

        CopyStaticClassDescriptors(constructor.Type, constructor);
        if (!TryEnsureClassConstructorMetadataPropertyDescriptor(
                constructor,
                "prototype",
                out var constructorPrototype)
            || constructorPrototype.Value is not object targetPrototype
            || !PropertyDescriptorStore.TryGetOwn(
                constructor.Type,
                "prototype",
                out var typePrototype)
            || typePrototype.Value is not object sourcePrototype)
        {
            return constructorValue;
        }

        foreach (var key in PropertyDescriptorStore.GetOwnKeys(sourcePrototype))
        {
            if (PropertyDescriptorStore.TryGetOwn(
                    sourcePrototype,
                    key,
                    out var descriptor))
            {
                PropertyDescriptorStore.DefineOrUpdate(
                    targetPrototype,
                    key,
                    CloneDescriptor(descriptor));
            }
        }
        return constructorValue;
    }

    public static object SetClassConstructorPrototype(object constructorValue, object? baseConstructorValue)
    {
        var validatedBase = ValidateClassHeritage(baseConstructorValue);
        if (constructorValue is JsClassConstructorObject classConstructor)
        {
            PrototypeChain.SetPrototype(classConstructor, validatedBase);
            LinkClassInstancePrototype(classConstructor, validatedBase);
            return classConstructor;
        }

        PrototypeChain.SetPrototype(constructorValue, validatedBase);
        return constructorValue;
    }

    private static void LinkClassInstancePrototype(
        JsClassConstructorObject derivedConstructor,
        object? baseConstructor)
    {
        if (baseConstructor is null || baseConstructor is JsNull)
        {
            return;
        }

        _ = TryEnsureClassConstructorMetadataPropertyDescriptor(
            derivedConstructor,
            "prototype",
            out var derivedPrototypeDescriptor);
        var basePrototype = ObjectRuntime.GetProperty(baseConstructor, "prototype");
        if (derivedPrototypeDescriptor.Value is object derivedPrototype
            && basePrototype is not null
            && basePrototype is not JsNull)
        {
            PrototypeChain.SetPrototype(derivedPrototype, basePrototype);
        }
    }

    public static object? ValidateClassHeritage(object? heritage)
    {
        if (heritage is null || heritage is JsNull)
        {
            return heritage;
        }

        if (!ObjectRuntime.IsConstructibleValue(heritage))
        {
            throw new TypeError("Class extends value is not a constructor or null");
        }

        var prototype = JavaScriptRuntime.ObjectRuntime.GetProperty(heritage, "prototype");
        if (prototype is not null && prototype is not JsNull && !TypeUtilities.IsConstructorReturnOverride(prototype))
        {
            throw new TypeError("Class extends value does not have valid prototype property");
        }

        return heritage;
    }

    public static object RegisterLazyClassMethodDataProperty(
        object ownerValue,
        object keyValue,
        object clrMethodNameValue,
        object lengthValue,
        object functionNameValue,
        object isStaticValue,
        object isPrivateValue,
        object isGeneratorValue,
        object isAsyncValue,
        object scopesValue)
    {
        var ownerType = ResolveClassOwnerType(ownerValue);
        var scopes = scopesValue as object[]
            ?? (ownerValue is JsClassConstructorObject classConstructorValue
                ? classConstructorValue.Scopes
                : EmptyScopes);
        var propertyKey = ObjectRuntime.ToPropertyKeyString(keyValue);
        var clrMethodName = clrMethodNameValue as string
            ?? throw new TypeError("Class method definition requires a CLR method name");
        var metadata = new LazyClassMethodDataProperty(
            propertyKey,
            clrMethodName,
            lengthValue is double d ? d : 0d,
            functionNameValue as string ?? propertyKey,
            TypeUtilities.ToBoolean(isStaticValue),
            TypeUtilities.ToBoolean(isPrivateValue),
            TypeUtilities.ToBoolean(isGeneratorValue),
            TypeUtilities.ToBoolean(isAsyncValue),
            scopes);

        var slot = _lazyClassMetadata.GetOrCreateValue(ownerType);
        lock (slot)
        {
            var existingIndex = slot.Methods.FindIndex(existing =>
                existing.IsStatic == metadata.IsStatic
                && string.Equals(existing.PropertyKey, metadata.PropertyKey, StringComparison.Ordinal));

            if (existingIndex >= 0)
            {
                slot.Methods[existingIndex] = metadata;
            }
            else
            {
                slot.Methods.Add(metadata);
            }
        }

        return ownerValue;
    }

    public static object DefineClassMethodDataProperty(
        object targetValue,
        object keyValue,
        object ownerValue,
        object clrMethodNameValue,
        object lengthValue,
        object functionNameValue,
        object isStaticValue,
        object isPrivateValue,
        object isGeneratorValue,
        object isAsyncValue,
        object scopesValue)
    {
        ArgumentNullException.ThrowIfNull(targetValue);

        var ownerType = ResolveClassOwnerType(ownerValue);
        var scopes = scopesValue as object[]
            ?? (ownerValue is JsClassConstructorObject classConstructorValue
                ? classConstructorValue.Scopes
                : EmptyScopes);
        var clrMethodName = clrMethodNameValue as string
            ?? throw new TypeError("Class method definition requires a CLR method name");
        var key = ObjectRuntime.ToPropertyKeyString(keyValue);
        var functionName = functionNameValue as string ?? key;
        var length = lengthValue is double d ? d : 0d;
        var isStatic = TypeUtilities.ToBoolean(isStaticValue);
        var isPrivate = TypeUtilities.ToBoolean(isPrivateValue);
        var isGenerator = TypeUtilities.ToBoolean(isGeneratorValue);
        var isAsync = TypeUtilities.ToBoolean(isAsyncValue);

        var flags = (isStatic ? BindingFlags.Static : BindingFlags.Instance)
            | BindingFlags.Public
            | BindingFlags.NonPublic;
        var method = ownerType.GetMethod(clrMethodName, flags)
            ?? throw new TypeError($"Class method '{clrMethodName}' was not found on {ownerType.FullName}");

        Func<object[], object?[]?, object?> functionValue = (_, args) =>
            InvokeClassMethodFunction(ownerType, method, scopes, isStatic, isPrivate, args);

        if (isAsync)
        {
            AsyncFunction.InitializeFunctionInstance(functionValue, length, functionName);
        }
        else
        {
            Function.InitializeFunctionInstance(functionValue, length, functionName);
        }
        Function.DefineRestrictedFunctionProperties(functionValue);

        if (isGenerator)
        {
            GeneratorObject.InitializeGeneratorFunctionSurface(functionValue);
        }

        PropertyDescriptorStore.DefineOrUpdate(targetValue, key, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Value = functionValue,
            Writable = true,
            Enumerable = false,
            Configurable = true
        });

        return targetValue;
    }

    public static object DefineClassMethodAccessorProperty(
        object targetValue,
        object keyValue,
        object ownerValue,
        object clrMethodNameValue,
        object lengthValue,
        object functionNameValue,
        object isStaticValue,
        object isPrivateValue,
        object isSetterValue,
        object isGeneratorValue,
        object isAsyncValue,
        object scopesValue)
    {
        ArgumentNullException.ThrowIfNull(targetValue);

        var ownerType = ResolveClassOwnerType(ownerValue);
        var scopes = scopesValue as object[]
            ?? (ownerValue is JsClassConstructorObject classConstructorValue
                ? classConstructorValue.Scopes
                : EmptyScopes);
        var clrMethodName = clrMethodNameValue as string
            ?? throw new TypeError("Class accessor definition requires a CLR method name");
        var key = ObjectRuntime.ToPropertyKeyString(keyValue);
        var functionName = functionNameValue as string ?? key;
        var length = lengthValue is double d ? d : 0d;
        var isStatic = TypeUtilities.ToBoolean(isStaticValue);
        var isPrivate = TypeUtilities.ToBoolean(isPrivateValue);
        var isSetter = TypeUtilities.ToBoolean(isSetterValue);
        var isGenerator = TypeUtilities.ToBoolean(isGeneratorValue);
        var isAsync = TypeUtilities.ToBoolean(isAsyncValue);

        var flags = (isStatic ? BindingFlags.Static : BindingFlags.Instance)
            | BindingFlags.Public
            | BindingFlags.NonPublic;
        var method = ownerType.GetMethod(clrMethodName, flags)
            ?? throw new TypeError($"Class accessor '{clrMethodName}' was not found on {ownerType.FullName}");

        Func<object[], object?[]?, object?> functionValue = (_, args) =>
            InvokeClassMethodFunction(ownerType, method, scopes, isStatic, isPrivate, args);

        if (isAsync)
        {
            AsyncFunction.InitializeFunctionInstance(functionValue, length, functionName);
        }
        else
        {
            Function.InitializeFunctionInstance(functionValue, length, functionName);
        }
        Function.DefineRestrictedFunctionProperties(functionValue);

        if (isGenerator)
        {
            GeneratorObject.InitializeGeneratorFunctionSurface(functionValue);
        }

        object? existingGet = isSetter ? null : functionValue;
        object? existingSet = isSetter ? functionValue : null;
        if (PropertyDescriptorStore.TryGetOwn(targetValue, key, out var existing)
            && existing.Kind == JsPropertyDescriptorKind.Accessor)
        {
            existingGet ??= existing.Get;
            existingSet ??= existing.Set;
        }

        PropertyDescriptorStore.DefineOrUpdate(targetValue, key, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Accessor,
            Get = existingGet,
            Set = existingSet,
            Enumerable = false,
            Configurable = true
        });

        return targetValue;
    }

    private static Type ResolveClassOwnerType(object ownerValue)
        => ownerValue switch
        {
            Type type => type,
            JsClassConstructorObject classConstructorValue => classConstructorValue.Type,
            _ => throw new TypeError("Class method definition requires a class constructor value")
        };

    internal static bool TryEnsureClassConstructorMetadataPropertyDescriptor(
        JsClassConstructorObject classConstructorValue,
        string propName,
        out JsPropertyDescriptor descriptor)
    {
        if (PropertyDescriptorStore.TryGetOwn(classConstructorValue, propName, out descriptor))
        {
            return true;
        }

        if (string.Equals(propName, "length", StringComparison.Ordinal))
        {
            descriptor = new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Value = (double)classConstructorValue.FormalParameterCount,
                Writable = false,
                Enumerable = false,
                Configurable = true
            };
            PropertyDescriptorStore.DefineOrUpdate(classConstructorValue, propName, descriptor);
            return true;
        }

        if (string.Equals(propName, "name", StringComparison.Ordinal))
        {
            if (PropertyDescriptorStore.TryGetOwn(
                    classConstructorValue.Type,
                    propName,
                    out var typeNameDescriptor)
                && (typeNameDescriptor.Kind == JsPropertyDescriptorKind.Accessor
                    || typeNameDescriptor.Value is not null))
            {
                descriptor = CloneDescriptor(typeNameDescriptor);
            }
            else
            {
                descriptor = new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Value = classConstructorValue.Type.Name,
                    Writable = false,
                    Enumerable = false,
                    Configurable = true
                };
            }

            PropertyDescriptorStore.DefineOrUpdate(
                classConstructorValue,
                propName,
                descriptor);
            return true;
        }

        if (string.Equals(propName, "prototype", StringComparison.Ordinal))
        {
            var protoObj = ObjectRuntime.CreateOrdinaryObject();

            if (PropertyDescriptorStore.TryGetOwn(classConstructorValue.Type, "prototype", out var typePrototypeDescriptor)
                && typePrototypeDescriptor.Kind == JsPropertyDescriptorKind.Data
                && typePrototypeDescriptor.Value is object existingPrototype
                && existingPrototype is not JsNull
                && existingPrototype is not string
                && !existingPrototype.GetType().IsValueType)
            {
                foreach (var key in PropertyDescriptorStore.GetOwnKeys(existingPrototype))
                {
                    if (PropertyDescriptorStore.TryGetOwn(existingPrototype, key, out var existingDescriptor))
                    {
                        PropertyDescriptorStore.DefineOrUpdate(protoObj, key, CloneDescriptor(existingDescriptor));
                    }
                }

                var existingPrototypeParent = JavaScriptRuntime.PrototypeChain.GetPrototypeOrNull(existingPrototype);
                if (existingPrototypeParent != null)
                {
                    JavaScriptRuntime.PrototypeChain.SetPrototype(protoObj, existingPrototypeParent);
                }
            }

            PropertyDescriptorStore.DefineOrUpdate(classConstructorValue, "prototype", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = false,
                Writable = false,
                Value = protoObj
            });

            PropertyDescriptorStore.DefineOrUpdate(protoObj, "constructor", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = classConstructorValue
            });

            return PropertyDescriptorStore.TryGetOwn(classConstructorValue, propName, out descriptor);
        }

        descriptor = default;
        return false;
    }

    internal static void EnsureClassConstructorCoreMetadataProperties(object target)
    {
        if (target is not JsClassConstructorObject classConstructorValue)
        {
            return;
        }

        _ = TryEnsureClassConstructorMetadataPropertyDescriptor(classConstructorValue, "length", out _);
        _ = TryEnsureClassConstructorMetadataPropertyDescriptor(classConstructorValue, "name", out _);
        _ = TryEnsureClassConstructorMetadataPropertyDescriptor(classConstructorValue, "prototype", out _);
    }

    internal static bool TryEnsureLazyClassMethodDataProperty(
        object target,
        string propName,
        out JsPropertyDescriptor descriptor)
    {
        descriptor = default;
        if ((target is JsObject jsObject && jsObject.IsInlineLazyClassMethodDeleted(propName))
            || PropertyDescriptorStore.IsDeleted(target, propName)
            || !TryResolveLazyClassMethodTarget(target, out var ownerType, out var ownerValue, out var isStatic)
            || !TryGetCurrentRealmValueCaches(out var valueCaches)
            || !valueCaches.LazyClassMetadata.TryGetValue(ownerType, out var slot))
        {
            return false;
        }

        LazyClassMethodDataProperty? metadata;
        lock (slot)
        {
            metadata = slot.Methods.FirstOrDefault(method =>
                method.IsStatic == isStatic
                && string.Equals(method.PropertyKey, propName, StringComparison.Ordinal));
        }

        if (metadata == null)
        {
            return false;
        }

        DefineClassMethodDataProperty(
            target,
            metadata.PropertyKey,
            ownerValue,
            metadata.ClrMethodName,
            metadata.Length,
            metadata.FunctionName,
            metadata.IsStatic,
            metadata.IsPrivate,
            metadata.IsGenerator,
            metadata.IsAsync,
            metadata.Scopes);

        return PropertyDescriptorStore.TryGetOwn(target, propName, out descriptor);
    }

    internal static IEnumerable<string> GetLazyClassMethodOwnKeys(object target)
    {
        if (!TryResolveLazyClassMethodTarget(target, out var ownerType, out _, out var isStatic)
            || !TryGetCurrentRealmValueCaches(out var valueCaches)
            || !valueCaches.LazyClassMetadata.TryGetValue(ownerType, out var slot))
        {
            return System.Array.Empty<string>();
        }

        lock (slot)
        {
            return slot.Methods
                .Where(method => method.IsStatic == isStatic
                    && (target is not JsObject jsObject
                        || !jsObject.IsInlineLazyClassMethodDeleted(method.PropertyKey))
                    && !PropertyDescriptorStore.IsDeleted(target, method.PropertyKey)
                    && !PropertyDescriptorStore.TryGetOwn(target, method.PropertyKey, out _))
                .Select(method => method.PropertyKey)
                .ToArray();
        }
    }

    internal static void MarkLazyClassMethodPropertyDeleted(object target, string propName)
    {
        if (!TryResolveLazyClassMethodTarget(target, out var ownerType, out _, out var isStatic)
            || !TryGetCurrentRealmValueCaches(out var valueCaches)
            || !valueCaches.LazyClassMetadata.TryGetValue(ownerType, out var slot))
        {
            return;
        }

        lock (slot)
        {
            if (!slot.Methods.Any(method =>
                method.IsStatic == isStatic
                && string.Equals(method.PropertyKey, propName, StringComparison.Ordinal)))
            {
                return;
            }
        }

        if (target is JsObject jsObject && !jsObject.HasSharedIntrinsicBaseline)
        {
            jsObject.MarkInlineLazyClassMethodDeleted(propName);
            return;
        }

        PropertyDescriptorStore.Delete(target, propName);
    }

    private static bool TryResolveLazyClassMethodTarget(
        object target,
        out Type ownerType,
        out object ownerValue,
        out bool isStatic)
    {
        switch (target)
        {
            case Type type:
                ownerType = type;
                ownerValue = type;
                isStatic = true;
                return true;
            case JsClassConstructorObject classConstructorValue:
                ownerType = classConstructorValue.Type;
                ownerValue = classConstructorValue;
                isStatic = true;
                return true;
        }

        var constructorLookup = target is JsObject
            ? PropertyDescriptorStore.GetOwnLookupCore(target, "constructor", out var constructorDescriptor)
                == PropertyDescriptorLookup.Found
            : PropertyDescriptorStore.TryGetOwn(target, "constructor", out constructorDescriptor);
        if (constructorLookup
            && constructorDescriptor.Kind == JsPropertyDescriptorKind.Data)
        {
            switch (constructorDescriptor.Value)
            {
                case JsClassConstructorObject classConstructorValue:
                    ownerType = classConstructorValue.Type;
                    ownerValue = classConstructorValue;
                    isStatic = false;
                    return true;
                case Type type:
                    ownerType = type;
                    ownerValue = type;
                    isStatic = false;
                    return true;
            }
        }

        ownerType = null!;
        ownerValue = null!;
        isStatic = false;
        return false;
    }

    private static object? InvokeClassMethodFunction(
        Type ownerType,
        MethodInfo method,
        object[] scopes,
        bool isStatic,
        bool isPrivate,
        object?[]? args)
    {
        var receiver = ResolveLexicalThis(GetCurrentThis());
        if (isPrivate && !HasClassPrivateMethodBrand(receiver, ownerType, isStatic))
        {
            throw new TypeError("Receiver does not have the requested private method");
        }

        object? instance = null;
        if (!isStatic)
        {
            if (receiver is null || receiver is JsNull || !ownerType.IsInstanceOfType(receiver))
            {
                if (!IsPrototypeObjectForClass(receiver, ownerType))
                {
                    throw new TypeError("Class method receiver is incompatible with its declaring class");
                }

                instance = RuntimeHelpers.GetUninitializedObject(ownerType);
                ownerType.GetField("_scopes", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    ?.SetValue(instance, scopes);
            }
            else
            {
                instance = receiver;
            }
        }

        var invokeArgs = BuildClassMethodInvokeArguments(method, scopes, args);
        try
        {
            return method.Invoke(instance, invokeArgs);
        }
        catch (TargetInvocationException tie) when (tie.InnerException != null)
        {
            ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
            throw;
        }
    }

    public static object ResolveGeneratedClassMethodReceiver(
        object? receiver,
        Type ownerType,
        object[] scopes,
        object? privateBrand,
        JsFunctionObject functionObject)
    {
        receiver = ResolveLexicalThis(receiver);
        if (privateBrand != null
            && !HasGeneratedClassPrivateBrand(
                receiver,
                ownerType,
                privateBrand,
                functionObject))
        {
            throw new TypeError("Receiver does not have the requested private method");
        }

        if (receiver != null
            && receiver is not JsNull
            && ownerType.IsInstanceOfType(receiver))
        {
            return receiver;
        }

        if (!IsPrototypeObjectForClass(receiver, ownerType))
        {
            throw new TypeError("Class method receiver is incompatible with its declaring class");
        }

        var instance = RuntimeHelpers.GetUninitializedObject(ownerType);
        ownerType.GetField(
                "_scopes",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(instance, scopes);
        return instance;
    }

    public static object ValidateGeneratedStaticMethodReceiver(
        object? receiver,
        Type ownerType,
        object? privateBrand,
        JsFunctionObject functionObject)
    {
        receiver = ResolveLexicalThis(receiver);
        var hasOwnerType = receiver switch
        {
            Type type => type == ownerType,
            JsClassConstructorObject constructor => constructor.Type == ownerType,
            _ => false
        };
        if (!hasOwnerType)
        {
            throw new TypeError(
                "Class method receiver is incompatible with its declaring class");
        }

        if (privateBrand != null
            && !OwnPropertiesContainFunction(receiver!, functionObject))
        {
            throw new TypeError("Receiver does not have the requested private method");
        }

        return receiver!;
    }

    private static bool OwnPropertiesContainFunction(
        object owner,
        JsFunctionObject functionObject)
    {
        foreach (var key in PropertyDescriptorStore.GetOwnKeys(owner))
        {
            if (PropertyDescriptorStore.TryGetOwn(owner, key, out var descriptor)
                && ((descriptor.Kind == JsPropertyDescriptorKind.Data
                        && ReferenceEquals(descriptor.Value, functionObject))
                    || (descriptor.Kind == JsPropertyDescriptorKind.Accessor
                        && (ReferenceEquals(descriptor.Get, functionObject)
                            || ReferenceEquals(descriptor.Set, functionObject)))))
            {
                return true;
            }
        }
        return false;
    }

    private static bool HasGeneratedClassPrivateBrand(
        object? receiver,
        Type ownerType,
        object privateBrand,
        JsFunctionObject functionObject)
    {
        if (receiver is null
            || receiver is JsNull
            || !ownerType.IsInstanceOfType(receiver))
        {
            return false;
        }

        if (privateBrand is not JsClassConstructorObject classConstructor)
        {
            return PrototypeChainContainsFunction(receiver, functionObject);
        }

        var expectedPrototype = ObjectRuntime.GetProperty(
            classConstructor,
            "prototype");
        var current = PrototypeChain.GetPrototypeOrNull(receiver);
        while (current != null && current is not JsNull)
        {
            if (ReferenceEquals(current, expectedPrototype))
            {
                return true;
            }
            current = PrototypeChain.GetPrototypeOrNull(current);
        }
        return false;
    }

    private static bool PrototypeChainContainsFunction(
        object receiver,
        JsFunctionObject functionObject)
    {
        var current = PrototypeChain.GetPrototypeOrNull(receiver);
        while (current != null && current is not JsNull)
        {
            foreach (var key in PropertyDescriptorStore.GetOwnKeys(current))
            {
                if (!PropertyDescriptorStore.TryGetOwn(
                        current,
                        key,
                        out var descriptor))
                {
                    continue;
                }

                if ((descriptor.Kind == JsPropertyDescriptorKind.Data
                        && ReferenceEquals(descriptor.Value, functionObject))
                    || (descriptor.Kind == JsPropertyDescriptorKind.Accessor
                        && (ReferenceEquals(descriptor.Get, functionObject)
                            || ReferenceEquals(descriptor.Set, functionObject))))
                {
                    return true;
                }
            }
            current = PrototypeChain.GetPrototypeOrNull(current);
        }
        return false;
    }

    public static object ValidateClassPrivateMethodReceiver(object? receiver, Type ownerType, bool isStatic)
    {
        receiver = ResolveLexicalThis(receiver);
        if (!HasClassPrivateMethodBrand(receiver, ownerType, isStatic))
        {
            throw new TypeError("Receiver does not have the requested private method");
        }

        return receiver!;
    }

    public static object ValidateDirectClassPrivateMethodReceiver(object? receiver, Type ownerType)
    {
        receiver = ResolveLexicalThis(receiver);
        var hasBrand = receiver switch
        {
            Type type => type == ownerType,
            JsClassConstructorObject classConstructorValue => classConstructorValue.Type == ownerType,
            _ => receiver != null && ownerType.IsInstanceOfType(receiver)
        };

        if (!hasBrand)
        {
            throw new TypeError("Receiver does not have the requested private method");
        }

        return receiver!;
    }

    private static bool IsPrototypeObjectForClass(object? receiver, Type ownerType)
    {
        // JS permits calling prototype methods with the prototype object as the receiver
        // (for example, C.prototype.m()). Generated CLR methods still need a declaring
        // class instance target, so detect the runtime prototype object shape here.
        if (receiver is null || receiver is JsNull)
        {
            return false;
        }

        return PropertyDescriptorStore.TryGetOwn(receiver, "constructor", out var constructorDescriptor)
            && constructorDescriptor.Kind == JsPropertyDescriptorKind.Data
            && constructorDescriptor.Value is JsClassConstructorObject classConstructorValue
            && classConstructorValue.Type == ownerType;
    }

    private static bool HasClassPrivateMethodBrand(object? receiver, Type ownerType, bool isStatic)
    {
        if (receiver is null || receiver is JsNull)
        {
            return false;
        }

        if (isStatic)
        {
            return receiver switch
            {
                Type type => type == ownerType,
                JsClassConstructorObject classConstructorValue => classConstructorValue.Type == ownerType,
                _ => false
            };
        }

        return ownerType.IsInstanceOfType(receiver);
    }

    private static object?[] BuildClassMethodInvokeArguments(MethodInfo method, object[] scopes, object?[]? args)
    {
        var parameters = method.GetParameters();
        if (parameters.Length == 0)
        {
            return System.Array.Empty<object?>();
        }

        var invokeArgs = new object?[parameters.Length];
        var sourceArgs = args ?? System.Array.Empty<object?>();
        var jsArgIndex = 0;

        for (var i = 0; i < parameters.Length; i++)
        {
            if (i == 0 && parameters[i].ParameterType == typeof(object[]))
            {
                invokeArgs[i] = scopes;
                continue;
            }

            if (string.Equals(parameters[i].Name, "newTarget", StringComparison.Ordinal))
            {
                invokeArgs[i] = null;
                continue;
            }

            invokeArgs[i] = jsArgIndex < sourceArgs.Length ? sourceArgs[jsArgIndex] : null;
            jsArgIndex++;
        }

        return invokeArgs;
    }

    public static object SetClassConstructorInferredName(object constructorValue, object nameValue)
    {
        if (nameValue is not string inferredName || string.IsNullOrWhiteSpace(inferredName))
        {
            return constructorValue;
        }

        var descriptor = new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Value = inferredName,
            Writable = false,
            Enumerable = false,
            Configurable = true
        };

        if (constructorValue is JsClassConstructorObject classConstructorValue)
        {
            if (HasOwnOrLazyClassNameProperty(classConstructorValue))
            {
                return classConstructorValue;
            }

            if (PropertyDescriptorStore.TryGetOwn(
                    classConstructorValue.Type,
                    "name",
                    out var typeNameDescriptor)
                && (typeNameDescriptor.Kind == JsPropertyDescriptorKind.Accessor
                    || typeNameDescriptor.Value is not null))
            {
                PropertyDescriptorStore.DefineOrUpdate(
                    classConstructorValue,
                    "name",
                    CloneDescriptor(typeNameDescriptor));
                return classConstructorValue;
            }

            PropertyDescriptorStore.DefineOrUpdate(classConstructorValue, "name", descriptor);
            return classConstructorValue;
        }

        if (constructorValue is Type staticType)
        {
            if (HasOwnOrLazyClassNameProperty(staticType))
            {
                return staticType;
            }

            PropertyDescriptorStore.DefineOrUpdate(staticType, "name", descriptor);
        }

        return constructorValue;
    }

    private static bool HasOwnOrLazyClassNameProperty(object target)
    {
        if (PropertyDescriptorStore.TryGetOwn(target, "name", out _))
        {
            return true;
        }

        if (!TryResolveLazyClassMethodTarget(target, out var ownerType, out _, out var isStatic)
            || !TryGetCurrentRealmValueCaches(out var valueCaches)
            || !valueCaches.LazyClassMetadata.TryGetValue(ownerType, out var slot))
        {
            return false;
        }

        lock (slot)
        {
            return slot.Methods.Any(method =>
                method.IsStatic == isStatic
                && string.Equals(method.PropertyKey, "name", StringComparison.Ordinal));
        }
    }

    public static object SetFunctionInferredName(object functionValue, object nameValue)
    {
        if (!CallableOperations.IsCallable(functionValue)
            || nameValue is not string inferredName
            || string.IsNullOrWhiteSpace(inferredName))
        {
            return functionValue;
        }

        if (PropertyDescriptorStore.TryGetOwn(functionValue, "name", out var existingDescriptor)
            && existingDescriptor.Kind == JsPropertyDescriptorKind.Data
            && existingDescriptor.Value is string existingName
            && !string.IsNullOrEmpty(existingName))
        {
            return functionValue;
        }

        Function.DefineMetadataProperty(functionValue, "name", inferredName);
        return functionValue;
    }

    public static object?[]? GetCurrentArguments()
    {
        var current = _currentInvocation.Value;
        if (current?.CurrentArguments is { } materializedArguments)
        {
            return materializedArguments;
        }

        if (current?.CurrentCallArguments is not { } callArguments)
        {
            return null;
        }

        materializedArguments = callArguments.ToArray();
        PublishInvocationFrame(
            current with
            {
                CurrentArguments = materializedArguments
            });
        return materializedArguments;
    }

    public static object?[] GetCurrentArgumentsOrFallback(
        object?[] fallback)
        => GetCurrentArguments() ?? fallback;

    public static object?[] GetCurrentArgumentsForGeneratedCallableOrFallback(
        Type generatedFunctionType,
        object?[] fallback)
    {
        ArgumentNullException.ThrowIfNull(generatedFunctionType);
        ArgumentNullException.ThrowIfNull(fallback);

        return _currentInvocation.Value?.CurrentCallee?.GetType()
                == generatedFunctionType
            ? GetCurrentArguments() ?? fallback
            : fallback;
    }

    public static object? GetArgumentOrUndefined(object?[] arguments, int index)
        => (uint)index < (uint)arguments.Length
            ? arguments[index]
            : null;

    public static object?[]? SetCurrentArguments(object?[]? value)
    {
        var current = _currentInvocation.Value;
        var previous = current?.CurrentArguments;
        PublishInvocationFrame(
            (current ?? new InvocationFrame()) with
            {
                CurrentArguments = value
            });
        return previous;
    }

    /// <summary>
    /// Establishes the JS invocation state for a generated function object's static direct-call
    /// adapter. <see cref="PopGeneratedFunctionDirectCall"/> must be called from a finally block.
    /// </summary>
    public static void PushGeneratedFunctionDirectCall(
        JsFunctionObject? functionObject,
        object?[] arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        if (functionObject is null)
        {
            var argumentsOnly = JsCallArguments.FromArray(arguments);
            var argumentsOnlyState = PushInvocationFrame(
                InvocationContextRequirements.Arguments,
                currentThis: null,
                argumentsOnly,
                currentCallee: null,
                currentNewTarget: null);
            (_generatedFunctionDirectCallStack ??= new()).Push(
                new GeneratedFunctionDirectCallState(argumentsOnlyState));
            return;
        }

        var requirements = functionObject.InvocationRequirements;
        if (functionObject.SupportsExplicitInvocationContext
            || requirements == InvocationContextRequirements.None)
        {
            (_generatedFunctionDirectCallStack ??= new())
                .Push(GeneratedFunctionDirectCallState.NoContext);
            return;
        }

        var lexicalSuperScopes =
            (requirements & InvocationContextRequirements.LexicalSuper) != 0
            ? functionObject.GetLexicalSuperScopes()
            : null;
        var lexicalSuperReceiver = lexicalSuperScopes is not null
            ? functionObject.GetLexicalSuperReceiver()
            : null;
        var callArguments = JsCallArguments.FromArray(arguments);
        var invocationState = PushInvocationFrame(
            requirements,
            functionObject.ResolveThisArgument(null),
            callArguments,
            functionObject,
            functionObject.ResolveCallNewTarget(),
            lexicalSuperReceiver,
            lexicalSuperScopes);

        (_generatedFunctionDirectCallStack ??= new()).Push(
            new GeneratedFunctionDirectCallState(invocationState));
    }

    /// <summary>
    /// Restores the most recent invocation state established by
    /// <see cref="PushGeneratedFunctionDirectCall"/>.
    /// </summary>
    public static void PopGeneratedFunctionDirectCall()
    {
        if (_generatedFunctionDirectCallStack is not { Count: > 0 } stack)
        {
            throw new InvalidOperationException(
                "No generated function direct-call state is available.");
        }

        var callState = stack.Pop();
        RestoreInvocationFrame(callState.InvocationState);
    }

    private readonly record struct GeneratedFunctionDirectCallState(
        InvocationFrameState InvocationState)
    {
        public static GeneratedFunctionDirectCallState NoContext => default;
    }

    /// <summary>
    /// Saves the current arguments onto a thread-local stack and sets new arguments.
    /// Called before <c>newobj</c> so that the constructor chain can observe the actual call-site arguments
    /// via the <c>arguments</c> keyword.
    /// </summary>
    public static void PushCurrentArguments(object?[]? value)
    {
        _constructorArgStack ??= new Stack<object?[]?>();
        _constructorArgStack.Push(
            _currentInvocation.Value?.CurrentArguments);
        SetCurrentArguments(value);
    }

    /// <summary>
    /// Restores the previous arguments from the thread-local stack.
    /// Called after <c>newobj</c> completes.
    /// </summary>
    public static void PopCurrentArguments()
    {
        if (_constructorArgStack?.Count > 0)
        {
            SetCurrentArguments(_constructorArgStack.Pop());
        }
    }

    public static void PushCurrentNewTarget(object? value)
    {
        _constructorNewTargetStack ??= new Stack<object?>();
        _constructorNewTargetStack.Push(
            _currentInvocation.Value?.CurrentNewTarget);
        SetCurrentNewTarget(value);
    }

    public static void PopCurrentNewTarget()
    {
        if (_constructorNewTargetStack?.Count > 0)
        {
            SetCurrentNewTarget(_constructorNewTargetStack.Pop());
        }
    }

    public static object? GetCurrentNewTarget()
    {
        return _currentInvocation.Value?.CurrentNewTarget;
    }

    public static object? GetCurrentNewTargetOrReceiverType(object? receiver)
        => _currentInvocation.Value?.CurrentNewTarget ?? receiver?.GetType();

    public static object? SetCurrentNewTarget(object? value)
    {
        var current = _currentInvocation.Value;
        var previous = current?.CurrentNewTarget;
        PublishInvocationFrame(
            (current ?? new InvocationFrame()) with
            {
                CurrentNewTarget = value
            });
        return previous;
    }

    public static object? GetCurrentCallee()
    {
        return _currentInvocation.Value?.CurrentCallee;
    }

    public static object? SetCurrentCallee(object? value)
    {
        var current = _currentInvocation.Value;
        var previous = current?.CurrentCallee;
        PublishInvocationFrame(
            (current ?? new InvocationFrame()) with
            {
                CurrentCallee = value
            });
        return previous;
    }

    public static object? ResolveWithBindingOrDefault(object? nameValue, object? defaultValue)
    {
        var callee = _currentInvocation.Value?.CurrentCallee;
        if (callee is not null
            && JavaScriptRuntime.Function.TryGetBoundWithObject(callee, out var withObject)
            && withObject is not null)
        {
            var name = nameValue as string ?? DotNet2JSConversions.ToString(nameValue);
            if (JavaScriptRuntime.ObjectRuntime.HasPropertyIn(name, withObject))
            {
                return JavaScriptRuntime.ObjectRuntime.GetProperty(withObject, name);
            }
        }

        return defaultValue;
    }

    public static object EnsureTemporalDeadZoneInitialized(object value, string bindingName)
    {
        if (ReferenceEquals(value, TemporalDeadZoneSentinel))
        {
            throw new ReferenceError($"Cannot access '{bindingName}' before initialization");
        }

        return value;
    }

    public static object GetImportMeta(object? moduleIdOrPath)
    {
        var url = GetImportMetaUrl(moduleIdOrPath);
        var meta = GetCurrentModuleState().ImportMetaByUrl.GetOrAdd(url, static key =>
        {
            var meta = new JsObject();
            if (!string.IsNullOrEmpty(key))
            {
                meta["url"] = key;
            }
            return meta;
        });

        return meta;
    }

    private static string GetImportMetaUrl(object? moduleIdOrPath)
    {
        var key = moduleIdOrPath?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(key))
        {
            return string.Empty;
        }

        if (Path.IsPathRooted(key))
        {
            var fullPath = Path.GetFullPath(key);
            var builder = new UriBuilder
            {
                Scheme = Uri.UriSchemeFile,
                Host = string.Empty,
                Path = fullPath,
            };
            return builder.Uri.AbsoluteUri;
        }

        if (Uri.TryCreate(key, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.AbsoluteUri;
        }

        return key;
    }

    /// <summary>
    /// Materializes the implicit non-arrow function `arguments` object for the current call.
    /// This captures the full runtime argument list (including extra args beyond formal parameters)
    /// and optionally maps simple-parameter indices back to leaf-scope parameter storage.
    /// </summary>
    public static ArgumentsObject CreateArgumentsObject(object? scopeInstance, string[]? parameterNames, bool includeCallee, bool restrictCallee)
    {
        var args = GetCurrentArguments();
        return new ArgumentsObject(
            args,
            scopeInstance,
            parameterNames,
            includeCallee ? GetCurrentCallee() : null,
            restrictCallee);
    }

    public static ArgumentsObject CreateArgumentsObject(
        GeneratedInvocationContext context,
        object? scopeInstance,
        string[]? parameterNames,
        bool includeCallee,
        bool restrictCallee)
        => new(
            GeneratedInvocationContext.GetArguments(context),
            scopeInstance,
            parameterNames,
            includeCallee
                ? GeneratedInvocationContext.GetCallee(context)
                : null,
            restrictCallee);

    /// <summary>
    /// Gets the count of arguments passed to the current function.
    /// Used for rest parameter initialization.
    /// </summary>
    public static int GetArgumentCount()
    {
        var args = GetCurrentArguments();
        return args?.Length ?? 0;
    }

    /// <summary>
    /// Collects rest arguments starting from the specified index into an array.
    /// Used for rest parameter (...args) initialization.
    /// </summary>
    public static Array CollectRestArguments(double startIndex)
    {
        var startIndexAsInt = (int)startIndex;
        var args = GetCurrentArguments();

        if (args == null || startIndexAsInt >= args.Length)
        {
            return new Array();
        }

        // Collect arguments from startIndex to end
        var restArgs = new object?[args.Length - startIndexAsInt];
        System.Array.Copy(args, startIndexAsInt, restArgs, 0, restArgs.Length);
        return new Array(restArgs);
    }

    public static Array CollectRestArguments(
        GeneratedInvocationContext context,
        double startIndex)
    {
        var start = System.Math.Max(0, (int)startIndex);
        var count = GeneratedInvocationContext.GetArgumentCount(context);
        var result = new Array();
        for (var index = start; index < count; index++)
        {
            result.Add(GeneratedInvocationContext.GetArgument(context, index));
        }
        return result;
    }

    /// <summary>
    /// Registers a module-scoped require delegate by module id/filename.
    /// Used by dynamic import() to resolve the module loading context.
    /// </summary>
    public static void RegisterModuleRequire(string moduleId, Modules.CommonJS.RequireDelegate require)
    {
        if (string.IsNullOrWhiteSpace(moduleId) || require == null)
        {
            return;
        }

        GetCurrentModuleState().RequireByModuleId[moduleId] = require;
    }

    /// <summary>
    /// Resolves a previously-registered module-scoped require delegate.
    /// </summary>
    public static Modules.CommonJS.RequireDelegate? GetRequireForModule(string? moduleId)
    {
        if (string.IsNullOrWhiteSpace(moduleId))
        {
            return null;
        }

        return GetCurrentModuleState().RequireByModuleId.TryGetValue(moduleId, out var require)
            ? require
            : null;
    }

    private static Modules.RuntimeModuleState GetCurrentModuleState()
        => RuntimeExecutionContext.Current?.Realm.ModuleState
            ?? throw new InvalidOperationException(
                "Module state requires an active JavaScript runtime.");

    /// <summary>
    /// Creates the backing object for a JavaScript object literal.
    /// Returns a <see cref="JsObject"/> that stores numeric and boolean values without boxing.
    /// </summary>
    public static JsObject CreateObjectLiteral()
    {
        return ObjectRuntime.CreateOrdinaryObject();
    }

    /// <summary>
    /// Cache for template objects indexed by call site ID.
    /// Per ECMA-262 spec, each unique call site should return the same template object identity.
    /// </summary>
    private static ConcurrentDictionary<string, Array> _templateObjectCache
        => GetCurrentRealmValueCaches().TemplateObjects;
    private const int MaxTemplateObjectCacheEntries = 4096;

    private static RuntimeRealmValueCacheState GetCurrentRealmValueCaches()
        => RuntimeExecutionContext.CurrentOrOverride?.Realm.ValueCaches
            ?? throw new InvalidOperationException(
                "Realm-created value caches require an active JavaScript runtime.");

    private static bool TryGetCurrentRealmValueCaches(
        out RuntimeRealmValueCacheState valueCaches)
    {
        valueCaches = RuntimeExecutionContext.CurrentOrOverride?.Realm.ValueCaches!;
        return valueCaches != null;
    }

    /// <summary>
    /// Creates a template object for tagged template expressions.
    /// Returns a cached instance for the same call site to preserve object identity.
    /// The template object is an array with the cooked strings and a .raw property with raw strings.
    /// </summary>
    /// <param name="callSiteId">Unique identifier for the call site (e.g., "Module:Line:Column")</param>
    /// <param name="cooked">Cooked string array (with escape sequences processed)</param>
    /// <param name="raw">Raw string array (escape sequences not processed)</param>
    public static object CreateTemplateObject(string callSiteId, object[] cooked, object[] raw)
    {
        if (_templateObjectCache.TryGetValue(callSiteId, out var existing))
        {
            return existing;
        }

        if (_templateObjectCache.Count >= MaxTemplateObjectCacheEntries)
        {
            // Keep cache growth bounded to avoid unbounded memory retention in long-lived hosts.
            return CreateTemplateObjectCore(cooked, raw);
        }

        return _templateObjectCache.GetOrAdd(callSiteId, _ => CreateTemplateObjectCore(cooked, raw));
    }

    public static object CreateTemplateObject(object callSiteId, object cooked, object raw)
    {
        return CreateTemplateObject((string)callSiteId, (object[])cooked, (object[])raw);
    }

    private static Array CreateTemplateObjectCore(object[] cooked, object[] raw)
    {
        // Create array with cooked strings
        var templateObject = new Array(cooked);

        // Add .raw property with raw strings
        var rawJsArray = new Array(raw);
        ObjectRuntime.SetProperty(templateObject, "raw", rawJsArray);

        // Template objects should be frozen (immutable)
        // For now we just return the object - freezing can be added later if needed
        return templateObject;
    }

    public static ServiceContainer BuildServiceProvider()
    {
        var container = RuntimeOwnershipFactory.CreateIsolatedRealm().Services;
        ConfigureServiceProvider(container);
        return container;
    }

    internal static void ConfigureServiceProvider(ServiceContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.RegisterInstance(new GlobalThisOptions());
        container.RegisterInstance(HostRuntimeIntrinsicDescriptors.Empty);
        container.RegisterInstance(new ConsoleOutputSinks());

        container.Register<Modules.CommonJS.Require>();
        container.RegisterInstance<IPropertyDescriptorStore>(new PropertyDescriptorStore());
        container.Register<IEnvironment, DefaultEnvironment>();
        container.Register<Node.IChildProcessLauncher, Node.DefaultChildProcessLauncher>();
        container.Register<Node.DiagnosticsChannelRuntime>();
    }
}
