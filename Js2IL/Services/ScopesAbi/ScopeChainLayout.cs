using System.Reflection.Metadata;

namespace Js2IL.Services.ScopesAbi;

/// <summary>
/// Represents a single slot in the scopes array (object[] scopes).
/// Each slot holds an instance of a specific scope class.
/// </summary>
/// <param name="Index">The 0-based index in the scopes array.</param>
/// <param name="ScopeName">The name of the scope (module-qualified if needed).</param>
/// <param name="ScopeTypeHandle">The TypeDefinitionHandle for the scope class (for castclass).</param>
public sealed record ScopeSlot(
    int Index,
    string ScopeName,
    TypeDefinitionHandle ScopeTypeHandle
);

/// <summary>
/// Defines the layout of the scopes array (object[] scopes) for a callable.
/// The ordering is outermost → innermost (global/module first).
/// </summary>
/// <param name="Slots">The ordered list of scope slots. Slots[0] is global/module scope.</param>
public sealed record ScopeChainLayout(
    IReadOnlyList<ScopeSlot> Slots
)
{
    /// <summary>
    /// Gets the number of slots in the scopes array.
    /// </summary>
    public int Length => Slots.Count;

    /// <summary>
    /// Returns true if this layout contains no slots (no parent scopes needed).
    /// </summary>
    public bool IsEmpty => Slots.Count == 0;

    /// <summary>
    /// Finds a slot by scope name.
    /// </summary>
    /// <param name="scopeName">The scope name to find.</param>
    /// <returns>The slot if found, otherwise null.</returns>
    public ScopeSlot? FindSlot(string scopeName)
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].ScopeName == scopeName)
            {
                return Slots[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Gets the index for a given scope name.
    /// </summary>
    /// <param name="scopeName">The scope name to find.</param>
    /// <returns>The slot index if found, otherwise -1.</returns>
    public int IndexOf(string scopeName)
    {
        var slot = FindSlot(scopeName);
        return slot?.Index ?? -1;
    }

    /// <summary>
    /// Creates an empty scope chain layout (for callables that don't need parent scopes).
    /// </summary>
    public static ScopeChainLayout Empty { get; } = new(Array.Empty<ScopeSlot>());
}
