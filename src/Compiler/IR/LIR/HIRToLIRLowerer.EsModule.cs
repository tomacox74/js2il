using System;
using System.Collections.Generic;
using Jroc.Services.ScopesAbi;
using Jroc.SymbolTables;

namespace Jroc.IR;

/// <summary>
/// Native static ES module lowering: module main prologue (export registration, <c>__esModule</c>
/// marking, source requires, and default/namespace import initialization), export write-mirroring,
/// and named-import read interception. All linking is emitted as direct runtime calls on
/// <c>EsModuleLinker</c>; no JavaScript helper functions or export getter closures are generated.
/// </summary>
public sealed partial class HIRToLIRLowerer
{
    private const string EsModuleIntrinsic = Jroc.IL.LIRToILCompiler.EsModuleIntrinsicName;

    private bool IsNativeEsModuleMain
        => _callableKind == CallableKind.ModuleMain
           && _scope is { Kind: ScopeKind.Global, EsModuleLink: not null };

    private TempVariable CreateEsModuleStringTemp(string value)
    {
        var temp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(value, temp));
        DefineTempStorage(temp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        return temp;
    }

    private TempVariable LoadModuleParameterAsObject(int jsParameterIndex)
    {
        var temp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRLoadParameter(jsParameterIndex, temp));
        DefineTempStorage(temp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return temp;
    }

    /// <summary>
    /// Emits the module main prologue for a native static ES module. Order matters: local exports are
    /// registered and the exports object is marked before source modules are required, so a self- or
    /// cyclic import observes the (possibly uninitialized) live accessors.
    /// </summary>
    private void EmitNativeEsModulePrologueIfNeeded()
    {
        if (!IsNativeEsModuleMain)
        {
            return;
        }

        var plan = _scope!.EsModuleLink!;
        var exportsTemp = LoadModuleParameterAsObject(0);

        // 1. Register each local export: installs a live, enumerable accessor over the binding cell.
        foreach (var exportName in plan.LocalExportNames)
        {
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
                EsModuleIntrinsic,
                nameof(JavaScriptRuntime.Modules.ESM.EsModuleLinker.RegisterLocalExport),
                new[]
                {
                    exportsTemp,
                    CreateEsModuleStringTemp(plan.ModuleId),
                    CreateEsModuleStringTemp(exportName)
                }));
        }

        // 2. Mark the exports object as an ES module (__esModule) for namespace/interop consumers.
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
            EsModuleIntrinsic,
            nameof(JavaScriptRuntime.Modules.ESM.EsModuleLinker.MarkEsModule),
            new[] { exportsTemp }));

        // 3. Require each source module (source order) and store into its hidden module-scope binding.
        if (plan.Requires.Count > 0)
        {
            var requireTemp = LoadModuleParameterAsObject(1);
            foreach (var require in plan.Requires)
            {
                var specifierTemp = CreateEsModuleStringTemp(require.Specifier);
                var moduleTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRCallRequire(requireTemp, specifierTemp, moduleTemp, ContractType: null));
                DefineTempStorage(moduleTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                if (!TryStoreToBinding(require.SourceBinding, moduleTemp, out _))
                {
                    throw new InvalidOperationException(
                        $"Failed to store required ES module source '{require.Specifier}'.");
                }
            }
        }

        // 4. Initialize default/namespace import bindings once from their source module object.
        foreach (var importInit in plan.ImportInits)
        {
            if (!TryLoadVariable(importInit.SourceBinding, out var sourceTemp))
            {
                throw new InvalidOperationException(
                    "Failed to load ES module source binding during import initialization.");
            }

            var resultTemp = CreateTempVariable();
            var methodName = importInit.Kind == EsModuleImportKind.Default
                ? nameof(JavaScriptRuntime.Modules.ESM.EsModuleLinker.GetDefault)
                : nameof(JavaScriptRuntime.Modules.ESM.EsModuleLinker.GetNamespace);
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                EsModuleIntrinsic,
                methodName,
                new[] { EnsureObject(sourceTemp) },
                resultTemp));
            DefineTempStorage(resultTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            if (!TryStoreToBinding(importInit.LocalBinding, resultTemp, out _))
            {
                throw new InvalidOperationException(
                    "Failed to store ES module default/namespace import binding.");
            }
        }

        // 5. Register re-exports of named imports as live forwarding accessors on the exports object.
        // These require the source module object, so they run after the source requires above.
        foreach (var reexport in plan.Reexports)
        {
            if (!TryLoadVariable(reexport.SourceBinding, out var reexportSourceTemp))
            {
                throw new InvalidOperationException(
                    $"Failed to load ES module source binding for re-export '{reexport.ExportName}'.");
            }

            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
                EsModuleIntrinsic,
                nameof(JavaScriptRuntime.Modules.ESM.EsModuleLinker.RegisterReexport),
                new[]
                {
                    exportsTemp,
                    CreateEsModuleStringTemp(reexport.ExportName),
                    EnsureObject(reexportSourceTemp),
                    CreateEsModuleStringTemp(reexport.ImportName)
                }));
        }

        // 6. Register namespace re-exports (`export * as ns from "mod"`) as live accessors resolving the
        // (stable) source module namespace object.
        foreach (var namespaceReexport in plan.NamespaceReexports)
        {
            if (!TryLoadVariable(namespaceReexport.SourceBinding, out var namespaceSourceTemp))
            {
                throw new InvalidOperationException(
                    $"Failed to load ES module source binding for namespace re-export '{namespaceReexport.ExportName}'.");
            }

            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
                EsModuleIntrinsic,
                nameof(JavaScriptRuntime.Modules.ESM.EsModuleLinker.RegisterNamespaceReexport),
                new[]
                {
                    exportsTemp,
                    CreateEsModuleStringTemp(namespaceReexport.ExportName),
                    EnsureObject(namespaceSourceTemp)
                }));
        }

        // 7. Register star re-exports (`export * from "mod"`) last so explicit local/named exports already
        // installed on the exports object take precedence over forwarded names.
        foreach (var starReexport in plan.StarReexports)
        {
            if (!TryLoadVariable(starReexport.SourceBinding, out var starSourceTemp))
            {
                throw new InvalidOperationException(
                    "Failed to load ES module source binding for star re-export.");
            }

            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
                EsModuleIntrinsic,
                nameof(JavaScriptRuntime.Modules.ESM.EsModuleLinker.RegisterStarReexports),
                new[]
                {
                    exportsTemp,
                    EnsureObject(starSourceTemp)
                }));
        }
    }

    /// <summary>
    /// Mirrors a write to an exported local binding into its runtime export cell(s) so ESM importers
    /// and CommonJS consumers observe live values. No-op for bindings that are not exported.
    /// </summary>
    private void MirrorEsModuleExport(BindingInfo binding, TempVariable value)
    {
        if (binding.EsModuleExports is not { Count: > 0 } exports)
        {
            return;
        }

        var boxedValue = EnsureObject(value);
        foreach (var export in exports)
        {
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
                EsModuleIntrinsic,
                nameof(JavaScriptRuntime.Modules.ESM.EsModuleLinker.SetLocalExport),
                new[]
                {
                    CreateEsModuleStringTemp(export.ModuleId),
                    CreateEsModuleStringTemp(export.ExportName),
                    boxedValue
                }));
        }
    }

    /// <summary>
    /// Lowers a read of a live named import binding to <c>EsModuleLinker.GetImport(source, name)</c>,
    /// re-reading the source module property on every access. Returns false when this binding is not a
    /// named import.
    /// </summary>
    private bool TryLowerNamedEsModuleImportRead(BindingInfo binding, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (binding.EsModuleImport is not { Kind: EsModuleImportKind.Named } namedImport)
        {
            return false;
        }

        if (!TryLoadVariable(namedImport.SourceModuleBinding, out var sourceTemp))
        {
            throw new InvalidOperationException(
                $"Failed to load ES module source binding for named import '{binding.Name}'.");
        }

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            EsModuleIntrinsic,
            nameof(JavaScriptRuntime.Modules.ESM.EsModuleLinker.GetImport),
            new[]
            {
                EnsureObject(sourceTemp),
                CreateEsModuleStringTemp(namedImport.ImportName ?? binding.Name)
            },
            resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }
}
