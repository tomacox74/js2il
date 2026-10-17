namespace JavaScriptRuntime.Modules.ESM;

internal sealed class ModuleNamespace : JsObject, IExoticJsObject
{
    private object? _exports;
    private readonly string[] _exportNames;
    private readonly string _tagKey;
    private readonly Func<object?>? _evaluate;

    internal ModuleNamespace(object exports)
        : this(ObjectRuntime.GetOwnEnumerableKeysInOrder(exports), null)
    {
        _exports = exports;
    }

    internal ModuleNamespace(IEnumerable<string> exportNames, Func<object?>? evaluate)
    {
        _exportNames = exportNames.Order(StringComparer.Ordinal).ToArray();
        _evaluate = evaluate;
        _tagKey = ObjectRuntime.ToPropertyKeyString(Symbol.toStringTag);
        PrototypeChain.InitializePrototype(this, JsNull.Null);
        ObjectRuntime.preventExtensions(this);
    }

    private bool IsSymbolLike(string key)
        => ObjectRuntime.IsEncodedSymbolKey(key) || _evaluate is not null && key == "then";

    private void EnsureEvaluated()
    {
        if (_exports is null && _evaluate is not null)
            _exports = _evaluate();
    }

    internal override bool HasOwnPropertyValue(string key)
    {
        if (IsSymbolLike(key))
            return key == _tagKey;
        EnsureEvaluated();
        return System.Array.BinarySearch(_exportNames, key, StringComparer.Ordinal) >= 0;
    }

    internal override bool TryGetOwnPropertyValue(string key, out object? value)
    {
        if (!HasOwnPropertyValue(key))
        {
            value = null;
            return false;
        }
        value = key == _tagKey
            ? _evaluate is null ? "Module" : "Deferred Module"
            : ObjectRuntime.GetProperty(_exports!, key);
        return true;
    }

    internal override PropertyDescriptorLookup GetOwnPropertyDescriptor(string key, out JsPropertyDescriptor descriptor)
    {
        if (!TryGetOwnPropertyValue(key, out var value))
        {
            descriptor = default;
            return PropertyDescriptorLookup.None;
        }
        descriptor = new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Value = value,
            Writable = key != _tagKey,
            Enumerable = key != _tagKey,
            Configurable = false
        };
        return PropertyDescriptorLookup.Found;
    }

    internal override bool DefineOwnProperty(string key, JsPropertyDescriptor descriptor)
        => GetOwnPropertyDescriptor(key, out var current) == PropertyDescriptorLookup.Found
            && descriptor.Kind == JsPropertyDescriptorKind.Data
            && descriptor.Writable == current.Writable
            && descriptor.Enumerable == current.Enumerable
            && !descriptor.Configurable
            && Operators.SameValue(descriptor.Value, current.Value);

    internal override bool SetOwnPropertyValue(string key, object? value) => false;

    internal override bool DeleteOwnProperty(string key) => !HasOwnPropertyValue(key);

    internal override IEnumerable<string> GetOwnPropertyKeys()
    {
        EnsureEvaluated();
        return _exportNames.Append(_tagKey);
    }
}
