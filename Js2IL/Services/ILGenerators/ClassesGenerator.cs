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
            // Resolve authoritative .NET names from symbol table; fall back if absent
            var ns = classScope.DotNetNamespace ?? "Classes";
            var name = classScope.DotNetTypeName ?? classScope.Name;
            var tb = new TypeBuilder(_metadata, ns, name);

            // Determine attributes for when we add the type at the end
            var typeAttrs = parentType.IsNil
                ? TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit
                : TypeAttributes.NestedPublic | TypeAttributes.Class | TypeAttributes.BeforeFieldInit;

            // Handle class fields with default initializers (ECMAScript class field syntax)
            // Example: class C { foo = 42; }
            // We emit them as instance fields of type object and initialize in .ctor.
            var fieldsWithInits = new System.Collections.Generic.List<(FieldDefinitionHandle Field, Expression? Init)>();
            foreach (var element in cdecl.Body.Body)
            {
                if (element is Acornima.Ast.PropertyDefinition pdef && pdef.Key is Identifier pid)
                {
                    // Create field signature: object
                    var fSig = new BlobBuilder();
                    new BlobEncoder(fSig).Field().Type().Object();
                    var fSigHandle = _metadata.GetOrAddBlob(fSig);
                    var fh = tb.AddFieldDefinition(FieldAttributes.Public, pid.Name, fSigHandle);
                    _classRegistry.RegisterField(classScope.Name, pid.Name, fh);
                    fieldsWithInits.Add((fh, pdef.Value as Expression));
                }
            }

            // Emit a parameterless .ctor that calls System.Object::.ctor and initializes fields
            var ctorHandle = EmitParameterlessConstructor(tb, fieldsWithInits);

            // Methods: create stubs for now; real method codegen will come later
            foreach (var element in cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>())
            {
                EmitMethod(tb, element);
            }

            // Finally, create the type definition (after fields and methods were added)
            var typeHandle = tb.AddTypeDefinition(typeAttrs, _bcl.ObjectType);
            if (!parentType.IsNil)
            {
                _metadata.AddNestedType(typeHandle, parentType);
            }
            // Register the class type for later lookup using the JS-visible identifier (scope name)
            _classRegistry.Register(classScope.Name, typeHandle);

            return typeHandle;
        }

        private MethodDefinitionHandle EmitParameterlessConstructor(
            TypeBuilder tb,
            System.Collections.Generic.List<(FieldDefinitionHandle Field, Expression? Init)> fieldsWithInits)
        {
            // Signature: instance void .ctor()
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(0, r => r.Void(), p => { });
            var ctorSig = _metadata.GetOrAddBlob(sigBuilder);

            // Body
            var ilbbCtor = new BlobBuilder();
            var ilCtor = new InstructionEncoder(ilbbCtor);
            ilCtor.OpCode(ILOpCode.Ldarg_0);
            ilCtor.Call(_bcl.Object_Ctor_Ref);

            // Initialize fields with default values if provided
            foreach (var (field, initExpr) in fieldsWithInits)
            {
                ilCtor.OpCode(ILOpCode.Ldarg_0);
                if (initExpr is StringLiteral s)
                {
                    ilCtor.LoadString(_metadata.GetOrAddUserString(s.Value));
                }
                else if (initExpr is NumericLiteral n)
                {
                    ilCtor.LoadConstantR8(n.Value);
                    ilCtor.OpCode(ILOpCode.Box);
                    ilCtor.Token(_bcl.DoubleType);
                }
                else if (initExpr is null)
                {
                    // no initializer → leave default null; skip write
                    ilCtor.OpCode(ILOpCode.Pop); // keep behavior consistent with current implementation
                }
                else
                {
                    ilCtor.OpCode(ILOpCode.Ldnull);
                }

                // If we emitted a value, store it
                if (initExpr is not null)
                {
                    ilCtor.OpCode(ILOpCode.Stfld);
                    ilCtor.Token(field);
                }
            }

            ilCtor.OpCode(ILOpCode.Ret);

            var ctorBody = _methodBodies.AddMethodBody(ilCtor);
            return tb.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                ".ctor",
                ctorSig,
                ctorBody);
        }

        private MethodDefinitionHandle EmitMethod(TypeBuilder tb, Acornima.Ast.MethodDefinition element)
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
            return tb.AddMethodDefinition(MethodAttributes.Public | MethodAttributes.HideBySig, mname, msig, mbody);
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
