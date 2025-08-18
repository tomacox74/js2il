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
            // Example: class C { foo = 42; static bar = 1; }
            // We emit instance fields as object and initialize in .ctor.
            // Static fields are emitted as static object fields and initialized in a type initializer (.cctor).
            var fieldsWithInits = new System.Collections.Generic.List<(FieldDefinitionHandle Field, Expression? Init)>();
            var staticFieldsWithInits = new System.Collections.Generic.List<(FieldDefinitionHandle Field, Expression? Init)>();
            var declaredFieldNames = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            foreach (var element in cdecl.Body.Body)
            {
                if (element is Acornima.Ast.PropertyDefinition pdef)
                {
                    // Create field signature: object
                    var fSig = new BlobBuilder();
                    new BlobEncoder(fSig).Field().Type().Object();
                    var fSigHandle = _metadata.GetOrAddBlob(fSig);

                    // Private field (#name)
                    if (pdef.Key is Acornima.Ast.PrivateIdentifier priv)
                    {
                        var pname = priv.Name; // JS-visible name without '#'
                        var emittedName = ManglePrivateFieldName(pname);
                        if (pdef.Static)
                        {
                            var fh = tb.AddFieldDefinition(FieldAttributes.Private | FieldAttributes.Static, emittedName, fSigHandle);
                            // Track static private separately if needed later; for now reuse RegisterStaticField
                            _classRegistry.RegisterStaticField(classScope.Name, pname, fh);
                            staticFieldsWithInits.Add((fh, pdef.Value as Expression));
                        }
                        else
                        {
                            var fh = tb.AddFieldDefinition(FieldAttributes.Private, emittedName, fSigHandle);
                            _classRegistry.RegisterPrivateField(classScope.Name, pname, fh);
                            fieldsWithInits.Add((fh, pdef.Value as Expression));
                        }
                        declaredFieldNames.Add(pname);
                    }
                    // Public field (identifier)
                    else if (pdef.Key is Identifier pid)
                    {
                        if (pdef.Static)
                        {
                            var fh = tb.AddFieldDefinition(FieldAttributes.Public | FieldAttributes.Static, pid.Name, fSigHandle);
                            _classRegistry.RegisterStaticField(classScope.Name, pid.Name, fh);
                            staticFieldsWithInits.Add((fh, pdef.Value as Expression));
                        }
                        else
                        {
                            var fh = tb.AddFieldDefinition(FieldAttributes.Public, pid.Name, fSigHandle);
                            _classRegistry.RegisterField(classScope.Name, pid.Name, fh);
                            fieldsWithInits.Add((fh, pdef.Value as Expression));
                        }
                        declaredFieldNames.Add(pid.Name);
                    }
                }
            }

            // Pre-scan methods (including constructor) for assignments to this.<prop> and declare fields for them
            System.Collections.Generic.IEnumerable<string> FindThisAssignedProps(Acornima.Ast.Node node)
            {
                if (node is null) yield break;
                switch (node)
                {
                    case Acornima.Ast.AssignmentExpression a when a.Left is Acornima.Ast.MemberExpression me && me.Object is Acornima.Ast.ThisExpression && !me.Computed && me.Property is Identifier pid:
                        yield return pid.Name;
                        break;
                    case Acornima.Ast.BlockStatement b:
                        foreach (var s in b.Body)
                            foreach (var n in FindThisAssignedProps(s)) yield return n;
                        break;
                    case Acornima.Ast.ExpressionStatement es:
                        foreach (var n in FindThisAssignedProps(es.Expression)) yield return n;
                        break;
                    case Acornima.Ast.IfStatement ifs:
                        foreach (var n in FindThisAssignedProps(ifs.Consequent)) yield return n;
                        if (ifs.Alternate != null) foreach (var n in FindThisAssignedProps(ifs.Alternate)) yield return n;
                        break;
                    case Acornima.Ast.ForStatement fs:
                        if (fs.Init is Acornima.Ast.Node init) foreach (var n in FindThisAssignedProps(init)) yield return n;
                        if (fs.Test is Acornima.Ast.Node test) foreach (var n in FindThisAssignedProps(test)) yield return n;
                        if (fs.Update is Acornima.Ast.Node upd) foreach (var n in FindThisAssignedProps(upd)) yield return n;
                        foreach (var n in FindThisAssignedProps(fs.Body)) yield return n;
                        break;
                    case Acornima.Ast.CallExpression ce:
                        foreach (var arg in ce.Arguments)
                            foreach (var n in FindThisAssignedProps(arg)) yield return n;
                        break;
                    case Acornima.Ast.MemberExpression mem:
                        if (mem.Object is Acornima.Ast.Node on) foreach (var n in FindThisAssignedProps(on)) yield return n;
                        if (mem.Property is Acornima.Ast.Node pn) foreach (var n in FindThisAssignedProps(pn)) yield return n;
                        break;
                    case Acornima.Ast.AssignmentExpression a2:
                        if (a2.Right is Acornima.Ast.Node rn) foreach (var n in FindThisAssignedProps(rn)) yield return n;
                        break;
                }
            }

            foreach (var m in cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>())
            {
                if (m.Value is FunctionExpression fe && fe.Body is BlockStatement body)
                {
                    foreach (var prop in FindThisAssignedProps(body).Distinct(StringComparer.Ordinal))
                    {
                        if (!declaredFieldNames.Contains(prop))
                        {
                            var fSig = new BlobBuilder();
                            new BlobEncoder(fSig).Field().Type().Object();
                            var fSigHandle = _metadata.GetOrAddBlob(fSig);
                            var fh = tb.AddFieldDefinition(FieldAttributes.Public, prop, fSigHandle);
                            _classRegistry.RegisterField(classScope.Name, prop, fh);
                            declaredFieldNames.Add(prop);
                        }
                    }
                }
            }

            // Detect explicit constructor method (name 'constructor')
            var ctorMethod = cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>()
                .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");
            if (ctorMethod != null && ctorMethod.Value is FunctionExpression ctorFunc)
            {
                EmitExplicitConstructor(tb, ctorFunc, fieldsWithInits, classScope.Name);
            }
            else
            {
                // Emit a parameterless .ctor that calls System.Object::.ctor and initializes fields
                _ = EmitParameterlessConstructor(tb, fieldsWithInits, classScope.Name);
            }

            // Methods: create stubs for now; real method codegen will come later
            foreach (var element in cdecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>())
            {
                var mname = (element.Key as Identifier)?.Name;
                if (string.Equals(mname, "constructor", StringComparison.Ordinal))
                {
                    // already emitted as .ctor above
                    continue;
                }
                EmitMethod(tb, element, classScope.Name);
            }

            // Finally, create the type definition (after fields and methods were added)
            var typeHandle = tb.AddTypeDefinition(typeAttrs, _bcl.ObjectType);
            if (!parentType.IsNil)
            {
                _metadata.AddNestedType(typeHandle, parentType);
            }
            // Register the class type for later lookup using the JS-visible identifier (scope name)
            _classRegistry.Register(classScope.Name, typeHandle);

            // If there are static field initializers, synthesize a type initializer (.cctor) to assign them.
            if (staticFieldsWithInits.Count > 0)
            {
                // Signature: static void .cctor()
                var sigBuilder = new BlobBuilder();
                new BlobEncoder(sigBuilder)
                    .MethodSignature(isInstanceMethod: false)
                    .Parameters(0, r => r.Void(), p => { });
                var cctorSig = _metadata.GetOrAddBlob(sigBuilder);

                var ilGen = new ILMethodGenerator(_variables, _bcl, _metadata, _methodBodies, _dispatchTableGenerator, _classRegistry, inClassMethod: false, currentClassName: classScope.Name);

                // For each static field with an initializer: evaluate and stsfld
                foreach (var (field, initExpr) in staticFieldsWithInits)
                {
                    if (initExpr is null)
                    {
                        // default null; no store needed
                        continue;
                    }
                    ((IMethodExpressionEmitter)ilGen).Emit(initExpr, new TypeCoercion() { boxResult = true });
                    ilGen.IL.OpCode(ILOpCode.Stsfld);
                    ilGen.IL.Token(field);
                }

                ilGen.IL.OpCode(ILOpCode.Ret);

                var cctorBody = _methodBodies.AddMethodBody(ilGen.IL);
                tb.AddMethodDefinition(
                    MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    ".cctor",
                    cctorSig,
                    cctorBody);
            }

            return typeHandle;
        }

        private static string ManglePrivateFieldName(string name)
        {
            // Ensure private fields don't collide with public fields/methods and are clearly internal
            return "__js2il_priv_" + name;
        }

        private MethodDefinitionHandle EmitParameterlessConstructor(
            TypeBuilder tb,
            System.Collections.Generic.List<(FieldDefinitionHandle Field, Expression? Init)> fieldsWithInits,
            string className)
        {
            // Signature: instance void .ctor()
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(0, r => r.Void(), p => { });
            var ctorSig = _metadata.GetOrAddBlob(sigBuilder);

            // Body - use ILMethodGenerator for consistent expression emission
            var ilGen = new ILMethodGenerator(_variables, _bcl, _metadata, _methodBodies, _dispatchTableGenerator, _classRegistry, inClassMethod: true, currentClassName: className);
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

        private MethodDefinitionHandle EmitExplicitConstructor(
            TypeBuilder tb,
            FunctionExpression ctorFunc,
            System.Collections.Generic.List<(FieldDefinitionHandle Field, Expression? Init)> fieldsWithInits,
            string className)
        {
            // Signature: instance void .ctor(object, ...)
            var paramCount = ctorFunc.Params.Count;
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature(isInstanceMethod: true)
                .Parameters(paramCount, r => r.Void(), p => { for (int i = 0; i < paramCount; i++) p.AddParameter().Type().Object(); });
            var ctorSig = _metadata.GetOrAddBlob(sigBuilder);

            // Create a generator with parameter variables so identifiers resolve
            var paramNames = ctorFunc.Params.OfType<Identifier>().Select(p => p.Name);
            var methodVariables = new Variables(_variables, "constructor", paramNames, isNestedFunction: false);
            var ilGen = new ILMethodGenerator(methodVariables, _bcl, _metadata, _methodBodies, _dispatchTableGenerator, _classRegistry, inClassMethod: true, currentClassName: className);

            // base .ctor
            ilGen.IL.OpCode(ILOpCode.Ldarg_0);
            ilGen.IL.Call(_bcl.Object_Ctor_Ref);

            // Initialize fields with default values
            EmitFieldInitializers(ilGen, fieldsWithInits);

            // Emit constructor body statements (no default return value emission)
            if (ctorFunc.Body is BlockStatement bstmt)
            {
                ilGen.GenerateStatementsForBody(bstmt.Body);
            }

            // Return from constructor (void)
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

    private MethodDefinitionHandle EmitMethod(TypeBuilder tb, Acornima.Ast.MethodDefinition element, string className)
        {
            var mname = (element.Key as Identifier)?.Name ?? "method";
            var msig = BuildMethodSignature(element.Value as FunctionExpression, isStatic: element.Static);

            // Use ILMethodGenerator for body emission to reuse existing statement/expression logic
            // Build method variables context (no JS parameters yet for class methods)
            var paramNames = element.Value is FunctionExpression fe
                ? fe.Params.OfType<Identifier>().Select(p => p.Name)
                : Enumerable.Empty<string>();
            var methodVariables = new Variables(_variables, mname, paramNames, isNestedFunction: false);
            var ilGen = new ILMethodGenerator(methodVariables, _bcl, _metadata, _methodBodies, _dispatchTableGenerator, _classRegistry, inClassMethod: !element.Static, currentClassName: className);

            bool hasExplicitReturn = false;
            if (element.Value is FunctionExpression fexpr && fexpr.Body is BlockStatement bstmt)
            {
                hasExplicitReturn = bstmt.Body.Any(s => s is ReturnStatement);
                ilGen.GenerateStatementsForBody(bstmt.Body);
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
