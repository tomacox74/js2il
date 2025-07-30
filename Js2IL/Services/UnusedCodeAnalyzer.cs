using Acornima.Ast;

namespace Js2IL.Services;

public class UnusedCodeAnalysisResult
{
    public List<string> UnusedFunctions { get; set; } = new();
    public List<string> UnusedProperties { get; set; } = new();
    public List<string> UnusedVariables { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

public class UnusedCodeAnalyzer
{
    private readonly AstWalker _walker = new();
    
    // Track declared items
    private readonly HashSet<string> _declaredFunctions = new();
    private readonly HashSet<string> _declaredVariables = new();
    private readonly HashSet<string> _declaredProperties = new();
    
    // Track used items
    private readonly HashSet<string> _usedFunctions = new();
    private readonly HashSet<string> _usedVariables = new();
    private readonly HashSet<string> _usedProperties = new();

    public UnusedCodeAnalysisResult Analyze(Acornima.Ast.Program ast)
    {
        // Clear previous analysis
        ClearState();
        
        // First pass: collect all declarations
        _walker.Visit(ast, CollectDeclarations);
        
        // Second pass: collect all usages
        _walker.Visit(ast, CollectUsages);
        
        // Generate results
        return GenerateResults();
    }

    private void ClearState()
    {
        _declaredFunctions.Clear();
        _declaredVariables.Clear();
        _declaredProperties.Clear();
        _usedFunctions.Clear();
        _usedVariables.Clear();
        _usedProperties.Clear();
    }

    private void CollectDeclarations(Node node)
    {
        switch (node)
        {
            case FunctionDeclaration funcDecl:
                if (funcDecl.Id?.Name != null)
                {
                    _declaredFunctions.Add(funcDecl.Id.Name);
                }
                break;

            case VariableDeclarator varDeclarator:
                if (varDeclarator.Id is Identifier id)
                {
                    _declaredVariables.Add(id.Name);
                }
                break;

            case Property property:
                if (property.Key is Identifier propId)
                {
                    _declaredProperties.Add(propId.Name);
                }
                else if (property.Key is Literal literal && literal.Value is string propName)
                {
                    _declaredProperties.Add(propName);
                }
                break;

            case FunctionExpression funcExpr:
                if (funcExpr.Id?.Name != null)
                {
                    _declaredFunctions.Add(funcExpr.Id.Name);
                }
                break;
        }
    }

    private void CollectUsages(Node node)
    {
        switch (node)
        {
            case CallExpression callExpr:
                // Track function calls
                if (callExpr.Callee is Identifier calleeId)
                {
                    _usedFunctions.Add(calleeId.Name);
                }
                else if (callExpr.Callee is MemberExpression memberExpr)
                {
                    // Method call on object (e.g., obj.method())
                    if (memberExpr.Property is Identifier propId)
                    {
                        _usedFunctions.Add(propId.Name);
                    }
                }
                break;

            case Identifier identifier:
                // Track variable usage (but skip declarations which are handled separately)
                if (!IsInDeclarationContext(identifier))
                {
                    _usedVariables.Add(identifier.Name);
                    _usedFunctions.Add(identifier.Name); // Functions can be referenced as variables
                }
                break;

            case MemberExpression memberExpr:
                // Track property access
                if (memberExpr.Property is Identifier propIdentifier && !memberExpr.Computed)
                {
                    _usedProperties.Add(propIdentifier.Name);
                }
                break;

            case AssignmentExpression assignExpr:
                // Track property assignments
                if (assignExpr.Left is MemberExpression leftMember && 
                    leftMember.Property is Identifier leftPropIdentifier && 
                    !leftMember.Computed)
                {
                    _usedProperties.Add(leftPropIdentifier.Name);
                }
                break;
        }
    }

    private bool IsInDeclarationContext(Identifier identifier)
    {
        // This is a simplified check. In a more sophisticated analyzer,
        // we would need to track the context more carefully
        return false;
    }

    private UnusedCodeAnalysisResult GenerateResults()
    {
        var result = new UnusedCodeAnalysisResult();

        // Find unused functions
        foreach (var declaredFunc in _declaredFunctions)
        {
            if (!_usedFunctions.Contains(declaredFunc))
            {
                result.UnusedFunctions.Add(declaredFunc);
            }
        }

        // Find unused variables (excluding functions which are handled separately)
        foreach (var declaredVar in _declaredVariables)
        {
            if (!_usedVariables.Contains(declaredVar) && !_declaredFunctions.Contains(declaredVar))
            {
                result.UnusedVariables.Add(declaredVar);
            }
        }

        // Find unused properties
        foreach (var declaredProp in _declaredProperties)
        {
            if (!_usedProperties.Contains(declaredProp))
            {
                result.UnusedProperties.Add(declaredProp);
            }
        }

        // Add warnings for common edge cases
        if (result.UnusedFunctions.Count == 0 && result.UnusedProperties.Count == 0 && result.UnusedVariables.Count == 0)
        {
            result.Warnings.Add("No unused code detected");
        }

        return result;
    }
}