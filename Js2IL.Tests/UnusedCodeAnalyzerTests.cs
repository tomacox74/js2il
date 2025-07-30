using Js2IL.Services;
using Acornima.Ast;

namespace Js2IL.Tests;

public class UnusedCodeAnalyzerTests
{
    private readonly JavaScriptParser _parser = new();
    private readonly UnusedCodeAnalyzer _analyzer = new();

    [Fact]
    public void Analyze_ShouldDetectUnusedFunction()
    {
        // Arrange
        var jsCode = @"
            function usedFunction() {
                console.log('used');
            }
            
            function unusedFunction() {
                console.log('unused');
            }
            
            usedFunction();
        ";
        
        var ast = _parser.ParseJavaScript(jsCode);

        // Act
        var result = _analyzer.Analyze(ast);

        // Assert
        Assert.Contains("unusedFunction", result.UnusedFunctions);
        Assert.DoesNotContain("usedFunction", result.UnusedFunctions);
    }

    [Fact]
    public void Analyze_ShouldDetectUnusedVariable()
    {
        // Arrange
        var jsCode = @"
            var usedVariable = 10;
            var unusedVariable = 20;
            
            console.log(usedVariable);
        ";
        
        var ast = _parser.ParseJavaScript(jsCode);

        // Act
        var result = _analyzer.Analyze(ast);

        // Assert
        Assert.Contains("unusedVariable", result.UnusedVariables);
        Assert.DoesNotContain("usedVariable", result.UnusedVariables);
    }

    [Fact]
    public void Analyze_ShouldDetectUnusedProperty()
    {
        // Arrange
        var jsCode = @"
            var obj = {
                usedProperty: 'used',
                unusedProperty: 'unused'
            };
            
            console.log(obj.usedProperty);
        ";
        
        var ast = _parser.ParseJavaScript(jsCode);

        // Act
        var result = _analyzer.Analyze(ast);

        // Assert
        Assert.Contains("unusedProperty", result.UnusedProperties);
        Assert.DoesNotContain("usedProperty", result.UnusedProperties);
    }

    [Fact]
    public void Analyze_ShouldHandleMethodCalls()
    {
        // Arrange
        var jsCode = @"
            var obj = {
                usedMethod: function() { return 'used'; },
                unusedMethod: function() { return 'unused'; }
            };
            
            obj.usedMethod();
        ";
        
        var ast = _parser.ParseJavaScript(jsCode);

        // Act
        var result = _analyzer.Analyze(ast);

        // Assert
        Assert.Contains("unusedMethod", result.UnusedProperties);
        Assert.DoesNotContain("usedMethod", result.UnusedProperties);
    }

    [Fact]
    public void Analyze_ShouldHandleFunctionReferences()
    {
        // Arrange
        var jsCode = @"
            function referencedFunction() {
                return 'referenced';
            }
            
            function unusedFunction() {
                return 'unused';
            }
            
            var callback = referencedFunction;
            callback();
        ";
        
        var ast = _parser.ParseJavaScript(jsCode);

        // Act
        var result = _analyzer.Analyze(ast);

        // Assert
        Assert.Contains("unusedFunction", result.UnusedFunctions);
        Assert.DoesNotContain("referencedFunction", result.UnusedFunctions);
    }

    [Fact]
    public void Analyze_EmptyProgram_ShouldReturnNoUnusedItems()
    {
        // Arrange
        var jsCode = "";
        var ast = _parser.ParseJavaScript(jsCode);

        // Act
        var result = _analyzer.Analyze(ast);

        // Assert
        Assert.Empty(result.UnusedFunctions);
        Assert.Empty(result.UnusedVariables);
        Assert.Empty(result.UnusedProperties);
        Assert.Contains("No unused code detected", result.Warnings);
    }
}