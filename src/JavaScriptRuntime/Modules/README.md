# Module runtime layout

Runtime support for JavaScript modules is split by module standard so that
CommonJS-specific and ECMAScript-module-specific code stay separate, and any
code that must bridge the two standards lives in a single neutral place.

## Directories and namespaces

- `CommonJS/` -- namespace `JavaScriptRuntime.Modules.CommonJS`
  CommonJS-only runtime: the `Module` object, `require` resolution and the
  require function adapter, module execution, the compiled-module main delegate
  contract, module execution context (`__filename`, `__dirname`, and bound
  `require`), the module cache, and CommonJS-specific errors.
  Files: `Module.cs`, `Require.cs`, `ModuleExecutor.cs`, `ModuleParameters.cs`,
  `RequireFunctionTarget.cs`, `ModuleNotFoundError.cs`, `ModuleCache.cs`,
  `ModuleContext.cs`.

- `ESM/` -- namespace `JavaScriptRuntime.Modules.ESM`
  ECMAScript-module-only runtime: the static-module linker with live binding
  cells (`EsModuleLinker`, `EsModuleBinding`) and dynamic `import()` support.
  Files: `EsModuleLinker.cs`, `DynamicImport.cs`.

- `Shared/` -- namespace `JavaScriptRuntime.Modules.Shared`
  Neutral module infrastructure used by both standards, plus the code that
  translates between them. Module id/specifier normalization (`ModuleName`),
  the host-provided local modules assembly handle (`LocalModulesAssembly`), and
  the ESM/CommonJS interop bridge that projects `module.exports` into an ESM
  namespace (`EsModuleInterop`).
  Files: `ModuleName.cs`, `LocalModulesAssembly.cs`, `EsModuleInterop.cs`.

## Boundary rules

- Code specific to one module standard stays in that standard's directory
  (`CommonJS/` or `ESM/`). Do not put ESM code in the CommonJS directory or
  CommonJS code in the ESM directory.
- Code that translates or bridges across standards (for example projecting a
  CommonJS `module.exports` value into an ES module namespace object) belongs in
  `Shared/`, not in `CommonJS/` or `ESM/`.
- Neutral infrastructure that neither standard "owns" (module id normalization,
  the execution context, the local modules assembly handle) also belongs in
  `Shared/`.
- The directory is named `Modules` (not the ambiguous bare `Module`) because it
  covers both module systems and their shared support, not a single object.
