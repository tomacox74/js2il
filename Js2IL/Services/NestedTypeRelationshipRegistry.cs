using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.Services
{
    /// <summary>
    /// Collects NestedClass table relationships and emits them in a single, sorted pass.
    ///
    /// ECMA-335 requires the NestedClass table to be sorted by the NestedClass column (TypeDef token).
    /// Emitting rows from multiple places can violate this ordering and cause load-time BadImageFormatException.
    /// </summary>
    internal sealed class NestedTypeRelationshipRegistry
    {
        private readonly HashSet<(TypeDefinitionHandle Nested, TypeDefinitionHandle Enclosing)> _relationships = new();
        private bool _emitted;

        public void Add(TypeDefinitionHandle nested, TypeDefinitionHandle enclosing)
        {
            if (nested.IsNil) throw new ArgumentException("Nested type handle cannot be nil.", nameof(nested));
            if (enclosing.IsNil) throw new ArgumentException("Enclosing type handle cannot be nil.", nameof(enclosing));
            _relationships.Add((nested, enclosing));
        }

        public void EmitAllSorted(MetadataBuilder metadataBuilder)
        {
            if (metadataBuilder == null) throw new ArgumentNullException(nameof(metadataBuilder));
            if (_emitted)
            {
                return;
            }

            foreach (var (nested, enclosing) in _relationships
                .OrderBy(r => MetadataTokens.GetRowNumber(r.Nested)))
            {
                metadataBuilder.AddNestedType(nested, enclosing);
            }

            _emitted = true;
        }
    }
}
