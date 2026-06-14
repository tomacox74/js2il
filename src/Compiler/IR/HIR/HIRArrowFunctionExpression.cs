using Jroc.Services.TwoPhaseCompilation;
using Jroc.SymbolTables;

namespace Jroc.HIR;

public sealed class HIRArrowFunctionExpression : HIRExpression
{
	public HIRArrowFunctionExpression(CallableId callableId, Scope functionScope)
	{
		CallableId = callableId;
		FunctionScope = functionScope;
	}

	public CallableId CallableId { get; }
	public Scope FunctionScope { get; }
}
