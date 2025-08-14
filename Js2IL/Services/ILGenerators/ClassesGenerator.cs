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
        private readonly ClassRegistry _classRegistry;

        public ClassesGenerator(MetadataBuilder metadata, BaseClassLibraryReferences bcl, MethodBodyStreamEncoder methodBodies, ClassRegistry classRegistry)
        {
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _bcl = bcl ?? throw new ArgumentNullException(nameof(bcl));
            _methodBodies = methodBodies;
            _classRegistry = classRegistry ?? throw new ArgumentNullException(nameof(classRegistry));
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

            // Use System.Object as base type for now (ExpandoObject is sealed and cannot be inherited)
            var typeAttrs = parentType.IsNil
                ? TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit
                : TypeAttributes.NestedPublic | TypeAttributes.Class | TypeAttributes.BeforeFieldInit;
            var typeHandle = tb.AddTypeDefinition(typeAttrs, _bcl.ObjectType);

            if (!parentType.IsNil)
            {
                _metadata.AddNestedType(typeHandle, parentType);
            }

            // Register the class type for later lookup
            _classRegistry.Register(name, typeHandle);

            // Emit a parameterless .ctor that calls ExpandoObject::.ctor
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
                // Minimal body: support console.log("...") inside method; else return null
                var il = new InstructionEncoder(new BlobBuilder());
                if (element.Value is FunctionExpression fexpr && fexpr.Body is BlockStatement bstmt)
                {
                    // Look for a single statement: console.log("...");
                    if (bstmt.Body.Count == 1 && bstmt.Body[0] is ExpressionStatement estmt && estmt.Expression is CallExpression call &&
                        call.Callee is MemberExpression mex && mex.Object is Identifier oid && oid.Name == "console" &&
                        mex.Property is Identifier pid && pid.Name == "log" && call.Arguments.Count == 1 && call.Arguments[0] is StringLiteral sarg)
                    {
                        // Build object[] with 1 element (string literal)
                        il.LoadConstantI4(1);
                        il.OpCode(ILOpCode.Newarr);
                        il.Token(_bcl.ObjectType);
                        il.OpCode(ILOpCode.Dup);
                        il.LoadConstantI4(0);
                        il.LoadString(_metadata.GetOrAddUserString(sarg.Value));
                        il.OpCode(ILOpCode.Stelem_ref);
                        // Call runtime Console.Log
                        var rt = new Runtime(_metadata, il);
                        rt.InvokeConsoleLog();
                        // Return null (undefined)
                        il.OpCode(ILOpCode.Ldnull);
                        il.OpCode(ILOpCode.Ret);
                    }
                    else
                    {
                        il.OpCode(ILOpCode.Ldnull);
                        il.OpCode(ILOpCode.Ret);
                    }
                }
                else
                {
                    il.OpCode(ILOpCode.Ldnull);
                    il.OpCode(ILOpCode.Ret);
                }
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
