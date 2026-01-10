using Acornima.Ast;

namespace Js2IL.HIR;

public sealed class HIRArrowFunctionExpression : HIRExpression
{
	public HIRArrowFunctionExpression(ArrowFunctionExpression arrow)
	{
		Arrow = arrow;
	}

	public ArrowFunctionExpression Arrow { get; }
}
