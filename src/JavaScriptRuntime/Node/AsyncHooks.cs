using System.Runtime.CompilerServices;

namespace JavaScriptRuntime.Node;

[NodeModule("async_hooks")]
public sealed partial class AsyncHooks
{
    private readonly AsyncContextRuntime _runtime;
    private readonly AsyncResourceConstructor _asyncResource;
    private readonly AsyncLocalStorageConstructor _asyncLocalStorage;
    private readonly Delegate _createHook;
    private readonly Delegate _executionAsyncId;
    private readonly Delegate _executionAsyncResource;
    private readonly Delegate _triggerAsyncId;

    public AsyncHooks()
    {
        _runtime = GlobalThis.ServiceProvider?.Resolve<AsyncContextRuntime>()
            ?? new AsyncContextRuntime();
        _asyncResource = new AsyncResourceConstructor(_runtime);
        _asyncLocalStorage = new AsyncLocalStorageConstructor(_runtime);
        _createHook = AsyncHooksSurface.CreateFunction(
            (Func<object?, object>)CreateHook,
            "createHook",
            1);
        _executionAsyncId = AsyncHooksSurface.CreateFunction(
            (Func<double>)ExecutionAsyncId,
            "executionAsyncId",
            0);
        _executionAsyncResource = AsyncHooksSurface.CreateFunction(
            (Func<object>)ExecutionAsyncResource,
            "executionAsyncResource",
            0);
        _triggerAsyncId = AsyncHooksSurface.CreateFunction(
            (Func<double>)TriggerAsyncId,
            "triggerAsyncId",
            0);
    }

    public object AsyncResource => _asyncResource;

    public object AsyncLocalStorage => _asyncLocalStorage;

    public object asyncWrapProviders
        => throw new NotImplementedException(
            "The intrinsic node:async_hooks module does not implement 'async_hooks.asyncWrapProviders'.");

    public Delegate createHook => _createHook;

    public Delegate executionAsyncId => _executionAsyncId;

    public Delegate executionAsyncResource => _executionAsyncResource;

    public Delegate triggerAsyncId => _triggerAsyncId;

    public object CreateHook(object? callbacks)
        => new AsyncHookObject(_runtime, callbacks);

    public double ExecutionAsyncId() => _runtime.ExecutionAsyncId;

    public object ExecutionAsyncResource() => _runtime.ExecutionResource;

    public double TriggerAsyncId() => _runtime.TriggerAsyncId;
}

public sealed class AsyncContextRuntime
{
    private static int _activeContextRuntimeCount;
    private static int _enabledHookCount;
    private readonly List<AsyncHookObject> _hooks = [];
    private long _nextAsyncId = 1;
    private AsyncContextFrame? _frame;
    private object _executionResource = new JsObject();
    private long _executionAsyncId = 1;
    private long _triggerAsyncId;
    private bool _hasActiveFrame;

    internal static bool HasAnyActiveContext
        => Volatile.Read(ref _activeContextRuntimeCount) != 0;

    internal static bool HasAnyEnabledHooks
        => Volatile.Read(ref _enabledHookCount) != 0;

    public double ExecutionAsyncId => _executionAsyncId;

    public object ExecutionResource => _executionResource;

    public double TriggerAsyncId => _triggerAsyncId;

    internal AsyncResourceState CreateResource(
        object receiver,
        string type,
        object? options)
    {
        var triggerAsyncId = ReadTriggerAsyncId(options) ?? _executionAsyncId;
        var requireManualDestroy = ReadBooleanOption(options, "requireManualDestroy");
        var state = new AsyncResourceState(
            this,
            Interlocked.Increment(ref _nextAsyncId),
            triggerAsyncId,
            type,
            receiver,
            _frame,
            requireManualDestroy);
        EmitInit(state);
        return state;
    }

    internal object? RunInResource(
        AsyncResourceState state,
        object? callback,
        object? thisArgument,
        object?[]? arguments)
    {
        EnsureCallable(callback);

        var priorResource = _executionResource;
        var priorAsyncId = _executionAsyncId;
        var priorTriggerAsyncId = _triggerAsyncId;
        var priorFrame = _frame;
        _executionResource = state.Resource;
        _executionAsyncId = state.AsyncId;
        _triggerAsyncId = state.TriggerAsyncId;
        SetFrame(state.Frame);

        try
        {
            EmitBefore(state.AsyncId);
            try
            {
                return CallableOperations.Call(callback, thisArgument, arguments);
            }
            finally
            {
                EmitAfter(state.AsyncId);
            }
        }
        finally
        {
            SetFrame(priorFrame);
            _triggerAsyncId = priorTriggerAsyncId;
            _executionAsyncId = priorAsyncId;
            _executionResource = priorResource;
        }
    }

    internal object? RunWithStore(
        AsyncLocalStorageObject storage,
        object? store,
        object? callback,
        object? thisArgument,
        object?[]? arguments)
    {
        EnsureCallable(callback);
        var priorFrame = _frame;
        SetFrame(new AsyncContextFrame(
            storage,
            storage.Generation,
            store,
            priorFrame));
        try
        {
            return CallableOperations.Call(callback, thisArgument, arguments);
        }
        finally
        {
            SetFrame(priorFrame);
        }
    }

    internal object? RunWithoutStore(
        AsyncLocalStorageObject storage,
        object? callback,
        object? thisArgument,
        object?[]? arguments)
    {
        EnsureCallable(callback);
        var priorFrame = _frame;
        SetFrame(RemoveStorage(priorFrame, storage));
        try
        {
            return CallableOperations.Call(callback, thisArgument, arguments);
        }
        finally
        {
            SetFrame(priorFrame);
        }
    }

    internal object? RunWithCapturedFrame(
        AsyncContextFrame? frame,
        object? callback,
        object? thisArgument,
        object?[]? arguments)
    {
        EnsureCallable(callback);
        var priorFrame = _frame;
        SetFrame(frame);
        try
        {
            return CallableOperations.Call(callback, thisArgument, arguments);
        }
        finally
        {
            SetFrame(priorFrame);
        }
    }

    internal AsyncContextFrame? CaptureFrame() => _frame;

    internal void EnterWith(AsyncLocalStorageObject storage, object? store)
        => SetFrame(new AsyncContextFrame(
            storage,
            storage.Generation,
            store,
            _frame));

    internal void Disable(AsyncLocalStorageObject storage)
    {
        storage.InvalidateContexts();
        SetFrame(RemoveStorage(_frame, storage));
    }

    internal bool TryGetStore(
        AsyncLocalStorageObject storage,
        out object? store)
    {
        for (var current = _frame; current is not null; current = current.Parent)
        {
            if (ReferenceEquals(current.Storage, storage)
                && current.StorageGeneration == storage.Generation)
            {
                store = current.Store;
                return true;
            }
        }

        store = null;
        return false;
    }

    internal void EnableHook(AsyncHookObject hook)
    {
        if (!_hooks.Contains(hook))
        {
            _hooks.Add(hook);
            Interlocked.Increment(ref _enabledHookCount);
        }
    }

    internal void DisableHook(AsyncHookObject hook)
    {
        if (_hooks.Remove(hook))
        {
            Interlocked.Decrement(ref _enabledHookCount);
        }
    }

    internal Action CaptureAction(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        var frame = _frame;
        if (frame is null)
        {
            return action;
        }

        return new CapturedAction(this, frame, action).Invoke;
    }

    internal void Reset()
    {
        SetFrame(null);
        _executionAsyncId = 1;
        _triggerAsyncId = 0;
        _executionResource = new JsObject();
        _nextAsyncId = 1;
        if (_hooks.Count != 0)
        {
            Interlocked.Add(ref _enabledHookCount, -_hooks.Count);
            _hooks.Clear();
        }
    }

    internal static AsyncContextSnapshot? CaptureCurrentSnapshot()
    {
        if (!HasAnyActiveContext
            || GlobalThis.ServiceProvider?.TryResolve<AsyncContextRuntime>(
                out var runtime) != true)
        {
            return null;
        }

        var frame = runtime!._frame;
        return frame is null ? null : new AsyncContextSnapshot(runtime, frame);
    }

    internal static object BindCurrentCallback(object callback)
    {
        var snapshot = CaptureCurrentSnapshot();
        return snapshot is null
            ? callback
            : new ContextBoundFunctionObject(snapshot, callback);
    }

    internal static void RunJobSnapshot(
        AsyncContextSnapshot? snapshot,
        AsyncResourceState? resourceState,
        in EngineCore.JobCallbackRecord jobCallback)
    {
        if (resourceState is not null)
        {
            resourceState.Runtime.RunPromiseJob(
                resourceState,
                snapshot?.Frame,
                jobCallback);
            return;
        }

        if (snapshot is null)
        {
            if (HasAnyActiveContext
                && GlobalThis.ServiceProvider?.TryResolve<AsyncContextRuntime>(
                    out var runtime) == true
                && runtime!._frame is not null)
            {
                runtime.RunWithFrame(null, jobCallback);
            }
            else
            {
                EngineCore.HostJobCallbacks.HostCallJobCallback(
                    jobCallback,
                    v: null,
                    argumentsList: System.Array.Empty<object?>());
            }
            return;
        }

        snapshot.Runtime.RunWithFrame(snapshot.Frame, jobCallback);
    }

    internal static AsyncResourceState? TryCreatePromiseResource(object promise)
    {
        if (!HasAnyEnabledHooks
            || GlobalThis.ServiceProvider?.TryResolve<AsyncContextRuntime>(
                out var runtime) != true)
        {
            return null;
        }

        return runtime!.CreateResource(promise, "PROMISE", null);
    }

    internal static void EmitPromiseResolve(AsyncResourceState? state)
    {
        state?.Runtime.EmitPromiseResolveCore(state.AsyncId);
    }

    internal void EmitDestroy(AsyncResourceState state)
    {
        if (state.DestroyEmitted)
        {
            return;
        }

        state.DestroyEmitted = true;
        EmitHook(hook => hook.Destroy, state.AsyncId);
    }

    private static void EnsureCallable(object? callback)
    {
        if (!CallableOperations.IsCallable(callback))
        {
            throw new TypeError("The \"fn\" argument must be of type function.");
        }
    }

    private void EmitInit(AsyncResourceState state)
    {
        if (_hooks.Count == 0)
        {
            return;
        }

        foreach (var hook in _hooks.ToArray())
        {
            if (hook.Init is { } callback)
            {
                CallableOperations.Call4(
                    callback,
                    null,
                    (double)state.AsyncId,
                    state.Type,
                    (double)state.TriggerAsyncId,
                    state.Resource);
            }
        }
    }

    private void EmitBefore(long asyncId)
        => EmitHook(hook => hook.Before, asyncId);

    private void EmitAfter(long asyncId)
        => EmitHook(hook => hook.After, asyncId);

    private void EmitPromiseResolveCore(long asyncId)
        => EmitHook(hook => hook.PromiseResolve, asyncId);

    private void EmitHook(Func<AsyncHookObject, object?> selector, long asyncId)
    {
        if (_hooks.Count == 0)
        {
            return;
        }

        foreach (var hook in _hooks.ToArray())
        {
            if (selector(hook) is { } callback)
            {
                CallableOperations.Call1(callback, null, (double)asyncId);
            }
        }
    }

    private static AsyncContextFrame? RemoveStorage(
        AsyncContextFrame? frame,
        AsyncLocalStorageObject storage)
    {
        if (frame is null)
        {
            return null;
        }

        var parent = RemoveStorage(frame.Parent, storage);
        return ReferenceEquals(frame.Storage, storage)
            ? parent
            : new AsyncContextFrame(
                frame.Storage,
                frame.StorageGeneration,
                frame.Store,
                parent);
    }

    private static long? ReadTriggerAsyncId(object? options)
    {
        if (options is null || options is JsNull)
        {
            return null;
        }

        var value = ObjectRuntime.GetProperty(options, "triggerAsyncId");
        if (value is null)
        {
            return null;
        }

        var number = TypeUtilities.ToNumber(value);
        if (double.IsNaN(number) || number < -1 || number != System.Math.Truncate(number))
        {
            throw new RangeError("The value of \"options.triggerAsyncId\" is out of range.");
        }

        return checked((long)number);
    }

    private static bool ReadBooleanOption(object? options, string name)
    {
        if (options is null || options is JsNull)
        {
            return false;
        }

        var value = ObjectRuntime.GetProperty(options, name);
        return value is not null && TypeUtilities.ToBoolean(value);
    }

    private void RunWithFrame(AsyncContextFrame? frame, Action callback)
    {
        var priorFrame = _frame;
        SetFrame(frame);
        try
        {
            callback();
        }
        finally
        {
            SetFrame(priorFrame);
        }
    }

    private void RunWithFrame(
        AsyncContextFrame? frame,
        in EngineCore.JobCallbackRecord jobCallback)
    {
        var priorFrame = _frame;
        SetFrame(frame);
        try
        {
            EngineCore.HostJobCallbacks.HostCallJobCallback(
                jobCallback,
                v: null,
                argumentsList: System.Array.Empty<object?>());
        }
        finally
        {
            SetFrame(priorFrame);
        }
    }

    private void RunPromiseJob(
        AsyncResourceState state,
        AsyncContextFrame? frame,
        in EngineCore.JobCallbackRecord jobCallback)
    {
        var priorResource = _executionResource;
        var priorAsyncId = _executionAsyncId;
        var priorTriggerAsyncId = _triggerAsyncId;
        var priorFrame = _frame;
        _executionResource = state.Resource;
        _executionAsyncId = state.AsyncId;
        _triggerAsyncId = state.TriggerAsyncId;
        SetFrame(frame);

        try
        {
            EmitBefore(state.AsyncId);
            try
            {
                EngineCore.HostJobCallbacks.HostCallJobCallback(
                    jobCallback,
                    v: null,
                    argumentsList: System.Array.Empty<object?>());
            }
            finally
            {
                EmitAfter(state.AsyncId);
            }
        }
        finally
        {
            SetFrame(priorFrame);
            _triggerAsyncId = priorTriggerAsyncId;
            _executionAsyncId = priorAsyncId;
            _executionResource = priorResource;
        }
    }

    private void SetFrame(AsyncContextFrame? frame)
    {
        frame = RemoveInactiveStorages(frame);
        _frame = frame;
        var hasFrame = frame is not null;
        if (hasFrame == _hasActiveFrame)
        {
            return;
        }

        _hasActiveFrame = hasFrame;
        if (hasFrame)
        {
            Interlocked.Increment(ref _activeContextRuntimeCount);
        }
        else
        {
            Interlocked.Decrement(ref _activeContextRuntimeCount);
        }
    }

    private sealed class CapturedAction(
        AsyncContextRuntime runtime,
        AsyncContextFrame frame,
        Action callback)
    {
        public void Invoke() => runtime.RunWithFrame(frame, callback);
    }

    private sealed class ContextBoundFunctionObject : JsFunctionObject
    {
        private readonly AsyncContextSnapshot _snapshot;
        private readonly object _callback;

        public ContextBoundFunctionObject(
            AsyncContextSnapshot snapshot,
            object callback)
        {
            _snapshot = snapshot;
            _callback = callback;
            Function.DefineMetadataProperty(
                this,
                "name",
                AsyncHooksSurface.GetFunctionName(callback));
            Function.DefineMetadataProperty(
                this,
                "length",
                AsyncHooksSurface.GetFunctionLength(callback));
        }

        public override bool RequiresInvocationContext => false;

        protected override object? CallCore(
            object? thisArgument,
            in JsCallArguments arguments)
            => _snapshot.Runtime.RunWithCapturedFrame(
                _snapshot.Frame,
                _callback,
                thisArgument,
                arguments.ToArray());
    }

    private static AsyncContextFrame? RemoveInactiveStorages(
        AsyncContextFrame? frame)
    {
        if (frame is null)
        {
            return null;
        }

        var parent = RemoveInactiveStorages(frame.Parent);
        if (frame.StorageGeneration != frame.Storage.Generation)
        {
            return parent;
        }

        return ReferenceEquals(parent, frame.Parent)
            ? frame
            : new AsyncContextFrame(
                frame.Storage,
                frame.StorageGeneration,
                frame.Store,
                parent);
    }
}

internal sealed record AsyncContextFrame(
    AsyncLocalStorageObject Storage,
    int StorageGeneration,
    object? Store,
    AsyncContextFrame? Parent);

internal sealed record AsyncContextSnapshot(
    AsyncContextRuntime Runtime,
    AsyncContextFrame Frame);

internal sealed class AsyncResourceState(
    AsyncContextRuntime runtime,
    long asyncId,
    long triggerAsyncId,
    string type,
    object resource,
    AsyncContextFrame? frame,
    bool requireManualDestroy)
{
    public AsyncContextRuntime Runtime { get; } = runtime;

    public long AsyncId { get; } = asyncId;

    public bool DestroyEmitted { get; set; }

    public AsyncContextFrame? Frame { get; } = frame;

    public bool RequireManualDestroy { get; } = requireManualDestroy;

    public object Resource { get; } = resource;

    public long TriggerAsyncId { get; } = triggerAsyncId;

    public string Type { get; } = type;
}

internal sealed class AsyncResourceConstructor : JsFunctionObject
{
    private readonly AsyncContextRuntime _runtime;

    public AsyncResourceConstructor(AsyncContextRuntime runtime)
    {
        _runtime = runtime;
        Prototype = new JsObject();
        AsyncHooksSurface.DefineMethod(
            Prototype,
            "runInAsyncScope",
            (Func<object[], object?[]?, object?>)RunInAsyncScope,
            1);
        AsyncHooksSurface.DefineMethod(
            Prototype,
            "emitDestroy",
            (Func<object[], object?[]?, object?>)EmitDestroy,
            0);
        AsyncHooksSurface.DefineMethod(
            Prototype,
            "asyncId",
            (Func<object[], object?[]?, object?>)AsyncId,
            0);
        AsyncHooksSurface.DefineMethod(
            Prototype,
            "triggerAsyncId",
            (Func<object[], object?[]?, object?>)TriggerAsyncId,
            0);
        AsyncHooksSurface.DefineMethod(
            Prototype,
            "bind",
            (Func<object[], object?[]?, object?>)Bind,
            1);
        this["prototype"] = Prototype;
        Prototype["constructor"] = this;
        this["bind"] = AsyncHooksSurface.CreateFunction(
            (Func<object[], object?[]?, object?>)StaticBind,
            "bind",
            2);
    }

    public override bool IsConstructor => true;

    public JsObject Prototype { get; }

    protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
        => throw new TypeError("Class constructor AsyncResource cannot be invoked without 'new'");

    protected override object? ConstructCore(in JsCallArguments arguments, object? newTarget)
    {
        var resource = new AsyncResourceObject(_runtime, Prototype);
        Initialize(
            resource,
            arguments.GetArgument(0),
            arguments.Count > 1 ? arguments.GetArgument(1) : null);
        return resource;
    }

    protected override object? ConstructBodyCore(
        object receiver,
        in JsCallArguments arguments,
        object? newTarget)
    {
        Initialize(
            receiver,
            arguments.GetArgument(0),
            arguments.Count > 1 ? arguments.GetArgument(1) : null);
        return null;
    }

    private void Initialize(object receiver, object? type, object? options)
    {
        if (type is not string resourceType || resourceType.Length == 0)
        {
            throw new TypeError("The \"type\" argument must be of type string.");
        }

        var state = _runtime.CreateResource(receiver, resourceType, options);
        AsyncResourceObject.SetState(receiver, state);
    }

    private static object? RunInAsyncScope(object[] scopes, object?[]? args)
    {
        var receiver = RuntimeServices.GetCurrentThis()!;
        var state = AsyncResourceObject.GetState(receiver);
        return state.Runtime.RunInResource(
            state.State,
            args is { Length: > 0 } ? args[0] : null,
            args is { Length: > 1 } ? args[1] : null,
            args is { Length: > 2 } ? args[2..] : null);
    }

    private static object? EmitDestroy(object[] scopes, object?[]? args)
    {
        var receiver = RuntimeServices.GetCurrentThis()!;
        var state = AsyncResourceObject.GetState(receiver);
        state.Runtime.EmitDestroy(state.State);
        return receiver;
    }

    private static object? AsyncId(object[] scopes, object?[]? args)
        => (double)AsyncResourceObject.GetState(
            RuntimeServices.GetCurrentThis()!).State.AsyncId;

    private static object? TriggerAsyncId(object[] scopes, object?[]? args)
        => (double)AsyncResourceObject.GetState(
            RuntimeServices.GetCurrentThis()!).State.TriggerAsyncId;

    private static object? Bind(object[] scopes, object?[]? args)
    {
        var receiver = RuntimeServices.GetCurrentThis()!;
        return AsyncResourceObject.GetState(receiver).CreateBound(
            args is { Length: > 0 } ? args[0] : null,
            args is { Length: > 1 } ? args[1] : receiver);
    }

    private static object? StaticBind(object[] scopes, object?[]? args)
    {
        var constructor = (AsyncResourceConstructor)RuntimeServices.GetCurrentThis()!;
        var type = args is { Length: > 1 }
            ? DotNet2JSConversions.ToString(args[1])
            : args is { Length: > 0 }
                ? AsyncHooksSurface.GetFunctionName(args[0])
                : string.Empty;
        if (type.Length == 0)
        {
            type = "bound-anonymous-fn";
        }
        var resource = new AsyncResourceObject(constructor._runtime, constructor.Prototype);
        constructor.Initialize(resource, type, null);
        return resource.bind(
            args is { Length: > 0 } ? args[0]! : null!,
            args is { Length: > 2 } ? args[2] : resource);
    }
}

public sealed partial class AsyncResourceObject : JsObject
{
    private static readonly ConditionalWeakTable<object, ResourceHolder> States = new();
    private readonly AsyncContextRuntime _runtime;

    internal AsyncResourceObject(AsyncContextRuntime runtime, object prototype)
    {
        _runtime = runtime;
        PrototypeChain.SetPrototype(this, prototype);
    }

    object? Jroc.Runtime.Node.Contracts.IJavaScriptValueHost.JavaScriptValue => this;

    public Delegate bind(object fn) => bind(fn, this);

    public Delegate bind(object fn, object? thisArg)
        => GetState(this).CreateBound(fn, thisArg);

    public object emitDestroy()
    {
        var holder = GetState(this);
        holder.Runtime.EmitDestroy(holder.State);
        return this;
    }

    public double asyncId() => GetState(this).State.AsyncId;

    public object? runInAsyncScope(object fn)
        => runInAsyncScope(fn, null, []);

    public object? runInAsyncScope(object fn, object? thisArg, params object?[] args)
    {
        var holder = GetState(this);
        return holder.Runtime.RunInResource(holder.State, fn, thisArg, args);
    }

    public double triggerAsyncId() => GetState(this).State.TriggerAsyncId;

    internal static ResourceHolder GetState(object receiver)
    {
        if (!States.TryGetValue(receiver, out var holder))
        {
            throw new TypeError(
                "The \"this\" argument must be an instance of AsyncResource.");
        }

        return holder;
    }

    internal static void SetState(object receiver, AsyncResourceState state)
    {
        States.Remove(receiver);
        States.Add(receiver, new ResourceHolder(state.Runtime, state));
    }

    internal sealed class ResourceHolder(
        AsyncContextRuntime runtime,
        AsyncResourceState state)
    {
        public AsyncContextRuntime Runtime { get; } = runtime;

        public AsyncResourceState State { get; } = state;

        public Delegate CreateBound(object? callback, object? thisArgument)
        {
            if (!CallableOperations.IsCallable(callback))
            {
                throw new TypeError("The \"fn\" argument must be of type function.");
            }

            Func<object[], object?[]?, object?> bound = (_, args)
                => Runtime.RunInResource(State, callback, thisArgument, args);
            return AsyncHooksSurface.CreateFunction(
                bound,
                AsyncHooksSurface.GetFunctionName(callback),
                AsyncHooksSurface.GetFunctionLength(callback));
        }
    }
}

internal sealed class AsyncLocalStorageConstructor : JsFunctionObject
{
    private readonly AsyncContextRuntime _runtime;

    public AsyncLocalStorageConstructor(AsyncContextRuntime runtime)
    {
        _runtime = runtime;
        Prototype = new JsObject();
        AsyncHooksSurface.DefineMethod(
            Prototype,
            "disable",
            (Func<object[], object?[]?, object?>)Disable,
            0);
        AsyncHooksSurface.DefineMethod(
            Prototype,
            "getStore",
            (Func<object[], object?[]?, object?>)GetStore,
            0);
        AsyncHooksSurface.DefineMethod(
            Prototype,
            "enterWith",
            (Func<object[], object?[]?, object?>)EnterWith,
            1);
        AsyncHooksSurface.DefineMethod(
            Prototype,
            "run",
            (Func<object[], object?[]?, object?>)Run,
            2);
        AsyncHooksSurface.DefineMethod(
            Prototype,
            "exit",
            (Func<object[], object?[]?, object?>)Exit,
            1);
        PropertyDescriptorStore.DefineOrUpdate(Prototype, "name", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Accessor,
            Enumerable = false,
            Configurable = true,
            Get = (Func<object[], object?[]?, object?>)Name
        });
        this["prototype"] = Prototype;
        Prototype["constructor"] = this;
        this["bind"] = AsyncHooksSurface.CreateFunction(
            (Func<object[], object?[]?, object?>)Bind,
            "bind",
            1);
        this["snapshot"] = AsyncHooksSurface.CreateFunction(
            (Func<object[], object?[]?, object?>)Snapshot,
            "snapshot",
            0);
    }

    public override bool IsConstructor => true;

    public JsObject Prototype { get; }

    protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
        => throw new TypeError(
            "Class constructor AsyncLocalStorage cannot be invoked without 'new'");

    protected override object? ConstructCore(in JsCallArguments arguments, object? newTarget)
        => new AsyncLocalStorageObject(
            _runtime,
            arguments.Count > 0 ? arguments.GetArgument(0) : null,
            Prototype);

    private static AsyncLocalStorageObject GetReceiver(object[] scopes)
        => RuntimeServices.GetCurrentThis() as AsyncLocalStorageObject
            ?? throw new TypeError(
                "The \"this\" argument must be an instance of AsyncLocalStorage.");

    private static object? Disable(object[] scopes, object?[]? args)
        => GetReceiver(scopes).disable();

    private static object? GetStore(object[] scopes, object?[]? args)
        => GetReceiver(scopes).getStore();

    private static object? EnterWith(object[] scopes, object?[]? args)
        => GetReceiver(scopes).enterWith(args is { Length: > 0 } ? args[0] : null);

    private static object? Run(object[] scopes, object?[]? args)
        => GetReceiver(scopes).run(
            args is { Length: > 0 } ? args[0] : null,
            args is { Length: > 1 } ? args[1] : null,
            args is { Length: > 2 } ? args[2..] : []);

    private static object? Exit(object[] scopes, object?[]? args)
        => GetReceiver(scopes).exit(
            args is { Length: > 0 } ? args[0] : null,
            args is { Length: > 1 } ? args[1..] : []);

    private static object? Name(object[] scopes, object?[]? args)
        => GetReceiver(scopes).name;

    private static object? Bind(object[] scopes, object?[]? args)
    {
        var constructor = (AsyncLocalStorageConstructor)RuntimeServices.GetCurrentThis()!;
        var callback = args is { Length: > 0 } ? args[0] : null;
        var frame = constructor._runtime.CaptureFrame();
        if (!CallableOperations.IsCallable(callback))
        {
            throw new TypeError("The \"fn\" argument must be of type function.");
        }

        Func<object[], object?[]?, object?> bound = (callScopes, callArgs)
            => constructor._runtime.RunWithCapturedFrame(
                frame,
                callback,
                RuntimeServices.GetCurrentThis(),
                callArgs);
        return AsyncHooksSurface.CreateFunction(
            bound,
            AsyncHooksSurface.GetFunctionName(callback),
            AsyncHooksSurface.GetFunctionLength(callback));
    }

    private static object? Snapshot(object[] scopes, object?[]? args)
    {
        var constructor = (AsyncLocalStorageConstructor)RuntimeServices.GetCurrentThis()!;
        var frame = constructor._runtime.CaptureFrame();
        Func<object[], object?[]?, object?> snapshot = (callScopes, callArgs)
            => constructor._runtime.RunWithCapturedFrame(
                frame,
                callArgs is { Length: > 0 } ? callArgs[0] : null,
                RuntimeServices.GetCurrentThis(),
                callArgs is { Length: > 1 } ? callArgs[1..] : null);
        return AsyncHooksSurface.CreateFunction(snapshot, string.Empty, 1);
    }
}

public sealed partial class AsyncLocalStorageObject : JsObject
{
    private readonly AsyncContextRuntime _runtime;
    private int _generation;

    internal AsyncLocalStorageObject(
        AsyncContextRuntime runtime,
        object? options,
        object prototype)
    {
        _runtime = runtime;
        PrototypeChain.SetPrototype(this, prototype);
        if (options is not null && options is not JsNull)
        {
            name = ObjectRuntime.GetProperty(options, "name") as string;
            var defaultValue = ObjectRuntime.GetProperty(options, "defaultValue");
            if (defaultValue is not null)
            {
                DefaultValue = defaultValue;
                HasDefaultValue = true;
            }
        }
    }

    object? Jroc.Runtime.Node.Contracts.IJavaScriptValueHost.JavaScriptValue => this;

    public string? name { get; }

    private object? DefaultValue { get; }

    private bool HasDefaultValue { get; }

    internal int Generation => Volatile.Read(ref _generation);

    internal void InvalidateContexts()
        => Interlocked.Increment(ref _generation);

    public object? disable()
    {
        _runtime.Disable(this);
        return null;
    }

    public object? enterWith(object? store)
    {
        _runtime.EnterWith(this, store);
        return null;
    }

    public object? exit(object? callback, params object?[] args)
        => _runtime.RunWithoutStore(this, callback, null, args);

    public object? getStore()
        => _runtime.TryGetStore(this, out var store)
            ? store
            : HasDefaultValue
                ? DefaultValue
                : null;

    public object? run(object? store, object? callback, params object?[] args)
        => _runtime.RunWithStore(this, store, callback, null, args);
}

public sealed partial class AsyncHookObject : JsObject
{
    private readonly AsyncContextRuntime _runtime;

    internal AsyncHookObject(AsyncContextRuntime runtime, object? callbacks)
    {
        if (callbacks is null || callbacks is JsNull)
        {
            throw new TypeError("The \"callbacks\" argument must be of type object.");
        }

        _runtime = runtime;
        Init = ReadCallback(callbacks, "init");
        Before = ReadCallback(callbacks, "before");
        After = ReadCallback(callbacks, "after");
        Destroy = ReadCallback(callbacks, "destroy");
        PromiseResolve = ReadCallback(callbacks, "promiseResolve");
        this["enable"] = AsyncHooksSurface.CreateFunction(
            (Func<object?>)enable,
            "enable",
            0);
        this["disable"] = AsyncHooksSurface.CreateFunction(
            (Func<object?>)disable,
            "disable",
            0);
    }

    object? Jroc.Runtime.Node.Contracts.IJavaScriptValueHost.JavaScriptValue => this;

    internal object? After { get; }

    internal object? Before { get; }

    internal object? Destroy { get; }

    internal object? Init { get; }

    internal object? PromiseResolve { get; }

    public object disable()
    {
        _runtime.DisableHook(this);
        return this;
    }

    public object enable()
    {
        _runtime.EnableHook(this);
        return this;
    }

    private static object? ReadCallback(object callbacks, string name)
    {
        var callback = ObjectRuntime.GetProperty(callbacks, name);
        if (callback is null)
        {
            return null;
        }

        if (!CallableOperations.IsCallable(callback))
        {
            throw new TypeError($"Hook callback \"{name}\" must be a function.");
        }

        return callback;
    }
}

internal static class AsyncHooksSurface
{
    public static double GetFunctionLength(object? callback)
    {
        if (callback is null)
        {
            return 0;
        }

        var value = ObjectRuntime.GetProperty(callback, "length");
        return value is null ? 0 : TypeUtilities.ToNumber(value);
    }

    public static string GetFunctionName(object? callback)
    {
        if (callback is null)
        {
            return string.Empty;
        }

        return ObjectRuntime.GetProperty(callback, "name") as string ?? string.Empty;
    }

    public static T CreateFunction<T>(T callback, string name, double length)
        where T : Delegate
    {
        Function.InitializeFunctionInstance(
            callback,
            length,
            name,
            requiresInvocationContext: true);
        Function.MarkUndefinedPrototype(callback);
        return callback;
    }

    public static void DefineMethod(
        object target,
        string name,
        Delegate callback,
        double length)
    {
        Function.InitializeFunctionInstance(
            callback,
            length,
            name,
            requiresInvocationContext: true);
        Function.MarkUndefinedPrototype(callback);
        PropertyDescriptorStore.DefineOrUpdate(target, name, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = true,
            Value = callback
        });
    }
}
