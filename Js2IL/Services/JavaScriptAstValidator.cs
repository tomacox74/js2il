using Acornima.Ast;

namespace Js2IL.Services;

public class JavaScriptAstValidator : IAstValidator
{
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
                    result.Errors.Add($"Class declarations are not yet supported (line {node.Location.Start.Line})");
                    result.IsValid = false;
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

                case NodeType.ArrowFunctionExpression:
                    result.Warnings.Add($"Arrow functions are experimental (line {node.Location.Start.Line})");
                    break;
            }
        });

        return result;
    }
} 