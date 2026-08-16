namespace JavaScriptRuntime;

using JavaScriptRuntime.DependencyInjection;
using JavaScriptRuntime.Modules.CommonJS;

internal sealed class RuntimeExecutionContext
{
    private static readonly AsyncLocal<AmbientState?> Ambient = new();

    private readonly object _stateGate = new();
    private GlobalThis? _globalObject;
    private string _moduleDirectory = string.Empty;
    private string _moduleFilename = string.Empty;

    private RuntimeExecutionContext(
        RuntimeRealm realm,
        bool isHosted,
        string? compiledAssemblyPath)
    {
        Realm = realm;
        IsHosted = isHosted;
        CompiledAssemblyPath = compiledAssemblyPath;
        DescriptorStore = realm.Services.Resolve<IPropertyDescriptorStore>();
    }

    internal static RuntimeExecutionContext? Current
        => Ambient.Value?.Frame?.Context;

    internal static ServiceContainer? ServiceProviderOverride
    {
        get => Ambient.Value?.ServiceProviderOverride;
        set
        {
            var current = Ambient.Value;
            Ambient.Value = new AmbientState(
                current?.Frame,
                value);
        }
    }

    internal RuntimeRealm Realm { get; }

    internal RuntimeAgent Agent => Realm.Agent;

    internal ServiceContainer Services => Realm.Services;

    internal IPropertyDescriptorStore DescriptorStore { get; }

    internal bool IsHosted { get; private set; }

    internal string? CompiledAssemblyPath { get; private set; }

    internal static RuntimeExecutionContext GetOrCreate(
        ServiceContainer services,
        bool? isHosted = null,
        string? compiledAssemblyPath = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (services.TryResolve<RuntimeExecutionContext>(out var existing)
            && existing != null)
        {
            if (isHosted.HasValue)
            {
                existing.Configure(isHosted.Value, compiledAssemblyPath);
            }

            return existing;
        }

        var realm = services.OwningRealm
            ?? throw new InvalidOperationException(
                "Runtime service containers must have an owning realm.");
        var context = new RuntimeExecutionContext(
            realm,
            isHosted ?? false,
            compiledAssemblyPath);
        services.RegisterInstance(context);
        return context;
    }

    internal static RuntimeExecutionContext? CurrentOrOverride
    {
        get
        {
            if (Current is { } current)
            {
                return current;
            }

            var services = ServiceProviderOverride;
            return services == null
                ? null
                : GetOrCreate(services);
        }
    }

    internal IDisposable Enter()
        => EnterCore(asRoot: false);

    internal IDisposable EnterAsRoot()
        => EnterCore(asRoot: true);

    internal static IDisposable SuppressInheritedState()
    {
        var previous = Ambient.Value;
        var invocationState = RuntimeServices.CaptureAndClearAmbientInvocationState();
        try
        {
            SetAmbient(null);
            return new AmbientSuppressionScope(previous, invocationState);
        }
        catch
        {
            RuntimeServices.RestoreAmbientInvocationState(invocationState);
            throw;
        }
    }

    internal static void SetLegacyServiceProvider(ServiceContainer? services)
    {
        var current = Ambient.Value;
        if (current?.Frame is { IsLegacy: false })
        {
            throw new InvalidOperationException(
                "The active runtime execution frame must be exited through its scope.");
        }

        var frame = services == null
            ? null
            : CreateFrame(
                GetOrCreate(services),
                isLegacy: true,
                inheritActiveModuleState: true);
        SetAmbient(new AmbientState(
            frame,
            current?.ServiceProviderOverride));
    }

    internal void SetModuleLocation(string directory, string filename)
    {
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(filename);

        if (Ambient.Value?.Frame is { } frame
            && ReferenceEquals(frame.Context, this))
        {
            frame.ModuleDirectory = directory;
            frame.ModuleFilename = filename;
            return;
        }

        lock (_stateGate)
        {
            _moduleDirectory = directory;
            _moduleFilename = filename;
        }
    }

    internal (string Directory, string Filename) GetModuleLocation()
    {
        if (Ambient.Value?.Frame is { } frame
            && ReferenceEquals(frame.Context, this))
        {
            return (frame.ModuleDirectory, frame.ModuleFilename);
        }

        lock (_stateGate)
        {
            return (_moduleDirectory, _moduleFilename);
        }
    }

    internal Module? GetCurrentParentModule()
    {
        var frame = Ambient.Value?.Frame;
        return frame != null && ReferenceEquals(frame.Context, this)
            ? frame.CurrentParentModule
            : null;
    }

    internal void SetCurrentParentModule(Module? module)
    {
        var frame = Ambient.Value?.Frame;
        if (frame == null || !ReferenceEquals(frame.Context, this))
        {
            throw new InvalidOperationException(
                "Active require state requires a runtime execution frame.");
        }

        frame.CurrentParentModule = module;
    }

    internal GlobalThis GetOrCreateGlobalObject()
    {
        var instance = Volatile.Read(ref _globalObject);
        if (instance == null)
        {
            lock (_stateGate)
            {
                instance = _globalObject;
                if (instance == null)
                {
                    // Publish before bootstrapping (see GlobalThis.Bootstrap remarks): a
                    // reentrant globalThis/GetOrCreateGlobalObject() lookup that happens
                    // during this realm's own intrinsic bootstrap must observe this
                    // instance instead of recursively constructing another one.
                    instance = new GlobalThis(Realm.Intrinsics);
                    Volatile.Write(ref _globalObject, instance);
                }
            }
        }

        // Deliberately outside _stateGate: bootstrap materializes intrinsics and must not
        // pull this context lock into the intrinsic lock order. Bootstrap is idempotent,
        // blocks concurrent callers until the realm graph is wired, and returns
        // immediately for the thread that is running the bootstrap.
        instance.Bootstrap();
        return instance;
    }

    private IDisposable EnterCore(bool asRoot)
    {
        if (Realm.IsDisposed)
        {
            throw new ObjectDisposedException(nameof(RuntimeRealm));
        }

        var previous = Ambient.Value;
        object? invocationState = null;
        if (asRoot)
        {
            invocationState = RuntimeServices.CaptureAndClearAmbientInvocationState();
        }

        var frame = CreateFrame(
            this,
            isLegacy: false,
            inheritActiveModuleState: !asRoot);
        try
        {
            SetAmbient(new AmbientState(
                frame,
                asRoot ? null : previous?.ServiceProviderOverride));
            return new ExecutionScope(
                frame,
                previous,
                invocationState);
        }
        catch
        {
            if (invocationState != null)
            {
                RuntimeServices.RestoreAmbientInvocationState(invocationState);
            }

            throw;
        }
    }

    private void Configure(bool isHosted, string? compiledAssemblyPath)
    {
        lock (_stateGate)
        {
            IsHosted = isHosted;
            CompiledAssemblyPath = compiledAssemblyPath;
        }
    }

    private static void SetAmbient(AmbientState? next)
    {
        var previousStore = Ambient.Value?.Frame?.Context.DescriptorStore;
        Ambient.Value = next;
        PropertyDescriptorStore.OnExecutionContextChanged(
            previousStore,
            next?.Frame?.Context.DescriptorStore);
    }

    private static ExecutionFrame CreateFrame(
        RuntimeExecutionContext context,
        bool isLegacy,
        bool inheritActiveModuleState)
    {
        var (directory, filename) = context.GetModuleLocation();
        var frame = new ExecutionFrame(
            context,
            isLegacy,
            directory,
            filename);
        if (inheritActiveModuleState
            && Ambient.Value?.Frame is { } current
            && ReferenceEquals(current.Context, context))
        {
            frame.CurrentParentModule = current.CurrentParentModule;
        }

        return frame;
    }

    private sealed record AmbientState(
        ExecutionFrame? Frame,
        ServiceContainer? ServiceProviderOverride);

    private sealed class ExecutionFrame
    {
        internal ExecutionFrame(
            RuntimeExecutionContext context,
            bool isLegacy,
            string moduleDirectory,
            string moduleFilename)
        {
            Context = context;
            IsLegacy = isLegacy;
            ModuleDirectory = moduleDirectory;
            ModuleFilename = moduleFilename;
        }

        internal RuntimeExecutionContext Context { get; }

        internal bool IsLegacy { get; }

        internal string ModuleDirectory { get; set; }

        internal string ModuleFilename { get; set; }

        internal Module? CurrentParentModule { get; set; }
    }

    private sealed class ExecutionScope : IDisposable
    {
        private readonly ExecutionFrame _frame;
        private readonly AmbientState? _previous;
        private readonly object? _invocationState;
        private bool _disposed;

        internal ExecutionScope(
            ExecutionFrame frame,
            AmbientState? previous,
            object? invocationState)
        {
            _frame = frame;
            _previous = previous;
            _invocationState = invocationState;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (!ReferenceEquals(Ambient.Value?.Frame, _frame))
            {
                throw new InvalidOperationException(
                    "Runtime execution frames must be exited in reverse entry order.");
            }

            SetAmbient(_previous);
            if (_invocationState != null)
            {
                RuntimeServices.RestoreAmbientInvocationState(_invocationState);
            }

            _disposed = true;
        }
    }

    private sealed class AmbientSuppressionScope : IDisposable
    {
        private readonly AmbientState? _previous;
        private readonly object _invocationState;
        private bool _disposed;

        internal AmbientSuppressionScope(
            AmbientState? previous,
            object invocationState)
        {
            _previous = previous;
            _invocationState = invocationState;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (Ambient.Value?.Frame != null)
            {
                throw new InvalidOperationException(
                    "The runtime execution frame must exit before inherited state is restored.");
            }

            SetAmbient(_previous);
            RuntimeServices.RestoreAmbientInvocationState(_invocationState);
            _disposed = true;
        }
    }
}
