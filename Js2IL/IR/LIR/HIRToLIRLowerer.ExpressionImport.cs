using Js2IL.HIR;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerImportExpression(HIRImportExpression importExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // Lower the module specifier (validated to be a string literal)
        if (!TryLowerExpression(importExpr.Specifier, out var specifierTemp))
        {
            return false;
        }

        // Get the current module ID for relative path resolution
        // In CommonJS wrapper, __filename is available as a parameter
        var currentModuleIdTemp = CreateTempVariable();
        
        // Try to load __filename from the CommonJS wrapper environment
        // The __filename binding should be in the global scope for CommonJS modules
        if (_scope != null && _scope.Bindings.TryGetValue("__filename", out var filenameBinding))
        {
            // Create a Symbol from the binding and lower it as a variable read
            var filenameSymbol = new Symbol(filenameBinding);
            var filenameExpr = new HIRVariableExpression(filenameSymbol);
            
            if (!TryLowerExpression(filenameExpr, out currentModuleIdTemp))
            {
                // Fallback: use empty string if __filename loading fails
                _methodBodyIR.Instructions.Add(new LIRConstString(string.Empty, currentModuleIdTemp));
                DefineTempStorage(currentModuleIdTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
            }
        }
        else
        {
            // Fallback: use empty string for module ID
            _methodBodyIR.Instructions.Add(new LIRConstString(string.Empty, currentModuleIdTemp));
            DefineTempStorage(currentModuleIdTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        }

        // Emit the import call
        _methodBodyIR.Instructions.Add(new LIRCallImport(specifierTemp, currentModuleIdTemp, resultTempVar));
        
        // Import returns a Promise<object>
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        return true;
    }
}
