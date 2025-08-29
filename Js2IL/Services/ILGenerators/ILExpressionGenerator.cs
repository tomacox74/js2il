using System;
using Acornima.Ast;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Dedicated expression emitter. Initially a thin wrapper to enable incremental refactoring
    /// out of ILMethodGenerator without changing call sites.
    /// </summary>
    internal sealed class ILExpressionGenerator : IMethodExpressionEmitter
    {
        private readonly IMethodExpressionEmitter _inner;

        /// <summary>
        /// Create an expression generator that delegates to another emitter.
        /// </summary>
        /// <param name="inner">An existing expression emitter (e.g., ILMethodGenerator) to delegate to.</param>
        public ILExpressionGenerator(IMethodExpressionEmitter inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        /// <inheritdoc />
        public JavascriptType Emit(Expression expression, TypeCoercion typeCoercion, ConditionalBranching? branching = null)
        {
            return _inner.Emit(expression, typeCoercion, branching);
        }
    }
}
