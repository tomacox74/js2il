using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using Acornima.Ast;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using Js2IL.Services.TwoPhaseCompilation;

namespace Js2IL.Services.ILGenerators
{
    internal sealed class ClassesGenerator
    {
        private readonly MetadataBuilder _metadata;
        private readonly BaseClassLibraryReferences _bcl;
        private readonly MethodBodyStreamEncoder _methodBodies;
        private readonly ClassRegistry _classRegistry;
        private readonly Variables _variables;

        private readonly IServiceProvider _serviceProvider;

        public ClassesGenerator(IServiceProvider serviceProvider, MetadataBuilder metadata, BaseClassLibraryReferences bcl, MethodBodyStreamEncoder methodBodies, ClassRegistry classRegistry, Variables variables)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _bcl = bcl ?? throw new ArgumentNullException(nameof(bcl));
            _methodBodies = methodBodies;
            _classRegistry = classRegistry ?? throw new ArgumentNullException(nameof(classRegistry));
            _variables = variables ?? throw new ArgumentNullException(nameof(variables));
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
            var moduleName = _variables.GetGlobalScopeName();
            var parent = classScope.Parent;
            if (parent == null || parent.Kind == ScopeKind.Global)
            {
                return moduleName;
            }

            // CallableDiscovery uses module-qualified scope paths (e.g., "<module>/<function>").
            // Scope.GetQualifiedName() omits the global scope, so prefix with module.
            return $"{moduleName}/{parent.GetQualifiedName()}";
        }

        /// <summary>
        /// Determines which parent scopes will be available to a class method at runtime.
        /// Walks the scope tree from classScope up to global to build the ordered list.
        /// </summary>
        private System.Collections.Generic.List<string> DetermineParentScopesForClassMethod(Scope classScope)
        {
            var scopeNames = new System.Collections.Generic.List<string>();
            var moduleName = _variables.GetGlobalScopeName();
            
            // Walk up from class's parent to root, collecting ancestor scope names
            var current = classScope.Parent;
            var ancestors = new System.Collections.Generic.Stack<string>();
            while (current != null)
            {
                // Registry keys for non-global scopes are module-qualified (<module>/<scope>).
                // Scope.Name in the symbol table is typically unqualified (e.g., "testFunction").
                // Ensure we pass module-qualified names so Variables can map this._scopes correctly.
                var name = current.Name;
                if (!string.IsNullOrEmpty(name) && !name.Contains('/') && name != moduleName)
                {
                    name = $"{moduleName}/{name}";
                }

                ancestors.Push(name);
                current = current.Parent;
            }
            
            // Add ancestors in reverse order (global first, then intermediate scopes)
            while (ancestors.Count > 0)
            {
                scopeNames.Add(ancestors.Pop());
            }
            
            return scopeNames;
        }

        public void DeclareClasses(SymbolTable table)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));
            EmitClassesRecursiveTwoPhase(table.Root);
        }

        private void EmitClassesRecursiveTwoPhase(Scope scope)
        {
            foreach (var child in scope.Children)
            {
                if (child.Kind == ScopeKind.Class && child.AstNode is ClassDeclaration cdecl)
                {
                    DeclareClassTwoPhase(child, cdecl, parentType: default);
                }
                EmitClassesRecursiveTwoPhase(child);
            }
        }

        private TypeDefinitionHandle DeclareClassTwoPhase(Scope classScope, ClassDeclaration cdecl, TypeDefinitionHandle parentType)
        {
            var callableRegistry = _serviceProvider.GetRequiredService<CallableRegistry>();

            var jsClassName = classScope.Name;
            var registryClassName = GetRegistryClassName(classScope);
            var declaringScopeName = GetClassDeclaringScopeName(classScope);

            var ns = classScope.DotNetNamespace ?? "Classes";
            var name = classScope.DotNetTypeName ?? classScope.Name;
            var tb = new TypeBuilder(_metadata, ns, name);

            var typeAttrs = parentType.IsNil
                ? TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit
                : TypeAttributes.NestedPublic | TypeAttributes.Class | TypeAttributes.BeforeFieldInit;

            // Determine whether this class needs to capture parent scopes (same heuristic as legacy path)
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
                        .Where(m => m.Value is FunctionExpression &&
                                    (m.Key as Identifier)?.Name != "constructor"))
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

            // Fields (instance + static) and registries - same as legacy declaration.
            var declaredFieldNames = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            foreach (var pdef in cdecl.Body.Body.OfType<Acornima.Ast.PropertyDefinition>())
            {
                var fSig = new BlobBuilder();
                new BlobEncoder(fSig).Field().Type().Object();
                var fSigHandle = _metadata.GetOrAddBlob(fSig);

                if (pdef.Key is Acornima.Ast.PrivateIdentifier priv)
                {
                    var pname = priv.Name;
                    var emittedName = ManglePrivateFieldName(pname);
                    if (pdef.Static)
                    {
                        var fh = tb.AddFieldDefinition(FieldAttributes.Private | FieldAttributes.Static, emittedName, fSigHandle);
                        _classRegistry.RegisterStaticField(registryClassName, pname, fh);
                    }
                    else
                    {
                        var fh = tb.AddFieldDefinition(FieldAttributes.Private, emittedName, fSigHandle);
                        _classRegistry.RegisterPrivateField(registryClassName, pname, fh);
                    }
                    declaredFieldNames.Add(pname);
                }
                else if (pdef.Key is Identifier pid)
                {
                    if (pdef.Static)
                    {
                        var fh = tb.AddFieldDefinition(FieldAttributes.Public | FieldAttributes.Static, pid.Name, fSigHandle);
                        _classRegistry.RegisterStaticField(registryClassName, pid.Name, fh);
                    }
                    else
                    {
                        var fh = tb.AddFieldDefinition(FieldAttributes.Public, pid.Name, fSigHandle);
                        _classRegistry.RegisterField(registryClassName, pid.Name, fh);
                    }
                    declaredFieldNames.Add(pid.Name);
                }
            }

            FieldDefinitionHandle? scopesField = null;
            if (classNeedsParentScopes)
            {
                var scopesSig = new BlobBuilder();
                new BlobEncoder(scopesSig).Field().Type().SZArray().Object();
                var scopesSigHandle = _metadata.GetOrAddBlob(scopesSig);
                scopesField = tb.AddFieldDefinition(FieldAttributes.Private, "_scopes", scopesSigHandle);
                _classRegistry.RegisterPrivateField(registryClassName, "_scopes", scopesField.Value);
            }

            // Pre-scan methods for this.<prop> assignments (same as legacy) to declare backing fields.
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
                            var fSig = new BlobBuilder();
                            new BlobEncoder(fSig).Field().Type().Object();
                            var fSigHandle = _metadata.GetOrAddBlob(fSig);
                            var fh = tb.AddFieldDefinition(FieldAttributes.Public, prop, fSigHandle);
                            _classRegistry.RegisterField(registryClassName, prop, fh);
                            declaredFieldNames.Add(prop);
                        }
                    }
                }
            }

            // Determine first preallocated method def for this class (if any)
            MethodDefinitionHandle? firstMethod = null;
            foreach (var id in callableRegistry.AllCallables)
            {
                // CallableRegistry is shared across modules; class names may collide across modules.
                // Restrict matching to this class's declaring scope (module-qualified) to avoid collisions.
                if (!string.Equals(id.DeclaringScopeName, declaringScopeName, StringComparison.Ordinal))
                {
                    continue;
                }

                if (id.Kind is not (
                    CallableKind.ClassConstructor or
                    CallableKind.ClassMethod or
                    CallableKind.ClassGetter or
                    CallableKind.ClassSetter or
                    CallableKind.ClassStaticMethod or
                    CallableKind.ClassStaticGetter or
                    CallableKind.ClassStaticSetter or
                    CallableKind.ClassStaticInitializer))
                {
                    continue;
                }

                // Match this class by naming conventions
                if (id.Kind == CallableKind.ClassConstructor || id.Kind == CallableKind.ClassStaticInitializer)
                {
                    if (!string.Equals(id.Name, classScope.Name, StringComparison.Ordinal))
                    {
                        continue;
                    }
                }
                else
                {
                    if (!Js2IL.Utilities.JavaScriptCallableNaming.TrySplitClassMethodCallableName(id.Name, out var candidateClassName, out _))
                    {
                        continue;
                    }

                    if (!string.Equals(candidateClassName, classScope.Name, StringComparison.Ordinal))
                    {
                        continue;
                    }
                }

                if (callableRegistry.TryGetDeclaredToken(id, out var tok) && tok.Kind == HandleKind.MethodDefinition)
                {
                    var mdh = (MethodDefinitionHandle)tok;
                    if (firstMethod == null || MetadataTokens.GetRowNumber(mdh) < MetadataTokens.GetRowNumber(firstMethod.Value))
                    {
                        firstMethod = mdh;
                    }
                }
            }

            var typeHandle = tb.AddTypeDefinition(typeAttrs, _bcl.ObjectType, firstFieldOverride: null, firstMethodOverride: firstMethod);
            if (!parentType.IsNil)
            {
                _metadata.AddNestedType(typeHandle, parentType);
            }
            _classRegistry.Register(registryClassName, typeHandle);

            // Two-phase: register constructor + instance method signatures/parameter counts so call sites can resolve
            // without requiring bodies to be compiled yet.
            // Note: static methods are not currently part of the direct callvirt optimization paths.
            var className = registryClassName;

            // Constructor
            var ctorMember = cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>()
                .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");
            FunctionExpression? ctorFunc = ctorMember?.Value as FunctionExpression;

            MethodDefinitionHandle ctorHandle = default;
            if (ctorMember != null)
            {
                if (!callableRegistry.TryGetDeclaredTokenForAstNode(ctorMember, out var ctorTok) || ctorTok.Kind != HandleKind.MethodDefinition)
                {
                    throw new InvalidOperationException($"[TwoPhase] Missing declared token for class constructor: {className}");
                }
                ctorHandle = (MethodDefinitionHandle)ctorTok;
            }
            else
            {
                // Default constructor callable (synthetic) - find by name
                var ctorId = callableRegistry.AllCallables.FirstOrDefault(c =>
                    c.Kind == CallableKind.ClassConstructor &&
                    string.Equals(c.DeclaringScopeName, declaringScopeName, StringComparison.Ordinal) &&
                    string.Equals(c.Name, jsClassName, StringComparison.Ordinal));
                if (ctorId != null && callableRegistry.TryGetDeclaredToken(ctorId, out var ctorTok) && ctorTok.Kind == HandleKind.MethodDefinition)
                {
                    ctorHandle = (MethodDefinitionHandle)ctorTok;
                }
            }

            if (!ctorHandle.IsNil)
            {
                var needsScopes = classNeedsParentScopes;
                var userParamCount = ctorFunc?.Params.Count ?? 0;
                var totalParamCount = needsScopes ? userParamCount + 1 : userParamCount;
                var ctorSig = MethodBuilder.BuildMethodSignature(
                    _metadata,
                    isInstance: true,
                    paramCount: totalParamCount,
                    hasScopesParam: needsScopes,
                    returnsVoid: true);

                int minUserParams = ctorFunc != null ? ILMethodGenerator.CountRequiredParameters(ctorFunc.Params) : 0;
                int minTotalParams = needsScopes ? minUserParams + 1 : minUserParams;
                int maxTotalParams = totalParamCount;
                _classRegistry.RegisterConstructor(className, ctorHandle, ctorSig, minTotalParams, maxTotalParams);
            }

            // Instance methods/accessors (non-static only)
            foreach (var member in cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>().Where(m => m.Key is Identifier))
            {
                var memberName = ((Identifier)member.Key).Name;
                if (memberName == "constructor") continue;

                if (member.Static) continue;

                if (!callableRegistry.TryGetDeclaredTokenForAstNode(member, out var methTok) || methTok.Kind != HandleKind.MethodDefinition)
                {
                    // Missing token should not happen in two-phase mode.
                    continue;
                }
                var methodHandle = (MethodDefinitionHandle)methTok;
                if (methodHandle.IsNil) continue;

                var clrMethodName = member.Kind switch
                {
                    PropertyKind.Get => $"get_{memberName}",
                    PropertyKind.Set => $"set_{memberName}",
                    _ => memberName
                };

                var funcExpr = member.Value as FunctionExpression;
                var paramCount = funcExpr?.Params.Count ?? 0;
                var msig = MethodBuilder.BuildMethodSignature(
                    _metadata,
                    isInstance: true,
                    paramCount: paramCount,
                    hasScopesParam: false,
                    returnsVoid: false);

                int minParams = funcExpr != null ? ILMethodGenerator.CountRequiredParameters(funcExpr.Params) : 0;
                int maxParams = funcExpr?.Params.Count ?? 0;
                _classRegistry.RegisterMethod(className, clrMethodName, methodHandle, msig, minParams, maxParams);
            }

            return typeHandle;
        }

        private void EmitClassesRecursive(Scope scope)
        {


            foreach (var child in scope.Children)
            {
                if (child.Kind == ScopeKind.Class && child.AstNode is ClassDeclaration cdecl)
                {
                    EmitClass(child, cdecl, parentType: default);
                }
                // Recurse to find nested classes
                EmitClassesRecursive(child);
            }
        }

        private TypeDefinitionHandle EmitClass(Scope classScope, ClassDeclaration cdecl, TypeDefinitionHandle parentType)
        {
            var registryClassName = GetRegistryClassName(classScope);
            // Resolve authoritative .NET names from symbol table; fall back if absent
            var ns = classScope.DotNetNamespace ?? "Classes";
            var name = classScope.DotNetTypeName ?? classScope.Name;
            var tb = new TypeBuilder(_metadata, ns, name);

            // Determine attributes for when we add the type at the end
            var typeAttrs = parentType.IsNil
                ? TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit
                : TypeAttributes.NestedPublic | TypeAttributes.Class | TypeAttributes.BeforeFieldInit;

            // Check if this class needs to access parent scope variables (computed during symbol table build)
            // Also check if any constructor or method needs parent scopes (e.g., instantiates classes needing scopes)
            bool classNeedsParentScopes = classScope.ReferencesParentScopeVariables;
            if (!classNeedsParentScopes)
            {
                // Check constructor
                var ctor = cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>()
                    .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");
                if (ctor?.Value is FunctionExpression ctorExpr)
                {
                    classNeedsParentScopes = ShouldCreateMethodScopeInstance(ctorExpr, classScope);
                }
                
                // Check other methods
                if (!classNeedsParentScopes)
                {
                    foreach (var method in cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>())
                    {
                        if (method.Value is FunctionExpression funcExpr && 
                            (method.Key as Identifier)?.Name != "constructor")
                        {
                            if (ShouldCreateMethodScopeInstance(funcExpr, classScope))
                            {
                                classNeedsParentScopes = true;
                                break;
                            }
                        }
                    }
                }
            }

            // Handle class fields with default initializers (ECMAScript class field syntax)
            // Example: class C { foo = 42; static bar = 1; }
            // We emit instance fields as object and initialize in .ctor.
            // Static fields are emitted as static object fields and initialized in a type initializer (.cctor).
            var fieldsWithInits = new System.Collections.Generic.List<(FieldDefinitionHandle Field, Expression? Init)>();
            var staticFieldsWithInits = new System.Collections.Generic.List<(FieldDefinitionHandle Field, Expression? Init)>();
            var declaredFieldNames = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            foreach (var element in cdecl.Body.Body)
            {
                if (element is Acornima.Ast.PropertyDefinition pdef)
                {
                    // Create field signature: object
                    var fSig = new BlobBuilder();
                    new BlobEncoder(fSig).Field().Type().Object();
                    var fSigHandle = _metadata.GetOrAddBlob(fSig);

                    // Private field (#name)
                    if (pdef.Key is Acornima.Ast.PrivateIdentifier priv)
                    {
                        var pname = priv.Name; // JS-visible name without '#'
                        var emittedName = ManglePrivateFieldName(pname);
                        if (pdef.Static)
                        {
                            var fh = tb.AddFieldDefinition(FieldAttributes.Private | FieldAttributes.Static, emittedName, fSigHandle);
                            // Track static private separately if needed later; for now reuse RegisterStaticField
                            _classRegistry.RegisterStaticField(registryClassName, pname, fh);
                            staticFieldsWithInits.Add((fh, pdef.Value as Expression));
                        }
                        else
                        {
                            var fh = tb.AddFieldDefinition(FieldAttributes.Private, emittedName, fSigHandle);
                            _classRegistry.RegisterPrivateField(registryClassName, pname, fh);
                            fieldsWithInits.Add((fh, pdef.Value as Expression));
                        }
                        declaredFieldNames.Add(pname);
                    }
                    // Public field (identifier)
                    else if (pdef.Key is Identifier pid)
                    {
                        if (pdef.Static)
                        {
                            var fh = tb.AddFieldDefinition(FieldAttributes.Public | FieldAttributes.Static, pid.Name, fSigHandle);
                            _classRegistry.RegisterStaticField(registryClassName, pid.Name, fh);
                            staticFieldsWithInits.Add((fh, pdef.Value as Expression));
                        }
                        else
                        {
                            var fh = tb.AddFieldDefinition(FieldAttributes.Public, pid.Name, fSigHandle);
                            _classRegistry.RegisterField(registryClassName, pid.Name, fh);
                            fieldsWithInits.Add((fh, pdef.Value as Expression));
                        }
                        declaredFieldNames.Add(pid.Name);
                    }
                }
            }

            // Only add _scopes field if the class actually needs to access parent scope variables
            FieldDefinitionHandle? scopesField = null;
            if (classNeedsParentScopes)
            {
                var scopesSig = new BlobBuilder();
                new BlobEncoder(scopesSig).Field().Type().SZArray().Object();
                var scopesSigHandle = _metadata.GetOrAddBlob(scopesSig);
                scopesField = tb.AddFieldDefinition(FieldAttributes.Private, "_scopes", scopesSigHandle);
                _classRegistry.RegisterPrivateField(registryClassName, "_scopes", scopesField.Value);
            }


            // Pre-scan methods (including constructor) for assignments to this.<prop> and declare fields for them
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
                            var fSig = new BlobBuilder();
                            new BlobEncoder(fSig).Field().Type().Object();
                            var fSigHandle = _metadata.GetOrAddBlob(fSig);
                            var fh = tb.AddFieldDefinition(FieldAttributes.Public, prop, fSigHandle);
                            _classRegistry.RegisterField(registryClassName, prop, fh);
                            declaredFieldNames.Add(prop);
                        }
                    }
                }
            }

            // Detect explicit constructor method (name 'constructor')
            var ctorMethod = cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>()
                .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");
            
            // Emit constructor - handles both explicit and parameterless cases
            var ctorFunc = ctorMethod?.Value as FunctionExpression;
            EmitConstructor(tb, ctorFunc, fieldsWithInits, classScope, classNeedsParentScopes);

            // Create the type definition now (after fields and constructor, but before other methods)
            // This allows method bodies to reference the class type when emitting this.method() calls
            var typeHandle = tb.AddTypeDefinition(typeAttrs, _bcl.ObjectType);
            if (!parentType.IsNil)
            {
                _metadata.AddNestedType(typeHandle, parentType);
            }
            // Register the class type for later lookup using the JS-visible identifier (scope name)
            _classRegistry.Register(registryClassName, typeHandle);

            // Methods: create stubs for now; real method codegen will come later
            foreach (var element in cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>())
            {
                var mname = (element.Key as Identifier)?.Name;
                if (string.Equals(mname, "constructor", StringComparison.Ordinal))
                {
                    // already emitted as .ctor above
                    continue;
                }
                EmitMethod(tb, element, classScope);
            }

            // If there are static field initializers, synthesize a type initializer (.cctor) to assign them.
            if (staticFieldsWithInits.Count > 0)
            {
                // Signature: static void .cctor()
                var sigBuilder = new BlobBuilder();
                new BlobEncoder(sigBuilder)
                    .MethodSignature(isInstanceMethod: false)
                    .Parameters(0, r => r.Void(), p => { });
                var cctorSig = _metadata.GetOrAddBlob(sigBuilder);

                var ilGen = new ILMethodGenerator(_serviceProvider, _variables, _bcl, _metadata, _methodBodies, _classRegistry, functionRegistry: null, inClassMethod: false, currentClassName: registryClassName);

                // For each static field with an initializer: evaluate and stsfld
                foreach (var (field, initExpr) in staticFieldsWithInits)
                {
                    if (initExpr is null)
                    {
                        // default null; no store needed
                        continue;
                    }
                    ilGen.ExpressionEmitter.Emit(initExpr, new TypeCoercion() { boxResult = true });
                    ilGen.IL.OpCode(ILOpCode.Stsfld);
                    ilGen.IL.Token(field);
                }

                ilGen.IL.OpCode(ILOpCode.Ret);

                var cctorBody = _methodBodies.AddMethodBody(ilGen.IL);
                tb.AddMethodDefinition(
                    MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    ".cctor",
                    cctorSig,
                    cctorBody);
            }

            return typeHandle;
        }

        private static string ManglePrivateFieldName(string name)
        {
            // Ensure private fields don't collide with public fields/methods and are clearly internal
            return "__js2il_priv_" + name;
        }

        /// <summary>
        /// Emits a constructor for the class, handling both parameterless and explicit constructors.
        /// </summary>
        /// <param name="tb">Type builder for adding the constructor definition.</param>
        /// <param name="ctorFunc">The constructor function expression, or null for a parameterless constructor.</param>
        /// <param name="fieldsWithInits">List of instance fields with their initializer expressions.</param>
        /// <param name="classScope">The scope representing this class.</param>
        /// <param name="needsScopes">Whether the constructor needs a scopes parameter for accessing parent scope variables.</param>
        /// <returns>The method definition handle for the emitted constructor.</returns>
        private MethodDefinitionHandle EmitConstructor(
            TypeBuilder tb,
            FunctionExpression? ctorFunc,
            System.Collections.Generic.List<(FieldDefinitionHandle Field, Expression? Init)> fieldsWithInits,
            Scope classScope,
            bool needsScopes)
        {
            var registryClassName = GetRegistryClassName(classScope);

            // Try the new IR-based compilation pipeline first for simple constructors
            if (ctorFunc != null && !needsScopes && fieldsWithInits.Count == 0)
            {
                var ctorScope = classScope.Children.FirstOrDefault(s => s.Kind == ScopeKind.Function && s.Name == "constructor");
                if (ctorScope != null)
                {
                    var jsMethodCompiler = _serviceProvider.GetRequiredService<JsMethodCompiler>();
                    var (compiledCtor, ctorSignature) = jsMethodCompiler.TryCompileClassConstructor(tb, ctorFunc, ctorScope, _methodBodies, needsScopes);
                    IR.IRPipelineMetrics.RecordConstructorAttempt(!compiledCtor.IsNil);
                    if (!compiledCtor.IsNil)
                    {
                        // Successfully compiled via IR pipeline - signature is returned from TryCompileIRToIL
                        _classRegistry.RegisterConstructor(registryClassName, compiledCtor, ctorSignature, 0, 0);
                        return compiledCtor;
                    }
                }
            }

            // Fallback to the old direct AST-to-IL code path
            var className = registryClassName;
            var userParamCount = ctorFunc?.Params.Count ?? 0;
            var totalParamCount = needsScopes ? userParamCount + 1 : userParamCount;

            // Build constructor signature using shared helper
            var ctorSig = MethodBuilder.BuildMethodSignature(
                _metadata,
                isInstance: true,
                paramCount: totalParamCount,
                hasScopesParam: needsScopes,
                returnsVoid: true);

            // Create Variables context with appropriate parameter tracking
            var paramNames = ctorFunc != null 
                ? ILMethodGenerator.ExtractParameterNames(ctorFunc.Params)
                : Enumerable.Empty<string>();
                
            Variables methodVariables;
            // Registry scope names are module-qualified for all non-global scopes.
            // Class method/constructor scopes are named by SymbolTableBuilder using only the method name.
            var constructorScopeName = $"{_variables.GetGlobalScopeName()}/constructor";
            if (needsScopes)
            {
                var parentScopeNames = DetermineParentScopesForClassMethod(classScope);
                // For constructors with scopes: arg0=this, arg1=scopes[], user params start at arg2
                methodVariables = new Variables(_variables, constructorScopeName, paramNames, parentScopeNames, parameterStartIndex: 2);
            }
            else
            {
                // No scopes: arg0=this, user params start at arg1
                methodVariables = new Variables(_variables, constructorScopeName, paramNames, isNestedFunction: false);
            }
            
            var ilGen = new ILMethodGenerator(_serviceProvider, methodVariables, _bcl, _metadata, _methodBodies, _classRegistry, functionRegistry: null, inClassMethod: true, currentClassName: className);

            // Call base System.Object constructor
            ilGen.IL.OpCode(ILOpCode.Ldarg_0);
            ilGen.IL.Call(_bcl.Object_Ctor_Ref);

            // Store scopes parameter to this._scopes field if needed
            if (needsScopes)
            {
                ilGen.IL.OpCode(ILOpCode.Ldarg_0); // load this
                ilGen.IL.OpCode(ILOpCode.Ldarg_1); // load scopes parameter
                ilGen.IL.OpCode(ILOpCode.Stfld); // store to this._scopes
                _classRegistry.TryGetPrivateField(className, "_scopes", out var _scopesFieldHandle);
                ilGen.IL.Token(_scopesFieldHandle);
            }

            // Initialize fields with default values
            EmitFieldInitializers(ilGen, fieldsWithInits);

            // Only emit constructor body logic if there's an explicit constructor
            if (ctorFunc != null)
            {
                // Initialize default parameter values
                // (constructors: arg0=this, arg1=scopes[] if needed, user params start at arg2 or arg1)
                ushort paramStartIndex = (ushort)(needsScopes ? 2 : 1);
                ilGen.EmitDefaultParameterInitializers(ctorFunc.Params, parameterStartIndex: paramStartIndex);

                // Check if constructor needs a scope instance for destructured parameters or block-scoped locals
                bool hasDestructuredParams = ctorFunc.Params.Any(p => p is ObjectPattern);
                // Destructured parameters ALWAYS need a scope instance to store extracted values
                bool needScopeInstance = hasDestructuredParams || ShouldCreateMethodScopeInstance(ctorFunc, classScope);
                
                // If we need a scope instance, initialize parameters (both simple and destructured)
                bool scopeCreated = false;
                if (needScopeInstance && ctorFunc.Params.Count > 0)
                {
                    // Parameters are stored in the constructor scope (both simple identifiers and destructured)
                    scopeCreated = EmitDestructuredParameterInitialization(
                        ilGen,
                        methodVariables,
                        constructorScopeName,
                        ctorFunc.Params,
                        paramStartIndex);
                }

                // Emit constructor body statements
                if (ctorFunc.Body is BlockStatement bstmt)
                {
                    // Pass false if we already created the scope, true if GenerateStatementsForBody should create it
                    bool shouldCreateScope = needScopeInstance && !scopeCreated;
                    ilGen.GenerateStatementsForBody(methodVariables.GetLeafScopeName(), shouldCreateScope, bstmt.Body);
                }
            }

            // Return from constructor (void)
            ilGen.IL.OpCode(ILOpCode.Ret);

            // Include locals created by ILMethodGenerator (e.g., scope instance for block-scoped vars)
            var (localSignature, bodyAttributes) = MethodBuilder.CreateLocalVariableSignature(_metadata, methodVariables, this._bcl);

            var ctorBody = _methodBodies.AddMethodBody(ilGen.IL, maxStack: 32, localVariablesSignature: localSignature, attributes: bodyAttributes);
            
            // Add parameter metadata for all parameters (scopes + user parameters) for better IL readability
            // Parameters must be added before AddMethodDefinition so they can be referenced by the method
            ParameterHandle paramList = default;
            int seqNum = 1;
            
            // Add scopes parameter metadata if needed
            if (needsScopes)
            {
                paramList = _metadata.AddParameter(ParameterAttributes.None, _metadata.GetOrAddString("scopes"), sequenceNumber: seqNum++);
            }
            
            // Add metadata for user-defined parameters
            if (ctorFunc != null)
            {
                foreach (var param in paramNames)
                {
                    var paramHandle = _metadata.AddParameter(ParameterAttributes.None, _metadata.GetOrAddString(param), sequenceNumber: seqNum++);
                    // Remember first parameter handle for method definition
                    if (paramList.IsNil)
                        paramList = paramHandle;
                }
            }
            
            var ctorDef = tb.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                ".ctor",
                ctorSig,
                ctorBody,
                paramList);
            
            // Calculate min/max parameter counts for default parameter support
            int minUserParams = ctorFunc != null ? ILMethodGenerator.CountRequiredParameters(ctorFunc.Params) : 0;
            int minTotalParams = needsScopes ? minUserParams + 1 : minUserParams;
            int maxTotalParams = totalParamCount;
            
            _classRegistry.RegisterConstructor(className, ctorDef, ctorSig, minTotalParams, maxTotalParams);
            return ctorDef;
        }

        private void EmitFieldInitializers(ILMethodGenerator ilGen, System.Collections.Generic.List<(FieldDefinitionHandle Field, Expression? Init)> fieldsWithInits)
        {
            foreach (var (field, initExpr) in fieldsWithInits)
            {
                ilGen.IL.OpCode(ILOpCode.Ldarg_0);
                if (initExpr is null)
                {
                        // no initializer -> leave default null; skip write
                        // TODO: Refactor this logic for better clarity and maintainability
                    ilGen.IL.OpCode(ILOpCode.Pop);
                }
                else
                {
                    // Use ILMethodGenerator to emit the initializer expression, then box numbers if needed
                    ilGen.ExpressionEmitter.Emit(initExpr, new TypeCoercion() { boxResult = true });
                    ilGen.IL.OpCode(ILOpCode.Stfld);
                    ilGen.IL.Token(field);
                }
            }
        }

    private MethodDefinitionHandle EmitMethod(TypeBuilder tb, Acornima.Ast.MethodDefinition element, Scope classScope)
        {
            var memberName = (element.Key as Identifier)?.Name ?? "method";
            var className = GetRegistryClassName(classScope);
            var funcExpr = element.Value as FunctionExpression;

            // Accessors are modeled as MethodDefinition with Kind Get/Set.
            // Emit CLR-friendly names to avoid collisions and match standard getter/setter conventions.
            var mname = element.Kind switch
            {
                PropertyKind.Get => $"get_{memberName}",
                PropertyKind.Set => $"set_{memberName}",
                _ => memberName
            };

            // Prefer matching by AST node for correctness across methods vs accessors.
            var mscope = funcExpr != null
                ? classScope.Children.FirstOrDefault(s => s.Kind == ScopeKind.Function && ReferenceEquals(s.AstNode, funcExpr))
                : classScope.Children.FirstOrDefault(s => s.Kind == ScopeKind.Function && s.Name == memberName);

            if (mscope != null)
            {
                var jsMethodCompiler = _serviceProvider.GetRequiredService<JsMethodCompiler>();
                
                // For instance methods, pass the _scopes field handle if it exists
                FieldDefinitionHandle? scopesFieldHandle = null;
                if (!element.Static && _classRegistry.TryGetPrivateField(className, "_scopes", out var scopesField))
                {
                    scopesFieldHandle = scopesField;
                }
                
                var methodDefHandle = jsMethodCompiler.TryCompileMethod(tb, mname, element, mscope, _methodBodies, scopesFieldHandle);
                IR.IRPipelineMetrics.RecordClassMethodAttempt(!methodDefHandle.IsNil);
                if (!methodDefHandle.IsNil)
                {
                    // Successfully compiled method via JsMethodCompiler
                    // Register in ClassRegistry so call sites can find it with correct parameter counts
                    if (!element.Static && funcExpr != null)
                    {
                        var irParamCount = funcExpr.Params.Count;
                        var irSig = MethodBuilder.BuildMethodSignature(
                            _metadata,
                            isInstance: true,
                            paramCount: irParamCount,
                            hasScopesParam: false,
                            returnsVoid: false);
                        int minParams = ILMethodGenerator.CountRequiredParameters(funcExpr.Params);
                        int maxParams = irParamCount;
                        _classRegistry.RegisterMethod(className, mname, methodDefHandle, irSig, minParams, maxParams);
                    }
                    return methodDefHandle;
                }
            }

            var paramCount = funcExpr != null ? funcExpr.Params.Count : 0;
            var msig = MethodBuilder.BuildMethodSignature(
                _metadata,
                isInstance: !element.Static,
                paramCount: paramCount,
                hasScopesParam: false,
                returnsVoid: false);

            // Use ILMethodGenerator for body emission to reuse existing statement/expression logic
            // Build method variables context (no JS parameters yet for class methods)
            var paramNames = element.Value is FunctionExpression fe
                ? ILMethodGenerator.ExtractParameterNames(fe.Params)
                : Enumerable.Empty<string>();
            
            // For class instance methods, determine which parent scopes will be available via this._scopes
            // For static methods, use standard nested function semantics
            // Registry scope names are module-qualified for all non-global scopes.
            // Class method scopes are named by SymbolTableBuilder using only the method name.
            var methodScopeName = $"{_variables.GetGlobalScopeName()}/{mname}";
            Variables methodVariables;
            if (!element.Static && _classRegistry.TryGetPrivateField(className, "_scopes", out var _))
            {
                // Instance method with _scopes field: use explicit parent scope list
                var parentScopeNames = DetermineParentScopesForClassMethod(classScope);
                methodVariables = new Variables(_variables, methodScopeName, paramNames, parentScopeNames);
            }
            else
            {
                // Static method or instance method without _scopes: standard semantics
                methodVariables = new Variables(_variables, methodScopeName, paramNames, isNestedFunction: false);
            }
            
            var ilGen = new ILMethodGenerator(_serviceProvider, methodVariables, _bcl, _metadata, _methodBodies, _classRegistry, functionRegistry: null, inClassMethod: true, currentClassName: className);

            // Initialize default parameter values (instance methods: arg0=this, params start at arg1; static methods: params start at arg0)
            // and check for explicit return
            bool hasExplicitReturn = false;
            if (element.Value is FunctionExpression fexpr)
            {
                ushort paramStartIndex = (ushort)(element.Static ? 0 : 1);
                ilGen.EmitDefaultParameterInitializers(fexpr.Params, parameterStartIndex: paramStartIndex);
                
                if (fexpr.Body is BlockStatement bstmt)
                {
                    hasExplicitReturn = bstmt.Body.Any(s => s is ReturnStatement);
                    
                    // Check if method needs a scope instance for destructured parameters or block-scoped locals
                    bool hasDestructuredParams = fexpr.Params.Any(p => p is ObjectPattern);
                    // Destructured parameters ALWAYS need a scope instance to store extracted values
                    bool needScopeInstance = hasDestructuredParams || ShouldCreateMethodScopeInstance(fexpr, classScope);
                    
                    // If we need a scope instance, initialize parameters (both simple and destructured)
                    bool scopeCreated = false;
                    if (needScopeInstance && fexpr.Params.Count > 0)
                    {
                        // Parameters are stored in the method scope (both simple identifiers and destructured)
                        scopeCreated = EmitDestructuredParameterInitialization(
                            ilGen,
                            methodVariables,
                            methodScopeName,
                            fexpr.Params,
                            paramStartIndex);
                    }
                    
                    // Pass false if we already created the scope, true if GenerateStatementsForBody should create it
                    bool shouldCreateScope = needScopeInstance && !scopeCreated;
                    ilGen.GenerateStatementsForBody(methodVariables.GetLeafScopeName(), shouldCreateScope, bstmt.Body);
                }
                else
                {
                    // No body or unsupported shape: default to returning undefined (null)
                }
            }

            if (!hasExplicitReturn)
            {
                // Methods default to returning 'this' (fluent pattern).
                // Accessors default to returning undefined (null) when missing explicit return.
                if (element.Kind is PropertyKind.Get or PropertyKind.Set)
                {
                    ilGen.IL.OpCode(ILOpCode.Ldnull);
                }
                else if (!element.Static)
                {
                    ilGen.IL.OpCode(ILOpCode.Ldarg_0); // load 'this'
                }
                else
                {
                    ilGen.IL.OpCode(ILOpCode.Ldnull);
                }
                ilGen.IL.OpCode(ILOpCode.Ret);
            }

            // Include locals created by ILMethodGenerator (e.g., scopes)
            var (localSignature, bodyAttributes) = MethodBuilder.CreateLocalVariableSignature(_metadata, methodVariables, this._bcl);

            var mbody = _methodBodies.AddMethodBody(ilGen.IL, maxStack: 32, localVariablesSignature: localSignature, attributes: bodyAttributes);
            var attrs = MethodAttributes.Public | MethodAttributes.HideBySig;
            if (element.Static)
            {
                attrs |= MethodAttributes.Static;
            }
            var methodDef = tb.AddMethodDefinition(attrs, mname, msig, mbody);

            // Register instance methods in ClassRegistry for call site validation (static methods use different invocation path)
            if (!element.Static && element.Value is FunctionExpression methodFunc)
            {
                int minParams = ILMethodGenerator.CountRequiredParameters(methodFunc.Params);
                int maxParams = methodFunc.Params.Count;
                _classRegistry.RegisterMethod(className, mname, methodDef, msig, minParams, maxParams);
            }

            return methodDef;
        }

        private bool ShouldCreateMethodScopeInstance(FunctionExpression fexpr, Scope classScope)
        {
            if (fexpr.Body is not BlockStatement body) return false;
            
            // Find the method scope within the class scope's children
            // The method scope name should match the FunctionExpression's parent MethodDefinition's key
            Scope? methodScope = null;
            foreach (var child in classScope.Children)
            {
                if (child.Kind == ScopeKind.Function && child.AstNode == fexpr)
                {
                    methodScope = child;
                    break;
                }
            }
            
            // If we found the method scope, check if it has any captured bindings
            if (methodScope != null)
            {
                // Check if ANY binding in this method scope is captured
                foreach (var binding in methodScope.Bindings.Values)
                {
                    if (binding.IsCaptured)
                    {
                        return true; // Need scope instance for captured variables
                    }
                }
            }
            
            // Also walk the AST to check for nested functions or class instantiations
            // that would require a scope instance
            bool found = false;
            void Walk(Acornima.Ast.Node? n)
            {
                if (n == null || found) return;
                switch (n)
                {
                    case BlockStatement b:
                        foreach (var s in b.Body) Walk(s);
                        break;
                    case VariableDeclaration vd:
                        // Don't trigger on let/const alone - only matters if captured
                        // The binding.IsCaptured check above handles this
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
                        Walk(ws.Test); Walk(ws.Body); break;
                    case DoWhileStatement dws:
                        Walk(dws.Body); Walk(dws.Test); break;
                    case IfStatement ifs:
                        Walk(ifs.Test); Walk(ifs.Consequent); Walk(ifs.Alternate); break;
                    case ReturnStatement:
                        break; // ignore
                    case FunctionDeclaration:
                    case FunctionExpression:
                    case ArrowFunctionExpression:
                        // Nested functions require capturing outer scope variables
                        found = true; return;
                    case NewExpression ne:
                        // Check if instantiating a class that needs parent scopes
                        // If so, this method needs a scope instance to be passed in the scope array
                        if (ne.Callee is Identifier classId)
                        {
                            var className = classId.Name;
                            // Look up the class scope to check if it references parent scopes
                            var foundClassScope = FindClassScope(classScope, className);
                            if (foundClassScope != null && foundClassScope.ReferencesParentScopeVariables)
                            {
                                found = true; return;
                            }
                        }
                        // Continue walking arguments
                        foreach (var arg in ne.Arguments) Walk(arg as Acornima.Ast.Node);
                        break;
                    default:
                        // Recurse into common expression containers
                        if (n is ExpressionStatement es) Walk(es.Expression);
                        else if (n is AssignmentExpression ae) { Walk(ae.Left); Walk(ae.Right); }
                        else if (n is CallExpression ce) { Walk(ce.Callee); foreach (var a in ce.Arguments) Walk(a as Acornima.Ast.Node); }
                        break;
                }
            }
            Walk(body);
            return found;
        }

        /// <summary>
        /// Searches for a class scope by name starting from the given scope and walking up the tree.
        /// </summary>
        private Scope? FindClassScope(Scope startScope, string className)
        {
            // Search current scope and siblings
            var current = startScope;
            while (current != null)
            {
                // Check current scope's children for the class
                foreach (var child in current.Children)
                {
                    if (child.Kind == ScopeKind.Class && child.Name == className)
                    {
                        return child;
                    }
                }
                // Move up to parent scope
                current = current.Parent;
            }
            return null;
        }

        /// <summary>
        /// Handles initialization of destructured parameters in class methods and constructors.
        /// Creates scope instance early and initializes both simple identifier params and object-pattern destructured params.
        /// </summary>
        /// <param name="ilGen">IL method generator for emitting IL code.</param>
        /// <param name="methodVariables">Variables context for the method.</param>
        /// <param name="scopeName">Name of the scope (method name or "constructor").</param>
        /// <param name="parameters">Function parameters from AST.</param>
        /// <param name="paramStartIndex">Starting parameter index (accounts for 'this' and/or 'scopes[]').</param>
        /// <returns>True if scope instance was created, false otherwise.</returns>
        private bool EmitDestructuredParameterInitialization(
            ILMethodGenerator ilGen,
            Variables methodVariables,
            string scopeName,
            NodeList<Node> parameters,
            ushort paramStartIndex)
        {
            var registry = methodVariables.GetVariableRegistry();
            if (registry == null)
            {
                return false;
            }

            var fields = registry.GetVariablesForScope(scopeName);
            if (fields == null || !fields.Any())
            {
                return false;
            }

            // Create the scope instance early so we can initialize destructured params
            ScopeInstanceEmitter.EmitCreateLeafScopeInstance(methodVariables, ilGen.IL, _metadata);
            
            // Initialize destructured parameters into scope fields
            var localScope = methodVariables.GetLocalScopeSlot();
            if (localScope.Address < 0)
                return false;

            var fieldNames = new System.Collections.Generic.HashSet<string>(fields.Select(f => f.Name));
            ushort jsParamSeq = paramStartIndex;
            
            // Initialize simple identifier params first
            for (int i = 0; i < parameters.Count; i++)
            {
                var paramNode = parameters[i];
                Identifier? pid = paramNode as Identifier;
                if (pid == null && paramNode is AssignmentPattern ap)
                {
                    pid = ap.Left as Identifier;
                }
                
                if (pid != null && fieldNames.Contains(pid.Name))
                {
                    ilGen.IL.LoadLocal(localScope.Address);
                    ilGen.EmitLoadParameterWithDefault(paramNode, jsParamSeq);
                    var fieldHandle = registry.GetFieldHandle(scopeName, pid.Name);
                    ilGen.IL.OpCode(ILOpCode.Stfld);
                    ilGen.IL.Token(fieldHandle);
                }
                jsParamSeq++;
            }
            
            // Now handle object-pattern destructuring
            var runtime = new Runtime(ilGen.IL, _serviceProvider.GetRequiredService<TypeReferenceRegistry>(), _serviceProvider.GetRequiredService<MemberReferenceRegistry>());
            MethodBuilder.EmitObjectPatternParameterDestructuring(
                _metadata,
                ilGen.IL,
                runtime,
                methodVariables,
                scopeName,
                parameters,
                ilGen.ExpressionEmitter, // Pass expression emitter for default value support
                startingJsParamSeq: paramStartIndex,
                castScopeForStore: true);

            return true;
        }

    }
}
