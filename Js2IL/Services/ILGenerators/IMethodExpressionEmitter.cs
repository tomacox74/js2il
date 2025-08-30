using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acornima.Ast;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.Services.ILGenerators
{
    internal enum CallSiteContext { Statement, Expression }

    record ConditionalBranching
    {
        public LabelHandle BranchOnTrue { get; init; }

        public LabelHandle? BranchOnFalse { get; init; }
    }

    record TypeCoercion
    {
        /// <summary>
        /// When true, the expression emitter will convert the result to a string.
        /// Default is false.
        /// </summary>
        /// <remarks>
        /// A example would be the expression "hello " + 5.   This parameter drops a hint that we need "5", not the number 5.
        /// Converting a number to a string at compile time instead of runtime is preferred.
        /// </remarks>
        public bool toString = false;

        /// <summary>
        /// When true, the expression emitter will box primitive results (e.g., numbers) before returning.
        /// Default is false
        /// </summary>
        public bool boxResult = false;
    }

    record ExpressionResult
    {
        public JavascriptType JsType { get; init; }
        public Type? ClrType { get; init; }
    }

    /// <summary>
    /// For any given javascript expression this component emits the IL code that represents the expression.
    /// At execution time the result of the expression will be available in the IL stack.
    /// </summary>
    /// <remarks>
    /// This interface is needed to prevent circular dependencies between the the expression emitters for nested expressions.
    /// </remarks>
    internal interface IMethodExpressionEmitter
    {
        /// <summary>
        /// Emits the IL code for the given expression.
        /// </summary>
        /// <param name="expression">The expression to emit.</param>a value maybe needed to be changed to 
        /// <param name="typeCoercion">In some cases we have a expected type we want from the expression.. for example we need to force to a string or we want to keep it boxed</param>
        /// <param name="branching">If the expression is part of a conditional branching (such as for or if), this parameter will contain the branching information.</param>
        /// <remarks>Branching operators in IL are mutually exclusive wth normal comparison operators.  Is in the case of conditional branching we need to emit the br opcodes instead of the comparison opcodes.</remarks>
        ExpressionResult Emit(Expression expression, TypeCoercion typeCoercion, CallSiteContext context = CallSiteContext.Expression, ConditionalBranching? branching = null);
    }
}
