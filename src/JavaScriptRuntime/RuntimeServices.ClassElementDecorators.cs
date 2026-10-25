namespace JavaScriptRuntime;

public partial class RuntimeServices
{
    internal sealed record ClassElementDecoration(string Name, string Kind, bool IsStatic, object[] Decorators)
    {
        internal List<object> ValueInitializers { get; } = new();
        internal List<object> ExtraInitializers { get; } = new();
    }

    public static object PrepareClassElementDecorators(object owner, string name, string kind, bool isStatic, object[] decorators)
    {
        var constructor = (JsClassConstructorObject)owner;
        constructor.ElementDecorations.Add(new(name, kind, isStatic, decorators));
        return owner;
    }

    public static object ApplyClassElementDecorators(object owner)
    {
        var constructor = (JsClassConstructorObject)owner;
        var staticInitializers = new List<object>();
        foreach (var element in constructor.ElementDecorations)
        {
            var target = element.IsStatic ? owner : ObjectRuntime.GetProperty(owner, "prototype")!;
            _ = TryEnsureLazyClassMethodDataProperty(target, element.Name, out _);
            PropertyDescriptorStore.TryGetOwn(target, element.Name, out var descriptor);
            var value = element.Kind switch
            {
                "field" => null,
                "getter" => descriptor.Get,
                "setter" => descriptor.Set,
                _ => descriptor.Value
            };
            var access = new JsObject();
            access["has"] = (BuiltinFunction1)((_, receiver) => ObjectRuntime.HasPropertyIn(element.Name, receiver));
            if (element.Kind != "setter")
                access["get"] = (BuiltinFunction1)((_, receiver) => ObjectRuntime.GetProperty(receiver!, element.Name));
            if (element.Kind is "field" or "setter")
                access["set"] = (BuiltinFunction2)((_, receiver, argument) => ObjectRuntime.SetItem(receiver!, element.Name, argument, true));
            for (var i = element.Decorators.Length - 1; i >= 0; i--)
            {
                var addInitializer = new ClassDecoratorAddInitializer(element.ExtraInitializers);
                var context = new JsObject
                {
                    ["kind"] = element.Kind, ["name"] = element.Name,
                    ["static"] = element.IsStatic, ["private"] = false,
                    ["access"] = access, ["addInitializer"] = addInitializer
                };
                object? replacement;
                try
                {
                    replacement = CallableOperations.Call2(element.Decorators[i], null, value, context);
                }
                finally
                {
                    addInitializer.Finish();
                }
                if (replacement == null) continue;
                if (!CallableOperations.IsCallable(replacement))
                    throw new TypeError("Element decorator must return a callable or undefined");
                if (element.Kind == "field")
                    element.ValueInitializers.Insert(0, replacement);
                else
                    value = replacement;
            }
            if (element.Kind != "field")
            {
                if (element.Kind == "getter") descriptor.Get = value;
                else if (element.Kind == "setter") descriptor.Set = value;
                else descriptor.Value = value;
                PropertyDescriptorStore.DefineOrUpdate(target, element.Name, descriptor);
                (element.IsStatic ? staticInitializers : constructor.InstanceDecoratorInitializers)
                    .AddRange(element.ExtraInitializers);
            }
        }
        foreach (var initializer in staticInitializers)
            CallableOperations.Call0(initializer, owner);
        return owner;
    }

    public static object? TransformDecoratedClassField(Type ownerType, object receiver, string name, object? value)
    {
        var constructor = CaptureClassPrivateBrand(ownerType, receiver, GetCurrentCallee()) as JsClassConstructorObject;
        if (constructor == null) return value;
        foreach (var element in constructor.ElementDecorations)
        {
            if (element.Kind != "field" || element.Name != name
                || element.IsStatic != (receiver is JsClassConstructorObject))
                continue;
            foreach (var initializer in element.ValueInitializers)
                value = CallableOperations.Call1(initializer, receiver, value);
        }
        return value;
    }

    public static object CompleteDecoratedClassField(Type ownerType, object receiver, string name)
    {
        var constructor = CaptureClassPrivateBrand(ownerType, receiver, GetCurrentCallee()) as JsClassConstructorObject;
        if (constructor == null) return receiver;
        foreach (var element in constructor.ElementDecorations)
        {
            if (element.Kind == "field" && element.Name == name
                && element.IsStatic == (receiver is JsClassConstructorObject))
                foreach (var initializer in element.ExtraInitializers)
                    CallableOperations.Call0(initializer, receiver);
        }
        return receiver;
    }
}
