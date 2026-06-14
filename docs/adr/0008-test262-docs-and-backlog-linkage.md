# ADR 0008: test262 Docs and Backlog Linkage

- Date: 2026-04-20
- Status: Accepted

## Context

Issue #933 follows the intake/bootstrap work (#928), metadata parser (#929), MVP runner (#930), baseline/result classification (#931), and CI/reporting workflow (#932).

At this point JROC already has:

- bounded named test262 MVP suites
- a machine-readable `summary.json` artifact for each runner invocation
- ECMA-262 section docs that act as the repository's human-readable support matrix

What is still missing is the connection between those two surfaces. Without it, a contributor can see that a test262 case passed or failed, but still has to manually infer:

- which ECMA-262 clause doc should be updated
- whether the result belongs to an existing backlog issue
- when a new issue should be opened instead of mutating docs

## Decision

JROC keeps the ECMA-262 docs as the human-readable source of truth and treats test262 output as linked evidence.

### Linkage manifest

The current bounded MVP slice is mapped in `tests/test262/mvp-linkage.json`.

Each linkage group records:

- the upstream test262 file paths
- the ECMA-262 clause(s) and section JSON doc(s) they inform
- the specific `support.entries` rows they provide evidence for
- backlog ownership hints (existing issue ids when present, otherwise the backlog doc used for triage)

### Summary annotation

The runner copies applicable linkage information into `summary.json`:

- matching result entries get `linkageGroupIds`
- the report gets a top-level `linkage` section with the referenced groups, guidance, and per-group verdict/kind counts

This makes CI artifacts directly usable for docs/backlog triage without forcing contributors to rediscover the clause mapping from file names alone.

### ECMA-262 doc evidence

Relevant ECMA-262 `support.entries` may now cite bounded test262 evidence alongside repo-local regression tests using:

- `test262Suites`
- `test262Paths`

These fields are evidence only; they do not replace the support status or notes maintained in the docs.

## Contributor workflow

1. Treat `summary.json` as evidence, not the source of truth for support status.
2. If linked results stay matched but the current support note or status is stale, update the relevant ECMA-262 section JSON and regenerate the markdown.
3. If linked results go unexpected and an open issue already tracks the same clause or feature, attach the artifact to that issue.
4. If linked results go unexpected and no issue exists, open a new issue and link the affected clause, support entry, and artifact.
5. Use the ECMA-262 backlog docs for ranking broader missing-feature families, not for every narrow regression.

## Consequences

### Positive

- test262 artifacts now point back to the docs contributors actually maintain
- contributors have a deterministic rule for docs vs existing issue vs new issue
- bounded CI output becomes useful for backlog refreshes and prioritization

### Negative

- the linkage manifest needs maintenance as the bounded suites evolve
- the mapping is intentionally incomplete until later issues broaden the suite scope

### Mitigations

- keep the linkage manifest small and reviewed alongside suite changes
- only link the bounded MVP files that are intentionally used as evidence today
- keep support status decisions in the ECMA-262 docs rather than letting raw test output become the primary narrative
