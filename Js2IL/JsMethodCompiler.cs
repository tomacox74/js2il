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
    /// Only class instance methods are not static currently, so we default to static.
    /// </summary>
    public bool IsStatic {get; set; } = true;

    /// <summary>
    /// Whether the method has a scopes array as its first IL argument.
    /// User-defined functions have this (arg0 = scopes, arg1+ = JS params).
    /// Module Main methods don't have this (arg0+ = module wrapper params).
    /// </summary>
    public bool HasScopesParameter { get; set; } = true;
}

/// <summary>
/// Per method compiling from JS to IL
/// </summary>
/// <remarks>
/// AST -> HIR -> LIR -> IL
/// </remarks>
internal sealed class JsMethodCompiler
{
    private readonly MetadataBuilder _metadataBuilder;
    private readonly TypeReferenceRegistry _typeReferenceRegistry;
    private readonly MemberReferenceRegistry _memberReferenceRegistry;
    private readonly BaseClassLibraryReferences _bclReferences;
    private readonly CompiledMethodCache _compiledMethodCache;

    public JsMethodCompiler(MetadataBuilder metadataBuilder, TypeReferenceRegistry typeReferenceRegistry, MemberReferenceRegistry memberReferenceRegistry, BaseClassLibraryReferences bclReferences, CompiledMethodCache compiledMethodCache)
    {
        _metadataBuilder = metadataBuilder;
        _typeReferenceRegistry = typeReferenceRegistry;
        _memberReferenceRegistry = memberReferenceRegistry;
        _bclReferences = bclReferences;
        _compiledMethodCache = compiledMethodCache;
    }

    /// <summary>
    /// Creates a new LIRToILCompiler instance for compiling a single method.
    /// </summary>
    private LIRToILCompiler CreateILCompiler()
    {
        return new LIRToILCompiler(_metadataBuilder, _typeReferenceRegistry, _memberReferenceRegistry, _bclReferences, _compiledMethodCache);
    }

    #region Public API - Entry Points

    public MethodDefinitionHandle TryCompileMethod(TypeBuilder typeBuilder, string methodName, Node node, Scope scope, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        // Extract params and body from the node based on its type
        NodeList<Node>? functionParams = null;
        Node bodyNode = node;
        
        if (node is FunctionDeclaration funcDecl)
        {
            functionParams = funcDecl.Params;
            bodyNode = funcDecl.Body;
        }
        else if (node is Acornima.Ast.MethodDefinition classMethDef && classMethDef.Value is FunctionExpression methodFuncExpr)
        {
            functionParams = methodFuncExpr.Params;
            bodyNode = classMethDef; // HIRBuilder handles MethodDefinition
        }

        // Check for simple identifier parameters only (no defaults, destructuring, rest)
        if (functionParams.HasValue && !AllParamsAreSimpleIdentifiers(functionParams.Value))
        {
            return default;
        }

        if (!TryLowerASTToLIR(bodyNode, scope, out var lirMethod))
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
            methodDescriptor.Parameters = Array.Empty<MethodParameterDescriptor>();
        }

        return CreateILCompiler().TryCompile(methodDescriptor, lirMethod!, methodBodyStreamEncoder);
    }

    public MethodDefinitionHandle TryCompileArrowFunction(string methodName, Node node, Scope scope, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        // Check for simple identifier parameters only (no defaults, destructuring, rest)
        if (node is ArrowFunctionExpression arrowFunc && !AllParamsAreSimpleIdentifiers(arrowFunc.Params))
        {
            return default;
        }

        if (!TryLowerASTToLIR(node, scope, out var lirMethod))
        {
            return default;
        }

        // Create the type builder for the arrow function
        var arrowTypeBuilder = new TypeBuilder(_metadataBuilder, "Functions", methodName);

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

        if (!TryLowerASTToLIR(ctorFunc, constructorScope, out var lirMethod))
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
        if (!TryLowerASTToLIR(node, scope, out var lirMethod))
        {
            return default;
        }

        // create the tools we need to generate the module type and method
        var programTypeBuilder = new TypeBuilder(_metadataBuilder, "Scripts", moduleName);

        MethodParameterDescriptor[] parameters = [
            new MethodParameterDescriptor("exports", typeof(object)),
            new MethodParameterDescriptor("require", typeof(JavaScriptRuntime.CommonJS.RequireDelegate)),
            new MethodParameterDescriptor("module", typeof(object)),
            new MethodParameterDescriptor("__filename", typeof(string)),
            new MethodParameterDescriptor("__dirname", typeof(string))
        ];
        var methodDescriptor = new MethodDescriptor(
            "Main",
            programTypeBuilder,
            parameters);

        methodDescriptor.ReturnsVoid = true;
        methodDescriptor.HasScopesParameter = false;

        var methodDefinitionHandle = CreateILCompiler().TryCompile(methodDescriptor, lirMethod!, methodBodyStreamEncoder);

        // Define the Script main type via TypeBuilder
        programTypeBuilder.AddTypeDefinition(
            TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit,
            _bclReferences.ObjectType);

        return methodDefinitionHandle;
    }

    #endregion

    #region Core Pipeline - AST to LIR to IL

    /// <summary>
    /// Returns true if all parameters are simple identifiers (no destructuring, defaults, or rest patterns).
    /// Used to determine if the IR pipeline can be used for a function.
    /// </summary>
    private static bool AllParamsAreSimpleIdentifiers(in NodeList<Node> parameters)
    {
        return parameters.All(param => param is Identifier);
    }

    private bool TryLowerASTToLIR(Node node, Scope scope, out MethodBodyIR? methodBody)
    {
        methodBody = null;

        if (!HIRBuilder.TryParseMethod(node, scope, out var hirMethod))
        {
            IR.IRPipelineMetrics.RecordFailure($"HIR parse failed for node type {node.Type}");
            return false;
        }

        if (!HIRToLIRLowerer.TryLower(hirMethod!, scope, out var lirMethod))
        {
            IR.IRPipelineMetrics.RecordFailure("HIR->LIR lowering failed");
            return false;
        }

        methodBody = lirMethod!;
        return true;
    }

    #endregion
}
