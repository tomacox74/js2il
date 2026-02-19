# Node Gap Popularity Backlog (2026-02-18)

> Purpose: Persist a holistic, popularity-weighted view of missing functionality so triage context is not lost between sessions.
> Scope: Node.js compatibility first, with adjacent ECMA impacts called out where they block common Node workloads.

## Inputs Used

- Current support inventory: `docs/nodejs/Index.md`
- Stated limitations: `docs/nodejs/NodeLimitations.json`
- Module-level coverage: `docs/nodejs/*.json`
- Runtime module footprint: `JavaScriptRuntime/Node/*`
- Internal demand signals: `require(...)` usage across `Js2IL.Tests/**/*.js` and `samples/**/*.js`
- ECMA tracking breadth: `docs/ECMA262/Index.md`

## Current Baseline (Snapshot)

- Node docs track **8 modules** (all currently `partial`) and **14 globals** (**14 `supported`**, **0 `partial`**).
- Major blockers currently documented:
   - `require(id)` is marked **supported** but still limited to implemented core modules + compiled local modules (no `node_modules` / `package.json` resolution)
   - no ESM `import.meta.url`
- ECMA matrix remains sparse (many sections incomplete/untracked), which increases risk for modern package behavior.

## Popularity-Weighted Missing Functionality

### P0 (Highest impact / broadest unblock)

1. **Buffer + binary I/O path**
   - Status: ✅ **Completed**
   - Why: unlocks large portions of npm ecosystem (`fs`, crypto-like flows, parsers, protocol clients).
   - Current state: Buffer binary workflow surface is implemented (`Buffer.from/isBuffer/alloc/allocUnsafe/byteLength/concat/compare`, `slice/subarray/copy/write/fill/equals/indexOf/lastIndexOf/includes`, array-like indexing, `read/writeInt8/16/32`, `read/writeUInt8/16/32`, `read/writeFloatLE/BE`, `read/writeDoubleLE/BE`) and fs sync Buffer read/write interop is in place.
   - Remaining gap: none for this backlog item; any future long-tail Node Buffer parity work should be tracked as a separate lower-priority item.

2. **events/EventEmitter core semantics**
   - Status: 🟡 **Baseline implemented**
   - Why: central dependency pattern across Node packages and many polyfills.
   - Current state: `on/addListener`, `once`, `off/removeListener`, `emit`, `listenerCount`, `removeAllListeners` are implemented.
   - Remaining gap: advanced APIs (`errorMonitor`, `prepend*`, `rawListeners`, max listeners controls, async iterator helpers).

3. **util essentials**
   - Status: 🔴 **Not started in docs/runtime inventory**
   - Why: common in transitive dependencies.
   - Suggested first APIs: `promisify`, `inherits`, `types` subset, `inspect` minimal compatibility.

### P1 (High value, after P0 foundations)

4. **stream core baseline**
   - Status: 🔴 **Not started in docs/runtime inventory**
   - Why: critical for many adapters and network/file stacks.
   - Suggested first surface: minimal `Readable`/`Writable` + pipeline primitives required by popular libs.

5. **fs/promises breadth expansion**
   - Status: 🟡 **Partially implemented**
   - Why: modern Node codepaths prefer async fs APIs.
   - Current state: `access`, `readdir({ withFileTypes: true })`, `mkdir({ recursive: true })`, `copyFile` are implemented.
   - Remaining additions: `readFile`, `writeFile`, `stat/lstat`, `realpath`, basic `watch` strategy.

6. **process expansion**
   - Status: 🟡 **Partially implemented**
   - Why: common runtime feature checks and environment access.
   - Current state: `argv`, `exit`, `exitCode`, `env`, `chdir`, `nextTick`, `platform`, and `versions.node` are implemented.
   - Remaining additions: `cwd()`, broader `versions` surface, and tighter `nextTick` semantics parity.

### P2 (Important but can follow the core runtime path)

7. **url/querystring compatibility**
   - Status: 🔴 **Not started in docs/runtime inventory**
   - Why: very common in tooling and HTTP stacks.
   - Suggested additions: robust `URL`, `URLSearchParams`, parse/format compatibility helpers.

8. **crypto minimum practical subset**
   - Status: 🔴 **Not started in docs/runtime inventory**
   - Why: common package requirement for hashing/randomness.
   - Suggested additions: `createHash`, `randomBytes`, `webcrypto.getRandomValues` bridge.

### P3 (Large scope / sequence after foundations)

9. **http/https/net/tls layers**
   - Status: 🔴 **Not started in docs/runtime inventory**
   - Why: high ecosystem reach, but dependent on streams/events/buffer maturity.

10. **ESM interop baseline**
    - Status: 🔴 **Not started in docs/runtime inventory**
    - Why: increasingly common package entrypoints.
    - Suggested baseline: loader behavior for common import/export shapes and `import.meta.url` support plan.

## Internal Demand Signals (Repo-local)

Observed frequently in tests/samples and therefore good near-term ROI:

- `path`, `fs`, `process`, timers, `perf_hooks`, and `events` are actively exercised.
- `Buffer` now has dedicated execution + generator tests under `Js2IL.Tests/Node/Buffer`.
- Integration tests reference `node:child_process`, `node:fs`, `node:path`, `node:os`.
- Existing Node module docs remain partial, indicating broad gaps despite progress in path/fs/process/events/buffer slices.

## Recommended Sequencing (Execution)

### Two-week slice suggestion

- **Week 1:** expand `util` baseline (`promisify`, `inherits`, minimal `types`/`inspect`) + add one transitive-dependency integration fixture
- **Week 2:** expand `fs/promises` (`readFile`, `writeFile`, `stat/lstat`) and tighten `process` parity (`cwd`, richer `versions`, `nextTick` semantics)

### Gate for each delivered item

- Add execution tests (+ generator tests where applicable)
- Update `docs/nodejs/*.json`
- Regenerate node markdown/index artifacts
- Note user-visible behavior in changelog when appropriate

## Risks / Caveats

- Popularity ranking combines ecosystem heuristics with local demand signals; refine weekly with fresh failing-fixture data.
- Large subsystems (`streams`, `http`) should not start before Buffer/events primitives stabilize.

## Ownership Handoff Notes

When resuming work, start from this file plus `TriageScoreboard.md`, then pick highest unfinished P0 item.
