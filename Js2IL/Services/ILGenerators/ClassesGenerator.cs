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
        private readonly Variables _variables;
        private readonly Dispatch.DispatchTableGenerator _dispatchTableGenerator;

        public ClassesGenerator(MetadataBuilder metadata, BaseClassLibraryReferences bcl, MethodBodyStreamEncoder methodBodies, ClassRegistry classRegistry, Variables variables, Dispatch.DispatchTableGenerator dispatchTableGenerator)
        {
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _bcl = bcl ?? throw new ArgumentNullException(nameof(bcl));
            _methodBodies = methodBodies;
            _classRegistry = classRegistry ?? throw new ArgumentNullException(nameof(classRegistry));
            _variables = variables ?? throw new ArgumentNullException(nameof(variables));
            _dispatchTableGenerator = dispatchTableGenerator ?? throw new ArgumentNullException(nameof(dispatchTableGenerator));
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

            // Body - use ILMethodGenerator for consistent expression emission
            var ilGen = new ILMethodGenerator(_variables, _bcl, _metadata, _methodBodies, _dispatchTableGenerator, _classRegistry);
            ilGen.IL.OpCode(ILOpCode.Ldarg_0);
            ilGen.IL.Call(_bcl.Object_Ctor_Ref);

            // Initialize fields with default values if provided using ILMethodGenerator.Emit
            EmitFieldInitializers(ilGen, fieldsWithInits);

            ilGen.IL.OpCode(ILOpCode.Ret);

            var ctorBody = _methodBodies.AddMethodBody(ilGen.IL);
            return tb.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                ".ctor",
                ctorSig,
                ctorBody);
        }

        private void EmitFieldInitializers(ILMethodGenerator ilGen, System.Collections.Generic.List<(FieldDefinitionHandle Field, Expression? Init)> fieldsWithInits)
        {
            foreach (var (field, initExpr) in fieldsWithInits)
            {
                ilGen.IL.OpCode(ILOpCode.Ldarg_0);
                if (initExpr is null)
                {
                        // no initializer -> leave default null; skip write
                        // TODO: Refactor this logic for better clarity and maintainability
                    ilGen.IL.OpCode(ILOpCode.Pop);
                }
                else
                {
                    // Use ILMethodGenerator to emit the initializer expression, then box numbers if needed
                    ((IMethodExpressionEmitter)ilGen).Emit(initExpr, new TypeCoercion() { boxResult = true });
                    ilGen.IL.OpCode(ILOpCode.Stfld);
                    ilGen.IL.Token(field);
                }
            }
        }

        private MethodDefinitionHandle EmitMethod(TypeBuilder tb, Acornima.Ast.MethodDefinition element)
        {
            var mname = (element.Key as Identifier)?.Name ?? "method";
            var msig = BuildMethodSignature(element.Value as FunctionExpression, isStatic: element.Static);

            // Use ILMethodGenerator for body emission to reuse existing statement/expression logic
            // Build method variables context (no JS parameters yet for class methods)
            var paramNames = element.Value is FunctionExpression fe
                ? fe.Params.OfType<Identifier>().Select(p => p.Name)
                : Enumerable.Empty<string>();
            var methodVariables = new Variables(_variables, mname, paramNames, isNestedFunction: false);
            var ilGen = new ILMethodGenerator(methodVariables, _bcl, _metadata, _methodBodies, _dispatchTableGenerator, _classRegistry);

            bool hasExplicitReturn = false;
            if (element.Value is FunctionExpression fexpr && fexpr.Body is BlockStatement bstmt)
            {
                hasExplicitReturn = bstmt.Body.Any(s => s is ReturnStatement);
                ilGen.GenerateStatements(bstmt.Body);
            }
            else
            {
                // No body or unsupported shape: default to returning undefined (null)
            }

            if (!hasExplicitReturn)
            {
                ilGen.IL.OpCode(ILOpCode.Ldnull);
                ilGen.IL.OpCode(ILOpCode.Ret);
            }

            // Include locals created by ILMethodGenerator (e.g., scopes)
            StandaloneSignatureHandle localSignature = default;
            MethodBodyAttributes bodyAttributes = MethodBodyAttributes.None;
            var localCount = methodVariables.GetNumberOfLocals();
            if (localCount > 0)
            {
                var localSig = new BlobBuilder();
                var localEncoder = new BlobEncoder(localSig).LocalVariableSignature(localCount);
                for (int i = 0; i < localCount; i++)
                {
                    localEncoder.AddVariable().Type().Object();
                }
                localSignature = _metadata.AddStandaloneSignature(_metadata.GetOrAddBlob(localSig));
                bodyAttributes = MethodBodyAttributes.InitLocals;
            }

            var mbody = _methodBodies.AddMethodBody(ilGen.IL, localVariablesSignature: localSignature, attributes: bodyAttributes);
            var attrs = MethodAttributes.Public | MethodAttributes.HideBySig;
            if (element.Static)
            {
                attrs |= MethodAttributes.Static;
            }
            var methodDef = tb.AddMethodDefinition(attrs, mname, msig, mbody);

            // Note: Static methods will be invoked via MemberReference built at call site. Registration not required here.

            return methodDef;
        }

        private BlobHandle BuildMethodSignature(FunctionExpression? f, bool isStatic)
        {
            var paramCount = f != null ? f.Params.Count : 0;
            var sig = new BlobBuilder();
            new BlobEncoder(sig)
                .MethodSignature(isInstanceMethod: !isStatic)
                .Parameters(paramCount, r => r.Type().Object(), p => {
                    for (int i = 0; i < paramCount; i++) p.AddParameter().Type().Object();
                });
            return _metadata.GetOrAddBlob(sig);
        }
    }
}
