using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Jroc.Runtime;
using Jroc.Utilities.Ecma335;

namespace Jroc.Services;

internal sealed record FacadeEmissionResult(
    MethodDefinitionHandle RootRunMethod,
    IReadOnlyDictionary<string, TypeDefinitionHandle> ModuleTypes);

internal sealed class FacadeEmitter
{
    private readonly MetadataBuilder _metadata;
    private readonly BaseClassLibraryReferences _bclReferences;
    private readonly MemberReferenceRegistry _memberReferences;
    private readonly NestedTypeRelationshipRegistry _nestedTypes;

    internal FacadeEmitter(
        MetadataBuilder metadata,
        BaseClassLibraryReferences bclReferences,
        MemberReferenceRegistry memberReferences,
        NestedTypeRelationshipRegistry nestedTypes)
    {
        _metadata = metadata;
        _bclReferences = bclReferences;
        _memberReferences = memberReferences;
        _nestedTypes = nestedTypes;
    }

    internal int GetMethodCount(
        JrocFacadeNamePlan plan,
        IReadOnlyDictionary<string, TypeDefinitionHandle> moduleExportContracts)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(moduleExportContracts);

        var importMethodCount = plan.Modules.Count(module => moduleExportContracts.ContainsKey(module.ModuleId));
        if (moduleExportContracts.ContainsKey(plan.EntryModuleId))
        {
            importMethodCount++;
        }

        return plan.Modules.Count + 1 + importMethodCount;
    }

    internal int GetTypeCount(JrocFacadeNamePlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        return Flatten(BuildTypeTree(plan)).Count();
    }

    internal FacadeEmissionResult Emit(
        JrocFacadeNamePlan plan,
        IReadOnlyList<ModuleDefinition> modules,
        IReadOnlyDictionary<string, MethodDefinitionHandle> moduleInitializers,
        IReadOnlyDictionary<string, TypeDefinitionHandle> moduleExportContracts,
        MethodBodyStreamEncoder methodBodyStream)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(modules);
        ArgumentNullException.ThrowIfNull(moduleInitializers);
        ArgumentNullException.ThrowIfNull(moduleExportContracts);

        var root = BuildTypeTree(plan);
        var types = Flatten(root).ToArray();
        var moduleById = modules
            .GroupBy(module => module.ModuleId, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal);

        foreach (var type in types.Where(type => type.ModuleId is not null))
        {
            var moduleId = type.ModuleId!;
            if (!moduleById.TryGetValue(moduleId, out var module)
                || !moduleInitializers.TryGetValue(module.Name, out var moduleInitializer))
            {
                throw new InvalidOperationException(
                    $"Could not resolve module initializer for facade module '{moduleId}'.");
            }

            type.RunMethod = EmitRunMethod(
                methodBodyStream,
                moduleInitializer,
                moduleId);
            if (moduleExportContracts.TryGetValue(moduleId, out var exportContract))
            {
                type.ImportMethod = EmitImportMethod(
                    methodBodyStream,
                    moduleInitializer,
                    moduleId,
                    exportContract);
            }
        }

        var nextMethod = MetadataTokens.MethodDefinitionHandle(
            _metadata.GetRowCount(TableIndex.MethodDef) + 1);
        for (var index = 0; index < types.Length; index++)
        {
            var type = types[index];
            var firstMethod = !type.FirstMethod.IsNil
                ? type.FirstMethod
                : types.Skip(index + 1)
                    .Select(candidate => candidate.FirstMethod)
                    .FirstOrDefault(handle => !handle.IsNil);
            if (firstMethod.IsNil)
            {
                firstMethod = nextMethod;
            }

            var typeBuilder = new TypeBuilder(
                _metadata,
                string.Empty,
                type.Name);
            var visibility = type.Parent is null
                ? TypeAttributes.Public
                : TypeAttributes.NestedPublic;
            type.TypeHandle = typeBuilder.AddTypeDefinition(
                visibility
                | TypeAttributes.Class
                | TypeAttributes.Abstract
                | TypeAttributes.Sealed
                | TypeAttributes.BeforeFieldInit,
                _bclReferences.ObjectType,
                firstFieldOverride: null,
                firstMethodOverride: firstMethod);

            if (type.Parent is not null)
            {
                _nestedTypes.Add(type.TypeHandle, type.Parent.TypeHandle);
            }
        }

        var moduleTypes = types
            .Where(type => type.Parent is not null && type.ModuleId is not null)
            .ToDictionary(
                type => type.ModuleId!,
                type => type.TypeHandle,
                StringComparer.Ordinal);

        return new FacadeEmissionResult(root.RunMethod, moduleTypes);
    }

    private MethodDefinitionHandle EmitRunMethod(
        MethodBodyStreamEncoder methodBodyStream,
        MethodDefinitionHandle moduleInitializer,
        string moduleId)
    {
        var signatureBuilder = new BlobBuilder();
        new BlobEncoder(signatureBuilder)
            .MethodSignature(isInstanceMethod: false)
            .Parameters(
                parameterCount: 1,
                returnType => returnType.Void(),
                parameters => parameters.AddParameter().Type().SZArray().String());

        var parameter = _metadata.AddParameter(
            ParameterAttributes.None,
            _metadata.GetOrAddString("args"),
            sequenceNumber: 1);
        _metadata.AddCustomAttribute(
            parameter,
            _memberReferences.GetOrAddConstructor(
                typeof(ParamArrayAttribute),
                Type.EmptyTypes),
            CreateParameterlessAttributeValue());

        var bodyBuilder = new BlobBuilder();
        var il = new InstructionEncoder(bodyBuilder);
        il.OpCode(ILOpCode.Ldnull);
        il.OpCode(ILOpCode.Ldftn);
        il.Token(moduleInitializer);
        il.OpCode(ILOpCode.Newobj);
        il.Token(_bclReferences.ModuleMainDelegate_Ctor_Ref);
        il.LoadString(_metadata.GetOrAddUserString(moduleId));
        il.LoadArgument(0);
        il.OpCode(ILOpCode.Call);
        il.Token(_memberReferences.GetOrAddMethod(
            typeof(CompiledScriptRunner),
            nameof(CompiledScriptRunner.Run),
            [
                typeof(JavaScriptRuntime.Modules.CommonJS.ModuleMainDelegate),
                typeof(string),
                typeof(string[])
            ]));
        il.OpCode(ILOpCode.Ret);

        var bodyOffset = methodBodyStream.AddMethodBody(
            il,
            maxStack: 3,
            localVariablesSignature: default,
            attributes: MethodBodyAttributes.None);

        return _metadata.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
            MethodImplAttributes.IL,
            _metadata.GetOrAddString("Run"),
            _metadata.GetOrAddBlob(signatureBuilder),
            bodyOffset,
            parameter);
    }

    private MethodDefinitionHandle EmitImportMethod(
        MethodBodyStreamEncoder methodBodyStream,
        MethodDefinitionHandle moduleInitializer,
        string moduleId,
        TypeDefinitionHandle exportContract)
    {
        var signatureBuilder = new BlobBuilder();
        new BlobEncoder(signatureBuilder)
            .MethodSignature(isInstanceMethod: false)
            .Parameters(
                parameterCount: 0,
                returnType => returnType.Type().Type(exportContract, false),
                parameters => { });

        var bodyBuilder = new BlobBuilder();
        var il = new InstructionEncoder(bodyBuilder);
        il.OpCode(ILOpCode.Ldtoken);
        il.Token(exportContract);
        il.OpCode(ILOpCode.Call);
        il.Token(_bclReferences.Type_GetTypeFromHandle_Ref);
        il.OpCode(ILOpCode.Ldnull);
        il.OpCode(ILOpCode.Ldftn);
        il.Token(moduleInitializer);
        il.OpCode(ILOpCode.Newobj);
        il.Token(_bclReferences.ModuleMainDelegate_Ctor_Ref);
        il.LoadString(_metadata.GetOrAddUserString(moduleId));
        il.OpCode(ILOpCode.Call);
        il.Token(_memberReferences.GetOrAddMethod(
            typeof(CompiledScriptRunner),
            nameof(CompiledScriptRunner.Import),
            [
                typeof(Type),
                typeof(JavaScriptRuntime.Modules.CommonJS.ModuleMainDelegate),
                typeof(string)
            ]));
        il.OpCode(ILOpCode.Castclass);
        il.Token(exportContract);
        il.OpCode(ILOpCode.Ret);

        var bodyOffset = methodBodyStream.AddMethodBody(
            il,
            maxStack: 4,
            localVariablesSignature: default,
            attributes: MethodBodyAttributes.None);

        return _metadata.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
            MethodImplAttributes.IL,
            _metadata.GetOrAddString("Import"),
            _metadata.GetOrAddBlob(signatureBuilder),
            bodyOffset,
            parameterList: MetadataTokens.ParameterHandle(_metadata.GetRowCount(TableIndex.Param) + 1));
    }

    private BlobHandle CreateParameterlessAttributeValue()
    {
        var blob = new BlobBuilder();
        blob.WriteUInt16(0x0001);
        blob.WriteUInt16(0);
        return _metadata.GetOrAddBlob(blob);
    }

    private static FacadeType BuildTypeTree(JrocFacadeNamePlan plan)
    {
        var root = new FacadeType(plan.RootTypeName, plan.EntryModuleId, parent: null);
        var scripts = new FacadeType("Scripts", moduleId: null, root);
        root.Children.Add(scripts);

        foreach (var moduleName in plan.Modules)
        {
            var current = scripts;
            foreach (var segment in moduleName.TypePath)
            {
                var child = current.Children.FirstOrDefault(
                    candidate => string.Equals(candidate.Name, segment, StringComparison.Ordinal));
                if (child is null)
                {
                    child = new FacadeType(segment, moduleId: null, current);
                    current.Children.Add(child);
                }

                current = child;
            }

            current.ModuleId = moduleName.ModuleId;
        }

        SortChildren(root);
        return root;
    }

    private static IEnumerable<FacadeType> Flatten(FacadeType type)
    {
        yield return type;
        foreach (var child in type.Children)
        {
            foreach (var descendant in Flatten(child))
            {
                yield return descendant;
            }
        }
    }

    private static void SortChildren(FacadeType type)
    {
        type.Children.Sort((left, right) => StringComparer.Ordinal.Compare(left.Name, right.Name));
        foreach (var child in type.Children)
        {
            SortChildren(child);
        }
    }

    private sealed class FacadeType(
        string name,
        string? moduleId,
        FacadeType? parent)
    {
        internal string Name { get; } = name;

        internal string? ModuleId { get; set; } = moduleId;

        internal FacadeType? Parent { get; } = parent;

        internal List<FacadeType> Children { get; } = [];

        internal MethodDefinitionHandle RunMethod { get; set; }

        internal MethodDefinitionHandle ImportMethod { get; set; }

        internal MethodDefinitionHandle FirstMethod
            => !RunMethod.IsNil ? RunMethod : ImportMethod;

        internal TypeDefinitionHandle TypeHandle { get; set; }
    }
}
