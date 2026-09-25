using System;
using System.Collections.Generic;

namespace JavaScriptRuntime;

public static partial class Iterator
{
    private static object? ConstructorZip(object? thisArgument, object? iterables, object? options)
        => CreateZip(iterables, options, keyed: false);

    private static object? ConstructorZipKeyed(object? thisArgument, object? iterables, object? options)
        => CreateZip(iterables, options, keyed: true);

    private static object CreateZip(object? iterables, object? options, bool keyed)
    {
        if (!Proxy.IsObjectLikeValue(iterables))
        {
            throw new TypeError("Iterator.zip requires an object");
        }

        if (options is not null && !Proxy.IsObjectLikeValue(options))
        {
            throw new TypeError("Iterator.zip options must be an object");
        }

        var modeValue = options is null ? null : ObjectRuntime.GetProperty(options, "mode");
        var mode = modeValue is null ? "shortest" : modeValue as string;
        if (mode is not ("shortest" or "longest" or "strict"))
        {
            throw new TypeError("Invalid Iterator.zip mode");
        }

        var paddingOption = mode == "longest" && options is not null
            ? ObjectRuntime.GetProperty(options, "padding")
            : null;
        if (paddingOption is not null && !Proxy.IsObjectLikeValue(paddingOption))
        {
            throw new TypeError("Iterator.zip padding must be an object");
        }

        var iterators = new List<IteratorRecord>();
        var keys = new List<string>();
        if (keyed)
        {
            var ownKeys = ObjectRuntime.GetOwnPropertyKeysInOrder(iterables!, includeEncodedSymbolKeys: true);
            foreach (var key in ownKeys)
            {
                try
                {
                    var propertyKey = ObjectRuntime.ToExternalPropertyKey(key);
                    var descriptor = ObjectRuntime.getOwnPropertyDescriptor(iterables!, propertyKey);
                    if (descriptor is null
                        || !TypeUtilities.ToBoolean(ObjectRuntime.GetProperty(descriptor, "enumerable")))
                    {
                        continue;
                    }

                    var value = ObjectRuntime.GetItem(iterables!, propertyKey);
                    if (value is null)
                    {
                        continue;
                    }

                    keys.Add(key);
                    iterators.Add(GetIteratorFlattenable(value, allowStrings: false, "zipKeyed"));
                }
                catch
                {
                    CloseAllPreservingThrow(iterators);
                    throw;
                }
            }
        }
        else
        {
            var input = GetIterableIterator(iterables!);
            while (true)
            {
                IteratorStep step;
                try
                {
                    step = input.Next();
                }
                catch
                {
                    CloseAllPreservingThrow(iterators);
                    throw;
                }

                if (step.Done)
                {
                    break;
                }

                try
                {
                    iterators.Add(GetIteratorFlattenable(step.Value, allowStrings: false, "zip"));
                }
                catch
                {
                    CloseAllPreservingThrow(iterators);
                    input.ClosePreservingThrow();
                    throw;
                }
            }
        }

        var padding = new object?[iterators.Count];
        if (paddingOption is not null)
        {
            try
            {
                if (keyed)
                {
                    for (var i = 0; i < keys.Count; i++)
                    {
                        padding[i] = ObjectRuntime.GetItem(paddingOption, ObjectRuntime.ToExternalPropertyKey(keys[i]));
                    }
                }
                else
                {
                    var paddingIterator = GetIterableIterator(paddingOption);
                    var exhausted = false;
                    for (var i = 0; i < padding.Length; i++)
                    {
                        if (exhausted)
                        {
                            break;
                        }

                        var step = paddingIterator.Next();
                        if (step.Done)
                        {
                            exhausted = true;
                        }
                        else
                        {
                            padding[i] = step.Value;
                        }
                    }

                    if (!exhausted)
                    {
                        paddingIterator.Close();
                    }
                }
            }
            catch
            {
                CloseAllPreservingThrow(iterators);
                throw;
            }
        }

        return new ZipIteratorHelper(iterators, keys, padding, mode, keyed);
    }

    private static IteratorRecord GetIterableIterator(object value)
    {
        var method = ObjectRuntime.GetItem(value, Symbol.iterator);
        if (!CallableOperations.IsCallable(method))
        {
            throw new TypeError("Value is not iterable");
        }

        var iterator = CallableOperations.Call(method, value, System.Array.Empty<object?>());
        if (!Proxy.IsObjectLikeValue(iterator))
        {
            throw new TypeError("Iterator method must return an object");
        }

        return GetIteratorDirect(iterator!);
    }

    private static void CloseAllPreservingThrow(List<IteratorRecord> iterators)
    {
        for (var i = iterators.Count - 1; i >= 0; i--)
        {
            iterators[i].ClosePreservingThrow();
        }
    }

    private sealed class ZipIteratorHelper : JsObject, IJavaScriptIterator
    {
        private readonly IteratorRecord[] _iterators;
        private readonly bool[] _open;
        private readonly List<string> _keys;
        private readonly object?[] _padding;
        private readonly string _mode;
        private readonly bool _keyed;
        private bool _done;
        private bool _executing;
        private bool _yielded;

        public ZipIteratorHelper(
            List<IteratorRecord> iterators,
            List<string> keys,
            object?[] padding,
            string mode,
            bool keyed)
        {
            _iterators = iterators.ToArray();
            _open = new bool[_iterators.Length];
            System.Array.Fill(_open, true);
            _keys = keys;
            _padding = padding;
            _mode = mode;
            _keyed = keyed;
            InitializeHelperSurface(this);
        }

        public bool HasReturn => true;

        public IteratorResultObject Next()
        {
            if (_executing)
            {
                throw new TypeError("Iterator helper is already running");
            }

            if (_done)
            {
                return IteratorResult.Create(null, true);
            }

            _executing = true;
            try
            {
                if (_iterators.Length == 0)
                {
                    _done = true;
                    return IteratorResult.Create(null, true);
                }

                var results = new object?[_iterators.Length];
                for (var i = 0; i < _iterators.Length; i++)
                {
                    if (!_open[i])
                    {
                        results[i] = _padding[i];
                        continue;
                    }

                    IteratorStep step;
                    try
                    {
                        step = _iterators[i].Next();
                    }
                    catch
                    {
                        _open[i] = false;
                        throw;
                    }

                    if (!step.Done)
                    {
                        results[i] = step.Value;
                        continue;
                    }

                    _open[i] = false;
                    if (_mode == "shortest")
                    {
                        _done = true;
                        CloseAll();
                        return IteratorResult.Create(null, true);
                    }

                    if (_mode == "strict")
                    {
                        if (i == 0)
                        {
                            for (var j = 1; j < _iterators.Length; j++)
                            {
                                try
                                {
                                    if (_iterators[j].StepIsDone())
                                    {
                                        _open[j] = false;
                                        continue;
                                    }
                                }
                                catch
                                {
                                    _open[j] = false;
                                    throw;
                                }

                                throw new TypeError("Iterators have different lengths");
                            }

                            _done = true;
                            return IteratorResult.Create(null, true);
                        }

                        throw new TypeError("Iterators have different lengths");
                    }

                    if (!HasOpenIterators())
                    {
                        _done = true;
                        return IteratorResult.Create(null, true);
                    }

                    results[i] = _padding[i];
                }

                object value;
                if (_keyed)
                {
                    var result = ObjectRuntime.CreateOrdinaryObject();
                    PrototypeChain.SetPrototype(result, JsNull.Null);
                    for (var i = 0; i < results.Length; i++)
                    {
                        if (!ObjectRuntime.CreateDataProperty(result, _keys[i], results[i]))
                        {
                            throw new TypeError("Could not create a zipped property");
                        }
                    }

                    value = result;
                }
                else
                {
                    var result = new JavaScriptRuntime.Array();
                    foreach (var item in results)
                    {
                        result.Add(item);
                    }

                    value = result;
                }

                _yielded = true;
                return IteratorResult.Create(value, false);
            }
            catch
            {
                _done = true;
                CloseAllPreservingThrow();
                throw;
            }
            finally
            {
                _executing = false;
            }
        }

        public void Return()
        {
            if (_executing)
            {
                throw new TypeError("Iterator helper is already running");
            }

            if (_done)
            {
                return;
            }

            _done = true;
            _executing = _yielded;
            try
            {
                CloseAll();
            }
            finally
            {
                _executing = false;
            }
        }

        private bool HasOpenIterators()
        {
            foreach (var open in _open)
            {
                if (open)
                {
                    return true;
                }
            }

            return false;
        }

        private void CloseAll()
        {
            Exception? failure = null;
            for (var i = _open.Length - 1; i >= 0; i--)
            {
                if (!_open[i])
                {
                    continue;
                }

                _open[i] = false;
                try
                {
                    _iterators[i].Close();
                }
                catch (Exception ex)
                {
                    failure ??= ex;
                }
            }

            if (failure is not null)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
            }
        }

        private void CloseAllPreservingThrow()
        {
            for (var i = _open.Length - 1; i >= 0; i--)
            {
                if (_open[i])
                {
                    _open[i] = false;
                    _iterators[i].ClosePreservingThrow();
                }
            }
        }
    }
}
