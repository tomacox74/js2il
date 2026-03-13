using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Js2IL.Services;

/// <summary>
/// Tracks TypeDefinitionHandles for module root types (Modules.&lt;ModuleName&gt;).
/// Used to support nesting the global scope type under the module type.
/// </summary>
internal sealed class ModuleTypeMetadataRegistry
{
    private readonly Dictionary<string, TypeDefinitionHandle> _modules = new(StringComparer.Ordinal);

    public bool TryGet(string moduleName, out TypeDefinitionHandle type)
    {
        if (moduleName == null) throw new ArgumentNullException(nameof(moduleName));
        return _modules.TryGetValue(moduleName, out type);
    }

    public void Add(string moduleName, TypeDefinitionHandle type)
    {
        if (moduleName == null) throw new ArgumentNullException(nameof(moduleName));
        if (type.IsNil) throw new ArgumentException("Module type handle cannot be nil.", nameof(type));
        _modules[moduleName] = type;
    }
}
