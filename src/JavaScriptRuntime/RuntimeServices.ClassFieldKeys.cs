namespace JavaScriptRuntime;

public partial class RuntimeServices
{
    public static object ThrowClassNameTDZ(string name)
        => throw new ReferenceError($"Cannot access '{name}' before initialization");

    public static object GetClassFieldHomeObject(object receiver)
        => PrototypeChain.GetPrototypeOrNull(receiver)
            ?? throw new InvalidOperationException("A class field initializer requires the instance prototype.");

    public static object SetClassComputedFieldKey(object constructorValue, object fieldId, object? key)
    {
        if (constructorValue is not JsClassConstructorObject constructor || fieldId is not string id)
        {
            throw new InvalidOperationException("Computed class field keys require a class constructor and field identity.");
        }

        var propertyKey = ObjectRuntime.ToPropertyKeyString(key);
        constructor.ComputedFieldKeys[id] = propertyKey;
        return propertyKey;
    }

    public static object GetClassComputedFieldKey(Type ownerType, object? receiver, object fieldId)
    {
        var constructor = CaptureClassPrivateBrand(ownerType, receiver, GetCurrentCallee()) as JsClassConstructorObject
            ?? GetExecutingClassConstructor(ownerType);
        if (fieldId is not string id
            || !constructor.ComputedFieldKeys.TryGetValue(id, out var key))
        {
            throw new InvalidOperationException("The computed class field name was not captured during class evaluation.");
        }

        return key;
    }
}
