using Acornima.Ast;

namespace Jroc.Services.TwoPhaseCompilation;

/// <summary>
/// AST-independent callable facts established while callable syntax is analyzed.
/// Lowering and IL emission consume this value rather than rediscovering facts from syntax nodes.
/// </summary>
public sealed record CallableSemantics
{
    public static CallableSemantics Default { get; } = new();

    public int FunctionLength { get; init; }

    public bool IsAsync { get; init; }

    public bool IsGenerator { get; init; }

    public bool IsConstructable { get; init; }

    public bool HasLegacyCallerArguments { get; init; }

    public bool HasSimpleParameters { get; init; }

    public bool IsNamedFunctionExpression { get; init; }

    public bool IsAnonymousFunctionExpression { get; init; }

    public bool UsesThis { get; init; }

    public bool UsesNewTarget { get; init; }

    public bool UsesSuper { get; init; }

    public bool UsesPrivateNames { get; init; }

    public bool HasNestedArrowLexicalContext { get; init; }

    public bool NestedArrowUsesThis { get; init; }

    public bool NestedArrowUsesNewTarget { get; init; }

    public bool NestedArrowUsesSuper { get; init; }

    public static CallableSemantics FromNode(
        Node? callableNode,
        CallableKind callableKind,
        bool hasRestrictedFunctionProperties = false,
        bool isMethodDefinition = false)
    {
        var callable = callableNode is MethodDefinition method
            ? method.Value
            : callableNode;
        NodeList<Node> parameters = default;
        var isAsync = false;
        var isGenerator = false;
        switch (callable)
        {
            case FunctionDeclaration function:
                parameters = function.Params;
                isAsync = function.Async;
                isGenerator = function.Generator;
                break;
            case FunctionExpression function:
                parameters = function.Params;
                isAsync = function.Async;
                isGenerator = function.Generator;
                break;
            case ArrowFunctionExpression function:
                parameters = function.Params;
                isAsync = function.Async;
                break;
        }
        var requirements = InspectBody(callable);
        var nestedArrowRequirements =
            AnalyzeNestedArrowLexicalContext(callableNode);

        return new CallableSemantics
        {
            FunctionLength = CountExpectedFunctionLength(parameters),
            IsAsync = isAsync,
            IsGenerator = isGenerator,
            IsConstructable = callableKind == CallableKind.ClassConstructor
                || callableKind is CallableKind.FunctionDeclaration
                    or CallableKind.FunctionExpression
                    && !isMethodDefinition
                    && !isAsync
                    && !isGenerator,
            HasLegacyCallerArguments = !hasRestrictedFunctionProperties
                && callableKind is CallableKind.FunctionDeclaration
                    or CallableKind.FunctionExpression
                && !isAsync
                && !isGenerator,
            HasSimpleParameters = parameters.All(parameter => parameter is Identifier),
            IsNamedFunctionExpression = callable is FunctionExpression { Id: not null },
            IsAnonymousFunctionExpression = callable is FunctionExpression { Id: null },
            UsesThis = requirements.UsesThis,
            UsesNewTarget = requirements.UsesNewTarget,
            UsesSuper = requirements.UsesSuper,
            UsesPrivateNames = requirements.UsesPrivateNames,
            NestedArrowUsesThis = nestedArrowRequirements.UsesThis,
            NestedArrowUsesNewTarget =
                nestedArrowRequirements.UsesNewTarget,
            NestedArrowUsesSuper = nestedArrowRequirements.UsesSuper,
            HasNestedArrowLexicalContext =
                nestedArrowRequirements.UsesThis
                || nestedArrowRequirements.UsesNewTarget
                || nestedArrowRequirements.UsesSuper
        };
    }

    private static int CountExpectedFunctionLength(NodeList<Node> parameters)
    {
        var count = 0;
        foreach (var parameter in parameters)
        {
            if (parameter is RestElement or AssignmentPattern)
            {
                break;
            }

            count++;
        }

        return count;
    }

    private static CallableRequirements InspectBody(Node? root)
    {
        var requirements = new CallableRequirements();
        if (root == null)
        {
            return requirements;
        }

        Visit(root, isRoot: true);
        return requirements;

        void Visit(Node node, bool isRoot)
        {
            if (!isRoot && node is FunctionDeclaration
                or FunctionExpression
                or ArrowFunctionExpression
                or ClassDeclaration
                or ClassExpression)
            {
                return;
            }

            requirements = node switch
            {
                ThisExpression => requirements with { UsesThis = true },
                MetaProperty => requirements with { UsesNewTarget = true },
                Super => requirements with { UsesSuper = true },
                PrivateIdentifier => requirements with { UsesPrivateNames = true },
                CallExpression { Callee: Identifier { Name: "eval" } } =>
                    requirements with { UsesThis = true, UsesNewTarget = true },
                _ => requirements
            };

            foreach (var child in node.ChildNodes)
            {
                Visit(child, isRoot: false);
            }
        }
    }

    private static CallableRequirements AnalyzeNestedArrowLexicalContext(
        Node? root)
    {
        if (root == null)
        {
            return default;
        }

        var requirements = new CallableRequirements();
        Visit(root, isRoot: true);
        return requirements;

        void Visit(Node node, bool isRoot)
        {
            if (!isRoot && node is FunctionDeclaration or FunctionExpression)
            {
                return;
            }

            if (!isRoot && node is ArrowFunctionExpression arrow)
            {
                VisitArrowBody(arrow.Body);
                return;
            }

            foreach (var child in node.ChildNodes)
            {
                Visit(child, isRoot: false);
            }
        }

        void VisitArrowBody(Node node)
        {
            if (node is FunctionDeclaration or FunctionExpression)
            {
                return;
            }

            requirements = node switch
            {
                ThisExpression =>
                    requirements with { UsesThis = true },
                MetaProperty =>
                    requirements with { UsesNewTarget = true },
                Super =>
                    requirements with { UsesSuper = true },
                _ => requirements
            };

            foreach (var child in node.ChildNodes)
            {
                VisitArrowBody(child);
            }
        }
    }

    private readonly record struct CallableRequirements(
        bool UsesThis = false,
        bool UsesNewTarget = false,
        bool UsesSuper = false,
        bool UsesPrivateNames = false);
}
