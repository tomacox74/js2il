using Acornima.Ast;

namespace Js2IL.HIR;

public sealed class HIRFunctionExpression : HIRExpression
{
	public HIRFunctionExpression(FunctionExpression function)
	{
		Function = function;
	}

	public FunctionExpression Function { get; }
}
