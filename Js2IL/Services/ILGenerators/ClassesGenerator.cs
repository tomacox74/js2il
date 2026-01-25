using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Acornima.Ast;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.SymbolTables;
using Js2IL.Utilities;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Services.ILGenerators
{
    internal sealed class ClassesGenerator
    {
        private readonly MetadataBuilder _metadata;
        private readonly BaseClassLibraryReferences _bcl;
        private readonly ClassRegistry _classRegistry;
        private readonly NestedTypeRelationshipRegistry _nestedTypeRelationshipRegistry;
        private readonly string _moduleName;

        private SymbolTable? _activeSymbolTable;

        private readonly IServiceProvider _serviceProvider;

        public ClassesGenerator(
            IServiceProvider serviceProvider,
            MetadataBuilder metadata,
            BaseClassLibraryReferences bcl,
            ClassRegistry classRegistry,
            NestedTypeRelationshipRegistry nestedTypeRelationshipRegistry,
            string moduleName)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _bcl = bcl ?? throw new ArgumentNullException(nameof(bcl));
            _classRegistry = classRegistry ?? throw new ArgumentNullException(nameof(classRegistry));
            _nestedTypeRelationshipRegistry = nestedTypeRelationshipRegistry ?? throw new ArgumentNullException(nameof(nestedTypeRelationshipRegistry));
            _moduleName = moduleName ?? throw new ArgumentNullException(nameof(moduleName));
        }

        private static int CountRequiredParameters(in NodeList<Node> parameters)
        {
            // Parameters with defaults (AssignmentPattern) are optional.
            return parameters.Count(p => p is not AssignmentPattern);
        }

        private string GetRegistryClassName(Scope classScope)
        {
            if (classScope == null) throw new ArgumentNullException(nameof(classScope));
            var ns = classScope.DotNetNamespace ?? "Classes";
            var name = classScope.DotNetTypeName ?? classScope.Name;
            return $"{ns}.{name}";
        }

        private string GetClassDeclaringScopeName(Scope classScope)
        {
            if (classScope == null) throw new ArgumentNullException(nameof(classScope));
            var parent = classScope.Parent;
            if (parent == null || parent.Kind == ScopeKind.Global)
            {
                return _moduleName;
            }

            // CallableDiscovery uses module-qualified scope paths (e.g., "<module>/<function>").
            // Scope.GetQualifiedName() omits the global scope, so prefix with module.
            return $"{_moduleName}/{parent.GetQualifiedName()}";
        }

        public void DeclareClasses(SymbolTable table)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));

            _activeSymbolTable = table;

            // When compiling a JS "module" (a single entry file), the generated module root type is
            // Modules.<ModuleName>. Top-level JS classes should be nested under that module type.
            // This keeps reflection names module-qualified (Modules.<Module>+<ClassName>) and avoids
            // emitting a parallel top-level namespace like Classes.<ModuleName>.<ClassName>.
            var moduleTypeRegistry = _serviceProvider.GetRequiredService<ModuleTypeMetadataRegistry>();
            moduleTypeRegistry.TryGet(_moduleName, out var moduleTypeHandle);

            EmitClassesRecursiveTwoPhase(table.Root, moduleTypeHandle);

            _activeSymbolTable = null;
        }

        private bool TryResolveEnclosingTypeForClass(Scope classScope, TypeDefinitionHandle moduleTypeHandle, out TypeDefinitionHandle parentType)
        {
            parentType = default;

            // Walk up to find the nearest enclosing callable owner type. If none, fall back to
            // module root type for global/module-scope classes.
            for (var current = classScope.Parent; current != null; current = current.Parent)
            {
                if (current.Kind == ScopeKind.Global)
                {
                    if (!moduleTypeHandle.IsNil)
                    {
                        parentType = moduleTypeHandle;
                        return true;
                    }

                    return false;
                }

                if (current.Kind != ScopeKind.Function)
                {
                    continue;
                }

                // Function declarations have a stable owner type: Modules.<Module>+<FunctionName>
                if (current.AstNode is FunctionDeclaration)
                {
                    var functionTypes = _serviceProvider.GetRequiredService<FunctionTypeMetadataRegistry>();
                    var fd = (FunctionDeclaration)current.AstNode;
                    var functionName = (fd.Id as Identifier)?.Name ?? current.Name;

                    if (!functionTypes.TryGet(_moduleName, functionName, out var ownerType) || ownerType.IsNil)
                    {
                        // Owner type not yet declared (planned pipeline may defer this). Declare it now so
                        // nested classes can be emitted under it.
                        var callables = _serviceProvider.GetRequiredService<CallableRegistry>();
                        if (!callables.TryGetCallableIdForAstNode(fd, out var callable))
                        {
                            return false;
                        }

                        if (!callables.TryGetDeclaredToken(callable, out var tok) || tok.Kind != HandleKind.MethodDefinition)
                        {
                            return false;
                        }

                        var expected = (MethodDefinitionHandle)tok;
                        if (expected.IsNil)
                        {
                            return false;
                        }

                        // Owner type will be nested under the module TypeDef later via NestedClass rows.
                        var tb = new TypeBuilder(_metadata, string.Empty, functionName);
                        ownerType = tb.AddTypeDefinition(
                            TypeAttributes.NestedPublic | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                            _bcl.ObjectType,
                            firstFieldOverride: null,
                            firstMethodOverride: expected);

                        functionTypes.Add(_moduleName, functionName, ownerType);
                    }

                    parentType = ownerType;
                    return true;
                }

                // Arrow functions and function expressions have an anonymous owner type recorded in the registry.
                if (current.AstNode is ArrowFunctionExpression or FunctionExpression)
                {
                    var anon = _serviceProvider.GetService<AnonymousCallableTypeMetadataRegistry>();
                    if (anon != null)
                    {
                        var declaringScope = current.Parent != null
                            ? ScopeNaming.GetRegistryScopeName(current.Parent)
                            : _moduleName;

                        if (anon.TryGetOwnerTypeHandle(_moduleName, declaringScope, current.Name, out var anonOwner)
                            && !anonOwner.IsNil)
                        {
                            parentType = anonOwner;
                            return true;
                        }
                    }

                    // If we can't resolve the anonymous owner type, keep legacy behavior.
                    return false;
                }

                // Some function-like scopes (e.g., class methods) do not have separate owner types.
                // Keep legacy behavior in that case.
                return false;
            }

            return false;
        }

        private void EmitClassesRecursiveTwoPhase(Scope scope, TypeDefinitionHandle moduleTypeHandle)
        {
            foreach (var child in scope.Children)
            {
                if (child.Kind == ScopeKind.Class && child.AstNode is ClassDeclaration cdecl)
                {
                    // Nest module-scope classes under Modules.<ModuleName>, and function-local classes
                    // under the enclosing function owner type.
                    var parentType = TryResolveEnclosingTypeForClass(child, moduleTypeHandle, out var resolved)
                        ? resolved
                        : default;

                    DeclareClassTwoPhase(child, cdecl, parentType);
                }

                EmitClassesRecursiveTwoPhase(child, moduleTypeHandle);
            }
        }

        private TypeDefinitionHandle DeclareClassTwoPhase(Scope classScope, ClassDeclaration cdecl, TypeDefinitionHandle parentType)
        {
            var callableRegistry = _serviceProvider.GetRequiredService<CallableRegistry>();

            var jsClassName = classScope.Name;
            var registryClassName = GetRegistryClassName(classScope);
            var declaringScopeName = GetClassDeclaringScopeName(classScope);

            // Idempotency: classes may be predeclared in an earlier pass to stabilize TypeDef ordering.
            // If already declared, skip emitting a duplicate TypeDef/fields.
            if (_classRegistry.TryGet(registryClassName, out var existingTypeDef) && !existingTypeDef.IsNil)
            {
                return existingTypeDef;
            }

            // Registry key can remain stable (based on DotNetNamespace/DotNetTypeName), but the emitted
            // CLR type layout should reflect nesting when parentType is provided.
            var ns = parentType.IsNil
                ? (classScope.DotNetNamespace ?? "Classes")
                : string.Empty;
            var name = classScope.DotNetTypeName ?? classScope.Name;
            var tb = new TypeBuilder(_metadata, ns, name);

            var typeAttrs = parentType.IsNil
                ? TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit
                : TypeAttributes.NestedPublic | TypeAttributes.Class | TypeAttributes.BeforeFieldInit;

            // Determine whether this class needs to capture parent scopes.
            bool classNeedsParentScopes = classScope.ReferencesParentScopeVariables;
            if (!classNeedsParentScopes)
            {
                var ctor = cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>()
                    .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");
                if (ctor?.Value is FunctionExpression ctorExpr)
                {
                    classNeedsParentScopes = ShouldCreateMethodScopeInstance(ctorExpr, classScope);
                }

                if (!classNeedsParentScopes)
                {
                    foreach (var method in cdecl.Body.Body
                        .OfType<Acornima.Ast.MethodDefinition>()
                        .Where(m => m.Value is FunctionExpression && (m.Key as Identifier)?.Name != "constructor"))
                    {
                        var funcExpr = (FunctionExpression)method.Value;
                        if (ShouldCreateMethodScopeInstance(funcExpr, classScope))
                        {
                            classNeedsParentScopes = true;
                            break;
                        }
                    }
                }
            }

            // If this is a derived class (extends), ensure it can pass scopes to the base class constructor
            // when the base class requires it.
            EntityHandle baseTypeHandle = _bcl.ObjectType;
            if (cdecl.SuperClass is Identifier superId)
            {
                var baseScope = FindClassScope(classScope, superId.Name);
                if (baseScope != null)
                {
                    // Resolve CLR base type.
                    var baseRegistryName = GetRegistryClassName(baseScope);
                    if (_classRegistry.TryGet(baseRegistryName, out var baseTypeDef) && !baseTypeDef.IsNil)
                    {
                        baseTypeHandle = baseTypeDef;
                    }

                    // Propagate base scope requirements.
                    bool baseNeedsParentScopes = baseScope.ReferencesParentScopeVariables;
                    if (!baseNeedsParentScopes && baseScope.AstNode is ClassDeclaration baseDecl)
                    {
                        var baseCtor = baseDecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>()
                            .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");
                        if (baseCtor?.Value is FunctionExpression baseCtorExpr)
                        {
                            baseNeedsParentScopes = ShouldCreateMethodScopeInstance(baseCtorExpr, baseScope);
                        }

                        if (!baseNeedsParentScopes)
                        {
                            foreach (var method in baseDecl.Body.Body
                                .OfType<Acornima.Ast.MethodDefinition>()
                                .Where(m => m.Value is FunctionExpression && (m.Key as Identifier)?.Name != "constructor"))
                            {
                                var funcExpr = (FunctionExpression)method.Value;
                                if (ShouldCreateMethodScopeInstance(funcExpr, baseScope))
                                {
                                    baseNeedsParentScopes = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (baseNeedsParentScopes)
                    {
                        classNeedsParentScopes = true;
                    }
                }
            }

            // Fields (instance + static)
            var declaredFieldNames = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);

            bool TryGetStableInstanceFieldUserClassTypeHandle(string fieldName, out EntityHandle typeHandle)
            {
                typeHandle = default;

                if (!classScope.StableInstanceFieldUserClassNames.TryGetValue(fieldName, out var jsClassName) ||
                    string.IsNullOrEmpty(jsClassName))
                {
                    return false;
                }

                var foundClassScope = FindClassScope(classScope, jsClassName);
                if (foundClassScope == null)
                {
                    return false;
                }

                var foundRegistryClassName = GetRegistryClassName(foundClassScope);
                if (!_classRegistry.TryGet(foundRegistryClassName, out var foundTypeDef))
                {
                    return false;
                }

                typeHandle = foundTypeDef;
                return true;
            }

            void EncodeFieldType(BlobEncoder encoder, Type? clrType, string fieldName)
            {
                var t = encoder.Field().Type();
                if (clrType == typeof(double))
                {
                    t.Double();
                }
                else if (clrType == typeof(bool))
                {
                    t.Boolean();
                }
                else if (clrType == typeof(string))
                {
                    t.String();
                }
                else if (clrType?.Namespace?.StartsWith("JavaScriptRuntime", StringComparison.Ordinal) == true)
                {
                    var typeRef = _bcl.TypeReferenceRegistry.GetOrAdd(clrType);
                    t.Type(typeRef, isValueType: clrType.IsValueType);
                }
                else if (TryGetStableInstanceFieldUserClassTypeHandle(fieldName, out var userClassTypeHandle))
                {
                    t.Type(userClassTypeHandle, isValueType: false);
                }
                else
                {
                    t.Object();
                }
            }

            Type? TryGetStableInstanceFieldClrType(string fieldName)
            {
                return classScope.StableInstanceFieldClrTypes.TryGetValue(fieldName, out var t) ? t : null;
            }

            foreach (var pdef in cdecl.Body.Body.OfType<Acornima.Ast.PropertyDefinition>())
            {
                if (pdef.Key is Acornima.Ast.PrivateIdentifier priv)
                {
                    var pname = priv.Name;
                    var emittedName = ManglePrivateFieldName(pname);
                    var clrType = pdef.Static ? typeof(object) : TryGetStableInstanceFieldClrType(pname);
                    var fSig = new BlobBuilder();
                    EncodeFieldType(new BlobEncoder(fSig), clrType, pname);
                    var fSigHandle = _metadata.GetOrAddBlob(fSig);
                    if (pdef.Static)
                    {
                        var fh = tb.AddFieldDefinition(FieldAttributes.Private | FieldAttributes.Static, emittedName, fSigHandle);
                        _classRegistry.RegisterStaticField(registryClassName, pname, fh);
                        _classRegistry.RegisterStaticFieldClrType(registryClassName, pname, typeof(object));
                    }
                    else
                    {
                        var fh = tb.AddFieldDefinition(FieldAttributes.Private, emittedName, fSigHandle);
                        _classRegistry.RegisterPrivateField(registryClassName, pname, fh);
                        _classRegistry.RegisterPrivateFieldClrType(registryClassName, pname, clrType ?? typeof(object));

                        if (TryGetStableInstanceFieldUserClassTypeHandle(pname, out var userFieldTypeHandle))
                        {
                            _classRegistry.RegisterPrivateFieldTypeHandle(registryClassName, pname, userFieldTypeHandle);
                        }
                    }
                    declaredFieldNames.Add(pname);
                }
                else if (pdef.Key is Identifier pid)
                {
                    var clrType = pdef.Static ? typeof(object) : TryGetStableInstanceFieldClrType(pid.Name);
                    var fSig = new BlobBuilder();
                    EncodeFieldType(new BlobEncoder(fSig), clrType, pid.Name);
                    var fSigHandle = _metadata.GetOrAddBlob(fSig);
                    if (pdef.Static)
                    {
                        var fh = tb.AddFieldDefinition(FieldAttributes.Public | FieldAttributes.Static, pid.Name, fSigHandle);
                        _classRegistry.RegisterStaticField(registryClassName, pid.Name, fh);
                        _classRegistry.RegisterStaticFieldClrType(registryClassName, pid.Name, typeof(object));
                    }
                    else
                    {
                        var fh = tb.AddFieldDefinition(FieldAttributes.Public, pid.Name, fSigHandle);
                        _classRegistry.RegisterField(registryClassName, pid.Name, fh);
                        _classRegistry.RegisterFieldClrType(registryClassName, pid.Name, clrType ?? typeof(object));

                        if (TryGetStableInstanceFieldUserClassTypeHandle(pid.Name, out var userFieldTypeHandle))
                        {
                            _classRegistry.RegisterFieldTypeHandle(registryClassName, pid.Name, userFieldTypeHandle);
                        }
                    }
                    declaredFieldNames.Add(pid.Name);
                }
            }

            if (classNeedsParentScopes)
            {
                var scopesSig = new BlobBuilder();
                new BlobEncoder(scopesSig).Field().Type().SZArray().Object();
                var scopesSigHandle = _metadata.GetOrAddBlob(scopesSig);
                var scopesField = tb.AddFieldDefinition(FieldAttributes.Private, "_scopes", scopesSigHandle);
                _classRegistry.RegisterPrivateField(registryClassName, "_scopes", scopesField);
            }

            // PL5.4a: If the constructor explicitly returns a value, stash it in a hidden field
            // so the `new` call site can decide whether to override the constructed instance.
            // Only needed when the constructor contains `return <expr>` (not bare `return;`).
            static bool ConstructorHasReturnValue(Node? node)
            {
                if (node == null) return false;

                switch (node)
                {
                    case ReturnStatement rs:
                        return rs.Argument != null;

                    // Don't count returns inside nested functions/arrow functions/classes.
                    case FunctionDeclaration:
                    case FunctionExpression:
                    case ArrowFunctionExpression:
                    case ClassDeclaration:
                    case ClassExpression:
                        return false;

                    case BlockStatement b:
                        return b.Body.Any(ConstructorHasReturnValue);
                    case ExpressionStatement es:
                        return ConstructorHasReturnValue(es.Expression);
                    case IfStatement ifs:
                        return ConstructorHasReturnValue(ifs.Test)
                            || ConstructorHasReturnValue(ifs.Consequent)
                            || ConstructorHasReturnValue(ifs.Alternate);
                    case ForStatement fs:
                        return ConstructorHasReturnValue(fs.Init)
                            || ConstructorHasReturnValue(fs.Test)
                            || ConstructorHasReturnValue(fs.Update)
                            || ConstructorHasReturnValue(fs.Body);
                    case ForInStatement fis:
                        return ConstructorHasReturnValue(fis.Left)
                            || ConstructorHasReturnValue(fis.Right)
                            || ConstructorHasReturnValue(fis.Body);
                    case ForOfStatement fos:
                        return ConstructorHasReturnValue(fos.Left)
                            || ConstructorHasReturnValue(fos.Right)
                            || ConstructorHasReturnValue(fos.Body);
                    case WhileStatement ws:
                        return ConstructorHasReturnValue(ws.Test) || ConstructorHasReturnValue(ws.Body);
                    case DoWhileStatement dws:
                        return ConstructorHasReturnValue(dws.Body) || ConstructorHasReturnValue(dws.Test);
                    case SwitchStatement ss:
                        return ConstructorHasReturnValue(ss.Discriminant) || ss.Cases.Any(ConstructorHasReturnValue);
                    case SwitchCase sc:
                        return ConstructorHasReturnValue(sc.Test) || sc.Consequent.Any(ConstructorHasReturnValue);
                    case TryStatement ts:
                        return ConstructorHasReturnValue(ts.Block)
                            || ConstructorHasReturnValue(ts.Handler)
                            || ConstructorHasReturnValue(ts.Finalizer);
                    case CatchClause cc:
                        return ConstructorHasReturnValue(cc.Body);
                }

                return false;
            }

            var ctorMemberForReturn = cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>()
                .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");

            if (ctorMemberForReturn?.Value is FunctionExpression ctorFuncForReturn
                && ctorFuncForReturn.Body is BlockStatement ctorBody
                && ConstructorHasReturnValue(ctorBody))
            {
                var fSig = new BlobBuilder();
                new BlobEncoder(fSig).Field().Type().Object();
                var fSigHandle = _metadata.GetOrAddBlob(fSig);

                // This field is read from the `new C()` call site (outside the class) to implement
                // JavaScript constructor return override semantics. It must be accessible from
                // other types in the same generated assembly.
                var fh = tb.AddFieldDefinition(FieldAttributes.Assembly, "__js2il_ctorReturn", fSigHandle);
                _classRegistry.RegisterPrivateField(registryClassName, "__js2il_ctorReturn", fh);
            }

            // Pre-scan methods for this.<prop> assignments to declare backing fields.
            System.Collections.Generic.IEnumerable<string> FindThisAssignedProps(Acornima.Ast.Node node)
            {
                if (node is null) yield break;
                switch (node)
                {
                    case Acornima.Ast.AssignmentExpression a when a.Left is Acornima.Ast.MemberExpression me && me.Object is Acornima.Ast.ThisExpression && !me.Computed && me.Property is Identifier pid:
                        yield return pid.Name;
                        break;
                    case Acornima.Ast.BlockStatement b:
                        foreach (var s in b.Body)
                            foreach (var n in FindThisAssignedProps(s)) yield return n;
                        break;
                    case Acornima.Ast.ExpressionStatement es:
                        foreach (var n in FindThisAssignedProps(es.Expression)) yield return n;
                        break;
                    case Acornima.Ast.IfStatement ifs:
                        foreach (var n in FindThisAssignedProps(ifs.Consequent)) yield return n;
                        if (ifs.Alternate != null) foreach (var n in FindThisAssignedProps(ifs.Alternate)) yield return n;
                        break;
                    case Acornima.Ast.ForStatement fs:
                        if (fs.Init is Acornima.Ast.Node init) foreach (var n in FindThisAssignedProps(init)) yield return n;
                        if (fs.Test is Acornima.Ast.Node test) foreach (var n in FindThisAssignedProps(test)) yield return n;
                        if (fs.Update is Acornima.Ast.Node upd) foreach (var n in FindThisAssignedProps(upd)) yield return n;
                        foreach (var n in FindThisAssignedProps(fs.Body)) yield return n;
                        break;
                    case Acornima.Ast.CallExpression ce:
                        foreach (var arg in ce.Arguments)
                            foreach (var n in FindThisAssignedProps(arg)) yield return n;
                        break;
                    case Acornima.Ast.MemberExpression mem:
                        if (mem.Object is Acornima.Ast.Node on) foreach (var n in FindThisAssignedProps(on)) yield return n;
                        if (mem.Property is Acornima.Ast.Node pn) foreach (var n in FindThisAssignedProps(pn)) yield return n;
                        break;
                    case Acornima.Ast.AssignmentExpression a2:
                        if (a2.Right is Acornima.Ast.Node rn) foreach (var n in FindThisAssignedProps(rn)) yield return n;
                        break;
                }
            }

            foreach (var m in cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>())
            {
                if (m.Value is FunctionExpression fe && fe.Body is BlockStatement body)
                {
                    foreach (var prop in FindThisAssignedProps(body).Distinct(StringComparer.Ordinal))
                    {
                        if (!declaredFieldNames.Contains(prop))
                        {
                            var clrType = TryGetStableInstanceFieldClrType(prop);
                            var fSig = new BlobBuilder();
                            EncodeFieldType(new BlobEncoder(fSig), clrType, prop);
                            var fSigHandle = _metadata.GetOrAddBlob(fSig);
                            var fh = tb.AddFieldDefinition(FieldAttributes.Public, prop, fSigHandle);
                            _classRegistry.RegisterField(registryClassName, prop, fh);
                            _classRegistry.RegisterFieldClrType(registryClassName, prop, clrType ?? typeof(object));

                            if (TryGetStableInstanceFieldUserClassTypeHandle(prop, out var userFieldTypeHandle))
                            {
                                _classRegistry.RegisterFieldTypeHandle(registryClassName, prop, userFieldTypeHandle);
                            }
                            declaredFieldNames.Add(prop);
                        }
                    }
                }
            }

            // Identify the constructor callable and use its preallocated MethodDef as the first method.
            var ctorMember = cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>()
                .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");

            CallableId ctorCallable;
            if (ctorMember != null)
            {
                if (!callableRegistry.TryGetCallableIdForAstNode(ctorMember, out ctorCallable))
                {
                    throw new InvalidOperationException($"[Classes] Missing CallableId for class constructor AST node: {jsClassName}");
                }
            }
            else
            {
                // CallableDiscovery uses ClassBody as the synthetic ctor AST node.
                if (!callableRegistry.TryGetCallableIdForAstNode(cdecl.Body, out ctorCallable))
                {
                    throw new InvalidOperationException($"[Classes] Missing CallableId for synthetic class constructor: {jsClassName}");
                }
            }

            if (!callableRegistry.TryGetDeclaredToken(ctorCallable, out var ctorToken) || ctorToken.Kind != HandleKind.MethodDefinition)
            {
                throw new InvalidOperationException($"[Classes] Missing declared MethodDef token for class constructor: {ctorCallable.DisplayName}");
            }
            var ctorMethodDef = (MethodDefinitionHandle)ctorToken;

            // Declare the TypeDef now. Use the preallocated ctor MethodDef as the MethodList pointer.
            var typeHandle = tb.AddTypeDefinition(typeAttrs, baseTypeHandle, firstFieldOverride: null, firstMethodOverride: ctorMethodDef);
            if (!parentType.IsNil)
            {
                _nestedTypeRelationshipRegistry.Add(typeHandle, parentType);
            }
            _classRegistry.Register(registryClassName, typeHandle);

            // Register constructor signature for call-site validation.
            var ctorParamCount = (ctorMember?.Value as FunctionExpression)?.Params.Count ?? 0;
            var ctorTotalParamCount = classNeedsParentScopes ? ctorParamCount + 1 : ctorParamCount;
            var ctorSig = MethodBuilder.BuildMethodSignature(
                _metadata,
                isInstance: true,
                paramCount: ctorTotalParamCount,
                hasScopesParam: classNeedsParentScopes,
                returnsVoid: true);
            var ctorMinUserParams = ctorMember?.Value is FunctionExpression ctorFuncExpr
                ? CountRequiredParameters(ctorFuncExpr.Params)
                : 0;

            // Keep ClassRegistry min/max parameter counts as JS argument counts (excluding scopes),
            // consistent with instance method registration and with LIR lowering/IL emission.
            _classRegistry.RegisterConstructor(registryClassName, ctorMethodDef, ctorSig, classNeedsParentScopes, ctorMinUserParams, ctorParamCount);

            // Register instance methods (tokens are preallocated in Phase 1, bodies emitted in Phase 2).
            foreach (var member in cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>().Where(m => m.Key is Identifier))
            {
                var memberName = ((Identifier)member.Key).Name;
                if (string.Equals(memberName, "constructor", StringComparison.Ordinal))
                {
                    continue;
                }
                if (member.Static)
                {
                    continue;
                }
                if (!callableRegistry.TryGetCallableIdForAstNode(member, out var methodCallable))
                {
                    continue;
                }
                if (!callableRegistry.TryGetDeclaredToken(methodCallable, out var methodToken) || methodToken.Kind != HandleKind.MethodDefinition)
                {
                    continue;
                }

                var funcExpr = member.Value as FunctionExpression;
                var jsParamCount = funcExpr?.Params.Count ?? 0;
                var minParams = funcExpr != null ? CountRequiredParameters(funcExpr.Params) : 0;

                // Determine an inferred stable return type (if any) from the method scope.
                // Generator methods always return a GeneratorObject (boxed as object), regardless of
                // yielded/returned JS value types.
                Scope? methodScope = null;
                if (funcExpr != null)
                {
                    // Prefer the symbol table indexer (handles cases where the scope is attached
                    // to MethodDefinition rather than FunctionExpression).
                    methodScope = _activeSymbolTable?.FindScopeByAstNode(funcExpr)
                        ?? _activeSymbolTable?.FindScopeByAstNode(member);

                    // Fallback: scan direct children.
                    if (methodScope == null)
                    {
                        foreach (var child in classScope.Children)
                        {
                            if (child.Kind != ScopeKind.Function)
                            {
                                continue;
                            }

                            if (ReferenceEquals(child.AstNode, funcExpr) || ReferenceEquals(child.AstNode, member))
                            {
                                methodScope = child;
                                break;
                            }
                        }
                    }

                    // Last-resort fallback by name under the class scope.
                    if (methodScope == null && member.Key is Identifier ident)
                    {
                        methodScope = classScope.Children.FirstOrDefault(s =>
                            s.Kind == ScopeKind.Function
                            && string.Equals(s.Name, ident.Name, StringComparison.Ordinal));
                    }
                }

                bool isGeneratorMethod = (funcExpr != null && funcExpr.Body != null && ContainsYieldExpression(funcExpr.Body, funcExpr))
                    || methodScope?.IsGenerator == true
                    || funcExpr?.Generator == true;
                bool isAsyncMethod = methodScope?.IsAsync == true || funcExpr?.Async == true;

                Type returnClrType = isGeneratorMethod
                    ? typeof(object)
                    : (methodScope?.StableReturnClrType ?? typeof(object));

                var clrName = member.Kind switch
                {
                    PropertyKind.Get => $"get_{memberName}",
                    PropertyKind.Set => $"set_{memberName}",
                    _ => memberName
                };

                // Resumable class methods (async/generator) require the standard js2il calling convention
                // (leading object[] scopes) so the state machine can bind/resume correctly.
                // We keep ClassRegistry min/max counts as JS parameter counts (excluding scopes).
                var hasScopesParam = isAsyncMethod || isGeneratorMethod;

                var sig = MethodBuilder.BuildMethodSignature(
                    _metadata,
                    isInstance: true,
                    paramCount: hasScopesParam ? jsParamCount + 1 : jsParamCount,
                    hasScopesParam: hasScopesParam,
                    returnsVoid: false,
                    returnClrType: returnClrType);

                _classRegistry.RegisterMethod(
                    registryClassName,
                    clrName,
                    (MethodDefinitionHandle)methodToken,
                    sig,
                    returnClrType,
                    hasScopesParam,
                    minParams,
                    jsParamCount);
            }

            return typeHandle;
        }

        private static bool ContainsYieldExpression(Node node, Node functionBoundaryNode)
        {
            bool found = false;

            void Walk(Node? n)
            {
                if (n == null || found)
                {
                    return;
                }

                if (n is YieldExpression)
                {
                    found = true;
                    return;
                }

                // Do not traverse into nested function boundaries.
                if (n is FunctionDeclaration or FunctionExpression or ArrowFunctionExpression
                    && !ReferenceEquals(n, functionBoundaryNode))
                {
                    return;
                }

                foreach (var child in n.ChildNodes)
                {
                    Walk(child);
                    if (found) return;
                }
            }

            Walk(node);
            return found;
        }

        private static string ManglePrivateFieldName(string name)
        {
            return "__js2il_priv_" + name;
        }

        private bool ShouldCreateMethodScopeInstance(FunctionExpression fexpr, Scope classScope)
        {
            if (fexpr.Body is not BlockStatement body) return false;

            // Find the method scope within the class scope's children
            Scope? methodScope = null;
            foreach (var child in classScope.Children)
            {
                if (child.Kind == ScopeKind.Function && child.AstNode == fexpr)
                {
                    methodScope = child;
                    break;
                }
            }

            if (methodScope != null)
            {
                foreach (var binding in methodScope.Bindings.Values)
                {
                    if (binding.IsCaptured)
                    {
                        return true;
                    }
                }
            }

            bool found = false;
            void Walk(Acornima.Ast.Node? n)
            {
                if (n == null || found) return;
                switch (n)
                {
                    case BlockStatement b:
                        foreach (var s in b.Body) Walk(s);
                        break;
                    case ForStatement fs:
                        Walk(fs.Init as Acornima.Ast.Node);
                        Walk(fs.Test as Acornima.Ast.Node);
                        Walk(fs.Update as Acornima.Ast.Node);
                        Walk(fs.Body);
                        break;
                    case ForOfStatement fof:
                        Walk(fof.Left as Acornima.Ast.Node);
                        Walk(fof.Right as Acornima.Ast.Node);
                        Walk(fof.Body);
                        break;
                    case WhileStatement ws:
                        Walk(ws.Test);
                        Walk(ws.Body);
                        break;
                    case DoWhileStatement dws:
                        Walk(dws.Body);
                        Walk(dws.Test);
                        break;
                    case IfStatement ifs:
                        Walk(ifs.Test);
                        Walk(ifs.Consequent);
                        Walk(ifs.Alternate);
                        break;
                    case FunctionDeclaration:
                    case FunctionExpression:
                    case ArrowFunctionExpression:
                        found = true;
                        return;
                    case NewExpression ne:
                        if (ne.Callee is Identifier classId)
                        {
                            var foundClassScope = FindClassScope(classScope, classId.Name);
                            bool instantiatedClassNeedsScopes = false;
                            if (foundClassScope != null)
                            {
                                instantiatedClassNeedsScopes =
                                    foundClassScope.ReferencesParentScopeVariables ||
                                    foundClassScope.Children.Any(c => c.ReferencesParentScopeVariables);

                                if (!instantiatedClassNeedsScopes)
                                {
                                    // Fall back to ABI/layout computation (more authoritative than the propagated flag)
                                    // to catch cases like: Outer().ctor news Inner(), and Inner().ctor reads a global.
                                    try
                                    {
                                        var layoutBuilder = _serviceProvider.GetService<Js2IL.Services.ScopesAbi.EnvironmentLayoutBuilder>();
                                        if (layoutBuilder != null)
                                        {
                                            var layout = layoutBuilder.Build(foundClassScope, Js2IL.Services.ScopesAbi.CallableKind.Constructor);
                                            instantiatedClassNeedsScopes = layout.NeedsParentScopes;
                                        }
                                    }
                                    catch
                                    {
                                        // Ignore and fall back to conservative flags only.
                                    }
                                }
                            }

                            if (instantiatedClassNeedsScopes)
                            {
                                found = true;
                                return;
                            }
                        }
                        foreach (var arg in ne.Arguments) Walk(arg as Acornima.Ast.Node);
                        break;
                    default:
                        if (n is ExpressionStatement es) Walk(es.Expression);
                        else if (n is AssignmentExpression ae) { Walk(ae.Left); Walk(ae.Right); }
                        else if (n is CallExpression ce) { Walk(ce.Callee); foreach (var a in ce.Arguments) Walk(a as Acornima.Ast.Node); }
                        break;
                }
            }

            Walk(body);
            return found;
        }

        private static Scope? FindClassScope(Scope startScope, string className)
        {
            var current = startScope;
            while (current != null)
            {
                foreach (var child in current.Children)
                {
                    if (child.Kind == ScopeKind.Class && child.Name == className)
                    {
                        return child;
                    }
                }
                current = current.Parent;
            }
            return null;
        }
    }
}
