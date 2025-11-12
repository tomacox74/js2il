using Acornima.Ast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Js2IL.SymbolTables
{
    /// <summary>
    /// Builds a SymbolTable from a JavaScript AST.
    /// </summary>
    public class SymbolTableBuilder
    {
        private int _closureCounter = 0;
        private string? _currentAssignmentTarget = null;
        private const string DefaultClassesNamespace = "Classes";
        // Track visited function expression nodes to avoid duplicating scopes when traversal
        // reaches the same AST node via multiple paths (explicit handling + reflective walk).
        private readonly HashSet<Node> _visitedFunctionExpressions = new();
        private readonly HashSet<Node> _visitedArrowFunctions = new();

        private static string SanitizeForMetadata(string name)
        {
            if (string.IsNullOrEmpty(name)) return "_";
            var chars = name.Select(ch => char.IsLetterOrDigit(ch) || ch == '_' ? ch : '_').ToArray();
            var result = new string(chars);
            if (char.IsDigit(result[0])) result = "_" + result;
            return result;
        }

        public SymbolTable Build(Node astRoot, string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var globalScope = new Scope(fileName, ScopeKind.Global, null, astRoot);
            BuildScopeRecursive(astRoot, globalScope);
            return new SymbolTable(globalScope);
        }

        private void BuildScopeRecursive(Node node, Scope currentScope)
        {
            switch (node)
            {
                case Acornima.Ast.Program program:
                    foreach (var statement in program.Body)
                        BuildScopeRecursive(statement, currentScope);
                    break;
                case ClassDeclaration classDecl:
                    var className = (classDecl.Id as Identifier)?.Name ?? $"Class{++_closureCounter}";
                    var classScope = new Scope(className, ScopeKind.Class, currentScope, classDecl);
                    // Author authoritative .NET naming for classes here
                    classScope.DotNetNamespace = DefaultClassesNamespace; // policy: all JS classes under "Classes"
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
                            }
                            if (mfunc.Body is BlockStatement mblock)
                            {
                                foreach (var st in mblock.Body) BuildScopeRecursive(st, methodScope);
                            }
                        }
                    }
                    break;
                case FunctionDeclaration funcDecl:
                    var funcName = (funcDecl.Id as Identifier)?.Name ?? $"Closure{++_closureCounter}";
                    var funcScope = new Scope(funcName, ScopeKind.Function, currentScope, funcDecl);
                    currentScope.Bindings[funcName] = new BindingInfo(funcName, BindingKind.Function, funcDecl);
                        // Register parameters in the function's own scope
                        foreach (var p in funcDecl.Params)
                        {
                            if (p is Identifier pid)
                            {
                                funcScope.Bindings[pid.Name] = new BindingInfo(pid.Name, BindingKind.Var, pid);
                                funcScope.Parameters.Add(pid.Name);
                            }
                        }
                        if (funcDecl.Body is BlockStatement fblock)
                        {
                            foreach (var statement in fblock.Body)
                                BuildScopeRecursive(statement, funcScope);
                        }
                        else
                        {
                            BuildScopeRecursive(funcDecl.Body, funcScope);
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
                    foreach (var param in funcExpr.Params)
                    {
                        if (param is Identifier id)
                        {
                            funcExprScope.Bindings[id.Name] = new BindingInfo(id.Name, BindingKind.Var, id);
                            funcExprScope.Parameters.Add(id.Name);
                        }
                        else if (param is ObjectPattern op)
                        {
                            // Destructured parameter: bind each property identifier as a local binding in function scope
                            foreach (var pnode in op.Properties)
                            {
                                if (pnode is Property prop)
                                {
                                    if (prop.Value is Identifier bid && !funcExprScope.Bindings.ContainsKey(bid.Name))
                                    {
                                        funcExprScope.Bindings[bid.Name] = new BindingInfo(bid.Name, BindingKind.Var, bid);
                                    }
                                }
                            }
                            // Note: a synthetic CLR parameter will be added for this pattern during codegen (e.g., p0)
                        }
                    }
                    if (funcExpr.Body is BlockStatement funcExprBlock)
                    {
                        // For function bodies, process statements directly in function scope without creating a block scope
                        foreach (var statement in funcExprBlock.Body)
                            BuildScopeRecursive(statement, funcExprScope);
                    }
                    else
                    {
                        // Non-block body (expression body)
                        BuildScopeRecursive(funcExpr.Body, funcExprScope);
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
                                BuildScopeRecursive(decl.Init, currentScope);
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
                                BuildScopeRecursive(decl.Init, currentScope);
                                _currentAssignmentTarget = previousTarget;
                            }

                            continue;
                        }

                        // Fallback: just visit the initializer if present
                        if (decl.Init != null)
                        {
                            BuildScopeRecursive(decl.Init, currentScope);
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
                        BuildScopeRecursive(statement, blockScope);
                    break;
                case ExpressionStatement exprStmt:
                    BuildScopeRecursive(exprStmt.Expression, currentScope);
                    break;
                case AssignmentExpression assignExpr:
                    BuildScopeRecursive(assignExpr.Right, currentScope);
                    BuildScopeRecursive(assignExpr.Left, currentScope);
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
                        else if (param is ObjectPattern op)
                        {
                            // Destructured parameter: bind each property identifier as a local binding in arrow function scope
                            foreach (var pnode in op.Properties)
                            {
                                if (pnode is Property prop)
                                {
                                    if (prop.Value is Identifier bid && !arrowScope.Bindings.ContainsKey(bid.Name))
                                    {
                                        arrowScope.Bindings[bid.Name] = new BindingInfo(bid.Name, BindingKind.Var, bid);
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
                            BuildScopeRecursive(statement, arrowScope);
                    }
                    else
                    {
                        // Arrow function with expression body
                        BuildScopeRecursive(arrowFunc.Body, arrowScope);
                    }
                    break;
                case CallExpression callExpr:
                    // Process callee and arguments but don't create scopes for call expressions themselves
                    BuildScopeRecursive(callExpr.Callee, currentScope);
                    foreach (var arg in callExpr.Arguments)
                    {
                        BuildScopeRecursive(arg, currentScope);
                    }
                    break;
                case MemberExpression memberExpr:
                    BuildScopeRecursive(memberExpr.Object, currentScope);
                    if (memberExpr.Computed)
                    {
                        BuildScopeRecursive(memberExpr.Property, currentScope);
                    }
                    break;
                case ArrayExpression arrayExpr:
                    foreach (var element in arrayExpr.Elements)
                    {
                        if (element != null)
                        {
                            BuildScopeRecursive(element, currentScope);
                        }
                    }
                    break;
                case ForStatement forStmt:
                    // Process init, test, and update expressions
                    if (forStmt.Init != null)
                        BuildScopeRecursive(forStmt.Init, currentScope);
                    if (forStmt.Test != null)
                        BuildScopeRecursive(forStmt.Test, currentScope);
                    if (forStmt.Update != null)
                        BuildScopeRecursive(forStmt.Update, currentScope);
                    
                    // Process the body statement (which may be a block or a single statement)
                    if (forStmt.Body != null)
                        BuildScopeRecursive(forStmt.Body, currentScope);
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
                    BuildScopeRecursive(forOf.Right, currentScope);
                    if (forOf.Body != null)
                        BuildScopeRecursive(forOf.Body, currentScope);
                    break;
                default:
                    // For other node types, recursively process their children
                    ProcessChildNodes(node, currentScope);
                    break;
                // Add more cases as needed for other node types
            }
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
                    binding.RuntimeIntrinsicType = t;
                }
            }
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
            var asm = typeof(JavaScriptRuntime.Require).Assembly;
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

        private void ProcessChildNodes(Node node, Scope currentScope)
        {
            // Use reflection to get all node properties and recursively process them
            var properties = node.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(node);
                if (value is Node childNode)
                {
                    BuildScopeRecursive(childNode, currentScope);
                }
                else if (value is System.Collections.IEnumerable enumerable && 
                         !(value is string))
                {
                    foreach (var item in enumerable)
                    {
                        if (item is Node childNodeInList)
                        {
                            BuildScopeRecursive(childNodeInList, currentScope);
                        }
                    }
                }
            }
        }
    }
}
