# ADR 0009: test262 Post-MVP Expansion Plan

- Date: 2026-04-20
- Status: Accepted

## Context

Issue #934 follows the intake/bootstrap work (#928), metadata parser (#929), MVP runner (#930), classified baseline artifact (#931), CI/reporting workflow (#932), and docs/backlog linkage (#933).

At this point JS2IL already has:

- a pinned upstream intake and local bootstrap model
- a bounded MVP runner with named suites and `summary.json`
- a docs/backlog feedback loop for the current bounded evidence

What it still lacks is a durable plan for **everything outside the MVP**. The runner and metadata parser already expose the real post-MVP blocker families:

- `module-flag`
- `resolution-negative`
- `async-requirement`
- `raw-flag`
- `agent-requirement`
- `can-block-requirement`
- path exclusions such as `test/intl402/**`, `test/annexB/**`, and `test/staging/**`

Keeping all of that behind one issue makes prioritization and contributor workflow too vague. It also leaves umbrella issue #927 carrying work that now belongs to narrower follow-ons.

## Decision

JS2IL replaces the single post-MVP bucket with a checked-in rollout plan plus dedicated follow-on issues for each expansion area.

The machine-readable plan lives in `tests/test262/post-mvp-expansion.json`.

### Ranked rollout areas

1. **Modules** - issue [#981](https://github.com/tomacox74/js2il/issues/981)
2. **Async and Promise-dependent tests** - issue [#982](https://github.com/tomacox74/js2il/issues/982)
3. **Raw and harness-heavy tests** - issue [#983](https://github.com/tomacox74/js2il/issues/983)
4. **Intl and environment-sensitive suites** - issue [#985](https://github.com/tomacox74/js2il/issues/985)
5. **Agent and `CanBlock`-dependent tests** - issue [#984](https://github.com/tomacox74/js2il/issues/984)

Each area records:

- the current blocker codes and/or path exclusions that already identify the work
- the rollout order
- the dedicated issue number that owns the first bounded implementation slice
- the support strategy that should guide future runner/harness expansion

### Umbrella handling

Issue #934 is satisfied once the expansion work is decomposed into these bounded areas with explicit strategies. After that, umbrella issue #927 no longer carries unique implementation work beyond the already-landed phases (#928-#933) plus the new follow-on issues (#981-#985), so it can close as well.

## Contributor workflow

1. Use `tests/test262/post-mvp-expansion.json` as the source of truth for post-MVP area ownership.
2. When a `summary.json` report hits one of the known blocker codes or path exclusions, attach the evidence to the owning follow-on issue instead of reopening a generic umbrella discussion.
3. Keep future runner or harness work bounded to one expansion area at a time unless a change is truly cross-cutting.
4. Update the relevant ECMA-262 docs/backlog surfaces when a bounded expansion area lands new durable support claims.
5. Only add a new expansion area when a genuinely new blocker family appears that is not already covered by the current plan.

## Consequences

### Positive

- Contributors now have concrete issue ownership for the known post-MVP blocker families.
- The phased `test262` program can close its original umbrella without losing future work.
- Future expansion PRs can stay small and reviewable instead of reviving one vague long-running issue.

### Negative

- The rollout plan is another checked-in manifest that must stay aligned with issue ownership.
- Some excluded suites remain policy decisions rather than near-term implementation promises.

### Mitigations

- Keep the manifest narrow and tied to blocker families that already exist in runner output.
- Update the plan only when a new follow-on issue or blocker family is actually introduced.
- Continue using ADR 0008's docs/backlog feedback loop so expansion work still feeds back into human-readable support status.
