using Jroc.Services.TwoPhaseCompilation;
using Jroc.SymbolTables;

namespace Jroc.HIR;

public sealed class HIRArrowFunctionExpression : HIRExpression
{
	public HIRArrowFunctionExpression(
		CallableId callableId,
		Scope functionScope,
		bool requiresLexicalSuperConstructorContext,
		bool containsSuperConstructorCallInBody)
	{
		CallableId = callableId;
		FunctionScope = functionScope;
		RequiresLexicalSuperConstructorContext = requiresLexicalSuperConstructorContext;
		ContainsSuperConstructorCallInBody = containsSuperConstructorCallInBody;
	}

	public CallableId CallableId { get; }
	public Scope FunctionScope { get; }
	public bool RequiresLexicalSuperConstructorContext { get; }
	public bool ContainsSuperConstructorCallInBody { get; }
}
