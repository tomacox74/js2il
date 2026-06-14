# JROC Triage Scoreboard

> **Last Updated**: 2026-04-20
> **Planning Horizon**: Rolling 2 weeks
> **North Star**: Real-world unblock impact
> **Live Queue**: 20 open issues / 1 open PR

This file is the working source of truth for implementation prioritization.

## Session Context Snapshots

- [IssueTriage.md](./IssueTriage.md): Current open-issue ordering snapshot synced to live GitHub state.
- [NodeGapPopularityBacklog.md](./NodeGapPopularityBacklog.md): Holistic missing-functionality analysis with the remaining Node backlog after the recent April merges.
- [ECMA262TopMissingBacklog.md](./ECMA262TopMissingBacklog.md): Current ECMA-262 issue-creation candidates derived from the generated section docs.

## Current Queue Highlights

- Recent landed work: [#969](https://github.com/tomacox74/jroc/pull/969)-[#973](https://github.com/tomacox74/jroc/pull/973) delivered the extractor networking fix, the test project split, pinned `test262` intake, and the MVP runner; [#975](https://github.com/tomacox74/jroc/pull/975), [#977](https://github.com/tomacox74/jroc/pull/977), and [#978](https://github.com/tomacox74/jroc/pull/978) then added classified reporting, bounded CI suites, and docs/backlog linkage.
- Primary open Node follow-ons are now [#949](https://github.com/tomacox74/jroc/issues/949) and [#956](https://github.com/tomacox74/jroc/issues/956).
- Primary open `test262` follow-ons are now the explicit post-MVP issues [#981](https://github.com/tomacox74/jroc/issues/981)-[#985](https://github.com/tomacox74/jroc/issues/985); [#927](https://github.com/tomacox74/jroc/issues/927) and [#934](https://github.com/tomacox74/jroc/issues/934) are now just closure items for the roadmap split.
- Docs/tooling hygiene is represented by [#979](https://github.com/tomacox74/jroc/issues/979).
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
1. [#949](https://github.com/tomacox74/jroc/issues/949) - global `fetch` / `Headers` / `Request` / `Response` baseline
2. [#956](https://github.com/tomacox74/jroc/issues/956) - TLS/HTTPS trust, client-auth, and agent parity
3. No dedicated issue yet - whichever top remaining gap stays highest in [NodeGapPopularityBacklog.md](./NodeGapPopularityBacklog.md) after [#949](https://github.com/tomacox74/jroc/issues/949) and [#956](https://github.com/tomacox74/jroc/issues/956) are re-ranked or split

**Recently delivered**
- [#946](https://github.com/tomacox74/jroc/issues/946) - global URL support
- [#947](https://github.com/tomacox74/jroc/issues/947) - extractor network mode under JROC
- [#950](https://github.com/tomacox74/jroc/issues/950)-[#955](https://github.com/tomacox74/jroc/issues/955) - child_process / timers-promises / loader / fs / crypto / stream follow-ons

### Lane B - Conformance / `test262` / ECMA Semantics (Secondary)

Goal: improve correctness in small, reportable slices that can feed back into docs and backlog triage.

**Selection criteria**
- High leverage for conformance confidence
- Tight scope with strong testability
- Produces actionable results instead of a broad, noisy failure set

**Current ranked queue**
1. [#981](https://github.com/tomacox74/jroc/issues/981) - module-mode conformance slice
2. [#982](https://github.com/tomacox74/jroc/issues/982) - async and Promise conformance slice
3. [#983](https://github.com/tomacox74/jroc/issues/983) - raw and harness-heavy conformance slice
4. [#985](https://github.com/tomacox74/jroc/issues/985) - Intl and environment-sensitive suite strategy
5. [#984](https://github.com/tomacox74/jroc/issues/984) - agent and CanBlock conformance slice
6. [#934](https://github.com/tomacox74/jroc/issues/934) - close after the roadmap split lands; it no longer owns unique implementation work
7. [#927](https://github.com/tomacox74/jroc/issues/927) - close after [#934](https://github.com/tomacox74/jroc/issues/934); the original phased program is now fully decomposed into concrete follow-ons

**Recently delivered**
- [#928](https://github.com/tomacox74/jroc/issues/928) - pinned intake/bootstrap
- [#929](https://github.com/tomacox74/jroc/issues/929) - metadata/frontmatter parser
- [#930](https://github.com/tomacox74/jroc/issues/930) - MVP runner for the narrow initial slice
- [#931](https://github.com/tomacox74/jroc/issues/931) - classified MVP results, exclusions, and baseline outputs
- [#932](https://github.com/tomacox74/jroc/issues/932) - CI workflow and machine-readable reporting
- [#933](https://github.com/tomacox74/jroc/issues/933) - docs/backlog linkage for bounded `test262` evidence

**Current caveat**
- The current runner now has stable MVP verdict classification, bounded CI suites, and docs linkage, but it is still not a broad official coverage claim. Keep future expansion work bounded to one of [#981](https://github.com/tomacox74/jroc/issues/981)-[#985](https://github.com/tomacox74/jroc/issues/985) rather than treating the landed slice as full-corpus conformance reporting.

### Lane C - Issue Throughput + Reliability Hygiene

Goal: keep the issue queue actionable while preventing status-document drift.

**Selection criteria**
- User-facing bug/regression severity
- Reproducibility and clear acceptance criteria
- Documentation drift risk

**Current queue**
- [#979](https://github.com/tomacox74/jroc/issues/979) - changelog archive layout and browse-only index
- Keep `docs/tracking-issues` synchronized after each merge tranche
- Backfill `priority:*` labels and future `lane:*` labels on the remaining 20 open issues
- Keep [#949](https://github.com/tomacox74/jroc/issues/949), [#956](https://github.com/tomacox74/jroc/issues/956), [#979](https://github.com/tomacox74/jroc/issues/979), and [#981](https://github.com/tomacox74/jroc/issues/981)-[#985](https://github.com/tomacox74/jroc/issues/985) scoped with crisp acceptance criteria
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
