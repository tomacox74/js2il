# Node.js compatibility measurement

This directory defines the measurement foundation for [#2376](https://github.com/tomacox74/js2il/issues/2376),
under the [Node compatibility baseline](https://github.com/tomacox74/js2il/issues/2372).
It does **not** publish a compatibility score or claim that JROC implements
Node 24. The differential runner, executable corpus, package workloads and
reporting gates are separate implementation issues.

## Artifact contracts and future catalogue integration

The stable suite namespace is `nodejs-compatibility`. Inventory and environment
artifacts use distinct `artifactKind` values, `api-inventory` and
`reference-environment`, with independent version-1 contracts in
[`artifact.schema.json`](../../../scripts/nodeCompatibility/artifact.schema.json).
The generator validates inventory structure, identities, links and digests before
writing it. The reference checker validates the inventory before reading its
provenance and validates the complete environment envelope before publishing it.
Missing required fields, unknown fields/kinds/versions, duplicate identities,
unresolved owner/contract links and inconsistent digests are explicit failures.

Offline consumers can use the same validator without invoking Git, .NET,
fixtures, a database or the reference-environment checker:

```js
const { validateArtifact } = require('./scripts/nodeCompatibility/artifacts');
validateArtifact(inventory, 'api-inventory');
validateArtifact(environment, 'reference-environment');
```

Run `npm ci --ignore-scripts` first to install the lockfile-pinned tooling
dependencies, including the JSON Schema validator. Schema validation establishes
artifact shape; the shared validator additionally checks identity/link/digest
invariants. `validateEnvironment` in `checkReference.js` adds the pinned baseline
and runtime-policy checks. None of these checks independently authenticates a
producer or establishes trusted catalogue acceptance.

These are offline measurement inputs, not executable results or a publication
protocol. `referenceEnvironmentEligible: true` means only that the local checker
accepted the declared environment and clean worktree. It does not mean cases
ran, behavior passed, CI validated that SHA, or the catalogue accepted the evidence.
Node component-version values are retained verbatim, including empty strings
for components not enabled in a Node build.

The Test262 catalogue is undergoing independent changes. Keep execution,
serialization, reporting and any future ingestion adapter separate; do not add
catalogue connectivity, credentials, schema migrations, leases or publication
dependencies to these tools. Revisit additive integration when the catalogue
changes settle and the Node runner plus initial executable corpus produce real
evidence. Preserve Test262 identities, history, authority and permissions, and
keep Node/ECMA-262 denominators separate. Historical import requires retained
provenance and must not promote local/historical results to trusted acceptance.
This contract does not promise compatibility with today's Test262 SQL/API.

## Reference and reproduction

The initial reference is **Node 24.22.1**, Linux x64, .NET SDK 10.0.113,
and Microsoft.NETCore.App 10.0.13. The exact Node source commit and official
documentation digest live in
[`baseline.json`](../../../scripts/nodeCompatibility/baseline.json).
This is one reproducible measurement profile, not a claim of portability to
every supported Node platform. Windows, macOS, ARM64 and optional features
remain explicitly unmeasured until separately reviewed profiles exist.

```sh
# Select the exact reference, not a moving "24" alias.
nvm use 24.22.1

# Download only the pinned official API JSON and verify its SHA-256.
# The ignored download is a cache, not a second authority.
npm run generate:node-compatibility -- --download

# Offline regeneration/check after the input has been downloaded.
npm run check:node-compatibility
npm run test:node-compatibility

# From a clean committed worktree, write the reference environment envelope.
npm run --silent check:node-reference > artifacts/nodeCompatibility/environment.json
```

`--input <path>` can supply an independently retained copy of `all.json`;
the same pinned hash is required. Generation is deterministic and does not
import/execute Node modules under test or download npm packages.

`check:node-reference` fails on a different Node patch, public builtin roster,
OS/architecture, SDK or default .NET 10.0 runtime patch selection. It also
rejects Node flags or runtime-altering environment overrides, stale contract
evidence, missing provenance and a dirty worktree. Override names, not their
potentially sensitive values, are recorded.
A missing executable or failed Git/.NET command is an explicit failure,
not a default value. `--allow-dirty` is available for local diagnostics only;
the output records the dirty tree and `referenceEnvironmentEligible: false`.
Do not publish that envelope as a clean baseline.

The envelope records actual JROC HEAD, inventory/config/behavior-registration
hashes, timestamp, Node component versions, installed .NET runtimes and hashes
of tracked npm/NuGet lockfiles. The current repository has npm lockfiles but
does not thereby establish a locked NuGet dependency graph or bitwise
reproducibility. The seed JROC SHA in the manifest identifies the implementation
snapshot this work started from, **not** the revision measured in later runs.
`behaviorRegistrationSha256` identifies pending behavior registrations, not an
executable corpus. There is deliberately no `corpusRevision` in these artifacts:
`executableBehaviorCases` is zero. The executable corpus identity and execution
result contract belong to the runner/corpus work, not this environment envelope.
Future execution envelopes must additionally identify actual fixtures, runner,
compiled artifact, effective runtime/options and package installation inputs.

### Digest boundaries (version 1)

The inventory retains declarations/configuration and target definitions in one
file, but they have separate semantic identities. The environment carries all
three semantic digests as well as the full inventory-file checksum:

| Digest | Exact input and purpose |
| --- | --- |
| `inventorySha256` | SHA-256 of the exact inventory file bytes, including formatting/newline and all evidence. This is an envelope field, not a self-hash inside the inventory. |
| `targetInventorySha256` | Semantic JSON hash of `{ suiteId, scopeId, applicability: defaults.applicability, documents, entries, behaviors }`. Omit `documentationUrl` and `sourceUrl` from each document, `contracts` from each entry, and `status` from each behavior. Everything else in those collections is retained, including expectations, signatures, structural sections and exclusions. |
| `implementationEvidenceSha256` | Semantic JSON hash of `{ suiteId, contracts, associations }`, where `associations` is the ordered list of `{ id, contracts }` for entries with contract links. It covers declaration/adapter/override hashes and their mappings, not target definitions or execution results. |
| `referenceProfileSha256` | Semantic JSON hash of the complete `reference` object, including the exact Node documentation/source pin and platform/.NET profile. |
| `behaviorRegistrationSha256` | Semantic JSON hash of the complete ordered `behaviors` array, including pending status. This is an environment field, not an executable-corpus revision. |
| `baselineConfigSha256` | SHA-256 of the exact baseline configuration file bytes. |

Semantic JSON hashing recursively sorts object keys with JavaScript's default
string ordering, preserves array order, serializes with `JSON.stringify` without
whitespace/newline, encodes UTF-8, then computes SHA-256 as lowercase hex.
Undefined and non-finite values are rejected. This is a versioned Node artifact
rule, not the Test262 catalogue's PostgreSQL numeric canonicalization contract.
Retain the original JSON bytes for byte-checksum verification. Reformatting or
reordering object keys leaves semantic digests unchanged but changes a byte hash;
array reordering changes semantic identity. The generator supplies deterministic
collection order.

A contract-only change must not reidentify target scope. Changing only a
reference pin/URL must not reidentify otherwise unchanged targets or contract
evidence. Adding/removing/changing target definitions, applicability or scenario
expectations does change target identity. These boundaries have regression tests;
the whole artifact checksum remains sensitive to every change. Consumers still
compare exact profiles and inspect scope differences before comparing results.

## Inventory scope and provenance

[`inventory.v1.json`](inventory.v1.json) is a generated, versioned candidate
inventory from the full official Node 24.22.1 `api/all.json`, not a list of
JROC's existing methods. It includes public module entry points (including
subpath and `node:`-only entry points), documented classes, constructors,
methods, static methods, properties, events and globals. Missing JROC modules
such as `node:http2` and `node:worker_threads` remain candidates.

Each documentation page has an immutable source-commit URL, versioned HTML
URL and inclusion/exclusion decision. Each API entry refers to that page and
retains its identifying heading(s), documented signature shapes and stability
metadata where provided. Properties whose official JSON name is just `Type`
are identified using their heading. Duplicate global records and overload
headings are merged by semantic identity; nested object members remain distinct.
Reviewed section-namespace overrides preserve separate public entry points
when the source uses the same class heading for different APIs, for example
callback `inspector.Session` versus `inspector/promises.Session`.

The builtin roster was collected using
`require('node:module').builtinModules` on the exact reference.
Underscore-prefixed private modules are excluded by rule. Public legacy
aliases and prefix-only modules remain in the roster. This is not a roster of
exports observed by executing every module.

The document list retains excluded pages with reasons: native addon/Node-API
and C++ embedding ABIs, Node executable CLI/debugger behavior, introductory
documentation and historical deprecation cross-references. APIs still
documented in their current pages are not removed because they are deprecated.
These exclusions bound the measurement; they are not evidence of compatibility
with those omitted Node features.

**Completeness limits:** API JSON is not a complete behavioral specification.
Options, inherited members and external web specifications need additional
review/cases; arbitrary prose headings are retained as `documents[].sections`,
not silently converted into API methods or discarded as covered behavior.
Methods documented only in prose may still need explicit inventory additions.
Do not label the candidate count "all Node behavior."

## Stable identities and applicability

IDs do not contain a version, array index, implementation status or result:

- Module: `node:module/http`.
- API: `node:api/http/method/http.request`.
- Class member: `node:api/http/class/http.Agent/method/destroy`.
- Behavior: `node:behavior/loader/commonjs-relative`.

API names are URI-encoded inside an ID component. Preserve an ID when its
semantic target is unchanged; record aliases/migrations when correcting an
extraction identity. Changing a signature adds a form to its existing family,
not another API or an automatic extra behavioral pass. Class constructors,
instance methods, static methods and properties are distinct targets. Global
and module exposure can be distinct targets even when they share implementation.

One independently counted behavior scenario has a stable case ID, mapped API
target(s), input/configuration, observable assertion(s), applicability, fixture
provenance and a bounded completion condition. It is a case, not an assertion,
an overload count, a retry or a duplicate port. Multiple assertions or target
APIs do not multiply that scenario's count. Variants/platforms form separately
named measurement cells and must not silently inflate cross-platform totals.
Cases without an API mapping must identify a loader/workflow target explicitly.

Eight loader behavior IDs are registered with expectations. They are
**not-run registrations**, not implemented tests or passing observations.
They cover CommonJS, ESM, package conditions/imports/type, caching, transitive
resolution and failure behavior; their executable cases belong to the corpus
and package issues.

API applicability defaults to `platform: review-required` and
`feature: review-required`; this means **unknown**, not "works everywhere"
or "excluded." Entry-level reviewed applicability will override these defaults
as cases are introduced. Loader registrations identify their public loading
feature but still have no execution evidence. Retain platform-specific
semantics and optional flags/configuration in the eventual case/profile.
JROC's lack of an implementation is never a reason to mark a Node API
inapplicable. Every reviewed exclusion needs a rationale and provenance.

## Three independent evidence layers

1. **Contract declaration:** `contracts` identifies generated declarations,
   original documentation versions, source hashes and override/adapter hashes.
   Entries link to matching declarations conservatively; an absent link means
   unreviewed/unmapped, not proof the runtime lacks that API. Existing contract
   pins are not silently regenerated to the measurement reference.
2. **Implementation availability:** a configured adapter implementation and a
   generated stub are different observations. Neither proves full availability
   from compiled JavaScript. Options, setters and call forms may be unsupported
   even for a configured member. Default availability is `unverified`.
3. **Behavioral evidence:** only a scoped, successfully completed reference and
   JROC execution with preserved assertions can establish a pass. All current
   entries default to `not-run`. C# tests expecting `NotImplementedException`,
   generated roster tests and manually written support labels are not passes.

No support percentage is derived from adapter metadata.

## Counting rules

Report these dimensions separately, with raw counts and the scope/profile ID:

| Measure | Numerator / denominator |
| --- | --- |
| Candidate API inventory | Count distinct included API IDs, grouped by kind and document; report public module entry points separately from member APIs. |
| API exercise coverage | Distinct applicable API IDs reached by valid mapped cases / distinct reviewed-applicable API IDs in that same kind/scope/profile. Show unknown applicability and exclusions beside this ratio. |
| Registered behavior scope | Count distinct registered scenario cells, partitioned into reviewed applicable, excluded with reason, and applicability unknown. |
| Behavior exercise coverage | Applicable cells with conclusive Node/JROC observations / all registered applicable cells. Unknowns and the unregistered behavior gap remain visible. |
| Observed behavior pass rate | `pass / (pass + mismatch + compile-failure + unsupported + execution-timeout)` for conclusive applicable cells with a valid reference. |
| End-to-end registered success | `pass / all registered applicable cells`, including not-run and unresolved cells in the denominator. |

Compile failure or unsupported behavior after a valid reference is a conclusive
negative observation, even though JROC did not reach the API. It does not
establish API **exercise** coverage. A runtime timeout is a product failure only
after confirming the harness and reference were healthy and the completion
budget is suitable; otherwise classify it as infrastructure/inconclusive.

Unknown applicability cannot be silently treated as excluded or used to claim
100% coverage. Until applicability is reviewed, publish candidate counts and
unknown counts, not an applicable-scope percentage. A zero denominator is
**undefined**, never 100%. A high pass rate over a small selected corpus is not
overall Node compatibility. Do not combine APIs, scenarios, npm workflows,
security advisories and Test262 cases into one weighted score.

## Result vocabulary

| Outcome | Meaning |
| --- | --- |
| `pass` | Valid reference and JROC runs satisfy the scenario's observable assertions/comparison. |
| `mismatch` | Valid reference; JROC completed with different behavior, error, side effect or process outcome. |
| `compile-failure` | Valid reference; JROC cannot compile the case, excluding explicit unsupported-feature classification. |
| `unsupported` | Valid reference; a known unsupported compiler/runtime feature prevents the required behavior. |
| `not-run` | No execution observation; includes pending registrations. |
| `execution-timeout` | Valid reference; healthy harness, but JROC did not meet the declared completion budget. |
| `reference-failure` | Node cannot provide the required valid reference observation, including reference timeout. |
| `infrastructure-failure` | Tooling, setup, permissions, fixture provisioning or runner failure prevents a trustworthy product result. |
| `flaky` | Repeated controlled attempts conflict; preserve each attempt and do not select only successes. |
| `inconclusive` | Available observations do not justify either a product outcome or a known infrastructure classification. |

Reference/infrastructure failures are neither passes nor ordinary mismatches.
Keep them, flaky/inconclusive cases and not-run cells in scope accounting, even
when they are excluded from a separately labeled conclusive pass-rate ratio.
Normalization must be explicit and limited to environmental variation;
meaningful errors, bytes, effects and ordering cannot be normalized away.

## Updates and comparisons

Security patches are mandatory reference-maintenance work, not a reason to
silently use a moving latest-version alias. Select the patched Node version,
resolve its immutable source revision, verify/download its official API
document, review the builtin roster and contract mappings, update the explicit
profile and regenerate the inventory. Keep prior baseline artifacts and metadata
under their original identities; commit the new reference and publish a new
baseline envelope. The old configuration is recoverable from its Git revision.

Record added/removed/changed API IDs, signature forms, cases and applicability.
Compare the common scope separately from newly added or removed cells. Node,
JROC, corpus, runner, package lockfiles or environment changes must be visible;
do not attribute a changed denominator/reference expectation to a compiler
improvement. Security regression expectations take precedence over reproducing
a vulnerable Node result.

The support documents' historical Node 22 labels and the existing Node 24.18.1
contract generation pins remain distinct from this Node 24.22.1 **measurement
reference**. This work does not upgrade those support claims. The generated
support index links here and labels that distinction explicitly.
