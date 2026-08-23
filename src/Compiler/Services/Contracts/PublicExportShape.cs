using Acornima;
using Acornima.Ast;
using Jroc.SymbolTables;
using Jroc.Utilities;

namespace Jroc.Services.Contracts;

internal enum PublicExportShapeKind
{
    NoExports,
    Known,
    Unknown
}

internal enum PublicExportMemberKind
{
    Named,
    Default,
    Namespace
}

internal enum PublicExportValueKind
{
    None,
    Value,
    CallableOrConstructable
}

internal sealed class PublicModuleExportShape
{
    public PublicModuleExportShape(
        ModuleDefinition module,
        PublicExportShapeKind kind,
        IReadOnlyList<PublicExportMember> members,
        PublicExportValueKind directValueKind,
        Node? directValueSourceNode)
    {
        Module = module ?? throw new ArgumentNullException(nameof(module));
        Kind = kind;
        Members = members ?? throw new ArgumentNullException(nameof(members));
        DirectValueKind = directValueKind;
        DirectValueSourceNode = directValueSourceNode;
    }

    public ModuleDefinition Module { get; }

    public PublicExportShapeKind Kind { get; }

    public IReadOnlyList<PublicExportMember> Members { get; }

    public PublicExportValueKind DirectValueKind { get; }

    public Node? DirectValueSourceNode { get; }

    public bool HasExports => Kind != PublicExportShapeKind.NoExports;
}

internal sealed class PublicExportMember
{
    public PublicExportMember(
        string exportName,
        PublicExportMemberKind kind,
        ModuleDefinition sourceModule,
        Node? sourceNode,
        Type? stableClrType,
        bool hasUnknownSource)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exportName);
        ExportName = exportName;
        Kind = kind;
        SourceModule = sourceModule ?? throw new ArgumentNullException(nameof(sourceModule));
        SourceNode = sourceNode;
        StableClrType = stableClrType;
        HasUnknownSource = hasUnknownSource;
    }

    public string ExportName { get; }

    public PublicExportMemberKind Kind { get; }

    public ModuleDefinition SourceModule { get; }

    public Node? SourceNode { get; }

    public Type? StableClrType { get; }

    public bool HasUnknownSource { get; }
}

internal static class PublicExportShapeAnalyzer
{
    public static IReadOnlyDictionary<string, PublicModuleExportShape> Analyze(Modules modules)
    {
        ArgumentNullException.ThrowIfNull(modules);

        var shapesByPath = new Dictionary<string, PublicModuleExportShape>(StringComparer.OrdinalIgnoreCase);
        var inProgress = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var module in modules._modules.Values.OrderBy(module => module.ModuleId, StringComparer.Ordinal))
        {
            _ = AnalyzeModule(module, modules, shapesByPath, inProgress);
        }

        return modules._modules.Values
            .GroupBy(module => module.ModuleId, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => shapesByPath[group.First().Path],
                StringComparer.Ordinal);
    }

    private static PublicModuleExportShape AnalyzeModule(
        ModuleDefinition module,
        Modules modules,
        Dictionary<string, PublicModuleExportShape> shapesByPath,
        HashSet<string> inProgress)
    {
        if (shapesByPath.TryGetValue(module.Path, out var existing))
        {
            return existing;
        }

        if (!inProgress.Add(module.Path))
        {
            return new PublicModuleExportShape(
                module,
                PublicExportShapeKind.Unknown,
                Array.Empty<PublicExportMember>(),
                PublicExportValueKind.Value,
                directValueSourceNode: null);
        }

        var shape = module.ModuleRecord?.HasStaticModuleSyntax == true
            ? AnalyzeEsModule(module, modules, shapesByPath, inProgress)
            : AnalyzeCommonJsModule(module);

        shapesByPath[module.Path] = shape;
        inProgress.Remove(module.Path);
        return shape;
    }

    private static PublicModuleExportShape AnalyzeEsModule(
        ModuleDefinition module,
        Modules modules,
        Dictionary<string, PublicModuleExportShape> shapesByPath,
        HashSet<string> inProgress)
    {
        var record = module.ModuleRecord;
        if (record == null)
        {
            return NoExports(module);
        }

        var members = new List<PublicExportMember>();
        var seenNames = new HashSet<string>(StringComparer.Ordinal);
        var hasUnknownExports = false;

        foreach (var (exportName, resolved) in record.ResolvedExports.OrderBy(kvp => kvp.Key, StringComparer.Ordinal))
        {
            var kind = string.Equals(exportName, "default", StringComparison.Ordinal)
                ? PublicExportMemberKind.Default
                : resolved.Kind == ModuleExportKind.Namespace
                    ? PublicExportMemberKind.Namespace
                    : PublicExportMemberKind.Named;

            Node? sourceNode = null;
            Type? stableClrType = null;
            var unknownSource = false;
            if (kind != PublicExportMemberKind.Namespace)
            {
                if (!ReferenceEquals(resolved.TargetModule, module))
                {
                    var sourceShape = AnalyzeModule(resolved.TargetModule, modules, shapesByPath, inProgress);
                    var sourceMember = sourceShape.Members.FirstOrDefault(member =>
                        string.Equals(member.ExportName, resolved.BindingName, StringComparison.Ordinal));
                    if (sourceMember != null)
                    {
                        sourceNode = sourceMember.SourceNode;
                        stableClrType = sourceMember.StableClrType;
                        unknownSource = sourceMember.HasUnknownSource
                            || sourceShape.Kind == PublicExportShapeKind.Unknown;
                    }
                }

                sourceNode ??= FindBindingSourceNode(resolved.TargetModule, resolved.BindingName);
                stableClrType ??= sourceNode == null
                    ? TryGetStableClrType(resolved.TargetModule, resolved.BindingName)
                    : TryGetStableClrType(resolved.TargetModule, resolved.BindingName) ?? InferLiteralClrType(sourceNode);
                if (!unknownSource && resolved.TargetModule.ModuleRecord?.HasStaticModuleSyntax != true)
                {
                    unknownSource = AnalyzeCommonJsModule(resolved.TargetModule).Kind == PublicExportShapeKind.Unknown;
                }

                hasUnknownExports |= unknownSource;
            }

            AddOrReplace(
                members,
                seenNames,
                new PublicExportMember(
                    exportName,
                    kind,
                    resolved.TargetModule,
                    sourceNode,
                    stableClrType,
                    unknownSource));
        }

        foreach (var starExport in record.StarExportEntries)
        {
            if (!TryGetDependencyModule(module, starExport.ModuleRequest, modules, out var dependencyModule))
            {
                hasUnknownExports = true;
                continue;
            }

            var dependencyShape = AnalyzeModule(dependencyModule, modules, shapesByPath, inProgress);
            if (dependencyShape.Kind == PublicExportShapeKind.Unknown)
            {
                hasUnknownExports = true;
            }

            foreach (var dependencyMember in dependencyShape.Members
                         .Where(member => !string.Equals(member.ExportName, "default", StringComparison.Ordinal))
                         .OrderBy(member => member.ExportName, StringComparer.Ordinal))
            {
                if (seenNames.Contains(dependencyMember.ExportName))
                {
                    continue;
                }

                members.Add(new PublicExportMember(
                    dependencyMember.ExportName,
                    PublicExportMemberKind.Named,
                    dependencyMember.SourceModule,
                    dependencyMember.SourceNode,
                    dependencyMember.StableClrType,
                    dependencyMember.HasUnknownSource || dependencyShape.Kind == PublicExportShapeKind.Unknown));
                seenNames.Add(dependencyMember.ExportName);
            }
        }

        if (HasContractMemberNameCollision(members))
        {
            return new PublicModuleExportShape(
                module,
                PublicExportShapeKind.Unknown,
                Array.Empty<PublicExportMember>(),
                PublicExportValueKind.Value,
                directValueSourceNode: null);
        }

        if (hasUnknownExports)
        {
            return new PublicModuleExportShape(
                module,
                PublicExportShapeKind.Unknown,
                members,
                PublicExportValueKind.Value,
                directValueSourceNode: null);
        }

        return members.Count == 0
            ? NoExports(module)
            : new PublicModuleExportShape(
                module,
                PublicExportShapeKind.Known,
                members,
                PublicExportValueKind.None,
                directValueSourceNode: null);
    }

    private static PublicModuleExportShape AnalyzeCommonJsModule(ModuleDefinition module)
    {
        var members = new List<PublicExportMember>();
        var seenNames = new HashSet<string>(StringComparer.Ordinal);
        var directValueKind = PublicExportValueKind.None;
        Node? directValueSourceNode = null;
        var unknown = false;
        var sawExportAssignment = false;
        var exportsAliasStillTargetsModuleExports = true;

        foreach (var statement in module.Ast.Body)
        {
            if (statement is ExpressionStatement { Expression: AssignmentExpression topLevelAssignment }
                && TryClassifyCommonJsAssignment(topLevelAssignment, out var assignment))
            {
                sawExportAssignment = true;
                ApplyKnownCommonJsAssignment(
                    module,
                    assignment,
                    members,
                    seenNames,
                    ref directValueKind,
                    ref directValueSourceNode,
                    ref unknown,
                    ref exportsAliasStillTargetsModuleExports);
                continue;
            }

            if (ContainsCommonJsExportAssignment(statement))
            {
                sawExportAssignment = true;
                unknown = true;
            }
        }

        if (!sawExportAssignment)
        {
            return NoExports(module);
        }

        if (HasContractMemberNameCollision(members))
        {
            return new PublicModuleExportShape(
                module,
                PublicExportShapeKind.Unknown,
                Array.Empty<PublicExportMember>(),
                PublicExportValueKind.Value,
                directValueSourceNode);
        }

        if (unknown)
        {
            return new PublicModuleExportShape(
                module,
                PublicExportShapeKind.Unknown,
                members,
                directValueKind == PublicExportValueKind.None ? PublicExportValueKind.Value : directValueKind,
                directValueSourceNode);
        }

        if (directValueKind == PublicExportValueKind.None && members.Count == 0)
        {
            return NoExports(module);
        }

        return new PublicModuleExportShape(
            module,
            PublicExportShapeKind.Known,
            members,
            directValueKind,
            directValueSourceNode);
    }

    private static void ApplyKnownCommonJsAssignment(
        ModuleDefinition module,
        CommonJsExportAssignment assignment,
        List<PublicExportMember> members,
        HashSet<string> seenNames,
        ref PublicExportValueKind directValueKind,
        ref Node? directValueSourceNode,
        ref bool unknown,
        ref bool exportsAliasStillTargetsModuleExports)
    {
        if (assignment.Kind == CommonJsExportAssignmentKind.Unknown)
        {
            unknown = true;
            return;
        }

        if (assignment.Kind == CommonJsExportAssignmentKind.ModuleExportsReplacement)
        {
            exportsAliasStillTargetsModuleExports = false;
            members.Clear();
            seenNames.Clear();
            directValueSourceNode = assignment.ValueNode;
            directValueKind = GetDirectValueKind(module, assignment.ValueNode);

            if (assignment.ValueNode is ObjectExpression objectExpression
                && TryCreateObjectLiteralMembers(module, objectExpression, members, seenNames))
            {
                if (members.Count == 0)
                {
                    directValueKind = PublicExportValueKind.Value;
                    directValueSourceNode = objectExpression;
                }
                else
                {
                    directValueKind = PublicExportValueKind.None;
                    directValueSourceNode = null;
                }
                return;
            }

            if (assignment.ValueNode is ObjectExpression)
            {
                unknown = true;
                directValueKind = PublicExportValueKind.Value;
            }

            return;
        }

        if (assignment.ExportName == null)
        {
            unknown = true;
            return;
        }

        if (assignment.UsesExportsAlias && !exportsAliasStillTargetsModuleExports)
        {
            unknown = true;
            return;
        }

        if (directValueKind != PublicExportValueKind.None)
        {
            if (!assignment.UsesExportsAlias && directValueSourceNode is ObjectExpression)
            {
                directValueKind = PublicExportValueKind.None;
                directValueSourceNode = null;
            }
            else
            {
                unknown = true;
                return;
            }
        }

        AddOrReplace(
            members,
            seenNames,
            new PublicExportMember(
                assignment.ExportName,
                PublicExportMemberKind.Named,
                module,
                assignment.ValueNode,
                InferLiteralClrType(assignment.ValueNode),
                hasUnknownSource: false));
    }

    private static bool TryCreateObjectLiteralMembers(
        ModuleDefinition module,
        ObjectExpression objectExpression,
        List<PublicExportMember> members,
        HashSet<string> seenNames)
    {
        foreach (var propertyNode in objectExpression.Properties)
        {
            if (propertyNode is not Property property)
            {
                return false;
            }

            if (!TryGetPropertyName(property, out var exportName))
            {
                return false;
            }

            AddOrReplace(
                members,
                seenNames,
                new PublicExportMember(
                    exportName,
                    PublicExportMemberKind.Named,
                    module,
                    property,
                    InferLiteralClrType(property.Value),
                    hasUnknownSource: false));
        }

        return true;
    }

    private static bool ContainsCommonJsExportAssignment(Node node)
    {
        var found = false;
        new AstWalker().Visit(node, current =>
        {
            if (!found
                && current is AssignmentExpression assignment
                && TryClassifyCommonJsAssignment(assignment, out _))
            {
                found = true;
            }
        });

        return found;
    }

    private static bool TryClassifyCommonJsAssignment(
        AssignmentExpression assignment,
        out CommonJsExportAssignment result)
    {
        result = default;
        if (assignment.Operator != Operator.Assignment)
        {
            return false;
        }

        if (IsModuleExportsReference(assignment.Left))
        {
            result = new CommonJsExportAssignment(
                CommonJsExportAssignmentKind.ModuleExportsReplacement,
                null,
                assignment.Right);
            return true;
        }

        if (assignment.Left is MemberExpression member
            && TryGetCommonJsExportObject(member.Object, out var usesExportsAlias))
        {
            if (TryGetMemberName(member, out var name))
            {
                result = new CommonJsExportAssignment(
                    CommonJsExportAssignmentKind.NamedProperty,
                    name,
                    assignment.Right,
                    usesExportsAlias);
            }
            else
            {
                result = new CommonJsExportAssignment(
                    CommonJsExportAssignmentKind.Unknown,
                    null,
                    assignment.Right,
                    usesExportsAlias);
            }

            return true;
        }

        return false;
    }

    private static bool TryGetCommonJsExportObject(Expression expression, out bool usesExportsAlias)
    {
        if (expression is Identifier { Name: "exports" })
        {
            usesExportsAlias = true;
            return true;
        }

        if (IsModuleExportsReference(expression))
        {
            usesExportsAlias = false;
            return true;
        }

        usesExportsAlias = false;
        return false;
    }

    private static bool IsModuleExportsReference(Node node)
    {
        if (node is not MemberExpression member)
        {
            return false;
        }

        return member.Object is Identifier { Name: "module" }
            && TryGetMemberName(member, out var propertyName)
            && string.Equals(propertyName, "exports", StringComparison.Ordinal);
    }

    private static bool TryGetMemberName(MemberExpression member, out string name)
    {
        if (!member.Computed && member.Property is Identifier identifier)
        {
            name = identifier.Name;
            return true;
        }

        if (member.Computed
            && member.Property is Literal { Value: string literalName }
            && !string.IsNullOrWhiteSpace(literalName))
        {
            name = literalName;
            return true;
        }

        name = string.Empty;
        return false;
    }

    private static bool TryGetPropertyName(Property property, out string name)
    {
        if (!property.Computed && property.Key is Identifier identifier)
        {
            name = identifier.Name;
            return true;
        }

        if (property.Key is Literal { Value: string literalName }
            && !string.IsNullOrWhiteSpace(literalName))
        {
            name = literalName;
            return true;
        }

        name = string.Empty;
        return false;
    }

    private static Node? FindBindingSourceNode(ModuleDefinition module, string? bindingName)
    {
        if (string.IsNullOrWhiteSpace(bindingName))
        {
            return null;
        }

        foreach (var statement in module.Ast.Body)
        {
            switch (statement)
            {
                case ExportDefaultDeclaration exportDefault
                    when string.Equals(bindingName, "default", StringComparison.Ordinal)
                         || string.Equals(bindingName, EsModuleNames.SyntheticDefault, StringComparison.Ordinal):
                    return exportDefault.Declaration as Node;

                case FunctionDeclaration functionDeclaration
                    when functionDeclaration.Id != null
                         && string.Equals(functionDeclaration.Id.Name, bindingName, StringComparison.Ordinal):
                    return functionDeclaration;

                case ClassDeclaration classDeclaration
                    when classDeclaration.Id != null
                         && string.Equals(classDeclaration.Id.Name, bindingName, StringComparison.Ordinal):
                    return classDeclaration;

                case VariableDeclaration variableDeclaration:
                    foreach (var declarator in variableDeclaration.Declarations)
                    {
                        if (declarator.Id is Identifier identifier
                            && string.Equals(identifier.Name, bindingName, StringComparison.Ordinal))
                        {
                            return declarator.Init;
                        }
                    }

                    break;

                case ExportNamedDeclaration { Declaration: FunctionDeclaration functionDeclaration }
                    when functionDeclaration.Id != null
                         && string.Equals(functionDeclaration.Id.Name, bindingName, StringComparison.Ordinal):
                    return functionDeclaration;

                case ExportNamedDeclaration { Declaration: ClassDeclaration classDeclaration }
                    when classDeclaration.Id != null
                         && string.Equals(classDeclaration.Id.Name, bindingName, StringComparison.Ordinal):
                    return classDeclaration;

                case ExportNamedDeclaration { Declaration: VariableDeclaration variableDeclaration }:
                    foreach (var declarator in variableDeclaration.Declarations)
                    {
                        if (declarator.Id is Identifier identifier
                            && string.Equals(identifier.Name, bindingName, StringComparison.Ordinal))
                        {
                            return declarator.Init;
                        }
                    }

                    break;
            }
        }

        return module.SymbolTable?.Root.Bindings.TryGetValue(bindingName, out var binding) == true
            ? binding.DeclarationNode
            : null;
    }

    private static Type? TryGetStableClrType(ModuleDefinition module, string? bindingName)
    {
        if (string.IsNullOrWhiteSpace(bindingName)
            || module.SymbolTable?.Root is not Jroc.SymbolTables.Scope globalScope
            || !globalScope.Bindings.TryGetValue(bindingName, out var binding)
            || !binding.IsStableType
            || binding.ClrType == null)
        {
            return null;
        }

        return MapClrType(binding.ClrType);
    }

    private static Type? InferLiteralClrType(Node? node)
    {
        return node switch
        {
            Literal { Value: string } => typeof(string),
            Literal { Value: bool } => typeof(bool),
            Literal { Value: int or long or float or double or decimal } => typeof(double),
            Literal { Value: System.Numerics.BigInteger } => typeof(System.Numerics.BigInteger),
            _ => null
        };
    }

    private static Type MapClrType(Type type)
    {
        if (type == typeof(double)
            || type == typeof(bool)
            || type == typeof(string)
            || type == typeof(System.Numerics.BigInteger))
        {
            return type;
        }

        return typeof(object);
    }

    private static PublicExportValueKind GetDirectValueKind(ModuleDefinition module, Node? node)
    {
        if (node is FunctionDeclaration or FunctionExpression or ArrowFunctionExpression or ClassDeclaration or ClassExpression)
        {
            return PublicExportValueKind.CallableOrConstructable;
        }

        if (node is Identifier identifier)
        {
            foreach (var statement in module.Ast.Body)
            {
                if (statement is FunctionDeclaration { Id: not null } functionDeclaration
                    && string.Equals(functionDeclaration.Id.Name, identifier.Name, StringComparison.Ordinal))
                {
                    return PublicExportValueKind.CallableOrConstructable;
                }

                if (statement is ClassDeclaration { Id: not null } classDeclaration
                    && string.Equals(classDeclaration.Id.Name, identifier.Name, StringComparison.Ordinal))
                {
                    return PublicExportValueKind.CallableOrConstructable;
                }

                if (statement is VariableDeclaration variableDeclaration)
                {
                    foreach (var declarator in variableDeclaration.Declarations)
                    {
                        if (declarator.Id is Identifier variableId
                            && string.Equals(variableId.Name, identifier.Name, StringComparison.Ordinal)
                            && declarator.Init is FunctionExpression or ArrowFunctionExpression or ClassExpression)
                        {
                            return PublicExportValueKind.CallableOrConstructable;
                        }
                    }
                }
            }
        }

        return PublicExportValueKind.Value;
    }

    private static void AddOrReplace(
        List<PublicExportMember> members,
        HashSet<string> seenNames,
        PublicExportMember member)
    {
        if (seenNames.Add(member.ExportName))
        {
            members.Add(member);
            return;
        }

        for (var index = 0; index < members.Count; index++)
        {
            if (string.Equals(members[index].ExportName, member.ExportName, StringComparison.Ordinal))
            {
                members[index] = member;
                return;
            }
        }
    }

    private static bool HasContractMemberNameCollision(IEnumerable<PublicExportMember> members)
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var member in members)
        {
            if (!names.Add(ToPascalCase(member.ExportName)))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryGetDependencyModule(
        ModuleDefinition module,
        string? moduleRequest,
        Modules modules,
        out ModuleDefinition dependencyModule)
    {
        dependencyModule = null!;
        if (string.IsNullOrWhiteSpace(moduleRequest))
        {
            return false;
        }

        var resolvedPath = module.ModuleRecord?.RequestedModules
            .FirstOrDefault(request => string.Equals(request.Specifier, moduleRequest, StringComparison.Ordinal))
            ?.ResolvedPath
            ?? module.Dependencies
                .FirstOrDefault(dependency => string.Equals(dependency.Request, moduleRequest, StringComparison.Ordinal))
                ?.ResolvedPath;

        if (resolvedPath != null && modules._modules.TryGetValue(resolvedPath, out var resolvedDependencyModule))
        {
            dependencyModule = resolvedDependencyModule;
            return true;
        }

        return false;
    }

    private static PublicModuleExportShape NoExports(ModuleDefinition module) =>
        new(
            module,
            PublicExportShapeKind.NoExports,
            Array.Empty<PublicExportMember>(),
            PublicExportValueKind.None,
            directValueSourceNode: null);

    private static string ToPascalCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var parts = new List<string>();
        var builder = new System.Text.StringBuilder();

        foreach (var character in value)
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
            else if (builder.Length > 0)
            {
                parts.Add(builder.ToString());
                builder.Clear();
            }
        }

        if (builder.Length > 0)
        {
            parts.Add(builder.ToString());
        }

        if (parts.Count == 0)
        {
            return value;
        }

        builder.Clear();
        foreach (var part in parts)
        {
            builder.Append(char.ToUpperInvariant(part[0]));
            if (part.Length > 1)
            {
                builder.Append(part[1..]);
            }
        }

        return builder.ToString();
    }

    private readonly record struct CommonJsExportAssignment(
        CommonJsExportAssignmentKind Kind,
        string? ExportName,
        Node? ValueNode,
        bool UsesExportsAlias = false);

    private enum CommonJsExportAssignmentKind
    {
        ModuleExportsReplacement,
        NamedProperty,
        Unknown
    }
}
