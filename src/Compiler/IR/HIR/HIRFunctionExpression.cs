using Jroc.Services.TwoPhaseCompilation;
using Jroc.SymbolTables;

namespace Jroc.HIR;

public sealed class HIRFunctionExpression : HIRExpression
{
	public HIRFunctionExpression(CallableId callableId, Scope functionScope, bool isNonConstructible = false)
	{
		CallableId = callableId;
		FunctionScope = functionScope;
		IsNonConstructible = isNonConstructible;
	}

	public CallableId CallableId { get; }
	public Scope FunctionScope { get; }
	public bool IsNonConstructible { get; }
}
