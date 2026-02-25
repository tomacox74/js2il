using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Acornima;
using Acornima.Ast;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services.Contracts;

/// <summary>
/// Emits strongly-typed .NET contract interfaces for CommonJS <c>module.exports</c>.
///
/// Design goals:
/// - Minimal, conservative shape inference (safe defaults to <see cref="object"/>)
/// - Hosting-friendly: exports contracts implement <see cref="IDisposable"/>, nested objects/instances implement <see cref="Js2IL.Runtime.IJsHandle"/>
/// - Exports contracts are annotated with <see cref="Js2IL.Runtime.JsModuleAttribute"/> so <see cref="Js2IL.Runtime.JsEngine.LoadModule{TExports}()"/> can resolve the module id.
/// </summary>
internal sealed class ModuleExportsContractEmitter
{
    private readonly MetadataBuilder _metadata;
    private readonly BaseClassLibraryReferences _bcl;
    private readonly TypeReferenceRegistry _typeRefs;

    public ModuleExportsContractEmitter(MetadataBuilder metadataBuilder, BaseClassLibraryReferences bclReferences)
    {
        _metadata = metadataBuilder ?? throw new ArgumentNullException(nameof(metadataBuilder));
        _bcl = bclReferences ?? throw new ArgumentNullException(nameof(bclReferences));
        _typeRefs = bclReferences.TypeReferenceRegistry;
    }

    public void Emit(Modules modules, string assemblyName)
    {
        ArgumentNullException.ThrowIfNull(modules);
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyName);

        var rootModulePath = modules.rootModule.Path;

        foreach (var module in modules._modules.Values)
        {
            var moduleId = JavaScriptRuntime.CommonJS.ModuleName.GetModuleIdForManifestFromPath(module.Path, rootModulePath);
            var isRoot = ReferenceEquals(module, modules.rootModule);

            if (!TryGetModuleExportsObject(module.Ast, out var exportsObject))
            {
                continue;
            }

            EmitModuleContracts(module, assemblyName, moduleId, isRoot, exportsObject);
        }
    }

    private void EmitModuleContracts(ModuleDefinition module, string assemblyName, string moduleId, bool isRootModule, ObjectExpression exportsObject)
    {
        var symbolTable = module.SymbolTable;

        var (contractNamespace, exportsInterfaceName) = GetExportsContractName(assemblyName, moduleId, isRootModule);

        var topLevel = BuildTopLevelDeclarationIndex(module.Ast);

        // IMPORTANT: Metadata ordering. In ECMA-335, TypeDef.MethodList (and similar lists) must be non-decreasing.
        // If we emit MethodDefs for the exports interface before emitting the TypeDefs for instance/nested interfaces,
        // then later-created TypeDefs can end up with MethodList pointers that are greater than the exports TypeDef,
        // and the CLR will refuse to load the generated types.
        //
        // Strategy:
        // 1) Emit all dependent TypeDefs first (class instance + nested object interfaces)
        // 2) Then emit exports interface MethodDefs/PropertyDefs and add its TypeDef last.

        // Emit class instance interfaces first so function inference can reference them without creating new TypeDefs later.
        var instanceInterfacesByClassName = new Dictionary<string, TypeDefinitionHandle>(StringComparer.Ordinal);
        foreach (var kvp in topLevel.Classes.OrderBy(kvp => kvp.Key, StringComparer.Ordinal))
        {
            var className = kvp.Key;
            var classDecl = kvp.Value;
            instanceInterfacesByClassName[className] = EmitHandleInterface(
                contractNamespace,
                "I" + ToPascalCase(className),
                members: classDecl,
                symbolTable: symbolTable,
                classNameForFields: className);
        }

        TypeDefinitionHandle? TryGetClassInstanceInterface(string className)
        {
            return instanceInterfacesByClassName.TryGetValue(className, out var handle) ? handle : null;
        }

        // Emit nested-object interfaces that are directly exported (module.exports = { nested: { ... } }).
        var nestedInterfacesByExportName = new Dictionary<string, TypeDefinitionHandle>(StringComparer.Ordinal);
        foreach (var prop in exportsObject.Properties)
        {
            if (prop is not Property p)
            {
                continue;
            }

            if (!TryGetPropertyName(p.Key, out var exportName))
            {
                continue;
            }

            if (p.Value is ObjectExpression nestedObj)
            {
                var nestedInterfaceName = "I" + ToPascalCase(exportName);
                var nestedType = EmitHandleInterface(contractNamespace, nestedInterfaceName, members: nestedObj, symbolTable: symbolTable, classNameForFields: null);
                nestedInterfacesByExportName[exportName] = nestedType;
            }
        }

        // Now emit the exports interface.
        var exportsTypeBuilder = new TypeBuilder(_metadata, contractNamespace, exportsInterfaceName);
        PropertyDefinitionHandle firstExportsProperty = default;

        // Add members (methods + property getters) before adding the TypeDef.
        foreach (var prop in exportsObject.Properties)
        {
            if (prop is not Property p)
            {
                continue;
            }

            if (!TryGetPropertyName(p.Key, out var exportName))
            {
                continue;
            }

            // Determine the exported value expression.
            var valueNode = p.Value;

            // Shorthand: { version } -> Property with key 'version' and value Identifier 'version'.
            // We treat it the same as an explicit value.

            if (TryResolveExportAsClass(valueNode, topLevel, out var className))
            {
                var instanceInterface = TryGetClassInstanceInterface(className);
                if (!instanceInterface.HasValue)
                {
                    // If we can't resolve the class instance interface, fall back to object.
                    var phFallback = EmitReadOnlyProperty(exportsTypeBuilder, ToPascalCase(exportName), TypeOrHandle.FromClr(typeof(object)));
                    if (firstExportsProperty.IsNil)
                    {
                        firstExportsProperty = phFallback;
                    }
                    continue;
                }

                var openCtorRef = _typeRefs.GetOrAdd(typeof(Js2IL.Runtime.IJsConstructor<>));
                var ph = EmitReadOnlyProperty(
                    exportsTypeBuilder,
                    ToPascalCase(exportName),
                    TypeOrHandle.FromGenericInstantiation(openCtorRef, instanceInterface.Value));
                if (firstExportsProperty.IsNil)
                {
                    firstExportsProperty = ph;
                }
                continue;
            }

            if (TryResolveExportAsFunction(valueNode, topLevel, out var functionNode, out var functionNameForInference))
            {
                var methodName = ToPascalCase(exportName);
                var method = BuildContractMethodFromFunction(methodName, functionNode, topLevel, instanceInterfacesByClassName, ensureClassInstanceInterface: null, symbolTable: symbolTable);
                EmitInterfaceMethod(exportsTypeBuilder, method);
                continue;
            }

            if (valueNode is ObjectExpression nestedObj)
            {
                // Nested exported object: interface was emitted in the pre-pass.
                if (!nestedInterfacesByExportName.TryGetValue(exportName, out var nestedType))
                {
                    // Shouldn't happen, but fall back to object.
                    var phFallback = EmitReadOnlyProperty(exportsTypeBuilder, ToPascalCase(exportName), TypeOrHandle.FromClr(typeof(object)));
                    if (firstExportsProperty.IsNil)
                    {
                        firstExportsProperty = phFallback;
                    }
                    continue;
                }

                var ph = EmitReadOnlyProperty(exportsTypeBuilder, ToPascalCase(exportName), TypeOrHandle.FromHandle(nestedType));
                if (firstExportsProperty.IsNil)
                {
                    firstExportsProperty = ph;
                }
                continue;
            }

            // Default: exported value projected as a read-only property.
            // Prefer stable binding type from symbol table when available (e.g. const x = complexExpr).
            TypeOrHandle clrType;
            if (valueNode is Identifier exportedId
                && symbolTable?.Root is Js2IL.SymbolTables.Scope globalScope
                && globalScope.Bindings.TryGetValue(exportedId.Name, out var exportedBinding)
                && exportedBinding.IsStableType
                && exportedBinding.ClrType != null)
            {
                clrType = TypeOrHandle.FromClr(MapClrType(exportedBinding.ClrType));
            }
            else
            {
                clrType = InferClrTypeFromExpression(valueNode, topLevel, classFields: null, instanceInterfacesByClassName, ensureClassInstanceInterface: null);
            }
            var propHandle = EmitReadOnlyProperty(exportsTypeBuilder, ToPascalCase(exportName), clrType);
            if (firstExportsProperty.IsNil)
            {
                firstExportsProperty = propHandle;
            }
        }

        var exportsTypeDef = exportsTypeBuilder.AddTypeDefinition(
            TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract,
            default);

        if (!firstExportsProperty.IsNil)
        {
            _metadata.AddPropertyMap(exportsTypeDef, firstExportsProperty);
        }

        // Exports contracts should be disposable so they can shut down the runtime.
        _metadata.AddInterfaceImplementation(exportsTypeDef, _typeRefs.GetOrAdd(typeof(IDisposable)));

        AddJsModuleAttribute(exportsTypeDef, moduleId);
    }

    private TypeDefinitionHandle EmitHandleInterface(
        string @namespace,
        string interfaceName,
        object? members,
        SymbolTable? symbolTable,
        string? classNameForFields)
    {
        var typeBuilder = new TypeBuilder(_metadata, @namespace, interfaceName);
        PropertyDefinitionHandle firstProperty = default;

        Dictionary<string, Type>? stableFields = null;
        if (!string.IsNullOrWhiteSpace(classNameForFields) && symbolTable?.Root is Js2IL.SymbolTables.Scope rootScope)
        {
            var classScope = FindClassScope(rootScope, classNameForFields!);
            stableFields = classScope?.StableInstanceFieldClrTypes;
        }

        if (members is ClassDeclaration classDecl)
        {
            // Instance field properties (stable inferred only)
            if (stableFields != null)
            {
                foreach (var field in stableFields.OrderBy(kvp => kvp.Key, StringComparer.Ordinal))
                {
                    var propName = ToPascalCase(field.Key);
                    var propType = TypeOrHandle.FromClr(MapClrType(field.Value));
                    var ph = EmitReadOnlyProperty(typeBuilder, propName, propType);
                    if (firstProperty.IsNil)
                    {
                        firstProperty = ph;
                    }
                }
            }

            // Instance methods
            foreach (var element in classDecl.Body.Body)
            {
                if (element is not Acornima.Ast.MethodDefinition md)
                {
                    continue;
                }

                if (md.Kind == PropertyKind.Constructor)
                {
                    continue;
                }

                if (!TryGetPropertyName(md.Key, out var name))
                {
                    continue;
                }

                if (md.Value is not FunctionExpression fn)
                {
                    continue;
                }

                var methodName = ToPascalCase(name);
                var method = BuildContractMethodFromFunction(
                    methodName,
                    fn,
                    topLevelIndex: null,
                    instanceInterfacesByClassName: null,
                    ensureClassInstanceInterface: null,
                    classFields: stableFields,
                    symbolTable: symbolTable,
                    scopeLookupNode: md);
                EmitInterfaceMethod(typeBuilder, method);
            }
        }
        else if (members is ObjectExpression obj)
        {
            // Nested object interface projected as a handle.
            foreach (var prop in obj.Properties)
            {
                if (prop is not Property p)
                {
                    continue;
                }

                if (!TryGetPropertyName(p.Key, out var memberName))
                {
                    continue;
                }

                var valueNode = p.Value;

                if (valueNode is FunctionExpression fn)
                {
                    var method = BuildContractMethodFromFunction(
                        ToPascalCase(memberName),
                        fn,
                        topLevelIndex: null,
                        instanceInterfacesByClassName: null,
                        ensureClassInstanceInterface: null);
                    EmitInterfaceMethod(typeBuilder, method);
                    continue;
                }

                var clrType = InferClrTypeFromExpression(
                    valueNode,
                    topLevelIndex: null,
                    classFields: null,
                    instanceInterfacesByClassName: null,
                    ensureClassInstanceInterface: null);
                var ph = EmitReadOnlyProperty(typeBuilder, ToPascalCase(memberName), clrType);
                if (firstProperty.IsNil)
                {
                    firstProperty = ph;
                }
            }
        }

        var typeDef = typeBuilder.AddTypeDefinition(
            TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract,
            default);

        if (!firstProperty.IsNil)
        {
            _metadata.AddPropertyMap(typeDef, firstProperty);
        }

        // Handles should be projected via JsHandleProxy.
        _metadata.AddInterfaceImplementation(typeDef, _typeRefs.GetOrAdd(typeof(Js2IL.Runtime.IJsHandle)));

        return typeDef;
    }

    private static Js2IL.SymbolTables.Scope? FindClassScope(Js2IL.SymbolTables.Scope scope, string className)
    {
        if (scope.Kind == ScopeKind.Class && string.Equals(scope.Name, className, StringComparison.Ordinal))
        {
            return scope;
        }

        foreach (var child in scope.Children)
        {
            var found = FindClassScope(child, className);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private readonly record struct TopLevelIndex(
        IReadOnlyDictionary<string, FunctionDeclaration> Functions,
        IReadOnlyDictionary<string, ClassDeclaration> Classes,
        IReadOnlyDictionary<string, Expression> VariableInitializers);

    private static TopLevelIndex BuildTopLevelDeclarationIndex(Acornima.Ast.Program program)
    {
        var functions = new Dictionary<string, FunctionDeclaration>(StringComparer.Ordinal);
        var classes = new Dictionary<string, ClassDeclaration>(StringComparer.Ordinal);
        var vars = new Dictionary<string, Expression>(StringComparer.Ordinal);

        foreach (var stmt in program.Body)
        {
            switch (stmt)
            {
                case FunctionDeclaration fd when fd.Id is Identifier id:
                    functions[id.Name] = fd;
                    break;
                case ClassDeclaration cd when cd.Id is Identifier id:
                    classes[id.Name] = cd;
                    break;
                case VariableDeclaration vd:
                    foreach (var decl in vd.Declarations)
                    {
                        if (decl.Id is Identifier vid && decl.Init is Expression init)
                        {
                            vars[vid.Name] = init;
                        }
                    }
                    break;
            }
        }

        return new TopLevelIndex(functions, classes, vars);
    }

    private static bool TryGetModuleExportsObject(Acornima.Ast.Program program, out ObjectExpression exports)
    {
        exports = null!;

        // Walk top-level statements and take the last assignment to module.exports.
        ObjectExpression? last = null;

        foreach (var stmt in program.Body)
        {
            if (stmt is not ExpressionStatement es)
            {
                continue;
            }

            if (es.Expression is not AssignmentExpression assign)
            {
                continue;
            }

            if (!IsModuleExportsLValue(assign.Left))
            {
                continue;
            }

            if (assign.Right is ObjectExpression obj)
            {
                last = obj;
            }
        }

        if (last == null)
        {
            return false;
        }

        exports = last;
        return true;
    }

    private static bool IsModuleExportsLValue(Node left)
    {
        // module.exports = ...
        if (left is MemberExpression { Object: Identifier { Name: "module" }, Property: Identifier { Name: "exports" } })
        {
            return true;
        }

        // module["exports"] = ...
        if (left is MemberExpression { Object: Identifier { Name: "module" }, Property: Literal { Value: "exports" } })
        {
            return true;
        }

        return false;
    }

    private static bool TryGetPropertyName(Expression key, out string name)
    {
        switch (key)
        {
            case Identifier id:
                name = id.Name;
                return true;
            case Literal lit when lit.Value is string s:
                name = s;
                return true;
            default:
                name = string.Empty;
                return false;
        }
    }

    private static (string Namespace, string ExportsInterfaceName) GetExportsContractName(string assemblyName, string moduleId, bool isRootModule)
    {
        // See docs/DotNetLibraryHosting.md "Naming generated export contracts for nested modules".
        var rootNamespace = $"Js2IL.{assemblyName}";

        if (isRootModule)
        {
            return (rootNamespace, "I" + ToPascalCase(assemblyName) + "Exports");
        }

        var segments = moduleId.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
        {
            return (rootNamespace, "IExports");
        }

        var namespaceSegments = segments.Length > 1
            ? segments.Take(segments.Length - 1).Select(ToPascalCase).ToArray()
            : Array.Empty<string>();

        var ns = namespaceSegments.Length == 0
            ? rootNamespace
            : rootNamespace + "." + string.Join(".", namespaceSegments);

        var last = segments[^1];
        var displayName = (string.Equals(last, "index", StringComparison.OrdinalIgnoreCase) && segments.Length > 1)
            ? segments[^2]
            : last;

        var iface = "I" + ToPascalCase(displayName) + "Exports";
        return (ns, iface);
    }

    private static bool TryResolveExportAsClass(Node valueNode, TopLevelIndex topLevel, out string className)
    {
        className = string.Empty;

        if (valueNode is Identifier id && topLevel.Classes.ContainsKey(id.Name))
        {
            className = id.Name;
            return true;
        }

        if (valueNode is ClassExpression ce && ce.Id is Identifier cid)
        {
            className = cid.Name;
            return true;
        }

        return false;
    }

    private static bool TryResolveExportAsFunction(Node valueNode, TopLevelIndex topLevel, out Node functionNode, out string? nameForInference)
    {
        nameForInference = null;

        switch (valueNode)
        {
            case Identifier id when topLevel.Functions.TryGetValue(id.Name, out var decl):
                functionNode = decl;
                nameForInference = id.Name;
                return true;
            case FunctionExpression fe:
                functionNode = fe;
                nameForInference = fe.Id is Identifier fid ? fid.Name : null;
                return true;
            case ArrowFunctionExpression af:
                functionNode = af;
                nameForInference = null;
                return true;
            default:
                functionNode = null!;
                return false;
        }
    }

    private sealed record ContractMethod(string Name, IReadOnlyList<string> ParamNames, IReadOnlyList<TypeOrHandle> ParamTypes, TypeOrHandle ReturnType);

    private readonly record struct TypeOrHandle(Type? ClrType, EntityHandle? Handle, TypeReferenceHandle? OpenGenericTypeRef, EntityHandle? GenericArgHandle, Type? GenericArgClrType)
    {
        public static TypeOrHandle FromClr(Type type) => new(type, null, null, null, null);
        public static TypeOrHandle FromHandle(EntityHandle handle) => new(null, handle, null, null, null);

        public static TypeOrHandle FromGenericInstantiation(TypeReferenceHandle openGenericTypeRef, EntityHandle genericArg)
            => new(null, null, openGenericTypeRef, genericArg, null);

        public static TypeOrHandle FromGenericInstantiation(TypeReferenceHandle openGenericTypeRef, Type genericArgClrType)
            => new(null, null, openGenericTypeRef, null, genericArgClrType);
    }

    private ContractMethod BuildContractMethodFromFunction(
        string methodName,
        Node functionNode,
        TopLevelIndex? topLevelIndex,
        Dictionary<string, TypeDefinitionHandle>? instanceInterfacesByClassName,
        Func<string, TypeDefinitionHandle>? ensureClassInstanceInterface,
        Dictionary<string, Type>? classFields = null,
        SymbolTable? symbolTable = null,
        Node? scopeLookupNode = null)
    {
        var paramNames = new List<string>();
        foreach (var p in GetFunctionParams(functionNode))
        {
            if (p is Identifier pid)
            {
                paramNames.Add(pid.Name);
            }
            else
            {
                // Destructuring, defaults, etc. Keep names stable but generic.
                paramNames.Add("arg" + (paramNames.Count + 1));
            }
        }

        // Prefer stable return type from symbol table when available.
        TypeOrHandle baseReturnType;
        var lookupNode = scopeLookupNode ?? functionNode;
        var scope = symbolTable?.FindScopeByAstNode(lookupNode);
        if (scope?.StableReturnClrType != null)
        {
            baseReturnType = TypeOrHandle.FromClr(MapClrType(scope.StableReturnClrType));
        }
        else
        {
            baseReturnType = InferReturnTypeFromFunction(functionNode, topLevelIndex, classFields, instanceInterfacesByClassName, ensureClassInstanceInterface);
        }

        var returnType = WrapReturnTypeForAsyncFunction(functionNode, baseReturnType);

        // Very conservative parameter typing: if the return type is numeric, assume numeric params.
        var paramTypes = new List<TypeOrHandle>(paramNames.Count);
        for (var i = 0; i < paramNames.Count; i++)
        {
            if (baseReturnType.ClrType == typeof(double))
            {
                paramTypes.Add(TypeOrHandle.FromClr(typeof(double)));
            }
            else
            {
                paramTypes.Add(TypeOrHandle.FromClr(typeof(object)));
            }
        }

        return new ContractMethod(methodName, paramNames, paramTypes, returnType);
    }

    private TypeOrHandle WrapReturnTypeForAsyncFunction(Node functionNode, TypeOrHandle baseReturnType)
    {
        var isAsync = functionNode switch
        {
            FunctionDeclaration fd => fd.Async,
            FunctionExpression fe => fe.Async,
            ArrowFunctionExpression af => af.Async,
            _ => false
        };

        if (!isAsync)
        {
            return baseReturnType;
        }

        // JS async functions always return a Promise.
        // Hosting contracts should expose that as Task/Task<T>.
        if (baseReturnType.ClrType == typeof(void))
        {
            return TypeOrHandle.FromClr(typeof(Task));
        }

        var taskOfT = _typeRefs.GetOrAdd(typeof(Task<>));

        if (baseReturnType.ClrType != null)
        {
            return TypeOrHandle.FromGenericInstantiation(taskOfT, baseReturnType.ClrType);
        }

        if (baseReturnType.Handle.HasValue)
        {
            return TypeOrHandle.FromGenericInstantiation(taskOfT, baseReturnType.Handle.Value);
        }

        return TypeOrHandle.FromGenericInstantiation(taskOfT, typeof(object));
    }

    private static IEnumerable<Node> GetFunctionParams(Node functionNode)
    {
        return functionNode switch
        {
            FunctionDeclaration fd => fd.Params,
            FunctionExpression fe => fe.Params,
            ArrowFunctionExpression af => af.Params,
            _ => Array.Empty<Node>()
        };
    }

    private static TypeOrHandle InferReturnTypeFromFunction(
        Node functionNode,
        TopLevelIndex? topLevelIndex,
        Dictionary<string, Type>? classFields,
        Dictionary<string, TypeDefinitionHandle>? instanceInterfacesByClassName,
        Func<string, TypeDefinitionHandle>? ensureClassInstanceInterface)
    {
        // Arrow with expression body
        if (functionNode is ArrowFunctionExpression { Body: Expression exprBody })
        {
            return InferClrTypeFromExpression(exprBody, topLevelIndex, classFields, instanceInterfacesByClassName, ensureClassInstanceInterface);
        }

        // Block body: look for a single return statement.
        if (GetFunctionBody(functionNode) is BlockStatement block)
        {
            ReturnStatement? onlyReturn = null;
            var returnCount = 0;

            foreach (var stmt in block.Body)
            {
                if (stmt is ReturnStatement rs)
                {
                    returnCount++;
                    onlyReturn ??= rs;
                }
            }

            if (returnCount == 0)
            {
                return TypeOrHandle.FromClr(typeof(void));
            }

            if (returnCount == 1 && onlyReturn != null)
            {
                if (onlyReturn.Argument is Expression arg)
                {
                    return InferClrTypeFromExpression(arg, topLevelIndex, classFields, instanceInterfacesByClassName, ensureClassInstanceInterface);
                }

                return TypeOrHandle.FromClr(typeof(void));
            }
        }

        return TypeOrHandle.FromClr(typeof(object));
    }

    private static Node? GetFunctionBody(Node functionNode)
    {
        return functionNode switch
        {
            FunctionDeclaration fd => fd.Body,
            FunctionExpression fe => fe.Body,
            ArrowFunctionExpression af => af.Body,
            _ => null
        };
    }

    private static TypeOrHandle InferClrTypeFromExpression(
        Node expr,
        TopLevelIndex? topLevelIndex,
        Dictionary<string, Type>? classFields,
        Dictionary<string, TypeDefinitionHandle>? instanceInterfacesByClassName,
        Func<string, TypeDefinitionHandle>? ensureClassInstanceInterface)
    {
        switch (expr)
        {
            case Literal lit:
                return lit.Value switch
                {
                    string => TypeOrHandle.FromClr(typeof(string)),
                    bool => TypeOrHandle.FromClr(typeof(bool)),
                    int or long or float or double or decimal => TypeOrHandle.FromClr(typeof(double)),
                    _ => TypeOrHandle.FromClr(typeof(object))
                };

            case BinaryExpression be:
                // Conservative: arithmetic returns double, comparisons return bool.
                return be.Operator is Operator.Equality or Operator.Inequality or Operator.StrictEquality or Operator.StrictInequality
                    or Operator.GreaterThan or Operator.GreaterThanOrEqual or Operator.LessThan or Operator.LessThanOrEqual
                    ? TypeOrHandle.FromClr(typeof(bool))
                    : TypeOrHandle.FromClr(typeof(double));

            case UnaryExpression ue when ue.Operator == Operator.LogicalNot:
                return TypeOrHandle.FromClr(typeof(bool));

            case Identifier id when topLevelIndex != null:
                if (topLevelIndex.Value.VariableInitializers.TryGetValue(id.Name, out var init))
                {
                    return InferClrTypeFromExpression(init, topLevelIndex, classFields, instanceInterfacesByClassName, ensureClassInstanceInterface);
                }
                return TypeOrHandle.FromClr(typeof(object));

            case NewExpression ne when ne.Callee is Identifier ctorId && instanceInterfacesByClassName != null:
                // new Counter(...) => ICounter (handle) if we have a known instance contract.
                if (instanceInterfacesByClassName.TryGetValue(ctorId.Name, out var instanceHandle) && !instanceHandle.IsNil)
                {
                    return TypeOrHandle.FromHandle(instanceHandle);
                }
                return TypeOrHandle.FromClr(typeof(object));

            case MemberExpression { Object: ThisExpression, Property: Identifier pid } when classFields != null:
                if (classFields.TryGetValue(pid.Name, out var fieldType))
                {
                    return TypeOrHandle.FromClr(MapClrType(fieldType));
                }
                return TypeOrHandle.FromClr(typeof(object));

            default:
                return TypeOrHandle.FromClr(typeof(object));
        }
    }

    private static Type MapClrType(Type type)
    {
        if (type == typeof(double) || type == typeof(bool) || type == typeof(string))
        {
            return type;
        }

        return typeof(object);
    }

    private void EmitInterfaceMethod(TypeBuilder typeBuilder, ContractMethod method)
    {
        ArgumentNullException.ThrowIfNull(typeBuilder);

        // Emit parameter metadata first so we can pass the correct ParamList handle.
        ParameterHandle firstParam = default;
        for (ushort i = 0; i < method.ParamNames.Count; i++)
        {
            var name = method.ParamNames[i] ?? string.Empty;
            var handle = _metadata.AddParameter(
                attributes: ParameterAttributes.None,
                name: _metadata.GetOrAddString(name),
                sequenceNumber: (ushort)(i + 1));

            if (i == 0)
            {
                firstParam = handle;
            }
        }

        var signature = BuildMethodSignature(
            isInstance: true,
            paramNames: method.ParamNames,
            paramTypes: method.ParamTypes,
            returnType: method.ReturnType);

        typeBuilder.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
            method.Name,
            signature,
            bodyOffset: -1,
            parameterList: firstParam);
    }

    private PropertyDefinitionHandle EmitReadOnlyProperty(TypeBuilder typeBuilder, string propertyName, TypeOrHandle propertyType)
    {
        // get_PropertyName()
        var getterName = "get_" + propertyName;

        var getterSig = BuildMethodSignature(
            isInstance: true,
            paramNames: Array.Empty<string>(),
            paramTypes: Array.Empty<TypeOrHandle>(),
            returnType: propertyType);

        var getter = typeBuilder.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot,
            getterName,
            getterSig,
            bodyOffset: -1);

        // Property signature
        var propSig = BuildPropertySignature(propertyType);
        var propHandle = _metadata.AddProperty(
            attributes: PropertyAttributes.None,
            name: _metadata.GetOrAddString(propertyName),
            signature: propSig);

        _metadata.AddMethodSemantics(propHandle, MethodSemanticsAttributes.Getter, getter);

        // No setters (exports are read-only).

        return propHandle;
    }

    private BlobHandle BuildPropertySignature(TypeOrHandle returnType)
    {
        var sig = new BlobBuilder();
        new BlobEncoder(sig)
            .PropertySignature(isInstanceProperty: true)
            .Parameters(0,
                returnTypeEncoder => EncodeReturnType(returnTypeEncoder, returnType),
                parameters => { });

        return _metadata.GetOrAddBlob(sig);
    }

    private BlobHandle BuildMethodSignature(bool isInstance, IReadOnlyList<string> paramNames, IReadOnlyList<TypeOrHandle> paramTypes, TypeOrHandle returnType)
    {
        var sig = new BlobBuilder();
        var encoder = new BlobEncoder(sig)
            .MethodSignature(isInstanceMethod: isInstance);

        encoder.Parameters(
            parameterCount: paramTypes.Count,
            returnType: r => EncodeReturnType(r, returnType),
            parameters: p =>
            {
                for (var i = 0; i < paramTypes.Count; i++)
                {
                    EncodeParamType(p.AddParameter().Type(), paramTypes[i]);
                }
            });

        return _metadata.GetOrAddBlob(sig);
    }

    private void EncodeReturnType(ReturnTypeEncoder encoder, TypeOrHandle type)
    {
        if (type.ClrType == typeof(void))
        {
            encoder.Void();
            return;
        }

        EncodeParamType(encoder.Type(), type);
    }

    private void EncodeParamType(SignatureTypeEncoder encoder, TypeOrHandle type)
    {
        if (type.OpenGenericTypeRef.HasValue && (type.GenericArgHandle.HasValue || type.GenericArgClrType != null))
        {
            var inst = encoder.GenericInstantiation(type.OpenGenericTypeRef.Value, genericArgumentCount: 1, isValueType: false);
            var arg = inst.AddArgument();

            if (type.GenericArgHandle.HasValue)
            {
                arg.Type(type.GenericArgHandle.Value, isValueType: false);
            }
            else
            {
                // At this point, type.GenericArgClrType is guaranteed non-null by the condition above.
                EncodeParamType(arg, TypeOrHandle.FromClr(type.GenericArgClrType!));
            }
            return;
        }

        if (type.ClrType != null)
        {
            if (type.ClrType == typeof(object)) { encoder.Object(); return; }
            if (type.ClrType == typeof(string)) { encoder.String(); return; }
            if (type.ClrType == typeof(double)) { encoder.Double(); return; }
            if (type.ClrType == typeof(bool)) { encoder.Boolean(); return; }

            // Non-primitive reference types (e.g., Task)
            if (!type.ClrType.IsGenericType)
            {
                encoder.Type(_typeRefs.GetOrAdd(type.ClrType), isValueType: false);
                return;
            }

            encoder.Object();
            return;
        }

        if (type.Handle.HasValue)
        {
            encoder.Type(type.Handle.Value, isValueType: false);
            return;
        }

        encoder.Object();
    }

    private void AddJsModuleAttribute(TypeDefinitionHandle exportsTypeDef, string moduleId)
    {
        var valueBlob = CreateSingleStringCustomAttributeValue(moduleId);

        _metadata.AddCustomAttribute(
            parent: exportsTypeDef,
            constructor: _bcl.JsModuleAttribute_Ctor_Ref,
            value: valueBlob);
    }

    private BlobHandle CreateSingleStringCustomAttributeValue(string value)
    {
        var blob = new BlobBuilder();
        blob.WriteUInt16(0x0001);
        WriteSerString(blob, value);
        blob.WriteUInt16(0);
        return _metadata.GetOrAddBlob(blob);
    }

    private static void WriteSerString(BlobBuilder blob, string value)
    {
        var utf8 = Encoding.UTF8.GetBytes(value);
        WriteCompressedUInt32(blob, (uint)utf8.Length);
        blob.WriteBytes(utf8);
    }

    private static void WriteCompressedUInt32(BlobBuilder blob, uint value)
    {
        if (value <= 0x7Fu)
        {
            blob.WriteByte((byte)value);
            return;
        }

        if (value <= 0x3FFFu)
        {
            blob.WriteByte((byte)((value >> 8) | 0x80u));
            blob.WriteByte((byte)(value & 0xFFu));
            return;
        }

        if (value <= 0x1FFFFFFFu)
        {
            blob.WriteByte((byte)((value >> 24) | 0xC0u));
            blob.WriteByte((byte)((value >> 16) & 0xFFu));
            blob.WriteByte((byte)((value >> 8) & 0xFFu));
            blob.WriteByte((byte)(value & 0xFFu));
            return;
        }

        throw new ArgumentOutOfRangeException(nameof(value), "Value too large for compressed integer encoding.");
    }

    private static string ToPascalCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "";
        }

        // Split on common separators and non-alphanumerics.
        var parts = new List<string>();
        var sb = new StringBuilder();

        foreach (var c in value)
        {
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(c);
            }
            else
            {
                if (sb.Length > 0)
                {
                    parts.Add(sb.ToString());
                    sb.Clear();
                }
            }
        }

        if (sb.Length > 0)
        {
            parts.Add(sb.ToString());
        }

        if (parts.Count == 0)
        {
            return value;
        }

        var result = new StringBuilder();
        foreach (var part in parts)
        {
            if (part.Length == 0) continue;
            result.Append(char.ToUpperInvariant(part[0]));
            if (part.Length > 1)
            {
                result.Append(part.Substring(1));
            }
        }

        return result.ToString();
    }
}
