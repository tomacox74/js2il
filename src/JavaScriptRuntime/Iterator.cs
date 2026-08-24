using System;
using System.Linq;
using System.Reflection;

namespace JavaScriptRuntime;

public static class Iterator
{
    /// <summary>Realm-owned <c>%IteratorPrototype%</c> (issue #1824).</summary>
    internal static object Prototype
        => RuntimeIntrinsics.Current.GetOrCreate(
            RuntimeIntrinsicSlot.IteratorPrototype,
            static () => new JsObject());

    /// <summary>Realm-owned <c>%IteratorHelperPrototype%</c> (issue #1824).</summary>
    internal static object HelperPrototype
        => RuntimeIntrinsics.Current.GetOrCreate(
            RuntimeIntrinsicSlot.IteratorHelperPrototype,
            static () => new JsObject(),
            static prototype => PrototypeChain.SetPrototype(prototype, Prototype));

    internal static void ConfigureIntrinsicSurface(object iteratorConstructorValue)
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        Function.InitializeFunctionInstance(iteratorConstructorValue, 0d, "Iterator");
        DefineDataProperty(iteratorConstructorValue, "prototype", Prototype);
        DefineFunctionProperty(iteratorConstructorValue, "from", (BuiltinFunction1)ConstructorFrom, 1d);

        DefineDataProperty(Prototype, "constructor", iteratorConstructorValue);
        DefineFunctionProperty(Prototype, "next", (BuiltinFunction0)PrototypeNext, 0d);
        DefineFunctionProperty(Prototype, "return", (BuiltinFunction0)PrototypeReturn, 0d);
        DefineFunctionProperty(Prototype, "drop", (BuiltinFunctionVariadic)PrototypeDrop, 1d);
        DefineFunctionProperty(Prototype, "every", (BuiltinFunction1)PrototypeEvery, 1d);
        DefineFunctionProperty(Prototype, "filter", (BuiltinFunction1)PrototypeFilter, 1d);
        DefineFunctionProperty(Prototype, "find", (BuiltinFunction1)PrototypeFind, 1d);
        DefineFunctionProperty(Prototype, "flatMap", (BuiltinFunction1)PrototypeFlatMap, 1d);
        DefineFunctionProperty(Prototype, "forEach", (BuiltinFunction1)PrototypeForEach, 1d);
        DefineFunctionProperty(Prototype, "map", (BuiltinFunction1)PrototypeMap, 1d);
        DefineFunctionProperty(Prototype, "reduce", (BuiltinFunctionVariadic)PrototypeReduce, 1d);
        DefineFunctionProperty(Prototype, "some", (BuiltinFunction1)PrototypeSome, 1d);
        DefineFunctionProperty(Prototype, "take", (BuiltinFunctionVariadic)PrototypeTake, 1d);
        DefineFunctionProperty(Prototype, "toArray", (BuiltinFunction0)PrototypeToArray, 0d);
        DefineFunctionProperty(Prototype, Symbol.iterator.DebugId, (BuiltinFunction0)PrototypeSymbolIterator, 0d, "[Symbol.iterator]");
        DefineDataProperty(Prototype, Symbol.toStringTag.DebugId, "Iterator");

        DefineFunctionProperty(HelperPrototype, "next", (BuiltinFunction0)PrototypeNext, 0d);
        DefineFunctionProperty(HelperPrototype, "return", (BuiltinFunction0)PrototypeReturn, 0d);
        DefineFunctionProperty(HelperPrototype, Symbol.iterator.DebugId, (BuiltinFunction0)PrototypeSymbolIterator, 0d, "[Symbol.iterator]");
        DefineDataProperty(HelperPrototype, Symbol.toStringTag.DebugId, "Iterator Helper");
    }

    internal static void InitializeIteratorSurface(object iterator)
    {
        if (PrototypeChain.GetPrototypeOrNull(iterator) == null)
        {
            if (iterator is JsObject jsObject)
            {
                PrototypeChain.InitializePrototype(jsObject, Prototype);
            }
            else
            {
                PrototypeChain.SetPrototype(iterator, Prototype);
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
        if (value is IJavaScriptIterator iterator)
        {
            InitializeIteratorSurface(iterator);
            return iterator;
        }

        if (TryCreateIteratorLikeWrapper(value, out var iteratorLike))
        {
            return iteratorLike;
        }

        var wrapped = ObjectRuntime.GetIterator(value);
        InitializeIteratorSurface(wrapped);
        return wrapped;
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
        string? name = null)
    {
        Function.InitializeFunctionInstance(
            method,
            length,
            name ?? key,
            requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(method));
        Function.MarkUndefinedPrototype(method);
        DefineDataProperty(target, key, method);
    }

    private static object? ConstructorFrom(object? thisArgument, object? value)
    {
        return From(value);
    }

    /// <summary>
    /// Generic <c>next()</c> implementation shared by every concrete iterator kind's
    /// own prototype (e.g. <see cref="Array.IteratorPrototype"/>), in addition to
    /// <see cref="Prototype"/> itself.
    /// </summary>
    internal static object? PrototypeNext(object? thisArgument)
    {
        return GetReceiverIterator(thisArgument, "next").Next();
    }

    private static object? PrototypeReturn(object? thisArgument)
    {
        var iterator = GetReceiverIterator(thisArgument, "return");
        if (iterator.HasReturn)
        {
            iterator.Return();
        }

        return IteratorResult.Create(null, done: true);
    }

    private static object? PrototypeDrop(object? thisArgument, in JsCallArguments arguments)
    {
        return new DropIteratorHelper(GetReceiverIterator(thisArgument, "drop"), GetNonNegativeInteger(arguments, "drop"));
    }

    private static object? PrototypeEvery(object? thisArgument, object? predicateArgument)
    {
        var iterator = GetReceiverIterator(thisArgument, "every");
        var predicate = GetRequiredCallback(predicateArgument, "every");
        long index = 0;

        try
        {
            while (true)
            {
                var step = iterator.Next();
                if (step.done)
                {
                    return true;
                }

                if (!Operators.IsTruthy(InvokeCallback(predicate, step.value, (double)index++)))
                {
                    CloseIterator(iterator);
                    return false;
                }
            }
        }
        catch
        {
            CloseIterator(iterator);
            throw;
        }
    }

    private static object? PrototypeFilter(object? thisArgument, object? predicateArgument)
    {
        return new FilterIteratorHelper(GetReceiverIterator(thisArgument, "filter"), GetRequiredCallback(predicateArgument, "filter"));
    }

    private static object? PrototypeFind(object? thisArgument, object? predicateArgument)
    {
        var iterator = GetReceiverIterator(thisArgument, "find");
        var predicate = GetRequiredCallback(predicateArgument, "find");
        long index = 0;

        try
        {
            while (true)
            {
                var step = iterator.Next();
                if (step.done)
                {
                    return null;
                }

                if (Operators.IsTruthy(InvokeCallback(predicate, step.value, (double)index++)))
                {
                    CloseIterator(iterator);
                    return step.value;
                }
            }
        }
        catch
        {
            CloseIterator(iterator);
            throw;
        }
    }

    private static object? PrototypeFlatMap(object? thisArgument, object? mapperArgument)
    {
        return new FlatMapIteratorHelper(GetReceiverIterator(thisArgument, "flatMap"), GetRequiredCallback(mapperArgument, "flatMap"));
    }

    private static object? PrototypeForEach(object? thisArgument, object? procedureArgument)
    {
        var iterator = GetReceiverIterator(thisArgument, "forEach");
        var procedure = GetRequiredCallback(procedureArgument, "forEach");
        long index = 0;

        try
        {
            while (true)
            {
                var step = iterator.Next();
                if (step.done)
                {
                    return null;
                }

                _ = InvokeCallback(procedure, step.value, (double)index++);
            }
        }
        catch
        {
            CloseIterator(iterator);
            throw;
        }
    }

    private static object? PrototypeMap(object? thisArgument, object? mapperArgument)
    {
        return new MapIteratorHelper(GetReceiverIterator(thisArgument, "map"), GetRequiredCallback(mapperArgument, "map"));
    }

    private static object? PrototypeReduce(object? thisArgument, in JsCallArguments arguments)
    {
        var iterator = GetReceiverIterator(thisArgument, "reduce");
        var reducer = GetRequiredCallback(arguments.GetArgument(0), "reduce");
        bool hasInitialValue = arguments.Count > 1;
        object? accumulator = null;
        long index = 0;

        try
        {
            if (hasInitialValue)
            {
                accumulator = arguments.GetArgument(1);
            }
            else
            {
                var first = iterator.Next();
                if (first.done)
                {
                    throw new TypeError("Reduce of empty iterator with no initial value");
                }

                accumulator = first.value;
                index = 1;
            }

            while (true)
            {
                var step = iterator.Next();
                if (step.done)
                {
                    return accumulator;
                }

                accumulator = InvokeCallback(reducer, accumulator, step.value, (double)index++);
            }
        }
        catch
        {
            CloseIterator(iterator);
            throw;
        }
    }

    private static object? PrototypeSome(object? thisArgument, object? predicateArgument)
    {
        var iterator = GetReceiverIterator(thisArgument, "some");
        var predicate = GetRequiredCallback(predicateArgument, "some");
        long index = 0;

        try
        {
            while (true)
            {
                var step = iterator.Next();
                if (step.done)
                {
                    return false;
                }

                if (Operators.IsTruthy(InvokeCallback(predicate, step.value, (double)index++)))
                {
                    CloseIterator(iterator);
                    return true;
                }
            }
        }
        catch
        {
            CloseIterator(iterator);
            throw;
        }
    }

    private static object? PrototypeTake(object? thisArgument, in JsCallArguments arguments)
    {
        return new TakeIteratorHelper(GetReceiverIterator(thisArgument, "take"), GetNonNegativeInteger(arguments, "take"));
    }

    private static object? PrototypeToArray(object? thisArgument)
    {
        var iterator = GetReceiverIterator(thisArgument, "toArray");
        var result = new JavaScriptRuntime.Array();

        try
        {
            while (true)
            {
                var step = iterator.Next();
                if (step.done)
                {
                    return result;
                }

                result.Add(step.value);
            }
        }
        catch
        {
            CloseIterator(iterator);
            throw;
        }
    }

    private static object? PrototypeSymbolIterator(object? thisArgument)
    {
        return thisArgument;
    }

    private static IJavaScriptIterator GetReceiverIterator(object? thisValue, string methodName)
    {
        if (thisValue is IJavaScriptIterator iterator)
        {
            return iterator;
        }

        if (thisValue is GeneratorObject generator)
        {
            return new GeneratorIteratorAdapter(generator);
        }

        if (TryCreateIteratorLikeWrapper(thisValue, out var iteratorLike))
        {
            return iteratorLike;
        }

        throw new TypeError($"Iterator.prototype.{methodName} called on incompatible receiver");
    }

    private static bool TryCreateIteratorLikeWrapper(object? value, out IJavaScriptIterator iterator)
    {
        if (value is not null && value is not JsNull && CallableMember.TryCreate(value, "next", out var next))
        {
            iterator = new IteratorLikeWrapper(value, next);
            return true;
        }

        iterator = default!;
        return false;
    }

    private static object GetRequiredCallback(object? callback, string methodName)
    {
        if (CallableOperations.IsCallable(callback))
        {
            return callback!;
        }

        throw new TypeError($"Iterator.prototype.{methodName} requires a callback function");
    }

    private static double GetNonNegativeInteger(in JsCallArguments arguments, string methodName)
    {
        if (arguments.Count == 0)
        {
            throw new TypeError($"Iterator.prototype.{methodName} requires a limit");
        }

        var value = TypeUtilities.ToNumber(arguments.GetArgument(0));
        if (double.IsNaN(value))
        {
            return 0;
        }

        if (double.IsNegativeInfinity(value) || value < 0)
        {
            throw new RangeError($"Iterator.prototype.{methodName} requires a non-negative limit");
        }

        if (double.IsPositiveInfinity(value))
        {
            return double.PositiveInfinity;
        }

        return System.Math.Truncate(value);
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

    private static void CloseIterator(IJavaScriptIterator iterator)
    {
        if (iterator.HasReturn)
        {
            iterator.Return();
        }
    }

    private abstract class IteratorHelperBase : JsObject, IJavaScriptIterator
    {
        private bool _closed;

        protected IteratorHelperBase(IJavaScriptIterator source)
        {
            Source = source;
            InitializeHelperSurface(this);
        }

        protected IJavaScriptIterator Source { get; }

        protected bool Done { get; set; }

        public bool HasReturn => true;

        public IteratorResultObject Next()
        {
            if (Done)
            {
                return IteratorResult.Create(null, true);
            }

            try
            {
                return NextCore();
            }
            catch
            {
                Abort();
                throw;
            }
        }

        public void Return()
        {
            Done = true;
            CloseEarly();
        }

        protected abstract IteratorResultObject NextCore();

        protected IteratorResultObject Finish(object? value = null)
        {
            Done = true;
            return IteratorResult.Create(value, true);
        }

        protected IteratorResultObject FinishAndClose(object? value = null)
        {
            Done = true;
            CloseEarly();
            return IteratorResult.Create(value, true);
        }

        protected virtual void Abort()
        {
            Done = true;
            CloseSource();
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
            if (Source.HasReturn)
            {
                Source.Return();
            }
        }

        protected virtual void OnClose()
        {
        }
    }

    private sealed class MapIteratorHelper : IteratorHelperBase
    {
        private readonly object _mapper;
        private long _index;

        public MapIteratorHelper(IJavaScriptIterator source, object mapper)
            : base(source)
        {
            _mapper = mapper;
        }

        protected override IteratorResultObject NextCore()
        {
            var step = Source.Next();
            if (step.done)
            {
                return Finish();
            }

            return IteratorResult.Create(InvokeCallback(_mapper, step.value, (double)_index++), false);
        }
    }

    private sealed class FilterIteratorHelper : IteratorHelperBase
    {
        private readonly object _predicate;
        private long _index;

        public FilterIteratorHelper(IJavaScriptIterator source, object predicate)
            : base(source)
        {
            _predicate = predicate;
        }

        protected override IteratorResultObject NextCore()
        {
            while (true)
            {
                var step = Source.Next();
                if (step.done)
                {
                    return Finish();
                }

                if (Operators.IsTruthy(InvokeCallback(_predicate, step.value, (double)_index++)))
                {
                    return IteratorResult.Create(step.value, false);
                }
            }
        }
    }

    private sealed class DropIteratorHelper : IteratorHelperBase
    {
        private readonly double _limit;
        private double _dropped;

        public DropIteratorHelper(IJavaScriptIterator source, double limit)
            : base(source)
        {
            _limit = limit;
        }

        protected override IteratorResultObject NextCore()
        {
            while (_dropped < _limit)
            {
                var skipped = Source.Next();
                if (skipped.done)
                {
                    return Finish();
                }

                _dropped++;
            }

            var step = Source.Next();
            return step.done ? Finish() : step;
        }
    }

    private sealed class TakeIteratorHelper : IteratorHelperBase
    {
        private readonly double _limit;
        private double _taken;

        public TakeIteratorHelper(IJavaScriptIterator source, double limit)
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
            if (step.done)
            {
                return Finish();
            }

            _taken++;
            return step;
        }
    }

    private sealed class FlatMapIteratorHelper : IteratorHelperBase
    {
        private readonly object _mapper;
        private long _index;
        private IJavaScriptIterator? _inner;

        public FlatMapIteratorHelper(IJavaScriptIterator source, object mapper)
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
                    var innerStep = _inner.Next();
                    if (!innerStep.done)
                    {
                        return innerStep;
                    }

                    ReleaseInner();
                }

                var step = Source.Next();
                if (step.done)
                {
                    return Finish();
                }

                var mapped = InvokeCallback(_mapper, step.value, (double)_index++);
                _inner = From(mapped);
            }
        }

        protected override void OnClose()
        {
            CloseInner();
        }

        protected override void Abort()
        {
            ReleaseInner();
            base.Abort();
        }

        protected override void CloseEarly()
        {
            CloseInner();
            base.CloseEarly();
        }

        private void CloseInner()
        {
            if (_inner == null)
            {
                return;
            }

            if (_inner.HasReturn)
            {
                _inner.Return();
            }

            _inner = null;
        }

        private void ReleaseInner()
        {
            _inner = null;
        }
    }

    private readonly record struct CallableMember(object? Callable, MethodInfo? Method)
    {
        public static bool TryCreate(object target, string name, out CallableMember member)
        {
            var propertyValue = ObjectRuntime.GetProperty(target, name);
            if (CallableOperations.IsCallable(propertyValue))
            {
                member = new CallableMember(propertyValue, null);
                return true;
            }

            var method = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(candidate =>
                    string.Equals(candidate.Name, name, StringComparison.OrdinalIgnoreCase)
                    && candidate.GetParameters().Length <= 1);

            if (method != null)
            {
                member = new CallableMember(null, method);
                return true;
            }

            member = default;
            return false;
        }

        public bool HasValue => Callable != null || Method != null;

        public object? Invoke(object target, params object?[] args)
        {
            var previousThis = RuntimeServices.SetCurrentThis(target);
            try
            {
                if (Callable != null)
                {
                    return CallableOperations.Call(Callable, target, args);
                }

                if (Method == null)
                {
                    return null;
                }

                var parameters = Method.GetParameters();
                return Method.Invoke(target, parameters.Length == 0 ? System.Array.Empty<object?>() : args);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }
    }

    private sealed class GeneratorIteratorAdapter : IJavaScriptIterator
    {
        private readonly GeneratorObject _generator;

        public GeneratorIteratorAdapter(GeneratorObject generator)
        {
            _generator = generator;
        }

        public bool HasReturn => true;

        public IteratorResultObject Next()
            => (IteratorResultObject)_generator.next();

        public void Return()
        {
            _ = _generator.@return(null);
        }
    }

    private sealed class IteratorLikeWrapper : IJavaScriptIterator
    {
        private readonly object _target;
        private readonly CallableMember _next;
        private readonly CallableMember _return;

        public IteratorLikeWrapper(object target, CallableMember next)
        {
            _target = target;
            _next = next;
            _ = CallableMember.TryCreate(target, "return", out _return);
            InitializeIteratorSurface(this);
        }

        public bool HasReturn => _return.HasValue;

        public IteratorResultObject Next()
        {
            var result = _next.Invoke(_target);
            if (result is IteratorResultObject iteratorResult)
            {
                return iteratorResult;
            }

            if (result is IIteratorResult typedResult)
            {
                return IteratorResult.Create(typedResult.value, typedResult.done);
            }

            if (result == null)
            {
                throw new TypeError("Iterator.next() returned null or undefined");
            }

            var done = TypeUtilities.ToBoolean(ObjectRuntime.GetItem(result, "done"));
            var value = ObjectRuntime.GetItem(result, "value");
            return IteratorResult.Create(value, done);
        }

        public void Return()
        {
            if (HasReturn)
            {
                _ = _return.Invoke(_target);
            }
        }
    }
}
