namespace JavaScriptRuntime;

public partial class RuntimeServices
{
    public static object SetClassLexicalPrivateBrand(
        object constructor, Type enclosingType, object? enclosingOwner, object? lexicalThis)
    {
        ((JsClassConstructorObject)constructor).LexicalPrivateBrandOwner =
            enclosingOwner as JsClassConstructorObject
            ?? CaptureClassPrivateBrand(enclosingType, lexicalThis, GetCurrentCallee()) as JsClassConstructorObject;
        return constructor;
    }

    private static bool TryGetLexicalClassPrivateBrand(
        JsClassConstructorObject? lexicalOwner, Type ownerType, out JsClassConstructorObject constructor)
    {
        for (var owner = lexicalOwner; owner != null; owner = owner.LexicalPrivateBrandOwner)
        {
            if (owner.Type == ownerType)
            {
                constructor = owner;
                return true;
            }
        }
        constructor = null!;
        return false;
    }

    public static object? InitializeClassPrivateField(object receiver, Type ownerType, string name, object? value)
    {
        ObjectRuntime.EnsureCanAddPrivateElement(receiver);
        var owner = GetPrivateFieldOwner(ownerType);
        var slot = _classInstancePrivateBrands.GetOrCreateValue(receiver);
        lock (slot.Constructors)
        {
            if (!slot.Fields.TryGetValue(owner, out var fields))
            {
                fields = new(StringComparer.Ordinal);
                slot.Fields.Add(owner, fields);
            }
            if (!fields.TryAdd(name, value))
                throw new TypeError("Cannot initialize a private field more than once");
        }
        return value;
    }

    public static object? GetClassPrivateField(object receiver, Type ownerType, string name)
        => AccessClassPrivateField(receiver, ownerType, name, null, write: false);

    public static object? SetClassPrivateField(object receiver, Type ownerType, string name, object? value)
        => AccessClassPrivateField(receiver, ownerType, name, value, write: true);

    private static object GetPrivateFieldOwner(Type ownerType)
        => CaptureClassPrivateBrand(ownerType, ResolveLexicalThis(GetCurrentThis()), GetCurrentCallee())
            ?? (GetCurrentCallee() is JsClassConstructorObject constructor && constructor.Type == ownerType
                ? constructor : ownerType);

    private static object? AccessClassPrivateField(object receiver, Type ownerType, string name, object? value, bool write)
    {
        var owner = GetPrivateFieldOwner(ownerType);
        if (receiver != null && _classInstancePrivateBrands.TryGetValue(receiver, out var slot))
        {
            lock (slot.Constructors)
            {
                if (slot.Fields.TryGetValue(owner, out var fields) && fields.TryGetValue(name, out var previous))
                {
                    if (write) fields[name] = value;
                    return write ? value : previous;
                }
            }
        }
        throw new TypeError("Receiver does not have the requested private field");
    }
}
