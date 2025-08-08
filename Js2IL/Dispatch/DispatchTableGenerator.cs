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


        public void GenerateDispatchTable(NodeList<Statement> statements)
        {
            GetTopLevelFunctions(statements);
            GenerateDispatchTableClass();
        }

        /// <summary>
        /// Enumerates top-level function declarations in the AST.
        /// </summary>
        public void GetTopLevelFunctions(NodeList<Statement> statements)
        {
            foreach (var stmt in statements)
            {
                if (stmt is FunctionDeclaration funcDecl && funcDecl.Id is Identifier id)
                {
                    _functions.Add(new FunctionInfo(id.Name, funcDecl));
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

            FieldDefinitionHandle firstField = default;

            // Add a field for each function
            foreach (var function in _functions)
            {
                var paramCount = function.Declaration.Params.Count; // js params only
                var blobBuilder = new BlobBuilder();
                var fieldSigEncoder = new BlobEncoder(blobBuilder).FieldSignature();
                if (paramCount == 0)
                {
                    // Func<object, object>
                    var genericInst = fieldSigEncoder.GenericInstantiation(
                        _bclReferences.Func2Generic_TypeRef,
                        2,
                        false);
                    genericInst.AddArgument().Type(_bclReferences.ObjectType, false); // scope param
                    genericInst.AddArgument().Type(_bclReferences.ObjectType, false); // return object
                }
                else if (paramCount == 1)
                {
                    // Func<object, object, object>
                    var genericInst = fieldSigEncoder.GenericInstantiation(
                        _bclReferences.Func3Generic_TypeRef,
                        3,
                        false);
                    genericInst.AddArgument().Type(_bclReferences.ObjectType, false); // scope param
                    genericInst.AddArgument().Type(_bclReferences.ObjectType, false); // js param1
                    genericInst.AddArgument().Type(_bclReferences.ObjectType, false); // return object
                }
                else
                {
                    throw new NotSupportedException("Only up to 1 parameter supported currently");
                }
                var fieldSignature = _metadataBuilder.GetOrAddBlob(blobBuilder);
                var field = _metadataBuilder.AddFieldDefinition(
                    FieldAttributes.Public | FieldAttributes.Static,
                    _metadataBuilder.GetOrAddString(function.Name),
                    fieldSignature);

                if (firstField.IsNil)
                {
                    firstField = field;
                }

                function.FieldDefinitionHandle = field;
            }

            // create the DispatchTable class
            MethodDefinitionHandle noMethods = MetadataTokens.MethodDefinitionHandle(_metadataBuilder.GetRowCount(TableIndex.MethodDef) + 1);
            _metadataBuilder.AddTypeDefinition(
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.BeforeFieldInit,
                _metadataBuilder.GetOrAddString(""),
                _metadataBuilder.GetOrAddString("DispatchTable"),
                _bclReferences.ObjectType,
                firstField,
                noMethods
            );
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

            // Create the method signature for the LoadDispatchTable method.
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

                // 3. Create the delegate (Func<object,object> or Func<object,object,object>)
                il.OpCode(ILOpCode.Newobj);
                if (function.Declaration.Params.Count == 0)
                {
                    il.Token(_bclReferences.FuncObjectObject_Ctor_Ref);
                }
                else if (function.Declaration.Params.Count == 1)
                {
                    il.Token(_bclReferences.FuncObjectObjectObject_Ctor_Ref);
                }
                else
                {
                    throw new NotSupportedException("Only up to 1 parameter supported currently");
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

            return _metadataBuilder.AddMethodDefinition(
                MethodAttributes.Static | MethodAttributes.Public,
                MethodImplAttributes.IL,
                _metadataBuilder.GetOrAddString(LoadDispatchTableMethod),
                methodSig,
                bodyOffset,
                parameterList: MetadataTokens.ParameterHandle(_metadataBuilder.GetRowCount(TableIndex.Param) + 1)
            );
        }
    }
}
