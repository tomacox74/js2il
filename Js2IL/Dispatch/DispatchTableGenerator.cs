using Acornima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using Js2IL.Services;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Dispatch
{
    /// <summary>
    /// Generates a DispatchTable class for top-level function declarations.
    /// </summary>
    internal class DispatchTableGenerator
    {
        private MetadataBuilder _metadataBuilder;
        private BaseClassLibraryReferences _bclReferences;

        private List<FunctionInfo> _functions = new List<FunctionInfo>();

        private MethodBodyStreamEncoder _methodBodyStreamEncoder;

        public static readonly string LoadDispatchTableMethod = "_LoadDispatchTable";

        public DispatchTableGenerator(MetadataBuilder metadataBuilder, BaseClassLibraryReferences bclReferences, MethodBodyStreamEncoder methodBodyStreamEncoder)
        {
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            _methodBodyStreamEncoder = methodBodyStreamEncoder;
        }

        public class FunctionInfo
        {
            public FunctionInfo(string name, FunctionDeclaration declaration)
            {
                Name = name;
                Declaration = declaration;
            }

            public string Name { get; init; }
            public FunctionDeclaration Declaration { get; init; }

            public MethodDefinitionHandle MethodDefinitionHandle { get; set; } = default;

            public FieldDefinitionHandle FieldDefinitionHandle { get; set; } = default;
        }


        public void GenerateDispatchTable(SymbolTable symbolTable)
        {
            GetAllFunctions(symbolTable);
            GenerateDispatchTableClass();
        }

        /// <summary>
        /// Gets all function declarations from the symbol table, including nested functions.
        /// </summary>
        public void GetAllFunctions(SymbolTable symbolTable)
        {
            foreach (var (functionScope, functionDeclaration) in symbolTable.GetAllFunctions())
            {
                if (functionDeclaration.Id is Identifier id)
                {
                    _functions.Add(new FunctionInfo(id.Name, functionDeclaration));
                }
            }
        }

        public FieldDefinitionHandle GetFieldDefinitionHandle(string methodName)
        {
            var function = _functions.FirstOrDefault(f => f.Name == methodName);
            if (function != null)
            {
                return function.FieldDefinitionHandle;
            }

            return default; // Return default if not found
        }

        public MethodDefinitionHandle GetMethodDefinitionHandle(string methodName)
        {
            var function = _functions.FirstOrDefault(f => f.Name == methodName);
            if (function != null)
            {
                return function.MethodDefinitionHandle;
            }

            return default; // Return default if not found
        }

        public FunctionDeclaration? GetFunctionDeclaration(string methodName)
        {
            var function = _functions.FirstOrDefault(f => f.Name == methodName);
            return function?.Declaration;
        }

        /// <summary>
        /// Generates the C# code for the DispatchTable class with Action fields for each function.
        /// </summary>
        public void GenerateDispatchTableClass()
        {
            // short circuit if no functions
            if (_functions.Count == 0)
            {
                return; // No functions to generate
            }

            // Use TypeBuilder to create DispatchTable type and its fields
            var tb = new TypeBuilder(_metadataBuilder, "", "DispatchTable");

            // Add a field for each function
            foreach (var function in _functions)
            {
                var paramCount = function.Declaration.Params.Count; // js params only
                var blobBuilder = new BlobBuilder();
                var fieldSigEncoder = new BlobEncoder(blobBuilder).FieldSignature();
                if (paramCount <= 6)
                {
                    int genericArity = paramCount + 2; // scopes + params + return
                    var genericTypeRef = genericArity switch
                    {
                        2 => _bclReferences.Func2Generic_TypeRef,
                        3 => _bclReferences.Func3Generic_TypeRef,
                        4 => _bclReferences.Func4Generic_TypeRef,
                        5 => _bclReferences.Func5Generic_TypeRef,
                        6 => _bclReferences.Func6Generic_TypeRef,
                        7 => _bclReferences.Func7Generic_TypeRef,
                        8 => _bclReferences.Func8Generic_TypeRef,
                        _ => throw new NotSupportedException($"Unsupported generic arity {genericArity}")
                    };
                    var genericInst = fieldSigEncoder.GenericInstantiation(genericTypeRef, genericArity, false);
                    genericInst.AddArgument().SZArray().Type(_bclReferences.ObjectType, false); // scopes array
                    for (int i = 0; i < paramCount; i++)
                    {
                        genericInst.AddArgument().Type(_bclReferences.ObjectType, false);
                    }
                    genericInst.AddArgument().Type(_bclReferences.ObjectType, false); // return object
                }
                else
                {
                    throw new NotSupportedException($"Only up to 6 parameters supported currently (got {paramCount})");
                }
                var fieldSignature = _metadataBuilder.GetOrAddBlob(blobBuilder);
                var fieldHandle = tb.AddFieldDefinition(
                    FieldAttributes.Public | FieldAttributes.Static,
                    function.Name,
                    fieldSignature);

                function.FieldDefinitionHandle = fieldHandle;
            }

            // create the DispatchTable class
            tb.AddTypeDefinition(
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.BeforeFieldInit,
                _bclReferences.ObjectType);
        }

        public FieldDefinitionHandle SetMethodDefinitionHandle(string methodName, MethodDefinitionHandle methodHandle)
        {
            // Find the function info by name
            var function = _functions.FirstOrDefault(f => f.Name == methodName);
            if (function != null)
            {
                // Set the method definition handle for the function
                function.MethodDefinitionHandle = methodHandle;
                return function.FieldDefinitionHandle;
            }

            // If the function is not found, return default
            return default;
        }

        public MethodDefinitionHandle GenerateLoadDispatchTableMethod()
        {
            if (_functions.Count == 0)
            {
                return default; // No functions to generate
            }

            // Create the method signature for the LoadDispatchTable method first.
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature()
                .Parameters(0, returnType => returnType.Void(), parameters => { });
            var methodSig = this._metadataBuilder.GetOrAddBlob(sigBuilder);

            var methodIl = new BlobBuilder();
            var il = new InstructionEncoder(methodIl);

            foreach (var function in _functions)
            {
                // 1. Load null for the instance (Action targets static method, so null)
                il.OpCode(ILOpCode.Ldnull);

                // 2. Load the function pointer for the static method
                il.OpCode(ILOpCode.Ldftn);
                il.Token(function.MethodDefinitionHandle); // The handle of the static method

                // 3. Create the delegate (Func<object[],object> or Func<object[],object,object>)
                il.OpCode(ILOpCode.Newobj);
                if (function.Declaration.Params.Count <= 6)
                {
                    var (_, ctorRef) = _bclReferences.GetFuncObjectArrayWithParams(function.Declaration.Params.Count);
                    il.Token(ctorRef);
                }
                else
                {
                    throw new NotSupportedException($"Only up to 6 parameters supported currently (got {function.Declaration.Params.Count})");
                }

                // 4. Store the delegate in the static field
                il.OpCode(ILOpCode.Stsfld);
                il.Token(function.FieldDefinitionHandle); // The static field to hold the Action
            }

            // Emit IL: return.
            il.OpCode(ILOpCode.Ret);

            var bodyOffset = _methodBodyStreamEncoder.AddMethodBody(
                il,
                localVariablesSignature: default,
                attributes: MethodBodyAttributes.None);

            // Use TypeBuilder to define the _Loader type and attach the loader method as first method
            var tbLoader = new TypeBuilder(_metadataBuilder, "Functions", "_Loader");

            var loaderMethod = tbLoader.AddMethodDefinition(
                MethodAttributes.Static | MethodAttributes.Public,
                LoadDispatchTableMethod,
                methodSig,
                bodyOffset
            );

            tbLoader.AddTypeDefinition(
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.BeforeFieldInit,
                _bclReferences.ObjectType);

            return loaderMethod;
        }
    }
}
