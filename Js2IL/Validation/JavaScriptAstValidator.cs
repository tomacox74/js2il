using Acornima.Ast;
using Js2IL.Services;
using Js2IL.Utilities;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Reflection;

namespace Js2IL.Validation;

public class JavaScriptAstValidator : IAstValidator
{
    private static readonly Lazy<HashSet<string>> SupportedRequireModules = new(() =>
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            foreach (var name in JavaScriptRuntime.Node.NodeModuleRegistry.GetSupportedModuleNames())
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    set.Add(name);
                }
            }
        }
        catch { /* Ignore reflection errors; result will be empty set */ }
        return set;
    });

    public ValidationResult Validate(Acornima.Ast.Program ast)
    {
        var result = new ValidationResult { IsValid = true };

        // Validate async/await usage - await is only valid inside async functions.
        ValidateAsyncAwait(ast, result);
        
        // Track contexts where 'this' is supported.
        var contextStack = new Stack<ValidationContext>();
        contextStack.Push(new ValidationContext { AllowsThis = false, ScopeOwner = null, MethodDefinitionFunctionValue = null });
        
        // Visit all nodes in the AST
        var walker = new AstWalker();
        walker.VisitWithContext(ast, node =>
        {
            var currentContext = contextStack.Peek();
            
            // Push new context for class methods and constructors
            if (node is MethodDefinition methodDef)
            {
                contextStack.Push(new ValidationContext
                {
                    AllowsThis = true,
                    ScopeOwner = methodDef,
                    // Track the function expression that is the method body so we don't treat it as nested
                    MethodDefinitionFunctionValue = methodDef.Value
                });
            }
            // Push new context for functions (exclude the method body itself).
            else if ((node is ArrowFunctionExpression || node is FunctionExpression || node is FunctionDeclaration)
                     && !ReferenceEquals(node, currentContext.MethodDefinitionFunctionValue))
            {
                contextStack.Push(new ValidationContext
                {
                    AllowsThis = node is not ArrowFunctionExpression,
                    ScopeOwner = node,
                    MethodDefinitionFunctionValue = currentContext.MethodDefinitionFunctionValue
                });
            }
            
            // Check for unsupported features
            switch (node.Type)
            {
                case NodeType.ClassDeclaration:
                case NodeType.ClassExpression:
                    // Classes are supported - validation for class-specific features 
                    // is handled by MethodDefinition and PropertyDefinition cases
                    break;

                case NodeType.ImportDeclaration:
                case NodeType.ExportNamedDeclaration:
                case NodeType.ExportDefaultDeclaration:
                    result.Errors.Add($"ES6 modules are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.YieldExpression:
                    result.Errors.Add($"Generators are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.RestElement:
                    // RestElement is used for both rest parameters and rest destructuring patterns.
                    // Rest destructuring is supported; rest parameters are validated in ValidateFunctionParameters.
                    break;

                case NodeType.ForInStatement:
                    // for...in loops are supported (PL2.4)
                    break;

                case NodeType.SwitchStatement:
                    // Switch statements are supported (PL2.6)
                    break;

                case NodeType.WithStatement:
                    result.Errors.Add($"The 'with' statement is not supported (deprecated and problematic) (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.LabeledStatement:
                    // Labeled statements are supported (used for labeled break/continue)
                    break;

                case NodeType.DebuggerStatement:
                    result.Errors.Add($"The 'debugger' statement is not supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.MetaProperty:
                    // new.target and import.meta
                    result.Errors.Add($"new.target/import.meta are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.Super:
                    result.Errors.Add($"The 'super' keyword is not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.ThisExpression:
                    // 'this' is supported in class methods/constructors and non-arrow functions.
                    if (!currentContext.AllowsThis)
                    {
                        result.Errors.Add($"The 'this' keyword is not yet supported in this context (line {node.Location.Start.Line})");
                        result.IsValid = false;
                    }
                    break;

                case NodeType.ArrayPattern:
                    // Array destructuring is supported.
                    break;

                case NodeType.ObjectPattern:
                    // Object destructuring is supported.
                    break;

                case NodeType.AssignmentExpression:
                    // Destructuring assignment is supported.
                    break;

                case NodeType.Property:
                    // Check for computed properties, getters/setters in object literals
                    ValidateProperty(node, result);
                    break;

                case NodeType.MethodDefinition:
                    // Check for getters/setters in classes
                    ValidateMethodDefinition(node, result);
                    break;

                case NodeType.CallExpression:
                    // Detect require(...) patterns and spread in call arguments
                    ValidateCallExpression(node, result);
                    break;

                case NodeType.FunctionDeclaration:
                case NodeType.FunctionExpression:
                case NodeType.ArrowFunctionExpression:
                    // Check for parameter count limit
                    ValidateFunctionParameters(node, result);
                    break;
            }
        }, exitNode =>
        {
            if (contextStack.Count > 1 && ReferenceEquals(contextStack.Peek().ScopeOwner, exitNode))
            {
                contextStack.Pop();
            }
        });

        return result;
    }

    private static void ValidateAsyncAwait(Node ast, ValidationResult result)
    {
        // Track whether we're inside an async function for await validation.
        // async functions themselves are now supported; await is only valid inside them.
        var asyncFunctionDepth = 0;
        var finallyDepth = 0; // Track if we're inside a finally block
        var visited = new HashSet<Node>(ReferenceEqualityComparer<Node>.Default);

        void WalkForAsyncAwait(Node? node)
        {
            if (node is null) return;
            if (!visited.Add(node)) return;

            // Track entering async functions
            bool isAsyncFunction = node switch
            {
                FunctionDeclaration fd => fd.Async,
                FunctionExpression fe => fe.Async,
                ArrowFunctionExpression af => af.Async,
                _ => false
            };

            if (isAsyncFunction)
            {
                asyncFunctionDepth++;
            }

            // Handle try statements explicitly to distinguish finally blocks
            if (node is TryStatement tryStmt)
            {
                WalkForAsyncAwait(tryStmt.Block);

                if (tryStmt.Handler != null)
                {
                    WalkForAsyncAwait(tryStmt.Handler);
                }

                if (tryStmt.Finalizer != null)
                {
                    finallyDepth++;
                    WalkForAsyncAwait(tryStmt.Finalizer);
                    finallyDepth--;
                }

                return;
            }

            // Validate await is only inside async functions
            if (node.Type == NodeType.AwaitExpression)
            {
                if (asyncFunctionDepth == 0)
                {
                    result.Errors.Add($"The 'await' keyword is only valid inside async functions (line {node.Location.Start.Line})");
                    result.IsValid = false;
                }
            }

            // Validate for-await-of is not yet supported
            if (node is ForOfStatement forOfStmt && forOfStmt.Await)
            {
                result.Errors.Add($"The 'for await...of' statement is not yet supported (line {node.Location.Start.Line}). Use Promise.all() with a regular for...of loop instead.");
                result.IsValid = false;
            }

            // Walk children
            var type = node.GetType();
            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!prop.CanRead) continue;
                if (prop.GetIndexParameters().Length != 0) continue;

                object? value;
                try { value = prop.GetValue(node); }
                catch { continue; }

                if (value is null || value is string) continue;

                if (value is Node childNode)
                {
                    WalkForAsyncAwait(childNode);
                }
                else if (value is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        if (item is Node itemNode)
                        {
                            WalkForAsyncAwait(itemNode);
                        }
                    }
                }
            }

            // Track exiting async functions
            if (isAsyncFunction)
            {
                asyncFunctionDepth--;
            }
        }

        WalkForAsyncAwait(ast);
    }

    private static void WalkAllNodes(Node? root, HashSet<Node> visited, Action<Node> visit)
    {
        if (root is null) return;
        if (!visited.Add(root)) return;

        visit(root);

        var type = root.GetType();
        foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!prop.CanRead) continue;
            if (prop.GetIndexParameters().Length != 0) continue;

            object? value;
            try
            {
                value = prop.GetValue(root);
            }
            catch
            {
                continue;
            }

            if (value is null) continue;
            if (value is Node childNode)
            {
                WalkAllNodes(childNode, visited, visit);
                continue;
            }

            if (value is string) continue;

            if (value is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    if (item is Node itemNode)
                    {
                        WalkAllNodes(itemNode, visited, visit);
                    }
                }
            }
        }
    }

    // Simple reference equality comparer so the visited set doesn't depend on Node.Equals implementations.
    private sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        public static readonly ReferenceEqualityComparer<T> Default = new();
        public bool Equals(T? x, T? y) => ReferenceEquals(x, y);
        public int GetHashCode(T obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
    }

    // Note: Object/Array patterns (including nested + defaults + rest) and destructuring assignment are supported.

    private void ValidateProperty(Node node, ValidationResult result)
    {
        if (node is Property prop)
        {
            // Computed property names are supported in object literals (ObjectExpression),
            // but are still rejected in non-expression contexts (e.g., patterns) for now.
            // Heuristic: in object literals, the property value is an Expression node.
            if (prop.Computed && prop.Value is not Acornima.Ast.Expression)
            {
                result.Errors.Add($"Computed property names are not yet supported in this context (line {node.Location.Start.Line})");
                result.IsValid = false;
            }

            // Check for getters and setters
            if (prop.Kind == PropertyKind.Get)
            {
                result.Errors.Add($"Getter properties are not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
            }
            else if (prop.Kind == PropertyKind.Set)
            {
                result.Errors.Add($"Setter properties are not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
            }
        }
    }

    private void ValidateMethodDefinition(Node node, ValidationResult result)
    {
        if (node is MethodDefinition method)
        {
            // Check for getters and setters in classes
            if (method.Kind == PropertyKind.Get)
            {
                result.Errors.Add($"Getter methods in classes are not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
            }
            else if (method.Kind == PropertyKind.Set)
            {
                result.Errors.Add($"Setter methods in classes are not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
            }
            // Static methods are supported
        }
    }

    private void ValidateCallExpression(Node node, ValidationResult result)
    {
        if (node is CallExpression call)
        {
            // Check for spread in function call arguments
            foreach (var arg in call.Arguments)
            {
                if (arg is SpreadElement)
                {
                    result.Errors.Add($"Spread in function calls is not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                }
            }

            // Check for require() patterns
            if (call.Callee is Identifier id && id.Name == "require")
            {
                if (call.Arguments.Count == 0)
                {
                    result.Errors.Add($"require() called without arguments (line {node.Location.Start.Line})");
                    result.IsValid = false;
                }
                else if (call.Arguments[0] is Literal lit && lit.Value is string modName)
                {
                    var normalizedName = JavaScriptRuntime.Node.NodeModuleRegistry.NormalizeModuleName(modName);
                    var isLocalModule = normalizedName.StartsWith(".") || normalizedName.StartsWith("/");

                    if (!SupportedRequireModules.Value.Contains(normalizedName) && !isLocalModule)
                    {
                        result.Errors.Add($"Module '{modName}' is not yet supported (line {node.Location.Start.Line})");
                        result.IsValid = false;
                    }
                }
                else
                {
                    // Dynamic/non-literal require argument detected.
                    result.Errors.Add($"Dynamic require() with non-literal argument is not supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                }
            }
        }
    }

    private void ValidateFunctionParameters(Node node, ValidationResult result)
    {
        NodeList<Node>? parameters = node switch
        {
            FunctionDeclaration fd => fd.Params,
            FunctionExpression fe => fe.Params,
            ArrowFunctionExpression af => af.Params,
            _ => null
        };

        int paramCount = parameters?.Count ?? 0;

        // Check for parameter count limit (issue #220)
        if (paramCount > 6)
        {
            result.Errors.Add($"Functions with more than 6 parameters are not yet supported (line {node.Location.Start.Line})");
            result.IsValid = false;
        }

        // Rest parameters (...args) are not supported.
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                if (param is RestElement)
                {
                    result.Errors.Add($"Rest parameters are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;
                }
            }
        }
    }

    private class ValidationContext
    {
        public bool AllowsThis { get; set; }
        public Node? ScopeOwner { get; set; }
        // Track the FunctionExpression that is the direct body of a MethodDefinition
        // so we don't incorrectly treat it as a nested function
        public Node? MethodDefinitionFunctionValue { get; set; }
    }
}
