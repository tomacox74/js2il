using Acornima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services.ILGenerators
{
    internal class JavaScriptFunctionGenerator
    {
        private Variables _variables;
        private BaseClassLibraryReferences _bclReferences;
        private MetadataBuilder _metadataBuilder;
        private MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private Dispatch.DispatchTableGenerator _dispatchTableGenerator;
        private MethodDefinitionHandle _firstMethod = default;

    // Tracks owner types in the Functions namespace
    private readonly Dictionary<string, TypeDefinitionHandle> _globalFunctionOwnerTypes = new();
    private readonly Dictionary<string, TypeDefinitionHandle> _nestedOwnerTypes = new();
    private TypeDefinitionHandle _moduleOwnerType = default;

        public MethodDefinitionHandle FirstMethod => _firstMethod;

        public JavaScriptFunctionGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, Dispatch.DispatchTableGenerator dispatchTableGenerator)
        {
            _variables = variables;
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            _methodBodyStreamEncoder = methodBodyStreamEncoder;
            _dispatchTableGenerator = dispatchTableGenerator;
        }

        public void DeclareFunctions(SymbolTable symbolTable)
        {
            // New hosting model: one owner type per module under the Functions namespace.
            // All top-level functions are static methods on Functions.<RootName>.
            // Nested functions get their own nested owner type under Functions.<RootName>.

            var root = symbolTable.Root;
            var moduleName = root.Name;

            // 1) Plan and emit top-level methods via a TypeBuilder per hosting strategy
            var topLevelFunctions = root.Children.Where(c => c.Kind == ScopeKind.Function && c.AstNode is FunctionDeclaration).ToList();
            var globalMethods = new List<(string Name, MethodDefinitionHandle Handle, Scope Scope, Variables Vars)>();

            if (topLevelFunctions.Count > 1)
            {
                // Module owner type under Functions namespace
                var moduleTb = new TypeBuilder(_metadataBuilder, "Functions", moduleName);

                foreach (var funcScope in topLevelFunctions)
                {
                    var functionDeclaration = (FunctionDeclaration)funcScope.AstNode!;
                    var functionName = (functionDeclaration.Id as Identifier)!.Name;

                    var paramNames = functionDeclaration.Params.OfType<Identifier>().Select(p => p.Name).ToArray();
                    var functionVariables = new Variables(_variables, functionName, paramNames, isNestedFunction: false);
                    var methodGenerator = new ILMethodGenerator(functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _dispatchTableGenerator);
                    var methodDefinition = GenerateMethodForFunction(functionDeclaration, functionVariables, methodGenerator, funcScope, symbolTable, moduleTb);
                    if (this._firstMethod.IsNil) _firstMethod = methodDefinition;

                    globalMethods.Add((functionName, methodDefinition, funcScope, functionVariables));
                }

                // Define the module owner type after adding its methods
                _moduleOwnerType = moduleTb.AddTypeDefinition(
                    TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                    _bclReferences.ObjectType);
            }
            else if (topLevelFunctions.Count == 1)
            {
                // Single top-level function: per-function owner type for compatibility
                var funcScope = topLevelFunctions[0];
                var functionDeclaration = (FunctionDeclaration)funcScope.AstNode!;
                var functionName = (functionDeclaration.Id as Identifier)!.Name;

                var tb = new TypeBuilder(_metadataBuilder, "Functions", functionName);

                var paramNames = functionDeclaration.Params.OfType<Identifier>().Select(p => p.Name).ToArray();
                var functionVariables = new Variables(_variables, functionName, paramNames, isNestedFunction: false);
                var methodGenerator = new ILMethodGenerator(functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _dispatchTableGenerator);
                var methodDefinition = GenerateMethodForFunction(functionDeclaration, functionVariables, methodGenerator, funcScope, symbolTable, tb);
                if (this._firstMethod.IsNil) _firstMethod = methodDefinition;

                globalMethods.Add((functionName, methodDefinition, funcScope, functionVariables));

                var globalOwnerType = tb.AddTypeDefinition(
                    TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                    _bclReferences.ObjectType);
                _globalFunctionOwnerTypes[functionName] = globalOwnerType;
                _moduleOwnerType = default; // not used in this mode
            }

            // 3) Generate nested function methods and create nested owner types under the module owner
            foreach (var gm in globalMethods)
            {
                var funcScope = gm.Scope;
                var functionVariables = gm.Vars;
                var outerName = gm.Name;
                var nestedFunctions = funcScope.Children.Where(c => c.Kind == ScopeKind.Function && c.AstNode is FunctionDeclaration).ToList();
                if (nestedFunctions.Count == 0) continue;

                // Build a nested owner TypeBuilder for this outer function's nested methods
                var nestedTb = new TypeBuilder(_metadataBuilder, "", outerName);
                MethodDefinitionHandle firstNestedMethod = default;
                foreach (var nestedScope in nestedFunctions)
                {
                    var nestedDecl = (FunctionDeclaration)nestedScope.AstNode!;
                    var nestedName = (nestedDecl.Id as Identifier)!.Name;
                    var nestedParamNames = nestedDecl.Params.OfType<Identifier>().Select(p => p.Name).ToArray();
                    var nestedVars = new Variables(functionVariables, nestedName, nestedParamNames, isNestedFunction: true);
                    var nestedGen = new ILMethodGenerator(nestedVars, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _dispatchTableGenerator);
                    var nestedMethod = GenerateMethodForFunction(nestedDecl, nestedVars, nestedGen, nestedScope, symbolTable, nestedTb);
                    if (firstNestedMethod.IsNil) firstNestedMethod = nestedMethod;
                    if (this._firstMethod.IsNil) _firstMethod = nestedMethod;
                }

                if (!firstNestedMethod.IsNil)
                {
                    var nestedHandle = nestedTb.AddTypeDefinition(
                        TypeAttributes.NestedPublic | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                        _bclReferences.ObjectType);
                    var parent = !_moduleOwnerType.IsNil ? _moduleOwnerType : (_globalFunctionOwnerTypes.TryGetValue(outerName, out var t) ? t : default);
                    if (!parent.IsNil)
                    {
                        _metadataBuilder.AddNestedType(nestedHandle, parent);
                        _nestedOwnerTypes[outerName] = nestedHandle;
                    }
                }
            }
        }

        // No longer pre-creates owner types; types are created after methods to ensure the first method handle is correct.
        private TypeDefinitionHandle EnsureGlobalFunctionOwnerType(string functionName)
        {
            return _globalFunctionOwnerTypes.TryGetValue(functionName, out var existing) ? existing : default;
        }

        private TypeDefinitionHandle EnsureNestedOwnerForGlobal(string globalFunctionName, TypeDefinitionHandle parentHandle)
        {
            return _nestedOwnerTypes.TryGetValue(globalFunctionName, out var existing) ? existing : default;
        }

        private void DeclareFunctionsRecursive(Scope scope, Variables parentVars, SymbolTable symbolTable)
        {
            // Generate methods for each function declared directly in this scope
            foreach (var funcScope in scope.Children.Where(c => c.Kind == ScopeKind.Function && c.AstNode is FunctionDeclaration))
            {
                var functionDeclaration = (FunctionDeclaration)funcScope.AstNode!;
                var functionName = (functionDeclaration.Id as Identifier)!.Name;
                var paramNames = functionDeclaration.Params
                    .OfType<Identifier>()
                    .Select(p => p.Name)
                    .ToArray();

                bool isNested = scope.Kind == ScopeKind.Function; // nested if parent is a function
                var functionVariables = new Variables(parentVars, functionName, paramNames, isNestedFunction: isNested);
                var methodGenerator = new ILMethodGenerator(functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _dispatchTableGenerator);
                var methodDefinition = GenerateMethodForFunction(functionDeclaration, functionVariables, methodGenerator, funcScope, symbolTable);
                if (this._firstMethod.IsNil)
                {
                    this._firstMethod = methodDefinition;
                }

                // Recurse into nested functions with this function's Variables as the new parent
                DeclareFunctionsRecursive(funcScope, functionVariables, symbolTable);
            }
        }

    public MethodDefinitionHandle GenerateMethodForFunction(FunctionDeclaration functionDeclaration, Variables functionVariables, ILMethodGenerator methodGenerator, Scope? functionScope = null, SymbolTable? symbolTable = null, TypeBuilder? typeBuilder = null)
        {
            var functionName = (functionDeclaration.Id as Acornima.Ast.Identifier)!.Name;

            if (functionDeclaration.Body is not BlockStatement blockStatement)
            {
                throw new NotSupportedException($"Unsupported function body type: {functionDeclaration.Body.Type}");
            }

            var variables = functionVariables;
            var il = methodGenerator.IL;

            // Parameters are already registered in Variables constructor

            // Create a scope instance for the function itself so that local vars (declared within the function)
            // have a backing scope object to store their fields. The Variables instance for the function only
            // carries the parent (global) scope as parameter 0, so we must allocate a local slot + instantiate.
            var registry = variables.GetVariableRegistry();
            if (registry != null)
            {
                try
                {
                    // Only create a local scope if there are fields in this function scope
                    var fields = registry.GetVariablesForScope(functionName) ?? Enumerable.Empty<Js2IL.Services.VariableBindings.VariableInfo>();
                    if (fields.Any())
                    {
                        var functionScopeTypeHandle = registry.GetScopeTypeHandle(functionName);
                        if (!functionScopeTypeHandle.IsNil)
                        {
                            // Build constructor member reference (parameterless instance .ctor)
                            var ctorSigBuilder = new BlobBuilder();
                            new BlobEncoder(ctorSigBuilder)
                                .MethodSignature(isInstanceMethod: true)
                                .Parameters(0, rt => rt.Void(), p => { });
                            var ctorRef = _metadataBuilder.AddMemberReference(
                                functionScopeTypeHandle,
                                _metadataBuilder.GetOrAddString(".ctor"),
                                _metadataBuilder.GetOrAddBlob(ctorSigBuilder));

                            // newobj scope
                            il.OpCode(ILOpCode.Newobj);
                            il.Token(ctorRef);
                            // store into a new local slot associated with this function scope name
                            var scopeLocal = variables.CreateScopeInstance(functionName);
                            il.StoreLocal(scopeLocal.Address);

                            // Initialize parameter fields on the scope from CLR arguments
                            // JS parameters start at arg1 (arg0 is scopes[])
                            ushort jsParamSeq = 1;
                            foreach (var param in functionDeclaration.Params.OfType<Acornima.Ast.Identifier>())
                            {
                                // Load scope instance (target for stfld)
                                il.LoadLocal(scopeLocal.Address);
                                // Load CLR arg for this parameter (object already)
                                il.LoadArgument(jsParamSeq);
                                // Store to the corresponding field on the scope
                                var fieldHandle = registry.GetFieldHandle(functionName, param.Name);
                                il.OpCode(ILOpCode.Stfld);
                                il.Token(fieldHandle);
                                jsParamSeq++;
                            }
                        }
                    }
                }
                catch
                {
                    // If scope type not found we silently continue (no locals used)
                }
            }

            // Emit body statements
            var hasExplicitReturn = blockStatement.Body.Any(s => s is ReturnStatement);
            
            // Initialize nested function variables before generating other statements
            if (functionScope != null)
            {
                // Get nested functions from the function's child scopes
                var nestedFunctions = functionScope.Children
                    .Where(scope => scope.Kind == ScopeKind.Function && scope.AstNode is FunctionDeclaration)
                    .Select(scope => (FunctionDeclaration)scope.AstNode!)
                    .ToList();
                methodGenerator.InitializeLocalFunctionVariables(nestedFunctions);
            }
            
            methodGenerator.GenerateStatements(blockStatement.Body);
            if (!hasExplicitReturn)
            {
                // Implicit return undefined => null
                il.OpCode(ILOpCode.Ldnull);
                il.OpCode(ILOpCode.Ret);
            }

            // Add method body including any scope locals we created (function scope instance)
            StandaloneSignatureHandle localSignature = default;
            MethodBodyAttributes bodyAttributes = MethodBodyAttributes.None;
            var localCount = variables.GetNumberOfLocals();
            if (localCount > 0)
            {
                var localSig = new BlobBuilder();
                var localEncoder = new BlobEncoder(localSig).LocalVariableSignature(localCount);
                for (int i = 0; i < localCount; i++)
                {
                    localEncoder.AddVariable().Type().Object();
                }
                localSignature = _metadataBuilder.AddStandaloneSignature(_metadataBuilder.GetOrAddBlob(localSig));
                bodyAttributes = MethodBodyAttributes.InitLocals;
            }

            var bodyoffset = _methodBodyStreamEncoder.AddMethodBody(
                il,
                localVariablesSignature: localSignature,
                attributes: bodyAttributes);
            // Build method signature: static object (object[] scopes, object param1, ...)
            var sigBuilder = new BlobBuilder();
            var paramCount = 1 + functionDeclaration.Params.Count; // scope array + declared params
            new BlobEncoder(sigBuilder)
                .MethodSignature()
                .Parameters(paramCount, returnType => returnType.Type().Object(), parameters =>
                {
                    // scope array parameter
                    parameters.AddParameter().Type().SZArray().Object();
                    // each JS parameter as System.Object for now
                    foreach (var p in functionDeclaration.Params)
                    {
                        parameters.AddParameter().Type().Object();
                    }
                });
            var methodSig = _metadataBuilder.GetOrAddBlob(sigBuilder);

            // Add parameters with names
            var scopeParamName = "scopes";
            ParameterHandle firstParamHandle = _metadataBuilder.AddParameter(
                ParameterAttributes.None,
                _metadataBuilder.GetOrAddString(scopeParamName),
                sequenceNumber: 1);
            // subsequent params
            ushort seq = 2;
            foreach (var p in functionDeclaration.Params)
            {
                if (p is Acornima.Ast.Identifier pid)
                {
                    _metadataBuilder.AddParameter(ParameterAttributes.None, _metadataBuilder.GetOrAddString(pid.Name), sequenceNumber: seq++);
                }
                else
                {
                    _metadataBuilder.AddParameter(ParameterAttributes.None, _metadataBuilder.GetOrAddString($"param{seq-1}"), sequenceNumber: seq++);
                }
            }

            if (typeBuilder is null)
            {
                throw new InvalidOperationException("TypeBuilder is required for method emission.");
            }

            var methodDefinition = typeBuilder.AddMethodDefinition(
                MethodAttributes.Static | MethodAttributes.Public,
                functionName,
                methodSig,
                bodyoffset,
                parameterList: firstParamHandle);

            // Register with dispatch table
            _dispatchTableGenerator.SetMethodDefinitionHandle(functionName, methodDefinition);

            return methodDefinition;
        }
    }
}
