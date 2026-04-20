using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace JavaScriptRuntime;

public sealed class ArgumentsObject : IDictionary<string, object?>
{
    private static readonly ConcurrentDictionary<(Type ScopeType, string Name), FieldInfo?> _fieldCache = new();

    private readonly object?[] _indexedValues;
    private readonly bool[] _indexedPresent;
    private readonly string?[] _mappedParameterNames;
    private readonly object? _scopeInstance;
    private Dictionary<string, object?>? _extraProperties;
    private object? _lengthValue;
    private bool _hasLengthProperty = true;
    private object? _calleeValue;
    private bool _hasCalleeProperty;

    public ArgumentsObject(object?[]? args, object? scopeInstance, string[]? parameterNames, object? calleeValue)
    {
        _indexedValues = args != null && args.Length > 0 ? (object?[])args.Clone() : [];
        _indexedPresent = new bool[_indexedValues.Length];
        System.Array.Fill(_indexedPresent, true);
        _mappedParameterNames = BuildMappedParameterNames(parameterNames, _indexedValues.Length);
        _scopeInstance = scopeInstance;
        _lengthValue = (double)_indexedValues.Length;
        _calleeValue = calleeValue;
        _hasCalleeProperty = calleeValue is not null;
        UpdateLengthDescriptor();
        UpdateCalleeDescriptor();
    }

    public object? this[string key]
    {
        get
        {
            if (TryGetValue(key, out var value))
            {
                return value;
            }

            throw new KeyNotFoundException($"Key '{key}' not found.");
        }
        set => SetValue(key, value);
    }

    public ICollection<string> Keys => EnumerateOwnKeys().ToArray();

    public ICollection<object?> Values => EnumerateOwnKeys().Select(key => this[key]).ToArray();

    public int Count => EnumerateOwnKeys().Count();

    public bool IsReadOnly => false;

    public void Add(string key, object? value) => SetValue(key, value);

    public bool ContainsKey(string key)
    {
        if (string.Equals(key, "length", StringComparison.Ordinal))
        {
            return _hasLengthProperty;
        }

        if (string.Equals(key, "callee", StringComparison.Ordinal))
        {
            return _hasCalleeProperty;
        }

        if (TryGetIndexedSlot(key, out var index))
        {
            if (index < _indexedPresent.Length)
            {
                return _indexedPresent[index];
            }

            return _extraProperties?.ContainsKey(key) ?? false;
        }

        return _extraProperties?.ContainsKey(key) ?? false;
    }

    public bool Remove(string key)
    {
        if (string.Equals(key, "length", StringComparison.Ordinal))
        {
            if (!_hasLengthProperty)
            {
                return false;
            }

            _hasLengthProperty = false;
            PropertyDescriptorStore.Delete(this, "length");
            return true;
        }

        if (string.Equals(key, "callee", StringComparison.Ordinal))
        {
            if (!_hasCalleeProperty)
            {
                return false;
            }

            _hasCalleeProperty = false;
            _calleeValue = null;
            PropertyDescriptorStore.Delete(this, "callee");
            return true;
        }

        if (TryGetIndexedSlot(key, out var index))
        {
            if (index >= _indexedPresent.Length)
            {
                return _extraProperties?.Remove(key) ?? false;
            }

            if (!_indexedPresent[index])
            {
                return false;
            }

            _indexedPresent[index] = false;
            _indexedValues[index] = null;
            _mappedParameterNames[index] = null;
            return true;
        }

        return _extraProperties?.Remove(key) ?? false;
    }

    public bool TryGetValue(string key, out object? value)
    {
        if (string.Equals(key, "length", StringComparison.Ordinal))
        {
            value = _hasLengthProperty ? _lengthValue : null;
            return _hasLengthProperty;
        }

        if (string.Equals(key, "callee", StringComparison.Ordinal))
        {
            value = _calleeValue;
            return _hasCalleeProperty;
        }

        if (TryGetIndexedSlot(key, out var index))
        {
            if (index >= _indexedPresent.Length)
            {
                if (_extraProperties != null && _extraProperties.TryGetValue(key, out value))
                {
                    return true;
                }

                value = null;
                return false;
            }

            if (!_indexedPresent[index])
            {
                value = null;
                return false;
            }

            var mappedParameterName = _mappedParameterNames[index];
            value = mappedParameterName != null
                ? ReadMappedParameterValue(mappedParameterName)
                : _indexedValues[index];
            return true;
        }

        if (_extraProperties != null && _extraProperties.TryGetValue(key, out value))
        {
            return true;
        }

        value = null;
        return false;
    }

    public void Add(KeyValuePair<string, object?> item) => SetValue(item.Key, item.Value);

    public void Clear()
    {
        for (var i = 0; i < _indexedPresent.Length; i++)
        {
            _indexedPresent[i] = false;
            _indexedValues[i] = null;
            _mappedParameterNames[i] = null;
        }

        _extraProperties?.Clear();
        _hasLengthProperty = false;
        _hasCalleeProperty = false;
        _calleeValue = null;
        PropertyDescriptorStore.Delete(this, "length");
        PropertyDescriptorStore.Delete(this, "callee");
    }

    public bool Contains(KeyValuePair<string, object?> item)
        => TryGetValue(item.Key, out var value) && Equals(value, item.Value);

    public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
    {
        foreach (var item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    public bool Remove(KeyValuePair<string, object?> item)
        => Contains(item) && Remove(item.Key);

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        foreach (var key in EnumerateOwnKeys())
        {
            yield return new KeyValuePair<string, object?>(key, this[key]);
        }
    }

    public IJavaScriptIterator values() => new ValuesIterator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal IJavaScriptIterator CreateValueIterator() => new ValueIterator(this);

    private static string?[] BuildMappedParameterNames(string[]? parameterNames, int argumentCount)
    {
        var mappedNames = new string?[argumentCount];
        if (parameterNames == null || parameterNames.Length == 0)
        {
            return mappedNames;
        }

        var seenNames = new HashSet<string>(StringComparer.Ordinal);
        for (var i = System.Math.Min(argumentCount, parameterNames.Length) - 1; i >= 0; i--)
        {
            var parameterName = parameterNames[i];
            if (seenNames.Add(parameterName))
            {
                mappedNames[i] = parameterName;
            }
        }

        return mappedNames;
    }

    private IEnumerable<string> EnumerateOwnKeys()
    {
        for (var i = 0; i < _indexedPresent.Length; i++)
        {
            if (_indexedPresent[i])
            {
                yield return i.ToString(CultureInfo.InvariantCulture);
            }
        }

        if (_extraProperties == null)
        {
            goto descriptorKeys;
        }

        foreach (var key in _extraProperties.Keys)
        {
            yield return key;
        }

descriptorKeys:
        if (_hasLengthProperty)
        {
            yield return "length";
        }

        if (_hasCalleeProperty)
        {
            yield return "callee";
        }
    }

    private static bool TryGetIndexedSlot(string key, out int index)
    {
        if (!ObjectRuntime.TryParseCanonicalIndexString(key, out index))
        {
            return false;
        }

        return index >= 0;
    }

    private object? ReadMappedParameterValue(string parameterName)
    {
        if (_scopeInstance == null)
        {
            return null;
        }

        var field = _fieldCache.GetOrAdd((_scopeInstance.GetType(), parameterName), static key =>
            key.ScopeType.GetField(key.Name, BindingFlags.Instance | BindingFlags.Public));
        return field?.GetValue(_scopeInstance);
    }

    private void SetMappedParameterValue(string parameterName, object? value)
    {
        if (_scopeInstance == null)
        {
            return;
        }

        var field = _fieldCache.GetOrAdd((_scopeInstance.GetType(), parameterName), static key =>
            key.ScopeType.GetField(key.Name, BindingFlags.Instance | BindingFlags.Public));
        field?.SetValue(_scopeInstance, value);
    }

    private void SetValue(string key, object? value)
    {
        if (string.Equals(key, "length", StringComparison.Ordinal))
        {
            _lengthValue = value;
            _hasLengthProperty = true;
            UpdateLengthDescriptor();
            return;
        }

        if (string.Equals(key, "callee", StringComparison.Ordinal))
        {
            _calleeValue = value;
            _hasCalleeProperty = true;
            UpdateCalleeDescriptor();
            return;
        }

        if (TryGetIndexedSlot(key, out var index) && index < _indexedValues.Length)
        {
            var mappedParameterName = _mappedParameterNames[index];
            if (_indexedPresent[index] && mappedParameterName != null)
            {
                SetMappedParameterValue(mappedParameterName, value);
            }

            _indexedValues[index] = value;
            _indexedPresent[index] = true;
            return;
        }

        (_extraProperties ??= new Dictionary<string, object?>(StringComparer.Ordinal))[key] = value;
    }

    private void UpdateLengthDescriptor()
    {
        PropertyDescriptorStore.DefineOrUpdate(this, "length", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = true,
            Value = _lengthValue
        });
    }

    private void UpdateCalleeDescriptor()
    {
        if (!_hasCalleeProperty)
        {
            PropertyDescriptorStore.Delete(this, "callee");
            return;
        }

        PropertyDescriptorStore.DefineOrUpdate(this, "callee", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = true,
            Value = _calleeValue
        });
    }

    private sealed class ValueIterator : IJavaScriptIterator
    {
        private readonly ArgumentsObject _argumentsObject;
        private int _index;
        private bool _isClosed;

        public ValueIterator(ArgumentsObject argumentsObject)
        {
            _argumentsObject = argumentsObject;
            Iterator.InitializeIteratorSurface(this);
        }

        public bool HasReturn => true;

        public IteratorResultObject Next()
        {
            if (_isClosed)
            {
                return new IteratorResultObject(null, done: true);
            }

            var length = System.Math.Max(0, TypeUtilities.ToInt32(_argumentsObject._lengthValue));
            if (_index >= length)
            {
                return new IteratorResultObject(null, done: true);
            }

            var value = ObjectRuntime.GetItem(_argumentsObject, (double)_index);
            _index++;
            return new IteratorResultObject(value, done: false);
        }

        public void Return()
        {
            _isClosed = true;
        }
    }
}
