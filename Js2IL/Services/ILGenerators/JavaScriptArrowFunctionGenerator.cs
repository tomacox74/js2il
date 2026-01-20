using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Acornima.Ast;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Services.ILGenerators
{
    internal sealed class JavaScriptArrowFunctionGenerator
    {
        private readonly BaseClassLibraryReferences _bclReferences;
        private readonly MetadataBuilder _metadataBuilder;
        private readonly MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private readonly SymbolTable? _symbolTable;
        private readonly CallableRegistry _callableRegistry;

        private readonly IServiceProvider _serviceProvider;

        public JavaScriptArrowFunctionGenerator(
            IServiceProvider serviceProvider,
            BaseClassLibraryReferences bclReferences,
            MetadataBuilder metadataBuilder,
            MethodBodyStreamEncoder methodBodyStreamEncoder,
            SymbolTable? symbolTable = null)
        {
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            _methodBodyStreamEncoder = methodBodyStreamEncoder;
            _symbolTable = symbolTable;
            _serviceProvider = serviceProvider;
            _callableRegistry = serviceProvider.GetRequiredService<CallableRegistry>();
        }

        internal MethodDefinitionHandle GenerateArrowFunctionMethod(
            ArrowFunctionExpression arrowFunction,
            string arrowTypeName)
        {
            const string ilMethodName = "__js_call__";

            // Two-phase lookup:
            // - Phase 1 may preallocate a MethodDef token for this arrow.
            // - A MethodDef token does NOT imply the body is compiled.
            // Only short-circuit when the registry says the body is compiled.
            MethodDefinitionHandle? expectedPreallocatedHandle = null;
            if (_callableRegistry.TryGetDeclaredTokenForAstNode(arrowFunction, out var existingToken) &&
                existingToken.Kind == HandleKind.MethodDefinition)
            {
                var existingMdh = (MethodDefinitionHandle)existingToken;
                if (_callableRegistry.IsBodyCompiledForAstNode(arrowFunction))
                {
                    return existingMdh;
                }
                expectedPreallocatedHandle = existingMdh;
            }

            if (_symbolTable == null)
            {
                throw new NotSupportedException("Arrow-function compilation requires a SymbolTable (IR-only path)." );
            }

            var arrowScope = _symbolTable.FindScopeByAstNode(arrowFunction);
            if (arrowScope == null)
            {
                throw new InvalidOperationException("Arrow function scope not found in SymbolTable.");
            }

            var methodCompiler = _serviceProvider.GetRequiredService<JsMethodCompiler>();

            // Two-phase: compile body-only using preallocated MethodDef, then finalize.
            if (expectedPreallocatedHandle.HasValue)
            {
                if (!_callableRegistry.TryGetCallableIdForAstNode(arrowFunction, out var callableId))
                {
                    throw new InvalidOperationException("[TwoPhase] Expected callable id for preallocated arrow function, but none was registered.");
                }

                var compiledBody = methodCompiler.TryCompileCallableBody(
                    callable: callableId,
                    expectedMethodDef: expectedPreallocatedHandle.Value,
                    ilMethodName: ilMethodName,
                    node: arrowFunction,
                    scope: arrowScope,
                    methodBodyStreamEncoder: _methodBodyStreamEncoder,
                    isInstanceMethod: false,
                    hasScopesParameter: true,
                    scopesFieldHandle: null,
                    returnsVoid: false);

                IR.IRPipelineMetrics.RecordArrowFunctionAttempt(compiledBody != null);
                if (compiledBody == null)
                {
                    throw new NotSupportedException($"IR pipeline could not compile arrow function '{ilMethodName}' in scope '{arrowScope.GetQualifiedName()}'.");
                }

                var irTb = new TypeBuilder(_metadataBuilder, string.Empty, arrowTypeName);
                _ = MethodDefinitionFinalizer.EmitMethod(_metadataBuilder, irTb, compiledBody);

                _callableRegistry.SetDeclaredTokenForAstNode(arrowFunction, expectedPreallocatedHandle.Value);
                _callableRegistry.MarkBodyCompiledForAstNode(arrowFunction);
                return expectedPreallocatedHandle.Value;
            }

            // Non-two-phase: compile directly (emits MethodDef + TypeDef).
            var mdh = methodCompiler.TryCompileArrowFunction(arrowTypeName, ilMethodName, arrowFunction, arrowScope, _methodBodyStreamEncoder);
            IR.IRPipelineMetrics.RecordArrowFunctionAttempt(!mdh.IsNil);
            if (mdh.IsNil)
            {
                throw new NotSupportedException($"IR pipeline could not compile arrow function '{arrowTypeName}' in scope '{arrowScope.GetQualifiedName()}'.");
            }

            _callableRegistry.SetDeclaredTokenForAstNode(arrowFunction, mdh);
            _callableRegistry.MarkBodyCompiledForAstNode(arrowFunction);
            return mdh;
        }
    }
}
