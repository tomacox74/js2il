using Acornima.Ast;
using System.Collections.Generic;
using System.Reflection;

namespace Js2IL.Services;

public class JavaScriptAstValidator : IAstValidator
{
    private static readonly Lazy<HashSet<string>> SupportedRequireModules = new(() =>
    {
        var set = new HashSet<string>();
        try
        {
            // Use a known runtime type to locate the assembly (Require lives in runtime assembly)
            var asm = typeof(JavaScriptRuntime.Require).Assembly;
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
                    // Allow spread; codegen handles it for array/object literals and calls.
                    break;

                case NodeType.CallExpression:
                    // Detect require(...) patterns. Only literal string specifiers are supported.
                    if (node is CallExpression call && call.Callee is Identifier id && id.Name == "require")
                    {
                        if (call.Arguments.Count == 0)
                        {
                            result.Errors.Add($"require() called without arguments (line {node.Location.Start.Line})");
                            result.IsValid = false;
                        }
                        else if (call.Arguments[0] is Literal lit && lit.Value is string modName)
                        {
                            if (!SupportedRequireModules.Value.Contains(modName))
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
                    break;
            }
        });

        return result;
    }
} 