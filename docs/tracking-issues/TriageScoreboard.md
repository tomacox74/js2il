# JS2IL Triage Scoreboard

> **Last Updated**: 2026-02-17  
> **Planning Horizon**: Rolling 2 weeks  
> **North Star**: Real-world unblock impact

This file is the working source of truth for implementation prioritization.

## Session Context Snapshots

- [NodeGapPopularityBacklog_2026-02-17.md](./NodeGapPopularityBacklog_2026-02-17.md): Holistic missing-functionality analysis with popularity-weighted priority ranking.

## Capacity Split

- **70%** feature implementation
  - Primary: Node.js compatibility
  - Secondary: ECMA-262 high-leverage semantics
- **30%** issue throughput + documentation/status consistency

## Priority Lanes

### Lane A — Node Compatibility (Primary)

Goal: unblock real package/script scenarios with highest user impact.

**Selection criteria**
- Unblocks widely-used ecosystem patterns
- Reproducible via focused execution tests
- Clear runtime/hosting touchpoints

**Candidate queue (rank during weekly triage)**
- [ ] Expand `fs` APIs needed by real-world fixtures
- [ ] Expand `path` parity for common bundler/tooling patterns
- [ ] Expand `process` surface used by transpiled/bundled code
- [ ] Improve `child_process` and `os` minimal parity based on failing fixtures
- [ ] Add missing Node globals where frequently requested

### Lane B — ECMA-262 High-Leverage Semantics (Secondary)

Goal: improve language/runtime correctness where breakage is common.

**Selection criteria**
- High breakage frequency in partner or fixture workloads
- Tight scope with strong testability
- Low-to-medium implementation risk relative to impact

**Candidate queue (rank during weekly triage)**
- [ ] `Object.hasOwn` and `Object.is`
- [ ] `Array.from`, `Array.prototype.forEach`, `every`, `some`, `includes`
- [ ] Remaining Symbol/well-known-symbol gaps affecting iterator semantics
- [ ] TDZ and getter/setter edge-case completeness

### Lane C — Issue Throughput + Reliability Hygiene

Goal: reduce issue aging while keeping docs/status trustworthy.

**Selection criteria**
- User-facing bug/regression severity
- Reproducibility and clear acceptance criteria
- Documentation drift risk

**Candidate queue (continuous)**
- [ ] Prioritize issues labeled regression/bug/unblocker
- [ ] Triage stale issues and close/retag with reproducible scope
- [ ] Keep coverage/status docs synchronized with shipped behavior

## Mandatory PR Gate (Feature Work)

All feature PRs must include:

1. Execution tests (and generator tests when applicable)
2. Documentation updates in relevant JSON source docs:
   - `docs/ECMA262/**/Section*.json` for language features
   - `docs/nodejs/*.json` for Node modules/globals
3. Regenerated markdown/index artifacts after JSON updates
4. Changelog entry when user-visible behavior changes

If documentation is not synchronized, PR is not ready to merge.

## Weekly Triage Cadence

At the start of each week:

1. Re-rank top 3 items in each lane
2. Commit to a 2-week slice using the 70/30 split
3. Define acceptance criteria and owner for each committed item
4. Verify all open feature PRs include docs sync updates

## Source-of-Truth Order

When status signals conflict, use this precedence:

1. Runtime behavior validated by tests
2. `CHANGELOG.md` entries for released behavior
3. Section/module JSON docs under `docs/ECMA262` and `docs/nodejs`
4. Strategy/roadmap markdown summaries

Older phase roadmaps remain useful for context, but this scoreboard drives active prioritization.
