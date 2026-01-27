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

        // Validate spec-level early errors that Acornima may parse but considers static errors.
        // In particular, break/continue target rules are specified under iteration statements.
        ValidateIterationStatementEarlyErrors(ast, result);

        // Validate async/await usage - await is only valid inside async functions.
        ValidateAsyncAwait(ast, result);

        // Validate generator/yield usage - yield is only valid inside generator functions.
        ValidateGenerators(ast, result);
        
        // Track contexts where 'this' and 'super' are supported.
        var contextStack = new Stack<ValidationContext>();
        contextStack.Push(new ValidationContext
        {
            AllowsThis = false,
            AllowsSuper = false,
            ScopeOwner = null,
            MethodDefinitionFunctionValue = null,
            InDerivedClass = false
        });
        
        // Visit all nodes in the AST
        var walker = new AstWalker();
        walker.VisitWithContext(ast, node =>
        {
            var currentContext = contextStack.Peek();

            // Track when we're inside a derived class (class with an extends clause).
            if (node is ClassDeclaration cd)
            {
                contextStack.Push(new ValidationContext
                {
                    AllowsThis = currentContext.AllowsThis,
                    AllowsSuper = currentContext.AllowsSuper,
                    ScopeOwner = cd,
                    MethodDefinitionFunctionValue = currentContext.MethodDefinitionFunctionValue,
                    InObjectPattern = currentContext.InObjectPattern,
                    InDerivedClass = cd.SuperClass != null
                });
            }
            else if (node is ClassExpression ce)
            {
                contextStack.Push(new ValidationContext
                {
                    AllowsThis = currentContext.AllowsThis,
                    AllowsSuper = currentContext.AllowsSuper,
                    ScopeOwner = ce,
                    MethodDefinitionFunctionValue = currentContext.MethodDefinitionFunctionValue,
                    InObjectPattern = currentContext.InObjectPattern,
                    InDerivedClass = ce.SuperClass != null
                });
            }

            // Track when we're inside an object binding pattern (destructuring).
            // We currently support object patterns but do not support computed keys inside them.
            if (node is ObjectPattern)
            {
                contextStack.Push(new ValidationContext
                {
                    AllowsThis = currentContext.AllowsThis,
                    AllowsSuper = currentContext.AllowsSuper,
                    ScopeOwner = node,
                    MethodDefinitionFunctionValue = currentContext.MethodDefinitionFunctionValue,
                    InObjectPattern = true,
                    InDerivedClass = currentContext.InDerivedClass
                });
            }
            
            // Push new context for class methods and constructors
            if (node is MethodDefinition methodDef)
            {
                contextStack.Push(new ValidationContext
                {
                    AllowsThis = true,
                    // Allow super in class methods/constructors only when we're inside a derived class.
                    AllowsSuper = currentContext.InDerivedClass,
                    ScopeOwner = methodDef,
                    // Track the function expression that is the method body so we don't treat it as nested
                    MethodDefinitionFunctionValue = methodDef.Value,
                    InDerivedClass = currentContext.InDerivedClass
                });
            }
            // Push new context for functions (exclude the method body itself).
            else if ((node is ArrowFunctionExpression || node is FunctionExpression || node is FunctionDeclaration)
                     && !ReferenceEquals(node, currentContext.MethodDefinitionFunctionValue))
            {
                contextStack.Push(new ValidationContext
                {
                    AllowsThis = node is not ArrowFunctionExpression,
                    AllowsSuper = false,
                    ScopeOwner = node,
                    MethodDefinitionFunctionValue = currentContext.MethodDefinitionFunctionValue
                    ,
                    InDerivedClass = currentContext.InDerivedClass
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
                    // Generator/yield validity is handled by ValidateGenerators.
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
                    // Support super in derived class methods/constructors.
                    if (!currentContext.AllowsSuper)
                    {
                        result.Errors.Add($"The 'super' keyword is not yet supported in this context (line {node.Location.Start.Line})");
                        result.IsValid = false;
                    }
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
                    ValidateProperty(node, result, contextStack.Peek());
                    break;

                case NodeType.MethodDefinition:
                    // Check for getters/setters in classes
                    ValidateMethodDefinition(node, result);
                    break;

                case NodeType.PropertyDefinition:
                    // Check for computed keys in class field definitions
                    ValidatePropertyDefinition(node, result);
                    break;

                case NodeType.StaticBlock:
                    result.Errors.Add($"Class static blocks are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
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

    private static void ValidateIterationStatementEarlyErrors(Acornima.Ast.Program ast, ValidationResult result)
    {
        var walker = new AstWalker();
        var functionContexts = new Stack<EarlyErrorContext>();
        functionContexts.Push(new EarlyErrorContext());

        walker.VisitWithContext(ast, node =>
        {
            // Function boundaries reset label/loop/switch target sets.
            if (node is FunctionDeclaration or FunctionExpression or ArrowFunctionExpression)
            {
                functionContexts.Push(new EarlyErrorContext());
                return;
            }

            var ctx = functionContexts.Peek();

            switch (node)
            {
                case LabeledStatement labeledStmt:
                    {
                        var labelNode = labeledStmt.Label;
                        var labelName = labelNode?.Name;
                        if (string.IsNullOrWhiteSpace(labelName))
                        {
                            break;
                        }

                        if (!ctx.ActiveLabelNames.Add(labelName))
                        {
                            AddError(result,
                                $"Duplicate label '{labelName}' is not allowed",
                                labelNode!);
                            // Still push so the exit pop stays consistent.
                        }

                        ctx.LabelStack.Push(new LabelEntry(labelName, labeledStmt.Body, labelNode!));
                        break;
                    }

                case SwitchStatement:
                    ctx.BreakableStack.Push(BreakableKind.Switch);
                    break;

                case DoWhileStatement:
                case WhileStatement:
                case ForStatement:
                case ForInStatement:
                case ForOfStatement:
                    ctx.BreakableStack.Push(BreakableKind.Iteration);
                    ctx.IterationDepth++;

                    if (node is ForOfStatement forOfStmt && forOfStmt.Await)
                    {
                        // Covered elsewhere; just ignore here.
                    }
                    else if (node is ForInStatement forIn)
                    {
                        ValidateForInOfLeft(forIn.Left, isForOf: false, result);
                    }
                    else if (node is ForOfStatement forOf)
                    {
                        ValidateForInOfLeft(forOf.Left, isForOf: true, result);
                    }
                    break;

                case BreakStatement breakStmt:
                    ValidateBreakStatement(breakStmt, ctx, result);
                    break;

                case ContinueStatement continueStmt:
                    ValidateContinueStatement(continueStmt, ctx, result);
                    break;
            }
        }, exitNode =>
        {
            // Function boundaries restore previous context.
            if (exitNode is FunctionDeclaration or FunctionExpression or ArrowFunctionExpression)
            {
                if (functionContexts.Count > 1)
                {
                    functionContexts.Pop();
                }
                return;
            }

            var ctx = functionContexts.Peek();

            switch (exitNode)
            {
                case LabeledStatement:
                    if (ctx.LabelStack.Count > 0)
                    {
                        var entry = ctx.LabelStack.Pop();
                        ctx.ActiveLabelNames.Remove(entry.Name);
                    }
                    break;

                case SwitchStatement:
                    if (ctx.BreakableStack.Count > 0)
                    {
                        ctx.BreakableStack.Pop();
                    }
                    break;

                case DoWhileStatement:
                case WhileStatement:
                case ForStatement:
                case ForInStatement:
                case ForOfStatement:
                    if (ctx.BreakableStack.Count > 0)
                    {
                        ctx.BreakableStack.Pop();
                    }
                    if (ctx.IterationDepth > 0)
                    {
                        ctx.IterationDepth--;
                    }
                    break;
            }
        });
    }

    private static void ValidateBreakStatement(BreakStatement breakStmt, EarlyErrorContext ctx, ValidationResult result)
    {
        if (breakStmt.Label == null)
        {
            if (ctx.BreakableStack.Count == 0)
            {
                AddError(result, "Illegal break statement (not inside a loop or switch)", breakStmt);
            }
            return;
        }

        var labelName = breakStmt.Label.Name;
        if (!TryFindLabelTarget(labelName, ctx, out _))
        {
            AddError(result, $"Undefined label '{labelName}' in break statement", breakStmt.Label);
        }
    }

    private static void ValidateContinueStatement(ContinueStatement continueStmt, EarlyErrorContext ctx, ValidationResult result)
    {
        if (continueStmt.Label == null)
        {
            if (ctx.IterationDepth == 0)
            {
                AddError(result, "Illegal continue statement (not inside a loop)", continueStmt);
            }
            return;
        }

        var labelName = continueStmt.Label.Name;
        if (!TryFindLabelTarget(labelName, ctx, out var targetStatement))
        {
            AddError(result, $"Undefined label '{labelName}' in continue statement", continueStmt.Label);
            return;
        }

        if (!IsIterationStatement(targetStatement))
        {
            AddError(result, $"Illegal continue target '{labelName}' (label does not refer to a loop)", continueStmt.Label);
        }
    }

    private static bool TryFindLabelTarget(string labelName, EarlyErrorContext ctx, out Node? targetStatement)
    {
        foreach (var entry in ctx.LabelStack)
        {
            if (string.Equals(entry.Name, labelName, StringComparison.Ordinal))
            {
                targetStatement = entry.TargetStatement;
                return true;
            }
        }

        targetStatement = null;
        return false;
    }

    private static bool IsIterationStatement(Node? node)
    {
        return node is DoWhileStatement
            or WhileStatement
            or ForStatement
            or ForInStatement
            or ForOfStatement;
    }

    private static void ValidateForInOfLeft(Node left, bool isForOf, ValidationResult result)
    {
        // Spec-level early error: for-in/of variable declarations must not have initializers
        // and must declare exactly one binding.
        if (left is VariableDeclaration vd)
        {
            if (vd.Declarations.Count != 1)
            {
                AddError(result,
                    $"Invalid {(isForOf ? "for...of" : "for...in")} head: exactly one variable declarator is required",
                    vd);
                return;
            }

            var decl = vd.Declarations[0];
            if (decl.Init != null)
            {
                AddError(result,
                    $"Invalid {(isForOf ? "for...of" : "for...in")} head: variable declarator cannot have an initializer",
                    decl.Init);
            }

            return;
        }

        // Defensive: Acornima usually won't produce these, but if it does, treat as early error.
        if (left is AssignmentExpression)
        {
            AddError(result,
                $"Invalid {(isForOf ? "for...of" : "for...in")} head: assignment is not allowed in the loop target",
                left);
        }
    }

    private static void AddError(ValidationResult result, string message, Node node)
    {
        var loc = node.Location.Start;
        result.Errors.Add($"{message} (line {loc.Line}, col {loc.Column})");
        result.IsValid = false;
    }

    private enum BreakableKind
    {
        Iteration,
        Switch
    }

    private sealed record LabelEntry(string Name, Node TargetStatement, Node LabelNode);

    private sealed class EarlyErrorContext
    {
        public int IterationDepth;
        public Stack<BreakableKind> BreakableStack { get; } = new();
        public Stack<LabelEntry> LabelStack { get; } = new();
        public HashSet<string> ActiveLabelNames { get; } = new(StringComparer.Ordinal);
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

    private static void ValidateGenerators(Node ast, ValidationResult result)
    {
        var generatorFunctionDepth = 0;
        var visited = new HashSet<Node>(ReferenceEqualityComparer<Node>.Default);

        void WalkForGenerators(Node? node)
        {
            if (node == null) return;
            if (!visited.Add(node)) return;

            bool isGeneratorFunction = node switch
            {
                FunctionDeclaration fd => fd.Generator,
                FunctionExpression fe => fe.Generator,
                // Arrow functions cannot be generators.
                _ => false
            };

            bool isAsyncGeneratorFunction = node switch
            {
                FunctionDeclaration fd => fd.Async && fd.Generator,
                FunctionExpression fe => fe.Async && fe.Generator,
                _ => false
            };

            if (isAsyncGeneratorFunction)
            {
                result.Errors.Add($"Async generators (async function*) are not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
            }

            if (isGeneratorFunction)
            {
                generatorFunctionDepth++;
            }

            if (node is YieldExpression ye)
            {
                if (generatorFunctionDepth == 0)
                {
                    result.Errors.Add($"The 'yield' keyword is only valid inside generator functions (line {node.Location.Start.Line})");
                    result.IsValid = false;
                }
            }

            // Walk children (reflection-based, consistent with ValidateAsyncAwait)
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
                    WalkForGenerators(childNode);
                }
                else if (value is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        if (item is Node itemNode)
                        {
                            WalkForGenerators(itemNode);
                        }
                    }
                }
            }

            if (isGeneratorFunction)
            {
                generatorFunctionDepth--;
            }
        }

        WalkForGenerators(ast);
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

    private void ValidateProperty(Node node, ValidationResult result, ValidationContext currentContext)
    {
        if (node is Property prop)
        {
            // Computed keys are supported in object literals, but not yet supported in object binding patterns.
            if (prop.Computed && currentContext.InObjectPattern)
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
            if (method.Key is PrivateIdentifier)
            {
                result.Errors.Add($"Private methods in classes are not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
                return;
            }

            if (method.Computed || method.Key is not Identifier)
            {
                result.Errors.Add($"Computed/non-identifier method names in classes are not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
                return;
            }

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

    private void ValidatePropertyDefinition(Node node, ValidationResult result)
    {
        if (node is PropertyDefinition pdef
            && (pdef.Computed || (pdef.Key is not Identifier && pdef.Key is not PrivateIdentifier)))
        {
            result.Errors.Add($"Computed/non-identifier class field names are not yet supported (line {node.Location.Start.Line})");
            result.IsValid = false;
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
        public bool AllowsSuper { get; set; }
        public Node? ScopeOwner { get; set; }
        // Track the FunctionExpression that is the direct body of a MethodDefinition
        // so we don't incorrectly treat it as a nested function
        public Node? MethodDefinitionFunctionValue { get; set; }
        public bool InObjectPattern { get; set; }
        public bool InDerivedClass { get; set; }
    }
}
