using JavaScriptRuntime.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime;

public class RuntimeServices
{
    private static readonly System.Threading.AsyncLocal<object?> _currentThis = new();
    private static readonly System.Threading.AsyncLocal<object?[]?> _currentArguments = new();
    private static readonly System.Threading.AsyncLocal<object?> _currentNewTarget = new();
    private static readonly System.Threading.AsyncLocal<object?> _currentCallee = new();
    [ThreadStatic] private static Stack<object?[]?>? _constructorArgStack;
    [ThreadStatic] private static Stack<object?>? _derivedConstructorThisStack;
    private static readonly ConcurrentDictionary<string, ExpandoObject> _importMetaByUrl = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, JavaScriptRuntime.CommonJS.RequireDelegate> _requireByModuleId = new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConditionalWeakTable<Type, LazyClassMetadataSlot> _lazyClassMetadata = new();
    private static readonly ConditionalWeakTable<object, DeletedLazyClassMethodSlot> _deletedLazyClassMethodProperties = new();

    // ABI compatibility: when a callee doesn't need scopes, we still pass a 1-element scopes array.
    // NOTE: Consumers must treat scopes arrays as immutable.
    public static readonly object[] EmptyScopes = new object[1];
    public static readonly object TemporalDeadZoneSentinel = new();

    private sealed class DerivedConstructorThisBinding
    {
        public object? Value = TemporalDeadZoneSentinel;
    }

    private sealed class LazyClassMetadataSlot
    {
        public readonly List<LazyClassMethodDataProperty> Methods = new();
    }

    private sealed class DeletedLazyClassMethodSlot
    {
        public readonly HashSet<string> Keys = new(StringComparer.Ordinal);
    }

    private sealed record LazyClassMethodDataProperty(
        string PropertyKey,
        string ClrMethodName,
        double Length,
        string FunctionName,
        bool IsStatic,
        bool IsPrivate,
        bool IsGenerator,
        bool IsAsync,
        object[] Scopes);

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
        return _currentThis.Value;
    }

    public static object? SetCurrentThis(object? value)
    {
        var previous = _currentThis.Value;
        _currentThis.Value = value;
        return previous;
    }

    public static void PushDerivedConstructorThisBinding()
    {
        _derivedConstructorThisStack ??= new Stack<object?>();
        _derivedConstructorThisStack.Push(_currentThis.Value);
        _currentThis.Value = new DerivedConstructorThisBinding();
    }

    public static void InitializeDerivedConstructorThisBinding(object? value)
    {
        if (_currentThis.Value is DerivedConstructorThisBinding binding)
        {
            binding.Value = value;
            return;
        }

        _currentThis.Value = value;
    }

    public static void ConstructDerivedFunctionBase(object receiver, object constructor, object[] args)
    {
        object? constructed;
        if (constructor is Delegate del)
        {
            constructed = JavaScriptRuntime.Function.ConstructWithReceiver(del, receiver, args, constructor);
        }
        else if (constructor is JavaScriptRuntime.Proxy)
        {
            constructed = JavaScriptRuntime.Object.ConstructValue(constructor, args);
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
            _currentThis.Value = stack.Pop();
            return;
        }

        _currentThis.Value = null;
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

    public static object CreateClassConstructorValue(object typeValue, object scopesValue, object formalParamCountValue)
    {
        if (typeValue is not Type type)
        {
            throw new TypeError("Class constructor value requires a CLR Type");
        }

        int length = 0;
        if (formalParamCountValue is double d) length = (int)d;

        var scopes = scopesValue as object[] ?? EmptyScopes;
        return new ClassConstructorValue(type, scopes, length);
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
            ?? (ownerValue is ClassConstructorValue classConstructorValue
                ? classConstructorValue.Scopes
                : EmptyScopes);
        var propertyKey = JavaScriptRuntime.Object.ToPropertyKeyString(keyValue);
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
            ?? (ownerValue is ClassConstructorValue classConstructorValue
                ? classConstructorValue.Scopes
                : EmptyScopes);
        var clrMethodName = clrMethodNameValue as string
            ?? throw new TypeError("Class method definition requires a CLR method name");
        var key = JavaScriptRuntime.Object.ToPropertyKeyString(keyValue);
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

    private static Type ResolveClassOwnerType(object ownerValue)
        => ownerValue switch
        {
            Type type => type,
            ClassConstructorValue classConstructorValue => classConstructorValue.Type,
            _ => throw new TypeError("Class method definition requires a class constructor value")
        };

    internal static bool TryEnsureClassConstructorMetadataPropertyDescriptor(
        ClassConstructorValue classConstructorValue,
        string propName,
        out JsPropertyDescriptor descriptor)
    {
        if (PropertyDescriptorStore.TryGetOwn(classConstructorValue, propName, out descriptor!))
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

        if (string.Equals(propName, "prototype", StringComparison.Ordinal))
        {
            var protoObj = JavaScriptRuntime.Object.CreateOrdinaryObject();
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

            return PropertyDescriptorStore.TryGetOwn(classConstructorValue, propName, out descriptor!);
        }

        descriptor = null!;
        return false;
    }

    internal static void EnsureClassConstructorCoreMetadataProperties(object target)
    {
        if (target is not ClassConstructorValue classConstructorValue)
        {
            return;
        }

        _ = TryEnsureClassConstructorMetadataPropertyDescriptor(classConstructorValue, "length", out _);
        _ = TryEnsureClassConstructorMetadataPropertyDescriptor(classConstructorValue, "prototype", out _);
    }

    internal static bool TryEnsureLazyClassMethodDataProperty(
        object target,
        string propName,
        out JsPropertyDescriptor descriptor)
    {
        descriptor = null!;
        if (IsLazyClassMethodPropertyDeleted(target, propName)
            || !TryResolveLazyClassMethodTarget(target, out var ownerType, out var ownerValue, out var isStatic)
            || !_lazyClassMetadata.TryGetValue(ownerType, out var slot))
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

        return PropertyDescriptorStore.TryGetOwn(target, propName, out descriptor!);
    }

    internal static IEnumerable<string> GetLazyClassMethodOwnKeys(object target)
    {
        if (!TryResolveLazyClassMethodTarget(target, out var ownerType, out _, out var isStatic)
            || !_lazyClassMetadata.TryGetValue(ownerType, out var slot))
        {
            return System.Array.Empty<string>();
        }

        lock (slot)
        {
            return slot.Methods
                .Where(method => method.IsStatic == isStatic
                    && !IsLazyClassMethodPropertyDeleted(target, method.PropertyKey)
                    && !PropertyDescriptorStore.TryGetOwn(target, method.PropertyKey, out _))
                .Select(method => method.PropertyKey)
                .ToArray();
        }
    }

    internal static void MarkLazyClassMethodPropertyDeleted(object target, string propName)
    {
        if (!TryResolveLazyClassMethodTarget(target, out var ownerType, out _, out var isStatic)
            || !_lazyClassMetadata.TryGetValue(ownerType, out var slot))
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

        var deletedSlot = _deletedLazyClassMethodProperties.GetOrCreateValue(target);
        lock (deletedSlot)
        {
            deletedSlot.Keys.Add(propName);
        }
    }

    private static bool IsLazyClassMethodPropertyDeleted(object target, string propName)
    {
        if (!_deletedLazyClassMethodProperties.TryGetValue(target, out var deletedSlot))
        {
            return false;
        }

        lock (deletedSlot)
        {
            return deletedSlot.Keys.Contains(propName);
        }
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
            case ClassConstructorValue classConstructorValue:
                ownerType = classConstructorValue.Type;
                ownerValue = classConstructorValue;
                isStatic = true;
                return true;
        }

        if (PropertyDescriptorStore.TryGetOwn(target, "constructor", out var constructorDescriptor)
            && constructorDescriptor.Kind == JsPropertyDescriptorKind.Data)
        {
            switch (constructorDescriptor.Value)
            {
                case ClassConstructorValue classConstructorValue:
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
                throw new TypeError("Class method receiver is incompatible with its declaring class");
            }

            instance = receiver;
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
                ClassConstructorValue classConstructorValue => classConstructorValue.Type == ownerType,
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

        if (constructorValue is ClassConstructorValue classConstructorValue)
        {
            PropertyDescriptorStore.DefineOrUpdate(classConstructorValue, "name", descriptor);
            return classConstructorValue;
        }

        if (constructorValue is Type staticType)
        {
            PropertyDescriptorStore.DefineOrUpdate(staticType, "name", descriptor);
        }

        return constructorValue;
    }

    public static object?[]? GetCurrentArguments()
    {
        return _currentArguments.Value;
    }

    public static object?[]? SetCurrentArguments(object?[]? value)
    {
        var previous = _currentArguments.Value;
        _currentArguments.Value = value;
        return previous;
    }

    /// <summary>
    /// Saves the current arguments onto a thread-local stack and sets new arguments.
    /// Called before <c>newobj</c> so that the constructor chain can observe the actual call-site arguments
    /// via the <c>arguments</c> keyword.
    /// </summary>
    public static void PushCurrentArguments(object?[]? value)
    {
        _constructorArgStack ??= new Stack<object?[]?>();
        _constructorArgStack.Push(_currentArguments.Value);
        _currentArguments.Value = value;
    }

    /// <summary>
    /// Restores the previous arguments from the thread-local stack.
    /// Called after <c>newobj</c> completes.
    /// </summary>
    public static void PopCurrentArguments()
    {
        if (_constructorArgStack?.Count > 0)
        {
            _currentArguments.Value = _constructorArgStack.Pop();
        }
    }

    public static object? GetCurrentNewTarget()
    {
        return _currentNewTarget.Value;
    }

    public static object? SetCurrentNewTarget(object? value)
    {
        var previous = _currentNewTarget.Value;
        _currentNewTarget.Value = value;
        return previous;
    }

    public static object? GetCurrentCallee()
    {
        return _currentCallee.Value;
    }

    public static object? SetCurrentCallee(object? value)
    {
        var previous = _currentCallee.Value;
        _currentCallee.Value = value;
        return previous;
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
        var meta = _importMetaByUrl.GetOrAdd(url, static key =>
        {
            var exp = new ExpandoObject();
            var dict = (IDictionary<string, object?>)exp;
            if (!string.IsNullOrEmpty(key))
            {
                dict["url"] = key;
            }
            return exp;
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
        var args = _currentArguments.Value;
        return new ArgumentsObject(args, scopeInstance, parameterNames, includeCallee ? _currentCallee.Value : null, restrictCallee);
    }

    /// <summary>
    /// Gets the count of arguments passed to the current function.
    /// Used for rest parameter initialization.
    /// </summary>
    public static int GetArgumentCount()
    {
        var args = _currentArguments.Value;
        return args?.Length ?? 0;
    }

    /// <summary>
    /// Collects rest arguments starting from the specified index into an array.
    /// Used for rest parameter (...args) initialization.
    /// </summary>
    public static object CollectRestArguments(object startIndexObj)
    {
        // Convert to int using JavaScript number conversion
        int startIndex = startIndexObj switch
        {
            int i => i,
            double d => (int)d,
            _ => 0
        };
        
        var args = _currentArguments.Value;
        
        if (args == null || startIndex >= args.Length)
        {
            return new Array();
        }

        // Collect arguments from startIndex to end
        var restArgs = new object?[args.Length - startIndex];
        System.Array.Copy(args, startIndex, restArgs, 0, restArgs.Length);
        return new Array(restArgs);
    }

    /// <summary>
    /// Registers a module-scoped require delegate by module id/filename.
    /// Used by dynamic import() to resolve the module loading context.
    /// </summary>
    public static void RegisterModuleRequire(string moduleId, CommonJS.RequireDelegate require)
    {
        if (string.IsNullOrWhiteSpace(moduleId) || require == null)
        {
            return;
        }

        _requireByModuleId[moduleId] = require;
    }

    /// <summary>
    /// Resolves a previously-registered module-scoped require delegate.
    /// </summary>
    public static CommonJS.RequireDelegate? GetRequireForModule(string? moduleId)
    {
        if (string.IsNullOrWhiteSpace(moduleId))
        {
            return null;
        }

        return _requireByModuleId.TryGetValue(moduleId, out var require) ? require : null;
    }

    /// <summary>
    /// Creates the backing object for a JavaScript object literal.
    /// Returns a <see cref="JsObject"/> that stores numeric and boolean values without boxing.
    /// </summary>
    public static JsObject CreateObjectLiteral()
    {
        return JavaScriptRuntime.Object.CreateOrdinaryObject();
    }

    /// <summary>
    /// Cache for template objects indexed by call site ID.
    /// Per ECMA-262 spec, each unique call site should return the same template object identity.
    /// </summary>
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Array> _templateObjectCache = new();
    private const int MaxTemplateObjectCacheEntries = 4096;

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
        Object.SetProperty(templateObject, "raw", rawJsArray);

        // Template objects should be frozen (immutable)
        // For now we just return the object - freezing can be added later if needed
        return templateObject;
    }

    public static ServiceContainer BuildServiceProvider()
    {
        var container = new ServiceContainer();
        container.RegisterInstance(new GlobalThisOptions());
        
        // Register default engine dependencies
        container.Register<EngineCore.ITickSource, EngineCore.TickSource>();
        container.Register<EngineCore.IWaitHandle, EngineCore.WaitHandle>();
        container.Register<EngineCore.NodeSchedulerState>();
        container.Register<EngineCore.NodeEventLoopPump>();
        container.Register<EngineCore.ICleanupJobScheduler, EngineCore.NodeSchedulerState>();
        container.Register<EngineCore.IMicrotaskScheduler, EngineCore.NodeSchedulerState>();
        container.Register<EngineCore.IScheduler, EngineCore.NodeSchedulerState>();
        container.Register<EngineCore.IIOScheduler, EngineCore.NodeSchedulerState>();
        container.Register<EngineCore.IFinalizationRegistryHost, EngineCore.FinalizationRegistryHost>();
        container.Register<CommonJS.Require>();
        container.Register<LocalModulesAssembly>();
        container.Register<IEnvironment, DefaultEnvironment>();
        container.Register<Node.IChildProcessLauncher, Node.DefaultChildProcessLauncher>();
        
        return container;
    }
}
