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
        IReadOnlyDictionary<string, Type> fieldClrTypesByMemberName)
    {
        Shape = shape;
        TypeName = typeName;
        TypeHandle = typeHandle;
        ConstructorHandle = constructorHandle;
        FieldHandlesByMemberName = fieldHandlesByMemberName;
        FieldClrTypesByMemberName = fieldClrTypesByMemberName;
    }

    public ObjectLiteralShapeInfo Shape { get; }

    public string TypeName { get; }

    public TypeDefinitionHandle TypeHandle { get; }

    public MethodDefinitionHandle ConstructorHandle { get; }

    public IReadOnlyDictionary<string, FieldDefinitionHandle> FieldHandlesByMemberName { get; }

    public IReadOnlyDictionary<string, Type> FieldClrTypesByMemberName { get; }
}
