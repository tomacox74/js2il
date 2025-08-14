using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using Acornima.Ast;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services.ILGenerators
{
    internal sealed class ClassesGenerator
    {
        private readonly MetadataBuilder _metadata;
        private readonly BaseClassLibraryReferences _bcl;
        private readonly MethodBodyStreamEncoder _methodBodies;

        public ClassesGenerator(MetadataBuilder metadata, BaseClassLibraryReferences bcl, MethodBodyStreamEncoder methodBodies)
        {
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _bcl = bcl ?? throw new ArgumentNullException(nameof(bcl));
            _methodBodies = methodBodies;
        }

        public void DeclareClasses(SymbolTable table)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));
            EmitClassesRecursive(table.Root);
        }

        private void EmitClassesRecursive(Scope scope)
        {
            foreach (var child in scope.Children)
            {
                if (child.Kind == ScopeKind.Class && child.AstNode is ClassDeclaration cdecl)
                {
                    EmitClass(child, cdecl, parentType: default);
                }
                // Recurse to find nested classes
                EmitClassesRecursive(child);
            }
        }

        private TypeDefinitionHandle EmitClass(Scope classScope, ClassDeclaration cdecl, TypeDefinitionHandle parentType)
        {
            var name = classScope.Name;
            var tb = new TypeBuilder(_metadata, "Classes", name);

            // For now inherit from System.Object
            var typeAttrs = parentType.IsNil
                ? TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit
                : TypeAttributes.NestedPublic | TypeAttributes.Class | TypeAttributes.BeforeFieldInit;
            var typeHandle = tb.AddTypeDefinition(typeAttrs, _bcl.ObjectType);

            if (!parentType.IsNil)
            {
                _metadata.AddNestedType(typeHandle, parentType);
            }

            // Emit a parameterless .ctor that calls System.Object::.ctor
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(0, r => r.Void(), p => { });
            var ctorSig = _metadata.GetOrAddBlob(sigBuilder);

            var encoder = new InstructionEncoder(new BlobBuilder());
            encoder.OpCode(ILOpCode.Ldarg_0);
            encoder.Call(_bcl.Object_Ctor_Ref);
            encoder.OpCode(ILOpCode.Ret);

            var methodBody = _methodBodies.AddMethodBody(encoder);
            tb.AddMethodDefinition(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                ".ctor", ctorSig, methodBody);

            // Methods: create stubs for now; real method codegen will come later
            foreach (var element in cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>())
            {
                var mname = (element.Key as Identifier)?.Name ?? "method";
                var msig = BuildMethodSignature(element.Value as FunctionExpression);
                // Empty body that returns null (object)
                var il = new InstructionEncoder(new BlobBuilder());
                il.OpCode(ILOpCode.Ldnull);
                il.OpCode(ILOpCode.Ret);
                var mbody = _methodBodies.AddMethodBody(il);
                tb.AddMethodDefinition(MethodAttributes.Public | MethodAttributes.HideBySig, mname, msig, mbody);
            }

            return typeHandle;
        }

        private BlobHandle BuildMethodSignature(FunctionExpression? f)
        {
            var paramCount = f != null ? f.Params.Count : 0;
            var sig = new BlobBuilder();
            new BlobEncoder(sig)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(paramCount, r => r.Type().Object(), p => {
                    for (int i = 0; i < paramCount; i++) p.AddParameter().Type().Object();
                });
            return _metadata.GetOrAddBlob(sig);
        }
    }
}
