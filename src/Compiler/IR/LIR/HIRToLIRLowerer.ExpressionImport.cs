using Js2IL.HIR;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private static Scope? GetRootScope(Scope? scope)
    {
        if (scope == null)
        {
            return null;
        }

        var root = scope;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        return root;
    }

    private static BindingInfo? TryResolveBinding(Scope? scope, string name)
    {
        var current = scope;
        while (current != null)
        {
            if (current.Bindings.TryGetValue(name, out var binding))
            {
                return binding;
            }

            current = current.UsesGlobalScopeSemantics ? null : current.Parent;
        }

        return null;
    }

    private bool TryLowerCurrentModuleId(out TempVariable currentModuleIdTemp)
    {
        var root = GetRootScope(_scope);
        if (!string.IsNullOrWhiteSpace(root?.ModuleId))
        {
            currentModuleIdTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString(root.ModuleId!, currentModuleIdTemp));
            DefineTempStorage(currentModuleIdTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
            return true;
        }

        var filenameBinding = TryResolveBinding(_scope, "__filename");
        if (filenameBinding != null
            && TryLowerExpression(new HIRVariableExpression(new Symbol(filenameBinding)), out currentModuleIdTemp))
        {
            currentModuleIdTemp = EnsureObject(currentModuleIdTemp);
            return true;
        }

        if (!string.IsNullOrWhiteSpace(root?.AstNode.Location.SourceFile))
        {
            currentModuleIdTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString(root.AstNode.Location.SourceFile, currentModuleIdTemp));
            DefineTempStorage(currentModuleIdTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
            return true;
        }

        currentModuleIdTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(string.Empty, currentModuleIdTemp));
        DefineTempStorage(currentModuleIdTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        return true;
    }

    private bool TryLowerCurrentModulePath(out TempVariable currentModulePathTemp)
    {
        var filenameBinding = TryResolveBinding(_scope, "__filename");
        if (filenameBinding != null
            && TryLowerExpression(new HIRVariableExpression(new Symbol(filenameBinding)), out currentModulePathTemp))
        {
            currentModulePathTemp = EnsureObject(currentModulePathTemp);
            return true;
        }

        var root = GetRootScope(_scope);
        if (!string.IsNullOrWhiteSpace(root?.AstNode.Location.SourceFile))
        {
            currentModulePathTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString(root.AstNode.Location.SourceFile, currentModulePathTemp));
            DefineTempStorage(currentModulePathTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
            return true;
        }

        currentModulePathTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(currentModulePathTemp));
        DefineTempStorage(currentModulePathTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryLowerImportExpression(HIRImportExpression importExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // Lower the module specifier (validated to be a string literal)
        if (!TryLowerExpression(importExpr.Specifier, out var specifierTemp))
        {
            return false;
        }

        if (!TryLowerCurrentModuleId(out var currentModuleIdTemp))
        {
            return false;
        }

        // Emit the import call
        _methodBodyIR.Instructions.Add(new LIRCallImport(specifierTemp, currentModuleIdTemp, resultTempVar));
        
        // Import returns a Promise<object>
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        return true;
    }
}
