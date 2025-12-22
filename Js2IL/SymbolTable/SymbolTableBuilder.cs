using Acornima.Ast;

namespace Js2IL.SymbolTables
{
    /// <summary>
    /// Builds a SymbolTable from a JavaScript AST.
    /// </summary>
    public partial class SymbolTableBuilder
    {
        public const string DefaultClassesNamespace = "Classes";

        // Track visited function expression nodes to avoid duplicating scopes when traversal
        // reaches the same AST node via multiple paths (explicit handling + reflective walk).
        private readonly HashSet<Node> _visitedArrowFunctions = new();
        private readonly HashSet<Node> _visitedFunctionExpressions = new();

        private int _closureCounter = 0;
        private string? _currentAssignmentTarget = null;

        public void Build(ModuleDefinition module)
        {
            var globalScope = new Scope(module.Name, ScopeKind.Global, null, module.Ast);

            AddModuleBuiltInParameters(globalScope, module.Ast);

            BuildScopeRecursive(globalScope,module.Ast, globalScope);
            AnalyzeFreeVariables(globalScope);
            MarkCapturedVariables(globalScope);
            InferVariableClrTypes(globalScope);
            module.SymbolTable = new SymbolTable(globalScope);
        }

        private void AddModuleBuiltInParameters(Scope globalScope, Node astNode)
        {
            // Add built-in parameters for module system using shared ModuleParameters definition
            foreach (var param in JavaScriptRuntime.CommonJS.ModuleParameters.Parameters)
            {
                var bindingKind = param.IsConst ? BindingKind.Const : BindingKind.Var;
                globalScope.Bindings[param.Name] = new BindingInfo(param.Name, bindingKind, astNode);
                globalScope.Parameters.Add(param.Name);
            }
        }

        /// <summary>
        /// Recursively checks if an AST node contains any identifier that isn't in the local variables set.
        /// Stops at nested function boundaries.
        /// </summary>
        private static bool ContainsFreeVariable(Node? node, HashSet<string> localVariables)
        {
            if (node == null) return false;

            switch (node)
            {
                case Identifier id:
                    // Check if this identifier is not local and not a known global intrinsic
                    return !localVariables.Contains(id.Name) && !IsKnownGlobalIntrinsic(id.Name);

                case VariableDeclaration vd:
                    // Add declared variables to local set for subsequent analysis
                    var newLocals = new HashSet<string>(localVariables);
                    foreach (var decl in vd.Declarations)
                    {
                        if (decl.Id is Identifier vid)
                        {
                            newLocals.Add(vid.Name);
                        }
                    }
                    // Check initializers with updated local set
                    foreach (var decl in vd.Declarations)
                    {
                        if (ContainsFreeVariable(decl.Init, newLocals)) return true;
                    }
                    return false;

                case BlockStatement bs:
                    return bs.Body.Any(stmt => ContainsFreeVariable(stmt, localVariables));

                case ExpressionStatement es:
                    return ContainsFreeVariable(es.Expression, localVariables);

                case ReturnStatement rs:
                    return ContainsFreeVariable(rs.Argument, localVariables);

                case IfStatement ifs:
                    return ContainsFreeVariable(ifs.Test, localVariables) ||
                           ContainsFreeVariable(ifs.Consequent, localVariables) ||
                           ContainsFreeVariable(ifs.Alternate, localVariables);

                case ForStatement fs:
                    return ContainsFreeVariable(fs.Init as Node, localVariables) ||
                           ContainsFreeVariable(fs.Test, localVariables) ||
                           ContainsFreeVariable(fs.Update, localVariables) ||
                           ContainsFreeVariable(fs.Body, localVariables);

                case WhileStatement ws:
                    return ContainsFreeVariable(ws.Test, localVariables) ||
                           ContainsFreeVariable(ws.Body, localVariables);

                case DoWhileStatement dws:
                    return ContainsFreeVariable(dws.Body, localVariables) ||
                           ContainsFreeVariable(dws.Test, localVariables);

                case BinaryExpression be:
                    return ContainsFreeVariable(be.Left, localVariables) ||
                           ContainsFreeVariable(be.Right, localVariables);

                case UpdateExpression upe:
                    return ContainsFreeVariable(upe.Argument, localVariables);

                case UnaryExpression ue:
                    return ContainsFreeVariable(ue.Argument, localVariables);

                case CallExpression ce:
                    return ContainsFreeVariable(ce.Callee, localVariables) ||
                           ce.Arguments.Any(arg => ContainsFreeVariable(arg as Node, localVariables));

                case MemberExpression me:
                    return ContainsFreeVariable(me.Object, localVariables) ||
                           (me.Computed && ContainsFreeVariable(me.Property as Node, localVariables));

                case AssignmentExpression ae:
                    return ContainsFreeVariable(ae.Left, localVariables) ||
                           ContainsFreeVariable(ae.Right, localVariables);

                case ConditionalExpression ce:
                    return ContainsFreeVariable(ce.Test, localVariables) ||
                           ContainsFreeVariable(ce.Consequent, localVariables) ||
                           ContainsFreeVariable(ce.Alternate, localVariables);

                case ArrayExpression arr:
                    return arr.Elements.Any(elem => ContainsFreeVariable(elem as Node, localVariables));

                case ObjectExpression obj:
                    return obj.Properties.Any(prop => ContainsFreeVariable(prop, localVariables));

                case Property prop:
                    return ContainsFreeVariable(prop.Key as Node, localVariables) ||
                           ContainsFreeVariable(prop.Value as Node, localVariables);

                case TemplateLiteral tl:
                    // Template strings: only expressions can reference variables (quasis are raw text)
                    return tl.Expressions.Any(expr => ContainsFreeVariable(expr as Node, localVariables));

                case TaggedTemplateExpression tte:
                    return ContainsFreeVariable(tte.Tag, localVariables) ||
                           ContainsFreeVariable(tte.Quasi, localVariables);

                // Classes: check method bodies for free variable references
                case ClassDeclaration classDecl:
                    // Iterate through class methods and check their bodies
                    foreach (var element in classDecl.Body.Body)
                    {
                        if (element is MethodDefinition mdef && mdef.Value is FunctionExpression mfunc)
                        {
                            // Build local variables set for this method (includes parameters)
                            var methodLocals = new HashSet<string>(localVariables);
                            foreach (var param in mfunc.Params)
                            {
                                if (param is Identifier pid)
                                {
                                    methodLocals.Add(pid.Name);
                                }
                            }
                            
                            // Check method body for free variables
                            if (mfunc.Body is BlockStatement mblock)
                            {
                                foreach (var stmt in mblock.Body)
                                {
                                    if (ContainsFreeVariable(stmt, methodLocals))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        else if (element is PropertyDefinition propDef)
                        {
                            // Check field initializer expressions (instance or static fields)
                            if (propDef.Value != null && ContainsFreeVariable(propDef.Value, localVariables))
                            {
                                return true;
                            }
                        }
                    }
                    return false;

                // Stop at nested function boundaries - they have their own scope
                case FunctionExpression:
                case FunctionDeclaration:
                case ArrowFunctionExpression:
                    return false;

                default:
                    // For unknown node types, conservatively return false
                    return false;
            }
        }

        /// <summary>
        /// Checks if an identifier is a known global intrinsic that doesn't require scope access.
        /// </summary>
        private static bool IsKnownGlobalIntrinsic(string name)
        {
            return name == "console" || name == "Math" || name == "Object" || name == "Array" ||
                   name == "String" || name == "Number" || name == "Boolean" || name == "Date" ||
                   name == "JSON" || name == "undefined" || name == "null" || name == "Infinity" || name == "NaN" ||
                   name == "process" || name == "__dirname" || name == "__filename" || name == "require" ||
                   name == "Buffer" || name == "Int32Array" || name == "Error" || name == "Promise" ||
                   name == "setTimeout" || name == "setInterval" || name == "clearTimeout" || name == "clearInterval" ||
                   name == "setImmediate" || name == "clearImmediate";
        }

        private static string NormalizeModuleName(string s)
        {
            var trimmed = (s ?? string.Empty).Trim();
            if (trimmed.StartsWith("node:", StringComparison.OrdinalIgnoreCase))
                trimmed = trimmed.Substring("node:".Length);
            return trimmed;
        }

        private static Type? ResolveNodeModuleType(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            var asm = typeof(JavaScriptRuntime.Node.NodeModuleAttribute).Assembly;
            // Scan JavaScriptRuntime.Node namespace for [NodeModule(Name=key)]
            foreach (var t in asm.GetTypes())
            {
                if (!t.IsClass || t.IsAbstract) continue;
                if (!string.Equals(t.Namespace, "JavaScriptRuntime.Node", StringComparison.Ordinal)) continue;
                var attr = t.GetCustomAttributes(false).FirstOrDefault(a => a.GetType().FullName == "JavaScriptRuntime.Node.NodeModuleAttribute");
                if (attr == null) continue;
                var nameProp = attr.GetType().GetProperty("Name");
                var nameVal = nameProp?.GetValue(attr) as string;
                if (string.Equals(nameVal, key, StringComparison.OrdinalIgnoreCase))
                    return t;
            }
            return null;
        }

        private static string SanitizeForMetadata(string name)
        {
            if (string.IsNullOrEmpty(name)) return "_";
            var chars = name.Select(ch => char.IsLetterOrDigit(ch) || ch == '_' ? ch : '_').ToArray();
            var result = new string(chars);
            if (char.IsDigit(result[0])) result = "_" + result;
            return result;
        }

        private static void TryAssignClrTypeForRequireInit(VariableDeclarator decl, BindingInfo binding)
        {
            // Pattern: const name = require('path') or require("path")
            var init = decl.Init;
            if (init is CallExpression call && call.Callee is Identifier calleeId && calleeId.Name == "require")
            {
                if (call.Arguments.Count == 1 && call.Arguments[0] is Literal lit && lit.Value is string s)
                {
                    var moduleKey = NormalizeModuleName(s);
                    var t = ResolveNodeModuleType(moduleKey);
                    binding.ClrType = t;
                }
            }
        }

        /// <summary>
        /// Analyzes all scopes to determine which ones reference variables from parent scopes.
        /// This is used to determine if classes need _scopes fields and if functions need scope arrays.
        /// </summary>
        private void AnalyzeFreeVariables(Scope scope)
        {
            // Process children first (bottom-up)
            foreach (var child in scope.Children)
            {
                AnalyzeFreeVariables(child);
            }

            // For class scopes, check if any method child references parent variables
            if (scope.Kind == ScopeKind.Class)
            {
                foreach (var methodScope in scope.Children.Where(c => c.Kind == ScopeKind.Function))
                {
                    if (methodScope.ReferencesParentScopeVariables)
                    {
                        scope.ReferencesParentScopeVariables = true;
                        break;
                    }
                }
                
                // Also check class field initializers directly
                if (!scope.ReferencesParentScopeVariables && scope.AstNode is ClassDeclaration classDecl)
                {
                    var localVariables = new HashSet<string>(scope.Bindings.Keys);
                    foreach (var element in classDecl.Body.Body)
                    {
                        if (element is PropertyDefinition propDef && propDef.Value != null)
                        {
                            if (ContainsFreeVariable(propDef.Value, localVariables))
                            {
                                scope.ReferencesParentScopeVariables = true;
                                break;
                            }
                        }
                    }
                }
            }
            // For function scopes, check if the function body references any non-local variables
            else if (scope.Kind == ScopeKind.Function && scope.AstNode is FunctionExpression funcExpr)
            {
                scope.ReferencesParentScopeVariables = CheckFunctionReferencesParentVariables(funcExpr, scope);
            }
            else if (scope.Kind == ScopeKind.Function && scope.AstNode is FunctionDeclaration funcDecl)
            {
                // Convert FunctionDeclaration to use its body like FunctionExpression
                if (funcDecl.Body is BlockStatement body)
                {
                    scope.ReferencesParentScopeVariables = CheckBodyReferencesParentVariables(body, scope);
                }
            }
            else if (scope.Kind == ScopeKind.Function && scope.AstNode is ArrowFunctionExpression arrowExpr)
            {
                scope.ReferencesParentScopeVariables = CheckArrowFunctionReferencesParentVariables(arrowExpr, scope);
            }
        }

        private void BuildScopeRecursive(Scope globalScope, Node node, Scope currentScope)
        {
            switch (node)
            {
                case Acornima.Ast.Program program:
                    foreach (var statement in program.Body)
                        BuildScopeRecursive(globalScope, statement, currentScope);
                    break;
                case ClassDeclaration classDecl:
                    var className = (classDecl.Id as Identifier)?.Name ?? $"Class{++_closureCounter}";
                    var classScope = new Scope(className, ScopeKind.Class, currentScope, classDecl);
                    // Author authoritative .NET naming for classes here
                    classScope.DotNetNamespace = DefaultClassesNamespace + "." + globalScope.Name; 
                    // Per user guidance, keep type name same as scope name for now
                    classScope.DotNetTypeName = SanitizeForMetadata(className);
                    currentScope.Bindings[className] = new BindingInfo(className, BindingKind.Let, classDecl);
                    // Process class body members for nested functions or fields later if needed
                    foreach (var element in classDecl.Body.Body)
                    {
                        // Methods are represented as MethodDefinition (not to be confused with IL). We can capture their keys if needed later.
                        if (element is MethodDefinition mdef && mdef.Value is FunctionExpression mfunc)
                        {
                            // Create a pseudo-scope for the method if we compile methods as functions later
                            var mname = (mdef.Key as Identifier)?.Name ?? $"Method_L{mdef.Location.Start.Line}C{mdef.Location.Start.Column}";
                            var methodScope = new Scope(mname, ScopeKind.Function, classScope, mfunc);
                            foreach (var p in mfunc.Params)
                            {
                                if (p is Identifier pid)
                                {
                                    methodScope.Bindings[pid.Name] = new BindingInfo(pid.Name, BindingKind.Var, pid);
                                    methodScope.Parameters.Add(pid.Name);
                                }
                                else if (p is AssignmentPattern ap && ap.Left is Identifier apId)
                                {
                                    // Parameter with default value (e.g., a = 10)
                                    methodScope.Bindings[apId.Name] = new BindingInfo(apId.Name, BindingKind.Var, apId);
                                    methodScope.Parameters.Add(apId.Name);
                                }
                                else if (p is ObjectPattern op)
                                {
                                    // Destructuring parameter - register bindings for each property
                                    foreach (var pnode in op.Properties)
                                    {
                                        if (pnode is Property prop)
                                        {
                                            // Handle default values: {a = 10} where Value is AssignmentPattern
                                            Identifier? bindId = null;
                                            if (prop.Value is AssignmentPattern apPattern && apPattern.Left is Identifier apLeftId)
                                            {
                                                bindId = apLeftId;
                                            }
                                            else
                                            {
                                                bindId = prop.Value as Identifier ?? prop.Key as Identifier;
                                            }
                                            
                                            if (bindId != null && !methodScope.Bindings.ContainsKey(bindId.Name))
                                            {
                                                methodScope.Bindings[bindId.Name] = new BindingInfo(bindId.Name, BindingKind.Var, bindId);
                                                // Mark as parameter so TypeGenerator creates fields/locals for them
                                                methodScope.Parameters.Add(bindId.Name);
                                            }
                                            // Track that this is a destructured parameter (needs field for storage)
                                            if (bindId != null && !methodScope.DestructuredParameters.Contains(bindId.Name))
                                            {
                                                methodScope.DestructuredParameters.Add(bindId.Name);
                                            }
                                        }
                                    }
                                }
                            }
                            if (mfunc.Body is BlockStatement mblock)
                            {
                                foreach (var st in mblock.Body) BuildScopeRecursive(globalScope, st, methodScope);
                            }
                        }
                    }
                    break;
                case FunctionDeclaration funcDecl:
                    var funcName = (funcDecl.Id as Identifier)?.Name ?? $"Closure{++_closureCounter}";
                    var funcScope = new Scope(funcName, ScopeKind.Function, currentScope, funcDecl);
                    currentScope.Bindings[funcName] = new BindingInfo(funcName, BindingKind.Function, funcDecl);
                        // Register parameters (identifiers + object pattern properties) via helper
                        BindObjectPatternParameters(funcDecl.Params, funcScope);
                        if (funcDecl.Body is BlockStatement fblock)
                        {
                            foreach (var statement in fblock.Body)
                                BuildScopeRecursive(globalScope, statement, funcScope);
                        }
                        else
                        {
                            BuildScopeRecursive(globalScope, funcDecl.Body, funcScope);
                        }
                        break;
                case FunctionExpression funcExpr:
                    // Global de-duplication: if we've already processed this exact FunctionExpression node,
                    // skip creating another scope for it.
                    if (_visitedFunctionExpressions.Contains(funcExpr))
                    {
                        break;
                    }
                    _visitedFunctionExpressions.Add(funcExpr);
                    // Naming must align with ILExpressionGenerator: FunctionExpression_<assignmentTarget> OR FunctionExpression_L{line}C{col}
                    var funcExprName = (funcExpr.Id as Identifier)?.Name ??
                        (!string.IsNullOrEmpty(_currentAssignmentTarget)
                            ? $"FunctionExpression_{_currentAssignmentTarget}"
                            : $"FunctionExpression_L{funcExpr.Location.Start.Line}C{funcExpr.Location.Start.Column}");
                    // Create the scope (constructor links it to the parent); no manual add to avoid duplicates
                    var funcExprScope = new Scope(funcExprName, ScopeKind.Function, currentScope, funcExpr);
                    // Named function expressions create an internal binding for the function name that is
                    // only visible inside the function body (used for recursion). It must not leak to the
                    // outer scope. Authoritative binding here so downstream codegen can allocate a field.
                    if (funcExpr.Id is Identifier internalId && !funcExprScope.Bindings.ContainsKey(internalId.Name))
                    {
                        funcExprScope.Bindings[internalId.Name] = new BindingInfo(internalId.Name, BindingKind.Function, funcExpr);
                    }
                    BindObjectPatternParameters(funcExpr.Params, funcExprScope);
                    if (funcExpr.Body is BlockStatement funcExprBlock)
                    {
                        // For function bodies, process statements directly in function scope without creating a block scope
                        foreach (var statement in funcExprBlock.Body)
                            BuildScopeRecursive(globalScope, statement, funcExprScope);
                    }
                    else
                    {
                        // Non-block body (expression body)
                        BuildScopeRecursive(globalScope, funcExpr.Body, funcExprScope);
                    }
                    break;
                case VariableDeclaration varDecl:
                    foreach (var decl in varDecl.Declarations)
                    {
                        // Support simple identifier declarations
                        if (decl.Id is Identifier id)
                        {
                            var kind = varDecl.Kind switch
                            {
                                VariableDeclarationKind.Var => BindingKind.Var,
                                VariableDeclarationKind.Let => BindingKind.Let,
                                VariableDeclarationKind.Const => BindingKind.Const,
                                _ => BindingKind.Var
                            };

                            // Hoist `var` declared inside block statements to the nearest function/global scope
                            // per JavaScript semantics. `let`/`const` remain block-scoped.
                            Scope targetScope = currentScope;
                            if (kind == BindingKind.Var && currentScope.Kind == ScopeKind.Block)
                            {
                                var ancestor = currentScope.Parent;
                                while (ancestor != null && ancestor.Kind != ScopeKind.Function && ancestor.Kind != ScopeKind.Global)
                                {
                                    ancestor = ancestor.Parent;
                                }
                                if (ancestor != null)
                                {
                                    targetScope = ancestor;
                                }
                            }

                            var binding = new BindingInfo(id.Name, kind, decl);
                            // Attempt early CLR type resolution for: const x = require('<module>')
                            TryAssignClrTypeForRequireInit(decl, binding);
                            targetScope.Bindings[id.Name] = binding;

                            // Track assignment target for naming nested functions
                            if (decl.Init != null)
                            {
                                var previousTarget = _currentAssignmentTarget;
                                _currentAssignmentTarget = id.Name;
                                BuildScopeRecursive(globalScope, decl.Init, currentScope);
                                _currentAssignmentTarget = previousTarget;
                            }
                            continue;
                        }

                        // Handle object destructuring patterns, e.g., const { performance } = require('perf_hooks');
                        if (decl.Id is ObjectPattern objPattern)
                        {
                            var kind = varDecl.Kind switch
                            {
                                VariableDeclarationKind.Var => BindingKind.Var,
                                VariableDeclarationKind.Let => BindingKind.Let,
                                VariableDeclarationKind.Const => BindingKind.Const,
                                _ => BindingKind.Var
                            };

                            // Determine hoisting target for `var` declarations made inside blocks
                            Scope targetScope = currentScope;
                            if (kind == BindingKind.Var && currentScope.Kind == ScopeKind.Block)
                            {
                                var ancestor = currentScope.Parent;
                                while (ancestor != null && ancestor.Kind != ScopeKind.Function && ancestor.Kind != ScopeKind.Global)
                                {
                                    ancestor = ancestor.Parent;
                                }
                                if (ancestor != null)
                                {
                                    targetScope = ancestor;
                                }
                            }

                            // Synthetic temporary binding to hold the initializer object so IL can mirror snapshots
                            // Name policy: use "perf" when destructuring a perf_hooks require; otherwise a generic name.
                            string tempName = "__obj";
                            if (decl.Init is CallExpression call && call.Callee is Identifier calleeId && calleeId.Name == "require"
                                && call.Arguments.Count == 1 && call.Arguments[0] is Literal lit && lit.Value is string s && NormalizeModuleName(s) == "perf_hooks")
                            {
                                tempName = "perf";
                            }
                            if (!targetScope.Bindings.ContainsKey(tempName))
                            {
                                var tempBinding = new BindingInfo(tempName, kind, decl);
                                TryAssignClrTypeForRequireInit(decl, tempBinding);
                                targetScope.Bindings[tempName] = tempBinding;
                            }

                            // Create bindings for each property in the pattern (only simple identifiers for now)
                            foreach (var pnode in objPattern.Properties)
                            {
                                if (pnode is Property prop)
                                {
                                    // The binding identifier is in prop.Value for patterns like { performance }
                                    if (prop.Value is Identifier bid)
                                    {
                                        if (!targetScope.Bindings.ContainsKey(bid.Name))
                                        {
                                            targetScope.Bindings[bid.Name] = new BindingInfo(bid.Name, kind, decl);
                                        }
                                    }
                                }
                            }

                            // Visit initializer expression to record any nested references
                            if (decl.Init != null)
                            {
                                var previousTarget = _currentAssignmentTarget;
                                _currentAssignmentTarget = tempName;
                                BuildScopeRecursive(globalScope, decl.Init, currentScope);
                                _currentAssignmentTarget = previousTarget;
                            }

                            continue;
                        }

                        // Fallback: just visit the initializer if present
                        if (decl.Init != null)
                        {
                            BuildScopeRecursive(globalScope, decl.Init, currentScope);
                        }
                    }
                    break;
                case BlockStatement blockStmt:
                    // Compute deterministic block scope name (must match codegen expectations)
                    var blockName = $"Block_L{blockStmt.Location.Start.Line}C{blockStmt.Location.Start.Column}";
                    // Guard: reflection-based traversal may encounter the same BlockStatement twice (e.g., via multiple properties).
                    // Avoid creating duplicate scopes by checking for an existing child with the same name under the current scope.
                    var existingBlock = currentScope.Children.FirstOrDefault(s => s.Kind == ScopeKind.Block && s.Name == blockName);
                    if (existingBlock != null)
                    {
                        // Already processed this block; skip to avoid duplicate nested types
                        break;
                    }

                    var blockScope = new Scope(blockName, ScopeKind.Block, currentScope, blockStmt);
                    foreach (var statement in blockStmt.Body)
                        BuildScopeRecursive(globalScope, statement, blockScope);
                    break;
                case ExpressionStatement exprStmt:
                    BuildScopeRecursive(globalScope, exprStmt.Expression, currentScope);
                    break;
                case AssignmentExpression assignExpr:
                    BuildScopeRecursive(globalScope, assignExpr.Right, currentScope);
                    BuildScopeRecursive(globalScope, assignExpr.Left, currentScope);
                    break;
                case ArrowFunctionExpression arrowFunc:
                    // Avoid duplicate scopes for the same ArrowFunctionExpression node
                    if (_visitedArrowFunctions.Contains(arrowFunc))
                    {
                        break;
                    }
                    _visitedArrowFunctions.Add(arrowFunc);
                    // Match ILExpressionGenerator naming: ArrowFunction_<assignmentTarget> OR ArrowFunction_L{line}C{col}
                    var arrowName = !string.IsNullOrEmpty(_currentAssignmentTarget)
                        ? $"ArrowFunction_{_currentAssignmentTarget}"
                        : $"ArrowFunction_L{arrowFunc.Location.Start.Line}C{arrowFunc.Location.Start.Column}";
                    var arrowScope = new Scope(arrowName, ScopeKind.Function, currentScope, arrowFunc);
                    int syntheticIndex = 0;
                    foreach (var param in arrowFunc.Params)
                    {
                        if (param is Identifier id)
                        {
                            arrowScope.Bindings[id.Name] = new BindingInfo(id.Name, BindingKind.Var, id);
                            arrowScope.Parameters.Add(id.Name);
                        }
                        else if (param is AssignmentPattern ap && ap.Left is Identifier apId)
                        {
                            // Parameter with default value (e.g., a = 10)
                            arrowScope.Bindings[apId.Name] = new BindingInfo(apId.Name, BindingKind.Var, apId);
                            arrowScope.Parameters.Add(apId.Name);
                        }
                        else if (param is ObjectPattern op)
                        {
                            // Destructured parameter: bind each property identifier as a local binding in arrow function scope
                            foreach (var pnode in op.Properties)
                            {
                                if (pnode is Property prop)
                                {
                                    // Handle default values: {a = 10} where Value is AssignmentPattern
                                    // Extract the binding identifier
                                    Identifier? bindId = null;
                                    if (prop.Value is AssignmentPattern apPattern && apPattern.Left is Identifier apLeftId)
                                    {
                                        bindId = apLeftId;
                                    }
                                    else
                                    {
                                        // Binding target name: prefer value identifier (alias), else shorthand key identifier
                                        bindId = prop.Value as Identifier ?? prop.Key as Identifier;
                                    }
                                    
                                    if (bindId != null && !arrowScope.Bindings.ContainsKey(bindId.Name))
                                    {
                                        arrowScope.Bindings[bindId.Name] = new BindingInfo(bindId.Name, BindingKind.Var, bindId);
                                        // Mark as parameter so TypeGenerator creates fields for them
                                        arrowScope.Parameters.Add(bindId.Name);
                                    }
                                }
                            }
                            // Parameter list will still receive a synthetic name during codegen; no binding needed for it.
                        }
                        else
                        {
                            // Fallback: attempt to read a 'Name' property via reflection to support alternate AST shapes
                            try
                            {
                                var prop = param.GetType().GetProperty("Name");
                                var name = prop?.GetValue(param) as string;
                                if (!string.IsNullOrEmpty(name))
                                {
                                    arrowScope.Bindings[name!] = new BindingInfo(name!, BindingKind.Var, param);
                                    arrowScope.Parameters.Add(name!);
                                }
                                else
                                {
                                    var syn = $"p{syntheticIndex++}";
                                    arrowScope.Bindings[syn] = new BindingInfo(syn, BindingKind.Var, param);
                                    arrowScope.Parameters.Add(syn);
                                }
                            }
                            catch
                            {
                                var syn = $"p{syntheticIndex++}";
                                arrowScope.Bindings[syn] = new BindingInfo(syn, BindingKind.Var, param);
                                arrowScope.Parameters.Add(syn);
                            }
                        }
                    }
                    if (arrowFunc.Body is BlockStatement arrowBlock)
                    {
                        // For function bodies, process statements directly in function scope without creating a block scope
                        foreach (var statement in arrowBlock.Body)
                            BuildScopeRecursive(globalScope, statement, arrowScope);
                    }
                    else
                    {
                        // Arrow function with expression body
                        BuildScopeRecursive(globalScope, arrowFunc.Body, arrowScope);
                    }
                    break;
                case CallExpression callExpr:
                    // Process callee and arguments but don't create scopes for call expressions themselves
                    BuildScopeRecursive(globalScope, callExpr.Callee, currentScope);
                    foreach (var arg in callExpr.Arguments)
                    {
                        BuildScopeRecursive(globalScope, arg, currentScope);
                    }
                    break;
                case MemberExpression memberExpr:
                    BuildScopeRecursive(globalScope, memberExpr.Object, currentScope);
                    if (memberExpr.Computed)
                    {
                        BuildScopeRecursive(globalScope, memberExpr.Property, currentScope);
                    }
                    break;
                case ArrayExpression arrayExpr:
                    foreach (var element in arrayExpr.Elements)
                    {
                        if (element != null)
                        {
                            BuildScopeRecursive(globalScope, element, currentScope);
                        }
                    }
                    break;
                case ForStatement forStmt:
                    // Process init, test, and update expressions
                    if (forStmt.Init != null)
                        BuildScopeRecursive(globalScope, forStmt.Init, currentScope);
                    if (forStmt.Test != null)
                        BuildScopeRecursive(globalScope, forStmt.Test, currentScope);
                    if (forStmt.Update != null)
                        BuildScopeRecursive(globalScope, forStmt.Update, currentScope);
                    
                    // Process the body statement (which may be a block or a single statement)
                    if (forStmt.Body != null)
                        BuildScopeRecursive(globalScope, forStmt.Body, currentScope);
                    break;
                case ForOfStatement forOf:
                    // Register loop variable binding if declared (e.g., for (const x of arr))
                    if (forOf.Left is VariableDeclaration forOfDecl)
                    {
                        foreach (var decl in forOfDecl.Declarations)
                        {
                            if (decl.Id is Identifier id)
                            {
                                var kind = forOfDecl.Kind switch
                                {
                                    VariableDeclarationKind.Var => BindingKind.Var,
                                    VariableDeclarationKind.Let => BindingKind.Let,
                                    VariableDeclarationKind.Const => BindingKind.Const,
                                    _ => BindingKind.Var
                                };
                                currentScope.Bindings[id.Name] = new BindingInfo(id.Name, kind, decl);
                            }
                        }
                    }
                    // Visit the iterable expression and loop body
                    BuildScopeRecursive(globalScope, forOf.Right, currentScope);
                    if (forOf.Body != null)
                        BuildScopeRecursive(globalScope, forOf.Body, currentScope);
                    break;
                default:
                    // For other node types, recursively process their children
                    ProcessChildNodes(globalScope, node, currentScope);
                    break;
                // Add more cases as needed for other node types
            }
        }

        /// <summary>
        /// Checks if an arrow function references variables not declared locally or in parameters.
        /// </summary>
        private bool CheckArrowFunctionReferencesParentVariables(ArrowFunctionExpression arrowExpr, Scope functionScope)
        {
            if (arrowExpr.Body is BlockStatement body)
            {
                return CheckBodyReferencesParentVariables(body, functionScope);
            }
            else
            {
                // Expression body - check the expression directly
                var localVariables = new HashSet<string>(functionScope.Bindings.Keys);
                localVariables.UnionWith(functionScope.Parameters);
                return ContainsFreeVariable(arrowExpr.Body, localVariables);
            }
        }

        /// <summary>
        /// Checks if a block statement references variables not declared in the given scope or its parameters.
        /// </summary>
        private bool CheckBodyReferencesParentVariables(BlockStatement body, Scope scope)
        {
            var localVariables = new HashSet<string>(scope.Bindings.Keys);
            localVariables.UnionWith(scope.Parameters);
            return ContainsFreeVariable(body, localVariables);
        }

        /// <summary>
        /// Checks if a function expression references variables not declared locally or in parameters.
        /// </summary>
        private bool CheckFunctionReferencesParentVariables(FunctionExpression funcExpr, Scope functionScope)
        {
            if (funcExpr.Body is not BlockStatement body) return false;
            return CheckBodyReferencesParentVariables(body, functionScope);
        }

        /// <summary>
        /// Recursively collects identifiers that are not in localVariables but are in targetVariables.
        /// </summary>
        private void CollectFreeVariables(Node? node, HashSet<string> localVariables, HashSet<string> targetVariables, HashSet<string> result)
        {
            if (node == null) return;

            switch (node)
            {
                case Identifier id:
                    // If this identifier is not local, not a global intrinsic, and is in target scope
                    if (!localVariables.Contains(id.Name) && 
                        !IsKnownGlobalIntrinsic(id.Name) && 
                        targetVariables.Contains(id.Name))
                    {
                        result.Add(id.Name);
                    }
                    break;

                case VariableDeclaration vd:
                    // Add declared variables to local set
                    var newLocals = new HashSet<string>(localVariables);
                    foreach (var decl in vd.Declarations)
                    {
                        if (decl.Id is Identifier vid)
                        {
                            newLocals.Add(vid.Name);
                        }
                    }
                    // Check initializers with updated local set
                    foreach (var decl in vd.Declarations)
                    {
                        CollectFreeVariables(decl.Init, newLocals, targetVariables, result);
                    }
                    break;

                case BlockStatement bs:
                    foreach (var stmt in bs.Body)
                    {
                        CollectFreeVariables(stmt, localVariables, targetVariables, result);
                    }
                    break;

                case ExpressionStatement es:
                    CollectFreeVariables(es.Expression, localVariables, targetVariables, result);
                    break;

                case ReturnStatement rs:
                    CollectFreeVariables(rs.Argument, localVariables, targetVariables, result);
                    break;

                case IfStatement ifs:
                    CollectFreeVariables(ifs.Test, localVariables, targetVariables, result);
                    CollectFreeVariables(ifs.Consequent, localVariables, targetVariables, result);
                    CollectFreeVariables(ifs.Alternate, localVariables, targetVariables, result);
                    break;

                case ForStatement fs:
                    CollectFreeVariables(fs.Init as Node, localVariables, targetVariables, result);
                    CollectFreeVariables(fs.Test, localVariables, targetVariables, result);
                    CollectFreeVariables(fs.Update, localVariables, targetVariables, result);
                    CollectFreeVariables(fs.Body, localVariables, targetVariables, result);
                    break;

                case WhileStatement ws:
                    CollectFreeVariables(ws.Test, localVariables, targetVariables, result);
                    CollectFreeVariables(ws.Body, localVariables, targetVariables, result);
                    break;

                case DoWhileStatement dws:
                    CollectFreeVariables(dws.Body, localVariables, targetVariables, result);
                    CollectFreeVariables(dws.Test, localVariables, targetVariables, result);
                    break;

                case BinaryExpression be:
                    CollectFreeVariables(be.Left, localVariables, targetVariables, result);
                    CollectFreeVariables(be.Right, localVariables, targetVariables, result);
                    break;

                case UpdateExpression upe:
                    CollectFreeVariables(upe.Argument, localVariables, targetVariables, result);
                    break;

                case UnaryExpression ue:
                    CollectFreeVariables(ue.Argument, localVariables, targetVariables, result);
                    break;

                case CallExpression ce:
                    CollectFreeVariables(ce.Callee, localVariables, targetVariables, result);
                    foreach (var arg in ce.Arguments)
                    {
                        CollectFreeVariables(arg as Node, localVariables, targetVariables, result);
                    }
                    break;

                case MemberExpression me:
                    CollectFreeVariables(me.Object, localVariables, targetVariables, result);
                    if (me.Computed)
                    {
                        CollectFreeVariables(me.Property as Node, localVariables, targetVariables, result);
                    }
                    break;

                case TemplateLiteral template:
                    // Only expressions can reference identifiers; quasis are static strings.
                    foreach (var expr in template.Expressions)
                    {
                        CollectFreeVariables(expr, localVariables, targetVariables, result);
                    }
                    break;

                case TaggedTemplateExpression tagged:
                    CollectFreeVariables(tagged.Tag, localVariables, targetVariables, result);
                    CollectFreeVariables(tagged.Quasi, localVariables, targetVariables, result);
                    break;

                case AssignmentExpression ae:
                    CollectFreeVariables(ae.Left, localVariables, targetVariables, result);
                    CollectFreeVariables(ae.Right, localVariables, targetVariables, result);
                    break;

                case ConditionalExpression ce:
                    CollectFreeVariables(ce.Test, localVariables, targetVariables, result);
                    CollectFreeVariables(ce.Consequent, localVariables, targetVariables, result);
                    CollectFreeVariables(ce.Alternate, localVariables, targetVariables, result);
                    break;

                case ArrayExpression arr:
                    foreach (var elem in arr.Elements)
                    {
                        CollectFreeVariables(elem as Node, localVariables, targetVariables, result);
                    }
                    break;

                case ObjectExpression obj:
                    foreach (var prop in obj.Properties)
                    {
                        CollectFreeVariables(prop, localVariables, targetVariables, result);
                    }
                    break;

                case Property prop:
                    CollectFreeVariables(prop.Key as Node, localVariables, targetVariables, result);
                    CollectFreeVariables(prop.Value as Node, localVariables, targetVariables, result);
                    break;

                case FunctionDeclaration funcDecl:
                    // Process the function body with the function's local variables
                    if (funcDecl.Body is BlockStatement funcBody)
                    {
                        foreach (var stmt in funcBody.Body)
                        {
                            CollectFreeVariables(stmt, localVariables, targetVariables, result);
                        }
                    }
                    break;

                case FunctionExpression funcExpr:
                    // Process the function body with the function's local variables
                    if (funcExpr.Body is BlockStatement funcExprBody)
                    {
                        foreach (var stmt in funcExprBody.Body)
                        {
                            CollectFreeVariables(stmt, localVariables, targetVariables, result);
                        }
                    }
                    break;

                case ArrowFunctionExpression arrowExpr:
                    // Process the arrow function body
                    if (arrowExpr.Body is BlockStatement arrowBody)
                    {
                        foreach (var stmt in arrowBody.Body)
                        {
                            CollectFreeVariables(stmt, localVariables, targetVariables, result);
                        }
                    }
                    else
                    {
                        // Expression body
                        CollectFreeVariables(arrowExpr.Body, localVariables, targetVariables, result);
                    }
                    break;

                case ClassDeclaration classDecl:
                    // Process class methods and field initializers
                    foreach (var element in classDecl.Body.Body)
                    {
                        if (element is MethodDefinition mdef && mdef.Value is FunctionExpression mfunc)
                        {
                            // Process method body
                            if (mfunc.Body is BlockStatement mblock)
                            {
                                foreach (var stmt in mblock.Body)
                                {
                                    CollectFreeVariables(stmt, localVariables, targetVariables, result);
                                }
                            }
                        }
                        else if (element is PropertyDefinition propDef)
                        {
                            // Process field initializer expression
                            if (propDef.Value != null)
                            {
                                CollectFreeVariables(propDef.Value, localVariables, targetVariables, result);
                            }
                        }
                    }
                    break;

                default:
                    // For unknown node types, do nothing
                    break;
            }
        }

        /// <summary>
        /// Collects the names of variables from targetScope (and its ancestors) that are referenced in childScope's AST node.
        /// </summary>
        private HashSet<string> CollectReferencedParentVariables(Scope childScope, Scope targetScope)
        {
            var result = new HashSet<string>();
            var childLocals = new HashSet<string>(childScope.Bindings.Keys);
            childLocals.UnionWith(childScope.Parameters);
            
            // Collect all ancestor variables, not just immediate parent
            var ancestorVariables = new HashSet<string>();
            var currentAncestor = targetScope;
            while (currentAncestor != null)
            {
                foreach (var key in currentAncestor.Bindings.Keys)
                {
                    ancestorVariables.Add(key);
                }
                currentAncestor = currentAncestor.Parent;
            }
            
            if (childScope.AstNode != null)
            {
                CollectFreeVariables(childScope.AstNode, childLocals, ancestorVariables, result);
            }
            
            return result;
        }

        /// <summary>
        /// Marks variables as captured if they are referenced by any child scope.
        /// This enables optimization: uncaptured variables can use local variables instead of fields.
        /// </summary>
        private void MarkCapturedVariables(Scope scope)
        {
            // For each child scope that references parent variables
            foreach (var child in scope.Children)
            {
                // Only mark variables as captured if the child scope is a function or class scope
                // Block scopes don't create closures, so variables referenced from block scopes
                // don't need to be captured (they can use locals)
                // Function scopes create closures, class scopes have field initializers that may reference outer variables
                if ((child.Kind == ScopeKind.Function || child.Kind == ScopeKind.Class) && child.ReferencesParentScopeVariables)
                {
                    // Find which specific variables from this scope (and ancestors) are referenced by the child
                    var capturedVars = CollectReferencedParentVariables(child, scope);
                    foreach (var varName in capturedVars)
                    {
                        // Mark the variable in whichever ancestor scope it's declared
                        var searchScope = scope;
                        while (searchScope != null)
                        {
                            if (searchScope.Bindings.TryGetValue(varName, out var binding))
                            {
                                binding.IsCaptured = true;
                                break;
                            }
                            searchScope = searchScope.Parent;
                        }
                    }
                }
                
                // Recurse into child scopes
                MarkCapturedVariables(child);
            }
        }

        private void ProcessChildNodes(Scope globalScope, Node node, Scope currentScope)
        {
            // Use reflection to get all node properties and recursively process them
            var properties = node.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(node);
                if (value is Node childNode)
                {
                    BuildScopeRecursive(globalScope, childNode, currentScope);
                }
                else if (value is System.Collections.IEnumerable enumerable && 
                         !(value is string))
                {
                    foreach (var item in enumerable)
                    {
                        if (item is Node childNodeInList)
                        {
                            BuildScopeRecursive(globalScope, childNodeInList, currentScope);
                        }
                    }
                }
            }
        }
    }
}

namespace Js2IL.SymbolTables
{
    public partial class SymbolTableBuilder
    {
        /// <summary>
        /// Helper to bind identifier parameters and object pattern property identifiers uniformly.
        /// </summary>
        private static void BindObjectPatternParameters(IEnumerable<Node> parameters, Scope scope)
        {
            foreach (var p in parameters)
            {
                if (p is Identifier id)
                {
                    if (!scope.Bindings.ContainsKey(id.Name))
                    {
                        scope.Bindings[id.Name] = new BindingInfo(id.Name, BindingKind.Var, id);
                    }
                    if (!scope.Parameters.Contains(id.Name))
                    {
                        scope.Parameters.Add(id.Name);
                    }
                }
                else if (p is AssignmentPattern ap && ap.Left is Identifier apId)
                {
                    // Parameter with default value (e.g., a = 10)
                    // Extract the identifier from the left side and register it as a parameter
                    if (!scope.Bindings.ContainsKey(apId.Name))
                    {
                        scope.Bindings[apId.Name] = new BindingInfo(apId.Name, BindingKind.Var, apId);
                    }
                    if (!scope.Parameters.Contains(apId.Name))
                    {
                        scope.Parameters.Add(apId.Name);
                    }
                }
                else if (p is ObjectPattern op)
                {
                    foreach (var pnode in op.Properties)
                    {
                        if (pnode is Property prop)
                        {
                            // Handle default values: {a = 10} where Value is AssignmentPattern
                            Identifier? bindId = null;
                            if (prop.Value is AssignmentPattern apPattern && apPattern.Left is Identifier apLeftId)
                            {
                                bindId = apLeftId;
                            }
                            else
                            {
                                bindId = prop.Value as Identifier ?? prop.Key as Identifier;
                            }
                            
                            if (bindId != null && !scope.Bindings.ContainsKey(bindId.Name))
                            {
                                scope.Bindings[bindId.Name] = new BindingInfo(bindId.Name, BindingKind.Var, bindId);
                            }
                            // Add destructured properties to Parameters set so TypeGenerator creates fields/locals
                            if (bindId != null && !scope.Parameters.Contains(bindId.Name))
                            {
                                scope.Parameters.Add(bindId.Name);
                            }
                            // Track that this is a destructured parameter (needs field for storage)
                            if (bindId != null && !scope.DestructuredParameters.Contains(bindId.Name))
                            {
                                scope.DestructuredParameters.Add(bindId.Name);
                            }
                        }
                    }
                }
                else if (p is AssignmentPattern ap2 && ap2.Left is ObjectPattern opWithDefault)
                {
                    // Handle destructured parameter with default value: { x, y } = { x: 0, y: 0 }
                    foreach (var pnode in opWithDefault.Properties)
                    {
                        if (pnode is Property prop)
                        {
                            // Handle nested default values: {a = 10} = {...} where Value is AssignmentPattern
                            Identifier? bindId = null;
                            if (prop.Value is AssignmentPattern apPattern && apPattern.Left is Identifier apLeftId)
                            {
                                bindId = apLeftId;
                            }
                            else
                            {
                                bindId = prop.Value as Identifier ?? prop.Key as Identifier;
                            }
                            
                            if (bindId != null && !scope.Bindings.ContainsKey(bindId.Name))
                            {
                                scope.Bindings[bindId.Name] = new BindingInfo(bindId.Name, BindingKind.Var, bindId);
                            }
                            // Add destructured properties to Parameters set
                            if (bindId != null && !scope.Parameters.Contains(bindId.Name))
                            {
                                scope.Parameters.Add(bindId.Name);
                            }
                            // Track that this is a destructured parameter (needs field for storage)
                            if (bindId != null && !scope.DestructuredParameters.Contains(bindId.Name))
                            {
                                scope.DestructuredParameters.Add(bindId.Name);
                            }
                        }
                    }
                }
            }
        }
    }
}
