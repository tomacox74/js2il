using System.Collections.Immutable;

namespace Jroc.HIR;

public sealed class HIRPreparedDynamicFunctionExpression(
    HIRExpression callee,
    IEnumerable<HIRExpression> arguments,
    HIRFunctionExpression? factory,
    IEnumerable<string> sources,
    bool construct,
    string? syntaxError = null) : HIRExpression
{
    public HIRExpression Callee { get; } = callee;
    public ImmutableArray<HIRExpression> Arguments { get; } = arguments.ToImmutableArray();
    public HIRFunctionExpression? Factory { get; } = factory;
    public ImmutableArray<string> Sources { get; } = sources.ToImmutableArray();
    public bool Construct { get; } = construct;
    public string? SyntaxError { get; } = syntaxError;
}
