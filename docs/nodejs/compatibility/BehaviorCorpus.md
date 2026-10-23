# First public-API behavior corpus

Issue [#2374](https://github.com/tomacox74/js2il/issues/2374) adds **19 original
scenarios** for `path`, `buffer`, `events` and CommonJS/ESM resolution. This is a
bounded, reviewable first slice, not a Node test-suite port, package cohort or
whole-Node compatibility score.

The executable manifest and provenance metadata live in
[`scripts/differential-test/corpus/node-public-api-v1`](../../../scripts/differential-test/corpus/node-public-api-v1).
All eight existing loader behavior registrations have scenario mappings; their
inventory registration status remains unchanged because measurement inputs are
not execution evidence.

## Selected behavior and upstream discovery scope

| Area | Original scenarios | Assertion intent |
| --- | ---: | --- |
| path | 4 | POSIX/Win32 normalization, joining/relative paths, empty inputs, object/export shape, parse/format, extensions and invalid-argument errors. |
| buffer | 4 | UTF-8/hex/base64, exact binary side effects, allocation boundaries, unsigned endian operations, shared subarray storage, error codes and export/prototype shape. |
| events | 4 | Listener receiver and synchronous ordering, once/reentrancy/removal, listener inspection, invalid listeners, unhandled errors and module export shape. |
| loader | 7 | CommonJS relative/cache identity, ESM relative imports and package type, import/require export conditions, package imports, transitive lookup and negative resolution/error codes. |

The checked-in `upstream-inventory.json` identifies **32 inspected Node test
files** at `ce7aaf1cee78b8cf0ce17364260535c418d3ed26`, including their immutable
URLs, SHA-256, byte counts, exact license provenance and triage reasons. This is
an explicitly selected discovery inventory, not all upstream tests for these
modules. Each file is classified as runnable, adaptation-required,
platform-specific or excluded, with applicability and execution eligibility
recorded separately.

No upstream test is currently directly runnable in this corpus: Node's
common/assert/fixture dependencies need review and adaptation. Flag-dependent
experimental package maps, private Node internals, symlink prerequisites and
the >4 GiB buffer test are identified explicitly, not silently discarded.
Unknown whole-file applicability remains unknown; a missing harness is not a
product failure. The 64-bit buffer lead is applicable to the x64 architecture
but exceeds this runner's resource budget.

The executable fixtures are independently written project code under the
repository's Apache-2.0 license, with original-case rationales and related
upstream discovery paths. They do **not** claim to preserve or port the complete
upstream assertions. No upstream code is redistributed. The upstream Node
MIT/license notices are identified separately, including the content digest of
the pinned license file. These Node-specific APIs do not require WPT cases;
no WPT or Test262 coverage is claimed.

## Reproduction

Use the exact profile in `baseline.json`, currently Node 24.22.1, Linux x64,
.NET SDK 10.0.113 and Microsoft.NETCore.App 10.0.13:

```sh
nvm use 24.22.1
dotnet build src/Cli/Jroc.csproj -c Release

# Offline regeneration and consistency checks.
npm run generate:node-behavior-corpus
npm run check:node-behavior-corpus
npm run test:node-compatibility

# Optional network operation: recheck pinned source/license bytes, never repin.
npm run check:node-behavior-sources

# Clean committed worktree; all selected cases run, including known failures.
npm run diff:test:node:public-api -- --attempts 2

# Diagnostic dirty-worktree runs remain explicitly local/unverified.
npm run diff:test:node:public-api -- --allow-dirty --case node:scenario/public-api/path/posix

# Preserve a new report directory; an existing report cannot be overwritten.
npm run report:node-behavior-corpus -- artifacts/nodeCompatibility/runs/<uuid> \
  artifacts/nodeCompatibility/report-<uuid>

# Exact-profile integration: all 19 references, 38 attempts, offline reporting.
npm run test:node-public-api:integration
```

A nonzero differential CLI exit is expected while any selected scenario has a
non-pass outcome. It does not mean the measurement failed: verify the completed
bundle and distinguish product gaps from reference/infrastructure failures.
The integration test does not require product failures to remain failures; it
requires healthy reference/harness evidence and reconciled counts.

No npm install or external network is needed during measured workflows.
`node_modules` directories in this fixture tree contain only original,
checked-in resolution stubs, including distinct nested dependency versions.
All fixture/package/harness bytes are content-pinned; no registry package
support is inferred.

## Identity, provenance and interpretation

`manifest.json` retains the shared version-1 executable scenario contract.
Its optional `corpusMetadata` names `corpus.json`, a `behavior-corpus` artifact
validated by `behavior.schema.json`. Every scenario retains that metadata as
an input. Execution and offline readback verify exact fixture/dependency hashes,
scenario/target/policy mappings, source pin, applicability review and profile.
Unknown or conflicting metadata fails explicitly before a product result.

Scenario IDs are stable across fixture revisions; fixture/content digests,
comparison policies, reference profiles and run/attempt identities are
independent. Changing code requires regenerating metadata, not renaming a
scenario. Changing the reference requires an explicit applicability review;
regeneration refuses to silently accept a new profile. Prior Git revisions and
immutable bundles retain earlier definitions/evidence.

Original assertions validate expected values and meaningful error type/code.
Raw bytes, JSON types and required event ordering are preserved without
whitespace trimming, timing matching or arbitrary scheduling normalization.
Missing runtime features may produce mismatches; `unsupported` is reserved for
the runner's structured compiler evidence, not inferred from stderr.

The first-slice report consumes only an offline-verified bundle. JSON and
generated Markdown keep scenario outcomes separate from upstream lead counts,
API mapping counts and retained attempt counts. Many-to-many mappings do not
multiply scenarios. Unmapped inventory candidates remain explicitly untested;
mapped targets describe assertion intent, not exhaustive API support or proof
that every operation executed before an earlier failure.

Reports include local provenance, lossless base64 stdout/stderr/observable
snapshots and full-bundle/artifact hashes. Full compiler/binary evidence remains
in the named local bundle, not in Git or a central publication system. Reports
are capped at 8 MiB and written into new directories without overwriting history.
The retained first measurement is under [`baselines`](baselines).

These observations remain `local-unverified`, even for a clean checkout.
Neither successful readback nor environment eligibility attests the
source-to-binary relationship or grants trusted catalogue acceptance. Broader
scorecards/gates are #2378; packages are #2375. There are no Test262 catalogue
schema, authority, database, permission or publication changes.
