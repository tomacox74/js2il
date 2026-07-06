using System.Collections.ObjectModel;
using System.Reflection;

namespace JavaScriptRuntime;

public interface IRuntimeIntrinsicCatalog
{
    IReadOnlyCollection<string> GlobalNames { get; }

    IReadOnlyCollection<string> IntrinsicObjectNames { get; }

    IReadOnlyCollection<string> ModuleSpecifiers { get; }

    IReadOnlyCollection<string> KnownGlobalNames { get; }

    bool TryGetGlobalBinding(string name, out RuntimeGlobalBindingDescriptor? descriptor);

    bool TryGetIntrinsicObject(string name, out RuntimeIntrinsicObjectDescriptor? descriptor);

    bool TryGetModuleBinding(string specifier, out RuntimeModuleBindingDescriptor? descriptor);

    bool TryGetKnownGlobal(string name, out RuntimeKnownGlobalDescriptor? descriptor);
}

public sealed class RuntimeIntrinsicCatalog : IRuntimeIntrinsicCatalog
{
    private readonly ReadOnlyDictionary<string, RuntimeGlobalBindingDescriptor> _globals;
    private readonly ReadOnlyDictionary<string, RuntimeIntrinsicObjectDescriptor> _intrinsicObjects;
    private readonly ReadOnlyDictionary<string, RuntimeModuleBindingDescriptor> _modules;
    private readonly ReadOnlyDictionary<string, RuntimeKnownGlobalDescriptor> _knownGlobals;

    public RuntimeIntrinsicCatalog()
        : this(HostRuntimeIntrinsicDescriptors.Empty)
    {
    }

    public RuntimeIntrinsicCatalog(HostRuntimeIntrinsicDescriptors? hostDescriptors)
    {
        hostDescriptors ??= HostRuntimeIntrinsicDescriptors.Empty;

        var globals = BuildBuiltInGlobalBindings();
        var intrinsicObjects = BuildBuiltInIntrinsicObjects();
        var modules = BuildBuiltInModuleBindings();
        var knownGlobals = BuildBuiltInKnownGlobals(globals);

        ApplyHostGlobalBindings(globals, knownGlobals, hostDescriptors.GlobalBindings);
        AddHostEntries(
            intrinsicObjects,
            hostDescriptors.IntrinsicObjects,
            descriptor => descriptor.Name,
            "intrinsic object");
        AddHostEntries(
            modules,
            hostDescriptors.ModuleBindings,
            descriptor => NormalizeModuleSpecifier(descriptor.Specifier),
            "module");
        AddHostKnownGlobals(knownGlobals, hostDescriptors.KnownGlobals);

        _globals = new ReadOnlyDictionary<string, RuntimeGlobalBindingDescriptor>(globals);
        _intrinsicObjects = new ReadOnlyDictionary<string, RuntimeIntrinsicObjectDescriptor>(intrinsicObjects);
        _modules = new ReadOnlyDictionary<string, RuntimeModuleBindingDescriptor>(modules);
        _knownGlobals = new ReadOnlyDictionary<string, RuntimeKnownGlobalDescriptor>(knownGlobals);
    }

    public IReadOnlyCollection<string> GlobalNames => _globals.Keys;

    public IReadOnlyCollection<string> IntrinsicObjectNames => _intrinsicObjects.Keys;

    public IReadOnlyCollection<string> ModuleSpecifiers => _modules.Keys;

    public IReadOnlyCollection<string> KnownGlobalNames => _knownGlobals.Keys;

    public bool TryGetGlobalBinding(string name, out RuntimeGlobalBindingDescriptor? descriptor)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            descriptor = null;
            return false;
        }

        return _globals.TryGetValue(name, out descriptor);
    }

    public bool TryGetIntrinsicObject(string name, out RuntimeIntrinsicObjectDescriptor? descriptor)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            descriptor = null;
            return false;
        }

        return _intrinsicObjects.TryGetValue(name, out descriptor);
    }

    public bool TryGetModuleBinding(string specifier, out RuntimeModuleBindingDescriptor? descriptor)
    {
        var key = NormalizeModuleSpecifier(specifier);
        if (string.IsNullOrWhiteSpace(key))
        {
            descriptor = null;
            return false;
        }

        return _modules.TryGetValue(key, out descriptor);
    }

    public bool TryGetKnownGlobal(string name, out RuntimeKnownGlobalDescriptor? descriptor)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            descriptor = null;
            return false;
        }

        return _knownGlobals.TryGetValue(name, out descriptor);
    }

    private static Dictionary<string, RuntimeGlobalBindingDescriptor> BuildBuiltInGlobalBindings()
    {
        var builtInGlobalAttributes = new RuntimeGlobalPropertyAttributes();
        var globals = new Dictionary<string, RuntimeGlobalBindingDescriptor>(StringComparer.Ordinal)
        {
            ["undefined"] = RuntimeGlobalBindingDescriptor.ForValue("undefined", null, builtInGlobalAttributes)
        };

        foreach (var property in typeof(GlobalThis).GetProperties(BindingFlags.Public | BindingFlags.Static))
        {
            if (property.GetMethod == null || property.GetIndexParameters().Length != 0)
            {
                continue;
            }

            globals[property.Name] = RuntimeGlobalBindingDescriptor.ForFactory(
                property.Name,
                () => property.GetValue(null),
                builtInGlobalAttributes);
        }

        AddBuiltInGlobalFunction(globals, nameof(GlobalThis.setTimeout), () => (Func<object, object, object[], object>)GlobalThis.setTimeout, builtInGlobalAttributes);
        AddBuiltInGlobalFunction(globals, nameof(GlobalThis.clearTimeout), () => (Func<object, object?>)GlobalThis.clearTimeout, builtInGlobalAttributes);
        AddBuiltInGlobalFunction(globals, nameof(GlobalThis.setImmediate), () => (Func<object, object[], object>)GlobalThis.setImmediate, builtInGlobalAttributes);
        AddBuiltInGlobalFunction(globals, nameof(GlobalThis.setInterval), () => (Func<object, object, object[], object>)GlobalThis.setInterval, builtInGlobalAttributes);
        AddBuiltInGlobalFunction(globals, nameof(GlobalThis.clearImmediate), () => (Func<object, object?>)GlobalThis.clearImmediate, builtInGlobalAttributes);
        AddBuiltInGlobalFunction(globals, nameof(GlobalThis.clearInterval), () => (Func<object, object?>)GlobalThis.clearInterval, builtInGlobalAttributes);
        AddBuiltInGlobalFunction(globals, nameof(GlobalThis.gc), () => (Func<object?>)GlobalThis.gc, builtInGlobalAttributes);
        AddBuiltInGlobalFunction(globals, nameof(GlobalThis.parseInt), () => (Func<object?, object?, double>)GlobalThis.parseInt, builtInGlobalAttributes);
        AddBuiltInGlobalFunction(globals, nameof(GlobalThis.parseFloat), () => (Func<object?, double>)GlobalThis.parseFloat, builtInGlobalAttributes);
        AddBuiltInGlobalFunction(globals, nameof(GlobalThis.isFinite), () => (Func<object?, bool>)GlobalThis.isFinite, builtInGlobalAttributes);
        AddBuiltInGlobalFunction(globals, nameof(GlobalThis.isNaN), () => (Func<object?, bool>)GlobalThis.isNaN, builtInGlobalAttributes);

        return globals;
    }

    private static Dictionary<string, RuntimeIntrinsicObjectDescriptor> BuildBuiltInIntrinsicObjects()
    {
        var intrinsicObjects = new Dictionary<string, RuntimeIntrinsicObjectDescriptor>(StringComparer.Ordinal);
        foreach (var info in IntrinsicObjectRegistry.GetAll())
        {
            intrinsicObjects[info.Name] = new RuntimeIntrinsicObjectDescriptor(info.Name, info.Type, info.CallKind);
        }

        return intrinsicObjects;
    }

    private static Dictionary<string, RuntimeModuleBindingDescriptor> BuildBuiltInModuleBindings()
    {
        var modules = new Dictionary<string, RuntimeModuleBindingDescriptor>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in Node.NodeModuleRegistry.GetSupportedModuleNames())
        {
            if (Node.NodeModuleRegistry.TryGetModuleType(name, out var moduleType) && moduleType != null)
            {
                modules[NormalizeModuleSpecifier(name)] = RuntimeModuleBindingDescriptor.ForType(name, moduleType);
            }
        }

        return modules;
    }

    private static Dictionary<string, RuntimeKnownGlobalDescriptor> BuildBuiltInKnownGlobals(
        IReadOnlyDictionary<string, RuntimeGlobalBindingDescriptor> globals)
    {
        var knownGlobals = new Dictionary<string, RuntimeKnownGlobalDescriptor>(StringComparer.Ordinal);
        foreach (var global in globals.Values)
        {
            knownGlobals[global.Name] = new RuntimeKnownGlobalDescriptor(global.Name);
        }

        return knownGlobals;
    }

    private static void ApplyHostGlobalBindings(
        Dictionary<string, RuntimeGlobalBindingDescriptor> globals,
        Dictionary<string, RuntimeKnownGlobalDescriptor> knownGlobals,
        IReadOnlyList<RuntimeGlobalBindingDescriptor> hostGlobals)
    {
        foreach (var descriptor in hostGlobals)
        {
            if (globals.ContainsKey(descriptor.Name)
                && descriptor.OverwritePolicy == RuntimeGlobalOverwritePolicy.PreserveExisting)
            {
                continue;
            }

            globals[descriptor.Name] = descriptor;
            knownGlobals[descriptor.Name] = new RuntimeKnownGlobalDescriptor(descriptor.Name);
        }
    }

    private static void AddHostKnownGlobals(
        Dictionary<string, RuntimeKnownGlobalDescriptor> knownGlobals,
        IReadOnlyList<RuntimeKnownGlobalDescriptor> hostKnownGlobals)
    {
        foreach (var descriptor in hostKnownGlobals)
        {
            knownGlobals[descriptor.Name] = descriptor;
        }
    }

    private static void AddHostEntries<T>(
        Dictionary<string, T> entries,
        IReadOnlyList<T> hostEntries,
        Func<T, string> getKey,
        string entryKind)
    {
        foreach (var descriptor in hostEntries)
        {
            var key = getKey(descriptor);
            if (entries.ContainsKey(key))
            {
                throw new InvalidOperationException($"Host runtime intrinsic {entryKind} '{key}' duplicates an existing {entryKind}.");
            }

            entries[key] = descriptor;
        }
    }

    private static void AddBuiltInGlobalFunction(
        Dictionary<string, RuntimeGlobalBindingDescriptor> globals,
        string name,
        Func<object?> valueFactory,
        RuntimeGlobalPropertyAttributes propertyAttributes)
    {
        globals[name] = RuntimeGlobalBindingDescriptor.ForFactory(name, valueFactory, propertyAttributes);
    }

    private static string NormalizeModuleSpecifier(string specifier)
    {
        return Node.NodeModuleRegistry.NormalizeModuleName(specifier);
    }
}
