using System.Reflection.Metadata;
using Jroc.SymbolTables;

namespace Jroc.Services.VariableBindings;

/// <summary>
/// Metadata for an eligible object literal's generated CLR type.
/// Populated by TypeGenerator and consumed by later construction/access phases.
/// </summary>
public sealed class ObjectLiteralTypeMetadata
{
    public ObjectLiteralTypeMetadata(
        ObjectLiteralShapeInfo shape,
        string typeName,
        TypeDefinitionHandle typeHandle,
        MethodDefinitionHandle constructorHandle,
        IReadOnlyDictionary<string, FieldDefinitionHandle> fieldHandlesByMemberName,
        IReadOnlyDictionary<string, Type> fieldClrTypesByMemberName,
        IReadOnlyDictionary<string, MethodDefinitionHandle> getterHandlesByMemberName,
        IReadOnlyDictionary<string, MethodDefinitionHandle> setterHandlesByMemberName)
    {
        Shape = shape;
        TypeName = typeName;
        TypeHandle = typeHandle;
        ConstructorHandle = constructorHandle;
        FieldHandlesByMemberName = fieldHandlesByMemberName;
        FieldClrTypesByMemberName = fieldClrTypesByMemberName;
        GetterHandlesByMemberName = getterHandlesByMemberName;
        SetterHandlesByMemberName = setterHandlesByMemberName;
    }

    public ObjectLiteralShapeInfo Shape { get; }

    public string TypeName { get; }

    public TypeDefinitionHandle TypeHandle { get; }

    public MethodDefinitionHandle ConstructorHandle { get; }

    /// <summary>
    /// Private backing fields (named "_&lt;member&gt;") behind the generated accessor methods.
    /// </summary>
    public IReadOnlyDictionary<string, FieldDefinitionHandle> FieldHandlesByMemberName { get; }

    public IReadOnlyDictionary<string, Type> FieldClrTypesByMemberName { get; }

    /// <summary>
    /// Generated "get_&lt;member&gt;" accessor methods returning the backing field.
    /// </summary>
    public IReadOnlyDictionary<string, MethodDefinitionHandle> GetterHandlesByMemberName { get; }

    /// <summary>
    /// Generated "set_&lt;member&gt;" accessor methods that store the backing field and
    /// mirror the value into JsObject storage.
    /// </summary>
    public IReadOnlyDictionary<string, MethodDefinitionHandle> SetterHandlesByMemberName { get; }
}
