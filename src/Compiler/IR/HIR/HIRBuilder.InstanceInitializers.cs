using Acornima.Ast;
using Jroc.Services;
using Jroc.Services.TwoPhaseCompilation;

namespace Jroc.HIR;

partial class HIRMethodBuilder
{
    private bool TryBuildSuperCallInitializers(out IReadOnlyList<HIRStatement> initializers)
    {
        var statements = new List<HIRStatement>();
        initializers = statements;
        if (!TryGetEnclosingClassScope(_currentScope, out var classScope))
            return false;
        var body = classScope.AstNode switch
        {
            ClassDeclaration declaration => declaration.Body,
            ClassExpression expression => expression.Body,
            _ => null
        };
        if (body == null) return false;
        var owner = GetRegistryClassName(classScope);
        var builder = new HIRMethodBuilder(classScope);
        foreach (var field in body.Body.OfType<PropertyDefinition>().Where(field => !field.Static))
        {
            HIRExpression? value = new HIRLiteralExpression(JavascriptType.Undefined, null);
            if (field.Value != null && (!builder.TryParseExpression(field.Value, out value) || value == null))
                return false;
            if (field.Key is PrivateIdentifier privateName)
            {
                statements.Add(new HIRStoreUserClassInstanceFieldStatement
                {
                    RegistryClassName = owner, FieldName = privateName.Name, IsPrivateField = true,
                    Value = value!, Location = SourceLocation.FromNode(field)
                });
            }
            else if (!field.Computed && field.Key is Identifier name)
            {
                statements.Add(new HIRStoreUserClassInstanceFieldStatement
                {
                    RegistryClassName = owner, FieldName = name.Name, Value = value!, IsPrivateField = false,
                    HasDecorators = field.Decorators.Count > 0,
                    Location = SourceLocation.FromNode(field)
                });
            }
            else
            {
                HIRExpression key = ClassElementNames.TryGetPropertyName(field.Key, field.Computed, out var fieldName)
                    ? new HIRLiteralExpression(JavascriptType.String, fieldName)
                    : new HIRClassComputedFieldKeyExpression(owner, ClassElementNames.GetFieldIdentity(field));
                statements.Add(new HIRExpressionStatement(new HIRDefineClassDataPropertyExpression(
                    new HIRThisExpression(), key, value!, isField: true)
                {
                    DecoratedFieldOwner = field.Decorators.Count > 0 ? owner : null,
                    DecoratedFieldName = fieldName
                }));
            }
        }
        return true;
    }
}
