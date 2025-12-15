using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Acornima.Ast;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.Services.ILGenerators
{
    /// <summary>
    /// Dedicated expression emitter. Initially a thin wrapper to enable incremental refactoring
    /// out of ILMethodGenerator without changing call sites.
    /// </summary>
    internal sealed class ILExpressionGenerator : IMethodExpressionEmitter
    {
        private readonly ILMethodGenerator _owner;

        private Variables _variables => _owner.Variables;

        private InstructionEncoder _il => _owner.IL;

        private BinaryOperators _binaryOperators;

        /// <summary>
        /// Maps compound assignment operator names to their corresponding IL bitwise opcodes.
        /// Returns null if the operator is not a bitwise compound assignment.
        /// </summary>
        private static ILOpCode? GetCompoundBitwiseOpCode(string operatorName) => operatorName switch
        {
            "BitwiseOrAssignment" => ILOpCode.Or,
            "BitwiseAndAssignment" => ILOpCode.And,
            "BitwiseXorAssignment" => ILOpCode.Xor,
            "LeftShiftAssignment" => ILOpCode.Shl,
            "RightShiftAssignment" => ILOpCode.Shr,
            "UnsignedRightShiftAssignment" => ILOpCode.Shr_un,
            _ => null
        };

        /// <summary>
        /// Create an expression generator that delegates to another emitter.
        /// </summary>
        /// <param name="owner">Owning ILMethodGenerator providing shared state and helpers.</param>
        public ILExpressionGenerator(ILMethodGenerator owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            _binaryOperators = new BinaryOperators(owner.MetadataBuilder, _il, _variables, this, owner.BclReferences, owner.Runtime, owner);
        }

        // ---- Small helpers to reduce duplication ----
        private void EmitLoadScopeObject(ScopeObjectReference slot)
        {
            if (slot.Location == ObjectReferenceLocation.Local)
            {
                _il.LoadLocal(slot.Address);
            }
            else if (slot.Location == ObjectReferenceLocation.Parameter)
            {
                _il.LoadArgument(slot.Address);
            }
            else if (slot.Location == ObjectReferenceLocation.ScopeArray)
            {
                // In class instance methods, scopes are stored in this._scopes field, not in arg_0
                if (_owner.InClassMethod && !string.IsNullOrEmpty(_owner.CurrentClassName))
                {
                    // Load this._scopes field
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldarg_0); // load 'this'
                    if (_owner.ClassRegistry.TryGetPrivateField(_owner.CurrentClassName, "_scopes", out var scopesField))
                    {
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldfld);
                        _il.Token(scopesField);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Class '{_owner.CurrentClassName}' missing _scopes field");
                    }
                }
                else
                {
                    // Regular function: arg_0 is the scopes array
                    _il.LoadArgument(0);
                }
                _il.LoadConstantI4(slot.Address);
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldelem_ref);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported scope object reference location: {slot.Location}");
            }
        }

        // Load a scope object and cast it to its concrete scope type for verifiable ldfld/stfld
        // Cast is only needed when loading from parameters or scope arrays (typed as object)
        // Locals are now strongly-typed, so no cast is needed
        private void EmitLoadScopeObjectTyped(ScopeObjectReference slot, string scopeName)
        {
            EmitLoadScopeObject(slot);
            
            // Only cast if loading from parameter or scope array (not from local)
            if (slot.Location != ObjectReferenceLocation.Local)
            {
                var reg = _variables.GetVariableRegistry();
                var tdef = reg?.GetScopeTypeHandle(scopeName) ?? default;
                if (!tdef.IsNil)
                {
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Castclass);
                    _il.Token(tdef);
                }
            }
        }

        private void EmitLoadScopeObjectByName(string scopeName)
        {
            var slot = _variables.GetScopeLocalSlot(scopeName);
            EmitLoadScopeObjectTyped(slot, scopeName);
        }

        private void EmitBoxedArgsArray(IReadOnlyList<Expression> args)
        {
            _il.EmitNewArray(args.Count, _owner.BclReferences.ObjectType, (il, i) =>
            {
                // Emit each argument with boxResult=true so primitives are boxed exactly once
                _ = Emit(args[i], new TypeCoercion { boxResult = true });
            });
        }

        private void EmitBoxedArgsInline(IReadOnlyList<Expression> args)
        {
            for (int i = 0; i < args.Count; i++)
            {
                // Emit each argument boxed to an object once
                _ = Emit(args[i], new TypeCoercion { boxResult = true });
            }
        }

        private void EmitLoadVariableField(Variable v)
        {
            var slot = _variables.GetScopeLocalSlot(v.ScopeName);
            EmitLoadScopeObjectTyped(slot, v.ScopeName);
            _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldfld);
            _il.Token(v.FieldHandle);
        }

        private void EmitScopeArray(IReadOnlyList<string> scopeNames)
        {
            _il.EmitNewArray(scopeNames.Count, _owner.BclReferences.ObjectType, (il, i) =>
            {
                EmitLoadScopeObjectByName(scopeNames[i]);
            });
        }

        private void EmitBoxIfNeeded(JavascriptType type)
        {
            if (type == JavascriptType.Number)
            {
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Box);
                _il.Token(_owner.BclReferences.DoubleType);
            }
            else if (type == JavascriptType.Boolean)
            {
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Box);
                _il.Token(_owner.BclReferences.BooleanType);
            }
        }

        private void EmitUnboxBoolIfLoadedFromBoxedSource(Expression expr)
        {
            // Identifiers are loaded via LoadVariable, which now auto-unboxes when requested.
            // Only unbox for member/this sources that still yield a boxed boolean.
            if (expr is MemberExpression || expr is ThisExpression)
            {
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Unbox_any);
                _il.Token(_owner.BclReferences.BooleanType);
            }
        }

        /// <inheritdoc />
        public ExpressionResult Emit(Expression expression, TypeCoercion typeCoercion, CallSiteContext context = CallSiteContext.Expression,  ConditionalBranching? branching = null)
        {
            var _metadataBuilder = _owner.MetadataBuilder;
            var _bclReferences = _owner.BclReferences;
            var _runtime = _owner.Runtime;

            JavascriptType javascriptType = JavascriptType.Unknown;
            Type? clrType = null;

            switch (expression)
            {
                case ThisExpression:
                    if (_owner.InClassMethod)
                    {
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldarg_0);
                        javascriptType = JavascriptType.Object;
                    }
                    else
                    {
                        throw ILEmitHelpers.NotSupported("Unsupported 'this' expression outside of class context", expression);
                    }
                    break;
                case AssignmentExpression assignmentExpression:
                    javascriptType = EmitAssignment(assignmentExpression, typeCoercion);
                    break;
                case CallExpression callExpression:
                    // Delegate call emission to local helper (migrated from ILMethodGenerator)
                    clrType = GenerateCallExpression(callExpression, context);
                    // Infer JS type from known CLR return when available to allow boxing and conditional branching
                    if (clrType == typeof(double))
                    {
                        javascriptType = JavascriptType.Number;
                    }
                    else if (clrType == typeof(bool))
                    {
                        javascriptType = JavascriptType.Boolean;
                    }
                    else
                    {
                        javascriptType = JavascriptType.Object;
                    }
                    break;
                case ArrowFunctionExpression arrowFunction:
                    {
                        var idParams = ILMethodGenerator.ExtractParameterNames(arrowFunction.Params).ToList();
                        // Fallback: try to reflect a 'Name' property from param nodes when not Identifier (robust for different AST shapes)
                        if (idParams.Count == 0 && arrowFunction.Params.Count > 0)
                        {
                            foreach (var p in arrowFunction.Params)
                            {
                                if (p is Identifier iid) { idParams.Add(iid.Name); continue; }
                                var prop = p.GetType().GetProperty("Name");
                                if (prop != null)
                                {
                                    var val = prop.GetValue(p) as string;
                                    if (!string.IsNullOrEmpty(val)) { idParams.Add(val!); continue; }
                                }
                                idParams.Add($"p{idParams.Count}");
                            }
                        }
                        var paramNames = idParams.ToArray();
                        var registryScopeName = !string.IsNullOrEmpty(_owner.CurrentAssignmentTarget)
                            ? $"ArrowFunction_{_owner.CurrentAssignmentTarget}"
                            : $"ArrowFunction_L{arrowFunction.Location.Start.Line}C{arrowFunction.Location.Start.Column}";
                        var ilMethodName = $"ArrowFunction_L{arrowFunction.Location.Start.Line}C{arrowFunction.Location.Start.Column}";
                        var methodHandle = _owner.GenerateArrowFunctionMethod(arrowFunction, registryScopeName, ilMethodName, paramNames);

                        // Register arrow function with parameter count if it has a name (assigned to a variable)
                        if (!string.IsNullOrEmpty(_owner.CurrentAssignmentTarget))
                        {
                            _owner.RegisterFunction(_owner.CurrentAssignmentTarget, methodHandle, paramNames.Length);
                        }

                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldnull);
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldftn);
                        _il.Token(methodHandle);
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Newobj);
                        var ctorRef = _bclReferences.GetFuncCtorRef(paramNames.Length);
                        _il.Token(ctorRef);
                        // Immediately bind the new delegate to the appropriate closure scopes so that
                        // callbacks invoked later (e.g., Array.map) have a valid scopes array.
                        // Stack currently: [delegate]
                        var allNames = _variables.GetAllScopeNames().ToList();
                        var slots = allNames.Select(n => new { Name = n, Slot = _variables.GetScopeLocalSlot(n) }).ToList();
                        // Determine which scopes are actually loadable in this context and capture only those.
                        var capture = new System.Collections.Generic.List<string>();
                        // If a global scope is available via the scopes[] parameter, include it.
                        var globalEntry = slots.FirstOrDefault(e => e.Slot.Location == ObjectReferenceLocation.ScopeArray && e.Slot.Address == 0);
                        if (globalEntry != null)
                        {
                            capture.Add(globalEntry.Name);
                        }
                        // Include the current function/global scope only if its local slot exists (created earlier).
                        var leafSlot = _variables.GetLocalScopeSlot();
                        if (leafSlot.Address >= 0)
                        {
                            capture.Add(_variables.GetLeafScopeName());
                        }
                        // Note: If neither is available, capture will be empty and Bind will receive an empty scopes array.
                        // Build scopes array and call Closure.Bind(object, object[])
                        EmitScopeArray(capture);
                        _owner.Runtime.InvokeClosureBindObject();
                        javascriptType = JavascriptType.Object;
                    }
                    break;
                case FunctionExpression funcExpr:
                    {
                        // Collect parameter names; include synthetic names for non-Identifier params (e.g., destructuring)
                        var idList = ILMethodGenerator.ExtractParameterNames(funcExpr.Params).ToList();
                        if (idList.Count == 0 && funcExpr.Params.Count > 0)
                        {
                            foreach (var p in funcExpr.Params)
                            {
                                if (p is Identifier iid) { idList.Add(iid.Name); continue; }
                                var prop = p.GetType().GetProperty("Name");
                                if (prop != null)
                                {
                                    var val = prop.GetValue(p) as string;
                                    if (!string.IsNullOrEmpty(val)) { idList.Add(val!); continue; }
                                }
                                idList.Add($"p{idList.Count}");
                            }
                        }
                        var paramNames = idList.ToArray();
                        // If the function expression is named (named function expression), prefer its declared name
                        // so that the symbol table scope name and variable registry line up (important for IIFE recursion scenarios).
                        string registryScopeName;
                        if (funcExpr.Id is Identifier fid && !string.IsNullOrEmpty(fid.Name))
                        {
                            registryScopeName = fid.Name;
                        }
                        else if (!string.IsNullOrEmpty(_owner.CurrentAssignmentTarget))
                        {
                            registryScopeName = $"FunctionExpression_{_owner.CurrentAssignmentTarget}";
                        }
                        else
                        {
                            registryScopeName = $"FunctionExpression_L{funcExpr.Location.Start.Line}C{funcExpr.Location.Start.Column}";
                        }
                        var ilMethodName = $"FunctionExpression_L{funcExpr.Location.Start.Line}C{funcExpr.Location.Start.Column}";
                        var methodHandle = _owner.GenerateFunctionExpressionMethod(funcExpr, registryScopeName, ilMethodName, paramNames);

                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldnull);
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldftn);
                        _il.Token(methodHandle);
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Newobj);
                        var ctorRef = _bclReferences.GetFuncCtorRef(paramNames.Length);
                        _il.Token(ctorRef);
                        javascriptType = JavascriptType.Object;
                    }
                    break;
                case ArrayExpression arrayExpression:
                    GenerateArrayExpression(arrayExpression);
                    javascriptType = JavascriptType.Object;
                    clrType = typeof(JavaScriptRuntime.Array);
                    break;
                case NewExpression newExpression:
                    {
                        var res = EmitNewExpression(newExpression);
                        javascriptType = res.JsType;
                        clrType = res.ClrType;
                    }
                    break;
                case BinaryExpression binaryExpression:
                    {
                        var res = _binaryOperators.Generate(binaryExpression, typeCoercion, branching);
                        javascriptType = res.JsType;
                        clrType = res.ClrType;
                    }
                    break;
                case NumericLiteral:
                    javascriptType = LoadValue(expression, typeCoercion);
                    clrType = typeof(double);
                    break;
                case BooleanLiteral:
                    javascriptType = LoadValue(expression, typeCoercion);
                    break;
                case UpdateExpression updateExpression:
                    javascriptType = GenerateUpdateExpression(updateExpression, context);
                    break;
                case UnaryExpression unaryExpression:
                    javascriptType = EmitUnaryExpression(unaryExpression, typeCoercion, branching);
                    break;
                case ObjectExpression:
                    GenerateObjectExpresion((ObjectExpression)expression);
                    javascriptType = JavascriptType.Object;
                    break;
                case MemberExpression memberExpression:
                    {
                        var res = EmitMemberExpression(memberExpression);
                        javascriptType = res.JsType;
                        clrType = res.ClrType;
                    }
                    break;
                case TemplateLiteral template:
                    {
                        string GetQuasiText(TemplateElement te)
                        {
                            var valProp = te.Value;
                            var cooked = valProp.Cooked;
                            if (!string.IsNullOrEmpty(cooked)) return cooked!;
                            var raw = valProp.Raw;
                            if (raw != null) return raw;
                            return string.Empty;
                        }

                        var quasis = template.Quasis;
                        var exprs = template.Expressions;
                        string firstText = quasis.Count > 0 ? GetQuasiText(quasis[0]) : string.Empty;
                        _il.LoadString(_metadataBuilder.GetOrAddUserString(firstText ?? string.Empty));
                        for (int i = 0; i < exprs.Count; i++)
                        {
                            _ = Emit(exprs[i], new TypeCoercion { boxResult = true });
                            _runtime.InvokeOperatorsAdd();
                            string tail = (i + 1) < quasis.Count ? GetQuasiText(quasis[i + 1]) : string.Empty;
                            _il.LoadString(_metadataBuilder.GetOrAddUserString(tail ?? string.Empty));
                            _runtime.InvokeOperatorsAdd();
                        }
                        javascriptType = JavascriptType.Object;
                        clrType = typeof(string);
                    }
                    break;
                case ConditionalExpression cond:
                    {
                        var trueLabel = _il.DefineLabel();
                        var falseLabel = _il.DefineLabel();
                        var endLabel = _il.DefineLabel();

                        Emit(cond.Test, new TypeCoercion(), CallSiteContext.Expression, new ConditionalBranching { BranchOnTrue = trueLabel, BranchOnFalse = falseLabel });

                        _il.MarkLabel(trueLabel);
                        var armCoercion = new TypeCoercion { boxResult = true, toString = typeCoercion.toString };
                        _ = Emit(cond.Consequent, armCoercion);
                        _il.Branch(System.Reflection.Metadata.ILOpCode.Br, endLabel);

                        _il.MarkLabel(falseLabel);
                        _ = Emit(cond.Alternate, armCoercion);

                        _il.MarkLabel(endLabel);
                        javascriptType = JavascriptType.Object;
                    }
                    break;
                case Identifier identifier:
                    {
                        var name = identifier.Name;
                        var localVar = _variables.FindVariable(name);
                        if (localVar != null)
                        {
                            // Load variable using centralized helper; auto-unboxes number/boolean when boxResult == false
                            _il.EmitLoadVariable(localVar, _variables, _owner.BclReferences, 
                                unbox: !typeCoercion.boxResult,
                                inClassMethod: _owner.InClassMethod,
                                currentClassName: _owner.CurrentClassName,
                                classRegistry: _owner.ClassRegistry);
                            // Prevent downstream Emit() from boxing again; variables are already boxed when needed
                            typeCoercion.boxResult = false;
                            javascriptType = localVar.Type;
                            // Propagate known CLR runtime type (e.g., const perf = require('perf_hooks')) so downstream
                            // member/property emission can bind typed getters and direct instance calls.
                            clrType = localVar.ClrType;
                        }
                        else
                        {
                            // If not a local variable, attempt to resolve a public static property on GlobalThis at compile-time
                            var gvType = typeof(JavaScriptRuntime.GlobalThis);
                            var prop = gvType.GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
                            if (prop != null && prop.GetMethod != null)
                            {
                                var getter = prop.GetMethod;
                                var declType = getter.DeclaringType!;
                                var mref = _runtime.GetStaticMethodRef(declType, getter.Name, getter.ReturnType);
                                _il.OpCode(System.Reflection.Metadata.ILOpCode.Call);
                                _il.Token(mref);
                                // Determine JS type from the CLR property type so downstream coercion/boxing behaves correctly
                                if (prop.PropertyType == typeof(double))
                                {
                                    javascriptType = JavascriptType.Number;
                                }
                                else if (prop.PropertyType == typeof(bool))
                                {
                                    javascriptType = JavascriptType.Boolean;
                                }
                                // Strings are treated as Object in our JS type lattice
                                else
                                {
                                    javascriptType = JavascriptType.Object;
                                }
                                // Respect requested boxing; do not forcibly disable it here.
                                clrType = prop.PropertyType;
                            }
                            else
                            {
                                // Fallback: dynamic lookup (legacy). This should be avoided for known globals.
                                var getGlobal = _runtime.GetStaticMethodRef(
                                    typeof(JavaScriptRuntime.GlobalThis),
                                    nameof(JavaScriptRuntime.GlobalThis.Get),
                                    typeof(object),
                                    typeof(string));
                                _il.Ldstr(_metadataBuilder, name);
                                _il.Call(getGlobal);
                                typeCoercion.boxResult = false;
                                javascriptType = JavascriptType.Object;
                                clrType = null;
                            }
                        }
                    }
                    break;
                default:
                    javascriptType = LoadValue(expression, typeCoercion);
                    break;
            }

        if (branching != null && expression is not BinaryExpression)
            {
                if (javascriptType == JavascriptType.Boolean)
                {
                    _il.Branch(System.Reflection.Metadata.ILOpCode.Brtrue, branching.BranchOnTrue);
                    if (branching.BranchOnFalse.HasValue)
                    {
                        _il.Branch(System.Reflection.Metadata.ILOpCode.Br, branching.BranchOnFalse.Value);
                    }
                    else
                    {
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Pop);
                    }
                    return new ExpressionResult { JsType = JavascriptType.Unknown, ClrType = null };
                }
                else
                {
                    // Coerce any non-boolean value using JS truthiness via TypeUtilities.ToBoolean(object)
                    var toBool = _owner.Runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.ToBoolean), typeof(bool), typeof(object));
                    // Current value is on stack in its native representation; ensure it's boxed for ToBoolean(object)
                    EmitBoxIfNeeded(javascriptType);
                    // else: objects/strings already boxed/reference types in our model
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Call);
                    _il.Token(toBool);
                    _il.Branch(System.Reflection.Metadata.ILOpCode.Brtrue, branching.BranchOnTrue);
                    if (branching.BranchOnFalse.HasValue)
                    {
                        _il.Branch(System.Reflection.Metadata.ILOpCode.Br, branching.BranchOnFalse.Value);
                    }
                    else
                    {
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Pop);
                    }
                    return new ExpressionResult { JsType = JavascriptType.Unknown, ClrType = null };
                }
            }

            if (typeCoercion.boxResult)
            {
                EmitBoxIfNeeded(javascriptType);
            }

            return new ExpressionResult { JsType = javascriptType, ClrType = clrType, IsBoxed = typeCoercion.boxResult };
        }

        // Emits a call expression (function or member call) with context and optional result discard.
        // Migrated from ILMethodGenerator to centralize expression emission.
        private Type? GenerateCallExpression(Acornima.Ast.CallExpression callExpression, global::Js2IL.Services.ILGenerators.CallSiteContext context)
        {
            var _metadataBuilder = _owner.MetadataBuilder;
            var _runtime = _owner.Runtime;
            var _classRegistry = _owner.ClassRegistry;
            var _bclReferences = _owner.BclReferences;

            // General member call: obj.method(...)
            if (callExpression.Callee is Acornima.Ast.MemberExpression mem)
            {
                // Resolve the method name from the property (supports identifier and computed string literal)
                string? methodName = null;
                if (!mem.Computed && mem.Property is Acornima.Ast.Identifier idProp)
                {
                    methodName = idProp.Name;
                }
                else if (mem.Computed && mem.Property is Acornima.Ast.Literal litProp)
                {
                    if (litProp.Value is string sname) methodName = sname;
                    else if (litProp.Raw is string r && r.Length >= 2 && (r.StartsWith("\"") || r.StartsWith("'")))
                    {
                        // Strip quotes from raw if provided like ['replace']
                        methodName = r.Substring(1, r.Length - 2);
                    }
                }

                if (methodName == null)
                {
                    // Fallback when property cannot be resolved to a name we support
                    throw ILEmitHelpers.NotSupported($"Unsupported member call property kind: {mem.Property.Type}", mem.Property);
                }

                // If the receiver is definitely a string, route to a dedicated string-method emitter
                // (only when unambiguously a string literal/template or String(x) conversion)
                if (IsDefinitelyString(mem.Object))
                {
                    return EmitStringInstanceMethodCall(mem.Object, methodName, callExpression);
                }
                if (mem.Object is Acornima.Ast.Identifier baseId)
                {
                    // If the base identifier resolves to a known class, emit a static call without loading an instance.
                    if (_classRegistry.TryGet(baseId.Name, out var classType) && !classType.IsNil)
                    {
                        // Build static method signature: object method(object, ...)
                        var sArgCount = callExpression.Arguments.Count;
                        var sMsig = MethodBuilder.BuildMethodSignature(
                            _metadataBuilder,
                            isInstance: false,
                            paramCount: sArgCount,
                            hasScopesParam: false,
                            returnsVoid: false);
                        var sMref = _metadataBuilder.AddMemberReference(classType, _metadataBuilder.GetOrAddString(methodName), sMsig);
                        // Push arguments
                        for (int i = 0; i < callExpression.Arguments.Count; i++)
                        {
                            Emit(callExpression.Arguments[i], new TypeCoercion() { boxResult = true });
                        }
                        _il.Call(sMref);
                        return null;
                    }
                    // Step 1: Is it a variable?
                    var baseVar = _variables.FindVariable(baseId.Name);
                    if (baseVar != null)
                    {
                        // If the variable is known to be a CLR string, route to the string instance helper
                        if (baseVar.ClrType == typeof(string))
                        {
                            return EmitStringInstanceMethodCall(mem.Object, methodName, callExpression);
                        }

                        // Evaluate receiver once; subsequent paths assume instance is already on stack
                        var recvRes = Emit(mem.Object, new TypeCoercion()); // stack: [receiver]

                        // If this variable is a known runtime intrinsic type, emit a direct instance call using the on-stack instance
                        var runtimeType = baseVar.ClrType;
                        if (runtimeType != null)
                        {
                            if (TryEmitIntrinsicInstanceCallOnStack(runtimeType, methodName, callExpression))
                            {
                                return null;
                            }
                        }

                        // Non-intrinsic instance method: check if the variable was previously bound to a known class via `new`
                        var argCount = callExpression.Arguments.Count;
                        
                        TypeDefinitionHandle targetType = default;
                        string? className = null;
                        if (_owner.TryGetVariableClass(baseId.Name, out var cname))
                        {
                            className = cname;
                            if (_classRegistry.TryGet(cname, out var th)) targetType = th;
                        }
                        
                        if (!targetType.IsNil && className != null)
                        {
                            // Check if this method is registered with parameter count metadata
                            if (_classRegistry.TryGetMethod(className, methodName, out var methodDef, out var methodSig, out var minParams, out var maxParams))
                            {
                                // Method with tracked parameter counts - validate and pad arguments
                                if (argCount < minParams || argCount > maxParams)
                                {
                                    throw ILEmitHelpers.NotSupported($"Method {className}.{methodName} expects {minParams}-{maxParams} arguments but got {argCount}", callExpression);
                                }
                                
                                // Use registered member reference (creates it from the method definition)
                                var mrefHandle = _metadataBuilder.AddMemberReference(targetType, _metadataBuilder.GetOrAddString(methodName), methodSig);
                                
                                // Push provided arguments
                                for (int i = 0; i < argCount; i++)
                                {
                                    Emit(callExpression.Arguments[i], new TypeCoercion() { boxResult = true });
                                }
                                
                                // Pad missing optional arguments with ldnull
                                int paddingNeeded = maxParams - argCount;
                                for (int i = 0; i < paddingNeeded; i++)
                                {
                                    _il.OpCode(ILOpCode.Ldnull);
                                }
                                
                                _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
                                _il.Token(mrefHandle);
                                return null;
                            }
                            else
                            {
                                // Method not registered (no default parameters) - use shared helper
                                var msig = MethodBuilder.BuildMethodSignature(
                                    _metadataBuilder,
                                    isInstance: true,
                                    paramCount: argCount,
                                    hasScopesParam: false,
                                    returnsVoid: false);

                                var mrefHandle = _metadataBuilder.AddMemberReference(targetType, _metadataBuilder.GetOrAddString(methodName), msig);
                                // Push arguments
                                EmitBoxedArgsInline(callExpression.Arguments);
                                _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
                                _il.Token(mrefHandle);
                                return null;
                            }
                        }

                        // Dynamic dispatch through runtime: Object.CallMember(receiver, name, object[])
                        _il.Ldstr(_metadataBuilder, methodName);
                        EmitBoxedArgsArray(callExpression.Arguments);
                        var dynCall = _owner.Runtime.GetStaticMethodRef(
                            typeof(JavaScriptRuntime.Object),
                            nameof(JavaScriptRuntime.Object.CallMember),
                            typeof(object),
                            typeof(object), typeof(string), typeof(object[]));
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Call);
                        _il.Token(dynCall);
                        return null;
                    }
                    else
                    {
                        // First, try host intrinsic static call (e.g., console.log)
                        if (TryEmitHostIntrinsicStaticCall(baseId.Name, methodName, callExpression))
                        {
                            // Most host intrinsic statics (e.g., Console.Log) return object; treat as dynamic when unknown
                            return null;
                        }

                        // Otherwise, if the identifier refers to a public static property on GlobalThis (e.g., process),
                        // load that instance and perform a normal instance call on it.
                        var gvType = typeof(JavaScriptRuntime.GlobalThis);
                        var gvProp = gvType.GetProperty(baseId.Name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
                        if (gvProp?.GetMethod != null)
                        {
                            // Load the global instance (stack: instance)
                            var getterDecl = gvProp.GetMethod.DeclaringType!;
                            var getterRef = _runtime.GetStaticMethodRef(getterDecl, gvProp.GetMethod.Name, gvProp.PropertyType);
                            _il.OpCode(System.Reflection.Metadata.ILOpCode.Call);
                            _il.Token(getterRef);

                            // Reflect instance method on the returned type
                            var rt = gvProp.PropertyType;
                            var methods = rt
                                .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                                .Where(mi => string.Equals(mi.Name, methodName, StringComparison.Ordinal));

                            var chosen = methods.FirstOrDefault(mi =>
                            {
                                var ps = mi.GetParameters();
                                return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
                            }) ?? methods.FirstOrDefault(mi => mi.GetParameters().Length == callExpression.Arguments.Count);

                            if (chosen == null)
                            {
                                throw ILEmitHelpers.NotSupported($"Method not found: {rt.FullName}.{methodName} with {callExpression.Arguments.Count} arg(s)", callExpression);
                            }

                            var ps = chosen.GetParameters();
                            var expectsParamsArray = ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
                            var reflectedParamTypes = ps.Select(p => p.ParameterType).ToArray();
                            var reflectedReturnType = chosen.ReturnType;

                            var mrefHandle = _runtime.GetInstanceMethodRef(rt, chosen.Name, reflectedReturnType, reflectedParamTypes);

                            // Cast the global instance receiver to its concrete return type before callvirt
                            _il.OpCode(System.Reflection.Metadata.ILOpCode.Castclass);
                            _il.Token(_owner.Runtime.GetRuntimeTypeHandle(rt));

                            if (expectsParamsArray) EmitBoxedArgsArray(callExpression.Arguments);
                            else EmitBoxedArgsInline(callExpression.Arguments);

                            _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
                            _il.Token(mrefHandle);
                            return reflectedReturnType;
                        }
                        // Step 4 fallback (legacy): no intrinsic mapping and not a GlobalThis property
                        throw ILEmitHelpers.NotSupported($"Unsupported member call base identifier: '{baseId.Name}'", baseId);
                    }
                }
                
                // Special case: this.method() calls in class instance methods
                // Optimize to direct callvirt instead of dynamic Object.CallMember dispatch
                if (mem.Object is Acornima.Ast.ThisExpression && _owner.InClassMethod && _owner.CurrentClassName != null)
                {
                    var argCount = callExpression.Arguments.Count;
                    if (_classRegistry.TryGet(_owner.CurrentClassName, out var targetType) && !targetType.IsNil)
                    {
                        // Load this (arg_0)
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldarg_0);
                        
                        // Check if this method is registered with parameter count metadata
                        if (_classRegistry.TryGetMethod(_owner.CurrentClassName, methodName, out var methodDef, out var methodSig, out var minParams, out var maxParams))
                        {
                            // Method with tracked parameter counts - validate and pad arguments
                            if (argCount < minParams || argCount > maxParams)
                            {
                                throw ILEmitHelpers.NotSupported($"Method {_owner.CurrentClassName}.{methodName} expects {minParams}-{maxParams} arguments but got {argCount}", callExpression);
                            }
                            
                            // Use registered member reference
                            var mrefHandle = _metadataBuilder.AddMemberReference(targetType, _metadataBuilder.GetOrAddString(methodName), methodSig);
                            
                            // Push provided arguments
                            for (int i = 0; i < argCount; i++)
                            {
                                Emit(callExpression.Arguments[i], new TypeCoercion() { boxResult = true });
                            }
                            
                            // Pad missing optional arguments with ldnull
                            int paddingNeeded = maxParams - argCount;
                            for (int i = 0; i < paddingNeeded; i++)
                            {
                                _il.OpCode(ILOpCode.Ldnull);
                            }
                            
                            _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
                            _il.Token(mrefHandle);
                            return null;
                        }
                        else
                        {
                            // Method not registered (no default parameters) - use standard signature
                            var msig = MethodBuilder.BuildMethodSignature(
                                _metadataBuilder,
                                isInstance: true,
                                paramCount: argCount,
                                hasScopesParam: false,
                                returnsVoid: false);

                            var mrefHandle = _metadataBuilder.AddMemberReference(targetType, _metadataBuilder.GetOrAddString(methodName), msig);
                            // Push arguments
                            EmitBoxedArgsInline(callExpression.Arguments);
                            _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
                            _il.Token(mrefHandle);
                            return null;
                        }
                    }
                }
                
                // Receiver is an arbitrary expression (e.g., (expr).method(...))
                // If the receiver's CLR type is known and method is resolvable, emit a direct callvirt.
                // Otherwise fall back to the generic runtime dispatcher via Object.CallMember.
                var recv = Emit(mem.Object, new TypeCoercion());
                // Avoid direct callvirt for JavaScriptRuntime.Array to ensure consistent params object[] dispatch
                if (recv.ClrType != null && recv.ClrType != typeof(JavaScriptRuntime.Array))
                {
                    var rt = recv.ClrType;
                    var methods = rt
                        .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                        .Where(mi => string.Equals(mi.Name, methodName, StringComparison.Ordinal));

                    // Prefer a single-parameter params object[] overload when available to unify semantics
                    // (e.g., Array methods like slice, includes, join). Fallback to exact arity match.
                    var chosen = methods.FirstOrDefault(mi =>
                                        mi.GetParameters().Length == 1 && mi.GetParameters()[0].ParameterType == typeof(object[]))
                                 ?? methods.FirstOrDefault(mi => mi.GetParameters().Length == callExpression.Arguments.Count);

                    if (chosen != null)
                    {
                        var ps = chosen.GetParameters();
                        var expectsParamsArray = ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
                        var reflectedParamTypes = expectsParamsArray ? new[] { typeof(object[]) } : ps.Select(p => p.ParameterType).ToArray();
                        var reflectedReturnType = chosen.ReturnType;
                        var mrefHandle = _owner.Runtime.GetInstanceMethodRef(rt, chosen.Name, reflectedReturnType, reflectedParamTypes);

                        // Ensure the receiver is the declaring CLR type before callvirt (verifier safety)
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Castclass);
                        _il.Token(_owner.Runtime.GetRuntimeTypeHandle(rt));

                        if (expectsParamsArray) EmitBoxedArgsArray(callExpression.Arguments);
                        else EmitBoxedArgsInline(callExpression.Arguments);

                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
                        _il.Token(mrefHandle);
                        return reflectedReturnType;
                    }
                }

                // Fallback: dynamic dispatcher using Object.CallMember
                // Stack currently has [receiver]
                _il.Ldstr(_metadataBuilder, methodName); // [receiver, name]
                EmitBoxedArgsArray(callExpression.Arguments);
                var callMember = _owner.Runtime.GetStaticMethodRef(
                    typeof(JavaScriptRuntime.Object),
                    nameof(JavaScriptRuntime.Object.CallMember),
                    typeof(object),
                    typeof(object), typeof(string), typeof(object[]));
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Call);
                _il.Token(callMember);
                return null;
            }
            else if (callExpression.Callee is Acornima.Ast.Identifier identifier)
            {
                // Simple function call: f(...)
                return EmitFunctionCall(identifier, callExpression, context);
            }
            else if (callExpression.Callee is Acornima.Ast.FunctionExpression || callExpression.Callee is Acornima.Ast.ArrowFunctionExpression)
            {
                // Immediately Invoked Function Expression (IIFE) support.
                // Strategy:
                // 1. Emit the inline function/arrow expression to obtain its delegate on the stack.
                // 2. Construct a scopes array similar to nested function invocation semantics.
                // 3. Push call arguments boxed.
                // 4. Invoke appropriate delegate Invoke overload (object Invoke(object[] scopes, ...args)).

                var calleeExpr = callExpression.Callee;
                // Re-use normal Emit path to build the delegate (handles both function & arrow expressions).
                _ = Emit(calleeExpr, new TypeCoercion { boxResult = false }); // stack: [delegate]

                // Determine scopes to pass
                var scopeNames = DetermineScopesForDelegateCall();

                EmitScopeArray(scopeNames); // stack: [delegate, scopes[]]

                // Emit arguments
                EmitBoxedArgsInline(callExpression.Arguments); // stack: [delegate, scopes[], arg0, ...]

                var argCount = callExpression.Arguments.Count;
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
                if (argCount <= 6)
                {
                    _il.Token(_bclReferences.GetFuncInvokeRef(argCount));
                }
                else
                {
                    throw ILEmitHelpers.NotSupported($"IIFE with more than 6 arguments not supported (got {argCount})", callExpression);
                }

                return null; // dynamic return type
            }
            else
            {
                throw ILEmitHelpers.NotSupported($"Unsupported call expression callee type: {callExpression.Callee.Type}", callExpression.Callee);
            }
        }

        /// <summary>
        /// Emits a static call on a host intrinsic object (e.g., console.log), discovered via IntrinsicObjectRegistry.
        /// Returns true if a matching intrinsic was found and emitted.
        /// </summary>
        private bool TryEmitHostIntrinsicStaticCall(string objectName, string methodName, Acornima.Ast.CallExpression callExpression)
        {
            var _bclReferences = _owner.BclReferences;
            var _runtime = _owner.Runtime;

            // Special-case: console is a global variable, not a constructible intrinsic, but we emit its calls
            // as static method calls on JavaScriptRuntime.Console to preserve historical IL snapshots.
            var type = string.Equals(objectName, "console", StringComparison.Ordinal)
                ? typeof(JavaScriptRuntime.Console)
                : JavaScriptRuntime.IntrinsicObjectRegistry.Get(objectName);
            if (type == null)
            {
                return false;
            }

            // Reflect static method candidates
            var allMethods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var methods = allMethods.Where(mi => string.Equals(mi.Name, methodName, StringComparison.OrdinalIgnoreCase));

            // Prefer exact arity match first, then zero-parameter when no args, then params object[]
            var chosen = methods.FirstOrDefault(mi => mi.GetParameters().Length == callExpression.Arguments.Count);
            if (chosen == null && callExpression.Arguments.Count == 0)
            {
                chosen = methods.FirstOrDefault(mi => mi.GetParameters().Length == 0);
            }
            if (chosen == null)
            {
                chosen = methods.FirstOrDefault(mi =>
                {
                    var ps = mi.GetParameters();
                    return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
                });
            }

            if (chosen == null)
            {
                // Fallback: map console.error/warn -> console.log when specific overloads are not present
                if (string.Equals(objectName, "console", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(methodName, "log", StringComparison.OrdinalIgnoreCase))
                {
                    var alt = allMethods
                        .Where(mi => string.Equals(mi.Name, "log", StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(mi =>
                        {
                            var ps = mi.GetParameters();
                            // Prefer exact arity, then params object[]
                            if (ps.Length == callExpression.Arguments.Count) return 2;
                            if (ps.Length == 1 && ps[0].ParameterType == typeof(object[])) return 1;
                            return 0;
                        })
                        .FirstOrDefault();
                    if (alt != null)
                    {
                        chosen = alt;
                    }
                }
            }
            if (chosen == null)
            {
                throw ILEmitHelpers.NotSupported($"Host intrinsic method not found: {type.FullName}.{methodName} with {callExpression.Arguments.Count} arg(s)", callExpression);
            }

            var ps = chosen.GetParameters();
            var expectsParamsArray = ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
            var paramTypes = ps.Select(p => p.ParameterType).ToArray();
            var retType = chosen.ReturnType;

            var mref = _runtime.GetStaticMethodRef(type, chosen.Name, retType, paramTypes);

            if (expectsParamsArray)
            {
                EmitBoxedArgsArray(callExpression.Arguments);
            }
            else
            {
                // Emit each argument matching the chosen method's parameter types when possible.
                var psChosen = chosen.GetParameters();
                for (int i = 0; i < callExpression.Arguments.Count; i++)
                {
                    var targetType = (i < psChosen.Length) ? psChosen[i].ParameterType : typeof(object);
                    if (targetType == typeof(string))
                    {
                        _ = Emit(callExpression.Arguments[i], new TypeCoercion { toString = true, boxResult = false });
                    }
                    else
                    {
                        // Default to boxed object for non-string parameters (simple, keeps behavior consistent with dynamic paths)
                        _ = Emit(callExpression.Arguments[i], new TypeCoercion { boxResult = true });
                    }
                }
            }

            _il.Call(mref);
            return true;
        }

        // Lightweight compile-time analyzer: return true only when the expression is unambiguously a string
        private static bool IsDefinitelyString(Expression expr)
        {
            if (expr is StringLiteral) return true;
            if (expr is TemplateLiteral) return true;
            if (expr is CallExpression ce && ce.Callee is Identifier id && string.Equals(id.Name, "String", StringComparison.Ordinal) && ce.Arguments.Count == 1)
                return true;
            return false;
        }

        // Centralized emitter for string instance methods using reflection-based dispatch.
        private Type? EmitStringInstanceMethodCall(Expression receiver, string methodName, CallExpression callExpression)
        {
            var _runtime = _owner.Runtime;
            var _bclReferences = _owner.BclReferences;

            // Push receiver coerced to string (first param in all runtime String methods)
            _ = Emit(receiver, new TypeCoercion { toString = true });

            var stringType = JavaScriptRuntime.IntrinsicObjectRegistry.Get("String")
                ?? throw ILEmitHelpers.NotSupported("Host intrinsic 'String' not found", callExpression);

            // Gather candidate methods by name (case-insensitive to map JS camelCase to CLR PascalCase like LocaleCompare)
            var candidates = stringType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => string.Equals(m.Name, methodName, StringComparison.OrdinalIgnoreCase))
                .Where(m => m.GetParameters().Length >= 1 && m.GetParameters()[0].ParameterType == typeof(string))
                .ToList();

            if (candidates.Count == 0)
            {
                throw ILEmitHelpers.NotSupported($"Host intrinsic method not found: String.{methodName}", callExpression);
            }

            // Pre-detect regex literal in arg0 to optionally enable an expanded signature binding
            bool hasRegex = false; string? regexPattern = null; bool regexGlobal = false; bool regexIgnoreCase = false;
            if (callExpression.Arguments.Count >= 1 && callExpression.Arguments[0] is Literal lit)
            {
                var rawProp = GetPropertyIgnoreCase(lit, "Raw");
                if (rawProp != null)
                {
                    var rawObj = rawProp.GetValue(lit);
                    var raw = rawObj as string ?? rawObj?.ToString() ?? string.Empty;
                    var (pattern, flags) = ParseRegexRaw(raw);
                    if (pattern != null)
                    {
                        hasRegex = true;
                        regexPattern = pattern;
                        regexGlobal = flags != null && flags.Contains('g');
                        regexIgnoreCase = flags != null && flags.Contains('i');
                    }
                }
            }

            int argCount = callExpression.Arguments.Count;

            MethodInfo? chosen = null;
            bool useRegexExpansion = false;

            // 1) Prefer regex expansion when a regex literal is present and a 5-arg pattern exists
            if (hasRegex)
            {
                var regexMatches = candidates.Where(m =>
                {
                    var ps = m.GetParameters();
                    if (ps.Length != 5) return false;
                    // (string receiver, string pattern, string replacement|object callback, bool global, bool ignoreCase)
                    return ps[0].ParameterType == typeof(string)
                        && ps[1].ParameterType == typeof(string)
                        && (ps[2].ParameterType == typeof(string) || ps[2].ParameterType == typeof(object))
                        && ps[3].ParameterType == typeof(bool)
                        && ps[4].ParameterType == typeof(bool);
                }).ToList();
                if (regexMatches.Count > 0 && argCount == 2)
                {
                    // Optimization: if replacement is an arrow function of the form r => `\\${r}` (escape match with backslash),
                    // prefer string-based replacement using regex '$&' whole-match token to avoid callback delegate emission.
                    bool patternIsEscape = false;
                    if (callExpression.Arguments[1] is ArrowFunctionExpression afe && afe.Body is TemplateLiteral t)
                    {
                        if (t.Expressions.Count == 1 && t.Quasis.Count == 2 && t.Quasis[0].Value.Cooked == "\\")
                        {
                            patternIsEscape = true;
                        }
                    }

                    if (patternIsEscape)
                    {
                        // Prefer the overload with string replacement
                        chosen = regexMatches.First(m => m.GetParameters()[2].ParameterType == typeof(string));
                        useRegexExpansion = true;
                        // We'll handle emission below by loading replacement string "\\$&" instead of emitting the function
                        // and setting flags as usual.
                    }
                    else
                    {
                        chosen = regexMatches.First();
                        useRegexExpansion = true;
                    }
                }
            }

            // 2) If not using regex expansion, find a method we can call with provided args and pad missing optionals
            if (chosen == null)
            {
                // Prefer the smallest parameter count that can accept the provided args (>= 1 + argCount)
                var viable = candidates
                    .Where(m => m.GetParameters().Length >= 1 + argCount)
                    .OrderBy(m => m.GetParameters().Length)
                    .ToList();

                if (viable.Count == 0)
                {
                    // Fallback to exact arity if none accept padding
                    viable = candidates.Where(m => m.GetParameters().Length == 1 + argCount).ToList();
                }

                if (viable.Count > 0)
                {
                    // Among viable, prefer more specific parameter types over object
                    chosen = viable
                        .OrderByDescending(m => m.GetParameters().Skip(1).Take(argCount).Count(p => p.ParameterType != typeof(object)))
                        .First();
                }
            }

            if (chosen == null)
            {
                throw ILEmitHelpers.NotSupported($"No compatible overload found for String.{methodName} with {argCount} argument(s)", callExpression);
            }

            // Emit arguments based on the chosen parameters
            var chosenParams = chosen.GetParameters();
            if (useRegexExpansion)
            {
                // Expect 2 JS args: pattern (regex literal), replacement; expand to (pattern string, replacement string, bool g, bool i)
                if (!hasRegex || regexPattern is null || argCount != 2)
                {
                    throw ILEmitHelpers.NotSupported("Regex expansion requires a regex literal as first argument and exactly 2 JS arguments.", callExpression);
                }
                // pattern
                _il.Ldstr(_owner.MetadataBuilder, regexPattern);
                // replacement: if method expects string, coerce to string; if object, pass as-is (delegate/callback)
                var chosenParamsLocal = chosen.GetParameters();
                if (chosenParamsLocal[2].ParameterType == typeof(string))
                {
                    // Special-case escape pattern r => `\\${r}` -> replacement string "\\$&"
                    if (callExpression.Arguments[1] is ArrowFunctionExpression afe2 && afe2.Body is TemplateLiteral t2
                        && t2.Expressions.Count == 1 && t2.Quasis.Count == 2 && t2.Quasis[0].Value.Cooked == "\\")
                    {
                        _il.Ldstr(_owner.MetadataBuilder, "\\$&");
                    }
                    else
                    {
                        _ = Emit(callExpression.Arguments[1], new TypeCoercion { toString = true });
                    }
                }
                else
                {
                    _ = Emit(callExpression.Arguments[1], new TypeCoercion { boxResult = true });
                }
                // flags
                _il.LoadConstantI4(regexGlobal ? 1 : 0);
                _il.LoadConstantI4(regexIgnoreCase ? 1 : 0);
            }
            else
            {
                // For each JS argument, coerce based on parameter type
                for (int i = 0; i < argCount; i++)
                {
                    var targetParamType = chosenParams[i + 1].ParameterType;
                    if (targetParamType == typeof(string))
                    {
                        _ = Emit(callExpression.Arguments[i], new TypeCoercion { toString = true });
                    }
                    else if (targetParamType == typeof(bool))
                    {
                        var arg = callExpression.Arguments[i];
                        if (arg is BooleanLiteral bl)
                        {
                            _il.LoadConstantI4(bl.Value ? 1 : 0);
                        }
                        else if (arg is Literal glit && glit.Value is bool bv)
                        {
                            _il.LoadConstantI4(bv ? 1 : 0);
                        }
                        else
                        {
                            _il.LoadConstantI4(0);
                        }
                    }
                    else
                    {
                        _ = Emit(callExpression.Arguments[i], new TypeCoercion { boxResult = true });
                    }
                }

                // Pad any remaining parameters with defaults: null for ref types; false for bool
                for (int pi = 1 + argCount; pi < chosenParams.Length; pi++)
                {
                    var pt = chosenParams[pi].ParameterType;
                    if (pt == typeof(bool))
                    {
                        _il.LoadConstantI4(0);
                    }
                    else
                    {
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldnull);
                    }
                }
            }

            // Emit the call
            var mref = _runtime.GetStaticMethodRef(stringType, chosen.Name, chosen.ReturnType, chosenParams.Select(p => p.ParameterType).ToArray());
            _il.Call(mref);

            // Return CLR type for downstream typing
            return chosen.ReturnType;
        }

        /// <summary>
        /// Attempts to emit an instance method call on a runtime intrinsic object (e.g., require('path') -> Path).
        /// Returns true if the call was emitted; otherwise false.
        /// </summary>
        private bool TryEmitIntrinsicInstanceCall(Variable baseVar, string methodName, Acornima.Ast.CallExpression callExpression)
        {
            var _bclReferences = _owner.BclReferences;
            var _runtime = _owner.Runtime;

            // Only applies to runtime intrinsic objects backed by a known CLR type.
            // If the Variable cache lacks the type (e.g., resolved from registry in nested function),
            // consult the shared registry for a recorded RuntimeIntrinsicType.
            var runtimeType = baseVar.ClrType;
            if (runtimeType == null)
            {
                var reg = _owner.Variables.GetVariableRegistry();
                var vi = reg?.GetVariableInfo(baseVar.ScopeName, baseVar.Name) ?? reg?.FindVariable(baseVar.Name);
                runtimeType = vi?.ClrType;
            }
            if (runtimeType == null)
            {
                return false;
            }

            // Load instance from the variable's scope field safely
            var slot = _variables.GetScopeLocalSlot(baseVar.ScopeName);
            if (slot.Address == -1)
            {
                // In contexts like arrow-function parameters, the variable may not be addressable as a local field.
                // Signal to caller that intrinsic emission isn't possible so it can fallback to dynamic dispatch.
                return false;
            }
            EmitLoadScopeObject(slot);
            // Cast the scope instance before field access for verifier safety
            var regScope = _variables.GetVariableRegistry();
            var tdef = regScope?.GetScopeTypeHandle(baseVar.ScopeName) ?? default;
            if (!tdef.IsNil)
            {
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Castclass);
                _il.Token(tdef);
            }
            _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldfld);
            _il.Token(baseVar.FieldHandle);

            // Reflect and select the target method, preferring params object[]
            var rt = runtimeType;
            var methods = rt
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => string.Equals(mi.Name, methodName, StringComparison.Ordinal));

            var chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
            }) ?? methods.FirstOrDefault(mi => mi.GetParameters().Length == callExpression.Arguments.Count);

            if (chosen == null)
            {
                throw ILEmitHelpers.NotSupported($"Intrinsic method not found: {rt.FullName}.{methodName} with {callExpression.Arguments.Count} arg(s)", callExpression);
            }

            var psChosen = chosen.GetParameters();
            var expectsParamsArray = psChosen.Length == 1 && psChosen[0].ParameterType == typeof(object[]);
            var reflectedParamTypes = psChosen.Select(p => p.ParameterType).ToArray();
            var reflectedReturnType = chosen.ReturnType;

            var mrefHandle = _runtime.GetInstanceMethodRef(rt, chosen.Name, reflectedReturnType, reflectedParamTypes);

            // Cast receiver (currently object from ldfld) to the declaring CLR type for verifier safety
            _il.OpCode(System.Reflection.Metadata.ILOpCode.Castclass);
            _il.Token(_owner.Runtime.GetRuntimeTypeHandle(rt));

            // Push arguments as either a packed object[] or individual boxed args
            if (expectsParamsArray) EmitBoxedArgsArray(callExpression.Arguments);
            else EmitBoxedArgsInline(callExpression.Arguments);

            _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
            _il.Token(mrefHandle);

            return true;
        }

        /// <summary>
        /// Emits an intrinsic instance method call using the receiver already on the evaluation stack.
        /// Returns true if a matching method was found and emitted; otherwise false.
        /// </summary>
        private bool TryEmitIntrinsicInstanceCallOnStack(Type runtimeType, string methodName, Acornima.Ast.CallExpression callExpression)
        {
            var _runtime = _owner.Runtime;

            var rt = runtimeType;
            var methods = rt
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => string.Equals(mi.Name, methodName, StringComparison.Ordinal));

            var chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
            }) ?? methods.FirstOrDefault(mi => mi.GetParameters().Length == callExpression.Arguments.Count);

            if (chosen == null)
            {
                return false;
            }

            var psChosen = chosen.GetParameters();
            var expectsParamsArray = psChosen.Length == 1 && psChosen[0].ParameterType == typeof(object[]);
            var reflectedParamTypes = psChosen.Select(p => p.ParameterType).ToArray();
            var reflectedReturnType = chosen.ReturnType;

            var mrefHandle = _runtime.GetInstanceMethodRef(rt, chosen.Name, reflectedReturnType, reflectedParamTypes);

            // Cast receiver on stack to concrete CLR type before callvirt
            _il.OpCode(System.Reflection.Metadata.ILOpCode.Castclass);
            _il.Token(_owner.Runtime.GetRuntimeTypeHandle(rt));

            if (expectsParamsArray) EmitBoxedArgsArray(callExpression.Arguments);
            else EmitBoxedArgsInline(callExpression.Arguments);

            _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
            _il.Token(mrefHandle);
            return true;
        }

        // Emits a call to a function identified by an Identifier in the current scope, including scope array construction and delegate dispatch.
        private Type? EmitFunctionCall(Acornima.Ast.Identifier identifier, Acornima.Ast.CallExpression callExpression, global::Js2IL.Services.ILGenerators.CallSiteContext context)
        {
            var _runtime = _owner.Runtime;
            var _bclReferences = _owner.BclReferences;

            // Node-style require("module") support as a built-in
            if (string.Equals(identifier.Name, "require", StringComparison.Ordinal))
            {
                if (callExpression.Arguments.Count != 1)
                {
                    throw new ArgumentException("require expects exactly one argument");
                }
                // Coerce argument to string (for literals this emits ldstr directly)
                _ = Emit(callExpression.Arguments[0], new TypeCoercion() { toString = true });
                _runtime.InvokeRequire();
                // Identify module type for ClrType surface
                string? mod = null;
                var arg0 = callExpression.Arguments[0];
                if (arg0 is StringLiteral s) mod = s.Value;
                else if (arg0 is Literal glit && glit.Value is string gs) mod = gs;
                return ResolveNodeModuleType(mod ?? string.Empty);
            }

            // Global String(x) conversion support
            if (string.Equals(identifier.Name, "String", StringComparison.Ordinal))
            {
                if (callExpression.Arguments.Count != 1)
                {
                    throw new ArgumentException("String() expects exactly one argument");
                }
                _ = Emit(callExpression.Arguments[0], new TypeCoercion() { toString = true });
                return typeof(string);
            }

            // generic method for invoking intrusive functions
            // TODO:  This should be done after checking variables not before... much work to be done
            if (_runtime.TryInvokeIntrinsicFunction(this, identifier.Name, callExpression.Arguments)) {
                return null; // dynamic return type
            }

            var functionVariable = _variables.FindVariable(identifier.Name);
            if (functionVariable == null)
            {
                throw new ArgumentException($"Function {identifier.Name} is not defined.");
            }

            // Check if this function is stored as a local variable (uncaptured) or field (captured/function declaration)
            bool isLocalVariable = functionVariable.LocalSlot >= 0;

            // For field variables, we'll need scope reference for store operations
            ScopeObjectReference scopeObjectReference = default;
            if (!isLocalVariable)
            {
                scopeObjectReference = _variables.GetScopeLocalSlot(functionVariable.ScopeName);
                if (scopeObjectReference.Address == -1)
                {
                    throw new InvalidOperationException($"Scope '{functionVariable.ScopeName}' not found in local slots");
                }
            }

            // Load the delegate to be invoked (from local or scope field). If this is a named function expression's
            // internal binding and delegate is still null (first recursive call), lazily self-bind by creating
            // a delegate to the generated method. We detect named function expression by functionVariable.VariableType == Function
            // and registry scope name starting with "FunctionExpression_" or the original name being the same as scope name when
            // that scope originated from a named function expression (SymbolTable added internal binding).
            //
            // Correct stack discipline:
            // - Load delegate value only (no need to keep a copy of the scope on the stack for the common case).
            // - If null, create and store the delegate, then reload the field to obtain the receiver for Invoke.
            _il.EmitLoadVariable(functionVariable, _variables, _bclReferences, unbox: false, inClassMethod: false, currentClassName: null, classRegistry: null); // stack: [delegate]
            // Duplicate delegate for null check
            _il.OpCode(System.Reflection.Metadata.ILOpCode.Dup);
            var nonNullLabel = _il.DefineLabel();
            var afterBindLabel = _il.DefineLabel();
            
            // Attempt to resolve method handle from function registry via owner generator BEFORE branching
            // so we know the parameter count in both null and non-null paths
            var fnReg = _owner.FunctionRegistry;
            var registryHandle = fnReg?.Get(identifier.Name) ?? default;
            // Get the expected parameter count for the function
            int expectedParamCount = fnReg?.GetParameterCount(identifier.Name) ?? callExpression.Arguments.Count;
            
            // Check if the function is registered (either pre-registered or fully registered).
            // For recursive calls, the function is pre-registered but handle is nil.
            bool isRegisteredFunction = fnReg?.IsRegistered(identifier.Name) ?? false;
            
            _il.Branch(System.Reflection.Metadata.ILOpCode.Brtrue_s, nonNullLabel);
            // Null path: pop the duplicate null
            _il.OpCode(System.Reflection.Metadata.ILOpCode.Pop);
            if (isRegisteredFunction && !registryHandle.IsNil)
            {
                // Registered function with valid handle - create typed delegate at compile-time
                // Create delegate and store into the variable (field or local)
                // Use the function's actual parameter count, not the call site's argument count
                if (!isLocalVariable)
                {
                    EmitLoadScopeObject(scopeObjectReference);
                }
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldnull);
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldftn);
                _il.Token(registryHandle);
                var ctorRefLazy = _bclReferences.GetFuncCtorRef(expectedParamCount);
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Newobj);
                _il.Token(ctorRefLazy);
                if (isLocalVariable)
                {
                    _il.StoreLocal(functionVariable.LocalSlot);
                }
                else
                {
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Stfld);
                    _il.Token(functionVariable.FieldHandle);
                }
                // Reload the stored delegate to use as the call receiver
                _il.EmitLoadVariable(functionVariable, _variables, _bclReferences, unbox: false, inClassMethod: false, currentClassName: null, classRegistry: null);
            }
            else if (isRegisteredFunction && registryHandle.IsNil)
            {
                // Pre-registered function (nil handle) - use runtime self-binding
                // This happens during recursive function calls before the method is finalized
                var _metadataBuilder = _owner.MetadataBuilder;
                // Stack the scope for stfld (only for field variables)
                if (!isLocalVariable)
                {
                    EmitLoadScopeObject(scopeObjectReference);
                }
                // Call System.Reflection.MethodBase.GetCurrentMethod() to get current function
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Call);
                _il.Token(_bclReferences.MethodBase_GetCurrentMethod_Ref);
                // Load parameter count as Int32
                _il.LoadConstantI4(expectedParamCount);
                // Call Closure.CreateSelfDelegate(MethodBase, int) to create typed delegate
                var closureTypeRef = _owner.Runtime.GetRuntimeTypeHandle(typeof(JavaScriptRuntime.Closure));
                var makeSelfSig = new BlobBuilder();
                new BlobEncoder(makeSelfSig)
                    .MethodSignature(isInstanceMethod: false)
                    .Parameters(2,
                        rt => rt.Type().Object(),
                        p => {
                            p.AddParameter().Type().Type(_bclReferences.MethodBaseType, isValueType: false);
                            p.AddParameter().Type().Int32();
                        });
                var makeSelfRef = _metadataBuilder.AddMemberReference(
                    closureTypeRef,
                    _metadataBuilder.GetOrAddString(nameof(JavaScriptRuntime.Closure.CreateSelfDelegate)),
                    _metadataBuilder.GetOrAddBlob(makeSelfSig));
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Call);
                _il.Token(makeSelfRef);
                // Store delegate to variable (local or field)
                if (isLocalVariable)
                {
                    _il.StoreLocal(functionVariable.LocalSlot);
                }
                else
                {
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Stfld);
                    _il.Token(functionVariable.FieldHandle);
                }
                // Reload to use as call receiver
                _il.EmitLoadVariable(functionVariable, _variables, _bclReferences, unbox: false, inClassMethod: false, currentClassName: null, classRegistry: null);
            }
            else
            {
                // Not registered - leave null delegate on stack to preserve behavior (will throw)
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldnull);
            }
            _il.Branch(ILOpCode.Br, afterBindLabel);
            _il.MarkLabel(nonNullLabel);
            // Non-null path: delegate already on stack; do nothing
            _il.MarkLabel(afterBindLabel);

            var argCount = callExpression.Arguments.Count;

            // If function is not in registry, use runtime type inspection via Closure.InvokeWithArgs
            // This handles cases where a variable holds a function returned from another function
            if (!isRegisteredFunction)
            {
                // Stack: [delegate]
                // Build scopes array
                {
                    var allNames = _variables.GetAllScopeNames().ToList();
                    var slots = allNames.Select(n => _variables.GetScopeLocalSlot(n)).ToList();
                    bool inFunctionContext = slots.Any(s => s.Location == ObjectReferenceLocation.ScopeArray);
                    var neededScopeNames = GetNeededScopesForFunction(functionVariable, context).ToList();
                    if (inFunctionContext && neededScopeNames.Count == 1)
                    {
                        _il.LoadArgument(0); // pass-through existing scopes[]
                    }
                    else
                    {
                        EmitScopeArray(neededScopeNames);
                    }
                }
                // Stack: [delegate, scopes]
                
                // Build args array
                _il.EmitNewArray(argCount, _bclReferences.ObjectType, (il, i) =>
                {
                    _ = Emit(callExpression.Arguments[i], new TypeCoercion { boxResult = true });
                });
                // Stack: [delegate, scopes, args[]]
                
                // Call Closure.InvokeWithArgs(object target, object[] scopes, params object[] args)
                _owner.Runtime.InvokeClosureInvokeWithArgs();
                return null;
            }

            // First argument: create scope array with appropriate scopes for the function
            // Strategy:
            // - If inside a function context (this method has a scopes[] parameter), we may need to enrich the scopes[]
            //   with the current local scope when invoking a locally declared nested function. Passing the raw scopes[]
            //   (global-only) caused IndexOutOfRangeExceptions for patterns that expect [global, local] where local is
            //   the scope owning the callee (e.g., Array_Map_NestedParam inner function expecting scopes[1]).
            // - Build a scopes array via GetNeededScopesForFunction in both contexts. When the result set matches the
            //   existing pass-through (single global) we reuse the existing scopes[] to avoid unnecessary allocations.
            {
                var allNames = _variables.GetAllScopeNames().ToList();
                var slots = allNames.Select(n => _variables.GetScopeLocalSlot(n)).ToList();
                bool inFunctionContext = slots.Any(s => s.Location == ObjectReferenceLocation.ScopeArray);
                var neededScopeNames = GetNeededScopesForFunction(functionVariable, context).ToList();
                // If we are in a function context and the needed scopes list is exactly one (the global) we can
                // just forward the existing scopes[] parameter. Otherwise emit a new scopes array to match expected shape.
                if (inFunctionContext && neededScopeNames.Count == 1)
                {
                    _il.LoadArgument(0); // pass-through existing scopes[]
                }
                else
                {
                    EmitScopeArray(neededScopeNames);
                }
            }

            // Additional arguments: directly emit each call argument (boxed as needed)
            EmitBoxedArgsInline(callExpression.Arguments);
            
            // Pad with null for missing parameters (to support default parameters)
            for (int i = argCount; i < expectedParamCount; i++)
            {
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldnull);
            }

            // Invoke correct delegate based on EXPECTED parameter count (not call-site argument count)
            // This ensures we pass the right number of arguments, including nulls for default parameters
            // All generated functions are constructed with a delegate that accepts the scope array
            // as the first parameter, so calls must always use the array-based Invoke overloads.
            _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
            if (expectedParamCount <= 6)
            {
                _il.Token(_bclReferences.GetFuncInvokeRef(expectedParamCount));
            }
            else
            {
                throw ILEmitHelpers.NotSupported($"Only up to 6 parameters supported currently (expected {expectedParamCount}, got {argCount})", callExpression);
            }
            return null;
        }

        private static Type? ResolveNodeModuleType(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            if (name.StartsWith("node:", StringComparison.OrdinalIgnoreCase))
                name = name.Substring("node:".Length);
            var asm = typeof(JavaScriptRuntime.Object).Assembly;
            foreach (var t in asm.GetTypes())
            {
                if (!string.Equals(t.Namespace, "JavaScriptRuntime.Node", StringComparison.Ordinal)) continue;
                var attribs = t.GetCustomAttributes(typeof(JavaScriptRuntime.Node.NodeModuleAttribute), inherit: false);
                if (attribs.Length == 0) continue;
                var attr = (JavaScriptRuntime.Node.NodeModuleAttribute)attribs[0]!;
                if (string.Equals(attr.Name, name, StringComparison.OrdinalIgnoreCase)) return t;
            }
            return null;
        }

        /// <summary>
        /// Determines which scopes are needed for a specific function call.
        /// Rules:
        /// - In Main (no scope-array parameter): pass the current (leaf) scope instance.
        /// - In a function context (scope-array present): pass only the caller's local scope that holds the callee delegate.
        ///   Historical snapshots do not include the global scope alongside the caller local for nested calls, except in some
        ///   statement contexts where [global, local] is expected.
        /// </summary>
        private IEnumerable<string> GetNeededScopesForFunction(Variable functionVariable, CallSiteContext context)
        {
            var names = _variables.GetAllScopeNames().ToList();
            var slots = names.Select(n => new { Name = n, Slot = _variables.GetScopeLocalSlot(n) }).ToList();

            bool inFunctionContext = slots.Any(e => e.Slot.Location == ObjectReferenceLocation.ScopeArray);
            if (!inFunctionContext)
            {
                // Main: pass only the current (leaf) scope
                var globalName = _variables.GetLeafScopeName();
                yield return globalName;
                yield break;
            }

            // Inside a function: include the global scope first when present, then the local scope owning the callee
            // when it's declared locally. Previous logic attempted to include the parent scope name (prefix) instead
            // of the actual local scope, causing nested functions to receive a scopes[] without the expected local slot.
            // If the callee lives on a parent/global scope already in the incoming scopes[], include only that owning scope.
            if (!string.IsNullOrEmpty(functionVariable.ScopeName))
            {
                var targetSlot = _variables.GetScopeLocalSlot(functionVariable.ScopeName);
                if (targetSlot.Location == ObjectReferenceLocation.Local)
                {
                    // Local declaration: build [global, local] when global exists, else just local.
                    var globalEntry = slots.FirstOrDefault(e => e.Slot.Location == ObjectReferenceLocation.ScopeArray && e.Slot.Address == 0);
                    if (globalEntry != null)
                    {
                        yield return globalEntry.Name; // global first
                    }
                    yield return functionVariable.ScopeName; // include actual local scope owning the callee
                }
                else if (targetSlot.Location == ObjectReferenceLocation.ScopeArray ||
                         targetSlot.Location == ObjectReferenceLocation.Parameter)
                {
                    // Callee lives on a parent/global scope: include only that owning scope
                    yield return functionVariable.ScopeName;
                }
            }
        }

        /// <summary>
        /// Load literal expressions onto the IL stack (number, boolean, string, null via generic literal).
        /// Mirrors previous logic from ILMethodGenerator.
        /// </summary>
        private JavascriptType LoadValue(Expression expression, TypeCoercion typeCoercion)
        {
            var _metadataBuilder = _owner.MetadataBuilder;

            JavascriptType type = JavascriptType.Unknown;

            switch (expression)
            {
                case BooleanLiteral booleanLiteral:
                    if (typeCoercion.toString)
                    {
                        _il.Ldstr(_metadataBuilder, booleanLiteral.Value ? "true" : "false");
                        // treat as object/string in this coercion path
                        type = JavascriptType.Object;
                    }
                    else
                    {
                        _il.LoadConstantI4(booleanLiteral.Value ? 1 : 0); // Load boolean literal
                        type = JavascriptType.Boolean;
                    }
                    break;
                case NumericLiteral numericLiteral:
                    if (typeCoercion.toString)
                    {
                        //does dotnet ToString behave the same as JavaScript?
                        var numberAsString = numericLiteral.Value.ToString();
                        _il.Ldstr(_metadataBuilder, numberAsString); // Load numeric literal as string
                    }
                    else
                    {
                        _il.LoadConstantR8(numericLiteral.Value); // Load numeric literal
                    }

                    type = JavascriptType.Number;

                    break;
                case StringLiteral stringLiteral:
                    _il.Ldstr(_metadataBuilder, stringLiteral.Value); // Load string literal
                    break;
                case Literal genericLiteral:
                    // Some literals (especially booleans/null) may come through the generic Literal node
                    if (genericLiteral.Value is bool b)
                    {
                        if (typeCoercion.toString)
                        {
                            _il.Ldstr(_metadataBuilder, b ? "true" : "false");
                            type = JavascriptType.Object;
                        }
                        else
                        {
                            _il.LoadConstantI4(b ? 1 : 0);
                            type = JavascriptType.Boolean;
                        }
                        break;
                    }
                    if (genericLiteral.Value is null)
                    {
                        // JavaScript 'null' literal → box JavaScriptRuntime.JsNull.Null
                        _il.LoadConstantI4((int)JavaScriptRuntime.JsNull.Null);
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Box);
                        _il.Token(_owner.Runtime.GetRuntimeTypeHandle(typeof(JavaScriptRuntime.JsNull)));
                        type = JavascriptType.Null;
                        break;
                    }
                    throw ILEmitHelpers.NotSupported($"Unsupported literal value type: {genericLiteral.Value?.GetType().Name ?? "null"}", genericLiteral);
                default:
                    throw ILEmitHelpers.NotSupported($"Unsupported expression type: {expression.Type}", expression);
            }

            return type;
        }

        // Emit an assignment expression and return its JavaScript type
        private JavascriptType EmitAssignment(AssignmentExpression assignmentExpression, TypeCoercion typeCoercion)
        {
            var _runtime = _owner.Runtime;
            var _classRegistry = _owner.ClassRegistry;

            // Support assignments to identifiers and to this.property within class instance methods
            if (assignmentExpression.Left is Identifier aid)
            {
                var variable = _variables.FindVariable(aid.Name) ?? throw new InvalidOperationException($"Variable '{aid.Name}' not found");
                
                // Guard: const reassignment attempts throw at runtime
                // Check the specific scope where the variable is defined
                if (IsConstBinding(variable.ScopeName, aid.Name))
                {
                    var ctor = _runtime.GetErrorCtorRef("TypeError", 1);
                    _il.EmitThrowError(_owner.MetadataBuilder, ctor, "Assignment to constant variable.");
                    return JavascriptType.Unknown; // unreachable
                }

                // Check if this is an uncaptured local variable (uses IL local, not scope field)
                bool isLocalVariable = variable.LocalSlot >= 0;
                
                // Load the appropriate scope instance that holds this field (only for field variables)
                ScopeObjectReference scopeSlot = default;
                if (!isLocalVariable)
                {
                    scopeSlot = _variables.GetScopeLocalSlot(variable.ScopeName);
                    if (scopeSlot.Address == -1)
                    {
                        throw new InvalidOperationException($"Scope '{variable.ScopeName}' not found in local slots");
                    }
                }
                
                // Determine if this is a compound assignment (e.g., +=, |=, &=)
                var opName = assignmentExpression.Operator.ToString();

                if (string.Equals(opName, "AdditionAssignment", StringComparison.Ordinal))
                {
                    // Pattern: target = target + <rhs> using JS semantics via Operators.Add
                    // Load current value
                    ILEmitHelpers.EmitLoadVariable(_il, variable, _variables, _owner.BclReferences);

                    // Compute RHS as boxed object
                    var prevAssignment = _owner.CurrentAssignmentTarget;
                    _owner.CurrentAssignmentTarget = aid.Name;
                    _ = Emit(assignmentExpression.Right, new TypeCoercion { boxResult = true });
                    _owner.CurrentAssignmentTarget = prevAssignment;

                    // Apply JS '+' semantics
                    _owner.Runtime.InvokeOperatorsAdd();

                    // Store back
                    ILEmitHelpers.EmitStoreVariable(_il, variable, _variables, scopeAlreadyLoaded: false);

                    // Resulting type after '+=' is dynamic; assume object, but hint CLR string for string appends
                    variable.Type = JavascriptType.Object;
                    if (variable.ClrType == typeof(string))
                    {
                        // keep as string
                    }
                    else
                    {
                        // If RHS is a string literal or known CLR string, mark as string
                        // (lightweight heuristic to aid subsequent string dispatch)
                        try
                        {
                            //if (assignmentExpression.Right is Acornima.Ast.StringLiteral)
                                // variable.ClrType = typeof(string);
                        }
                        catch { /* best-effort */ }
                    }
                    return JavascriptType.Object;
                }
                else if (TryEmitBitwiseCompoundAssignment(opName, assignmentExpression, variable, scopeSlot, aid.Name))
                {
                    // Bitwise compound assignments (|=, &=, ^=, <<=, >>=, >>>=) handled
                    variable.Type = JavascriptType.Number;
                    return JavascriptType.Number;
                }
                else if (TryEmitArithmeticCompoundAssignment(opName, assignmentExpression, variable, scopeSlot, aid.Name))
                {
                    // Arithmetic compound assignments (-=, *=, /=, %=, **=) handled
                    variable.Type = JavascriptType.Number;
                    return JavascriptType.Number;
                }
                else
                {
                    // Simple assignment '='
                    // Check if variable is a field (not uncaptured local)
                    bool isFieldVariable = variable.LocalSlot < 0;
                    
                    // For field variables, load scope instance first (before evaluating RHS)
                    if (isFieldVariable)
                    {
                        EmitLoadScopeObjectTyped(scopeSlot, variable.ScopeName);
                    }
                    
                    var prevAssignment = _owner.CurrentAssignmentTarget;
                    _owner.CurrentAssignmentTarget = aid.Name;
                    // Always box the RHS so storing into an object-typed field is verifiable and consistent
                    var rhsResult = Emit(assignmentExpression.Right, new TypeCoercion { boxResult = true });
                    _owner.CurrentAssignmentTarget = prevAssignment;
                    variable.Type = rhsResult.JsType;
                    //variable.ClrType = rhsResult.ClrType;
                    
                    // Store to variable (scope already loaded for fields, not needed for locals)
                    ILEmitHelpers.EmitStoreVariable(_il, variable, _variables, scopeAlreadyLoaded: isFieldVariable);
                    
                    return rhsResult.JsType;
                }
            }
            else if (_owner.InClassMethod && assignmentExpression.Left is MemberExpression me && me.Object is ThisExpression && !me.Computed && me.Property is Identifier pid)
            {
                // this.prop = <expr>
                _il.OpCode(ILOpCode.Ldarg_0); // load 'this'
                var rhsType = Emit(assignmentExpression.Right, new TypeCoercion() { boxResult = true }).JsType;
                // Lookup field by current class name
                if (string.IsNullOrEmpty(_owner.CurrentClassName) || !_classRegistry.TryGetField(_owner.CurrentClassName!, pid.Name, out var fieldHandle))
                {
                    throw ILEmitHelpers.NotSupported($"Unknown field '{pid.Name}' on class '{_owner.CurrentClassName}'", pid);
                }
                _il.OpCode(ILOpCode.Stfld);
                _il.Token(fieldHandle);
                return JavascriptType.Object;
            }
            else if (assignmentExpression.Left is MemberExpression aindex && aindex.Computed)
            {
                // Support indexed element assignment: target[index] = value for Int32Array.
                // Evaluate receiver once and capture result/type info
                var baseRes = Emit(aindex.Object, new TypeCoercion() { boxResult = false });
                bool isKnownInt32 = baseRes.ClrType == typeof(JavaScriptRuntime.Int32Array);
                var opName = assignmentExpression.Operator.ToString();
                
                if (isKnownInt32)
                {
                    // Store base (instance) so we don't double-evaluate side effects
                    int instLocal = _owner.Variables.AllocateBlockScopeLocal($"IdxAssign_Instance_L{assignmentExpression.Location.Start.Line}C{assignmentExpression.Location.Start.Column}");
                    _il.StoreLocal(instLocal);

                    // Evaluate index; if not already a JS number, unbox to double (boxed numeric) then convert to int32
                    var idxExpr = aindex.Property;
                    var idxJsType = Emit(idxExpr, new TypeCoercion() { boxResult = false }).JsType;
                    if (idxJsType != JavascriptType.Number)
                    {
                        bool likelyBoxedNumeric = idxExpr is Identifier || idxExpr is MemberExpression || idxExpr is ThisExpression;
                        if (likelyBoxedNumeric)
                        {
                            _il.OpCode(ILOpCode.Unbox_any);
                            _il.Token(_owner.BclReferences.DoubleType);
                        }
                    }
                    _il.OpCode(ILOpCode.Conv_i4);
                    int idxLocal = _owner.Variables.AllocateBlockScopeLocal($"IdxAssign_Index_L{assignmentExpression.Location.Start.Line}C{assignmentExpression.Location.Start.Column}");
                    _il.OpCode(ILOpCode.Box); _il.Token(_owner.BclReferences.Int32Type); _il.StoreLocal(idxLocal);

                    // Check if this is a compound assignment
                    ILOpCode? bitwiseOp = GetCompoundBitwiseOpCode(opName);

                    if (bitwiseOp != null)
                    {
                        // Compound bitwise assignment: array[index] op= value
                        // Get current value: instance.get_Item(index)
                        _il.LoadLocal(instLocal);
                        _il.LoadLocal(idxLocal); // index as object (boxed)
                        var getItemRef = _owner.Runtime.GetInstanceMethodRef(typeof(JavaScriptRuntime.Int32Array), "get_Item", typeof(object), typeof(object));
                        _il.OpCode(ILOpCode.Callvirt); _il.Token(getItemRef);
                        // Result is boxed double, convert to int32 for bitwise op
                        _il.OpCode(ILOpCode.Unbox_any); _il.Token(_owner.BclReferences.DoubleType);
                        _il.OpCode(ILOpCode.Conv_i4);
                        // Current value (int32) is on stack

                        // Evaluate RHS and convert to int32
                        _ = Emit(assignmentExpression.Right, new TypeCoercion() { boxResult = false });
                        _il.OpCode(ILOpCode.Conv_i4);

                        // Apply bitwise operation
                        _il.OpCode(bitwiseOp.Value);

                        // Store result: instance.set_Item(index, value)
                        int valLocal = _owner.Variables.AllocateBlockScopeLocal($"IdxAssign_Value_L{assignmentExpression.Location.Start.Line}C{assignmentExpression.Location.Start.Column}");
                        _il.OpCode(ILOpCode.Box); _il.Token(_owner.BclReferences.Int32Type); _il.StoreLocal(valLocal);

                        _il.LoadLocal(instLocal);
                        _il.LoadLocal(idxLocal); // index as object (boxed)
                        _il.LoadLocal(valLocal); // value as object (boxed)
                        var setItemRef = _owner.Runtime.GetInstanceMethodRef(typeof(JavaScriptRuntime.Int32Array), "set_Item", typeof(void), typeof(object), typeof(object));
                        _il.OpCode(ILOpCode.Callvirt); _il.Token(setItemRef);
                        return JavascriptType.Number;
                    }

                    // Simple assignment: Evaluate RHS -> int32 then store boxed value (for result)
                    _ = Emit(assignmentExpression.Right, new TypeCoercion() { boxResult = false });
                    _il.OpCode(ILOpCode.Conv_i4);
                    int valLocal2 = _owner.Variables.AllocateBlockScopeLocal($"IdxAssign_Value_L{assignmentExpression.Location.Start.Line}C{assignmentExpression.Location.Start.Column}");
                    _il.OpCode(ILOpCode.Box); _il.Token(_owner.BclReferences.Int32Type); _il.StoreLocal(valLocal2);

                    // Call instance.set_Item(index,value)
                    _il.LoadLocal(instLocal);                           // instance
                    _il.LoadLocal(idxLocal); // index as object (boxed)
                    _il.LoadLocal(valLocal2); // value as object (boxed)
                    var setItemRef2 = _owner.Runtime.GetInstanceMethodRef(typeof(JavaScriptRuntime.Int32Array), "set_Item", typeof(void), typeof(object), typeof(object));
                    _il.OpCode(ILOpCode.Callvirt); _il.Token(setItemRef2);
                    // Do not leave the assigned value on the stack here; expression statement
                    // cleanup did not recognize this specialized fast-path and caused a residual
                    // stack value (leading to InvalidProgramException). For now we drop it; when
                    // assignment expression values are consumed (e.g. chained or returned) we can
                    // re-introduce a conditional emission that preserves the value.
                    return JavascriptType.Number; // semantic placeholder (value not actually on stack)
                }
                // Dynamic fallback for unknown receiver types: handle assignment (including compound operations)
                // We already evaluated the receiver once (value currently on stack). If its emission had side-effects we can't re-run blindly.
                // Strategy: store first evaluation into a temp local, then reuse.
                // Receiver value currently on stack: store into temp local for reuse
                int recvLocal = _owner.Variables.AllocateBlockScopeLocal($"IdxAssign_DynRecv_L{assignmentExpression.Location.Start.Line}C{assignmentExpression.Location.Start.Column}");
                _il.StoreLocal(recvLocal);
                
                // Check if this is a compound bitwise assignment
                ILOpCode? compoundOp = GetCompoundBitwiseOpCode(opName);
                
                if (compoundOp != null)
                {
                    // Compound bitwise operation: receiver[index] op= value
                    // Get current value: Object.GetItem(receiver, index)
                    _il.LoadLocal(recvLocal); // receiver
                    Emit(aindex.Property, new TypeCoercion() { boxResult = true }); // index (boxed)
                    int idxLocal = _owner.Variables.AllocateBlockScopeLocal($"CompoundIdx_L{assignmentExpression.Location.Start.Line}");
                    _il.StoreLocal(idxLocal);
                    _il.LoadLocal(idxLocal);
                    var getItemRef = _owner.Runtime.GetStaticMethodRef(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.GetItem),
                        typeof(object),
                        typeof(object),
                        typeof(object));
                    _il.OpCode(ILOpCode.Call);
                    _il.Token(getItemRef);
                    // Current value is boxed object, safely convert to int32 using CoerceToInt32
                    var coerceToInt32Ref = _owner.Runtime.GetStaticMethodRef(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.CoerceToInt32),
                        typeof(int),
                        typeof(object));
                    _il.OpCode(ILOpCode.Call);
                    _il.Token(coerceToInt32Ref);
                    
                    // Evaluate RHS and convert to int32
                    // Use CoerceToInt32 to safely handle boxed values from scope fields
                    _ = Emit(assignmentExpression.Right, new TypeCoercion() { boxResult = true });
                    var coerceRhsRef = _owner.Runtime.GetStaticMethodRef(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.CoerceToInt32),
                        typeof(int),
                        typeof(object));
                    _il.OpCode(ILOpCode.Call);
                    _il.Token(coerceRhsRef);
                    
                    // Apply bitwise operation
                    _il.OpCode(compoundOp.Value);
                    
                    // Box result as int32 for AssignItem
                    _il.OpCode(ILOpCode.Box); _il.Token(_owner.BclReferences.Int32Type);
                    int valLocal = _owner.Variables.AllocateBlockScopeLocal($"CompoundVal_L{assignmentExpression.Location.Start.Line}");
                    _il.StoreLocal(valLocal);
                    
                    // Call AssignItem(receiver, index, value)
                    _il.LoadLocal(recvLocal);
                    _il.LoadLocal(idxLocal);
                    _il.LoadLocal(valLocal);
                    var assignItemRef = _owner.Runtime.GetStaticMethodRef(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.AssignItem),
                        typeof(object),
                        typeof(object),
                        typeof(object),
                        typeof(object));
                    _il.OpCode(ILOpCode.Call);
                    _il.Token(assignItemRef);
                    _il.OpCode(ILOpCode.Pop);
                    return JavascriptType.Number;
                }
                
                // Simple assignment: call JavaScriptRuntime.Object.AssignItem(receiver, index, value)
                _il.LoadLocal(recvLocal); // receiver
                var idxType = Emit(aindex.Property, new TypeCoercion() { boxResult = true }); // index (boxed)
                var rhsType = Emit(assignmentExpression.Right, new TypeCoercion() { boxResult = true }); // value (boxed)
                // Correct method has signature object? AssignItem(object receiver, object index, object value)
                // Ensure we request method with 3 parameters (object receiver, object index, object value)
                var assignItemRef2 = _owner.Runtime.GetStaticMethodRef(
                    typeof(JavaScriptRuntime.Object),
                    nameof(JavaScriptRuntime.Object.AssignItem),
                    typeof(object), // return type
                    typeof(object), // receiver
                    typeof(object), // index
                    typeof(object)); // value
                _il.OpCode(ILOpCode.Call);
                _il.Token(assignItemRef2);
                // Ignore returned assigned value for statement contexts
                _il.OpCode(ILOpCode.Pop);
                return JavascriptType.Object;
            }
        else if (assignmentExpression.Left is MemberExpression mex && !mex.Computed && mex.Property is Identifier propId2 && propId2.Name == "exitCode")
            {
                // <base>.exitCode = <expr> ; if base is Process
                var baseRes = Emit(mex.Object, new TypeCoercion());
                if (baseRes.ClrType == typeof(JavaScriptRuntime.Node.Process))
                {
            // Compute RHS as a JavaScript number (double)
            var rhsType = Emit(assignmentExpression.Right, new TypeCoercion() { boxResult = false }).JsType;
            // Ensure numeric argument is a double (Conv_r8 handles ints/booleans to number)
            _il.OpCode(System.Reflection.Metadata.ILOpCode.Conv_r8);
            var setExit = _runtime.GetInstanceMethodRef(typeof(JavaScriptRuntime.Node.Process), "set_exitCode", typeof(void), typeof(double));
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
                    _il.Token(setExit);
                    return JavascriptType.Number;
                }
                throw ILEmitHelpers.NotSupported("Assignment to property 'exitCode' is only supported on process object", assignmentExpression.Left);
            }
            else if (assignmentExpression.Left is MemberExpression mprop && !mprop.Computed && mprop.Property is Identifier pid2)
            {
                // Dynamic property assignment: <base>.prop = <expr>
                // Evaluate base; leave receiver on stack
                _ = Emit(mprop.Object, new TypeCoercion() { boxResult = false });
                // Push property name and RHS (boxed) and call setter helper
                _il.Ldstr(_owner.MetadataBuilder, pid2.Name);
                _ = Emit(assignmentExpression.Right, new TypeCoercion() { boxResult = true });
                var setPropRef = _owner.Runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.SetProperty), typeof(object), typeof(object), typeof(string), typeof(object));
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Call);
                _il.Token(setPropRef);
                // Drop the returned assigned value in statement contexts
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Pop);
                return JavascriptType.Object;
            }
            else
            {
                throw ILEmitHelpers.NotSupported($"Unsupported assignment target type: {assignmentExpression.Left.Type}", assignmentExpression.Left);
            }
        }

        /// <summary>
        /// Attempts to emit bitwise compound assignment operators (|=, &=, ^=, <<=, >>=, >>>=).
        /// Returns true if the operator was handled, false otherwise.
        /// </summary>
        private bool TryEmitBitwiseCompoundAssignment(string opName, AssignmentExpression assignmentExpression, 
            Variable variable, ScopeObjectReference scopeSlot, string variableName)
        {
            ILOpCode? bitwiseOp = opName switch
            {
                "BitwiseOrAssignment" => ILOpCode.Or,
                "BitwiseAndAssignment" => ILOpCode.And,
                "BitwiseXorAssignment" => ILOpCode.Xor,
                "LeftShiftAssignment" => ILOpCode.Shl,
                "RightShiftAssignment" => ILOpCode.Shr,
                "UnsignedRightShiftAssignment" => ILOpCode.Shr_un,
                _ => null
            };

            if (bitwiseOp == null)
                return false;

            // Pattern: target = target <op> rhs
            // For bitwise operations, convert operands to int32, apply operation, convert back to double

            // Load current value
            ILEmitHelpers.EmitLoadVariable(_il, variable, _variables, _owner.BclReferences);

            // Convert LHS to int32 (unbox if needed, then convert to int32)
            _il.OpCode(ILOpCode.Unbox_any);
            _il.Token(_owner.BclReferences.DoubleType);
            _il.OpCode(ILOpCode.Conv_i4);

            // Compute RHS and convert to int32
            var prevAssignment = _owner.CurrentAssignmentTarget;
            _owner.CurrentAssignmentTarget = variableName;
            var rhsResult = Emit(assignmentExpression.Right, new TypeCoercion { boxResult = false });
            _owner.CurrentAssignmentTarget = prevAssignment;
            
            // Ensure RHS is unboxed and converted to int32
            if (rhsResult.JsType == JavascriptType.Number)
            {
                // Already a number (double), just convert to int32
                _il.OpCode(ILOpCode.Conv_i4);
            }
            else if (rhsResult.JsType == JavascriptType.Boolean)
            {
                // Unbox boolean and convert to int32
                _il.OpCode(ILOpCode.Unbox_any);
                _il.Token(_owner.BclReferences.BooleanType);
                _il.OpCode(ILOpCode.Conv_i4);
            }
            else
            {
                // Slow path: boxed value, unbox to double then convert to int32
                _il.OpCode(ILOpCode.Unbox_any);
                _il.Token(_owner.BclReferences.DoubleType);
                _il.OpCode(ILOpCode.Conv_i4);
            }

            // Apply bitwise operation
            _il.OpCode(bitwiseOp.Value);

            // Convert result back to double and box
            _il.OpCode(ILOpCode.Conv_r8);
            if (variable.ClrType != typeof(double))
            {
                _il.OpCode(ILOpCode.Box);
                _il.Token(_owner.BclReferences.DoubleType);
            }

            // Store back
            ILEmitHelpers.EmitStoreVariable(_il, variable, _variables, scopeAlreadyLoaded: false);

            return true;
        }

        /// <summary>
        /// Attempts to emit arithmetic compound assignment operators (-=, *=, /=, %=, **=).
        /// Returns true if the operator was handled, false otherwise.
        /// </summary>
        private bool TryEmitArithmeticCompoundAssignment(string opName, AssignmentExpression assignmentExpression,
            Variable variable, ScopeObjectReference scopeSlot, string variableName)
        {
            ILOpCode? arithmeticOp = opName switch
            {
                "SubtractionAssignment" => ILOpCode.Sub,
                "MultiplicationAssignment" => ILOpCode.Mul,
                "DivisionAssignment" => ILOpCode.Div,
                "RemainderAssignment" => ILOpCode.Rem,
                _ => null
            };

            if (arithmeticOp == null)
            {
                // Check for exponentiation separately as it requires a method call
                if (string.Equals(opName, "ExponentiationAssignment", StringComparison.Ordinal))
                {
                    // Pattern: target = Math.Pow(target, rhs)
                    ILEmitHelpers.EmitLoadVariable(_il, variable, _variables, _owner.BclReferences);
                    _il.OpCode(ILOpCode.Unbox_any);
                    _il.Token(_owner.BclReferences.DoubleType);

                    var prevAssignment = _owner.CurrentAssignmentTarget;
                    _owner.CurrentAssignmentTarget = variableName;
                    _ = Emit(assignmentExpression.Right, new TypeCoercion { boxResult = false });
                    _owner.CurrentAssignmentTarget = prevAssignment;
                    _il.OpCode(ILOpCode.Conv_r8);

                    // Create method signature: double Pow(double, double)
                    var powSig = new BlobBuilder();
                    new BlobEncoder(powSig)
                        .MethodSignature(isInstanceMethod: false)
                        .Parameters(2,
                            returnType => returnType.Type().Double(),
                            parameters =>
                            {
                                parameters.AddParameter().Type().Double();
                                parameters.AddParameter().Type().Double();
                            });
                    var powMethodSig = _owner.MetadataBuilder.GetOrAddBlob(powSig);

                    // Add a MemberRef to Math.Pow(double, double)
                    var mathPowMethodRef = _owner.MetadataBuilder.AddMemberReference(
                        _owner.BclReferences.SystemMathType,
                        _owner.MetadataBuilder.GetOrAddString("Pow"),
                        powMethodSig);

                    _il.OpCode(ILOpCode.Call);
                    _il.Token(mathPowMethodRef);

                    _il.OpCode(ILOpCode.Box);
                    _il.Token(_owner.BclReferences.DoubleType);
                    ILEmitHelpers.EmitStoreVariable(_il, variable, _variables, scopeAlreadyLoaded: false);
                    return true;
                }
                return false;
            }

            // Pattern: target = target <op> rhs
            // Convert to double, apply operation, box result

            // Load current value
            ILEmitHelpers.EmitLoadVariable(_il, variable, _variables, _owner.BclReferences);

            // Convert LHS to double (unbox if needed)
            _il.OpCode(ILOpCode.Unbox_any);
            _il.Token(_owner.BclReferences.DoubleType);

            // Compute RHS and convert to double
            var prevAssignment2 = _owner.CurrentAssignmentTarget;
            _owner.CurrentAssignmentTarget = variableName;
            _ = Emit(assignmentExpression.Right, new TypeCoercion { boxResult = false });
            _owner.CurrentAssignmentTarget = prevAssignment2;
            _il.OpCode(ILOpCode.Conv_r8);

            // Apply arithmetic operation
            _il.OpCode(arithmeticOp.Value);

            // Box result
            _il.OpCode(ILOpCode.Box);
            _il.Token(_owner.BclReferences.DoubleType);

            // Store back
            ILEmitHelpers.EmitStoreVariable(_il, variable, _variables, scopeAlreadyLoaded: false);

            return true;
        }

        // Helper to emit a UnaryExpression and return its JavaScript type (or Unknown when control-flow handled)
        private JavascriptType EmitUnaryExpression(UnaryExpression unaryExpression, TypeCoercion typeCoercion, ConditionalBranching? branching)
        {
            var _bclReferences = _owner.BclReferences;
            var _runtime = _owner.Runtime;

            // Support logical not: !expr and simple unary negation for numeric literals
            var op = unaryExpression.Operator;
            if (op == Acornima.Operator.LogicalNot)
            {
                // If we're in a conditional context, invert the branch directly: if (!x) ... => branch on x == false
                if (branching != null)
                {
                    var argType = Emit(unaryExpression.Argument, new TypeCoercion() { boxResult = false }).JsType;

                    if (argType == JavascriptType.Boolean)
                    {
                        // Only unbox when the argument is a variable/field access (boxed at load time).
                        // Calls and comparisons typically yield a raw bool already on the stack.
                        bool argIsBoxedSource = unaryExpression.Argument is Identifier
                            || unaryExpression.Argument is MemberExpression
                            || unaryExpression.Argument is ThisExpression;
                        if (argIsBoxedSource)
                        {
                            EmitUnboxBoolIfLoadedFromBoxedSource(unaryExpression.Argument);
                        }
                        // Brfalse => when arg is false, jump to BranchOnTrue (since !arg is true)
                        _il.Branch(System.Reflection.Metadata.ILOpCode.Brfalse, branching.BranchOnTrue);
                    }
                    else if (argType == JavascriptType.Number)
                    {
                        // ToBoolean(number): 0 => false; so Brfalse when number == 0. Compare to 0 and branch on equality.
                        _il.LoadConstantR8(0);
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ceq);
                        _il.Branch(System.Reflection.Metadata.ILOpCode.Brtrue, branching.BranchOnTrue);
                    }
                    else
                    {
                        // General truthiness for objects: ToBoolean(object) then branch
                        var toBoolRef = _owner.Runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.ToBoolean), typeof(bool), typeof(object));
                        // Ensure object on stack
                        EmitBoxIfNeeded(argType);
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Call);
                        _il.Token(toBoolRef);
                        _il.Branch(System.Reflection.Metadata.ILOpCode.Brfalse, branching.BranchOnTrue);
                    }

                    if (branching.BranchOnFalse.HasValue)
                    {
                        _il.Branch(System.Reflection.Metadata.ILOpCode.Br, branching.BranchOnFalse.Value);
                    }

                    return JavascriptType.Unknown;
                }
                else
                {
                    // Non-branching context: compute the boolean value and invert it on the stack.
                    var argType = Emit(unaryExpression.Argument, new TypeCoercion() { boxResult = false }).JsType;

                    if (argType == JavascriptType.Boolean)
                    {
                        // Avoid unboxing raw bool results (e.g., from calls). Only unbox when loaded from variables/fields.
                        bool argIsBoxedSource = unaryExpression.Argument is Identifier
                            || unaryExpression.Argument is MemberExpression
                            || unaryExpression.Argument is ThisExpression;
                        if (argIsBoxedSource)
                        {
                            EmitUnboxBoolIfLoadedFromBoxedSource(unaryExpression.Argument);
                        }
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldc_i4_0);
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ceq);
                    }
                    else if (argType == JavascriptType.Number)
                    {
                        _il.LoadConstantR8(0);
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ceq);
                    }
                    else
                    {
                        // General truthiness via ToBoolean(object), then invert
                        var toBoolRef = _owner.Runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.ToBoolean), typeof(bool), typeof(object));
                        EmitBoxIfNeeded(argType);
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Call);
                        _il.Token(toBoolRef);
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldc_i4_0);
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ceq);
                    }

                    return JavascriptType.Boolean;
                }
            }
            else if (op == Acornima.Operator.TypeOf)
            {
                // Emit typeof: evaluate argument (boxed), then call JavaScriptRuntime.TypeUtilities.Typeof(object)
                var _ = Emit(unaryExpression.Argument, new TypeCoercion() { boxResult = true });
                var mref = _owner.Runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.Typeof), typeof(string), typeof(object));
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Call);
                _il.Token(mref);
                return JavascriptType.Object; // string
            }
            else if (op == Acornima.Operator.UnaryNegation)
            {
                // General unary negation: coerce argument to number and negate
                if (unaryExpression.Argument is Acornima.Ast.NumericLiteral numericArg)
                {
                    if (typeCoercion.toString)
                    {
                        var numberAsString = (-numericArg.Value).ToString();
                        _il.Ldstr(_owner.MetadataBuilder, numberAsString);
                        return JavascriptType.Object; // string
                    }
                    else
                    {
                        _il.LoadConstantR8(-numericArg.Value);
                        if (typeCoercion.boxResult)
                        {
                            _il.OpCode(System.Reflection.Metadata.ILOpCode.Box);
                            _il.Token(_owner.BclReferences.DoubleType);
                            return JavascriptType.Object;
                        }
                        return JavascriptType.Number;
                    }
                }

                // Evaluate argument; if not already a number, box and convert using TypeUtilities.ToNumber(object)
                var argJsType = Emit(unaryExpression.Argument, new TypeCoercion() { boxResult = false }).JsType;
                if (argJsType != JavascriptType.Number)
                {
                    EmitBoxIfNeeded(argJsType);
                    var toNumRef = _owner.Runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.ToNumber), typeof(double), typeof(object));
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Call);
                    _il.Token(toNumRef);
                }
                // Negate the double on the stack
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Neg);
                if (typeCoercion.boxResult)
                {
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Box);
                    _il.Token(_owner.BclReferences.DoubleType);
                    return JavascriptType.Object;
                }
                return JavascriptType.Number;
            }
            else
            {
                throw ILEmitHelpers.NotSupported($"Unsupported unary operator: {op}", unaryExpression);
            }
        }

        // Helper to emit a NewExpression and return both JavaScript and CLR types (moved from ILMethodGenerator)
        private ExpressionResult EmitNewExpression(NewExpression newExpression)
        {
            var _classRegistry = _owner.ClassRegistry;
            var _runtime = _owner.Runtime;

            // Support `new Identifier(...)` for classes emitted under Classes namespace
            if (newExpression.Callee is Identifier cid)
            {
                // General path: if Identifier maps to a JavaScriptRuntime intrinsic via IntrinsicObjectRegistry
                // and it has a compatible constructor, emit that instead of hardcoding specific names.
                var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(cid.Name);
                if (intrinsicType != null)
                {
                    // Ignore static classes or non-constructible types
                    bool isStaticClass = intrinsicType.IsAbstract && intrinsicType.IsSealed;
                    if (!isStaticClass)
                    {
                        var argc = newExpression.Arguments.Count;
                        // Support common ctor shapes: .ctor() and .ctor(object)
                        if (argc == 0)
                        {
                            var hasDefault = intrinsicType.GetConstructor(Type.EmptyTypes) != null;
                            if (hasDefault)
                            {
                                var ctorRef = _owner.Runtime.GetConstructorRef(intrinsicType, System.Array.Empty<Type>());
                                _il.OpCode(System.Reflection.Metadata.ILOpCode.Newobj);
                                _il.Token(ctorRef);
                                return new ExpressionResult { JsType = JavascriptType.Object, ClrType = intrinsicType };
                            }
                        }
                        else if (argc == 1)
                        {
                            var hasObjectCtor = intrinsicType.GetConstructor(new[] { typeof(object) }) != null;
                            if (hasObjectCtor)
                            {
                                // Push the single argument boxed
                                Emit(newExpression.Arguments[0], new TypeCoercion() { boxResult = true });
                                var ctorRef = _owner.Runtime.GetConstructorRef(intrinsicType, typeof(object));
                                _il.OpCode(System.Reflection.Metadata.ILOpCode.Newobj);
                                _il.Token(ctorRef);
                                return new ExpressionResult { JsType = JavascriptType.Object, ClrType = intrinsicType };
                            }
                        }
                        else if (argc == 2)
                        {
                            // Allow two-argument intrinsic constructors when available (e.g., RegExp(pattern, flags))
                            var hasTwoObjectCtor = intrinsicType.GetConstructor(new[] { typeof(object), typeof(object) }) != null;
                            if (hasTwoObjectCtor)
                            {
                                Emit(newExpression.Arguments[0], new TypeCoercion() { boxResult = true });
                                Emit(newExpression.Arguments[1], new TypeCoercion() { boxResult = true });
                                var ctorRef = _owner.Runtime.GetConstructorRef(intrinsicType, typeof(object), typeof(object));
                                _il.OpCode(System.Reflection.Metadata.ILOpCode.Newobj);
                                _il.Token(ctorRef);
                                return new ExpressionResult { JsType = JavascriptType.Object, ClrType = intrinsicType };
                            }
                        }
                        // If no compatible ctor found, fall through to class registry / Error handling
                    }
                }

                // Try Classes registry first (cached ctor + signature + param count)
                if (_classRegistry.TryGetConstructor(cid.Name, out var ctorDef, out var minParamCount, out var maxParamCount))
                {
                    var argc = newExpression.Arguments.Count;
                    
                    // Check if this class has a _scopes field (meaning it needs parent scope access)
                    bool classNeedsScopes = _classRegistry.TryGetPrivateField(cid.Name, "_scopes", out var _);
                    int expectedMinArgs = classNeedsScopes ? minParamCount - 1 : minParamCount;
                    int expectedMaxArgs = classNeedsScopes ? maxParamCount - 1 : maxParamCount;
                    
                    // With default parameters, call site can provide anywhere from min to max arguments
                    if (argc < expectedMinArgs || argc > expectedMaxArgs)
                    {
                        if (expectedMinArgs == expectedMaxArgs)
                        {
                            throw ILEmitHelpers.NotSupported($"Constructor for class '{cid.Name}' expects {expectedMinArgs} argument(s) but call site has {argc}.", newExpression);
                        }
                        else
                        {
                            throw ILEmitHelpers.NotSupported($"Constructor for class '{cid.Name}' expects {expectedMinArgs}-{expectedMaxArgs} argument(s) but call site has {argc}.", newExpression);
                        }
                    }

                    // Determine scopes to pass (only if class needs them)
                    if (classNeedsScopes)
                    {
                        var scopeNames = DetermineScopesForDelegateCall();
                        EmitScopeArray(scopeNames); // stack: [scopes[]]
                    }

                    // Emit arguments provided at call site
                    for (int i = 0; i < argc; i++)
                    {
                        Emit(newExpression.Arguments[i], new TypeCoercion() { boxResult = true });
                    }
                    
                    // Pad with null for missing optional parameters (those with defaults)
                    int paddingNeeded = expectedMaxArgs - argc;
                    for (int i = 0; i < paddingNeeded; i++)
                    {
                        _il.OpCode(ILOpCode.Ldnull);
                    }
                    
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Newobj);
                    _il.Token(ctorDef);
                    if (!string.IsNullOrEmpty(_owner.CurrentAssignmentTarget))
                    {
                        _owner.RecordVariableToClass(_owner.CurrentAssignmentTarget!, cid.Name);
                    }
                    return new ExpressionResult { JsType = JavascriptType.Object, ClrType = null };
                }
                // No cached constructor found: treat as unsupported (all emitted classes must register constructors).
                if (_classRegistry.TryGet(cid.Name, out var registeredType) && !registeredType.IsNil)
                {
                    throw ILEmitHelpers.NotSupported($"Constructor for class '{cid.Name}' was not cached. All classes must register their constructors.", newExpression);
                }

                // Built-in Error types from JavaScriptRuntime (Error, TypeError, etc.)
                // Build ctor: choose overload by arg count (support 0 or 1(param: string))
                var argc2 = newExpression.Arguments.Count;
                if (argc2 > 1)
                {
                    throw ILEmitHelpers.NotSupported($"Only up to 1 constructor argument supported for built-in Error types (got {argc2})", newExpression);
                }
                var ctorRef2 = _runtime.GetErrorCtorRef(cid.Name, argc2);

                // Push args
                for (int i = 0; i < argc2; i++)
                {
                    Emit(newExpression.Arguments[i], new TypeCoercion() { boxResult = true });
                }
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Newobj);
                _il.Token(ctorRef2);
                // Best-effort map CLR type for known JavaScript error classes
                Type? errorClrType = typeof(JavaScriptRuntime.Object).Assembly.GetType($"JavaScriptRuntime.{cid.Name}");
                return new ExpressionResult { JsType = JavascriptType.Object, ClrType = errorClrType };
            }

            throw ILEmitHelpers.NotSupported($"Unsupported new-expression callee: {newExpression.Callee.Type}", newExpression.Callee);
        }

        // Emits the IL for a member access expression and returns both JS and CLR type when known.
        private ExpressionResult EmitMemberExpression(MemberExpression memberExpression)
        {
            var _runtime = _owner.Runtime;
            var _classRegistry = _owner.ClassRegistry;
            var _metadataBuilder = _owner.MetadataBuilder;

            // Handle private instance fields: this.#name inside class methods
            if (!memberExpression.Computed && memberExpression.Object is ThisExpression && memberExpression.Property is Acornima.Ast.PrivateIdentifier ppid)
            {
                if (_owner.InClassMethod && _owner.CurrentClassName != null && _classRegistry.TryGetPrivateField(_owner.CurrentClassName, ppid.Name, out var privField))
                {
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldarg_0);
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldfld);
                    _il.Token(privField);
                    return new ExpressionResult { JsType = JavascriptType.Object, ClrType = null };
                }
            }

            // Special handling: if the object is a known class identifier and property is an identifier,
            // allow static field/method access without evaluating an instance first.
            if (!memberExpression.Computed && memberExpression.Object is Identifier staticBase && memberExpression.Property is Identifier staticProp)
            {
                if (_classRegistry.TryGet(staticBase.Name, out var typeHandle))
                {
                    // Support static field access: ClassName.prop
                    if (_classRegistry.TryGetStaticField(staticBase.Name, staticProp.Name, out var sfield))
                    {
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldsfld);
                        _il.Token(sfield);
                        return new ExpressionResult { JsType = JavascriptType.Object, ClrType = null };
                    }
                    // Static method invocations are handled in GenerateCallExpression; here we only support fields
                }
            }

            // Evaluate the base object and capture its resolved CLR type (if any)
            var baseResult = Emit(memberExpression.Object, new TypeCoercion());

            if (!memberExpression.Computed && memberExpression.Property is Identifier propId)
            {
                // First, support array.length
                if (propId.Name == "length")
                {
                    // Generic length: arrays, strings, and collections
                    var getLen = _owner.Runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetLength), typeof(double), typeof(object));
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Call);
                    _il.Token(getLen);
                    return new ExpressionResult { JsType = JavascriptType.Number, ClrType = typeof(double) };
                }

                // If the base resolved to a known runtime type, allow direct instance member access

                // New: If base CLR type is known and has a public instance property with this name, emit typed getter
                if (baseResult.ClrType != null)
                {
                    var baseClr = baseResult.ClrType;
                    var pi = baseClr.GetProperty(propId.Name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreCase);
                    if (pi?.GetMethod != null)
                    {
                        // Cast receiver to the declaring CLR type before invoking the getter
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Castclass);
                        _il.Token(_owner.Runtime.GetRuntimeTypeHandle(baseClr));
                        var getterRef = _owner.Runtime.GetInstanceMethodRef(baseClr, pi.GetMethod.Name, pi.PropertyType);
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
                        _il.Token(getterRef);
                        return new ExpressionResult { JsType = JavascriptType.Object, ClrType = pi.PropertyType };
                    }
                }

                // Handle instance field access for known classes: this.field or var field on known class instance
                FieldDefinitionHandle fieldHandle;
                if ((memberExpression.Object is ThisExpression && _owner.InClassMethod && _owner.CurrentClassName != null && _classRegistry.TryGetField(_owner.CurrentClassName, propId.Name, out fieldHandle))
                    || (memberExpression.Object is Identifier baseIdent2 && _owner.TryGetVariableClass(baseIdent2.Name, out var cname) && _classRegistry.TryGetField(cname, propId.Name, out fieldHandle)))
                {
                    // If the base was a variable bound to a known class but loaded as object, cast to class type first
                    if (memberExpression.Object is Identifier idObj && _owner.TryGetVariableClass(idObj.Name, out var className) && _classRegistry.TryGet(className, out var tdef) && !tdef.IsNil)
                    {
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Castclass);
                        _il.Token(tdef);
                    }
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Ldfld);
                    _il.Token(fieldHandle);
                    return new ExpressionResult { JsType = JavascriptType.Object, ClrType = null };
                }

                // Fallback: dynamic property lookup on runtime object graphs (e.g., ExpandoObject from JSON.parse)
                var getProp = _owner.Runtime.GetStaticMethodRef(typeof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetProperty), typeof(object), typeof(object), typeof(string));
                _il.Ldstr(_metadataBuilder, propId.Name);
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Call);
                _il.Token(getProp);
                return new ExpressionResult { JsType = JavascriptType.Object, ClrType = null };
            }

            if (memberExpression.Computed)
            {
                var idxExpr = memberExpression.Property;
                Emit(idxExpr, new TypeCoercion() { boxResult = true });
                _runtime.InvokeGetItemFromObject();
                return new ExpressionResult { JsType = JavascriptType.Object, ClrType = null };
            }

            throw ILEmitHelpers.NotSupported("Only 'length', instance fields on known classes, or computed indexing supported.", memberExpression);
        }

        // Generate an object literal using ExpandoObject semantics
        private void GenerateObjectExpresion(ObjectExpression objectExpression)
        {
            var _bclReferences = _owner.BclReferences;
            var _metadataBuilder = _owner.MetadataBuilder;

            // Create new ExpandoObject
            _il.OpCode(System.Reflection.Metadata.ILOpCode.Newobj);
            _il.Token(_bclReferences.Expando_Ctor_Ref);

            foreach (var property in objectExpression.Properties)
            {
                if (property is not ObjectProperty op)
                {
                    ILEmitHelpers.ThrowNotSupported($"Unsupported object property type: {property.Type}", property);
                }
                else
                {
                    // Determine property name string per JS semantics: Identifier, StringLiteral, or NumericLiteral
                    string? propName = null;
                    if (op.Key is Identifier keyId)
                    {
                        propName = keyId.Name;
                    }
                    else if (op.Key is Literal lit)
                    {
                        switch (lit)
                        {
                            case StringLiteral s:
                                propName = s.Value;
                                break;
                            case NumericLiteral n:
                                propName = n.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                                break;
                            default:
                                ILEmitHelpers.ThrowNotSupported($"Unsupported object property key literal type: {op.Key.Type}", op.Key);
                                break;
                        }
                    }
                    else
                    {
                        ILEmitHelpers.ThrowNotSupported($"Unsupported object property key type: {op.Key.Type}", op.Key);
                    }

                    if (op.Value is not Expression valueExpr)
                    {
                        ILEmitHelpers.ThrowNotSupported($"Unsupported object property value type: {op.Value.Type}", op.Value);
                    }
                    else
                    {
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Dup);
                        _il.Ldstr(_metadataBuilder, propName ?? string.Empty);
                        _ = Emit(valueExpr, new TypeCoercion() { boxResult = true });
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
                        _il.Token(_bclReferences.IDictionary_SetItem_Ref);
                    }
                }
            }
            // Object remains on stack
        }

        // Generate a JavaScript array literal using the runtime Array helper
        private void GenerateArrayExpression(ArrayExpression arrayExpression)
        {
            var _runtime = _owner.Runtime;
            var _bclReferences = _owner.BclReferences;

            // 1) push capacity
            _il.LoadConstantI4(arrayExpression.Elements.Count);
            // 2) invoke runtime array ctor (produces JavaScriptRuntime.Array instance boxed as object)
            _runtime.InvokeArrayCtor();

            // For each element: handle SpreadElement by pushing range; otherwise Add single item
            for (int i = 0; i < arrayExpression.Elements.Count; i++)
            {
                var element = arrayExpression.Elements[i];
                if (element is SpreadElement spread)
                {
                    // Duplicate array ref, evaluate argument (boxed), call PushRange
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Dup);
                    _ = Emit(spread.Argument!, new TypeCoercion() { boxResult = true });
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
                    var pushRange = _owner.Runtime.GetInstanceMethodRef(typeof(JavaScriptRuntime.Array), nameof(JavaScriptRuntime.Array.PushRange), typeof(void), typeof(object));
                    _il.Token(pushRange);
                }
                else
                {
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Dup); // array instance
                    _ = Emit(element!, new TypeCoercion() { boxResult = true });
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
                    _il.Token(_bclReferences.Array_Add_Ref);
                }
            }

            // If this array literal is initializing/assigning a variable, tag it as a runtime JavaScriptRuntime.Array
            if (!string.IsNullOrEmpty(_owner.CurrentAssignmentTarget))
            {
                var v = _variables.FindVariable(_owner.CurrentAssignmentTarget!);
                if (v != null)
                {
                    //v.ClrType = typeof(JavaScriptRuntime.Array);
                }
            }
        }

    // Handle increment/decrement (x++, ++x, x--, --x) on identifiers.
    // Postfix leaves the ORIGINAL value on the stack; prefix leaves UPDATED value.
        private JavascriptType GenerateUpdateExpression(UpdateExpression updateExpression, CallSiteContext context = CallSiteContext.Expression)
        {
            var _bclReferences = _owner.BclReferences;

            if (updateExpression.Operator != Acornima.Operator.Increment && updateExpression.Operator != Acornima.Operator.Decrement)
            {
                ILEmitHelpers.ThrowNotSupported($"Unsupported update expression operator: {updateExpression.Operator}", updateExpression);
            }

            // Handle postfix increment (x++) and decrement (x--)
            var variableName = (updateExpression.Argument as Identifier)!.Name;
            var variable = _variables.FindVariable(variableName);

            if (variable == null)
            {
                throw new InvalidOperationException("Variable reference is null.");
            }

            // If bound variable is const, emit a TypeError and throw
            if (IsConstBinding(variable.ScopeName, variableName))
            {
                var ctor = _owner.Runtime.GetErrorCtorRef("TypeError", 1);
                _il.EmitThrowError(_owner.MetadataBuilder, ctor, "Assignment to constant variable.");
                // Throws; unreachable, but return a value to satisfy signature
                return JavascriptType.Number;
            }
            // For parameters, we don't need a scope slot; handled in the parameter branch below.
            // For uncaptured local variables, we also don't need a scope slot.
            // Only resolve scopeLocalIndex for field-backed variables.
            ScopeObjectReference scopeLocalIndex = new ScopeObjectReference { Location = ObjectReferenceLocation.Local, Address = -1 };
            if (!variable.IsParameter && variable is not LocalVariable)
            {
                scopeLocalIndex = _variables.GetScopeLocalSlot(variable.ScopeName);
                if (scopeLocalIndex.Address == -1)
                {
                    throw new InvalidOperationException($"Scope '{variable.ScopeName}' not found in local slots");
                }
            }

            // Special-case: parameter variables (no backing field when no nested functions). Update via starg.
            if (variable.IsParameter)
            {
                var pindex = variable.ParameterIndex;
                // Load current numeric value (double)
                _il.LoadArgument(pindex);
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Unbox_any);
                _il.Token(_bclReferences.DoubleType); // [cur]

                bool isInc = updateExpression.Operator == Acornima.Operator.Increment;
                if (context == CallSiteContext.Statement)
                {
                    // cur (+/-) 1 -> box -> starg
                    _il.LoadConstantR8(1.0);
                    _il.OpCode(isInc ? System.Reflection.Metadata.ILOpCode.Add : System.Reflection.Metadata.ILOpCode.Sub);
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Box);
                    _il.Token(_bclReferences.DoubleType);
                    _il.StoreArgument(pindex);
                    return JavascriptType.Unknown;
                }
                else
                {
                    if (updateExpression.Prefix)
                    {
                        // (++x or --x): updated value is the result
                        _il.LoadConstantR8(1.0);
                        _il.OpCode(isInc ? System.Reflection.Metadata.ILOpCode.Add : System.Reflection.Metadata.ILOpCode.Sub); // [updated]
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Dup); // [updated, updated]
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Box);
                        _il.Token(_bclReferences.DoubleType); // [updated, boxedUpdated]
                        _il.StoreArgument(pindex); // leaves [updated]
                        return JavascriptType.Number;
                    }
                    else
                    {
                        // (x++ or x--): result is original; store updated
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Dup); // [cur, cur]
                        _il.LoadConstantR8(1.0);
                        _il.OpCode(isInc ? System.Reflection.Metadata.ILOpCode.Add : System.Reflection.Metadata.ILOpCode.Sub); // [cur, updated]
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Box);
                        _il.Token(_bclReferences.DoubleType); // [cur, boxedUpdated]
                        _il.StoreArgument(pindex); // leaves [cur]
                        return JavascriptType.Number;
                    }
                }
            }

            // Field-backed (local or parent scope) variable path
            // Algorithm:
            //   1) Load scope and current value
            //   2) Compute UPDATED = value (+/-) 1 and store back (stfld)
            //   3) Produce result: UPDATED for prefix; ORIGINAL for postfix (by reloading updated and reversing +/- 1)

            if (context == CallSiteContext.Statement)
            {
                // Statement-context: load, update, store
                ILEmitHelpers.EmitLoadVariable(_il, variable, _variables, _owner.BclReferences, unbox: true);
                _il.LoadConstantR8(1.0);
                if (updateExpression.Operator == Acornima.Operator.Increment)
                {
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Add);
                }
                else // Decrement
                {
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Sub);
                }
                ILEmitHelpers.EmitStoreVariable(_il, variable, _variables, scopeAlreadyLoaded: false, valueIsBoxed: false, bclReferences: _owner.BclReferences);
                return JavascriptType.Unknown;
            }
            else
            {
                // Expression-context
                // 1) Load current value
                ILEmitHelpers.EmitLoadVariable(_il, variable, _variables, _owner.BclReferences, unbox: true); // [cur]

                // 2) Compute updated value and store back
                _il.LoadConstantR8(1.0);
                if (updateExpression.Operator == Acornima.Operator.Increment)
                {
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Add);
                }
                else // Decrement
                {
                    _il.OpCode(System.Reflection.Metadata.ILOpCode.Sub);
                }

                ILEmitHelpers.EmitStoreVariable(_il, variable, _variables, scopeAlreadyLoaded: false, valueIsBoxed: true, bclReferences: _owner.BclReferences); // []

                // 3) Reload UPDATED and optionally reverse to get ORIGINAL
                ILEmitHelpers.EmitLoadVariable(_il, variable, _variables, _owner.BclReferences, unbox: true); // [updated]

                if (!updateExpression.Prefix)
                {
                    _il.LoadConstantR8(1.0);
                    if (updateExpression.Operator == Acornima.Operator.Increment)
                    {
                        // original = updated - 1
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Sub);
                    }
                    else // Decrement
                    {
                        // original = updated + 1
                        _il.OpCode(System.Reflection.Metadata.ILOpCode.Add);
                    }
                }
                return JavascriptType.Number;
            }
        }

        // Consult the registry to see if the current scope has a const binding for this name
        private bool IsConstBinding(string scopeName, string variableName)
        {
            var registry = _variables.GetVariableRegistry();
            var info = registry?.GetVariableInfo(scopeName, variableName) ?? registry?.FindVariable(variableName);
            return info != null && info.BindingKind == SymbolTables.BindingKind.Const;
        }

        // Case-insensitive property getter helper for AST reflection
        private static System.Reflection.PropertyInfo? GetPropertyIgnoreCase(object target, string propertyName)
        {
            if (target == null || string.IsNullOrEmpty(propertyName)) return null;
            var t = target.GetType();
            return t.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
        }

        // Parse a Raw regex literal string like "/pattern/flags" into (pattern, flags)
        private static (string? pattern, string? flags) ParseRegexRaw(string raw)
        {
            if (string.IsNullOrEmpty(raw) || raw[0] != '/') return (null, null);
            int lastSlash = -1;
            bool escaped = false;
            for (int i = 1; i < raw.Length; i++)
            {
                char c = raw[i];
                if (!escaped)
                {
                    if (c == '\\') { escaped = true; continue; }
                    if (c == '/') { lastSlash = i; break; }
                }
                else
                {
                    escaped = false;
                }
            }
            if (lastSlash <= 0) return (null, null);
            var pattern = raw.Substring(1, lastSlash - 1);
            var flags = lastSlash + 1 < raw.Length ? raw.Substring(lastSlash + 1) : string.Empty;
            return (pattern, flags);
        }

        private Type? EmitArrayInstanceMethodCall(Expression receiver, string methodName, CallExpression callExpression)
        {
            var _runtime = _owner.Runtime;
            var _bclReferences = _owner.BclReferences;

            // Evaluate receiver expression; if it's already a JavaScriptRuntime.Array, good. Otherwise, attempt to coerce arrays only for simple literals later.
            // For now, we assume upstream code constructs arrays via ArrayExpression which produces JavaScriptRuntime.Array.
            _ = Emit(receiver, new TypeCoercion { boxResult = false });

            var arrayType = typeof(JavaScriptRuntime.Array);
            // Best-effort castclass to JavaScriptRuntime.Array; emit type reference via runtime helper
            /*
            try
            {
                var arrayTypeRef = _runtime.GetRuntimeTypeHandle(arrayType);
                _il.OpCode(System.Reflection.Metadata.ILOpCode.Castclass);
                _il.Token(arrayTypeRef);
            }
            catch
            {
                // If type ref cannot be obtained, skip cast; downstream callvirt may still work for correct instances
            }*/

            // Reflect instance method (map/join/sort)
            var methods = arrayType
                .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                .Where(mi => string.Equals(mi.Name, methodName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (methods.Count == 0)
            {
                ILEmitHelpers.ThrowNotSupported($"Array method not found: {arrayType.FullName}.{methodName}", callExpression);
            }

            // Prefer exact arity match first (e.g., parameterless join())
            var chosen = methods.FirstOrDefault(mi => mi.GetParameters().Length == callExpression.Arguments.Count);

            // If no args at callsite, strongly prefer a true zero-parameter overload when available
            if (chosen == null && callExpression.Arguments.Count == 0)
            {
                chosen = methods.FirstOrDefault(mi => mi.GetParameters().Length == 0);
            }

            // Fall back to params object[] overload when available
            if (chosen == null)
            {
                chosen = methods.FirstOrDefault(mi =>
                {
                    var ps = mi.GetParameters();
                    return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
                });
            }

            if (chosen == null)
            {
                // Fallback to first available (e.g., parameterless join())
                chosen = methods.OrderBy(mi => mi.GetParameters().Length).First();
            }

            var psChosen = chosen.GetParameters();
            var expectsParamsArray = psChosen.Length == 1 && psChosen[0].ParameterType == typeof(object[]);
            var reflectedParamTypes = psChosen.Select(p => p.ParameterType).ToArray();
            var reflectedReturnType = chosen.ReturnType;

            var mrefHandle = _runtime.GetInstanceMethodRef(arrayType, chosen.Name, reflectedReturnType, reflectedParamTypes);

            if (expectsParamsArray) EmitBoxedArgsArray(callExpression.Arguments);
            else EmitBoxedArgsInline(callExpression.Arguments);

            _il.OpCode(System.Reflection.Metadata.ILOpCode.Callvirt);
            _il.Token(mrefHandle);

            return reflectedReturnType;
        }

        /// <summary>
        /// Determines which scopes to pass for delegate calls (arrow functions, function expressions).
        /// If inside a function (global scope array present) and we have a local leaf scope,
        /// pass [global, leaf]; otherwise pass just the leaf/global scope like top-level Main semantics.
        /// </summary>
        private List<string> DetermineScopesForDelegateCall()
        {
            var scopeNames = new List<string>();
            try
            {
                var allNames = _variables.GetAllScopeNames().ToList();
                var slots = allNames.Select(n => new { Name = n, Slot = _variables.GetScopeLocalSlot(n) }).ToList();
                
                // Build the full scope chain from global (index 0) to current scope
                // This ensures nested classes/functions have access to all ancestor scopes
                var orderedSlots = slots
                    .Where(e => e.Slot.Location == ObjectReferenceLocation.ScopeArray)
                    .OrderBy(e => e.Slot.Address)
                    .ToList();
                
                if (orderedSlots.Any())
                {
                    // Include all scopes from the scope array (global through all ancestors)
                    foreach (var entry in orderedSlots)
                    {
                        scopeNames.Add(entry.Name);
                    }
                }
                
                // Include the current local scope only if it was actually instantiated
                // (has a valid local slot >= 0). Empty scopes are not instantiated.
                var leafName = _variables.GetLeafScopeName();
                if (!string.IsNullOrEmpty(leafName) && !scopeNames.Contains(leafName))
                {
                    var leafSlot = _variables.GetScopeLocalSlot(leafName);
                    if (leafSlot.Address >= 0)
                    {
                        scopeNames.Add(leafName);
                    }
                }
                
                // Fallback: if nothing was added and leaf has valid slot, include it
                if (!scopeNames.Any())
                {
                    var leafSlot = _variables.GetScopeLocalSlot(leafName);
                    if (leafSlot.Address >= 0)
                    {
                        scopeNames.Add(leafName);
                    }
                }
            }
            catch
            {
                // Conservative fallback: single leaf scope name if discovery fails and it has valid slot
                scopeNames.Clear();
                var leafName = _variables.GetLeafScopeName();
                var leafSlot = _variables.GetScopeLocalSlot(leafName);
                if (leafSlot.Address >= 0)
                {
                    scopeNames.Add(leafName);
                }
            }
            return scopeNames;
        }
    }
}
