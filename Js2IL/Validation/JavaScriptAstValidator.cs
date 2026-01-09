using Acornima.Ast;
using Js2IL.Services;
using Js2IL.Utilities;
using System.Collections.Generic;
using System.Reflection;

namespace Js2IL.Validation;

public class JavaScriptAstValidator : IAstValidator
{
    private static readonly Lazy<HashSet<string>> SupportedRequireModules = new(() =>
    {
        var set = new HashSet<string>();
        try
        {
            // Use a known runtime type to locate the assembly (Require lives in runtime assembly)
            var asm = typeof(JavaScriptRuntime.Node.NodeModuleAttribute).Assembly;
            foreach (var t in asm.GetTypes())
            {
                if (!t.IsClass || t.IsAbstract) continue;
                var attr = t.GetCustomAttribute<JavaScriptRuntime.Node.NodeModuleAttribute>(false);
                if (attr != null && !string.IsNullOrWhiteSpace(attr.Name))
                {
                    set.Add(attr.Name);
                }
            }
        }
        catch { /* Ignore reflection errors; result will be empty set */ }
        return set;
    });

    public ValidationResult Validate(Acornima.Ast.Program ast)
    {
        var result = new ValidationResult { IsValid = true };
        
        // Track whether we're currently inside a class method or constructor,
        // and whether we're inside a nested function within a class method
        var contextStack = new Stack<ValidationContext>();
        contextStack.Push(new ValidationContext { IsInClassMethod = false, IsInNestedFunctionInClassMethod = false, MethodDefinitionFunctionValue = null });
        
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
                    IsInClassMethod = true, 
                    IsInNestedFunctionInClassMethod = false,
                    // Track the function expression that is the method body so we don't treat it as nested
                    MethodDefinitionFunctionValue = methodDef.Value
                });
            }
            // Push new context for nested functions within class methods (but not the method body itself)
            else if ((node is ArrowFunctionExpression || node is FunctionExpression || node is FunctionDeclaration) 
                     && currentContext.IsInClassMethod
                     && !ReferenceEquals(node, currentContext.MethodDefinitionFunctionValue))
            {
                contextStack.Push(new ValidationContext { IsInClassMethod = true, IsInNestedFunctionInClassMethod = true, MethodDefinitionFunctionValue = null });
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

                case NodeType.AwaitExpression:
                    result.Errors.Add($"Async/await is not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.YieldExpression:
                    result.Errors.Add($"Generators are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.RestElement:
                    // Rest parameters (...args) and rest properties are not supported
                    result.Errors.Add($"Rest parameters/properties are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.ForInStatement:
                    // for...in loops are supported (PL2.4)
                    break;

                case NodeType.SwitchStatement:
                    result.Errors.Add($"Switch statements are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
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
                    // 'this' is supported in class methods and constructors, but not elsewhere
                    // or in nested functions within class methods (issue #244)
                    if (!currentContext.IsInClassMethod)
                    {
                        result.Errors.Add($"The 'this' keyword is not yet supported outside of class methods and constructors (line {node.Location.Start.Line})");
                        result.IsValid = false;
                    }
                    else if (currentContext.IsInNestedFunctionInClassMethod)
                    {
                        result.Errors.Add($"Arrow functions or nested functions using 'this' inside class constructors/methods are not yet supported (line {node.Location.Start.Line})");
                        result.IsValid = false;
                    }
                    break;

                case NodeType.ArrayPattern:
                    // Array destructuring is not supported
                    result.Errors.Add($"Array destructuring is not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.ObjectPattern:
                    // Object destructuring in declarations/parameters is supported,
                    // but check for nested patterns (rest properties handled by RestElement case)
                    ValidateObjectPattern(node, result);
                    break;

                case NodeType.AssignmentExpression:
                    // Check for destructuring assignment (not in declarations)
                    ValidateAssignmentExpression(node, result);
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
            // Pop context when leaving class methods
            if (exitNode is MethodDefinition)
            {
                contextStack.Pop();
            }
            // Pop context when leaving nested functions within class methods
            else if ((exitNode is ArrowFunctionExpression || exitNode is FunctionExpression || exitNode is FunctionDeclaration)
                     && contextStack.Count > 1 && contextStack.Peek().IsInNestedFunctionInClassMethod)
            {
                contextStack.Pop();
            }
        });

        return result;
    }

    private void ValidateObjectPattern(Node node, ValidationResult result)
    {
        if (node is ObjectPattern pattern)
        {
            foreach (var prop in pattern.Properties)
            {
                // Check for nested destructuring (combined if statements per review comment)
                if (prop is AssignmentProperty assignProp &&
                    (assignProp.Value is ObjectPattern || assignProp.Value is ArrayPattern))
                {
                    result.Errors.Add($"Nested destructuring patterns are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                }
                // Note: Rest properties (RestElement) are handled by the main RestElement case
            }
        }
    }

    private void ValidateAssignmentExpression(Node node, ValidationResult result)
    {
        if (node is AssignmentExpression assign)
        {
            // Check for destructuring assignment (left side is a pattern, not an identifier)
            if (assign.Left is ArrayPattern || assign.Left is ObjectPattern)
            {
                result.Errors.Add($"Destructuring assignment is not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
            }
        }
    }

    private void ValidateProperty(Node node, ValidationResult result)
    {
        if (node is Property prop)
        {
            // Check for computed property names
            if (prop.Computed)
            {
                result.Errors.Add($"Computed property names are not yet supported (line {node.Location.Start.Line})");
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
                    var isLocalModule = modName.StartsWith(".") || modName.StartsWith("/");

                    if (!SupportedRequireModules.Value.Contains(modName) && !isLocalModule)
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
        int paramCount = node switch
        {
            FunctionDeclaration fd => fd.Params.Count,
            FunctionExpression fe => fe.Params.Count,
            ArrowFunctionExpression af => af.Params.Count,
            _ => 0
        };

        // Check for parameter count limit (issue #220)
        if (paramCount > 6)
        {
            result.Errors.Add($"Functions with more than 6 parameters are not yet supported (line {node.Location.Start.Line})");
            result.IsValid = false;
        }
    }

    private class ValidationContext
    {
        public bool IsInClassMethod { get; set; }
        public bool IsInNestedFunctionInClassMethod { get; set; }
        // Track the FunctionExpression that is the direct body of a MethodDefinition
        // so we don't incorrectly treat it as a nested function
        public Node? MethodDefinitionFunctionValue { get; set; }
    }
}
