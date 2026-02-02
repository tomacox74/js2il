using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace JavaScriptRuntime;

/// <summary>
/// Native implementation of the ECMA-262 For-In Iterator object.
///
/// This iterator is mutation-aware:
/// - It snapshots the key list for a given "level" (target) when that level begins enumeration.
/// - Before yielding a key, it re-checks that the key is still present and enumerable.
///
/// Prototype chain support in this runtime is currently approximated for CLR objects by walking
/// the CLR type hierarchy (declared members per type). For ExpandoObject/IDictionary, an explicit
/// JS-observable [[Prototype]] chain is supported via the opt-in PrototypeChain side-table.
/// </summary>
public sealed class ForInIterator : IJavaScriptIterator<string>
{
    private readonly object _root;
    private readonly HashSet<string> _visited = new(StringComparer.Ordinal);

    // When an explicit [[Prototype]] chain is enabled/assigned for the root object,
    // we enumerate own enumerable keys for each level in that chain.
    private readonly bool _usePrototypeChain;
    private object? _currentTarget;

    // For non-reflection targets, we enumerate a single "level" and then complete.
    private bool _doneSingleTarget;

    // For reflection targets, we walk the CLR type hierarchy.
    private readonly bool _useTypeChain;
    private Type? _currentType;

    private List<string>? _currentKeys;
    private int _currentIndex;

    public ForInIterator(object root)
    {
        _root = root ?? throw new ArgumentNullException(nameof(root));

        _currentTarget = _root;
        _usePrototypeChain = PrototypeChain.Enabled
            && PrototypeChain.GetPrototypeOrNull(_root) is not null
            && PrototypeChain.GetPrototypeOrNull(_root) is not JsNull;

        // Only CLR objects (non-expando, non-array-like) use a type chain.
        _useTypeChain = !_usePrototypeChain
            && !(root is ExpandoObject)
            && root is not JavaScriptRuntime.Array
            && root is not JavaScriptRuntime.Int32Array
            && root is not string
            && root is not IDictionary;

        if (_useTypeChain)
        {
            _currentType = root.GetType();
        }
    }

    public IteratorResultObject<string> Next()
    {
        while (true)
        {
            if (_usePrototypeChain)
            {
                if (_currentTarget == null || _currentTarget is JsNull)
                {
                    return IteratorResult.Create<string>(null, done: true);
                }

                if (_currentKeys == null)
                {
                    _currentKeys = GetOwnEnumerableKeysSingleTarget(_currentTarget);
                    _currentIndex = 0;
                }

                while (_currentIndex < _currentKeys.Count)
                {
                    var key = _currentKeys[_currentIndex++];
                    if (_visited.Contains(key))
                    {
                        continue;
                    }

                    if (!IsEnumerableAndPresent(_currentTarget, key))
                    {
                        continue;
                    }

                    _visited.Add(key);
                    return IteratorResult.Create(key, done: false);
                }

                // Advance to the next prototype.
                _currentKeys = null;
                _currentTarget = PrototypeChain.GetPrototypeOrNull(_currentTarget);
                continue;
            }

            if (_useTypeChain)
            {
                if (_currentType == null || _currentType == typeof(object))
                {
                    return IteratorResult.Create<string>(null, done: true);
                }

                if (_currentKeys == null)
                {
                    _currentKeys = GetOwnEnumerableKeysForType(_root, _currentType);
                    _currentIndex = 0;
                }

                while (_currentIndex < _currentKeys.Count)
                {
                    var key = _currentKeys[_currentIndex++];
                    if (_visited.Contains(key))
                    {
                        continue;
                    }

                    if (!IsEnumerableAndPresent(_root, key))
                    {
                        continue;
                    }

                    _visited.Add(key);
                    return IteratorResult.Create(key, done: false);
                }

                // Advance to base type (prototype approximation).
                _currentKeys = null;
                _currentType = _currentType.BaseType;
                continue;
            }

            // Single-target (Expando/Array/String/IDictionary) enumeration.
            if (_doneSingleTarget)
            {
                return IteratorResult.Create<string>(null, done: true);
            }

            if (_currentKeys == null)
            {
                _currentKeys = GetOwnEnumerableKeysSingleTarget(_root);
                _currentIndex = 0;
            }

            while (_currentIndex < _currentKeys.Count)
            {
                var key = _currentKeys[_currentIndex++];
                if (_visited.Contains(key))
                {
                    continue;
                }

                if (!IsEnumerableAndPresent(_root, key))
                {
                    continue;
                }

                _visited.Add(key);
                return IteratorResult.Create(key, done: false);
            }

            _doneSingleTarget = true;
            return IteratorResult.Create<string>(null, done: true);
        }
    }

    IteratorResultObject IJavaScriptIterator.Next()
    {
        var res = ((IJavaScriptIterator<string>)this).Next();
        return IteratorResult.Create((object?)res.value, res.done);
    }

    public bool HasReturn => false;

    public void Return()
    {
        // For-in iterators do not currently expose an observable IteratorClose path in this runtime.
        // Consumers will stop calling Next(); nothing to release.
    }

    private static List<string> GetOwnEnumerableKeysSingleTarget(object target)
    {
        // ExpandoObject (object literal)
        if (target is ExpandoObject exp)
        {
            var dict = (IDictionary<string, object?>)exp;
            return dict.Keys
                .Where(k => PropertyDescriptorStore.IsEnumerableOrDefaultTrue(exp, k))
                .ToList();
        }

        // JS Array: enumerate indices
        if (target is JavaScriptRuntime.Array jsArr)
        {
            var keys = new List<string>(jsArr.Count);
            for (int i = 0; i < jsArr.Count; i++)
            {
                keys.Add(i.ToString());
            }
            return keys;
        }

        // Typed array: enumerate indices
        if (target is JavaScriptRuntime.Int32Array i32)
        {
            var keys = new List<string>((int)i32.length);
            for (int i = 0; i < i32.length; i++)
            {
                keys.Add(i.ToString());
            }
            return keys;
        }

        // String: enumerate indices
        if (target is string s)
        {
            var keys = new List<string>(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                keys.Add(i.ToString());
            }
            return keys;
        }

        // IDictionary: enumerate keys (stringified)
        if (target is IDictionary dictObj)
        {
            var keys = new List<string>();
            foreach (var k in dictObj.Keys)
            {
                var strKey = DotNet2JSConversions.ToString(k);
                if (strKey != null && PropertyDescriptorStore.IsEnumerableOrDefaultTrue(target, strKey))
                {
                    keys.Add(strKey);
                }
            }
            return keys;
        }

        return new List<string>();
    }

    private static List<string> GetOwnEnumerableKeysForType(object instance, Type type)
    {
        // Enumerate declared public instance fields/properties on the current CLR type.
        // This approximates a prototype chain for CLR objects.
        var keys = new List<string>();

        foreach (var f in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            if (f.IsSpecialName) continue;
            if (f.Name.StartsWith("<", StringComparison.Ordinal)) continue;
            keys.Add(f.Name);
        }

        foreach (var p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            if (p.GetIndexParameters().Length != 0) continue;
            var getter = p.GetMethod;
            if (getter == null || !getter.IsPublic) continue;
            if (p.IsSpecialName) continue;
            keys.Add(p.Name);
        }

        return keys;
    }

    private static bool IsEnumerableAndPresent(object target, string key)
    {
        // ExpandoObject
        if (target is ExpandoObject exp)
        {
            var dict = (IDictionary<string, object?>)exp;
            return dict.ContainsKey(key) && PropertyDescriptorStore.IsEnumerableOrDefaultTrue(exp, key);
        }

        // JS Array
        if (target is JavaScriptRuntime.Array jsArr)
        {
            if (!int.TryParse(key, out var idx) || idx < 0)
            {
                return false;
            }
            return idx < jsArr.Count;
        }

        // Typed array
        if (target is JavaScriptRuntime.Int32Array i32)
        {
            if (!int.TryParse(key, out var idx) || idx < 0)
            {
                return false;
            }
            return idx < i32.length;
        }

        // String
        if (target is string s)
        {
            if (!int.TryParse(key, out var idx) || idx < 0)
            {
                return false;
            }
            return idx < s.Length;
        }

        // IDictionary: re-check by stringifying current keys.
        if (target is IDictionary dictObj)
        {
            foreach (var k in dictObj.Keys)
            {
                if (string.Equals(DotNet2JSConversions.ToString(k), key, StringComparison.Ordinal))
                {
                    return PropertyDescriptorStore.IsEnumerableOrDefaultTrue(target, key);
                }
            }
            return false;
        }

        // CLR object: treat public instance fields/properties as enumerable.
        // Presence check uses reflection lookup on the runtime type chain.
        var t = target.GetType();
        while (t != null && t != typeof(object))
        {
            var f = t.GetField(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            if (f != null)
            {
                return true;
            }

            var p = t.GetProperty(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            if (p != null && p.GetIndexParameters().Length == 0 && p.GetMethod != null && p.GetMethod.IsPublic)
            {
                return true;
            }

            t = t.BaseType;
        }

        return false;
    }
}
