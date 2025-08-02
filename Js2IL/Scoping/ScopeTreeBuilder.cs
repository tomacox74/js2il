using Acornima.Ast;
using System;
using System.Collections.Generic;
using System.IO;

namespace Js2IL.Scoping
{
    /// <summary>
    /// Builds a ScopeTree from a JavaScript AST.
    /// </summary>
    public class ScopeTreeBuilder
    {
        private int _closureCounter = 0;
        private string? _currentAssignmentTarget = null;

        public ScopeTree Build(Node astRoot, string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var globalScope = new ScopeNode(fileName, ScopeKind.Global, null, astRoot);
            BuildScopeRecursive(astRoot, globalScope);
            return new ScopeTree(globalScope);
        }

        private void BuildScopeRecursive(Node node, ScopeNode currentScope)
        {
            switch (node)
            {
                case Acornima.Ast.Program program:
                    foreach (var statement in program.Body)
                        BuildScopeRecursive(statement, currentScope);
                    break;
                case FunctionDeclaration funcDecl:
                    var funcName = (funcDecl.Id as Identifier)?.Name ?? $"Closure{++_closureCounter}";
                    var funcScope = new ScopeNode(funcName, ScopeKind.Function, currentScope, funcDecl);
                    currentScope.Bindings[funcName] = new BindingInfo(funcName, BindingKind.Function, funcDecl);
                    foreach (var param in funcDecl.Params)
                    {
                        if (param is Identifier id)
                            funcScope.Bindings[id.Name] = new BindingInfo(id.Name, BindingKind.Var, id);
                    }
                    if (funcDecl.Body is BlockStatement block)
                    {
                        // For function bodies, process statements directly in function scope (for var hoisting)
                        // but create nested block scopes for explicit blocks
                        foreach (var statement in block.Body)
                            BuildScopeRecursive(statement, funcScope);
                    }
                    break;
                case FunctionExpression funcExpr:
                    var funcExprName = (funcExpr.Id as Identifier)?.Name ?? 
                        (!string.IsNullOrEmpty(_currentAssignmentTarget) 
                            ? $"Function_{_currentAssignmentTarget}"
                            : $"Closure{++_closureCounter}_L{funcExpr.Location.Start.Line}C{funcExpr.Location.Start.Column}");
                    var funcExprScope = new ScopeNode(funcExprName, ScopeKind.Function, currentScope, funcExpr);
                    foreach (var param in funcExpr.Params)
                    {
                        if (param is Identifier id)
                            funcExprScope.Bindings[id.Name] = new BindingInfo(id.Name, BindingKind.Var, id);
                    }
                    if (funcExpr.Body is BlockStatement funcExprBlock)
                    {
                        // For function bodies, process statements directly in function scope
                        foreach (var statement in funcExprBlock.Body)
                            BuildScopeRecursive(statement, funcExprScope);
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
                    var blockScope = new ScopeNode($"Block_{Guid.NewGuid().ToString("N").Substring(0, 8)}", ScopeKind.Block, currentScope, blockStmt);
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
                    var arrowScope = new ScopeNode(arrowName, ScopeKind.Function, currentScope, arrowFunc);
                    foreach (var param in arrowFunc.Params)
                    {
                        if (param is Identifier id)
                            arrowScope.Bindings[id.Name] = new BindingInfo(id.Name, BindingKind.Var, id);
                    }
                    if (arrowFunc.Body is BlockStatement arrowBlock)
                    {
                        // For function bodies, process statements directly in function scope
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
                default:
                    // For other node types, recursively process their children
                    ProcessChildNodes(node, currentScope);
                    break;
                // Add more cases as needed for other node types
            }
        }

        private void ProcessChildNodes(Node node, ScopeNode currentScope)
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
