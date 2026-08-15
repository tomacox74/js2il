namespace JavaScriptRuntime;

using JavaScriptRuntime.DependencyInjection;
using JavaScriptRuntime.Modules.CommonJS;

internal sealed class RuntimeExecutionContext
{
    private static readonly AsyncLocal<AmbientState?> Ambient = new();

    private readonly object _stateGate = new();
    private readonly List<KeyValuePair<string, RequireDelegate>> _registeredModuleRequires = [];
    private GlobalThis? _globalObject;

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

    internal string ModuleDirectory { get; private set; } = string.Empty;

    internal string ModuleFilename { get; private set; } = string.Empty;

    internal IReadOnlyList<KeyValuePair<string, RequireDelegate>> RegisteredModuleRequires
        => _registeredModuleRequires;

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
            : new ExecutionFrame(
                GetOrCreate(services),
                IsLegacy: true);
        SetAmbient(new AmbientState(
            frame,
            current?.ServiceProviderOverride));
    }

    internal void SetModuleLocation(string directory, string filename)
    {
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(filename);

        lock (_stateGate)
        {
            ModuleDirectory = directory;
            ModuleFilename = filename;
        }
    }

    internal (string Directory, string Filename) GetModuleLocation()
    {
        lock (_stateGate)
        {
            return (ModuleDirectory, ModuleFilename);
        }
    }

    internal GlobalThis GetOrCreateGlobalObject()
    {
        lock (_stateGate)
        {
            return _globalObject ??= new GlobalThis();
        }
    }

    internal void TrackModuleRequire(string moduleId, RequireDelegate require)
    {
        _registeredModuleRequires.Add(
            new KeyValuePair<string, RequireDelegate>(moduleId, require));
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

        var frame = new ExecutionFrame(this, IsLegacy: false);
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

    private sealed record AmbientState(
        ExecutionFrame? Frame,
        ServiceContainer? ServiceProviderOverride);

    private sealed record ExecutionFrame(
        RuntimeExecutionContext Context,
        bool IsLegacy);

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
}
