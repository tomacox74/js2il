using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.SymbolTables;
using Js2IL.IR;
using Js2IL.IL;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.Utilities.Ecma335;
using Js2IL.Services;
using Js2IL.Services.TwoPhaseCompilation;
using Microsoft.Extensions.DependencyInjection;
using ScopesCallableKind = Js2IL.Services.ScopesAbi.CallableKind;
using Js2IL.Utilities;

namespace Js2IL;

sealed record MethodParameterDescriptor
{
    public MethodParameterDescriptor(string name, Type parameterType)
    {
        Name = name;
        ParameterType = parameterType;
    }

    public string Name { get; init; }
    public Type ParameterType { get; init; }
}

sealed record MethodDescriptor
{
    public MethodDescriptor(string name, TypeBuilder typeBuilder, IReadOnlyList<MethodParameterDescriptor> parameters)
    {
        Name = name;
        TypeBuilder = typeBuilder;
        Parameters = parameters;
    }

    public string Name { get; init; }
    public TypeBuilder TypeBuilder { get; init; }
    public IReadOnlyList<MethodParameterDescriptor> Parameters { get; set; }

    /// <summary>
    ///  Default is to return an object
    /// </summary>
    public bool ReturnsVoid { get; set; } = false;

    /// <summary>
    /// CLR return type for the emitted method signature when <see cref="ReturnsVoid"/> is false.
    /// Defaults to <see cref="object"/> (JavaScript value).
    /// </summary>
    public Type ReturnClrType { get; set; } = typeof(object);

    /// <summary>
    /// Optional metadata handle for a non-BCL reference return type (e.g., a user-defined JS class TypeDef).
    /// When set, IL signature emission uses this handle instead of <see cref="ReturnClrType"/>.
    /// </summary>
    public EntityHandle ReturnTypeHandle { get; set; } = default;

    /// <summary>
    /// Only class instance methods are not static currently, so we default to static.
    /// </summary>
    public bool IsStatic {get; set; } = true;

    /// <summary>
    /// Whether the method has a scopes array as its first IL argument.
    /// User-defined functions have this (arg0 = scopes, arg1+ = JS params).
    /// Module Main methods don't have this (arg0+ = module wrapper params).
    /// </summary>
    public bool HasScopesParameter { get; set; } = true;

    /// <summary>
    /// For instance methods that access parent scopes, this is the field handle to the _scopes field.
    /// When set, IL emission loads scopes via: ldarg.0 (this), ldfld ScopesFieldHandle
    /// </summary>
    public FieldDefinitionHandle? ScopesFieldHandle { get; set; }

    /// <summary>
    /// True if this method is a class constructor (.ctor).
    /// When true, the IL emitter will prepend a base System.Object::.ctor() call.
    /// </summary>
    public bool IsConstructor { get; set; } = false;

    /// <summary>
    /// True if this constructor belongs to a derived class (i.e., the class has an extends clause).
    /// When true, the IL emitter must NOT prepend System.Object::.ctor(); the base constructor
    /// should be invoked via an explicit or synthesized <c>super(...)</c> call.
    /// </summary>
    public bool IsDerivedConstructor { get; set; } = false;
}

/// <summary>
/// Per method compiling from JS to IL
/// </summary>
/// <remarks>
/// AST -> HIR -> LIR -> IL
/// </remarks>
internal sealed class JsMethodCompiler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MetadataBuilder _metadataBuilder;
    private readonly TypeReferenceRegistry _typeReferenceRegistry;
    private readonly MemberReferenceRegistry _memberReferenceRegistry;
    private readonly BaseClassLibraryReferences _bclReferences;
    private readonly Services.VariableBindings.ScopeMetadataRegistry _scopeMetadataRegistry;
    private readonly FunctionTypeMetadataRegistry _functionTypeMetadataRegistry;
    private readonly AnonymousCallableTypeMetadataRegistry _anonymousCallableTypeMetadataRegistry;
    private readonly Services.NestedTypeRelationshipRegistry _nestedTypeRelationshipRegistry;
    private readonly Services.ModuleTypeMetadataRegistry _moduleTypeRegistry;

    public JsMethodCompiler(
        MetadataBuilder metadataBuilder,
        TypeReferenceRegistry typeReferenceRegistry,
        MemberReferenceRegistry memberReferenceRegistry,
        BaseClassLibraryReferences bclReferences,
        Services.VariableBindings.ScopeMetadataRegistry scopeMetadataRegistry,
        FunctionTypeMetadataRegistry functionTypeMetadataRegistry,
        AnonymousCallableTypeMetadataRegistry anonymousCallableTypeMetadataRegistry,
        Services.NestedTypeRelationshipRegistry nestedTypeRelationshipRegistry,
        Services.ModuleTypeMetadataRegistry moduleTypeRegistry,
        IServiceProvider serviceProvider)
    {
        _metadataBuilder = metadataBuilder;
        _typeReferenceRegistry = typeReferenceRegistry;
        _memberReferenceRegistry = memberReferenceRegistry;
        _bclReferences = bclReferences;
        _scopeMetadataRegistry = scopeMetadataRegistry;
        _functionTypeMetadataRegistry = functionTypeMetadataRegistry;
        _anonymousCallableTypeMetadataRegistry = anonymousCallableTypeMetadataRegistry;
        _nestedTypeRelationshipRegistry = nestedTypeRelationshipRegistry;
        _moduleTypeRegistry = moduleTypeRegistry;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Creates a new LIRToILCompiler instance for compiling a single method.
    /// </summary>
    private LIRToILCompiler CreateILCompiler()
    {
        return new LIRToILCompiler(_metadataBuilder, _typeReferenceRegistry, _memberReferenceRegistry, _bclReferences, _scopeMetadataRegistry, _serviceProvider);
    }

    #region Public API - Entry Points

    public CompiledCallableBody CompileClassConstructorBodyTwoPhase(
        CallableId callable,
        MethodDefinitionHandle expectedMethodDef,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        SymbolTable symbolTable,
        Scope classScope,
        Node classNode,
        Node? ctorNodeOverride,
        bool needsScopes)
    {
        if (expectedMethodDef.IsNil) throw new ArgumentException("Expected MethodDef cannot be nil.", nameof(expectedMethodDef));

        var (classBody, superClass) = classNode switch
        {
            ClassDeclaration classDecl => (classDecl.Body, classDecl.SuperClass),
            ClassExpression classExpr => (classExpr.Body, classExpr.SuperClass),
            _ => throw new InvalidOperationException($"Unsupported class node type: {classNode.Type}")
        };

        // Prefer IR pipeline (AST -> HIR -> LIR -> IL). HIRBuilder injects:
        // - this._scopes = scopes (when needsScopes)
        // - instance field initializers
        // LIRToILCompiler injects:
        // - base System.Object::.ctor() call for constructors
        // Preserve legacy behavior for ClassDeclaration constructors (compile the FunctionExpression),
        // but prefer MethodDefinition for ClassExpression constructors so HIRBuilder has class context.
        Node ctorNode;
        Scope ctorScope;
        if (ctorNodeOverride is Acornima.Ast.MethodDefinition ctorMethodDef && ctorMethodDef.Value is FunctionExpression ctorFunc)
        {
            if (classNode is ClassDeclaration)
            {
                ctorNode = ctorFunc;
            }
            else
            {
                ctorNode = ctorMethodDef;
            }

            ctorScope = symbolTable.FindScopeByAstNode(ctorFunc)
                ?? symbolTable.FindScopeByAstNode(ctorMethodDef)
                ?? classScope;
        }
        else
        {
            ctorNode = classBody;
            ctorScope = classScope;
        }

        var irBody = TryCompileCallableBody(
            callable: callable,
            expectedMethodDef: expectedMethodDef,
            ilMethodName: ".ctor",
            node: ctorNode,
            scope: ctorScope,
            methodBodyStreamEncoder: methodBodyStreamEncoder,
            isInstanceMethod: true,
            hasScopesParameter: needsScopes,
            scopesFieldHandle: null,
            returnsVoid: true,
            callableKindOverride: ScopesCallableKind.Constructor,
            isDerivedConstructor: superClass != null);

        if (irBody != null)
        {
            return irBody;
        }

        var lastFailure = IR.IRPipelineMetrics.GetLastFailure();
        var extra = string.IsNullOrWhiteSpace(lastFailure) ? string.Empty : $"\nIR failure: {lastFailure}";
        throw new NotSupportedException(
            $"IR pipeline could not compile class constructor body for callable '{callable}' (node={ctorNode.Type}, scope='{ctorScope.GetQualifiedName()}').{extra}");
    }

    public CompiledCallableBody CompileClassStaticInitializerBodyTwoPhase(
        CallableId callable,
        MethodDefinitionHandle expectedMethodDef,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        Scope classScope,
        Node classNode)
    {
        if (expectedMethodDef.IsNil) throw new ArgumentException("Expected MethodDef cannot be nil.", nameof(expectedMethodDef));

        var irBody = TryCompileCallableBody(
            callable: callable,
            expectedMethodDef: expectedMethodDef,
            ilMethodName: ".cctor",
            node: classNode,
            scope: classScope,
            methodBodyStreamEncoder: methodBodyStreamEncoder,
            isInstanceMethod: false,
            hasScopesParameter: false,
            scopesFieldHandle: null,
            returnsVoid: true,
            callableKindOverride: ScopesCallableKind.ClassStaticInitializer);

        if (irBody != null)
        {
            return irBody;
        }

        throw new NotSupportedException(
            $"IR pipeline could not compile class static initializer body for callable '{callable}' (node={classNode.Type}, scope='{classScope.GetQualifiedName()}').");
    }

    public CompiledCallableBody CompileClassMethodBodyTwoPhase(
        CallableId callable,
        MethodDefinitionHandle expectedMethodDef,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        ClassRegistry classRegistry,
        SymbolTable symbolTable,
        Scope classScope,
        Acornima.Ast.MethodDefinition methodDef,
        string clrMethodName,
        bool hasScopes)
    {
        if (expectedMethodDef.IsNil) throw new ArgumentException("Expected MethodDef cannot be nil.", nameof(expectedMethodDef));

        var className = ScopeNaming.GetRegistryClassName(classScope);
        var funcExpr = methodDef.Value as FunctionExpression;
        var methodScope = funcExpr != null ? symbolTable.FindScopeByAstNode(funcExpr) : null;
        // Some symbol table shapes attach the scope to the MethodDefinition rather than the FunctionExpression.
        methodScope ??= symbolTable.FindScopeByAstNode(methodDef);

        // Last-resort fallback: locate the method's function scope by name under the class scope.
        if (methodScope == null && methodDef.Key is Identifier methodNameIdent)
        {
            methodScope = classScope.Children.FirstOrDefault(s =>
                s.Kind == ScopeKind.Function
                && string.Equals(s.Name, methodNameIdent.Name, StringComparison.Ordinal));
        }

        methodScope ??= classScope;

        // Resumable class methods (async/generator) require the standard js2il calling convention
        // (leading scopes array) for state machine emission.
        bool isAsyncMethod = methodScope?.IsAsync == true || funcExpr?.Async == true;
        bool isGeneratorMethod = methodScope?.IsGenerator == true || funcExpr?.Generator == true;
        var hasScopesParameter = isAsyncMethod || isGeneratorMethod;

        FieldDefinitionHandle? scopesFieldHandle = null;
        if (!methodDef.Static && hasScopes)
        {
            if (!classRegistry.TryGetPrivateField(className, "_scopes", out var scopesField))
            {
                throw new InvalidOperationException($"Class '{className}' expected to have a _scopes field but none was registered.");
            }
            scopesFieldHandle = scopesField;
        }

        var callableKindOverride = methodDef.Static ? (ScopesCallableKind?)null : ScopesCallableKind.ClassMethod;
        var irBody = TryCompileCallableBody(
            callable: callable,
            expectedMethodDef: expectedMethodDef,
            ilMethodName: clrMethodName,
            node: methodDef,
            scope: methodScope!,
            methodBodyStreamEncoder: methodBodyStreamEncoder,
            isInstanceMethod: !methodDef.Static,
            hasScopesParameter: hasScopesParameter,
            scopesFieldHandle: scopesFieldHandle,
            returnsVoid: false,
            callableKindOverride: callableKindOverride);

        if (irBody != null)
        {
            return irBody;
        }

        var lastFailure = IR.IRPipelineMetrics.GetLastFailure();
        var extra = string.IsNullOrWhiteSpace(lastFailure) ? string.Empty : $"\nIR failure: {lastFailure}";
        throw new NotSupportedException(
            $"IR pipeline could not compile class method body for callable '{callable}' (node={methodDef.Type}, scope='{methodScope!.GetQualifiedName()}').{extra}");
    }

    /// <summary>
    /// Two-phase API: attempt to compile a callable body to IL without emitting a MethodDef row.
    /// Phase 1 must preallocate <paramref name="expectedMethodDef"/>.
    /// </summary>
    public CompiledCallableBody? TryCompileCallableBody(
        CallableId callable,
        MethodDefinitionHandle expectedMethodDef,
        string ilMethodName,
        Node node,
        Scope scope,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        bool isInstanceMethod,
        bool hasScopesParameter,
        FieldDefinitionHandle? scopesFieldHandle,
        bool returnsVoid = false,
        ScopesCallableKind? callableKindOverride = null,
        bool isDerivedConstructor = false)
    {
        // Extract params/body from supported node shapes
        NodeList<Node>? functionParams = null;
        Node bodyNode = node;

        if (node is FunctionDeclaration funcDecl)
        {
            functionParams = funcDecl.Params;
            bodyNode = node;
        }
        else if (node is FunctionExpression funcExpr)
        {
            functionParams = funcExpr.Params;
            bodyNode = node;
        }
        else if (node is ArrowFunctionExpression arrowExpr)
        {
            functionParams = arrowExpr.Params;
            bodyNode = arrowExpr;
        }
        else if (node is Acornima.Ast.MethodDefinition classMethDef && classMethDef.Value is FunctionExpression methodFuncExpr)
        {
            functionParams = methodFuncExpr.Params;
            bodyNode = classMethDef; // HIRBuilder handles MethodDefinition
        }

        // IR pipeline supports identifier params, simple defaults, destructuring patterns, and rest parameters.
        if (functionParams.HasValue && !ParamsSupportedForIR(functionParams.Value))
        {
            return null;
        }

        var inferredKind = ScopesCallableKind.Function;
        if (isInstanceMethod)
        {
            inferredKind = hasScopesParameter ? ScopesCallableKind.Constructor : ScopesCallableKind.ClassMethod;
        }

        var callableKind = callableKindOverride ?? inferredKind;

        if (!TryLowerASTToLIR(bodyNode, scope, callableKind, hasScopesParameter, out var lirMethod, callableId: callable, isDerivedConstructor: isDerivedConstructor))
        {
            return null;
        }

        var parameters = new List<MethodParameterDescriptor>();
        if (hasScopesParameter)
        {
            parameters.Add(new MethodParameterDescriptor("scopes", typeof(object[])));
        }

        foreach (var paramName in lirMethod!.Parameters)
        {
            parameters.Add(new MethodParameterDescriptor(paramName, typeof(object)));
        }

        // Dummy TypeBuilder (body-only compilation does not emit a TypeDef).
        // Use a deterministic unique name to avoid any future collisions if this ever changes.
        var dummyTypeName = $"<TwoPhaseDummy_M{MetadataTokens.GetRowNumber(expectedMethodDef)}>";
        var dummyType = new TypeBuilder(_metadataBuilder, "", dummyTypeName);

        var methodDescriptor = new MethodDescriptor(ilMethodName, dummyType, parameters)
        {
            IsStatic = !isInstanceMethod,
            HasScopesParameter = hasScopesParameter,
            ReturnsVoid = returnsVoid,
            ReturnClrType = returnsVoid
                ? typeof(void)
                : (lirMethod.IsGenerator
                    ? typeof(object)
                    : (((scope.Kind == ScopeKind.Function && scope.Parent?.Kind == ScopeKind.Class)
                            ? scope.StableReturnClrType
                            : null)
                        ?? typeof(object))),
            ReturnTypeHandle = default,
            ScopesFieldHandle = scopesFieldHandle,
            IsConstructor = callableKind == ScopesCallableKind.Constructor,
            IsDerivedConstructor = callableKind == ScopesCallableKind.Constructor && isDerivedConstructor
        };

        // Allow class instance methods that `return this` to have a class-typed IL signature.
        // This enables fluent call chains without boxing back to object.
        if (!returnsVoid
            && !lirMethod.IsGenerator
            && scope.Kind == ScopeKind.Function
            && scope.Parent?.Kind == ScopeKind.Class
            && scope.StableReturnIsThis)
        {
            var classScope = scope.Parent;
            var classRegistry = _serviceProvider.GetRequiredService<Services.ClassRegistry>();
            var registryClassName = $"{classScope.DotNetNamespace ?? SymbolTableBuilder.DefaultClassesNamespace}.{classScope.DotNetTypeName ?? classScope.Name}";
            if (classRegistry.TryGet(registryClassName, out var classTypeDef) && !classTypeDef.IsNil)
            {
                methodDescriptor.ReturnTypeHandle = classTypeDef;
                methodDescriptor.ReturnClrType = typeof(object);
            }
        }

        return CreateILCompiler().TryCompileCallableBody(callable, expectedMethodDef, methodDescriptor, lirMethod!, methodBodyStreamEncoder);
    }

    public MethodDefinitionHandle TryCompileMethod(TypeBuilder typeBuilder, string methodName, Node node, Scope scope, MethodBodyStreamEncoder methodBodyStreamEncoder)
        => TryCompileMethod(typeBuilder, methodName, node, scope, methodBodyStreamEncoder, scopesFieldHandle: null);

    public MethodDefinitionHandle TryCompileMethod(TypeBuilder typeBuilder, string methodName, Node node, Scope scope, MethodBodyStreamEncoder methodBodyStreamEncoder, FieldDefinitionHandle? scopesFieldHandle)
    {
        // Extract params and body from the node based on its type
        NodeList<Node>? functionParams = null;
        Node bodyNode = node;
        
        if (node is FunctionDeclaration funcDecl)
        {
            functionParams = funcDecl.Params;
            bodyNode = node;
        }
        else if (node is Acornima.Ast.MethodDefinition classMethDef && classMethDef.Value is FunctionExpression methodFuncExpr)
        {
            functionParams = methodFuncExpr.Params;
            bodyNode = classMethDef; // HIRBuilder handles MethodDefinition
        }

        // IR pipeline supports identifier params, simple defaults, destructuring patterns, and rest parameters.
        if (functionParams.HasValue && !ParamsSupportedForIR(functionParams.Value))
        {
            return default;
        }

        var callableKind = ScopesCallableKind.Function;
        if (node is Acornima.Ast.MethodDefinition md && !md.Static)
        {
            callableKind = ScopesCallableKind.ClassMethod;
        }

        var hasScopesParameter = true;
        if (node is Acornima.Ast.MethodDefinition md2 && !md2.Static)
        {
            hasScopesParameter = false;
        }

        if (!TryLowerASTToLIR(bodyNode, scope, callableKind, hasScopesParameter, out var lirMethod))
        {
            return default;
        }

        // Build parameter descriptors: scopes array + JS parameters
        var parameters = new List<MethodParameterDescriptor>
        {
            new MethodParameterDescriptor("scopes", typeof(object[]))
        };
        
        // Add JS function parameters (all typed as object)
        foreach (var paramName in lirMethod!.Parameters)
        {
            parameters.Add(new MethodParameterDescriptor(paramName, typeof(object)));
        }

        var methodDescriptor = new MethodDescriptor(
            methodName,
            typeBuilder,
            parameters);

        if (node is Acornima.Ast.MethodDefinition methodDef)
        {
            methodDescriptor.IsStatic = methodDef.Static;
            // Instance methods don't have the scopes parameter
            methodDescriptor.Parameters = parameters.Skip(1).ToList();
            methodDescriptor.HasScopesParameter = false;
            // Instance methods access scopes via this._scopes field
            if (!methodDef.Static && scopesFieldHandle.HasValue)
            {
                methodDescriptor.ScopesFieldHandle = scopesFieldHandle;
            }
        }

        return CreateILCompiler().TryCompile(methodDescriptor, lirMethod!, methodBodyStreamEncoder);
    }

    public MethodDefinitionHandle TryCompileArrowFunction(string typeName, string methodName, Node node, Scope scope, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        // Keep arrow-function IR support conservative for now; parameter destructuring for arrows
        // remains on the legacy path until snapshots are updated intentionally.
        if (node is ArrowFunctionExpression arrowFunc && !AllParamsAreSimpleIdentifiers(arrowFunc.Params))
        {
            return default;
        }

        if (!TryLowerASTToLIR(node, scope, ScopesCallableKind.Function, hasScopesParameter: true, out var lirMethod))
        {
            return default;
        }

        // Create the type builder for the arrow function
        var arrowTypeBuilder = new TypeBuilder(_metadataBuilder, "Functions", typeName);

        // Build parameter descriptors: scopes array + JS parameters
        var parameters = new List<MethodParameterDescriptor>
        {
            new MethodParameterDescriptor("scopes", typeof(object[]))
        };
        
        // Add JS function parameters (all typed as object)
        foreach (var paramName in lirMethod!.Parameters)
        {
            parameters.Add(new MethodParameterDescriptor(paramName, typeof(object)));
        }

        var methodDescriptor = new MethodDescriptor(
            methodName,
            arrowTypeBuilder,
            parameters);

        var methodDefinitionHandle = CreateILCompiler().TryCompile(methodDescriptor, lirMethod!, methodBodyStreamEncoder);

        // Define the arrow function type
        arrowTypeBuilder.AddTypeDefinition(
            TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
            _bclReferences.ObjectType);

        return methodDefinitionHandle;
    }

    /// <summary>
    /// Attempts to compile a class constructor using the IR pipeline.
    /// Falls back to legacy emitter if IR compilation fails.
    /// Note: TypeBuilder is shared and managed by ClassesGenerator, not created here.
    /// </summary>
    /// <returns>A tuple of (MethodDefinitionHandle, BlobHandle signature), or default values if compilation fails.</returns>
    public (MethodDefinitionHandle MethodDef, BlobHandle Signature) TryCompileClassConstructor(
        TypeBuilder typeBuilder, 
        FunctionExpression ctorFunc, 
        Scope constructorScope, 
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        bool needsScopes)
    {
        // IR pipeline doesn't yet handle:
        // - Base constructor calls (required for all constructors)
        // - Field initializations
        // - Scope parameter storage
        // - Constructor parameters
        // Fall back to legacy emitter for all these cases
        if (needsScopes || ctorFunc.Params.Count > 0)
        {
            return default;
        }

        if (!TryLowerASTToLIR(ctorFunc, constructorScope, ScopesCallableKind.Constructor, hasScopesParameter: false, out var lirMethod))
        {
            return default;
        }

        // For constructors: instance method, returns void, no parameters for now
        var methodDescriptor = new MethodDescriptor(
            ".ctor",
            typeBuilder,
            Array.Empty<MethodParameterDescriptor>());

        methodDescriptor.IsStatic = false;
        methodDescriptor.ReturnsVoid = true;
        methodDescriptor.IsConstructor = true;
        methodDescriptor.IsDerivedConstructor = constructorScope.Parent?.AstNode is ClassDeclaration cd && cd.SuperClass != null;

        // Note: This won't produce valid constructor IL yet because we need:
        // 1. Base constructor call (ldarg.0 + call System.Object::.ctor)
        // 2. Field initializations
        // The IR pipeline needs to be extended to handle these in future PRs.
        // For now, this will compile but produce incomplete constructor IL,
        // so the fail-fast guards above should prevent reaching here.
        return CreateILCompiler().TryCompileWithSignature(methodDescriptor, lirMethod!, methodBodyStreamEncoder);
    }

    public MethodDefinitionHandle TryCompileMainMethod(string moduleName, Node node, Scope scope, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        if (!TryLowerASTToLIR(node, scope, ScopesCallableKind.ModuleMain, hasScopesParameter: false, out var lirMethod))
        {
            return default;
        }

        if (!_moduleTypeRegistry.TryGet(moduleName, out var moduleTypeHandle) || moduleTypeHandle.IsNil)
        {
            throw new InvalidOperationException($"Expected module type handle to be registered for module '{moduleName}', but none was found.");
        }

        // create the tools we need to generate the module type and method
        var programTypeBuilder = new TypeBuilder(_metadataBuilder, "Modules", moduleName);

        MethodParameterDescriptor[] parameters = [
            new MethodParameterDescriptor("exports", typeof(object)),
            new MethodParameterDescriptor("require", typeof(JavaScriptRuntime.CommonJS.RequireDelegate)),
            new MethodParameterDescriptor("module", typeof(object)),
            new MethodParameterDescriptor("__filename", typeof(string)),
            new MethodParameterDescriptor("__dirname", typeof(string))
        ];
        var methodDescriptor = new MethodDescriptor(
            "__js_module_init__",
            programTypeBuilder,
            parameters);

        methodDescriptor.ReturnsVoid = true;
        methodDescriptor.HasScopesParameter = false;

        var methodDefinitionHandle = CreateILCompiler().TryCompile(methodDescriptor, lirMethod!, methodBodyStreamEncoder);

        // Establish all nesting relationships now that the module type exists.
        // This is done in sorted order to satisfy ECMA-335 NestedClass table ordering.
        var nestingPlanner = new Services.Nesting.ScopeNestingPlanner(
            _scopeMetadataRegistry,
            _functionTypeMetadataRegistry,
            _anonymousCallableTypeMetadataRegistry,
            _serviceProvider);

        foreach (var (nested, enclosing) in nestingPlanner.PlanModuleNesting(moduleName, moduleTypeHandle, scope))
        {
            _nestedTypeRelationshipRegistry.Add(nested, enclosing);
        }

        return methodDefinitionHandle;
    }

    #endregion

    #region Core Pipeline - AST to LIR to IL

    /// <summary>
    /// Returns true if all parameters are simple identifiers or simple default parameters.
    /// Supports: Identifier, AssignmentPattern with Identifier left-hand side.
    /// Does not support: destructuring patterns, rest patterns, nested defaults.
    /// Used to determine if the IR pipeline can be used for arrow functions.
    /// </summary>
    private static bool AllParamsAreSimpleIdentifiers(in NodeList<Node> parameters)
    {
        return parameters.All(param => param switch
        {
            Identifier => true,
            AssignmentPattern ap => ap.Left is Identifier,
            _ => false
        });
    }

    /// <summary>
    /// Returns true if parameters are supported by the IR pipeline for function declarations/methods.
    /// Supports: Identifier, AssignmentPattern with Identifier left-hand side, ObjectPattern, ArrayPattern, RestElement.
    /// </summary>
    private static bool ParamsSupportedForIR(in NodeList<Node> parameters)
    {
        return parameters.All(param => param switch
        {
            Identifier => true,
            AssignmentPattern ap => ap.Left is Identifier,
            ObjectPattern => true,
            ArrayPattern => true,
            // RestElement at the parameter list level is a rest parameter (...args).
            RestElement => true,
            _ => false
        });
    }

    private bool TryLowerASTToLIR(Node node, Scope scope, ScopesCallableKind callableKind, bool hasScopesParameter, out MethodBodyIR? methodBody, CallableId? callableId = null, bool isDerivedConstructor = false)
    {
        methodBody = null;

        var isAsyncCallable = node switch
        {
            FunctionDeclaration fd => fd.Async,
            FunctionExpression fe => fe.Async,
            ArrowFunctionExpression af => af.Async,
            Acornima.Ast.MethodDefinition md when md.Value is FunctionExpression mfe => mfe.Async,
            _ => false
        };

        var isGeneratorCallable = node switch
        {
            FunctionDeclaration fd => fd.Generator,
            FunctionExpression fe => fe.Generator,
            // Arrow functions cannot be generators.
            Acornima.Ast.MethodDefinition md when md.Value is FunctionExpression mfe => mfe.Generator,
            _ => false
        };

        if (!HIRBuilder.TryParseMethod(node, scope, callableKind, hasScopesParameter, out var hirMethod))
        {
            IR.IRPipelineMetrics.RecordFailureIfUnset($"HIR parse failed for node type {node.Type}");
            return false;
        }

        var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
        if (!HIRToLIRLowerer.TryLower(hirMethod!, scope, _scopeMetadataRegistry, callableKind, hasScopesParameter, classRegistry, out var lirMethod, isAsync: isAsyncCallable, isGenerator: isGeneratorCallable, callableId: callableId, isDerivedConstructor: isDerivedConstructor))
        {
            IR.IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR lowering failed for scope '{scope.GetQualifiedName()}' (kind={scope.Kind}) node={node.Type}");
            return false;
        }

        // Normalize intrinsic-specific patterns (e.g., Int32Array element access) into explicit LIR instructions.
        // This keeps the LIR->IL compiler simpler and avoids fragile late pattern-matching.
        LIRIntrinsicNormalization.Normalize(lirMethod!, classRegistry);

        // Normalize generic member calls into explicit early-bound typed member call instructions when provably safe.
        // This keeps the LIR->IL compiler focused on IL mechanics rather than type-directed rewrites.
        LIRMemberCallNormalization.Normalize(lirMethod!, classRegistry);

        // Normalize generic LIR patterns into more explicit typed forms when provably safe.
        // This keeps LIR->IL focused on IL mechanics rather than type-directed rewrites.
        LIRTypeNormalization.Normalize(lirMethod!, classRegistry);

        methodBody = lirMethod!;
        return true;
    }

    // Backward-compatible helper for existing call sites.
    private bool TryLowerASTToLIR(Node node, Scope scope, out MethodBodyIR? methodBody)
        => TryLowerASTToLIR(node, scope, ScopesCallableKind.Function, hasScopesParameter: true, out methodBody);

    #endregion
}
