using Jroc.HIR;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private void AttachClassLexicalPrivateBrand(Scope classScope, TempVariable constructor)
    {
        for (var scope = classScope.Parent; scope != null; scope = scope.Parent)
        {
            if (scope.ClassSemantics is not { } enclosingClass)
                continue;
            var ownerType = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetUserClassType(enclosingClass.RegistryClassName, ownerType));
            DefineTempStorage(ownerType, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
            if (!_classInitializationOwnerTempsByRegistryName.TryGetValue(enclosingClass.RegistryClassName, out var owner))
            {
                owner = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConstUndefined(owner));
                DefineTempStorage(owner, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            }
            var lexicalThis = CreateTempVariable();
            if (_isDerivedConstructor || _isLexicallyEnclosedByDerivedConstructor)
                _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                    nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis), [], lexicalThis));
            else
                _methodBodyIR.Instructions.Add(new LIRLoadThis(lexicalThis));
            DefineTempStorage(lexicalThis, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            var result = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                nameof(JavaScriptRuntime.RuntimeServices.SetClassLexicalPrivateBrand),
                [EnsureObject(constructor), ownerType, EnsureObject(owner), lexicalThis], result,
                [typeof(object), typeof(Type), typeof(object), typeof(object)]));
            DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return;
        }
    }

    private bool UsesInstancePrivateField(string owner, string name)
        => _classRegistry != null && _classRegistry.TryGetPrivateField(owner, name, out _)
            && !_classRegistry.TryGetStaticPrivateField(owner, name, out _)
            && name != "_scopes" && name != "__jroc_ctorReturn";

    private TempVariable EmitPrivateFieldOperation(string operation, string owner, string name, TempVariable receiver, TempVariable? value = null)
    {
        var ownerType = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRGetUserClassType(owner, ownerType));
        DefineTempStorage(ownerType, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
        var key = CreateStringConstant(name);
        var args = new List<TempVariable> { EnsureObject(receiver), ownerType, key };
        var types = new List<Type> { typeof(object), typeof(Type), typeof(string) };
        if (value.HasValue)
        {
            args.Add(EnsureObject(value.Value));
            types.Add(typeof(object));
        }
        var result = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(operation, args, result, types.ToArray()));
        DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return result;
    }
}
