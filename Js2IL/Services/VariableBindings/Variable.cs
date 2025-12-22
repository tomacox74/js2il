using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using JavaScriptRuntime;

namespace Js2IL.Services
{
    /// <summary>
    /// Represents a JavaScript variable that maps to a field in a scope type.
    /// </summary>
    internal abstract record Variable
    {
        public required string Name;
        
        public JavascriptType Type = JavascriptType.Unknown;

        private Type? _clrType;

        // If this variable holds a known intrinsic runtime object (e.g., Node module instance),
        // capture its CLR type so emitters can bind directly to its methods.
        public required Type? ClrType 
        { 
            get => _clrType;
            set
            {
                if (IsStableType && _clrType != null && _clrType != value)
                {
                    throw new InvalidOperationException(
                        $"Attempted to change ClrType of stable-typed variable '{Name}' " +
                        $"from '{_clrType.FullName}' to '{value?.FullName}'. This indicates a bug in type inference.");
                }
                _clrType = value;
            }
        }

        /// <summary>
        /// Indicates whether the variable's type has been inferred during static analysis
        /// and is known to never change. When true, any attempt to change ClrType is a bug.
        /// </summary>
        public bool IsStableType { get; init; } = false;

        // Unified optional metadata for compatibility with existing emitters
        public bool IsParameter { get; init; } = false;
        // For parameters: IL argument index (including any leading non-JS params already accounted for by caller)
        public int ParameterIndex { get; init; } = -1;
        // Declaring scope name for field-backed variables
        public string ScopeName { get; init; } = string.Empty;
        // Field handle for ldfld/stfld (for field-backed variables)
        public FieldDefinitionHandle FieldHandle { get; init; }
        // Local variable slot index for uncaptured variables (or -1 if not a local)
        public int LocalSlot { get; init; } = -1;
    }

    internal record LocalVariable : Variable;

    internal record ParameterVariable : Variable;

    internal record ScopeVariable : Variable
    {
        // Index into the scopes[] array for parent scope access
        public int ParentScopeIndex { get; init; } = -1;
    }

    internal enum ObjectReferenceLocation
    {
        Local,
        Parameter,
        ScopeArray
    }

    internal record ScopeObjectReference
    {
        public required ObjectReferenceLocation Location;
        public required int Address; // Name of the variable in the scope
    }
}
