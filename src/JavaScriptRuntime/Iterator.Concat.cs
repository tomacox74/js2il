using System;

namespace JavaScriptRuntime;

public static partial class Iterator
{
    private static object? ConstructorConcat(object? thisArgument, in JsCallArguments arguments)
    {
        var iterables = new ConcatIterable[arguments.Count];
        for (var index = 0; index < iterables.Length; index++)
        {
            var item = arguments.GetArgument(index);
            if (!Proxy.IsObjectLikeValue(item))
            {
                throw new TypeError("Iterator.concat requires iterable objects");
            }

            var method = ObjectRuntime.GetItem(item!, Symbol.iterator);
            if (!CallableOperations.IsCallable(method))
            {
                throw new TypeError("Iterator.concat requires a callable Symbol.iterator method");
            }

            iterables[index] = new ConcatIterable(item!, method!);
        }

        return new ConcatIteratorHelper(iterables);
    }

    private readonly record struct ConcatIterable(object Iterable, object OpenMethod);

    private sealed class ConcatIteratorHelper : IteratorHelperBase, IJavaScriptIterator
    {
        private readonly ConcatIterable[] _iterables;
        private IteratorRecord? _active;
        private int _index;
        private bool _returning;

        public ConcatIteratorHelper(ConcatIterable[] iterables)
            : base(new IteratorRecord(new JsObject(), nextMethod: null))
        {
            _iterables = iterables;
        }

        IteratorResultObject IJavaScriptIterator.Next()
        {
            if (_returning)
            {
                throw new TypeError("Iterator helper is already running");
            }

            return Next();
        }

        void IJavaScriptIterator.Return()
        {
            if (_returning)
            {
                throw new TypeError("Iterator helper is already running");
            }

            _returning = true;
            try
            {
                Return();
            }
            finally
            {
                _returning = false;
            }
        }

        protected override IteratorResultObject NextCore()
        {
            while (true)
            {
                if (_active is null)
                {
                    if (_index == _iterables.Length)
                    {
                        return Finish();
                    }

                    var iterable = _iterables[_index++];
                    var iterator = CallableOperations.Call(
                        iterable.OpenMethod,
                        iterable.Iterable,
                        System.Array.Empty<object?>());
                    if (!Proxy.IsObjectLikeValue(iterator))
                    {
                        throw new TypeError("Iterator.concat iterable method must return an object");
                    }

                    _active = GetIteratorDirect(iterator!);
                }

                var step = _active.Next();
                if (step.Done)
                {
                    _active = null;
                    continue;
                }

                return IteratorResult.Create(step.Value, done: false);
            }
        }

        protected override void CloseEarly()
        {
            var active = _active;
            _active = null;
            active?.Close();
        }

        protected override void CompleteAbruptly()
        {
            _active = null;
            base.CompleteAbruptly();
        }
    }
}
