namespace JavaScriptRuntime;

public partial class RuntimeServices
{
    private sealed record ClassDecoration(object Constructor, List<object> Initializers);

    public static object ApplyClassDecorators(object constructor, object[] decorators, object? name)
    {
        var initializers = new List<object>();
        for (var index = decorators.Length - 1; index >= 0; index--)
        {
            var addInitializer = new ClassDecoratorAddInitializer(initializers);
            var context = new JsObject
            {
                ["kind"] = "class",
                ["name"] = name,
                ["addInitializer"] = addInitializer
            };
            object? replacement;
            try
            {
                replacement = CallableOperations.Call2(decorators[index], null, constructor, context);
            }
            finally
            {
                addInitializer.Finish();
            }

            if (replacement != null)
            {
                if (!CallableOperations.IsCallable(replacement))
                {
                    throw new TypeError("Class decorator must return a callable or undefined");
                }
                constructor = replacement;
            }
        }

        return new ClassDecoration(constructor, initializers);
    }

    public static object CompleteClassDecorators(object state)
    {
        if (state is not ClassDecoration decoration)
        {
            throw new InvalidOperationException("Class initializers require a completed decoration.");
        }
        foreach (var initializer in decoration.Initializers)
        {
            CallableOperations.Call0(initializer, decoration.Constructor);
        }
        return decoration.Constructor;
    }

    private sealed class ClassDecoratorAddInitializer : JsFunctionObject
    {
        private readonly List<object> _initializers;
        private bool _finished;

        public ClassDecoratorAddInitializer(List<object> initializers)
        {
            _initializers = initializers;
            Function.DefineMetadataProperty(this, "name", "addInitializer");
            Function.DefineMetadataProperty(this, "length", 1d);
        }

        public override bool RequiresInvocationContext => false;

        public void Finish() => _finished = true;

        protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
        {
            if (_finished)
            {
                throw new TypeError("Cannot add initializers after decoration has completed");
            }
            var callback = arguments.GetArgument(0);
            if (!CallableOperations.IsCallable(callback))
            {
                throw new TypeError("Class decorator initializer must be callable");
            }
            _initializers.Add(callback!);
            return null;
        }
    }
}
