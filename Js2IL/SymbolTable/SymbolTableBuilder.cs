using Acornima.Ast;
using System;
using System.Collections.Generic;
using System.IO;

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
                    var funcExprName = (funcExpr.Id as Identifier)?.Name ?? 
                        (!string.IsNullOrEmpty(_currentAssignmentTarget) 
                            ? $"Function_{_currentAssignmentTarget}"
                            : $"Closure{++_closureCounter}_L{funcExpr.Location.Start.Line}C{funcExpr.Location.Start.Column}");
                    var funcExprScope = new Scope(funcExprName, ScopeKind.Function, currentScope, funcExpr);
                    foreach (var param in funcExpr.Params)
                    {
                        if (param is Identifier id)
                        {
                            funcExprScope.Bindings[id.Name] = new BindingInfo(id.Name, BindingKind.Var, id);
                            funcExprScope.Parameters.Add(id.Name);
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
                        if (decl.Id is Identifier id)
                        {
                            var kind = varDecl.Kind switch
                            {
                                VariableDeclarationKind.Var => BindingKind.Var,
                                VariableDeclarationKind.Let => BindingKind.Let,
                                VariableDeclarationKind.Const => BindingKind.Const,
                                _ => BindingKind.Var
                            };
                            currentScope.Bindings[id.Name] = new BindingInfo(id.Name, kind, decl);
                        }
                        // Track assignment target for naming nested functions
                        if (decl.Init != null && decl.Id is Identifier targetId)
                        {
                            var previousTarget = _currentAssignmentTarget;
                            _currentAssignmentTarget = targetId.Name;
                            BuildScopeRecursive(decl.Init, currentScope);
                            _currentAssignmentTarget = previousTarget;
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
                    var arrowName = !string.IsNullOrEmpty(_currentAssignmentTarget) 
                        ? $"ArrowFunction_{_currentAssignmentTarget}"
                        : $"ArrowFunction{++_closureCounter}_L{arrowFunc.Location.Start.Line}C{arrowFunc.Location.Start.Column}";
                    var arrowScope = new Scope(arrowName, ScopeKind.Function, currentScope, arrowFunc);
                    foreach (var param in arrowFunc.Params)
                    {
                        if (param is Identifier id)
                        {
                            arrowScope.Bindings[id.Name] = new BindingInfo(id.Name, BindingKind.Var, id);
                            arrowScope.Parameters.Add(id.Name);
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
