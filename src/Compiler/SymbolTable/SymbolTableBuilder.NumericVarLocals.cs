using Acornima;
using Acornima.Ast;

namespace Jroc.SymbolTables;

public partial class SymbolTableBuilder
{
    private void InferDefinitelyInitializedNumericVarLocals(Scope root)
    {
        if (root.Kind == ScopeKind.Function)
        {
            var proposedClrTypes = root.Bindings.Values
                .Where(binding => binding.IsStableType && binding.ClrType != null)
                .ToDictionary(
                    binding => binding.Name,
                    binding => binding.ClrType!,
                    StringComparer.Ordinal);

            InferDefinitelyInitializedNumericVarLocals(root, proposedClrTypes);

            foreach (var (name, clrType) in proposedClrTypes)
            {
                var binding = root.Bindings[name];
                binding.ClrType = clrType;
                binding.IsStableType = true;
            }
        }

        foreach (var child in root.Children)
        {
            InferDefinitelyInitializedNumericVarLocals(child);
        }
    }

    private void InferDefinitelyInitializedNumericVarLocals(
        Scope scope,
        Dictionary<string, Type> proposedClrTypes)
    {
        foreach (var binding in scope.Bindings.Values)
        {
            binding.CanUseUnboxedLocal = false;
        }

        if (scope.Kind != ScopeKind.Function || !TryGetCallableBody(scope, out var body))
        {
            return;
        }

        bool changed;
        do
        {
            changed = false;
            foreach (var binding in scope.Bindings.Values)
            {
                if (binding.Kind != BindingKind.Var
                    || binding.IsCaptured
                    || scope.Parameters.Contains(binding.Name)
                    || binding.CanUseUnboxedLocal)
                {
                    continue;
                }

                var analyzer = new NumericVarDefiniteAssignmentAnalyzer(
                    this,
                    scope,
                    binding,
                    proposedClrTypes);
                if (!analyzer.TryAnalyze(body))
                {
                    continue;
                }

                if (!proposedClrTypes.TryGetValue(binding.Name, out var existingType))
                {
                    proposedClrTypes[binding.Name] = typeof(double);
                    changed = true;
                }
                else if (existingType != typeof(double))
                {
                    continue;
                }

                binding.CanUseUnboxedLocal = true;
                changed = true;
            }
        }
        while (changed);

        // General stable-type inference can see a later numeric initializer and infer
        // through an earlier hoisted read (for example, `var y = x; var x = 1`).
        // Such an initializer evaluates to undefined and must not acquire a numeric
        // local or callable-return fact.
        foreach (var binding in scope.Bindings.Values)
        {
            if (binding.Kind != BindingKind.Var
                || binding.CanUseUnboxedLocal
                || binding.DeclarationNode is not VariableDeclarator { Init: Expression initializer })
            {
                continue;
            }

            var analyzer = new NumericVarDefiniteAssignmentAnalyzer(
                this,
                scope,
                binding,
                proposedClrTypes);
            if (!analyzer.InitializerForwardsUnsafeNumericDependency(initializer))
            {
                continue;
            }

            proposedClrTypes.Remove(binding.Name);
            binding.ClrType = null;
            binding.IsStableType = false;
        }
    }

    private static bool TryGetCallableBody(Scope scope, out BlockStatement body)
    {
        body = scope.AstNode switch
        {
            FunctionDeclaration declaration => declaration.Body,
            FunctionExpression expression => expression.Body,
            ArrowFunctionExpression { Body: BlockStatement block } => block,
            _ => null!
        };
        return body != null;
    }

    private sealed class NumericVarDefiniteAssignmentAnalyzer
    {
        private readonly SymbolTableBuilder _builder;
        private readonly Scope _functionScope;
        private readonly BindingInfo _target;
        private readonly Dictionary<string, Type> _proposedClrTypes;
        private bool _sawNumericWrite;

        public NumericVarDefiniteAssignmentAnalyzer(
            SymbolTableBuilder builder,
            Scope functionScope,
            BindingInfo target,
            Dictionary<string, Type> proposedClrTypes)
        {
            _builder = builder;
            _functionScope = functionScope;
            _target = target;
            _proposedClrTypes = proposedClrTypes;
        }

        public bool TryAnalyze(BlockStatement body)
        {
            if (ContainsAbruptLoopControl(body))
            {
                return false;
            }

            var definitelyAssigned = false;
            return AnalyzeStatementList(body.Body, _functionScope, ref definitelyAssigned)
                && _sawNumericWrite;
        }

        public bool InitializerForwardsUnsafeNumericDependency(Expression expression)
            => InitializerForwardsUnsafeNumericDependency(expression, _functionScope);

        private bool InitializerForwardsUnsafeNumericDependency(
            Expression expression,
            Scope currentScope)
        {
            return expression switch
            {
                Identifier => HasUnsafeNumericVarDependency(expression, currentScope, parent: null),
                ParenthesizedExpression parenthesized
                    => InitializerForwardsUnsafeNumericDependency(parenthesized.Expression, currentScope),
                SequenceExpression sequence when sequence.Expressions.Count > 0
                    => InitializerForwardsUnsafeNumericDependency(sequence.Expressions[^1], currentScope),
                ConditionalExpression conditional
                    => InitializerForwardsUnsafeNumericDependency(conditional.Consequent, currentScope)
                       || InitializerForwardsUnsafeNumericDependency(conditional.Alternate, currentScope),
                LogicalExpression logical
                    => InitializerForwardsUnsafeNumericDependency(logical.Left, currentScope)
                       || InitializerForwardsUnsafeNumericDependency(logical.Right, currentScope),
                AssignmentExpression assignment
                    => InitializerForwardsUnsafeNumericDependency(assignment.Right, currentScope),
                _ => false
            };
        }

        private bool AnalyzeStatementList(
            NodeList<Statement> statements,
            Scope currentScope,
            ref bool definitelyAssigned)
        {
            foreach (var statement in statements)
            {
                if (!AnalyzeStatement(statement, currentScope, ref definitelyAssigned))
                {
                    return false;
                }
            }

            return true;
        }

        private bool AnalyzeStatement(Node statement, Scope currentScope, ref bool definitelyAssigned)
        {
            currentScope = GetScopeForNode(currentScope, statement);

            switch (statement)
            {
                case BlockStatement block:
                    return AnalyzeStatementList(block.Body, currentScope, ref definitelyAssigned);

                case VariableDeclaration declaration:
                    foreach (var declarator in declaration.Declarations)
                    {
                        if (declarator.Id is Identifier id
                            && ResolvesToTarget(currentScope, id.Name))
                        {
                            if (declarator.Init == null)
                            {
                                continue;
                            }

                            if (!AnalyzeExpression(declarator.Init, currentScope, ref definitelyAssigned)
                                || InferNumericValueType(declarator.Init, currentScope) != typeof(double))
                            {
                                return false;
                            }

                            definitelyAssigned = true;
                            _sawNumericWrite = true;
                        }
                        else
                        {
                            if (ContainsTargetReference(declarator.Id, currentScope))
                            {
                                return false;
                            }

                            if (declarator.Init != null
                                && !AnalyzeExpression(declarator.Init, currentScope, ref definitelyAssigned))
                            {
                                return false;
                            }
                        }
                    }
                    return true;

                case ExpressionStatement expressionStatement:
                    return AnalyzeExpression(expressionStatement.Expression, currentScope, ref definitelyAssigned);

                case IfStatement ifStatement:
                    if (!AnalyzeExpression(ifStatement.Test, currentScope, ref definitelyAssigned))
                    {
                        return false;
                    }

                    var thenAssigned = definitelyAssigned;
                    if (!AnalyzeStatement(ifStatement.Consequent, currentScope, ref thenAssigned))
                    {
                        return false;
                    }

                    var elseAssigned = definitelyAssigned;
                    if (ifStatement.Alternate != null
                        && !AnalyzeStatement(ifStatement.Alternate, currentScope, ref elseAssigned))
                    {
                        return false;
                    }

                    definitelyAssigned = thenAssigned && elseAssigned;
                    return true;

                case ForStatement forStatement:
                    if (forStatement.Init is Node init
                        && !AnalyzeStatementOrExpression(init, currentScope, ref definitelyAssigned))
                    {
                        return false;
                    }

                    if (forStatement.Test != null
                        && !AnalyzeExpression(forStatement.Test, currentScope, ref definitelyAssigned))
                    {
                        return false;
                    }

                    var loopAssigned = definitelyAssigned;
                    if (!AnalyzeStatement(forStatement.Body, currentScope, ref loopAssigned)
                        || (forStatement.Update != null
                            && !AnalyzeExpression(forStatement.Update, currentScope, ref loopAssigned)))
                    {
                        return false;
                    }

                    return true;

                case WhileStatement whileStatement:
                    if (!AnalyzeExpression(whileStatement.Test, currentScope, ref definitelyAssigned))
                    {
                        return false;
                    }

                    var whileAssigned = definitelyAssigned;
                    return AnalyzeStatement(whileStatement.Body, currentScope, ref whileAssigned);

                case DoWhileStatement doWhileStatement:
                    var doAssigned = definitelyAssigned;
                    if (!AnalyzeStatement(doWhileStatement.Body, currentScope, ref doAssigned)
                        || !AnalyzeExpression(doWhileStatement.Test, currentScope, ref doAssigned))
                    {
                        return false;
                    }
                    return true;

                case ReturnStatement returnStatement:
                    return returnStatement.Argument == null
                        || AnalyzeExpression(returnStatement.Argument, currentScope, ref definitelyAssigned);

                case ThrowStatement throwStatement:
                    return AnalyzeExpression(throwStatement.Argument, currentScope, ref definitelyAssigned);

                case LabeledStatement labeledStatement:
                    return AnalyzeStatement(labeledStatement.Body, currentScope, ref definitelyAssigned);

                case EmptyStatement:
                case BreakStatement:
                case ContinueStatement:
                case DebuggerStatement:
                    return true;

                case FunctionDeclaration:
                    return !ContainsTargetReference(statement, currentScope);

                case ForInStatement:
                case ForOfStatement:
                case SwitchStatement:
                case TryStatement:
                case WithStatement:
                    return !ContainsTargetReference(statement, currentScope);

                default:
                    return !ContainsTargetReference(statement, currentScope);
            }
        }

        private bool AnalyzeStatementOrExpression(Node node, Scope currentScope, ref bool definitelyAssigned)
            => node is Statement statement
                ? AnalyzeStatement(statement, currentScope, ref definitelyAssigned)
                : node is Expression expression
                    && AnalyzeExpression(expression, currentScope, ref definitelyAssigned);

        private bool AnalyzeExpression(Expression expression, Scope currentScope, ref bool definitelyAssigned)
        {
            currentScope = GetScopeForNode(currentScope, expression);

            switch (expression)
            {
                case Identifier identifier:
                    return !ResolvesToTarget(currentScope, identifier.Name) || definitelyAssigned;

                case AssignmentExpression assignment
                    when assignment.Left is Identifier id
                         && ResolvesToTarget(currentScope, id.Name):
                    if (assignment.Operator != Operator.Assignment && !definitelyAssigned)
                    {
                        return false;
                    }

                    if (!AnalyzeExpression(assignment.Right, currentScope, ref definitelyAssigned)
                        || InferNumericValueType(assignment.Right, currentScope) != typeof(double))
                    {
                        return false;
                    }

                    definitelyAssigned = true;
                    _sawNumericWrite = true;
                    return true;

                case AssignmentExpression assignment:
                    if (assignment.Left is Expression leftExpression)
                    {
                        if (!AnalyzeExpression(leftExpression, currentScope, ref definitelyAssigned))
                        {
                            return false;
                        }
                    }
                    else if (ContainsTargetReference(assignment.Left, currentScope))
                    {
                        return false;
                    }

                    return AnalyzeExpression(assignment.Right, currentScope, ref definitelyAssigned);

                case UpdateExpression update
                    when update.Argument is Identifier id
                         && ResolvesToTarget(currentScope, id.Name):
                    if (!definitelyAssigned)
                    {
                        return false;
                    }
                    _sawNumericWrite = true;
                    return true;

                case SequenceExpression sequence:
                    foreach (var item in sequence.Expressions)
                    {
                        if (!AnalyzeExpression(item, currentScope, ref definitelyAssigned))
                        {
                            return false;
                        }
                    }
                    return true;

                case ConditionalExpression conditional:
                    if (!AnalyzeExpression(conditional.Test, currentScope, ref definitelyAssigned))
                    {
                        return false;
                    }

                    var consequentAssigned = definitelyAssigned;
                    var alternateAssigned = definitelyAssigned;
                    if (!AnalyzeExpression(conditional.Consequent, currentScope, ref consequentAssigned)
                        || !AnalyzeExpression(conditional.Alternate, currentScope, ref alternateAssigned))
                    {
                        return false;
                    }

                    definitelyAssigned = consequentAssigned && alternateAssigned;
                    return true;

                case LogicalExpression logical:
                    if (!AnalyzeExpression(logical.Left, currentScope, ref definitelyAssigned))
                    {
                        return false;
                    }

                    var rightAssigned = definitelyAssigned;
                    return AnalyzeExpression(logical.Right, currentScope, ref rightAssigned);

                case CallExpression { Optional: true } optionalCall:
                    if (!AnalyzeExpression(optionalCall.Callee, currentScope, ref definitelyAssigned))
                    {
                        return false;
                    }

                    var argumentAssigned = definitelyAssigned;
                    foreach (var argument in optionalCall.Arguments)
                    {
                        if (!AnalyzeChildNode(argument, currentScope, ref argumentAssigned))
                        {
                            return false;
                        }
                    }
                    return true;

                case MemberExpression member:
                    if (!AnalyzeExpression(member.Object, currentScope, ref definitelyAssigned))
                    {
                        return false;
                    }

                    if (!member.Computed)
                    {
                        return true;
                    }

                    if (!member.Optional)
                    {
                        return AnalyzeExpression(member.Property, currentScope, ref definitelyAssigned);
                    }

                    var propertyAssigned = definitelyAssigned;
                    return AnalyzeExpression(member.Property, currentScope, ref propertyAssigned);

                case FunctionExpression:
                case ArrowFunctionExpression:
                case ClassExpression:
                    return !ContainsTargetReference(expression, currentScope);

                default:
                    return AnalyzeNodeChildren(expression, currentScope, ref definitelyAssigned);
            }
        }

        private bool AnalyzeNodeChildren(Node node, Scope currentScope, ref bool definitelyAssigned)
        {
            foreach (var child in node.ChildNodes)
            {
                if (!AnalyzeChildNode(child, currentScope, ref definitelyAssigned))
                {
                    return false;
                }
            }

            return true;
        }

        private bool AnalyzeChildNode(Node node, Scope currentScope, ref bool definitelyAssigned)
        {
            if (node is Expression expression)
            {
                return AnalyzeExpression(expression, currentScope, ref definitelyAssigned);
            }

            if (node is Statement statement)
            {
                return AnalyzeStatement(statement, currentScope, ref definitelyAssigned);
            }

            currentScope = GetScopeForNode(currentScope, node);
            if (node is Property property)
            {
                if (property.Computed
                    && property.Key is Expression key
                    && !AnalyzeExpression(key, currentScope, ref definitelyAssigned))
                {
                    return false;
                }

                return property.Value is not Node value
                    || AnalyzeChildNode(value, currentScope, ref definitelyAssigned);
            }

            foreach (var child in node.ChildNodes)
            {
                if (!AnalyzeChildNode(child, currentScope, ref definitelyAssigned))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ContainsTargetReference(Node node, Scope currentScope)
        {
            currentScope = GetScopeForNode(currentScope, node);
            if (node is Identifier identifier && ResolvesToTarget(currentScope, identifier.Name))
            {
                return true;
            }

            foreach (var child in node.ChildNodes)
            {
                if (ContainsTargetReference(child, currentScope))
                {
                    return true;
                }
            }

            return false;
        }

        private Scope GetScopeForNode(Scope currentScope, INode node)
            => currentScope.Children.FirstOrDefault(child => ReferenceEquals(child.AstNode, node))
               ?? currentScope;

        private bool ResolvesToTarget(Scope currentScope, string name)
            => string.Equals(name, _target.Name, StringComparison.Ordinal)
               && ReferenceEquals(TryResolveBinding(currentScope, name), _target);

        private Type? InferExpressionType(Expression expression, Scope currentScope)
        {
            var shadowedProposalNames = new HashSet<string>(StringComparer.Ordinal);
            CollectShadowedProposalNames(
                expression,
                currentScope,
                parent: null,
                shadowedProposalNames);

            Dictionary<string, Type> scopeSafeProposals;
            if (shadowedProposalNames.Count == 0)
            {
                scopeSafeProposals = _proposedClrTypes;
            }
            else
            {
                scopeSafeProposals = _proposedClrTypes
                    .Where(entry => !shadowedProposalNames.Contains(entry.Key))
                    .ToDictionary(entry => entry.Key, entry => entry.Value, StringComparer.Ordinal);
            }

            return _builder.InferExpressionClrType(
                expression,
                currentScope,
                scopeSafeProposals,
                _target,
                _functionScope);
        }

        private Type? InferNumericValueType(Expression expression, Scope currentScope)
        {
            if (InitializerForwardsUnsafeNumericDependency(expression, currentScope))
            {
                return null;
            }

            var inferredType = InferExpressionType(expression, currentScope);
            if (inferredType != null)
            {
                return inferredType;
            }

            return expression is NonUpdateUnaryExpression
                   {
                       Operator: Operator.UnaryNegation or Operator.UnaryPlus
                   } unary
                   && InferNumericValueType(unary.Argument, currentScope) == typeof(double)
                ? typeof(double)
                : null;
        }

        private bool HasUnsafeNumericVarDependency(Node node, Scope currentScope, Node? parent)
        {
            currentScope = GetScopeForNode(currentScope, node);
            if (node is Identifier identifier)
            {
                if (parent is MemberExpression member
                    && !member.Computed
                    && ReferenceEquals(member.Property, identifier))
                {
                    return false;
                }

                if (parent is Property property
                    && !property.Computed
                    && !property.Shorthand
                    && ReferenceEquals(property.Key, identifier))
                {
                    return false;
                }

                var dependency = TryResolveBinding(currentScope, identifier.Name);
                return dependency != null
                       && !ReferenceEquals(dependency, _target)
                       && dependency.Kind == BindingKind.Var
                       && !dependency.DeclaringScope.Parameters.Contains(dependency.Name)
                       && !dependency.CanUseUnboxedLocal;
            }

            foreach (var child in node.ChildNodes)
            {
                if (HasUnsafeNumericVarDependency(child, currentScope, node))
                {
                    return true;
                }
            }

            return false;
        }

        private void CollectShadowedProposalNames(
            Node node,
            Scope currentScope,
            Node? parent,
            HashSet<string> shadowedNames)
        {
            currentScope = GetScopeForNode(currentScope, node);
            if (node is Identifier identifier)
            {
                if (parent is MemberExpression member
                    && !member.Computed
                    && ReferenceEquals(member.Property, identifier))
                {
                    return;
                }

                if (parent is Property property
                    && !property.Computed
                    && !property.Shorthand
                    && ReferenceEquals(property.Key, identifier))
                {
                    return;
                }

                var binding = TryResolveBinding(currentScope, identifier.Name);
                if (_proposedClrTypes.ContainsKey(identifier.Name)
                    && binding != null
                    && !ReferenceEquals(binding.DeclaringScope, _functionScope))
                {
                    shadowedNames.Add(identifier.Name);
                }
                return;
            }

            foreach (var child in node.ChildNodes)
            {
                CollectShadowedProposalNames(child, currentScope, node, shadowedNames);
            }
        }

        private static bool ContainsAbruptLoopControl(Node node)
        {
            if (node is BreakStatement or ContinueStatement)
            {
                return true;
            }

            if (node is FunctionDeclaration or FunctionExpression or ArrowFunctionExpression)
            {
                return false;
            }

            foreach (var child in node.ChildNodes)
            {
                if (ContainsAbruptLoopControl(child))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
