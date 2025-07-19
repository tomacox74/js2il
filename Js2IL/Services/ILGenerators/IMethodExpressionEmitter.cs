using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acornima.Ast;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.Services.ILGenerators
{
    record ConditionalBranching
    {
        public LabelHandle BranchOnTrue { get; init; }

        public LabelHandle? BranchOnFalse { get; init; }
    }

    record TypeCoercion
    {
        public bool toString = false;
        public bool boxed = false;
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
        JavascriptType Emit(Expression expression, TypeCoercion typeCoercion, ConditionalBranching? branching = null);
    }
}
