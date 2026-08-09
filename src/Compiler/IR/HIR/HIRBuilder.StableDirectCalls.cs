using Acornima.Ast;
using Jroc.SymbolTables;
using Jroc.Utilities;

namespace Jroc.HIR;

partial class HIRMethodBuilder
{
    private bool TryCreateStableDirectCallableTarget(
        CallExpression sourceCall,
        HIRExpression callee,
        out HIRStableDirectCallableTarget? target)
    {
        target = null;

        if (callee is not HIRVariableExpression { Name: var symbol }
            || sourceCall.Arguments.Any(argument => argument is SpreadElement)
            || symbol.BindingInfo.Kind != BindingKind.Const
            || symbol.BindingInfo.HasNonInitializationWrite
            || symbol.BindingInfo.DeclarationNode is not VariableDeclarator { Init: { } initializer }
            || initializer is not ArrowFunctionExpression and not FunctionExpression
            || !IsStableBindingDefinitelyInitializedAtCall(
                symbol.BindingInfo,
                initializer,
                sourceCall))
        {
            return false;
        }

        var root = _currentScope;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        var callableScope = FindStableCallableScope(initializer, root);
        if (callableScope?.Parent == null
            || StableCallableRequiresFunctionObjectInvocation(initializer, callableScope))
        {
            return false;
        }

        var callableId = initializer switch
        {
            ArrowFunctionExpression arrow =>
                CreateArrowFunctionCallableId(callableScope, arrow),
            FunctionExpression function =>
                CreateFunctionExpressionCallableId(
                    callableScope,
                    function,
                    GetDeclaringScopeName(callableScope.Parent),
                    ArgumentsObjectSemantics.IsStrictScope(callableScope)),
            _ => throw new InvalidOperationException(
                $"Unexpected stable callable initializer {initializer.Type}.")
        };

        target = new HIRStableDirectCallableTarget(callableId, callableScope);
        return true;
    }

    private bool IsStableBindingDefinitelyInitializedAtCall(
        BindingInfo binding,
        Node initializer,
        CallExpression sourceCall)
    {
        var root = _currentScope;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        if (!TryFindAstPath(root.AstNode, binding.DeclarationNode, out var declarationPath)
            || !TryFindAstPath(root.AstNode, sourceCall, out var callPath))
        {
            return false;
        }

        // A recursive call in the initializer's callable body cannot execute while the
        // function object itself is being created, so the const binding is initialized.
        if (callPath.Any(node => AstNodesMatch(node, initializer)))
        {
            return true;
        }

        for (var listIndex = declarationPath.Count - 2; listIndex >= 0; listIndex--)
        {
            var statementList = declarationPath[listIndex];
            if (statementList is not Acornima.Ast.Program and not BlockStatement)
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

            // Function declarations are instantiated at block entry and can be invoked by
            // earlier statements. A non-hoisted callable/class boundary in the later
            // statement is safe because its value cannot exist before that statement runs.
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

    private static bool StableCallableRequiresFunctionObjectInvocation(
        Node initializer,
        Scope callableScope)
    {
        if (callableScope.MayUseBoundWithObject)
        {
            return true;
        }

        return initializer switch
        {
            ArrowFunctionExpression arrow =>
                arrow.Async || UsesLexicalInvocationContext(arrow),
            FunctionExpression function =>
                function.Async
                || function.Generator
                || ArgumentsObjectSemantics.IsStrictScope(callableScope)
                || UsesOrdinaryFunctionObjectContext(function),
            _ => true
        };
    }

    private static bool UsesLexicalInvocationContext(ArrowFunctionExpression arrow)
        => VisitOwnCallableSemantics(
            arrow,
            rejectOwnNewTarget: true,
            namedFunctionExpressionName: null);

    private static bool UsesOrdinaryFunctionObjectContext(FunctionExpression function)
    {
        if (VisitOwnCallableSemantics(
                function,
                rejectOwnNewTarget: false,
                namedFunctionExpressionName: (function.Id as Identifier)?.Name))
        {
            return true;
        }

        return function.Id is Identifier identifier
            && ContainsIdentifierValueReference(function, identifier.Name);
    }

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

            if (!isRoot
                && node is FunctionDeclaration
                    or FunctionExpression)
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

    private static bool ContainsIdentifierValueReference(Node root, string name)
    {
        return Visit(root, parent: null);

        bool Visit(Node node, Node? parent)
        {
            if (node is Identifier identifier
                && string.Equals(identifier.Name, name, StringComparison.Ordinal)
                && IsIdentifierValueReference(identifier, parent))
            {
                return true;
            }

            foreach (var child in node.ChildNodes)
            {
                if (Visit(child, node))
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

    private static Scope? FindStableCallableScope(Node declarationNode, Scope root)
    {
        if (AstNodesMatch(root.AstNode, declarationNode))
        {
            return root;
        }

        foreach (var child in root.Children)
        {
            var found = FindStableCallableScope(declarationNode, child);
            if (found != null)
            {
                return found;
            }
        }

        return null;
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
