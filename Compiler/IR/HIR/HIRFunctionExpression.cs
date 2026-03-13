using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.SymbolTables;

namespace Js2IL.HIR;

public sealed class HIRFunctionExpression : HIRExpression
{
	public HIRFunctionExpression(CallableId callableId, Scope functionScope)
	{
		CallableId = callableId;
		FunctionScope = functionScope;
	}

	public CallableId CallableId { get; }
	public Scope FunctionScope { get; }
}
