using System;
using System.Collections.Generic;

namespace JavaScriptRuntime;

public static class Iterator
{
    /// <summary>Realm-owned <c>%IteratorPrototype%</c> (issue #1824).</summary>
    internal static object Prototype
        => GetPrototype(RuntimeIntrinsics.Current);

    private static object GetPrototype(RuntimeIntrinsics intrinsics)
        => intrinsics.GetOrCreate(
            RuntimeIntrinsicSlot.IteratorPrototype,
            static () => new JsObject());

    /// <summary>Realm-owned prototype for runtime-native iterator adapters.</summary>
    internal static object AdapterPrototype
        => GetAdapterPrototype(RuntimeIntrinsics.Current);

    private static object GetAdapterPrototype(RuntimeIntrinsics intrinsics)
        => intrinsics.GetOrCreate(
            RuntimeIntrinsicSlot.IteratorAdapterPrototype,
            static () => new JsObject(),
            prototype =>
            {
                PrototypeChain.SetPrototype(prototype, GetPrototype(intrinsics));
                DefineFunctionProperty(prototype, "next", (BuiltinFunction0)PrototypeNext, 0d);
                DefineFunctionProperty(prototype, "return", (BuiltinFunction0)PrototypeReturn, 0d);
            });

    /// <summary>Realm-owned <c>%WrapForValidIteratorPrototype%</c>.</summary>
    internal static object WrapperPrototype
        => GetWrapperPrototype(RuntimeIntrinsics.Current);

    private static object GetWrapperPrototype(RuntimeIntrinsics intrinsics)
        => intrinsics.GetOrCreate(
            RuntimeIntrinsicSlot.IteratorWrapperPrototype,
            static () => new JsObject(),
            prototype =>
            {
                PrototypeChain.SetPrototype(prototype, GetPrototype(intrinsics));
                DefineFunctionProperty(prototype, "next", (BuiltinFunction0)WrapperPrototypeNext, 0d);
                DefineFunctionProperty(prototype, "return", (BuiltinFunction0)WrapperPrototypeReturn, 0d);
            });

    /// <summary>Realm-owned <c>%IteratorHelperPrototype%</c> (issue #1824).</summary>
    internal static object HelperPrototype
        => GetHelperPrototype(RuntimeIntrinsics.Current);

    private static object GetHelperPrototype(RuntimeIntrinsics intrinsics)
        => intrinsics.GetOrCreate(
            RuntimeIntrinsicSlot.IteratorHelperPrototype,
            static () => new JsObject(),
            prototype => PrototypeChain.SetPrototype(prototype, GetPrototype(intrinsics)));

    internal static void ConfigureIntrinsicSurface(object iteratorConstructorValue)
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
        var intrinsics = RuntimeIntrinsics.Current;
        var iteratorPrototype = GetPrototype(intrinsics);
        var helperPrototype = GetHelperPrototype(intrinsics);
        GetWrapperPrototype(intrinsics);

        Function.InitializeFunctionInstance(iteratorConstructorValue, 0d, "Iterator");
        Function.MarkConstructible(iteratorConstructorValue);
        DefineDataProperty(iteratorConstructorValue, "prototype", iteratorPrototype);
        DefineFunctionProperty(
            iteratorConstructorValue,
            "from",
            (BuiltinFunction1)ConstructorFrom,
            1d,
            requiresInvocationContext: true);

        DefineImmutablePrototypeAccessor(iteratorPrototype, "constructor", iteratorConstructorValue);
        DefineFunctionProperty(iteratorPrototype, "drop", (BuiltinFunctionVariadic)PrototypeDrop, 1d);
        DefineFunctionProperty(iteratorPrototype, "every", (BuiltinFunction1)PrototypeEvery, 1d);
        DefineFunctionProperty(iteratorPrototype, "filter", (BuiltinFunction1)PrototypeFilter, 1d);
        DefineFunctionProperty(iteratorPrototype, "find", (BuiltinFunction1)PrototypeFind, 1d);
        DefineFunctionProperty(iteratorPrototype, "flatMap", (BuiltinFunction1)PrototypeFlatMap, 1d);
        DefineFunctionProperty(iteratorPrototype, "forEach", (BuiltinFunction1)PrototypeForEach, 1d);
        DefineFunctionProperty(iteratorPrototype, "map", (BuiltinFunction1)PrototypeMap, 1d);
        DefineFunctionProperty(iteratorPrototype, "reduce", (BuiltinFunctionVariadic)PrototypeReduce, 1d);
        DefineFunctionProperty(iteratorPrototype, "some", (BuiltinFunction1)PrototypeSome, 1d);
        DefineFunctionProperty(iteratorPrototype, "take", (BuiltinFunctionVariadic)PrototypeTake, 1d);
        DefineFunctionProperty(iteratorPrototype, "toArray", (BuiltinFunction0)PrototypeToArray, 0d);
        DefineFunctionProperty(iteratorPrototype, Symbol.iterator.DebugId, (BuiltinFunction0)PrototypeSymbolIterator, 0d, "[Symbol.iterator]");
        DefineFunctionProperty(iteratorPrototype, Symbol.dispose.DebugId, (BuiltinFunction0)PrototypeSymbolDispose, 0d, "[Symbol.dispose]");
        DefineImmutablePrototypeAccessor(iteratorPrototype, Symbol.toStringTag.DebugId, "Iterator", "[Symbol.toStringTag]");

        DefineFunctionProperty(helperPrototype, "next", (BuiltinFunction0)HelperPrototypeNext, 0d);
        DefineFunctionProperty(helperPrototype, "return", (BuiltinFunction0)HelperPrototypeReturn, 0d);
        DefineFunctionProperty(helperPrototype, Symbol.iterator.DebugId, (BuiltinFunction0)PrototypeSymbolIterator, 0d, "[Symbol.iterator]");
        DefineDataProperty(helperPrototype, Symbol.toStringTag.DebugId, "Iterator Helper");
    }

    internal static object GetIntrinsicPrototypeForConstructor(object? constructor)
        => GetPrototype(RuntimeIntrinsics.GetFunctionRealm(constructor));

    internal static void InitializeIteratorSurface(object iterator)
    {
        if (PrototypeChain.GetPrototypeOrNull(iterator) == null)
        {
            if (iterator is JsObject jsObject)
            {
                PrototypeChain.InitializePrototype(jsObject, AdapterPrototype);
            }
            else
            {
                PrototypeChain.SetPrototype(iterator, AdapterPrototype);
            }
        }
    }

    internal static void InitializeHelperSurface(object iterator)
    {
        if (iterator is JsObject jsObject)
        {
            PrototypeChain.InitializePrototype(jsObject, HelperPrototype);
        }
        else
        {
            PrototypeChain.SetPrototype(iterator, HelperPrototype);
        }
    }

    public static IJavaScriptIterator From(object? value)
    {
        var intrinsics = RuntimeIntrinsics.Current;
        var record = GetIteratorFlattenable(value, allowStrings: true, "from");
        if (record.IteratorObject is IJavaScriptIterator iterator
            && InheritsFromIteratorPrototype(
                record.IteratorObject,
                GetPrototype(intrinsics)))
        {
            return iterator;
        }

        return new IteratorLikeWrapper(
            record,
            GetWrapperPrototype(intrinsics));
    }

    private static void DefineDataProperty(object target, string key, object? value)
    {
        PropertyDescriptorStore.DefineOrUpdate(target, key, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = true,
            Value = value
        });
    }

    /// <summary>
    /// Defines a data property whose value is a builtin delegate, giving the resulting
    /// function object the correct spec-mandated <c>length</c>/<c>name</c> metadata.
    /// Without this, <see cref="Function.InitializeFunctionInstance(object)"/> derives
    /// <c>name</c> from the underlying CLR method (e.g. "PrototypeEvery" instead of "every").
    /// </summary>
    private static void DefineFunctionProperty(
        object target,
        string key,
        Delegate method,
        double length,
        string? name = null,
        bool? requiresInvocationContext = null)
    {
        Function.InitializeFunctionInstance(
            method,
            length,
            name ?? key,
            requiresInvocationContext:
                requiresInvocationContext
                ?? !BuiltinFunctionDelegates.IsReceiverAware(method));
        Function.MarkUndefinedPrototype(method);
        DefineDataProperty(target, key, method);
    }

    private static void DefineImmutablePrototypeAccessor(
        object home,
        string key,
        object? value,
        string? displayName = null)
    {
        var propertyName = displayName ?? key;
        BuiltinFunction0 getter = _ => value;
        BuiltinFunction1 setter = (thisArgument, newValue) =>
        {
            if (!Proxy.IsObjectLikeValue(thisArgument))
            {
                throw new TypeError($"Cannot set {propertyName} on a non-object");
            }

            if (ReferenceEquals(thisArgument, home))
            {
                throw new TypeError($"Cannot assign to read only property '{propertyName}'");
            }

            if (!ObjectRuntime.hasOwn(thisArgument!, key))
            {
                if (!ObjectRuntime.CreateDataProperty(thisArgument!, key, newValue))
                {
                    throw new TypeError($"Cannot create property '{propertyName}'");
                }

                return null;
            }

            _ = ObjectRuntime.SetProperty(thisArgument!, key, newValue, throwOnError: true);
            return null;
        };

        Function.InitializeFunctionInstance(getter, 0d, $"get {propertyName}", requiresInvocationContext: false);
        Function.InitializeFunctionInstance(setter, 1d, $"set {propertyName}", requiresInvocationContext: false);
        Function.MarkUndefinedPrototype(getter);
        Function.MarkUndefinedPrototype(setter);
        PropertyDescriptorStore.DefineOrUpdate(home, key, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Accessor,
            Enumerable = false,
            Configurable = true,
            Get = getter,
            Set = setter
        });
    }

    private static object? ConstructorFrom(object? thisArgument, object? value)
    {
        var intrinsics =
            RuntimeIntrinsics.GetFunctionRealm(
                RuntimeServices.GetCurrentCallee());
        var record = GetIteratorFlattenable(value, allowStrings: true, "from");
        return InheritsFromIteratorPrototype(
                record.IteratorObject,
                GetPrototype(intrinsics))
            ? record.IteratorObject
            : new IteratorLikeWrapper(
                record,
                GetWrapperPrototype(intrinsics));
    }

    /// <summary>
    /// Generic <c>next()</c> implementation shared by every concrete iterator kind's
    /// own prototype (e.g. <see cref="Array.IteratorPrototype"/>) and runtime iterator wrappers.
    /// </summary>
    internal static object? PrototypeNext(object? thisArgument)
    {
        if (thisArgument is IJavaScriptIterator iterator)
        {
            return iterator.Next();
        }

        throw new TypeError("Iterator prototype next called on incompatible receiver");
    }

    private static object? PrototypeReturn(object? thisArgument)
    {
        if (thisArgument is not IJavaScriptIterator iterator)
        {
            throw new TypeError("Iterator Helper.prototype.return called on incompatible receiver");
        }

        if (iterator.HasReturn)
        {
            iterator.Return();
        }

        return IteratorResult.Create(null, done: true);
    }

    private static object? WrapperPrototypeNext(object? thisArgument)
    {
        if (thisArgument is not IteratorLikeWrapper wrapper)
        {
            throw new TypeError("Iterator wrapper next called on incompatible receiver");
        }

        return wrapper.NextRaw();
    }

    private static object? WrapperPrototypeReturn(object? thisArgument)
    {
        if (thisArgument is not IteratorLikeWrapper wrapper)
        {
            throw new TypeError("Iterator wrapper return called on incompatible receiver");
        }

        return wrapper.ReturnRaw();
    }

    private static object? HelperPrototypeNext(object? thisArgument)
    {
        if (thisArgument is not IteratorHelperBase helper)
        {
            throw new TypeError("Iterator helper next called on incompatible receiver");
        }

        return helper.Next();
    }

    private static object? HelperPrototypeReturn(object? thisArgument)
    {
        if (thisArgument is not IteratorHelperBase helper)
        {
            throw new TypeError("Iterator helper return called on incompatible receiver");
        }

        helper.Return();
        return IteratorResult.Create(null, done: true);
    }

    private static object? PrototypeDrop(object? thisArgument, in JsCallArguments arguments)
    {
        var receiver = RequireObjectReceiver(thisArgument, "drop");
        double limit;
        try
        {
            limit = GetNonNegativeInteger(arguments, "drop");
        }
        catch
        {
            CloseIteratorPreservingThrow(receiver);
            throw;
        }

        return new DropIteratorHelper(GetIteratorDirect(receiver), limit);
    }

    private static object? PrototypeEvery(object? thisArgument, object? predicateArgument)
    {
        var receiver = RequireObjectReceiver(thisArgument, "every");
        var predicate = GetRequiredCallbackOrClose(receiver, predicateArgument, "every");
        var iterator = GetIteratorDirect(receiver);
        long index = 0;

        while (true)
        {
            var step = iterator.Next();
            if (step.Done)
            {
                return true;
            }

            object? result;
            try
            {
                result = InvokeCallback(predicate, step.Value, (double)index);
                index++;
            }
            catch
            {
                iterator.ClosePreservingThrow();
                throw;
            }

            if (!Operators.IsTruthy(result))
            {
                iterator.Close();
                return false;
            }
        }
    }

    private static object? PrototypeFilter(object? thisArgument, object? predicateArgument)
    {
        var receiver = RequireObjectReceiver(thisArgument, "filter");
        var predicate = GetRequiredCallbackOrClose(receiver, predicateArgument, "filter");
        return new FilterIteratorHelper(GetIteratorDirect(receiver), predicate);
    }

    private static object? PrototypeFind(object? thisArgument, object? predicateArgument)
    {
        var receiver = RequireObjectReceiver(thisArgument, "find");
        var predicate = GetRequiredCallbackOrClose(receiver, predicateArgument, "find");
        var iterator = GetIteratorDirect(receiver);
        long index = 0;

        while (true)
        {
            var step = iterator.Next();
            if (step.Done)
            {
                return null;
            }

            object? result;
            try
            {
                result = InvokeCallback(predicate, step.Value, (double)index);
                index++;
            }
            catch
            {
                iterator.ClosePreservingThrow();
                throw;
            }

            if (Operators.IsTruthy(result))
            {
                iterator.Close();
                return step.Value;
            }
        }
    }

    private static object? PrototypeFlatMap(object? thisArgument, object? mapperArgument)
    {
        var receiver = RequireObjectReceiver(thisArgument, "flatMap");
        var mapper = GetRequiredCallbackOrClose(receiver, mapperArgument, "flatMap");
        return new FlatMapIteratorHelper(GetIteratorDirect(receiver), mapper);
    }

    private static object? PrototypeForEach(object? thisArgument, object? procedureArgument)
    {
        var receiver = RequireObjectReceiver(thisArgument, "forEach");
        var procedure = GetRequiredCallbackOrClose(receiver, procedureArgument, "forEach");
        var iterator = GetIteratorDirect(receiver);
        long index = 0;

        while (true)
        {
            var step = iterator.Next();
            if (step.Done)
            {
                return null;
            }

            try
            {
                _ = InvokeCallback(procedure, step.Value, (double)index);
                index++;
            }
            catch
            {
                iterator.ClosePreservingThrow();
                throw;
            }
        }
    }

    private static object? PrototypeMap(object? thisArgument, object? mapperArgument)
    {
        var receiver = RequireObjectReceiver(thisArgument, "map");
        var mapper = GetRequiredCallbackOrClose(receiver, mapperArgument, "map");
        return new MapIteratorHelper(GetIteratorDirect(receiver), mapper);
    }

    private static object? PrototypeReduce(object? thisArgument, in JsCallArguments arguments)
    {
        var receiver = RequireObjectReceiver(thisArgument, "reduce");
        var reducer = GetRequiredCallbackOrClose(receiver, arguments.GetArgument(0), "reduce");
        var iterator = GetIteratorDirect(receiver);
        bool hasInitialValue = arguments.Count > 1;
        object? accumulator = null;
        long index = 0;

        if (hasInitialValue)
        {
            accumulator = arguments.GetArgument(1);
        }
        else
        {
            var first = iterator.Next();
            if (first.Done)
            {
                throw new TypeError("Reduce of empty iterator with no initial value");
            }

            accumulator = first.Value;
            index = 1;
        }

        while (true)
        {
            var step = iterator.Next();
            if (step.Done)
            {
                return accumulator;
            }

            try
            {
                accumulator = InvokeCallback(reducer, accumulator, step.Value, (double)index);
                index++;
            }
            catch
            {
                iterator.ClosePreservingThrow();
                throw;
            }
        }
    }

    private static object? PrototypeSome(object? thisArgument, object? predicateArgument)
    {
        var receiver = RequireObjectReceiver(thisArgument, "some");
        var predicate = GetRequiredCallbackOrClose(receiver, predicateArgument, "some");
        var iterator = GetIteratorDirect(receiver);
        long index = 0;

        while (true)
        {
            var step = iterator.Next();
            if (step.Done)
            {
                return false;
            }

            object? result;
            try
            {
                result = InvokeCallback(predicate, step.Value, (double)index);
                index++;
            }
            catch
            {
                iterator.ClosePreservingThrow();
                throw;
            }

            if (Operators.IsTruthy(result))
            {
                iterator.Close();
                return true;
            }
        }
    }

    private static object? PrototypeTake(object? thisArgument, in JsCallArguments arguments)
    {
        var receiver = RequireObjectReceiver(thisArgument, "take");
        double limit;
        try
        {
            limit = GetNonNegativeInteger(arguments, "take");
        }
        catch
        {
            CloseIteratorPreservingThrow(receiver);
            throw;
        }

        return new TakeIteratorHelper(GetIteratorDirect(receiver), limit);
    }

    private static object? PrototypeToArray(object? thisArgument)
    {
        var receiver = RequireObjectReceiver(thisArgument, "toArray");
        var iterator = GetIteratorDirect(receiver);
        var result = new JavaScriptRuntime.Array();

        while (true)
        {
            var step = iterator.Next();
            if (step.Done)
            {
                return result;
            }

            result.Add(step.Value);
        }
    }

    private static object? PrototypeSymbolIterator(object? thisArgument)
    {
        return thisArgument;
    }

    private static object? PrototypeSymbolDispose(object? thisArgument)
    {
        if (thisArgument is null or JsNull)
        {
            throw new TypeError("Iterator.prototype[Symbol.dispose] called on null or undefined");
        }

        var receiver = thisArgument;
        var returnMethod = ObjectRuntime.GetProperty(receiver, "return");
        if (returnMethod is null or JsNull)
        {
            return null;
        }

        if (!CallableOperations.IsCallable(returnMethod))
        {
            throw new TypeError("Iterator return method is not callable");
        }

        _ = CallableOperations.Call(returnMethod, receiver, System.Array.Empty<object?>());
        return null;
    }

    private static object RequireObjectReceiver(object? thisValue, string methodName)
    {
        if (Proxy.IsObjectLikeValue(thisValue))
        {
            return thisValue!;
        }

        throw new TypeError($"Iterator.prototype.{methodName} called on incompatible receiver");
    }

    private static IteratorRecord GetIteratorDirect(object receiver)
        => new(receiver, ObjectRuntime.GetProperty(receiver, "next"));

    private static object GetRequiredCallback(object? callback, string methodName)
    {
        if (CallableOperations.IsCallable(callback))
        {
            return callback!;
        }

        throw new TypeError($"Iterator.prototype.{methodName} requires a callback function");
    }

    private static object GetRequiredCallbackOrClose(object receiver, object? callback, string methodName)
    {
        try
        {
            return GetRequiredCallback(callback, methodName);
        }
        catch
        {
            CloseIteratorPreservingThrow(receiver);
            throw;
        }
    }

    private static double GetNonNegativeInteger(in JsCallArguments arguments, string methodName)
    {
        var value = TypeUtilities.ToNumber(arguments.GetArgument(0));
        if (double.IsNaN(value) || double.IsNegativeInfinity(value))
        {
            throw new RangeError($"Iterator.prototype.{methodName} requires a non-negative finite-or-positive-infinity limit");
        }

        if (double.IsPositiveInfinity(value))
        {
            return double.PositiveInfinity;
        }

        var integer = System.Math.Truncate(value);
        if (integer < 0)
        {
            throw new RangeError($"Iterator.prototype.{methodName} requires a non-negative finite-or-positive-infinity limit");
        }

        return integer;
    }

    private static object? InvokeCallback(
        object callback,
        object? argument0,
        object? argument1)
        => CallableOperations.Call2(callback, null, argument0, argument1);

    private static object? InvokeCallback(
        object callback,
        object? argument0,
        object? argument1,
        object? argument2)
        => CallableOperations.Call3(callback, null, argument0, argument1, argument2);

    private static void CloseIteratorPreservingThrow(object iterator)
    {
        try
        {
            new IteratorRecord(iterator, nextMethod: null).Close();
        }
        catch
        {
        }
    }

    private static IteratorRecord GetIteratorFlattenable(
        object? value,
        bool allowStrings,
        string methodName = "flatMap")
    {
        var isObject = Proxy.IsObjectLikeValue(value);
        if (!isObject && !(allowStrings && value is string))
        {
            throw new TypeError($"Iterator.prototype.{methodName} requires an object");
        }

        var iteratorObject = value;
        if (isObject
            && iteratorObject is IJavaScriptIterator
            && PrototypeChain.GetPrototypeOrNull(iteratorObject!) == null)
        {
            InitializeIteratorSurface(iteratorObject!);
        }

        var iteratorMethod = ObjectRuntime.GetItem(iteratorObject!, Symbol.iterator);
        if (iteratorMethod is not null and not JsNull)
        {
            if (!CallableOperations.IsCallable(iteratorMethod))
            {
                throw new TypeError("Symbol.iterator is not callable");
            }

            iteratorObject = CallableOperations.Call(
                iteratorMethod,
                iteratorObject,
                System.Array.Empty<object?>());
            if (!Proxy.IsObjectLikeValue(iteratorObject))
            {
                throw new TypeError("Iterator method did not return an object");
            }
        }
        else if (!isObject)
        {
            throw new TypeError("Iterator method is missing");
        }

        return GetIteratorDirect(iteratorObject!);
    }

    private static bool InheritsFromIteratorPrototype(
        object value,
        object iteratorPrototype)
    {
        var current = value;
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
        while (PrototypeChain.TryGetPrototype(current, out var prototype)
               && prototype is not null and not JsNull)
        {
            if (!visited.Add(prototype))
            {
                return false;
            }

            if (ReferenceEquals(prototype, iteratorPrototype))
            {
                return true;
            }

            current = prototype;
        }

        return false;
    }

    private readonly record struct IteratorStep(object? Value, bool Done);

    private sealed class IteratorRecord
    {
        private readonly object _iterator;
        private readonly object? _nextMethod;

        public IteratorRecord(object iterator, object? nextMethod)
        {
            _iterator = iterator;
            _nextMethod = nextMethod;
        }

        public object IteratorObject => _iterator;

        public object? NextRaw()
        {
            if (!CallableOperations.IsCallable(_nextMethod))
            {
                throw new TypeError("Iterator next method is not callable");
            }

            return CallableOperations.Call(
                _nextMethod!,
                _iterator,
                System.Array.Empty<object?>());
        }

        public IteratorStep Next()
        {
            var result = NextRaw();
            if (!Proxy.IsObjectLikeValue(result))
            {
                throw new TypeError("Iterator next method must return an object");
            }

            var done = TypeUtilities.ToBoolean(ObjectRuntime.GetProperty(result!, "done"));
            if (done)
            {
                return new IteratorStep(null, true);
            }

            return new IteratorStep(ObjectRuntime.GetProperty(result!, "value"), false);
        }

        public object? ReturnForWrapper()
        {
            var returnMethod = ObjectRuntime.GetProperty(_iterator, "return");
            if (returnMethod is null or JsNull)
            {
                return IteratorResult.Create(null, done: true);
            }

            if (!CallableOperations.IsCallable(returnMethod))
            {
                throw new TypeError("Iterator return method is not callable");
            }

            return CallableOperations.Call(
                returnMethod,
                _iterator,
                System.Array.Empty<object?>());
        }

        public void Close()
        {
            var result = ReturnForWrapper();
            if (!Proxy.IsObjectLikeValue(result))
            {
                throw new TypeError("Iterator return method must return an object");
            }
        }

        public void ClosePreservingThrow()
        {
            try
            {
                Close();
            }
            catch
            {
            }
        }
    }

    private abstract class IteratorHelperBase : JsObject, IJavaScriptIterator
    {
        private bool _closed;
        private bool _executing;

        protected IteratorHelperBase(IteratorRecord source)
        {
            Source = source;
            InitializeHelperSurface(this);
        }

        protected IteratorRecord Source { get; }

        protected bool Done { get; set; }

        public bool HasReturn => true;

        public IteratorResultObject Next()
        {
            if (_executing)
            {
                throw new TypeError("Iterator helper is already running");
            }

            if (Done)
            {
                return IteratorResult.Create(null, true);
            }

            _executing = true;
            try
            {
                return NextCore();
            }
            catch
            {
                CompleteAbruptly();
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

            if (Done)
            {
                return;
            }

            Done = true;
            CloseEarly();
        }

        protected abstract IteratorResultObject NextCore();

        protected IteratorResultObject Finish(object? value = null)
        {
            Done = true;
            _closed = true;
            return IteratorResult.Create(value, true);
        }

        protected IteratorResultObject FinishAndClose(object? value = null)
        {
            Done = true;
            CloseEarly();
            return IteratorResult.Create(value, true);
        }

        protected virtual void CompleteAbruptly()
        {
            Done = true;
            _closed = true;
        }

        protected virtual void CloseEarly()
        {
            CloseSource();
        }

        protected void CloseSource()
        {
            if (_closed)
            {
                return;
            }

            _closed = true;
            OnClose();
            Source.Close();
        }

        protected void CloseSourcePreservingThrow()
        {
            if (_closed)
            {
                return;
            }

            _closed = true;
            Source.ClosePreservingThrow();
        }

        protected virtual void OnClose()
        {
        }
    }

    private sealed class MapIteratorHelper : IteratorHelperBase
    {
        private readonly object _mapper;
        private long _index;

        public MapIteratorHelper(IteratorRecord source, object mapper)
            : base(source)
        {
            _mapper = mapper;
        }

        protected override IteratorResultObject NextCore()
        {
            var step = Source.Next();
            if (step.Done)
            {
                return Finish();
            }

            object? mapped;
            try
            {
                mapped = InvokeCallback(_mapper, step.Value, (double)_index);
            }
            catch
            {
                CloseSourcePreservingThrow();
                throw;
            }

            _index++;
            return IteratorResult.Create(mapped, false);
        }
    }

    private sealed class FilterIteratorHelper : IteratorHelperBase
    {
        private readonly object _predicate;
        private long _index;

        public FilterIteratorHelper(IteratorRecord source, object predicate)
            : base(source)
        {
            _predicate = predicate;
        }

        protected override IteratorResultObject NextCore()
        {
            while (true)
            {
                var step = Source.Next();
                if (step.Done)
                {
                    return Finish();
                }

                object? selected;
                try
                {
                    selected = InvokeCallback(_predicate, step.Value, (double)_index);
                }
                catch
                {
                    CloseSourcePreservingThrow();
                    throw;
                }

                _index++;
                if (Operators.IsTruthy(selected))
                {
                    return IteratorResult.Create(step.Value, false);
                }
            }
        }
    }

    private sealed class DropIteratorHelper : IteratorHelperBase
    {
        private readonly double _limit;
        private double _dropped;

        public DropIteratorHelper(IteratorRecord source, double limit)
            : base(source)
        {
            _limit = limit;
        }

        protected override IteratorResultObject NextCore()
        {
            while (_dropped < _limit)
            {
                var skipped = Source.Next();
                if (skipped.Done)
                {
                    return Finish();
                }

                _dropped++;
            }

            var step = Source.Next();
            return step.Done
                ? Finish()
                : IteratorResult.Create(step.Value, false);
        }
    }

    private sealed class TakeIteratorHelper : IteratorHelperBase
    {
        private readonly double _limit;
        private double _taken;

        public TakeIteratorHelper(IteratorRecord source, double limit)
            : base(source)
        {
            _limit = limit;
        }

        protected override IteratorResultObject NextCore()
        {
            if (_taken >= _limit)
            {
                return FinishAndClose();
            }

            var step = Source.Next();
            if (step.Done)
            {
                return Finish();
            }

            _taken++;
            return IteratorResult.Create(step.Value, false);
        }
    }

    private sealed class FlatMapIteratorHelper : IteratorHelperBase
    {
        private readonly object _mapper;
        private long _index;
        private IteratorRecord? _inner;

        public FlatMapIteratorHelper(IteratorRecord source, object mapper)
            : base(source)
        {
            _mapper = mapper;
        }

        protected override IteratorResultObject NextCore()
        {
            while (true)
            {
                if (_inner != null)
                {
                    IteratorStep innerStep;
                    try
                    {
                        innerStep = _inner.Next();
                    }
                    catch
                    {
                        ReleaseInner();
                        CloseSourcePreservingThrow();
                        throw;
                    }

                    if (!innerStep.Done)
                    {
                        return IteratorResult.Create(innerStep.Value, false);
                    }

                    ReleaseInner();
                }

                var step = Source.Next();
                if (step.Done)
                {
                    return Finish();
                }

                try
                {
                    var mapped = InvokeCallback(_mapper, step.Value, (double)_index);
                    _index++;
                    _inner = GetIteratorFlattenable(mapped, allowStrings: false);
                }
                catch
                {
                    CloseSourcePreservingThrow();
                    throw;
                }
            }
        }

        protected override void OnClose()
        {
            CloseInner();
        }

        protected override void CompleteAbruptly()
        {
            ReleaseInner();
            base.CompleteAbruptly();
        }

        protected override void CloseEarly()
        {
            try
            {
                CloseInner();
            }
            catch
            {
                ReleaseInner();
                try
                {
                    base.CloseEarly();
                }
                catch
                {
                }

                throw;
            }

            base.CloseEarly();
        }

        private void CloseInner()
        {
            if (_inner == null)
            {
                return;
            }

            _inner.Close();
            _inner = null;
        }

        private void ReleaseInner()
        {
            _inner = null;
        }
    }

    private sealed class IteratorLikeWrapper : IJavaScriptIterator
    {
        private readonly IteratorRecord _record;

        public IteratorLikeWrapper(
            IteratorRecord record,
            object wrapperPrototype)
        {
            _record = record;
            PrototypeChain.SetPrototype(this, wrapperPrototype);
        }

        public bool HasReturn => true;

        public IteratorResultObject Next()
        {
            var step = _record.Next();
            return IteratorResult.Create(step.Value, step.Done);
        }

        public object? NextRaw()
            => _record.NextRaw();

        public void Return()
        {
            _record.Close();
        }

        public object? ReturnRaw()
            => _record.ReturnForWrapper();
    }
}
