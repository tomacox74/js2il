# JS2IL Triage Scoreboard

> **Last Updated**: 2026-04-13
> **Planning Horizon**: Rolling 2 weeks
> **North Star**: Real-world unblock impact
> **Live Queue**: 17 open issues / 0 open PRs

This file is the working source of truth for implementation prioritization.

## Session Context Snapshots

- [IssueTriage.md](./IssueTriage.md): Current open-issue ordering snapshot synced to live GitHub state.
- [NodeGapPopularityBacklog.md](./NodeGapPopularityBacklog.md): Holistic missing-functionality analysis with the remaining Node backlog after the recent April merges.
- [ECMA262TopMissingBacklog.md](./ECMA262TopMissingBacklog.md): Current ECMA-262 issue-creation candidates derived from the generated section docs.

## Current Queue Highlights

- Recent landed work: [#969](https://github.com/tomacox74/js2il/pull/969)-[#973](https://github.com/tomacox74/js2il/pull/973) delivered the extractor networking fix, the test project split, pinned `test262` intake, the metadata parser, and the MVP runner.
- Primary open Node follow-ons are now [#949](https://github.com/tomacox74/js2il/issues/949) and [#956](https://github.com/tomacox74/js2il/issues/956).
- Primary open `test262` follow-ons are [#931](https://github.com/tomacox74/js2il/issues/931)-[#934](https://github.com/tomacox74/js2il/issues/934) under umbrella [#927](https://github.com/tomacox74/js2il/issues/927).
- The performance queue remains unchanged and explicitly secondary to compatibility and conformance.

## Capacity Split

- **70%** feature implementation
  - Primary: Node.js compatibility
  - Secondary: `test262`-driven conformance and ECMA-262 correctness
- **30%** issue throughput + documentation/status consistency

## Priority Lanes

### Lane A - Node Compatibility (Primary)

Goal: unblock real package and script scenarios with the highest ecosystem impact.

**Selection criteria**
- Unblocks widely used platform assumptions
- Reproducible via focused execution tests
- Clear runtime/hosting touchpoints

**Current ranked queue**
1. [#949](https://github.com/tomacox74/js2il/issues/949) - global `fetch` / `Headers` / `Request` / `Response` baseline
2. [#956](https://github.com/tomacox74/js2il/issues/956) - TLS/HTTPS trust, client-auth, and agent parity
3. No dedicated issue yet - whichever top remaining gap stays highest in [NodeGapPopularityBacklog.md](./NodeGapPopularityBacklog.md) after [#949](https://github.com/tomacox74/js2il/issues/949) and [#956](https://github.com/tomacox74/js2il/issues/956) are re-ranked or split

**Recently delivered**
- [#946](https://github.com/tomacox74/js2il/issues/946) - global URL support
- [#947](https://github.com/tomacox74/js2il/issues/947) - extractor network mode under JS2IL
- [#950](https://github.com/tomacox74/js2il/issues/950)-[#955](https://github.com/tomacox74/js2il/issues/955) - child_process / timers-promises / loader / fs / crypto / stream follow-ons

### Lane B - Conformance / `test262` / ECMA Semantics (Secondary)

Goal: improve correctness in small, reportable slices that can feed back into docs and backlog triage.

**Selection criteria**
- High leverage for conformance confidence
- Tight scope with strong testability
- Produces actionable results instead of a broad, noisy failure set

**Current ranked queue**
1. [#931](https://github.com/tomacox74/js2il/issues/931) - classify negative tests, exclusions, and baselines
2. [#932](https://github.com/tomacox74/js2il/issues/932) - add CI workflow and machine-readable reporting
3. [#933](https://github.com/tomacox74/js2il/issues/933) - connect conformance results to ECMA-262 docs and backlog
4. [#934](https://github.com/tomacox74/js2il/issues/934) - expand beyond the MVP to modules, async, and harness-heavy suites
5. [#927](https://github.com/tomacox74/js2il/issues/927) remains the umbrella tracker and should stay open until the child lane is actually complete

**Recently delivered**
- [#928](https://github.com/tomacox74/js2il/issues/928) - pinned intake/bootstrap
- [#929](https://github.com/tomacox74/js2il/issues/929) - metadata/frontmatter parser
- [#930](https://github.com/tomacox74/js2il/issues/930) - MVP runner for the narrow initial slice

**Current caveat**
- The current runner is an MVP foundation, not a broad official coverage claim. Keep automation/reporting/docs integration behind the remaining follow-ons rather than treating the landed slice as full-corpus conformance reporting.

### Lane C - Issue Throughput + Reliability Hygiene

Goal: keep the issue queue actionable while preventing status-document drift.

**Selection criteria**
- User-facing bug/regression severity
- Reproducibility and clear acceptance criteria
- Documentation drift risk

**Current queue**
- Keep `docs/tracking-issues` synchronized after each merge tranche
- Backfill `priority:*` labels and future `lane:*` labels on the remaining 17 open issues
- Keep [#949](https://github.com/tomacox74/js2il/issues/949), [#956](https://github.com/tomacox74/js2il/issues/956), and [#931](https://github.com/tomacox74/js2il/issues/931)-[#934](https://github.com/tomacox74/js2il/issues/934) scoped with crisp acceptance criteria
- Close or retag stale issues only when the replacement scope is clearly documented

## Mandatory PR Gate (Feature Work)

All feature PRs must include:

1. Execution tests (and generator tests when applicable)
2. Documentation updates in relevant JSON source docs:
   - `docs/ECMA262/**/Section*.json` for language features
   - `docs/nodejs/*.json` for Node modules/globals
3. Regenerated markdown/index artifacts after JSON updates
4. Tracking-doc updates when the queue or ranking materially changes
5. `CHANGELOG.md` entry when behavior changes are user-visible

If documentation is not synchronized, the PR is not ready to merge.

## Weekly Triage Cadence

At the start of each week:

1. Re-rank the top 3 items in each lane
2. Commit to a 2-week slice using the 70/30 split
3. Define acceptance criteria and owner for each committed item
4. Verify all open feature PRs include docs-sync updates

## Source-of-Truth Order

When status signals conflict, use this precedence:

1. Runtime behavior validated by tests
2. Merged state on `origin/master` plus `CHANGELOG.md`
3. Generated JSON-backed docs under `docs/ECMA262` and `docs/nodejs`
4. Live GitHub issue / PR state
5. Strategy / roadmap markdown summaries

Older phase roadmaps remain useful for context, but this scoreboard drives active prioritization.
