using System.Reflection.Metadata;
using Acornima.Ast;
using Js2IL.Services.VariableBindings;
using Js2IL.SymbolTables;
using Js2IL.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Services.Nesting;

internal sealed class ScopeNestingPlanner
{
    private readonly ScopeMetadataRegistry _scopeMetadataRegistry;
    private readonly FunctionTypeMetadataRegistry _functionTypeMetadataRegistry;
    private readonly AnonymousCallableTypeMetadataRegistry _anonymousCallableTypeMetadataRegistry;
    private readonly IServiceProvider _serviceProvider;

    public ScopeNestingPlanner(
        ScopeMetadataRegistry scopeMetadataRegistry,
        FunctionTypeMetadataRegistry functionTypeMetadataRegistry,
        AnonymousCallableTypeMetadataRegistry anonymousCallableTypeMetadataRegistry,
        IServiceProvider serviceProvider)
    {
        _scopeMetadataRegistry = scopeMetadataRegistry ?? throw new ArgumentNullException(nameof(scopeMetadataRegistry));
        _functionTypeMetadataRegistry = functionTypeMetadataRegistry ?? throw new ArgumentNullException(nameof(functionTypeMetadataRegistry));
        _anonymousCallableTypeMetadataRegistry = anonymousCallableTypeMetadataRegistry ?? throw new ArgumentNullException(nameof(anonymousCallableTypeMetadataRegistry));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IReadOnlyList<(TypeDefinitionHandle Nested, TypeDefinitionHandle Enclosing)> PlanModuleNesting(
        string moduleName,
        TypeDefinitionHandle moduleTypeHandle,
        Scope globalScope)
    {
        if (string.IsNullOrWhiteSpace(moduleName)) throw new ArgumentException("Module name is required.", nameof(moduleName));
        if (moduleTypeHandle.IsNil) throw new ArgumentException("Module type handle must not be nil.", nameof(moduleTypeHandle));
        if (globalScope == null) throw new ArgumentNullException(nameof(globalScope));

        var relationships = new List<(TypeDefinitionHandle Nested, TypeDefinitionHandle Enclosing)>();

        // Global scope nests under the module type: Modules.<ModuleName>.Scope
        var globalKey = ScopeNaming.GetRegistryScopeName(globalScope);
        var globalScopeTypeHandle = GetScopeTypeHandleOrThrow(globalKey);
        relationships.Add((globalScopeTypeHandle, moduleTypeHandle));

        // Child scopes nest under their parent scope.
        var visited = new HashSet<string>(StringComparer.Ordinal);
        CollectScopeNestingRelationships(moduleName, globalScope, enclosingForParentScopeType: moduleTypeHandle, relationships, visited);

        return relationships.Distinct().ToArray();
    }

    private TypeDefinitionHandle GetScopeTypeHandleOrThrow(string scopeKey)
    {
        if (_scopeMetadataRegistry.TryGetScopeTypeHandle(scopeKey, out var typeHandle))
        {
            return typeHandle;
        }

        throw new InvalidOperationException($"Expected scope type handle to be registered for scope '{scopeKey}', but none was found.");
    }

    private void CollectScopeNestingRelationships(
        string moduleName,
        Scope parentScope,
        TypeDefinitionHandle enclosingForParentScopeType,
        List<(TypeDefinitionHandle Nested, TypeDefinitionHandle Enclosing)> relationships,
        HashSet<string> visited)
    {
        var parentScopeKey = ScopeNaming.GetRegistryScopeName(parentScope);
        var parentTypeHandle = GetScopeTypeHandleOrThrow(parentScopeKey);

        foreach (var child in parentScope.Children)
        {
            var childKey = ScopeNaming.GetRegistryScopeName(child);
            if (!visited.Add(childKey))
            {
                continue;
            }

            var childTypeHandle = GetScopeTypeHandleOrThrow(childKey);

            // Special case: for-loop loop-head lexical environments (CreatePerIterationEnvironment).
            // These scopes should be nested as siblings of the parent scope type, under the same
            // enclosing owner type (module/callable/class), so they are constructible from that
            // owner while remaining NestedPrivate.
            if (child.Kind == ScopeKind.Block &&
                (child.Name.StartsWith("For_", StringComparison.Ordinal) ||
                 child.Name.StartsWith("ForIn_", StringComparison.Ordinal) ||
                 child.Name.StartsWith("ForOf_", StringComparison.Ordinal)))
            {
                relationships.Add((childTypeHandle, enclosingForParentScopeType));
                CollectScopeNestingRelationships(moduleName, child, enclosingForParentScopeType, relationships, visited);
                continue;
            }

            // Special case: class member scopes (ctor/get/set/method bodies) should be nested under the
            // runtime class TypeDef as siblings of the class scope type.
            //
            // Desired layout:
            //   <ClassName>
            //     + Scope           (class lexical scope)
            //     + Scope_ctor      (constructor body scope)
            //     + Scope_get_x     (getter body scope)
            //     + Scope_set_x     (setter body scope)
            //     + Scope_method    (method body scope)
            //
            // rather than nesting those member scopes under <ClassName>+Scope.
            if (parentScope.Kind == ScopeKind.Class
                && child.Kind == ScopeKind.Function
                && !string.IsNullOrWhiteSpace(child.DotNetTypeName)
                && child.DotNetTypeName.StartsWith("Scope", StringComparison.Ordinal))
            {
                var classRegistry = _serviceProvider.GetService<ClassRegistry>();
                if (classRegistry != null)
                {
                    var registryClassName = ScopeNaming.GetRegistryClassName(parentScope);
                    if (classRegistry.TryGet(registryClassName, out var classTypeHandle) && !classTypeHandle.IsNil)
                    {
                        relationships.Add((childTypeHandle, classTypeHandle));
                        CollectScopeNestingRelationships(moduleName, child, classTypeHandle, relationships, visited);
                        continue;
                    }
                }

                // Fallback: if we can't resolve the runtime class TypeDef, keep legacy nesting.
                relationships.Add((childTypeHandle, parentTypeHandle));
                CollectScopeNestingRelationships(moduleName, child, parentTypeHandle, relationships, visited);
                continue;
            }

            // Special case: class scopes should be nested under their runtime class TypeDef.
            // This keeps Modules.<Module>.Scope free of nested types and produces a clean layout:
            //   Modules.<Module>+<ClassName>
            //     + Scope
            // rather than nesting the class scope under the parent scope type.
            if (child.Kind == ScopeKind.Class)
            {
                var classRegistry = _serviceProvider.GetService<ClassRegistry>();
                if (classRegistry != null)
                {
                    var registryClassName = ScopeNaming.GetRegistryClassName(child);
                    if (classRegistry.TryGet(registryClassName, out var classTypeHandle) && !classTypeHandle.IsNil)
                    {
                        relationships.Add((childTypeHandle, classTypeHandle));
                        CollectScopeNestingRelationships(moduleName, child, classTypeHandle, relationships, visited);
                        continue;
                    }
                }

                // Fallback: if we can't resolve the runtime class TypeDef, keep legacy nesting.
                relationships.Add((childTypeHandle, parentTypeHandle));
                CollectScopeNestingRelationships(moduleName, child, parentTypeHandle, relationships, visited);
                continue;
            }

            // Special case: function declarations have a dedicated callable owner type.
            // We nest the function's *scope type* under that owner type so IL reads as:
            //   .class ... Modules.<Module>.<FunctionName>/Scope
            // rather than:
            //   .class ... Modules.<Module>.Scope/<FunctionName>
            if (child.Kind == ScopeKind.Function && child.AstNode is FunctionDeclaration)
            {
                var declaringScopeName = child.Parent != null
                    ? Utilities.ScopeNaming.GetRegistryScopeName(child.Parent)
                    : moduleName;

                if (_functionTypeMetadataRegistry.TryGet(moduleName, declaringScopeName, child.Name, out var ownerTypeHandle) && !ownerTypeHandle.IsNil)
                {
                    relationships.Add((childTypeHandle, ownerTypeHandle));
                    CollectScopeNestingRelationships(moduleName, child, ownerTypeHandle, relationships, visited);
                }
                else
                {
                    // Fallback: if the owner type isn't available for some reason, nest under the parent scope.
                    relationships.Add((childTypeHandle, parentTypeHandle));
                    CollectScopeNestingRelationships(moduleName, child, parentTypeHandle, relationships, visited);
                }
            }
            else if (child.Kind == ScopeKind.Function && child.AstNode is ArrowFunctionExpression)
            {
                // Arrow function scope type (named "Scope") nests under its anonymous callable owner type.
                if (_anonymousCallableTypeMetadataRegistry.TryGetOwnerTypeHandle(moduleName, parentScopeKey, child.Name, out var arrowOwner) && !arrowOwner.IsNil)
                {
                    relationships.Add((childTypeHandle, arrowOwner));
                    CollectScopeNestingRelationships(moduleName, child, arrowOwner, relationships, visited);
                }
                else
                {
                    relationships.Add((childTypeHandle, parentTypeHandle));
                    CollectScopeNestingRelationships(moduleName, child, parentTypeHandle, relationships, visited);
                }
            }
            else if (child.Kind == ScopeKind.Function && child.AstNode is FunctionExpression)
            {
                // Function-expression scope type (named "Scope") nests under its anonymous callable owner type.
                if (_anonymousCallableTypeMetadataRegistry.TryGetOwnerTypeHandle(moduleName, parentScopeKey, child.Name, out var funcExprOwner) && !funcExprOwner.IsNil)
                {
                    relationships.Add((childTypeHandle, funcExprOwner));
                    CollectScopeNestingRelationships(moduleName, child, funcExprOwner, relationships, visited);
                }
                else
                {
                    relationships.Add((childTypeHandle, parentTypeHandle));
                    CollectScopeNestingRelationships(moduleName, child, parentTypeHandle, relationships, visited);
                }
            }
            else
            {
                relationships.Add((childTypeHandle, parentTypeHandle));
                CollectScopeNestingRelationships(moduleName, child, parentTypeHandle, relationships, visited);
            }
        }
    }

}
