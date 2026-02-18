# Node Gap Popularity Backlog (2026-02-17)

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

- Node docs track **7 modules**, all marked `partial`, and **13 globals** mostly supported.
- Major blockers explicitly documented:
  - No `Buffer` support
  - `require()` only partially supported
  - no ESM `import.meta.url`
- ECMA matrix remains sparse (many sections incomplete/untracked), which increases risk for modern package behavior.

## Popularity-Weighted Missing Functionality

### P0 (Highest impact / broadest unblock)

1. **Buffer + binary I/O path**
   - Why: unlocks large portions of npm ecosystem (`fs`, crypto-like flows, parsers, protocol clients).
   - Current blocker: `fs` is text-only in documented behavior.

2. **events/EventEmitter core semantics**
   - Why: central dependency pattern across Node packages and many polyfills.
   - Typical minimum: `on`, `once`, `off/removeListener`, `emit`, listener ordering.

3. **util essentials**
   - Why: common in transitive dependencies.
   - Suggested first APIs: `promisify`, `inherits`, `types` subset, `inspect` minimal compatibility.

### P1 (High value, after P0 foundations)

4. **stream core baseline**
   - Why: critical for many adapters and network/file stacks.
   - Suggested first surface: minimal `Readable`/`Writable` + pipeline primitives required by popular libs.

5. **fs/promises breadth expansion**
   - Why: modern Node codepaths prefer async fs APIs.
   - Suggested additions: `readFile`, `writeFile`, `stat/lstat`, `realpath`, basic `watch` strategy.

6. **process expansion**
   - Why: common runtime feature checks and environment access.
   - Suggested additions: `env`, `cwd/chdir`, `nextTick`, `versions`, `platform`.

### P2 (Important but can follow the core runtime path)

7. **url/querystring compatibility**
   - Why: very common in tooling and HTTP stacks.
   - Suggested additions: robust `URL`, `URLSearchParams`, parse/format compatibility helpers.

8. **crypto minimum practical subset**
   - Why: common package requirement for hashing/randomness.
   - Suggested additions: `createHash`, `randomBytes`, `webcrypto.getRandomValues` bridge.

### P3 (Large scope / sequence after foundations)

9. **http/https/net/tls layers**
   - Why: high ecosystem reach, but dependent on streams/events/buffer maturity.

10. **ESM interop baseline**
    - Why: increasingly common package entrypoints.
    - Suggested baseline: loader behavior for common import/export shapes and `import.meta.url` support plan.

## Internal Demand Signals (Repo-local)

Observed frequently in tests/samples and therefore good near-term ROI:

- `path`, `fs`, `process`, timers, and `perf_hooks` are actively exercised.
- Integration tests reference `node:child_process`, `node:fs`, `node:path`, `node:os`.
- Existing Node module docs remain partial, indicating broad gaps despite progress in path/fs/process/timers slices.

## Recommended Sequencing (Execution)

### Two-week slice suggestion

- **Week 1:** Buffer foundation + `fs` binary read/write interoperability tests
- **Week 2:** `events` baseline + `util.promisify` and one consumer-style integration test

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
