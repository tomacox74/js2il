using Acornima;
using Acornima.Ast;
using Jroc.Services.ILGenerators;

namespace Jroc.Services;

/// <summary>
/// Lowers undecorated, noncomputed public auto-accessors to ordinary private
/// fields and class accessors before scope discovery and callable planning.
/// </summary>
internal sealed class ClassAutoAccessorNormalizer : AstRewriter
{
    private readonly HashSet<string> _privateNames = new(StringComparer.Ordinal);
    private int _nextBackingField;

    public static Acornima.Ast.Program Normalize(Acornima.Ast.Program program)
    {
        var normalizer = new ClassAutoAccessorNormalizer();
        normalizer.CollectPrivateNames(program);
        return (Acornima.Ast.Program)normalizer.Visit(program)!;
    }

    private void CollectPrivateNames(Node node)
    {
        if (node is PrivateIdentifier identifier)
        {
            _privateNames.Add(identifier.Name);
        }
        foreach (var child in node.ChildNodes)
        {
            CollectPrivateNames(child);
        }
    }

    protected override object VisitClassBody(ClassBody node)
    {
        var visited = (ClassBody)base.VisitClassBody(node)!;
        if (!visited.Body.Any(element => element is AccessorProperty))
        {
            return visited;
        }

        var elements = new List<Node>();
        foreach (var element in visited.Body)
        {
            if (element is not AccessorProperty accessor)
            {
                elements.Add(element);
                continue;
            }

            if (accessor.Decorators.Count != 0)
            {
                ILEmitHelpers.ThrowNotSupported("Decorated auto-accessors are not supported.", accessor);
            }
            if (accessor.Computed)
            {
                ILEmitHelpers.ThrowNotSupported("Computed auto-accessors are not supported; their property key must be evaluated exactly once.", accessor);
            }
            if (accessor.Key is PrivateIdentifier)
            {
                ILEmitHelpers.ThrowNotSupported("Private auto-accessors are not supported.", accessor);
            }
            if (!ClassElementNames.TryGetPropertyName(accessor.Key, computed: false, out _))
            {
                ILEmitHelpers.ThrowNotSupported("Unsupported auto-accessor property name.", accessor);
            }

            string name;
            do
            {
                name = $"__jroc_auto_accessor_{_nextBackingField++}";
            }
            while (!_privateNames.Add(name));

            var backingKey = new PrivateIdentifier(name) { Range = accessor.Key.Range, Location = accessor.Key.Location };
            var decorators = NodeList.Empty<Decorator>();
            // The backing field stays at the original position in the initializer
            // sequence; methods have no initialization-time side effects.
            elements.Add(new PropertyDefinition(backingKey, accessor.Value, false, accessor.Static, decorators)
            {
                Range = accessor.Range,
                Location = accessor.Location
            });
            elements.Add(CreateAccessor(accessor, backingKey, setter: false));
            elements.Add(CreateAccessor(accessor, backingKey, setter: true));
        }
        var body = NodeList.From(elements);
        return visited.UpdateWith(body);
    }

    private static MethodDefinition CreateAccessor(AccessorProperty accessor, PrivateIdentifier backingKey, bool setter)
    {
        var receiver = new ThisExpression { Range = accessor.Key.Range, Location = accessor.Key.Location };
        var member = new MemberExpression(receiver, backingKey, false, false)
        {
            Range = accessor.Range,
            Location = accessor.Location
        };
        var parameter = new Identifier("value") { Range = accessor.Key.Range, Location = accessor.Key.Location };
        Statement statement;
        if (setter)
        {
            var assignment = new AssignmentExpression("=", member, parameter)
            {
                Range = accessor.Range,
                Location = accessor.Location
            };
            statement = new NonSpecialExpressionStatement(assignment) { Range = accessor.Range, Location = accessor.Location };
        }
        else
        {
            statement = new ReturnStatement(member) { Range = accessor.Range, Location = accessor.Location };
        }
        var statements = NodeList.From(new[] { statement });
        var body = new FunctionBody(statements, strict: true) { Range = accessor.Range, Location = accessor.Location };
        var parameters = setter ? NodeList.From<Node>(new[] { parameter }) : NodeList.Empty<Node>();
        var function = new FunctionExpression(null, parameters, body, generator: false, async: false)
        {
            Range = accessor.Range,
            Location = accessor.Location
        };
        var decorators = NodeList.Empty<Decorator>();
        return new MethodDefinition(setter ? PropertyKind.Set : PropertyKind.Get,
            accessor.Key, function, false, accessor.Static, decorators)
        {
            Range = accessor.Range,
            Location = accessor.Location
        };
    }
}
