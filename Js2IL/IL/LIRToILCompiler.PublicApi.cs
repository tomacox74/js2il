using Js2IL.IR;
using Js2IL.Services;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Services.VariableBindings;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using Js2IL.DebugSymbols;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    private List<MethodSequencePoint>? _sequencePoints;
    private StandaloneSignatureHandle _localVariablesSignature;
    private int _ilLength;
    private DebugSymbolRegistry.MethodLocal[]? _locals;

    #region Public API

    /// <summary>
    /// Two-phase API: compile a callable body to IL and return the resulting body metadata
    /// without emitting a MethodDef row. The MethodDef row is emitted later in a deterministic
    /// per-type order.
    /// </summary>
    public CompiledCallableBody? TryCompileCallableBody(
        CallableId callable,
        MethodDefinitionHandle expectedMethodDef,
        MethodDescriptor methodDescriptor,
        MethodBodyIR methodBody,
        MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        if (_compiled)
        {
            throw new InvalidOperationException("LIRToILCompiler can only compile a single method. Create a new instance for each method.");
        }
        _compiled = true;
        _methodBody = methodBody;

        var methodSig = BuildMethodSignature(methodDescriptor);

        // Compile body
        if (!TryCompileMethodBodyToIL(methodDescriptor, methodBodyStreamEncoder, out var bodyOffset))
        {
            return null;
        }

        var methodAttributes = ComputeMethodAttributes(methodDescriptor);

        var points = _sequencePoints?.ToArray() ?? Array.Empty<MethodSequencePoint>();
        _serviceProvider.GetService<DebugSymbolRegistry>()?.SetMethodDebugInfo(
            expectedMethodDef,
            points,
            _localVariablesSignature,
            _ilLength,
            _locals ?? Array.Empty<DebugSymbolRegistry.MethodLocal>());

        var result = new CompiledCallableBody
        {
            Callable = callable,
            MethodName = methodDescriptor.Name,
            ExpectedMethodDef = expectedMethodDef,
            Attributes = methodAttributes,
            Signature = methodSig,
            BodyOffset = bodyOffset,
            ParameterNames = methodDescriptor.Parameters.Select(p => p.Name).ToArray(),
            SequencePoints = points
        };
        result.Validate();
        return result;
    }

    public MethodDefinitionHandle TryCompile(MethodDescriptor methodDescriptor, MethodBodyIR methodBody, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        var (methodDef, _) = TryCompileWithSignature(methodDescriptor, methodBody, methodBodyStreamEncoder);
        return methodDef;
    }

    public (MethodDefinitionHandle MethodDef, BlobHandle Signature) TryCompileWithSignature(MethodDescriptor methodDescriptor, MethodBodyIR methodBody, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        if (_compiled)
        {
            throw new InvalidOperationException("LIRToILCompiler can only compile a single method. Create a new instance for each method.");
        }
        _compiled = true;
        _methodBody = methodBody;

        var programTypeBuilder = methodDescriptor.TypeBuilder;

        var methodSig = BuildMethodSignature(methodDescriptor);

        // Compile the method body to IL
        if (!TryCompileMethodBodyToIL(methodDescriptor, methodBodyStreamEncoder, out var bodyOffset))
        {
            // Failed to compile IL
            return default;
        }

        var parameterNames = methodDescriptor.Parameters.Select(p => p.Name).ToArray();

        var methodAttributes = ComputeMethodAttributes(methodDescriptor);

        var methodDefinitionHandle = programTypeBuilder.AddMethodDefinition(
            methodAttributes,
            methodDescriptor.Name,
            methodSig,
            bodyOffset);

        var points = _sequencePoints?.ToArray() ?? Array.Empty<MethodSequencePoint>();
        _serviceProvider.GetService<DebugSymbolRegistry>()?.SetMethodDebugInfo(
            methodDefinitionHandle,
            points,
            _localVariablesSignature,
            _ilLength,
            _locals ?? Array.Empty<DebugSymbolRegistry.MethodLocal>());

        // Add parameter names to metadata (sequence starts at 1 for first parameter)
        int sequence = 1;
        foreach (var paramName in parameterNames)
        {
            _metadataBuilder.AddParameter(
                ParameterAttributes.None,
                _metadataBuilder.GetOrAddString(paramName),
                sequence++);
        }

        return (methodDefinitionHandle, methodSig);
    }

    #endregion
}