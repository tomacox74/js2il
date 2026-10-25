using Acornima.Ast;
using Jroc.SymbolTables;

namespace Jroc.HIR;

partial class HIRMethodBuilder
{
    private bool TryBuildClassElementDecoratorPreparation(Node element, string owner, out HIRExpression? preparation)
    {
        preparation = null;
        var decorators = element switch
        {
            MethodDefinition method => method.Decorators.Select(decorator => decorator.Expression),
            PropertyDefinition field => field.Decorators.Select(decorator => decorator.Expression),
            _ => Enumerable.Empty<Expression>()
        };
        var values = new List<HIRExpression>();
        foreach (var decorator in decorators)
        {
            if (!TryParseExpression(decorator, out var value) || value == null) return false;
            values.Add(value);
        }
        if (values.Count == 0) return true;
        var (key, computed, isStatic, kind) = element switch
        {
            MethodDefinition method => (method.Key, method.Computed, method.Static,
                method.Kind == PropertyKind.Get ? "getter" : method.Kind == PropertyKind.Set ? "setter" : "method"),
            PropertyDefinition field => (field.Key, field.Computed, field.Static, "field"),
            _ => throw new InvalidOperationException("Decorators require a class element")
        };
        if (!Jroc.Services.ClassElementNames.TryGetPropertyName(key, computed, out var name) || name == null)
            return false;
        preparation = new HIRClassElementDecoratorsExpression(owner, name, kind, isStatic, values);
        return true;
    }

    private bool TryBuildDecoratedClass(
        Node classNode,
        Scope classScope,
        out HIRExpression? expression)
    {
        expression = null;
        var decorators = classNode switch
        {
            ClassDeclaration declaration => declaration.Decorators,
            ClassExpression classExpression => classExpression.Decorators,
            _ => default
        };
        var decoratorExpressions = new List<HIRExpression>();
        foreach (var decorator in decorators)
        {
            // Decorator expressions run in the enclosing environment, before heritage.
            if (!TryParseExpression(decorator.Expression, out var parsed) || parsed == null)
            {
                return false;
            }
            decoratorExpressions.Add(parsed);
        }

        if (!TryBuildClassStaticInitializationStatements(
                classNode, classScope, out var statements, out var bindingIndex))
        {
            return false;
        }

        var heritage = classNode switch
        {
            ClassDeclaration declaration => declaration.SuperClass,
            ClassExpression classExpression => classExpression.SuperClass,
            _ => null
        };
        HIRExpression? superClass = null;
        if (heritage != null)
        {
            var heritageBuilder = new HIRMethodBuilder(classScope) { _classHeritageScope = classScope };
            if (!heritageBuilder.TryParseExpressionForPrologue(
                    UnwrapClassHeritageExpression(heritage)!, out superClass)
                || superClass == null)
            {
                return false;
            }
        }

        var name = classNode switch
        {
            ClassDeclaration { Id: Identifier id } => id.Name,
            ClassExpression { Id: Identifier id } => id.Name,
            ClassDeclaration => "default",
            _ => null
        };
        var registryName = GetRegistryClassName(classScope);
        if (name != null && classScope.Bindings.TryGetValue(name, out var binding))
        {
            statements.Insert(bindingIndex, new HIRVariableDeclaration(
                new Symbol(binding),
                new HIRInitializedUserClassTypeExpression(
                    registryName, classScope, [], superClass,
                    isClassExpression: true, explicitName: name)));
            bindingIndex++;
        }
        statements.Insert(bindingIndex, new HIRClassDecorationApplicationStatement(registryName));

        expression = new HIRInitializedUserClassTypeExpression(
            registryName, classScope, statements, superClass,
            isClassExpression: true, explicitName: name,
            decorators: decoratorExpressions);
        return true;
    }
}
