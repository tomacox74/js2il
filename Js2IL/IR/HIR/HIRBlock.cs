using System.Collections.Immutable;

namespace Js2IL.HIR;

public class HIRBlock : HIRStatement
{
    public HIRBlock(IEnumerable<HIRStatement> statements)
    {
        Statements = statements.ToImmutableArray();
    }
  
    public ImmutableArray<HIRStatement> Statements { get; }
}