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
        
        // Visit all nodes in the AST
        var walker = new AstWalker();
        walker.Visit(ast, node =>
        {
            // Check for unsupported features
            switch (node.Type)
            {
                case NodeType.ClassDeclaration:
                case NodeType.ClassExpression:
                    // Classes are supported: no validation error or warning.
                    // But check for unsupported class features
                    ValidateClassBody(node, result);
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

                case NodeType.SpreadElement:
                    // Spread is allowed in array/object literals, but not in function call arguments
                    ValidateSpreadElement(node, result);
                    break;

                case NodeType.RestElement:
                    // Rest parameters (...args) and rest properties are not supported
                    result.Errors.Add($"Rest parameters/properties are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.ForInStatement:
                    result.Errors.Add($"for...in loops are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
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
                    result.Errors.Add($"Labeled statements are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.DebuggerStatement:
                    result.Errors.Add($"The 'debugger' statement is not supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.MetaProperty:
                    // new.target and import.meta
                    if (node is MetaProperty metaProp)
                    {
                        result.Errors.Add($"new.target/import.meta are not yet supported (line {node.Location.Start.Line})");
                        result.IsValid = false;
                    }
                    break;

                case NodeType.Super:
                    result.Errors.Add($"The 'super' keyword is not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.ArrayPattern:
                    // Array destructuring is not supported
                    result.Errors.Add($"Array destructuring is not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                    break;

                case NodeType.ObjectPattern:
                    // Object destructuring in declarations/parameters is supported,
                    // but check for nested patterns and rest properties
                    ValidateObjectPattern(node, result);
                    break;

                case NodeType.VariableDeclarator:
                    // Check for array destructuring patterns in variable declarations
                    ValidateVariableDeclarator(node, result);
                    break;

                case NodeType.AssignmentExpression:
                    // Check for destructuring assignment (not in declarations)
                    ValidateAssignmentExpression(node, result);
                    break;

                case NodeType.Property:
                    // Check for computed properties, getters/setters in object literals
                    // Also check for nested destructuring when used as AssignmentProperty
                    ValidateProperty(node, result);
                    break;

                case NodeType.MethodDefinition:
                    // Check for getters/setters and static members in classes
                    ValidateMethodDefinition(node, result);
                    break;

                case NodeType.PropertyDefinition:
                    // Check for static class fields
                    ValidatePropertyDefinition(node, result);
                    break;

                case NodeType.CallExpression:
                    // Detect require(...) patterns and spread in call arguments
                    ValidateCallExpression(node, result);
                    break;
            }
        });

        return result;
    }

    private void ValidateClassBody(Node node, ValidationResult result)
    {
        // Validation for class-specific features is handled by MethodDefinition and PropertyDefinition
    }

    private void ValidateSpreadElement(Node node, ValidationResult result)
    {
        // SpreadElement parent check - if inside a CallExpression arguments, it's not supported
        // The walker visits nodes depth-first, so we can't easily check parent here
        // Instead, we check in CallExpression validation
    }

    private void ValidateVariableDeclarator(Node node, ValidationResult result)
    {
        if (node is VariableDeclarator decl)
        {
            // Check for array pattern in variable declarations
            if (decl.Id is ArrayPattern)
            {
                result.Errors.Add($"Array destructuring is not yet supported (line {node.Location.Start.Line})");
                result.IsValid = false;
            }
        }
    }

    private void ValidateObjectPattern(Node node, ValidationResult result)
    {
        if (node is ObjectPattern pattern)
        {
            foreach (var prop in pattern.Properties)
            {
                // Check for nested destructuring
                if (prop is AssignmentProperty assignProp)
                {
                    if (assignProp.Value is ObjectPattern || assignProp.Value is ArrayPattern)
                    {
                        result.Errors.Add($"Nested destructuring patterns are not yet supported (line {node.Location.Start.Line})");
                        result.IsValid = false;
                    }
                }
                // Check for rest properties
                if (prop is RestElement)
                {
                    result.Errors.Add($"Rest properties in destructuring are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
                }
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

    private void ValidatePropertyDefinition(Node node, ValidationResult result)
    {
        // Static class fields are supported - no validation errors needed
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
} 