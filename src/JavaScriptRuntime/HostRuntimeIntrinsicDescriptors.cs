using System.Collections.ObjectModel;

namespace JavaScriptRuntime;

public enum RuntimeGlobalOverwritePolicy
{
    PreserveExisting = 0,
    ReplaceExisting
}

public sealed record RuntimeGlobalPropertyAttributes(
    bool Enumerable = false,
    bool Configurable = true,
    bool Writable = true);

public sealed class RuntimeGlobalBindingDescriptor
{
    private RuntimeGlobalBindingDescriptor(
        string name,
        object? value,
        Func<object?>? valueFactory,
        RuntimeGlobalPropertyAttributes propertyAttributes,
        RuntimeGlobalOverwritePolicy overwritePolicy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(propertyAttributes);

        Name = name;
        Value = value;
        ValueFactory = valueFactory;
        PropertyAttributes = propertyAttributes;
        OverwritePolicy = overwritePolicy;
    }

    /// <summary>
    /// Gets the JavaScript global binding name exposed to hosted code.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the fixed value used for the global binding when <see cref="ValueFactory" /> is not configured.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Gets the factory used to create the global binding value for each runtime application.
    /// </summary>
    public Func<object?>? ValueFactory { get; }

    /// <summary>
    /// Gets the JavaScript property attributes applied when the global binding is defined.
    /// </summary>
    public RuntimeGlobalPropertyAttributes PropertyAttributes { get; }

    /// <summary>
    /// Gets the overwrite policy used when a global binding with the same name already exists.
    /// </summary>
    public RuntimeGlobalOverwritePolicy OverwritePolicy { get; }

    /// <summary>
    /// Gets a value indicating whether this descriptor creates values through <see cref="ValueFactory" />.
    /// </summary>
    public bool UsesFactory => ValueFactory != null;

    public object? CreateValue() => ValueFactory != null ? ValueFactory() : Value;

    public static RuntimeGlobalBindingDescriptor ForValue(
        string name,
        object? value,
        RuntimeGlobalPropertyAttributes? propertyAttributes = null,
        RuntimeGlobalOverwritePolicy overwritePolicy = RuntimeGlobalOverwritePolicy.PreserveExisting)
    {
        return new RuntimeGlobalBindingDescriptor(
            name,
            value,
            valueFactory: null,
            propertyAttributes ?? new RuntimeGlobalPropertyAttributes(),
            overwritePolicy);
    }

    public static RuntimeGlobalBindingDescriptor ForFactory(
        string name,
        Func<object?> valueFactory,
        RuntimeGlobalPropertyAttributes? propertyAttributes = null,
        RuntimeGlobalOverwritePolicy overwritePolicy = RuntimeGlobalOverwritePolicy.PreserveExisting)
    {
        ArgumentNullException.ThrowIfNull(valueFactory);

        return new RuntimeGlobalBindingDescriptor(
            name,
            value: null,
            valueFactory,
            propertyAttributes ?? new RuntimeGlobalPropertyAttributes(),
            overwritePolicy);
    }
}

public sealed class RuntimeIntrinsicObjectDescriptor
{
    public RuntimeIntrinsicObjectDescriptor(
        string name,
        Type type,
        IntrinsicCallKind callKind = IntrinsicCallKind.None)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(type);

        Name = name;
        Type = type;
        CallKind = callKind;
    }

    public string Name { get; }

    public Type Type { get; }

    public IntrinsicCallKind CallKind { get; }
}

public sealed class RuntimeModuleBindingDescriptor
{
    private RuntimeModuleBindingDescriptor(string specifier, Type? moduleType, Func<object?>? moduleFactory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(specifier);

        Specifier = specifier;
        ModuleType = moduleType;
        ModuleFactory = moduleFactory;
    }

    public string Specifier { get; }

    public Type? ModuleType { get; }

    public Func<object?>? ModuleFactory { get; }

    public bool UsesFactory => ModuleFactory != null;

    public object? CreateModule()
    {
        if (ModuleFactory == null)
        {
            throw new InvalidOperationException("This module descriptor does not define a module factory.");
        }

        return ModuleFactory();
    }

    public static RuntimeModuleBindingDescriptor ForType(string specifier, Type moduleType)
    {
        ArgumentNullException.ThrowIfNull(moduleType);
        return new RuntimeModuleBindingDescriptor(specifier, moduleType, moduleFactory: null);
    }

    public static RuntimeModuleBindingDescriptor ForFactory(string specifier, Func<object?> moduleFactory)
    {
        ArgumentNullException.ThrowIfNull(moduleFactory);
        return new RuntimeModuleBindingDescriptor(specifier, moduleType: null, moduleFactory);
    }
}

public sealed class RuntimeKnownGlobalDescriptor
{
    public RuntimeKnownGlobalDescriptor(string name, Type? valueType = null, bool isConstant = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        ValueType = valueType;
        IsConstant = isConstant;
    }

    public string Name { get; }

    public Type? ValueType { get; }

    public bool IsConstant { get; }
}

public sealed class HostRuntimeIntrinsicDescriptors
{
    public static HostRuntimeIntrinsicDescriptors Empty { get; } = new(
        global::System.Array.Empty<RuntimeGlobalBindingDescriptor>(),
        global::System.Array.Empty<RuntimeIntrinsicObjectDescriptor>(),
        global::System.Array.Empty<RuntimeModuleBindingDescriptor>(),
        global::System.Array.Empty<RuntimeKnownGlobalDescriptor>());

    internal HostRuntimeIntrinsicDescriptors(
        IReadOnlyList<RuntimeGlobalBindingDescriptor> globalBindings,
        IReadOnlyList<RuntimeIntrinsicObjectDescriptor> intrinsicObjects,
        IReadOnlyList<RuntimeModuleBindingDescriptor> moduleBindings,
        IReadOnlyList<RuntimeKnownGlobalDescriptor> knownGlobals)
    {
        GlobalBindings = Freeze(globalBindings);
        IntrinsicObjects = Freeze(intrinsicObjects);
        ModuleBindings = Freeze(moduleBindings);
        KnownGlobals = Freeze(knownGlobals);
    }

    public IReadOnlyList<RuntimeGlobalBindingDescriptor> GlobalBindings { get; }

    public IReadOnlyList<RuntimeIntrinsicObjectDescriptor> IntrinsicObjects { get; }

    public IReadOnlyList<RuntimeModuleBindingDescriptor> ModuleBindings { get; }

    public IReadOnlyList<RuntimeKnownGlobalDescriptor> KnownGlobals { get; }

    private static ReadOnlyCollection<T> Freeze<T>(IReadOnlyList<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var copy = new T[source.Count];
        for (var i = 0; i < source.Count; i++)
        {
            copy[i] = source[i];
        }

        return global::System.Array.AsReadOnly(copy);
    }
}

public sealed class HostRuntimeIntrinsicDescriptorsBuilder
{
    private readonly List<RuntimeGlobalBindingDescriptor> _globalBindings = new();
    private readonly List<RuntimeIntrinsicObjectDescriptor> _intrinsicObjects = new();
    private readonly List<RuntimeModuleBindingDescriptor> _moduleBindings = new();
    private readonly List<RuntimeKnownGlobalDescriptor> _knownGlobals = new();

    public HostRuntimeIntrinsicDescriptorsBuilder AddGlobal(RuntimeGlobalBindingDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        _globalBindings.Add(descriptor);
        return this;
    }

    public HostRuntimeIntrinsicDescriptorsBuilder AddGlobalValue(
        string name,
        object? value,
        RuntimeGlobalPropertyAttributes? propertyAttributes = null,
        RuntimeGlobalOverwritePolicy overwritePolicy = RuntimeGlobalOverwritePolicy.PreserveExisting)
    {
        return AddGlobal(RuntimeGlobalBindingDescriptor.ForValue(name, value, propertyAttributes, overwritePolicy));
    }

    public HostRuntimeIntrinsicDescriptorsBuilder AddGlobalFactory(
        string name,
        Func<object?> valueFactory,
        RuntimeGlobalPropertyAttributes? propertyAttributes = null,
        RuntimeGlobalOverwritePolicy overwritePolicy = RuntimeGlobalOverwritePolicy.PreserveExisting)
    {
        return AddGlobal(RuntimeGlobalBindingDescriptor.ForFactory(name, valueFactory, propertyAttributes, overwritePolicy));
    }

    public HostRuntimeIntrinsicDescriptorsBuilder AddIntrinsicObject(RuntimeIntrinsicObjectDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        _intrinsicObjects.Add(descriptor);
        return this;
    }

    public HostRuntimeIntrinsicDescriptorsBuilder AddIntrinsicObject(
        string name,
        Type type,
        IntrinsicCallKind callKind = IntrinsicCallKind.None)
    {
        return AddIntrinsicObject(new RuntimeIntrinsicObjectDescriptor(name, type, callKind));
    }

    public HostRuntimeIntrinsicDescriptorsBuilder AddModule(RuntimeModuleBindingDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        _moduleBindings.Add(descriptor);
        return this;
    }

    public HostRuntimeIntrinsicDescriptorsBuilder AddModuleType(string specifier, Type moduleType)
    {
        return AddModule(RuntimeModuleBindingDescriptor.ForType(specifier, moduleType));
    }

    public HostRuntimeIntrinsicDescriptorsBuilder AddModuleFactory(string specifier, Func<object?> moduleFactory)
    {
        return AddModule(RuntimeModuleBindingDescriptor.ForFactory(specifier, moduleFactory));
    }

    public HostRuntimeIntrinsicDescriptorsBuilder AddKnownGlobal(RuntimeKnownGlobalDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        _knownGlobals.Add(descriptor);
        return this;
    }

    public HostRuntimeIntrinsicDescriptorsBuilder AddKnownGlobal(
        string name,
        Type? valueType = null,
        bool isConstant = false)
    {
        return AddKnownGlobal(new RuntimeKnownGlobalDescriptor(name, valueType, isConstant));
    }

    public HostRuntimeIntrinsicDescriptors Build()
    {
        return new HostRuntimeIntrinsicDescriptors(_globalBindings, _intrinsicObjects, _moduleBindings, _knownGlobals);
    }
}
