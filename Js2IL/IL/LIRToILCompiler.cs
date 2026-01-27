using Js2IL.IR;
using Js2IL.Services;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Services.VariableBindings;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

/// <summary>
/// Compiles LIR (Low-level IR) to IL bytecode.
/// </summary>
/// <remarks>
/// This class is responsible for the final stage of the JS to IL compilation pipeline:
/// LIR -> IL.
/// This class is designed for single-use; create a new instance for each method compilation.
/// </remarks>
internal sealed partial class LIRToILCompiler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MetadataBuilder _metadataBuilder;
    private readonly TypeReferenceRegistry _typeReferenceRegistry;
    private readonly BaseClassLibraryReferences _bclReferences;
    private readonly MemberReferenceRegistry _memberRefRegistry;
    private readonly ScopeMetadataRegistry _scopeMetadataRegistry;
    private MethodBodyIR? _methodBody;
    private bool _compiled;

    private static void EmitReturnType(ReturnTypeEncoder returnType, Type clrReturnType, EntityHandle returnTypeHandle = default)
    {
        if (!returnTypeHandle.IsNil)
        {
            returnType.Type().Type(returnTypeHandle, isValueType: false);
            return;
        }

        if (clrReturnType == typeof(double))
        {
            returnType.Type().Double();
            return;
        }

        if (clrReturnType == typeof(bool))
        {
            returnType.Type().Boolean();
            return;
        }

        if (clrReturnType == typeof(string))
        {
            returnType.Type().String();
            return;
        }

        // Default ABI: JavaScript value as object.
        returnType.Type().Object();
    }

    private BlobHandle BuildMethodSignature(MethodDescriptor methodDescriptor)
    {
        var methodParameters = methodDescriptor.Parameters;

        var sigBuilder = new BlobBuilder();
        new BlobEncoder(sigBuilder)
            .MethodSignature(isInstanceMethod: !methodDescriptor.IsStatic)
            .Parameters(methodParameters.Count, returnType =>
            {
                if (methodDescriptor.ReturnsVoid)
                {
                    returnType.Void();
                }
                else
                {
                    EmitReturnType(returnType, methodDescriptor.ReturnClrType, methodDescriptor.ReturnTypeHandle);
                }
            }, parameters =>
            {
                for (int i = 0; i < methodParameters.Count; i++)
                {
                    var parameterDefinition = methodParameters[i];

                    if (parameterDefinition.ParameterType == typeof(object))
                    {
                        parameters.AddParameter().Type().Object();
                    }
                    else if (parameterDefinition.ParameterType == typeof(string))
                    {
                        parameters.AddParameter().Type().String();
                    }
                    else if (parameterDefinition.ParameterType.IsArray
                        && parameterDefinition.ParameterType.GetElementType() == typeof(object))
                    {
                        parameters.AddParameter().Type().SZArray().Object();
                    }
                    else
                    {
                        var typeRef = _typeReferenceRegistry.GetOrAdd(parameterDefinition.ParameterType!);
                        parameters.AddParameter().Type().Type(typeRef, false);
                    }
                }
            });

        return _metadataBuilder.GetOrAddBlob(sigBuilder);
    }

    private static MethodAttributes ComputeMethodAttributes(MethodDescriptor methodDescriptor)
    {
        MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig;

        if (string.Equals(methodDescriptor.Name, ".ctor", StringComparison.Ordinal))
        {
            methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        }
        else if (string.Equals(methodDescriptor.Name, ".cctor", StringComparison.Ordinal))
        {
            methodAttributes = MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        }
        else if (methodDescriptor.IsStatic)
        {
            methodAttributes |= MethodAttributes.Static;
        }

        return methodAttributes;
    }

    private static int GetIlArgIndexForScopesArray(MethodDescriptor methodDescriptor)
    {
        if (!methodDescriptor.HasScopesParameter)
        {
            throw new InvalidOperationException("Expected a scopes parameter but methodDescriptor.HasScopesParameter is false");
        }

        // Static: arg0 = scopes. Instance: arg0 = this, arg1 = scopes.
        return methodDescriptor.IsStatic ? 0 : 1;
    }

    private Type GetDeclaredScopeFieldClrType(string scopeName, string fieldName)
    {
        if (TryGetAsyncScopeBaseFieldClrType(fieldName, out var asyncFieldType))
            return asyncFieldType;

        if (TryGetGeneratorScopeBaseFieldClrType(fieldName, out var generatorFieldType))
            return generatorFieldType;

        return _scopeMetadataRegistry.TryGetFieldClrType(scopeName, fieldName, out var t)
            ? t
            : typeof(object);
    }

    private static bool TryGetAsyncScopeBaseFieldClrType(string fieldName, out Type fieldClrType)
    {
        fieldClrType = fieldName switch
        {
            "_asyncState" => typeof(int),
            "_deferred" => typeof(JavaScriptRuntime.PromiseWithResolvers),
            "_moveNext" => typeof(object),
            "_pendingException" => typeof(object),
            "_hasPendingException" => typeof(bool),
            "_pendingReturnValue" => typeof(object),
            "_hasPendingReturn" => typeof(bool),
            _ => typeof(object)
        };

        return fieldName is "_asyncState"
            or "_deferred"
            or "_moveNext"
            or "_pendingException"
            or "_hasPendingException"
            or "_pendingReturnValue"
            or "_hasPendingReturn";
    }

    private static bool TryGetGeneratorScopeBaseFieldClrType(string fieldName, out Type fieldClrType)
    {
        fieldClrType = fieldName switch
        {
            "_genState" => typeof(int),
            "_started" => typeof(bool),
            "_done" => typeof(bool),
            "_resumeValue" => typeof(object),
            "_resumeException" => typeof(object),
            "_hasResumeException" => typeof(bool),
            "_returnValue" => typeof(object),
            "_hasReturn" => typeof(bool),
            "_yieldStarMode" => typeof(double),
            "_yieldStarTarget" => typeof(object),
            "_yieldStarIndex" => typeof(double),
            "_yieldStarLength" => typeof(double),
            _ => typeof(object)
        };

        return fieldName is "_genState"
            or "_started"
            or "_done"
            or "_resumeValue"
            or "_resumeException"
            or "_hasResumeException"
            or "_returnValue"
            or "_hasReturn"
            or "_yieldStarMode"
            or "_yieldStarTarget"
            or "_yieldStarIndex"
            or "_yieldStarLength";
    }

    private bool TryResolveAsyncScopeBaseFieldToken(string fieldName, out EntityHandle token)
    {
        if (!TryGetAsyncScopeBaseFieldClrType(fieldName, out _))
        {
            token = default;
            return false;
        }

        token = _memberRefRegistry.GetOrAddField(typeof(JavaScriptRuntime.AsyncScope), fieldName);
        return true;
    }

    private bool TryResolveGeneratorScopeBaseFieldToken(string fieldName, out EntityHandle token)
    {
        if (!TryGetGeneratorScopeBaseFieldClrType(fieldName, out _))
        {
            token = default;
            return false;
        }

        token = _memberRefRegistry.GetOrAddField(typeof(JavaScriptRuntime.GeneratorScope), fieldName);
        return true;
    }

    private void EmitCastOrBoxAfterTypedFieldLoad(
        Type fieldClrType,
        ValueStorage targetStorage,
        InstructionEncoder ilEncoder,
        bool allowReferenceNarrowingCast)
    {
        // If the loaded value is reference-typed (often object) but the consumer expects a more specific
        // reference type (e.g., JavaScriptRuntime.Array), insert a castclass.
        if (allowReferenceNarrowingCast
            && !fieldClrType.IsValueType
            && targetStorage.Kind == ValueStorageKind.Reference
            && targetStorage.ClrType != null
            && targetStorage.ClrType != typeof(object)
            && fieldClrType != targetStorage.ClrType)
        {
            ilEncoder.OpCode(ILOpCode.Castclass);
            ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(targetStorage.ClrType));
            return;
        }

        if (!fieldClrType.IsValueType)
            return;

        // If the consumer expects an unboxed value with matching CLR type, do not box.
        if (targetStorage.Kind == ValueStorageKind.UnboxedValue && targetStorage.ClrType == fieldClrType)
            return;

        // Otherwise, box the loaded value type so downstream code (which is often object-based) continues to work.
        if (fieldClrType == typeof(double))
        {
            ilEncoder.OpCode(ILOpCode.Box);
            ilEncoder.Token(_bclReferences.DoubleType);
        }
        else if (fieldClrType == typeof(bool))
        {
            ilEncoder.OpCode(ILOpCode.Box);
            ilEncoder.Token(_bclReferences.BooleanType);
        }
    }

    private void EmitBoxIfNeededForTypedScopeFieldLoad(Type fieldClrType, ValueStorage targetStorage, InstructionEncoder ilEncoder)
    {
        EmitCastOrBoxAfterTypedFieldLoad(fieldClrType, targetStorage, ilEncoder, allowReferenceNarrowingCast: true);
    }

    private static Type GetDeclaredUserClassFieldClrType(
        Js2IL.Services.ClassRegistry classRegistry,
        string registryClassName,
        string fieldName,
        bool isPrivateField,
        bool isStaticField)
    {
        if (isStaticField)
        {
            return classRegistry.TryGetStaticFieldClrType(registryClassName, fieldName, out var t)
                ? t
                : typeof(object);
        }

        if (isPrivateField)
        {
            return classRegistry.TryGetPrivateFieldClrType(registryClassName, fieldName, out var t)
                ? t
                : typeof(object);
        }

        return classRegistry.TryGetFieldClrType(registryClassName, fieldName, out var t2)
            ? t2
            : typeof(object);
    }

    private static bool TryGetDeclaredUserClassFieldTypeHandle(
        Js2IL.Services.ClassRegistry classRegistry,
        string registryClassName,
        string fieldName,
        bool isPrivateField,
        bool isStaticField,
        out EntityHandle typeHandle)
    {
        typeHandle = default;

        if (isStaticField)
        {
            return classRegistry.TryGetStaticFieldTypeHandle(registryClassName, fieldName, out typeHandle);
        }

        if (isPrivateField)
        {
            return classRegistry.TryGetPrivateFieldTypeHandle(registryClassName, fieldName, out typeHandle);
        }

        return classRegistry.TryGetFieldTypeHandle(registryClassName, fieldName, out typeHandle);
    }

    private void EmitBoxIfNeededForTypedUserClassFieldLoad(Type fieldClrType, ValueStorage targetStorage, InstructionEncoder ilEncoder)
    {
        EmitCastOrBoxAfterTypedFieldLoad(fieldClrType, targetStorage, ilEncoder, allowReferenceNarrowingCast: false);
    }

    private void EmitLoadTempAsDouble(TempVariable value, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        var storage = GetTempStorage(value);
        if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(double))
        {
            EmitLoadTemp(value, ilEncoder, allocation, methodDescriptor);
            return;
        }

        // Peephole: avoid boxing a known double just to immediately coerce it back to double.
        // This happens when lowering inserts ConvertToObject around numeric literals/constants.
        if (!IsMaterialized(value, allocation) && TryFindDefInstruction(value) is LIRConvertToObject convertToObject && convertToObject.SourceType == typeof(double))
        {
            EmitLoadTemp(convertToObject.Source, ilEncoder, allocation, methodDescriptor);
            return;
        }

        EmitLoadTempAsObject(value, ilEncoder, allocation, methodDescriptor);
        var toNumberMref = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.TypeUtilities),
            nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
            parameterTypes: new[] { typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(toNumberMref);
    }

    private void EmitLoadTempAsBoolean(TempVariable value, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        var storage = GetTempStorage(value);
        if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(bool))
        {
            EmitLoadTemp(value, ilEncoder, allocation, methodDescriptor);
            return;
        }

        EmitLoadTempAsObject(value, ilEncoder, allocation, methodDescriptor);
        var toBooleanMref = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.TypeUtilities),
            nameof(JavaScriptRuntime.TypeUtilities.ToBoolean),
            parameterTypes: new[] { typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(toBooleanMref);
    }

    private void EmitLoadTempAsString(TempVariable value, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        var storage = GetTempStorage(value);
        if (storage.Kind == ValueStorageKind.Reference && storage.ClrType == typeof(string))
        {
            EmitLoadTemp(value, ilEncoder, allocation, methodDescriptor);
            return;
        }

        EmitLoadTempAsObject(value, ilEncoder, allocation, methodDescriptor);
        ilEncoder.OpCode(ILOpCode.Castclass);
        ilEncoder.Token(_bclReferences.StringType);
    }

    private static int GetIlArgIndexForJsParameter(MethodDescriptor methodDescriptor, int jsParameterIndex)
    {
        // Base IL-argument index for JS parameter 0:
        // - static without scopes: base=0
        // - static with scopes: base=1 (arg0=scopes)
        // - instance without scopes: base=1 (arg0=this)
        // - instance with scopes: base=2 (arg0=this, arg1=scopes)
        int baseIndex = (methodDescriptor.IsStatic ? 0 : 1) + (methodDescriptor.HasScopesParameter ? 1 : 0);
        return baseIndex + jsParameterIndex;
    }

    /// <summary>
    /// Gets the method body, throwing if not yet set.
    /// </summary>
    private MethodBodyIR MethodBody => _methodBody ?? throw new InvalidOperationException("MethodBody has not been set.");

    public LIRToILCompiler(
        MetadataBuilder metadataBuilder,
        TypeReferenceRegistry typeReferenceRegistry,
        MemberReferenceRegistry memberReferenceRegistry,
        BaseClassLibraryReferences bclReferences,
        ScopeMetadataRegistry scopeMetadataRegistry,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _metadataBuilder = metadataBuilder;
        _typeReferenceRegistry = typeReferenceRegistry;
        _bclReferences = bclReferences;
        _memberRefRegistry = memberReferenceRegistry;
        _scopeMetadataRegistry = scopeMetadataRegistry;
    }

    // Public API moved to LIRToILCompiler.PublicApi.cs

    // Method Body Compilation moved to LIRToILCompiler.MethodBodyCompilation.cs

    // Instruction Emission moved to LIRToILCompiler.InstructionEmission.cs

    // Branch Condition Handling moved to LIRToILCompiler.Branching.cs

    // Temp/Local Variable Management moved to LIRToILCompiler.TempsLocals.cs

    // Intrinsic/Runtime Helpers moved to LIRToILCompiler.IntrinsicRuntimeHelpers.cs

    // Handle Resolution Helpers moved to LIRToILCompiler.Handles.cs
}