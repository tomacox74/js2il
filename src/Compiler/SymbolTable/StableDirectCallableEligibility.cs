using Acornima.Ast;
using Jroc.Utilities;

namespace Jroc.SymbolTables;

internal static class StableDirectCallableEligibility
{
    internal static bool TryGetEligibleCall(
        BindingInfo binding,
        CallExpression sourceCall,
        Scope currentScope,
        out Node? initializer,
        out Scope? callableScope,
        out CallableMaterializationReason failureReason)
    {
        initializer = null;
        callableScope = null;
        failureReason = CallableMaterializationReason.None;

        if (sourceCall.Optional)
        {
            failureReason = CallableMaterializationReason.OptionalCall;
            return false;
        }

        if (sourceCall.Arguments.Any(argument => argument is SpreadElement))
        {
            failureReason = CallableMaterializationReason.SpreadCall;
            return false;
        }

        if (binding.Kind != BindingKind.Const
            || binding.DeclarationNode is not VariableDeclarator { Init: { } candidateInitializer }
            || candidateInitializer is not ArrowFunctionExpression and not FunctionExpression)
        {
            failureReason = CallableMaterializationReason.MutableOrHoistedBinding;
            return false;
        }

        initializer = candidateInitializer;
        if (binding.HasNonInitializationWrite)
        {
            failureReason = CallableMaterializationReason.Reassigned;
            return false;
        }

        var root = currentScope;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        if (IsWithinActiveWithEnvironment(root.AstNode, sourceCall))
        {
            failureReason = CallableMaterializationReason.WithEnvironment;
            return false;
        }

        if (!IsBindingDefinitelyInitializedAtCall(
                binding,
                candidateInitializer,
                sourceCall,
                currentScope))
        {
            failureReason = CallableMaterializationReason.InitializationNotProven;
            return false;
        }

        callableScope = FindCallableScope(candidateInitializer, root);
        if (callableScope?.Parent == null)
        {
            failureReason = CallableMaterializationReason.InitializationNotProven;
            return false;
        }

        failureReason = GetCallableIneligibilityReason(
            candidateInitializer,
            callableScope);
        return failureReason == CallableMaterializationReason.None;
    }

    internal static CallableMaterializationReason GetCallableIneligibilityReason(
        Node initializer,
        Scope callableScope)
    {
        if (callableScope.MayUseBoundWithObject)
        {
            return CallableMaterializationReason.WithEnvironment;
        }

        return initializer switch
        {
            ArrowFunctionExpression { Async: true } =>
                CallableMaterializationReason.AsyncOrGenerator,
            ArrowFunctionExpression arrow when UsesLexicalInvocationContext(arrow) =>
                CallableMaterializationReason.InvocationContextRequired,
            FunctionExpression { Async: true } or FunctionExpression { Generator: true } =>
                CallableMaterializationReason.AsyncOrGenerator,
            FunctionExpression { Id: Identifier } =>
                CallableMaterializationReason.NamedFunctionExpression,
            FunctionExpression function
                when ArgumentsObjectSemantics.IsStrictScope(callableScope)
                    || UsesOrdinaryFunctionObjectContext(function) =>
                CallableMaterializationReason.InvocationContextRequired,
            _ => CallableMaterializationReason.None
        };
    }

    internal static Scope? FindCallableScope(Node declarationNode, Scope root)
    {
        if (AstNodesMatch(root.AstNode, declarationNode))
        {
            return root;
        }

        foreach (var child in root.Children)
        {
            var found = FindCallableScope(declarationNode, child);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private static bool IsWithinActiveWithEnvironment(Node root, CallExpression sourceCall)
    {
        if (!TryFindAstPath(root, sourceCall, out var callPath))
        {
            return false;
        }

        for (var index = 0; index + 1 < callPath.Count; index++)
        {
            if (callPath[index] is WithStatement withStatement
                && AstNodesMatch(callPath[index + 1], withStatement.Body))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsBindingDefinitelyInitializedAtCall(
        BindingInfo binding,
        Node initializer,
        CallExpression sourceCall,
        Scope currentScope)
    {
        var root = currentScope;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        if (!TryFindAstPath(root.AstNode, binding.DeclarationNode, out var declarationPath)
            || !TryFindAstPath(root.AstNode, sourceCall, out var callPath))
        {
            return false;
        }

        if (callPath.Any(node => AstNodesMatch(node, initializer)))
        {
            return true;
        }

        for (var pathIndex = 0; pathIndex + 1 < declarationPath.Count; pathIndex++)
        {
            if (declarationPath[pathIndex] is not ForStatement forStatement
                || forStatement.Init == null
                || !AstNodesMatch(declarationPath[pathIndex + 1], forStatement.Init))
            {
                continue;
            }

            var callForIndex = callPath.FindIndex(node => AstNodesMatch(node, forStatement));
            if (callForIndex < 0 || callForIndex + 1 >= callPath.Count)
            {
                return false;
            }

            var callRegion = callPath[callForIndex + 1];
            return !AstNodesMatch(callRegion, forStatement.Init);
        }

        for (var listIndex = declarationPath.Count - 2; listIndex >= 0; listIndex--)
        {
            var statementList = declarationPath[listIndex];
            if (statementList is not Acornima.Ast.Program
                and not BlockStatement
                and not SwitchCase)
            {
                continue;
            }

            var declarationStatement = declarationPath[listIndex + 1];
            if (declarationStatement is not VariableDeclaration)
            {
                continue;
            }

            var statements = statementList switch
            {
                Acornima.Ast.Program program => program.Body.Cast<Node>().ToArray(),
                BlockStatement block => block.Body.Cast<Node>().ToArray(),
                SwitchCase switchCase => switchCase.Consequent.Cast<Node>().ToArray(),
                _ => Array.Empty<Node>()
            };
            var declarationStatementIndex = Array.FindIndex(
                statements,
                statement => AstNodesMatch(statement, declarationStatement));
            var callListIndex = callPath.FindIndex(node => AstNodesMatch(node, statementList));
            if (declarationStatementIndex < 0
                || callListIndex < 0
                || callListIndex + 1 >= callPath.Count)
            {
                return false;
            }

            var callStatement = callPath[callListIndex + 1];
            var callStatementIndex = Array.FindIndex(
                statements,
                statement => AstNodesMatch(statement, callStatement));
            if (callStatementIndex <= declarationStatementIndex)
            {
                return false;
            }

            var outermostDeferredBoundary = callPath
                .Skip(callListIndex + 1)
                .FirstOrDefault(node => node is FunctionDeclaration
                    or FunctionExpression
                    or ArrowFunctionExpression
                    or ClassDeclaration
                    or ClassExpression);
            return outermostDeferredBoundary is not FunctionDeclaration;
        }

        return false;
    }

    private static bool UsesLexicalInvocationContext(ArrowFunctionExpression arrow)
        => VisitOwnCallableSemantics(
            arrow,
            rejectOwnNewTarget: true,
            namedFunctionExpressionName: null);

    private static bool UsesOrdinaryFunctionObjectContext(FunctionExpression function)
        => VisitOwnCallableSemantics(
            function,
            rejectOwnNewTarget: false,
            namedFunctionExpressionName: null);

    private static bool VisitOwnCallableSemantics(
        Node callable,
        bool rejectOwnNewTarget,
        string? namedFunctionExpressionName)
    {
        return Visit(callable, parent: null, isRoot: true, nestedArrowDepth: 0);

        bool Visit(Node node, Node? parent, bool isRoot, int nestedArrowDepth)
        {
            if (!isRoot && node is ClassDeclaration classDeclaration)
            {
                return VisitNestedClassEvaluation(
                    classDeclaration.SuperClass,
                    classDeclaration.Body,
                    nestedArrowDepth);
            }

            if (!isRoot && node is ClassExpression classExpression)
            {
                return VisitNestedClassEvaluation(
                    classExpression.SuperClass,
                    classExpression.Body,
                    nestedArrowDepth);
            }

            if (!isRoot && node is FunctionDeclaration or FunctionExpression)
            {
                return false;
            }

            if (!isRoot && node is ArrowFunctionExpression)
            {
                nestedArrowDepth++;
            }

            if (node is ThisExpression or Super)
            {
                return true;
            }

            if (node is MetaProperty
                && (rejectOwnNewTarget || nestedArrowDepth > 0))
            {
                return true;
            }

            if (node is Identifier identifier
                && IsIdentifierValueReference(identifier, parent)
                && (string.Equals(identifier.Name, "arguments", StringComparison.Ordinal)
                    || namedFunctionExpressionName != null
                        && string.Equals(
                            identifier.Name,
                            namedFunctionExpressionName,
                            StringComparison.Ordinal)))
            {
                return true;
            }

            foreach (var child in node.ChildNodes)
            {
                if (Visit(child, node, isRoot: false, nestedArrowDepth))
                {
                    return true;
                }
            }

            return false;
        }

        bool VisitNestedClassEvaluation(
            Expression? superClass,
            ClassBody body,
            int nestedArrowDepth)
        {
            if (superClass != null
                && Visit(superClass, body, isRoot: false, nestedArrowDepth))
            {
                return true;
            }

            foreach (var element in body.Body)
            {
                Node? computedKey = element switch
                {
                    MethodDefinition { Computed: true } method => method.Key,
                    PropertyDefinition { Computed: true } property => property.Key,
                    _ => null
                };
                if (computedKey != null
                    && Visit(computedKey, element, isRoot: false, nestedArrowDepth))
                {
                    return true;
                }
            }

            return false;
        }
    }

    private static bool IsIdentifierValueReference(Identifier identifier, Node? parent)
    {
        return parent switch
        {
            null => false,
            VariableDeclarator declarator when ReferenceEquals(declarator.Id, identifier) => false,
            FunctionDeclaration function when ReferenceEquals(function.Id, identifier)
                || function.Params.Any(parameter => ReferenceEquals(parameter, identifier)) => false,
            FunctionExpression function when ReferenceEquals(function.Id, identifier)
                || function.Params.Any(parameter => ReferenceEquals(parameter, identifier)) => false,
            ArrowFunctionExpression arrow when arrow.Params.Any(
                parameter => ReferenceEquals(parameter, identifier)) => false,
            MemberExpression member when !member.Computed
                && ReferenceEquals(member.Property, identifier) => false,
            Property property when !property.Computed
                && ReferenceEquals(property.Key, identifier)
                && !ReferenceEquals(property.Value, identifier) => false,
            MethodDefinition method when !method.Computed
                && ReferenceEquals(method.Key, identifier) => false,
            PropertyDefinition property when !property.Computed
                && ReferenceEquals(property.Key, identifier) => false,
            _ => true
        };
    }

    private static bool TryFindAstPath(Node root, Node target, out List<Node> path)
    {
        var candidatePath = new List<Node>();
        if (Find(root))
        {
            path = candidatePath;
            return true;
        }

        path = new List<Node>();
        return false;

        bool Find(Node node)
        {
            candidatePath.Add(node);
            if (AstNodesMatch(node, target))
            {
                return true;
            }

            foreach (var child in node.ChildNodes)
            {
                if (Find(child))
                {
                    return true;
                }
            }

            candidatePath.RemoveAt(candidatePath.Count - 1);
            return false;
        }
    }

    private static bool AstNodesMatch(Node a, Node b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if (a.GetType() != b.GetType())
        {
            return false;
        }

        var aLocation = a.Location;
        var bLocation = b.Location;
        if (aLocation.Start.Line <= 0 || bLocation.Start.Line <= 0)
        {
            return false;
        }

        return aLocation.Start.Line == bLocation.Start.Line
            && aLocation.Start.Column == bLocation.Start.Column
            && aLocation.End.Line == bLocation.End.Line
            && aLocation.End.Column == bLocation.End.Column;
    }
}
