using Acornima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Js2IL.Services.ILGenerators
{
    internal class FunctionGenerator
    {
        private Variables _variables;
        private BaseClassLibraryReferences _bclReferences;
        private MetadataBuilder _metadataBuilder;
        private MethodBodyStreamEncoder _methodBodyStreamEncoder;
        private Dispatch.DispatchTableGenerator _dispatchTableGenerator;
        private MethodDefinitionHandle _firstMethod = default;

        public MethodDefinitionHandle FirstMethod => _firstMethod;

        public FunctionGenerator(Variables variables, BaseClassLibraryReferences bclReferences, MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, Dispatch.DispatchTableGenerator dispatchTableGenerator)
        {
            _variables = variables;
            _bclReferences = bclReferences;
            _metadataBuilder = metadataBuilder;
            _methodBodyStreamEncoder = methodBodyStreamEncoder;
            _dispatchTableGenerator = dispatchTableGenerator;
        }

        public void DeclareFunctions(IEnumerable<FunctionDeclaration> functionDeclarations)
        {
            // Iterate through each function declaration in the block
            foreach (var functionDeclaration in functionDeclarations)
            {
                DeclareFunction(functionDeclaration);
            }
        }

        public void DeclareFunction(FunctionDeclaration functionDeclaration)
        {
            var functionVariables = new Variables(_variables);
            var methodGenerator = new ILMethodGenerator(functionVariables, _bclReferences, _metadataBuilder, _methodBodyStreamEncoder, _dispatchTableGenerator);

            var methodDefinition = methodGenerator.GenerateMethodForFunction(functionDeclaration);
            if (this._firstMethod.IsNil)
            {
                this._firstMethod = methodDefinition;
            }
        }
    }
}
