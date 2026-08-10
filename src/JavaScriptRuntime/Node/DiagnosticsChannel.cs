using JavaScriptRuntime.DependencyInjection;

namespace JavaScriptRuntime.Node;

[NodeModule("diagnostics_channel")]
public sealed partial class DiagnosticsChannel
{
    private readonly DiagnosticsChannelRuntime _runtime;
    private readonly ChannelConstructor _channelConstructor;

    public DiagnosticsChannel()
    {
        _runtime = GlobalThis.ServiceProvider?.Resolve<DiagnosticsChannelRuntime>()
            ?? new DiagnosticsChannelRuntime();
        _channelConstructor = new ChannelConstructor(_runtime);
    }

    public object Channel => _channelConstructor;

    public object channel(object? name) => _runtime.GetOrCreate(name);

    public bool hasSubscribers(object? name) => _runtime.HasSubscribers(name);

    public object? subscribe(object? name, object? onMessage)
    {
        _runtime.GetOrCreate(name).Subscribe(onMessage);
        return null;
    }

    public object? subscribe(object? name, Delegate onMessage)
        => subscribe(name, (object)onMessage);

    public bool unsubscribe(object? name, object? onMessage)
        => _runtime.GetOrCreate(name).Unsubscribe(onMessage);

    public bool unsubscribe(object? name, Delegate onMessage)
        => unsubscribe(name, (object)onMessage);

    public object? tracingChannel(object? nameOrChannels)
        => throw new NotImplementedException(
            "The intrinsic node:diagnostics_channel module does not implement 'diagnostics_channel.tracingChannel'.");
}

public sealed class DiagnosticsChannelRuntime
{
    private readonly Dictionary<object, ChannelObject> _channels = new();
    private readonly object _lock = new();

    public ChannelObject GetOrCreate(object? name)
    {
        ValidateName(name);
        lock (_lock)
        {
            if (_channels.TryGetValue(name!, out var channel))
            {
                return channel;
            }

            channel = new ChannelObject(name!);
            _channels.Add(name!, channel);
            return channel;
        }
    }

    public bool HasSubscribers(object? name)
    {
        ValidateName(name);
        lock (_lock)
        {
            return _channels.TryGetValue(name!, out var channel)
                && channel.hasSubscribers;
        }
    }

    internal static void ValidateName(object? name)
    {
        if (name is not string and not Symbol)
        {
            throw new TypeError(
                "The \"channel\" argument must be of type string or symbol.");
        }
    }
}

public sealed partial class ChannelObject : JsObject
{
    private readonly List<object> _subscribers = [];
    private readonly object _lock = new();

    internal ChannelObject(object name)
    {
        this["name"] = name;
        DiagnosticsChannelSurface.DefineMethod(
            this,
            "publish",
            (Func<object[], object?[]?, object?>)((_, args) => publish(
                args is { Length: > 0 } ? args[0] : null)),
            1);
        DiagnosticsChannelSurface.DefineMethod(
            this,
            "subscribe",
            (Func<object[], object?[]?, object?>)((_, args) =>
            {
                Subscribe(args is { Length: > 0 } ? args[0] : null);
                return null;
            }),
            1);
        DiagnosticsChannelSurface.DefineMethod(
            this,
            "unsubscribe",
            (Func<object[], object?[]?, object?>)((_, args) => Unsubscribe(
                args is { Length: > 0 } ? args[0] : null)),
            1);
        PropertyDescriptorStore.DefineOrUpdate(this, "hasSubscribers", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Accessor,
            Enumerable = false,
            Configurable = true,
            Get = (Func<object[], object?[]?, object?>)((_, _) => hasSubscribers)
        });
    }

    object? Jroc.Runtime.Node.Contracts.IJavaScriptValueHost.JavaScriptValue => this;

    public bool hasSubscribers
    {
        get
        {
            lock (_lock)
            {
                return _subscribers.Count != 0;
            }
        }
    }

    public object? publish(object? message)
    {
        object[] subscribers;
        lock (_lock)
        {
            if (_subscribers.Count == 0)
            {
                return null;
            }

            subscribers = _subscribers.ToArray();
        }

        var name = this["name"];
        foreach (var subscriber in subscribers)
        {
            try
            {
                CallableOperations.Call2(subscriber, null, message, name);
            }
            catch (Exception exception)
            {
                ScheduleUncaughtException(exception);
            }
        }

        return null;
    }

    public void Subscribe(Delegate onMessage) => Subscribe((object)onMessage);

    public bool Unsubscribe(Delegate onMessage) => Unsubscribe((object)onMessage);

    internal void Subscribe(object? onMessage)
    {
        if (!CallableOperations.IsCallable(onMessage))
        {
            throw new TypeError(
                "The \"onMessage\" argument must be of type function.");
        }

        lock (_lock)
        {
            _subscribers.Add(onMessage!);
        }
    }

    internal bool Unsubscribe(object? onMessage)
    {
        lock (_lock)
        {
            var index = _subscribers.FindIndex(
                subscriber => ReferenceEquals(subscriber, onMessage));
            if (index < 0)
            {
                return false;
            }

            _subscribers.RemoveAt(index);
            return true;
        }
    }

    private static void ScheduleUncaughtException(Exception exception)
    {
        var scheduler = GlobalThis.ServiceProvider?.Resolve<EngineCore.NodeSchedulerState>();
        if (scheduler is null)
        {
            throw exception;
        }

        ((EngineCore.IScheduler)scheduler).ScheduleImmediate(
            () => System.Runtime.ExceptionServices.ExceptionDispatchInfo
                .Capture(exception)
                .Throw());
    }
}

internal sealed class ChannelConstructor(DiagnosticsChannelRuntime runtime) : JsFunctionObject
{
    public override bool IsConstructor => true;

    protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
        => throw new TypeError("Class constructor Channel cannot be invoked without 'new'");

    protected override object? ConstructCore(in JsCallArguments arguments, object? newTarget)
        => runtime.GetOrCreate(arguments.GetArgument(0));
}

internal static class DiagnosticsChannelSurface
{
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
