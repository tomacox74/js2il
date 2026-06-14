using System.Collections.Immutable;

namespace Jroc.HIR;

public class HIRBlock : HIRStatement
{
    public HIRBlock(IEnumerable<HIRStatement> statements, string? scopeName = null)
    {
        Statements = statements.ToImmutableArray();
        ScopeName = scopeName;
    }
  
    public ImmutableArray<HIRStatement> Statements { get; }
    public string? ScopeName { get; }
}
