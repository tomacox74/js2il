---
name: node-module-contracts
description: Generate a new Node.js module interface contract or update an existing one from pinned official Node.js 24 LTS documentation, including intrinsic adapters, manual validation, and support docs.
tier: standard
applyTo: 'scripts/nodeContracts/**,src/JavaScriptRuntime/Node/**,src/JavaScriptRuntime/IJavaScript*.cs,tests/Jroc.NodeContracts.Tests/**,tests/Jroc.Tests/Node/**,docs/nodejs/**,package.json,CHANGELOG.md'
---

# Node Module Contracts

Use this skill when adding a generated contract for a Node module that does not
have one yet, or when changing an existing generated Node module contract.

## Goals

1. Generate the complete public module surface from the official Node.js 24 LTS
   documentation.
2. Keep the public contract independent from JROC's current implementation
   coverage.
3. Bind implemented intrinsic members directly at compile time.
4. Make unavailable intrinsic members fail explicitly with
   `NotImplementedException`.
5. Keep generation deterministic, reviewable, and manually validated.

## Non-Negotiable Rules

- The source of truth is the **official public Node.js 24 LTS documentation**.
  Use the machine-readable JSON published by Node.js for an exact pinned
  `v24.x.y` release.
- **Never use `docs/nodejs` as contract-generation input.** Those files describe
  JROC support and may intentionally be incomplete.
- Do not derive the public contract from JROC runtime classes. Runtime classes
  describe current implementation, not the complete Node API.
- Do not manually edit `*.Generated.cs` files.
- Do not omit a public Node member because JROC does not implement it.
- Generated contracts expose public ABI abstractions, not concrete runtime
  implementations:
  - arrays use `IJavaScriptArray`, not `JavaScriptRuntime.Array`;
  - promises use `IJavaScriptPromise`, not `JavaScriptRuntime.Promise`;
  - generated Node classes and stable shapes use public contract interfaces
    when that support exists;
  - host/CLR implementation types must not leak into public contracts.
- Generated intrinsic adapters use statically emitted calls. Do not introduce
  reflection, late-bound method discovery, or a generic invocation helper.
- Contract generation and contract tests are manual. Do not add them to the
  normal solution or every-run CI unless repository policy explicitly changes.
- A shared generator change changes its SHA-256 provenance and normally requires
  regenerating every contract produced by that generator.

## Current Layout

| Purpose | Location |
| --- | --- |
| Shared generator | `scripts/nodeContracts/generateNodeModuleInterface.js` |
| Pin and drift locks | `scripts/nodeContracts/*.node24.lock.json` |
| Parsing/runtime implementation overrides | `scripts/nodeContracts/*.node24.overrides.json` |
| Generated interfaces | `src/JavaScriptRuntime/Node/Contracts/I*Module.Generated.cs` |
| Generated intrinsic adapters | `src/JavaScriptRuntime/Node/*.I*Module.Generated.cs` |
| Runtime intrinsic implementations | `src/JavaScriptRuntime/Node/*.cs` |
| Manual contract tests | `tests/Jroc.NodeContracts.Tests` |
| Manual workflow documentation | `scripts/nodeContracts/README.md` |
| JROC support documentation | `docs/nodejs` |

The current generator has explicit modes for:

- `fs`;
- `fs/promises`;
- `console`;
- `path`.

Extend the shared generator for another module. Do not copy it into a
module-specific generator.

## Decide Which Workflow Applies

### Generate a new module contract

Use the new-contract workflow when no attributed interface exists in
`src/JavaScriptRuntime/Node/Contracts`.

### Update an existing module contract

Use the update workflow when changing any of:

- the pinned Node.js patch version or source document;
- the upstream public API surface;
- JavaScript-to-.NET type mappings;
- optional/rest/overload normalization;
- generated member metadata;
- intrinsic implementation mappings;
- generated adapter invocation rules;
- the shared generator itself.

An intrinsic implementation change that does not affect the contract may only
need an override-map, adapter, runtime, and test update. Still run the contract
check for that module.

## Phase 1: Establish the Official Input

1. Determine the canonical module specifier without the `node:` prefix, for
   example `path` or `fs/promises`.
2. Identify the official Node documentation JSON containing that module:
   `https://nodejs.org/docs/v24.18.1/api/<document>.json` for the current pin.
   A submodule can live in its parent document; `fs/promises` is in `fs.json`.
3. Use the same exact Node 24 LTS patch release as the other contracts unless
   the task explicitly advances the repository-wide pin.
4. Download the JSON and calculate its SHA-256.
5. Inspect its module/class/section structure and count the public members the
   generator will consume.
6. Record the exact version, URL, hash, and drift-detection counts in a lock
   file.

Example:

```sh
mkdir -p artifacts/nodeContracts
curl -fsSLo artifacts/nodeContracts/path.node24.json \
  https://nodejs.org/docs/v24.18.1/api/path.json
sha256sum artifacts/nodeContracts/path.node24.json
```

The generator accepts `--input <file>` for reproducible local generation, but
the input must match the checked-in lock hash. `artifacts/` is gitignored.

Do not change a hash or expected count merely to silence a failure. First
inspect the upstream API change and decide how it affects the normalized
contract.

## Phase 2A: Add a New Contract

### 1. Add module configuration

Extend `generateNodeModuleInterface.js` with one mode/configuration for the
module. The configuration must identify:

- canonical module specifier;
- documentation prefix used in signatures;
- interface name;
- intrinsic class name;
- display name such as `node:path`;
- generated output stem;
- lock and override stems;
- official documentation module/section used in generated provenance;
- generated C# contract alias.

Prefer moving repeated mode conditionals toward a small data-driven
configuration when doing so reduces complexity. Do not create a second
generation pipeline.

The current script's mode selection is not fully centralized. Wire every
existing mode touch-point:

1. command-line flag parsing;
2. mutually exclusive mode validation;
3. the `contract` metadata selection;
4. `contractAlias`;
5. `documentationModule`;
6. module/section extraction and count assertions in `generateInterface`;
7. the mode argument emitted by stale-file diagnostics.

An unrecognized flag currently falls through to the default `fs` mode. Add the
new flag to parsing before running it, and preferably reject unknown flags as
part of the same change. Verify that the new command reports the intended
generated paths and provenance before accepting output; otherwise a missed
mode branch can silently regenerate `fs`.

### 2. Add a lock file

Add `<module>.node24.lock.json` with:

- canonical documentation module;
- exact Node `24.x.y` version;
- immutable official source URL;
- SHA-256 of the raw JSON;
- expected method, property, class, and relevant subsection counts.

Counts are drift detectors. They should correspond to the exact sections read
by the generator.

### 3. Add a narrow override file

Add `<module>.node24.overrides.json`.

Use overrides only for:

- documented public members that the structured JSON does not expose in the
  section being normalized, with an exact official source citation;
- a narrowly reviewed parsing/type clarification;
- intrinsic implementation metadata.

Do not use overrides to redefine the public contract around JROC's current
method signatures.

`intrinsicImplementations` maps a JavaScript member to its statically bound
runtime implementation:

```json
{
  "intrinsicImplementations": {
    "join": { "style": "direct" },
    "readFile": { "style": "argument-array" },
    "debug": { "style": "direct", "target": "InvokeContractDebug" },
    "stat": { "style": "direct", "argumentCount": 1 },
    "table": { "style": "direct", "parameterCounts": [1] }
  }
}
```

Supported concepts:

- `direct`: call a known runtime method/property directly;
- `argument-array`: generate an `object[]` bridge for a legacy intrinsic method
  that already accepts one argument array;
- `target`: use a different known CLR member name;
- `argumentCount`: intentionally pass only a supported leading subset;
- `minimumArgumentCount`: append `null` arguments required by a runtime
  overload;
- `parameterCounts`: only selected generated overloads are implemented.

Every use must correspond to a concrete compile-time-resolvable runtime member.
Unmapped generated members receive an explicit `NotImplementedException`
adapter.

### 4. Normalize the complete public surface

Add module-specific extraction only where the official JSON shape requires it.
Assert the lock counts before rendering.

The normalized output must include all selected public:

- methods and documented call forms;
- properties and access semantics;
- deprecated members;
- experimental members where present in Node 24 LTS;
- JavaScript member names through `[NodeModuleMember]`.

Generation must fail with an actionable error when a public signature is
missing structured metadata or cannot be mapped safely.

### 5. Add or prepare the intrinsic class

The generated adapter expects a partial intrinsic class under
`JavaScriptRuntime.Node`:

```csharp
[NodeModule("example")]
public sealed partial class Example
{
}
```

If the runtime module does not exist, add its intrinsic registration and
implementation separately from generated code.

The generated partial class implements the public contract. Implemented
members call existing intrinsic members directly. Unavailable members throw:

```text
The intrinsic node:<module> module does not implement '<prefix><member>'.
```

Do not add placeholder behavior that looks successful.

### 6. Add package scripts

Add consistent manual commands:

```json
"generate:node-contract-<name>": "node scripts/nodeContracts/generateNodeModuleInterface.js --<mode>",
"check:node-contract-<name>": "node scripts/nodeContracts/generateNodeModuleInterface.js --<mode> --check",
"test:node-contract-<name>": "dotnet test tests/Jroc.NodeContracts.Tests/Jroc.NodeContracts.Tests.csproj --nologo --filter FullyQualifiedName~<TestClass>"
```

Document the commands in `scripts/nodeContracts/README.md`.

### 7. Add manual contract tests

Add one focused test class under `tests/Jroc.NodeContracts.Tests`. Cover:

- `[NodeModuleInterface]` canonical identity;
- `[GeneratedCode]` tool and `sha256:<64 lowercase hex>` version;
- representative required, optional, union, overload, and rest mappings;
- `[NodeModuleMember]` on every public generated member;
- expected distinct method and property counts;
- absence of concrete `Array` and `Promise` ABI types;
- the intrinsic class implements the generated interface;
- representative direct intrinsic delegation;
- no `InvokeContractMember` or reflection bridge;
- representative unavailable method/property throws
  `NotImplementedException` with the expected message.

Keep this project outside the regular solution/CI workflow.

## Phase 2B: Update an Existing Contract

1. Identify why the generated output must change.
2. Read the existing lock, override file, generator mode, generated interface,
   generated adapter, and manual tests before editing.
3. If advancing Node:
   - download the new exact `v24.x.y` JSON;
   - compare the old and new official surfaces;
   - update the lock URL, hash, version, and reviewed counts;
   - update parsing/type exceptions only when justified by the upstream change.
4. If changing type mappings:
   - update the shared mapping in the generator;
   - preserve ABI interfaces instead of concrete runtime classes;
   - regenerate every contract affected by the shared mapping.
5. If adding runtime support for an existing public member:
   - implement the intrinsic behavior;
   - add an `intrinsicImplementations` entry with a direct, statically
     resolvable invocation;
   - regenerate the adapter;
   - add runtime and manual contract coverage.
6. If removing runtime support, remove the implementation mapping and
   regenerate so the adapter throws explicitly. Do not remove the member from
   the public contract.
7. If the shared generator changes, regenerate and check all generated
   contracts because the normalized generator SHA appears in every generated
   `[GeneratedCode]` attribute.

## Normative ABI Type Mappings

Use the mappings established by issue #1659:

| Node/JavaScript type | Contract type |
| --- | --- |
| `number`, integer, fd, mode, byte count | `double` |
| `bigint` | `System.Numerics.BigInteger` |
| required `boolean` | `bool` |
| required `string` or string-literal union | `string` |
| `symbol` | `JavaScriptRuntime.Symbol` |
| method result only `undefined` | `void` |
| value `undefined`, `any`, `unknown`, unconstrained value | `object?` |
| guaranteed JavaScript `null` | `JavaScriptRuntime.JsNull` |
| unshaped object/options object | `object?` |
| array or tuple | `IJavaScriptArray` |
| `Promise<T>` | `IJavaScriptPromise` |
| iterator/iterable | `IJavaScriptIterator` |
| async iterator/iterable | `IJavaScriptAsyncIterator` |
| function/callback/listener | `System.Delegate` |
| `Buffer` | `JavaScriptRuntime.Node.Buffer` |
| `ArrayBuffer` | `JavaScriptRuntime.ArrayBuffer` |
| `SharedArrayBuffer` | `JavaScriptRuntime.SharedArrayBuffer` |
| `DataView` | `JavaScriptRuntime.DataView` |
| JavaScript typed array | corresponding public runtime typed-array type |
| `Date` | `JavaScriptRuntime.Date` |
| `RegExp` | `JavaScriptRuntime.RegExp` |
| stable Node class/shape | corresponding public generated contract only after the shared #1660 adapter/metadata support exists; otherwise `object?` |

Union rules:

- If every union member maps to one CLR type, use that type.
- If union members have incompatible CLR representations, use `object?` unless
  one generated contract safely represents the union.
- Optional parameters and `T | undefined` normally use `object?` so omission,
  undefined, null, coercible values, and invalid values remain distinguishable
  to runtime semantics.
- Do not use nullable value types such as `double?` merely for optional Node
  parameters.
- Rest parameters use `params object?[]`.
- Callbacks use `Delegate`, not invented `Action`/`Func` signatures.

Nested stable shapes are tracked by issue #1660. Until the generator/runtime
adapter support exists, do not expose a built-in concrete implementation as a
shortcut. Retain `object?` with documented metadata, or implement the shared
nested-contract architecture as part of the task.

## Generation and Validation

### Generate

Run the module's generation command. If the shared generator changed, run all
contract generators:

```sh
npm run generate:node-contract-fs
npm run generate:node-contract-fs-promises
npm run generate:node-contract-console
npm run generate:node-contract-path
```

Include any newly added module command.

### Check determinism

Run the corresponding `check:*` commands after generation. Run generation a
second time and confirm it produces no diff.

The generated header must include:

- exact Node.js version;
- official source URL;
- official document SHA-256.

`GeneratedCodeAttribute.Version` is the normalized shared generator source
SHA-256, not the Node version. Node provenance remains in the generated header
and lock.

### Run manual tests

Run the module-specific `test:node-contract-*` command. If shared ABI or
generator behavior changed, run the entire manual project:

```sh
dotnet test tests/Jroc.NodeContracts.Tests/Jroc.NodeContracts.Tests.csproj --nologo
```

Run a normal build because generated contracts ship in the runtime/package:

```sh
dotnet build --no-restore
```

If intrinsic runtime behavior changed, also run focused execution/generator
tests under `tests/Jroc.Tests/Node/<Module>`.

### Review generated output

Before committing, inspect the diff and verify:

- complete public Node surface for the selected official sections;
- stable ordering and formatting;
- correct canonical `[NodeModuleInterface]`;
- `[NodeModuleMember]` preserves exact JavaScript names;
- no concrete runtime `Array`/`Promise` leakage;
- no reflection or runtime method discovery;
- direct calls resolve to the intended overload;
- unsupported members throw explicitly;
- lock counts and hashes changed only for understood reasons;
- `--check` passes;
- `git diff --check` reports no hand-authored whitespace errors.

Generated decompiler/snapshot files may preserve tool-produced trailing
whitespace; do not hand-edit generated output merely to satisfy
`git diff --check`.

## Documentation Follow-Through

Contract generation and JROC support documentation are separate concerns:

- The contract comes from official Node.js 24 LTS docs.
- `docs/nodejs/<module>.json` records what JROC currently implements.

For a new module, add its JSON support document following
`docs/nodejs/ModuleDoc.schema.json`. For an existing module, update the JSON
only when runtime support changed.

Regenerate documentation:

```sh
npm run generate:node-modules
```

Update `CHANGELOG.md` when adding a contract, changing public ABI mappings,
advancing the Node pin, or adding meaningful runtime support.

## Common Failure Modes

### Documentation hash mismatch

The downloaded bytes do not match the lock. Confirm the exact versioned URL.
If intentionally advancing Node, inspect the upstream diff before updating the
lock.

### Count mismatch

The official public surface changed or the wrong section is being read. Review
the JSON structure and update normalization plus tests intentionally.

### Generated files are stale

Run the generator named by the diagnostic. If the shared generator changed,
regenerate every mode.

### Cannot map a type

Do not silently omit the member. Add a public runtime ABI abstraction or a
narrow, cited mapping clarification. Do not expose an intrinsic implementation
class.

### Direct adapter call does not compile

The override metadata does not match a concrete intrinsic signature. Adjust
the runtime API or use the existing static bridge options (`target`,
`argument-array`, or reviewed argument counts). Do not fall back to reflection.

### Runtime member is unavailable

Leave the public contract member generated and omit its intrinsic mapping. The
generated adapter must throw `NotImplementedException`.

## Pull Request Checklist

- Branch from current `master`.
- Keep lock, generator/overrides, generated files, tests, runtime changes, and
  directly related documentation in one reviewable PR.
- Explain the official Node source/version and whether this is a new contract,
  upstream refresh, ABI mapping change, or runtime implementation update.
- Call out generated files and the manual commands run.
- Link #1659 for top-level contract generation and #1660 when nested contracts
  are involved.
- Do not claim full runtime support merely because the complete interface was
  generated.
