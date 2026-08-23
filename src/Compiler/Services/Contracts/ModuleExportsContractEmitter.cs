using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Acornima;
using Acornima.Ast;
using Jroc.SymbolTables;
using Jroc.Utilities.Ecma335;

namespace Jroc.Services.Contracts;

internal sealed record ModuleExportsContractEmissionPlan(
    IReadOnlyDictionary<string, TypeDefinitionHandle> ExportContractHandles,
    int MethodDefinitionCount);

/// <summary>
/// Emits strongly-typed .NET contract interfaces for public module exports.
///
/// Design goals:
/// - Minimal, conservative shape inference (safe defaults to <see cref="object"/>)
/// - Hosting-friendly: exports, constructor, nested object, and instance contracts implement BCL lifetime interfaces only.
/// - Contracts carry generated metadata attributes so the runtime can resolve module/export names without leaking runtime types.
/// </summary>
internal sealed class ModuleExportsContractEmitter
{
    private const string FallbackObjectContractKey = "\0object";
    private const string FallbackArrayContractKey = "\0array";
    private const string FallbackCallableContractKey = "\0callable";
    private const string FallbackConstructorContractKey = "\0constructor";
    private const string ConstructorContractKeyPrefix = "\0constructor:";

    private readonly MetadataBuilder _metadata;
    private readonly BaseClassLibraryReferences _bcl;
    private readonly TypeReferenceRegistry _typeRefs;
    private readonly GeneratedContractMetadataReferences _generatedMetadata;

    public ModuleExportsContractEmitter(
        MetadataBuilder metadataBuilder,
        BaseClassLibraryReferences bclReferences,
        GeneratedContractMetadataReferences generatedMetadata)
    {
        _metadata = metadataBuilder ?? throw new ArgumentNullException(nameof(metadataBuilder));
        _bcl = bclReferences ?? throw new ArgumentNullException(nameof(bclReferences));
        _typeRefs = bclReferences.TypeReferenceRegistry;
        _generatedMetadata = generatedMetadata ?? throw new ArgumentNullException(nameof(generatedMetadata));
    }

    public IReadOnlyDictionary<string, TypeDefinitionHandle> Emit(
        Modules modules,
        string assemblyName,
        IReadOnlyDictionary<string, PublicModuleExportShape> exportShapes,
        ModuleExportsContractEmissionPlan plan,
        IReadOnlyDictionary<string, TypeDefinitionHandle> moduleFacadeTypes,
        NestedTypeRelationshipRegistry nestedTypes)
    {
        ArgumentNullException.ThrowIfNull(modules);
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyName);
        ArgumentNullException.ThrowIfNull(exportShapes);
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(moduleFacadeTypes);
        ArgumentNullException.ThrowIfNull(nestedTypes);

        var emittedContracts = new Dictionary<string, TypeDefinitionHandle>(StringComparer.Ordinal);
        var firstMethodRow = _metadata.GetRowCount(TableIndex.MethodDef);

        foreach (var module in modules._modules.Values)
        {
            if (emittedContracts.ContainsKey(module.ModuleId))
            {
                continue;
            }

            if (!exportShapes.TryGetValue(module.ModuleId, out var exportShape)
                || !exportShape.HasExports)
            {
                continue;
            }

            if (!moduleFacadeTypes.TryGetValue(module.ModuleId, out var facadeType))
            {
                throw new InvalidOperationException(
                    $"Could not resolve generated facade type for exported module '{module.ModuleId}'.");
            }

            var isRoot = ReferenceEquals(module, modules.rootModule);
            var emittedContract = EmitModuleContracts(
                module,
                assemblyName,
                module.ModuleId,
                isRoot,
                exportShape,
                facadeType,
                nestedTypes);
            if (!plan.ExportContractHandles.TryGetValue(module.ModuleId, out var expectedContract)
                || emittedContract != expectedContract)
            {
                throw new InvalidOperationException(
                    $"Generated export contract TypeDef token mismatch for module '{module.ModuleId}'. " +
                    $"Expected 0x{MetadataTokens.GetToken(expectedContract):X8}, " +
                    $"got 0x{MetadataTokens.GetToken(emittedContract):X8}.");
            }

            emittedContracts[module.ModuleId] = emittedContract;
        }

        var emittedMethodCount = _metadata.GetRowCount(TableIndex.MethodDef) - firstMethodRow;
        if (emittedMethodCount != plan.MethodDefinitionCount)
        {
            throw new InvalidOperationException(
                $"Generated export contract MethodDef count mismatch. " +
                $"Expected {plan.MethodDefinitionCount}, emitted {emittedMethodCount}.");
        }

        return emittedContracts;
    }

    public ModuleExportsContractEmissionPlan Plan(
        Modules modules,
        IReadOnlyDictionary<string, PublicModuleExportShape> exportShapes,
        int firstTypeDefinitionRow)
    {
        ArgumentNullException.ThrowIfNull(modules);
        ArgumentNullException.ThrowIfNull(exportShapes);
        if (firstTypeDefinitionRow <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(firstTypeDefinitionRow));
        }

        var handles = new Dictionary<string, TypeDefinitionHandle>(StringComparer.Ordinal);
        var nextTypeDefinitionRow = firstTypeDefinitionRow;
        var methodDefinitionCount = 0;

        foreach (var module in modules._modules.Values)
        {
            if (handles.ContainsKey(module.ModuleId)
                || !exportShapes.TryGetValue(module.ModuleId, out var exportShape)
                || !exportShape.HasExports)
            {
                continue;
            }

            var topLevel = BuildTopLevelDeclarationIndex(module.Ast);
            nextTypeDefinitionRow += CountDependentContractTypes(module, exportShape, topLevel);
            handles[module.ModuleId] = MetadataTokens.TypeDefinitionHandle(nextTypeDefinitionRow++);
            methodDefinitionCount += CountModuleContractMethods(module, exportShape, topLevel);
        }

        return new ModuleExportsContractEmissionPlan(handles, methodDefinitionCount);
    }

    private TypeDefinitionHandle EmitModuleContracts(
        ModuleDefinition module,
        string assemblyName,
        string moduleId,
        bool isRootModule,
        PublicModuleExportShape exportShape,
        TypeDefinitionHandle facadeType,
        NestedTypeRelationshipRegistry nestedTypes)
    {
        var symbolTable = module.SymbolTable;

        var contractNamespace = GetContractNamespace(assemblyName, moduleId, isRootModule);

        var topLevels = new Dictionary<ModuleDefinition, TopLevelIndex>();
        TopLevelIndex GetTopLevel(ModuleDefinition sourceModule)
        {
            if (!topLevels.TryGetValue(sourceModule, out var topLevel))
            {
                topLevel = BuildTopLevelDeclarationIndex(sourceModule.Ast);
                topLevels[sourceModule] = topLevel;
            }

            return topLevel;
        }

        var topLevel = GetTopLevel(module);

        // IMPORTANT: Metadata ordering. In ECMA-335, TypeDef.MethodList (and similar lists) must be non-decreasing.
        // If we emit MethodDefs for the exports interface before emitting the TypeDefs for instance/nested interfaces,
        // then later-created TypeDefs can end up with MethodList pointers that are greater than the exports TypeDef,
        // and the CLR will refuse to load the generated types.
        //
        // Strategy:
        // 1) Emit all dependent TypeDefs first (class instance + nested object interfaces)
        // 2) Then emit exports interface MethodDefs/PropertyDefs and add its TypeDef last.

        var fallbackRequirements = GetFallbackContractRequirements(
            module,
            exportShape,
            topLevel);
        var instanceInterfacesByClassName =
            new Dictionary<string, TypeDefinitionHandle>(StringComparer.Ordinal);
        if (fallbackRequirements.HasFlag(FallbackContractKind.Object))
        {
            instanceInterfacesByClassName[FallbackObjectContractKey] = EmitDynamicObjectInterface(
                facadeType,
                nestedTypes);
        }
        if (fallbackRequirements.HasFlag(FallbackContractKind.Array))
        {
            instanceInterfacesByClassName[FallbackArrayContractKey] = EmitArrayInterface(
                string.Empty,
                "IArray",
                facadeType,
                nestedTypes);
        }
        if (fallbackRequirements.HasFlag(FallbackContractKind.Callable))
        {
            instanceInterfacesByClassName[FallbackCallableContractKey] = EmitCallableInterface(
                facadeType,
                nestedTypes);
        }
        if (fallbackRequirements.HasFlag(FallbackContractKind.Constructor))
        {
            instanceInterfacesByClassName[FallbackConstructorContractKey] = EmitFallbackConstructorInterface(
                facadeType,
                nestedTypes,
                instanceInterfacesByClassName[FallbackObjectContractKey]);
        }

        // Emit class instance and constructor contracts first so function inference can reference them.
        var classContracts = BuildClassContractDefinitions(module, exportShape, topLevel);
        foreach (var classContract in classContracts.OrderBy(contract => contract.ContractName, StringComparer.Ordinal))
        {
            instanceInterfacesByClassName[classContract.ContractName] = EmitHandleInterface(
                contractNamespace,
                "I" + ToPascalCase(classContract.ContractName),
                members: classContract.ClassNode,
                symbolTable: classContract.SourceModule.SymbolTable,
                classNameForFields: classContract.ScopeClassName,
                GetTopLevel(classContract.SourceModule),
                instanceInterfacesByClassName);
        }

        var constructorInterfacesByClassName = new Dictionary<string, TypeDefinitionHandle>(StringComparer.Ordinal);
        foreach (var classContract in classContracts.OrderBy(contract => contract.ContractName, StringComparer.Ordinal))
        {
            constructorInterfacesByClassName[classContract.ContractName] = EmitConstructorInterface(
                contractNamespace,
                "I" + ToPascalCase(classContract.ContractName) + "Constructor",
                classContract.ClassNode,
                TypeOrHandle.FromHandle(instanceInterfacesByClassName[classContract.ContractName]),
                classContract.SourceModule.SymbolTable,
                classContract.ScopeClassName,
                GetTopLevel(classContract.SourceModule),
                instanceInterfacesByClassName);
            instanceInterfacesByClassName[
                ConstructorContractKeyPrefix + classContract.ContractName] =
                constructorInterfacesByClassName[classContract.ContractName];
        }

        // Emit nested-object interfaces that are directly exported (module.exports = { nested: { ... } }).
        var nestedInterfacesByExportName = new Dictionary<string, TypeDefinitionHandle>(StringComparer.Ordinal);
        foreach (var member in exportShape.Members)
        {
            var memberTopLevel = ReferenceEquals(member.SourceModule, module)
                ? topLevel
                : GetTopLevel(member.SourceModule);
            var memberValueNode = GetValueNode(member.SourceNode);
            if (TryResolveObjectExpression(memberValueNode, memberTopLevel, out var nestedObj))
            {
                var nestedInterfaceName = "I" + ToPascalCase(member.ExportName);
                var nestedType = EmitHandleInterface(
                    contractNamespace,
                    nestedInterfaceName,
                    members: nestedObj,
                    symbolTable: member.SourceModule.SymbolTable,
                    classNameForFields: null,
                    memberTopLevel,
                    instanceInterfacesByClassName);
                nestedInterfacesByExportName[member.ExportName] = nestedType;
            }
            else if (TryResolveArrayExpression(memberValueNode, memberTopLevel, out _))
            {
                nestedInterfacesByExportName[member.ExportName] = EmitArrayInterface(
                    contractNamespace,
                    "I" + ToPascalCase(member.ExportName) + "Array");
            }
        }

        TypeDefinitionHandle? directValueInterface = null;
        var directContractValueNode = GetValueNode(exportShape.DirectValueSourceNode);
        if (exportShape.Kind != PublicExportShapeKind.Unknown)
        {
            if (TryResolveObjectExpression(directContractValueNode, topLevel, out var directObject))
            {
                directValueInterface = EmitHandleInterface(
                    contractNamespace,
                    "IValue",
                    directObject,
                    symbolTable,
                    classNameForFields: null,
                    topLevel,
                    instanceInterfacesByClassName);
            }
            else if (TryResolveArrayExpression(directContractValueNode, topLevel, out _))
            {
                directValueInterface = EmitArrayInterface(contractNamespace, "IValueArray");
            }
        }

        // Now emit the exports interface.
        var exportsTypeBuilder = new TypeBuilder(_metadata, string.Empty, "IExports");
        PropertyDefinitionHandle firstExportsProperty = default;

        // Add members (methods + property getters) before adding the TypeDef.
        if (exportShape.DirectValueKind != PublicExportValueKind.None
            || exportShape.Kind == PublicExportShapeKind.Unknown)
        {
            var directValueNode = GetValueNode(exportShape.DirectValueSourceNode);
            var directClassName = string.Empty;
            var fallbackPropertyName = GetAvailableContractMemberName(
                "Value",
                exportShape.Members.Select(member => ToPascalCase(member.ExportName)));
            TypeOrHandle valueType;
            if (exportShape.Kind == PublicExportShapeKind.Unknown)
            {
                valueType = TypeOrHandle.FromClr(typeof(object));
            }
            else if (directValueInterface.HasValue)
            {
                valueType = TypeOrHandle.FromHandle(directValueInterface.Value);
            }
            else if (TryResolveExportAsClass(
                         directValueNode,
                         topLevel,
                         fallbackPropertyName,
                         out directClassName,
                         out _)
                     && constructorInterfacesByClassName.TryGetValue(directClassName, out var directConstructor))
            {
                valueType = TypeOrHandle.FromHandle(directConstructor);
            }
            else
            {
                valueType = TypeOrHandle.FromClr(
                    MapClrType(
                        InferClrTypeFromExpression(
                            directValueNode,
                            topLevel,
                            classFields: null,
                            instanceInterfacesByClassName,
                            ensureClassInstanceInterface: null)
                        .ClrType ?? typeof(object)));
            }
            var valueProperty = EmitReadOnlyProperty(
                exportsTypeBuilder,
                fallbackPropertyName,
                valueType,
                exportName: null,
                isExportValue: true);
            if (firstExportsProperty.IsNil)
            {
                firstExportsProperty = valueProperty;
            }

            if (exportShape.DirectValueKind == PublicExportValueKind.CallableOrConstructable)
            {
                if (TryResolveExportAsFunction(directValueNode, topLevel, out var directFunction, out _))
                {
                    EmitInterfaceMethod(
                        exportsTypeBuilder,
                        BuildContractMethodFromFunction(
                            "Call",
                            directFunction,
                            topLevel,
                            instanceInterfacesByClassName,
                            ensureClassInstanceInterface: null,
                            symbolTable: symbolTable),
                        exportName: null,
                        isExportValue: true);
                }
                else
                {
                    EmitInterfaceMethod(
                        exportsTypeBuilder,
                        new ContractMethod(
                            "Call",
                            ["arguments"],
                            [TypeOrHandle.FromClr(typeof(object[]))],
                            TypeOrHandle.FromClr(typeof(object)),
                            IsParamArray: true),
                        exportName: null,
                        isExportValue: true);
                }

                TypeOrHandle constructReturnType = TypeOrHandle.FromClr(typeof(object));
                if (TryResolveExportAsClass(
                        directValueNode,
                        topLevel,
                        fallbackPropertyName,
                        out directClassName,
                        out _)
                    && instanceInterfacesByClassName.TryGetValue(directClassName, out var directInstance))
                {
                    constructReturnType = TypeOrHandle.FromHandle(directInstance);
                }
                EmitInterfaceMethod(
                    exportsTypeBuilder,
                    new ContractMethod(
                        "Construct",
                        ["arguments"],
                        [TypeOrHandle.FromClr(typeof(object[]))],
                        constructReturnType,
                        IsParamArray: true),
                    exportName: null,
                    isExportValue: true);
            }
        }

        foreach (var member in exportShape.Members)
        {
            var exportName = member.ExportName;
            var valueNode = GetValueNode(member.SourceNode);
            var memberTopLevel = GetTopLevel(member.SourceModule);
            var memberSymbolTable = member.SourceModule.SymbolTable;

            if (!member.HasUnknownSource
                && TryResolveExportAsClass(valueNode, memberTopLevel, exportName, out var className, out _))
            {
                if (!constructorInterfacesByClassName.TryGetValue(className, out var constructorInterface)
                    || constructorInterface.IsNil)
                {
                    // If we can't resolve the class instance interface, fall back to object.
                    var phFallback = EmitReadOnlyProperty(
                        exportsTypeBuilder,
                        ToPascalCase(exportName),
                        TypeOrHandle.FromClr(typeof(object)),
                        exportName,
                        isExportValue: false);
                    if (firstExportsProperty.IsNil)
                    {
                        firstExportsProperty = phFallback;
                    }
                    continue;
                }

                var ph = EmitReadOnlyProperty(
                    exportsTypeBuilder,
                    ToPascalCase(exportName),
                    TypeOrHandle.FromHandle(constructorInterface),
                    exportName,
                    isExportValue: false);
                if (firstExportsProperty.IsNil)
                {
                    firstExportsProperty = ph;
                }
                continue;
            }

            if (!member.HasUnknownSource
                && TryResolveExportAsFunction(valueNode, memberTopLevel, out var functionNode, out _))
            {
                var methodName = ToPascalCase(exportName);
                var method = BuildContractMethodFromFunction(
                    methodName,
                    functionNode,
                    memberTopLevel,
                    instanceInterfacesByClassName,
                    ensureClassInstanceInterface: null,
                    symbolTable: memberSymbolTable);
                EmitInterfaceMethod(exportsTypeBuilder, method, exportName, isExportValue: false);
                continue;
            }

            if (!member.HasUnknownSource
                && (TryResolveObjectExpression(valueNode, memberTopLevel, out _)
                    || TryResolveArrayExpression(valueNode, memberTopLevel, out _)))
            {
                // Nested exported object/array: interface was emitted in the pre-pass.
                if (!nestedInterfacesByExportName.TryGetValue(exportName, out var nestedType))
                {
                    // Shouldn't happen, but fall back to object.
                    var phFallback = EmitReadOnlyProperty(
                        exportsTypeBuilder,
                        ToPascalCase(exportName),
                        TypeOrHandle.FromClr(typeof(object)),
                        exportName,
                        isExportValue: false);
                    if (firstExportsProperty.IsNil)
                    {
                        firstExportsProperty = phFallback;
                    }
                    continue;
                }

                var ph = EmitReadOnlyProperty(
                    exportsTypeBuilder,
                    ToPascalCase(exportName),
                    TypeOrHandle.FromHandle(nestedType),
                    exportName,
                    isExportValue: false);
                if (firstExportsProperty.IsNil)
                {
                    firstExportsProperty = ph;
                }
                continue;
            }

            // Default: exported value projected as a read-only property.
            // Prefer stable binding type from symbol table when available (e.g. const x = complexExpr).
            TypeOrHandle clrType;
            if (member.StableClrType != null)
            {
                clrType = TypeOrHandle.FromClr(MapClrType(member.StableClrType));
            }
            else if (valueNode is Identifier exportedId
                && memberSymbolTable?.Root is Jroc.SymbolTables.Scope globalScope
                && globalScope.Bindings.TryGetValue(exportedId.Name, out var exportedBinding)
                && exportedBinding.IsStableType
                && exportedBinding.ClrType != null)
            {
                clrType = TypeOrHandle.FromClr(MapClrType(exportedBinding.ClrType));
            }
            else
            {
                clrType = InferClrTypeFromExpression(
                    valueNode,
                    memberTopLevel,
                    classFields: null,
                    instanceInterfacesByClassName,
                    ensureClassInstanceInterface: null);
            }
            var propHandle = EmitProperty(
                exportsTypeBuilder,
                ToPascalCase(exportName),
                clrType,
                canWrite: true,
                exportName,
                isExportValue: false);
            if (firstExportsProperty.IsNil)
            {
                firstExportsProperty = propHandle;
            }
        }

        var exportsTypeDef = exportsTypeBuilder.AddTypeDefinition(
            TypeAttributes.NestedPublic | TypeAttributes.Interface | TypeAttributes.Abstract,
            default);
        nestedTypes.Add(exportsTypeDef, facadeType);

        if (!firstExportsProperty.IsNil)
        {
            _metadata.AddPropertyMap(exportsTypeDef, firstExportsProperty);
        }

        // Exports contracts should be disposable so they can shut down the runtime.
        _metadata.AddInterfaceImplementation(exportsTypeDef, _typeRefs.GetOrAdd(typeof(IDisposable)));

        AddJsModuleAttribute(exportsTypeDef, moduleId);
        return exportsTypeDef;
    }

    private TypeDefinitionHandle EmitDynamicObjectInterface(
        TypeDefinitionHandle facadeType,
        NestedTypeRelationshipRegistry nestedTypes)
    {
        var typeBuilder = new TypeBuilder(_metadata, string.Empty, "IObject");
        EmitDynamicObjectHelpers(typeBuilder);

        var typeDef = typeBuilder.AddTypeDefinition(
            TypeAttributes.NestedPublic | TypeAttributes.Interface | TypeAttributes.Abstract,
            default);
        nestedTypes.Add(typeDef, facadeType);
        _metadata.AddInterfaceImplementation(typeDef, _typeRefs.GetOrAdd(typeof(IDisposable)));
        AddGeneratedMarkerAttribute(typeDef, _generatedMetadata.JsObjectContractAttributeCtor);
        return typeDef;
    }

    private TypeDefinitionHandle EmitCallableInterface(
        TypeDefinitionHandle facadeType,
        NestedTypeRelationshipRegistry nestedTypes)
    {
        var typeBuilder = new TypeBuilder(_metadata, string.Empty, "ICallable");
        EmitInterfaceMethod(
            typeBuilder,
            new ContractMethod(
                "Invoke",
                ["arguments"],
                [TypeOrHandle.FromClr(typeof(object[]))],
                TypeOrHandle.FromClr(typeof(object)),
                IsParamArray: true));

        var typeDef = typeBuilder.AddTypeDefinition(
            TypeAttributes.NestedPublic | TypeAttributes.Interface | TypeAttributes.Abstract,
            default);
        nestedTypes.Add(typeDef, facadeType);
        _metadata.AddInterfaceImplementation(typeDef, _typeRefs.GetOrAdd(typeof(IDisposable)));
        AddGeneratedMarkerAttribute(typeDef, _generatedMetadata.JsCallableContractAttributeCtor);
        return typeDef;
    }

    private TypeDefinitionHandle EmitFallbackConstructorInterface(
        TypeDefinitionHandle facadeType,
        NestedTypeRelationshipRegistry nestedTypes,
        TypeDefinitionHandle objectContract)
    {
        var typeBuilder = new TypeBuilder(_metadata, string.Empty, "IConstructor");
        EmitInterfaceMethod(
            typeBuilder,
            new ContractMethod(
                "Construct",
                ["arguments"],
                [TypeOrHandle.FromClr(typeof(object[]))],
                TypeOrHandle.FromHandle(objectContract),
                IsParamArray: true));

        var typeDef = typeBuilder.AddTypeDefinition(
            TypeAttributes.NestedPublic | TypeAttributes.Interface | TypeAttributes.Abstract,
            default);
        nestedTypes.Add(typeDef, facadeType);
        _metadata.AddInterfaceImplementation(typeDef, _typeRefs.GetOrAdd(typeof(IDisposable)));
        return typeDef;
    }

    private void EmitDynamicObjectHelpers(TypeBuilder typeBuilder)
    {
        EmitInterfaceMethod(
            typeBuilder,
            new ContractMethod(
                "GetDynamicProperty",
                ["name"],
                [TypeOrHandle.FromClr(typeof(string))],
                TypeOrHandle.FromClr(typeof(object))));
        EmitInterfaceMethod(
            typeBuilder,
            new ContractMethod(
                "SetDynamicProperty",
                ["name", "value"],
                [TypeOrHandle.FromClr(typeof(string)), TypeOrHandle.FromClr(typeof(object))],
                TypeOrHandle.FromClr(typeof(void))));
        EmitInterfaceMethod(
            typeBuilder,
            new ContractMethod(
                "HasDynamicProperty",
                ["name"],
                [TypeOrHandle.FromClr(typeof(string))],
                TypeOrHandle.FromClr(typeof(bool))));
    }

    private TypeDefinitionHandle EmitHandleInterface(
        string @namespace,
        string interfaceName,
        object? members,
        SymbolTable? symbolTable,
        string? classNameForFields,
        TopLevelIndex? topLevelIndex = null,
        Dictionary<string, TypeDefinitionHandle>? projectionInterfaces = null)
    {
        var typeBuilder = new TypeBuilder(_metadata, @namespace, interfaceName);
        PropertyDefinitionHandle firstProperty = default;

        Dictionary<string, Type>? stableFields = null;
        if (!string.IsNullOrWhiteSpace(classNameForFields) && symbolTable?.Root is Jroc.SymbolTables.Scope rootScope)
        {
            var classScope = FindClassScope(rootScope, classNameForFields!);
            stableFields = classScope?.StableInstanceFieldClrTypes;
        }

        if (members is Node classNode && TryGetClassBody(classNode, out var classBody))
        {
            // Instance field properties (stable inferred only)
            if (stableFields != null)
            {
                foreach (var field in stableFields.OrderBy(kvp => kvp.Key, StringComparer.Ordinal))
                {
                    var propName = ToPascalCase(field.Key);
                    var propType = TypeOrHandle.FromClr(MapClrType(field.Value));
                    var ph = EmitProperty(typeBuilder, propName, propType, canWrite: true, exportName: field.Key);
                    if (firstProperty.IsNil)
                    {
                        firstProperty = ph;
                    }
                }
            }

            // Instance methods
            foreach (var (md, declaringBody) in GetEffectiveClassMethods(
                         classNode,
                         topLevelIndex,
                         isStatic: false))
            {
                if (!TryGetPropertyName(md.Key, out var name))
                {
                    continue;
                }

                if (md.Value is not FunctionExpression fn)
                {
                    continue;
                }

                if (md.Kind == PropertyKind.Get)
                {
                    var getterType = InferReturnTypeFromFunction(
                        fn,
                        topLevelIndex,
                        classFields: stableFields,
                        instanceInterfacesByClassName: projectionInterfaces,
                        ensureClassInstanceInterface: null);
                    var ph = EmitProperty(
                        typeBuilder,
                        ToPascalCase(name),
                        getterType,
                        canWrite: HasClassAccessor(declaringBody, name, isStatic: false, PropertyKind.Set),
                        exportName: name);
                    if (firstProperty.IsNil)
                    {
                        firstProperty = ph;
                    }
                    continue;
                }

                if (md.Kind == PropertyKind.Set)
                {
                    if (HasClassAccessor(declaringBody, name, isStatic: false, PropertyKind.Get))
                    {
                        continue;
                    }

                    var ph = EmitProperty(
                        typeBuilder,
                        ToPascalCase(name),
                        TypeOrHandle.FromClr(typeof(object)),
                        canWrite: true,
                        emitGetter: false,
                        exportName: name);
                    if (firstProperty.IsNil)
                    {
                        firstProperty = ph;
                    }
                    continue;
                }

                var methodName = ToPascalCase(name);
                var method = BuildContractMethodFromFunction(
                    methodName,
                    fn,
                    topLevelIndex,
                    instanceInterfacesByClassName: projectionInterfaces,
                    ensureClassInstanceInterface: null,
                    classFields: stableFields,
                    symbolTable: symbolTable,
                    scopeLookupNode: fn);
                EmitInterfaceMethod(typeBuilder, method, exportName: name);
            }
        }
        else if (members is ObjectExpression obj)
        {
            // Nested object interface projected as a handle.
            var childInterfacesByMemberName = new Dictionary<string, TypeDefinitionHandle>(StringComparer.Ordinal);
            foreach (var prop in obj.Properties)
            {
                if (prop is not Property p || !TryGetPropertyName(p.Key, out var memberName))
                {
                    continue;
                }

                if (p.Value is ObjectExpression childObject)
                {
                    childInterfacesByMemberName[memberName] = EmitHandleInterface(
                        @namespace,
                        interfaceName + ToPascalCase(memberName),
                        childObject,
                        symbolTable,
                        classNameForFields: null,
                        topLevelIndex,
                        projectionInterfaces);
                }
                else if (p.Value is ArrayExpression)
                {
                    childInterfacesByMemberName[memberName] = EmitArrayInterface(
                        @namespace,
                        interfaceName + ToPascalCase(memberName) + "Array");
                }
            }

            EmitDynamicObjectHelpers(typeBuilder);

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

                if (childInterfacesByMemberName.TryGetValue(memberName, out var childInterface))
                {
                    var childProperty = EmitProperty(
                        typeBuilder,
                        ToPascalCase(memberName),
                        TypeOrHandle.FromHandle(childInterface),
                        canWrite: true,
                        exportName: memberName);
                    if (firstProperty.IsNil)
                    {
                        firstProperty = childProperty;
                    }
                    continue;
                }

                if (p.Kind == PropertyKind.Get)
                {
                    var getterType = InferReturnTypeFromFunction(
                        valueNode,
                        topLevelIndex,
                        classFields: null,
                        instanceInterfacesByClassName: projectionInterfaces,
                        ensureClassInstanceInterface: null);
                    var getterProperty = EmitProperty(
                        typeBuilder,
                        ToPascalCase(memberName),
                        getterType,
                        canWrite: HasObjectAccessor(obj, memberName, PropertyKind.Set),
                        exportName: memberName);
                    if (firstProperty.IsNil)
                    {
                        firstProperty = getterProperty;
                    }
                    continue;
                }

                if (p.Kind == PropertyKind.Set)
                {
                    if (HasObjectAccessor(obj, memberName, PropertyKind.Get))
                    {
                        continue;
                    }

                    var setterProperty = EmitProperty(
                        typeBuilder,
                        ToPascalCase(memberName),
                        TypeOrHandle.FromClr(typeof(object)),
                        canWrite: true,
                        emitGetter: false,
                        exportName: memberName);
                    if (firstProperty.IsNil)
                    {
                        firstProperty = setterProperty;
                    }
                    continue;
                }

                if (valueNode is FunctionExpression or ArrowFunctionExpression)
                {
                    var method = BuildContractMethodFromFunction(
                        ToPascalCase(memberName),
                        valueNode,
                        topLevelIndex,
                        instanceInterfacesByClassName: projectionInterfaces,
                        ensureClassInstanceInterface: null);
                    EmitInterfaceMethod(typeBuilder, method, exportName: memberName);
                    continue;
                }

                var clrType = InferClrTypeFromExpression(
                    valueNode,
                    topLevelIndex,
                    classFields: null,
                    instanceInterfacesByClassName: projectionInterfaces,
                    ensureClassInstanceInterface: null);
                var ph = EmitProperty(typeBuilder, ToPascalCase(memberName), clrType, canWrite: true, exportName: memberName);
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

        _metadata.AddInterfaceImplementation(typeDef, _typeRefs.GetOrAdd(typeof(IDisposable)));
        if (members is ObjectExpression)
        {
            AddGeneratedMarkerAttribute(typeDef, _generatedMetadata.JsObjectContractAttributeCtor);
        }

        return typeDef;
    }

    private TypeDefinitionHandle EmitConstructorInterface(
        string @namespace,
        string interfaceName,
        Node classNode,
        TypeOrHandle instanceType,
        SymbolTable? symbolTable,
        string? classNameForFields,
        TopLevelIndex? topLevelIndex = null,
        Dictionary<string, TypeDefinitionHandle>? projectionInterfaces = null)
    {
        var typeBuilder = new TypeBuilder(_metadata, @namespace, interfaceName);
        PropertyDefinitionHandle firstProperty = default;

        var constructorFunction = FindConstructorFunction(classNode);
        var constructMethod = constructorFunction == null
            ? new ContractMethod(
                "Construct",
                Array.Empty<string>(),
                Array.Empty<TypeOrHandle>(),
                instanceType)
            : BuildContractMethodFromFunction(
                "Construct",
                constructorFunction,
                topLevelIndex,
                instanceInterfacesByClassName: projectionInterfaces,
                ensureClassInstanceInterface: null,
                symbolTable: symbolTable,
                scopeLookupNode: constructorFunction) with { ReturnType = instanceType };
        EmitInterfaceMethod(typeBuilder, constructMethod);

        if (TryGetClassBody(classNode, out var classBody))
        {
            foreach (var (md, declaringBody) in GetEffectiveClassMethods(
                         classNode,
                         topLevelIndex,
                         isStatic: true))
            {
                if (!TryGetPropertyName(md.Key, out var name)
                    || md.Value is not FunctionExpression fn)
                {
                    continue;
                }

                if (md.Kind == PropertyKind.Get)
                {
                    var getterType = InferReturnTypeFromFunction(
                        fn,
                        topLevelIndex,
                        classFields: null,
                        instanceInterfacesByClassName: projectionInterfaces,
                        ensureClassInstanceInterface: null);
                    var ph = EmitProperty(
                        typeBuilder,
                        ToPascalCase(name),
                        getterType,
                        canWrite: HasClassAccessor(declaringBody, name, isStatic: true, PropertyKind.Set),
                        exportName: name);
                    if (firstProperty.IsNil)
                    {
                        firstProperty = ph;
                    }
                    continue;
                }

                if (md.Kind == PropertyKind.Set)
                {
                    if (HasClassAccessor(declaringBody, name, isStatic: true, PropertyKind.Get))
                    {
                        continue;
                    }

                    var ph = EmitProperty(
                        typeBuilder,
                        ToPascalCase(name),
                        TypeOrHandle.FromClr(typeof(object)),
                        canWrite: true,
                        emitGetter: false,
                        exportName: name);
                    if (firstProperty.IsNil)
                    {
                        firstProperty = ph;
                    }
                    continue;
                }

                var method = BuildContractMethodFromFunction(
                    ToPascalCase(name),
                    fn,
                    topLevelIndex,
                    instanceInterfacesByClassName: projectionInterfaces,
                    ensureClassInstanceInterface: null,
                    symbolTable: symbolTable,
                    scopeLookupNode: fn);
                EmitInterfaceMethod(typeBuilder, method, exportName: name);
            }

            foreach (var propertyDefinition in classBody.Body
                         .OfType<Acornima.Ast.PropertyDefinition>()
                         .Where(property => property.Static))
            {
                if (!TryGetPropertyName(propertyDefinition.Key, out var name))
                {
                    continue;
                }

                var propertyType = InferClrTypeFromExpression(
                    propertyDefinition.Value,
                    topLevelIndex,
                    classFields: null,
                    instanceInterfacesByClassName: projectionInterfaces,
                    ensureClassInstanceInterface: null);
                var ph = EmitProperty(
                    typeBuilder,
                    ToPascalCase(name),
                    propertyType,
                    canWrite: true,
                    exportName: name);
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

        _metadata.AddInterfaceImplementation(typeDef, _typeRefs.GetOrAdd(typeof(IDisposable)));
        return typeDef;
    }

    private TypeDefinitionHandle EmitArrayInterface(
        string @namespace,
        string interfaceName,
        TypeDefinitionHandle facadeType = default,
        NestedTypeRelationshipRegistry? nestedTypes = null)
    {
        var typeBuilder = new TypeBuilder(_metadata, @namespace, interfaceName);

        var firstProperty = EmitProperty(
            typeBuilder,
            "Length",
            TypeOrHandle.FromClr(typeof(double)),
            canWrite: true,
            exportName: "length");

        EmitInterfaceMethod(
            typeBuilder,
            new ContractMethod(
                "Get",
                ["index"],
                [TypeOrHandle.FromClr(typeof(double))],
                TypeOrHandle.FromClr(typeof(object))));
        EmitInterfaceMethod(
            typeBuilder,
            new ContractMethod(
                "Set",
                ["index", "value"],
                [TypeOrHandle.FromClr(typeof(double)), TypeOrHandle.FromClr(typeof(object))],
                TypeOrHandle.FromClr(typeof(void))));
        EmitInterfaceMethod(
            typeBuilder,
            new ContractMethod(
                "HasIndex",
                ["index"],
                [TypeOrHandle.FromClr(typeof(double))],
                TypeOrHandle.FromClr(typeof(bool))));
        EmitInterfaceMethod(
            typeBuilder,
            new ContractMethod(
                "Push",
                ["values"],
                [TypeOrHandle.FromClr(typeof(object[]))],
                TypeOrHandle.FromClr(typeof(double)),
                IsParamArray: true));

        var typeDef = typeBuilder.AddTypeDefinition(
            (facadeType.IsNil ? TypeAttributes.Public : TypeAttributes.NestedPublic)
            | TypeAttributes.Interface
            | TypeAttributes.Abstract,
            default);
        if (!facadeType.IsNil)
        {
            nestedTypes?.Add(typeDef, facadeType);
        }
        _metadata.AddPropertyMap(typeDef, firstProperty);
        _metadata.AddInterfaceImplementation(typeDef, _typeRefs.GetOrAdd(typeof(IDisposable)));
        AddGeneratedMarkerAttribute(typeDef, _generatedMetadata.JsArrayContractAttributeCtor);
        return typeDef;
    }

    private static Jroc.SymbolTables.Scope? FindClassScope(Jroc.SymbolTables.Scope scope, string className)
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

    [Flags]
    private enum FallbackContractKind
    {
        None = 0,
        Object = 1,
        Array = 2,
        Callable = 4,
        Constructor = 8
    }

    private sealed record ClassContractDefinition(
        string ContractName,
        string ScopeClassName,
        Node ClassNode,
        ModuleDefinition SourceModule);

    private static IReadOnlyList<ClassContractDefinition> BuildClassContractDefinitions(
        ModuleDefinition module,
        PublicModuleExportShape exportShape,
        TopLevelIndex topLevel)
    {
        var results = new Dictionary<string, ClassContractDefinition>(StringComparer.Ordinal);

        foreach (var (className, classDeclaration) in topLevel.Classes)
        {
            results[className] = new ClassContractDefinition(
                className,
                className,
                classDeclaration,
                module);
        }

        foreach (var member in exportShape.Members)
        {
            var memberTopLevel = ReferenceEquals(member.SourceModule, module)
                ? topLevel
                : BuildTopLevelDeclarationIndex(member.SourceModule.Ast);
            if (TryResolveExportAsClass(
                    GetValueNode(member.SourceNode),
                    memberTopLevel,
                    member.ExportName,
                    out var contractName,
                    out var classNode)
                && !results.ContainsKey(contractName))
            {
                var scopeClassName = classNode switch
                {
                    ClassDeclaration { Id: Identifier id } => id.Name,
                    ClassExpression { Id: Identifier id } => id.Name,
                    Identifier id => id.Name,
                    _ => contractName
                };

                results[contractName] = new ClassContractDefinition(
                    contractName,
                    scopeClassName,
                    classNode,
                    member.SourceModule);
            }
        }

        if (TryResolveExportAsClass(
                GetValueNode(exportShape.DirectValueSourceNode),
                topLevel,
                "Default",
                out var directContractName,
                out var directClassNode)
            && !results.ContainsKey(directContractName))
        {
            var scopeClassName = directClassNode switch
            {
                ClassDeclaration { Id: Identifier id } => id.Name,
                ClassExpression { Id: Identifier id } => id.Name,
                Identifier id => id.Name,
                _ => directContractName
            };
            results[directContractName] = new ClassContractDefinition(
                directContractName,
                scopeClassName,
                directClassNode,
                module);
        }

        return results.Values.ToArray();
    }

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
                case ExportNamedDeclaration { Declaration: FunctionDeclaration fd } when fd.Id is Identifier id:
                    functions[id.Name] = fd;
                    break;
                case ExportNamedDeclaration { Declaration: ClassDeclaration cd } when cd.Id is Identifier id:
                    classes[id.Name] = cd;
                    break;
                case ExportNamedDeclaration { Declaration: VariableDeclaration vd }:
                    foreach (var decl in vd.Declarations)
                    {
                        if (decl.Id is Identifier vid && decl.Init is Expression init)
                        {
                            vars[vid.Name] = init;
                        }
                    }
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

    private static FallbackContractKind GetFallbackContractRequirements(
        ModuleDefinition module,
        PublicModuleExportShape exportShape,
        TopLevelIndex topLevel)
    {
        var requirements = FallbackContractKind.None;

        foreach (var classContract in BuildClassContractDefinitions(module, exportShape, topLevel))
        {
            var classTopLevel = ReferenceEquals(classContract.SourceModule, module)
                ? topLevel
                : BuildTopLevelDeclarationIndex(classContract.SourceModule.Ast);
            foreach (var (method, _) in GetEffectiveClassMethods(
                         classContract.ClassNode,
                         classTopLevel,
                         isStatic: false)
                     .Concat(GetEffectiveClassMethods(
                         classContract.ClassNode,
                         classTopLevel,
                         isStatic: true)))
            {
                requirements |= GetFunctionFallbackRequirements(method.Value, classTopLevel);
            }
        }

        foreach (var member in exportShape.Members)
        {
            var memberTopLevel = ReferenceEquals(member.SourceModule, module)
                ? topLevel
                : BuildTopLevelDeclarationIndex(member.SourceModule.Ast);
            var valueNode = GetValueNode(member.SourceNode);
            if (TryResolveExportAsFunction(valueNode, memberTopLevel, out var function, out _))
            {
                requirements |= GetFunctionFallbackRequirements(function, memberTopLevel);
            }
            else if (TryResolveObjectExpression(valueNode, memberTopLevel, out var objectExpression))
            {
                requirements |= GetObjectFallbackRequirements(objectExpression, memberTopLevel);
            }
        }

        var directValueNode = GetValueNode(exportShape.DirectValueSourceNode);
        if (TryResolveExportAsFunction(directValueNode, topLevel, out var directFunction, out _))
        {
            requirements |= GetFunctionFallbackRequirements(directFunction, topLevel);
        }
        else if (TryResolveObjectExpression(directValueNode, topLevel, out var directObject))
        {
            requirements |= GetObjectFallbackRequirements(directObject, topLevel);
        }

        if (requirements.HasFlag(FallbackContractKind.Constructor))
        {
            requirements |= FallbackContractKind.Object;
        }

        return requirements;
    }

    private static FallbackContractKind GetObjectFallbackRequirements(
        ObjectExpression objectExpression,
        TopLevelIndex topLevel)
    {
        var requirements = FallbackContractKind.None;
        foreach (var property in objectExpression.Properties.OfType<Property>())
        {
            requirements |= property.Value switch
            {
                FunctionExpression or ArrowFunctionExpression =>
                    GetFunctionFallbackRequirements(property.Value, topLevel),
                ObjectExpression childObject =>
                    GetObjectFallbackRequirements(childObject, topLevel),
                _ => FallbackContractKind.None
            };
        }

        return requirements;
    }

    private static FallbackContractKind GetFunctionFallbackRequirements(
        Node functionNode,
        TopLevelIndex topLevel)
    {
        if (functionNode is ArrowFunctionExpression { Body: Expression expressionBody })
        {
            return GetExpressionFallbackRequirement(expressionBody, topLevel);
        }

        if (GetFunctionBody(functionNode) is not BlockStatement block)
        {
            return FallbackContractKind.None;
        }

        var requirements = FallbackContractKind.None;
        foreach (var returnStatement in block.Body.OfType<ReturnStatement>())
        {
            requirements |= GetExpressionFallbackRequirement(
                returnStatement.Argument,
                topLevel);
        }

        return requirements;
    }

    private static FallbackContractKind GetExpressionFallbackRequirement(
        Node? expression,
        TopLevelIndex topLevel)
    {
        switch (expression)
        {
            case FunctionExpression or ArrowFunctionExpression:
                return FallbackContractKind.Callable;
            case ObjectExpression:
                return FallbackContractKind.Object;
            case ArrayExpression:
                return FallbackContractKind.Array;
            case ClassExpression:
                return FallbackContractKind.Constructor | FallbackContractKind.Object;
            case CallExpression
            {
                Callee: MemberExpression
                {
                    Object: Identifier { Name: "Object" },
                    Property: Identifier { Name: "create" }
                }
            }:
                return FallbackContractKind.Object;
            case Identifier identifier when topLevel.Functions.ContainsKey(identifier.Name):
                return FallbackContractKind.Callable;
            case Identifier identifier when topLevel.Classes.ContainsKey(identifier.Name):
                return FallbackContractKind.None;
            case Identifier identifier
                when topLevel.VariableInitializers.TryGetValue(identifier.Name, out var initializer):
                return GetExpressionFallbackRequirement(initializer, topLevel);
            default:
                return FallbackContractKind.None;
        }
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

    private static bool TryGetClassBody(object? classNode, out ClassBody body)
    {
        switch (classNode)
        {
            case ClassDeclaration classDeclaration:
                body = classDeclaration.Body;
                return true;
            case ClassExpression classExpression:
                body = classExpression.Body;
                return true;
            default:
                body = null!;
                return false;
        }
    }

    private static FunctionExpression? FindConstructorFunction(Node classNode)
    {
        if (!TryGetClassBody(classNode, out var classBody))
        {
            return null;
        }

        return classBody.Body
            .OfType<Acornima.Ast.MethodDefinition>()
            .FirstOrDefault(method => method.Kind == PropertyKind.Constructor)
            ?.Value as FunctionExpression;
    }

    private static bool HasClassAccessor(
        ClassBody classBody,
        string propertyName,
        bool isStatic,
        PropertyKind kind)
        => classBody.Body.OfType<Acornima.Ast.MethodDefinition>().Any(method =>
            method.Static == isStatic
            && method.Kind == kind
            && TryGetPropertyName(method.Key, out var candidate)
            && string.Equals(candidate, propertyName, StringComparison.Ordinal));

    private static IReadOnlyList<(Acornima.Ast.MethodDefinition Method, ClassBody DeclaringBody)>
        GetEffectiveClassMethods(
            Node classNode,
            TopLevelIndex? topLevelIndex,
            bool isStatic)
    {
        var methods = new List<(Acornima.Ast.MethodDefinition, ClassBody)>();
        var seenNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (var body in GetClassHierarchyBodies(classNode, topLevelIndex))
        {
            var namedMethods = body.Body
                .OfType<Acornima.Ast.MethodDefinition>()
                .Where(method =>
                    method.Static == isStatic
                    && method.Kind != PropertyKind.Constructor
                    && method.Value is FunctionExpression
                    && TryGetPropertyName(method.Key, out _))
                .GroupBy(
                    method =>
                    {
                        _ = TryGetPropertyName(method.Key, out var name);
                        return name;
                    },
                    StringComparer.Ordinal);

            foreach (var group in namedMethods)
            {
                if (!seenNames.Add(group.Key))
                {
                    continue;
                }

                var definitions = group.ToArray();
                var representative = definitions.Last();
                if (representative.Kind is PropertyKind.Get or PropertyKind.Set)
                {
                    representative = definitions.LastOrDefault(method => method.Kind == PropertyKind.Get)
                        ?? definitions.LastOrDefault(method => method.Kind == PropertyKind.Set)
                        ?? representative;
                }

                methods.Add((representative, body));
            }
        }

        return methods;
    }

    private static IEnumerable<ClassBody> GetClassHierarchyBodies(
        Node classNode,
        TopLevelIndex? topLevelIndex)
    {
        var current = classNode;
        var visited = new HashSet<Node>(ReferenceEqualityComparer.Instance);
        while (visited.Add(current) && TryGetClassBody(current, out var body))
        {
            yield return body;

            var superClass = current switch
            {
                ClassDeclaration declaration => declaration.SuperClass,
                ClassExpression expression => expression.SuperClass,
                _ => null
            };
            if (superClass is not Identifier identifier
                || topLevelIndex == null
                || !topLevelIndex.Value.Classes.TryGetValue(identifier.Name, out var parent))
            {
                yield break;
            }

            current = parent;
        }
    }

    private static bool HasObjectAccessor(
        ObjectExpression objectExpression,
        string propertyName,
        PropertyKind kind)
        => objectExpression.Properties.OfType<Property>().Any(property =>
            property.Kind == kind
            && TryGetPropertyName(property.Key, out var candidate)
            && string.Equals(candidate, propertyName, StringComparison.Ordinal));

    private static string GetContractNamespace(string assemblyName, string moduleId, bool isRootModule)
    {
        var rootNamespace = $"Jroc.{JrocFacadeNamePlanner.NormalizeIdentifier(assemblyName, stripLeadingAtSign: true)}";

        if (isRootModule)
        {
            return rootNamespace;
        }

        var segments = moduleId.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
        {
            return rootNamespace;
        }

        var namespaceSegments = segments.Length > 1
            ? segments.Take(segments.Length - 1).Select(segment => ToPascalCase(JrocFacadeNamePlanner.NormalizeIdentifier(segment, stripLeadingAtSign: true))).ToArray()
            : Array.Empty<string>();

        return namespaceSegments.Length == 0
            ? rootNamespace
            : rootNamespace + "." + string.Join(".", namespaceSegments);
    }

    private static int CountModuleContractMethods(
        ModuleDefinition module,
        PublicModuleExportShape exportShape,
        TopLevelIndex topLevel)
    {
        var fallbackRequirements = GetFallbackContractRequirements(
            module,
            exportShape,
            topLevel);
        var count = CountFallbackContractMethods(fallbackRequirements);
        foreach (var classContract in BuildClassContractDefinitions(module, exportShape, topLevel))
        {
            var classTopLevel = ReferenceEquals(classContract.SourceModule, module)
                ? topLevel
                : BuildTopLevelDeclarationIndex(classContract.SourceModule.Ast);
            if (classContract.SourceModule.SymbolTable?.Root is Jroc.SymbolTables.Scope rootScope)
            {
                count += (FindClassScope(rootScope, classContract.ScopeClassName)?.StableInstanceFieldClrTypes.Count ?? 0) * 2;
            }

            count += CountClassInstanceMemberMethods(classContract.ClassNode, classTopLevel);
            count += CountClassConstructorMemberMethods(classContract.ClassNode, classTopLevel);
        }

        foreach (var member in exportShape.Members)
        {
            var memberTopLevel = ReferenceEquals(member.SourceModule, module)
                ? topLevel
                : BuildTopLevelDeclarationIndex(member.SourceModule.Ast);
            var valueNode = GetValueNode(member.SourceNode);
            if (TryResolveObjectExpression(valueNode, memberTopLevel, out var objectExpression))
            {
                count += CountObjectContractMethods(objectExpression);
            }
            else if (TryResolveArrayExpression(valueNode, memberTopLevel, out _))
            {
                count += CountArrayContractMethods();
            }
        }

        if (exportShape.DirectValueKind != PublicExportValueKind.None
            || exportShape.Kind == PublicExportShapeKind.Unknown)
        {
            var directValueNode = GetValueNode(exportShape.DirectValueSourceNode);
            if (exportShape.Kind != PublicExportShapeKind.Unknown)
            {
                if (TryResolveObjectExpression(directValueNode, topLevel, out var directObject))
                {
                    count += CountObjectContractMethods(directObject);
                }
                else if (TryResolveArrayExpression(directValueNode, topLevel, out _))
                {
                    count += CountArrayContractMethods();
                }
            }

            count++;
            if (exportShape.DirectValueKind == PublicExportValueKind.CallableOrConstructable)
            {
                count += 2;
            }
        }

        foreach (var member in exportShape.Members)
        {
            var valueNode = GetValueNode(member.SourceNode);
            var memberTopLevel = ReferenceEquals(member.SourceModule, module)
                ? topLevel
                : BuildTopLevelDeclarationIndex(member.SourceModule.Ast);
            if (!member.HasUnknownSource
                && (TryResolveExportAsClass(valueNode, memberTopLevel, member.ExportName, out _, out _)
                    || TryResolveExportAsFunction(valueNode, memberTopLevel, out _, out _)
                    || TryResolveObjectExpression(valueNode, memberTopLevel, out _)
                    || TryResolveArrayExpression(valueNode, memberTopLevel, out _)))
            {
                count += 1;
            }
            else
            {
                count += 2;
            }
        }

        return count;
    }

    private static int CountDependentContractTypes(
        ModuleDefinition module,
        PublicModuleExportShape exportShape,
        TopLevelIndex topLevel)
    {
        var fallbackRequirements = GetFallbackContractRequirements(
            module,
            exportShape,
            topLevel);
        var count = CountFallbackContractTypes(fallbackRequirements)
                    + (BuildClassContractDefinitions(module, exportShape, topLevel).Count * 2);
        foreach (var member in exportShape.Members)
        {
            var memberTopLevel = ReferenceEquals(member.SourceModule, module)
                ? topLevel
                : BuildTopLevelDeclarationIndex(member.SourceModule.Ast);
            var valueNode = GetValueNode(member.SourceNode);
            if (TryResolveObjectExpression(valueNode, memberTopLevel, out var objectExpression))
            {
                count += CountObjectGraphTypes(objectExpression);
            }
            else if (TryResolveArrayExpression(valueNode, memberTopLevel, out _))
            {
                count += 1;
            }
        }

        var directValueNode = GetValueNode(exportShape.DirectValueSourceNode);
        if (exportShape.Kind != PublicExportShapeKind.Unknown)
        {
            if (TryResolveObjectExpression(directValueNode, topLevel, out var directObject))
            {
                count += CountObjectGraphTypes(directObject);
            }
            else if (TryResolveArrayExpression(directValueNode, topLevel, out _))
            {
                count += 1;
            }
        }
        return count;
    }

    private static int CountFallbackContractMethods(FallbackContractKind requirements)
    {
        var count = 0;
        if (requirements.HasFlag(FallbackContractKind.Object))
        {
            count += 3;
        }
        if (requirements.HasFlag(FallbackContractKind.Array))
        {
            count += CountArrayContractMethods();
        }
        if (requirements.HasFlag(FallbackContractKind.Callable))
        {
            count++;
        }
        if (requirements.HasFlag(FallbackContractKind.Constructor))
        {
            count++;
        }
        return count;
    }

    private static int CountFallbackContractTypes(FallbackContractKind requirements)
    {
        var count = 0;
        if (requirements.HasFlag(FallbackContractKind.Object))
        {
            count++;
        }
        if (requirements.HasFlag(FallbackContractKind.Array))
        {
            count++;
        }
        if (requirements.HasFlag(FallbackContractKind.Callable))
        {
            count++;
        }
        if (requirements.HasFlag(FallbackContractKind.Constructor))
        {
            count++;
        }
        return count;
    }

    private static int CountObjectGraphTypes(ObjectExpression objectExpression)
    {
        var count = 1;
        foreach (var property in objectExpression.Properties)
        {
            if (property is not Property objectProperty)
            {
                continue;
            }

            count += objectProperty.Value switch
            {
                ObjectExpression childObject => CountObjectGraphTypes(childObject),
                ArrayExpression => 1,
                _ => 0
            };
        }

        return count;
    }

    private static int CountClassInstanceMemberMethods(
        Node classNode,
        TopLevelIndex topLevelIndex)
    {
        var count = 0;
        foreach (var (method, declaringBody) in GetEffectiveClassMethods(
                     classNode,
                     topLevelIndex,
                     isStatic: false))
        {
            if (!TryGetPropertyName(method.Key, out var name))
            {
                continue;
            }

            count += method.Kind == PropertyKind.Get
                     && HasClassAccessor(declaringBody, name, isStatic: false, PropertyKind.Set)
                ? 2
                : 1;
        }

        return count;
    }

    private static int CountClassConstructorMemberMethods(
        Node classNode,
        TopLevelIndex topLevelIndex)
    {
        var count = 1; // Construct
        if (!TryGetClassBody(classNode, out var classBody))
        {
            return count;
        }

        foreach (var (method, declaringBody) in GetEffectiveClassMethods(
                     classNode,
                     topLevelIndex,
                     isStatic: true))
        {
            if (!TryGetPropertyName(method.Key, out var name))
            {
                continue;
            }

            count += method.Kind == PropertyKind.Get
                     && HasClassAccessor(declaringBody, name, isStatic: true, PropertyKind.Set)
                ? 2
                : 1;
        }

        foreach (var propertyDefinition in classBody.Body
                     .OfType<Acornima.Ast.PropertyDefinition>()
                     .Where(property => property.Static))
        {
            if (TryGetPropertyName(propertyDefinition.Key, out _))
            {
                count += 2;
            }
        }

        return count;
    }

    private static int CountObjectContractMethods(ObjectExpression objectExpression)
    {
        var count = 3;
        foreach (var property in objectExpression.Properties)
        {
            if (property is not Property objectProperty
                || !TryGetPropertyName(objectProperty.Key, out _))
            {
                continue;
            }

            count += objectProperty.Value switch
            {
                ObjectExpression childObject => CountObjectContractMethods(childObject),
                ArrayExpression => CountArrayContractMethods(),
                _ => 0
            };

            if (objectProperty.Kind is PropertyKind.Get or PropertyKind.Set)
            {
                count += 1;
            }
            else if (objectProperty.Value is FunctionExpression or ArrowFunctionExpression)
            {
                count += 1;
            }
            else
            {
                count += 2;
            }
        }

        return count;
    }

    private static int CountArrayContractMethods()
        => 6;

    private static Node? GetValueNode(Node? node)
        => node is Property property ? property.Value : node;

    private static bool TryResolveObjectExpression(
        Node? valueNode,
        TopLevelIndex topLevel,
        out ObjectExpression objectExpression)
    {
        if (valueNode is ObjectExpression direct)
        {
            objectExpression = direct;
            return true;
        }

        if (valueNode is Identifier id
            && topLevel.VariableInitializers.TryGetValue(id.Name, out var initializer)
            && initializer is ObjectExpression fromInitializer)
        {
            objectExpression = fromInitializer;
            return true;
        }

        objectExpression = null!;
        return false;
    }

    private static bool TryResolveArrayExpression(
        Node? valueNode,
        TopLevelIndex topLevel,
        out ArrayExpression arrayExpression)
    {
        if (valueNode is ArrayExpression direct)
        {
            arrayExpression = direct;
            return true;
        }

        if (valueNode is Identifier id
            && topLevel.VariableInitializers.TryGetValue(id.Name, out var initializer)
            && initializer is ArrayExpression fromInitializer)
        {
            arrayExpression = fromInitializer;
            return true;
        }

        arrayExpression = null!;
        return false;
    }

    private static bool TryResolveExportAsClass(
        Node? valueNode,
        TopLevelIndex topLevel,
        string fallbackName,
        out string className,
        out Node classNode)
    {
        className = string.Empty;
        classNode = null!;

        if (valueNode is Identifier id && topLevel.Classes.TryGetValue(id.Name, out var classDeclaration))
        {
            className = id.Name;
            classNode = classDeclaration;
            return true;
        }

        if (valueNode is Identifier variableId
            && topLevel.VariableInitializers.TryGetValue(variableId.Name, out var initializer)
            && initializer is ClassExpression variableClassExpression)
        {
            className = variableClassExpression.Id is Identifier namedClassExpression
                ? namedClassExpression.Name
                : variableId.Name;
            classNode = variableClassExpression;
            return true;
        }

        if (valueNode is ClassExpression ce && ce.Id is Identifier cid)
        {
            className = cid.Name;
            classNode = ce;
            return true;
        }

        if (valueNode is ClassExpression anonymousClass)
        {
            className = ToPascalCase(fallbackName);
            classNode = anonymousClass;
            return true;
        }

        if (valueNode is ClassDeclaration cd && cd.Id is Identifier did)
        {
            className = did.Name;
            classNode = cd;
            return true;
        }

        return false;
    }

    private static bool TryResolveExportAsFunction(Node? valueNode, TopLevelIndex topLevel, out Node functionNode, out string? nameForInference)
    {
        nameForInference = null;

        switch (valueNode)
        {
            case FunctionDeclaration fd:
                functionNode = fd;
                nameForInference = fd.Id is Identifier fid ? fid.Name : null;
                return true;
            case Identifier id when topLevel.Functions.TryGetValue(id.Name, out var decl):
                functionNode = decl;
                nameForInference = id.Name;
                return true;
            case Identifier id when topLevel.VariableInitializers.TryGetValue(id.Name, out var initializer)
                                    && initializer is FunctionExpression or ArrowFunctionExpression:
                functionNode = initializer;
                nameForInference = id.Name;
                return true;
            case FunctionExpression fe:
                functionNode = fe;
                nameForInference = fe.Id is Identifier functionId ? functionId.Name : null;
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

    private sealed record ContractMethod(
        string Name,
        IReadOnlyList<string> ParamNames,
        IReadOnlyList<TypeOrHandle> ParamTypes,
        TypeOrHandle ReturnType,
        bool IsParamArray = false);

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
        if (!CanProjectNaturalParameters(functionNode))
        {
            return new ContractMethod(
                methodName,
                ["arguments"],
                [TypeOrHandle.FromClr(typeof(object[]))],
                WrapReturnTypeForAsyncFunction(
                    functionNode,
                    InferReturnTypeFromFunction(
                        functionNode,
                        topLevelIndex,
                        classFields,
                        instanceInterfacesByClassName,
                        ensureClassInstanceInterface)),
                IsParamArray: true);
        }

        var inferredReturnType = InferReturnTypeFromFunction(
            functionNode,
            topLevelIndex,
            classFields,
            instanceInterfacesByClassName,
            ensureClassInstanceInterface);

        // Prefer generated projection handles over a broad stable System.Object
        // result. Otherwise use stable primitive inference when available.
        TypeOrHandle baseReturnType;
        var lookupNode = scopeLookupNode ?? functionNode;
        var scope = symbolTable?.FindScopeByAstNode(lookupNode);
        if (inferredReturnType.Handle.HasValue
            || inferredReturnType.OpenGenericTypeRef.HasValue)
        {
            baseReturnType = inferredReturnType;
        }
        else if (scope?.StableReturnClrType != null)
        {
            baseReturnType = TypeOrHandle.FromClr(MapClrType(scope.StableReturnClrType));
        }
        else
        {
            baseReturnType = inferredReturnType;
        }

        var returnType = WrapReturnTypeForAsyncFunction(functionNode, baseReturnType);

        // Keep parameters conservative because JavaScript functions frequently accept
        // callbacks, objects, or mixed primitives even when the return type is stable.
        var paramNames = GetFunctionParams(functionNode)
            .OfType<Identifier>()
            .Select(parameter => parameter.Name)
            .ToArray();
        var paramTypes = new List<TypeOrHandle>(paramNames.Length);
        for (var i = 0; i < paramNames.Length; i++)
        {
            paramTypes.Add(TypeOrHandle.FromClr(typeof(object)));
        }

        return new ContractMethod(methodName, paramNames, paramTypes, returnType);
    }

    private static bool CanProjectNaturalParameters(Node functionNode)
        => GetFunctionParams(functionNode).All(parameter => parameter is Identifier);

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
        Node? expr,
        TopLevelIndex? topLevelIndex,
        Dictionary<string, Type>? classFields,
        Dictionary<string, TypeDefinitionHandle>? instanceInterfacesByClassName,
        Func<string, TypeDefinitionHandle>? ensureClassInstanceInterface)
    {
        switch (expr)
        {
            case FunctionExpression or ArrowFunctionExpression:
                if (TryGetProjectionInterface(
                        instanceInterfacesByClassName,
                        FallbackCallableContractKey,
                        out var callableContract))
                {
                    return TypeOrHandle.FromHandle(callableContract);
                }
                return TypeOrHandle.FromClr(typeof(object));

            case ObjectExpression:
                if (TryGetProjectionInterface(
                        instanceInterfacesByClassName,
                        FallbackObjectContractKey,
                        out var objectContract))
                {
                    return TypeOrHandle.FromHandle(objectContract);
                }
                return TypeOrHandle.FromClr(typeof(object));

            case ArrayExpression:
                if (TryGetProjectionInterface(
                        instanceInterfacesByClassName,
                        FallbackArrayContractKey,
                        out var arrayContract))
                {
                    return TypeOrHandle.FromHandle(arrayContract);
                }
                return TypeOrHandle.FromClr(typeof(object));

            case ClassExpression:
                if (TryGetProjectionInterface(
                        instanceInterfacesByClassName,
                        FallbackConstructorContractKey,
                        out var constructorContract))
                {
                    return TypeOrHandle.FromHandle(constructorContract);
                }
                return TypeOrHandle.FromClr(typeof(object));

            case CallExpression
            {
                Callee: MemberExpression
                {
                    Object: Identifier { Name: "Object" },
                    Property: Identifier { Name: "create" }
                }
            }:
                if (TryGetProjectionInterface(
                        instanceInterfacesByClassName,
                        FallbackObjectContractKey,
                        out var createdObjectContract))
                {
                    return TypeOrHandle.FromHandle(createdObjectContract);
                }
                return TypeOrHandle.FromClr(typeof(object));

            case Literal lit:
                return lit.Value switch
                {
                    string => TypeOrHandle.FromClr(typeof(string)),
                    bool => TypeOrHandle.FromClr(typeof(bool)),
                    int or long or float or double or decimal => TypeOrHandle.FromClr(typeof(double)),
                    System.Numerics.BigInteger => TypeOrHandle.FromClr(typeof(System.Numerics.BigInteger)),
                    _ => TypeOrHandle.FromClr(typeof(object))
                };

            case BinaryExpression be:
                // Conservative: arithmetic returns double, comparisons return bool.
                if (be.Operator == Operator.Addition
                    && (InferClrTypeFromExpression(be.Left, topLevelIndex, classFields, instanceInterfacesByClassName, ensureClassInstanceInterface).ClrType == typeof(string)
                        || InferClrTypeFromExpression(be.Right, topLevelIndex, classFields, instanceInterfacesByClassName, ensureClassInstanceInterface).ClrType == typeof(string)))
                {
                    return TypeOrHandle.FromClr(typeof(string));
                }

                return be.Operator is Operator.Equality or Operator.Inequality or Operator.StrictEquality or Operator.StrictInequality
                    or Operator.GreaterThan or Operator.GreaterThanOrEqual or Operator.LessThan or Operator.LessThanOrEqual
                    or Operator.In or Operator.InstanceOf
                    ? TypeOrHandle.FromClr(typeof(bool))
                    : TypeOrHandle.FromClr(typeof(double));

            case UnaryExpression ue when ue.Operator == Operator.LogicalNot:
                return TypeOrHandle.FromClr(typeof(bool));

            case Identifier id
                when topLevelIndex != null
                     && topLevelIndex.Value.Functions.ContainsKey(id.Name):
                if (TryGetProjectionInterface(
                        instanceInterfacesByClassName,
                        FallbackCallableContractKey,
                        out var functionContract))
                {
                    return TypeOrHandle.FromHandle(functionContract);
                }
                return TypeOrHandle.FromClr(typeof(object));

            case Identifier id
                when topLevelIndex != null
                     && topLevelIndex.Value.Classes.ContainsKey(id.Name):
                if (TryGetProjectionInterface(
                        instanceInterfacesByClassName,
                        ConstructorContractKeyPrefix + id.Name,
                        out var namedConstructorContract))
                {
                    return TypeOrHandle.FromHandle(namedConstructorContract);
                }
                if (TryGetProjectionInterface(
                        instanceInterfacesByClassName,
                        FallbackConstructorContractKey,
                        out var fallbackConstructorContract))
                {
                    return TypeOrHandle.FromHandle(fallbackConstructorContract);
                }
                return TypeOrHandle.FromClr(typeof(object));

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

    private static bool TryGetProjectionInterface(
        Dictionary<string, TypeDefinitionHandle>? interfaces,
        string key,
        out TypeDefinitionHandle handle)
    {
        if (interfaces != null
            && interfaces.TryGetValue(key, out handle)
            && !handle.IsNil)
        {
            return true;
        }

        handle = default;
        return false;
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

    private void EmitInterfaceMethod(
        TypeBuilder typeBuilder,
        ContractMethod method,
        string? exportName = null,
        bool isExportValue = false,
        bool isParamArray = false)
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

            if ((isParamArray || method.IsParamArray) && i == method.ParamNames.Count - 1)
            {
                _metadata.AddCustomAttribute(
                    handle,
                    _bcl.ParamArrayAttribute_Ctor_Ref,
                    CreateParameterlessAttributeValue());
            }
        }

        var signature = BuildMethodSignature(
            isInstance: true,
            paramNames: method.ParamNames,
            paramTypes: method.ParamTypes,
            returnType: method.ReturnType);

        var methodHandle = typeBuilder.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
            method.Name,
            signature,
            bodyOffset: -1,
            parameterList: firstParam);

        AddExportMemberAttributes(methodHandle, exportName, isExportValue);
    }

    private PropertyDefinitionHandle EmitReadOnlyProperty(
        TypeBuilder typeBuilder,
        string propertyName,
        TypeOrHandle propertyType,
        string? exportName = null,
        bool isExportValue = false)
        => EmitProperty(typeBuilder, propertyName, propertyType, canWrite: false, exportName, isExportValue);

    private PropertyDefinitionHandle EmitProperty(
        TypeBuilder typeBuilder,
        string propertyName,
        TypeOrHandle propertyType,
        bool canWrite,
        string? exportName = null,
        bool isExportValue = false,
        bool emitGetter = true)
    {
        MethodDefinitionHandle getter = default;
        MethodDefinitionHandle setter = default;

        if (emitGetter)
        {
            var getterName = "get_" + propertyName;

            var getterSig = BuildMethodSignature(
                isInstance: true,
                paramNames: Array.Empty<string>(),
                paramTypes: Array.Empty<TypeOrHandle>(),
                returnType: propertyType);

            getter = typeBuilder.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot,
                getterName,
                getterSig,
                bodyOffset: -1);
            AddExportMemberAttributes(getter, exportName, isExportValue);
        }

        if (canWrite)
        {
            var parameter = _metadata.AddParameter(
                attributes: ParameterAttributes.None,
                name: _metadata.GetOrAddString("value"),
                sequenceNumber: 1);
            var setterSig = BuildMethodSignature(
                isInstance: true,
                paramNames: ["value"],
                paramTypes: [propertyType],
                returnType: TypeOrHandle.FromClr(typeof(void)));

            setter = typeBuilder.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot,
                "set_" + propertyName,
                setterSig,
                bodyOffset: -1,
                parameterList: parameter);
            AddExportMemberAttributes(setter, exportName, isExportValue);
        }

        // Property signature
        var propSig = BuildPropertySignature(propertyType);
        var propHandle = _metadata.AddProperty(
            attributes: PropertyAttributes.None,
            name: _metadata.GetOrAddString(propertyName),
            signature: propSig);

        if (!getter.IsNil)
        {
            _metadata.AddMethodSemantics(propHandle, MethodSemanticsAttributes.Getter, getter);
        }

        if (!setter.IsNil)
        {
            _metadata.AddMethodSemantics(propHandle, MethodSemanticsAttributes.Setter, setter);
        }

        return propHandle;
    }

    private void AddExportMemberAttributes(
        MethodDefinitionHandle method,
        string? exportName,
        bool isExportValue)
    {
        if (exportName != null)
        {
            _metadata.AddCustomAttribute(
                method,
                _generatedMetadata.JsExportNameAttributeCtor,
                CreateSingleStringCustomAttributeValue(exportName));
        }

        if (isExportValue)
        {
            _metadata.AddCustomAttribute(
                method,
                _generatedMetadata.JsExportValueAttributeCtor,
                CreateParameterlessAttributeValue());
        }
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
            if (type.ClrType == typeof(object[]))
            {
                encoder.SZArray().Object();
                return;
            }

            if (type.ClrType == typeof(object)) { encoder.Object(); return; }
            if (type.ClrType == typeof(string)) { encoder.String(); return; }
            if (type.ClrType == typeof(double)) { encoder.Double(); return; }
            if (type.ClrType == typeof(bool)) { encoder.Boolean(); return; }

            // Non-primitive reference types (e.g., Task)
            if (!type.ClrType.IsGenericType)
            {
                encoder.Type(
                    _typeRefs.GetOrAdd(type.ClrType),
                    isValueType: type.ClrType.IsValueType);
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
            constructor: _generatedMetadata.JsModuleAttributeCtor,
            value: valueBlob);
    }

    private void AddGeneratedMarkerAttribute(
        TypeDefinitionHandle typeDefinition,
        MethodDefinitionHandle constructor)
    {
        _metadata.AddCustomAttribute(
            typeDefinition,
            constructor,
            CreateParameterlessAttributeValue());
    }

    private BlobHandle CreateParameterlessAttributeValue()
    {
        var blob = new BlobBuilder();
        blob.WriteUInt16(0x0001);
        blob.WriteUInt16(0);
        return _metadata.GetOrAddBlob(blob);
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

    private static string GetAvailableContractMemberName(
        string preferredName,
        IEnumerable<string> existingNames)
    {
        var used = new HashSet<string>(existingNames, StringComparer.OrdinalIgnoreCase);
        if (used.Add(preferredName))
        {
            return preferredName;
        }

        var index = 1;
        while (true)
        {
            var candidate = preferredName + index.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (used.Add(candidate))
            {
                return candidate;
            }

            index++;
        }
    }
}
