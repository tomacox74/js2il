# Plan: npm Package Imports (Node/CommonJS Resolution)

## Goal
Enable JS2IL to compile and execute code that imports npm packages in two ways:

1. **CLI entry by npm module id**
   - Example (Hosting.Domino):
     - Today: `js2il node_modules/@mixmark-io/domino/lib/index.js`
     - Target: `js2il --moduleid @mixmark-io/domino`
     - `@mixmark-io/domino` resolves to the package’s `.js` entry (e.g. `lib/index.js`).

2. **`require()` of npm packages inside JavaScript modules**
   - Example: `require('turndown')`
   - Target resolution at compile time: resolve to a concrete `.js` file under `node_modules` (e.g. `../../node_modules/turndown/lib/turndown.cjs.js`).

The **core requirement** is to resolve module IDs to physical JavaScript files at **compile time** using Node.js/CommonJS rules as closely as practical.

## Non-Goals (for this feature increment)
- Implementing ESM `import` / `export` semantics.
- Supporting non-`.js` runtime loading (`.json`, `.node`, `.mjs`, `.cjs`) as compile targets.
  - If Node resolution leads to a non-`.js` target, JS2IL should report a **compile-time error**.
- Reproducing Node’s filesystem resolution at runtime.
  - JS2IL is a compiler; runtime should resolve *types*, not walk `node_modules`.

## Constraints / Principles
1. **Resolution mirrors Node.js CommonJS as closely as possible**
   - Baseline specification reference: https://nodejs.org/api/modules.html
2. **npm package sources compile into .NET types just like local modules**
3. **Avoid name collisions** between local modules and npm packages
   - Use a dedicated CLR namespace for packages.
4. **Runtime must not replicate compile-time Node resolution**
   - Runtime should use compiler-emitted metadata to map module IDs to CLR types.
5. **Hosting APIs must continue to work**
   - `JsEngine.LoadModule(moduleId)` should support package IDs (e.g. `turndown`, `@mixmark-io/domino`).

## Current State (relevant code)
- Compile-time module graph loading: `Js2IL/ModuleLoader.cs`
  - Today it only follows `require()` dependencies for specifiers starting with `.` or `/`.
- Module id conventions and sanitization: `JavaScriptRuntime/CommonJS/ModuleName.cs`
- Runtime require: `JavaScriptRuntime/CommonJS/Require.cs`
  - Bare specifiers are treated as **Node core modules** only.
- Host module loading: `JavaScriptRuntime/Hosting/JsEngine.cs`, `JavaScriptRuntime/Hosting/JsRuntimeInstance.cs`
- Assembly module-id manifest: `Js2IL.Runtime.JsCompiledModuleAttribute`

## Design Overview
### High-level architecture
- **Compiler** resolves module IDs/specifiers → **physical `.js` file paths**.
- Compiler compiles resolved files to CLR types.
- Compiler emits an **assembly-level mapping** from logical module IDs → CLR type names.
- **Runtime** resolves `require()` and `LoadModule()` by consulting the mapping.

This keeps Node-style resolution in one place (compile time) while enabling runtime loading without filesystem/package.json logic.

## Module Resolution (Compiler)
Introduce a compiler-side resolver that implements Node/CommonJS rules.

### Resolver responsibilities
Given:
- `specifier` (string literal argument to `require()` or `--moduleid`)
- `importerFile` (the current module file path, when resolving a `require()`)
- `baseDirectory` (directory of importer or working directory for CLI)

Return:
- a single canonical **absolute** file path to a `.js` file
- or a diagnostic explaining why resolution failed

### Resolution categories
1. **Relative / absolute specifiers**
   - Relative: `./x`, `../x`
   - Absolute local (POSIX-style): `/x`

   Probe order (Node-style, compile-time):
   - If specifier points to a file:
     - exact file
     - exact file + `.js`
   - If specifier points to a directory:
     - directory `package.json` (resolve `exports`/`main`)
     - `index.js`

2. **Bare specifiers (npm package IDs and subpaths)**
   - Examples:
     - `turndown`
     - `@mixmark-io/domino`
     - `@scope/pkg/lib/foo`
     - `pkg/sub/path`

   Steps:
   - Parse into:
     - package name: `pkg` or `@scope/pkg`
     - optional subpath: `/sub/path`
   - Starting from `baseDirectory`, walk up the directory tree:
     - At each level, look for `node_modules/<packageName>`
     - If found, resolve:
       - If there is a subpath: resolve within package root
       - Else: resolve package entry using `package.json`

### package.json handling
Target behavior is **CommonJS** resolution with modern `exports` support.

- If `package.json` has `exports`:
  - Use `exports` to resolve `.` (package root) and subpaths.
  - Treat resolution as `require()` conditions; support at least:
    - `require`
    - `node`
    - `default`
  - If the resolved target is not `.js`, emit a compile-time error.

- Else if `package.json` has `main`:
  - Resolve `main` and apply file/directory probing rules.

- Else:
  - Fall back to `index.js` under package root.

### File type policy
- JS2IL only compiles `.js` sources for npm resolution.
- If resolution results in:
  - `.json`, `.node`, `.mjs`, `.cjs` (or any non-`.js`) → **compile-time error**.

> Note: Many packages ship `.cjs` files. This plan keeps the initial scope strict per current requirements.

### Canonical module IDs
Resolution produces a physical file path, but JS2IL needs a **logical module id** to:
- uniquely represent the module inside the compilation
- allow runtime `require()` lookups by ID

The plan introduces two IDs per module:

1. **Logical module ID (human / Node-ish)**
   - Examples:
     - `./index` (for root module)
     - `./utils/parse`
     - `turndown`
     - `@mixmark-io/domino`
     - `@mixmark-io/domino/lib/index`

2. **CLR type identity (internal, collision-safe)**
   - Namespace:
     - local modules: `Modules`
     - npm package modules: `Packages`
   - Type name:
     - derived from logical module ID via a **reversible encoding** (see below)

### Collision avoidance strategy
Current `ModuleName.SanitizeModuleId()` replaces many characters with `_`, which is lossy.
That is not sufficient for npm scopes/subpaths.

Plan:
- For **package modules**, use namespace `Packages`.
- Use a **reversible encoding** of the logical module ID into a valid CLR identifier:
  - Output must contain only `[A-Za-z0-9_]` and not start with a digit.
  - Encoding must be deterministic and reversible to avoid collisions.

Implementation idea:
- Encode bytes of UTF-8 module ID as hex with a sentinel prefix, e.g.
  - `pkg_` + `x` + hex bytes
- Or encode each non-identifier char as `_uXXXX_` (Unicode code point), with an escape rule.

Exact encoding can be chosen based on simplicity + length constraints.

## Module Graph Loading Changes
Update the compiler’s dependency walk to use the new resolver.

### Changes in ModuleLoader
- Stop skipping bare specifiers.
- For each `require('<string>')`:
  - resolve to a `.js` file path using the Node resolver
  - add that resolved file to the module cache and continue recursively

### Validation changes
`require()` must continue to require a **static string literal** for dependency discovery.

Update validation so that:
- bare specifiers are permitted (they are no longer “unsupported” by default)
- core node modules remain permitted
- unresolved modules are surfaced via module loader diagnostics (compile-time resolution failure), not via AST validation.

## Runtime Behavior (Require)
Runtime should not implement Node module resolution.

### Assembly-emitted metadata mapping
Add a new assembly-level attribute that maps:
- logical module ID → CLR type full name

Example:
- `moduleId = "turndown"`
- `typeName = "Packages.<encoded>"`

This is distinct from the existing `JsCompiledModuleAttribute(moduleId)` manifest.

### Runtime lookup rules
When `require(specifier)` is called:

1. Normalize specifier (`node:` prefix trimming, slashes)
2. If specifier is a Node core module:
   - load via `NodeModuleRegistry` (existing behavior)
3. Else resolve via compiled-module mapping:
   - Try exact match for the module ID
   - Apply minimal CommonJS probing in terms of *module IDs* only (not filesystem):
     - try `<id>`
     - try `<id>.js`
     - try `<id>/index`
     - try `<id>/index.js`

This preserves the common pattern `require('./foo')` where `foo/index.js` exists.

### Local relative requires
Runtime already normalizes dot segments against the parent module ID.
That stays, but the final lookup uses the mapping instead of `Modules.<sanitized>` guessing.

## Hosting: JsEngine.LoadModule
Hosts should be able to load modules by logical module ID, including packages.

Plan:
- `JsEngine.LoadModule(assembly, moduleId)` should:
  - for bare package-like IDs: pass through to runtime require as-is
  - for local IDs: keep existing normalization (`./` prefix)

The runtime mapping makes `LoadModule("turndown")` and `LoadModule("@mixmark-io/domino")` possible without host-side Node logic.

## CLI: `--moduleid`
Add a new CLI option:
- `js2il --moduleid <npmModuleId> [--output ...] [options]`

Behavior:
- Mutually exclusive with positional `InputFile`.
- Resolve `moduleid` to an entry `.js` file using the same compiler resolver.
  - Base directory for resolution: `Environment.CurrentDirectory`.
- Compile the resolved file as the root module.

## Samples
### Hosting.Domino
Update the sample guidance:
- compile via `--moduleid @mixmark-io/domino`
- host can load by module id

## Tests
Add tests that validate both resolution and execution.

### Resolution tests
- Directory fixture with nested folders and multiple `node_modules` locations.
- Verify upward directory walking resolves the nearest `node_modules`.
- Verify:
  - bare package: `require('pkg')`
  - scoped package: `require('@scope/pkg')`
  - subpath: `require('pkg/sub/path')`
  - directory fallback: `index.js`
  - `package.json` `main` resolution
  - `package.json` `exports` root and subpath mapping

### Negative tests
- `package.json` resolves to `.json` / `.cjs` / `.mjs` / `.node` → compile-time error.

### Runtime tests
- `require('pkg')` returns expected exports.
- Ensure Node core modules still take precedence over local/package with same name.

## Real-World Test Fixtures (Public npm packages)
To test resolution against real packages without committing `node_modules`, use the fixture installer:

- Install (into gitignored `test_output/npm-fixtures`): `npm run fixtures:npm`
- Clean + reinstall: `node scripts/npm/installNpmFixtures.js --clean`

The curated fixtures list lives in `scripts/npm/fixtures.json` and intentionally favors tiny CommonJS-friendly packages:
- `isarray@2.0.5` (simple `main`)
- `inherits@2.0.4` (simple `main`)
- `wrappy@1.0.2` (simple `main`)
- `once@1.4.0` (transitive dependency: `wrappy`)
- `ms@2.1.3` (simple `main`)
- `get-intrinsic@1.2.4` (uses `exports` for `.`)
- `kleur@4.1.5` (uses `exports` including subpath export `./colors`)

These fixtures are good candidates for future end-to-end tests like:
- `require('once')` (bare require + transitive)
- `require('kleur/colors')` (exports subpath)

## Implementation Checklist
1. Add `NodeModuleResolver` in compiler layer (uses `IFileSystem`).
2. Update `ModuleLoader` to resolve all `require()` specifiers via resolver.
3. Update AST validator to permit bare requires.
4. Add new mapping attribute (moduleId → typeName) and emit it in `AssemblyGenerator`.
5. Add `Packages` namespace emission for modules under `node_modules`.
6. Update runtime `Require` to:
   - try Node core module
   - else resolve via mapping (with minimal id-level probing)
7. Add CLI `--moduleid`.
8. Update hosting `LoadModule` normalization for package IDs.
9. Add tests + update Hosting.Domino docs.

## Open Questions
- How strict should `exports` handling be initially (full conditional exports vs minimal subset)?
  - Plan baseline: support `require`/`node`/`default` conditions.
- Maximum encoded type name length (very deep subpaths) and any need for shortening.
  - If needed, introduce a reversible encoding for common cases and fall back to `hash + embedded metadata` for extreme paths.
