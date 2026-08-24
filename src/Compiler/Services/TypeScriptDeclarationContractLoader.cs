using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Acornima;
using Acornima.Ast;

namespace Jroc.Services;

internal static partial class TypeScriptDeclarationContractLoader
{
    private static readonly HashSet<string> JavaScriptReservedWords =
    [
        "await", "break", "case", "catch", "class", "const", "continue", "debugger",
        "default", "delete", "do", "else", "enum", "export", "extends", "false",
        "finally", "for", "function", "if", "import", "in", "instanceof", "new",
        "null", "return", "super", "switch", "this", "throw", "true", "try",
        "typeof", "var", "void", "while", "with", "yield"
    ];

    public static bool TryCreateContractAst(
        string modulePath,
        IFileSystem fileSystem,
        JavaScriptParser parser,
        out Acornima.Ast.Program contractAst)
    {
        contractAst = null!;
        if (!TryFindPackageRoot(modulePath, fileSystem, out var packageRoot, out var packageJsonPath))
        {
            return false;
        }

        string declarationPath;
        try
        {
            using var packageJson = JsonDocument.Parse(fileSystem.ReadAllText(packageJsonPath));
            if (!TryGetDeclarationPath(packageJson.RootElement, packageRoot, fileSystem, out declarationPath))
            {
                return false;
            }
        }
        catch
        {
            return false;
        }

        var model = new DeclarationModel();
        if (!TryReadDeclarationFile(declarationPath, fileSystem, model, new HashSet<string>(StringComparer.OrdinalIgnoreCase))
            || model.RootMembers.Count == 0
            || !TryBuildContractSource(model, out var source))
        {
            return false;
        }

        try
        {
            contractAst = parser.ParseJavaScript(source, declarationPath);
            return true;
        }
        catch
        {
            contractAst = null!;
            return false;
        }
    }

    private static bool TryFindPackageRoot(
        string modulePath,
        IFileSystem fileSystem,
        out string packageRoot,
        out string packageJsonPath)
    {
        var directory = Path.GetDirectoryName(Path.GetFullPath(modulePath));
        while (!string.IsNullOrWhiteSpace(directory))
        {
            var candidate = Path.Combine(directory, "package.json");
            var parent = Path.GetDirectoryName(directory);
            var isUnscopedPackage = string.Equals(
                Path.GetFileName(parent),
                "node_modules",
                StringComparison.OrdinalIgnoreCase);
            var isScopedPackage = string.Equals(
                Path.GetFileName(Path.GetDirectoryName(parent)),
                "node_modules",
                StringComparison.OrdinalIgnoreCase);
            if (fileSystem.FileExists(candidate) && (isUnscopedPackage || isScopedPackage))
            {
                packageRoot = directory;
                packageJsonPath = candidate;
                return true;
            }

            directory = Path.GetDirectoryName(directory);
        }

        packageRoot = string.Empty;
        packageJsonPath = string.Empty;
        return false;
    }

    private static bool TryGetDeclarationPath(
        JsonElement root,
        string packageRoot,
        IFileSystem fileSystem,
        out string declarationPath)
    {
        foreach (var propertyName in new[] { "types", "typings" })
        {
            if (!root.TryGetProperty(propertyName, out var element)
                || element.ValueKind != JsonValueKind.String
                || string.IsNullOrWhiteSpace(element.GetString()))
            {
                continue;
            }

            var candidate = Path.GetFullPath(Path.Combine(
                packageRoot,
                element.GetString()!.Replace('/', Path.DirectorySeparatorChar)));
            if (fileSystem.FileExists(candidate))
            {
                declarationPath = candidate;
                return true;
            }
        }

        declarationPath = string.Empty;
        return false;
    }

    private static bool TryReadDeclarationFile(
        string declarationPath,
        IFileSystem fileSystem,
        DeclarationModel model,
        HashSet<string> visited)
    {
        declarationPath = Path.GetFullPath(declarationPath);
        if (!visited.Add(declarationPath))
        {
            return true;
        }

        if (!fileSystem.FileExists(declarationPath))
        {
            return false;
        }

        var source = StripComments(fileSystem.ReadAllText(declarationPath));
        var declarationDirectory = Path.GetDirectoryName(declarationPath)!;
        if (InterfaceInheritanceRegex().IsMatch(source))
        {
            return false;
        }

        foreach (Match import in ImportRegex().Matches(source))
        {
            var specifier = import.Groups["path"].Value;
            if (!specifier.StartsWith(".", StringComparison.Ordinal))
            {
                continue;
            }

            var importedPath = Path.GetFullPath(Path.Combine(
                declarationDirectory,
                specifier.Replace('/', Path.DirectorySeparatorChar)));
            if (!Path.HasExtension(importedPath))
            {
                importedPath += ".d.ts";
            }

            if (!TryReadDeclarationFile(importedPath, fileSystem, model, visited))
            {
                return false;
            }
        }

        foreach (Match alias in FunctionTypeAliasRegex().Matches(source))
        {
            if (!TryParseParameters(alias.Groups["parameters"].Value, out var parameters))
            {
                return false;
            }

            var returnType = alias.Groups["return"].Value.Trim();
            if (HasTopLevelIntersection(returnType))
            {
                return false;
            }

            if (!model.FunctionAliases.TryAdd(
                    alias.Groups["name"].Value,
                    new DeclaredFunction(parameters, returnType)))
            {
                return false;
            }
        }

        foreach (var block in ExtractNamedBlocks(source, InterfaceHeaderRegex()))
        {
            if (!TryParseMembers(block.Body, model, out var members)
                || !model.Interfaces.TryAdd(block.Name, members))
            {
                return false;
            }
        }

        foreach (var block in ExtractNamedBlocks(source, DeclareModuleHeaderRegex()))
        {
            foreach (var declaration in SplitMemberDeclarations(block.Body))
            {
                var function = AmbientFunctionRegex().Match(declaration);
                var returnType = function.Success
                    ? function.Groups["return"].Value.Trim()
                    : string.Empty;
                if (!function.Success
                    || !TryParseParameters(function.Groups["parameters"].Value, out var parameters)
                    || HasTopLevelIntersection(returnType)
                    || !TryAddMember(
                        model.RootMembers,
                        new DeclaredMember(
                            function.Groups["name"].Value,
                            parameters,
                            returnType,
                            IsMethod: true)))
                {
                    return false;
                }
            }
        }

        foreach (Match declaration in DeclareConstRegex().Matches(source))
        {
            if (!model.Constants.TryAdd(
                    declaration.Groups["name"].Value,
                    declaration.Groups["type"].Value.Trim()))
            {
                return false;
            }
        }

        var exportAssignment = ExportAssignmentRegex().Match(source);
        if (exportAssignment.Success
            && model.Constants.TryGetValue(exportAssignment.Groups["name"].Value, out var rootType))
        {
            model.RootMembers.Clear();
            if (!TryExpandTypeMembers(rootType, model, out var members))
            {
                return false;
            }

            model.RootMembers.AddRange(members);
        }

        return true;
    }

    private static bool TryParseMembers(
        string body,
        DeclarationModel model,
        out List<DeclaredMember> members)
    {
        members = [];
        foreach (var declaration in SplitMemberDeclarations(body))
        {
            var method = MethodRegex().Match(declaration);
            if (method.Success)
            {
                var returnType = method.Groups["return"].Value.Trim();
                if (method.Groups["optional"].Success
                    || !TryParseParameters(method.Groups["parameters"].Value, out var parameters)
                    || HasTopLevelIntersection(returnType)
                    || !TryAddMember(
                        members,
                        new DeclaredMember(
                            method.Groups["name"].Value,
                            parameters,
                            returnType,
                            IsMethod: true)))
                {
                    return false;
                }

                continue;
            }

            var property = PropertyRegex().Match(declaration);
            if (!property.Success || property.Groups["optional"].Success)
            {
                return false;
            }

            var name = property.Groups["name"].Value;
            var type = property.Groups["type"].Value.Trim();
            if (HasTopLevelIntersection(type))
            {
                return false;
            }

            if (TryParseFunctionType(type, out var function))
            {
                if (!TryAddMember(
                        members,
                        new DeclaredMember(name, function.Parameters, function.ReturnType, IsMethod: true)))
                {
                    return false;
                }
            }
            else if (model.FunctionAliases.TryGetValue(type, out function))
            {
                if (!TryAddMember(
                        members,
                        new DeclaredMember(name, function.Parameters, function.ReturnType, IsMethod: true)))
                {
                    return false;
                }
            }
            else
            {
                if (!TryAddMember(
                        members,
                        new DeclaredMember(name, Array.Empty<DeclaredParameter>(), type, IsMethod: false)))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool TryExpandTypeMembers(
        string typeExpression,
        DeclarationModel model,
        out List<DeclaredMember> expanded)
    {
        expanded = [];
        foreach (var part in SplitTopLevel(typeExpression, '&'))
        {
            var trimmed = part.Trim();
            if (trimmed.StartsWith('{') && trimmed.EndsWith('}'))
            {
                if (!TryParseMembers(trimmed[1..^1], model, out var members))
                {
                    return false;
                }

                foreach (var member in members)
                {
                    if (!TryAddMember(expanded, member))
                    {
                        return false;
                    }
                }
            }
            else if (model.FunctionAliases.ContainsKey(RemoveGenericArguments(trimmed)))
            {
                // Callable object intersections need a callable root contract plus properties.
                // Until that shape is representable, reject instead of dropping the call signature.
                return false;
            }
            else if (model.Interfaces.TryGetValue(RemoveGenericArguments(trimmed), out var members))
            {
                foreach (var member in members)
                {
                    if (!TryAddMember(expanded, member))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        return expanded.Count > 0;
    }

    private static bool TryBuildContractSource(DeclarationModel model, out string source)
    {
        source = string.Empty;
        if (model.RootMembers.Count == 0)
        {
            return false;
        }

        var interfaces = new Dictionary<string, List<DeclaredMember>>(model.Interfaces, StringComparer.Ordinal);
        AddStandardLibraryTypes(interfaces, model.RootMembers);
        if (!HaveUniqueSynthesizedIdentifiers(interfaces.Keys)
            || interfaces.Values.Any(members => !HaveValidParameterIdentifiers(members))
            || !HaveValidParameterIdentifiers(model.RootMembers))
        {
            return false;
        }

        var builder = new StringBuilder();
        foreach (var (name, members) in interfaces.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            builder.Append("class ").Append(ToIdentifier(name)).AppendLine(" {");
            foreach (var member in members)
            {
                AppendClassMember(builder, member);
            }
            builder.AppendLine("}");
        }

        builder.AppendLine("module.exports = {");
        foreach (var member in model.RootMembers)
        {
            builder.Append("  ").Append(ToPropertyName(member.Name)).Append(": ");
            AppendMemberValue(builder, member);
            builder.AppendLine(",");
        }
        builder.AppendLine("};");
        source = builder.ToString();
        return true;
    }

    private static void AddStandardLibraryTypes(
        Dictionary<string, List<DeclaredMember>> interfaces,
        IEnumerable<DeclaredMember> rootMembers)
    {
        var referenced = new Queue<string>(rootMembers.Select(member => RemoveGenericArguments(member.ReturnType)));
        while (referenced.TryDequeue(out var typeName))
        {
            if (interfaces.ContainsKey(typeName))
            {
                foreach (var member in interfaces[typeName])
                {
                    referenced.Enqueue(RemoveGenericArguments(member.ReturnType));
                }
                continue;
            }

            List<DeclaredMember>? members = typeName switch
            {
                "Window" =>
                [
                    new DeclaredMember("document", Array.Empty<DeclaredParameter>(), "Document", IsMethod: false)
                ],
                "Document" =>
                [
                    new DeclaredMember("title", Array.Empty<DeclaredParameter>(), "string", IsMethod: false),
                    new DeclaredMember(
                        "getElementsByTagName",
                        [new DeclaredParameter("qualifiedName", IsOptional: false, IsRest: false)],
                        "HTMLCollection",
                        IsMethod: true)
                ],
                "HTMLCollection" or "HTMLCollectionOf" =>
                [
                    new DeclaredMember("length", Array.Empty<DeclaredParameter>(), "number", IsMethod: false)
                ],
                _ => null
            };

            if (members == null)
            {
                continue;
            }

            interfaces[typeName] = members;
            foreach (var member in members)
            {
                referenced.Enqueue(RemoveGenericArguments(member.ReturnType));
            }
        }
    }

    private static void AppendClassMember(StringBuilder builder, DeclaredMember member)
    {
        if (member.IsMethod)
        {
            builder.Append("  ").Append(ToPropertyName(member.Name)).Append('(')
                .Append(string.Join(", ", member.Parameters.Select(FormatParameter)))
                .Append(") { return ").Append(GetReturnExpression(member.ReturnType)).AppendLine("; }");
            return;
        }

        builder.Append("  get ").Append(ToPropertyName(member.Name))
            .Append("() { return ").Append(GetReturnExpression(member.ReturnType)).AppendLine("; }");
    }

    private static void AppendMemberValue(StringBuilder builder, DeclaredMember member)
    {
        if (member.IsMethod)
        {
            builder.Append("function(")
                .Append(string.Join(", ", member.Parameters.Select(FormatParameter)))
                .Append(") { return ").Append(GetReturnExpression(member.ReturnType)).Append("; }");
        }
        else
        {
            builder.Append(GetReturnExpression(member.ReturnType));
        }
    }

    private static string GetReturnExpression(string type)
    {
        var normalized = RemoveGenericArguments(type.Trim());
        return normalized switch
        {
            "string" => "\"\"",
            "number" => "0",
            "boolean" => "false",
            "void" or "undefined" => "undefined",
            "any" or "unknown" or "object" => "null",
            _ when IsNullableOrUnion(type) => "null",
            _ => $"new {ToIdentifier(normalized)}()"
        };
    }

    private static bool IsNullableOrUnion(string type) => type.Contains('|', StringComparison.Ordinal);

    private static bool HasTopLevelIntersection(string type)
        => SplitTopLevel(type, '&').Skip(1).Any();

    private static bool TryParseFunctionType(string type, out DeclaredFunction function)
    {
        var match = FunctionTypeRegex().Match(type);
        if (match.Success
            && TryParseParameters(match.Groups["parameters"].Value, out var parameters))
        {
            function = new DeclaredFunction(
                parameters,
                match.Groups["return"].Value.Trim());
            return true;
        }

        function = null!;
        return false;
    }

    private static bool TryParseParameters(
        string parameters,
        out IReadOnlyList<DeclaredParameter> parsedParameters)
    {
        var result = new List<DeclaredParameter>();
        var declarations = SplitTopLevel(parameters, ',')
            .Select(parameter => parameter.Trim())
            .Where(parameter => parameter.Length > 0)
            .ToArray();

        for (var index = 0; index < declarations.Length; index++)
        {
            var match = ParameterRegex().Match(declarations[index]);
            if (!match.Success)
            {
                parsedParameters = Array.Empty<DeclaredParameter>();
                return false;
            }

            var isRest = match.Groups["rest"].Success;
            var isOptional = match.Groups["optional"].Success;
            if ((isRest && isOptional) || (isRest && index != declarations.Length - 1))
            {
                parsedParameters = Array.Empty<DeclaredParameter>();
                return false;
            }

            result.Add(new DeclaredParameter(
                match.Groups["name"].Value,
                isOptional,
                isRest));
        }

        parsedParameters = result;
        return true;
    }

    private static IEnumerable<string> SplitMemberDeclarations(string body)
    {
        var start = 0;
        var depth = 0;
        for (var index = 0; index < body.Length; index++)
        {
            depth += body[index] switch
            {
                '(' or '{' or '<' or '[' => 1,
                ')' or '}' or '>' or ']' => -1,
                _ => 0
            };

            if ((body[index] == ';' || body[index] is '\r' or '\n') && depth == 0)
            {
                var declaration = body[start..index].Trim();
                if (declaration.Length > 0)
                {
                    yield return declaration;
                }

                start = index + 1;
            }
        }

        var remaining = body[start..].Trim();
        if (remaining.Length > 0)
        {
            yield return remaining;
        }
    }

    private static bool TryAddMember(List<DeclaredMember> members, DeclaredMember member)
    {
        if (members.Any(existing => string.Equals(existing.Name, member.Name, StringComparison.Ordinal)))
        {
            return false;
        }

        members.Add(member);
        return true;
    }

    private static bool HaveUniqueSynthesizedIdentifiers(IEnumerable<string> names)
    {
        var identifiers = new HashSet<string>(StringComparer.Ordinal);
        foreach (var name in names)
        {
            if (!identifiers.Add(ToIdentifier(name)))
            {
                return false;
            }
        }

        return true;
    }

    private static bool HaveValidParameterIdentifiers(IEnumerable<DeclaredMember> members)
        => members
            .Where(member => member.IsMethod)
            .All(member =>
            {
                var identifiers = new HashSet<string>(StringComparer.Ordinal);
                return member.Parameters.All(parameter =>
                    !JavaScriptReservedWords.Contains(parameter.Name)
                    && identifiers.Add(ToIdentifier(parameter.Name)));
            });

    private static string FormatParameter(DeclaredParameter parameter)
    {
        var identifier = ToIdentifier(parameter.Name);
        if (parameter.IsRest)
        {
            return $"...{identifier}";
        }

        return parameter.IsOptional ? $"{identifier} = undefined" : identifier;
    }

    private static IEnumerable<string> SplitTopLevel(string value, char separator)
    {
        var start = 0;
        var depth = 0;
        for (var index = 0; index < value.Length; index++)
        {
            depth += value[index] switch
            {
                '(' or '{' or '<' or '[' => 1,
                ')' or '}' or '>' or ']' => -1,
                _ => 0
            };
            if (value[index] == separator && depth == 0)
            {
                yield return value[start..index];
                start = index + 1;
            }
        }

        yield return value[start..];
    }

    private static IEnumerable<(string Name, string Body)> ExtractNamedBlocks(string source, Regex headerRegex)
    {
        foreach (Match match in headerRegex.Matches(source))
        {
            var openingBrace = source.IndexOf('{', match.Index + match.Length - 1);
            if (openingBrace < 0)
            {
                continue;
            }

            var depth = 1;
            var index = openingBrace + 1;
            while (index < source.Length && depth > 0)
            {
                depth += source[index] switch
                {
                    '{' => 1,
                    '}' => -1,
                    _ => 0
                };
                index++;
            }

            if (depth == 0)
            {
                yield return (match.Groups["name"].Value, source[(openingBrace + 1)..(index - 1)]);
            }
        }
    }

    private static string RemoveGenericArguments(string type)
    {
        var normalized = type.Trim();
        var genericStart = normalized.IndexOf('<');
        return genericStart >= 0 ? normalized[..genericStart].Trim() : normalized;
    }

    private static string ToIdentifier(string value)
    {
        var builder = new StringBuilder();
        foreach (var character in value)
        {
            builder.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
        }

        if (builder.Length == 0 || char.IsDigit(builder[0]))
        {
            builder.Insert(0, '_');
        }

        return builder.ToString();
    }

    private static string ToPropertyName(string value)
        => Regex.IsMatch(value, "^[A-Za-z_$][A-Za-z0-9_$]*$")
            ? value
            : JsonSerializer.Serialize(value);

    private static string StripComments(string source)
        => CommentRegex().Replace(source, string.Empty);

    private sealed class DeclarationModel
    {
        public Dictionary<string, DeclaredFunction> FunctionAliases { get; } = new(StringComparer.Ordinal);
        public Dictionary<string, List<DeclaredMember>> Interfaces { get; } = new(StringComparer.Ordinal);
        public Dictionary<string, string> Constants { get; } = new(StringComparer.Ordinal);
        public List<DeclaredMember> RootMembers { get; } = [];
    }

    private sealed record DeclaredParameter(string Name, bool IsOptional, bool IsRest);
    private sealed record DeclaredFunction(IReadOnlyList<DeclaredParameter> Parameters, string ReturnType);
    private sealed record DeclaredMember(
        string Name,
        IReadOnlyList<DeclaredParameter> Parameters,
        string ReturnType,
        bool IsMethod);

    [GeneratedRegex(@"import\s+\{[^}]+\}\s+from\s+[""'](?<path>[^""']+)[""']", RegexOptions.Multiline)]
    private static partial Regex ImportRegex();

    [GeneratedRegex(@"(?:export\s+)?type\s+(?<name>[A-Za-z_$][\w$]*)\s*=\s*\((?<parameters>[^)]*)\)\s*=>\s*(?<return>[^\r\n;]+)", RegexOptions.Multiline)]
    private static partial Regex FunctionTypeAliasRegex();

    [GeneratedRegex(@"(?:export\s+)?interface\s+(?<name>[A-Za-z_$][\w$]*)[^{]*\{", RegexOptions.Multiline)]
    private static partial Regex InterfaceHeaderRegex();

    [GeneratedRegex(@"(?:export\s+)?interface\s+[A-Za-z_$][\w$]*(?:\s*<[^>{}]*>)?\s+extends\b", RegexOptions.Multiline)]
    private static partial Regex InterfaceInheritanceRegex();

    [GeneratedRegex(@"declare\s+module\s+[""'](?<name>[^""']+)[""']\s*\{", RegexOptions.Multiline)]
    private static partial Regex DeclareModuleHeaderRegex();

    [GeneratedRegex(@"(?:export\s+)?declare\s+const\s+(?<name>[A-Za-z_$][\w$]*)\s*:\s*(?<type>[^\r\n]+)", RegexOptions.Multiline)]
    private static partial Regex DeclareConstRegex();

    [GeneratedRegex(@"export\s*=\s*(?<name>[A-Za-z_$][\w$]*)", RegexOptions.Multiline)]
    private static partial Regex ExportAssignmentRegex();

    [GeneratedRegex(@"^(?:export\s+)?function\s+(?<name>[A-Za-z_$][\w$]*)\s*\((?<parameters>[^)]*)\)\s*:\s*(?<return>.+)$")]
    private static partial Regex AmbientFunctionRegex();

    [GeneratedRegex(@"^(?:readonly\s+)?(?<name>[A-Za-z_$][\w$]*)(?<optional>\?)?\s*\((?<parameters>[^)]*)\)\s*:\s*(?<return>.+)$")]
    private static partial Regex MethodRegex();

    [GeneratedRegex(@"^(?:readonly\s+)?(?<name>[A-Za-z_$][\w$]*)(?<optional>\?)?\s*:\s*(?<type>.+)$")]
    private static partial Regex PropertyRegex();

    [GeneratedRegex(@"^\((?<parameters>[^)]*)\)\s*=>\s*(?<return>.+)$")]
    private static partial Regex FunctionTypeRegex();

    [GeneratedRegex(@"^(?<rest>\.\.\.)?(?<name>[A-Za-z_$][\w$]*)(?<optional>\?)?\s*:\s*.+$")]
    private static partial Regex ParameterRegex();

    [GeneratedRegex(@"/\*.*?\*/|//[^\r\n]*", RegexOptions.Singleline)]
    private static partial Regex CommentRegex();
}
