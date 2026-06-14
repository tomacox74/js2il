# Tracking Issues and Triage

This directory contains the active and historical tracking artifacts used to prioritize Node compatibility, `test262` adoption, and remaining ECMA-262 backlog work.

## Current Source of Truth (Active)

- **Primary planning doc**: [TriageScoreboard.md](./TriageScoreboard.md)
- **Live issue-order snapshot**: [IssueTriage.md](./IssueTriage.md)
- **Node backlog snapshot**: [NodeGapPopularityBacklog.md](./NodeGapPopularityBacklog.md)
- **ECMA issue-creation candidates**: [ECMA262TopMissingBacklog.md](./ECMA262TopMissingBacklog.md)

These files should stay aligned with:

1. `origin/master`
2. `gh issue list` / `gh pr list`
3. Generated support docs under `docs/nodejs` and `docs/ECMA262`

## Current Repo Snapshot (2026-04-20)

- `origin/master` @ `145d7a72`
- Open issues: **20**
- Open PRs: **1**
- Recent merged PRs that materially changed the queue: [#969](https://github.com/tomacox74/jroc/pull/969), [#970](https://github.com/tomacox74/jroc/pull/970), [#971](https://github.com/tomacox74/jroc/pull/971), [#972](https://github.com/tomacox74/jroc/pull/972), [#973](https://github.com/tomacox74/jroc/pull/973), [#975](https://github.com/tomacox74/jroc/pull/975), [#977](https://github.com/tomacox74/jroc/pull/977), [#978](https://github.com/tomacox74/jroc/pull/978)
- Remaining open lanes:
  - Node/runtime follow-ons: [#949](https://github.com/tomacox74/jroc/issues/949), [#956](https://github.com/tomacox74/jroc/issues/956)
  - `test262` program: the MVP foundation [#928](https://github.com/tomacox74/jroc/issues/928)-[#933](https://github.com/tomacox74/jroc/issues/933) is closed, and the concrete post-MVP lane is now [#981](https://github.com/tomacox74/jroc/issues/981)-[#985](https://github.com/tomacox74/jroc/issues/985); [#927](https://github.com/tomacox74/jroc/issues/927) and [#934](https://github.com/tomacox74/jroc/issues/934) remain open only until this roadmap-split work lands
  - Docs/tooling hygiene: [#979](https://github.com/tomacox74/jroc/issues/979)
  - Performance queue: [#451](https://github.com/tomacox74/jroc/issues/451), [#737](https://github.com/tomacox74/jroc/issues/737), [#738](https://github.com/tomacox74/jroc/issues/738), [#742](https://github.com/tomacox74/jroc/issues/742), [#743](https://github.com/tomacox74/jroc/issues/743), [#746](https://github.com/tomacox74/jroc/issues/746), [#747](https://github.com/tomacox74/jroc/issues/747), [#748](https://github.com/tomacox74/jroc/issues/748), [#768](https://github.com/tomacox74/jroc/issues/768), [#837](https://github.com/tomacox74/jroc/issues/837)

## Historical Context

- [Phase1-TrackingIssues.md](./Phase1-TrackingIssues.md) is a completed historical rest/spread planning document. Keep it for reference only; do not use it as the template for new GitHub issues.

## Updating These Docs

1. Refresh GitHub state with `gh issue list --state all` and `gh pr list --state all`.
2. Sync rankings against shipped behavior in `docs/nodejs/Index.md` and `docs/ECMA262/**/Section*.md`.
3. Update [TriageScoreboard.md](./TriageScoreboard.md) first, then bring [IssueTriage.md](./IssueTriage.md), [NodeGapPopularityBacklog.md](./NodeGapPopularityBacklog.md), and [ECMA262TopMissingBacklog.md](./ECMA262TopMissingBacklog.md) into alignment.
4. Keep historical docs explicitly marked historical when the active queue moves on.

## Mandatory Docs-Sync Gate

Feature work is not ready to merge until:

1. Relevant source JSON docs under `docs/ECMA262` and/or `docs/nodejs` are updated
2. Generated markdown/index artifacts are regenerated
3. Tracking docs reflect the new issue/priority state when the queue materially changes
4. `CHANGELOG.md` is updated for user-visible behavior

## Related Documentation

- **[TriageScoreboard.md](./TriageScoreboard.md)**: Active prioritization and lane ordering
- **[IssueTriage.md](./IssueTriage.md)**: Current open-issue ordering snapshot
- **[NodeGapPopularityBacklog.md](./NodeGapPopularityBacklog.md)**: Remaining Node backlog after the recent April delivery tranche
- **[ECMA262TopMissingBacklog.md](./ECMA262TopMissingBacklog.md)**: Current ECMA-262 issue-creation candidates derived from the generated section docs
- **[FeatureImplementationRoadmap.md](../archive/FeatureImplementationRoadmap.md)**: Historical technical roadmap
- **[ECMA262FeaturePriority.md](../archive/ECMA262FeaturePriority.md)**: Historical executive summary
- **[ECMA262FeatureStatus.md](../archive/ECMA262FeatureStatus.md)**: Historical status snapshot

---

*This directory supports ongoing Node compatibility, `test262`, and ECMA-262 triage.*
*Last updated: 2026-04-20*
